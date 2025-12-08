using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public int client_id { get; set; }
        public Client Client { get; set; } // Получить информацию о клиенте

        public int employee_id { get; set; }
        public Employee Employee { get; set; } // Получить информацию о сотруднике

        public int item_id { get; set; }
        public Item Item { get; set; } // Получить информацию о заложенном товаре
        public string Contract_number { get; set; }
        public decimal Pawn_amount { get; set; }
        public decimal Redemption_amount { get; set; }
        public DateTime Contract_date { get; set; }
        public DateTime Due_date { get; set; }
        public string Contract_status { get; set; }
        public DateTime Created_on { get; set; }
        public ICollection<Extension> Extensions { get; set; } = new List<Extension>();
        public ICollection<Redemption> Redemptions { get; set; } = new List<Redemption>();
    }
}
