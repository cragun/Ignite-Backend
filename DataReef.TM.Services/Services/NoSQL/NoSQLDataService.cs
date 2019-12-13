using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using DataReef.Core.Attributes;
using DataReef.TM.Contracts.Services.NoSQL;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services.Services.NoSQL
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(INoSQLDataService))]
    public class NoSQLDataService : INoSQLDataService
    {

        private static readonly string _dynamoDBAccessKeyId = ConfigurationManager.AppSettings["AWS_DynamoDB_AccessKeyID"];
        private static readonly string _dynamoDBSecretAccessKey = ConfigurationManager.AppSettings["AWS_DynamoDB_SecretAccessKey"];
        private static readonly string _dynamoDBTableName = ConfigurationManager.AppSettings["AWS_DynamoDB_TableName"];

        public NoSQLDataService()
        {

        }

        private AmazonDynamoDBClient _client;
        protected AmazonDynamoDBClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new AmazonDynamoDBClient(_dynamoDBAccessKeyId, _dynamoDBSecretAccessKey, RegionEndpoint.USWest2);
                }
                return _client;
            }
        }

        public string GetValue(string key, string tableName = null)
        {
            var table = Table.LoadTable(Client, tableName ?? _dynamoDBTableName);
            var item = table.GetItem(key);

            return item?.ToJson();
        }

        public T GetValue<T>(string key, string tableName = null)
        {
            var json = GetValue(key, tableName);
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void PutValue(string value, string tableName = null)
        {
            if (value.Length >= 256000)
            {
                throw new ApplicationException("Content too large for DynamoDB");
            }
            var table = Table.LoadTable(Client, tableName ?? _dynamoDBTableName);
            var doc = Document.FromJson(value);
            table.PutItem(doc);
        }

        public void PutValue<T>(T value, string tableName = null)
        {
            if (value == null)
            {
                return;
            }
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            PutValue(JsonConvert.SerializeObject(value, settings), tableName);
        }

        public bool IsHealthy()
        {

            try
            {
                Client.DescribeTable(_dynamoDBTableName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
