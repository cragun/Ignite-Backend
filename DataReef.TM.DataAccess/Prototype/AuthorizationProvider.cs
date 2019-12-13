using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataReef.TM.DataAccess.Prototype
{
    public class AuthorizationFiltersProvider
    {
        //ToDo : is this the best data stucture for this?
        public IDictionary<Type, object> AuthorizationPredicates
        {
            get;
            private set;
        }

        public AuthorizationContext Context { get; private set; }

        public AuthorizationFiltersProvider(AuthorizationContext context)
        {
            this.AuthorizationPredicates = new Dictionary<Type, object>();
            this.Context = context;
            Init();
        }

        private void Init()
        {
            AuthorizationPredicates = new Dictionary<Type, object>();

            switch (Context.ApplicationRole)
            {
                case ApplicationRole.Parent:
                    BuildParentFilters();
                    break;
                default:
                    break;
            }
        }


        public Expression<Func<TEntity, bool>> GetEntityFilter<TEntity>()
        {
            var type = typeof(TEntity);

            if (!AuthorizationPredicates.ContainsKey(type))
            {
                return True<TEntity>();
            }

            return AuthorizationPredicates[type] as Expression<Func<TEntity, bool>>;
        }

        private Expression<Func<TEntity, bool>> True<TEntity>()
        {
            var type = typeof(TEntity);

            var alwaysTrue = Expression.Constant(true);
            var parameter1 = Expression.Parameter(type);
            var lambdaExpression = Expression.Lambda(alwaysTrue, parameter1);
            return lambdaExpression as Expression<Func<TEntity, bool>>;
        }

        private AuthorizationFiltersProvider AddFilter<TEntity>(Expression<Func<TEntity, bool>> authorizationPredicate)
        {
            AuthorizationPredicates[typeof(TEntity)] = authorizationPredicate;
            return this;
        }


        private void BuildParentFilters()
        {
            if (Context.ApplicationRole != ApplicationRole.Parent)
                throw new ArgumentException("Invalid authorization context");

            //this.AddFilter<Person>(person => person.Relationships.Any(r => r.ToPerson.Guid == Context.PersonGuid) || person.Guid == Context.PersonGuid)

            //    .AddFilter<Family>(family => family.PersonalRelationships.Any(r => r.FromPersonID == Context.PersonGuid || r.ToPersonID == Context.PersonGuid))
            //    //.AddFilter<Student> .... ???? what next
            //    .AddFilter<Center>(center => center.Families.Any(f => 
            //                                      f.PersonalRelationships.Any(r => r.ToPersonID == Context.PersonGuid || r.FromPersonID == Context.PersonGuid)));


        }

    }


}
