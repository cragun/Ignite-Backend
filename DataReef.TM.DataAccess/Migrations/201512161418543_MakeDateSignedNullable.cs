namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeDateSignedNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("solar.Proposals", "DateSigned", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("solar.Proposals", "DateSigned", c => c.DateTime(nullable: false));
        }
    }
}
