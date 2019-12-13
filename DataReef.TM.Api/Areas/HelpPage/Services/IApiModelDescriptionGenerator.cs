using System;
using System.Collections.Generic;
using DataReef.TM.Api.Areas.HelpPage.Services.ModelDescriptions;

namespace DataReef.TM.Api.Areas.HelpPage.Services
{
    /// <summary>
    /// An interface that provides description for API models
    /// </summary>
    public interface IApiModelDescriptionGenerator
    {
        /// <summary>
        /// Provides a solution for retrieving the <see cref="ModelDescription" /> for an API model
        /// </summary>
        /// <param name="modelType">The type of the model</param>
        /// <returns>The <see cref="ModelDescription"/> for the specific <see cref="modelType"/></returns>
        ModelDescription GetOrCreateModelDescription(Type modelType);
        
        /// <summary>
        /// Provides a solution for retrieving the <see cref="ModelDescription" /> for an API model
        /// </summary>
        /// <param name="modelName">The name of the model</param>
        /// <returns>The <see cref="ModelDescription"/> for the specific <see cref="modelName"/></returns>
        ModelDescription GetOrCreateModelDescription(string modelName);

        /// <summary>
        /// Retrieve get all API registered model descriptions
        /// </summary>
        /// <returns>A list of models</returns>
        IEnumerable<ModelDescription> GetAllModelDescriptions();
    }
}