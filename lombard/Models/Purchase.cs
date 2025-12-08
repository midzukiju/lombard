using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int item_id { get; set; }
        public Item Item { get; set; }

        public decimal Buy_price { get; set; }
        public DateTime Buy_date { get; set; }
        public int client_id { get; set; }
        public Client Client { get; set; }

        public int buy_by_employee_id { get; set; }
        public Employee Employee { get; set; }

        public DateTime Created_on { get; set; }
    }
}
