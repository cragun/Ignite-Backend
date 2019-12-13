using System;

namespace DataReef.Core.Infrastructure.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(Guid guid, Type type)
        {
            Guid = guid;
            Type = type;
        }

        public Guid Guid { get; set; }
        public Type Type { get; set; }

        public override string ToString()
        {
            return Type.Name + " with Guid " + Guid + " was not found.";
        }
    }
}
