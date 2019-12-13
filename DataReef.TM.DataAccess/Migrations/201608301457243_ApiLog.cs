namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApiLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApiLogEntries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        User = c.String(),
                        Machine = c.String(),
                        RequestIpAddress = c.String(),
                        RequestContentType = c.String(),
                        RequestContentBody = c.String(),
                        RequestUri = c.String(),
                        RequestMethod = c.String(),
                        RequestRouteTemplate = c.String(),
                        RequestRouteData = c.String(),
                        RequestHeaders = c.String(),
                        RequestTimestamp = c.DateTime(),
                        ResponseContentType = c.String(),
                        ResponseContentBody = c.String(),
                        ResponseStatusCode = c.Int(),
                        ResponseHeaders = c.String(),
                        ResponseTimestamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ApiLogEntries");
        }
    }
}
