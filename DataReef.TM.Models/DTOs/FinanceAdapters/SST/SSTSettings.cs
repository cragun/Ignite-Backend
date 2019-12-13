using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SST
{
    public class SSTSettings
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string Enabled { get; set; }
        public string ApiKey { get; set; }
        public string EmailAddress { get; set; }

    }
}
