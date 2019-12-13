using Microsoft.Owin.Extensions;
using Owin;
using System;

namespace DataReef.TM.ClientApi.Middlewares.Auth
{
    public static class AuthMiddlewareExtensions
    {
        public static IAppBuilder UseTokenAuthentication(this IAppBuilder app, TokenAuthOptions options = null)
        {
            if (app == null) throw new ArgumentNullException("app");

            app.Use<TokenAuthMiddleware>(options ?? new TokenAuthOptions());
            app.UseStageMarker(PipelineStage.Authenticate);

            return app;
        }
    }
}