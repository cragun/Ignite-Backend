using DataReef.Integrations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Agemni
{
    public class AgemniLookupData:ILookupData
    {

        public AgemniLookupData()
        {
            this.ProtectionPackages = ProtectionPackage.AvailablePackages;
            this.Equipments = Equipment.AvailableEquipment;
            this.OtherEquipments = OtherEquipment.AvailableEquipment;
            this.SportsPackages = SportsPackage.AvailablePackages;
            this.SpanishAddons = SpanishAddOn.AvailableAddOns;
            this.SpanishPackages = BasePackage.AvailableSpanishPackages;
            this.EnglishPackages = BasePackage.AvailableEnglishPackages;
            this.InternationalPrograms = InternationalProgram.AvailablePrograms;
            this.DirecTVAddOns = DirecTVAddOn.AvailableAddOns;
            this.PremiumAddOns = PremiumAddOn.AvailableAddOns;

        }
     
        public ICollection<ProtectionPackage> ProtectionPackages { get; set; }

        public ICollection<Equipment> Equipments { get; set; }

        public ICollection<OtherEquipment> OtherEquipments { get; set; }

        public ICollection<SportsPackage> SportsPackages { get; set; }

        public ICollection<SpanishAddOn> SpanishAddons { get; set; }

        public ICollection<BasePackage> EnglishPackages { get; set; }

        public ICollection<BasePackage> SpanishPackages { get; set; }

        public ICollection<InternationalProgram> InternationalPrograms { get; set; }

        public ICollection<DirecTVAddOn> DirecTVAddOns { get; set; }

        public ICollection<PremiumAddOn> PremiumAddOns { get; set; }




    }
}
