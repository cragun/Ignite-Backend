using System;

namespace DataReef.CodeGenerator
{

    [System.AttributeUsage(System.AttributeTargets.Class,AllowMultiple=true)]
    public class ManyToManyAttribute:Attribute
    {
        public ManyToManyAttribute(string foreignEntity) {
            this.ForeignEntity=foreignEntity;
        }
        public string ForeignEntity{get;set;}
    }
}
