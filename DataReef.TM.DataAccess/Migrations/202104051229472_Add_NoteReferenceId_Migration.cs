namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_NoteReferenceId_Migration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "NoteReferenceId", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "NoteReferenceId");
        }
    }
}
