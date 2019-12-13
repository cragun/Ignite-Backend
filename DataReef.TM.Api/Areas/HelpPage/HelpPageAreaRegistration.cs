using System.Web.Http;
using System.Web.Mvc;

namespace DataReef.TM.Api.Areas.HelpPage
{
    public class HelpPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HelpPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}