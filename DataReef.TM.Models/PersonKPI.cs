using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models
{
    public class PersonKPI : EntityBase
    {
        #region Properties
        /// <summary>
        /// Guid of the Person.
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public int Value { get; set; }
        
        #endregion

        #region Navigation

        [ForeignKey("PersonID")]
        [DataMember]
        public Person Person { get; set; }       

        #endregion
    }
}
