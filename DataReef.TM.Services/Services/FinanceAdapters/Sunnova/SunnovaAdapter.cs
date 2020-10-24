﻿using DataReef.Core.Attributes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.Solar;
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
using System.Web.Script.Serialization;

namespace DataReef.TM.Services.Services.FinanceAdapters.Sunnova
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ISunnovaAdapter))]
    public class SunnovaAdapter : ISunnovaAdapter
    {
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["Sunnova.test.url"];
        private static readonly string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["Sunnova.Auth.Username"];
        private static readonly string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["Sunnova.Auth.Password"];

        private RestClient client
        {
            get
            {
                return new RestClient(url);
            }
        }

        public class TokenResponse
        {
            public string token { get; set; }
        }


        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);        

        public class SunnovaProjects
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Middle_Name { get; set; }
            public string Suffix { get; set; }
            public string Email { get; set; }
            public string Preferred_Contact_Method { get; set; }
            public string Preferred_Language { get; set; }
            public Phone Phone { get; set; }
            public Addresss Address { get; set; }
            public string external_id { get; set; }
        }


        public string GetSunnovaToken()
        {
            try
            {
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));

                var request = new RestRequest($"/gettoken/authentication", Method.GET);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetSunnovaToken Failed. {response.Content}");
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<TokenResponse>(content);

                return ret.token;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public List<SunnovaLead> CreateSunnovaLead(Property property)
        {
            using (var dc = new DataContext())
            {
                SunnovaProjects req = new SunnovaProjects();
                
                Phone phone = new Phone();
                phone.Number = property.GetMainPhoneNumber()?.Replace("-", "");
                phone.Type = "Mobile";

                Addresss addr = new Addresss();
                addr.City = property.City;
                addr.Country = "USA";
                addr.Latitude = property.Latitude;
                addr.Longitude = property.Longitude;
                addr.PostalCode = property.ZipCode;
                addr.State = property.State;
                addr.Street = property.Address1;

                var name = property.GetMainOccupant();

                req.FirstName = name.FirstName;
                req.LastName = name.LastName;
                req.Middle_Name = name.MiddleInitial;
                req.Email = property.GetMainEmailAddress();
                req.Phone = phone;
                req.Address = addr;
                req.external_id = property.ExternalID;
                req.Preferred_Contact_Method = "Email";
                req.Preferred_Language = "English";
                req.Suffix = "";

                string token = GetSunnovaToken();
                
                var request = new RestRequest($"/services/v1.0/leads", Method.POST);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Bearer " + token);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"CreateSunnovaLead Failed. {response.Content}");
                }

                var content = response.Content;
                var ReqJson = new JavaScriptSerializer().Serialize(req);
                var ret = JsonConvert.DeserializeObject<List<SunnovaLead>>(content);

                return ret;
            }
        }

    }
}
