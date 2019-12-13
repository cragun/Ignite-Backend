namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Property_Pitch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "Pitch", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "Pitch");
        }
    }
}
