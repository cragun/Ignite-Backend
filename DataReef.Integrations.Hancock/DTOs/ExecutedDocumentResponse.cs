using DataReef.TM.Models.DTOs.Signatures;
using System;

namespace DataReef.Integrations.Hancock.DTOs
{
    public class ExecutedDocumentResponse
    {
        public Guid Guid { get; set; }

        public string DocumentURL { get; set; }

        /// <summary>
        /// PDF base64 encoded
        /// </summary>
        public String Document { get; set; }

        public string Title { get; set; }

        public string Revision { get; set; }

        public string DocumentType { get; set; }


        public System.DateTime DateCreated { get; set; }


        public SignDocumentResponseHancock ToApiResponse()
        {
            return new SignDocumentResponseHancock
            {
                Guid = this.Guid,
                DocumentURL = this.DocumentURL
            };
        }
    }
}
