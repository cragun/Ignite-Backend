namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Genabilitychanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "GenabilityProviderAccountID", c => c.String());
            AddColumn("dbo.Properties", "GenabilityAccountID", c => c.String());
            AddColumn("solar.Tariffs", "MasterTariffID", c => c.String());
            AddColumn("solar.Tariffs", "TariffCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.Tariffs", "TariffCode");
            DropColumn("solar.Tariffs", "MasterTariffID");
            DropColumn("dbo.Properties", "GenabilityAccountID");
            DropColumn("dbo.Properties", "GenabilityProviderAccountID");
        }
    }
}
