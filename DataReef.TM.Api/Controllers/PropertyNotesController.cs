using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Properties;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.Auth.Helpers;
using System.Threading.Tasks;


namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Properties
    /// </summary>
    [RoutePrefix("api/v1/propertynotes")]
    public class PropertyNotesController : EntityCrudController<PropertyNote>
    {
        private readonly IPropertyNoteService _propertyNoteService;

        public PropertyNotesController(IPropertyNoteService propertyNoteService,
                                    ILogger logger)
            : base(propertyNoteService, logger)
        {
            this._propertyNoteService = propertyNoteService;
        }


        /// <summary>
        /// Gets all PropertyNotes found for the specified propertyID
        /// </summary>
        /// <param name="propertyID"></param>
        /// <returns></returns>
        [Route("property/{propertyID}")]
        [ResponseType(typeof(IEnumerable<PropertyNote>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllForProperty(Guid propertyID)
        {
            var result = await _propertyNoteService.GetNotesByPropertyID(propertyID);

            return Ok(result);
        }

        /// <summary>
        /// Finds all users with access to the property by partial match on email and name
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("property/{propertyID}/users/query")]
        [ResponseType(typeof(IEnumerable<PropertyNote>))]
        [HttpPost]
        public async Task<IHttpActionResult> QueryUsersForProperty(Guid propertyID, [FromBody]PropertyNoteUserQueryRequest request)
        {
            var result = await _propertyNoteService.QueryForPerson(propertyID, request?.Email, request?.Name);

            return Ok(result);
        }


        /// <summary>
        /// Gets the notes linked to the specified smartboard lead Id
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="apiKey"></param>
        /// <param name="igniteId"></param>
        /// <returns></returns>
        [Route("sb/{leadId}/{apiKey}")]
        [ResponseType(typeof(ICollection<SBNoteDTO>))]
        [HttpGet]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> GetAllForSmartboard(long leadId, string apiKey, long? igniteId)
        {
            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

            var result = _propertyNoteService.GetAllNotesForProperty(leadId, igniteId, DecyptApiKey);

            return Ok(result);
        }


        /// <summary>
        /// Gets the notes comments linked to the specified smartboard lead Id
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="apiKey"></param>
        ///// <param name="igniteId"></param>
        /// <returns></returns>
        [Route("sb/comments/{leadId}/{apiKey}")]
        [ResponseType(typeof(ICollection<SBNoteDTO>))]
        [HttpPost]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> GetNoteCommentsForSmartboard(long leadId, string apiKey, SBNoteDTO request)
        {
            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

            var result =  _propertyNoteService.GetNoteComments(leadId, request.IgniteID, DecyptApiKey, request.ParentID);

            return Ok(result);
        }

        #region transfer notes to new server

        /// <summary>
        /// transfer a new note in note server
        /// </summary>
        /// <param name="entity"></param> 
        /// <returns></returns>
        [Route("transfer")] 
        [HttpPost] 
        public IHttpActionResult AddEditNote(PropertyNote entity)
        { 
            var result = _propertyNoteService.AddEditNote(entity); 
            return Ok(result);
        }   

        /// <summary>
        /// Gets all PropertyNotes linked with specified propertyID from notes server
        /// </summary>
        /// <param name="propertyID"></param>
        /// <returns></returns>
        [Route("notes/{propertyID}")]
        [ResponseType(typeof(IEnumerable<PropertyNote>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetPropertyNotes(Guid propertyID)
        {
            var result = await _propertyNoteService.GetPropertyNotes(propertyID); 
            return Ok(result);
        }


        /// <summary>
        /// Gets PropertyNote linked with specified id from notes server
        /// </summary> 
        /// <param name="noteID"></param>
        /// <returns></returns>
        [Route("note/{noteID}")]
        [ResponseType(typeof(IEnumerable<PropertyNote>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetPropertyNoteById(Guid noteID)
        {
            if (noteID == Guid.Empty)
            {
                return BadRequest("Invalid request");
            }
           
            var result = await _propertyNoteService.GetPropertyNoteById(noteID);
            return Ok(result);
        }

        /// <summary>
        /// import notes to property server
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [Route("import/{limit}/{page}")] 
        [HttpGet]
        public IHttpActionResult ImportNotes(int page, int limit)
        {

            var result = _propertyNoteService.ImportNotes(page,limit);
            return Ok(result);
        } 

        #endregion


        public class testmodelforapi
        {
            public string ApiKey { get; set; }
            public long SBLeadId { get; set; }
            public long IgniteId { get; set; }

        }

        /// <summary>
        /// Gets apikey
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="apiKey"></param>
        /// <param name="igniteId"></param>
        /// <returns></returns>
        [Route("SendApikey/{leadId}/{apiKey}")]
        [ResponseType(typeof(ICollection<SBNoteDTO>))]
        [HttpGet]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> GetApikey(long leadId, string apiKey, long igniteId)
        {

            string apikey = await _propertyNoteService.getApiKey(leadId, igniteId, apiKey);

            string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(apikey);

            var result = new testmodelforapi
            {
                ApiKey = encryptedAPIkey,
                SBLeadId = leadId,
                IgniteId = igniteId
            };


            return Ok(result);
        }

        /// <summary>
        /// creates a new note from smartboard
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("sb/{apiKey}")]
        [ResponseType(typeof(SBNoteDTO))]
        [HttpPost]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> CreateNoteFromSmartboard([FromBody]SBNoteDTO request, string apiKey)
        {
            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);
            var result = _propertyNoteService.AddNoteFromSmartboard(request, DecyptApiKey);
            // var result = await _propertyNoteService.AddNoteFromSmartboard(request, apiKey);

            return Ok(result);
        }

        public class testmodel
        {
            public string OriginalApiKey { get; set; }
            public string EncryptedApiKey { get; set; }
            public string EncryptTextOfOriginalApiKey { get; set; }
            public string DecyptTextOfEncryptedApiKey { get; set; }

        }
        /// <summary>
        /// Check Apikey Encryption and Decryption
        /// </summary>
        /// <param name="originalapikey"></param>
        /// <param name="Encryptedapikey"></param>
        /// <returns></returns>
        [Route("EncryptApikey/{originalapikey}")]
        [HttpGet]
        public async Task<IHttpActionResult> CheckEncryptDecryptApikey(string originalapikey)
        {
            string EncryptApiKey = CryptographyHelper.getEncryptAPIKey(originalapikey);


            // string DecyptvalueofEncryptkey = CryptographyHelper.DecryptApiKey(EncryptApiKey);


            //  string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(Encryptedapikey);

            var result = new testmodel
            {
                OriginalApiKey = originalapikey,
                //  EncryptedApiKey = Encryptedapikey,
                EncryptTextOfOriginalApiKey = EncryptApiKey,
                //  DecyptTextOfEncryptedApiKey = DecyptApiKey
            };

            return Ok(result);
        }

        /// <summary>
        /// edits a note from smartboard
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("sb/{apiKey}")]
        [ResponseType(typeof(SBNoteDTO))]
        [HttpPatch]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> EditNoteFromSmartboard([FromBody]SBNoteDTO request, string apiKey)
        {

            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

            var result = _propertyNoteService.EditNoteFromSmartboard(request, DecyptApiKey);
            

            return Ok(result);
        }

        /// <summary>
        /// deletes a note by its specified id
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="userID"></param>
        /// <param name="apiKey"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [Route("sb/{leadId}/{userID}/{apiKey}")]
        [HttpDelete]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> DeleteNoteFromSmartboard(Guid noteId, string userID, string apiKey, string email)
        {

            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

            var result = _propertyNoteService.DeleteNoteFromSmartboard(noteId, userID, DecyptApiKey, email);

            return Ok(result);
        }

        /// <summary>
        /// returns data about notes created
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("sb/aboutNoteCreated")]
        [HttpPost]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> DataAboutNotesCreated([FromBody]NoteCreateDTO request, DateTime fromDate, DateTime toDate)
        {
            var result = await _propertyNoteService.NotesCreate(request, fromDate, toDate);
            return Ok(result);
        }

        /// <summary>
        /// / Gets Territories where Lead can transfer using SB apikey
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("sb/Territories/{leadId}/{apiKey}")]
        [HttpGet]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> CheckCanTransferLead(long leadId, string apiKey)
        {

            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

            var result = await _propertyNoteService.GetTerritoriesList(leadId, DecyptApiKey);
            return Ok(result);
        }
         

        /// <summary>
        /// / Update TerritoryId for Property which given By SB.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("sb/Transfer/{apiKey}")]
        [HttpPost]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> UpdateTerritoryIdInProperty([FromBody]SBNoteDTO request, string apiKey)
        {

            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

            var result = _propertyNoteService.UpdateTerritoryIdInProperty(request.LeadID, request.Guid, DecyptApiKey, request.Email);
            return Ok(result);
        }


        [Route("send/Email")]
        [HttpGet]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> SendEmailForTest(string emailid)
        {
            var result = _propertyNoteService.SendEmailForTest(emailid);
            return Ok(result);
        }


        [Route("send/notification")]
        [HttpPost]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> SendNotification(Person people)
        {
            var result = _propertyNoteService.SendNotification(people.fcm_token);
            return Ok(result);
        }


        /// <summary>
        /// Update SmartboardId By Email.
        /// </summary>
        [Route("sb/idupdate")]
        [HttpPost]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> UpdateSmartboardIdByEmail()
        {
            var result = await _propertyNoteService.UpdateSmartboardIdByEmail();
            return Ok(result);
        }
    }
}
