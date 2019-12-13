namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_PRMI_PhoneNumber : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE finance.providers
                 SET PhoneNumber = '(888)817-2344'
                 WHERE Name = 'PRMI'");
        }

        public override void Down()
        {
            Sql(@"UPDATE finance.providers
                 SET PhoneNumber = NULL
                 WHERE Name = 'PRMI'");
        }
    }
}
