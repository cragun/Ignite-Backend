using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalBasicInfo
    {
        public long ProposalNumber { get; set; }
        public DateTime Date { get; set; }

        public ProposalCustomer Customer { get; set; }

        public ProposalSalesRep SalesRep { get; set; }

        public string CompanyLogoUrl { get; set; }

        public ProposalBasicInfo()
        {
            Customer = new ProposalCustomer();
            SalesRep = new ProposalSalesRep();
        }

        public ProposalBasicInfo(Proposal proposal, string dealerName, DateTime date)
        {
            Date = date;
            ProposalNumber = proposal.Id;

            Customer = new ProposalCustomer(proposal);
            SalesRep = new ProposalSalesRep(proposal, dealerName);
        }
    }

    public class ProposalCustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string UtilityCompany { get; set; }

        public ProposalAddress Address { get; set; }

        public ProposalCustomer()
        {
            Address = new ProposalAddress();
        }

        public ProposalCustomer(Proposal proposal)
        {
            Address = new ProposalAddress(proposal);
            var mainOccupant = proposal.Property.GetMainOccupant();
            if (mainOccupant != null)
            {
                FirstName = mainOccupant.FirstName;
                LastName = mainOccupant.LastName;
            }

            FullName = proposal.NameOfOwner ?? $"{FirstName} {LastName}".Trim();

            EmailAddress = proposal?.Property?.PropertyBag?.FirstOrDefault(f => f.DisplayName == "Email Address")?.Value;
            PhoneNumber = proposal?.Property?.PropertyBag?.FirstOrDefault(f => f.DisplayName == "Phone Number")?.Value;
            UtilityCompany = proposal?.Tariff?.UtilityName;
        }
    }

    public class ProposalAddress
    {
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string FormattedAddress => $"{Address1}, {City}, {State} {ZipCode}";

        public ProposalAddress() { }

        public ProposalAddress(Proposal proposal)
        {
            City = proposal.City;
            State = proposal.State;
            ZipCode = proposal.ZipCode;
            Address1 = proposal.Address;
            Address2 = proposal.Address2;
        }
    }

    public class ProposalSalesRep
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DealerName { get; set; }

        public ProposalSalesRep() { }

        public ProposalSalesRep(Proposal proposal, string dealerName)
        {
            DealerName = dealerName;

            if (proposal.SalesRep == null)
            {
                return;
            }
            Name = $"{proposal.SalesRep.FirstName} {proposal.SalesRep.MiddleName} {proposal.SalesRep.LastName}".Replace("  ", " ");
            PhoneNumber = proposal.SalesRep.PhoneNumbers?.Any(x => x.IsPrimary) == true ?
                proposal.SalesRep.PhoneNumbers?.FirstOrDefault(x => x.IsPrimary)?.Number :
                proposal.SalesRep.PhoneNumbers?.FirstOrDefault()?.Number
                ?? string.Empty;
            Email = proposal.SalesRep.EmailAddresses?.FirstOrDefault() ?? string.Empty;
        }
    }
}
