using System.Data.Entity.Migrations.Design;
using System.Data.Entity.SqlServer;

namespace DataReef.Core.Infrastructure.DatabaseDesign
{
    public class NonClusteredPrimaryKeyCSharpMigrationCodeGenerator : CSharpMigrationCodeGenerator
    {
        protected override void Generate(System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation addPrimaryKeyOperation, System.Data.Entity.Migrations.Utilities.IndentedTextWriter writer)
        {



            if (addPrimaryKeyOperation.Name.Contains("MigrationHistory"))
            {
                addPrimaryKeyOperation.IsClustered = true;
                base.Generate(addPrimaryKeyOperation, writer);
            }
            else
            {
                addPrimaryKeyOperation.IsClustered = false;
                base.Generate(addPrimaryKeyOperation, writer);
            }

        }
        protected override void GenerateInline(System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation addPrimaryKeyOperation, System.Data.Entity.Migrations.Utilities.IndentedTextWriter writer)
        {
            if (addPrimaryKeyOperation.Name.Contains("MigrationHistory"))
            {
                addPrimaryKeyOperation.IsClustered = true;
                base.GenerateInline(addPrimaryKeyOperation, writer);
            }
            else
            {
                addPrimaryKeyOperation.IsClustered = false;
                base.GenerateInline(addPrimaryKeyOperation, writer);
            }
        }

        protected override void Generate(System.Data.Entity.Migrations.Model.CreateTableOperation createTableOperation, System.Data.Entity.Migrations.Utilities.IndentedTextWriter writer)
        {

            if (createTableOperation.Name.Contains("MigrationHistory"))
            {
                createTableOperation.PrimaryKey.IsClustered = true;
                base.Generate(createTableOperation, writer);
            }
            else
            {
                createTableOperation.PrimaryKey.IsClustered = false;
                base.Generate(createTableOperation, writer);
            }

        }

        protected override void Generate(System.Data.Entity.Migrations.Model.MoveTableOperation moveTableOperation, System.Data.Entity.Migrations.Utilities.IndentedTextWriter writer)
        {

            if (moveTableOperation.Name.Contains("MigrationHistory"))
            {
                moveTableOperation.CreateTableOperation.PrimaryKey.IsClustered = true;
                base.Generate(moveTableOperation, writer);
            }
            else
            {
                moveTableOperation.CreateTableOperation.PrimaryKey.IsClustered = false;
                base.Generate(moveTableOperation, writer);
            }
        }
    }
    public class NonClusteredPrimaryKeySqlMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void Generate(System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation addPrimaryKeyOperation)
        {
            if (addPrimaryKeyOperation.Name.Contains("MigrationHistory"))
            {
                addPrimaryKeyOperation.IsClustered = true;
                base.Generate(addPrimaryKeyOperation);
            }
            else
            {
                addPrimaryKeyOperation.IsClustered = false;
                base.Generate(addPrimaryKeyOperation);
            }

        }

        protected override void Generate(System.Data.Entity.Migrations.Model.CreateTableOperation createTableOperation)
        {

            if (createTableOperation.Name.Contains("MigrationHistory"))
            {
                createTableOperation.PrimaryKey.IsClustered = true;
                base.Generate(createTableOperation);
            }
            else
            {
                createTableOperation.PrimaryKey.IsClustered = false;
                base.Generate(createTableOperation);
            }
        }

        protected override void Generate(System.Data.Entity.Migrations.Model.MoveTableOperation moveTableOperation)
        {
            if (moveTableOperation.Name.Contains("MigrationHistory"))
            {
                moveTableOperation.CreateTableOperation.PrimaryKey.IsClustered = true;
                base.Generate(moveTableOperation);
            }
            else
            {
                moveTableOperation.CreateTableOperation.PrimaryKey.IsClustered = false;
                base.Generate(moveTableOperation);
            }


        }
    }
}
