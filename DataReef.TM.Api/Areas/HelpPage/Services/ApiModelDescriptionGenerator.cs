using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Serialization;
using Newtonsoft.Json;
using DataReef.Core.Attributes;
using DataReef.TM.Api.Areas.HelpPage.Services.ModelDescriptions;
using DataReef.TM.Api.Areas.HelpPage.Services.SampleGeneration;

namespace DataReef.TM.Api.Areas.HelpPage.Services
{
    /// <summary>
    /// Generates model descriptions for given types.
    /// </summary>
    [Service(typeof(IApiModelDescriptionGenerator), ServiceScope.Application)]
    internal class ApiModelDescriptionGenerator : IApiModelDescriptionGenerator
    {
        // Modify this to support more data annotation attributes.
        private readonly IDictionary<Type, Func<object, string>> annotationTextGenerator = new Dictionary<Type, Func<object, string>>
        {
            {typeof (RequiredAttribute), a => "Required"},
            {
                typeof (RangeAttribute), a =>
                {
                    var range = (RangeAttribute) a;
                    return String.Format(CultureInfo.CurrentCulture, "Range: inclusive between {0} and {1}",
                        range.Minimum, range.Maximum);
                }
            },
            {
                typeof (MaxLengthAttribute), a =>
                {
                    var maxLength = (MaxLengthAttribute) a;
                    return String.Format(CultureInfo.CurrentCulture, "Max length: {0}", maxLength.Length);
                }
            },
            {
                typeof (MinLengthAttribute), a =>
                {
                    var minLength = (MinLengthAttribute) a;
                    return String.Format(CultureInfo.CurrentCulture, "Min length: {0}", minLength.Length);
                }
            },
            {
                typeof (StringLengthAttribute), a =>
                {
                    var strLength = (StringLengthAttribute) a;
                    return String.Format(CultureInfo.CurrentCulture, "String length: inclusive between {0} and {1}",
                        strLength.MinimumLength, strLength.MaximumLength);
                }
            },
            {
                typeof (DataTypeAttribute), a =>
                {
                    var dataType = (DataTypeAttribute) a;
                    return String.Format(CultureInfo.CurrentCulture, "Data type: {0}",
                        dataType.CustomDataType ?? dataType.DataType.ToString());
                }
            },
            {
                typeof (RegularExpressionAttribute), a =>
                {
                    var regularExpression = (RegularExpressionAttribute) a;
                    return String.Format(CultureInfo.CurrentCulture, "Matching regular expression pattern: {0}",
                        regularExpression.Pattern);
                }
            },
        };

        // Modify this to add more default documentations.
        private readonly IDictionary<Type, string> defaultTypeDocumentation = new Dictionary<Type, string>
        {
            {typeof (Int16), "integer"},
            {typeof (Int32), "integer"},
            {typeof (Int64), "integer"},
            {typeof (UInt16), "unsigned integer"},
            {typeof (UInt32), "unsigned integer"},
            {typeof (UInt64), "unsigned integer"},
            {typeof (Byte), "byte"},
            {typeof (Char), "character"},
            {typeof (SByte), "signed byte"},
            {typeof (Uri), "URI"},
            {typeof (Single), "decimal number"},
            {typeof (Double), "decimal number"},
            {typeof (Decimal), "decimal number"},
            {typeof (String), "string"},
            {typeof (Guid), "globally unique identifier"},
            {typeof (TimeSpan), "time interval"},
            {typeof (DateTime), "date"},
            {typeof (DateTimeOffset), "date"},
            {typeof (Boolean), "boolean"},
        };

        private readonly IModelDocumentationProvider documentationProvider;

        private Dictionary<string, ModelDescription> GeneratedModels { get; set; }

        public ApiModelDescriptionGenerator(IModelDocumentationProvider documentationProvider)
        {
            this.documentationProvider = documentationProvider;
            this.GeneratedModels = new Dictionary<string, ModelDescription>(StringComparer.OrdinalIgnoreCase);

            HttpConfiguration httpConfiguration = GlobalConfiguration.Configuration;
            Collection<ApiDescription> apis = httpConfiguration.Services.GetApiExplorer().ApiDescriptions;
            foreach (ApiDescription api in apis)
            {
                Type parameterType;
                if (TryGetResourceParameter(api, httpConfiguration, out parameterType))
                    GetOrCreateModelDescription(parameterType);

                Type responseType;
                if (TryGetResourceResponse(api, out responseType))
                    GetOrCreateModelDescription(responseType);
            }
        }

