using System.Web.Mvc;

namespace DataReef.TM.Api.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle the API feedback
    /// </summary>
    [RouteArea("HelpPage", AreaPrefix = "help")]
    public class FeedbackController : Controller
    {
        /// <summary>
        /// Action that returns help API feedback page
        /// </summary>
        /// <returns>The <see cref="ActionResult"/> for the feedback page</returns>
        [Route("Feedback")]
        public ActionResult Index()
        {
            return View();
        }
    }
}