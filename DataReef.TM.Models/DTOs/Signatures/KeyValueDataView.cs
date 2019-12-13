using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class KeyValueDataView
    {
        public KeyValueDataView()
        {

        }
        public KeyValueDataView(KeyValue kv)
        {
            if (kv == null) return;
            kv.Key = this.Key;
            kv.Value = this.Value;
            kv.Notes = this.Notes;
            kv.ObjectID = this.ObjectID;

        }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Notes { get; set; }

        public Guid ObjectID { get; set; }

        public System.DateTime DateCreated { get; set; }

        public static KeyValueDataView FromDbModel(KeyValue kv)
        {
            KeyValueDataView ret = new Signatures.KeyValueDataView();

            ret.Key = kv.Key;
            ret.Value = kv.Value;
            ret.Notes = kv.Notes;
            ret.DateCreated = kv.DateCreated;
            ret.ObjectID = kv.ObjectID;
                
            return ret;
        }


    }
}
