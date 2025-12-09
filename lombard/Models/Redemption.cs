using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Redemption
    {
        public long Id { get; set; }
        public long contract_id { get; set; }
        public Contract Contract { get; set; }

        public DateTime Redemption_date { get; set; }
        public decimal Total_paid { get; set; }
        public long redeemed_by_employee_id { get; set; }
        public Employee Employee { get; set; }

        public DateTime Created_on { get; set; }
    }
}
