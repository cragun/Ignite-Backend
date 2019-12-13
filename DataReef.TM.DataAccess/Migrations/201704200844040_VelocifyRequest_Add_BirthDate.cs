namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VelocifyRequest_Add_BirthDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("PRMI.VelocifyRequests", "BirthDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("PRMI.VelocifyRequests", "BirthDate");
        }
    }
}
