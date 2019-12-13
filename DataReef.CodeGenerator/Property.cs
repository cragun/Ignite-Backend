using System;

namespace DataReef.CodeGenerator
{
    public class Property
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool? IsNullable { get; set; }
        public int? MaxLength { get; set; }
        public bool? IsUnicode { get; set; }
        public bool? IsFixedLength { get; set; }
        public int? Precision { get; set; }
        public bool? IsIdentity { get; set; }
        public int? Scale { get; set; }
        public int? MinLength { get; set; }
        public bool IsDeclared { get; set; }
        public string DefaultValue { get; set; }
        public bool IsCollection { get; set; }

        public string ToCoreDataName()
        {
            string ret = this.Name;

            if (ret == "Description") ret = "ClientDescription";
            if (ret == "IsDeleted") ret = "Deleted";

            return ret;
        }

    }
}

