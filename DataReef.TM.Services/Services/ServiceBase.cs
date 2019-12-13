using DataReef.Core.Attributes;
using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.Client;
using DataReef.TM.Models.Enums;
using DataReef.TM.Services.Services;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataReef.Core.Services
{
    public abstract class ServiceBase<T> where T : DbEntity
    {

        private List<string> includeFields = new List<string>();
        private Dictionary<string, PropertyInfo> fields = new Dictionary<string, PropertyInfo>();
        protected readonly ILogger logger;

        public ServiceBase(ILogger logger)
        {
            this.PrecacheFields();
            this.logger = logger;
        }

        private void PrecacheFields()
        {
            Type t = typeof(T);

            PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in props)
            {
                fields.Add(pi.Name.ToLower(), pi);

                //if (pi.GetCustomAttribute<IncludeableAttribute>() != null)
                //{
                //    includeFields.Add(pi.Name.ToLower());
                //}
            }
        }

        private string WhereString(string fieldName, string op, string value)
        {
            string ret = string.Empty;


            try
            {
                PropertyInfo pi = this.fields[fieldName.ToLower()];
                if (pi.PropertyType == typeof(string))
                {
                    if (op == "->") //starts with
                    {
                        ret = string.Format("{0}.StartsWith(\"{1}\")", fieldName, value);
                    }
                    else if (op == "~") //contains
                    {
                        ret = string.Format("{0}.Contains(\"{1}\")", fieldName, value);
                    }
                    else
                    {
                        ret = string.Format("{0}{1}\"{2}\"", fieldName, op, value);
                    }
                }
                else if (pi.PropertyType.IsEnum)
                {
                    int enumNumericValue;
                    var isNumber = Int32.TryParse(value, out enumNumericValue);
                    ret = string.Format("{0}{1}\"{2}\"", fieldName, op, isNumber ? Enum.ToObject(pi.PropertyType, enumNumericValue).ToString() : value);
                }
                else if (pi.PropertyType == typeof(DateTime))
                {
                    DateTime date;
                    if (DateTime.TryParse(value, out date))
                    {
                        ret = $"{fieldName}{op}DateTime({date.Year},{date.Month},{date.Day})";
                    }
                }
                else
                {
                    ret = string.Format("{0}{1}{2}", fieldName, op, value);
                }

            }
            catch (Exception)
            {
                //invalid field name, just ignore it
            }


            return ret;
        }

        protected void AssignFilters(string filterString, ref IQueryable<T> q)
        {
            AssignFilters<T>(filterString, ref q);
        }

        protected void AssignFilters<U>(string filterString, FilterLinkingOperator operation, ref IQueryable<U> q)
        {
            if (string.IsNullOrEmpty(filterString))
            {
                return;
            }

            char splitCharacter = (operation == FilterLinkingOperator.And ? '&' : '|');

            string[] filters = filterString.Split(splitCharacter);
            var whereCondition = string.Empty;

            foreach (string filter in filters)
            {
                string pattern = "(<=|>=|!=|=|>|<|->|~)";
                string[] parts = Regex.Split(filter, pattern);

                
                if (parts.Length >= 3)
                {
                    try
                    {
                        string field = parts[0];
                        string op = parts[1];
                        string value = parts[2];

                        op = op.Replace("==", "=");

                        // refactored the dynamic linq to work with nullable Guids like ToPersonID in ConnectionInvitations
                        Type propertyType = this.fields[field.ToLower()].PropertyType;
                        bool isGuid = propertyType == typeof(Guid);
                        bool isNullableGuid = propertyType == typeof(Guid?);

                        string where = string.Empty;

                        if (isGuid)
                        {
                            where = string.Format("{0}!=null && {0}.ToString().Equals(\"{1}\")", field, value);
                        }
                        else if (isNullableGuid)
                        {
                            where = string.Format("{0}!=null && {0}.Value.ToString().Equals(\"{1}\")", field, value);
                        }
                        else
                        {
                            where = this.WhereString(field, op, value);
                        }

                        if (!string.IsNullOrWhiteSpace(where))
                        {
                            string linkingOperation = operation == FilterLinkingOperator.And ? "&&" : "||";
                            whereCondition = string.IsNullOrEmpty(whereCondition) ? where : $"{whereCondition} {linkingOperation} {where}";
                            
                        }
                        

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                
            }
            if (!string.IsNullOrWhiteSpace(whereCondition))
            {
                q = q.Where(whereCondition);
            }
            
        }

        protected void AssignFilters<U>(string filterString, ref IQueryable<U> q)
        {
            if (string.IsNullOrEmpty(filterString))
            {
                return;
            }
            string[] filters = filterString.Split('&');

            foreach (string filter in filters)
            {
                string pattern = "(<=|>=|!=|=|>|<|->|~)";
                string[] parts = Regex.Split(filter, pattern);

                if (parts.Length >= 3)
                {
                    try
                    {
                        string field = parts[0];
                        string op = parts[1];
                        string value = parts[2];

                        op = op.Replace("==", "=");

                        // refactored the dynamic linq to work with nullable Guids like ToPersonID in ConnectionInvitations
                        Type propertyType = this.fields[field.ToLower()].PropertyType;
                        bool isGuid = propertyType == typeof(Guid);
                        bool isNullableGuid = propertyType == typeof(Guid?);

                        string where = string.Empty;

                        if (isGuid)
                        {
                            where = string.Format("{0}!=null && {0}.ToString().Equals(\"{1}\")", field, value);
                        }
                        else if (isNullableGuid)
                        {
                            where = string.Format("{0}!=null && {0}.Value.ToString().Equals(\"{1}\")", field, value);
                        }
                        else
                        {
                            where = this.WhereString(field, op, value);
                        }

                        if (!string.IsNullOrWhiteSpace(where)) q = q.Where(where);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            }
        }

        protected void AssignPredicates(IQueryable<T> q, string queryString)
        {
            List<string> parts = queryString.Split('|', ',', '&').ToList();
            foreach (string part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part) && this.includeFields.ConvertAll(s => s.ToLower()).Contains(part.ToLower()))
                {
                    q = q.Include(part);
                }

            }
        }

        //protected void SetupSerialization<U>(ICollection<U> items, string include, string exclude, string fields) where U : DbEntity
        //{
        //    if (items == null)
        //    {
        //        return;
        //    }

        //    foreach (var item in items)
        //    {
        //        item.SetupSerialization(include, exclude, fields);
        //    }
        //}

        protected static void AssignIncludes(string includeString, ref IQueryable<T> q)
        {
            AssignIncludes<T>(includeString, ref q);
            //List<string> parts = includeString.Split('|', ',', '&').ToList();

            //foreach (string part in parts)
            //{
            //    if (!string.IsNullOrWhiteSpace(part)) q = q.Include(part); //.AsNoTracking();
            //}
        }

        /// <summary>
        /// Created a generic method to allow use of include for different models other than the specified generic
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="includeString"></param>
        /// <param name="q"></param>
        protected static void AssignIncludes<U>(string includeString, ref IQueryable<U> q)
        {
            if (string.IsNullOrEmpty(includeString))
            {
                return;
            }

            List<string> parts = includeString.Split('|', ',', '&').ToList();

            foreach (string part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part)) q = q.Include(part); //.AsNoTracking();
            }
        }


        protected virtual void ProcessMail(EntityBase eb, DataAction action)
        {
            List<MailAttribute> matts = eb.GetType().GetCustomAttributes<MailAttribute>(true).ToList();

            if (matts.Any())
            {
                foreach (MailAttribute mat in matts)
                {
                    int dataAction = (int)action;
                    int crudAction = (int)mat.CrudAction;

                    //can we process mail  this data action
                    if (mat.CrudAction == CrudAction.Any || (dataAction | crudAction) == dataAction)
                    {



                    }
                }

            }

        }

        /// <summary>
        /// Find subscribers to the OU ( or a parent OU ) , that subscribe to this domain and action, and call the webhooks
        /// </summary>
        /// <param name="eb"></param>
        /// <param name="action"></param>
        /// <param name="ouID"></param>
        protected virtual void ProcessApiWebHooks(Guid objectID, ApiObjectType objectType, EventDomain domain, EventAction action, Guid ouID)
        {

            Task.Factory.StartNew(async () =>
            {

                try
                {
                    //get webhooks for this even and fire them off
                    var ouService = ServiceLocator.Current.GetInstance<IOUService>();

                    var webhooks = ouService.GetWebHooks(ouID, domain);

                    foreach (var webhook in webhooks)
                    {
                        ApiEvent apiEvent = new ApiEvent();
                        apiEvent.PrivateKey = webhook.PrivateKey;
                        apiEvent.Action = action;
                        apiEvent.Domain = domain;
                        apiEvent.EventID = Guid.NewGuid();
                        apiEvent.ObjectID = objectID;
                        apiEvent.ObjectType = objectType;
                        apiEvent.TimeStamp = System.DateTime.Today;

                        //todo: make http post
                        using (var client = new HttpClient())
                        {
                            var response = await client.PostAsJsonAsync(webhook.Url, apiEvent);
                            if (!response.IsSuccessStatusCode)
                            {
                                //log error
                            }
                        } //using client
                    } //foreach
                }
                catch (Exception)
                {


                }

            });

        }

        protected Type DataTypeOfPropertyNamed(string propertyName)
        {
            Type ret = null;
            Type t = typeof(T);
            PropertyInfo pi = t.GetProperty(propertyName);

            if (pi != null)
            {
                ret = pi.PropertyType; ;
            }

            return ret;
        }

        protected U RemoveDeletedItems<U>(U item) where U : EntityBase
        {
            if (item == null) return null;

            item.FilterCollections<U>();

            return item;
        }

        protected IList<U> RemoveDeletedItems<U>(IList<U> items) where U : EntityBase
        {
            if (items == null) return null;

            List<U> filteredList = new List<U>();

            foreach (U item in items)
            {
                filteredList.Add(this.RemoveDeletedItems(item));
            }

            return filteredList;
        }

        protected IQueryable<T> ApplyDeletedFilter(bool includeDeletedItems, IQueryable<T> items)
        {
            if (!includeDeletedItems)
                return items.Where(i => !i.IsDeleted);

            return items;
        }
    }
}