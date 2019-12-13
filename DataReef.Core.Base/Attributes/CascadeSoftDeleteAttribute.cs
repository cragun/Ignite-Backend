namespace DataReef.Core.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class,AllowMultiple = true)]  // multiuse attribute
    public class CascadeSoftDeleteAttribute:System.Attribute
    {
      
        public CascadeSoftDeleteAttribute(string primaryKeyTableName,string foreignKeyTableName,string foreignKeyFieldName)
        {
            this.PrimaryKeyTableName = primaryKeyTableName;
            this.ForeignKeyFieldName = foreignKeyFieldName;
            this.ForeignKeyTableName = foreignKeyTableName;
        }

        public string PrimaryKeyTableName { get; set; }
        public string ForeignKeyTableName { get; set; }
        public string ForeignKeyFieldName { get; set; }
       
    }
}
