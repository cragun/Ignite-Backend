namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WebHook_Added_QueueName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebHooks", "QueueName", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebHooks", "QueueName");
        }
    }
}
