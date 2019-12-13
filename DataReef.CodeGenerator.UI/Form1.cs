using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using DataReef.CodeGenerator;

namespace CodeGenerator
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> aliasedEntities = new Dictionary<string, string>();
        private List<Entity> entities = new List<Entity>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private Entity EntityWithName(string entityName)
        {
            Entity ret = null;
            ret = entities.Where(ee => ee.Name == entityName).FirstOrDefault();
            return ret;
        }

        public bool IsICollection(Type t)
        {
            if (t == typeof(byte[])) return false;

            bool isICollection = t.GetInterfaces()
                            .Any(x => x.IsGenericType &&
                            x.GetGenericTypeDefinition() == typeof(ICollection<>));

            if (!isICollection)
            {
                isICollection = t.GetInterfaces()
                            .Any(x => x.IsGenericType &&
                            x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            }

            return isICollection;
        }
        public string DefaultIOSValueForType(System.Type type)
        {
            string ret = string.Empty;

            if (type == typeof(System.Int16))
            {
                ret = "0";
            }
            else if (type == typeof(System.Int32))
            {
                ret = "0";
            }
            else if (type == typeof(System.Int64))
            {
                ret = "0";
            }
            else if (type == typeof(float))
            {
                ret = "0";
            }
            else if (type == typeof(double))
            {
                ret = "0";
            }
            else if (type == typeof(bool))
            {
                ret = "NO";
            }
            else if (type == typeof(decimal))
            {
                ret = "0";
            }

            else if (type.IsEnum)
            {
                ret = "0";
            }

            return ret;
        }
        private void LoadEntities()
        {
            foreach (TreeNode n in this.treeView1.Nodes)
            {
                if (n.Checked == false) continue;

                Type t = (Type)n.Tag;

                List<ReplacementTypeAttribute> aliases = t.GetCustomAttributes<ReplacementTypeAttribute>(false).ToList();
                if (aliases.Any())
                {
                    foreach (ReplacementTypeAttribute att in aliases)
                    {
                        this.aliasedEntities.Add(att.Type.Name, t.Name);
                    }
                }

                if (t.GetCustomAttribute<NotIncludedInClientAttribute>(false) != null)
                {
                    continue;
                }


                if (t.GetCustomAttribute<ClientClass>(true) != null)
                {
                    InheritedTypeAttribute inheritAttribute = t.GetCustomAttribute<InheritedTypeAttribute>();
                    bool shouldInherit = inheritAttribute != null;

                    Entity entity = new Entity();
                    entity.Name = t.Name;
                    entity.NamespacePrefix = this.txtPrefix.Text;
                    if (shouldInherit) entity.ParentEntityName = t.BaseType.Name; else entity.ParentEntityName = this.txtParentEntity.Text;
                    this.entities.Add(entity);

                    System.Reflection.PropertyInfo[] properties;

                    if (shouldInherit)
                    {
                        properties = t.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                    }
                    else
                    {
                        properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    }

                    foreach (PropertyInfo pi in properties)
                    {

                        if (pi.GetCustomAttribute<NotIncludedInClientAttribute>(false) != null)
                        {
                            continue;
                        }



                        bool isProperty = false;

                        if (pi.PropertyType == typeof(string))
                        {
                            isProperty = true;
                        }
                        else
                        {
                            isProperty = pi.PropertyType.IsValueType && pi.PropertyType.IsPublic;
                        }

                        if (isProperty)
                        {
                            Property prop = new Property();
                            prop.Name = pi.Name;
                            prop.Type = pi.PropertyType;
                            prop.DefaultValue = this.DefaultIOSValueForType(pi.PropertyType);
                            prop.IsDeclared = pi.DeclaringType == t;

                            bool canBeNull = !pi.PropertyType.IsValueType || (Nullable.GetUnderlyingType(pi.PropertyType) != null);
                            if (!canBeNull)
                            {
                                canBeNull = pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
                            }
                            prop.IsNullable = canBeNull;

                            StringLengthAttribute sla = pi.GetCustomAttribute<StringLengthAttribute>();
                            if (sla != null)
                            {
                                prop.MinLength = sla.MinimumLength;
                                prop.MaxLength = sla.MaximumLength;
                            }

                            //IndexedAttribute iat = pi.GetCustomAttribute<IndexedAttribute>();
                            //if(iat!=null)
                            //{
                            //    if(pi.DeclaringType.Equals(t))entity.Indexes.Add(prop.Name); 
                            //}

                            KeyAttribute kat = pi.GetCustomAttribute<KeyAttribute>();
                            if (kat != null)
                            {
                                if (pi.DeclaringType.Equals(t)) entity.Indexes.Add(prop.Name);
                            }

                            entity.Properties.Add(prop);
                        }
                        else
                        {
                            // relationship
                        }
                    }


                }
            }

            // relationships
            foreach (TreeNode n in this.treeView1.Nodes)
            {
                if (n.Checked == false) continue;
                Type t = (Type)n.Tag;

                if (t.GetCustomAttribute<ClientClass>()!=null)
                {
                    Entity entity = this.EntityWithName(t.Name);

                    if (entity == null) continue;

                    bool shouldInherit = t.GetCustomAttribute<InheritedTypeAttribute>() != null;

                    System.Reflection.PropertyInfo[] properties;

                    if (shouldInherit)
                    {
                        properties = t.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                    }
                    else
                    {
                        properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    }
                    
                    foreach (PropertyInfo pi in properties)
                    {
                        bool isProperty = false;

                        if (pi.PropertyType == typeof(string))
                        {
                            isProperty = true;
                        }
                        else if (pi.PropertyType == typeof(byte[]))
                        {
                            isProperty = true; 
                        }
                        else
                        {
                            isProperty = pi.PropertyType.IsValueType && pi.PropertyType.IsPublic;
                        }

                        if (!isProperty)
                        {
                            if(this.IsICollection(pi.PropertyType))
                            {
                                string typeName = pi.PropertyType.GetGenericArguments()[0].Name;
                                Entity de = this.EntityWithName(typeName);

                                if(de!=null)
                                {
                                    Relationship rel = new Relationship();
                                    rel.IsOptional = true;
                                    rel.Multiplicity = Multiplicity.Many;
                                    rel.Name = pi.Name;
                                    rel.DestinationEntity = de.Name;

                                    string name = string.Empty;

                                    if(!this.EntityHasRelationshipWith(pi.PropertyType.GetGenericArguments()[0],t,ref name))
                                    {
                                        Entity fe = this.EntityWithName(pi.PropertyType.GetGenericArguments()[0].Name);

                                        if (fe != null)
                                        {
                                            Relationship vrel = new Relationship();
                                            vrel.Name = entity.Name.ToCocoaCase();
                                            vrel.IsVirtual = true;
                                            vrel.Multiplicity = Multiplicity.One;
                                            vrel.IsOptional = true;
                                            vrel.DestinationEntity = entity.Name;
                                            vrel.InverseEntity = entity.Name;
                                            vrel.InverseName = rel.Name.ToCocoaCase();
                                            fe.Relationships.Add(vrel); 

                                            rel.InverseEntity = de.Name;
                                            rel.InverseName = entity.Name.ToCocoaCase(); ;
                                        }
                                        else
                                        {
                                            //shouldnt go here
                                            System.Diagnostics.Debugger.Break();
                                        }
                                    }
                                    else
                                    {
                                        rel.InverseEntity = de.Name;
                                        rel.InverseName = name.ToCocoaCase();
                                    }
                                    entity.Relationships.Add(rel);
                                }
                            }
                            else
                            {
                                Relationship rel = new Relationship();
                                Entity de = this.EntityWithName(pi.PropertyType.Name);

                                if(de!=null)
                                {
                                    rel.IsOptional = true;
                                    rel.Multiplicity = Multiplicity.One;
                                    rel.Name = pi.Name;
                                    if (de != null) rel.DestinationEntity = de.Name;

                                    string name = string.Empty;

                                    if (!this.EntityHasRelationshipWith(pi.PropertyType, t, ref name))
                                    {
                                        Entity fe = this.EntityWithName(pi.PropertyType.Name);

                                        if (fe != null)
                                        {
                                            Relationship vrel = new Relationship();
                                            vrel.Name = entity.Name.ToCocoaCase();
                                            vrel.IsVirtual = true;
                                            vrel.Multiplicity = Multiplicity.One;
                                            vrel.IsOptional = true;
                                            vrel.DestinationEntity = entity.Name;
                                            vrel.InverseEntity = entity.Name;
                                            vrel.InverseName = rel.Name.ToCocoaCase();
                                            fe.Relationships.Add(vrel);

                                            rel.InverseEntity = de.Name;
                                            rel.InverseName = entity.Name.ToCocoaCase(); ;
                                        }
                                        else
                                        {
                                            //shouldnt go here
                                            System.Diagnostics.Debugger.Break();
                                        }
                                    }
                                    else
                                    {
                                        rel.InverseEntity = de.Name;
                                        rel.InverseName = name.ToCocoaCase();
                                    }
                                    entity.Relationships.Add(rel);
                                }
                            }
                           
                           
                        }
                        
                    }
                }
            }


        }

        private bool EntityHasRelationshipWith(Type entity, Type withEntity, ref string name)
        {
            bool ret = false;

            PropertyInfo[] props = entity.GetProperties();

            foreach(PropertyInfo pi in props)
            {
                if (pi.PropertyType == withEntity)
                {
                    name = pi.Name;
                    ret = true;
                    break;
                }
                else if (this.IsICollection(pi.PropertyType) &&pi.PropertyType.IsGenericType)
                {
                   if(pi.PropertyType.GetGenericArguments()[0]==withEntity)
                   {
                       name = pi.Name;
                       ret = true;
                       break;
                   }
                }

            }


            return ret;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();

            string fileName = dialog.FileName;

            System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFrom(fileName);
            Type[] types = ass.GetTypes();

            foreach(Type t in types)
            {

                if (t.IsEnum) continue;

                if(t.Namespace.StartsWith("DataReef.TM.Models"))
                {
                    TreeNode node = new TreeNode(t.Name);
                    node.Checked = true;
                    node.Tag = t;
                    this.treeView1.Nodes.Add(node);
                }
                else if (t.BaseType!=null &&  t.IsEnum==false && t.BaseType.Name!="Attribute")
                {
                   /// System.Diagnostics.Debugger.Break();
                }
            }
        }




        public string ToCoreDataModel()
        {
            System.Xml.XmlDocument ret = new XmlDocument();

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(ms);

            writer.WriteStartDocument();
            //<model name="" userDefinedModelVersionIdentifier="" type="com.apple.IDECoreDataModeler.DataModel" documentVersion="1.0" lastSavedToolsVersion="2061" systemVersion="12A256" minimumToolsVersion="Automatic" macOSVersion="Automatic" iOSVersion="Automatic">
            writer.WriteStartElement("model");
            writer.WriteAttributeString("name", "");
            writer.WriteAttributeString("userDefinedModelVersionIdentifier", "");
            writer.WriteAttributeString("type", "com.apple.IDECoreDataModeler.DataModel");
            writer.WriteAttributeString("documentVersion", "1.0");
            writer.WriteAttributeString("lastSavedToolsVersion", "2061");
            writer.WriteAttributeString("systemVersion", "12A256");
            writer.WriteAttributeString("minimumToolsVersion", "Automatic");
            writer.WriteAttributeString("macOSVersion", "Automatic");
            writer.WriteAttributeString("iOSVersion", "Automatic");



            //<entity name="Binder" representedClassName="Binder" parentEntity="Container" syncable="YES">
            //<attribute name="dateLastAccessed" optional="YES" attributeType="Date" syncable="YES"/>
            //<relationship name="pages" optional="YES" toMany="YES" deletionRule="Cascade" destinationEntity="BinderPage" inverseName="binder" inverseEntity="BinderPage" syncable="YES"/>
            //</entity>

            //  ("<entity name=\"DRClientBase\" representedClassName=\"DRClientBase\" syncable=\"YES\">
            //<relationship name=\"saveResult\" optional=\"YES\" transient=\"YES\" maxCount=\"1\" deletionRule=\"Nullify\" 
            //destinationEntity=\"DRSaveResult\" inverseName=\"clientBase\" inverseEntity=\"DRSaveResult\" syncable=\"YES\"/> </entity>");

            writer.WriteStartElement("entity");
            writer.WriteAttributeString("name", "SCClientBase");
            writer.WriteAttributeString("representedClassName", "SCClientBase");
            writer.WriteAttributeString("syncable", "YES");

            writer.WriteStartElement("relationship");
            writer.WriteAttributeString("name", "saveResult");
            writer.WriteAttributeString("optional", "YES");
            writer.WriteAttributeString("transient", "NO");
            writer.WriteAttributeString("maxCount", "1");
            writer.WriteAttributeString("deletionRule", "Nullify");
            writer.WriteAttributeString("destinationEntity", "SCSaveResult");
            writer.WriteAttributeString("inverseName", "clientBase");
            writer.WriteAttributeString("inverseEntity", this.txtPrefix.Text + "SaveResult");
            writer.WriteAttributeString("syncable", "YES");
            writer.WriteEndElement();

            writer.WriteEndElement(); //DRClientBase


            writer.WriteStartElement("entity");
            writer.WriteAttributeString("name", this.txtPrefix.Text + "SaveResult");
            writer.WriteAttributeString("representedClassName", this.txtPrefix.Text + "SaveResult");
            writer.WriteAttributeString("syncable", "YES");

            writer.WriteStartElement("attribute");
            writer.WriteAttributeString("name", "action");
            writer.WriteAttributeString("optional", "YES");
            writer.WriteAttributeString("attributeType", "Integer 16");
            writer.WriteAttributeString("defaultValueString", "0");
            writer.WriteAttributeString("syncable", "YES");
            writer.WriteEndElement();

            writer.WriteStartElement("attribute");
            writer.WriteAttributeString("name", "exception");
            writer.WriteAttributeString("optional", "YES");
            writer.WriteAttributeString("attributeType", "String");
            writer.WriteAttributeString("syncable", "YES");
            writer.WriteEndElement();

            writer.WriteStartElement("attribute");
            writer.WriteAttributeString("name", "exceptionMessage");
            writer.WriteAttributeString("optional", "YES");
            writer.WriteAttributeString("attributeType", "String");
            writer.WriteAttributeString("syncable", "YES");
            writer.WriteEndElement();

            writer.WriteStartElement("attribute");
            writer.WriteAttributeString("name", "success");
            writer.WriteAttributeString("optional", "YES");
            writer.WriteAttributeString("attributeType", "Boolean");
            writer.WriteAttributeString("syncable", "YES");
            writer.WriteEndElement();

            writer.WriteStartElement("relationship");
            writer.WriteAttributeString("name", "clientBase");
            writer.WriteAttributeString("optional", "YES");
            writer.WriteAttributeString("maxCount", "1");
            writer.WriteAttributeString("deletionRule", "Nullify");
            writer.WriteAttributeString("destinationEntity", this.txtPrefix.Text + "ClientBase");
            writer.WriteAttributeString("inverseName", "saveResult");
            writer.WriteAttributeString("inverseEntity", this.txtPrefix.Text + "ClientBase");
            writer.WriteAttributeString("syncable", "YES");
            writer.WriteEndElement();
            writer.WriteEndElement();

            //            <entity name="DRSaveResult" representedClassName="DRSaveResult" syncable="YES">
            //    <attribute name="action" optional="YES" attributeType="Integer 16" defaultValueString="0" syncable="YES"/>
            //    <attribute name="exception" optional="YES" attributeType="String" syncable="YES"/>
            //    <attribute name="exceptionMessage" optional="YES" attributeType="String" syncable="YES"/>
            //    <attribute name="success" optional="YES" attributeType="Boolean" syncable="YES"/>
            //    <relationship name="clientBase" optional="YES" maxCount="1" deletionRule="Nullify" destinationEntity="DRClientBase" inverseName="saveResult" inverseEntity="DRClientBase" syncable="YES"/>
            //</entity>






            foreach (Entity e in this.entities)
            {


                if (e.IsExcluded) continue;

                writer.WriteStartElement("entity");
                writer.WriteAttributeString("name", e.NamespacePrefix + e.Name);
                writer.WriteAttributeString("representedClassName", e.NamespacePrefix + e.Name);
                if (!string.IsNullOrWhiteSpace(e.ParentEntityName)) writer.WriteAttributeString("parentEntity", e.NamespacePrefix + e.ParentEntityName);

                writer.WriteAttributeString("syncable", "YES");

                foreach (Property p in e.Properties)
                {
                    // if (p.IsDeclared || (!p.IsDeclared && string.IsNullOrWhiteSpace(e.ParentEntityName)))
                    // {
                    writer.WriteStartElement("attribute");
                    writer.WriteAttributeString("name", p.ToCoreDataName().ToCocoaCase());
                    writer.WriteAttributeString("optional", p.IsNullable.HasValue && p.IsNullable == true ? "YES" : "NO");
                    writer.WriteAttributeString("attributeType", this.DotNetTypeToObjectiveCType(p.Type));
                    if (!string.IsNullOrEmpty(p.DefaultValue)) writer.WriteAttributeString("defaultValueString", p.DefaultValue);
                    writer.WriteAttributeString("syncable", "YES");
                    if (p.MinLength.HasValue) writer.WriteAttributeString("minValueString", p.MinLength.Value.ToString());
                    if (p.MaxLength.HasValue) writer.WriteAttributeString("maxValueString", p.MaxLength.Value.ToString());
                    writer.WriteEndElement(); //property
                    //}
                }

                foreach (Relationship r in e.Relationships)
                {
                    writer.WriteStartElement("relationship");
                    writer.WriteAttributeString("name", r.Name.ToCocoaCase());
                    writer.WriteAttributeString("optional", r.IsOptional ? "YES" : "NO");
                    if (r.MinCount.HasValue) writer.WriteAttributeString("minCount", r.MinCount.ToString());
                    if (r.MaxCount.HasValue) writer.WriteAttributeString("maxCount", r.MaxCount.ToString());
                    writer.WriteAttributeString("deletionRule", "Nullify");
                    writer.WriteAttributeString("destinationEntity", e.NamespacePrefix + r.DestinationEntity);
                    if (!string.IsNullOrWhiteSpace(r.InverseName)) writer.WriteAttributeString("inverseName", r.InverseName);
                    if (!string.IsNullOrWhiteSpace(r.InverseEntity)) writer.WriteAttributeString("inverseEntity", e.NamespacePrefix + r.InverseEntity);
                    writer.WriteAttributeString("syncable", "YES");

                    if (r.Multiplicity == Multiplicity.Many)
                    {
                        writer.WriteAttributeString("toMany", "YES");
                    }
                    else
                    {
                        writer.WriteAttributeString("maxCount", "1");
                    }
                    writer.WriteEndElement();
                }

                //write indexes if any
                if (e.Indexes.Count > 0)
                {
                    writer.WriteStartElement("compoundIndexes");

                    foreach (string prop in e.Indexes)
                    {
                        writer.WriteStartElement("compoundIndex");
                        writer.WriteStartElement("index");
                        writer.WriteAttributeString("value", prop.ToCocoaCase());
                        writer.WriteEndElement(); //index                  
                        writer.WriteEndElement(); //compound index                  
                    }
                    writer.WriteEndElement(); //compoundIndexes
                }
                writer.WriteEndElement(); //entity
            }

            writer.WriteEndElement(); //model
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();
            ms.Position = 0;

            string xml = System.Text.Encoding.UTF8.GetString(ms.ToArray()).Trim();
            return xml;


        } //function

        public string DotNetTypeToObjectiveCType(System.Type type)
        {
            string ret = string.Empty;

            if (type == typeof(string))
            {
                ret = "String";
            }
            else if (type == typeof(System.DateTime))
            {
                ret = "Date";
            }
            else if (type == typeof(System.Int16))
            {
                ret = "Integer 16";
            }
            else if (type == typeof(System.Int32))
            {
                ret = "Integer 32";
            }
            else if (type == typeof(System.Int64))
            {
                ret = "Integer 64";
            }
            else if (type == typeof(float))
            {
                ret = "Double";
            }
            else if (type == typeof(double))
            {
                ret = "Double";
            }
            else if (type == typeof(bool))
            {
                ret = "Boolean";
            }
            else if (type == typeof(decimal))
            {
                ret = "Decimal";
            }
            else if (type == typeof(System.Byte[]))
            {
                ret = "Binary";
            }
            else if (type==typeof(Guid))
            {
                ret = "String";

            }
            else if (type.IsEnum)
            {
                ret = "Integer 16";
            }
            else
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type gt = type.GetGenericArguments()[0];
                    ret = this.DotNetTypeToObjectiveCType(gt);
                }
                else
                {
                    ret = "UNKNOWN";
                    if (type != null) System.Diagnostics.Debug.WriteLine("Type Not Defined: " + type.ToString());
                }
            }
            return ret;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.entities = new List<Entity>();
            this.aliasedEntities = new Dictionary<string, string>();
            this.LoadEntities();

            string rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string childFolder = "GeneratedCode";
            string folder = System.IO.Path.Combine(rootFolder, childFolder);

            if (System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.Delete(folder);
            }

            System.IO.Directory.CreateDirectory(folder);

            string xml = this.ToCoreDataModel();
            this.richTextBox1.Text = xml;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

            string rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string fileName = "RestKitInitializer.h";
            if (System.IO.File.Exists(System.IO.Path.Combine(rootFolder,fileName))) System.IO.File.Delete(fileName);

            fileName = "RestKitInitializer.m";
            if (System.IO.File.Exists(System.IO.Path.Combine(rootFolder, fileName))) System.IO.File.Delete(fileName);


            DataReef.CodeGenerator.RestKitGenerator.WriteRestKitClasses(rootFolder, this.entities);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in this.treeView1.Nodes)
            {
                node.Checked = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in this.treeView1.Nodes)
            {
                node.Checked = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
           
            
            string rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string folder = System.IO.Path.Combine(rootFolder, "ProtoBuf");
            if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);


            foreach (TreeNode node in this.treeView1.Nodes)
            {

                Entity entity = this.EntityWithName(node.Text);
               DataReef.CodeGenerator.ServiceGenerator.WriteProtoBuf(entity,(Type)node.Tag, folder); 
            }
        
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string fileName = "CoreDataEnums.h";
            if (System.IO.File.Exists(System.IO.Path.Combine(rootFolder, fileName))) System.IO.File.Delete(fileName);

            DataReef.CodeGenerator.RestKitGenerator.WriteObjectCEnumerations(rootFolder, this.entities);
        }
    }

}
