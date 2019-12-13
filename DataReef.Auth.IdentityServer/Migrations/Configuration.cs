using System;
using System.Data.Entity.Migrations;
using DataReef.Auth.Core.Models;
using DataReef.Auth.IdentityServer.Helpers;
using DataReef.Auth.IdentityServer.DataAccess;
using System.Collections.Generic;

namespace DataReef.Auth.IdentityServer.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(DataContext context)
        {
            SeedDevelopment(context);
        }

        private void SeedDevelopment(DataContext context)
        {
            IList<User> users = new List<User>()
            {
                new User()
                {
                    UserId = Guid.Parse("821413B1-7395-4789-994A-5F560DB9F53F"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "LLee@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    },
                },
                new User()
                {
                    UserId = Guid.Parse("8F13D447-C06D-468C-B08F-7D72E6F39D30"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "KHolden@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("C764E9A1-7DF4-4220-9101-FBB8C33C1B8D"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "HHoldaway@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("6B838C65-31D1-491D-B317-888D70FF9F3C"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "MPecora@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("03279163-D27B-40D5-B03E-2DFDF2107D8F"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "JScoville@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("C3D38AEF-EA7C-4DAF-A0E9-C42D246A4C0E"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "LLuz@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("06C1B73E-8D44-428E-918C-5C7576688870"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "BBitter@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("5A921E04-AE07-44B9-94DE-9553E681FDDF"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "SLawson@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("CB98A54D-ED67-4C35-BD60-F0577FFFCDF1"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "LBartolo@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("78130486-D617-4878-B8F2-D36BEB500FB3"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "AAdams@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("3F170442-9852-4A56-A9FC-FE3C8DF30CD8"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "JTesoro@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("1EF1FB4E-3DF1-4A78-8189-B3CB7E7659AE"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "JDelamba@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("14B3CF1B-FBCA-44FC-855D-98917BBEE403"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "MDetorielo@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("93C782D5-8551-4035-B2CE-B16389AE3224"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "JEmber@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("39ACA397-C4F8-4F6A-BC44-6DBCB88D9E34"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "JGuzman@smart.care",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                },
                new User()
                {
                    UserId = Guid.Parse("FDBD9EB0-972E-4E43-8294-BD64D501F779"),
                    OuId = Guid.Parse("5FFAABFE-E767-4C32-BC26-326139650616"),
                    TenantId = 1,
                    OuName = "Provo",
                    Credentials = new List<Credential>()
                    {
                        new Credential()
                        {
                            Username = "Ginger.Faulkner@hotmail.com",
                            Salt = CryptographyHelper.GenerateSalt()
                        },
                        new Credential()
                        {
                            Username = "8018675309",
                            Salt = CryptographyHelper.GenerateSalt()
                        }
                    }
                }
            };

            for (int i = 0; i < users.Count; i++)
            {
                if (i != users.Count - 1)
                {
                    users[i].Credentials[0].Password = CryptographyHelper.ComputePasswordHash("password" + (i + 1).ToString(), users[i].Credentials[0].Salt);
                }
                else
                {
                    // Last user is special because (it also has a pin credential)
                    users[i].Credentials[0].Password = CryptographyHelper.ComputePasswordHash("password20", users[i].Credentials[0].Salt);
                    users[i].Credentials[1].Password = CryptographyHelper.ComputePasswordHash("53272", users[i].Credentials[1].Salt);
                }
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
