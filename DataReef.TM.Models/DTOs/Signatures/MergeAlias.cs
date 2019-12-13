
namespace DataReef.TM.Models.DTOs.Signatures
{
    public class MergeAliasAttribute : System.Attribute
    {

        public MergeAliasAttribute(string alias)
        {
            this.Alias = alias;
        }

        public string Alias { get; set; }
    }
}
