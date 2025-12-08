using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    internal class items
    {
        public int Id { get; set; }
        public int item_category_id { get; set; }
        public string item_name { get; set; }
        public string item_description { get; set; }
        public decimal item_estimated_price { get; set; }
        public decimal item_market_price { get; set; }
        public string item_image { get; set; }
        public DateTime created_on { get; set; }
        public ICollection<contracts> Contracts { get; set; } = new List<contracts>();
        public ICollection<purchases> Buys { get; set; } = new List<purchases>();
        public ICollection<sales> Sales { get; set; } = new List<sales>();
    }
}
