using DataReef.TM.Models;
using DataReef.TM.Models.Client;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Common;
using DataReef.TM.Models.DTOs.Inquiries;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPersonService : IDataService<Person>
    {
        [OperationContract]
        List<Person> GetMine(OUMembersRequest request);

        [OperationContract]
        Person GetMayEdit(Guid uniqueId, bool mayEdit, string include = "", string exclude = "", string fields = "", bool deletedItems = false);

        [OperationContract]
        List<PersonLite> GetPeopleForOU(Guid ouID, bool deep);

        [OperationContract]
        void Reactivate(Guid personId,string smartBoardId);

        [OperationContract]
        void DeactivateUser(string smartBoardId);

        [OperationContract]
        IntegrationToken GenerateIntegrationToken();

        [OperationContract]
        List<CRMDisposition> CRMGetAvailableDispositions();

        [OperationContract]
        List<CRMDisposition> CRMGetAvailableNewDispositions();

        [OperationContract]
        List<CRMLeadSource> CRMGetAvailableLeadSources(Guid ouid);
        

        [OperationContract]
        PaginatedResult<Property> CRMGetProperties(CRMFilterRequest request);

        [OperationContract]
        PersonDTO GetPersonDTO(Guid personID, string include = "");

        [OperationContract]
        string GetUserSurvey(Guid personID, Guid? propertyID = null);

        [OperationContract]
        string SaveUserSurvey(Guid personID, string survey);

        [OperationContract]
        string SavePropertySurvey(Guid personID, Guid propertyID, string survey);

        [OperationContract]
        string SendEmailSummarytoCustomer(Guid propertyID, string summary);
        

        [OperationContract]
        string GetSurveyUrl(Guid personID, Guid propertyID);

        [OperationContract]
        PersonClockTime GetPersonClock(Guid personID, long min);

        [OperationContract]
        IEnumerable<Person> personDetails(Guid ouid, DateTime date);

        [OperationContract]
        IEnumerable<PersonOffboard> OuassociationRoleName(Guid personid);

        [OperationContract]
        void SBDeactivate(Guid personid);

        [OperationContract]        
        Task<IEnumerable<Person>> CalendarPageAppointMentsByOuid(Guid ouid);

    }
}