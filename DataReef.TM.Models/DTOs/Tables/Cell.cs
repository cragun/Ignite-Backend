
namespace DataReef.TM.Models.DTOs.Tables
{
    public class Cell
    {
        public string Value { get; set; }
        public VisibleBorders VisibleBorders { get; set; }

        public Cell()
        {
        }

        public Cell(string value, VisibleBorders visibleBorders = VisibleBorders.None)
        {
            Value = value;
            VisibleBorders = visibleBorders;
        }
    }
}
