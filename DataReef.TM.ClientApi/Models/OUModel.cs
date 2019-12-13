using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.ClientApi.Models
{
    public class OUModel
    {
        public Guid ID { get; set; }


        public string Name { get; set; }


        public Guid? ParentID { get; set; }

    }
}