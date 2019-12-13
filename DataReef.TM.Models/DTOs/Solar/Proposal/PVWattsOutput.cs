
namespace DataReef.TM.Models.DTOs.Solar.Proposal
{
    public class PVWattsOutput
    {
        public decimal[] ac_monthly { get; set; }
        public decimal[] dc_monthly { get; set; }
        public decimal[] poa_monthly { get; set; }
        public decimal[] solrad_monthly { get; set; }
        public decimal ac_annual { get; set; }
        public decimal solrad_annual { get; set; }
    }
}
