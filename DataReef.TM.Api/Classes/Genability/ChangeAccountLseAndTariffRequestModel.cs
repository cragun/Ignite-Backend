using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.Genability
{
    public class ChangeAccountLseAndTariffRequestModel
    {
        public string UtilityID      { get; set; }

        public string MasterTariffID { get; set; }
    }
}