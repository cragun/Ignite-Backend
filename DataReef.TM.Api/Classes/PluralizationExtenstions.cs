using System;
using DataReef.Core.Pluralization;


namespace DataReef.TM.Api.Classes
{
    /// <summary>
    /// String extensions to pluralize using the EF pluralization services
    /// </summary>
    public static class PluralizationExtenstions
    {
        public static String Pluralize(this String @string)
        {
            return new EnglishPluralizationService().Pluralize(@string);
        }

        public static String Singularize(this String @string)
        {
            return new EnglishPluralizationService().Singularize(@string);
        }
    }
}