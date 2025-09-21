using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.External;

namespace api.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _cfg;
        public SmtpEmailSender(IConfiguration cfg) => _cfg = cfg;

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            using var client = new System.Net.Mail.SmtpClient(
                _cfg["Smtp:Host"], int.Parse(_cfg["Smtp:Port"]!));
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(
                _cfg["Smtp:User"], _cfg["Smtp:Pass"]);

            var msg = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(_cfg["Smtp:From"]!),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            msg.To.Add(toEmail);
            await client.SendMailAsync(msg);
        }
    }
}