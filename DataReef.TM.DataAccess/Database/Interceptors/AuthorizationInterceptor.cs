using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Core.Common.CommandTrees;


namespace DataReef.TM.DataAccess.Database.Interceptors
{
    public class AuthorizationInterceptor : IDbCommandTreeInterceptor
    {
        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {

         



            //Storage space. After the query has been passed through the mapping pipeline
            //Right before it is translated into SQL
            if (interceptionContext.OriginalResult.DataSpace == System.Data.Entity.Core.Metadata.Edm.DataSpace.SSpace)
            {
                var queryCommand = interceptionContext.Result as DbQueryCommandTree;
                if (queryCommand != null)
                {
                    var newQuery = queryCommand.Query.Accept(new AuthorizationQueryVisitor());

                    interceptionContext.Result = new DbQueryCommandTree(
                                                queryCommand.MetadataWorkspace,
                                                queryCommand.DataSpace,
                                                newQuery);
                }
            }
        }
    }


}
