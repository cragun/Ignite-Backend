using DataReef.Core.Attributes;
using DataReef.TM.Models.DataViews.Solar.Proposal;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("Systems", Schema = "solar")]
    public class SolarSystem : EntityBase
    {
        /// <summary>
        /// units in WATTS
        /// </summary>
        [DataMember]
        public int SystemSize { get; set; }

        [DataMember]
        public int PanelCount { get; set; }

        [DataMember]
        public bool ApplyConsumptionSlope { get; set; }

        [DataMember]
        public double? EscalationRate { get; set; }

        [DataMember]
        public bool IsPPAPricingB { get; set; }

        [DataMember]
        public float GridRotation { get; set; }

        [DataMember]
        public int GridOffsetX { get; set; }

        [DataMember]
        public int GridOffsetY { get; set; }

        [DataMember]
        public string SystemJSON { get; set; }

        [DataMember]
        public string FinancePlansSettingsJSON { get; set; }

        [DataMember]
        public string PowerMetaDataJSON { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("Guid")]
        public Proposal Proposal { get; set; }

        [DataMember]
        [AttachOnUpdate]
        public ICollection<SolarSystemPowerConsumption> PowerConsumption { get; set; }

        [DataMember]
        public ICollection<RoofPlane> RoofPlanes { get; set; }

        [DataMember]
        [AttachOnUpdate]
        [InverseProperty(nameof(FinancePlan.SolarSystem))]
        public ICollection<FinancePlan> FinancePlans { get; set; }

        [DataMember]
        [ForeignKey("Guid")]
        public SystemProduction SystemProduction { get; set; }



        [DataMember]
        [InverseProperty(nameof(AdderItem.SolarSystem))]
        public ICollection<AdderItem> AdderItems { get; set; }

        #endregion

        public void ValidateSystemValid()
        {
            if (RoofPlanes == null || !RoofPlanes.Any()) return;
            var totaPanelsCount = RoofPlanes.Sum(rp => rp.PanelsCount);
            if (totaPanelsCount != PanelCount) throw new ApplicationException("PanelCount is not equal to the sum of panels on each roof plane!");
            var isSimple = RoofPlanes.Any(rp => rp.Name.Contains("Simple System"));
            if(!isSimple)
            {
                var isNotValid = RoofPlanes.Any(rp => !rp.IsValid);
                if (isNotValid) throw new ApplicationException("There are roof planes that have no points!");
            }            
        }


        public string GetEquipmentInfo()
        {
            string result = "";
            var roofPlanesDictionary = new Dictionary<string, int>();
            var invertersDictionary = new Dictionary<string, int>();

            foreach (var roofPlane in RoofPlanes)
            {
                if (roofPlane.PanelsCount <= 0) continue;

                // solar panels
                var solarPanel = roofPlane.SolarPanel;
                var roofPlanesDictionaryKey = $"{solarPanel.Name} {solarPanel.Description} [{solarPanel.Watts} W]";
                var solarPanelsCount = roofPlanesDictionary.ContainsKey(roofPlanesDictionaryKey) ? roofPlanesDictionary[roofPlanesDictionaryKey] : 0;
                solarPanelsCount += roofPlane.PanelsCount;
                roofPlanesDictionary[roofPlanesDictionaryKey] = solarPanelsCount;

                // inverters
                var inverter = roofPlane.Inverter;
                var invertersDictionaryKey = inverter.Manufacturer;
                var invertersCount = invertersDictionary.ContainsKey(invertersDictionaryKey) ? invertersDictionary[invertersDictionaryKey] : 0;
                if (inverter.IsMicroInverter)
                {
                    invertersCount += roofPlane.PanelsCount;
                }
                else
                {
                    invertersCount++;
                }
                invertersDictionary[invertersDictionaryKey] = invertersCount;
            }

            result += String.Join(", ", roofPlanesDictionary.Select(d => string.Format("{0} {1}", d.Value, d.Key)));
            result += " | ";
            result += String.Join(", ", invertersDictionary.Select(d => d.Value == 1 ? d.Key : string.Format("{0} {1}", d.Value, d.Key)));

            return result;
        }

        public int GetInvertersCount()
        {
            return RoofPlanes?
                    .Where(rp => rp.PanelsCount > 0
                              && rp.Inverter != null)?
                    .Select(rp => rp.Inverter.IsMicroInverter == true ? rp.PanelsCount : 1)?
                    .Sum() ?? 0;
        }

        public SolarPanel GetSolarPanel()
        {
            return RoofPlanes?
                    .FirstOrDefault(rp => rp.SolarPanel != null)?
                    .SolarPanel;
        }

        public Inverter GetInverter()
        {
            return RoofPlanes?
                    .FirstOrDefault(rp => rp.Inverter != null)?
                    .Inverter;
        }


        public List<Tuple<SolarPanel, List<RoofPlanePanel>>> GetPanels()
        {
            return RoofPlanes?
                        .Where(rp => rp.SolarPanel != null)
                        .Select(r => new Tuple<SolarPanel, List<RoofPlanePanel>>(r.SolarPanel, r.Panels?.ToList()))?
                        .ToList();
        }


        public SolarSystem Clone(Guid proposalID, CloneSettings cloneSettings)
        {

            if (this.AdderItems == null) throw new MissingMemberException("Missing System.AdderItems in Object Graph");
            if (this.RoofPlanes == null) throw new MissingMemberException("Missing System.RoofPlanes in Object Graph");
            if (this.FinancePlans == null) throw new MissingMemberException("Missing System.FinancePlans in Object Graph");
            if (this.PowerConsumption == null) throw new MissingMemberException("Missing System.PowerConsumption in Object Graph");

            SolarSystem ret = (SolarSystem)this.MemberwiseClone();
            ret.Reset();
            ret.Guid = proposalID;
            ret.Proposal = null;
            ret.AdderItems = AdderItems
                                .Select(ai => ai.Clone(ret.Guid, cloneSettings))
                                .ToList();

            ret.RoofPlanes = RoofPlanes
                                .Select(rp => rp.Clone(ret.Guid, cloneSettings))
                                .ToList();

            ret.FinancePlans = FinancePlans
                                .Select(fp => fp.Clone(ret.Guid, cloneSettings))
                                .ToList();

            ret.PowerConsumption = PowerConsumption
                            .Select(pc => pc.Clone(ret.Guid, cloneSettings))
                            .ToList();

            ret.SystemProduction = SystemProduction?.Clone(ret.Guid, cloneSettings);

            return ret;
        }

        private PowerMetaDataDataView _powerMetaData;
        public PowerMetaDataDataView GetPowerMetaData()
        {
            if (_powerMetaData == null && !string.IsNullOrWhiteSpace(PowerMetaDataJSON))
            {
                try
                {
                    _powerMetaData = JsonConvert.DeserializeObject<PowerMetaDataDataView>(PowerMetaDataJSON);
                }
                catch { }
            }
            return _powerMetaData;
        }

    }
}
