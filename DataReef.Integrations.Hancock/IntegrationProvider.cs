
using DataReef.Integrations.Hancock.DTOs;
using DataReef.TM.Models.DTOs.Signatures;
using RestSharp;
using System;
using System.Collections.Generic;
namespace DataReef.Integrations.Hancock
{



    public class IntegrationProvider
    {
        private readonly string _baseUrl;

        public IntegrationProvider()
        {
            _baseUrl = "https://hancock-api.datareef.com";
           // _baseUrl = "http://localhost:8988";

        }

        public UserInputsResponse GetUserInputs(Guid documentId)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest(string.Format("api/documents/{0}/userinputs", documentId), Method.GET);

            var response = client.Execute<UserInputsResponse>(request);
            return response.Data;
        }

        public RenderDocumentResponse RenderDocument(RenderDocumentRequest renderDocumentRequest)
        {
            var client = new RestClient(_baseUrl);
            client.Timeout = 10 * 60 * 1000;
            var request = new RestRequest("api/documents/render", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(renderDocumentRequest);

            var response = client.Execute<RenderDocumentResponse>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicationException(response.Content);
            }
        
            return response.Data;
        }

        public ExecutedDocumentResponse ExecuteDocument(ExecuteDocumentRequest executeDocumentRequest)
        {
            var client = new RestClient(_baseUrl);
            client.Timeout = 10 * 60 * 1000;

            var request = new RestRequest("api/documents/execute", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(executeDocumentRequest);

            var response = client.Execute<ExecutedDocumentResponse>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicationException(response.Content);
            }

            return response.Data;
        }


        public List<ExecutedDocumentResponse> GetExecutedDocumentsForProposalID(Guid proposalID)
        {
            var client = new RestClient(_baseUrl);
            client.Timeout = 10 * 60 * 1000;

            string url = string.Format("api/documents/executed?proposalID={0}", proposalID.ToString());
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-Type", "application/json");
           
            var response = client.Execute<List<ExecutedDocumentResponse>>(request);
            return response.Data;

        }

        public void DeleteDocument(Guid documentId)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest(string.Format("api/documents/{0}", documentId), Method.DELETE);
            client.Execute(request);
        }

        public List<MergeFieldResponse> GetMergeFields(Guid proposalTemplateId)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("api/designs/{designID}/fields", Method.GET);
            request.AddUrlSegment("designID", proposalTemplateId.ToString());
            request.AddHeader("Content-Type", "application/json");

            var response = client.Execute<List<MergeFieldResponse>>(request);
            return response.Data;
        }
    }
}
