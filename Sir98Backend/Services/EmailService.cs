using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace Sir98Backend.Services
{
    public class EmailService
    {
        private readonly string Email;
        private readonly string Password;
        private readonly string Host;
        private readonly int Port;

        public EmailService(IConfiguration configuration)
        {
            Email = configuration.GetValue<string>("EmailServer:Email") ?? throw new Exception();
            Password = configuration.GetValue<string>("EmailServer:Password") ?? throw new Exception();
            Host = configuration.GetValue<string>("EmailServer:Host") ?? throw new Exception();
            Port = configuration.GetValue<int>("EmailServer:Port");

            Console.WriteLine("Appsettings " + Email + " " + Password + " " + Host + " " + Port);
        }

        public void Send(MailMessage mail)
        {
            if (mail is null)
            {
                throw new ArgumentNullException($"{nameof(mail)} can not be null");
            }

            SmtpClient mySmtpClient = new(Host, Port);

            mySmtpClient.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicAuthenticationInfo = new(Email, Password);
            mySmtpClient.Credentials = basicAuthenticationInfo;

            try
            {
                mySmtpClient.Send(mail);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("SmtpException has occurred: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                throw new ApplicationException("SmtpException has occurred: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error has occurred: " + ex.Message);
                throw new ApplicationException("Exception has occurred: " + ex.Message, ex);
            }
        }
        public MailMessage CreateEmail(string receiverEmail, string subject, string message)
        {
            MailAddress from = new(Email, "SIR98");
            MailAddress to = new(receiverEmail);

            MailMessage myMail = new(from, to);
            myMail.Subject = subject;
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;
            myMail.Body = $"Hej <b>{receiverEmail}</b>" +
                          $"<br/>" +
                          $"<br/>" +
                          $"{message}" +
                          $"<br/>" +
                          $"<br/>" +
                          $"Venlig hilsen" +
                          $"<br/>" +
                          $"SIR98" +
                          $"<br/>" +
                          $"<br/>" +
                          $"Denne email kan ikke besvares"
                          ;
            myMail.BodyEncoding = System.Text.Encoding.UTF8;
            myMail.IsBodyHtml = true;

            MailAddress replyTo = new(Email);
            myMail.ReplyToList.Add(replyTo);

            return myMail;
        }
    }
}
