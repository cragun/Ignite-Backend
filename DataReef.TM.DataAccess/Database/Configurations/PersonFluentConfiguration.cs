using DataReef.TM.Models;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal class PersonFluentConfiguration : FluentEntityConfiguration<Person>
    {
        public PersonFluentConfiguration()
        {
            //HasMany(p => p.BlacklistedFrom)
            //    .WithMany(b => b.BlackList)
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("StudentID");
            //        m.MapRightKey("PersonID");
            //        m.ToTable("BlackLists");
            //    });
        }
    }
}