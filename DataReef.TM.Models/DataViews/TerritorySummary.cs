using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DataReef.TM.Models;

namespace DataReef.TM.Models.DataViews
{
    [DataContract]
    [NotMapped]
    public class TerritorySummary
    {
        [DataMember]
        public Guid TerritoryID { get; set; }

        [DataMember]
        public int PropertyCount { get; set; }

        [DataMember]
        public int CompletedCount { get; set; }

        [DataMember]
        public int PositivePrescreenNotContactedCount { get; set; }

        [DataMember]
        public int CustomerCount { get; set; }

        [DataMember]
        public int IneligibleCount { get; set; }

        [DataMember]
        public int SaleCount { get; set; }

        [DataMember]
        public int CallbackCount { get; set; }

        [DataMember]
        public int PrescreenPassCount { get; set; }

        [DataMember]
        public double AvgRating { get; set; }

        [DataMember]
        public int AssignmentCount { get; set; }

        [DataMember]
        public double PercentCompleted
        {
            get
            {
                try
                {
                    return (CompletedCount/((double) PropertyCount));
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            private set { }
        }

        [DataMember]
        public int CompletionBucket
        {
            get
            {
                try
                {
                    if (double.IsNaN(PercentCompleted)) return 0;

                    return Convert.ToInt16(Math.Round((PercentCompleted*10))*10);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            private set { }
        }

        public Territory Territory { get; set; }
    }
}