namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_DeleteIsProposal : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.Proposals", "IsDefault");
        }
        
        public override void Down()
        {
            AddColumn("solar.Proposals", "IsDefault", c => c.Boolean(nullable: false));
        }
    }
}
