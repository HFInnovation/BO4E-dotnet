using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using BO4E.meta;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

namespace BO4E.BO
{
    /// <summary>
    /// General abstract class for Business Objects for Energy (BO4E)
    /// </summary>
    /// This class is not intended to be instantiated. It just provides the boTyp and version
    /// attribute which are obligatory for all BO4E business objects.
    /// <author>Hochfrequenz Unternehmensberatung GmbH</author>
    [JsonConverter(typeof(BusinessObjectBaseConverter))]
    public abstract class BusinessObject : IEquatable<BusinessObject>
    {
        /// <summary>
        /// obligatory type of the business object in UPPER CASE
        /// </summary>
        /// <example>
        /// 'MESSLOKATION',
        /// 'MARKTLOKATION'
        /// </example>
        [JsonProperty(Required = Required.Default, Order = -2)]
        private string boTyp;

        /// <summary>
        /// Fields that are not part of the BO4E-definition are stored in a element, that is
        /// accessable under the key defined in userPropertiesName.
        /// </summary>
        /// This JObject representing a Messlokation
        /// <example>
        /// <code>
        /// {
        ///    "boTyp": "MESSLOKATION",
        ///    "versionStruktur": 1,
        ///    "messLokationsId": "DE123...",
        ///    "irgendwas unbekanntes": "xyz"
        /// }
        /// </code>
        /// is mapped to
        /// <code>
        /// {
        ///    "boTyp": "MESSLOKATION",
        ///    "versionStruktur": 1,
        ///    "messLokationsId": "DE123...",
        ///    "userProperties":
        ///    {
        ///        "irgendwas unbekanntes": "xyz"
        ///    }
        /// }
        /// </code>
        /// This keeps the Business Object simple but allows for user specific arguments beyond
        /// the BO4E standard to be passed along.
        /// </example>
        [JsonIgnore]
        public const string userPropertiesName = "userProperties";

        /// <summary>
        /// User properties (non bo4e standard)
        /// </summary>
        [JsonProperty(PropertyName = userPropertiesName, Required = Required.Default, Order = 100)]
        [JsonExtensionData]
        [DataCategory(DataCategory.USER_PROPERTIES)]
        public IDictionary<string, JToken> userProperties;

        /// <summary>
        /// generates the BO4E boTyp attribute value (class name as upper case)
        /// </summary>
        public BusinessObject()
        {
            boTyp = this.GetType().Name.ToUpper();
            versionStruktur = 1;
        }

        /// <summary>
        /// return <see cref="BusinessObject.boTyp"/> (as string, not as type)
        /// </summary>
        /// <returns></returns>
        public string GetBoTyp() => this.boTyp;

        /// <summary>
        /// This method is just to make sure the mapping actually makes sense.
        /// </summary>
        /// <param name="s">name of the business object</param>
        protected void SetBoTyp(string s)
        {
            //Debug.Assert(boTyp == s);
        }
        /// <summary>
        /// obligatory version of the BO4E definition. Currently hard coded to 1
        /// </summary>
        /// <example>
        /// 1
        /// </example>
        [JsonProperty(Required = Required.Default, Order = -3)]
        public int versionStruktur;

        /// <summary>
        /// allows adding a GUID to Business Objects for tracking across systems
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string guid;

        /// <summary>
        /// returns a JSON scheme for the Business Object
        /// </summary>
        /// <returns>a JSON scheme</returns>
        public JSchema GetJsonScheme()
        {
            return GetJsonScheme(this.GetType());
        }


        /// <summary>
        /// returns a JSON scheme for the given type <paramref name="boType"/>
        /// </summary>
        /// <param name="boType">a type derived from <see cref="BusinessObject"/></param>
        /// <returns>a JSON scheme</returns>
        /// <exception cref="System.ArgumentException" />
        public static JSchema GetJsonScheme(Type boType)
        {
            if (!boType.IsSubclassOf(typeof(BusinessObject)))
            {
                throw new ArgumentException($"You must only request JSON schemes for Business Objects. {boType.ToString()} is not a valid Business Object type.");
            }
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new StringEnumGenerationProvider());
            JSchema schema = generator.Generate(boType);
            return schema;
        }

