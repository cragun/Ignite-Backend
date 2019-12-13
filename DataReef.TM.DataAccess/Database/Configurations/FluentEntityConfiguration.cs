using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal abstract class FluentEntityConfiguration<TEntityType> : EntityTypeConfiguration<TEntityType>, IFluentEntityConfiguration where TEntityType : class
    {
        public void Build(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(this);
        }
    }
}