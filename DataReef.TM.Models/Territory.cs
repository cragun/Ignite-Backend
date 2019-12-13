using DataReef.Core.Attributes;
using DataReef.TM.Models.Credit;
using DataReef.TM.Models.DataViews;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Runtime.Serialization;


namespace DataReef.TM.Models
{
    [DataContract]
    public enum TerritoryStatus
    {
        [EnumMember]
        Open,

        [EnumMember]
        Closed,
    }

    [DataContract]
    public enum DataStatus
    {
        [EnumMember]
        Pending,

        [EnumMember]
        InProcess,

        [EnumMember]
        Complete,

        [EnumMember]
        Error
    }

    [CascadeSoftDelete("Territories", "Assignments", "TerritoryID")]
    public class Territory : EntityBase
    {

        public Territory()
        {
            Summary = new TerritorySummary();
            Summary.Territory = this;
        }

        #region Properties

        /// <summary>
        ///     Turns the territory on or off
        /// </summary>
        [DataMember]
        public TerritoryStatus Status { get; set; }

        /// <summary>
        ///     The status of the underlying Property and Occupant data for the territory.  populated by a worker so status is used
        ///     to convey to the client.. the status of the data
        /// </summary>
        [DataMember]
        public DataStatus DataStatus { get; set; }


        [JsonIgnore]
        [DataMember]
        public DbGeography Shape { get; set; }


        [DataMember]
        public string WellKnownText
        {
            get;
            set;
        }


        [DataMember]
        public Guid OUID { get; set; }

        [DataMember]
        public Guid? ShapeID { get; set; }

        [NotMapped]
        [DataMember]
        public TerritorySummary Summary { get; set; }

        [DataMember]
        public float CentroidLat { get; set; }

        [DataMember]
        public float CentroidLon { get; set; }

        [DataMember]
        public float Radius { get; set; }

        [DataMember]
        public int ShapesVersion { get; set; }

        [DataMember]
        public int? PropertyCount { get; set; }

        [DataMember]
        public bool IsArchived { get; set; }

        #endregion

        #region Navigation

        [ForeignKey("OUID")]
        [DataMember]
        public OU OU { get; set; }

        [DataMember]
        [AttachOnUpdate]
        public ICollection<Assignment> Assignments { get; set; }

        [InverseProperty("Territory")]
        [DataMember]
        [AttachOnUpdate]
        public ICollection<PrescreenBatch> Prescreens { get; set; }

        [DataMember]
        [InverseProperty("Territory")]
        [AttachOnUpdate]
        public ICollection<TerritoryShape> Shapes { get; set; }


        #endregion



        public static Territory Sample()
        {
            var ret = new Territory();

            ret.Name = "My Territory";
            ret.Guid = Guid.NewGuid();
            ret.Status = TerritoryStatus.Open;
            ret.IsDeleted = false;
            ret.DateCreated = DateTime.UtcNow;
            ret.OUID = Guid.NewGuid();
            //ret.WellKnownText="'POLYGON( (0 0, 30 0, 30 30, 0 30, 0 0) )'";

            return ret;
        }

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Assignments = FilterEntityCollection(Assignments, newInclusionPath);
            Prescreens = FilterEntityCollection(Prescreens, newInclusionPath);
            Shapes = FilterEntityCollection(Shapes, newInclusionPath);

            OU = FilterEntity(OU, newInclusionPath);

        }
    }
}