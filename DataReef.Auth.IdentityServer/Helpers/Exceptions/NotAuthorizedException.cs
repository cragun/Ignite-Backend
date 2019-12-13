using System;

namespace DataReef.Auth.IdentityServer.Helpers.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string message)
            : base(message)
        {

        }

        public NotAuthorizedException(string message, Exception exception)
            : base(message, exception)
        {

        }
    }
}