using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews
{
    [NotMapped]
    public class OUMembersRequest
    {
        /// <summary>
        /// Organization Guids
        /// </summary>
        [DataMember]
        public List<Guid> OUIDs { get; set; }
        /// <summary>
        /// Organization Guids to exclude.
        /// </summary>
        [DataMember]
        public List<Guid> ExcludeOUs { get; set; }
        [DataMember]
        public int PageNumber { get; set; }
        /// <summary>
        /// Default value is 20
        /// </summary>
        [DataMember]
        public int ItemsPerPage { get; set; }
        [DataMember]
        public string Filter { get; set; }
        [DataMember]
        public string Include { get; set; }
        [DataMember]
        public string Fields { get; set; }
        [DataMember]
        public string SortColumn { get; set; }
        /// <summary>
        /// "asc" or "desc". 
        /// Default value is "asc"
        /// </summary>
        [DataMember]
        public string SortOrder { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool OnlyDeleted { get; set; }

        [DataMember]
        public string Query { get; set; }

        public OUMembersRequest()
        {
            PageNumber = 1;
            ItemsPerPage = 20;
            SortOrder = "asc";
        }

    }
}