        private static bool TryGetResourceResponse(ApiDescription apiDescription, out Type responseType)
        {
            var responseDescription = apiDescription.ResponseDescription;

            if (responseDescription == null)
            {
                responseType = null;
                return false;
            }

            responseType = responseDescription.ResponseType;

            if (responseType != null && responseType != typeof(IHttpActionResult) && !responseType.Assembly.ManifestModule.Name.Equals("mscorlib.dll") && responseType.GetInterfaces().All(i => i != typeof(IHttpActionResult)))
                return true;

            responseType = null;
            return false;
        }

        private static bool TryGetResourceParameter(ApiDescription apiDescription, HttpConfiguration config, out Type resourceType)
        {
            var parameterDescription = apiDescription.ParameterDescriptions.FirstOrDefault(
                p => p.Source == ApiParameterSource.FromBody || (p.ParameterDescriptor != null && p.ParameterDescriptor.ParameterType == typeof(HttpRequestMessage)));

            if (parameterDescription == null)
            {
                resourceType = null;
                return false;
            }

            resourceType = parameterDescription.ParameterDescriptor.ParameterType;

            if (resourceType == typeof(HttpRequestMessage))
            {
                HelpPageSampleGenerator sampleGenerator = config.GetHelpPageSampleGenerator();
                resourceType = sampleGenerator.ResolveHttpRequestMessageType(apiDescription);
            }

            return resourceType != null;
        }

        public ModelDescription GetOrCreateModelDescription(Type modelType)
        {
            if (modelType == null)
                throw new ArgumentNullException("modelType");

            Type underlyingType = Nullable.GetUnderlyingType(modelType);
            if (underlyingType != null)
                modelType = underlyingType;

            string modelName = ModelNameHelper.GetModelName(modelType);

            return this.InternalGetOrCreateModelDescription(modelType, modelName);
        }
        public ModelDescription GetOrCreateModelDescription(string modelName)
        {
            ModelDescription modelDescription;
            this.GeneratedModels.TryGetValue(modelName, out modelDescription);
            return modelDescription;
        }

        public IEnumerable<ModelDescription> GetAllModelDescriptions()
        {
            return this.GeneratedModels.Values;
        }

        private ModelDescription InternalGetOrCreateModelDescription(Type modelType, string modelName)
        {
            ModelDescription modelDescription;

            if (this.GeneratedModels.TryGetValue(modelName, out modelDescription))
            {
                if (modelType != modelDescription.ModelType)
                {
                    throw new InvalidOperationException(
                        String.Format(
                            CultureInfo.CurrentCulture,
                            "A model description could not be created. Duplicate model name '{0}' was found for types '{1}' and '{2}'. " +
                            "Use the [ModelName] attribute to change the model name for at least one of the types so that it has a unique name.",
                            modelName,
                            modelDescription.ModelType.FullName,
                            modelType.FullName));
                }

                return modelDescription;
            }

            if (this.defaultTypeDocumentation.ContainsKey(modelType))
            {
                return this.GenerateSimpleTypeModelDescription(modelType);
            }

            if (modelType.IsEnum)
            {
                return this.GenerateEnumTypeModelDescription(modelType);
            }

            if (modelType.IsGenericType)
            {
                Type[] genericArguments = modelType.GetGenericArguments();

                if (genericArguments.Length == 1)
                {
                    Type enumerableType = typeof(IEnumerable<>).MakeGenericType(genericArguments);
                    if (enumerableType.IsAssignableFrom(modelType))
                    {
                        return this.GenerateCollectionModelDescription(modelType, genericArguments[0]);
                    }
                }
                if (genericArguments.Length == 2)
                {
                    Type dictionaryType = typeof(IDictionary<,>).MakeGenericType(genericArguments);
                    if (dictionaryType.IsAssignableFrom(modelType))
                    {
                        return this.GenerateDictionaryModelDescription(modelType, genericArguments[0], genericArguments[1]);
                    }

                    Type keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(genericArguments);
                    if (keyValuePairType.IsAssignableFrom(modelType))
                    {
                        return this.GenerateKeyValuePairModelDescription(modelType, genericArguments[0], genericArguments[1]);
                    }
                }
            }

            if (modelType.IsArray)
            {
                Type elementType = modelType.GetElementType();
                return this.GenerateCollectionModelDescription(modelType, elementType);
            }

            if (modelType == typeof(NameValueCollection))
            {
                return this.GenerateDictionaryModelDescription(modelType, typeof(string), typeof(string));
            }

            if (typeof(IDictionary).IsAssignableFrom(modelType))
            {
                return this.GenerateDictionaryModelDescription(modelType, typeof(object), typeof(object));
            }

            if (typeof(IEnumerable).IsAssignableFrom(modelType))
            {
                return this.GenerateCollectionModelDescription(modelType, typeof(object));
            }

            return this.GenerateComplexTypeModelDescription(modelType);
        }

