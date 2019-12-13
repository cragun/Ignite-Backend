using DataReef.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Geo
{
    [Table("Occupants")]
    public class Occupant : EntityBase
    {
        [DataMember]
        [StringLength(50)]
        public string FirstName { get; set; }

        [DataMember]
        [StringLength(2)]
        public string MiddleInitial { get; set; }

        [DataMember]
        [StringLength(50)]
        public string LastName { get; set; }

        [DataMember]
        [StringLength(3)]
        public string LastNameSuffix { get; set; }

        [DataMember]
        public Guid PropertyID { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("PropertyID")]
        public Property Property { get; set; }

        [DataMember]
        [AttachOnUpdate]
        [InverseProperty("Occupant")]
        public ICollection<Field> PropertyBag { get; set; }

        #endregion

        public override void PrepareNavigationProperties(Guid? createdById = null)
        {
            base.PrepareNavigationProperties();

            if (PropertyBag != null)
            {
                foreach (var item in PropertyBag)
                {
                    item.OccupantId = Guid;
                    if(!item.CreatedByID.HasValue && createdById.HasValue)
                    {
                        item.CreatedByID = createdById;
                    }
                }
            }
        }        
    }
}
