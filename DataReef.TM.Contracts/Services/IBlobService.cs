using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    public interface IBlobService
    {
        [OperationContract]
        void Upload(Guid guid, BlobModel blob, BlobAccessRights access = BlobAccessRights.Private, string bucketName = null);

        [OperationContract]
        void UploadByName(string name, BlobModel blob, BlobAccessRights access = BlobAccessRights.Private, string bucketName = null);

        [OperationContract]
        string UploadByNameGetFileUrl(string name, BlobModel blob, BlobAccessRights access, string bucketName = null);

        [OperationContract]
        string UploadByNameGetFileUrlPrivateBucket(string name, BlobModel blob, BlobAccessRights access, string bucketName = null); 

        [OperationContract]
        BlobModel Download(Guid guid, string bucketName = null);

        [OperationContract]
        BlobModel DownloadByName(string name, string bucketName = null);

        [OperationContract]
        void DeleteByName(string name, string bucketName = null);

        [OperationContract]
        List<string> GetKeys(string pathWithPrefix, int maxKeys = 100, string bucketName = null);

        [OperationContract]
        string GetFileURL(string name, string bucketName = null, int expirationDays = 3650);

        [OperationContract]
        bool FileExists(string name, string bucketName = null);

        [OperationContract]
        void CopyItem(string sourceName, string destinationName, BlobAccessRights access, string sourceBucket = null, string destinationBucket = null);

    }
}
