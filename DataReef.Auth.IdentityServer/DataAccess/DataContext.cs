using System.Data.Entity;
using DataReef.Auth.Core.Models;
using System.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace DataReef.Auth.IdentityServer.DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Credential> Credentials { get; set; }

        public DataContext()
            : base(ConfigurationManager.AppSettings["ConnectionStringName"])
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Credential>()
                .Property(p => p.Username)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));

            modelBuilder
                .Entity<User>()
                .Property(p => p.UserId)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_UniqueUserPerOU", 1) { IsUnique = true }));
            modelBuilder
                .Entity<User>()
                .Property(p => p.OuId)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_UniqueUserPerOU", 2) { IsUnique = true }));
        }
    }
}