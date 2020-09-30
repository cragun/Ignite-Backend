namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFiledsinProposal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "fcm_token", c => c.String());
            AddColumn("dbo.People", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("solar.Proposals", "ProductionKWH", c => c.Double(nullable: false));
            AddColumn("solar.Proposals", "ProductionKWHpercentage", c => c.Double(nullable: false));
            AddColumn("solar.Proposals", "IsManual", c => c.Boolean(nullable: false));
            AddColumn("solar.Proposals", "SystemSize", c => c.Double(nullable: false));
            DropColumn("dbo.Users", "StartDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "StartDate", c => c.DateTime());
            DropColumn("solar.Proposals", "SystemSize");
            DropColumn("solar.Proposals", "IsManual");
            DropColumn("solar.Proposals", "ProductionKWHpercentage");
            DropColumn("solar.Proposals", "ProductionKWH");
            DropColumn("dbo.People", "StartDate");
            DropColumn("dbo.People", "fcm_token");
        }
    }
}
