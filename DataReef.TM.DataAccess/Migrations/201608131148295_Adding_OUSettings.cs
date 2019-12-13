namespace DataReef.TM.DataAccess.Migrations
{
    using Models;
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_OUSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OUSettings",
                c => new
                {
                    Guid = c.Guid(nullable: false),
                    OUID = c.Guid(nullable: false),
                    Value = c.String(),
                    ValueIsDefault = c.Boolean(nullable: false),
                    Group = c.Int(nullable: false, defaultValue: (int)OUSettingGroupType.ConfigurationFile),
                    Inheritable = c.Boolean(nullable: false, defaultValue: true),
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
                .ForeignKey("dbo.OUs", t => t.OUID)
                .Index(t => t.OUID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);

            //add proc_OUSettings Stored Procedure
            string sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/Add_OUSettings.sql");
            SqlFile(sqlMigrationPath);

            // update OUTreeUP function w/ a fix.
            Sql("DROP FUNCTION [dbo].[OUTreeUP]");
            sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/OUTreeUP_SQL_Function.sql");
            SqlFile(sqlMigrationPath);

            // Migrating OU Custom Fields & Values to OUSettings
            sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/OUCustomFields_and_Values_to_OUSettings.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
            DropForeignKey("dbo.OUSettings", "OUID", "dbo.OUs");
            DropIndex("dbo.OUSettings", new[] { "ExternalID" });
            DropIndex("dbo.OUSettings", new[] { "Version" });
            DropIndex("dbo.OUSettings", new[] { "CreatedByID" });
            DropIndex("dbo.OUSettings", new[] { "DateCreated" });
            DropIndex("dbo.OUSettings", new[] { "TenantID" });
            DropIndex("dbo.OUSettings", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUSettings", new[] { "OUID" });
            AlterColumn("Spruce.QuoteResponses", "LoanRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("dbo.OUSettings");

            //drop proc_OUSettings Stored Procedure
            Sql("DROP PROCEDURE proc_OUSettings");

        }
    }
}
