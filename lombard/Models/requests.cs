using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Request
    {
        public long request_id { get; set; }
        public int service_id { get; set; }
        public string? requester_last_name { get; set; }
        public string requester_first_name { get; set; }
        public string requester_patronymic { get; set; }
        public string requester_number { get; set; }
        public string requester_city { get; set; }
        public DateTime created_on { get; set; }
    }
}
