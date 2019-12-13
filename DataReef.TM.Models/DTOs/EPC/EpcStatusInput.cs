using System;
using DataReef.TM.Models.Enums;

namespace DataReef.TM.Models.DTOs.EPC
{
    public class EpcStatusInput
    {
        public Guid PropertyID { get; set; }

        public string StatusName { get; set; }

        public Guid SalesRepID { get; set; }

        public EpcStatusProgress StatusProgress { get; set; }
    }
}
