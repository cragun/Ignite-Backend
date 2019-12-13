namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakingVelocifyRequestBirthdaynullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("PRMI.VelocifyRequests", "BirthDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("PRMI.VelocifyRequests", "BirthDate", c => c.DateTime(nullable: false));
        }
    }
}
