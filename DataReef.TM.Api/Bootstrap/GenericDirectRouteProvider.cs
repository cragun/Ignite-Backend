using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace DataReef.TM.Api.Bootstrap
{
    /// <summary>
    /// A custom direct route provider that enables inheritance of routes(actions) from base controller
    /// </summary>
    internal class GenericDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            // inherit route attributes decorated on base class controller's actions
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(true);
        }
    }
}