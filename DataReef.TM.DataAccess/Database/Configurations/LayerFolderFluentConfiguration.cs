using DataReef.TM.Models.Layers;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal class LayerFolderFluentConfiguration : FluentEntityConfiguration<LayerFolder>
    {
        public LayerFolderFluentConfiguration()
        {
            HasMany(lf => lf.Children)
                .WithOptional(lf => lf.Parent)
                .HasForeignKey(lf => lf.ParentID);

        }
    }
}