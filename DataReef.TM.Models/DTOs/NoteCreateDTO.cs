using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs
{
    public class NoteCreateDTO
    {
        public List<string> userId { get; set; }

        public List<string> apiKey { get; set; }
    }
}