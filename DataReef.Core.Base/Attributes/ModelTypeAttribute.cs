using System;

namespace DataReef.Core
{
    public class ModelTypeAttribute:System.Attribute
    {
        public ModelTypeAttribute(Type type)
        {
            this.ModelType = type;
        }

        public Type ModelType { get; set; }
    }
}
