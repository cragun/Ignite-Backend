using System.Web;
using Microsoft.Practices.Unity;

namespace DataReef.Core.Infrastructure.Unity
{
    /// <summary>
    /// Dependency lifetime manager for asp.net session
    /// </summary>
    public class SessionLifetimeManager : LifetimeManager
    {
        private readonly string contractName;

        public SessionLifetimeManager(string contractName)
        {
            this.contractName = contractName;
        }

        public override object GetValue()
        {
            return HttpContext.Current.Session[this.contractName];
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Session[this.contractName] = newValue;
        }

        public override void RemoveValue()
        {
            HttpContext.Current.Session.Remove(this.contractName);
        }
    }
}
