using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Mosaic.Models
{
    public class MosaicDocuSignResponse
    {
        public string envelopeId     { get; set; }
                                     
        public string uri            { get; set; }

        public string statusDateTime { get; set; }

        public string status         { get; set; }

        public string url            { get; set; }

    }
}
