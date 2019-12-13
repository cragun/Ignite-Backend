using DataReef.TM.Models.DTOs.Solar.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataReef.TM.Models.Tests.Solar.Finance
{
    public class LoanRequestTests
    {
        [Fact]
        public void TotalAddersWithFinancingFee()
        {
            var loanRequest = new LoanRequest();

            Assert.Equal(loanRequest.TotalAddersCosts, loanRequest.TotalAddersCostsWithFinancingFee);

            var adderItem = new Models.Solar.AdderItem();
            adderItem.Type = Enums.AdderItemType.Adder;
            adderItem.Quantity = 2;
            adderItem.Cost = 100;            
            adderItem.RateType = Enums.AdderItemRateType.Flat;

            var adderItem2 = new Models.Solar.AdderItem();
            adderItem2.Type = Enums.AdderItemType.Adder;
            adderItem2.Quantity = 2;
            adderItem2.Cost = 100;
            adderItem2.RateType = Enums.AdderItemRateType.Flat;

            loanRequest.Adders = new List<Models.Solar.AdderItem> { adderItem, adderItem2 };
            Assert.Equal(400, loanRequest.TotalAddersCostsWithFinancingFee);

            adderItem.FinancingFee = 10;
            adderItem2.FinancingFee = 10;
            Assert.Equal(440, loanRequest.TotalAddersCostsWithFinancingFee);

            //System size
            loanRequest.SystemSize = 3000;
            adderItem.RateType = Enums.AdderItemRateType.PerKw;
            adderItem2.RateType = Enums.AdderItemRateType.PerKw;
            Assert.Equal(1320, loanRequest.TotalAddersCostsWithFinancingFee);
        }
    }
}
