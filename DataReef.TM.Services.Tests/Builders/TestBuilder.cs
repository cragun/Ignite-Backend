namespace DataReef.TM.Services.Tests.Builders
{
    public abstract class TestBuilder<T> where T : new ()
    {
        public T Object { get; } = new T();

        public static implicit operator T(TestBuilder<T> builder)
        {
            return builder.Object;
        }
    }
}
