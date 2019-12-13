namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removing_OUCustomFields_and_Values : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OUCustomFields", "OUID", "dbo.OUs");
            DropForeignKey("dbo.OUCustomValues", "CustomFieldID", "dbo.OUCustomFields");
            DropForeignKey("dbo.OUCustomValues", "OUID", "dbo.OUs");
            DropIndex("dbo.OUCustomFields", new[] { "OUID" });
            DropIndex("dbo.OUCustomFields", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUCustomFields", new[] { "TenantID" });
            DropIndex("dbo.OUCustomFields", new[] { "DateCreated" });
            DropIndex("dbo.OUCustomFields", new[] { "CreatedByID" });
            DropIndex("dbo.OUCustomFields", new[] { "Version" });
            DropIndex("dbo.OUCustomFields", new[] { "ExternalID" });
            DropIndex("dbo.OUCustomValues", new[] { "CustomFieldID" });
            DropIndex("dbo.OUCustomValues", new[] { "OUID" });
            DropIndex("dbo.OUCustomValues", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUCustomValues", new[] { "TenantID" });
            DropIndex("dbo.OUCustomValues", new[] { "DateCreated" });
            DropIndex("dbo.OUCustomValues", new[] { "CreatedByID" });
            DropIndex("dbo.OUCustomValues", new[] { "Version" });
            DropIndex("dbo.OUCustomValues", new[] { "ExternalID" });
            DropColumn("solar.FinancePlans", "LoanRequestJSON");
            DropColumn("solar.FinancePlans", "LoanResponseJSON");
            DropColumn("solar.FinancePlans", "PPARequestJSON");
            DropColumn("solar.FinancePlans", "PPAResponseJSON");
            DropTable("dbo.OUCustomFields");
            DropTable("dbo.OUCustomValues");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OUCustomValues",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        CustomFieldID = c.Guid(nullable: false),
                        Value = c.String(nullable: false, maxLength: 200),
                        OwnerID = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.OUCustomFields",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 50),
                        IsRequired = c.Boolean(nullable: false),
                        HasValueList = c.Boolean(nullable: false),
                        ValueListType = c.Int(nullable: false),
                        DataType = c.Int(nullable: false),
                        DataLength = c.Int(nullable: false),
                        FormatString = c.String(maxLength: 50),
                        ClassName = c.String(maxLength: 100),
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
                .PrimaryKey(t => t.Guid);
            
            AddColumn("solar.FinancePlans", "PPAResponseJSON", c => c.String());
            AddColumn("solar.FinancePlans", "PPARequestJSON", c => c.String());
            AddColumn("solar.FinancePlans", "LoanResponseJSON", c => c.String());
            AddColumn("solar.FinancePlans", "LoanRequestJSON", c => c.String());
            CreateIndex("dbo.OUCustomValues", "ExternalID");
            CreateIndex("dbo.OUCustomValues", "Version");
            CreateIndex("dbo.OUCustomValues", "CreatedByID");
            CreateIndex("dbo.OUCustomValues", "DateCreated");
            CreateIndex("dbo.OUCustomValues", "TenantID");
            CreateIndex("dbo.OUCustomValues", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.OUCustomValues", "OUID");
            CreateIndex("dbo.OUCustomValues", "CustomFieldID");
            CreateIndex("dbo.OUCustomFields", "ExternalID");
            CreateIndex("dbo.OUCustomFields", "Version");
            CreateIndex("dbo.OUCustomFields", "CreatedByID");
            CreateIndex("dbo.OUCustomFields", "DateCreated");
            CreateIndex("dbo.OUCustomFields", "TenantID");
            CreateIndex("dbo.OUCustomFields", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.OUCustomFields", "OUID");
            AddForeignKey("dbo.OUCustomValues", "OUID", "dbo.OUs", "Guid");
            AddForeignKey("dbo.OUCustomValues", "CustomFieldID", "dbo.OUCustomFields", "Guid");
            AddForeignKey("dbo.OUCustomFields", "OUID", "dbo.OUs", "Guid");
        }
    }
}
