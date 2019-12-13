using System.Collections.ObjectModel;

namespace DataReef.TM.Api.Areas.HelpPage.Services.ModelDescriptions
{
    public class ParameterDescription
    {
        public ParameterDescription()
        {
            this.Annotations = new Collection<ParameterAnnotation>();
        }

        public Collection<ParameterAnnotation> Annotations { get; private set; }

        public string Documentation { get; set; }

        public string Name { get; set; }

        public ModelDescription TypeDescription { get; set; }

        public object DefaultValue { get; set; }
    }
}