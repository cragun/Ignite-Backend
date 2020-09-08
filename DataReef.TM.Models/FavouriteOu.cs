using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace DataReef.TM.Models
{
    public class FavouriteOu : EntityBase
    {
        #region Properties

        /// <summary>
        /// Guid of the person assigned a Ou
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public Guid OUID { get; set; }


        #endregion

        #region Navigation

        [ForeignKey("PersonID")]
        [DataMember]
        public Person Person { get; set; }

        [ForeignKey("OUID")]
        [DataMember]
        public OU OU { get; set; }


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
            OU = FilterEntity(OU, newInclusionPath);

        }

    }
}