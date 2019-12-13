namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models;
    using DataReef.TM.Models.Enums;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updating_PersonSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonSettings", "ValueType", c => c.Int(nullable: false, defaultValue: (int)SettingValueType.String));
            AddColumn("dbo.PersonSettings", "Group", c => c.Int(nullable: false, defaultValue: (int)PersonSettingGroupType.Prescreen));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonSettings", "Group");
            DropColumn("dbo.PersonSettings", "ValueType");
        }
    }
}
