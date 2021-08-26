namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Ous_OURoleID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "OURoleID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OUs", "OURoleID");
        }
    }
}
