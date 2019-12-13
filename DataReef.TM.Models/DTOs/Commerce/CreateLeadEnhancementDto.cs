using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Commerce
{
    public class CreateLeadEnhancementDto
    {
        public List<UploadedLeadDto> UploadedLeads { get; set; }
    }

    public class UploadedLeadDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public double JanuaryConsumption { get; set; }
        public double FebruaryConsumption { get; set; }
        public double MarchConsumption { get; set; }
        public double AprilConsumption { get; set; }
        public double MayConsumption { get; set; }
        public double JuneConsumption { get; set; }
        public double JulyConsumption { get; set; }
        public double AugustConsumption { get; set; }
        public double SeptemberConsumption { get; set; }
        public double OctoberConsumption { get; set; }
        public double NovemberConsumption { get; set; }
        public double DecemberConsumption { get; set; }
        public string Identifier { get; set; }

    }
}
