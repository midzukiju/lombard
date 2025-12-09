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
        public DbSet<clients> Clients { get; set; }
        public DbSet<employees> Employees { get; set; }
        public DbSet<items> Items { get; set; }
        public DbSet<contracts> Contracts { get; set; }

        // Таблицы транзакций
        public DbSet<extensions> Extensions { get; set; }
        public DbSet<redemptions> Redemptions { get; set; }
        public DbSet<purchases> Purchases { get; set; }
        public DbSet<sales> Sales { get; set; }
        public DbSet<Request> Requests { get; set; }

        // Вспомогательные таблицы
        public DbSet<interest_rates> Interest_rates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString =
                "Server=tompsons.beget.tech;" +
                "Port=3306;" +
                "Database=tompsons_stud03;" +
                "User=tompsons_stud03;" +
                $"Password=10230901Sd;" +
                "SslMode=Preferred;" +
                "ConvertZeroDateTime = True;" + 
                "Connection Timeout=30;"; ;

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =======================================================
            // 1. Определение первичных ключей (для нестандартных имен)
            // =======================================================
            modelBuilder.Entity<clients>().HasKey(c => c.client_id);
            modelBuilder.Entity<items>().HasKey(i => i.item_id);
            modelBuilder.Entity<employees>().HasKey(e => e.employee_id);

            // =======================================================
            // 2. Связи "ОДИН-ко-МНОГИМ" (One-to-Many)
            // =======================================================

            // ------------------------------------
            // 2.1. СВЯЗИ КЛИЕНТА (Client 1 -> Many)
            // ------------------------------------

            // Client (1) -> Contract (M)
            modelBuilder.Entity<contracts>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.client_id);

            // Client (1) -> Buy (M)
            modelBuilder.Entity<purchases>()
                .HasOne(b => b.Client)
                .WithMany(cl => cl.Purchases)
                .HasForeignKey(b => b.client_id);

            // Client (1) -> Sale (M)
            modelBuilder.Entity<sales>()
                .HasOne(s => s.Client)
                .WithMany(cl => cl.Sales)
                .HasForeignKey(s => s.client_id);

            // ------------------------------------
            // 2.2. СВЯЗИ СОТРУДНИКА (Employee 1 -> Many)
            // ------------------------------------

            // Employee (1) -> Contract (M)
            modelBuilder.Entity<contracts>()
                .HasOne(c => c.Employee)
                .WithMany(e => e.Contracts)
                .HasForeignKey(c => c.employee_id);

            // Employee (1) -> Extension (M) (сотрудник, оформивший продление)
            modelBuilder.Entity<extensions>()
                .HasOne(ext => ext.Employee)
                .WithMany(e => e.Extensions)
                .HasForeignKey(ext => ext.extended_by_employee_id);

            // Employee (1) -> Redemption (M) (сотрудник, принявший платеж)
            modelBuilder.Entity<redemptions>()
                .HasOne(r => r.Employee)
                .WithMany(e => e.Redemptions)
                .HasForeignKey(r => r.redeemed_by_employee_id);

            // Employee (1) -> Buy (M) (сотрудник, оформивший покупку)
            modelBuilder.Entity<purchases>()
                .HasOne(b => b.Employee)
                .WithMany(e => e.Buys)
                .HasForeignKey(b => b.buy_by_employee_id);

            // Employee (1) -> Sale (M) (сотрудник, оформивший продажу)
            modelBuilder.Entity<sales>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Sales)
                .HasForeignKey(s => s.sold_by_employee_id);

            // ------------------------------------
            // 2.3. СВЯЗИ ТОВАРА (Item 1 -> Many)
            // ------------------------------------

            // Item (1) -> Contract (M)
            modelBuilder.Entity<contracts>()
                .HasOne(c => c.Item)
                .WithMany(i => i.Contracts)
                .HasForeignKey(c => c.item_id);

            // Item (1) -> Buy (M)
            modelBuilder.Entity<purchases>()
                .HasOne(b => b.Item)
                .WithMany(i => i.Buys)
                .HasForeignKey(b => b.item_id);

            // Item (1) -> Sale (M)
            modelBuilder.Entity<sales>()
                .HasOne(s => s.Item)
                .WithMany(i => i.Sales)
                .HasForeignKey(s => s.item_id);

            // ------------------------------------
            // 2.4. СВЯЗИ КОНТРАКТА (Contract 1 -> Many)
            // ------------------------------------

            // Contract (1) -> Extension (M)
            modelBuilder.Entity<extensions>()
                .HasOne(ext => ext.Contract)
                .WithMany(c => c.Extensions)
                .HasForeignKey(ext => ext.contract_id);

            // Contract (1) -> Redemption (M)
            modelBuilder.Entity<redemptions>()
                .HasOne(r => r.Contract)
                .WithMany(c => c.Redemptions)
                .HasForeignKey(r => r.contract_id);

            base.OnModelCreating(modelBuilder);
        }

    }
}
