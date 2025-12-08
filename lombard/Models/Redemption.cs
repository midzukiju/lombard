using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Redemption
    {
        public int Id { get; set; }
        public int contract_id { get; set; }
        public Contract Contract { get; set; }

        public DateTime Redemption_date { get; set; }
        public decimal Total_paid { get; set; }
        public int redeemed_by_employee_id { get; set; }
        public Employee Employee { get; set; }

        public DateTime Created_on { get; set; }
    }
}
