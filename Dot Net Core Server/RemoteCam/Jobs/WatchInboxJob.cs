using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using log4net;
using log4net.Core;
using Microsoft.Extensions.Logging;
using Quartz;
using RemoteCam.Pi;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteCam.Jobs
{
    public class WatchInboxJob : IJob
    {
        static string[] Scopes = { GmailService.Scope.MailGoogleCom };
        static string ApplicationName = "RemoteCam";

        ILogger<WatchInboxJob> log;

        public WatchInboxJob(ILogger<WatchInboxJob> log)
        {
            this.log = log;
        }
        public Task Execute(IJobExecutionContext context)
        {
            log.LogInformation("Verify emails");
            CheckMails();
            return Task.FromResult(true);
        }

        private void CheckMails()
        {
            try
            {
                log.LogInformation("Loading credentials");
                UserCredential credential;
                using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
                log.LogInformation("Service creation");
                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
                log.LogInformation("List messages");
                var listRequest = service.Users.Messages.List("me");
                listRequest.IncludeSpamTrash = false;
                listRequest.LabelIds = "INBOX";
                var listResponse = listRequest.Execute();
                if (listResponse != null && listResponse.Messages != null && listResponse.Messages.Any())
                {
                    var message = listResponse.Messages.First();
                    Message request = service.Users.Messages.Get("me", message.Id).Execute();
                    string fromMailAddress = request
                        .Payload
                        .Headers
                        .Where(h => h.Name == "From")
                        .Select(h => h.Value)
                        .FirstOrDefault();
                    string toMailAddress = request
                        .Payload
                        .Headers
                        .Where(h => h.Name == "To")
                        .Select(h => h.Value)
                        .FirstOrDefault();

                    MailMessage replyMessage = new MailMessage();
                    replyMessage.Subject = $"Remote Cam - {DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")}";
                    replyMessage.From = new MailAddress(toMailAddress);
                    replyMessage.To.Add(new MailAddress(fromMailAddress));
                    replyMessage.Attachments.Add(new Attachment(Camera.GetLatestCaptureFile()));
                    MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(replyMessage);
                    Message replay = new Message();
                    replay.Raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(mimeMessage.ToString()))
                        .Replace('+', '-')
                        .Replace('/', '_')
                        .Replace("=", "");
                    service.Users.Messages.Send(replay, "me").Execute();

                    foreach (var m in listResponse.Messages)
                        service.Users.Messages.Delete("me", m.Id).Execute();
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.StackTrace);
            }
        }
    }
}
