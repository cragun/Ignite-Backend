
namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    public class AttachmentOptionDefinition
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public int Index { get; set; }

        public bool Enabled { get; set; }

        public string OUSettingName { get; set; }

        public string[] SubmissionReceiverEmails { get; set; }
    }
}
