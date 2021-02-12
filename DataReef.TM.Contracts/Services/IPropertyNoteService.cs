using System.ServiceModel;
using DataReef.TM.Models;
using System.Collections.Generic;
using System;
using DataReef.TM.Models.Commerce;
using DataReef.TM.Models.DTOs.Commerce;
using DataReef.TM.Models.DTOs.Common;
using DataReef.TM.Models.DTOs;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPropertyNoteService : IDataService<PropertyNote>
    {
        [OperationContract]
        Task<IEnumerable<PropertyNote>> GetNotesByPropertyID(Guid propertyID);

        [OperationContract]
        string getApiKey(long? smartboardLeadID, long? igniteID, string apiKey);

        [OperationContract]
        IEnumerable<SBNoteDTO> GetAllNotesForProperty(long? smartboardLeadID, long? igniteID, string apiKey);

        [OperationContract]
        IEnumerable<SBNoteDTO> GetNoteComments(long? smartboardLeadID, long? igniteID, string apiKey, Guid ParentID);

        [OperationContract]
        SBNoteDTO AddNoteFromSmartboard(SBNoteDTO noteRequest, string apiKey);

        [OperationContract]
        SBNoteDTO EditNoteFromSmartboard(SBNoteDTO noteRequest, string apiKey); 

        [OperationContract]
        IEnumerable<Territories> GetTerritoriesList(long smartboardLeadID, string apiKey);


        [OperationContract]
        SBNoteDTO DeleteNoteFromSmartboard(Guid noteID, string userID, string apiKey, string email);

        [OperationContract]
        IEnumerable<SBNoteData> NotesCreate(NoteCreateDTO request,DateTime fromDate,DateTime toDate);

        [OperationContract]
        IEnumerable<Models.Person> QueryForPerson(Guid propertyID, string email, string name);

        [OperationContract]
        SBUpdateProperty UpdateTerritoryIdInProperty(long? leadId, Guid? TerritoryId, string apiKey, string email);

        [OperationContract]
        string SendNotification(string fcm_token); 

        [OperationContract]
        string UpdateSmartboardIdByEmail(); 
    }
}