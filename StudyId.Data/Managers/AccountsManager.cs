using System.Collections;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyId.Data.DatabaseContext;
using StudyId.Data.Extentions;
using StudyId.Data.Managers.Interfaces;
using StudyId.Entities.Security;
using StudyId.Entities;
using StudyId.Entities.Extentions;
using StudyId.SmtpManager;

namespace StudyId.Data.Managers
{
    public class AccountsManager : IAccountsManager
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<AccountsManager> _logger;
        private readonly ISmtpManager _smtpManager;
        public AccountsManager(IServiceProvider services, ILogger<AccountsManager> logger, ISmtpManager smtpManager)
        {
            _services = services;
            _logger = logger;
            _smtpManager = smtpManager;
        }
        public PagedManagerResult<IList<Account>> GetAccounts(string? q, Role? role, Status? status, string? orderBy, bool? orderAsc, int page = 1, int take = 25)
        {
            var result = new PagedManagerResult<IList<Account>>() { Page = page, Take = take };
            var orderList = new List<EntitySorting>();
            if (string.IsNullOrEmpty(orderBy) || !orderAsc.HasValue || !typeof(Account).GetProperties().Any(x => orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries).Any(o => string.Equals(o, x.Name, StringComparison.CurrentCultureIgnoreCase))))
            {
                orderList.Add(new EntitySorting() { Column = "Created", SortAsc = false });
                result.OrderAsc = false;
                result.OrderBy = "Created";
            }
            else
            {
                if (orderBy.Contains(","))
                {
                    orderList.AddRange(orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(key => new EntitySorting() { Column = key, SortAsc = orderAsc.Value }));
                }
                else
                {
                    orderList.Add(new EntitySorting() { Column = orderBy, SortAsc = orderAsc.Value });
                }
                result.OrderAsc = orderAsc.Value;
                result.OrderBy = orderBy;
            }
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var items = dbContext.Accounts.OrderBy(orderList);
                if (!string.IsNullOrEmpty(q))
                {
                    var normalizedQ = q.Replace("\t", "").ToLower();
                    items = items.Where(x => (x.Email != null && x.Email.ToLower().Contains(normalizedQ)) || (x.FirstName + " " + x.LastName).ToLower().Contains(normalizedQ));
                }
                if (role.HasValue)
                {
                    items = items.Where(x => x.Role == role.Value);
                }
                if (status.HasValue)
                {
                    items = items.Where(x => x.Status == status.Value);
                }
                var skip = (page - 1) * take;
                result.Total = items.Count();
                if (result.Total == 1)
                {
                    result.Data = items.ToList();
                }
                else
                {
                    result.Data = items.Skip(skip).Take(take).ToList();
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public List<Account> GetAllAccounts()
        {
            var result = new List<Account>();
            using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
            var items = dbContext.Accounts;

            result = items.ToList();
            return result;
        }

        public ManagerResult<Account> GetAccount(Guid id)
        {
            var result = new ManagerResult<Account>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbAccount = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                
                if (dbAccount == null)
                {
                    result.Message = $"Account with id:{id} was not found in the database.";
                    return result;
                }
                result.Data = dbAccount;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }
        public ManagerResult<Account> GetAccountBySecurityToken(string token)
        {
            var result = new ManagerResult<Account>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbAccount = dbContext.Accounts.FirstOrDefault(x => x.SecurityToken == token);
                
                if (dbAccount == null)
                {
                    result.Message = $"The token was expired.";
                    return result;
                }
                result.Data = dbAccount;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }
        public ManagerResult RemoveAccount(Guid id)
        {
            var result = new ManagerResult();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbAccount = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (dbAccount == null)
                {
                    result.Message = $"Account with id:{id} was not found in the database.";
                    return result;
                }
                if (dbAccount.Email == "admin@study-id.com")
                {
                    result.Message = $"You don't have access to delete this account.";
                    return result;
                }
                dbContext.Remove(dbAccount);
                dbContext.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }
        public ManagerResult<Account> Login(string email, string password)
        {
            ManagerResult<Account> result = new ManagerResult<Account>();
            try
            {
                var hash = GetPasswordHash(email, password);
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var account = dbContext.Accounts.FirstOrDefault(r => r.Email == email && r.Password == hash);
                if (account == null)
                {
                    result.Success = false;
                    result.Message = "User Name or Password isn't correct.";
                    return result;
                }
                result.Success = true;
                result.Data = account;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }
        public ManagerResult Invite(IEnumerable<Account> accounts, string inviteBase)
        {
            var result = new ManagerResult();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                foreach (var account in accounts)
                {
                    account.Status = Status.Invited;
                    account.Created = DateTime.Now;
                    account.SecurityToken = GenerateTimeStampToken();
                    account.SecurityTokenExpired = DateTimeOffset.UtcNow.AddDays(1);
                    dbContext.Accounts.Add(account);
                    dbContext.SaveChanges();
                    var link = string.Format(inviteBase, account.SecurityToken);
                    var keys = new Hashtable {{"InviteLink", link}};
                    var mailTemplate = _smtpManager.GenerateHtmlBody("Accounts.AccountsInvite.html", keys);
                    _smtpManager.Send(account.Email, "StudyID: Your Account has been Created", mailTemplate.Data);
                }
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }
        public ManagerResult<Account> ResetPassword(string token, string newPassword)
        {
            var result = new ManagerResult<Account>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var account = dbContext.Accounts.FirstOrDefault(t => t.SecurityToken == token && t.SecurityTokenExpired>DateTime.UtcNow);
                if (account == null)
                {
                    result.Message = "The token was expired";
                    return result;
                }

                account.Password = GetPasswordHash(account.Email, newPassword);
                account.Updated = DateTime.UtcNow;
                account.SecurityToken = string.Empty;
                account.SecurityTokenExpired = DateTimeOffset.UtcNow;
                dbContext.SaveChanges();
                result.Data = account;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }
        public ManagerResult ResetPasswordRequest(string email, string baseUrlLink)
        {
            var result = new ManagerResult();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbAccount = dbContext.Accounts.FirstOrDefault(l => l.Email == email);
                if (dbAccount == null)
                {
                    result.Message = "User was not found.";
                    return result;
                }

                dbAccount.SecurityToken = GenerateTimeStampToken();
                dbAccount.SecurityTokenExpired = DateTimeOffset.UtcNow.AddDays(1);
                dbContext.SaveChanges();

                var link = string.Format(baseUrlLink, dbAccount.SecurityToken);
                var keys = new Hashtable {{"ResetPasswordLink", link}};
                var mailTemplate = _smtpManager.GenerateHtmlBody("Accounts.ForgotPassword.html", keys);
                _smtpManager.Send(dbAccount.Email, "StudyID: Your Password has been reset", mailTemplate.Data);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }
        private string? GetPasswordHash(string? email, string? password)
        {
            var salt = email.GetSHA256Hash();
            return (password + salt).GetSHA256Hash();
        }
        public ManagerResult<Account> FinishRegistration(string token, string firstName, string lastName, string password)
        {
            var result = new ManagerResult<Account>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbAccount = dbContext.Accounts.FirstOrDefault(t => t.SecurityToken == token && t.SecurityTokenExpired>DateTime.UtcNow);
                if (dbAccount == null)
                {
                    result.Message = $"The token was expired.";
                    return result;
                }
                dbAccount.FirstName = firstName;
                dbAccount.LastName = lastName;
                dbAccount.Status = Status.Active;
                dbAccount.Password = GetPasswordHash(dbAccount.Email, password);
                dbAccount.Updated = DateTime.UtcNow;
                dbContext.SaveChanges();
                result.Data = dbAccount;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
               
            }
            return result;
        }
        public ManagerResult ChangeStatus(Guid id, Status status)
        {
            var result = new ManagerResult();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                var dbApplication = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (dbApplication == null)
                {
                    result.Message = $"Account with id:{id} was not found in the db";
                    return result;
                }

                dbApplication.Status = status;
                dbApplication.Updated = DateTime.UtcNow;
                dbContext.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        public ManagerResult<bool> IsAccountExist(string email)
        {
            var result = new ManagerResult<bool>();
            try
            {
                using var dbContext = _services.GetRequiredService<StudyIdDbContext>();
                result.Data =  dbContext.Accounts.Any(x => x.Email == email);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                result.Message = ex.GetBaseException().Message;
            }
            return result;
        }

        private string GenerateTimeStampToken()
        {
            var dayTime = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            var key = Guid.NewGuid().ToByteArray();
            var hash = dayTime.Concat(key);
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }
}
