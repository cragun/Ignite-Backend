using DataReef.Core.Attributes;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [DataContract(IsReference = true)]
    [Versioned]
    public class PropertySurvey : EntityBase
    {

        public PropertySurvey()
        {
            
        }

        #region Properties

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public Guid PropertyID { get; set; }

        [DataMember]
        public string Value { get; set; }

        #endregion

        #region Navigation

        [DataMember]
        public Person Person { get; set; }

        [DataMember]
        public Property Property { get; set; }

        #endregion

        
    }
}