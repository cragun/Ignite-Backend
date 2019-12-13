using DataReef.Core.Attributes;
using DataReef.Core.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Geo
{
    [Table("Properties")]
    public abstract class PropertyBase : EntityBase
    {
        private string houseNumber = string.Empty;

        #region Properties


        [DataMember(EmitDefaultValue = false)]
        public double? Latitude { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double? Longitude { get; set; }

        [StringLength(20)]
        [DataMember]
        public string HouseNumber
        {
            get { return this.houseNumber; }
            set
            {
                this.houseNumber = value;

                try
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {

                        int houseNumber = 0;
                        bool parsed = int.TryParse(value, out houseNumber);

                        if (parsed == true)
                        {
                            this.IsEven = houseNumber % 2 == 0;
                        }
                    }
                }
                catch { }
            }
        }

        [DataMember]
        public bool IsEven { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50, ErrorMessage = "Address Line 1 cannot be longer than 50 characters")]
        [Index]
        public string Address1 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(200)]
        public string Address2 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string City { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(2)]
        public string State { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(5)]
        public string ZipCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(4)]
        public string PlusFour { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(2)]
        public string DeliveryPoint { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string StreetName { get; set; }

        [DataMember]
        public long? SmartBoardId { get; set; }

        /// <summary>
        /// Used to store Property Notes.
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        #endregion

        #region Navigation

        [DataMember]
        [AttachOnUpdate]
        [InverseProperty("Property")]
        public ICollection<Occupant> Occupants { get; set; }

        [DataMember]
        [AttachOnUpdate]
        [InverseProperty("Property")]
        public ICollection<Field> PropertyBag { get; set; }

        [DataMember]
        [AttachOnUpdate]
        [InverseProperty("Property")]
        public ICollection<PropertyAttribute> Attributes { get; set; }

        #endregion

        public Occupant GetMainOccupant()
        {
            int occupantsCount = Occupants?.Count ?? 0;
            if (occupantsCount > 0)
            {
                var mainOccupant = Occupants.First();
                if (occupantsCount == 1)
                {
                    if (String.IsNullOrEmpty(mainOccupant.FirstName + mainOccupant.LastName))
                    {
                        mainOccupant = null;
                    }
                }
                else
                {
                    mainOccupant = Occupants.FirstOrDefault(o => String.Format("{0} {1}", o.FirstName, o.LastName).ToUpperInvariant() == Name.ToUpperInvariant() && o.IsDeleted == false);
                    if (mainOccupant == null)
                    {
                        mainOccupant = Occupants.FirstOrDefault(o => !String.IsNullOrEmpty(o.FirstName) && Name.Contains(o.FirstName) && !String.IsNullOrEmpty(o.LastName) && Name.Contains(o.LastName) && o.IsDeleted == false);
                    }
                    if (mainOccupant == null)
                    {
                        mainOccupant = Occupants.OrderBy(o => o.LastName).ThenBy(o => o.FirstName).First();
                    }
                }
                return mainOccupant;
            }
            var propOccupantName = Name.FirstAndLastName();

            return new Occupant
            {
                FirstName = propOccupantName.Item1,
                LastName = propOccupantName.Item2
            };
        }

        public string GetMainEmailAddress()
        {
            return GetMainPropertyBagValue(@"Email Address");
        }

        public string GetMainPhoneNumber()
        {
            return GetMainPropertyBagValue(@"Phone Number");
        }

        private string GetMainPropertyBagValue(string propertyBagItemName)
        {
            var emailAddresses = PropertyBag?.Where(pb => pb.DisplayName == propertyBagItemName)?.ToList();
            if (emailAddresses != null && emailAddresses.Any())
            {
                var mainEmailAddress = emailAddresses.FirstOrDefault(pb => pb.Flags == 1);
                if (mainEmailAddress != null) return mainEmailAddress.Value;
                return emailAddresses.First().Value;
            }
            return null;
        }
    }

    [DataContract]
    [NotMapped]
    public class PropertySaveResultPayload
    {


        [DataMember]
        public Guid? AppointmentID { get; set; }

        [DataMember]
        public string GoogleEventID { get; set; }
    }
}
