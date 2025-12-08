using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Interest_rate
    {
        public int Id { get; set; }
        public int Category_id { get; set; }
        public int Min_days { get; set; }
        public int Max_days { get; set; }
        public decimal Daily_rate_percent { get; set; }
    }
}
