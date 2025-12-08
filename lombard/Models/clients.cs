using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    internal class clients
    {
        public int Id { get; set; } // В XAML был Id, но в ItemEditorViewModel, скорее всего, client_id
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
        public string City { get; set; }
        public string Street { get; set; }
        public int House_Number { get; set; }
        public DateTime Created_on { get; set; }
        public List<contracts> Contracts { get; set; } = new List<contracts>();
        public List<purchases> Buys { get; set; } = new List<purchases>();
        public List<sales> Sales { get; set; } = new List<sales>();
    }
}
