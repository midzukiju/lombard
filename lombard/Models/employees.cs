using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class employees
    {
        public long employee_id { get; set; } // В XAML был Id
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string patronymic { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int user_id { get; set; }
        public DateTime created_on { get; set; }
        public List<contracts> Contracts { get; set; } = new List<contracts>();
        public List<extensions> Extensions { get; set; } = new List<extensions>();
        public List<redemptions> Redemptions { get; set; } = new List<redemptions>();
        public List<purchases> Buys { get; set; } = new List<purchases>();
        public List<sales> Sales { get; set; } = new List<sales>();
    }
}
