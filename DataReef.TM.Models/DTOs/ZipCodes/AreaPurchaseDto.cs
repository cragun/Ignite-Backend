using System;

namespace DataReef.TM.Models.DTOs.ZipCodes
{
    public class AreaPurchaseDto
    {
        public Guid Guid { get; set; }
        public string OUName { get; set; }
        public Guid OUID { get; set; }
        public string BuyerName { get; set; }
        public string ZipAreaName { get; set; }
        public int NumberOfTokens { get; set; }
        public float TokenPriceInDollars { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
