using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataReef.TM.Models;


namespace DataReef.TM.Models
{
    /// <summary>
    /// Use PersonalConnections to Follow Someone.  The FromPersonID will be notified on the actions of the ToPersonID.  The ToPerson will be notified that the FromPerson is following him/her
    /// </summary>
   public  class PersonalConnection: EntityBase
   {

       #region Properties 

       [DataMember]
       public Guid FromPersonID { get; set; }

       [DataMember]
       public Guid ToPersonID { get; set; }

       [DataMember]
       [NotMapped]
       public bool DailyNotifications
       {
           get
           {
               return (this.Flags | 1) == 1;
           }

           set
           {
               long mask = 1 << 0;
               if(value==true)
               {
                   this.Flags |= mask;
               }
               else
               {
                   this.Flags &= ~mask;
               }
           }
       }

       [DataMember]
       [NotMapped]
       public bool LiveNotifications
       {
           get
           {
               return (this.Flags | 2) == 2;
           }

           set
           {
               long mask = 1 << 1;
               if (value == true)
               {
                   this.Flags |= mask;
               }
               else
               {
                   this.Flags &= ~mask;
               }
           }
       }


       /// <summary>
       /// Optional message sent to the with you FromPerson is connecting To
       /// </summary>
       [DataMember]
       [StringLength(100)]
       public string Message { get; set; }

       #endregion

       #region Navigation

       [ForeignKey("FromPersonID")]
       [DataMember]
       public Person FromPerson { get; set; }

       [ForeignKey("ToPersonID")]
       [DataMember]
       public Person ToPerson { get; set; }

       #endregion

       public override void FilterCollections<T>(string inclusionPath = "")
       {
           bool alreadyProcessed;
           string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
           if (alreadyProcessed)
           {
               return;
           }

           FromPerson   = FilterEntity(FromPerson,  newInclusionPath);
           ToPerson     = FilterEntity(ToPerson,    newInclusionPath);

       }

   }
}
