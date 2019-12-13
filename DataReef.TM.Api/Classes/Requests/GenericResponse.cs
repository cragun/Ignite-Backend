using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.Requests
{
    public class GenericResponse<T>
    {
        public T Response { get; set; }

    }
}