using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Tables
{
    public class Table
    {
        public List<Row> Rows { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Table FromString(string table)
        {
            return JsonConvert.DeserializeObject<Table>(table);
        }
    }
}
