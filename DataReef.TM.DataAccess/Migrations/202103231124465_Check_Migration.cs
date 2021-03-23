namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Check_Migration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "ReferenceId", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "ReferenceId");
        }
    }
}
