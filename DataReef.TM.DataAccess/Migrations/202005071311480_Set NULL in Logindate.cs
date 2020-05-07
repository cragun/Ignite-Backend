namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetNULLinLogindate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.People", "LastLoginDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "LastLoginDate", c => c.DateTime(nullable: false));
        }
    }
}
