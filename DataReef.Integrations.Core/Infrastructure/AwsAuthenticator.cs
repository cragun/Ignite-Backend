using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Security.Cryptography;

namespace DataReef.Integrations.Core.Infrastructure
{
    public class AwsAuthenticator : RestSharp.Authenticators.IAuthenticator
    {
        public string AccessKeyId { get; }
        public string AccessKeySecret { get; }
        public string Region { get; }
        public string ServiceName { get; set; }

        private DateTime now;
        private IRestClient _client;
        private IRestRequest _request;

        const string Algorithm = "AWS4-HMAC-SHA256";
        const string ContentType = "application/json";
        const string Host = "apigateway.eu-west-1.amazonaws.com";
        const string SignedHeaders = "content-type;host;x-amz-date";


        public AwsAuthenticator(string accessKeyId, string accessKeySecret, string region, string serviceName)
        {
            AccessKeyId = accessKeyId;
            AccessKeySecret = accessKeySecret;
            Region = region;
            ServiceName = serviceName;

            now = DateTime.UtcNow;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            _client = client;
            _request = request;

            string hashedRequestPayload = CreateRequestPayload();

            var authorization = GetAuthorization(hashedRequestPayload, _request.Method.ToString(), _request.Resource, string.Empty);

            _request.AddHeader("Host", client.BaseUrl.Host);
            _request.AddHeader("X-Amz-date", now.ToString("yyyyMMddTHHmmss") + "Z");
            _request.AddHeader("Authorization", authorization);
        }

        private string CreateRequestPayload()
        {
            string jsonString = _request
                                    .Parameters?
                                    .FirstOrDefault(p => p.Type.Equals(ParameterType.RequestBody))?
                                    .Value as string ?? "";
            string hashedRequestPayload = HexEncode(Hash(ToBytes(jsonString)));

            return hashedRequestPayload;
        }

        private byte[] ToBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str.ToCharArray());
        }

        private string HexEncode(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        private byte[] Hash(byte[] bytes)
        {
            return SHA256.Create().ComputeHash(bytes);
        }

        private byte[] HmacSha256(string data, byte[] key)
        {
            return new HMACSHA256(key).ComputeHash(ToBytes(data));
        }

        private string GetAuthorization(string hashedRequestPayload, string requestMethod, string canonicalUri, string canonicalQueryString)
        {
            var dateStamp = now.ToString("yyyyMMdd");
            var requestDate = now.ToString("yyyyMMddTHHmmss") + "Z";
            var credentialScope = string.Format("{0}/{1}/{2}/aws4_request", dateStamp, Region, ServiceName);

            var headers = new SortedDictionary<string, string> {
                { "content-type", "application/json" },
                { "host", _client.BaseUrl.Host},
                { "x-amz-date", requestDate }
            };

            string canonicalHeaders = string.Join("\n", headers.Select(x => x.Key.ToLowerInvariant() + ":" + x.Value.Trim())) + "\n";

            // Task 1: Create a Canonical Request For Signature Version 4
            string canonicalRequest = requestMethod + "\n" + canonicalUri + "\n" + canonicalQueryString + "\n" + canonicalHeaders + "\n" + SignedHeaders + "\n" + hashedRequestPayload;
            string hashedCanonicalRequest = HexEncode(Hash(ToBytes(canonicalRequest)));

            // Task 2: Create a String to Sign for Signature Version 4
            string stringToSign = Algorithm + "\n" + requestDate + "\n" + credentialScope + "\n" + hashedCanonicalRequest;

            // Task 3: Calculate the AWS Signature Version 4
            byte[] signingKey = GetSignatureKey(AccessKeySecret, dateStamp, Region, ServiceName);
            string signature = HexEncode(HmacSha256(stringToSign, signingKey));

            // Task 4: Prepare a signed request
            // Authorization: algorithm Credential=access key ID/credential scope, SignedHeadaers=SignedHeaders, Signature=signature

            var authorization = $"{Algorithm} Credential={AccessKeyId}/{dateStamp}/{Region}/{ServiceName}/aws4_request, SignedHeaders={SignedHeaders}, Signature={signature}";
            return authorization;
        }

        private byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
        {
            byte[] kDate = HmacSha256(dateStamp, ToBytes("AWS4" + key));
            byte[] kRegion = HmacSha256(regionName, kDate);
            byte[] kService = HmacSha256(serviceName, kRegion);
            return HmacSha256("aws4_request", kService);
        }

    }
}
