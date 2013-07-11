using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace DemoMailer.Controllers
{
    /// <summary>
    /// This class can be used to send e-mails to people.
    /// Scalability could be improved by making this an instantiable class and with asynchronous use of the SMTP client, but given the low-usage deployment scenario I think that's overkill.
    /// </summary>
    public static class Mailer
    {
        /// <summary>
        /// Verifies that a string is a valid e-mail address.
        /// </summary>
        /// <param name="address">The address to be verified.</param>
        /// <returns>If the address is valid, returns <c>true</c>; otherwise, <c>false</c>.</returns>
        public static bool VerifyEmail(string address)
        {
            // E-mail-verifying regular expression courtesy of msdn.  
            const string expression = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
            return Regex.IsMatch(address, expression, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Sends mail to the specified sender.
        /// </summary>
        /// <param name="to">To addresses</param>
        /// <param name="cc">Cc addresses</param>
        /// <param name="subject">The e-mail's subject line.</param>
        /// <param name="body">The e-mail's body.</param>
        public static void SendMail(string[] to, string[] cc, string subject, string body)
        {
            if (to == null || to.Length == 0)
            {
                throw new ArgumentException("At least one recipient must be specified.");
            }

            MailAddress fromAddress = new MailAddress("demo.mail.98989@gmail.com", "Demo Mail Account");
            MailAddress toAddress = new MailAddress(to.First());

            MailMessage message = new MailMessage(fromAddress, toAddress);

            foreach (string address in to.Skip(1))
            {
                message.To.Add(address);
            }

            if (cc != null)
            {
                foreach (string address in cc)
                {
                    message.CC.Add(address);
                }
            }

            message.Subject = subject;
            message.Body = body;

            using (SmtpClient client = ClientFactory())
            {
                client.Credentials = new NetworkCredential("demo.mail.98989@gmail.com", "IopIop789");
                client.EnableSsl = true;
                client.Send(message);
            }
        }

        /// <summary>
        /// Sets up an SMTP client that can be used to send e-mail.
        /// </summary>
        /// <returns>A fully-instantiated SMTP client.</returns>
        private static SmtpClient ClientFactory()
        {
            string host = WebConfigurationManager.AppSettings["smtpHost"];
            string rawPort = WebConfigurationManager.AppSettings["smtpPort"];

            int port;
            if (!int.TryParse(rawPort, out port))
            {
                return new SmtpClient(host);
            }

            return new SmtpClient(host, port);
        }
    }
}