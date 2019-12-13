using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataReef.TM.DataAccess.Prototype
{
    public class AuthorizationQueryable<T> : IOrderedQueryable<T>
    {
        private readonly IQueryable<T> internalQuery;        
        private readonly Repository repository;

        public AuthorizationQueryable(IQueryable<T> query, Repository repository)
       { 
            if (query == null)
                throw new ArgumentNullException("query");

            this.internalQuery = query;
            this.Expression = query.Expression;
            
            this.Provider = new AuthorizationQueryProvider(query.Provider);
            this.ElementType = typeof(T);

            this.repository = repository;

        }

        public IEnumerator<T> GetEnumerator()
        {
            return internalQuery.GetEnumerator();            
        }
        public IQueryable<T> Include(string path)
        {
            throw new UnauthorizedAccessException("Incude method is not authorized");
            //return (internalQuery as IQueryable<object>).Include(path) as IQueryable<T>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Type ElementType { get; internal set; }

        public Expression Expression { get; internal set; }

        public IQueryProvider Provider { get; internal set; }
    }

    public class AuthorizationQueryable : IOrderedQueryable
    {
        private readonly IQueryable internalQuery;        
        public AuthorizationQueryable(IQueryable query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            this.internalQuery = query;
            this.Expression = query.Expression;            
            this.ElementType = query.ElementType;
        }

        public IQueryable Include(string path)
        {
            throw new UnauthorizedAccessException("Incude method is not authorized");
            //return internalQuery.Include(path);
        }

        public Type ElementType { get; internal set; }

        public Expression Expression { get; internal set; }

        public IQueryProvider Provider { get; internal set; }

        public IEnumerator GetEnumerator()
        {
            return internalQuery.GetEnumerator();            
        }
    }

    /// <summary>
    /// Custom QueryProvider that will ensure two things
    /// 1. Wrap any new CreateQuery result over a DataLocalizationQuery
    /// 2. Translation visit before any execution
    /// </summary>
    internal class AuthorizationQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider baseProvider;        

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="baseProvider">The underlying provider to be called when executing</param>
        /// <param name="visitor">Expresison visitor that can handle replacement for datalocalization properties </param>
        /// <param name="cultureProvider">Current culture provider used for any translation</param>
        public AuthorizationQueryProvider(IQueryProvider baseProvider)
        {
            if (baseProvider == null)
                throw new ArgumentNullException("baseProvider");            

            this.baseProvider = baseProvider;            
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            //todo: revisit this
            return new AuthorizationQueryable<TElement>(baseProvider.CreateQuery<TElement>(expression), null);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new AuthorizationQueryable(baseProvider.CreateQuery(expression));
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return baseProvider.Execute<TResult>(expression);
        }

        public object Execute(Expression expression)
        {
            return baseProvider.Execute(expression);
        }
    }

}