        /// <summary>
        /// Get a BO4E compliant URI for this business object. 
        /// </summary>
        /// Use .ToString() on the result to pass it between services.
        /// <returns>a BO4E compliant URI object</returns>
        public Bo4eUri GetURI(bool includeUserProperties = false)
        {
            return Bo4eUri.GetUri(this, includeUserProperties);
        }

        /// <summary>
        /// (Some) Business Objects do have keys that should identify them in a unique manner across
        /// multiple systems. This method returns a list of these keys. The list contains the key
        /// names as they are serialised in JSON. This means that the fields PropertyName is part
        /// of the list if JsonPropertyAttribute.PropertyName is set in the Business Objects 
        /// definition. Please do not use this method trying to access the actual key values. Use 
        /// the <see cref="GetBoKeys"/> or <see cref="GetBoKeyFieldInfos(Type)"/> for this purpose.
        /// The list is sorted by the JsonPropertyAttribute.Order, assuming 0 if not specified.
        /// </summary>
        /// <returns>A list of the names (not the values) of the (composite) Business Object key or an empty list if no key attributes are defined.</returns>
        /// <example><code>
        /// Marktlokation malo = ... some initialisation code ...;
        /// malo.GetBoKeyNames()
        /// </code>
        /// [ "marktlokationsId" ]
        /// </example>
        /// <seealso cref="GetBoKeyNames(Type)"/>
        public List<string> GetBoKeyNames()
        {
            return GetBoKeyNames(this.GetType());
        }

