using System.Collections.Generic;

namespace DataReef.TM.Api.Areas.HelpPage.Models
{
    /// <summary>
    /// Model describing basic endpoint information 
    /// </summary>
    public class HelpPageControllerModel
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public HelpPageControllerModel()
        {
            this.Actions = new List<HelpPageControllerActionDescriptionModel>();
        }

        /// <summary>
        /// The name of the endpoint
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// List of actions that the endpoint can facilitate
        /// </summary>
        public ICollection<HelpPageControllerActionDescriptionModel> Actions { get; set; }
    }
}