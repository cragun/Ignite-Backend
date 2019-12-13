using System;
using System.IO;

namespace DataReef.CodeGenerator
{
    public class ClientProxyGenerator
    {

        public static void WriteClientProxy(Type t, string path)
        {
           
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

            string completePath = System.IO.Path.Combine(path, string.Format("{0}Proxy.cs", t.Name));

            System.IO.StreamWriter file = new System.IO.StreamWriter(completePath);
            TextReader tr = new StreamReader(@"ClientProxyTemplate.txt");
            string text = tr.ReadToEnd();
            text = text.Replace("[ClassName]", t.Name);

            file.WriteLine(text);

            file.Close();

        }



    }
}
