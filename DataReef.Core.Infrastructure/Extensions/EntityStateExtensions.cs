using System;
using System.Data.Entity;
using DataReef.Core.Enums;

namespace DataReef.Core.Infrastructure.Extensions
{
    public static class EntityStateExtensions
    {
        public static DataAction ToDataAction(this EntityState state)
        {
            switch (state)
            {
                case EntityState.Detached:
                case EntityState.Unchanged:
                    return DataAction.None;

                case EntityState.Added:
                    return DataAction.Insert;

                case EntityState.Deleted:
                    return DataAction.Delete;

                case EntityState.Modified:
                    return DataAction.Update;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state));
            }
        }
    }
}
