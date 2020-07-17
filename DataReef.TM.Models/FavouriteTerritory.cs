using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace DataReef.TM.Models
{
    public class FavouriteTerritory : EntityBase
    {
        #region Properties

        /// <summary>
        /// Guid of the person assigned a territory
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public Guid TerritoryID { get; set; }

        [DataMember]
        public bool isFavourite { get; set; }

     

        #endregion

        #region Navigation

        [ForeignKey("PersonID")]
        [DataMember]
        public Person Person { get; set; }

        [ForeignKey("TerritoryID")]
        [DataMember]
        public Territory Territory { get; set; }
     

        #endregion


        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Person      = FilterEntity(Person,      newInclusionPath);
            Territory   = FilterEntity(Territory,   newInclusionPath);

        }

    }
}