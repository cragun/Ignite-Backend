using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace DataReef.TM.Models
{
    public class AppointmentFavouritePerson : EntityBase
    {
        #region Properties

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public Guid FavouritePersonID { get; set; }

        #endregion
    }
}