using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Common
{
    /// <summary>
    /// Sequential GUIDs offer improved performance over plain GUIDs when used in an indexed column in the database, as the index is filled more efficiently
    /// </summary>
    public class SequentialGuidFactory
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern int UuidCreateSequential(out Guid value);

        private enum RpcUuidCodes : int
        {
            RPC_S_OK = 0,
            RPC_S_UUID_LOCAL_ONLY = 1824,
            RPC_S_UUID_NO_ADDRESS = 1739
        }

        /// <summary>
        /// Creates a new sequential Guid
        /// </summary>
        /// <param name="sqlCompatible">if true, then the some bytes of the sequential guid are shuffled and the result has the MSSQL's NEWSEQUENTIALID result format</param>
        /// <returns></returns>
        public static Guid CreateSequentialGuid(bool sqlCompatible = true)
        {
            Guid sequentialGuid;
            int resultCode = UuidCreateSequential(out sequentialGuid);

            switch (resultCode)
            {
                case (int)RpcUuidCodes.RPC_S_OK:
                    break;
                case (int)RpcUuidCodes.RPC_S_UUID_LOCAL_ONLY:
                    throw new Exception(@"UuidCreateSequential returned RPC_S_UUID_LOCAL_ONLY");
                case (int)RpcUuidCodes.RPC_S_UUID_NO_ADDRESS:
                    throw new Exception(@"UuidCreateSequential returned RPC_S_UUID_NO_ADDRESS");
                default:
                    throw new Exception(String.Format(@"UuidCreateSequential returned {0}", resultCode));
            }

            if (sqlCompatible)
            {
                byte[] guidBytes = sequentialGuid.ToByteArray();
                Array.Reverse(guidBytes, 0, 4);
                Array.Reverse(guidBytes, 4, 2);
                Array.Reverse(guidBytes, 6, 2);
                return new Guid(guidBytes);
            }
            else
            {
                return sequentialGuid;
            }
        }
    }
}