using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    public class GetTariffsResponse
    {
        /// <summary>
        /// The response status.
        /// </summary>
        public string status        { get; set; }

        /// <summary>
        /// The results count.
        /// </summary>
        public int count            { get; set; }

        /// <summary>
        /// The response type.
        /// </summary>
        public string type          { get; set; }

        /// <summary>
        /// The returned tariffs.
        /// </summary>
        public List<Tariff> results { get; set; }
    }
}
