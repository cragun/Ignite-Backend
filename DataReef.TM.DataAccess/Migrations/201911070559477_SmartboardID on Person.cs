namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SmartboardIDonPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "SmartBoardID", c => c.String(maxLength: 25));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "SmartBoardID");
        }
    }
}
