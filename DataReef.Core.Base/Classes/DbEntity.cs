using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections;

namespace DataReef.Core.Classes
{
    [JsonObject(IsReference = false)]
    [DataContract(IsReference = true)]
    public abstract class DbEntity
    {
        private List<string> navigationToSerialize = new List<string>();

        public DbEntity()
        {
            this.Guid = System.Guid.NewGuid();
            this.DateCreated = System.DateTime.UtcNow;
            this.DateLastModified = this.DateCreated;
            this.Version = 1;
            this.IsDeleted = false;
            this.PropertiesToSerialize = new List<string>();
            this.Name = string.Format("New {0}", this.GetType().Name);

        }

        protected HashSet<string> _defaultExcludedProperties = new HashSet<string> { "CustomValues" };
        [DataMember]
        [JsonIgnore]
        [NotMapped]
        public HashSet<string> DefaultExcludedProperties
        {
            get { return _defaultExcludedProperties; }
            set { _defaultExcludedProperties = value; }
        }

        public void AddDefaultExcludedProperties(params string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return;
            }

            _defaultExcludedProperties = _defaultExcludedProperties ?? new HashSet<string>(args);

            //foreach (var item in args)
            //{
            //    _defaultExcludedProperties.Add(item);
            //}
        }

        public void RemoveDefaultExcludedProperties(params string[] args)
        {
            if (args == null || args.Length == 0 || _defaultExcludedProperties == null)
            {
                return;
            }

            foreach (var item in args)
            {
                _defaultExcludedProperties.Remove(item);
            }
        }

        #region Properties
        [DataMember]
        [Key, Required]
        [NotPatchable]
        public virtual Guid Guid { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index("CLUSTERED_INDEX_ON_LONG", IsClustered = true)]
        [DataMember]
        public long Id { get; set; }


        [DataMember(EmitDefaultValue = false)]
        //[Indexed]
        [StringLength(100)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Used to store Bitwise Flags as determined by UI
        /// </summary>
        /// 
        [DataMember]
        public Int64? Flags { get; set; }


        /// <summary>
        /// The Tenant ID of the object and its family of data.  Each object belongs to a family of data (an OU or an Account). That family of data will be assigned a tenant id and all of 
        /// the data related to the tenant will be guaranteed to be stored on the same server.  If in doubt, use TenantID 0
        /// </summary>
        /// 
        [Index]
        [DataMember]
        [Required]
        [NotPatchable]
        public int TenantID { get; set; }


        /// <summary>
        /// UTC DateTime managed by server.  Date the record was created
        /// </summary>
        [Index]
        [NotPatchable]
        [DataMember(EmitDefaultValue = true)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual System.DateTime DateCreated { get; set; }

        [NotPatchable]
        [DataMember(EmitDefaultValue = false)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual System.DateTime? DateLastModified { get; set; }


        [DataMember]
        //[Indexed]
        [StringLength(100)]
        public virtual string CreatedByName { get; set; }

        [Index]
        [NotPatchable]
        [DataMember]
        public virtual Guid? CreatedByID { get; set; }

        [DataMember]
        [NotPatchable]
        public virtual Guid? LastModifiedBy { get; set; }

        [DataMember]
        //[Indexed]
        [StringLength(100)]
        public virtual string LastModifiedByName { get; set; }

        [Index]
        [NotPatchable]
        [DataMember(EmitDefaultValue = true)]
        public virtual int Version { get; set; }

        /// <summary>
        /// Flag for soft deletes.  All deletes are soft in this framework
        /// its better to call a Delete method than to maniuplate this field directly.
        /// should be ready only
        /// </summary>
        [NotPatchable]
        [DataMember(EmitDefaultValue = true)]
        public virtual bool IsDeleted { get; set; }

        [Index]
        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public virtual string ExternalID { get; set; }

        [NotMapped]
        [DataMember(EmitDefaultValue = false)]
        public virtual SaveResult SaveResult { get; set; }



        [StringLength(1000)]
        [DataMember(EmitDefaultValue = false)]
        public virtual string TagString { get; set; }


        public virtual List<string> Tags
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.TagString))
                {
                    string[] ret = this.TagString.Split(',', ';', '|');
                    return ret.ToList();
                }
                else
                {
                    return null;
                }

            }

