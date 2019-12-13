using System.Web;

using Microsoft.Practices.Unity;

namespace DataReef.Core.Infrastructure.Unity
{
    public class RequestLifetimeManager : LifetimeManager
    {
        private readonly string contractName;

        public RequestLifetimeManager(string contractName)
        {
            this.contractName = contractName;
        }

        public override object GetValue()
        {
            return HttpContext.Current.Items[this.contractName];
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Items[this.contractName] = newValue;
        }

        public override void RemoveValue()
        {
            HttpContext.Current.Items.Remove(this.contractName);
        }
    }
}
