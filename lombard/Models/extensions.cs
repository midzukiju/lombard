using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    internal class extensions
    {
        public int Id { get; set; }
        public int contract_id { get; set; }
        public Contract Contract { get; set; }

        public DateTime Old_due_date { get; set; }
        public DateTime New_due_date { get; set; }
        public decimal Extension_fee { get; set; }
        public int Extended_by_employee_id { get; set; }
        public Employee Employee { get; set; }

        public DateTime Created_on { get; set; }
    }
}
