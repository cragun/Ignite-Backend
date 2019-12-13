namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PersonSettingsReplaceSettingNameWithName : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE dbo.PersonSettings SET Name = SettingName");
            DropColumn("dbo.PersonSettings", "SettingName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PersonSettings", "SettingName", c => c.String());
            Sql("UPDATE dbo.PersonSettings SET SettingName = Name");
        }
    }
}