        // Change this to provide different name for the member.
        private static string GetMemberName(MemberInfo member, bool hasDataContractAttribute)
        {
            var jsonProperty = member.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonProperty != null && !String.IsNullOrEmpty(jsonProperty.PropertyName))
            {
                return jsonProperty.PropertyName;
            }

            if (hasDataContractAttribute)
            {
                var dataMember = member.GetCustomAttribute<DataMemberAttribute>();
                if (dataMember != null && !String.IsNullOrEmpty(dataMember.Name))
                {
                    return dataMember.Name;
                }
            }

			var propertyDescriptor = member.GetCustomAttribute<PropertyDescriptorAttribute>();
			if ((propertyDescriptor != null) && (!string.IsNullOrWhiteSpace(propertyDescriptor.PropertyName)))
			{
				return propertyDescriptor.PropertyName;
			}

            return member.Name;
        }

        private static bool ShouldDisplayMember(MemberInfo member, bool hasDataContractAttribute)
        {
            var jsonIgnore = member.GetCustomAttribute<JsonIgnoreAttribute>();
            var xmlIgnore = member.GetCustomAttribute<XmlIgnoreAttribute>();
            var ignoreDataMember = member.GetCustomAttribute<IgnoreDataMemberAttribute>();
            var nonSerialized = member.GetCustomAttribute<NonSerializedAttribute>();
            var apiExplorerSetting = member.GetCustomAttribute<ApiExplorerSettingsAttribute>();

            bool hasMemberAttribute = member.DeclaringType.IsEnum
                ? member.GetCustomAttribute<EnumMemberAttribute>() != null
                : member.GetCustomAttribute<DataMemberAttribute>() != null;

            // Display member only if all the followings are true:
            // no JsonIgnoreAttribute
            // no XmlIgnoreAttribute
            // no IgnoreDataMemberAttribute
            // no NonSerializedAttribute
            // no ApiExplorerSettingsAttribute with IgnoreApi set to true
            // no DataContractAttribute without DataMemberAttribute or EnumMemberAttribute
            return jsonIgnore == null &&
                   xmlIgnore == null &&
                   ignoreDataMember == null &&
                   nonSerialized == null &&
                   (apiExplorerSetting == null || !apiExplorerSetting.IgnoreApi) &&
                   (!hasDataContractAttribute || hasMemberAttribute);
        }

        private string CreateDefaultDocumentation(Type type)
        {
            string documentation;

            if (this.defaultTypeDocumentation.TryGetValue(type, out documentation))
                return documentation;

            if (this.documentationProvider != null)
                documentation = this.documentationProvider.GetDocumentation(type);

            return documentation;
        }

        private void GenerateAnnotations(MemberInfo property, ParameterDescription propertyModel)
        {
            var annotations = new List<ParameterAnnotation>();

            IEnumerable<Attribute> attributes = property.GetCustomAttributes();
            foreach (var attribute in attributes)
            {
                Func<object, string> textGenerator;
                if (this.annotationTextGenerator.TryGetValue(attribute.GetType(), out textGenerator))
                {
                    annotations.Add(
                        new ParameterAnnotation
                        {
                            AnnotationAttribute = attribute,
                            Documentation = textGenerator(attribute)
                        });
                }
            }

            // Rearrange the annotations
            annotations.Sort((x, y) =>
            {
                // Special-case RequiredAttribute so that it shows up on top
                if (x.AnnotationAttribute is RequiredAttribute)
                {
                    return -1;
                }
                if (y.AnnotationAttribute is RequiredAttribute)
                {
                    return 1;
                }

                // Sort the rest based on alphabetic order of the documentation
                return String.Compare(x.Documentation, y.Documentation, StringComparison.OrdinalIgnoreCase);
            });

            foreach (var annotation in annotations)
            {
                propertyModel.Annotations.Add(annotation);
            }
        }

        private CollectionModelDescription GenerateCollectionModelDescription(Type modelType, Type elementType)
        {
            ModelDescription collectionModelDescription = this.GetOrCreateModelDescription(elementType);
            if (collectionModelDescription != null)
            {
                return new CollectionModelDescription
                {
                    Name = ModelNameHelper.GetModelName(modelType),
                    ModelType = modelType,
                    ElementDescription = collectionModelDescription
                };
            }

            return null;
        }

