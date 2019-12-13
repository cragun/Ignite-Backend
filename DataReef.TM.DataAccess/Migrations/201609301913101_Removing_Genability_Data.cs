namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Removing_Genability_Data : DbMigration
    {
        public override void Up()
        {
            string query = @"UPDATE Properties
							SET GenabilityProviderAccountID = NULL,
								GenabilityAccountID = NULL

							UPDATE solar.Proposals
							SET GenabilityElectricityProviderProfileID = NULL

							UPDATE solar.Arrays
							SET GenabilitySolarProviderProfileID = NULL

							UPDATE Properties
							SET DeliveryPoint = NULL
							WHERE [Guid] IN
							(
								SELECT [Guid]
								FROM Properties
								WHERE DeliveryPoint IS NOT NULL AND LEN(DeliveryPoint) <> 2
							)

							UPDATE Properties
							SET PlusFour = NULL
							WHERE [Guid] IN
							(
								SELECT [Guid]
								FROM Properties
								WHERE PlusFour IS NOT NULL AND LEN(PlusFour) <> 4
							)";
            Sql(query);
        }

        public override void Down()
        {
        }
    }
}
