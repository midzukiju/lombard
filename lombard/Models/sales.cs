using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class sales
    {
        public long sale_id { get; set; }
        public long item_id { get; set; }
        public items Item { get; set; }

        public DateTime sale_date { get; set; }
        public decimal sale_price { get; set; }
        public long client_id { get; set; }
        public clients Client { get; set; }

        public long sold_by_employee_id { get; set; }
        public employees Employee { get; set; }

        public DateTime created_on { get; set; }
    }
}
