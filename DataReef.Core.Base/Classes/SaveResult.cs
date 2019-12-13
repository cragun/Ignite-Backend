using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DataReef.Core.Enums;

namespace DataReef.Core.Classes
{

    [DataContract]
    [NotMapped]
    public class SaveResult
    {

        public static SaveResult SuccessfulInsert
        {
            get
            {
                var ret = new SaveResult { Action = DataAction.Insert, Success = true };
                return ret;
            }
        }


        public static SaveResult SuccessfulUpdate
        {
            get
            {
                var ret = new SaveResult { Action = DataAction.Update, Success = true };
                return ret;
            }
        }


        public static SaveResult SuccessfulDeletion
        {
            get
            {
                var ret = new SaveResult { Action = DataAction.Delete, Success = true };
                return ret;
            }
        }

        public static SaveResult FromException(Exception ex, DataAction action)
        {
            var ret = new SaveResult
            {
                Success = false,
                Action = action,
                Exception = ex.GetType().Name,
                ExceptionMessage = ex.ToString()
            };
            return ret;
        }

        [DataMember(EmitDefaultValue = false)]
        public Guid? EntityUniqueId { get; set; }

        [DataMember]
        public DataAction Action { get; set; }

        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Exception { get; set; }

        [DataMember]
        public string ExceptionMessage { get; set; }

        [DataMember]
        public object Payload { get; set; }
    }
}
