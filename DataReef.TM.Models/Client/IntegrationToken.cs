using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Client
{
    [DataContract]
    public class IntegrationToken : EntityBase
    {
        /// <summary>
        /// User Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// UTC DateTime 
        /// </summary>
        [DataMember]
        public DateTime ExpirationDate { get; set; }

        #region Navigation Properties

        [ForeignKey(nameof(UserId))]
        public Person Person { get; set; }

        #endregion
    }
}
