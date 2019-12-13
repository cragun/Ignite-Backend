using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using DataReef.TM.Models;

namespace DataReef.TM.ClientApi.Responses
{
    public class AuthenticationResponse:ResponseBase
    {

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }




        public AuthenticationResponse(ApiToken token)
        {
            this.Expires = new ExpirationDate();
            this.StatusCode = 0;
            this.StatusDescription = "OK";
            this.Token = token.Guid;

            //2013-03-29 20:43:21 UTC
            this.Expires.DateTime = token.ExpirationDate.ToString("yyyy-MM-dd HH:mm:ss UTC");
            this.Expires.SystemTime =  (long)DateTimeToUnixTimestamp(token.ExpirationDate);

        }


        public AuthenticationResponse(System.Exception ex)
        {
            this.Expires = new ExpirationDate();
            this.StatusCode = 1;
            this.StatusDescription = ex.Message;
            this.Expires = null;

        }
        public Guid Token { get; set; }

        public ExpirationDate Expires {get;set;}



    }
}