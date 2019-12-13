namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addednavigationpropertiesonappointment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "AssigneeID", c => c.Guid());
            CreateIndex("dbo.Appointments", "AssigneeID");
            AddForeignKey("dbo.Appointments", "AssigneeID", "dbo.People", "Guid");
            AddForeignKey("dbo.Appointments", "CreatedByID", "dbo.People", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Appointments", "CreatedByID", "dbo.People");
            DropForeignKey("dbo.Appointments", "AssigneeID", "dbo.People");
            DropIndex("dbo.Appointments", new[] { "AssigneeID" });
            DropColumn("dbo.Appointments", "AssigneeID");
        }
    }
}
