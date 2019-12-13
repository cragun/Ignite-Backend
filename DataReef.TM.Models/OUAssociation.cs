using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    /// <summary>
    /// Associates a person with an account.  People are global to the TM System, a Person can be in multiple accounts.  This is the many to many object
    /// </summary>
    public class OUAssociation : EntityBase
    {
        /// <summary>
        /// Guid of the Person
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        /// <summary>
        /// Guid of the Account
        /// </summary>
        [DataMember]
        public Guid OUID { get; set; }

        [DataMember]
        public Guid OURoleID { get; set; }

        [DataMember]
        public OURoleType RoleType { get; set; }

        #region Navigation

        [ForeignKey("OUID")]
        //[JsonIgnore]
        [DataMember]
        public OU OU { get; set; }

        [ForeignKey("PersonID")]
        //[JsonIgnore]
        [DataMember]
        public Person Person { get; set; }

        [DataMember]
        [ForeignKey("OURoleID")]
        public OURole OURole { get; set; }


        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            OU = FilterEntity(OU, newInclusionPath);
            Person = FilterEntity(Person, newInclusionPath);
            OURole = FilterEntity(OURole, newInclusionPath);

        }


        public override void SetupSerialization(string include, string exclude, string fields)
        {
            // When we setup exclude Serialization for OUAssociations, the exclude property contains the property name.
            // because we're Setting it up on the child property, we need to remove the child property from property.            
            var props = new List<string> { "OU", "OURole", "Person" };

            foreach (var prop in props)
            {
                exclude = RemoveStartingPropFromChain(exclude, prop);
                include = RemoveStartingPropFromChain(include, prop);
            }

            if (OU != null)
            {
                OU.SetupSerialization(include, exclude, fields);
            }

            if (OURole != null)
            {
                OURole.SetupSerialization(include, exclude, fields);
            }

            if (Person != null)
            {
                Person.SetupSerialization(include, exclude, fields);
            }
        }

    }
}
