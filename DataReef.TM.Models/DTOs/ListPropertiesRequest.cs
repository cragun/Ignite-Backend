using System;

namespace DataReef.TM.Models.DTOs
{
    public class ListPropertiesRequest
    {
        public bool DeletedItems { get; set; }

        public int PageNumber { get; set; } = 1;

        public int ItemsPerPage { get; set; } = 20;

        public string Include { get; set; }

        public DateTime? LastRetrievedDate { get; set; }
    }
}