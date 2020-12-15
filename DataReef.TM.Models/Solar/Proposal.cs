using DataReef.TM.Models.DTOs.Proposals;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("Proposals", Schema = "solar")]
    public class Proposal : EntityBase
    {
        [DataMember]
        public ProposalStatus Status { get; set; }

        [NotMapped]
        public string SBProposalError { get; set; }

        /// <summary>
        /// Person who owns this proposal, who is making the proposal
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public string NameOfOwner { get; set; }

        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// The provider profile id for Electricity Usage Profile (="ELECTRICITY_" + Proposal.Guid)
        /// </summary>
        [DataMember]
        public string GenabilityElectricityProviderProfileID { get; set; }

        /// <summary>
        /// Guid to link the proposal to a property in a territory
        /// </summary>
        [DataMember]
        public Guid PropertyID { get; set; }

        [DataMember]
        public Guid? OUID { get; set; }

        /// <summary>
        /// The Guid of the address from the Geo Server
        /// </summary>
        [DataMember]
        public Guid? AddressID { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string PlusFour { get; set; }

        [DataMember]
        public double Lat { get; set; }

        [DataMember]
        public double Lon { get; set; }

        [DataMember]
        public System.DateTime? IntegrationDate { get; set; }

        [DataMember]
        public float SalesTax { get; set; }

        [DataMember]
        public ProposalImagerySource ImagerySource { get; set; }

        [DataMember]
        public string ProposalURL { get; set; }

        /// <summary>
        /// An array of <see cref="SignedDocumentDTO"/> that stores signed documents name and url.
        /// </summary>
        [DataMember]
        public string SignedDocumentsJSON { get; set; }

        [DataMember]
        public ProposalDesignSystemType DesignSystemType { get { return ProposalDesignSystemType.AdvancedDrawing; } set { } }

        /// <summary>
        /// Property used to store custom information (as JSON) related to the proposal.
        /// (e.g. a custom PricePerKW set by the rep to override Panels price per kw)
        /// </summary>
        [DataMember]
        public string MetaInfo { get; set; }

        [DataMember]
        public decimal? ZoomLevel { get; set; }

        [DataMember]
        public ProposalCreditStatus CreditStatus { get; set; }

        [DataMember]
        public double TotalBill { get; set; }

        [DataMember]
        public double TotalKWH { get; set; }

        [DataMember]
        public double ProductionKWH { get; set; }

        [DataMember]
        public double ProductionKWHpercentage { get; set; }

        [DataMember]
        public bool IsManual { get; set; }

        [DataMember]
        public double SystemSize { get; set; }




        #region Navigation properties

        [ForeignKey("PropertyID")]
        [DataMember]
        public Property Property { get; set; }

        [ForeignKey("Guid")]
        [DataMember]
        public SolarSystem SolarSystem { get; set; }

        [ForeignKey("Guid")]
        [DataMember]
        public SolarTariff Tariff { get; set; }

        /// <summary>
        /// Sales rep who made the proposal, based on PersonID
        /// </summary>
        [DataMember]
        [NotMapped]
        public Person SalesRep { get; set; }

        [DataMember]
        [InverseProperty(nameof(ProposalMediaItem.Proposal))]
        public ICollection<ProposalMediaItem> MediaItems { get; set; }

        #endregion

        public override void PrepareNavigationProperties(Guid? createdById = null)
        {
            if (this.Guid == null || this.Guid == Guid.Empty) this.Guid = Guid.NewGuid();

            if (Tariff != null)
            {
                Tariff.Guid = this.Guid;
                Tariff.Proposal = this;
            }

            if (SolarSystem == null) return;

            SolarSystem.Guid = this.Guid;
            SolarSystem.Proposal = this;

            if (SolarSystem.PowerConsumption != null && SolarSystem.PowerConsumption.Any())
            {
                foreach (var item in SolarSystem.PowerConsumption)
                {
                    item.SolarSystemID = this.Guid;
                    item.SolarSystem = SolarSystem;
                }
            }

            if (SolarSystem.AdderItems != null && SolarSystem.AdderItems.Any())
            {
                foreach (var adderItem in SolarSystem.AdderItems)
                {

                    if (adderItem.Guid == Guid.Empty) adderItem.Guid = Guid.NewGuid();

                    adderItem.SolarSystemID = this.Guid;
                    adderItem.SolarSystem = SolarSystem;

                    if (adderItem.RoofPlaneDetails == null || !adderItem.RoofPlaneDetails.Any())
                        continue;

                    foreach (var roofPlaneDetail in adderItem.RoofPlaneDetails)
                    {
                        roofPlaneDetail.AdderItemID = adderItem.Guid;
                        roofPlaneDetail.AdderItem = adderItem;
                        roofPlaneDetail.RoofPlane = SolarSystem.RoofPlanes.FirstOrDefault(rp => rp.Guid == roofPlaneDetail.RoofPlaneID);
                    }
                }
            }

            if (SolarSystem.RoofPlanes != null && SolarSystem.RoofPlanes.Any())
            {
                foreach (var roofPlane in SolarSystem.RoofPlanes)
                {
                    if (roofPlane.Guid == null || roofPlane.Guid == Guid.Empty) roofPlane.Guid = Guid.NewGuid();

                    roofPlane.SolarSystemID = this.Guid;
                    roofPlane.SolarSystem = SolarSystem;

                    if (roofPlane.Points != null && roofPlane.Points.Any())
                    {
                        foreach (var point in roofPlane.Points)
                        {
                            point.RoofPlaneID = roofPlane.Guid;
                            point.RoofPlane = roofPlane;
                        }
                    }

                    if (roofPlane.Edges != null && roofPlane.Edges.Any())
                    {
                        foreach (var edge in roofPlane.Edges)
                        {
                            edge.RoofPlaneID = roofPlane.Guid;
                            edge.RoofPlane = roofPlane;
                        }
                    }

                    if (roofPlane.Panels != null && roofPlane.Panels.Any())
                    {
                        foreach (var panel in roofPlane.Panels)
                        {
                            panel.RoofPlaneID = roofPlane.Guid;
                            panel.RoofPlane = roofPlane;
                        }
                    }

                    if (roofPlane.Obstructions != null && roofPlane.Obstructions.Any())
                    {
                        foreach (var obstruction in roofPlane.Obstructions)
                        {
                            obstruction.RoofPlaneID = roofPlane.Guid;
                            obstruction.RoofPlane = roofPlane;

                            if (obstruction.ObstructionPoints != null && obstruction.ObstructionPoints.Any())
                            {
                                foreach (var obstructionPoint in obstruction.ObstructionPoints)
                                {
                                    obstructionPoint.RoofPlaneObstructionID = obstruction.Guid;
                                    obstructionPoint.RoofPlaneObstruction = obstruction;
                                }
                            }
                        }
                    }
                }
            }

            if (SolarSystem.FinancePlans != null && SolarSystem.FinancePlans.Any())
            {
                foreach (var item in SolarSystem.FinancePlans)
                {
                    item.SolarSystemID = this.Guid;
                    item.SolarSystem = SolarSystem;
                }
            }

            var production = SolarSystem.SystemProduction;
            if (production != null)
            {
                production.Guid = this.Guid;
                production.SolarSystem = SolarSystem;


                if (production.Months != null)
                {
                    foreach (var month in production.Months)
                    {
                        month.SystemProductionID = this.Guid;
                    }
                }
            }
        }

        public string GetPropertyAddress()
        {
            return string.Format("{0}, {1}, {2}, {3}", Address, City, State, ZipCode);
        }

        public Proposal Clone(string name, CloneSettings cloneSettings)
        {

            Proposal clone = (Proposal)this.MemberwiseClone();
            clone.Reset();
            clone.Name = name;
            clone.ExternalID = $"{Guid}";
            clone.SolarSystem = this.SolarSystem.Clone(clone.Guid, cloneSettings);
            clone.Tariff = this.Tariff.Clone(clone.Guid, cloneSettings);
            clone.SalesRep = null;
            clone.Property = null;


            //serialize it and deserialize it to make a real copy free from EF tracking
            var json = JsonConvert.SerializeObject(clone, Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });


            var ret = JsonConvert.DeserializeObject<Proposal>(json);
            ret.Property = null;

            //now we have to wipe out back object references 

            foreach (var fp in ret.SolarSystem.FinancePlans)
            {
                //fp.ProposalData = null;   
            }




            return ret;

        }
    }
}
