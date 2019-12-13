using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;

namespace DataReef.TM.DataAccess.Database.Interceptors
{
    public class SoftDeleteQueryVisitor : DefaultExpressionVisitor
    {
        //ToDo: Test the IgnoreSoftDeletes with real business test cases
        public SoftDeleteQueryVisitor()
        {
            IgnoreSoftDeletes = new HashSet<StructuralType>();
        }
        
        private HashSet<StructuralType> IgnoreSoftDeletes { get; set; }

        public override DbExpression Visit(DbFilterExpression expression)
        {
            var binaryExpression = expression.Predicate as DbBinaryExpression;
            if (binaryExpression != null)
                ScanBinaryExpression(binaryExpression);

            return base.Visit(expression);
        }

        private void ScanBinaryExpression(DbBinaryExpression binaryExpression)
        {
            var left = binaryExpression.Left as DbBinaryExpression;
            var right = binaryExpression.Right as DbBinaryExpression;
            if (left != null)
            {
                ScanBinaryExpression(left);
            }
            if (right != null)
            {
                ScanBinaryExpression(right);
            }

            var propertyExpression = binaryExpression.Left as DbPropertyExpression;
            if (propertyExpression != null)
            {
                ScanPropertyExpression(propertyExpression);
            }
        }

        private void ScanPropertyExpression(DbPropertyExpression propertyExpression)
        {
            var propertyName = propertyExpression.Property.Name;
            if (propertyName == "IsDeleted")
            {
                IgnoreSoftDeletes.Add(propertyExpression.Property.DeclaringType);
            }
        }


        public override DbExpression Visit(DbScanExpression expression)
        {
            if (IgnoreSoftDeletes.Contains(expression.Target.ElementType))
                return expression;

            if (SoftDeleteHelper.IsSoftDeleteEntity(expression.Target.ElementType))
            {
                var binding = DbExpressionBuilder.Bind(expression);
                return DbExpressionBuilder.Filter(binding,
                                DbExpressionBuilder.NotEqual(
                                    DbExpressionBuilder.Property(DbExpressionBuilder.Variable(binding.VariableType, binding.VariableName), "IsDeleted"),
                                    DbExpression.FromBoolean(true)));

            }
            else
            {
                return base.Visit(expression);
            }
        }
    }
}
