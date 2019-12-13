using System.Linq;

namespace DataReef.CodeGenerator
{
    public static class StringExtensions
    {

        public static string Plural(this string input)
        {
            string ret=string.Empty;

            if (input.ToLower() == "person")
            {
                ret = string.Format("{0}eople", input.Substring(0, 1));
            }
            else if (input.Last().ToString().ToLower() == "s")
            {
                ret = input + "es";
            }
            else
            {
                ret = input + "s";
            }


            return ret;

        }
        public static string ToCocoaCase(this string input)
        {
            if (input == null) return null;

            string ret = input;

            if (input.Length <= 2)
            {
                ret = input.ToLower();
            }
            else if (input.ToLower()=="ouid"){
                ret = "ouID";
            }
            else if (input.Substring(0,2).ToLower()=="ou" && input.Length>4)
            {
                ret = "ou" + input.Substring(2, 1) + input.Substring(3, input.Length - (4-1));
            }
            else
            {
                ret = input.Substring(0, 1).ToLower() + input.Substring(1, input.Length - 1);
            }


            return ret;

        }

    }
}
