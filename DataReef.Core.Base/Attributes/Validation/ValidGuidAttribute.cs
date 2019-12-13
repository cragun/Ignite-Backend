using System;
using System.ComponentModel.DataAnnotations;

namespace DataReef.Core.Attributes.Validation
{
    public class ValidGuidAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value.GetType() != typeof(Guid))
                return false;

            return (Guid)value != Guid.Empty;
        }
    }
}
