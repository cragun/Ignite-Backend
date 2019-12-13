using System.Web.Hosting;

namespace DataReef.TM.Services
{
    public class HostNotifier : IRegisteredObject
    {
        public bool IsStopping { get; set; }

        // the hosting environment gives us 30 seconds to wrap things up here, and another 30 seconds for a second call with immediate = true;
        public void Stop(bool immediate) 
        {
            IsStopping = true;
        }

        public void Begin()
        {
            HostingEnvironment.RegisterObject(this);
        }

        public void Finish()
        {
            HostingEnvironment.UnregisterObject(this);
        }
    }
}
