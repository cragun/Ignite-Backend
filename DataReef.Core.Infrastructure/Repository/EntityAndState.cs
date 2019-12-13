using DataReef.Core.Enums;

namespace DataReef.Core.Infrastructure.Repository
{
    public sealed class EntityAndState
    {
        public object Entity { get; set; }

        public DataAction EntityState { get; set; }
    }
}
