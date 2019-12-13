using DataReef.Core.Common;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
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
    public class SBAppointmentDTO
    {
        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Details { get; set; }

        [DataMember]
        [JsonConverter(typeof(CustomJsonDateFormatConverter), "yyyy-MM-dd'T'HH:mm:ss")]
        public DateTime StartDate { get; set; }

        [DataMember]
        [JsonConverter(typeof(CustomJsonDateFormatConverter), "yyyy-MM-dd'T'HH:mm:ss")]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public string GoogleEventID { get; set; }

        [DataMember]
        public string TimeZone { get; set; }

        [DataMember]
        public AppointmentStatus Status { get; set; }

        public SBAppointmentDTO(Appointment app)
        {
            if(app != null)
            {
                Guid = app.Guid;
                Name = app.Name;
                Details = app.Details;
                StartDate = app.StartDate;
                EndDate = app.EndDate;
                GoogleEventID = app.GoogleEventID;
                TimeZone = app.TimeZone;
                Status = app.Status;
            }
        }
    }
}
