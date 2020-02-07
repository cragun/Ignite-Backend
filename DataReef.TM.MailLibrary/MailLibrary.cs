using DataReef.Core;
using DataReef.TM.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;

namespace DataReef.TM.Mail
{
    public class Library
    {
        private static string _senderEmail = ConfigurationManager.AppSettings["SenderEmail"] ?? "info@trismartsolar.com";
        public static string SenderName = ConfigurationManager.AppSettings["Email.SenderName"] ?? "Ingite App";

        public static void SendOUAssoicationConfirmationToAdmin(string toPersonName, string toPersonEmail, string fromPersonName, string ouName)
        {
            //todo: create from template and send
            string body = string.Format("{0} has accepted and joined your {1} team.  You can now assign territory to him/her. ", toPersonName, ouName);
            string subject = string.Format("New User in {0}", ouName);
            DataReef.Mail.Mailer.SendMail(new string[] { toPersonEmail }, _senderEmail, fromPersonName, subject, body);

        }

        public static void SendPassworReset(ResetPasswordTemplate template)
        {
            template.ResetURL = GetActionURL("resetpassword", "guid={0}", template.Guid);
            string body = TemplateHelper.GetTemplate(EmailTemplateType.ResetPassword, template);
            string subject = string.Format($"Reset your password for {SenderName}");
            DataReef.Mail.Mailer.SendMail(new string[] { template.ToPersonEmail }, _senderEmail, template.FromPersonName, subject, body);
        }

        public static void SendTerritoryAssignmentNotification(TerritoryAssignmentNotificationTemplate template, string from, string fromName, string subject, string toPersonEmail)
        {
            EmailTemplateType templateType = EmailTemplateType.TerritoryAssignmentNotification;

            string body = TemplateHelper.GetTemplate(templateType, template);

            DataReef.Mail.Mailer.SendMail(
                to: toPersonEmail,
                from: from,
                fromName: fromName,
                cc: null,
                bcc: null,
                subject: subject,
                body: body);
        }

        public static void SendUserInvitationEmail(UserInvitationTemplate template)
        {
            template.InvitationURL = GetActionURL("userinvitation", "guid={0}&username={1}", template.Guid, template.EncodedToPersonEmail());
            EmailTemplateType templateType = EmailTemplateType.UserInvitation;
            if (template.ToPersonId.HasValue)
            {
                template.InvitationURL += string.Format("&toPersonID={0}", template.ToPersonId.Value);
                 templateType = EmailTemplateType.ExistingUserInvitation;
            }
            string body = TemplateHelper.GetTemplate(templateType, template);

            //string body = "";
            //body = body + string.Format("You have been invited to join {0}.", template.OUName);

            //if (template.UserWasDeleted)
            //{
            //    body = body + "Your user was deleted but is now restored because of this invitation.You may use your credentials to log in.";
            //}
            //body = body + "<p> Make sure IGNITE application is installed on your iOS device.If IGNITE is not installed, please check the information below to download it.</p>";
            //body = body + "<p> If you are logged in, please go to the main organizations screen and accept the invitation from there.</p>";

            //if (!string.IsNullOrWhiteSpace(template.DownloadURL))
            //{
            //    body = body + string.Format("<a href='{0}'>Download app</a><br />", template.DownloadURL);
            //    body = body + "<p>After downloading the app, on your device, tap the link below to accept this invitation.</p>";
            //}
            //else
            //{
            //    body = body + "<p>Please check you e-mail for the latest Test Flight invitation or contact our Support Team to receive a new one.After installing the app, tap the link below to accept this invitation.</p>";
            //}
            //body = body + string.Format("<a href='{0}'>Accept invitation</a>", template.InvitationURL);

            string subject = string.Format("You've been invited to join {0}", template.OUName);
            DataReef.Mail.Mailer.SendMail(new string[] { template.ToPersonEmail }, _senderEmail, template.FromPersonName, subject, body);
        }

