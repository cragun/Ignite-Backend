using System;
using DataReef.Core.Attributes;

namespace DataReef.TM.Api.Areas.HelpPage.Services.SampleGeneration.ObjectGenerator
{
    /// <summary>
    /// An service that looks for predefined object generators and retrieves initialized objects based on that
    /// </summary>
    [Service("PredefinedObjectGenerator", typeof(IObjectGenerator))]
    public class PredefinedObjectGenerator : IObjectGenerator
    {
        private readonly IPredefinedObjectBuilder[] predefinedObjectBuilders;

        public PredefinedObjectGenerator(IPredefinedObjectBuilder[] predefinedObjectBuilders)
        {
            this.predefinedObjectBuilders = predefinedObjectBuilders;
        }

        public object GenerateObject(Type type)
        {
            foreach (var predefinedObjectBuilder in this.predefinedObjectBuilders)
            {
                if (predefinedObjectBuilder.CanBuild(type))
                    return predefinedObjectBuilder.Build();
            }

            return null;
        }

        public int Priority { get { return 0; } }
    }
}