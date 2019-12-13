
namespace DataReef.TM.Models.Solar
{
    public class CustomUtilityRate
    {
        /// <summary>
        /// This can be either the Id or the name of the utility provider
        /// </summary>
        public string UtilityProvider { get; set; }

        public decimal UtilityRate { get; set; }

        public decimal UtilityBaseFee { get; set; }
    }
}
