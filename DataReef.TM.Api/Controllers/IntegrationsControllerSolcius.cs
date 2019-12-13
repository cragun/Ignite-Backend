using DataReef.Core.Configuration;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Integrations.Spruce.DTOs;
using DataReef.TM.Api.CustomResponseTypes;
using DataReef.TM.Api.Mappers;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using DataReef.TM.Models.Spruce;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    public partial class IntegrationsController : ApiController
    {

        [Route("Solcius/Spruce/hardcreditcheck/callback")]
        [HttpPost]
        [InjectAuthPrincipal]
        [AllowAnonymous]
        public IHttpActionResult SpruceHardCreditCheckCallback(SpruceCallbackRequestModel request)
        {
            try
            {
                var filePath = HttpContext.Current.Server.MapPath("~/App_Data/Generated_HardCreditCheck_Callbacks.txt");
                var text = JsonConvert.SerializeObject(request);
                using (var file = File.CreateText(filePath))
                {
                    file.WriteLine(string.Format("\r\n{0}\r\n{1}\r\n", DateTime.UtcNow, text));
                    file.Close();
                }
            }
            catch
            {
            }

            Models.Spruce.QuoteResponse quoteResponse = _spruceQuoteResponseService.List(filter: String.Format("QuoteNumber={0}", request.QuoteNumber)).First();

            Models.Spruce.QuoteRequest quoteRequest = _spruceQuoteRequestService.Get(quoteResponse.Guid, include: "QuoteResponse,SetupInfo,AppInfo,CoAppInfo,GenDocsRequest");


            if (quoteRequest == null)
            {
                return InternalServerError();
            }

            quoteResponse = quoteRequest.QuoteResponse;

            quoteRequest.QuoteResponse.Decision = SpruceQuoteMapper.MapHardCreditCheckCallbackRequestDecisionToQuoteResponseDecision(request.QuoteStatus);
            quoteRequest.QuoteResponse.DecisionDateTime = Convert.ToDateTime(request.Milestones.CreditDecision.DecisionDate);
            quoteRequest.QuoteResponse.AmountFinanced = request.LoanTerms.AmountFinanced;
            quoteRequest.QuoteResponse.MaxApproved = request.LoanTerms.MaxApproved;
            quoteRequest.QuoteResponse.LoanRate = request.LoanTerms.LoanRate;
            quoteRequest.QuoteResponse.MonthlyPayment = request.LoanTerms.MonthlyPayment;
            quoteRequest.QuoteResponse.Term = request.LoanTerms.Term;
            quoteRequest.QuoteResponse.IntroRatePayment = request.LoanTerms.IntroRatePayment;
            quoteRequest.QuoteResponse.IntroTerm = request.LoanTerms.IntroTerm;
            quoteRequest.QuoteResponse.GotoTerm = request.LoanTerms.GotoTerm;

            if (quoteRequest.GenDocsRequest == null && request.QuoteStatus.Equals("APPR"))
            {

                string serviceUrl = System.Configuration.ConfigurationManager.AppSettings["SpruceUrl"].ToString();

                var user = _userService.Get(quoteRequest.CreatedByID.Value, "ExternalCredentials");

                Integrations.Spruce.DTOs.GenDocsRequest genDocsRequest = new DataReef.Integrations.Spruce.DTOs.GenDocsRequest();
                Models.Spruce.GenDocsRequest genDocsModel = quoteRequest.GenDocsRequest;

                if (genDocsModel == null)
                {
                    genDocsModel = new Models.Spruce.GenDocsRequest();
                }

                genDocsModel.Guid = quoteRequest.Guid;
                genDocsRequest.QuoteNumber = genDocsModel.QuoteNumber = quoteResponse.QuoteNumber;
                genDocsRequest.PartnerId = genDocsModel.PartnerId = quoteRequest.SetupInfo != null ? quoteRequest.SetupInfo.Partner : null;
                genDocsRequest.TotalCashSalesPrice = genDocsModel.TotalCashSalesPrice = quoteRequest.SetupInfo != null ? Convert.ToDecimal(quoteRequest.SetupInfo.CashSalesPrice) : 0;
                genDocsRequest.CashDownPayment = genDocsModel.CashDownPayment = quoteRequest.SetupInfo != null ? Convert.ToDecimal(quoteRequest.SetupInfo.DownPayment) : 0;
                genDocsRequest.AmountFinanced = genDocsModel.AmountFinanced = quoteRequest.SetupInfo != null ? Convert.ToDecimal(quoteRequest.SetupInfo.AmountFinanced) : 0;
                genDocsRequest.InstallCommencementDate = genDocsModel.InstallCommencementDate = DateTime.UtcNow.AddDays(30);
                genDocsRequest.SubstantialCompletionDate = genDocsModel.SubstantialCompletionDate = DateTime.UtcNow.AddDays(60);
                genDocsRequest.ProjectedPTODate = genDocsModel.ProjectedPTODate = DateTime.UtcNow.AddDays(90);
                genDocsRequest.EmailApplicant = genDocsModel.EmailApplicant = quoteRequest.AppInfo == null ? null : quoteRequest.AppInfo.Email;
                genDocsRequest.EmailCoapplicant = genDocsModel.EmailCoapplicant = quoteRequest.CoAppInfo == null ? null : quoteRequest.CoAppInfo.Email;
                genDocsModel.QuoteRequest = quoteRequest;

                int finOptId = Int32.Parse(quoteRequest.SetupInfo.FinOptId);
                Guid legionPropertyID = quoteRequest.LegionPropertyID;
                Guid newestProposalID = _proposalService.List(filter: String.Format("PropertyID={0}", legionPropertyID)).OrderByDescending(p => p.DateLastModified).First().Guid;
                Proposal newestProposal = _proposalService.Get(newestProposalID, include: "SolarSystem,SolarSystem.FinancePlans");

                var planID = newestProposal
                                    .SolarSystem
                                    .FinancePlans
                                    .Select(fp => new
                                    {
                                        Guid = fp.Guid,
                                        Resp = JsonConvert.DeserializeObject<LoanResponse>(fp.ResponseJSON),
                                        DateLastModified = fp.DateLastModified,
                                        FinanceOptionId = fp.FinancePlanDefinition.FinanceOptionId
                                    })
                                    .Where(fp => fp.FinanceOptionId == finOptId)
                                    .OrderByDescending(fp => fp.DateLastModified)
                                    .Select(fp => fp.Guid)
                                    .FirstOrDefault();

                var financePlan = newestProposal
                                    .SolarSystem
                                    .FinancePlans
                                    .FirstOrDefault(fp => fp.Guid == planID);

                if (financePlan != null)
                {
                    //todo: change LoanRequestSpruce with new loan model LoanRequest
                    var loanRequestJSON = JsonConvert.DeserializeObject<LoanRequestSpruce>(financePlan.RequestJSON);
                    genDocsRequest.SalesTax = genDocsModel.SalesTax = loanRequestJSON.TaxRate;
                }

                _spruceGenDocsRequestService.Update(genDocsModel);

                Integrations.Spruce.IntegrationProvider provider = new Integrations.Spruce.IntegrationProvider(serviceUrl);
                provider.GenerateDocuments(genDocsRequest, ConfigurationKeys.SpruceUsername, ConfigurationKeys.SprucePassword);
            }

            quoteRequest.CallbackJSON = JsonConvert.SerializeObject(request);

            _spruceQuoteRequestService.Update(quoteRequest);
            _spruceQuoteResponseService.Update(quoteResponse);

            return Ok(new { });
        }

        private void PrepareRequest(Models.Spruce.QuoteRequest request)
        {
            if (!String.IsNullOrEmpty(request.AppInfo.SSN)) request.AppInfo.SSN = request.AppInfo.SSN.Replace("-", "");
            if (!String.IsNullOrEmpty(request.AppInfo.CellPhone)) request.AppInfo.CellPhone = request.AppInfo.CellPhone.Replace("-", "");
            if (!String.IsNullOrEmpty(request.AppInfo.HomePhone)) request.AppInfo.HomePhone = request.AppInfo.HomePhone.Replace("-", "");
            if (request.CoAppInfo != null)
            {
                if (!String.IsNullOrEmpty(request.CoAppInfo.SSN)) request.CoAppInfo.SSN = request.CoAppInfo.SSN.Replace("-", "");
                if (!String.IsNullOrEmpty(request.CoAppInfo.CellPhone)) request.CoAppInfo.CellPhone = request.CoAppInfo.CellPhone.Replace("-", "");
                if (!String.IsNullOrEmpty(request.CoAppInfo.HomePhone)) request.CoAppInfo.HomePhone = request.CoAppInfo.HomePhone.Replace("-", "");
            }
        }

        [Route("Solcius/Spruce/documents/callback")]
        [HttpPost]
        [InjectAuthPrincipal]
        [AllowAnonymous]
        public IHttpActionResult GenerateDocumentsCallback(SpruceCallbackRequestModel request)
        {
            try
            {
                var filePath = HttpContext.Current.Server.MapPath("~/App_Data/Generated_Documents_Callbacks.txt");
                var text = JsonConvert.SerializeObject(request);
                using (var file = File.CreateText(filePath))
                {
                    file.WriteLine(string.Format("\r\n{0}\r\n{1}\r\n", DateTime.UtcNow, text));
                    file.Close();
                }
            }
            catch (Exception)
            {
            }


            Models.Spruce.QuoteResponse quoteResponse = _spruceQuoteResponseService.List(filter: String.Format("QuoteNumber={0}", request.QuoteNumber)).First();

            Models.Spruce.QuoteRequest quoteRequest = _spruceQuoteRequestService.Get(quoteResponse.Guid);


            if (quoteRequest == null)
            {
                return InternalServerError();
            }

            quoteRequest.CallbackJSON = JsonConvert.SerializeObject(request);

            _spruceQuoteRequestService.Update(quoteRequest);

            return Ok(new { });
        }

        private decimal GetAmountFinanced(LoanRequestSpruce loanRequest)
        {
            decimal purchasePrice = (loanRequest.PricePerWattASP * loanRequest.SystemSize) * (1 + loanRequest.TaxRate);
            return purchasePrice - loanRequest.DownPayment;
        }

        private IHttpActionResult GetErrors(string providerErrorMessage)
        {
            var errorMessage = providerErrorMessage;
            var matches = Regex.Matches(providerErrorMessage, @":\[\""(.*?)\""\]");
            if (matches != null && matches.Count > 0) errorMessage = string.Join("\n", matches.Cast<Match>().Select(m => m.Groups[1].Value));
            return new TextResult(errorMessage, Request, HttpStatusCode.InternalServerError);
        }
    }
}