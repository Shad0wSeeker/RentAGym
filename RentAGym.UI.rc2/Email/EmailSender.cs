using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace RentAGym.UI.rc2.Email
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            htmlMessage = htmlMessage.Replace("&amp;", "&amp&");
            MailMessage m = new MailMessage("rentagym.service@yandex.ru",email,subject, htmlMessage);
            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("rentagym.service@yandex.ru", "ueuoyffukyecquof");
            smtp.Host = "smtp.yandex.ru";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            return smtp.SendMailAsync(m);
        }
    }
}
