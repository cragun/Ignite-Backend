using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.ClientApi.Responses
{
    public class GenericResponse<T>:ResponseBase
    {

        public GenericResponse(T response)
        {
            this.Response = response;
        }

        public T Response { get; set; }
    }
}