using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DataReef.Core.Helpers
{
    /// <summary>
    /// A collection of type helpers.
    /// </summary>
    public static class TypeHelpers
    {
        /// <summary>
        /// Retrieves the property information from a strongly typed lambda.
        /// </summary>
        public static PropertyInfo GetPropertyInfo<TSource>(Expression<Func<TSource, object>> propertyLambda)
        {
            var type = typeof(TSource);

            LambdaExpression lambda = propertyLambda;
            MemberExpression memberExpression;

            var body = lambda.Body as UnaryExpression;

            if (body != null)
            {
                var unaryExpression = body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            var propInfo = memberExpression.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(
                    $"Expression '{propertyLambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }

        /// <summary>
        /// Retrieves the property information from a strongly typed lambda.
        /// </summary>
        public static string GetPropertyName<TSource>(Expression<Func<TSource, object>> propertyLambda)
        {
            LambdaExpression lambda = propertyLambda;
            MemberExpression memberExpression;

            var body = lambda.Body as UnaryExpression;

            if (body != null)
            {
                var unaryExpression = body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member.Name;
        }
    }
}
