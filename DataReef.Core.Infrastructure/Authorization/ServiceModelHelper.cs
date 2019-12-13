using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace DataReef.Core.Infrastructure.Authorization
{
    public static class ServiceModelHelper
    {
        public static MethodInfo GetActionMethod(OperationContext operationContext)
        {
            Uri addressedEndpointUri = operationContext.Channel.LocalAddress.Uri;

            ServiceEndpoint addressedEnpoint = operationContext.Host.Description.Endpoints.First(e => e.Address.Uri == addressedEndpointUri);

            string calledAction = operationContext.IncomingMessageHeaders.Action;
            OperationDescription operation = addressedEnpoint.Contract.Operations.First(e => MatchOperation(e, calledAction));

            MethodInfo methodInfo = operation.SyncMethod ?? operation.TaskMethod;

            return methodInfo;
        }

        public static bool MatchOperation(OperationDescription operation, string calledAction)
        {
            object operationSigniture = ConcatWithSeparator(new[] { operation.DeclaringContract.Namespace, operation.DeclaringContract.Name, operation.Name }, "/");

            return operationSigniture.Equals(calledAction);
        }

        public static object ConcatWithSeparator(string[] values, string separator)
        {
            return values.Aggregate((current, next) => current.EndsWith(separator, StringComparison.Ordinal) ? current + next : current + separator + next);
        }
    }
}