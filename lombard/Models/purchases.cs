using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class purchases
    {
        public long purchase_id { get; set; }
        public long item_id { get; set; }
        public items Item { get; set; }

        public decimal buy_price { get; set; }
        public DateTime buy_date { get; set; }
        public long client_id { get; set; }
        public clients Client { get; set; }

        public long buy_by_employee_id { get; set; }
        public employees Employee { get; set; }

        public DateTime created_on { get; set; }
    }
}
