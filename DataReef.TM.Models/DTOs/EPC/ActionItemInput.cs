using System;
using DataReef.TM.Models.Enums;

namespace DataReef.TM.Models.DTOs.EPC
{
    public class ActionItemInput
    {
        public Guid? Guid { get; set; }

        public Guid PropertyID { get; set; }

        public Guid? PersonID { get; set; }

        public string Description { get; set; }

        public ActionItemStatus? Status { get; set; }
    }
}