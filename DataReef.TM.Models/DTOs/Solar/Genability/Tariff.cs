using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// Tariffs are rate plans for electricity. They describe who the plan applies to (service and applicability), what the charges are, and other information about this electricity service.
    /// </summary>
    public class Tariff:BaseResponse
    {
        /// <summary>
        /// Unique Genability ID (primary key) for this tariff.
        /// </summary>
        public string tariffId { get; set; }

        /// <summary>
        /// Unique Genability ID that persists across all revisions of this tariff.
        /// </summary>
        public string masterTariffId { get; set; }

        /// <summary>
        /// Short code that the LSE uses as an alternate name for the tariff.
        /// </summary>
        public string tariffCode { get; set; }

        /// <summary>
        /// Name of the tariff as used by the LSE.
        /// </summary>
        public string tariffName { get; set; }

        /// <summary>
        /// ID of load serving entity this tariff belongs to.
        /// </summary>
        public string lseId { get; set; }

        /// <summary>
        /// Name of the load serving entity.
        /// </summary>
        public string lseName { get; set; }

        /// <summary>
        /// Type of service for the tariff. Current values include ELECTRICITY (grid power) and SOLAR_PV (Solar PPA or Lease).
        /// </summary>
        public string serviceType { get; set; }

        /// <summary>
        /// Unique Genability ID that identifies the prior revision of the tariffId above.
        /// </summary>
        public string priorTariffId { get; set; }

        /// <summary>
        /// In states like Texas where the load serving entity that sells the power is different than the load serving entity that distributes the power, this will contain the ID of the distribution LSE. Otherwise, it will be null.
        /// </summary>
        public string distributionLseId { get; set; }

        /// <summary>
        /// Possible values are:
        /// DEFAULT - tariff that is automatically given to this service class
        /// ALTERNATE - opt in alternate tariff for this service class
        /// OPTIONAL_EXTRA - opt in extra, such as green power or a smart thermostat program
        /// RIDER - charge that can apply to multiple tariffs. Often a regulatory mandated charge
        /// </summary>
        public string tariffType { get; set; }

        /// <summary>
        /// Possible values are:
        /// RESIDENTIAL - homes, apartments etc.
        /// GENERAL - commercial, industrial and other business and organization service types (often have additional applicability criteria)
        /// SPECIAL_USE - examples are government, agriculture, street lighting, transportation
        /// </summary>
        public string customerClass { get; set; }

        /// <summary>
        /// Number of customers that are on this master tariff.
        /// </summary>
        public int? customerCount { get; set; }

        /// <summary>
        /// The likelihood that a customer is on this tariff of all the tariffs in the search results. Only populated when getting more than one tariff.
        /// </summary>
        public decimal? customerLikelihood { get; set; }

        /// <summary>
        /// Where we got the customer count numbers from. Typically FERC (form 1 filings) or Genability (our own estimates).
        /// </summary>
        public string customerCountSource { get; set; }

        /// <summary>
        /// ID of the territory that this tariff applies to. This is typically the service area for the LSE in this regulatory region (i.e. a state in the USA).
        /// </summary>
        public string territoryId { get; set; }

        /// <summary>
        /// Date on which the tariff was or will be effective.
        /// </summary>
        public DateTime? effectiveDate { get; set; }

        /// <summary>
        /// Date on which this tariff is no longer effective. Can be null which means end date is not known or tariff is open ended.
        /// </summary>
        public DateTime? endDate { get; set; }

        /// <summary>
        /// If populated (usually is), its the timezone that this tariffs dates and times refer to.
        /// </summary>
        public string timeZone { get; set; }

        /// <summary>
        /// The minimum chargePeriod present on this tariff.
        /// </summary>
        public string billingPeriod { get; set; }

        /// <summary>
        /// ISO Currency code that the rates for this tariff refer to (e.g. USD for USA Dollar).
        /// </summary>
        public string currency { get; set; }

        /// <summary>
        /// A comma separated list of all the different ChargeType rates on this tariff.
        /// </summary>
        public string chargeTypes { get; set; }

        /// <summary>
        /// The most fine grained period for which charges are calculated.
        /// </summary>
        public string chargePeriod { get; set; }

        /// <summary>
        /// When applicable, the maximum monthly consumption allowed in order to be eligible for this tariff.
        /// </summary>
        public decimal? minMonthlyConsumption { get; set; }

        /// <summary>
        /// When applicable, the minimum monthly consumption allowed in order to be eligible for this tariff.
        /// </summary>
        public decimal? maxMonthlyConsumption { get; set; }

        /// <summary>
        /// When applicable, the maximum monthly demand allowed in order to be eligible for this tariff.
        /// </summary>
        public decimal? minMonthlyDemand { get; set; }

        /// <summary>
        /// When applicable, the minimum monthly demand allowed in order to be eligible for this tariff.
        /// </summary>
        public decimal? maxMonthlyDemand { get; set; }

        /// <summary>
        /// Indicates whether this tariff contains one or more Time of Use Rate.
        /// </summary>
        public bool? hasTimeOfUseRates { get; set; }

        /// <summary>
        /// Indicates whether this tariff contains one or more Tiered Rate.
        /// </summary>
        public bool? hasTieredRates { get; set; }

        /// <summary>
        /// Indicates whether this tariff contains one or more Rate that can be contracted (sometimes called by-passable or associated with a price to compare).
        /// </summary>
        public bool? hasContractedRates { get; set; }

        /// <summary>
        /// Indicates that this tariff has additional eligibility criteria, as specified in the TariffProperty collection (below).
        /// </summary>
        public bool? hasTariffApplicability { get; set; }

        /// <summary>
        /// Indicates that one or more rates on this tariff are only applicable to customer with a particular circumstance. 
        /// When true, this will be specified in the TariffProperty collection, and also on the TariffRate or rates in question.
        /// </summary>
        public bool? hasRateApplicability { get; set; }

        /// <summary>
        /// Indicates whether this tariff contains one or more net metered rates.
        /// </summary>
        public bool? hasNetMetering { get; set; }

        /// <summary>
        /// Tariff properties represent metadata about a tariff.
        /// </summary>
        public List<TariffProperty> properties { get; set; }

        /// <summary>
        /// The rates for this tariff.
        /// </summary>
        public List<TariffRate> rates { get; set; }

    }
}
