using System.Collections.Generic;

namespace DataReef.CodeGenerator
{
    public class Entity
    {
        private List<string> keys = new List<string>();
        private List<Property> properties = new List<Property>();
        private List<Relationship> relationships = new List<Relationship>();
        private List<string> indexes = new List<string>();

        public List<string> Keys
        {
            get { return this.keys; }
            set { this.keys = value; }
        }

        public List<string> Indexes
        {
            get { return this.indexes; }
            set { this.indexes = value; }
        }

        public List<Relationship> Relationships
        {
            get { return this.relationships; }
            set { this.relationships = value; }
        }

        public List<Property> Properties
        {
            get { return this.properties; }
            set { this.properties = value; }
        }

        public string FirstPrimaryKey()
        {
            if (this.keys.Count > 0)
            {
                return this.keys[0];
            }
            else
            {
                return null;
            }
        }

        public string Name { get; set; }
        public string NamespacePrefix { get; set; }
        public string ParentEntityName { get; set; }
        public string ConatainerName { get; set; }
        public string NameSpaceAndEntityName { get { return this.NamespacePrefix + this.Name; } }
        public bool IsExcluded { get; set; }
    }
}
