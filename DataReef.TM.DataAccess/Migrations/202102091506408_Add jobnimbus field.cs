namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addjobnimbusfield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "JobNimbusLeadID", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "JobNimbusLeadID");
        }
    }
}
