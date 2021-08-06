namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsDefaultProposal : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Proposals", "IsDefault");
        }
    }
}
