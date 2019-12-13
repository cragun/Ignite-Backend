
using Newtonsoft.Json;
using System.Collections.Generic;
namespace DataReef.TM.Models.DTOs.Charting
{
    public class Chart
    {
        public ChartType Type { get; set; }

        public bool DrawZeroVisible { get; set; }

        public string ValueLabelFormat { get; set; }

        public List<Series> Series { get; set; }

        public Legend Legend { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Chart FromString(string barChart)
        {
            return JsonConvert.DeserializeObject<Chart>(barChart);
        }
    }

    public class Legend
    {
        public Alignment Alignment { get; set; }

        public Docking Docking { get; set; }
    }

    public enum Alignment
    {
        Near = 0,
        Center = 1,
        Far = 2
    }

    public enum Docking
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3
    }
}
