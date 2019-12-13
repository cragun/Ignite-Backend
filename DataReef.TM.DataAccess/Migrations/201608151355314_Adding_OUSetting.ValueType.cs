namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Adding_OUSettingValueType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUSettings", "ValueType", c => c.Int(nullable: false));
Sql(@"
Update OUSettings
SET Name = 'Proposal', ValueType = 1
WHERE Name = 'ProposalEnabled'

Update OUSettings
SET ValueType = 1
WHERE Name = 'PPA'

Update OUSettings
SET Name = 'Loan', ValueType = 1
WHERE Name = 'SolarOwn'

Update OUSettings
SET Name = 'Program ID', ValueType = 0
WHERE Name = 'ProgramID'

Update OUSettings
SET Name = 'Sales Rep ID', ValueType = 0
WHERE Name = 'SalesRepID'

Update OUSettings
SET Name = 'Contractor ID', ValueType = 0
WHERE Name = 'CompanyID'
");
        }
        
        public override void Down()
        {
            DropColumn("dbo.OUSettings", "ValueType");
        }
    }
}
