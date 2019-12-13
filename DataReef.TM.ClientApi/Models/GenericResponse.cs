using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.ClientApi.Models
{
    public class GenericResponse<T>
    {

        public GenericResponse(T response)
        {
            this.Response = response;
            this.StatusDescription = "OK";
        }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public T Response { get; set; }

    }

}