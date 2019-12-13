using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class SignDocumentResponseHancock
    {
        [JsonIgnore]
        public Guid Guid { get; set; }

        public string DocumentURL { get; set; }



        public List<UserInput> UserInputs { get; set; }
    }
}
