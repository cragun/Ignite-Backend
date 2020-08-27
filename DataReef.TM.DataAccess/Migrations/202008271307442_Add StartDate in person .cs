namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStartDateinperson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "StartDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "StartDate");
        }
    }
}
