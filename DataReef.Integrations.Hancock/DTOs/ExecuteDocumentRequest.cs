using DataReef.TM.Models.DTOs.Signatures;
using System;
using System.Collections.Generic;

namespace DataReef.Integrations.Hancock.DTOs
{
    public class ExecuteDocumentRequest
    {
        public string IPAddress { get; set; }

        public Guid RenderedDocumentID { get; set; }

        public List<UserInput> UserInputs { get; set; }
    }
}
