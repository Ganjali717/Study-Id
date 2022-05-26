using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StudyId.SmtpManager;
using StudyId.WebApplication.Tests.Base;
namespace StudyId.WebApplication.Tests
{
    [TestClass]
    public class SmtpManagerTest
    {
        private readonly StudyIdTestServer<Program> _server = new StudyIdTestServer<Program>();
        [TestMethod]
        public void GenerateHtmlBody_ShouldToGenerateTemplateWithValidLink()
        {
            var scope = _server.Services.CreateScope();
            var smtpManager = scope.ServiceProvider.GetService<ISmtpManager>();
            Assert.IsNotNull(smtpManager);
            var link = "https://study-id.com/invite/" + Guid.NewGuid();
            var dictionary = new Hashtable();
            dictionary.Add("InviteLink", link);
            var bodyResult = smtpManager.GenerateHtmlBody("Accounts.AccountsInvite.html", dictionary);
            Assert.IsTrue(bodyResult.Success);
            Assert.IsTrue(!string.IsNullOrEmpty(bodyResult.Data));
            Assert.IsTrue(bodyResult.Data.Contains(link));
        }
        [TestMethod]
        public void Send_ShouldToRecieveSuccessConfirmationThatMessageWasSended()
        {
            var scope = _server.Services.CreateScope();
            var smtpManager = scope.ServiceProvider.GetService<ISmtpManager>();
            var smtpResult = smtpManager.Send("noreply@study-id.com", "study-id-unit-test-message", "<p>Study Id Unit Test Message<p>");
            Assert.IsTrue(smtpResult.Success);
        }
        [TestMethod]
        public void SendAttachment_ShouldToRecieveSuccessConfirmationThatMessageWasSended()
        {
            var scope = _server.Services.CreateScope();
            var smtpManager = scope.ServiceProvider.GetService<ISmtpManager>();
            var temp = Path.GetTempFileName();
            var bytes = File.ReadAllBytes(temp);
            File.Delete(temp);
            var memorySteam = new MemoryStream(bytes);
            var attachment = new Attachment(memorySteam, "tempfile.txt");
            var smtpResult = smtpManager.Send("noreply@study-id.com", "study-id-unit-test-message", "<p>Study Id Unit Test Message<p>", attachment);
            Assert.IsTrue(smtpResult.Success);
        }
        [TestMethod]
        public void SendAttachments_ShouldToRecieveSuccessConfirmationThatMessageWasSended()
        {
            var scope = _server.Services.CreateScope();
            var smtpManager = scope.ServiceProvider.GetService<ISmtpManager>();
            var temp = Path.GetTempFileName();
            var bytes = File.ReadAllBytes(temp);
            File.Delete(temp);
            var memorySteam = new MemoryStream(bytes);
            var memorySteam2 = new MemoryStream(bytes);
            var attachment = new Attachment(memorySteam, "tempfile.txt");
            var attachment2 = new Attachment(memorySteam2, "tempfile2.txt");
            var list = new List<Attachment>() { attachment, attachment2 }.ToArray();
            var smtpResult = smtpManager.Send("noreply@study-id.com", "study-id-unit-test-message", "<p>Study Id Unit Test Message<p>", list);
            Assert.IsTrue(smtpResult.Success);
        }
    }
}