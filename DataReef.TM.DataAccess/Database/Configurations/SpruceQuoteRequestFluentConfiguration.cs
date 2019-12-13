using DataReef.TM.Models.Spruce;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal class SpruceQuoteRequestFluentConfiguration : FluentEntityConfiguration<QuoteRequest>
    {
        public SpruceQuoteRequestFluentConfiguration()
        {
            HasRequired(q => q.AppEmployment).WithRequiredPrincipal(q => q.AppQuote);
            HasOptional(q => q.CoAppEmployment).WithOptionalPrincipal(q => q.CoAppQuote);

            HasRequired(q => q.AppIncomeDebt).WithRequiredPrincipal(q => q.AppQuote);
            HasOptional(q => q.CoAppIncomeDebt).WithOptionalPrincipal(q => q.CoAppQuote);
        }
    }
}