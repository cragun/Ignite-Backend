using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.ViewModels
{
    public class UserRegistrationViewModel
    {
        public Guid Guid { get; set; }
        public string Username { get; set; }

        public RegistrationViewModel ToRegistration()
        {
            return new RegistrationViewModel
            {
                Guid = Guid,
                Email = Username,
                ShowFormIfModelHasErrors = false
            };
        }
    }
}