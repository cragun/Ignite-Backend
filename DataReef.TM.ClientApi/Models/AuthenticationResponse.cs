using System;
using DataReef.TM.Models;

namespace DataReef.TM.ClientApi.Models
{
    public class AuthenticationResponse
    {
        public AuthenticationResponse(ApiToken token)
        {
            Expires = new ExpirationDate();
            StatusCode = 0;
            StatusDescription = "OK";
            Token = token.Guid;

            //2013-03-29 20:43:21 UTC
            Expires.DateTime = token.ExpirationDate.ToString("yyyy-MM-dd HH:mm:ss UTC");
            Expires.SystemTime = (long)DateTimeToUnixTimestamp(token.ExpirationDate);
        }


        public AuthenticationResponse(Exception ex)
        {
            Expires = new ExpirationDate();
            StatusCode = 1;
            StatusDescription = ex.Message;
            Expires = null;
        }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public Guid Token { get; set; }

        public ExpirationDate Expires { get; set; }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                    new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }


    public class ExpirationDate
    {
        public long SystemTime { get; set; }

        public string DateTime { get; set; }
    }
}