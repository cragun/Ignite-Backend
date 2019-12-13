using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.Genability
{
    public class TariffResponseModel
    {
        public string TariffID       { get; set; }

        public string MasterTariffID { get; set; }

        public string TariffCode     { get; set; }

        public string TariffName     { get; set; }

        public string UtilityID      { get; set; }

        public string UtilityName    { get; set; }

        public string PricePerKWH    { get; set; }
    }
}