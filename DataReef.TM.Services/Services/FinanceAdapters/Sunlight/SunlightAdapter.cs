﻿using DataReef.Core.Attributes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using DataReef.TM.Models.DTOs.Solar.Finance;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;


namespace DataReef.TM.Services.Services.FinanceAdapters.Sunlight
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ISunlightAdapter))]
    public class SunlightAdapter: ISunlightAdapter
    {
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["Sunlight.test.url"];
        private static readonly string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Auth.Username"];
        private static readonly string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Auth.Password"];
        private static readonly string Username = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Username"];
        private static readonly string Password = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Password"];

        private RestClient client
        {
            get
            {
                return new RestClient(url);
            }
        }

        public class TokenCredentials
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public class TokenResponse
        {
            public string access_token { get; set; }
        }

        public string GetSunlightToken()
        {
            try
            {
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));

                var cred = new TokenCredentials();
                cred.username = Username;
                cred.password = Password;

                var request = new RestRequest($"/gettoken/accesstoken", Method.POST);
                request.AddJsonBody(cred);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetSunlightToken Failed. {response.Content}");
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<TokenResponse>(content);

                return ret.access_token;


            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


        //public class SunlightApplicants
        //{
        //    public string firstName { get; set; }
        //    public string lastName { get; set; }
        //    public string email { get; set; }
        //    public string phone { get; set; }
        //    public bool isPrimary { get; set; }

        //}

        //public class SunlightProjects
        //{
        //    public string installStreet { get; set; }
        //    public string installCity { get; set; }
        //    public string installStateName { get; set; }
        //    public string installZipCode { get; set; }
        //    public SunlightApplicants applicants { get; set; }

        //}

        public class SunlightProjects
        {
            public Projects Projects { get; set; }
        }

        public string CreateSunlightApplicant()
        {
            try
            {
                SunlightProjects req = new SunlightProjects();
                Projects project = new Projects();

                Applicants applicnt = new Applicants();
                applicnt.firstName = "John";
                applicnt.lastName = "Consumer";
                applicnt.email = "slfapitesty@gmail.com";
                applicnt.phone = "8015557799";
                applicnt.isPrimary = true;

                project.applicants  = applicnt;
                project.installStreet = "3850 Sunny Side Drive";
                project.installCity = "Austin";
                project.installStateName = "Texas";
                project.installZipCode = "45637";

                req.Projects = project;

                string token = GetSunlightToken();
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));
                var request = new RestRequest($"/applicant/create/", Method.POST);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                request.AddHeader("SFAccessToken", "Bearer " + token);
                
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"CreateSunlightApplicant Failed. {response.Content}");
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<SunlightProjects>(content);

                return ret.Projects?.hashId + "responsecontent " + content + "token " + token ;


            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
