namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedSolarPanelAndInverter : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Inverters", "Efficiency", c => c.Double(nullable: false));
            AddColumn("solar.Panels", "ModuleType", c => c.Int(nullable: false));
            Sql("Update solar.Panels set ModuleType = 1 where OUID = '2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'");       //  module type Premium for SunEdison panels
            Sql("Update solar.Panels set ModuleType = 0 where OUID = 'C5F80842-F4EB-425E-A93D-0571E357D18A'");       //  module type Standard for Solcius panels
            Sql("Update solar.Inverters set Efficiency = 97 where OUID = '2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'");   //  efficency 97% for   SunEdison inverters
            Sql("Update solar.Inverters set Efficiency = 96.5 where OUID = 'C5F80842-F4EB-425E-A93D-0571E357D18A'"); //  efficency 96.5% for Solcius inverters
        }
        
        public override void Down()
        {
            DropColumn("solar.Panels", "ModuleType");
            DropColumn("solar.Inverters", "Efficiency");
        }
    }
}
