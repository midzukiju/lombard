using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace lombard.Models
{
    internal class AppDbContext : DbContext
    {
        // Основные таблицы
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        // Таблицы транзакций
        public DbSet<Extension> Extensions { get; set; }
        public DbSet<Redemption> Redemptions { get; set; }
        public DbSet<Buy> Buys { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Request> Requests { get; set; }

        // Вспомогательные таблицы
        public DbSet<Rate> Rates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer(
                "Server=https://lolek.beget.com/phpMyAdmin/sql.php?server=1&db=tompsons_stud03&table=request&pos=0;Database=Lombard;;Trusted_Connection=True;"
            );
        }
    }
}
