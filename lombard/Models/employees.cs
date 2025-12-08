using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    internal class employees
    {
        public int Id { get; set; } // В XAML был Id
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public DateTime created_on { get; set; }
        public List<contracts> Contracts { get; set; } = new List<contracts>();
        public List<extensions> Extensions { get; set; } = new List<extensions>();
        public List<redemptions> Redemptions { get; set; } = new List<redemptions>();
        public List<purchases> Buys { get; set; } = new List<purchases>();
        public List<sales> Sales { get; set; } = new List<sales>();
    }
}
