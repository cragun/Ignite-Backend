namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSurvey72model : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "survey.Survey72",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        ClientName = c.String(nullable: false),
                        AlternateClientName = c.String(),
                        StreetAddress = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        ZipCode = c.String(),
                        PhoneNumber = c.String(nullable: false),
                        AlternatePhoneNumber = c.String(),
                        Email = c.String(),
                        TypeOfResidence = c.String(),
                        EquipmentAge = c.Int(nullable: false),
                        EquipmentLocation = c.String(),
                        AgeOfHome = c.Int(nullable: false),
                        TypeOfAppointment = c.String(),
                        Price = c.Double(nullable: false),
                        AppointmentDate = c.DateTime(nullable: false),
                        AppointmentTimeInterval = c.String(nullable: false),
                        RepresentativeNotes = c.String(),
                        RepresentativeName = c.String(),
                        LeadSource = c.String(),
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Flags = c.Long(),
                        TenantID = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateLastModified = c.DateTime(),
                        CreatedByName = c.String(maxLength: 100),
                        CreatedByID = c.Guid(),
                        LastModifiedBy = c.Guid(),
                        LastModifiedByName = c.String(maxLength: 100),
                        Version = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ExternalID = c.String(maxLength: 50),
                        TagString = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Properties", t => t.Guid)
                .Index(t => t.Guid)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("survey.Survey72", "Guid", "dbo.Properties");
            DropIndex("survey.Survey72", new[] { "ExternalID" });
            DropIndex("survey.Survey72", new[] { "Version" });
            DropIndex("survey.Survey72", new[] { "CreatedByID" });
            DropIndex("survey.Survey72", new[] { "DateCreated" });
            DropIndex("survey.Survey72", new[] { "TenantID" });
            DropIndex("survey.Survey72", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("survey.Survey72", new[] { "Guid" });
            DropTable("survey.Survey72");
        }
    }
}