            set
            {
                if (value != null)
                {
                    string val = string.Join(",", value.ToArray());
                    this.TagString = val;
                }
                else
                {
                    this.TagString = null;
                }
            }
        }

        #endregion

        #region Helper Stuff

        /// <summary>
        /// Increase version and update DateLastModified
        /// </summary>
        public void Updated(Guid? modifiedBy = null, string modifiedByName = null)
        {
            Version += 1;
            DateLastModified = DateTime.UtcNow;
            LastModifiedBy = modifiedBy;
            LastModifiedByName = modifiedByName;
        }

        public bool ShouldSerializePropertyNamed(string name)
        {
            return
                // Exclude conditions
                (PropertiesToExclude == null || !PropertiesToExclude.Contains(name))
                &&
                (
                    (PropertiesToSerialize == null || PropertiesToSerialize.Count == 0)
                    ||
                    (PropertiesToSerialize != null && PropertiesToSerialize.Contains(name))
                );
        }

        [DataMember]
        [JsonIgnore]
        protected List<string> PropertiesToSerialize { get; set; }

        [DataMember]
        [JsonIgnore]
        protected List<string> NavigationToSerialize
        {
            get { return this.navigationToSerialize; }
            set
            {
                if (this.navigationToSerialize == null)
                {
                    this.navigationToSerialize = new List<string>();
                }
                else
                {
                    this.navigationToSerialize.Clear();
                }



                foreach (string path in value)
                {
                    this.AddNavigationSerializationPath(path);
                }
            }
        }

        [DataMember]
        [JsonIgnore]
        protected HashSet<string> PropertiesToExclude { get; set; }

        /// <summary>
        /// This method sets the properties to exclude to this object
        /// And if needed, it recursively sets the exclude properties to child properties.
        /// </summary>
        /// <param name="properties"></param>
        public void AddPropertiesToExclude(string properties)
        {
            if (string.IsNullOrWhiteSpace(properties))
            {
                PropertiesToExclude = PropertiesToExclude ?? new HashSet<string>(DefaultExcludedProperties);

                //foreach (var item in DefaultExcludedProperties)
                //{
                //    PropertiesToExclude.Add(item);
                //}
                return;
            }

            var props = properties
                            .Split(',', '|')
                            .ToList();

            // if the parent is included in the property, remove it
            for (int i = 0; i < props.Count; i++)
            {
                var prop = RemoveStartingPropFromChain(props[i]);
            }

            // verify if we have deep excludes like: Territory.WellKnownText
            var deepProps = props
                            .Where(p => p.IndexOf(".") > 0)
                            .ToList();

            // recursively add all deep exclude properties
            if (deepProps.Count > 0)
            {
                var assemblyProperties = GetType().GetProperties();

                foreach (var prop in deepProps)
                {
                    var entities = prop.Split('.');
                    // first one is the property inside this object instance
                    var navProperty = entities.FirstOrDefault().Trim();

                    var assemblyProperty = assemblyProperties.FirstOrDefault(ap => ap.Name == navProperty);
                    var navPropertyValue = assemblyProperty.GetValue(this);

                    // no need to continue if the property for this instance has no value
                    if (navPropertyValue == null)
                    {
                        continue;
                    }

                    var restOfTheProperties = prop.Substring(prop.IndexOf(".") + 1);

                    // if the navigation property is a collection, go deeper and set the properties to exclude
                    if (assemblyProperty.PropertyType.IsGenericType && navPropertyValue is IEnumerable)
                    {
                        var navCollection = navPropertyValue as IEnumerable<DbEntity>;
                        if (navCollection == null)
                        {
                            continue;
                        }

                        foreach (var navItem in navCollection)
                        {

                            navItem.AddPropertiesToExclude(restOfTheProperties);
                        }
                    }
                    else
                    {
                        // if it's not a collection, try a DB entity
                        var nav = navPropertyValue as DbEntity;
                        if (nav == null)
                        {
                            continue;
                        }
                        nav.AddNavigationSerializationPath(restOfTheProperties);
                        nav.AddPropertiesToExclude(restOfTheProperties);
                    }
                }
            }

            props = props
                        .Where(p => p.IndexOf(".") < 0)
                        .ToList();

            if (PropertiesToExclude == null)
            {
                PropertiesToExclude = new HashSet<string>();
            }

            props?.ForEach(p => PropertiesToExclude.Add(p));

            foreach (var item in DefaultExcludedProperties)
            {
                PropertiesToExclude.Add(item);
            }
        }

        public void AddNavigationSerializationPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            List<string> parts = path.Split('.').ToList();
            if (parts.Count > 1)
            {
                //add the local path
                this.navigationToSerialize.Add(parts[0]);
                string currentPath = parts[0];

                //now stip out local path and add the next path recursively to the child objects
                parts.RemoveAt(0);

                string nextPath = parts[0];
                string newPath = string.Join(".", parts.ToArray());

                //now assign the newPath to the nextPath
                RunActionOnProperty(currentPath, (eb) => eb.AddNavigationSerializationPath(newPath));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(path)) this.navigationToSerialize.Add(path);
                // we call this method to add the default exclude properties to ParametersToExclude when we setup deep serialization
                this.AddPropertiesToExclude(null);
                // try to set the default exclude properties on Navigation Property.
                RunActionOnProperty(path, (eb) => eb.AddPropertiesToExclude(null));
            }
        }

        private void RunActionOnProperty(string propName, Action<DbEntity> action)
        {
            PropertyInfo pi = this.GetType().GetProperty(propName);

            if (pi == null)
            {
                return;
            }

            if (typeof(DbEntity).IsAssignableFrom(pi.PropertyType))
            {
                DbEntity eb = (DbEntity)pi.GetValue(this);
                if (eb != null)
                {
                    action(eb);
                }
            }
            else if (pi.PropertyType.GenericTypeArguments.Count() > 0 && typeof(DbEntity).IsAssignableFrom(pi.PropertyType.GenericTypeArguments[0]))
            {
                dynamic coll = pi.GetValue(this);
                if (coll != null)
                {
                    foreach (DbEntity eb in coll)
                    {
                        action(eb);
                    }
                }
            }
        }

        public void AddFieldsToSerialize(string properties)
        {
            if (string.IsNullOrWhiteSpace(properties))
            {
                return;
            }

            List<string> parts = properties.Split('|', ',').ToList();

            PropertiesToSerialize = PropertiesToSerialize ?? new List<string>();

            PropertiesToSerialize.AddRange(parts);
        }

        public virtual void SetupSerialization(string include, string exclude, string fields)
        {
            //AddNavigationSerializationPath(include);
            var includes = include != null ? include.Split(',') : new string[0];
            foreach (var item in includes)
            {
                AddNavigationSerializationPath(item);
            }

            AddPropertiesToExclude(exclude);
            AddFieldsToSerialize(fields);

            if (PropertiesToExclude != null)
            {

                var includeProps = new HashSet<string>();

                if (PropertiesToSerialize != null && PropertiesToSerialize.Count > 0)
                {
                    PropertiesToSerialize.ForEach(p => includeProps.Add(p));
                }

                if (NavigationToSerialize != null && NavigationToSerialize.Count > 0)
                {
                    // NavigationToSerialize are comma separated
                    var navProperties = NavigationToSerialize
                                            .SelectMany(n => n.Split(',', '|'))
                                            .ToList();

                    navProperties.ForEach(p => includeProps.Add(p));
                }

                // remove all include and navigation from default exclude
                if (includeProps.Count > 0)
                {
                    var defaultExcludeToInclude = includeProps
                                                    .Where(ps => DefaultExcludedProperties.Contains(ps))
                                                    .ToList();

                    PropertiesToExclude.RemoveWhere(ex => defaultExcludeToInclude.Contains(ex));
                }
            }
        }

        /// <summary>
        /// This method removes the starting property from Include or Exclude property.
        /// e.g. will remove OU from OU.WellKnownText
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objTypeName">Given object type name or current object type name</param>
        /// <returns></returns>
        protected virtual string RemoveStartingPropFromChain(string value, string objTypeName = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var thisProp = (objTypeName ?? GetType().Name) + ".";
            var props = value.Split(",|".ToCharArray());

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                if (prop.StartsWith(thisProp))
                {
                    props[i] = prop.Substring(thisProp.Length);
                }
            }
            return string.Join(",", props);
        }

        #endregion

        #region Navigation

        #endregion

        #region Authorization


        public virtual Func<DbEntity, bool> IsValid()
        {
            Func<DbEntity, bool> ret = eb => eb.IsDeleted == false;
            return ret;
        }

        public virtual IEntityFilter<T> AuthorizationEntityFilter<T>(List<Guid> userContainers, Guid userGuid) where T : DbEntity
        {
            IEntityFilter<T> filter = from eb in EntityFilter<T>.AsQueryable() where 1 == 1 select eb;
            return filter;
        }

        public abstract void FilterCollections<T>(string inclusionPath = "") where T : DbEntity;

        #endregion
    }
}
