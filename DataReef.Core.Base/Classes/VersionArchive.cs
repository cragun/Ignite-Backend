using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DataReef.Core.Enums;

namespace DataReef.Core.Classes
{
    public class VersionArchive
    {

        /// <summary>
        /// Identity field for server objects
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int RowID { get; set; }

        /// <summary>
        /// Guid of version (when saved to server)
        /// </summary>
        [Index(name: "idx_archive_main", order: 1)]
        [DataMember]
        public Guid ObjectGuid { get; set; }

        /// <summary>
        /// The .net type of the object
        /// </summary>
        /// <example>DataReef.Models.Person</example>
        [Index(name:"idx_archive_main", order:0)]
        [StringLength(100)]
        [DataMember]
        public string ObjectType { get; set; }

        /// <summary>
        /// Latest version of the object (post change)
        /// </summary>
        [Index(name: "idx_archive_main", order: 2)]
        [DataMember]
        public int Version { get; set; }

        /// <summary>
        /// The date the change was created on the server
        /// </summary>
        [DataMember]
        public System.DateTime DateCreated { get; set; }

        /// <summary>
        /// UserID (GUID) of the user that caused the change
        /// </summary>
        [DataMember]
        public Guid? UserID { get; set; }

        /// <summary>
        /// UserName of the user that caused the change
        /// </summary>
        [DataMember]
        [StringLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// An internal GUID of the device
        /// </summary>
        [DataMember]
        public Guid UserDeviceID { get; set; }

        /// <summary>
        /// The JSON represensation of the object
        /// </summary>
        [DataMember]
        public string Json { get; set; }

        /// <summary>
        /// the DataAction (Insert,Update,Delete) that caused the version to change
        /// </summary>
        [DataMember]
        public DataAction DataAction { get; set; }
    

    }
}