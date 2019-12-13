namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdatingVelocifyRequestmodel : DbMigration
    {
        public override void Up()
        {
            Sql(@"IF COL_LENGTH('PRMI.VelocifyRequests', 'SalesRepEmail') IS NULL
                    BEGIN
                        ALTER TABLE PRMI.VelocifyRequests
                        ADD SalesRepEmail NVARCHAR(MAX)
                    END");
            AlterColumn("PRMI.VelocifyRequests", "SalesRepName", c => c.String(maxLength: 150));
            AlterColumn("PRMI.VelocifyRequests", "SalesRepPhone", c => c.String(maxLength: 150));
            AlterColumn("PRMI.VelocifyRequests", "SalesRepEmail", c => c.String(maxLength: 250));
            AlterColumn("PRMI.VelocifyRequests", "SalesRepCompanyName", c => c.String(maxLength: 250));
        }

        public override void Down()
        {
            AlterColumn("PRMI.VelocifyRequests", "SalesRepCompanyName", c => c.String());
            AlterColumn("PRMI.VelocifyRequests", "SalesRepEmail", c => c.String());
            AlterColumn("PRMI.VelocifyRequests", "SalesRepPhone", c => c.String());
            AlterColumn("PRMI.VelocifyRequests", "SalesRepName", c => c.String());
        }
    }
}
