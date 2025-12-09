using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Sale
    {
        public long Id { get; set; }
        public long item_id { get; set; }
        public Item Item { get; set; }

        public DateTime Sale_date { get; set; }
        public decimal Sale_price { get; set; }
        public long client_id { get; set; }
        public Client Client { get; set; }

        public long sold_by_employee_id { get; set; }
        public Employee Employee { get; set; }

        public DateTime Created_on { get; set; }
    }
}
