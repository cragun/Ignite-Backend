using System;

namespace DataReef.TM.Api.Areas.HelpPage.Services.SampleGeneration.ObjectGenerator
{
    /// <summary>
    /// Service that generates objects with sample data
    /// </summary>
    public interface IObjectGenerator
    {
        /// <summary>
        /// Generates an object with data.
        /// </summary>
        /// <param name="type">The type of the object</param>
        /// <returns>An initialized object, null if the object could not be materialized</returns>
        object GenerateObject(Type type);

        /// <summary>
        /// The generator priotity, smaller = higher priority
        /// </summary>
        int Priority { get; }
    }
}