using System.Collections.Generic;
using System.Runtime.Serialization;
using DataReef.Core.Attributes;

namespace DataReef.TM.Models.Sync
{
    //TODO: MAJOR - Define initial package and define Streamed entities
    [KnownType(typeof(EmployeeDataPacket))]
    [KnownType(typeof(CenterDataPacket))]
    [DataContract]
    public class BaseDataPacket
    {

    
        [DataMember]
        public ICollection<Address> Addresses { get; set; }

     

        [DataMember]
        [Streamed(100, "DateCreated")]
        public ICollection<Attachment> Attachments { get; set; }

    

     
        [DataMember]
        public ICollection<Identification> Identifications { get; set; }

     

        [DataMember]
        [Streamed(200, "DateCreated")]
        public ICollection<Note> Notes { get; set; }

        [DataMember]
        public ICollection<OU> OUS { get; set; }

        [DataMember]
        public ICollection<PersonalConnection> PersonalConnections { get; set; }

        [DataMember]
        public ICollection<Person> People { get; set; }

        [DataMember]
        public ICollection<PhoneNumber> PhoneNumbers { get; set; }

       
       
       


    }
}
