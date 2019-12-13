using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Google.Models
{
    public class SheetOptions
    {
        public string SheetID { get; set; }

        /// <summary>
        /// Use comma separated for values, and/or hyphen separated for range. (e.g. 1,2,5-7)
        /// </summary>
        public string Rows { get; set; }

        /// <summary>
        /// Specify the end column. e.g. AK
        /// </summary>
        public string EndColumn { get; set; }

        /// <summary>
        /// Specify the name of the Process column
        /// </summary>
        public string ProcessColumn { get; set; }

        /// <summary>
        /// Specify the name of the Process timestamp column
        /// </summary>
        public string ProcessTimestampColumn { get; set; }

        public bool SetProcessedFlag { get; set; }

        public List<string> GetRanges(string endColumn)
        {
            if (string.IsNullOrWhiteSpace(Rows))
            {
                return new List<string>();
            }

            var values = Rows
                            .Split(',');

            var rows = values
                            .Where(d => !d.Contains("-"))
                            .Select(v =>
                            {
                                int r;
                                return int.TryParse(v, out r) ? r : 0;
                            })
                            .ToList();
            var result = rows
                            .Select(r => $"A{r}:{endColumn}{r}")
                            .ToList();

            var ranges = values
                            .Where(d => d.Contains("-"))
                            .Select(v => v.Split('-'))
                            .ToList();

            result.AddRange(ranges.Select(r => $"A{r.FirstOrDefault()}:{endColumn}{r.LastOrDefault()}"));
            return result;
        }

        public int[] GetRows()
        {
            if (string.IsNullOrWhiteSpace(Rows))
            {
                return new int[0];
            }

            var values = Rows
                            .Split(',');

            var result = values
                            .Where(d => !d.Contains("-"))
                            .Select(v =>
                            {
                                int r;
                                return int.TryParse(v, out r) ? r : 0;
                            })
                            .ToList();

            var ranges = values
                            .Where(d => d.Contains("-"))
                            .ToList();

            foreach (var range in ranges)
            {
                var pair = range.Split('-');
                int start, end;
                if (int.TryParse(pair[0], out start)
                    && int.TryParse(pair[pair.Length - 1], out end)
                    && start <= end)
                {
                    for (int i = start; i <= end; i++)
                    {
                        result.Add(i);
                    }
                }
            }

            return result
                    .Where(r => r != 0)
                    .OrderBy(r => r)
                    .ToArray();
        }
    }
}
