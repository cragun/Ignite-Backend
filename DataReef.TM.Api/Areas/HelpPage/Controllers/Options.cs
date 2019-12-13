using System.Web.Mvc;

namespace DataReef.TM.Api.Areas.HelpPage.Controllers
{
    [RouteArea("HelpPage", AreaPrefix = "help")]
    public class OptionsController : Controller
    {
        [Route("Options")]
        public ActionResult Index()
        {
            return View();
        }
    }
}