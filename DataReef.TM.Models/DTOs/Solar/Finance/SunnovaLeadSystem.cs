using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class SunnovaLeadSystem
    {
        public LeadSystem Lead_System { get; set; }  
    }

    public class LeadSystem
    {
        public string Id { get; set; }
        public string Lead_Id { get; set; }
        public object External_Id { get; set; }
        public string Name { get; set; }
        public double Size_Kwh { get; set; }
        public int Lead_Array_Count { get; set; }
        public object Design_Notes { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime Last_Modified_Date { get; set; }
        public List<LeadArray> Lead_Arrays { get; set; }
        public List<object> System_Add_Ons { get; set; }
        public object Files { get; set; }
    }

    public class LeadArray
    {
        public string Id { get; set; }
        public object External_Id { get; set; }
        public string Name { get; set; }
        public double Size_Kwh { get; set; }
        public int Tilt { get; set; }
        public int Azimuth { get; set; }
        public int Shading_Coefficient { get; set; }
        public string Module_Manufacturer_Id { get; set; }
        public string Module_Manufacturer { get; set; }
        public string Module_Model_Id { get; set; }
        public string Module_Model { get; set; }
        public int Module_Quantity { get; set; }
        public string Inverter_Manufacturer_Id { get; set; }
        public string Inverter_Manufacturer { get; set; }
        public string Inverter_Model_Id { get; set; }
        public string Inverter_Model { get; set; }
        public int Inverter_Quantity { get; set; }
        public string Monitor_Manufacturer_Id { get; set; }
        public string Monitor_Manufacturer { get; set; }
        public string Monitor_Model_Id { get; set; }
        public string Monitor_Model { get; set; }
        public string Racking_Manufacturer_Id { get; set; }
        public string Racking_Manufacturer { get; set; }
        public object Other_Model_Id { get; set; }
        public object Other { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime Last_Modified_Date { get; set; }
    } 
}
