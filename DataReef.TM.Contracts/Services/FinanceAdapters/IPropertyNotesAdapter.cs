using DataReef.TM.Models;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.FinancialIntegration.LoanPal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPropertyNotesAdapter
    {
        [OperationContract]
        NoteResponse GetPropertyReferenceId(Property property, string apikey);

        [OperationContract]
        Task<List<AllNotes>> GetPropertyNotes(string referenceId);

        [OperationContract]
        Task<Models.DTOs.Solar.Finance.Note> GetPropertyNoteById(string noteID);

        [OperationContract]
        NoteResponse AddEditNote(string referenceId, PropertyNote note, IEnumerable<Person> taggedPersons, Person user);

        [OperationContract]
        Task<NoteResponse> SendEmailNotification(string subject, string body, string to);
    }
}
