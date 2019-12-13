namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedInquiryStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inquiries", "OldDisposition", c => c.String(maxLength: 50));
            DropColumn("dbo.Properties", "LatestStatus");
            DropIndex("dbo.Inquiries", "_dta_index_Inquiries_8_629577281__K18_K14_K5_K2");
            DropIndex("dbo.Inquiries", "_dta_index_Inquiries_8_629577281__K18_K14_K5_K2_K3");
            DropColumn("dbo.Inquiries", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Inquiries", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "LatestStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Inquiries", "OldDisposition");

            var query = @"CREATE NONCLUSTERED INDEX [_dta_index_Inquiries_8_629577281__K18_K14_K5_K2] ON [dbo].[Inquiries]
(
	[DateCreated] ASC,
	[Id] ASC,
	[Status] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO";

            Sql(query);

            query = @"CREATE NONCLUSTERED INDEX [_dta_index_Inquiries_8_629577281__K18_K14_K5_K2_K3] ON [dbo].[Inquiries]
(
	[DateCreated] ASC,
	[Id] ASC,
	[Status] ASC,
	[PropertyID] ASC,
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO";

            Sql(query);
        }
    }
}
