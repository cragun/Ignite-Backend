using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataReef.TM.Models
{
    /// <summary>
    /// Current locations are Lat Lon Pings from the client, recording the location of the user
    /// </summary>
    public class CurrentLocation:EntityBase
    {

        #region properties

       
        /// <summary>
        /// PersonID of the User
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        /// <summary>
        /// Latitude in decimal format
        /// </summary>
        [DataMember]
        public float Lat { get; set; }

        /// <summary>
        /// Longitude in decimal format
        /// </summary>
        [DataMember]
        public float Lon { get; set; }

        /// <summary>
        /// accuracy of Lat/Lon in Meters (as reported by device)
        /// </summary>
        [DataMember]
        public float Accuracy { get; set; }

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

    }
}
