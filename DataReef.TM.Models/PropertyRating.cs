using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    /// <summary>
    /// new model to replace propertyattribute.  This class should not have referential integrity.  The properties may not exist yet on 
    /// the application server.   We want to get away from PropertyAttributes that require a property to exist on the server 
    /// propertyattribute will be phased out
    /// </summary>
    /// 
    
    public class PropertyRating : EntityBase
    {
        [DataMember]
        public string RecordLocator { get; set; }

        [DataMember]
        [StringLength(50)]
        public string DisplayType { get; set; }

        [DataMember]
        [StringLength(150)]
        public string Value { get; set; }

        /// <summary>
        /// The guid of the territory that owns this rating
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string OwnerID { get; set; }


        [DataMember]
        [StringLength(50)]
        public string AttributeKey { get; set; }

        /// <summary>
        /// The number of minutes after the DateCreated that this property attribute is valid
        /// </summary>
        /// <value>defaults: 28800 mins for batch prescreen, 43200 mins for instant prescreen</value>
        [DataMember]
        [DefaultValue(0)]
        public int ExpiryMinutes { get; set; }

        #region Navigation
        //no navigation on purpose
        #endregion



    }
}
