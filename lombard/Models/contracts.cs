using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class contracts
    {
        public long contract_id { get; set; }
        public long client_id { get; set; }
        public clients Client { get; set; } // Получить информацию о клиенте

        public long employee_id { get; set; }
        public employees Employee { get; set; } // Получить информацию о сотруднике

        public long item_id { get; set; }
        public items Item { get; set; } // Получить информацию о заложенном товаре
        public int contract_number { get; set; }
        public decimal pawn_amount { get; set; }
        public decimal redemption_amount { get; set; }
        public DateTime contract_date { get; set; }
        public DateTime due_date { get; set; }
        public long status_id { get; set; }
        public DateTime created_on { get; set; }
        public ICollection<extensions> Extensions { get; set; } = new List<extensions>();
        public ICollection<redemptions> Redemptions { get; set; } = new List<redemptions>();
    }
}
