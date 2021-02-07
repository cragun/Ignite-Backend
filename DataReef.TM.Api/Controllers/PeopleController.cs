using DataReef.Core;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Api.Classes;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Api.Classes.ViewModels;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DTOs.Common;
using DataReef.TM.Models.DTOs.Inquiries;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Reporting.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Person
    /// </summary>
    [RoutePrefix("api/v1/people")]
    public class PeopleController : EntityCrudController<Person>
    {
        private readonly IPersonService peopleService;
        private readonly IPersonSettingService settingsService;
        private readonly IBlobService blobService;
        private readonly ICurrentLocationService currentLocationService;
        private readonly IInquiryService inquiryService;
        private readonly Lazy<IOUSettingService> ouSettingService;

        public PeopleController(IPersonService peopleService,
            IPersonSettingService settingsService,
            IBlobService blobService,
            ILogger logger,
            ICurrentLocationService currentLocationService,
            IInquiryService inquiry,
            Lazy<IOUSettingService> ouSettingService)
            : base(peopleService, logger)
        {
            this.peopleService = peopleService;
            this.blobService = blobService;
            this.currentLocationService = currentLocationService;
            this.inquiryService = inquiry;
            this.ouSettingService = ouSettingService;
            this.settingsService = settingsService;
        }


        [Route("{personID:guid}/locations/{date}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLocationsByDate(string personID, string date)
        {
            try
            {
                List<CurrentLocation> ret = currentLocationService.GetCurrentLocationsForPersonAndDate(Guid.Parse(personID), DateTime.Parse(date)).ToList();
                return Ok<List<CurrentLocation>>(ret);

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [Route("locations/latest")]
        [HttpPost]
        [ResponseType(typeof(ICollection<CurrentLocation>))]
        public async Task<IHttpActionResult> GetLatestLocationsForPeopleIds(GenericRequest<List<Guid>> request)
        {
            if (request == null || request.Request == null || request.Request.Count == 0)
            {
                return Ok(new List<CurrentLocation>());
            }

            var ret = currentLocationService.GetLatestLocations(request.Request);
            return Ok(ret);
        }

        [Route("{personID:guid}/image")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetImage(string personID)
        {
            try
            {
                Guid guid = Guid.Parse(personID);

                var blob = blobService.Download(guid);

                if (blob == null || blob.Content == null || blob.Content.Length == 0)
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                MemoryStream ms = new MemoryStream(blob.Content);
                HttpResponseMessage response = new HttpResponseMessage { Content = new StreamContent(ms) };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(blob.ContentType);
                response.Content.Headers.ContentLength = blob.Content.Length;
                return response;
            }
            catch (System.Exception ex)
            {
                HttpResponseMessage ret = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                throw new HttpResponseException(ret);
            }
        }

        [Route("mine")]
        [HttpPost]
        [ResponseType(typeof(List<Person>))]
        public async Task<IHttpActionResult> GetMine(OUMembersRequest request)
        {
            var ret = peopleService.GetMine(request);
            return Ok(ret);
        }

        [Route("me")]
        [HttpGet]
        [ResponseType(typeof(PersonDTO))]
        public async Task<IHttpActionResult> GetMe(string include = "")
        {
            var includeString = "PhoneNumbers";
            if (!string.IsNullOrEmpty(include))
            {
                includeString += $"&{include}";
            }
            var person = peopleService.GetPersonDTO(SmartPrincipal.UserId, includeString);

            return Ok(person);
        }

        [HttpGet]
        [Route("{personID:Guid}/inquiryStats")]
        [ResponseType(typeof(ICollection<InquiryStatisticsForPerson>))]
        public async Task<IHttpActionResult> GetPersonSummaryForCustomStatuses(Guid personID, string dispositions = "", DateTime? specifiedDay = null)
        {
            var inquiryStatuses = dispositions
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

            var data = inquiryService.GetInquiryStatisticsForPerson(personID, inquiryStatuses, specifiedDay);
            return Ok(data);
        }

        [Route("{guid:guid}/mayedit")]
        [HttpGet]
        [ResponseType(typeof(Person))]
        public async Task<IHttpActionResult> GetMayEdit(Guid guid, string include = "", string exclude = "", string fields = "")
        {
            var ret = peopleService.GetMayEdit(guid, true, include, exclude, fields);
            ret.RemoveDefaultExcludedProperties("MayEdit");
            return Ok(ret);
        }

        /// <summary>
        /// Method used to undelete a person, w/ associated User and Credential
        /// This method will also send an email letting the person know that the account has been reactivated
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        [Route("{personID:guid}/reactivate")]
        [HttpPost]
        public async Task<IHttpActionResult> Reactivate(Guid personID)
        {
            peopleService.Reactivate(personID, null);
            return Ok();
        }


        /// <param name="SmartBoardId"></param>
        /// <returns></returns>
        [Route("ActivateUser/{SmartBoardId}")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ActivateUser(string SmartBoardId)
        {
            peopleService.Reactivate(Guid.Empty, SmartBoardId);
            return Ok(true);
        }

        /// <param name="SmartBoardId"></param>
        /// <returns></returns>
        [Route("DeactivateUser/{SmartBoardId}")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> DeactivateUser(string SmartBoardId)
        {
            peopleService.DeactivateUser(SmartBoardId);
            return Ok(true);
        }

        [HttpGet]
        [Route("{personID:guid}/settings/{group?}")]
        [ResponseType(typeof(Dictionary<string, ValueTypePair<SettingValueType, string>>))]
        public async Task<IHttpActionResult> GetAllSettings(Guid personID, PersonSettingGroupType? group = null)
        {
            var data = settingsService.GetSettings(personID, group);
            return Ok(data);
        }

        [HttpPost]
        [Route("{personID:guid}/settings/{group?}")]
        [ResponseType(typeof(Dictionary<string, ValueTypePair<SettingValueType, string>>))]
        public async Task<IHttpActionResult> SetAllSettings([FromUri] Guid personID, [FromBody] Dictionary<string, ValueTypePair<SettingValueType, string>> data, [FromUri] PersonSettingGroupType? group = null)
        {
            settingsService.SetSettings(personID, group, data);
            return Ok();
        }

        [HttpGet]
        [Route("integration/token")]
        [ResponseType(typeof(Jwt))]
        public async Task<IHttpActionResult> GenerateIntegrationToken()
        {
            var token = peopleService.GenerateIntegrationToken();

            return Ok(new Jwt
            {
                Token = token.Guid.ToString(),
                Expiration = token.ExpirationDate.ToUnixTime()
            });
        }

        [HttpGet]
        [Route("integration/webview/parameters")]
        [ResponseType(typeof(GenericResponse<IntegrationParameters>))]
        public async Task<IHttpActionResult> GetWebViewParams()
        {
            var token = peopleService.GenerateIntegrationToken();

            var resp = new GenericResponse<IntegrationParameters>
            {
                Response = new IntegrationParameters
                {
                    LegionToken = token.Guid.ToString()
                }
            };
            return Ok(resp);
        }

        [HttpGet]
        [Route("crm/dispositions")]
        [ResponseType(typeof(GenericResponse<List<CRMDisposition>>))]
        public async Task<IHttpActionResult> GetCRMDispositions()
        {
            return Ok(new GenericResponse<List<CRMDisposition>> { Response = peopleService.CRMGetAvailableNewDispositions() });
        }

        [HttpGet]
        [Route("crm/dispositionfilters")]
        [ResponseType(typeof(List<CRMDisposition>))]
        public async Task<IHttpActionResult> GetNewCRMDispositions()
        {
            return Ok(peopleService.CRMGetAvailableNewDispositions());
        }

        [HttpPost]
        [Route("crm/data")]
        [ResponseType(typeof(PaginatedResult<Property>))]
        public async Task<IHttpActionResult> GetCRMData(CRMFilterRequest request)
        {
            var result = peopleService.CRMGetProperties(request);
            SetupSerialization(result.Data, request.Include, request.Exclude, request.Fields);
            return Ok(result);
        }

        [HttpGet]
        [Route("mysurvey")]
        [ResponseType(typeof(GenericResponse<string>))]
        public async Task<IHttpActionResult> GetMySurvey()
        {
            var result = peopleService.GetUserSurvey(SmartPrincipal.UserId);
            return Ok(new GenericResponse<string> { Response = result });
        }

        [HttpPost]
        [Route("savemysurvey")]
        [ResponseType(typeof(GenericResponse<string>))]
        public async Task<IHttpActionResult> SaveMySurvey([FromBody] GenericRequest<string> request)
        {
            var result = peopleService.SaveUserSurvey(SmartPrincipal.UserId, request.Request);
            return Ok(new GenericResponse<string> { Response = result });
        }

        [HttpGet]
        [Route("surveyurl/{propertyID}")]
        [ResponseType(typeof(GenericResponse<string>))]
        public async Task<IHttpActionResult> GetSurveyUrl(Guid propertyID)
        {
            var result = peopleService.GetSurveyUrl(SmartPrincipal.UserId, propertyID);
            return Ok(new GenericResponse<string> { Response = result });
        }

        [HttpGet]
        [Route("crm/LeadSources/{ouid}")]
        [ResponseType(typeof(GenericResponse<List<CRMLeadSource>>))]
        public async Task<IHttpActionResult> GetCRMLeadSources(Guid ouid)
        {
            return Ok(new GenericResponse<List<CRMLeadSource>> { Response = peopleService.CRMGetAvailableLeadSources(ouid) });
        }


        [Route("PersonClock/{min}")]
        [HttpGet]
        [ResponseType(typeof(PersonClockTime))]
        public async Task<IHttpActionResult> PersonClock(long min)
        {
            var person = peopleService.GetPersonClock(SmartPrincipal.UserId, min);
            return Ok(person);
        }


        [Route("deleteuser/{guid}")]
        [HttpPost]
        public async Task<HttpResponseMessage> DeleteUserByGuid(Guid guid)
        {
            var ret = base.DeleteByGuid(guid);
            peopleService.SBDeactivate(guid);
            return await ret;
        }



        [Route("{ouid}/{date}")]
        [HttpGet]
        [ResponseType(typeof(Person))]
        [AllowAnonymous]
        public async Task<IHttpActionResult> personDetails(Guid ouid, DateTime date)
        {

            var person = peopleService.personDetails(ouid, date);

            return Ok(person);
        }


        [HttpGet]
        [Route("ouassociation/{personid}")]
        [ResponseType(typeof(ICollection<PersonOffboard>))]
        public async Task<IHttpActionResult> GetOuassociationRoleName(Guid personid)
        {
            var persn = peopleService.OuassociationRoleName(personid);
            return Ok(persn);
        }

        [HttpGet]
        //[Route("CalenderApp/{ouid}/{date}")]
        [Route("CalenderApp/{ouid}")]
        [ResponseType(typeof(IEnumerable<Person>))]
        public async Task<IEnumerable<Person>> GetCalendarPageAppointMents(Guid ouid, string CurrentDate, string type)
        {
            var persn = await peopleService.CalendarPageAppointMentsByOuid(ouid, CurrentDate, type);
            return persn;
        }

        [HttpPost]
        [Route("addFavourite")]
        public async Task<IHttpActionResult> InsertFavoritePerson(AppointmentFavouritePerson request)
        {
            var territory = peopleService.InsertFavoritePerson(request.FavouritePersonID);
            return Ok(new GenericResponse<string> { Response = "added successfully" });
        }

        [HttpPost]
        [Route("removeFavourite")]
        public async Task<IHttpActionResult> RemoveFavoritePerson(AppointmentFavouritePerson request)
        {
            peopleService.RemoveFavoritePerson(request.FavouritePersonID);
            return Ok(new GenericResponse<string> { Response = "removed successfully" });
        }

        public override async Task<IEnumerable<Person>> GetMany(string delimitedStringOfGuids, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            include = include + ",OUAssociations";
            var personuser = await base.GetMany(delimitedStringOfGuids, include, exclude, fields, true);
            var list = personuser.Where(x => x.IsDeleted == false).ToList();
            list = list.Where(x => x.OUAssociations.Any(y => (y.RoleType == OURoleType.Member || y.RoleType == OURoleType.Manager) && y.IsDeleted == false)).ToList();
            //  list = list.Where(x => x.AssignedAppointments.Count > 0).ToList();
            return list;
        }

        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            OUsControllerCacheInvalidation();

            return await base.DeleteByGuid(guid);
        }


        public override async Task<Person> Post(Person item)
        {
            OUsControllerCacheInvalidation();

            return await base.Post(item);
        }

        public override async Task<Person> Patch(System.Web.Http.OData.Delta<Person> item)
        {
            OUsControllerCacheInvalidation();

            return await base.Patch(item);
        }

        public override async Task<HttpResponseMessage> Delete(Person item)
        {
            OUsControllerCacheInvalidation();

            return await base.Delete(item);
        }

        public override async Task<ICollection<Person>> PostMany(ICollection<Person> items)
        {
            OUsControllerCacheInvalidation();

            return await base.PostMany(items);
        }

        public override async Task<Person> Put(Person item)
        {
            OUsControllerCacheInvalidation();

            return await base.Put(item);
        }

        /// <summary>
        /// Invalidate cache for OUsController GET methods.
        /// </summary>
        public void OUsControllerCacheInvalidation()
        {
            var cache = Configuration.CacheOutputConfiguration().GetCacheOutputProvider(Request);
            string controllerName = typeof(OUsController).FullName;

            foreach (var key in cache.AllKeys)
            {
                if (key.StartsWith(controllerName, StringComparison.CurrentCultureIgnoreCase))
                {
                    cache.Remove(key);
                }
            }
        }


        [HttpPost]
        [Route("Updateactivity")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Updateactivity(Person prsn)
        {
            var result = peopleService.Updateactivity(prsn);
            return Ok(result);
        }



        [HttpGet]
        [Route("CheckNewMexico")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> CheckNewMexico()
        {
           // using (StreamReader sr = new StreamReader(@"C:\Users\admi\Downloads\mexico.txt"))
            using (StreamReader sr = new StreamReader(@"C:\Users\admi\Downloads\September_2020NM\September_2020NM.txt"))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] authorsList = line.ToString().Split('|');

                    if (authorsList[0] == "DBUSA_Household_ID") continue;
                    NewMexicoData nm = new NewMexicoData();

                    nm.DBUSA_Household_ID = authorsList[0];
                    nm.DBUSA_Individual_ID = authorsList[1];
                    nm.Location_ID = authorsList[2];
                    nm.Full_Name = authorsList[3];
                    nm.Name_Prefix = authorsList[4];
                    nm.First_Name = authorsList[5];
                    nm.Middle_Initial = authorsList[6];
                    nm.Last_Name = authorsList[7];
                    nm.Suffix = authorsList[8];
                    nm.Gender = authorsList[9];
                    nm.Date_Of_Birth_Year = authorsList[10];
                    nm.Date_Of_Birth_Month = authorsList[11];
                    nm.Age = authorsList[12];
                    nm.Age_Code_Description = authorsList[13];
                    nm.Ethnic_Code = authorsList[14];
                    nm.Ethnic_Group = authorsList[15];
                    nm.Religious_Affiliation = authorsList[16];
                    nm.Language_Preference = authorsList[17];
                    nm.Hispanic_Country_Of_Origin = authorsList[18];
                    nm.Assimilation_Code = authorsList[19];
                    nm.Education_Individual = authorsList[20];
                    nm.Occupation_Individual = authorsList[21];
                    nm.Occupation_Individual_Range = authorsList[22];
                    nm.Political_Party_Individual = authorsList[23];
                    nm.Political_Ideology = authorsList[24];
                    nm.Voter_ID = authorsList[25];
                    nm.Veteran_Individual = authorsList[26];
                    nm.Mail_Score_Code = authorsList[27];
                    nm.Standardized_Address = authorsList[28];
                    nm.City = authorsList[29];
                    nm.State = authorsList[30];
                    nm.Zip = authorsList[31];
                    nm.ZIP_Plus4 = authorsList[32];
                    nm.Carrier_Route = authorsList[33];
                    nm.Delivery_Point = authorsList[34];
                    nm.Delivery_Point_Check_Digit = authorsList[35];
                    nm.Address_Street_Number = authorsList[36];
                    nm.Address_Street_Pre = authorsList[37];
                    nm.Address_Street_Name = authorsList[38];
                    nm.Address_Street_Suffix = authorsList[39];
                    nm.Address_Street_Post = authorsList[40];
                    nm.Address_Suite_Name = authorsList[41];
                    nm.Address_Suite_Number = authorsList[42];
                    nm.Line_Of_Travel = authorsList[43];
                    nm.GeoCode_Match_Level = authorsList[44];
                    nm.Latitude = authorsList[45];
                    nm.Longitude = authorsList[46];
                    nm.Time_Zone_Code = authorsList[47];
                    nm.Time_Zone_Description = authorsList[48];
                    nm.Census_Tract = authorsList[49];
                    nm.Census_Block = authorsList[50];
                    nm.County_Code = authorsList[51];
                    nm.County_Name = authorsList[52];
                    nm.FIPS_Code = authorsList[53];
                    nm.CBSA_Code = authorsList[54];
                    nm.CBSA_Description = authorsList[55];
                    nm.Walk_Sequence = authorsList[56];
                    nm.District_Congressional = authorsList[57];
                    nm.District_State_Senate = authorsList[58];
                    nm.District_State_House = authorsList[59];
                    nm.District_State_Legislative = authorsList[60];
                    nm.DMA_Code = authorsList[61];
                    nm.DMA_Zone = authorsList[62];
                    nm.Email_Present = authorsList[63];
                    nm.Email = authorsList[64];
                    nm.Email_02 = authorsList[65];
                    nm.Email_03 = authorsList[66];
                    nm.Email_04 = authorsList[67];
                    nm.Email_05 = authorsList[68];
                    nm.Area_Code = authorsList[69];
                    nm.Phone = authorsList[70];
                    nm.DNC = authorsList[71];
                    nm.Scrubbed_Phoneable = authorsList[72];
                    nm.Area_Code_Cell_Phone = authorsList[73];
                    nm.Cell_Phone = authorsList[74];
                    nm.Dwelling_Type = authorsList[75];
                    nm.Home_Owner_Renter = authorsList[76];
                    nm.Length_Of_Residence = authorsList[77];
                    nm.Length_of_Residence_Code = authorsList[78];
                    nm.Length_Of_Residence_Description = authorsList[79];
                    nm.Recently_Moved_Flag = authorsList[80];
                    nm.Recently_Moved_Year = authorsList[81];
                    nm.Recently_Moved_Month = authorsList[82];
                    nm.Household_Size = authorsList[83];
                    nm.Household_Rank = authorsList[84];
                    nm.Number_Of_Adults = authorsList[85];
                    nm.Young_Adult_In_Household = authorsList[86];
                    nm.Senior_Adult_In_Household = authorsList[87];
                    nm.Generations_In_Household = authorsList[88];
                    nm.Marital_Status = authorsList[89];
                    nm.Recent_Divorce = authorsList[90];
                    nm.Single_Parent = authorsList[91];
                    nm.Children_Present = authorsList[92];
                    nm.Number_Children = authorsList[93];
                    nm.Childrens_Age_00_02 = authorsList[94];
                    nm.Childrens_Age_00_02_Male = authorsList[95];
                    nm.Childrens_Age_00_02_Female = authorsList[96];
                    nm.Childrens_Age_00_02_Unknown = authorsList[97];
                    nm.Childrens_Age_03_05 = authorsList[98];
                    nm.Childrens_Age_03_05_Male = authorsList[99];
                    nm.Childrens_Age_03_05_Female = authorsList[100];
                    nm.Childrens_Age_03_05_Unknown = authorsList[101];
                    nm.Childrens_Age_06_10 = authorsList[102];
                    nm.Childrens_Age_06_10_Male = authorsList[103];
                    nm.Childrens_Age_06_10_Female = authorsList[104];
                    nm.Childrens_Age_06_10_Unknown = authorsList[105];
                    nm.Childrens_Age_11_15 = authorsList[106];
                    nm.Childrens_Age_11_15_Male = authorsList[107];
                    nm.Childrens_Age_11_15_Female = authorsList[108];
                    nm.Childrens_Age_11_15_Unknown = authorsList[109];
                    nm.Childrens_Age_16_17 = authorsList[110];
                    nm.Childrens_Age_16_17_Male = authorsList[111];
                    nm.Childrens_Age_16_17_Female = authorsList[112];
                    nm.Childrens_Age_16_17_Unknown = authorsList[113];
                    nm.Child_Near_High_School_Graduation = authorsList[114];
                    nm.College_Graduate = authorsList[115];
                    nm.Christian_Families = authorsList[116];
                    nm.Business_Owner = authorsList[117];
                    nm.SOHO = authorsList[118];
                    nm.Working_Woman = authorsList[119];
                    nm.Veteran_Present_In_Household = authorsList[120];
                    nm.Credit_Card_User = authorsList[121];
                    nm.Mail_Order_Donor = authorsList[122];
                    nm.Mail_Order_Buyer = authorsList[123];
                    nm.Mail_Order_Responder = authorsList[124];
                    nm.TV_Satellite_Dish = authorsList[125];
                    nm.Pet_Owner = authorsList[126];
                    nm.Cat_Owner = authorsList[127];
                    nm.Dog_Owner = authorsList[128];
                    nm.Other_Pet_Owner = authorsList[129];
                    nm.New_Teen_Driver = authorsList[130];
                    nm.New_Teen_Driver_Gender = authorsList[131];
                    nm.Boat_Owner = authorsList[132];
                    nm.Truck_Owner = authorsList[133];
                    nm.Motorcycle_Owner = authorsList[134];
                    nm.RV_Owner = authorsList[135];
                    nm.Auto_Buy_New = authorsList[136];
                    nm.Auto_Buy_Used = authorsList[137];
                    nm.Auto_Buy_Used_5_Months_Or_Less = authorsList[138];
                    nm.Auto_Buy_Used_6_Months_Or_More = authorsList[139];
                    nm.Median_HseHld_Income = authorsList[140];
                    nm.Median_HseHld_Income_Code = authorsList[141];
                    nm.Median_HseHld_Income_Description = authorsList[142];
                    nm.Income = authorsList[143];
                    nm.Income_Code = authorsList[144];
                    nm.Income_Description = authorsList[145];
                    nm.NetWorth = authorsList[146];
                    nm.NetWorth_Code = authorsList[147];
                    nm.NetWorth_Description = authorsList[148];
                    nm.Millionaire = authorsList[149];
                    nm.Unsecured_Credit_Capacity = authorsList[150];
                    nm.Unsecured_Credit_Capacity_Code = authorsList[151];
                    nm.Unsecured_Credit_Capacity_Description = authorsList[152];
                    nm.Discretionary_Income = authorsList[153];
                    nm.Discretionary_Income_Code = authorsList[154];
                    nm.Discretionary_Income_Description = authorsList[155];
                    nm.Donor_Capacity = authorsList[156];
                    nm.Donor_Capacity_Code = authorsList[157];
                    nm.Donor_Capacity_Description = authorsList[158];
                    nm.Investment_Properties_Owned = authorsList[159];
                    nm.Estimated_Area_Credit_Rating = authorsList[160];
                    nm.CRA_Income_Classification_Code = authorsList[161];
                    nm.Credit_Range_New_Credit = authorsList[162];
                    nm.Lines_Of_Credit = authorsList[163];
                    nm.Home_Property_Type = authorsList[164];
                    nm.Home_Property_Type_Detail = authorsList[165];
                    nm.Home_Value = authorsList[166];
                    nm.Home_Value_Code = authorsList[167];
                    nm.Home_Value_Description = authorsList[168];
                    nm.Median_Home_Value = authorsList[169];
                    nm.Median_Home_Value_Code = authorsList[170];
                    nm.Median_Home_Value_Description = authorsList[171];
                    nm.Home_Built_Year = authorsList[172];
                    nm.Home_Built_Year_Code = authorsList[173];
                    nm.Home_Built_Year_Description = authorsList[174];
                    nm.Home_Square_Footage = authorsList[175];
                    nm.Home_Square_Footage_Range = authorsList[176];
                    nm.Land_Square_Footage = authorsList[177];
                    nm.Number_Of_Bedrooms = authorsList[178];
                    nm.Number_Of_Bathrooms = authorsList[179];
                    nm.Home_Air_Conditioning = authorsList[180];
                    nm.Home_Swimming_Pool = authorsList[181];
                    nm.Home_Sewer = authorsList[182];
                    nm.Home_Water = authorsList[183];
                    nm.Recent_Home_Buyer = authorsList[184];
                    nm.Recent_Mortgage_Borrower = authorsList[185];
                    nm.New_Home_Owner_Flag = authorsList[186];
                    nm.Most_Recent_Home_Purchase_Date_Flag = authorsList[187];
                    nm.Home_Purchase_Date = authorsList[188];
                    nm.Home_Purchase_Year = authorsList[189];
                    nm.Home_Purchase_Amount = authorsList[190];
                    nm.Home_Assessed_Value = authorsList[191];
                    nm.Home_Total_Loan = authorsList[192];
                    nm.Home_Loan_Date_1_Year = authorsList[193];
                    nm.Home_Loan_Date_1_Month = authorsList[194];
                    nm.Home_Loan_Date_2_Year = authorsList[195];
                    nm.Home_Loan_Date_2_Month = authorsList[196];
                    nm.Home_Loan_Date_3_Year = authorsList[197];
                    nm.Home_Loan_Date_3_Month = authorsList[198];
                    nm.Home_Loan_Amount_1 = authorsList[199];
                    nm.Home_Loan_Amount_2 = authorsList[200];
                    nm.Home_Loan_Amount_3 = authorsList[201];
                    nm.Home_Loan_Type_1 = authorsList[202];
                    nm.Home_Loan_Type_2 = authorsList[203];
                    nm.Home_Loan_Type_3 = authorsList[204];
                    nm.Home_Loan_Interest_Rate_1 = authorsList[205];
                    nm.Home_Loan_Interest_Rate_2 = authorsList[206];
                    nm.Home_Loan_Interest_Rate_3 = authorsList[207];
                    nm.Home_Loan_Interest_Rate_Type_1 = authorsList[208];
                    nm.Home_Loan_Interest_Rate_Type_2 = authorsList[209];
                    nm.Home_Loan_Interest_Rate_Type_3 = authorsList[210];
                    nm.Home_Loan_Transaction_Type_1 = authorsList[211];
                    nm.Home_Loan_Transaction_Type_2 = authorsList[212];
                    nm.Home_Loan_Transaction_Type_3 = authorsList[213];
                    nm.Home_Loan_Lender_Name_1 = authorsList[214];
                    nm.Home_Loan_Lender_Name_2 = authorsList[215];
                    nm.Home_Loan_Lender_Name_3 = authorsList[216];
                    nm.Home_Loan_To_Value_Code = authorsList[217];
                    nm.Home_Equity_Available_Code = authorsList[218];
                    nm.Home_Equity_Available_Description = authorsList[219];
                    nm.Donor_Charitable = authorsList[220];
                    nm.Donor_Animal_Welfare = authorsList[221];
                    nm.Donor_Arts_Cultural = authorsList[222];
                    nm.Donor_Childrens = authorsList[223];
                    nm.Donor_Environment_Wildlife = authorsList[224];
                    nm.Donor_Health = authorsList[225];
                    nm.Donor_International_Aid = authorsList[226];
                    nm.Donor_Political = authorsList[227];
                    nm.Donor_Political_Conservative = authorsList[228];
                    nm.Donor_Political_Liberal = authorsList[229];
                    nm.Donor_Religious = authorsList[230];
                    nm.Donor_Veterans = authorsList[231];
                    nm.CC_American_Express = authorsList[232];
                    nm.CC_American_Express_Gold_Platinum = authorsList[233];
                    nm.CC_Discover = authorsList[234];
                    nm.CC_Visa = authorsList[235];
                    nm.CC_Mastercard = authorsList[236];
                    nm.CC_Bank = authorsList[237];
                    nm.CC_Gas_Dept_Retail = authorsList[238];
                    nm.CC_Travel_Entertainment = authorsList[239];
                    nm.CC_Unknown = authorsList[240];
                    nm.CC_Gold_Platinum = authorsList[241];
                    nm.CC_Premium = authorsList[242];
                    nm.CC_Upscale_Dept = authorsList[243];
                    nm.CC_New_Issue = authorsList[244];
                    nm.Home_Living = authorsList[245];
                    nm.DIY_Living = authorsList[246];
                    nm.Sporty_Living = authorsList[247];
                    nm.Upscale_Living = authorsList[248];
                    nm.Cultural_Artistic_Living = authorsList[249];
                    nm.Highbrow = authorsList[250];
                    nm.Common_Living = authorsList[251];
                    nm.Professional_Living = authorsList[252];
                    nm.Broader_Living = authorsList[253];
                    nm.Arts = authorsList[254];
                    nm.Theater_Performing_Arts = authorsList[255];
                    nm.Food_Wines = authorsList[256];
                    nm.Foods_Natural = authorsList[257];
                    nm.Cooking_General = authorsList[258];
                    nm.Cooking_Gourmet = authorsList[259];
                    nm.Aviation = authorsList[260];
                    nm.Auto_Work = authorsList[261];
                    nm.Automotive_Buff = authorsList[262];
                    nm.Beauty_Cosmetics = authorsList[263];
                    nm.Career = authorsList[264];
                    nm.Career_Improvement = authorsList[265];
                    nm.Parenting = authorsList[266];
                    nm.Childrens_Interests = authorsList[267];
                    nm.Grandchildren = authorsList[268];
                    nm.Community_Charities = authorsList[269];
                    nm.Religious_Inspirational = authorsList[270];
                    nm.Crafts = authorsList[271];
                    nm.Photography = authorsList[272];
                    nm.Sewing_Knitting_Needlework = authorsList[273];
                    nm.Collector_Avid = authorsList[274];
                    nm.Collectibles_Grouping = authorsList[275];
                    nm.Collectibles_General = authorsList[276];
                    nm.Collectibles_Stamps = authorsList[277];
                    nm.Collectibles_Coins = authorsList[278];
                    nm.Collectibles_Arts = authorsList[279];
                    nm.Collectibles_Antiques = authorsList[280];
                    nm.Collectibles_Sports_Memorabilia = authorsList[281];
                    nm.Education_Online = authorsList[282];
                    nm.Exercise_Aerobic = authorsList[283];
                    nm.Exercise_Running_Jogging = authorsList[284];
                    nm.Exercise_Walking = authorsList[285];
                    nm.High_Tech_General = authorsList[286];
                    nm.Games_Computer_Games = authorsList[287];
                    nm.Games_Video_Games = authorsList[288];
                    nm.Games_Board_Puzzles = authorsList[289];
                    nm.Gaming_Casino = authorsList[290];
                    nm.Consumer_Electronics = authorsList[291];
                    nm.Environmental_Issues = authorsList[292];
                    nm.Gardening = authorsList[293];
                    nm.Home_Furnishings_Decorating = authorsList[294];
                    nm.House_Plant = authorsList[295];
                    nm.Home_Improvement_Grouping = authorsList[296];
                    nm.Home_Improvement = authorsList[297];
                    nm.Home_Improvement_DIY = authorsList[298];
                    nm.Health_Medical = authorsList[299];
                    nm.Dieting_Weight_Loss = authorsList[300];
                    nm.Self_Improvement = authorsList[301];
                    nm.Investments_Grouping = authorsList[302];
                    nm.Investments_Foreign = authorsList[303];
                    nm.Investments_Personal = authorsList[304];
                    nm.Investments_Real_Estate = authorsList[305];
                    nm.Investments_Stocks_Bonds = authorsList[306];
                    nm.Money_Seekers = authorsList[307];
                    nm.Sweepstakes_Contests = authorsList[308];
                    nm.Music_Home_Stereo = authorsList[309];
                    nm.Music_Player = authorsList[310];
                    nm.Music_Collector = authorsList[311];
                    nm.Music_Listener = authorsList[312];
                    nm.Movie_Grouping = authorsList[313];
                    nm.Movie_Music_General = authorsList[314];
                    nm.Movie_Collector = authorsList[315];
                    nm.Reading_General = authorsList[316];
                    nm.Reading_Audio_Books = authorsList[317];
                    nm.Reading_Magazines = authorsList[318];
                    nm.Reading_Religious_Inspirational = authorsList[319];
                    nm.Reading_Science_Fiction = authorsList[320];
                    nm.Reading_Financial_Newsletter_Subscribers = authorsList[321];
                    nm.Current_Affairs_Politics = authorsList[322];
                    nm.History_Military = authorsList[323];
                    nm.Outdoor_Grouping = authorsList[324];
                    nm.Outdoor_Enthusiast_General = authorsList[325];
                    nm.Outdoor_Fishing = authorsList[326];
                    nm.Outdoor_Boating_Sailing = authorsList[327];
                    nm.Outdoor_Camping_Hiking = authorsList[328];
                    nm.Outdoor_Hunting_Shooting = authorsList[329];
                    nm.Outdoor_Scuba_Diving = authorsList[330];
                    nm.Spectator_Sports_Grouping = authorsList[331];
                    nm.Spectator_Sports_General = authorsList[332];
                    nm.Spectator_Sports_Baseball = authorsList[333];
                    nm.Spectator_Sports_Basketball = authorsList[334];
                    nm.Spectator_Sports_Football = authorsList[335];
                    nm.Spectator_Sports_Hockey = authorsList[336];
                    nm.Spectator_Sports_Racing = authorsList[337];
                    nm.Spectator_Sports_Soccer = authorsList[338];
                    nm.Spectator_Sports_TV_Sports = authorsList[339];
                    nm.Spectator_NASCAR = authorsList[340];
                    nm.Smoking_Tobacco = authorsList[341];
                    nm.Sports_Grouping = authorsList[342];
                    nm.Sports_Equestrian = authorsList[343];
                    nm.Sports_Golf = authorsList[344];
                    nm.Sports_Motorcycling = authorsList[345];
                    nm.Sports_Skiing = authorsList[346];
                    nm.Sports_Tennis = authorsList[347];
                    nm.Travel_Grouping = authorsList[348];
                    nm.Travel_Cruises = authorsList[349];
                    nm.Travel_Domestic = authorsList[350];
                    nm.Travel_International = authorsList[351];
                    nm.Travel_RV = authorsList[352];
                    nm.Science_Space = authorsList[353];
                    nm.Woodworking = authorsList[354];
                    nm.Buyer_Books = authorsList[355];
                    nm.Buyer_Crafts_Hobbies = authorsList[356];
                    nm.Buyer_Gardening_Farming = authorsList[357];
                    nm.Buyer_Jewelry = authorsList[358];
                    nm.Buyer_Luggage = authorsList[359];
                    nm.Buyer_Online = authorsList[360];
                    nm.Buyer_Membership_Club = authorsList[361];
                    nm.Buyer_Merchandise_Male = authorsList[362];
                    nm.Buyer_Merchandise_Female = authorsList[363];
                    nm.Buyer_Health_Beauty = authorsList[364];
                    nm.Buyer_Childrens_Babycare = authorsList[365];
                    nm.Buyer_Childrens_Learning_Toys = authorsList[366];
                    nm.Buyer_Childrens_Back_To_School = authorsList[367];
                    nm.Apparel_Childrens = authorsList[368];
                    nm.Apparel_Infant_Toddlers = authorsList[369];
                    nm.Apparel_Womens = authorsList[370];
                    nm.Apparel_Womens_Petite = authorsList[371];
                    nm.Apparel_Womens_Plus_Size = authorsList[372];
                    nm.Apparel_Womens_Young = authorsList[373];
                    nm.Apparel_Mens = authorsList[374];
                    nm.Apparel_Mens_Big_Tall = authorsList[375];
                    nm.Apparel_Mens_Young = authorsList[376];
                    nm.Auto_Parts_Accessories = authorsList[377];
                    nm.Military_Memorabilia_Weapons = authorsList[378];
                    nm.Musical_Instruments = authorsList[379];
                    nm.Photography_Video_Equipment = authorsList[380];
                    nm.Sports_Leisure = authorsList[381];
                    nm.Value_Hunter = authorsList[382];
                    nm.Last_Update_Date = authorsList[383];
                    nm.Source_Data = authorsList[384];
                    nm.Production_Date = authorsList[385];

                    using (DataContext dc = new DataContext())
                    {
                        dc.NewMexicoData.Add(nm);
                        dc.SaveChanges();
                    }
                }
            }
            return Ok("Done");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("sb/GetUserRole/{usrSbid}")]
        public async Task<IHttpActionResult> GetUserRoleWithApikey(string usrSbid)
        {
            using (var dc = new DataContext())
            {
                var people = dc.People.FirstOrDefault(x => x.SmartBoardID.Equals(usrSbid, StringComparison.InvariantCultureIgnoreCase));

                if (people == null)
                {
                    throw new Exception("No account linked to the specified Smartboardid");
                }

                var persn = peopleService.OuassociationRoleName(people.Guid);
                List<PersonOffboard> prolist = new List<PersonOffboard>();
                foreach (var i in persn)
                {
                    PersonOffboard pro = new PersonOffboard();

                    var sbSettings = ouSettingService.Value.GetSettingsByOUID(i.OUID)?.FirstOrDefault(x => x.Name == "Integrations.Options.Selected")?.GetValue<ICollection<SelectedIntegrationOption>>()?.FirstOrDefault(s => s.Data?.SMARTBoard != null)?.Data?.SMARTBoard;

                    pro.Apikey = sbSettings != null ? sbSettings?.ApiKey : "";
                    pro.AssociateOuName = i.AssociateOuName;
                    pro.OUID = i.OUID;
                    pro.RoleID = i.RoleID;
                    pro.RoleName = i.RoleName;
                    prolist.Add(pro);
                }

                return Ok(prolist);
            }
        }
    }
}
