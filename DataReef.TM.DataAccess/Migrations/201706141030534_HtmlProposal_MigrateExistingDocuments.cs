namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HtmlProposal_MigrateExistingDocuments : DbMigration
    {
        public override void Up()
        {
            Sql(@";with ExistingProposalDocuments(SignedURL, ProposalID) as
                (select  fd.SignedURL, p.Guid--, p.ProposalURL
                from solar.FinanceDocuments fd
                join solar.FinancePlans fp on fd.FinancePlanID = fp.Guid and fp.IsDeleted = 0
                join solar.Proposals p on fp.SolarSystemID = p.Guid and p.IsDeleted = 0 and p.ProposalURL is null
                where fd.IsDeleted = 0
                and fd.DocumentType = 1
                and fd.SignedURL is not null)
                update prop
                set ProposalURL = docs.SignedURL
                from solar.Proposals prop
                join ExistingProposalDocuments docs on prop.Guid = docs.ProposalID");
        }
        
        public override void Down()
        {
        }
    }
}
