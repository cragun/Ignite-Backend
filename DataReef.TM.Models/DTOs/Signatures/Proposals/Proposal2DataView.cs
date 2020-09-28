using DataReef.TM.Models.DTOs.Proposals;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    /// <summary>
    /// This model is created to be used by the html proposal... for now
    /// </summary>
    public class Proposal2DataView
    {
        public Guid Guid { get; set; }
        public Guid ProposalID { get; set; }

        public DateTime? SignedDate { get; set; }
        public List<string> SignedDocuments { get; set; }
        public ProposalDesignSystemType DesignSystemType { get; set; }

        public List<ProposalMediaItemDataView> MediaItems { get; set; }

        public ProposalBasicInfo BasicInfo { get; set; }
        public ProposalSolution SolarSolution { get; set; }

        public ProposalEnergy Energy { get; set; }

        public ProposalForecast ForecastScenario { get; set; }

        public ProposalEnergyCosts EnergyCosts { get; set; }

        public ProposalSystemCosts SystemCosts { get; set; }

        public ProposalFinancing Financing { get; set; }

        public List<ProposalFinancePlanOption> FinancePlanOptions { get; set; }

        public Dictionary<string, string> Settings { get; set; }
        /// <summary>
        /// Dynamic object, containing proposal data set by the customer/sales rep before signing the proposal
        /// </summary>
        public string ProposalDataJSON { get; set; }

        public ICollection<string> Tags { get; set; }

        public bool? UsageCollected { get; set; }

        public string ExcludeProposalJSON { get; set; }


       
        public double ProductionKWH { get; set; }
    
        public double ProductionKWHpercentage { get; set; }
 
        public bool IsManual { get; set; }
        
        public double SystemSize { get; set; }


        public Proposal2DataView()
        {
            BasicInfo = new ProposalBasicInfo();
            SolarSolution = new ProposalSolution();
            Energy = new ProposalEnergy();
            SystemCosts = new ProposalSystemCosts();
            Financing = new ProposalFinancing();
        }

        public Proposal2DataView(ProposalDVConstructor param, bool roundAmounts = false)
        {
            Guid = param.Data.Guid;
            ProposalID = param.Proposal.Guid;
            SignedDate = param.Data.SignatureDate;
            UsageCollected = param.Proposal.Property.UsageCollected;
            ProductionKWH = param.Proposal.ProductionKWH;
            ProductionKWHpercentage = param.Proposal.ProductionKWHpercentage;
            IsManual = param.Proposal.IsManual;
            SystemSize = param.Proposal.SystemSize;

            ExcludeProposalJSON = param.Data.excludeProposalJSON ?? "[]";
            if (!string.IsNullOrEmpty(param.Proposal?.SignedDocumentsJSON))
            {
                SignedDocuments = JsonConvert.DeserializeObject<List<SignedDocumentDTO>>(param.Proposal.SignedDocumentsJSON)?.Select(x => x.Name)?.ToList();
            }
            
            DesignSystemType = param.Proposal.DesignSystemType;

            BasicInfo = new ProposalBasicInfo(param.Proposal, param.DealerName, param.Data.DateCreated);
            SolarSolution = new ProposalSolution(param.Proposal, param.FinancePlan, param.Request, param.Response);
            Energy = new ProposalEnergy(param.FinancePlan, param.Request, param.Response);

            EnergyCosts = new ProposalEnergyCosts(param.FinancePlan, param.UtilityInflationRate, param.Request, param.Response, roundAmounts);
            SystemCosts = new ProposalSystemCosts(param.FinancePlan, param.Request, roundAmounts);
            Financing = new ProposalFinancing(param.FinancePlan, param.Request, param.Response, roundAmounts);

            ProposalDataJSON = param.Data.ProposalDataJSON;

            var documentDataMediaItems = param?
                            .Data?
                            .DocumentDataLinks?
                            .Select(dl => new ProposalMediaItemDataView
                            {
                                Type = ProposalMediaItemDataViewType.Data_SolarSystem,
                                Url = dl.ContentURL,
                                ThumbUrl = dl.ThumbContentURL,
                                DataJSON = dl.DataJSON,
                            })
                            .ToList();

            var userValidations = param?
                                    .Data?
                                    .UserInputLinks?
                                    .Where(ui => ui.ValidationUrl?.Count > 0)
                                    .Where(ui => ui.Type == UserInputDataType.Signature ||
                                                 ui.Type == UserInputDataType.ThreeDCustomer)
                                    .Select(ui => new ProposalMediaItemDataView
                                    {
                                        Type = ProposalMediaItemDataViewType.Data_SignersImage,
                                        Url = ui.ValidationUrl?.LastOrDefault(),
                                        DataJSON = ui.DataJSON
                                    })
                                    .ToList();

            var userInputMediaItems = param?
                            .Data?
                            .UserInputLinks?
                            .Select(ui => new ProposalMediaItemDataView
                            {
                                Type = ui.Type.ToProposalEnum(),
                                Url = ui.ContentURL,
                                ThumbUrl = ui.ThumbContentURL,
                                DataJSON = ui.DataJSON
                            })
                            .ToList();

            MediaItems = (documentDataMediaItems ?? new List<ProposalMediaItemDataView>())
                            .Union(userInputMediaItems ?? new List<ProposalMediaItemDataView>())
                            .Union(userValidations ?? new List<ProposalMediaItemDataView>())
                            .ToList();

            Tags = param.Proposal.Tags;
        }
    }
}
