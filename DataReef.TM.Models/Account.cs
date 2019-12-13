using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;


namespace DataReef.TM.Models
{

    /// <summary>
    /// Accounts are paying entities.  In TM, they are the master company we sign up.  In Leap, they are the Company, or User if solo
    /// </summary>
    public class Account: EntityBase
    {

        /// <summary>
        /// Disbables the Account, Its OUS and all territories 
        /// </summary>
        [DataMember]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// The Guid of the Person who owns the Account
        /// </summary>
        [DataMember]
        public Guid OwnerID { get; set; }

        /// <summary>
        /// The GUID of the root ou for an account
        /// </summary>
        [DataMember]
        public Guid? RootOUID { get; set; }


        #region Navigation

        /// <summary>
        /// The peson who owns the account
        /// </summary>
        [DataMember]
        [ForeignKey("OwnerID")]
        public Person Owner { get; set; }

        /// <summary>
        /// The person who owns the account
        /// </summary>
        [DataMember]
        [ForeignKey("RootOUID")]
        public OU RootOU { get; set; }

        /// <summary>
        /// Collection of Many to Many objects of people who have rights to the account.  Rights are granted on an OU level
        /// </summary>
        [DataMember]
        [InverseProperty("Account")]
        public ICollection<AccountAssociation> Associations { get; set; }

        /// <summary>
        /// Collection of Many to Many objects of people who have rights to the account.  Rights are granted on an OU level
        /// </summary>
        [DataMember]
        [InverseProperty("Account")]
        public ICollection<OU> OUs { get; set; }


        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Associations    = FilterEntityCollection(Associations, newInclusionPath);
            OUs             = FilterEntityCollection(OUs, newInclusionPath);

            Owner   =   FilterEntity(Owner,     newInclusionPath);
            RootOU  =   FilterEntity(RootOU,    newInclusionPath);

        }

    }
}
