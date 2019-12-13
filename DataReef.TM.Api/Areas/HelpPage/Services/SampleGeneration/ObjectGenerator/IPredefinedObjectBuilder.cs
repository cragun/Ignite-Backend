using System;

namespace DataReef.TM.Api.Areas.HelpPage.Services.SampleGeneration.ObjectGenerator
{
    /// <summary>
    /// Predefined object builder that returns a custom materialized instance of an object
    /// </summary>
    public interface IPredefinedObjectBuilder
    {
        bool CanBuild(Type type);

        object Build();
    }
}