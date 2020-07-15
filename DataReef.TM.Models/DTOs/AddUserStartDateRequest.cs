using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs
{
    public class AddUserStartDateRequest
    {
        public Guid UserId { get; set; }
        public string ApiKey { get; set; }
        public DateTime Date { get; set; }

    }
}
