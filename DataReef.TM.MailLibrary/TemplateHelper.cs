using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;
using System.Linq;

namespace DataReef.TM
{
    public enum EmailTemplateType
    {
        UserInvitation,
        ResetPassword,
        ExistingUserInvitation,
        SalesOrderConfirmation,
        TerritoryAssignmentNotification,
        UserReactivation,
        PropertyAttachmentRejected,
        PropertyAttachmentSubmitted,
        AppointmentStatusChanged
    }

    public class TemplateHelper
    {
        private static readonly string _basePath;

        static TemplateHelper()
        {
            var appDomain = System.AppDomain.CurrentDomain;
            // to make it work for both web and non-web projects
            _basePath = appDomain.BaseDirectory ?? appDomain.RelativeSearchPath;

            InitRazorEngine();

            Enum
                .GetValues(typeof(EmailTemplateType))
                .OfType<EmailTemplateType>()
                .ToList()
                .ForEach(t =>
                {
                    LoadTemplate(t);
                });
        }

        /// <summary>
        /// Get the email template for given type
        /// </summary>
        /// <typeparam name="T">Template Object type</typeparam>
        /// <param name="templateType">Template type</param>
        /// <param name="data">The data used to replace the placeholders</param>
        /// <returns></returns>
        public static string GetTemplate<T>(EmailTemplateType templateType, T data)
        {
            return Engine.Razor.Run(templateType.ToString(), typeof(T), data);
        }

        // We cache 
        private static void LoadTemplate(EmailTemplateType templateType)
        {
            string name = string.Format("{0}.cshtml", templateType.ToString());
            string template = GetTemplateContent(name);

            Engine.Razor.Compile(template, templateType.ToString());
        }

        private static void InitRazorEngine()
        {
            string layout = GetTemplateContent("_Layout.cshtml");
            Engine.Razor.AddTemplate("_Layout.cshml", layout);
        }

        private static string GetTemplateContent(string fileName)
        {
            string path = Path.Combine(_basePath, "Templates", "Email", fileName);

            return File.ReadAllText(path);
        }
    }
}
