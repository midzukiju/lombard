using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class interest_rates
    {
        public long rate_id { get; set; }
        public int category_id { get; set; }
        public int min_days { get; set; }
        public int max_days { get; set; }
        public decimal daily_rate_percent { get; set; }
    }
}
