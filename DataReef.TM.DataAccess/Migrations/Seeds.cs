using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.DataAccess.Migrations.Seed;
using DataReef.TM.Models;
using DataReef.Core.Attributes;
using System.Reflection;
using System.Resources;
using System.Globalization;
using System.Collections;
using System.Configuration;
using DataReef.TM.DataAccess.Properties;


namespace DataReef.TM.DataAccess.Migrations
{
    public class Seeds
    {
        public static void SeedDevelopment(DataContext context)
        {
           // CreateStoredProcedures(context);

            //SeedFromExcel(context);
        }

        public static void PostProcess(DataContext context)
        {
            CreateStoredProcedures(context);
        }

        internal static void CreateCascadingSoftDeleteTriggers(DataContext context)
        {
           
            var sets =
            from p in typeof(DataContext).GetProperties()
            where p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
            let entityType = p.PropertyType.GetGenericArguments().First()
            where typeof(EntityBase).IsAssignableFrom(entityType) && entityType.IsAbstract == false
            select p;

            foreach (PropertyInfo pi in sets)
            {
                Type type = pi.PropertyType.GetGenericArguments().FirstOrDefault();
                if (type != null)
                {
                    List<CascadeSoftDeleteAttribute> attributes = type.GetCustomAttributes<CascadeSoftDeleteAttribute>().ToList();

                    if (attributes.Any())
                    {
                        try
                        {
                            foreach (CascadeSoftDeleteAttribute att in attributes)
                            {
                                string triggerName = string.Format("tg{0}_SoftDelete_{1}", att.PrimaryKeyTableName, att.ForeignKeyTableName);
                                System.Text.StringBuilder sb = new StringBuilder();
                                sb.AppendLine(string.Format("if exists (select * from sys.triggers where name='{0}') begin drop trigger {0} end ", triggerName));
                                context.Database.ExecuteSqlCommand(sb.ToString());
                                sb = new StringBuilder();
                                sb.AppendLine(string.Format("create trigger {0} on dbo.{1} After Update as begin Update {2} Set IsDeleted = Inserted.IsDeleted From Inserted,{2} where Inserted.Guid = {2}.{3} end", triggerName, att.PrimaryKeyTableName, att.ForeignKeyTableName, att.ForeignKeyFieldName));
                                context.Database.ExecuteSqlCommand(sb.ToString());
                            }
                        }
                        catch (System.Exception ex)
                        {
                            int x = 0;
                        }
                    }

                }
            }
        }

        internal static void CreateStoredProcedures(DataContext context)
        {

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            ResourceSet resourceSet = Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                // stored procedures
                if (entry.Value is string && entry.Key.ToString().StartsWith("proc_"))
                {
                    try
                    {
                        string sql = string.Format("if exists(select * from sys.objects where type = 'p' and name = '{0}') Begin Drop Procedure {0} END", entry.Key);
                        context.Database.ExecuteSqlCommand(sql);
                        context.Database.ExecuteSqlCommand(entry.Value.ToString());
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                // triggers
                if (entry.Value is string && entry.Key.ToString().StartsWith("tg_"))
                {
                    try
                    {
                        string sql = string.Format("if exists(select * from sys.objects where type = 'TR' and name = '{0}') Begin Drop TRIGGER {0} END", entry.Key);
                        context.Database.ExecuteSqlCommand(sql);
                        context.Database.ExecuteSqlCommand(entry.Value.ToString());
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

        }

        private static void SeedFromExcel(DataContext context)
        {
            try
            {
                var resourceName = "DataReef.TM.DataAccess.App_Data.SeedData.xlsm";

                SeedLogger.Info("Start seed from Excel file : " + resourceName);

                var connectionStringName = ConfigurationManager.AppSettings["ConnectionStringName"];
                var connectionStrimg = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                var seed = new DevelopmentSeed(connectionStrimg, resourceName);

                seed.ImportSeedData();
            }
            catch (Exception ex)
            {
                //Log exception 
                SeedLogger.Error(ex);
            }
        }
    }

}