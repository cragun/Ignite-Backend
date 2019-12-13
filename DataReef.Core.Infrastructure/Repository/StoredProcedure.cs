using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace DataReef.Core.Infrastructure.Repository
{
    public abstract class StoredProcedure
    {
        public abstract string Name { get; }

        public abstract string SqlCommand { get; }

        public abstract Type ReturnType { get; }

        public abstract Dictionary<string, string> GetParams();

        public virtual SqlParameter[] SqlParams
        {
            get
            {
                return GetParams().Select(param => new SqlParameter(param.Key, param.Value)).ToArray();
            }
        }

        public virtual int? CommandTimeout => null;

        public virtual void Read(DbRawSqlQuery result)
        {
        }

        public virtual void Read(int result)
        {
        }
    }
}
