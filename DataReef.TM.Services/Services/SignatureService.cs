using DataReef.Core;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Other;
using DataReef.Integrations.Hancock.DTOs;
using DataReef.TM.Contracts.FaultContracts;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DataReef.TM.Models.PRMI;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class SignatureService : ISignatureService
    {
        private readonly IBlobService _blobService;
        private readonly IProposalService _proposalService;
        private readonly IOUSettingService _ouSettingsService;
        private readonly IVelocifyService _velocifyService;
        private readonly IMortgageService _mortgageService;
        //private string senderEmailAddress = ConfigurationManager.AppSettings["SenderEmail"] ?? "noreply@datareef.com";
        private string senderEmailAddress = ConfigurationManager.AppSettings["SenderEmail"] ?? "support@smartboardcrm.com";
        
        public SignatureService(
            IBlobService blobService,
            IProposalService proposalService,
            IOUSettingService ouSettingsService,
            IVelocifyService velocifyService,
            IMortgageService mortgageService)
        {
            _blobService = blobService;
            _proposalService = proposalService;
            _ouSettingsService = ouSettingsService;
            _velocifyService = velocifyService;
            _mortgageService = mortgageService;
        }

        public List<SignDocumentResponse> SignDocument(SignDocumentRequest request, Guid ouid, Guid financePlanDefinitionId, FinancePlanType financePlanType, FinanceDocumentType documentType)
        {
            var callbackURL = Constants.APIBaseAddress + "/api/v1/sign/RightSignatureContractCallback";
            var redirectURL = Constants.APIBaseAddress + "/api/v1/sign/completed";
            var homeOwnerName = "";
            var homeOwnerEmailAddress = "";
            var salesRepName = "";
            var salesRepEmailAddress = "";
            byte[] contractContent;

            var response = new List<SignDocumentResponse>();

            using (var dataContext = new DataContext())
            {
                var ou = dataContext
                            .OUs
                            .FirstOrDefault(o => o.Guid == ouid);

                if (ou == null) throw new ArgumentException("Organization not found!");
                var rootOrganizationName = ou.RootOrganizationName;

                // remove existing documents
                _proposalService.DeleteDocuments(new List<Guid> { request.FinancePlanID }, dataContext, documentType);

                var contractState = dataContext
                                    .Properties
                                    .Where(p => p.Guid == request.PropertyID)
                                    .Select(p => p.State)
                                    .SingleOrDefault();

                if (string.IsNullOrEmpty(contractState)) throw new TemplateNotFoundException("Cound not get the state for the contract template.");

                var proposal = dataContext
                                            .Proposal
                                            .FirstOrDefault(p => p.Guid == request.ProposalID);
                var property = dataContext
                                .Properties
                                .FirstOrDefault(p => p.Guid == proposal.PropertyID);

                salesRepEmailAddress = dataContext
                                    .People
                                    .Where(p => p.Guid == proposal.PersonID)
                                    .Select(p => p.EmailAddressString)
                                    .SingleOrDefault();

                homeOwnerName = property.Name;

                homeOwnerEmailAddress = dataContext
                                    .Fields
                                    .Where(f => f.DisplayName == "Email Address" && f.PropertyId == proposal.PropertyID)
                                    .Select(f => f.Value)
                                    .FirstOrDefault();

                var settings = Task.Run(() => _ouSettingsService.GetSettings(ouid, OUSettingGroupType.ConfigurationFile)).Result;
                var key = documentType.GetTemplateName();
                var fileNames = new List<string>();

                if (settings.ContainsKey(key))
                {
                    var settingData = settings[key];
                    if (settingData != null && settingData.Value != null)
                    {
                        var financePlanDefinition = dataContext
                                        .FinancePlaneDefinitions
                                        .FirstOrDefault(fpd => fpd.Guid == financePlanDefinitionId);

                        var data = JsonConvert.DeserializeObject<Dictionary<Guid, Dictionary<FinancePlanType, Dictionary<string, List<string>>>>>(settingData.Value);
                        var financeProviderData = data[financePlanDefinition.ProviderID];
                        var statesData = financeProviderData[financePlanType];

                        // Add the files for "ALL" states
                        AddDictionaryDataToList(fileNames, statesData, "ALL");
                        // add the files for the property's state
                        AddDictionaryDataToList(fileNames, statesData, contractState);
                    }
                }

                foreach (var templatePath in fileNames)
                {

                    var postFix = templatePath.GetPostFix("_");

                    // get the file content from blob storage
                    byte[] templateByteArray;
                    try
                    {
                        var blob = Task.Run(() => _blobService.DownloadByName(templatePath, BlobService.S3SourceDocumentsBucket)).Result;
                        templateByteArray = blob.Content;
                    }
                    catch
                    {
                        throw new TemplateNotFoundException("Could not sign contract for your state.");
                    }

                    // populate template file w/ data
                    using (MemoryStream inputStream = new MemoryStream())
                    {
                        inputStream.Write(templateByteArray, 0, (int)templateByteArray.Length);
                        using (var document = new Document(inputStream))
                        {
                            if (request.DocumentVariables != null)
                            {
                                foreach (var contractVariable in request.DocumentVariables)
                                {
                                    var contractVariableName = Regex.Replace(contractVariable.Key, @"\s+", "");
                                    var contractVariableValue = contractVariable.Value.ToString();
                                    document.Replace(String.Format(@"[{0}]", contractVariableName), contractVariableValue, false, false);

                                    switch (contractVariableName)
                                    {
                                        case "HomeOwnerName": homeOwnerName = contractVariableValue; break;
                                        case "SalesRepName": salesRepName = contractVariableValue; break;
                                        case "EmailAddress": homeOwnerEmailAddress = contractVariableValue; break;
                                    }
                                }
                            }

                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                var pdfExportParameters = new ToPdfParameterList();
                                pdfExportParameters.EmbeddedFontNameList.Add("Times New Roman");
                                document.SaveToStream(outputStream, pdfExportParameters);
                                contractContent = outputStream.ToArray();
                            }
                        }
                    }

                    // build recipients list
                    var embeddedSigning = request.EmbeddedSigning.Equals("true", StringComparison.InvariantCultureIgnoreCase);

                    var recipients = new List<Recipient>();

                    if (request.DocumentVariables != null)
                    {
                        recipients.Add(new Recipient
                        {
                            Name = homeOwnerName,
                            Email = embeddedSigning ? "noemail@rightsignature.com" : homeOwnerEmailAddress,
                            Role = "signer"
                        });
                        if (request.NeedsSalesRepSignature)
                        {
                            recipients.Add(new Recipient
                            {
                                Name = salesRepName,
                                Email = embeddedSigning ? "noemail@rightsignature.com" : salesRepEmailAddress,
                                Role = "signer"
                            });
                        }
                        recipients.Add(new Recipient
                        {
                            IsSender = true,
                            Role = "cc"
                        });
                    }

                    // call the RightSignature WS
                    string rightSignatureApiToken = ConfigurationManager.AppSettings["RightSignatureApiToken"];
                    var signatureProvider = new Integrations.RightSignature.IntegrationProvider(rightSignatureApiToken);

                    // upload contract to AWS
                    var contractPath = GetDocumentPath(rootOrganizationName, documentType, request.ProposalID, postFix + "_unsigned");
                    var contractURL = UploadPDF(contractPath, contractContent);

                    // send document for signing
                    var documentIDResponse = signatureProvider.SendDocument(documentUrl: contractURL,
                                        subject: "contract",
                                        callbackURL: callbackURL,
                                        recipients: recipients,
                                        expiresInDays: 5,
                                        tags: new Dictionary<string, string> { { "ProposalID", request.ProposalID.ToString() }, { "PropertyID", request.PropertyID.ToString() } });

                    var documentId = signatureProvider.GetDocumentId(documentIDResponse);

                    // create a new ProposalDocument in DB
                    SaveDocument(new FinanceDocument
                    {
                        DocumentID = documentId,
                        FinancePlanID = request.FinancePlanID,
                        UnsignedURL = contractURL,
                        DocumentType = documentType,
                        ExpiryDate = DateTime.UtcNow.AddDays(5)
                    }, dataContext);

                    var contractResponse = new SignDocumentResponse
                    {
                        ContractID = documentId,
                        SignerLinks = embeddedSigning ? signatureProvider.GetSignerLinks(documentId, redirectURL) : null
                    };
                    response.Add(contractResponse);
                }
            }

            return response;
        }

        public SignDocumentResponseHancock SignDocumentHancock(SignDocumentRequestHancock request, Guid ouid, FinanceDocumentType documentType)
        {
            string rootOrganizationName;
            Proposal proposal;
            FinancePlan financePlan;
            var response = new SignDocumentResponseHancock();

            using (var dataContext = new DataContext())
            {
                var ou = dataContext
                            .OUs
                            .FirstOrDefault(o => o.Guid == ouid);

                if (ou == null) throw new ArgumentException("Organization not found!");
                rootOrganizationName = ou.RootOrganizationName;

                financePlan = dataContext.FinancePlans.Include(fp => fp.SolarSystem).Include(fp => fp.SolarSystem.Proposal).SingleOrDefault(fp => fp.Guid == request.FinancePlanID);

                proposal = financePlan.SolarSystem.Proposal;

                var settings = Task.Run(() => _ouSettingsService.GetSettings(ouid, OUSettingGroupType.ConfigurationFile)).Result;
            }

            var hostNotifier = new HostNotifier();
            if (request.UserInputs != null && request.UserInputs.Any(ui => !String.IsNullOrEmpty(ui.Content))) // we have the signatures, render final document
            {
                Task.Run(() =>
                {
                    using (var dataContext = new DataContext())
                    {
                        var proposalDocument = dataContext.FinanceDocuments.SingleOrDefault(pd => pd.FinancePlanID == request.FinancePlanID && pd.DocumentType == documentType);
                        if (proposalDocument == null) throw new Exception("The unsigned document could not be found");

                        try
                        {
                            var executeRequest = new ExecuteDocumentRequest
                            {
                                RenderedDocumentID = Guid.Parse(proposalDocument.DocumentID),
                                UserInputs = request.UserInputs.Where(ui => !String.IsNullOrEmpty(ui.Content)).ToList()
                            };

                            var signatureProvider = new DataReef.Integrations.Hancock.IntegrationProvider();
                            response = signatureProvider.ExecuteDocument(executeRequest).ToApiResponse();

                            proposalDocument.SignedURL = response.DocumentURL;
                            proposalDocument.SignedDate = DateTime.UtcNow;
                            dataContext.SaveChanges();

                            var propertyAddress = proposal.GetPropertyAddress();


                            var property = dataContext
                            .Properties
                            .FirstOrDefault(p => p.Guid == proposal.PropertyID);

                            var salesRepEmailAddress = dataContext
                                                .People
                                                .Where(p => p.Guid == proposal.PersonID)
                                                .Select(p => p.EmailAddressString)
                                                .SingleOrDefault();

                            var homeOwnerName = property.Name;

                            var homeOwnerEmailAddress = dataContext
                                                .Fields
                                                .Where(f => f.DisplayName == "Email Address" && f.PropertyId == proposal.PropertyID)
                                                .Select(f => f.Value)
                                                .FirstOrDefault();

                            var attachment = DownloadPDF(response.DocumentURL, "hancock-prod");
                            var attachments = new List<Tuple<string, byte[]>>
                            {
                                new Tuple<string, byte[]>(string.Format( "Proposal_{0}.pdf", proposal.Guid), attachment.Content)
                            };
                            SendEmailWithAttachment(senderEmailAddress, salesRepEmailAddress, homeOwnerEmailAddress, string.Format("Proposal for {0} at {1}", homeOwnerName, propertyAddress), string.Format("Attached you will find the proposal for {0} at {1}.", homeOwnerName, propertyAddress), attachments);
                        }
                        catch (Exception exception)
                        {
                            var errorMessage = exception.Message;
                            var innerException = exception.InnerException;
                            while (innerException != null)
                            {
                                errorMessage += Environment.NewLine + innerException.Message;
                                innerException = innerException.InnerException;
                            }
                            errorMessage += Environment.NewLine + exception.StackTrace;
                            proposalDocument.ErrorMessage = errorMessage;
                            dataContext.SaveChanges();
                        }
                    }
                });
            }
            else // render document for signing
            {
                var signatureProvider = new DataReef.Integrations.Hancock.IntegrationProvider();
                var userInputsResponse = signatureProvider.GetUserInputs(request.ProposalTemplateID);
                response.UserInputs = userInputsResponse.UserInputs;

                Task.Run(() =>
                {
                    using (var dataContext = new DataContext())
                    {
                        try
                        {
                            var mergeFieldsResponse = signatureProvider.GetMergeFields(request.ProposalTemplateID);

                            List<string> fieldNames = mergeFieldsResponse.Select(mf => mf.FieldName).ToList();
                            var proposalToMerge = dataContext.Proposal
                                                        .Include(p => p.Property.Territory.OU)
                                                        .Include(p => p.Property.Occupants)
                                                        .Include(p => p.Property.PropertyBag)
                                                        .Include(p => p.Tariff)
                                                        .Include(p => p.SolarSystem)
                                                        .Include(p => p.SolarSystem.RoofPlanes)
                                                        .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Points))
                                                        .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Edges))
                                                        .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Panels))
                                                        .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Obstructions))
                                                        .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.SolarPanel))
                                                        .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Inverter))
                                                        .Include(p => p.SolarSystem.PowerConsumption)
                                                        .Include(p => p.SolarSystem.SystemProduction)
                                                        .Single(p => p.Guid == proposal.Guid);
                            var salesRep = dataContext.People.Include(p => p.PhoneNumbers).FirstOrDefault(p => p.Guid == proposalToMerge.PersonID);
                            if (salesRep != null)
                            {
                                proposalToMerge.SalesRep = salesRep;
                            }

                            var proposalDataView = ProposalDataView.FromCoreModel(proposalToMerge, financePlan, request.ContractorID, SmartPrincipal.DeviceDate.LocalDateTime);
                            var mergeFields = proposalDataView.ToMergeDictionary(fieldNames);

                            mergeFields.AddRange(request.MergeFields);

                            var renderRequest = new RenderDocumentRequest
                            {
                                DocumentID = request.ProposalTemplateID,
                                ExternalID = request.FinancePlanID.ToString(),
                                MergeFields = mergeFields
                            };

                            response = signatureProvider.RenderDocument(renderRequest).ToApiResponse();

                            // remove existing documents
                            _proposalService.DeleteDocuments(new List<Guid> { request.FinancePlanID }, dataContext, null);

                            SaveDocument(new FinanceDocument
                            {
                                DocumentID = response.Guid.ToString(),
                                FinancePlanID = request.FinancePlanID,
                                UnsignedURL = response.DocumentURL,
                                DocumentType = documentType,
                                ExpiryDate = DateTime.UtcNow.AddDays(5)
                            }, dataContext);
                        }
                        catch (Exception exception)
                        {
                            var errorMessage = exception.Message;
                            var innerException = exception.InnerException;
                            while (innerException != null)
                            {
                                errorMessage += Environment.NewLine + innerException.Message;
                                innerException = innerException.InnerException;
                            }
                            errorMessage += Environment.NewLine + exception.StackTrace;

                            SaveDocument(new FinanceDocument
                            {
                                DocumentID = null,
                                FinancePlanID = request.FinancePlanID,
                                UnsignedURL = null,
                                SignedURL = null,
                                DocumentType = documentType,
                                ErrorMessage = errorMessage,
                                ExpiryDate = DateTime.UtcNow.AddDays(5)
                            }, dataContext);
                        }
                    }
                });
            }

            return response;
        }

        private void AddDictionaryDataToList(List<string> list, Dictionary<string, List<string>> dict, string key)
        {
            if (!dict.ContainsKey(key))
                return;

            var data = dict[key];
            if (data == null)
                return;

            list.AddRange(data);
        }

        /// <summary>
        /// Method that generates a path to store documents in AWS. Made it static to use it from API.
        /// </summary>
        /// <param name="rootOuName">Name of the Root OU</param>
        /// <param name="docType">Document type</param>
        /// <param name="fileName">File Name</param>
        /// <param name="postFix">file name postfix</param>
        /// <returns></returns>
        public static string GetDocumentPath(string rootOuName, FinanceDocumentType docType, object fileName, string postFix = null)
        {
            return string.Format("{0}/{1}s/{2}{3}.pdf", rootOuName, docType, fileName, postFix);
        }

        public void RightSignatureContractCallback(Callback callback)
        {
            if (callback.CallbackType != "Document" || callback.Status != "signed") return;

            var rightSignatureApiToken = ConfigurationManager.AppSettings["RightSignatureApiToken"];
            var signatureProvider = new DataReef.Integrations.RightSignature.IntegrationProvider(rightSignatureApiToken);
            var signedContractURL = HttpUtility.UrlDecode(signatureProvider.GetSignedDocumentURL(callback.Guid));
            Proposal proposal = null;

            using (var dataContext = new DataContext())
            {
                var doc = dataContext
                            .FinanceDocuments
                            .Include("FinancePlan.SolarSystem.Proposal")
                            .FirstOrDefault(pd => pd.DocumentID == callback.Guid);

                if (doc == null || doc.FinancePlan == null || doc.FinancePlan.SolarSystem == null || doc.FinancePlan.SolarSystem.Proposal == null)
                    return;

                var financePlan = doc.FinancePlan;
                proposal = financePlan.SolarSystem.Proposal;

                var property = dataContext
                                    .Properties
                                    .Include("Territory.OU")
                                    .FirstOrDefault(p => p.Guid == proposal.PropertyID);

                var forOrganization = property.Territory.OU.RootOrganizationName;

                // todo: if we'll have more contract types it's better to save the signature call with all the contract details including type and root OU name and send the guid of that entity instead of the property id

                byte[] signedContractContent;

                using (WebClient client = new WebClient())
                {
                    signedContractContent = client.DownloadData(signedContractURL);
                }

                var postFix = string.IsNullOrWhiteSpace(doc.UnsignedURL) ? string.Empty : doc.UnsignedURL.Replace("_unsigned", "").GetPostFix("_");
                var signedContractPath = GetDocumentPath(forOrganization, doc.DocumentType, proposal.Guid, postFix);

                var contractURL = UploadPDF(signedContractPath, signedContractContent);
                doc.SignedURL = contractURL;
                doc.SignedDate = DateTime.UtcNow;
                dataContext.SaveChanges();

                var isLastDoc = !dataContext
                                    .FinanceDocuments
                                    .Any(d => d.FinancePlanID == financePlan.Guid
                                           && d.Guid != doc.Guid
                                           && d.DocumentType == doc.DocumentType
                                           && d.SignedURL == null);

                if (proposal.Status == ProposalStatus.PendingSigning && isLastDoc)
                {
                    // only change status and generate inquiry when last document/contract is signed
                    proposal.Status = ProposalStatus.Signed;

                    // remove all other financial plans
                    dataContext.FinancePlans.RemoveRange(dataContext.FinancePlans.Where(fp => fp.SolarSystemID == proposal.Guid && fp.Guid != financePlan.Guid));

                    dataContext.SaveChanges();
                }

                // if there are more unsigned documents of the same type as this one, exit
                if (!isLastDoc)
                {
                    return;
                }

                // get all documents of the type we got this callback for
                // if we got here, it means that all documents have been signed
                var documents = dataContext
                                    .FinanceDocuments
                                    .Where(d => d.FinancePlanID == financePlan.Guid
                                           && d.DocumentType == doc.DocumentType)
                                    .ToList();

                SendDocumentsEmail(documents);
            }
        }

        public void SendSignedDocumentEmail(FinanceDocument document)
        {
            SendDocumentsEmail(new List<FinanceDocument> { document });
        }

        public void SendPricingRequestEmail(FinanceDocument document)
        {
            SendDocumentsEmail(new List<FinanceDocument> { document }, "Attached you will find the proposal{0} for {1} at {2}.");
        }

        public void SendDocumentsEmail(List<FinanceDocument> documents, string bodyFormat = null)
        {
            if (documents == null || !documents.Any()) return;
            var firstDocument = documents.First();
            var propertyAddress = "";
            var salesRepEmailAddress = "";
            var salesRepName = "";
            var homeOwnerName = "";
            var homeOwnerEmailAddress = "";

            try
            {
                using (var dataContext = new DataContext())
                {
                    var proposal = dataContext
                                    .Proposal
                                    .SingleOrDefault(p => p.SolarSystem.FinancePlans.Any(fp => fp.Guid == firstDocument.FinancePlanID));
                    var property = dataContext
                                    .Properties
                                    .FirstOrDefault(p => p.Guid == proposal.PropertyID);
                    propertyAddress = proposal.GetPropertyAddress();
                    var salesRep = dataContext.People.SingleOrDefault(p => p.Guid == proposal.PersonID);
                    if (salesRep != null)
                    {
                        salesRepEmailAddress = salesRep.EmailAddressString;
                        salesRepName = string.Format("{0} {1}", salesRep.FirstName, salesRep.LastName);
                    }
                    homeOwnerName = property.Name;
                    homeOwnerEmailAddress = dataContext.Fields.Where(f => f.DisplayName == "Email Address" && f.PropertyId == proposal.PropertyID).Select(f => f.Value).FirstOrDefault();
                }

                var attachments = new List<Tuple<string, byte[]>>();

                // create the email message object
                foreach (var document in documents)
                {
                    byte[] content;
                    using (WebClient client = new WebClient())
                    {
                        content = client.DownloadData(document.SignedURL);
                    }

                    var docName = Path.GetFileName(document.SignedURL);

                    attachments.Add(new Tuple<string, byte[]>(docName, content));
                }

                string subject = string.Format("signed document{0} for {1} at {2}", documents.Count > 1 ? "s" : "", homeOwnerName, propertyAddress);
                bodyFormat = bodyFormat ?? "Attached you will find the document{0} signed by {1} for {2}.\r\nSales Rep: {3}";
                string body = string.Format(bodyFormat, documents.Count > 1 ? "s" : "", homeOwnerName, propertyAddress, salesRepName);

                SendEmailWithAttachment(senderEmailAddress, salesRepEmailAddress, homeOwnerEmailAddress, subject, body, attachments);
            }
            catch (Exception)
            {

            }
        }

        private void SendEmailWithAttachment(string sender, string to, string cc, string subject, string body, List<Tuple<string, byte[]>> attachments)
        {
            var email = new MailMessage(sender, to, subject, body);

            if (!string.IsNullOrWhiteSpace(cc))
            {
                var items = cc.Split(';');
                foreach (var item in items)
                {
                    email.CC.Add(item);
                }
            }

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var att in attachments)
                {
                    var name = att.Item1;
                    var extension = Path.GetExtension(name);
                    if (string.IsNullOrWhiteSpace(extension))
                    {
                        name = Path.GetFileNameWithoutExtension(name) + ".pdf";
                    }
                    email.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(att.Item2), att.Item1));
                }
            }
            Mail.Library.SendEmail(email);
        }

        public void SendEmailReminder(string contractID)
        {
            string rightSignatureApiToken = ConfigurationManager.AppSettings["RightSignatureApiToken"];
            var signatureProvider = new DataReef.Integrations.RightSignature.IntegrationProvider(rightSignatureApiToken);
            signatureProvider.SendEmailReminder(contractID);
        }

        /// <summary>
        /// Save the document in db
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dataContext"></param>
        public void SaveDocument(FinanceDocument doc, DataContext dataContext)
        {
            var action = new Action<DataContext>((DataContext dc) =>
            {
                dc.FinanceDocuments.Add(doc);
                dc.SaveChanges();
            });

            if (dataContext == null)
            {
                using (dataContext = new DataContext())
                {
                    action(dataContext);
                }
            }
            else
            {
                action(dataContext);
            }
        }

        /// <summary>
        /// This method uploads the content using the "application/pdf" content type to the given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <returns>Full URL for the file</returns>
        private string UploadPDF(string path, byte[] content)
        {
            _blobService.UploadByName(path,
                    new BlobModel
                    {
                        Content = content,
                        ContentType = "application/pdf"
                    },
                    BlobAccessRights.PublicRead
                );
            return _blobService.GetFileURL(path).TrimParams();
        }

        private BlobModel DownloadPDF(string url, string bucketName = null)
        {
            var documentName = new Uri(url).AbsolutePath.Replace(bucketName, "").TrimStart('/');
            var document = Task.Run(() => _blobService.DownloadByName(documentName, bucketName)).Result;
            return document;
        }
    }
}