using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lombard.Models
{
    internal class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
        {
        }
        public AppDbContext()
        {
        }
        // Основные таблицы
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        // Таблицы транзакций
        public DbSet<Extension> Extensions { get; set; }
        public DbSet<Redemption> Redemptions { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Request> Requests { get; set; }

        // Вспомогательные таблицы
        public DbSet<Interest_rate> Interest_rates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString =
                "Server=tompsons.beget.tech;" +
                "Port=3306;" +
                "Database=tompsons_stud03;" +
                "User=tompsons_stud03;" +
                $"Password=10230901Sd;" +
                "SslMode=Preferred;" +
                "Connection Timeout=30;"; ;

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =======================================================
            // 1. Определение первичных ключей (для нестандартных имен)
            // =======================================================
            modelBuilder.Entity<Client>().HasKey(c => c.Id);
            modelBuilder.Entity<Item>().HasKey(i => i.Id);
            modelBuilder.Entity<Employee>().HasKey(e => e.Id);

            // =======================================================
            // 2. Связи "ОДИН-ко-МНОГИМ" (One-to-Many)
            // =======================================================

            // ------------------------------------
            // 2.1. СВЯЗИ КЛИЕНТА (Client 1 -> Many)
            // ------------------------------------

            // Client (1) -> Contract (M)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.client_id);

            // Client (1) -> Buy (M)
            modelBuilder.Entity<Purchase>()
                .HasOne(b => b.Client)
                .WithMany(cl => cl.Purchases)
                .HasForeignKey(b => b.client_id);

            // Client (1) -> Sale (M)
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Client)
                .WithMany(cl => cl.Sales)
                .HasForeignKey(s => s.client_id);

            // ------------------------------------
            // 2.2. СВЯЗИ СОТРУДНИКА (Employee 1 -> Many)
            // ------------------------------------

            // Employee (1) -> Contract (M)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Employee)
                .WithMany(e => e.Contracts)
                .HasForeignKey(c => c.employee_id);

            // Employee (1) -> Extension (M) (сотрудник, оформивший продление)
            modelBuilder.Entity<Extension>()
                .HasOne(ext => ext.Employee)
                .WithMany(e => e.Extensions)
                .HasForeignKey(ext => ext.Extended_by_employee_id);

            // Employee (1) -> Redemption (M) (сотрудник, принявший платеж)
            modelBuilder.Entity<Redemption>()
                .HasOne(r => r.Employee)
                .WithMany(e => e.Redemptions)
                .HasForeignKey(r => r.redeemed_by_employee_id);

            // Employee (1) -> Buy (M) (сотрудник, оформивший покупку)
            modelBuilder.Entity<Purchase>()
                .HasOne(b => b.Employee)
                .WithMany(e => e.Buys)
                .HasForeignKey(b => b.buy_by_employee_id);

            // Employee (1) -> Sale (M) (сотрудник, оформивший продажу)
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Sales)
                .HasForeignKey(s => s.sold_by_employee_id);

            // ------------------------------------
            // 2.3. СВЯЗИ ТОВАРА (Item 1 -> Many)
            // ------------------------------------

            // Item (1) -> Contract (M)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Item)
                .WithMany(i => i.Contracts)
                .HasForeignKey(c => c.item_id);

            // Item (1) -> Buy (M)
            modelBuilder.Entity<Purchase>()
                .HasOne(b => b.Item)
                .WithMany(i => i.Buys)
                .HasForeignKey(b => b.item_id);

            // Item (1) -> Sale (M)
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Item)
                .WithMany(i => i.Sales)
                .HasForeignKey(s => s.item_id);

            // ------------------------------------
            // 2.4. СВЯЗИ КОНТРАКТА (Contract 1 -> Many)
            // ------------------------------------

            // Contract (1) -> Extension (M)
            modelBuilder.Entity<Extension>()
                .HasOne(ext => ext.Contract)
                .WithMany(c => c.Extensions)
                .HasForeignKey(ext => ext.contract_id);

            // Contract (1) -> Redemption (M)
            modelBuilder.Entity<Redemption>()
                .HasOne(r => r.Contract)
                .WithMany(c => c.Redemptions)
                .HasForeignKey(r => r.contract_id);

            base.OnModelCreating(modelBuilder);
        }

    }
}
