using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Employee
    {
        public int Id { get; set; } // В XAML был Id
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public DateTime created_on { get; set; }
        public List<Contract> Contracts { get; set; } = new List<Contract>();
        public List<Extension> Extensions { get; set; } = new List<Extension>();
        public List<Redemption> Redemptions { get; set; } = new List<Redemption>();
        public List<Purchase> Buys { get; set; } = new List<Purchase>();
        public List<Sale> Sales { get; set; } = new List<Sale>();
    }
}
