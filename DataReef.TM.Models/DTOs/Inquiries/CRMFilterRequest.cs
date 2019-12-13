using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Inquiries
{
    public class CRMFilterRequest
    {
        public string Query { get; set; }
        public string Disposition { get; set; }
        public string SortColumn { get; set; } = "DateCreated";
        public bool SortAscending { get; set; }
        public string Include { get; set; }
        public string Exclude { get; set; }
        public string Fields { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 100;
        public Guid? PersonID { get; set; }

        public CRMFilterAppointment AppointmentQuery { get; set; }
        public IEnumerable<string> DispositionsQuery { get; set; }
        public IEnumerable<Guid> AppointmentAssignedIds { get; set; }
        public IEnumerable<Guid> AppointmentCreatedIds { get; set; }
        public IEnumerable<Guid> OUIds { get; set; }
        public IEnumerable<Guid> TerritoryIds { get; set; }

        public bool HasInclude(string value)
        {
            return Include?.Split(",|;".ToCharArray())?.Contains(value, StringComparer.OrdinalIgnoreCase) == true;
        }
    }
    

    public class CRMFilterAppointment
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public DateTime? Date { get; set; }
    }
}
