using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.LoanPal.Models.LoanPal
{
    public class LoanPalErrorModel
    {
        public string Keyword { get; set; }

        public string DataPath { get; set; }

        public string SchemaPath { get; set; }

        public Dictionary<string, string> Params { get; set; }

        public string Message { get; set; }

    }
}
