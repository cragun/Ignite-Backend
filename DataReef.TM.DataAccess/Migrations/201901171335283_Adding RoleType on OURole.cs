namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models.Enums;
    using System.Data.Entity.Migrations;

    public partial class AddingRoleTypeonOURole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OURoles", "RoleType", c => c.Int(nullable: false, defaultValue: (int)OURoleType.Member));

            Sql("UPDATE OURoles set RoleType = 3 WHERE IsOwner = 1 AND IsAdmin = 1");
            Sql("UPDATE OURoles set RoleType = 1 WHERE IsOwner = 1 AND IsAdmin = 0");
            Sql("UPDATE OURoles set RoleType = 2 WHERE IsOwner = 0 AND IsAdmin = 1");

            Sql(@"insert into OURoles
                (Guid, IsActive, IsAdmin, Name, TenantID, DateCreated, [Version], IsDeleted, IsOwner, RoleType )
                VALUES
                ('fbdaf230-a7c2-4ddf-941b-8db0ca5046a2', 1, 0, 'Installer', 0, GETUTCDATE(), 0, 0, 0, 8)");
        }

        public override void Down()
        {
            DropColumn("dbo.OURoles", "RoleType");
        }
    }
}
