using System.Net.Mail;

namespace Sir98Backend.Services
{
    public class EmailService
    {
        private string Email;
        private string Password;
        private string Host;
        private int Port;

        public EmailService()
        {
            string filepath = Environment.CurrentDirectory + "\\Keys\\Email server credentials.txt";
            List<string> keyForSigning = System.IO.File.ReadLines(filepath).ToList();
            Email = keyForSigning[0];
            Password = keyForSigning[1];
            Host = keyForSigning[2];
            if (Int32.TryParse(keyForSigning[3], out Port) == false)
            {
                throw new Exception("Port is not a number");
            }
            Console.WriteLine(Email + Password + Host + Port);
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
                throw;
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
