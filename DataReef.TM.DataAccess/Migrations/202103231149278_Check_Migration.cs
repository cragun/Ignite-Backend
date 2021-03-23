namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Check_Migration : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Properties", "ReferenceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Properties", "ReferenceId", c => c.String(maxLength: 200));
        }
    }
}
