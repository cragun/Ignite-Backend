using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public static class CommunicationExtensions
{
    public static void AddDataReefAuthHeader(this IRestRequest request)
    {
        var dataView = Guid.NewGuid().ToString();
        var token = $"asdfjkl;qweruiop{dataView}";
        var auth = sha256(token);

        request.AddHeader("DataView", dataView);
        request.AddHeader("Authorization", auth);
    }

    public static string sha256(string token)
    {
        SHA256Managed crypt = new SHA256Managed();
        string hash = String.Empty;
        byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(token), 0, Encoding.ASCII.GetByteCount(token));
        foreach (byte theByte in crypto)
        {
            hash += theByte.ToString("x2");
        }
        return hash;
    }
}
