using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using DataReef.Core.Attributes;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.TM.DataAccess.Exceptions;
using DataReef.TM.Models;

namespace DataReef.TM.DataAccess.Repository.Interceptors
{
    [Service("AuditFieldsInterceptor", typeof(IEntityInterceptor))]
    public class AuditFieldsInterceptor : IEntityInterceptor
    {
        public int Priority => 80;

        public InterceptorType Type => InterceptorType.None;

        private static readonly Dictionary<string, Func<object>> SpecialInsertProperties = new Dictionary<string, Func<object>>
        {
            {InterceptorConstants.DateCreatedProperty, () => DateTime.UtcNow},
            {InterceptorConstants.DateLastModifiedProperty, () => null},
            {InterceptorConstants.CreatedByProperty, () => SmartPrincipal.UserId},
            {InterceptorConstants.LastModifiedByProperty, () => null},
            {InterceptorConstants.VersionProperty, () => 0},
        };

        private static readonly Dictionary<string, Func<EntityBase, object>> SpecialUpdateProperties = new Dictionary<string, Func<EntityBase, object>>
        {
            {InterceptorConstants.LastModifiedByProperty, e => SmartPrincipal.UserId},
            {InterceptorConstants.DateLastModifiedProperty, e => DateTime.UtcNow},
            {InterceptorConstants.VersionProperty, e => e.Version + 1},
        };

        private static readonly HashSet<string> ForbidenUpdateProperties = new HashSet<string>
        {
            InterceptorConstants.CreatedByProperty,
            InterceptorConstants.DateCreatedProperty,
            InterceptorConstants.TenantProperty,
        };

        public IQueryable<T> InterceptSelect<T>(IQueryable<T> query) where T : DbEntity
        {
            return query;
        }

        public void InterceptAdded(DbEntityEntry entry, DataSaveOperationContext dataSaveOperationContext)
        {
            var specialPropertyNames = SpecialInsertProperties.Select(k => k.Key).ToArray();

            foreach (var property in entry.Entity.GetType().GetProperties().Where(p => specialPropertyNames.Contains(p.Name)))
            {
                Func<object> valueGeneratorFunction;
                if (!SpecialInsertProperties.TryGetValue(property.Name, out valueGeneratorFunction))
                    throw new InterceptorException("Failed to intercept insert property " + property.Name + " on " + entry.Entity.GetType().Name + "!");

                property.SetValue(entry.Entity, valueGeneratorFunction());
            }
        }

        public void InterceptUpdated(DbEntityEntry entry, DataSaveOperationContext dataSaveOperationContext)
        {
            var specialPropertyNames = SpecialUpdateProperties.Select(k => k.Key).Concat(ForbidenUpdateProperties).Distinct().ToArray();

            var propertyInfos = entry.Entity.GetType().GetProperties().Where(p => specialPropertyNames.Contains(p.Name)).ToList();

            foreach (var specialUpdateProperty in SpecialUpdateProperties)
            {
                var propertyInfo = propertyInfos.FirstOrDefault(p => p.Name.Equals(specialUpdateProperty.Key, StringComparison.CurrentCultureIgnoreCase));

                if (propertyInfo == null)
                    continue;

                var newValue = specialUpdateProperty.Value(entry.Entity as EntityBase);
                propertyInfo.SetValue(entry.Entity, newValue);
            }

            foreach (var forbiddenUpdateProperty in ForbidenUpdateProperties)
            {
                var propertyInfo = propertyInfos.FirstOrDefault(p => p.Name.Equals(forbiddenUpdateProperty, StringComparison.CurrentCultureIgnoreCase));

                if (propertyInfo == null)
                    continue;

                var dbPropertyEntry = entry.Property(forbiddenUpdateProperty);
                var originalPropertyValue = dbPropertyEntry.OriginalValue;
                propertyInfo.SetValue(entry.Entity, originalPropertyValue);
                dbPropertyEntry.CurrentValue = dbPropertyEntry.OriginalValue;
                dbPropertyEntry.IsModified = false;
            }
        }

        public void InterceptPostSave(IEnumerable<EntityAndState> modifiedEntities, DbContext dbContext)
        {
        }
    }
}
