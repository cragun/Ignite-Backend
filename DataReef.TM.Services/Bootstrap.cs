using AutoMapper;
using DataReef.Integrations.LoanPal.Models.LoanPal;
using DataReef.TM.Models.FinancialIntegration.LoanPal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Services
{
    public class Bootstrap
    {
        public static void InitAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<LoanPalApplicationRequest, ApplicationRequest>()
                   .ForMember(m => m.TotalSystemCost, options => options.MapFrom(src => src.TotalSystemCost.ToString("C", CultureInfo.CurrentCulture)));

                cfg.CreateMap<LoanPalApplicantModel, ApplicantModel>()
                   .ForMember(a => a.AnnualIncome, options => options.MapFrom(src => src.AnnualIncome.ToString("C", CultureInfo.CurrentCulture)))
                   .ForMember(a => a.DateOfBirth, options => options.MapFrom(src => src.DateOfBirth.ToString("yyyy-MM-dd")));

                cfg.CreateMap<ApplicantModel, LoanPalApplicantModel>();

                cfg.CreateMap<LoanPalAddressModel, AddressModel>();
                cfg.CreateMap<AddressModel, LoanPalAddressModel>();

                cfg.CreateMap<ApplicationResponse, LoanPalApplicationResponse>();

                cfg.CreateMap<LoanPalLoanOptionModel, LoanOptionModel>();
                cfg.CreateMap<LoanOptionModel, LoanPalLoanOptionModel>();

                cfg.CreateMap<LoanPalSalesRepModel, SalesRepModel>();
                cfg.CreateMap<SalesRepModel, LoanPalSalesRepModel>();

                cfg.CreateMap<LoanPalStipulationModel, StipulationModel>();
                cfg.CreateMap<StipulationModel, LoanPalStipulationModel>();
            });
        }
    }
}
