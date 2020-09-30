using DataReef.Core.Attributes;
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

namespace DataReef.TM.Services.Services.FinanceAdapters.Sunlight
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ISunlightAdapter))]
    public class SunlightAdapter : ISunlightAdapter
    {
        //private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["Sunlight.test.url"];
        //private static readonly string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Auth.Username"];
        //private static readonly string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Auth.Password"];
        //private static readonly string Username = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Username"];
        //private static readonly string Password = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Password"];
        //private static readonly string FrameUrl = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Frame.Url"];

        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Live.url"];
        private static readonly string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Live.Auth.Username"];
        private static readonly string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Live.Auth.Password"];
        private static readonly string Username = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Live.Username"];
        private static readonly string Password = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Live.Password"];
        private static readonly string FrameUrl = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Live.Frame.Url"];

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

        public string GetState(string shortState, string type)
        {
            string stateName = "";

            var list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("AL", "Alabama"));
            list.Add(new KeyValuePair<string, string>("AK", "Alaska"));
            list.Add(new KeyValuePair<string, string>("AZ", "Arizona"));
            list.Add(new KeyValuePair<string, string>("AR", "Arkansas"));
            list.Add(new KeyValuePair<string, string>("CA", "California"));
            list.Add(new KeyValuePair<string, string>("CO", "Colorado"));
            list.Add(new KeyValuePair<string, string>("CT", "Connecticut"));
            list.Add(new KeyValuePair<string, string>("DE", "Delaware"));
            list.Add(new KeyValuePair<string, string>("FL", "Florida"));
            list.Add(new KeyValuePair<string, string>("GA", "Georgia"));
            list.Add(new KeyValuePair<string, string>("HI", "Hawaii"));
            list.Add(new KeyValuePair<string, string>("ID", "Idaho"));
            list.Add(new KeyValuePair<string, string>("IL", "Illinois"));
            list.Add(new KeyValuePair<string, string>("IN", "Indiana"));
            list.Add(new KeyValuePair<string, string>("IA", "Iowa"));
            list.Add(new KeyValuePair<string, string>("KS", "Kansas"));
            list.Add(new KeyValuePair<string, string>("KY", "Kentucky"));
            list.Add(new KeyValuePair<string, string>("LA", "Louisiana"));
            list.Add(new KeyValuePair<string, string>("ME", "Maine"));
            list.Add(new KeyValuePair<string, string>("MD", "Maryland"));
            list.Add(new KeyValuePair<string, string>("MA", "Massachusetts"));
            list.Add(new KeyValuePair<string, string>("MI", "Michigan"));
            list.Add(new KeyValuePair<string, string>("MN", "Minnesota"));
            list.Add(new KeyValuePair<string, string>("MS", "Mississippi"));
            list.Add(new KeyValuePair<string, string>("MO", "Missouri"));
            list.Add(new KeyValuePair<string, string>("MT", "Montana"));
            list.Add(new KeyValuePair<string, string>("NE", "Nebraska"));
            list.Add(new KeyValuePair<string, string>("NV", "Nevada"));
            list.Add(new KeyValuePair<string, string>("NH", "New Hampshire"));
            list.Add(new KeyValuePair<string, string>("NJ", "New Jersey"));
            list.Add(new KeyValuePair<string, string>("NM", "New Mexico"));
            list.Add(new KeyValuePair<string, string>("NY", "New York"));
            list.Add(new KeyValuePair<string, string>("NC", "North Carolina"));
            list.Add(new KeyValuePair<string, string>("ND", "North Dakota"));
            list.Add(new KeyValuePair<string, string>("OH", "Ohio"));
            list.Add(new KeyValuePair<string, string>("OK", "Oklahoma"));
            list.Add(new KeyValuePair<string, string>("OR", "Oregon"));
            list.Add(new KeyValuePair<string, string>("PA", "Pennsylvania"));
            list.Add(new KeyValuePair<string, string>("RI", "Rhode Island"));
            list.Add(new KeyValuePair<string, string>("SC", "South Carolina"));
            list.Add(new KeyValuePair<string, string>("SD", "South Dakota"));
            list.Add(new KeyValuePair<string, string>("TN", "Tennessee"));
            list.Add(new KeyValuePair<string, string>("TX", "Texas"));
            list.Add(new KeyValuePair<string, string>("UT", "Utah"));
            list.Add(new KeyValuePair<string, string>("VT", "Vermont"));
            list.Add(new KeyValuePair<string, string>("VA", "Virginia"));
            list.Add(new KeyValuePair<string, string>("WA", "Washington"));
            list.Add(new KeyValuePair<string, string>("WV", "West Virginia"));
            list.Add(new KeyValuePair<string, string>("WI", "Wisconsin"));
            list.Add(new KeyValuePair<string, string>("WY", "Wyoming"));



            if (type.Equals("shortState"))
            {
                stateName = list.Where(x => x.Key.ToLower() == shortState.ToLower()).FirstOrDefault().Value;
            }
            else if (type.Equals("fullState"))
            {
                stateName = list.Where(x => x.Value.ToLower() == shortState.ToLower()).FirstOrDefault().Key;
            }

            return stateName;
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

        public class SunlightProjects
        {
            public string returnCode { get; set; }
            public List<Projects> projects { get; set; }
        }

        public class projectIdsmodel
        {
            public string projectIds { get; set; }
        }

        public class SunProject
        {
            public List<Projects> projects { get; set; }
        }

        public class SunlightProducts
        {
            public string returnCode { get; set; }
            public List<Products> products { get; set; }
        }

        public void fetchProduct(Projects data, string token)
        {
            SunlightProducts req = new SunlightProducts();
            Products product = new Products();

            product.loanType = "Single";
            product.term = data.term;
            product.apr = data.apr;
            product.isACH = true;
            product.stateName = data.installStateName;

            req.products = new List<Products>();
            req.products.Add(product);

            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));
            var request = new RestRequest($"/product/fetchproducts/", Method.POST);
            request.AddJsonBody(req);
            request.AddHeader("Authorization", "Basic " + svcCredentials);
            request.AddHeader("SFAccessToken", "Bearer " + token);

            var response = client.Execute(request);

            var content = response.Content;
            var ret = JsonConvert.DeserializeObject<SunlightProducts>(content);

            if (!ret.returnCode.Equals("200"))
            {
                throw new ApplicationException("Sorry, there seems to be a problem, Error 390. No products found. Please contact Sunlight Financial at (888) 850-3359 for assistance.");
            }

        }

        //  public string CreateSunlightAccount(Property property, FinancePlanDefinition financePlan)
        public string CreateSunlightAccount(Property property, FinancePlanDefinition financePlan)
        {
            //try
            //{
            using (var dc = new DataContext())
            {
                var proposal = dc.Proposal.Where(x => x.PropertyID == property.Guid && !x.IsDeleted).Select(y => y.Guid).FirstOrDefault();
                var proposalfianaceplan = dc.FinancePlans.Where(x => x.SolarSystemID == proposal && !x.IsDeleted).FirstOrDefault();
                var resp = JsonConvert.DeserializeObject<LoanResponse>(proposalfianaceplan.ResponseJSON);

                SunlightProjects req = new SunlightProjects();
                Projects project = new Projects();

                project.term = financePlan.TermInYears * 12;
                project.apr = double.Parse(String.Format("{0:0.00}", financePlan.Apr));
                project.isACH = true;
                project.installStreet = property.Address1 /*+ ", " + property.StreetName*/;
                project.installCity = property.City;
                project.installStateName = GetState(property.State, "shortState");
                project.installZipCode = property.ZipCode;
                project.sendLinkEmail = false;

                Applicants applicnt = new Applicants();

                var name = property.GetMainOccupant();

                project.applicants = new List<Applicants>();
                applicnt.firstName = name.FirstName;
                applicnt.lastName = name.LastName;
                applicnt.email = property.GetMainEmailAddress();
                applicnt.phone = property.GetMainPhoneNumber()?.Replace("-", "");
                applicnt.isPrimary = true;
                project.applicants.Add(applicnt);

                Quotes quote = new Quotes();

                project.quotes = new List<Quotes>();
                quote.loanAmount = resp.AmountFinanced.ToString();
                project.quotes.Add(quote);

                req.projects = new List<Projects>();
                req.projects.Add(project);

                string token = GetSunlightToken();
                //string token = "00DJ0000003HLkU!AR4AQA_G63cMTXNK9VDMFKnabv6OOH6yPOEmRG_n5TKKFfOGQghYAEPmzKs0.q2X6.EcfDI48TeOyvmi8wW5MHd77ST.MO19";


                fetchProduct(project, token);


                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));
                var request = new RestRequest($"/pricing/createaccount/", Method.POST);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                request.AddHeader("SFAccessToken", "Bearer " + token);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"CreateSunlightApplicant Failed. {response.Content}");
                }

                var content = response.Content;

                var ReqJson = new JavaScriptSerializer().Serialize(req);

                using (var db = new DataContext())
                {
                    var fianacepln = db.FinancePlans.Where(x => x.SolarSystemID == proposal && !x.IsDeleted).FirstOrDefault();
                    fianacepln.SunlightReqJson = ReqJson;
                    fianacepln.SunlightResponseJson = content;
                    db.SaveChanges();
                }


                var ret = JsonConvert.DeserializeObject<SunlightProjects>(content);

                token = Uri.EscapeDataString(token);
                string hashId = ret.projects?.FirstOrDefault().hashId;
                if (!string.IsNullOrEmpty(hashId))
                {
                    hashId = Uri.EscapeDataString(hashId);
                }


                string frame = FrameUrl.Replace("{tokenid}", token)
                                       .Replace("{hashid}", "&pid=" + hashId);




                return frame;
            }
            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}
        }





        public SunlightResponse GetSunlightloanstatus(Guid proposal)
        {
            SunlightResponse snresponse = new SunlightResponse();
            try
            {
                using (var dc = new DataContext())
                {
                    var proposalfianaceplan = dc.FinancePlans.Where(x => x.SolarSystemID == proposal && !x.IsDeleted).FirstOrDefault();
                    var resp = JsonConvert.DeserializeObject<SunlightProjects>(proposalfianaceplan.SunlightResponseJson);
                    string projectid = resp.projects?.FirstOrDefault().id;

                    //string requesttxt = "{'projectIds': '" + projectid + "'}";
                    string token = GetSunlightToken();

                    string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));
                    var request = new RestRequest($"/getstatus/status/", Method.POST);

                    projectIdsmodel prid = new projectIdsmodel();
                    prid.projectIds = projectid;


                    request.AddJsonBody(prid);
                    request.AddHeader("Authorization", "Basic " + svcCredentials);
                    request.AddHeader("SFAccessToken", "Bearer " + token);

                    var response = client.Execute(request);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ApplicationException($"GetSunlightloanstatus Failed. {response.Content}");
                    }

                    var content = response.Content;

                    var ret = JsonConvert.DeserializeObject<SunlightProjects>(content);


                    var ReqJson = new JavaScriptSerializer().Serialize(prid);

                    using (var db = new DataContext())
                    {
                        var fianacepln = db.FinancePlans.Where(x => x.SolarSystemID == proposal && !x.IsDeleted).FirstOrDefault();
                        fianacepln.SunlightReqJson = ReqJson;
                        fianacepln.SunlightResponseJson = content;
                        db.SaveChanges();
                    }                    

                    if (ret.returnCode != "200")
                    {
                        var error =  JsonConvert.DeserializeObject<SunlightError>(content);
                        snresponse.returnCode = error.returnCode;
                        snresponse.projectstatus = error.error?.FirstOrDefault()?.errorMessage;
                    }
                    else
                    {
                        string returnstr = ret.projects?.FirstOrDefault()?.projectStatus;
                        snresponse.returnCode = ret.returnCode;

                        if (!string.IsNullOrEmpty(returnstr))
                        {
                            snresponse.projectstatus = "Approved";
                        }
                        else
                        {
                            snresponse.projectstatus = "Declined";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                snresponse.returnCode = "500";
                snresponse.projectstatus = ex.Message;
            }
            return snresponse;
        }



        public SunlightResponse Sunlightsendloandocs(Guid proposal)
        {
            SunlightResponse snresponse = new SunlightResponse();
            try
            {
                using (var dc = new DataContext())
                {
                    var proposalfianaceplan = dc.FinancePlans.Where(x => x.SolarSystemID == proposal && !x.IsDeleted).FirstOrDefault();
                    var resp = JsonConvert.DeserializeObject<SunlightProjects>(proposalfianaceplan.SunlightResponseJson);
                    string projectid = resp.projects?.FirstOrDefault()?.id;

                    // string requesttxt = "{'projects': [{'id': '" + projectid + "'}]}";
                    SunlightProjects req = new SunlightProjects();
                    Projects project = new Projects();
                    project.id = projectid;
                    req.projects = new List<Projects>();
                    req.projects.Add(project);

                    string token = GetSunlightToken();

                    string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));
                    var request = new RestRequest($"/sendloandocs/request/", Method.POST);
                    request.AddJsonBody(req);
                    request.AddHeader("Authorization", "Basic " + svcCredentials);
                    request.AddHeader("SFAccessToken", "Bearer " + token);

                    var response = client.Execute(request);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ApplicationException($"Sunlightsendloandocs Failed. {response.Content}");
                    }

                    var content = response.Content;

                    var ReqJson = new JavaScriptSerializer().Serialize(req);
                    using (var db = new DataContext())
                    {
                        var fianacepln = db.FinancePlans.Where(x => x.SolarSystemID == proposal && !x.IsDeleted).FirstOrDefault();
                        fianacepln.SunlightReqJson = ReqJson;
                        fianacepln.SunlightResponseJson = content;
                        db.SaveChanges();
                    }

                    var ret = JsonConvert.DeserializeObject<SunlightProjects>(content);
                   // string returnstr = ret.projects?.FirstOrDefault()?.projectStatus + ret.projects?.FirstOrDefault()?.message;
                    if (ret.returnCode != "200")
                    {
                        var error = JsonConvert.DeserializeObject<SunlightError>(content);
                        snresponse.returnCode = error.returnCode;
                        snresponse.projectstatus = error.error?.FirstOrDefault()?.errorMessage;
                    }
                    else
                    {
                        string returnstr = ret.projects?.FirstOrDefault()?.projectStatus + ret.projects?.FirstOrDefault()?.message;
                        snresponse.returnCode = ret.returnCode;

                        if (!string.IsNullOrEmpty(returnstr))
                        {
                            snresponse.projectstatus = "Approved";
                        }
                        else
                        {
                            snresponse.projectstatus = "Declined";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                snresponse.returnCode = "500";
                snresponse.projectstatus = ex.Message;
            }
            return snresponse;
        }

        //public string CreateSunlightApplicant(string fname, string lname, string email, string phone, string street, string city, string state, string zipcode)
        //{
        //    try
        //    {

        //        state = GetState(state, "shortState");

        //        SunlightProjects req = new SunlightProjects();
        //        Projects project = new Projects();
        //        Applicants applicnt = new Applicants();

        //        applicnt.firstName = fname;
        //        applicnt.lastName = lname;
        //        applicnt.email = email;
        //        applicnt.phone = phone;
        //        applicnt.isPrimary = true;

        //        project.applicants = new List<Applicants>();
        //        project.applicants.Add(applicnt);
        //        project.installStreet = street;
        //        project.installCity = city;
        //        project.installStateName = state;
        //        project.installZipCode = zipcode;

        //        req.Projects = new List<Projects>();
        //        req.Projects.Add(project);

        //        string token = GetSunlightToken();
        //        string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));
        //        var request = new RestRequest($"/applicant/create/", Method.POST);
        //        request.AddJsonBody(req);
        //        request.AddHeader("Authorization", "Basic " + svcCredentials);
        //        request.AddHeader("SFAccessToken", "Bearer " + token);

        //        var response = client.Execute(request);

        //        if (response.StatusCode != HttpStatusCode.OK)
        //        {
        //            throw new ApplicationException($"CreateSunlightApplicant Failed. {response.Content}");
        //        }

        //        var content = response.Content;
        //        var ret = JsonConvert.DeserializeObject<SunlightProjects>(content);
        //        string frame = FrameUrl.Replace("{tokenid}", token).Replace("{hashid}", "&pid=" + ret.Projects?.FirstOrDefault().hashId);

        //        return frame;
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }

        //}
    }
}
