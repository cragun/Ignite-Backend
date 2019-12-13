using DataReef.TM.Models.DataViews.Geo;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.Signatures
{
    /// <summary>
    /// Class used 
    /// </summary>
    [DataContract]
    public class DocumentSignRequest
    {
        [DataMember]
        public Guid FinancePlanID { get; set; }
        [DataMember]
        public Guid ProposalTemplateID { get; set; }
        [DataMember]
        public string ContractorID { get; set; }
        [DataMember]
        public double? UtilityInflationRate { get; set; }
        [DataMember]
        public List<DocumentData> DocumentData { get; set; }
        [DataMember]
        public List<UserInputData> UserInput { get; set; }
        [DataMember]
        public GeoPoint Location { get; set; }
        /// <summary>
        /// Custom Proposal Specific data serialized as JSON
        /// </summary>
        [DataMember]
        public string ProposalDataJSON { get; set; }
    }

    [DataContract]
    public enum UserInputDataType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Signature = 1,
        [EnumMember]
        SalesRepSignature,
        [EnumMember]
        ThreeDCustomer = 1000,
        [EnumMember]
        ThreeDSalesRep = 1001,
        [EnumMember]
        EnergyBill = 5000,
    }

    [DataContract]
    public abstract class BaseUserInputData
    {
        [DataMember]
        public Guid Guid { get; set; } = Guid.NewGuid();
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public ContentType ContentType { get; set; }
        [DataMember]
        public UserInputDataType Type { get; set; }
        /// <summary>
        /// UserInput specific custom data serialized as JSON
        /// </summary>
        [DataMember]
        public string DataJSON { get; set; }
        public string GetContentType()
        {
            return ContentType == ContentType.String ? "text" : "image/png";
        }
    }

    public class UserInputData : BaseUserInputData
    {
        [DataMember]
        public byte[] Content { get; set; }
        [DataMember]
        public List<Guid> Validation { get; set; }
        [DataMember]
        public List<byte[]> ValidationContent { get; set; }
    }

    /// <summary>
    /// Class that will store links to content on 3rd party storage (e.g. AWS - S3)
    /// </summary>
    public class UserInputDataLinks : BaseUserInputData
    {
        public UserInputDataLinks()
        {
            ValidationUrl = new List<string>();
        }

        public UserInputDataLinks(BaseUserInputData userInputData, string imageUrl, string thumbnailUrl) : this()
        {
            Guid = userInputData.Guid;
            Name = userInputData.Name;
            ContentType = userInputData.ContentType;
            Type = userInputData.Type;
            ContentURL = imageUrl;
            ThumbContentURL = thumbnailUrl;
            DataJSON = userInputData.DataJSON;
        }
        [DataMember]
        public string ContentURL { get; set; }
        [DataMember]
        public string ThumbContentURL { get; set; }
        [DataMember]
        public List<string> ValidationUrl { get; set; }
    }

    [DataContract]
    public enum DocumentDataType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        SolarSystem = 1,
    }

    [DataContract]
    public abstract class BaseDocumentData
    {
        [DataMember]
        public Guid Guid { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public ContentType ContentType { get; set; }
        [DataMember]
        public DocumentDataType Type { get; set; }
        /// <summary>
        /// Document specific data serialzied as JSON
        /// </summary>
        [DataMember]
        public string DataJSON { get; set; }

        public string GetContentType()
        {
            return ContentType == ContentType.String ? "text" : "image/png";
        }
    }

    public class DocumentData : BaseDocumentData
    {
        /// <summary>
        /// can be a string for text, or a base64 encoded image
        /// </summary>
        public byte[] Content { get; set; }
    }

    public class DocumentDataLink : BaseDocumentData
    {
        public DocumentDataLink() { }

        public DocumentDataLink(DocumentData documentData, string documentUrl, string thumbnailUrl)
        {
            Guid = documentData.Guid;
            Name = documentData.Name;
            ContentType = documentData.ContentType;
            ContentURL = documentUrl;
            ThumbContentURL = thumbnailUrl;
            Type = documentData.Type;
        }

        /// <summary>
        /// Link to content
        /// </summary>
        [DataMember]
        public string ContentURL { get; set; }
        [DataMember]
        public string ThumbContentURL { get; set; }

    }

}
