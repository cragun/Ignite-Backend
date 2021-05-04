using DataReef.Core;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace DataReef.Mail
{
    public class Mailer
    {
        public static void SendMail(string[] recipients, string from, string fromName, string cc, string bcc, string subject, string body)
        {
            try
            {
                foreach (string to in recipients)
                {
                    SendMail(to, from, fromName, cc, bcc, subject, body);
                }
            }
            catch (Exception)
            {
                throw; ;
            }
        }

        public static void SendMail(string[] recipients, string from, string fromName, string subject, string body)
        {
            try
            {
                foreach (string to in recipients)
                {
                    SendMail(to, from, fromName, string.Empty, string.Empty, subject, body);
                }
            }
            catch (Exception)
            {
                throw; ;
            }
        }


        public static void SendMail(MailMessage email)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "email-smtp.us-west-2.amazonaws.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("AKIA4L2PBU2P6QM4ECEN", "BGPxa0Z7drmVzIHo6ADj+f2ACjl1JhMe0Hs8PMogme1H");
            smtpClient.Send(email);
        }
        //public static void SendMail(MailMessage email)
        //{
        //    string userName = ConfigurationManager.AppSettings["SendGrid-UserName"];
        //    string password = ConfigurationManager.AppSettings["SendGrid-Password"];
        //    string server = ConfigurationManager.AppSettings["SendGrid-Server"];

        //   // SmtpClient smtpClient = new SmtpClient(server, 25); 
        //    SmtpClient smtpClient = new SmtpClient(server, 587);
        //    // SmtpClient smtpClient = new SmtpClient(server, Convert.ToInt32(587)); 
        //    smtpClient.EnableSsl = true;
        //    smtpClient.Timeout = 100000;
        //    smtpClient.UseDefaultCredentials = false;
        //    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(userName, password);
        //    smtpClient.Credentials = credentials;


        //    smtpClient.Send(email);
        //}

        public static void SendMail(string to, string from, string fromName, string cc, string bcc, string subject, string body)
        {
            try
            {
                if (Constants.APIBaseAddress == "http://api-staging.ignite.trismartsolar.com")
                {
                    subject = "Testing  " + subject;
                    body = $"<p style='font-size:large;'><b> Test Email </b> </p> <br/> " + body;

                }

                string userName = ConfigurationManager.AppSettings["SendGrid-UserName"];
                string password = ConfigurationManager.AppSettings["SendGrid-Password"];
                string server = ConfigurationManager.AppSettings["SendGrid-Server"];

                string recip = to;
                bool reroute = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["RerouteRecipientForTesting"]);

                bool addEmailsToCC = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["AddEmailsToCC"]);

                if (reroute)
                {
                    recip = System.Configuration.ConfigurationManager.AppSettings["RerouteAddress"];
                }

                MailMessage mailMsg = new MailMessage();

                // To
                mailMsg.To.Add(new MailAddress(recip));

                // CC
                if (addEmailsToCC)
                {
                    var emailsToCC = ConfigurationManager.AppSettings["EmailsToCC"];
                    var ccEmails = emailsToCC.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var email in ccEmails)
                    {
                        mailMsg.CC.Add(new MailAddress(email));
                    }
                }

                // From
                mailMsg.From = new MailAddress(from, fromName);
                if (string.IsNullOrWhiteSpace(cc) == false) mailMsg.CC.Add(new MailAddress(cc));
                if (string.IsNullOrWhiteSpace(bcc) == false) mailMsg.Bcc.Add(new MailAddress(bcc));

                // Subject and multipart/alternative Body
                mailMsg.Subject = subject;
                string text = body;
                string html = body;
                //mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient(server, Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(userName, password);
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;

                smtpClient.Send(mailMsg);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string GetActionURL(string v1, string v2, string v3)
        {
            throw new NotImplementedException();
        }
    }
}