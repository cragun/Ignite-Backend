using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews
{
    [DataContract]
    [NotMapped]
    public class ValueTypePair<T, V>
    {
        public ValueTypePair() { }

        public ValueTypePair(T t, V v)
        {
            Type = t;
            Value = v;
        }

        [DataMember]
        public T Type { get; set; }

        [DataMember]
        public V Value { get; set; }

    }
}
