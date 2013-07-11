using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DemoMailer.Controllers
{
    public class MailController : ApiController
    {
        //
        // POST: /Mail/
        [HttpPost]
        [ActionName("send")]
        public HttpResponseMessage SendMail(string to, string cc, string subject, string body)
        {

            string[] toAddresses = SplitAddresses(to);
            string[] ccAddresses = SplitAddresses(cc);

            // If any of the addresses provided are invalid, refuse the request.
            if (toAddresses.Concat(ccAddresses).Any(address => !Mailer.VerifyEmail(address)))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // Send the e-mail.
            try
            {
                Mailer.SendMail(toAddresses, ccAddresses, subject, body);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// This divides semicolon-delimited addresses into an array of individual addresses, and removes empty entries.
        /// </summary>
        /// <param name="addresses">The address list.</param>
        /// <returns>An array of addresses taken from <paramref name="addresses"/>.</returns>
        private static string[] SplitAddresses(string addresses)
        {
            return (addresses == null) ? 
                new string[0] : 
                addresses.Split(';').Where(address => address.Trim().Length > 0).ToArray();
        }
    }
}
