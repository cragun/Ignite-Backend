
namespace DataReef.TM.Models.DTOs.Solar.Genability.Enums
{
    public enum TariffTypes
    {
        /// <summary>
        /// Tariff that is automatically given to this service class.
        /// </summary>
        DEFAULT,

        /// <summary>
        /// Opt in alternate tariff for this service class.
        /// </summary>
        ALTERNATIVE,

        /// <summary>
        /// Opt in extra, such as green power or a smart thermostat program.
        /// </summary>
        OPTIONAL_EXTRA,

        /// <summary>
        /// Charge that can apply to multiple tariffs. Often a regulatory mandated charge.
        /// </summary>
        RIDER

    }
}
