using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Request
    {
        public long Id { get; set; }
        public int Service_id { get; set; }
        public string? Requester_last_name { get; set; }
        public string Requester_first_name { get; set; }
        public string Requester_patronymic { get; set; }
        public string Requester_number { get; set; }
        public string Requester_city { get; set; }
        public DateTime Created_on { get; set; }
    }
}
