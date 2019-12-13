using DataReef.TM.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataReef.TM.Models.DTOs.Persons
{
    [NotMapped]
    public class PersonDTO : Person
    {
        public OURoleType RoleType { get; set; }

        public PermissionType PermissionType { get; set; }
    }
}
