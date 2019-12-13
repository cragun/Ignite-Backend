namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Proposal_OUID : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "OUID", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("solar.Proposals", "OUID");
        }
    }
}
