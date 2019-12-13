using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    public class Agreement : EntityBase
    {

        #region Properties

        [DataMember(EmitDefaultValue = false)]
        public Guid OUID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Content { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(255)]
        public string Uri { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(255)]
        public string MimeType { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public bool IsActive { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public bool RequiresSignature { get; set; }

        [DataMember]
        public bool IsRequired { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        #endregion

        #region Navigation

        [DataMember(EmitDefaultValue = false)]
        [ForeignKey("OUID")]
        public OU OU { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ICollection<AgreementPart> Parts { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ICollection<ExecutedAgreement> ExecutedAgreements { get; set; }


        #endregion


        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Parts               = FilterEntityCollection(Parts, newInclusionPath).ConvertAll(e => (AgreementPart)e);
            ExecutedAgreements  = FilterEntityCollection(ExecutedAgreements, newInclusionPath).ConvertAll(e => (ExecutedAgreement)e);

            FilterEntity(OU, newInclusionPath);

        }


    }
}
