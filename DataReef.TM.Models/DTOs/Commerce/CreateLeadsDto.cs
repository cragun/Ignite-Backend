using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Commerce
{
    public class CreateLeadsDto
    {
        private string _homeAge = null;
        private string _squareFootage = null;
        private string _lengthOfResidence = null;
        private string _estimatedIncome = null;

        [Description("c3253dad-95ab-4f0a-b27d-3b8902afb3b6")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Pipe delimited. e.g. min | max
        /// </summary>
        [Description("446924dc-bb35-4727-ab8d-42e091a21e0d")]
        public string HomeAge
        {
            get
            {
                try
                {
                    if (_homeAge == null) return null;

                    string[] parts = _homeAge.Split('|');

                    if (parts.Length == 1 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        _homeAge = _homeAge + "|200";
                    }
                    else if (parts.Length == 2 && string.IsNullOrWhiteSpace(parts[0]))
                    {
                        _homeAge = "0|" + _homeAge;
                    }

                    return _homeAge;
                }
                catch (Exception)
                {

                    return null;
                }

            }
            set
            {
                _homeAge = value;
            }
        }

        /// <summary>
        /// Pipe delimited. e.g. min | max
        /// </summary>
        [Description("42330b64-75ba-468e-8fcf-23cb40c8e256")]
        public string SquareFootage
        {
            get
            {
                try
                {
                    if (_squareFootage == null) return null;

                    string[] parts = _squareFootage.Split('|');

                    if (parts.Length == 1 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        _squareFootage = _squareFootage + "|50000";
                    }
                    else if (parts.Length == 2 && string.IsNullOrWhiteSpace(parts[0]))
                    {
                        _squareFootage = "1|" + _squareFootage;
                    }


                    return _squareFootage;
                }
                catch (Exception)
                {

                    return null;
                }

            }
            set
            {
                _squareFootage = value;
            }

        }

        /// <summary>
        /// Pipe delimited. e.g. min | max
        /// </summary>
        [Description("2f95294a-bbad-4f43-bc42-246d4afdcf94")]
        public string LengthOfResidence
        {
            get
            {
                try
                {
                    if (_lengthOfResidence == null) return null;

                    string[] parts = _lengthOfResidence.Split('|');

                    if (parts.Length == 1 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        _lengthOfResidence = _lengthOfResidence + "|100";
                    }
                    else if (parts.Length == 2 && string.IsNullOrWhiteSpace(parts[0]))
                    {
                        _lengthOfResidence = "1|" + _lengthOfResidence;
                    }


                    return _lengthOfResidence;
                }
                catch (Exception)
                {

                    return null;
                }

            }
            set
            {
                _lengthOfResidence = value;
            }
        }

        /// <summary>
        /// Pipe delimited. e.g. min | max
        /// </summary>
        [Description("cc641dca-877e-46ae-b5ce-063455b8d26a")]
        public string EstimatedIncome
        {
            get
            {
                try
                {
                    if (_estimatedIncome == null) return null;

                    string[] parts = _estimatedIncome.Split('|');

                    if (parts.Length == 1 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        _estimatedIncome = _estimatedIncome + "|10000000";
                    }
                    else if (parts.Length == 2 && string.IsNullOrWhiteSpace(parts[0]))
                    {
                        _estimatedIncome = "1|" + _estimatedIncome;
                    }


                    return _estimatedIncome;
                }
                catch (Exception)
                {

                    return null;
                }
            }
            set
            {
                _estimatedIncome = value;
            }
        }

        public int MaxNumberOfLeads { get; set; }

        public int LengthOfExclusivity { get; set; }

        public Dictionary<string, string[]> GetMapfulFriendly()
        {
            var fieldProps = GetType().GetPropertiesWithAttribute<DescriptionAttribute>();

            var dic =  fieldProps
                        .Where(f => f.GetValue(this) != null)
                        .ToDictionary(p => p.GetCustomAttribute<DescriptionAttribute>().Description,
                        p => (p.GetValue(this) as string)?.Split("|".ToCharArray()));



            //add owner
            dic["07785400-B34F-4AF2-B9AD-98079519333E"] = new string[]{ "8","9"};

            return  dic;

        }

    }
}
