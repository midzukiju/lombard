using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class clients
    {
        public long client_id { get; set; } // В XAML был Id, но в ItemEditorViewModel, скорее всего, client_id
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string patronymic { get; set; }
        public DateTime? date_of_birth { get; set; }
        public string passport_series { get; set; }
        public string passport_number { get; set; }
        public string passport_issued_by { get; set; }
        public DateTime? passport_issue_date { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int user_id { get; set; }
        public string city { get; set; } = string.Empty;
        public string street { get; set; }
        public int house_number { get; set; }
        public DateTime created_on { get; set; }
        public List<contracts> Contracts { get; set; } = new List<contracts>();
        public List<purchases> Purchases { get; set; } = new List<purchases>();
        public List<sales> Sales { get; set; } = new List<sales>();
    }
}
