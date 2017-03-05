using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CvarcWeb.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public void SendEmail(string email, string subject, string text)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("CvarcTeam", "cvarc-team@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new BodyBuilder {HtmlBody = text}.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("cvarc.team@gmail.com", "somePassword");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
