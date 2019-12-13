namespace DataReef.CodeGenerator
{
    public class ClientDefaultValueAttribute:System.Attribute
    {
        public ClientDefaultValueAttribute(string val)
        {
            this.DefaultValue = val;
        }

        public string DefaultValue { get; set; }
    }
}
