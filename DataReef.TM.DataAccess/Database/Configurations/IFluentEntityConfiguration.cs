using System.Data.Entity;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal interface IFluentEntityConfiguration
    {
        void Build(DbModelBuilder modelBuilder);
    }
}