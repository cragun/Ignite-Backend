using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.XPath;
using DataReef.Core.Attributes;
using DataReef.TM.Api.Areas.HelpPage.Services.ModelDescriptions;

namespace DataReef.TM.Api.Areas.HelpPage.Services
{
    /// <summary>
    /// A custom <see cref="IDocumentationProvider"/> that reads the API documentation from an XML documentation file.
    /// </summary>
    [Service(typeof(IDocumentationProvider), ServiceScope.Application)]
    [Service(typeof(IModelDocumentationProvider), ServiceScope.Application)]
    public class XmlDocumentationProvider : IDocumentationProvider, IModelDocumentationProvider
    {
        private readonly IEnumerable<XPathNavigator> documentNavigators;
        private const string AssemblyNameExpression = "/doc/assembly/name";
        private const string TypeExpression = "/doc/members/member[@name='T:{0}']";
        private const string MethodExpression = "/doc/members/member[@name='M:{0}']";
        private const string PropertyExpression = "/doc/members/member[@name='P:{0}']";
        private const string FieldExpression = "/doc/members/member[@name='F:{0}']";
        private const string ParameterExpression = "param[@name='{0}']";
        private const string DocumentationFilesPath = "./App_Data/";

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDocumentationProvider"/> class.
        /// </summary>
        public XmlDocumentationProvider()
        {
            var rootPath = HttpContext.Current.Server.MapPath("~");
            var documentPath = Path.GetFullPath(Path.Combine(rootPath, DocumentationFilesPath));
            var documents = Directory.GetFiles(documentPath, "*.xml");

            this.documentNavigators = documents.Select(p => new XPathDocument(p).CreateNavigator());
        }

        public string GetDocumentation(HttpControllerDescriptor controllerDescriptor)
        {
            XPathNavigator typeNode = this.GetTypeNode(controllerDescriptor.ControllerType);
            return GetTagValue(typeNode, "summary");
        }

        public virtual string GetDocumentation(HttpActionDescriptor actionDescriptor)
        {
            XPathNavigator methodNode = this.GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "summary");
        }

        public virtual string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            ReflectedHttpParameterDescriptor reflectedParameterDescriptor = parameterDescriptor as ReflectedHttpParameterDescriptor;
            if (reflectedParameterDescriptor != null)
            {
                XPathNavigator methodNode = this.GetMethodNode(reflectedParameterDescriptor.ActionDescriptor);
                if (methodNode != null)
                {
                    string parameterName = reflectedParameterDescriptor.ParameterInfo.Name;
                    XPathNavigator parameterNode = methodNode.SelectSingleNode(String.Format(CultureInfo.InvariantCulture, ParameterExpression, parameterName));
                    if (parameterNode != null)
                    {
                        return parameterNode.Value.Trim();
                    }
                }
            }

            return null;
        }

        public string GetResponseDocumentation(HttpActionDescriptor actionDescriptor)
        {
            XPathNavigator methodNode = this.GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "returns");
        }

        public string GetDocumentation(MemberInfo member)
        {
            string memberName = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetTypeName(member.DeclaringType), member.Name);
            string expression = member.MemberType == MemberTypes.Field ? FieldExpression : PropertyExpression;
            string selectExpression = String.Format(CultureInfo.InvariantCulture, expression, memberName);
            var assemblyNavigator = GetAssemblyNavigator(this.documentNavigators, member.DeclaringType.Assembly);
            if (assemblyNavigator == null)
                return null;

            XPathNavigator propertyNode = assemblyNavigator.SelectSingleNode(selectExpression);
            return GetTagValue(propertyNode, "summary");
        }

        public string GetDocumentation(Type type)
        {
            XPathNavigator typeNode = this.GetTypeNode(type);
            return GetTagValue(typeNode, "summary");
        }

        private XPathNavigator GetMethodNode(HttpActionDescriptor actionDescriptor)
        {
            ReflectedHttpActionDescriptor reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                string selectExpression = String.Format(CultureInfo.InvariantCulture, MethodExpression, GetMemberName(reflectedActionDescriptor.MethodInfo));
                var assemblyNavigator = GetAssemblyNavigator(this.documentNavigators, reflectedActionDescriptor.MethodInfo.DeclaringType.Assembly);
                return assemblyNavigator == null ? null : assemblyNavigator.SelectSingleNode(selectExpression);
            }

            return null;
        }

        private static string GetMemberName(MethodInfo method)
        {
            string name = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetTypeName(method.DeclaringType), method.Name);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => GetTypeName(param.ParameterType)).ToArray();
                name += String.Format(CultureInfo.InvariantCulture, "({0})", String.Join(",", parameterTypeNames));
            }

            return name;
        }

        private static string GetTagValue(XPathNavigator parentNode, string tagName)
        {
            if (parentNode != null)
            {
                XPathNavigator node = parentNode.SelectSingleNode(tagName);
                if (node != null)
                {
                    return node.Value.Trim();
                }
            }

            return null;
        }

        private XPathNavigator GetTypeNode(Type type)
        {
            string controllerTypeName = GetTypeName(type);
            var xPathNavigator = GetNodeNavigator(type, controllerTypeName);

            // controller is generic type but is dynamically initialized (no description for the initialization type)
            if (xPathNavigator == null)
            {
                controllerTypeName = GetTypeName(type, true);
                xPathNavigator = GetNodeNavigator(type, controllerTypeName);
            }

            return xPathNavigator;
        }

        private XPathNavigator GetNodeNavigator(Type type, string controllerTypeName)
        {
            string selectExpression = String.Format(CultureInfo.InvariantCulture, TypeExpression, controllerTypeName);
            var assemblyNavigator = GetAssemblyNavigator(this.documentNavigators, type.Assembly);
            var xPathNavigator = assemblyNavigator == null ? null : assemblyNavigator.SelectSingleNode(selectExpression);
            return xPathNavigator;
        }

        private static XPathNavigator GetAssemblyNavigator(IEnumerable<XPathNavigator> xpathNavigators, Assembly assembly)
        {
            foreach (var xpathNavigator in xpathNavigators)
            {
                var assemblyNameNode = xpathNavigator.SelectSingleNode(AssemblyNameExpression);
                if (assemblyNameNode != null && assemblyNameNode.Value.Equals(assembly.GetName().Name))
                    return xpathNavigator;
            }

            return null;
        }

        private static string GetTypeName(Type type, bool simpleGenericName = false)
        {
            string name = type.FullName;
            if (type.IsGenericType)
            {
                // Format the generic type name to something like: Generic{System.Int32,System.String}
                Type genericType = type.GetGenericTypeDefinition();
                Type[] genericArguments = type.GetGenericArguments();
                string genericTypeName = genericType.FullName;

                // If the request is to get the simple generic name without the arguments return
                if (simpleGenericName)
                    name = genericTypeName;
                else
                {
                    // Trim the generic parameter counts from the name
                    genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
                    string[] argumentTypeNames = genericArguments.Select(t => GetTypeName(t)).ToArray();
                    name = String.Format(CultureInfo.InvariantCulture, "{0}{{{1}}}", genericTypeName, String.Join(",", argumentTypeNames));
                }
            }
            if (type.IsNested)
            {
                // Changing the nested type name from OuterType+InnerType to OuterType.InnerType to match the XML documentation syntax.
                name = name.Replace("+", ".");
            }

            return name;
        }
    }
}
