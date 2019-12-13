using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.ViewModels
{
    /// <summary>
    /// Class used to pass parameters to the client.
    /// The client will use these parameters to replace placeholders in the Legion.LeftMenu.WebViewItems OU Setting.
    /// (e.g. for the following value http://somewebsite.com?legionToken={LegionToken}, the client will replace {property} with the value)
    /// </summary>
    public class IntegrationParameters
    {
        public string LegionToken { get; set; }

    }
}