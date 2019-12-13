using DataReef.TM.Models.DTOs.Signatures;
using System;
using System.Collections.Generic;

namespace DataReef.Integrations.Hancock.DTOs
{
    public class RenderDocumentResponse
    {
        public Guid Guid { get; set; }

        public string DocumentURL { get; set; }

        public List<UserInput> UserInputs { get; set; }

        public SignDocumentResponseHancock ToApiResponse()
        {
            return new SignDocumentResponseHancock
            {
                Guid = this.Guid,
                DocumentURL = this.DocumentURL,
                UserInputs = this.UserInputs
            };
        }
    }
}
