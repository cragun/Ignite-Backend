namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecreasePersonEmailAddressTo450MaxIndexSize : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.People", new[] { "EmailAddressString" });
            AlterColumn("dbo.People", "EmailAddressString", c => c.String(maxLength: 450));
            CreateIndex("dbo.People", "EmailAddressString", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.People", new[] { "EmailAddressString" });
            AlterColumn("dbo.People", "EmailAddressString", c => c.String(maxLength: 900));
            CreateIndex("dbo.People", "EmailAddressString", unique: true);
        }
    }
}
