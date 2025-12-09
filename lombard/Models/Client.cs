using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    public class Client
    {
        public long Id { get; set; } // В XAML был Id, но в ItemEditorViewModel, скорее всего, client_id
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string PassportIssuedBy { get; set; }
        public DateTime? PassportIssueDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Street { get; set; }
        public int House_Number { get; set; }
        public DateTime Created_on { get; set; }
        public List<Contract> Contracts { get; set; } = new List<Contract>();
        public List<Purchase> Purchases { get; set; } = new List<Purchase>();
        public List<Sale> Sales { get; set; } = new List<Sale>();
    }
}
