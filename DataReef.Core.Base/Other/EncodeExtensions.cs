using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public static class EncodeExtensions
{

    public static string UrlEncodeEmail(this string email)
    {
        return email == null ? null : WebUtility.UrlEncode(email).EscapeEmail();
    }

    public static string EscapeEmail(this string email)
    {
        return email == null ? null : email.Replace("%40", "@");
    }
}

