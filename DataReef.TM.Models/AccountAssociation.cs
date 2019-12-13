using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ComponentModel;
using Newtonsoft.Json;

namespace DataReef.TM.Models
{
    /// <summary>
    /// Associates a person with an account.  People are global to the TM System, a Person can be in multiple accounts.  This is the many to many object
    /// </summary>
   public  class AccountAssociation:EntityBase
    {
       /// <summary>
       /// Guid of the Person
       /// </summary>
       [DataMember]
       public Guid PersonID { get; set; }

       /// <summary>
       /// Guid of the Account
       /// </summary>
       [DataMember]
       public Guid AccountID { get; set; }


#region Navigation

       [ForeignKey("AccountID")]
       [JsonIgnore]
       [DataMember]
       public Account Account { get; set; }

       [ForeignKey("PersonID")]
       [JsonIgnore]
       [DataMember]
       public Person Person { get; set; }

#endregion

       public override void FilterCollections<T>(string inclusionPath = "")
       {
           bool alreadyProcessed;
           string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
           if (alreadyProcessed)
           {
               return;
           }

           Account  =   FilterEntity(Account,   newInclusionPath);
           Person   =   FilterEntity(Person,    newInclusionPath);

       }

    }
}