		private Type ResolveModelPropertyType(PropertyInfo property)
		{
			if (property == null)
			{
				return null;
			}

			Type resolvedType = property.PropertyType;

			// use PropertyDescriptorAttribute to "override" the descripted property type
			var propertyDescriptor = property.GetCustomAttribute<PropertyDescriptorAttribute>();

			if((propertyDescriptor != null) && (propertyDescriptor.PropertyType != null))
			{
				resolvedType = propertyDescriptor.PropertyType;
			}

			return resolvedType;
		}

        private ModelDescription GenerateComplexTypeModelDescription(Type modelType)
        {
            var complexModelDescription = new ComplexTypeModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                Documentation = this.CreateDefaultDocumentation(modelType)
            };

            this.GeneratedModels.Add(complexModelDescription.Name, complexModelDescription);
            bool hasDataContractAttribute = modelType.GetCustomAttribute<DataContractAttribute>() != null;
            PropertyInfo[] properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (ShouldDisplayMember(property, hasDataContractAttribute))
                {
                    var propertyModel = new ParameterDescription
                    {
                        Name = GetMemberName(property, hasDataContractAttribute)
                    };

                    if (this.documentationProvider != null)
                    {
                        propertyModel.Documentation = this.documentationProvider.GetDocumentation(property);
                    }

                    this.GenerateAnnotations(property, propertyModel);
                    complexModelDescription.Properties.Add(propertyModel);
                    var propertyType = this.ResolveModelPropertyType(property);
                    propertyModel.TypeDescription = this.GetOrCreateModelDescription(propertyType);
                }
            }

            FieldInfo[] fields = modelType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (ShouldDisplayMember(field, hasDataContractAttribute))
                {
                    var propertyModel = new ParameterDescription
                    {
                        Name = GetMemberName(field, hasDataContractAttribute)
                    };

                    if (this.documentationProvider != null)
                    {
                        propertyModel.Documentation = this.documentationProvider.GetDocumentation(field);
                    }

                    complexModelDescription.Properties.Add(propertyModel);
                    propertyModel.TypeDescription = this.GetOrCreateModelDescription(field.FieldType);
                }
            }

            return complexModelDescription;
        }

        private DictionaryModelDescription GenerateDictionaryModelDescription(Type modelType, Type keyType, Type valueType)
        {
            ModelDescription keyModelDescription = this.GetOrCreateModelDescription(keyType);
            ModelDescription valueModelDescription = this.GetOrCreateModelDescription(valueType);

            return new DictionaryModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                KeyModelDescription = keyModelDescription,
                ValueModelDescription = valueModelDescription
            };
        }

        private EnumTypeModelDescription GenerateEnumTypeModelDescription(Type modelType)
        {
            var enumDescription = new EnumTypeModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                Documentation = this.CreateDefaultDocumentation(modelType)
            };
            bool hasDataContractAttribute = modelType.GetCustomAttribute<DataContractAttribute>() != null;
            foreach (var field in modelType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (ShouldDisplayMember(field, hasDataContractAttribute))
                {
                    var enumValue = new EnumValueDescription
                    {
                        Name = field.Name,
                        Value = field.GetRawConstantValue().ToString()
                    };
                    if (this.documentationProvider != null)
                    {
                        enumValue.Documentation = this.documentationProvider.GetDocumentation(field);
                    }
                    enumDescription.Values.Add(enumValue);
                }
            }
            this.GeneratedModels.Add(enumDescription.Name, enumDescription);

            return enumDescription;
        }

        private KeyValuePairModelDescription GenerateKeyValuePairModelDescription(Type modelType, Type keyType,
            Type valueType)
        {
            ModelDescription keyModelDescription = this.GetOrCreateModelDescription(keyType);
            ModelDescription valueModelDescription = this.GetOrCreateModelDescription(valueType);

            return new KeyValuePairModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                KeyModelDescription = keyModelDescription,
                ValueModelDescription = valueModelDescription
            };
        }

        private ModelDescription GenerateSimpleTypeModelDescription(Type modelType)
        {
            var simpleModelDescription = new SimpleTypeModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType),
                ModelType = modelType,
                Documentation = this.CreateDefaultDocumentation(modelType)
            };
            this.GeneratedModels.Add(simpleModelDescription.Name, simpleModelDescription);

            return simpleModelDescription;
        }
    }
}