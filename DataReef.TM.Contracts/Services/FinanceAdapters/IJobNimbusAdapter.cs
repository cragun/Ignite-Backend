using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services.FinanceAdapters
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IJobNimbusAdapter
    {
        [OperationContract]
        Task<JobNimbusLeadResponseData> CreateJobNimbusLead(Property property, bool IsCreate);

        [OperationContract]
        Task<AppointmentJobNimbusLeadResponseData> CreateAppointmentJobNimbusLead(Appointment appointment, bool IsCreate);

        [OperationContract]
        NoteJobNimbusLeadResponseData CreateJobNimbusNote(PropertyNote note);

    }
}
