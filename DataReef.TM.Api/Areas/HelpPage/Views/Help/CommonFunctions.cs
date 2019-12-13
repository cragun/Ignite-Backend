using System.Net.Http;

namespace DataReef.TM.Api.Areas.HelpPage.Views.Help
{
    /// <summary>
    /// Common functions used to render view content
    /// </summary>
    public static class CommonFunctions
    {
        public static string GetAssociatedActionColor(HttpMethod method)
        {
            if (method == HttpMethod.Get)
                return "primary";

            if (method == HttpMethod.Post)
                return "success";

            if (method == HttpMethod.Put)
                return "warning";

            if (method.ToString() == "PATCH")
                return "warning";

            if (method == HttpMethod.Delete)
                return "danger";

            return string.Empty;
        }
    }
}