namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCompoundPropertyConstraint : DbMigration
    {
        public override void Up()
        {
            Sql("CREATE UNIQUE INDEX IX_UniqueProperty ON [dbo].[Properties] (TerritoryID, ExternalID) WHERE ExternalID IS NOT NULL");
        }

        public override void Down()
        {
            Sql(@"IF EXISTS(
                        SELECT * 
                        FROM sys.indexes 
                        WHERE name='IX_UniqueProperty'
                        AND object_id = OBJECT_ID('Properties'))
                    DROP INDEX IX_UniqueProperty ON Properties");
        }
    }
}
