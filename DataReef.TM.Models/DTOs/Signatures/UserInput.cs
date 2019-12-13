using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public enum UserInputType
    {
        None = 0,
        Signature = 1,
        SolarSystem = 2,
    }

    public class UserInput
    {
        public Guid Guid { get; set; }

        public List<Guid> Guids { get; set; }

        public UserInputType Type { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public int PageNumber { get; set; }

        /// <summary>
        /// base64 encoded binary
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// list of base64 encoded binary Validation Images
        /// </summary>
        public List<string> Validation { get; set; }
    }
}
