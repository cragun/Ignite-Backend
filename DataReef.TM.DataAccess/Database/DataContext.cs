using DataReef.Core.Attributes;
using DataReef.TM.DataAccess.Database.Configurations;
using DataReef.TM.Models;
using DataReef.TM.Models.Accounting;
using DataReef.TM.Models.Client;
using DataReef.TM.Models.Commerce;
using DataReef.TM.Models.Credit;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.FinancialIntegration;
using DataReef.TM.Models.Geo;
using DataReef.TM.Models.Layers;
using DataReef.TM.Models.PropertyAttachments;
using DataReef.TM.Models.PRMI;
using DataReef.TM.Models.PushNotifications;
using DataReef.TM.Models.Reporting;
using DataReef.TM.Models.Solar;
using DataReef.TM.Models.Spruce;
using DataReef.TM.Models.Surveys;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace DataReef.TM.DataAccess.Database
{
    [Service(typeof(DbContext))]
    public class DataContext : DbContext
    {
        public DataContext(DbConnection dbConnection)
            : base(dbConnection, false)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DataContext(string connectionStringOrName) : base(connectionStringOrName)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DataContext()
            : base(System.Configuration.ConfigurationManager.AppSettings["ConnectionStringName"] ?? "DataContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<AccountAssociation> AccountAssociations { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<ApiToken> ApiTokens { get; set; }
        public DbSet<IntegrationToken> ApiIntegrationTokens { get; set; }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<FavouriteTerritory> FavouriteTerritories { get; set; }
        public DbSet<AppointmentFavouritePerson> AppointmentFavouritePersons { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Authentication> Authentications { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<ExternalCredential> ExternalCredentials { get; set; }
        public DbSet<CurrentLocation> CurrentLocations { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }
        public DbSet<HighResolutionImage> HighResolutionImages { get; set; }

        public DbSet<Identification> Identifications { get; set; }
        public DbSet<ImagePurchase> ImagePurchases { get; set; }
        public DbSet<Inquiry> Inquiries { get; set; }
        public DbSet<KeyValue> KeyValues { get; set; }

        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<OU> OUs { get; set; }
        public DbSet<OUAssociation> OUAssociations { get; set; }
        public DbSet<OURole> OURoles { get; set; }
        public DbSet<OUSetting> OUSettings { get; set; }
        public DbSet<OUShape> OUShapes { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<PersonClockTime> PersonClockTime { get; set; }
        public DbSet<PersonSetting> PersonSettings { get; set; }
        public DbSet<PersonKPI> PersonKPIs { get; set; }
        public DbSet<PersonalConnection> PersonalConnections { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertySurvey> PropertySurveys { get; set; }
        public DbSet<PropertyNote> PropertyNotes { get; set; }
        public DbSet<PropertyAttribute> PropertyAttributes { get; set; }
        public DbSet<PropertyRating> PropertyRatings { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Territory> Territories { get; set; }
        public DbSet<TerritoryShape> TerritoryShapes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<UserInvitation> UserInvitations { get; set; }
        public DbSet<OUMediaItem> OUMediaItems { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        //Agreements
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<AgreementPart> AgreementParts { get; set; }
        public DbSet<ExecutedAgreement> ExecutedAgreements { get; set; }
        public DbSet<ExecutedAgreementPart> ExecutedAgreementParts { get; set; }

        //Accounting
        public DbSet<TokenAdjustment> TokenAdjustments { get; set; }
        public DbSet<TokenExpense> TokenExpenses { get; set; }
        public DbSet<TokenLedger> TokenLedgers { get; set; }
        public DbSet<TokenPurchase> TokenPurchases { get; set; }
        public DbSet<TokenTransfer> TokenTransfers { get; set; }

        //Finance
        public DbSet<FinancePlanDefinition> FinancePlaneDefinitions { get; set; }
        public DbSet<FinanceDetail> FinanceDetails { get; set; }
        public DbSet<FinanceProvider> FinanceProviders { get; set; }
        public DbSet<OUFinanceAssociation> OUFinanceAssociations { get; set; }


        //Geo
        public DbSet<Field> Fields { get; set; }
        public DbSet<Occupant> Occupants { get; set; }

        //prescreens
        public DbSet<PrescreenInstant> PrescreenInstants { get; set; }
        public DbSet<PrescreenBatch> PrescreenBatches { get; set; }
        public DbSet<PrescreenDetail> PrescreenDetails { get; set; }

        //solar
        public DbSet<FinancePlan> FinancePlans { get; set; }
        public DbSet<Inverter> Inverters { get; set; }
        public DbSet<PowerConsumption> PowerConsumption { get; set; }
        public DbSet<Proposal> Proposal { get; set; }
        public DbSet<ProposalMediaItem> ProposalMediaItems { get; set; }
        public DbSet<ProposalIntegrationAudit> ProposalIntegrationAudits { get; set; }
        public DbSet<ProposalData> ProposalData { get; set; }
        public DbSet<FinanceDocument> FinanceDocuments { get; set; }
        public DbSet<SolarPanel> SolarPanel { get; set; }
        public DbSet<SolarSystem> SolarSystem { get; set; }
        public DbSet<SystemProduction> SystemsProduction { get; set; }
        public DbSet<SystemProductionMonth> SystemProductionMonths { get; set; }
        public DbSet<SolarTariff> SolarTariffs { get; set; }
        public DbSet<RoofPlane> RoofPlanes { get; set; }
        public DbSet<ProposalRoofPlaneInfo> ProposalRoofPlaneInfo { get; set; }
        public DbSet<RoofPlaneEdge> RoofPlaneEdges { get; set; }
        public DbSet<RoofPlaneObstruction> RoofPlaneObstructions { get; set; }
        public DbSet<ObstructionPoint> ObstructionPoints { get; set; }
        public DbSet<RoofPlanePanel> RoofPlanePanels { get; set; }
        public DbSet<RoofPlanePoint> RoofPlanePoints { get; set; }
        public DbSet<AdderItem> AdderItems { get; set; }
        public DbSet<RoofPlaneDetail> RoofPlaneDetails { get; set; }

        //Spruce
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<CoApplicant> CoApplicants { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<IncomeDebt> IncomeDebts { get; set; }
        public DbSet<Init> Inits { get; set; }
        public DbSet<SpruceProperty> SpruceProperties { get; set; }
        public DbSet<QuoteRequest> QuoteRequests { get; set; }
        public DbSet<QuoteResponse> QuoteResponses { get; set; }
        public DbSet<GenDocsRequest> GenDocsRequests { get; set; }

        //reports
        public DbSet<Column> Columns { get; set; }
        public DbSet<OUReport> OUReports { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Report> Reports { get; set; }

        //layers
        public DbSet<Layer> Layers { get; set; }
        public DbSet<OULayer> OULayers { get; set; }
        public DbSet<LayerFolder> LayerFolders { get; set; }
        public DbSet<LayerDefinition> LayerDefinitions { get; set; }

        //surveys
        public DbSet<Survey72> Surveys72 { get; set; }

        // prescreens
        public DbSet<ZipArea> ZipAreas { get; set; }
        public DbSet<Zip5Shape> Zip5Shapes { get; set; }
        public DbSet<AreaPurchase> AreaPurchases { get; set; }

        // API log
        public DbSet<ApiLogEntry> ApiLogEntries { get; set; }

        // Commerce
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        //  PRMI
        public DbSet<VelocifyRequest> VelocifyRequests { get; set; }
        public DbSet<VelocifyResponse> VelocifyResponses { get; set; }

        //  Financial integration
        public DbSet<AdapterRequest> AdapterRequests { get; set; }

        //  EPC
        public DbSet<EpcStatus> EpcStatuses { get; set; }
        public DbSet<PropertyActionItem> PropertyActionItems { get; set; }

        // Photos
        public DbSet<PropertyAttachment> PropertyAttachments { get; set; }
        public DbSet<PropertyAttachmentItem> PropertyAttachmentItems { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<CRUDAudit> Audits { get; set; }

        private static readonly IConvention[] Conventions =
        {
            new AttributeToTableAnnotationConvention<SoftDeleteAttribute, bool>("SoftDeleteEntity", (type, attributes) => attributes.Any()),
        };

        private static readonly IEnumerable<IFluentEntityConfiguration> Configurations = new List<IFluentEntityConfiguration>
        {
            new OUFluentConfiguration(),
            new PersonFluentConfiguration(),
            new TerritoryFluentConfiguration(),
            new ProposalFluentConfiguration(),
            new SolarSystemFluentConfiguration(),
            new PropertyFluentConfiguration(),
            new ZipAreaFluentConfiguration()
        };

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuoteResponse>().Property(x => x.LoanRate).HasPrecision(18, 6);
            // Build conventions
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Add(Conventions);

            // Build entity configurations
            foreach (var configuration in Configurations)
                configuration.Build(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}