using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf.Meta;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DataReef.CodeGenerator
{
    public  class ServiceGenerator
    {
        static int fieldNumber = 1000;

        public static void WriteServices(Type t, string path)
        {

            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

            string completePath = System.IO.Path.Combine(path, string.Format("{0}Service.cs", t.Name));


            System.IO.StreamWriter file = new System.IO.StreamWriter(completePath);

            TextReader tr = new StreamReader(@"ServiceTemplate.txt");
            string text = tr.ReadToEnd();

            text = text.Replace("[ClassName]", t.Name);

            file.WriteLine(text);

            file.Close();

        }

        public static void WriteControllers(Type t, string path)
        {

            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

            string completePath = System.IO.Path.Combine(path, string.Format("{0}sController.cs", t.Name));


            System.IO.StreamWriter file = new System.IO.StreamWriter(completePath);

            TextReader tr = new StreamReader(@"ControllerTemplate.txt");
            string text = tr.ReadToEnd();

            text = text.Replace("[ClassName]", t.Name);

            file.WriteLine(text);

            file.Close();

        }

        private static string DotNetTypeToProtoType(Type t)
        {
            if (t == typeof(double))
            {
                return "double";
            }
            else if (t == typeof(float))
            {
                return "float";
            }
            else if (t == typeof(int))
            {
                return "int32";
            }
            else if (t == typeof(Int16))
            {
                return "int32";
            }
            else if (t == typeof(Int64))
            {
                return "int64";
            }
            else if (t == typeof(DateTime))
            {
                return "sint64";
            }
            else if (t == typeof(decimal))
            {
                return "float";
            }
            else if (t == typeof(Guid))
            {
                return "string";
            }
            else if (t == typeof(Boolean))
            {
                return "bool";
            }
            else if (t==typeof(System.DateTime))
            {
                return "int64";
            }
            else if (t == typeof(string))
            {
                return "string";
            }
             
            else
            {
                return t.Name;
            }
             
        }

        private static bool IsPrimitiveType(string name)
        {
            string[] primitives = new string[] { "double", "float", "int32", "int64", "sint64", "string", "float", "bool" };
            return primitives.Contains(name.ToLower());

        }
       
        private static void AddBaseType(Type t)
        {
            Type bt = t.BaseType;
            RuntimeTypeModel.Default[bt].AddSubType(fieldNumber++, t);

            if (t.BaseType.BaseType != null)
            {
                AddBaseType(t.BaseType);
            }

        }

        private static bool IsCollection(PropertyInfo pi)
        {
            bool ret = false;

            Type tColl = typeof(ICollection<>);

            Type t = pi.PropertyType;
            if (t.IsGenericType && tColl.IsAssignableFrom(t.GetGenericTypeDefinition()) || 
                t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tColl))
            {
                ret = true;
            }
           
            return ret;

        }
       

        public static StringBuilder ImportStringBuilder(Type t)
        {

            StringBuilder sb = new StringBuilder();

            PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<PropertyInfo> enums = properties.Where(pp => typeof(Enum).IsAssignableFrom(pp.PropertyType)).ToList();

            foreach (PropertyInfo pi in properties)
            {
                if (enums.Contains(pi)) continue;

                if (pi.GetCustomAttribute<DataMemberAttribute>(true) == null) continue;
                if (pi.GetCustomAttribute<JsonIgnoreAttribute>(true) != null) continue;
              
                string name = pi.Name;
                bool isCollection = IsCollection(pi);
                string type = DotNetTypeToProtoType(pi.PropertyType);

                if (isCollection)
                {
                    type = DotNetTypeToProtoType(pi.PropertyType.GetGenericArguments()[0]);
                }
                else if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = DotNetTypeToProtoType(pi.PropertyType.GetGenericArguments()[0]);
                }

                if (!IsPrimitiveType(type))
                {
                    sb.AppendLine(string.Format("import \"{0}.proto\";", type));
                }
            }

            return sb;

        }

        public static StringBuilder EnumStringBuilder(Type t)
        {

            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("enum " + t.Name + "{");
            sb.AppendLine("option allow_alias = true;");

            List<string> names = Enum.GetNames(t).ToList();

            int x = 0;

            foreach (string name in names)
            {
                sb.AppendLine(string.Format("{0} = {1};", name.ToUpper(), x.ToString()));
                x++;
            }

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine();
            return sb;

        }

        public static void WriteProtoBuf(Entity e,Type t, string path)
        {

            if (e == null) return;


            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            string completePath = System.IO.Path.Combine(path, string.Format("{0}.proto", t.Name));
            System.IO.StreamWriter file = new System.IO.StreamWriter(completePath);
            System.Text.StringBuilder sb = new StringBuilder();

            sb.AppendLine(ImportStringBuilder(t).ToString()); 

            PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<PropertyInfo> enums = properties.Where(pp => typeof(Enum).IsAssignableFrom(pp.PropertyType)).ToList();

            foreach (PropertyInfo pi in enums)
            {
                StringBuilder enumsb = EnumStringBuilder(pi.PropertyType);
                sb.Append(enumsb.ToString());
            }

            sb.AppendLine("message " + e.Name + "{");

            int x=1;
            foreach (PropertyInfo pi in properties)
            {
                if (pi.GetCustomAttribute<DataMemberAttribute>(true) == null) continue;
                if (pi.GetCustomAttribute<JsonIgnoreAttribute>(true) != null) continue;

                string name = pi.Name;
                bool isCollection = IsCollection(pi);
                string type = DotNetTypeToProtoType(pi.PropertyType);

                if (isCollection)
                {
                    type = DotNetTypeToProtoType(pi.PropertyType.GetGenericArguments()[0]);
                }
                else if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = DotNetTypeToProtoType(pi.PropertyType.GetGenericArguments()[0]);
                }
                

                sb.AppendLine(string.Format("{0} {1} {2} = {3};",
                    isCollection ? "repeated" : "optional",
                    type,
                    name,
                    x.ToString())
                    );
                x++;
            }
            sb.AppendLine("}");

            file.WriteLine(sb.ToString());
            file.Close();

        }

        public static void WriteServiceInterfaces(Type t,string path)
        {

            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

            string completePath = System.IO.Path.Combine(path, string.Format("I{0}Service.cs", t.Name));


            System.IO.StreamWriter file = new System.IO.StreamWriter(completePath);

            TextReader tr = new StreamReader(@"ServiceInterfaceTemplate.txt");
            string text = tr.ReadToEnd();

            text = text.Replace("[ClassName]", t.Name);

            file.WriteLine(text);

            file.Close();

        }

    }
}
