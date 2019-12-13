using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Services.InternalServices
{
    public interface IUtilServices
    {
        byte[] GetPDF(string url);

        string GetPDFUrl(string url);
    }
}
