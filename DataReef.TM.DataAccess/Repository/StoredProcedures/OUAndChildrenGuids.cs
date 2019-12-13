using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using DataReef.Core.Infrastructure.Repository;

namespace DataReef.TM.DataAccess.Repository.StoredProcedures
{
    public class OUAndChildrenGuids : StoredProcedure
    {
        private readonly Guid _ouid;
        private string _sqlCommand = string.Empty;

        public OUAndChildrenGuids(Guid ouid)
        {
            _ouid = ouid;
        }

        public override string Name => "proc_OUAndChildrenGuids";

        public override string SqlCommand
        {
            get
            {
                if (!string.IsNullOrEmpty(_sqlCommand))
                    return _sqlCommand;

                var sqlStringBuilder = new StringBuilder();
                sqlStringBuilder.Append("exec ");
                sqlStringBuilder.Append(Name);
                var parameters = GetParams();
                var lastParameterKey = parameters.Last().Key;
                foreach (var param in parameters)
                {
                    sqlStringBuilder.Append(" @");
                    sqlStringBuilder.Append(param.Key);
                    if (param.Key != lastParameterKey)
                    {
                        sqlStringBuilder.Append(",");
                    }
                }
                _sqlCommand = sqlStringBuilder.ToString();
                return _sqlCommand;
            }
        }

        public override Type ReturnType => typeof(Guid);

        public override Dictionary<string, string> GetParams()
        {
            return new Dictionary<string, string> { { "OUID", _ouid.ToString() } };
        }

        public override void Read(DbRawSqlQuery result)
        {
            Result = result.OfType<Guid>().ToList();
        }

        public IEnumerable<Guid> Result { get; private set; }
    }
}
