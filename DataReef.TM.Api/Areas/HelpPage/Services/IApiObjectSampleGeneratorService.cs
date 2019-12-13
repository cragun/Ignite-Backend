using System.Collections.Generic;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Areas.HelpPage.Services
{
    /// <summary>
    /// Generates data required to communicate using an endpoint
    /// </summary>
    public interface IApiObjectSampleGeneratorService
    {
        /// <summary>
        /// Generates a sample response
        /// </summary>
        /// <param name="apiDescription">The <see cref="ApiDescription"/> </param>
        /// <param name="sampleGenerationErrors"></param>
        /// <param name="mediaType">The output media type</param>
        /// <returns>An object of type corresponding to the requested <paramref name="mediaType"/> based on <paramref name="apiDescription"/></returns>
        object GenerateResponseSampleForApi(ApiDescription apiDescription, ICollection<string> sampleGenerationErrors, string mediaType = "application/json");


        /// <summary>
        /// Generates a sample response
        /// </summary>
        /// <param name="apiDescription">The <see cref="ApiDescription"/> </param>
        /// <param name="sampleGenerationErrors"></param>
        /// <param name="mediaType"></param>
        /// <returns>An object of type corresponding to the requested <paramref name="mediaType"/> based on <paramref name="apiDescription"/></returns>
        object GenerateRequestSampleForApi(ApiDescription apiDescription, ICollection<string> sampleGenerationErrors, string mediaType = "application/json");
    }
}