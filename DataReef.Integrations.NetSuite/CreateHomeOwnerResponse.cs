using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.NetSuite
{
    public class CreateHomeOwnerResponse
    {
        public OktaProfile OktaProfile { get; set; }

        public string SunEdCustId      { get; set; }
    }

    public class OktaProfile
    {
        public string Id                { get; set; }

        public string LastLogin         { get; set; }

        public string PasswordChanged   { get; set; }

        public string Created           { get; set; }

        public string Status            { get; set; }

        public string StatusChanged     { get; set; }

        public string Activated         { get; set; }

        public _Links _Links            { get; set; }

        public string LastUpdated       { get; set; }

        public Credentials Credentials  { get; set; }

        public Profile Profile          { get; set; }
    }

    public class _Links
    {
        public _Link Activate   { get; set; }

        public _Link Deactivate { get; set; }
    }

    public class _Link
    {
        public string href   { get; set; }

        public string method { get; set; }
    }

    public class Credentials
    {
        public Provider Provider { get; set; }
    }

    public class Provider
    {
        public string Type { get; set; }

        public string Name { get; set; }
    }

    public class Profile
    {
        public string FirstName     { get; set; }
                                    
        public string LastName      { get; set; }
                                    
        public string Email         { get; set; }
                                    
        public string Login         { get; set; }

        public string MobilePhone   { get; set; }

        public string NSInternalId  { get; set; }

        public string Application   { get; set; }

        public string PartnerId     { get; set; }

        public string Role          { get; set; }
    }
}