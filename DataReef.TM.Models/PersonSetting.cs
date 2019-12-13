using DataReef.TM.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [DataContract]
    public enum PersonSettingGroupType
    {
        [EnumMember]
        Membership = 1,
        [EnumMember]
        Prescreen,
        [EnumMember]
        SolarProposal,
        [EnumMember]
        KPIs,
        [EnumMember]
        Tallies
    }

    /// <summary>
    /// A setting for a Person.
    /// </summary>
    public class PersonSetting : EntityBase
    {

        #region Properties

        /// <summary>
        /// Guid of the Person.
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public SettingValueType ValueType { get; set; }
        [DataMember]
        public PersonSettingGroupType Group { get; set; }
        #endregion

        #region Navigation

        [ForeignKey("PersonID")]
        [DataMember]
        public Person Person { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Person = FilterEntity(Person, newInclusionPath);
        }

        public void CopyFrom(PersonSetting values)
        {
            Value = values.Value;
            ValueType = values.ValueType;
            Group = values.Group;
        }

        #region Well Known Setting Names
        public static readonly string TalliesLastResetDate = "TalliesLastResetDate";
        #endregion

    }
}
