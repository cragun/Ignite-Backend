namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuoteResponseDecisionGetTimeNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Spruce.QuoteResponses", "DecisionDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("Spruce.QuoteResponses", "DecisionDateTime", c => c.DateTime(nullable: false));
        }
    }
}
