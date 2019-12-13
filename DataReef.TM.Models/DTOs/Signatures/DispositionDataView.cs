using System;
using DataReef.Core.Extensions;

namespace DataReef.TM.Models.DTOs.Signatures
{
    /// <summary>
    /// Disposition
    /// </summary>
    public class DispositionDataView
    {
        public DispositionDataView(Inquiry inquiry)
        {
            if (inquiry == null) return;

            Property = new PropertyDataView(inquiry.Property);
            ActivityType = inquiry.ActivityType.SplitCamelCase();
            Disposition = inquiry.Disposition;
            Notes = inquiry.Notes;
            FollowUpDate = inquiry.FollowUpDate;
            ShouldIntegrateWithCalendar = inquiry.ShouldIntegrateWithCalendar;
            Color = inquiry.Color;
            Annotation = inquiry.Annotation;
            OUID = inquiry.OUID;
            IsArchive = inquiry.IsArchive;
            IsLead = inquiry.IsLead;
            Lat = inquiry.Lat;
            Lon = inquiry.Lon;
        }

        public PropertyDataView Property { get; set; }

        public string ActivityType { get; set; }

        public string Disposition { get; set; }

        public string Notes { get; set; }

        public DateTime? FollowUpDate { get; set; }

        public bool? ShouldIntegrateWithCalendar { get; set; }

        public string Color { get; set; }

        public string Annotation { get; set; }

        public Guid? OUID { get; set; }

        /// <summary>
        ///     When double knocking, all inquiries are Archived before starting the double knock.  So we know which
        /// </summary>
        public bool IsArchive { get; set; }

        /// <summary>
        ///     this person was interested and should be followed
        /// </summary>
        public bool IsLead { get; set; }


        /// <summary>
        ///     Longitude at the time of the inquiry
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        ///     Latitude at the time of the inquiry
        /// </summary>
        public double? Lon { get; set; }
    }
}
