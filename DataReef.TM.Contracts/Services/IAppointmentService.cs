using DataReef.TM.Models;
using DataReef.TM.Models.Client;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Common;
using DataReef.TM.Models.DTOs.Inquiries;
using DataReef.TM.Models.DTOs.SmartBoard;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IAppointmentService : IDataService<Appointment>
    {
        [OperationContract]
        Appointment SetAppointmentStatus(Guid appointmentID, AppointmentStatus status);

        [OperationContract]
        void VerifyUserAssignmentAndInvite(IEnumerable<Appointment> entities);

        [OperationContract]
        IEnumerable<SBAppointmentDTO> GetSmartboardAppointmentsForPropertyID(long propertyID, string apiKey);

        [OperationContract]
        IEnumerable<SBAppointmentDTO> GetSmartboardAppointmentsForOUID(Guid ouID, int pageIndex, int itemsPerPage, string apiKey);

        [OperationContract]
        ICollection<Person> GetMembersWithAppointment(OUMembersRequest request, Guid ouID, string apiKey, DateTime date);

        [OperationContract]
        IEnumerable<SBAppointmentDTO> GetSmartboardAppointmentsAssignedToUserId(string smartboardUserId, string apiKey);

        [OperationContract]
        SBAppointmentDTO InsertNewAppointment(SBCreateAppointmentRequest request, string apiKey);

        [OperationContract]
        SBAppointmentDTO EditSBAppointment(SBEditAppointmentRequest request, Guid appointmentID, string apiKey);

        [OperationContract]
        bool DeleteSBAppointment(Guid appointmentID, string apiKey);

        [OperationContract]
        SBAppointmentDTO SetAppointmentStatusFromSmartboard(SBSetAppointmentStatusRequest request, string apiKey);

        [OperationContract]
        void SendSMSTest(string mobilenumber);
        
    }
}