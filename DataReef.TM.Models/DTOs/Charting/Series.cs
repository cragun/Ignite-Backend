using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Charting
{
    public class Series
    {
        public string Name { get; set; }
        public string LabelAlignment { get; set; }
        public List<Point> Points { get; set; }
    }
}
