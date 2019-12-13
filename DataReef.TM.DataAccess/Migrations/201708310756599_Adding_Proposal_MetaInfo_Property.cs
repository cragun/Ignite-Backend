namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_Proposal_MetaInfo_Property : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "MetaInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.Proposals", "MetaInfo");
        }
    }
}
