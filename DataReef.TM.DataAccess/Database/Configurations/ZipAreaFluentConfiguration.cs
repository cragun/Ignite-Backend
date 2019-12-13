using DataReef.TM.Models;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal class ZipAreaFluentConfiguration : FluentEntityConfiguration<ZipArea>
    {
        public ZipAreaFluentConfiguration()
        {
            HasRequired(za => za.Shape).WithRequiredPrincipal(zs => zs.ZipArea);
        }
    }
}