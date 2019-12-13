using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Notifications
{
    public class AndroidNotification
    {
        public AndroidNotification()
        {
            Tags = new List<string>();
            Data = new Dictionary<string, string>();
        }


        public string Hub { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, string> Data;
    }
}
