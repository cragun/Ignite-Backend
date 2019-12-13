using System;

namespace DataReef.TM.Classes
{
    public class BaseTemplate
    {
        public string ToPersonName { get; set; }
        public string ToPersonEmail { get; set; }
        public string FromPersonName { get; set; }
        public Guid Guid { get; set; }
        public string RecipientEmailAddress { get; set; }

        public string EncodedToPersonEmail()
        {
            return ToPersonEmail.UrlEncodeEmail();
        }
    }
}
