namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addsbactivity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "SBLastActivityDate", c => c.DateTime());
            AddColumn("dbo.People", "SBActivityName", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "SBActivityName");
            DropColumn("dbo.People", "SBLastActivityDate");
        }
    }
}
