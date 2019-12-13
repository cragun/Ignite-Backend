using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs
{

    public class SBNoteDTO
    {
        public Guid? Guid { get; set; }

        public Guid PropertyID { get; set; }
        
        /// <summary>
        /// You may send the Ignite ID, as a backup if LeadID is not saved in Ignite.
        /// </summary>
        public long? IgniteID { get; set; }

        public long? LeadID { get; set; }

        public string Content { get; set; }

        public string UserID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateLastModified { get; set; }

        public SBNoteDTO()
        {

        }

        public SBNoteDTO(PropertyNote note, Property property, string userID)
        {
            if(note != null)
            {
                Guid = note.Guid;
                Content = note.Content;
                UserID = userID;
                DateCreated = note.DateCreated;
                DateLastModified = note.DateLastModified;
            }

            if(property != null)
            {
                PropertyID = property.Guid;
                LeadID = property.SmartBoardId;
                IgniteID = property.Id;
            }
        }
    }
}
