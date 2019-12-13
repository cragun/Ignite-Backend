namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendedlengthofAddress2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Addresses", "Address2", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Addresses", "Address2", c => c.String(maxLength: 20));
        }
    }
}
