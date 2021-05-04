using DataReef.TM.Models.DTOs.FinanceAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class lead_reference
    {
        public string ignite_id { get; set; }
        public string smartboard_id { get; set; }
    }

    public class lead_info
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleNameInitial { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string addressLine1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
        public string lattitude { get; set; }
        public string longitude { get; set; }
    }

    public class AddPropertyReference
    {
        public lead_reference lead_reference { get; set; }
        public string account_reference_id { get; set; }//api key
        public lead_info lead_info { get; set; }
    }

    public class NoteResponse
    {
        public string message { get; set; }
        public string refId { get; set; }
        public string noteId { get; set; }
        public string threadId { get; set; }
        public string replyId { get; set; }
    }

    public class NoteRequest
    {
        public string guid { get; set; }
        public string referenceId { get; set; }
        public string thread_id { get; set; }
        public string threadId { get; set; }
        public string message { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string source { get; set; }
        public List<string> attachments { get; set; }
        public List<string> parentIds { get; set; }
        //public Dictionary<int, NoteTaggedUser> taggedUsers { get; set; }
        public List<NoteTaggedUser> taggedUsers { get; set; }
        public NoteTaggedUser user { get; set; }
        public string personId { get; set; }
        public string jobNimbusId { get; set; }
        public string jobNimbusLeadId { get; set; }
        public int version { get; set; }
        public ThirdPartyPropertyType propertyType { get; set; }
    }

    public class NoteTaggedUser
    {
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public bool isSendEmail { get; set; }
        public bool isSendSms { get; set; }
        public string userId { get; set; }
    }

    public class Note
    {
        public List<string> attachments { get; set; }
        public string _id { get; set; }
        public string guid { get; set; }
        public string referenceId { get; set; }
        public string message { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string source { get; set; }
        public string threadId { get; set; }
        public string personId { get; set; }
        public string jobNimbusId { get; set; }
        public string jobNimbusLeadId { get; set; }
        public int? version { get; set; }
        public ThirdPartyPropertyType propertyType { get; set; }
        public List<NoteTaggedUser> user { get; set; }
        public List<NoteTaggedUser> taggedUsers { get; set; }
    }

    public class AllNotes
    {
        public Note notes { get; set; }
        public List<Note> replies { get; set; }
    }

    public class EmailNotifications
    {
        public string type { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
    }
}



