using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DataReef.TM.InternalServices
{
    [DebuggerDisplay("{Date} - R: {RecurrentEntitites.Count} - I: {IncidentalEntitites.Count}")]
    public class DailySchedueEntities<TRe, TIe>
    {
        public DailySchedueEntities(DateTime date)
        {
            this.Date = date;
            this.RecurrentEntitites = new List<TRe>();
            this.IncidentalEntitites = new List<TIe>();
        }

        public DateTime Date { get; set; }

        public List<TRe> RecurrentEntitites { get; set; }

        public List<TIe> IncidentalEntitites { get; set; }
    }
}