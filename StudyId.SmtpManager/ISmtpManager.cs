using System.Collections;
using System.Net.Mail;
namespace StudyId.SmtpManager
{
    /// <summary>
    /// Manager for sending the emails over smtp protocol or any web api
    /// </summary>
    public interface ISmtpManager
    {
        /// <summary>
        /// Send standard message to the receiver 
        /// </summary>
        /// <param name="to">Email address of receiver </param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="html">Optional:Flag is body formatted in html</param>
        /// <returns></returns>
        public SmtpResult Send(string to, string subject, string body, bool html = true);
        /// <summary>
        /// Send standard message to the receiver with attachment
        /// </summary>
        /// <param name="to">Email address of receiver </param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="attachment">Email attachment</param>
        /// <param name="html">Optional:Flag is body formatted in html</param>
        /// <returns></returns>
        public SmtpResult Send(string to, string subject, string body, Attachment attachment, bool html = true);
        /// <summary>
        /// Send standard message to the receiver with attachments
        /// </summary>
        /// <param name="to">Email address of receiver </param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="attachments">Email attachments</param>
        /// <param name="html">Optional:Flag is body formatted in html</param>
        /// <returns></returns>
        public SmtpResult Send(string to, string subject, string body, Attachment[] attachments, bool html = true);
        /// <summary>
        /// Generate email body from html template (should be stored in the Emails folder as embedded resource)
        /// </summary>
        /// <param name="templateName">Name of the template with namespace (Accounts.AccountsInvite.html)</param>
        /// <param name="dictionary">Dictionary of keys in the template for fill values</param>
        /// <returns>SmtpResult with Success flag and compiled email in the Data field</returns>
        public SmtpResult<string> GenerateHtmlBody(string templateName, Hashtable dictionary);
    }
}
