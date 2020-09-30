using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataReef.TM.Models
{

    [XmlRoot(ElementName = "root")]
    public class EsIDResponse
    {
        [XmlElement(ElementName = "row")]
        public List<EsIDRow> Row { get; set; }
    }

    public class EsIDRow
    {
        [XmlElement(ElementName = "esiid")]
        public string Esiid { get; set; }
        [XmlElement(ElementName = "address")]
        public string Address { get; set; }
        [XmlElement(ElementName = "address_2")]
        public string Address_2 { get; set; }
        [XmlElement(ElementName = "city")]
        public string City { get; set; }
        [XmlElement(ElementName = "state")]
        public string State { get; set; }
        [XmlElement(ElementName = "zip")]
        public string Zip { get; set; }
        [XmlElement(ElementName = "plus4")]
        public string Plus4 { get; set; }
        [XmlElement(ElementName = "read_day")]
        public string Read_day { get; set; }
        [XmlElement(ElementName = "premise_type")]
        public string Premise_type { get; set; }
        [XmlElement(ElementName = "metered")]
        public string Metered { get; set; }
        [XmlElement(ElementName = "energized")]
        public string Energized { get; set; }
        [XmlElement(ElementName = "power_region")]
        public string Power_region { get; set; }
        [XmlElement(ElementName = "status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "stationcode")]
        public string Stationcode { get; set; }
        [XmlElement(ElementName = "stationname")]
        public string Stationname { get; set; }
        [XmlElement(ElementName = "csv_file")]
        public string Csv_file { get; set; }
        [XmlElement(ElementName = "full_status")]
        public string Full_status { get; set; }
        [XmlElement(ElementName = "tdsp_duns")]
        public string Tdsp_duns { get; set; }
        [XmlElement(ElementName = "polr_customer_class")]
        public string Polr_customer_class { get; set; }
        [XmlElement(ElementName = "ams_meter_flag")]
        public string Ams_meter_flag { get; set; }
        [XmlElement(ElementName = "tdsp_name")]
        public string Tdsp_name { get; set; }
        [XmlElement(ElementName = "trans_count")]
        public string Trans_count { get; set; }
        [XmlElement(ElementName = "service_orders")]
        public string Service_orders { get; set; }
        [XmlElement(ElementName = "cmz")]
        public string Cmz { get; set; }
        [XmlElement(ElementName = "tdu")]
        public string Tdu { get; set; }
    }

   
}
