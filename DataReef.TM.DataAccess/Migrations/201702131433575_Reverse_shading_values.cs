namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Reverse_shading_values : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE solar.roofplanes
                  SET Shading = 100 - shading

                  UPDATE OUSettings
                  SET Value = 14
                  WHERE Name = 'SolarDefaultShading'");
        }

        public override void Down()
        {
            Sql(@"UPDATE solar.roofplanes
                  SET Shading = 100 - shading

                  UPDATE OUSettings
                  SET Value = 98
                  WHERE Name = 'SolarDefaultShading'");
        }
    }
}
