using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class BlobService : IBlobService
    {
        public static readonly string S3SourceDocumentsBucket = ConfigurationManager.AppSettings["AWS_S3_SourceDocs_BucketName"];
        private static readonly string _s3AccessKeyId = ConfigurationManager.AppSettings["AWS_S3_AccessKeyID"];
        private static readonly string _s3SecretAccessKey = ConfigurationManager.AppSettings["AWS_S3_SecretAccessKey"];
        private static readonly string _s3BucketName = ConfigurationManager.AppSettings["AWS_S3_BucketName"];
        private static readonly string _s3BucketNamePrivate = ConfigurationManager.AppSettings["AWS_S3_BucketName_Private"];
        
        private static readonly RegionEndpoint _region = RegionEndpoint.USWest2;
        private static readonly int UrlValidityDays = 3650;

        public BlobModel Download(Guid guid, string bucketName = null)
        {
            return DownloadByName(guid.ToString(), bucketName);
        }

        private AmazonS3Client _client;
        private AmazonS3Client Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new AmazonS3Client(_s3AccessKeyId, _s3SecretAccessKey, _region);
                }
                return _client;
            }
        }

        public BlobModel DownloadByName(string name, string bucketName = null)
        {
            var req = new GetObjectRequest
            {
                BucketName = bucketName ?? _s3BucketName,
                Key = name.ToLowerInvariant()
            };

            GetObjectResponse resp;

            try
            {
                resp = Client.GetObject(req);
            }
            catch (Exception exception)
            {
                // The Amazon S3 not found exception is not being correctly serialized and sent across WCF, probably because it wrongfully nests an inner exception, this is a workaround:
                if (exception.Message.Contains("key") && exception.Message.Contains("exist"))
                {
                    throw new Exception("404 " + exception.Message);
                }
                throw exception;
            }

            using (var ms = new MemoryStream())
            {
                resp.ResponseStream.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return new BlobModel
                {
                    Content = ms.ToArray(),
                    ContentType = resp.Headers.ContentType
                };
            }
        }

        public List<string> GetKeys(string pathWithPrefix, int maxKeys = 100, string bucketName = null)
        {
            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = bucketName ?? _s3BucketName,
                Prefix = pathWithPrefix.ToLowerInvariant(),
                MaxKeys = maxKeys
            };

            var result = new List<string>();
            do
            {
                ListObjectsResponse response = Client.ListObjects(request);

                result.AddRange(response.S3Objects.Select(obj => obj.Key));

                // If response is truncated, set the marker to get the next 
                // set of keys.
                if (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;
                }
                else
                {
                    request = null;
                }
            } while (request != null);

            return result;
        }

        public void Upload(Guid guid, BlobModel blob, BlobAccessRights access, string bucketName = null)
        {
            UploadByName(guid.ToString().ToLower(), blob, access, bucketName);
        }

        public void UploadByName(String name, BlobModel blob, BlobAccessRights access, string bucketName = null)
        {
            using (var stream = new MemoryStream(blob.Content))
            {
                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = bucketName ?? _s3BucketName,
                    Key = name.ToLowerInvariant(),
                    InputStream = stream,
                    ContentType = blob.ContentType,
                    CannedACL = GetAccessRights(access)
                };

                var response = Client.PutObject(request);
            }
        }

        public string UploadByNameGetFileUrl(string name, BlobModel blob, BlobAccessRights access, string bucketName = null)
        {
            name = name.ToLowerInvariant();
            UploadByName(name, blob, access, bucketName);

            if (access == BlobAccessRights.PublicRead)
            {
                return GetUrl(name, bucketName ?? _s3BucketName);
            }

            return GetFileURL(name, bucketName);
        }

        public void UploadByNamePrivateBucket(String name, BlobModel blob, BlobAccessRights access, string bucketName = null)
        {
            using (var stream = new MemoryStream(blob.Content))
            {
                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = bucketName ?? _s3BucketNamePrivate,
                    Key = name.ToLowerInvariant(),
                    InputStream = stream,
                    ContentType = blob.ContentType, 
                    CannedACL = S3CannedACL.Private
                };

                var response = Client.PutObject(request);
            }
        }

        public string UploadByNameGetFileUrlPrivateBucket(string name, BlobModel blob, BlobAccessRights access, string bucketName = null)
        {
            bucketName = bucketName ?? _s3BucketNamePrivate;
            name = name.ToLowerInvariant();
            UploadByNamePrivateBucket(name, blob, access, bucketName); 

            return GetFileURL(name, bucketName);
        }

        public void DeleteByName(string name, string bucketName = null)
        {
            var req = new DeleteObjectRequest
            {
                BucketName = bucketName ?? _s3BucketName,
                Key = name.ToLowerInvariant()
            };
            Client.DeleteObject(req);
        }

        private S3CannedACL GetAccessRights(BlobAccessRights access)
        {
            switch (access)
            {
                case BlobAccessRights.PublicRead: return S3CannedACL.PublicRead;
                case BlobAccessRights.PublicReadWrite: return S3CannedACL.PublicReadWrite;
                default: return S3CannedACL.Private;
            }
        }

        private string GetUrl(string key, string bucket)
        {
            return $"https://{bucket}.s3.amazonaws.com/{key.ToLowerInvariant()}";
        }

        public string GetFileURL(string name, string bucketName = null, int expirationDays = 3650)
        {
            var expiryUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName ?? _s3BucketName,
                Key = name.ToLowerInvariant(),
                Expires = DateTime.UtcNow.AddDays(expirationDays)
            };

            string bucketUrl = Client.GetPreSignedURL(expiryUrlRequest);
            return bucketUrl;
        }

        public bool FileExists(string name, string bucketName = null)
        {
            var req = new GetObjectMetadataRequest
            {
                BucketName = bucketName ?? _s3BucketName,
                Key = name.ToLowerInvariant()
            };

            try
            {
                var resp = Client.GetObjectMetadata(req);
                return true;
            }
            catch { }
            return false;
        }

        public void CopyItem(string sourceName, string destinationName, BlobAccessRights access, string sourceBucket = null, string destinationBucket = null)
        {
            var req = new CopyObjectRequest
            {
                SourceBucket = sourceBucket ?? _s3BucketName,
                DestinationBucket = destinationBucket ?? _s3BucketName,
                SourceKey = sourceName.ToLowerInvariant(),
                DestinationKey = destinationName.ToLowerInvariant(),
                CannedACL = GetAccessRights(access)
            };


            var response = Client.CopyObject(req);
        }
    }
}