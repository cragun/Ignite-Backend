using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes
{
    public class OUSettingsViewModel
    {
        public OUSettingsViewModel()
        {

        }

        

        public string OUName { get; set; }

        /// <summary>
        /// The Guid of the Parent OU
        /// </summary>
        public Guid ParentOUID { get; set; }

        public Guid OUID { get; set; }

        public List<String> States { get; set; }

        public string OwnerFirstName { get; set; }

        public string OwnerLastName { get; set; }

        [EmailAddress]
        public string OwnerEmail { get; set; }

        public List<Guid> SelectedPanels { get; set; }

        public List<Guid> SelectedInverters { get; set; }

        public int DefaultLosses { get; set; }

        public double DefaultUtilityInflationRate { get; set; }

        public double DefaultSystemDegradation { get; set; }

        public List<Guid> SelectedFinancePlans { get; set; }

        public bool EnablePreQualfication { get; set; }

        public bool EnableProposals { get; set; }

        public bool EnableOrganizationSettings { get; set; }

        public OffsetDataView DefaultOffsets { get; set; }

        public Guid ProposalTemplateID { get; set; }

        public int DefaultRoofTilt { get; set; }

       

    }
}