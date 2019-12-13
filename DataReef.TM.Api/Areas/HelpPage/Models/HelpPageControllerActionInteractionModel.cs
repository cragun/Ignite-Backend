using System.Collections.ObjectModel;
using DataReef.TM.Api.Areas.HelpPage.Services.ModelDescriptions;

namespace DataReef.TM.Api.Areas.HelpPage.Models
{
    /// <summary>
    /// This model denotes resources necessary to communicate with an endpoint action (ex: model samples, parameters samples, ...)
    /// </summary>
    public class HelpPageControllerActionInteractionModel
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public HelpPageControllerActionInteractionModel()
        {
            this.SampleGenerationErrors = new Collection<string>();
            this.UriParameters = new Collection<ParameterDescription>();
        }

        /// <summary>
        /// The full path to the action URL with parameters like {key} included
        /// </summary>
        public string ActionUrl { get; set; }

        /// <summary>
        /// The HTTP verge used to call the action
        /// </summary>
        public string HttpActionVerb { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ParameterDescription"/> collection that describes the URI parameters for the API.
        /// </summary>
        public Collection<ParameterDescription> UriParameters { get; private set; }

        /// <summary>
        /// Sample request object
        /// </summary>
        public object SampleRequests { get; set; }

        /// <summary>
        /// Sample response object
        /// </summary>
        public object SampleResponses { get; set; }

        /// <summary>
        /// Errors encounters when generating sample
        /// </summary>
        public Collection<string> SampleGenerationErrors { get; set; }
    }
}