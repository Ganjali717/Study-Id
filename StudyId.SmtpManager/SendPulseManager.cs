using System.Collections;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using StudyId.SmtpManager.Models.SendPulse;
namespace StudyId.SmtpManager
{
    public class SendPulseManager : ISmtpManager
    {
        private readonly string? _apiServer;
        private readonly string _authUrl = "/oauth/access_token";
        private readonly string _mailsUrl = "/smtp/emails";
        private readonly string _sendPulseTokenKey = "sendpulse.session.token";
        private readonly string _sendPulseGrandType = "client_credentials";
        private readonly string _sendPulseClientId;
        private readonly string _sendPulseClientSecret;
        private readonly string _sendPulseSender;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _http = new HttpClient();
        public SendPulseManager(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
            _apiServer = _configuration["SendPulse:ApiServer"] ?? string.Empty;
            _sendPulseClientId = _configuration["SendPulse:ClientId"] ?? string.Empty;
            _sendPulseClientSecret = _configuration["SendPulse:ClientSecret"] ?? string.Empty;
            _sendPulseSender = _configuration["SendPulse:Sender"] ?? string.Empty;
            if (string.IsNullOrEmpty(_apiServer)) throw new Exception("SendPulse:ApiServer is not present in the configuration file.");
            if (string.IsNullOrEmpty(_sendPulseClientId)) throw new Exception("SendPulse:ClientId is not present in the configuration file.");
            if (string.IsNullOrEmpty(_sendPulseClientSecret)) throw new Exception("SendPulse:ApiServer is not present in the configuration file.");
            if (string.IsNullOrEmpty(_sendPulseSender)) throw new Exception("SendPulse:Sender is not present in the configuration file.");
        }
        /// <summary>
        /// Gets existing token from memory cache. If token expired - generates new token.
        /// </summary>
        /// <returns>Bearer token</returns>
        private string GetToken()
        {
            var tokenResponse = _cache.Get<SendPulseSmtpTokenDto>(_sendPulseTokenKey);
            if (tokenResponse==null)
            {
                return CreateJwtToken();
            }
            var token = _cache.Get(_sendPulseTokenKey) as SendPulseSmtpTokenDto;
            return token?.Token ?? string.Empty;
        }
        /// <summary>
        /// Generates new token by sending POST request on SendPule API
        /// </summary>
        /// <returns>Bearer Token</returns>
        private string CreateJwtToken()
        {
            var res = new SendPulseAuthTokenDto
            {
                ClientId = _sendPulseClientId,
                ClientSecret = _sendPulseClientSecret,
                GrantType = _sendPulseGrandType
            };
            try
            {
                var response = _http.PostAsync($"{_apiServer}{_authUrl}", new StringContent(JsonConvert.SerializeObject(res), Encoding.UTF8, "application/json"));
                var result = response.Result.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonConvert.DeserializeObject<SendPulseSmtpTokenDto>(result);
                _cache.Set(_sendPulseTokenKey, tokenResponse, DateTimeOffset.Now.AddHours(1));
                return tokenResponse?.Token ?? string.Empty;
            }
            catch (Exception e)
            {
                Log.Error(e.GetBaseException().Message);
                return string.Empty;
            }
        }
        private string EncodeToBase64(string toEncode)
        {
            var toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }
        private string EncodeToBase64(Attachment toEncode)
        {
            return Convert.ToBase64String(ReadFullSteam(toEncode.ContentStream));
        }
        private byte[] ReadFullSteam(Stream input)
        {
            input.Position = 0; //jump to the start position
            var buffer = new byte[16 * 1024];
            using var ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
        public SmtpResult Send(string to, string subject, string body, bool html = true)
        {
            return Send(to, subject, body, Array.Empty<Attachment>(), html);
        }
        public SmtpResult Send(string to, string subject, string body, Attachment attachment, bool html = true)
        {
            return Send(to, subject, body, new[] {attachment}, html);
        }
        public SmtpResult Send(string to, string subject, string body, Attachment[] attachments, bool html = true)
        {
            var result = new SmtpResult();
            var message = new SendPulseMailDto { Email = new SendPulseMailBody
                {
                    From = new SendPulseMailAddress() { Name = _sendPulseSender.Split('|').First(), Email = _sendPulseSender.Split('|').Last() },
                    Subject = subject,
                    To = new List<SendPulseMailAddress>() { new() { Email = to } },
                    Html = EncodeToBase64(body)
                }
            };
            foreach (var attachment in attachments)
            {
                message.Email.AttachmentsBinary.Add(attachment.Name ?? string.Empty, EncodeToBase64(attachment));
            }
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
            try
            {
                _http.PostAsync($"{_apiServer}{_mailsUrl}", new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json")).Wait();
                result.Success = true;
            }
            catch (Exception e)
            {
                Log.Error(e.GetBaseException().Message);
                result.Message = e.GetBaseException().Message;
            }
            return result;
        }

        public SmtpResult<string> GenerateHtmlBody(string templateName, Hashtable dictionary)
        {
            var result = new SmtpResult<string>();
            try
            {
                var currentTemplate = ReadTemplate(templateName);
                result.Data = dictionary.Keys.Cast<string>().Aggregate(currentTemplate ?? string.Empty, (current, key) => current.Replace("{{"+key+"}}", dictionary[key].ToString()));
                result.Success = true;
            }
            catch (Exception e)
            {
                Log.Error(e.GetBaseException().Message);
                result.Message =e.GetBaseException().Message;
            }
            return result;
        }

        private string ReadTemplate(string name)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "StudyId.SmtpManager.Emails." + name;
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }
    }
}
