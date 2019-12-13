namespace DataReef.TM.Models.DTOs.Mortgage
{
    public class MortgageRequest
    {
        public string State { get; set; }

        public string HouseNumber { get; set; }

        public string ZipCode { get; set; }

        public string ZipPlusFour { get; set; }
    }
}
