using System;
using System.ComponentModel.DataAnnotations;

namespace DataReef.TM.Api.Classes
{
    //internal class NotEqualAttribute : Attribute
    //{
    //}


    public class NotEqualAttribute : ValidationAttribute
    {
        private string OtherProperty { get; set; }

        public NotEqualAttribute(string otherProperty)
        {
            OtherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // get other property value
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            // verify values
            if (Convert.ToString(value).Equals(Convert.ToString(otherValue)))
                return new ValidationResult(string.Format("{0} should not be equal to {1}.", validationContext.MemberName, OtherProperty));
            else
                return ValidationResult.Success;
        }
    }
}