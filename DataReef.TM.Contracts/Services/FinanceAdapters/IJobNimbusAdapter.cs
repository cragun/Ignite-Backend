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
        JobNimbusLeadResponseData CreateJobNimbusLead(Guid propertyid, string url, string apikey);

        [OperationContract]
        AppointmentJobNimbusLeadResponseData CreateAppointmentJobNimbusLead(Guid propertyid, string url, string apikey);

        [OperationContract]
        NoteJobNimbusLeadResponseData CreateJobNimbusNote(PropertyNote note, string url, string apikey);

    }
}
