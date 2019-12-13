using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalSolution
    {
        public int PanelsCount { get; set; }
        public int InvertersCount { get; set; }
        public double EstimatedSize { get; set; }
        public double AverageAnnualProduction { get; set; }
        public double FirstYearProduction { get; set; }

        public int Shading { get; set; }
        public int Orientation { get; set; }
        public int Slope { get; set; }
        public double SystemSizeDC { get; set; }
        public double SystemSizeAC { get; set; }

        public int PanelWatts { get; set; }
        public string PanelType { get; set; }
        public string InverterType { get; set; }
        public double DerateFactor { get; set; }
        public double AnnualSystemDegradation { get; set; }

        public ICollection<ProposalSolutionPerArray> RoofPlanes { get; set; }

        public ProposalSolution() { }

        public ProposalSolution(Proposal proposal, FinancePlan financePlan, LoanRequest request, LoanResponse response)
        {
            if (financePlan == null)
            {
                return;
            }

            PanelsCount = proposal.SolarSystem.PanelCount;
            InvertersCount = proposal.SolarSystem.GetInvertersCount();
            
            // TODO: Find out where DC / AC data should come from
            SystemSizeDC = SystemSizeAC = EstimatedSize = request.SystemSize;
            AverageAnnualProduction = request.ScenarioTermInYears == 0 ? 0 : ((double)response.TotalSystemProduction / request.ScenarioTermInYears);
            FirstYearProduction = (double)(financePlan.FinancePlanType == Enums.FinancePlanType.Lease ? request.LeaseProduction : request.FirstYearElectricityProduction);

            var roofPlanes = proposal
                                .SolarSystem?
                                .RoofPlanes?
                                .OrderByDescending(rp => rp.PanelsCount);

            RoofPlanes = roofPlanes
                ?.Select(r => new ProposalSolutionPerArray
                {
                    Orientation = r.Azimuth,
                    Shading = r.Shading,
                    Slope = (int)r.Tilt,
                    PanelType = r.SolarPanel?.Name,
                    PanelWatts = r.SolarPanel?.Watts ?? 0,
                    InverterType = r.Inverter?.Name
                })?.ToList();

            var roofPlane = roofPlanes?.FirstOrDefault();
            if (roofPlane != null)
            {
                Shading = roofPlane.Shading;
                Orientation = roofPlane.Azimuth;
                Slope = (int)roofPlane.Tilt;
                PanelType = roofPlane.SolarPanel?.Name;
                PanelWatts = roofPlane.SolarPanel?.Watts ?? 0;
                InverterType = roofPlane.Inverter?.Name;
                // TODO: check difference between AnnualSystemDegradation & DerateFactor 
                AnnualSystemDegradation = DerateFactor = request.Derate * 100;
            }
        }

        public class ProposalSolutionPerArray
        {
            public int Shading { get; set; }
            public int Orientation { get; set; }
            public int Slope { get; set; }
            public int PanelWatts { get; set; }
            public string PanelType { get; set; }
            public string InverterType { get; set; }
        }
    }
}
