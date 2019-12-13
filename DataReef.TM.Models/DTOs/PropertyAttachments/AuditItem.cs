using DataReef.Core.Infrastructure.Authorization;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    public class AuditItem
    {
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; }
        public Guid UserID { get; set; } = SmartPrincipal.UserId;
        public string Action { get; set; }

        /// <summary>
        /// Audited item IDs (image ID)
        /// </summary>
        public List<string> ItemIDs { get; set; }

    }
}