        public static void SendUserReactivationEmail(ReactivateAccountTemplate template)
        {
            template.FromPersonName = Constants.FromEmailName;

            var templateType = EmailTemplateType.UserReactivation;

            string body = TemplateHelper.GetTemplate(templateType, template);
            string subject = "Your user has been reactivated.";

            DataReef.Mail.Mailer.SendMail(new string[] { template.RecipientEmailAddress }, _senderEmail, template.FromPersonName, subject, body);
        }

        public static void SendUserInvitationAcceptedEmail(UserInvitationAcceptedTemplate template)
        {
            string subject = string.Format("Invitation to join {0} accepted", template.OUName);
            string body = string.Format("User {0}({1}) accepted your invitation on {2} having a role of {3}.", template.ToPersonName, template.ToPersonEmail, template.OUName, template.RoleName);

            DataReef.Mail.Mailer.SendMail(new string[] { template.RecipientEmailAddress }, _senderEmail, template.FromPersonName, subject, body);
        }

        public static void SendPropertyAttachmentRejectedEmail(PropertyAttachmentRejectedTemplate template)
        {
            template.FromPersonName = Constants.FromEmailName;

            var templateType = EmailTemplateType.PropertyAttachmentRejected;

            string body = TemplateHelper.GetTemplate(templateType, template);
            string subject = template.EmailSubject;

            DataReef.Mail.Mailer.SendMail(new string[] { template.RecipientEmailAddress }, _senderEmail, template.FromPersonName, subject, body);
        }

        public static void SendPropertyAttachmentSubmittedEmail(PropertyAttachmentSubmittedTemplate template)
        {
            template.FromPersonName = Constants.FromEmailName;

            var templateType = EmailTemplateType.PropertyAttachmentSubmitted;

            string body = TemplateHelper.GetTemplate(templateType, template);
            string subject = template.EmailSubject;

            DataReef.Mail.Mailer.SendMail(new string[] { template.RecipientEmailAddress }, _senderEmail, template.FromPersonName, subject, body);
        }

        public static void SendAppointmentStatusChangedEmail(AppointmentStatusChangedTemplate template)
        {
            template.FromPersonName = Constants.FromEmailName;

            var templateType = EmailTemplateType.AppointmentStatusChanged;

            string body = TemplateHelper.GetTemplate(templateType, template);
            string subject = template.EmailSubject;

            DataReef.Mail.Mailer.SendMail(new string[] { template.RecipientEmailAddress }, _senderEmail, template.FromPersonName, subject, body);
        }

        public static void SendEmail(MailMessage email)
        {
            DataReef.Mail.Mailer.SendMail(email);
        }

        public static void SendEmail(string to, string cc, string subject, string body, bool isHtml = false, List<Attachment> attachments = null)
        {
            var email = new MailMessage();
            email.From = new MailAddress(_senderEmail, SenderName);
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = isHtml;

            var toList = to.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var toItem in toList)
            {
                if (string.IsNullOrWhiteSpace(toItem))
                {
                    continue;
                }
                email.To.Add(new MailAddress(toItem));
            }

            if (!string.IsNullOrWhiteSpace(cc))
            {
                var items = cc.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in items)
                {
                    email.CC.Add(item);
                }
            }

            if (attachments != null)
            {
                foreach (var att in attachments)
                {
                    if (att != null)
                    {
                        email.Attachments.Add(att);
                    }
                }
            }

            SendEmail(email);
        }


        private static string GetActionURL(string action, string format, params object[] args)
        {
            string path = string.Format("{0}/home/redirect?action={1}&", Constants.APIBaseAddress, action);
            Uri baseUri = new Uri(path);

            if (string.IsNullOrEmpty(format))
            {
                return baseUri.AbsoluteUri;
            }

            var parameters = string.Format(format, args);
            return string.Format("{0}{1}", path, parameters);
        }


        public static void SendSalesOrderConfirmationEmail(SalesOrderConfirmationTemplate template, string from, string fromName, string subject)
        {
            EmailTemplateType templateType = EmailTemplateType.SalesOrderConfirmation;

            string body = TemplateHelper.GetTemplate(templateType, template);

            DataReef.Mail.Mailer.SendMail(
                to: template.PaymentEmailAddress,
                from: from,
                fromName: fromName,
                cc: null,
                bcc: null,
                subject: subject,
                body: body);
        }
    }
}
