using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    public class PriceResult
    {
        public string TariffID       { get; set; }

        public string MasterTariffID { get; set; }

        public string TariffCode     { get; set; }

        public string TariffName     { get; set; }

        public decimal PricePerKWH   { get; set; }

        public string UtilityID      { get; set; }

        public string UtilityName    { get; set; }

        public Nullable<bool> UsageCollected { get; set; }
    }
}
