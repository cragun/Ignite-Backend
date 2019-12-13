using DataReef.CodeGenerator;
using DataReef.Core.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;



namespace DataReef.TM.Models
{
    /// <summary>
    /// todo: Once the Auth is out, remove EntityBase
    /// </summary>
    /// 
    [ClientClass]
    [JsonObject(IsReference = false)]
    [DataContract(IsReference = true)]
    //[KnownType(typeof(Note))]
    public abstract class EntityBase : DbEntity
    {

        [NotMapped]
        [DataMember(EmitDefaultValue = false)]
        public virtual bool? IsNew { get; set; }

        #region Navigation

        #endregion

        #region Authorization

        //public virtual IEntityFilter<T> AuthorizationEntityFilter<T>(List<IdentityRole> roles, Guid userGuid) where T : EntityBase
        //{
        //    List<Guid> guids = new List<System.Guid>();
        //    foreach (IdentityRole ir in roles)
        //    {
        //        guids.AddRange(ir.ContainerIDs.Select(c => Guid.Parse(c)));
        //    }

        //    var q = from oc in EntityFilter<ObjectContainer>.AsQueryable()
        //            where guids.Contains(oc.ContainerID)
        //            select oc;

        //    IEntityFilter<T> filter = (IEntityFilter<T>)
        //        (
        //            q
        //        );

        //    return filter;
        //}

        #endregion

        public virtual void PrepareNavigationProperties(Guid? createdById = null)
        {
        }

        /// <summary>
        /// should be subclassed to removed the deleted items from its collections
        /// </summary>
        public override void FilterCollections<T>(string inclusionPath = "")
        {
        }

        protected ICollection<U> FilteredCollection<U>(ICollection<U> collection) where U : EntityBase
        {
            if (collection == null || collection.Count == 0)
                return collection;

            var returnData = collection
                                .Where(c => c.IsDeleted == false)
                                .ToList();

            return returnData;
        }

        /// <summary>
        /// Filters an entity and sets it to null if it has IsDeleted == true; 
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="inclusionPath">The classes navigation chain up to this point.</param>
        /// <param name="propagateFiltering">Flag that tells if the navigatio properties of the entities should be filtered checked as well.</param>
        public T FilterEntity<T>(T entity, string inclusionPath, bool propagateFiltering = true) where T : EntityBase
        {
            if (entity != null && entity.IsDeleted)
            {
                entity = null;
            }
            else if (entity != null && propagateFiltering)
            {
                entity.FilterCollections<T>(inclusionPath);
            }

            return entity;
        }

        /// <summary>
        /// Filter a collection of entities, removing the ones that have IsDeleted == true.
        /// </summary>
        /// <param name="entities">The collection of entities.</param>
        /// <param name="inclusionPath">The classes navigation chain up to this point.</param>
        /// <param name="propagateFiltering">Flag that tells if the navigatio properties of the entities should be filtered checked as well.</param>
        /// <returns>The list of filtered entities.</returns>
        public List<T> FilterEntityCollection<T>(IEnumerable<T> entities, string inclusionPath, bool propagateFiltering = true) where T : EntityBase
        {
            if (entities != null && entities.Any())
            {
                entities = entities.Where(e => e.IsDeleted == false).ToList();
                if (propagateFiltering)
                {
                    entities.ToList().ForEach(e =>
                    {
                        e.FilterCollections<T>(inclusionPath);
                    });
                }
            }

            return entities != null ? entities.ToList() : null;
        }

        /// <summary>
        /// Builds the path of classes.
        /// </summary>
        /// <param name="inclusionPath">The current navigation path.</param>
        /// <param name="className">The class that is currently investigated.</param>
        /// <param name="alreadyProcessed">Flag that tells if this class has already been included in the navigation.</param>
        /// <returns>The new navigation path.</returns>
        public string InclusionPathBuilder(string inclusionPath, string className, out bool alreadyProcessed)
        {
            alreadyProcessed = string.Format(".{0}.", inclusionPath).IndexOf(String.Format(".{0}.", className)) > -1;

            return String.Format("{0}.{1}", inclusionPath, className);
        }

        public string Serialize()
        {
            try
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.None,
                });
            }
            catch { }
            return null;
        }

        protected void Reset()
        {
            this.Guid = Guid.NewGuid();
            this.DateCreated = DateTime.UtcNow;
            this.DateLastModified = DateCreated;
            this.IsDeleted = false;
            this.Id = 0;
            this.Version = 1;
        }


    }
}
