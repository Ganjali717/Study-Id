using StudyId.Entities;
using StudyId.Entities.Security;

namespace StudyId.Data.Managers.Interfaces
{
    /// <summary>
    /// Provide an operations for working with the accounts from admin side
    /// </summary>
    public interface IAccountsManager
    {
        /// <summary>
        /// Search accounts entities by query and additional properties
        /// </summary>
        /// <param name="q">Search string</param>
        /// <param name="role">Role filter</param>
        /// <param name="status">Status filter</param>
        /// <param name="orderBy">Order column</param>
        /// <param name="orderAsc">Order ascending</param>
        /// <param name="page">Page number</param>
        /// <param name="take">Count of records to take</param>
        /// <returns>PagedManagerResult with the accounts entities in the data + total rows count</returns>
        PagedManagerResult<IList<Account>> GetAccounts(string? q, Role? role, Status? status, string? orderBy, bool? orderAsc, int page = 1, int take = 25);
        /// <summary>
        /// Search account by the account id
        /// </summary>
        /// <param name="id">Account Id</param>
        /// <returns>ManagerResult with the Account entity in the Data field</returns>
        ManagerResult<Account> GetAccount(Guid id);

        /// <summary>
        /// Search account by the account id
        /// </summary>
        /// <param name="id">Account Id</param>
        /// <returns>ManagerResult with the Account entity in the Data field</returns>
        List<Entities.Security.Account> GetAllAccounts();


        /// <summary>
        /// Search account by security token
        /// </summary>
        /// <param name="token">Security token</param>
        /// <returns>ManagerResult with the Account entity in the Data field</returns>
        ManagerResult<Account> GetAccountBySecurityToken(string token);
        /// <summary>
        /// Remove account from database
        /// </summary>
        /// <param name="id">Account Id</param>
        /// <returns>ManagerResult with Success flag if all success</returns>
        ManagerResult RemoveAccount(Guid id);
        /// <summary>
        /// Check the login credentials and return account entity in case if data is valid
        /// </summary>
        /// <param name="email">Username (Email)W</param>
        /// <param name="password">Password</param>
        /// <returns>Manager result with Account Entity in the data</returns>
        ManagerResult<Account> Login(string email, string password);

        /// <summary>
        /// Send the account invitations to the emails
        /// </summary>
        /// <param name="accounts">List of the invited accounts</param>
        /// <param name="inviteBase">Base url with {0} pattern on the end to put token</param>
        /// <returns>Manager result with Success flag if all ok</returns>
        ManagerResult Invite(IEnumerable<Account> accounts, string inviteBase);
        /// <summary>
        /// Reset pwd for account by using the secret token and new password
        /// </summary>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public ManagerResult<Account> ResetPassword(string token, string newPassword);

        /// <summary>
        /// Send the request for generating secret link to the account pwd reset
        /// </summary>
        /// <param name="email">Account email</param>
        /// <param name="baseUrlLink">Base recovery link with the {0} inside where will be placed token</param>
        /// <returns></returns>
        public ManagerResult ResetPasswordRequest(string email, string baseUrlLink);
        /// <summary>
        /// Proceed with the invite registration 
        /// </summary>
        /// <param name="token">Security token</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="password">Password</param>
        /// <returns>ManagerResult with success flag if ok</returns>
        public ManagerResult<Account> FinishRegistration(string token, string firstName, string lastName, string password);
        /// <summary>
        /// Change the status of the application
        /// </summary>
        /// <param name="id">Account id</param>
        /// <param name="status">Status value</param>
        /// <returns>ManageResult with Success flag</returns>
        ManagerResult ChangeStatus(Guid id, Status status);
        /// <summary>
        /// Validate if the account email exist in the system
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        ManagerResult<bool> IsAccountExist(string email);
    }
}
