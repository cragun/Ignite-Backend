using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Core
{
    public abstract class BaseBridge
    {
        public virtual string BaseUrl { get; protected set; }
        public virtual bool IsEnabled { get; protected set; }

        private RestClient _client;
        protected RestClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new RestClient(BaseUrl);
                }
                return _client;
            }
        }
    }
}
