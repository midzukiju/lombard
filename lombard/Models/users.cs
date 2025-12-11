using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class users
    {
        public int Id { get; set; }
        public string login { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty; // ⚠️ временно, лучше хэш!
        public int role_id { get; set; } // 1 = админ, 2 = оценщик, 3 = клиент

        // Связанные сущности (опционально)
        public clients? Client { get; set; }      // если User — клиент
        public employees? Employee { get; set; }  // если User — сотрудник/оценщик
    }
}
