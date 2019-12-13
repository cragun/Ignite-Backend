using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Tables
{
    public class Row
    {
        public bool IsHeader { get; set; }
        public List<Cell> Cells { get; set; }

        public Row()
        {
        }

        public Row(params Cell[] cells)
        {
            Cells = new List<Cell>(cells);
        }

        public Row(params string[] cellValues)
        {
            Cells = cellValues.Select(cv => new Cell(cv)).ToList();
        }
    }
}
