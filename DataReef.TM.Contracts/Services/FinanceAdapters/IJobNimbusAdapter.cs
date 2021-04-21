using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using System;
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
        JobNimbusLeadResponseData CreateJobNimbusLead(Guid propertyid);

        [OperationContract]
        AppointmentJobNimbusLeadResponseData CreateAppointmentJobNimbusLead(Guid propertyid);

        [OperationContract]
        NoteJobNimbusLeadResponseData CreateJobNimbusNote(PropertyNote note);

    }
}
