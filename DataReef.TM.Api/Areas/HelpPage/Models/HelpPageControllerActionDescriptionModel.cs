using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using DataReef.TM.Api.Areas.HelpPage.Services.ModelDescriptions;

namespace DataReef.TM.Api.Areas.HelpPage.Models
{
    /// <summary>
    /// Model describing an endpoint action
    /// </summary>
    public class HelpPageControllerActionDescriptionModel
    {
        /// <summary>
        /// The name of an endpoint action
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// The description of an endpoint action
        /// </summary>
        public string ActionDescription { get; set; }

        /// <summary>
        /// The Uri of an endpoint action with parameters
        /// </summary>
        public string ActionUri { get; set; }

        /// <summary>
        /// The action is part of the default CRUD API
        /// </summary>
        public bool IsPartOfCrudApi { get; set; }

        /// <summary>
        /// Alternative Uri's of an endpoint action with parameters. Most of them should be longer and unfriendlier
        /// </summary>
        public IEnumerable<string> AlternativeUris { get; set; }

        public HelpPageControllerActionDescriptionModel()
        {
            this.UriParameters = new Collection<ParameterDescription>();
        }

        /// <summary>
        /// The type of action
        /// </summary>
        public HttpMethod ActionType { get; set; }

        #region Uri Parameters Data

        /// <summary>
        /// Gets or sets the <see cref="ParameterDescription"/> collection that describes the URI parameters for the API.
        /// </summary>
        public Collection<ParameterDescription> UriParameters { get; private set; }

        #endregion

        #region Body Parameter Data

        /// <summary>
        /// Gets or sets the documentation for the request.
        /// </summary>
        public string RequestDocumentation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ModelDescription"/> that describes the request body.
        /// </summary>
        public ModelDescription RequestModelDescription { get; set; }

        /// <summary>
        /// Gets the request body parameter descriptions.
        /// </summary>
        public IList<ParameterDescription> RequestBodyParameters
        {
            get
            {
                return GetParameterDescriptions(this.RequestModelDescription);
            }
        }

        #endregion

        #region Response Data

        /// <summary>
        /// Gets or sets the <see cref="ModelDescription"/> that describes the resource.
        /// </summary>
        public ModelDescription ResourceDescription { get; set; }

        /// <summary>
        /// Gets the resource property descriptions.
        /// </summary>
        public IList<ParameterDescription> ResourceProperties
        {
            get
            {
                return GetParameterDescriptions(this.ResourceDescription);
            }
        }

        #endregion

        private static IList<ParameterDescription> GetParameterDescriptions(ModelDescription modelDescription)
        {
            ComplexTypeModelDescription complexTypeModelDescription = modelDescription as ComplexTypeModelDescription;
            if (complexTypeModelDescription != null)
            {
                return complexTypeModelDescription.Properties;
            }

            CollectionModelDescription collectionModelDescription = modelDescription as CollectionModelDescription;
            if (collectionModelDescription != null)
            {
                complexTypeModelDescription = collectionModelDescription.ElementDescription as ComplexTypeModelDescription;
                if (complexTypeModelDescription != null)
                {
                    return complexTypeModelDescription.Properties;
                }
            }

            return null;
        }
    }
}