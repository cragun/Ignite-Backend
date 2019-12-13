using System;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.Enums;

namespace DataReef.TM.ClientApi.Models
{
    public class ActionItemDataView
    {
        public ActionItemDataView(PropertyActionItem item)
        {
            if (item == null)
                return;

            ID = item.Guid;
            Property = item.Property != null ? new PropertyDataView(item.Property) : null;
            Person = item.Person != null ? new PersonDataView(item.Person) : null;
            Description = item.Description;
            Status = item.Status;
        }

        public Guid ID { get; set; }

        public PropertyDataView Property { get; set; }

        public PersonDataView Person { get; set; }

        public string Description { get; set; }

        public ActionItemStatus Status { get; set; }
    }
}
