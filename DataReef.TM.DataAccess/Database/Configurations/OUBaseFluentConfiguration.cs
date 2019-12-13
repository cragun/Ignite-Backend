using DataReef.TM.Models;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal class OUFluentConfiguration : FluentEntityConfiguration<OU>
    {
        public OUFluentConfiguration()
        {
            HasOptional(ou => ou.Parent)
                .WithMany(ou => ou.Children)
                .Map(m => m.MapKey("ParentID"));
        }
    }
}