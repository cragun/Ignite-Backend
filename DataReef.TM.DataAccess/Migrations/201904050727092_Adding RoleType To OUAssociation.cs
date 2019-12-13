namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models.Enums;
    using System.Data.Entity.Migrations;

    public partial class AddingRoleTypeToOUAssociation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUAssociations", "RoleType", c => c.Int(nullable: false, defaultValue: (int)OURoleType.None));

            Sql(@"UPDATE OUA
                    SET RoleType = OUR.RoleType
                    FROM OUAssociations OUA
                    INNER JOIN OURoles OUR
                    ON OUA.OURoleID = OUR.Guid");
        }

        public override void Down()
        {
            DropColumn("dbo.OUAssociations", "RoleType");
        }
    }
}
