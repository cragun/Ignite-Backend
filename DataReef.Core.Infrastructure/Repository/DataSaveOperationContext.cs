using DataReef.Core.Enums;

namespace DataReef.Core.Infrastructure.Repository
{
    public sealed class DataSaveOperationContext
    {
        public DataAction DataAction { get; set; }


        public static DataSaveOperationContext Insert
        {
            get { return new DataSaveOperationContext { DataAction = DataAction.Insert }; }
        }

        public static DataSaveOperationContext Update
        {
            get { return new DataSaveOperationContext { DataAction = DataAction.Update }; }
        }

        public static DataSaveOperationContext Delete
        {
            get { return new DataSaveOperationContext { DataAction = DataAction.Delete }; }
        }

    }
}
