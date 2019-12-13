using DataReef.Integrations.Google.Attributes;
using DataReef.Integrations.Google.Util;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataReef.Integrations.Google.Models
{
    public class FormModel : BaseModel
    {
        [Meta("Timestamp")]
        public DateTime TimeStamp { get; set; }

        [Meta("Provider Name")]
        public string ProviderName { get; set; }

        [Meta("States")]
        public List<string> States { get; set; }

        [Meta("Owner name")]
        public string OwnerName { get; set; }

        [Meta("Owner email address")]
        public string OwnerEmail { get; set; }

        //[Meta("Has sub organizations")]
        //public bool HasSubOUs { get; set; }

        //[Meta("Sub organization 1 Name", Count = 3, Size = 6)]
        //public List<SubOUModel> SubOUs { get; set; }

        [Meta("Finance plans")]
        public List<FinancePlanModel> FinancePlans { get; set; }

        [Meta("Instant prescreen source", Size = 2)]
        public PrescreenModel Prescreen { get; set; }

        [Meta("Inverters")]
        public List<InverterModel> Inverters { get; set; }

        [Meta("Solar Panels")]
        public List<PanelModel> Panels { get; set; }

        [Meta("Default Shading (percentage)")]
        public double DefaultShading { get; set; }

        [Meta("Default Annual Output Degradation (percentage)")]
        public double DefaultAnnualOutputDegradation { get; set; }

        [Meta("Solar Utility Inflation Rate (percentage)")]
        public double SolarUtilityInflationRate { get; set; }

        [Meta("Available Dispositions")]
        public List<DispositionModel> Dispositions { get; set; }

        [Meta("Ridge (inches)", Size = 5)]
        public FireOffsetModel FireOffset { get; set; }

        [Meta("Process (on next run)")]
        [JsonIgnore]
        public bool Process { get; set; }

        [Meta("Processed timestamp")]
        [JsonIgnore]
        public DateTime ProcessedTimestamp { get; set; }

        [JsonIgnore]
        public int RowNumber { get; set; }

        public FormModel(IList<string> headers, IList<string> values, int rowNumber) : base(headers, values)
        {
            RowNumber = rowNumber;
        }
    }

    public abstract class BaseModel
    {
        protected IList<string> Headers;
        protected IList<string> Values;

        protected Dictionary<int, PropertyInfo> ContextProperties;
        protected Dictionary<string, PropertyInfo> NameProperties;

        protected void SetValue(string rawValue, PropertyInfo prop, int index)
        {
            object value = null;

            if (prop.PropertyType == typeof(string))
            {
                value = rawValue;
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
                DateTime date;
                if (DateTime.TryParse(rawValue, out date))
                {
                    value = date;
                }
            }
            else if (prop.PropertyType == typeof(bool))
            {
                var lowerValue = rawValue?.ToLower();
                value = lowerValue == "yes" || lowerValue == "on" || lowerValue == "true";
            }
            else if (prop.PropertyType == typeof(double))
            {
                rawValue = rawValue?.TrimEnd('%');
                double doubleValue;
                if (double.TryParse(rawValue, out doubleValue))
                {
                    value = doubleValue;
                }
            }
            else if (prop.PropertyType == typeof(int))
            {
                int intValue;
                if (int.TryParse(rawValue, out intValue))
                {
                    value = intValue;
                }
            }
            else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericArgType = prop.PropertyType.GenericTypeArguments.FirstOrDefault();
                if (genericArgType?.IsSubclassOf(typeof(BaseModel)) == true)
                {
                    // get the meta attribute
                    var attrib = prop.GetCustomAttribute<MetaAttribute>();

                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(genericArgType);

                    IList newValue = Activator.CreateInstance(constructedListType) as IList;

                    if (attrib.Size > 1)
                    {
                        for (int i = 0; i < attrib.Count; i++)
                        {
                            var args = Values.Skip(index + (attrib.Size * i)).Take(attrib.Size).ToList();
                            // only create element if there's at least one value
                            if (args.Any(a => !string.IsNullOrWhiteSpace(a)))
                            {
                                var newElement = Activator.CreateInstance(genericArgType, args: args);
                                newValue.Add(newElement);
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(rawValue))
                        {
                            // split on new lines
                            var lines = Regex.Split(rawValue, "\n");

                            foreach (var line in lines)
                            {
                                var args = line
                                            .Split(Constants.PipeDelimiter)
                                            .Select(a => a.Trim())
                                            .ToList();

                                var newElement = Activator.CreateInstance(genericArgType, args: args);
                                newValue.Add(newElement);
                            }
                        }
                    }
                    value = newValue;
                }
                else
                {
                    value = rawValue
                                .Split(Constants.CommaDelimiter)
                                .Select(v => v.Trim())
                                .ToList();
                }
            }
            else if (prop.PropertyType.IsSubclassOf(typeof(BaseModel)) == true)
            {
                var attrib = prop.GetCustomAttribute<MetaAttribute>();
                if (attrib.Size > 1)
                {
                    for (int i = 0; i < attrib.Count; i++)
                    {
                        var args = Values.Skip(index + (attrib.Size * i)).Take(attrib.Size).ToList();
                        // only create element if there's at least one value
                        if (args.Any(a => !string.IsNullOrWhiteSpace(a)))
                        {
                            value = Activator.CreateInstance(prop.PropertyType, args: args);
                        }
                    }
                }
            }

            prop.SetValue(this, value);
        }

        protected virtual void ProcessContextData()
        {

            for (int i = 0; i < Values.Count; i++)
            {
                if (ContextProperties.ContainsKey(i))
                {
                    var prop = ContextProperties[i];
                    SetValue(Values[i], prop, i);
                }

            }
        }

        public virtual void ProcessData()
        {
            if (NameProperties.Count > 0)
            {
                ProcessMainData();
            }
            else
            {
                ProcessContextData();
            }
        }

        protected virtual void ProcessMainData()
        {
            if (NameProperties?.Count == 0)
            {
                throw new ArgumentException("No NameProperties data!");
            }

            for (int i = 0; i < Headers.Count; i++)
            {
                var header = Headers[i];
                if (NameProperties.ContainsKey(header))
                {
                    var prop = NameProperties[header];
                    if (Values.Count > i)
                    {
                        var value = Values[i];

                        SetValue(value, prop, i);
                    }
                }
            }
        }

        public BaseModel(IList<string> headers, IList<string> values) : this(values, false)
        {
            Headers = headers;
            ProcessData();
        }

        public BaseModel(IList<string> values, bool processData = true)
        {
            Values = values;

            var props = GetType().GetMetaProperties();

            var indexData = props
                                .Select(p => new { index = p.GetCustomAttribute<MetaAttribute>().ContextIndex, prop = p })
                                .GroupBy(p => p.index);

            if (indexData.Count() > 1)
            {
                ContextProperties = props
                                        .ToDictionary(p => p.GetCustomAttribute<MetaAttribute>().ContextIndex, p => p);
            }

            NameProperties = props
                                .Where(p => p.GetCustomAttribute<MetaAttribute>()?.Name != null)
                                .ToDictionary(p => p.GetCustomAttribute<MetaAttribute>().Name, p => p);

            if (processData)
            {
                ProcessData();
            }
        }
    }

    public class SubOUModel : BaseModel
    {
        [Meta(ContextIndex = 0)]
        public string Name { get; set; }
        [Meta(ContextIndex = 1)]
        public List<string> States { get; set; }
        [Meta(ContextIndex = 2)]
        public bool EnableProposal { get; set; }
        [Meta(ContextIndex = 3)]
        public bool EnableLoan { get; set; }
        [Meta(ContextIndex = 4)]
        public string ContractorID { get; set; }
        [Meta(ContextIndex = 5)]
        public string ProgramID { get; set; }

        public SubOUModel(List<string> values) : base(values)
        {
        }
    }

    public class FinancePlanModel : BaseModel
    {
        [Meta(ContextIndex = 0)]
        public string Provider { get; set; }
        [Meta(ContextIndex = 1)]
        public string Type { get; set; }
        [Meta(ContextIndex = 2)]
        public string Term { get; set; }
        [Meta(ContextIndex = 3)]
        public double InterestRate { get; set; }
        [Meta(ContextIndex = 4)]
        public string DisplayName { get; set; }

        public FinancePlanModel(List<string> values) : base(values)
        {

        }
    }
    public class PrescreenModel : BaseModel
    {
        public PrescreenModel(List<string> values) : base(values)
        {
        }

        [Meta(ContextIndex = 0)]
        public string Instant { get; set; }
        [Meta(ContextIndex = 1)]
        public string Batch { get; set; }
    }

    public class InverterModel : BaseModel
    {
        public InverterModel(List<string> values) : base(values)
        {
        }

        [Meta(ContextIndex = 0)]
        public string Model { get; set; }
        [Meta(ContextIndex = 1)]
        public string Manufacturer { get; set; }
        [Meta(ContextIndex = 2)]
        public string Efficiency { get; set; }
        [Meta(ContextIndex = 3)]
        public string Name { get; set; }
        [Meta(ContextIndex = 4)]
        public string IsMicro { get; set; }
        [Meta(ContextIndex = 5)]
        public string Rank { get; set; }
        [Meta(ContextIndex = 6)]
        public string MinSize { get; set; }
        [Meta(ContextIndex = 7)]
        public string MaxSize { get; set; }

    }

    public class PanelModel : BaseModel
    {
        public PanelModel(List<string> values) : base(values)
        {
        }

        [Meta(ContextIndex = 0)]
        public string Name { get; set; }
        [Meta(ContextIndex = 1)]
        public string Size { get; set; }
        [Meta(ContextIndex = 2)]
        public string Width { get; set; }
        [Meta(ContextIndex = 3)]
        public string Height { get; set; }
        [Meta(ContextIndex = 4)]
        public string ModuleType { get; set; }
        [Meta(ContextIndex = 5)]
        public string Description { get; set; }
        [Meta(ContextIndex = 6)]
        public int Rank { get; set; }

    }

    public class DispositionModel : BaseModel
    {
        public DispositionModel(List<string> values) : base(values)
        {
        }

        [Meta(ContextIndex = 0)]
        public string Category { get; set; }
        [Meta(ContextIndex = 1)]
        public string Title { get; set; }
        [Meta(ContextIndex = 2)]
        public List<string> SubCategories { get; set; }
        [Meta(ContextIndex = 3)]
        public string Color { get; set; }

    }

    public class FireOffsetModel : BaseModel
    {
        public FireOffsetModel(List<string> values) : base(values)
        {
        }

        [Meta(ContextIndex = 0)]
        public double Ridge { get; set; }
        [Meta(ContextIndex = 1)]
        public double Eave { get; set; }
        [Meta(ContextIndex = 2)]
        public double Hip { get; set; }
        [Meta(ContextIndex = 3)]
        public double Valley { get; set; }
        [Meta(ContextIndex = 4)]
        public double Edge { get; set; }

    }
}
