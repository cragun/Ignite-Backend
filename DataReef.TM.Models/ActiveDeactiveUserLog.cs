using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models
{
   public class ActiveDeactiveUserLog : EntityBase
    {
        public string Username { get; set; }
        public string OldState { get; set; }
        public string NewState { get; set; }  // active deactive
        public string Changer { get; set; }
        public string Reason { get; set; }
    }
}
