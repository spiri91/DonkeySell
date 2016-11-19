using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DonkeySellApi.Extra
{
    public interface IMailSender
    {
        Task SendNewPasswordMail(string newPassword, string email);

        Task SendEmailConfirmationMessage(string email, string guid, string username);
    }

    public class MailSender : IMailSender
    {
        public async Task SendNewPasswordMail(string newPassword, string email)
        {
            string body = "Hi, your new password is: " + newPassword;
            string subject = "New password";
            SendMail(body, email, subject);
        }

        public async Task SendEmailConfirmationMessage(string email, string guid, string username)
        {
            string body = ComposeMailBody(guid, username);
            string subject = "Mail Confirmation";
            SendMail(body, email, subject);
        }

        private string ComposeMailBody(string guid, string username)
        {
            StringBuilder str = new StringBuilder();
            str.Append("Hi " + username + "\r\n");
            str.Append("Please click on the address below to confirm your email! \r\n");
            var mailConfirmationAddress = ConfigurationManager.AppSettings.Get("apiAddress") + "/" + username +
                                          "/" + guid;
            str.Append(mailConfirmationAddress);
            str.Append("\r\n");
            str.Append("Regards");

            return str.ToString();
        }

        private void SendMail(string body, string email, string subject)
        {
            string id = ConfigurationManager.AppSettings.Get("mail");
            string password = ConfigurationManager.AppSettings.Get("password");
            SmtpClient client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new System.Net.NetworkCredential(id, password),
            };

            MailMessage mail = new MailMessage(id, email, subject, body);
            client.Send(mail);
        }
    }
}