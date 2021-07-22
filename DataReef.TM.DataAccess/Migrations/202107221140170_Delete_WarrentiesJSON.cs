namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Delete_WarrentiesJSON : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.ProposalsData", "WarrentiesJSON");
        }
        
        public override void Down()
        {
            AddColumn("solar.ProposalsData", "WarrentiesJSON", c => c.String());
        }
    }
}
