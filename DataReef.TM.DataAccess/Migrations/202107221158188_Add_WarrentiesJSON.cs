namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_WarrentiesJSON : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.ProposalsData", "WarrentiesJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.ProposalsData", "WarrentiesJSON");
        }
    }
}
