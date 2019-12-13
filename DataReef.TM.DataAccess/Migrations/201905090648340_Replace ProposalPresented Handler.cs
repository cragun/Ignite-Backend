namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplaceProposalPresentedHandler : DbMigration
    {
        public override void Up()
        {
            var sqlQuery = @"UPDATE [OUSettings]
                  SET[Value] = replace([Value], '{  ""EventSource"": ""Inquiry"",  """"EventAction"": 1,  ""HandlerClassFullName"": ""DataReef.TM.Services.InternalServices.Settings.EventHandlers.InquiryEventHandler"",  ""Conditions"": [{    ""Name"": ""Disposition"",    ""Operator"": "" = "",    ""Value"": ""ProposalPresented""  }]}', '{  ""EventSource"": ""Inquiry"",  ""EventAction"": 1,  ""HandlerClassFullName"": ""DataReef.TM.Services.InternalServices.Settings.EventHandlers.InquiryOverrideEnergyConsultantEventHandler"",  ""Conditions"": [{    ""Name"": ""Disposition"",    ""Operator"": "" = "",    ""Value"": ""ProposalPresented""  }]}')
                  WHERE [Name] = 'Legion.Internal.EventMessage.Handlers'";

            Sql(sqlQuery);
        }
        
        public override void Down()
        {
            var sqlQuery = @"UPDATE [OUSettings]
                    SET [Value] = replace([Value], ', {  ""EventSource"": ""Inquiry"",  ""EventAction"": 1,  ""HandlerClassFullName"": ""DataReef.TM.Services.InternalServices.Settings.EventHandlers.InquiryOverrideEnergyConsultantEventHandler"",  ""Conditions"": [{    ""Name"": ""Disposition"",    ""Operator"": "" = "",    ""Value"": ""ProposalPresented""  }]}', '')
                    WHERE [Name] = 'Legion.Internal.EventMessage.Handlers'";

            Sql(sqlQuery);
        }
    }
}
