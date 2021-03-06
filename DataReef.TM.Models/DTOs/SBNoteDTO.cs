using DataReef.TM.Models.DTOs.FinanceAdapters;
using DataReef.TM.Models.DTOs.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs
{

    public class SBNoteData
    {
         
        public string apiKey { get; set; }

        public long? LeadID { get; set; }

        public Guid PropertyID { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string userId { get; set; }

        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public DateTime DateCreated { get; set; }

    }

    public class SBNoteDTO
    {
        public string Action { get; set; }

        public Guid? Guid { get; set; }

        public Guid PropertyID { get; set; }
        public Guid PersonID { get; set; }

        public Guid ParentID { get; set; }
        public string ContentType { get; set; }
        public string Attachments { get; set; }

        /// <summary>
        /// You may send the Ignite ID, as a backup if LeadID is not saved in Ignite.
        /// </summary>
        /// 


        public string APIKey { get; set; }

        public long? IgniteID { get; set; }

        public long? LeadID { get; set; }

        public string Content { get; set; }



        public string UserID { get; set; }

        public string Email { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateLastModified { get; set; }
        public int? Count { get; set; }
        public DateTime? LastUpdateTime { get; set; }
        public bool IsSendSMS { get; set; }
        public bool IsSendEmail { get; set; }
        public List<SBTaggedUser> TaggedUsers { get; set; }
        public IEnumerable<PropertyNoteTagDTO> ContentTags { get; set; }
         
        public string JobNimbusID { get; set; } 
        public string JobNimbusLeadID { get; set; } 
        public int Version { get; set; } 
        public ThirdPartyPropertyType PropertyType { get; set; }

        public SBNoteDTO()
        {

        }
         
        public SBNoteDTO(PropertyNote note, Property property, string userID)
        {
            if (note != null)
            {
                Guid = note.Guid;
                Content = note.Content;
                UserID = userID;
                DateCreated = note.DateCreated;
                DateLastModified = note.DateLastModified;
            }

            if (property != null)
            {
                PropertyID = property.Guid;
                LeadID = property.SmartBoardId;
                IgniteID = property.Id;
            }
        }

    }

    public class SBTaggedUser
    {
        public string email { get; set; }
        public bool IsSendEmail { get; set; }
        public bool IsSendSMS { get; set; }
        public int SmartBoardId { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    } 

    public class Territories
    {
        public Guid TerritoryId { get; set; }
        public string Name { get; set; }
    }

    public class TerritoryApikey
    {
        public Guid TerritoryId { get; set; }
        public string Name { get; set; }
        public string Apikey { get; set; }
    }

    public class TerritoryModel
    {
        public string apikey { get; set; }
        public IEnumerable<Territories> TerritorieswithLatLong { get; set; }
        public IEnumerable<Territories> Territories { get; set; }
    }



    public class zapierOus
    {
        public Guid Ouid { get; set; }
        public string Name { get; set; }
    }


    public class zapierOusModel
    {
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }

        public IEnumerable<zapierOus> ouslist { get; set; }
    }

    public class SBUpdateProperty
    {
        public Guid PropertyId { get; set; }
        public long? LeadId { get; set; }
        public string apiKey { get; set; }
    }
}
