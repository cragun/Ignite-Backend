using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Solar
{
    /// <summary>
    /// Object used to store requests and responses made to 3rd party services
    /// </summary>
    [Table("ProposalIntegrationAudits", Schema = "solar")]
    public class ProposalIntegrationAudit : EntityBase
    {
        /// <summary>
        /// The ProposalID
        /// </summary>
        public Guid? ProposalID { get; set; }
        public Guid? OUID { get; set; }
        public string Url { get; set; }
        public string RequestJSON { get; set; }
        public string ResponseJSON { get; set; }
    }
}
