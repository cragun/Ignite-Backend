using System;
using System.Reflection;

namespace DataReef.TM.Api.Areas.HelpPage.Services.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}