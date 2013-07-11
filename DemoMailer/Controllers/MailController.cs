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

            string[] toAddresses = (to == null) ? new string[0] : to.Split(';');
            string[] ccAddresses = (cc == null) ? new string[0] : cc.Split(';');

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

    }
}
