using System.Collections.ObjectModel;

namespace DataReef.TM.Api.Areas.HelpPage.Models
{
    /// <summary>
    /// The model for the main (dashboard) API help page
    /// </summary>
    public class HelpPageApiDescriptionModel
    {
        /// <summary>
        /// Gets or sets the <see cref="HelpPageApiResourceModel" /> collection that describes the available endpoints and models/>
        /// </summary>
        public Collection<HelpPageApiResourceModel> ResouceModels { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpPageApiDescriptionModel"/> class.
        /// </summary>
        public HelpPageApiDescriptionModel()
        {
            this.ResouceModels = new Collection<HelpPageApiResourceModel>();
        }
    }
}