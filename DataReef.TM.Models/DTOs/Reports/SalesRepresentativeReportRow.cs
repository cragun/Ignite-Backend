using DataReef.TM.Models.DataViews.Inquiries;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Reports
{
    public class SalesRepresentativeReportRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<InquiryStatisticsForPerson> InquiryStatistics { get; set; }
    }
}
