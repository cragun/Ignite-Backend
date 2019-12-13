using System.Xml.Serialization;

namespace DataReef.Integrations.RedBell
{
    [XmlRoot("AVE")]
    public class Ave
    {
        public int OrderId { get; set; }

        public int OrderSid { get; set; }

        public decimal AveValue { get; set; }
    }
}