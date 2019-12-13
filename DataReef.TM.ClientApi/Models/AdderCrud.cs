using DataReef.TM.Models.DTOs.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.ClientApi.Models
{
    public class AdderCrud
    {

        public AdderCrud()
        {
            this.Delete = new List<Guid>();
            this.Insert = new List<AdderDataView>();
            this.Update = new List<AdderDataView>();
        }


        public ICollection<Guid> Delete { get; set; }

        public ICollection<AdderDataView> Insert { get; set; }

        /// <summary>
        /// Adders to add or replace.  In order to replace, the AdderID Guid must match an existing AdderID
        /// </summary>
        public ICollection<AdderDataView> Update { get; set; }


    }
}