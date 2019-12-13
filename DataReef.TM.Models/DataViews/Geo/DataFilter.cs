using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.Geography
{

    public enum FilterOperator
    {
        Undefined = 0,
        GreaterThan = 1,
        GreaterThanOrEqual = 2,
        LessThan = 3,
        LessThanOrEqual = 4,
        Equals = 5,
        In = 6,
        NotIn = 7,
        Between = 8,
        NotBetween = 9,
        NotEquals = 10
    }

    public class DataFilter
    {
        private List<string> values = new List<string>();

        public string OperandID { get; set; }
        public FilterOperator Comparitor { get; set; }

        public List<string> Values
        {
            get { return this.values; }
            set { this.values = value; }
        }

    }
}