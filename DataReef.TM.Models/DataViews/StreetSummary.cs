using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews
{
    [DataContract]
    [NotMapped]
    public class StreetSummary
    {
        [DataMember]
        public Guid TerritoryID { get; set; }

        [DataMember]
        public string StreetName { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }


        [DataMember]
        public int PropertyCount { get; set; }


        [DataMember]
        public int CompletedCount { get; set; }


        [DataMember]
        public double PercentCompleted
        {
            get
            {
                try
                {
                    return ((double) CompletedCount/PropertyCount);
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
    }
}