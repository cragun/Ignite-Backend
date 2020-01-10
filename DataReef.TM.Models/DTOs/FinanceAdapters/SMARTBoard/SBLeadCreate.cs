using DataReef.TM.Models.DTOs.FinanceAdapters.SST;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System.Linq;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard
{
    public class SBLeadCreateRequest
    {
        public SBLeadModel Lead { get; set; }
        public SBLeadKwhModel LeadKwh { get; set; }
        public SBCustomerModel Customer { get; set; }
        public SBAppointmentModel Appointment { get; set; }

        public SBEnergyUtilityModel EnergyUsage { get; set; }

        public SBUsageModel MonthlyUsage { get; set; }

        //public SBProposalDataModel ProjectDetails { get; set; }
        //[JsonProperty("HOA")]
        //public HOAModel HOA { get; set; }
    }

    public class HOAModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone_email")]
        public string PhoneEmail { get; set; }
    }

    public class SBLeadModel
    {
        public SBLeadModel(Property property, Person dealer, Person closer = null)
        {
            AssociatedId = property.Id;
            SalesOwner = dealer.EmailAddresses?.FirstOrDefault();
            Type = "Individual";

            var appointment = property?.GetLatestAppointment();
            Scheduler = appointment?.Creator?.EmailAddressString;
            Closer = closer?.EmailAddressString ?? appointment?.Assignee?.EmailAddressString;

            Latitude = property?.Latitude;
            Longitude = property?.Longitude;

            UtilityProviderID = property?.UtilityProviderID;
            UtilityProviderName = property?.UtilityProviderName;
            UsageCollected = property?.UsageCollected;
            LeadSource = property?.LeadSource;
            // Disposition = property?.LatestDisposition;
            DispositionTypeId = property?.DispositionTypeId;
        }

        [JsonProperty("associated_id")]
        public long AssociatedId { get; set; }

        /// <summary>
        /// "Individual" or "Business"
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = "Individual";

        /// <summary>
        /// "Individual" or "Business"
        /// </summary>
        [JsonProperty("tax_entity")]
        public string TaxEntity { get; set; }

        /// <summary>
        /// Sales owner's email
        /// </summary>
        [JsonProperty("sales_owner")]
        public string SalesOwner { get; set; }

        /// <summary>
        /// Email address of the person who created the appointment
        /// </summary>
        [JsonProperty("scheduler")]
        public string Scheduler { get; set; }

        /// <summary>
        /// Email address of the person who is assigned to the appointment
        /// </summary>
        [JsonProperty("closer")]
        public string Closer { get; set; }

        /// <summary>
        /// Latitude for the property location
        /// </summary>
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitude for the property location
        /// </summary>
        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        /// <summary>
        /// ID of the utility provider linked to the lead
        /// </summary>
        [JsonProperty("utility_provider_id")]
        public string UtilityProviderID { get; set; }

        /// <summary>
        /// Name of the utility provider linked to the lead
        /// </summary>
        [JsonProperty("utility_provider_name")]
        public string UtilityProviderName { get; set; }

        [JsonProperty("usageCollected")]
        public bool? UsageCollected { get; set; }


        [JsonProperty("leadSource")]
        public string LeadSource { get; set; }


        [JsonProperty("disposition_type_id")]
        public int? DispositionTypeId { get; set; }
        public string HoaName { get; set; }

        public string HoaPhoneEmail { get; set; }
    }

    public class SBCustomerModel
    {
        public SBCustomerModel(Property property)
        {
            var mainOccupant = property.GetMainOccupant();

            FirstName = mainOccupant?.FirstName;
            LastName = mainOccupant?.LastName;
            MiddleInitial = mainOccupant?.MiddleInitial;
            Address = property.Address1;
            AddressLine2 = property.Address2;
            City = property.City;
            State = property.State;
            Zip = property.ZipCode;
            Email = property.GetMainEmailAddress();
            Phone = property.GetMainPhoneNumber();
            Latitude = property.Latitude;
            Longitude = property.Longitude;
        }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("middle_initial")]
        public string MiddleInitial { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("address_line_2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("lat")]
        public double? Latitude { get; set; }

        [JsonProperty("lon")]
        public double? Longitude { get; set; }

    }

    public class SBAppointmentModel
    {
        public SBAppointmentModel(Property property)
        {
            var appointment = property.GetLatestAppointment();
            AssociatedID = appointment?.Id;
            EventDate = (appointment?.StartDate)?.ToString("yyyy-MM-dd");
            StartTime = (appointment?.StartDate)?.ToString("hh:mm tt");
            EndTime = (appointment?.EndDate)?.ToString("hh:mm tt");
            SBAppointmentType = "4";
        }

        [JsonProperty("associated_id")]
        public long? AssociatedID { get; set; }

        /// <summary>
        /// Date in YYYY-MM-DD format
        /// </summary>
        [JsonProperty("event_date")]
        public string EventDate { get; set; }

        /// <summary>
        /// Start time in HH:MM AM/PM format
        /// </summary>
        [JsonProperty("start")]
        public string StartTime { get; set; }

        /// <summary>
        /// Start time in HH:MM AM/PM format
        /// </summary>
        [JsonProperty("end")]
        public string EndTime { get; set; }

        /// <summary>
        /// SMARTBoard appointment type ID
        /// </summary>
        [JsonProperty("appointment_type_id")]
        public string SBAppointmentType { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }
    }

    public class SBLeadKwhModel
    {
        public SBLeadKwhModel(SolarSystem solarSystem)
        {
            TotalConsumptionInKwH = solarSystem?
                                        .PowerConsumption?
                                        .Sum(c => c.Watts);
        }
        /// <summary>
        /// Total consumption in kW
        /// </summary>
        [JsonProperty("kw_hours")]
        public decimal? TotalConsumptionInKwH { get; set; }
    }

    public class SBEnergyUtilityModel
    {

        public SBEnergyUtilityModel(Proposal proposal)
        {
            UtilityName = proposal?.Tariff?.UtilityName;
            UtilityAccountNumber = proposal?.Property?.GenabilityAccountID;
            AverageMonthlyBill = proposal?.SolarSystem?.PowerConsumption?.Average(pc => pc.Cost);
            TotalAnnualBill = proposal?.SolarSystem?.PowerConsumption?.Sum(pc => pc.Cost);
            AnnualConsumption = proposal?.SolarSystem?.PowerConsumption?.Sum(pc => pc.Watts) / 1000;
            AverageConsumption = proposal?.SolarSystem?.PowerConsumption?.Average(pc => pc.Watts) / 1000;
        }
        /// <summary>
        /// Residential or Commercial
        /// </summary>
        [JsonProperty("service_type")]
        public string ServiceType { get; set; } = "Residential";

        /// <summary>
        /// Name of utility
        /// </summary>
        [JsonProperty("utility")]
        public string UtilityName { get; set; }

        /// <summary>
        /// Power spec, i.e. "240 V : 120 V Split Phase"
        /// </summary>
        [JsonProperty("service_config_voltage")]
        public string ServiceConfigVoltage { get; set; }

        /// <summary>
        /// Customer's utility account number
        /// </summary>
        [JsonProperty("utility_account_number")]
        public string UtilityAccountNumber { get; set; }

        /// <summary>
        /// Average monthly bill
        /// </summary>
        [JsonProperty("average_billing_amount")]
        public decimal? AverageMonthlyBill { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Annual usage in kwh
        /// </summary>
        [JsonProperty("total_kwh")]
        public decimal? AnnualConsumption { get; set; }

        /// <summary>
        /// Average monthly usage in kwh
        /// </summary>
        [JsonProperty("average_kwh")]
        public decimal? AverageConsumption { get; set; }

        /// <summary>
        /// Total annual bill
        /// </summary>
        [JsonProperty("total_billing_amount")]
        public decimal? TotalAnnualBill { get; set; }
    }

    public class SBUsageModel
    {
        public SBUsageModel() { }

        public SBUsageModel(SolarSystem solarSystem)
        {
            if (solarSystem?.PowerConsumption == null)
            {
                return;
            }

            var properties = GetType().GetPropertiesWithAttribute<SBUsageAttribute>();

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttributes(typeof(SBUsageAttribute), false).FirstOrDefault() as SBUsageAttribute;
                if (attr == null)
                {
                    continue;
                }
                var value = solarSystem.PowerConsumption.FirstOrDefault(pc => pc.Month == attr.Month);

                if (value == null)
                {
                    continue;
                }

                switch (attr.UsageType)
                {
                    case SBUsageType.Consumption:
                        prop.SetValue(this, (int)value.Watts);
                        break;
                    case SBUsageType.Bill:
                        prop.SetValue(this, value.Cost);
                        break;
                    default:
                        break;
                }
            }

            AnnualConsumption = (int?)solarSystem.PowerConsumption?.Sum(pc => pc.Watts);
        }

        /// <summary>
        /// Annual consumption in watts
        /// </summary>
        [JsonProperty("annual_consumption")]
        public int? AnnualConsumption { get; set; }

        [SBUsage(Month = 1, UsageType = SBUsageType.Consumption)]
        [JsonProperty("jan_consumption")]
        public int Consumption01 { get; set; }

        [SBUsage(Month = 2, UsageType = SBUsageType.Consumption)]
        [JsonProperty("feb_consumption")]
        public int Consumption02 { get; set; }

        [SBUsage(Month = 3, UsageType = SBUsageType.Consumption)]
        [JsonProperty("mar_consumption")]
        public int Consumption03 { get; set; }

        [SBUsage(Month = 4, UsageType = SBUsageType.Consumption)]
        [JsonProperty("apr_consumption")]
        public int Consumption04 { get; set; }

        [SBUsage(Month = 5, UsageType = SBUsageType.Consumption)]
        [JsonProperty("may_consumption")]
        public int Consumption05 { get; set; }

        [SBUsage(Month = 6, UsageType = SBUsageType.Consumption)]
        [JsonProperty("jun_consumption")]
        public int Consumption06 { get; set; }

        [SBUsage(Month = 7, UsageType = SBUsageType.Consumption)]
        [JsonProperty("jul_consumption")]
        public int Consumption07 { get; set; }

        [SBUsage(Month = 8, UsageType = SBUsageType.Consumption)]
        [JsonProperty("aug_consumption")]
        public int Consumption08 { get; set; }

        [SBUsage(Month = 9, UsageType = SBUsageType.Consumption)]
        [JsonProperty("sep_consumption")]
        public int Consumption09 { get; set; }

        [SBUsage(Month = 10, UsageType = SBUsageType.Consumption)]
        [JsonProperty("oct_consumption")]
        public int Consumption10 { get; set; }

        [SBUsage(Month = 11, UsageType = SBUsageType.Consumption)]
        [JsonProperty("nov_consumption")]
        public int Consumption11 { get; set; }

        [SBUsage(Month = 12, UsageType = SBUsageType.Consumption)]
        [JsonProperty("dec_consumption")]
        public int Consumption12 { get; set; }



        [SBUsage(Month = 1, UsageType = SBUsageType.Bill)]
        [JsonProperty("jan_billing")]
        public decimal Bill01 { get; set; }

        [SBUsage(Month = 2, UsageType = SBUsageType.Bill)]
        [JsonProperty("feb_billing")]
        public decimal Bill02 { get; set; }

        [SBUsage(Month = 3, UsageType = SBUsageType.Bill)]
        [JsonProperty("mar_billing")]
        public decimal Bill03 { get; set; }

        [SBUsage(Month = 4, UsageType = SBUsageType.Bill)]
        [JsonProperty("apr_billing")]
        public decimal Bill04 { get; set; }

        [SBUsage(Month = 5, UsageType = SBUsageType.Bill)]
        [JsonProperty("may_billing")]
        public decimal Bill05 { get; set; }

        [SBUsage(Month = 6, UsageType = SBUsageType.Bill)]
        [JsonProperty("jun_billing")]
        public decimal Bill06 { get; set; }

        [SBUsage(Month = 7, UsageType = SBUsageType.Bill)]
        [JsonProperty("jul_billing")]
        public decimal Bill07 { get; set; }

        [SBUsage(Month = 8, UsageType = SBUsageType.Bill)]
        [JsonProperty("aug_billing")]
        public decimal Bill08 { get; set; }

        [SBUsage(Month = 9, UsageType = SBUsageType.Bill)]
        [JsonProperty("sep_billing")]
        public decimal Bill09 { get; set; }

        [SBUsage(Month = 10, UsageType = SBUsageType.Bill)]
        [JsonProperty("oct_billing")]
        public decimal Bill10 { get; set; }

        [SBUsage(Month = 11, UsageType = SBUsageType.Bill)]
        [JsonProperty("nov_billing")]
        public decimal Bill11 { get; set; }

        [SBUsage(Month = 12, UsageType = SBUsageType.Bill)]
        [JsonProperty("dec_billing")]
        public decimal Bill12 { get; set; }
    }
}
