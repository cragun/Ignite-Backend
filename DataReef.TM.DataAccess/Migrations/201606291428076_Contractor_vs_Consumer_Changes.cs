namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Contractor_vs_Consumer_Changes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Spruce.Applicants", "BirthDate", c => c.DateTime());
            AlterColumn("Spruce.CoApplicants", "BirthDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("Spruce.CoApplicants", "BirthDate", c => c.DateTime(nullable: false));
            AlterColumn("Spruce.Applicants", "BirthDate", c => c.DateTime(nullable: false));
        }
    }
}
