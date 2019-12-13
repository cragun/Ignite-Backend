using System.Linq.Expressions;

namespace DataReef.TM.DataAccess.Prototype
{
    public class AuthorizationVisitor : ExpressionVisitor
    {
        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }
    }
}
