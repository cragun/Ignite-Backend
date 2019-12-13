using System;
using System.Linq;
using DataReef.TM.Models;

namespace DataReef.TM.ClientApi.Models
{
    public class PersonDataView
    {
        public PersonDataView(Person person)
        {
            if (person == null)
                return;

            Id = person.Guid;
            FirstName = person.FirstName;
            LastName = person.LastName;
            EmailAddress = person.EmailAddressString;
            PhoneNumber = person.PhoneNumbers != null && person.PhoneNumbers.Any()
                ? person.PhoneNumbers.First().Number
                : null;
            IsActive = person.User?.IsActive ?? false;
        }

        public PersonDataView()
        {
        }

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        
        
    }
}