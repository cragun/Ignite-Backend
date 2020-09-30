using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class SunlightError
    {

        public string returnCode { get; set; }
        public List<Error> error { get; set; }        
    }
    public class Error
    {
        public string errorMessage { get; set; }

    }

}
