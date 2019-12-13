using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.Requests
{
    public class GenericRequest<T>
    {
        public T Request { get; set; }

    }
}