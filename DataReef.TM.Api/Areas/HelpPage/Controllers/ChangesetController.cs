using System.Web.Mvc;

namespace DataReef.TM.Api.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle version updates
    /// </summary>
    [RouteArea("HelpPage", AreaPrefix = "help")]
    public class ChangesetController : Controller
    {
        /// <summary>
        /// Action that returns application changeset details
        /// </summary>
        /// <returns>The <see cref="ActionResult"/> for the changeset page</returns>
        [Route("Changeset")]
        public ActionResult Index()
        {
            return View();
        }
    }
}