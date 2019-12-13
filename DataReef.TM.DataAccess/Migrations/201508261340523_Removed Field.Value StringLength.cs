namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedFieldValueStringLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Fields", "Value", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Fields", "Value", c => c.String(maxLength: 1000));
        }
    }
}
