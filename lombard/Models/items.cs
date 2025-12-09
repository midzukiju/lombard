using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class items
    {
        public long item_id { get; set; }
        public int category_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal estimated_price { get; set; }
        public decimal market_price { get; set; }
        public string image { get; set; }
        public DateTime created_on { get; set; }
        public ICollection<contracts> Contracts { get; set; } = new List<contracts>();
        public ICollection<purchases> Buys { get; set; } = new List<purchases>();
        public ICollection<sales> Sales { get; set; } = new List<sales>();
    }
}
