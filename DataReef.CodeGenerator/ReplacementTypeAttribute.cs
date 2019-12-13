using System;

namespace DataReef.CodeGenerator
{
    /// <summary>
    /// tells the code generator to replace the Type specified with the Type decorated
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ReplacementTypeAttribute:System.Attribute
    {
        public ReplacementTypeAttribute(Type type)
        {
            this.Type = type;

        }
        public Type Type { get; set; }
    }
}
