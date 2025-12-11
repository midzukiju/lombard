using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace lombard.Models
{
    // ⚠️ Рекомендую сделать public, если используешь DI или вне текущей сборки
    public class AppDbContext : DbContext
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
        public DbSet<users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString =
                    "Server=tompsons.beget.tech;" +
                    "Port=3306;" +
                    "Database=tompsons_stud03;" +
                    "User=tompsons_stud03;" +
                    "Password=10230901Sd;" +
                    "SslMode=Preferred;" +
                    "ConvertZeroDateTime=True;" +
                    "Connection Timeout=30;";

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =======================================================
            // 1. Определение первичных ключей (для нестандартных имен)
            // =======================================================
            modelBuilder.Entity<clients>().HasKey(c => c.client_id);
            modelBuilder.Entity<items>().HasKey(i => i.item_id);
            modelBuilder.Entity<employees>().HasKey(e => e.employee_id);
            modelBuilder.Entity<users>().HasKey(u => u.Id);

            // =======================================================
            // 2. Связи "ОДИН-ко-МНОГИМ" (One-to-Many)
            // =======================================================

            // ------------------------------------
            // 2.1. СВЯЗИ КЛИЕНТА (Client 1 -> Many)
            // ------------------------------------
            modelBuilder.Entity<contracts>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.client_id);

            modelBuilder.Entity<purchases>()
                .HasOne(b => b.Client)
                .WithMany(cl => cl.Purchases)
                .HasForeignKey(b => b.client_id);

            modelBuilder.Entity<sales>()
                .HasOne(s => s.Client)
                .WithMany(cl => cl.Sales)
                .HasForeignKey(s => s.client_id);

            // ------------------------------------
            // 2.2. СВЯЗИ СОТРУДНИКА (Employee 1 -> Many)
            // ------------------------------------
            modelBuilder.Entity<contracts>()
                .HasOne(c => c.Employee)
                .WithMany(e => e.Contracts)
                .HasForeignKey(c => c.employee_id);

            modelBuilder.Entity<extensions>()
                .HasOne(ext => ext.Employee)
                .WithMany(e => e.Extensions)
                .HasForeignKey(ext => ext.extended_by_employee_id);

            modelBuilder.Entity<redemptions>()
                .HasOne(r => r.Employee)
                .WithMany(e => e.Redemptions)
                .HasForeignKey(r => r.redeemed_by_employee_id);

            modelBuilder.Entity<purchases>()
                .HasOne(b => b.Employee)
                .WithMany(e => e.Buys)
                .HasForeignKey(b => b.buy_by_employee_id);

            modelBuilder.Entity<sales>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Sales)
                .HasForeignKey(s => s.sold_by_employee_id);

            // ------------------------------------
            // 2.3. СВЯЗИ ТОВАРА (Item 1 -> Many)
            // ------------------------------------
            modelBuilder.Entity<contracts>()
                .HasOne(c => c.Item)
                .WithMany(i => i.Contracts)
                .HasForeignKey(c => c.item_id);

            modelBuilder.Entity<purchases>()
                .HasOne(b => b.Item)
                .WithMany(i => i.Buys)
                .HasForeignKey(b => b.item_id);

            modelBuilder.Entity<sales>()
                .HasOne(s => s.Item)
                .WithMany(i => i.Sales)
                .HasForeignKey(s => s.item_id);

            // ------------------------------------
            // 2.4. СВЯЗИ КОНТРАКТА (Contract 1 -> Many)
            // ------------------------------------
            modelBuilder.Entity<extensions>()
                .HasOne(ext => ext.Contract)
                .WithMany(c => c.Extensions)
                .HasForeignKey(ext => ext.contract_id);

            modelBuilder.Entity<redemptions>()
                .HasOne(r => r.Contract)
                .WithMany(c => c.Redemptions)
                .HasForeignKey(r => r.contract_id);

            // =======================================================
            // 3. СВЯЗИ "ОДИН-К-ОДНОМУ" (User 1 <-> 1 Client / Employee)
            // =======================================================

            // User (1) <-> Client (1)
            modelBuilder.Entity<users>()
                .HasOne(u => u.Client)
                .WithOne(c => c.User)
                .HasForeignKey<clients>(c => c.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            // User (1) <-> Employee (1)
            modelBuilder.Entity<users>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<employees>(e => e.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}