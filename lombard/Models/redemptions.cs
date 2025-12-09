using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class redemptions
    {
        public long redemption_id { get; set; }
        public long contract_id { get; set; }
        public contracts Contract { get; set; }

        public DateTime redemption_date { get; set; }
        public decimal total_paid { get; set; }
        public long redeemed_by_employee_id { get; set; }
        public employees Employee { get; set; }

        public DateTime created_on { get; set; }
    }
}
