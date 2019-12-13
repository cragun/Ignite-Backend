namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendedAddress2fieldonPropertyBase : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Properties", "Address2", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Properties", "Address2", c => c.String(maxLength: 20));
        }
    }
}
