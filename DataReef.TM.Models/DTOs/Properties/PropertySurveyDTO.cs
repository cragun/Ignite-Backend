using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Properties
{
    public class PropertySurveyDTO
    {
        public Guid Guid { get; set; }

        public string PropertyName { get; set; }

        public string PropertyAddress { get; set; }

        public DateTime DateCreated { get; set; }

        public Guid PropertyID { get; set; }

        public Guid PersonID { get; set; }

        public string Value { get; set; }

        public PropertySurveyDTO(PropertySurvey propertySurvey)
        {
            this.Guid = propertySurvey.Guid;
            this.PropertyName = propertySurvey.Property?.Name;
            this.PropertyAddress = propertySurvey.Property?.Address1;
            this.DateCreated = propertySurvey.DateCreated;
            this.PropertyID = propertySurvey.PropertyID;
            this.PersonID = propertySurvey.PersonID;
            this.Value = propertySurvey.Value;
        }
    }
}
