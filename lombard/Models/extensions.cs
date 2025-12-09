using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class extensions
    {
        public long extension_id { get; set; }
        public long contract_id { get; set; }
        public contracts Contract { get; set; }

        public DateTime old_due_date { get; set; }
        public DateTime new_due_date { get; set; }
        public decimal extension_fee { get; set; }
        public long extended_by_employee_id { get; set; }
        public employees Employee { get; set; }

        public DateTime created_on { get; set; }
    }
}