        /// <summary>
        /// Same as <see cref="GetBoKeyNames(Type)"/> but allows static calling with a Business Object Type provided.
        /// </summary>
        /// <param name="boType">Business Object Type</param>
        /// <returns>A list just like the result of <see cref="GetBoKeyNames(Type)"/></returns>
        public static List<string> GetBoKeyNames(Type boType)
        {
            List<string> result = new List<string>();
            foreach (FieldInfo fi in GetBoKeyFieldInfos(boType))
            {
                JsonPropertyAttribute jpa = fi.GetCustomAttribute<JsonPropertyAttribute>();
                if (jpa != null && jpa.PropertyName != null)
                {
                    result.Add(jpa.PropertyName);
                }
                else
                {
                    result.Add(fi.Name.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// This method returns information about the object structure, especially if there are
        /// nested field, that could be used, e.g. to expand OData queries.
        /// </summary>
        /// <param name="boType">valid business object type
        /// <example>
        /// <code>typeof(Marktlokation)</code>
        /// </example></param>
        /// <returns>
        /// A dictionary with field names as keys (or the different JSON property name if set)
        /// and the type of the property as value. Nesting and different layers are denoted by
        /// using "."
        /// </returns>
        public static Dictionary<string, Type> GetExpandableFieldNames(Type boType)
        {
            return GetExpandableFieldNames(boType, true);
        }

        /// <summary>
        /// <see cref="GetExpandableFieldNames(Type)"/>
        /// </summary>
        /// <param name="boTypeName">name of the business object as string</param>
        /// <returns><see cref="GetExpandableFieldNames(Type)"/></returns>
        public static Dictionary<string, Type> GetExpandableFieldNames(string boTypeName)
        {
            Type clazz = Assembly.GetExecutingAssembly().GetType(BoMapper.packagePrefix + "." + boTypeName);
            if (clazz == null)
            {
                throw new ArgumentException($"{boTypeName} is not a valid Business Object name. Use one of the following: {string.Join("\n -", BoMapper.GetValidBoNames())}");
            }
            return GetExpandableFieldNames(clazz);
        }

        /// <summary>
        /// recursive function to return all expandable fields for a given type <paramref name="type"/>.
        /// Set <paramref name="rootLevel"/> when calling from outside the function itself.
        /// </summary>
        /// <param name="type">Type inherited from Business Object</param>
        /// <param name="rootLevel">true iff calling from outside the function itself / default</param>
        /// <returns>HashSet of strings</returns>
        protected static Dictionary<string, Type> GetExpandableFieldNames(Type type, bool rootLevel = true)
        {
            if (rootLevel && !type.IsSubclassOf(typeof(BO.BusinessObject)))
            {
                throw new ArgumentException("Only allowed for BusinessObjects");
            }
            Dictionary<string, Type> result = new Dictionary<string, Type>();
            foreach (FieldInfo field in type.GetFields())
            {
                string fieldName;
                JsonPropertyAttribute jpa = field.GetCustomAttribute<JsonPropertyAttribute>();
                if (jpa != null && jpa.PropertyName != null)
                {
                    fieldName = jpa.PropertyName;
                }
                else
                {
                    fieldName = field.Name;
                }
                if (field.FieldType.IsSubclassOf(typeof(BO.BusinessObject)))
                {
                    foreach (KeyValuePair<string, Type> subResult in GetExpandableFieldNames(field.FieldType, false))
                    {
                        result.Add(string.Join(".", new string[] { fieldName, subResult.Key }), subResult.Value);
                    }
                    result.Add(fieldName, field.FieldType);
                }
                else if (field.FieldType.IsSubclassOf(typeof(COM.COM)))
                {
                    result.Add(fieldName, field.FieldType);
                    // coms do not contain any exandable subfield since they're flat
                }
                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type listElementType = field.FieldType.GetGenericArguments()[0];
                    foreach (KeyValuePair<string, Type> subResult in GetExpandableFieldNames(listElementType, false))
                    {
                        result.Add(string.Join(".", new string[] { fieldName, subResult.Key }), subResult.Value);
                    }
                    result.Add(fieldName, field.FieldType);
                }
                else
                {
                    // nada
                }
            }
            return result;
        }

        /// <summary>
        /// Get a dictionary containing the key values of this Business Object.
        /// The dictionary has the JsonPropertyAttribute.PropertyName or FieldName
        /// of the key as key and the actual key value as value.
        /// </summary>
        /// <seealso cref="GetBoKeyFieldInfos(Type)"/>
        /// <returns>A dictionary with key value pairs.</returns>
        public Dictionary<string, object> GetBoKeys()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (FieldInfo fi in GetBoKeyFieldInfos(this.GetType()))
            {
                JsonPropertyAttribute jpa = fi.GetCustomAttribute<JsonPropertyAttribute>();
                if (jpa != null && jpa.PropertyName != null)
                {
                    result.Add(jpa.PropertyName, fi.GetValue(this));
                }
                else
                {
                    result.Add(fi.Name, fi.GetValue(this));
                }
            }
            return result;
        }

        /// <summary>
        /// Get a list of FieldInfo objects that contain the key of a Business Object.
        /// The list is sorted by the JsonPropertyAttribute.Order, assuming 0 if not specified.
        /// </summary>
        /// <param name="boType">Business Object type</param>
        /// <returns>A list of FieldInfos to be used for accessing the key values.</returns>
        public static List<FieldInfo> GetBoKeyFieldInfos(Type boType)
        {
            if (!boType.IsSubclassOf(typeof(BusinessObject)))
            {
                throw new ArgumentException($"Business Object keys are only defined on Business Object types but {boType.ToString()} is not a Business Object.");
            }
            return boType.GetFields()
                 .Where(f => f.GetCustomAttributes(typeof(BoKey), false).Length > 0)
                 .OrderBy(af => af.GetCustomAttribute<JsonPropertyAttribute>()?.Order)
                 .ToList<FieldInfo>();
        }

        /// <summary>BO4E Business Objects are considered equal iff all of their elements/fields are equal.</summary>
        /// <param name="b">another object</param>
        /// <returns><code>true</code> iff b has the same type as this object and all elements of this and object b are equal; <code>false</code> otherwise</returns>
        public override bool Equals(object b)
        {
            if (b == null || b.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals(b as BusinessObject);
        }

        /// <summary>
        /// BO4E Business Objects are considered equal iff all of their elements/fields are equal.
        /// </summary>
        /// The method throws an argument exception if you try to test invalid BO4E objects for
        /// equality, e.g. when at least one of the compared objects does lack mandatory fields.
        /// <param name="b">another Business Object</param>
        /// <returns>
        /// <code>true</code> iff b has the same type as this object and all elements
        /// of this and object b are equal; <code>false</code> otherwise
        /// </returns>
        public bool Equals(BusinessObject b)
        {
            if (b == null || b.GetType() != this.GetType())
            {
                return false;
            }
            try
            {
                return JsonConvert.SerializeObject(this) == JsonConvert.SerializeObject(b);
            }
            catch (JsonSerializationException e)
            {
                throw new ArgumentException($"You must not compare/call equals() on invalid Business Objects: {e.Message}");
            }
        }

        /// <summary>
        /// override hash code generation
        /// </summary>
        /// <returns>hash code as int</returns>
        public override int GetHashCode()
        {
            int result = 31;  // I read online that a medium sized prime was a good choice ;)
            unchecked
            {
                result *= this.GetType().GetHashCode();
                foreach (FieldInfo field in this.GetType().GetFields())
                {
                    if (field.GetValue(this) != null)
                    {
                        if (field.GetValue(this).GetType().IsGenericType && field.GetValue(this).GetType().GetGenericTypeDefinition() == typeof(List<>))
                        {
                            IEnumerable enumerable = field.GetValue(this) as IEnumerable;
                            Type listElementType = field.GetValue(this).GetType().GetGenericArguments()[0];
                            Type listType = typeof(List<>).MakeGenericType(listElementType);
                            int index = 0;
                            foreach (object listItem in enumerable)
                            {
                                // the index/position inside the list is taken into account, because
                                // if two lists contain the same items but in different order, they must not be considered equal.
                                result *= 19 + (17 * (++index)) * listItem.GetHashCode();
                            }
                        }
                        else
                        {
                            // Using + 19 because the default hash code of uninitialised enums is zero.
                            // This would screw up the calculation such that all objects with at least one null value had the same hash code, namely 0.
                            result *= 19 + field.GetValue(this).GetHashCode();
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// converts <see cref="BusinessObject.boTyp"/> to upper case.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        protected void DeserializationFixes(StreamingContext context)
        {
            if (boTyp != null)
            {
                this.boTyp = boTyp.ToUpper();
            }
        }

        /// <summary>
        /// Tests if the object does contain all mandatory information / fields.
        /// To do so, the function tries to serialize the object as JSON.
        /// If the serialization fails due to fields that are <see cref="JsonPropertyAttribute.Required"/> but not filled
        /// it returns false and true otherwise.
        /// </summary>
        /// <returns>true if COM object is compatible with BO4E standards</returns>
        public virtual bool IsValid()
        {
            try
            {
                JsonConvert.SerializeObject(this);
            }
            catch (JsonSerializationException)
            {
                return false;
            }
            return true;
        }

        internal class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
        {
            protected override JsonConverter ResolveContractConverter(Type objectType)
            {
                if (typeof(BusinessObject).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                {
                    return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
                }

                return base.ResolveContractConverter(objectType);
            }
        }


        internal class BusinessObjectBaseConverter : JsonConverter
        {
            //private static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(BusinessObject));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (objectType.IsAbstract)
                {
                    JObject jo = JObject.Load(reader);
                    Type boType;
                    if (!jo.ContainsKey("boTyp"))
                    {
                        throw new ArgumentException("If deserializing into an abstract BusinessObject the key \"boTyp\" has to be set. But it wasn't.");
                    }
                    boType = BoMapper.GetTypeForBoName(jo["boTyp"].Value<string>()); // ToDo: catch exception if boTyp is not set and throw exception with descriptive error message
                    if (boType == null)
                    {
                        foreach (var assembley in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            try
                            {
                                boType = assembley.GetTypes().FirstOrDefault(x => x.Name.ToUpper() == jo["boTyp"].Value<string>().ToUpper());
                            }
                            catch (ReflectionTypeLoadException)
                            {
                                continue;
                            }
                            if (boType != null)
                            {
                                break;
                            }
                        }
                        if (boType == null)
                        {
                            throw new NotImplementedException($"The type '{jo["boTyp"].Value<string>()}' does not exist in the BO4E standard.");
                        }
                    }
                    return JsonConvert.DeserializeObject(jo.ToString(), boType);
                }
                else
                {
                    serializer.ContractResolver.ResolveContract(objectType).Converter = null;
                    return serializer.Deserialize(reader, objectType);
                }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException("Serializing an abstract BusinessObject is not supported."); // won't be called because CanWrite returns false
            }
        }
    }
}