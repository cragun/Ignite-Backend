namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_SolarPanel_Efficiency : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Panels", "Efficiency", c => c.Double(nullable: false, defaultValue: 0));

            // Manually looked through all the datasheets and added the values below
            Sql(GetUpdateQuery("SunEdison 270 - Black on Black", "SunEdison Silvantis F Series  270", 16.4));
            Sql(GetUpdateQuery("Hanwha Q Cells", "Q.PRO BFR G4 260", 15.6));
            Sql(GetUpdateQuery("SolarWorld", "SW 280 Mono Black", 16.7));
            Sql(GetUpdateQuery("Sunniva", "OPT275-60-4-1B0", 16.95));
            Sql(GetUpdateQuery("Sunniva", "OPT285-60-4-1B0", 17.34));
            Sql(GetUpdateQuery("SolarWorld", "MOD-SW-290-BOB", 17.3));
            Sql(GetUpdateQuery("Hyundai", "MOD-HYU-285-BOB", 14.69));
            Sql(GetUpdateQuery("Trina", "MOD-TS-285-BOB", 16.2));
            Sql(GetUpdateQuery("LG NeON 2", "LG335N1C-A5", 19.6));
            Sql(GetUpdateQuery("LG NeON 2", "LG330N1C-A5", 19.3));
            Sql(GetUpdateQuery("LG NeON 2", "LG325N1C-A5", 19.0));
            Sql(GetUpdateQuery("REC", "REC275TP2 BLK2", 16.5));
            Sql(GetUpdateQuery("REC", "REC280TP2 BLK2", 16.8));
            Sql(GetUpdateQuery("REC", "REC285TP2 BLK2", 17.1));
            Sql(GetUpdateQuery("SunPower", "SPR-X22-360", 22.2));
        }

        public override void Down()
        {
            DropColumn("solar.Panels", "Efficiency");
        }

        private string GetUpdateQuery(string panelName, string description, double efficiency)
        {
            return $@"UPDATE solar.Panels
                      SET Efficiency = {efficiency}
                      WHERE Name = '{panelName}' AND Description = '{description}'";
        }
    }
}
