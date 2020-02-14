using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.SmartBoard
{
    [DataContract]
    [NotMapped]
    public class SBPropertyDTO
    {
        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public long LeadID { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string MiddleNameInitial { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string AddressLine1 { get; set; }

        [DataMember]
        public string AddressLine2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public double? Latitude { get; set; }

        [DataMember]
        public double? Longitude { get; set; }

        [DataMember]
        public Guid TerritoryID { get; set; }

        [DataMember]
        public int? DispositionTypeId { get; set; }

        public SBPropertyDTO(Property prop)
        {
            if(prop == null)
            {
                return;
            }

            Guid = prop.Guid;
            LeadID = prop.Id;

            var mainOccupant = prop.GetMainOccupant();

            FirstName = mainOccupant?.FirstName;
            LastName = mainOccupant?.LastName;
            MiddleNameInitial = mainOccupant?.MiddleInitial;
            EmailAddress = prop.GetMainEmailAddress();
            Phone = prop.GetMainPhoneNumber();
            AddressLine1 = prop.Address1;
            AddressLine2 = prop.Address2;
            City = prop.City;
            State = prop.State;
            ZipCode = prop.ZipCode;
            Latitude = prop.Latitude;
            Longitude = prop.Longitude;
            TerritoryID = prop.TerritoryID;
            DispositionTypeId = prop.DispositionTypeId;
        }
    }
}
