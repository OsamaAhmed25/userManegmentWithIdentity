using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace UserManegmentWithIdentity.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromMail = "Osama.netcore2022@outlook.com";
            var fromPassword = "Memo5055memo";
            var message = new MailMessage();
            message.From=new MailAddress(fromMail);
            message.Subject=subject;    
            message.To.Add(email);
            message.Body = $"<html><body>{htmlMessage} </body></html>";
            message.IsBodyHtml = true;
            var smtpClient = new SmtpClient("smtp-mail.outlook.com")
            {
              
                Port = 587,
                UseDefaultCredentials = false,
               
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(message);

        }
    }
}
