using AutoFixture;
using DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard;
using Newtonsoft.Json;
using Xunit;

namespace DataReef.TM.Models.Tests.Solar.FinanceAdapters.SMARTBoard
{

    public class SBProposalDataFixture
    {

        [Fact]
        public void GenerateDummyData()
        {
            var fixture = new Fixture();
            var data = fixture.Create<SBProposalDataModel>();

            var json = JsonConvert.SerializeObject(data);
        }
    }
}
