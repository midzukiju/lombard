using lombard.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace lombard
{
    public partial class Admin : Window
    {
        private const string ConnectionString =
            "Server=tompsons.beget.tech;Port=3306;Database=tompsons_stud03;User=tompsons_stud03;Password=10230901Sd;SslMode=Preferred;";

        private string _currentTable = "clients";

        // Храним текущие списки, чтобы не терять данные при добавлении
        private List<Client> _clients;
        private List<Employee> _employees;
        private List<Item> _items;
        private List<Interest_rate> _rates;
        private List<Contract> _contracts;
        private List<Extension> _extensions;
        private List<Redemption> _redemptions;
        private List<Purchase> _purchases;
        private List<Sale> _sales;

        public Admin()
        {
            InitializeComponent();
            LoadClients();
        }

        // ==============================
        // ЗАГРУЗКА ТАБЛИЦ
        // ==============================
        private void LoadClients()
        {
            _clients = new List<Client>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT client_id, last_name, first_name, patronymic, date_of_birth,
                       passport_series, passport_number, passport_issued_by, passport_issue_date,
                       phone, email, city, street, house_number, user_id, created_on
                FROM clients", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _clients.Add(new Client
                {
                    Id = reader.GetInt64("client_id"),
                    LastName = reader.IsDBNull("last_name") ? "" : reader.GetString("last_name"),
                    FirstName = reader.IsDBNull("first_name") ? "" : reader.GetString("first_name"),
                    Patronymic = reader.IsDBNull("patronymic") ? "" : reader.GetString("patronymic"),
                    DateOfBirth = reader.IsDBNull("date_of_birth") ? null : reader.GetDateTime("date_of_birth"),
                    PassportSeries = reader.IsDBNull("passport_series") ? "" : reader.GetInt32("passport_series").ToString(),
                    PassportNumber = reader.IsDBNull("passport_number") ? "" : reader.GetInt32("passport_number").ToString(),
                    PassportIssuedBy = reader.IsDBNull("passport_issued_by") ? "" : reader.GetString("passport_issued_by"),
                    PassportIssueDate = reader.IsDBNull("passport_issue_date") ? null : reader.GetDateTime("passport_issue_date"),
                    Phone = reader.IsDBNull("phone") ? "" : reader.GetString("phone"),
                    Email = reader.IsDBNull("email") ? "" : reader.GetString("email"),
                    City = reader.IsDBNull("city") ? "" : reader.GetString("city"),
                    Street = reader.IsDBNull("street") ? "" : reader.GetString("street"),
                    House_Number = reader.IsDBNull("house_number") ? 0 : reader.GetInt32("house_number"),
                    UserId = reader.IsDBNull("user_id") ? 0 : reader.GetInt32("user_id"),
                    Created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _clients;
            SetupColumns("clients");
            _currentTable = "clients";
            HighlightButton(clientsButton);
        }

        private void LoadEmployees()
        {
            _employees = new List<Employee>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT employee_id, last_name, first_name, patronymic, phone, email, user_id, created_on
                FROM employees", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _employees.Add(new Employee
                {
                    Id = reader.GetInt64("employee_id"),
                    LastName = reader.IsDBNull("last_name") ? "" : reader.GetString("last_name"),
                    FirstName = reader.IsDBNull("first_name") ? "" : reader.GetString("first_name"),
                    Patronymic = reader.IsDBNull("patronymic") ? "" : reader.GetString("patronymic"),
                    Number = reader.IsDBNull("phone") ? "" : reader.GetString("phone"),
                    Email = reader.IsDBNull("email") ? "" : reader.GetString("email"),
                    UserId = reader.IsDBNull("user_id") ? 0 : reader.GetInt32("user_id"),
                    created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _employees;
            SetupColumns("employees");
            _currentTable = "employees";
            HighlightButton(EmployeesButton);
        }

        private void LoadItems()
        {
            _items = new List<Item>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT item_id, category_id, name, description, estimated_value, market_value, img_path, created_on
                FROM items", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _items.Add(new Item
                {
                    Id = reader.GetInt64("item_id"),
                    item_category_id = reader.IsDBNull("category_id") ? 0 : reader.GetInt32("category_id"),
                    item_name = reader.IsDBNull("name") ? "" : reader.GetString("name"),
                    item_description = reader.IsDBNull("description") ? "" : reader.GetString("description"),
                    item_estimated_price = reader.IsDBNull("estimated_value") ? 0m : reader.GetDecimal("estimated_value"),
                    item_market_price = reader.IsDBNull("market_value") ? 0m : reader.GetDecimal("market_value"),
                    item_image = reader.IsDBNull("img_path") ? "" : reader.GetString("img_path"),
                    created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _items;
            SetupColumns("items");
            _currentTable = "items";
            HighlightButton(ItemsButton);
        }

        private void LoadRates()
        {
            _rates = new List<Interest_rate>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT rate_id, category_id, min_days, max_days, daily_rate_percent
                FROM interest_rates", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _rates.Add(new Interest_rate
                {
                    Id = reader.GetInt32("rate_id"),
                    Category_id = reader.IsDBNull("category_id") ? 0 : reader.GetInt32("category_id"),
                    Min_days = reader.IsDBNull("min_days") ? 0 : reader.GetInt32("min_days"),
                    Max_days = reader.IsDBNull("max_days") ? 0 : reader.GetInt32("max_days"),
                    Daily_rate_percent = reader.IsDBNull("daily_rate_percent") ? 0m : reader.GetDecimal("daily_rate_percent")
                });
            }
            MainDataGrid.ItemsSource = _rates;
            SetupColumns("rates");
            _currentTable = "rates";
            HighlightButton(RatesButton);
        }

        private void LoadContracts()
        {
            _contracts = new List<Contract>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT contract_id, client_id, employee_id, item_id, contract_number, pawn_amount, redemption_amount,
                       contract_date, due_date, status_id, created_on
                FROM contracts", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _contracts.Add(new Contract
                {
                    Id = reader.GetInt64("contract_id"),
                    client_id = reader.IsDBNull("client_id") ? 0L : reader.GetInt64("client_id"),
                    employee_id = reader.IsDBNull("employee_id") ? 0L : reader.GetInt64("employee_id"),
                    item_id = reader.IsDBNull("item_id") ? 0L : reader.GetInt64("item_id"),
                    Contract_number = reader.IsDBNull("contract_number") ? 0 : reader.GetInt32("contract_number"),
                    Pawn_amount = reader.IsDBNull("pawn_amount") ? 0m : reader.GetDecimal("pawn_amount"),
                    Redemption_amount = reader.IsDBNull("redemption_amount") ? 0m : reader.GetDecimal("redemption_amount"),
                    Contract_date = reader.GetDateTime("contract_date"),
                    Due_date = reader.GetDateTime("due_date"),
                    status_id = reader.IsDBNull("status_id") ? 0 : reader.GetInt32("status_id"),
                    Created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _contracts;
            SetupColumns("contracts");
            _currentTable = "contracts";
            HighlightButton(ContractsButton);
        }

        private void LoadExtensions()
        {
            _extensions = new List<Extension>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT extension_id, contract_id, old_due_date, new_due_date, extension_fee, extended_by_employee_id, extension_date
                FROM extensions", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _extensions.Add(new Extension
                {
                    Id = reader.GetInt64("extension_id"),
                    contract_id = reader.IsDBNull("contract_id") ? 0L : reader.GetInt64("contract_id"),
                    Old_due_date = reader.GetDateTime("old_due_date"),
                    New_due_date = reader.GetDateTime("new_due_date"),
                    Extension_fee = reader.IsDBNull("extension_fee") ? 0m : reader.GetDecimal("extension_fee"),
                    Extended_by_employee_id = reader.IsDBNull("extended_by_employee_id") ? 0L : reader.GetInt64("extended_by_employee_id"),
                    Created_on = reader.GetDateTime("extension_date")
                });
            }
            MainDataGrid.ItemsSource = _extensions;
            SetupColumns("extensions");
            _currentTable = "extensions";
            HighlightButton(ExtensionsButton);
        }

        private void LoadRedemptions()
        {
            _redemptions = new List<Redemption>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT redemption_id, contract_id, redemption_date, total_paid, redeemed_by_employee_id, created_on
                FROM redemptions", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _redemptions.Add(new Redemption
                {
                    Id = reader.GetInt64("redemption_id"),
                    contract_id = reader.IsDBNull("contract_id") ? 0L : reader.GetInt64("contract_id"),
                    Redemption_date = reader.GetDateTime("redemption_date"),
                    Total_paid = reader.IsDBNull("total_paid") ? 0m : reader.GetDecimal("total_paid"),
                    redeemed_by_employee_id = reader.IsDBNull("redeemed_by_employee_id") ? 0L : reader.GetInt64("redeemed_by_employee_id"),
                    Created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _redemptions;
            SetupColumns("redemptions");
            _currentTable = "redemptions";
            HighlightButton(RedemptionsButton);
        }

        private void LoadPurchases()
        {
            _purchases = new List<Purchase>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT buy_id, item_id, buy_price, buy_date, client_id, buy_by_emploee_id, created_on
                FROM purchases", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _purchases.Add(new Purchase
                {
                    Id = reader.GetInt64("buy_id"),
                    item_id = reader.IsDBNull("item_id") ? 0L : reader.GetInt64("item_id"),
                    Buy_price = reader.IsDBNull("buy_price") ? 0m : reader.GetDecimal("buy_price"),
                    Buy_date = reader.GetDateTime("buy_date"),
                    client_id = reader.IsDBNull("client_id") ? 0L : reader.GetInt64("client_id"),
                    buy_by_employee_id = reader.IsDBNull("buy_by_emploee_id") ? 0L : reader.GetInt64("buy_by_emploee_id"),
                    Created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _purchases;
            SetupColumns("purchases");
            _currentTable = "purchases";
            HighlightButton(PurchasesButton);
        }

        private void LoadSales()
        {
            _sales = new List<Sale>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT sale_id, item_id, sale_date, sale_price, client_id, sold_by_employee_id, created_on
                FROM sales", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _sales.Add(new Sale
                {
                    Id = reader.GetInt64("sale_id"),
                    item_id = reader.IsDBNull("item_id") ? 0L : reader.GetInt64("item_id"),
                    Sale_date = reader.GetDateTime("sale_date"),
                    Sale_price = reader.IsDBNull("sale_price") ? 0m : reader.GetDecimal("sale_price"),
                    client_id = reader.IsDBNull("client_id") ? 0L : reader.GetInt64("client_id"),
                    sold_by_employee_id = reader.IsDBNull("sold_by_employee_id") ? 0L : reader.GetInt64("sold_by_employee_id"),
                    Created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _sales;
            SetupColumns("sales");
            _currentTable = "sales";
            HighlightButton(SalesButton);
        }

        // ==============================
        // НАСТРОЙКА КОЛОНОК
        // ==============================
        private void SetupColumns(string table)
        {
            MainDataGrid.Columns.Clear();
            MainDataGrid.AutoGenerateColumns = false;
            MainDataGrid.IsReadOnly = false;
            switch (table)
            {
                case "clients":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Фамилия", Binding = new System.Windows.Data.Binding("LastName"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Имя", Binding = new System.Windows.Data.Binding("FirstName"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Отчество", Binding = new System.Windows.Data.Binding("Patronymic"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Паспорт (серия)", Binding = new System.Windows.Data.Binding("PassportSeries"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Паспорт (номер)", Binding = new System.Windows.Data.Binding("PassportNumber"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Телефон", Binding = new System.Windows.Data.Binding("Phone"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new System.Windows.Data.Binding("Email"), Width = 120 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Город", Binding = new System.Windows.Data.Binding("City"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Улица", Binding = new System.Windows.Data.Binding("Street"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дом", Binding = new System.Windows.Data.Binding("House_Number"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "User ID", Binding = new System.Windows.Data.Binding("UserId"), Width = 70 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("Created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "employees":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Фамилия", Binding = new System.Windows.Data.Binding("LastName"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Имя", Binding = new System.Windows.Data.Binding("FirstName"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Отчество", Binding = new System.Windows.Data.Binding("Patronymic"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Телефон", Binding = new System.Windows.Data.Binding("Number"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new System.Windows.Data.Binding("Email"), Width = 120 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "User ID", Binding = new System.Windows.Data.Binding("UserId"), Width = 70 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "items":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Категория ID", Binding = new System.Windows.Data.Binding("item_category_id"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new System.Windows.Data.Binding("item_name"), Width = 120 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Описание", Binding = new System.Windows.Data.Binding("item_description"), Width = 150 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Оценка", Binding = new System.Windows.Data.Binding("item_estimated_price") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Рынок", Binding = new System.Windows.Data.Binding("item_market_price") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Картинка", Binding = new System.Windows.Data.Binding("item_image"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "rates":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Категория ID", Binding = new System.Windows.Data.Binding("Category_id"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Мин. дней", Binding = new System.Windows.Data.Binding("Min_days"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Макс. дней", Binding = new System.Windows.Data.Binding("Max_days"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Ставка (%)", Binding = new System.Windows.Data.Binding("Daily_rate_percent") { StringFormat = "F2" }, Width = 100 });
                    break;
                case "contracts":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Клиент ID", Binding = new System.Windows.Data.Binding("client_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Товар ID", Binding = new System.Windows.Data.Binding("item_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Номер", Binding = new System.Windows.Data.Binding("Contract_number"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Залог", Binding = new System.Windows.Data.Binding("Pawn_amount") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Выкуп", Binding = new System.Windows.Data.Binding("Redemption_amount") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата контракта", Binding = new System.Windows.Data.Binding("Contract_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Срок до", Binding = new System.Windows.Data.Binding("Due_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Статус ID", Binding = new System.Windows.Data.Binding("status_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("Created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "extensions":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Контракт ID", Binding = new System.Windows.Data.Binding("contract_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Старый срок", Binding = new System.Windows.Data.Binding("Old_due_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Новый срок", Binding = new System.Windows.Data.Binding("New_due_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Плата", Binding = new System.Windows.Data.Binding("Extension_fee") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("Extended_by_employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("Created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "redemptions":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Контракт ID", Binding = new System.Windows.Data.Binding("contract_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата выкупа", Binding = new System.Windows.Data.Binding("Redemption_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сумма", Binding = new System.Windows.Data.Binding("Total_paid") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("redeemed_by_employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("Created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "purchases":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Товар ID", Binding = new System.Windows.Data.Binding("item_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Цена покупки", Binding = new System.Windows.Data.Binding("Buy_price") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата покупки", Binding = new System.Windows.Data.Binding("Buy_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Клиент ID", Binding = new System.Windows.Data.Binding("client_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("buy_by_employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("Created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "sales":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Товар ID", Binding = new System.Windows.Data.Binding("item_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата продажи", Binding = new System.Windows.Data.Binding("Sale_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Цена продажи", Binding = new System.Windows.Data.Binding("Sale_price") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Клиент ID", Binding = new System.Windows.Data.Binding("client_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("sold_by_employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("Created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
            }
        }

        // ==============================
        // КНОПКИ МЕНЮ
        // ==============================
        private void ClientsButton_Click(object sender, RoutedEventArgs e) => LoadClients();
        private void EmployeesButton_Click(object sender, RoutedEventArgs e) => LoadEmployees();
        private void ItemsButton_Click(object sender, RoutedEventArgs e) => LoadItems();
        private void RatesButton_Click(object sender, RoutedEventArgs e) => LoadRates();
        private void ContractsButton_Click(object sender, RoutedEventArgs e) => LoadContracts();
        private void ExtensionsButton_Click(object sender, RoutedEventArgs e) => LoadExtensions();
        private void RedemptionsButton_Click(object sender, RoutedEventArgs e) => LoadRedemptions();
        private void PurchasesButton_Click(object sender, RoutedEventArgs e) => LoadPurchases();
        private void SalesButton_Click(object sender, RoutedEventArgs e) => LoadSales();

        // ==============================
        // CRUD — добавление, удаление, сохранение
        // ==============================
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTable == "clients")
            {
                var newClient = new Client { Created_on = DateTime.Now };
                _clients.Add(newClient);
                RefreshGrid(_clients);
                MainDataGrid.SelectedItem = newClient;
            }
            else if (_currentTable == "employees")
            {
                var newEmp = new Employee { created_on = DateTime.Now };
                _employees.Add(newEmp);
                RefreshGrid(_employees);
                MainDataGrid.SelectedItem = newEmp;
            }
            // ... аналогично для всех таблиц ...
            else if (_currentTable == "items")
            {
                var newItem = new Item { created_on = DateTime.Now };
                _items.Add(newItem);
                RefreshGrid(_items);
                MainDataGrid.SelectedItem = newItem;
            }
            else if (_currentTable == "rates")
            {
                var newRate = new Interest_rate();
                _rates.Add(newRate);
                RefreshGrid(_rates);
                MainDataGrid.SelectedItem = newRate;
            }
            else if (_currentTable == "contracts")
            {
                var newContract = new Contract { Created_on = DateTime.Now };
                _contracts.Add(newContract);
                RefreshGrid(_contracts);
                MainDataGrid.SelectedItem = newContract;
            }
            else if (_currentTable == "extensions")
            {
                var newExt = new Extension { Created_on = DateTime.Now };
                _extensions.Add(newExt);
                RefreshGrid(_extensions);
                MainDataGrid.SelectedItem = newExt;
            }
            else if (_currentTable == "redemptions")
            {
                var newRed = new Redemption { Created_on = DateTime.Now };
                _redemptions.Add(newRed);
                RefreshGrid(_redemptions);
                MainDataGrid.SelectedItem = newRed;
            }
            else if (_currentTable == "purchases")
            {
                var newPur = new Purchase { Created_on = DateTime.Now };
                _purchases.Add(newPur);
                RefreshGrid(_purchases);
                MainDataGrid.SelectedItem = newPur;
            }
            else if (_currentTable == "sales")
            {
                var newSale = new Sale { Created_on = DateTime.Now };
                _sales.Add(newSale);
                RefreshGrid(_sales);
                MainDataGrid.SelectedItem = newSale;
            }
        }

        private void RefreshGrid<T>(List<T> list)
        {
            MainDataGrid.ItemsSource = null;
            MainDataGrid.ItemsSource = list;
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTable == "clients" && MainDataGrid.SelectedItem is Client client && client.Id > 0)
            {
                if (MessageBox.Show($"Удалить клиента {client.FirstName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM clients WHERE client_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", client.Id);
                    cmd.ExecuteNonQuery();
                    LoadClients();
                }
            }
            else if (_currentTable == "employees" && MainDataGrid.SelectedItem is Employee employee && employee.Id > 0)
            {
                if (MessageBox.Show($"Удалить сотрудника {employee.FirstName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM employees WHERE employee_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", employee.Id);
                    cmd.ExecuteNonQuery();
                    LoadEmployees();
                }
            }
            else if (_currentTable == "items" && MainDataGrid.SelectedItem is Item item && item.Id > 0)
            {
                if (MessageBox.Show($"Удалить товар {item.item_name}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM items WHERE item_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    cmd.ExecuteNonQuery();
                    LoadItems();
                }
            }
            else if (_currentTable == "rates" && MainDataGrid.SelectedItem is Interest_rate rate && rate.Id > 0)
            {
                if (MessageBox.Show($"Удалить процентную ставку {rate.Daily_rate_percent}%?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM interest_rates WHERE rate_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", rate.Id);
                    cmd.ExecuteNonQuery();
                    LoadRates();
                }
            }
            else if (_currentTable == "contracts" && MainDataGrid.SelectedItem is Contract contract && contract.Id > 0)
            {
                if (MessageBox.Show($"Удалить контракт №{contract.Contract_number}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM contracts WHERE contract_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", contract.Id);
                    cmd.ExecuteNonQuery();
                    LoadContracts();
                }
            }
            else if (_currentTable == "extensions" && MainDataGrid.SelectedItem is Extension extension && extension.Id > 0)
            {
                if (MessageBox.Show($"Удалить продление ID={extension.Id}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM extensions WHERE extension_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", extension.Id);
                    cmd.ExecuteNonQuery();
                    LoadExtensions();
                }
            }
            else if (_currentTable == "redemptions" && MainDataGrid.SelectedItem is Redemption redemption && redemption.Id > 0)
            {
                if (MessageBox.Show($"Удалить выкуп ID={redemption.Id}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM redemptions WHERE redemption_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", redemption.Id);
                    cmd.ExecuteNonQuery();
                    LoadRedemptions();
                }
            }
            else if (_currentTable == "purchases" && MainDataGrid.SelectedItem is Purchase purchase && purchase.Id > 0)
            {
                if (MessageBox.Show($"Удалить покупку ID={purchase.Id}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM purchases WHERE buy_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", purchase.Id);
                    cmd.ExecuteNonQuery();
                    LoadPurchases();
                }
            }
            else if (_currentTable == "sales" && MainDataGrid.SelectedItem is Sale sale && sale.Id > 0)
            {
                if (MessageBox.Show($"Удалить продажу ID={sale.Id}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM sales WHERE sale_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", sale.Id);
                    cmd.ExecuteNonQuery();
                    LoadSales();
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentTable == "clients" && MainDataGrid.SelectedItem is Client client)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (client.Id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
            INSERT INTO clients (
                last_name, first_name, patronymic, date_of_birth,
                passport_series, passport_number, passport_issued_by, passport_issue_date,
                phone, email, city, street, house_number, user_id, created_on
            ) VALUES (
                @last_name, @first_name, @patronymic, @date_of_birth,
                @passport_series, @passport_number, @passport_issued_by, @passport_issue_date,
                @phone, @email, @city, @street, @house_number, @user_id, @created_on
            )", conn);

                        cmd.Parameters.AddWithValue("@last_name", client.LastName ?? "");
                        cmd.Parameters.AddWithValue("@first_name", client.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@patronymic", client.Patronymic ?? "");
                        cmd.Parameters.AddWithValue("@date_of_birth", client.DateOfBirth ?? (object)DBNull.Value);
                        // 🔥 КРИТИЧЕСКИ ВАЖНО: парсим в INT
                        cmd.Parameters.AddWithValue("@passport_series",
                            string.IsNullOrEmpty(client.PassportSeries) ? (object)DBNull.Value : int.Parse(client.PassportSeries));
                        cmd.Parameters.AddWithValue("@passport_number",
                            string.IsNullOrEmpty(client.PassportNumber) ? (object)DBNull.Value : int.Parse(client.PassportNumber));
                        cmd.Parameters.AddWithValue("@passport_issued_by", client.PassportIssuedBy ?? "");
                        cmd.Parameters.AddWithValue("@passport_issue_date", client.PassportIssueDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@phone", client.Phone ?? "");
                        cmd.Parameters.AddWithValue("@email", client.Email ?? "");
                        cmd.Parameters.AddWithValue("@city", client.City ?? "");
                        cmd.Parameters.AddWithValue("@street", client.Street ?? "");
                        cmd.Parameters.AddWithValue("@house_number", client.House_Number);
                        cmd.Parameters.AddWithValue("@user_id", client.UserId);
                        cmd.Parameters.AddWithValue("@created_on", client.Created_on);

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // UPDATE — аналогично, с int.Parse
                        using var cmd = new MySqlCommand(@"
            UPDATE clients SET
                last_name = @last_name,
                first_name = @first_name,
                patronymic = @patronymic,
                date_of_birth = @date_of_birth,
                passport_series = @passport_series,
                passport_number = @passport_number,
                passport_issued_by = @passport_issued_by,
                passport_issue_date = @passport_issue_date,
                phone = @phone,
                email = @email,
                city = @city,
                street = @street,
                house_number = @house_number,
                user_id = @user_id
            WHERE client_id = @id", conn);

                        cmd.Parameters.AddWithValue("@id", client.Id);
                        cmd.Parameters.AddWithValue("@last_name", client.LastName ?? "");
                        cmd.Parameters.AddWithValue("@first_name", client.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@patronymic", client.Patronymic ?? "");
                        cmd.Parameters.AddWithValue("@date_of_birth", client.DateOfBirth ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@passport_series",
                            string.IsNullOrEmpty(client.PassportSeries) ? (object)DBNull.Value : int.Parse(client.PassportSeries));
                        cmd.Parameters.AddWithValue("@passport_number",
                            string.IsNullOrEmpty(client.PassportNumber) ? (object)DBNull.Value : int.Parse(client.PassportNumber));
                        cmd.Parameters.AddWithValue("@passport_issued_by", client.PassportIssuedBy ?? "");
                        cmd.Parameters.AddWithValue("@passport_issue_date", client.PassportIssueDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@phone", client.Phone ?? "");
                        cmd.Parameters.AddWithValue("@email", client.Email ?? "");
                        cmd.Parameters.AddWithValue("@city", client.City ?? "");
                        cmd.Parameters.AddWithValue("@street", client.Street ?? "");
                        cmd.Parameters.AddWithValue("@house_number", client.House_Number);
                        cmd.Parameters.AddWithValue("@user_id", client.UserId);

                        cmd.ExecuteNonQuery();
                    }
                    LoadClients(); // перезагрузка из БД
                }
                else if (_currentTable == "employees" && MainDataGrid.SelectedItem is Employee employee)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (employee.Id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO employees (
                                last_name, first_name, patronymic, phone, email, user_id, created_on
                            ) VALUES (
                                @last_name, @first_name, @patronymic, @phone, @email, @user_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@last_name", employee.LastName ?? "");
                        cmd.Parameters.AddWithValue("@first_name", employee.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@patronymic", employee.Patronymic ?? "");
                        cmd.Parameters.AddWithValue("@phone", employee.Number ?? "");
                        cmd.Parameters.AddWithValue("@email", employee.Email ?? "");
                        cmd.Parameters.AddWithValue("@user_id", employee.UserId);
                        cmd.Parameters.AddWithValue("@created_on", employee.created_on);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using var cmd = new MySqlCommand(@"
                            UPDATE employees SET
                                last_name = @last_name,
                                first_name = @first_name,
                                patronymic = @patronymic,
                                phone = @phone,
                                email = @email,
                                user_id = @user_id
                            WHERE employee_id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", employee.Id);
                        cmd.Parameters.AddWithValue("@last_name", employee.LastName ?? "");
                        cmd.Parameters.AddWithValue("@first_name", employee.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@patronymic", employee.Patronymic ?? "");
                        cmd.Parameters.AddWithValue("@phone", employee.Number ?? "");
                        cmd.Parameters.AddWithValue("@email", employee.Email ?? "");
                        cmd.Parameters.AddWithValue("@user_id", employee.UserId);
                        cmd.ExecuteNonQuery();
                    }
                    LoadEmployees();
                }
                else if (_currentTable == "items" && MainDataGrid.SelectedItem is Item item)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (item.Id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO items (
                                category_id, name, description, estimated_value, market_value, img_path, created_on
                            ) VALUES (
                                @category_id, @name, @description, @estimated_value, @market_value, @img_path, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@category_id", item.item_category_id);
                        cmd.Parameters.AddWithValue("@name", item.item_name ?? "");
                        cmd.Parameters.AddWithValue("@description", item.item_description ?? "");
                        cmd.Parameters.AddWithValue("@estimated_value", item.item_estimated_price);
                        cmd.Parameters.AddWithValue("@market_value", item.item_market_price);
                        cmd.Parameters.AddWithValue("@img_path", item.item_image ?? "");
                        cmd.Parameters.AddWithValue("@created_on", item.created_on);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using var cmd = new MySqlCommand(@"
                            UPDATE items SET
                                category_id = @category_id,
                                name = @name,
                                description = @description,
                                estimated_value = @estimated_value,
                                market_value = @market_value,
                                img_path = @img_path
                            WHERE item_id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", item.Id);
                        cmd.Parameters.AddWithValue("@category_id", item.item_category_id);
                        cmd.Parameters.AddWithValue("@name", item.item_name ?? "");
                        cmd.Parameters.AddWithValue("@description", item.item_description ?? "");
                        cmd.Parameters.AddWithValue("@estimated_value", item.item_estimated_price);
                        cmd.Parameters.AddWithValue("@market_value", item.item_market_price);
                        cmd.Parameters.AddWithValue("@img_path", item.item_image ?? "");
                        cmd.ExecuteNonQuery();
                    }
                    LoadItems();
                }
                else if (_currentTable == "rates" && MainDataGrid.SelectedItem is Interest_rate rate)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (rate.Id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO interest_rates (
                                category_id, min_days, max_days, daily_rate_percent
                            ) VALUES (
                                @category_id, @min_days, @max_days, @daily_rate_percent
                            )", conn);
                        cmd.Parameters.AddWithValue("@category_id", rate.Category_id);
                        cmd.Parameters.AddWithValue("@min_days", rate.Min_days);
                        cmd.Parameters.AddWithValue("@max_days", rate.Max_days);
                        cmd.Parameters.AddWithValue("@daily_rate_percent", rate.Daily_rate_percent);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using var cmd = new MySqlCommand(@"
                            UPDATE interest_rates SET
                                category_id = @category_id,
                                min_days = @min_days,
                                max_days = @max_days,
                                daily_rate_percent = @daily_rate_percent
                            WHERE rate_id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", rate.Id);
                        cmd.Parameters.AddWithValue("@category_id", rate.Category_id);
                        cmd.Parameters.AddWithValue("@min_days", rate.Min_days);
                        cmd.Parameters.AddWithValue("@max_days", rate.Max_days);
                        cmd.Parameters.AddWithValue("@daily_rate_percent", rate.Daily_rate_percent);
                        cmd.ExecuteNonQuery();
                    }
                    LoadRates();
                }
                else if (_currentTable == "contracts" && MainDataGrid.SelectedItem is Contract contract)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (contract.Id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO contracts (
                                client_id, employee_id, item_id, contract_number, pawn_amount, redemption_amount,
                                contract_date, due_date, status_id, created_on
                            ) VALUES (
                                @client_id, @employee_id, @item_id, @contract_number, @pawn_amount, @redemption_amount,
                                @contract_date, @due_date, @status_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@client_id", contract.client_id);
                        cmd.Parameters.AddWithValue("@employee_id", contract.employee_id);
                        cmd.Parameters.AddWithValue("@item_id", contract.item_id);
                        cmd.Parameters.AddWithValue("@contract_number", contract.Contract_number);
                        cmd.Parameters.AddWithValue("@pawn_amount", contract.Pawn_amount);
                        cmd.Parameters.AddWithValue("@redemption_amount", contract.Redemption_amount);
                        cmd.Parameters.AddWithValue("@contract_date", contract.Contract_date);
                        cmd.Parameters.AddWithValue("@due_date", contract.Due_date);
                        cmd.Parameters.AddWithValue("@status_id", contract.status_id);
                        cmd.Parameters.AddWithValue("@created_on", contract.Created_on);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using var cmd = new MySqlCommand(@"
                            UPDATE contracts SET
                                client_id = @client_id,
                                employee_id = @employee_id,
                                item_id = @item_id,
                                contract_number = @contract_number,
                                pawn_amount = @pawn_amount,
                                redemption_amount = @redemption_amount,
                                contract_date = @contract_date,
                                due_date = @due_date,
                                status_id = @status_id
                            WHERE contract_id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", contract.Id);
                        cmd.Parameters.AddWithValue("@client_id", contract.client_id);
                        cmd.Parameters.AddWithValue("@employee_id", contract.employee_id);
                        cmd.Parameters.AddWithValue("@item_id", contract.item_id);
                        cmd.Parameters.AddWithValue("@contract_number", contract.Contract_number);
                        cmd.Parameters.AddWithValue("@pawn_amount", contract.Pawn_amount);
                        cmd.Parameters.AddWithValue("@redemption_amount", contract.Redemption_amount);
                        cmd.Parameters.AddWithValue("@contract_date", contract.Contract_date);
                        cmd.Parameters.AddWithValue("@due_date", contract.Due_date);
                        cmd.Parameters.AddWithValue("@status_id", contract.status_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadContracts();
                }
                else if (_currentTable == "extensions" && MainDataGrid.SelectedItem is Extension extension)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (extension.Id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO extensions (
                                contract_id, old_due_date, new_due_date, extension_fee, extended_by_employee_id, extension_date
                            ) VALUES (
                                @contract_id, @old_due_date, @new_due_date, @extension_fee, @extended_by_employee_id, @extension_date
                            )", conn);
                        cmd.Parameters.AddWithValue("@contract_id", extension.contract_id);
                        cmd.Parameters.AddWithValue("@old_due_date", extension.Old_due_date);
                        cmd.Parameters.AddWithValue("@new_due_date", extension.New_due_date);
                        cmd.Parameters.AddWithValue("@extension_fee", extension.Extension_fee);
                        cmd.Parameters.AddWithValue("@extended_by_employee_id", extension.Extended_by_employee_id);
                        cmd.Parameters.AddWithValue("@extension_date", extension.Created_on);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using var cmd = new MySqlCommand(@"
                            UPDATE extensions SET
                                contract_id = @contract_id,
                                old_due_date = @old_due_date,
                                new_due_date = @new_due_date,
                                extension_fee = @extension_fee,
                                extended_by_employee_id = @extended_by_employee_id
                            WHERE extension_id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", extension.Id);
                        cmd.Parameters.AddWithValue("@contract_id", extension.contract_id);
                        cmd.Parameters.AddWithValue("@old_due_date", extension.Old_due_date);
                        cmd.Parameters.AddWithValue("@new_due_date", extension.New_due_date);
                        cmd.Parameters.AddWithValue("@extension_fee", extension.Extension_fee);
                        cmd.Parameters.AddWithValue("@extended_by_employee_id", extension.Extended_by_employee_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadExtensions();
                }
                else if (_currentTable == "redemptions" && MainDataGrid.SelectedItem is Redemption redemption)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (redemption.Id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO redemptions (
                                contract_id, redemption_date, total_paid, redeemed_by_employee_id, created_on
                            ) VALUES (
                                @contract_id, @redemption_date, @total_paid, @redeemed_by_employee_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@contract_id", redemption.contract_id);
                        cmd.Parameters.AddWithValue("@redemption_date", redemption.Redemption_date);
                        cmd.Parameters.AddWithValue("@total_paid", redemption.Total_paid);
                        cmd.Parameters.AddWithValue("@redeemed_by_employee_id", redemption.redeemed_by_employee_id);
                        cmd.Parameters.AddWithValue("@created_on", redemption.Created_on);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using var cmd = new MySqlCommand(@"
                            UPDATE redemptions SET
                                contract_id = @contract_id,
                                redemption_date = @redemption_date,
                                total_paid = @total_paid,
                                redeemed_by_employee_id = @redeemed_by_employee_id
                            WHERE redemption_id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", redemption.Id);
                        cmd.Parameters.AddWithValue("@contract_id", redemption.contract_id);
                        cmd.Parameters.AddWithValue("@redemption_date", redemption.Redemption_date);
                        cmd.Parameters.AddWithValue("@total_paid", redemption.Total_paid);
                        cmd.Parameters.AddWithValue("@redeemed_by_employee_id", redemption.redeemed_by_employee_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadRedemptions();
                }
                else if (_currentTable == "purchases" && MainDataGrid.SelectedItem is Purchase purchase)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (purchase.Id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO purchases (
                                item_id, buy_price, buy_date, client_id, buy_by_emploee_id, created_on
                            ) VALUES (
                                @item_id, @buy_price, @buy_date, @client_id, @buy_by_emploee_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@item_id", purchase.item_id);
                        cmd.Parameters.AddWithValue("@buy_price", purchase.Buy_price);
                        cmd.Parameters.AddWithValue("@buy_date", purchase.Buy_date);
                        cmd.Parameters.AddWithValue("@client_id", purchase.client_id);
                        cmd.Parameters.AddWithValue("@buy_by_emploee_id", purchase.buy_by_employee_id);
                        cmd.Parameters.AddWithValue("@created_on", purchase.Created_on);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using var cmd = new MySqlCommand(@"
                            UPDATE purchases SET
                                item_id = @item_id,
                                buy_price = @buy_price,
                                buy_date = @buy_date,
                                client_id = @client_id,
                                buy_by_emploee_id = @buy_by_emploee_id
                            WHERE buy_id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", purchase.Id);
                        cmd.Parameters.AddWithValue("@item_id", purchase.item_id);
                        cmd.Parameters.AddWithValue("@buy_price", purchase.Buy_price);
                        cmd.Parameters.AddWithValue("@buy_date", purchase.Buy_date);
                        cmd.Parameters.AddWithValue("@client_id", purchase.client_id);
                        cmd.Parameters.AddWithValue("@buy_by_emploee_id", purchase.buy_by_employee_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadPurchases();
                }
                else if (_currentTable == "sales" && MainDataGrid.SelectedItem is Sale sale)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (sale.Id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO sales (
                                item_id, sale_date, sale_price, client_id, sold_by_employee_id, created_on
                            ) VALUES (
                                @item_id, @sale_date, @sale_price, @client_id, @sold_by_employee_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@item_id", sale.item_id);
                        cmd.Parameters.AddWithValue("@sale_date", sale.Sale_date);
                        cmd.Parameters.AddWithValue("@sale_price", sale.Sale_price);
                        cmd.Parameters.AddWithValue("@client_id", sale.client_id);
                        cmd.Parameters.AddWithValue("@sold_by_employee_id", sale.sold_by_employee_id);
                        cmd.Parameters.AddWithValue("@created_on", sale.Created_on);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using var cmd = new MySqlCommand(@"
                            UPDATE sales SET
                                item_id = @item_id,
                                sale_date = @sale_date,
                                sale_price = @sale_price,
                                client_id = @client_id,
                                sold_by_employee_id = @sold_by_employee_id
                            WHERE sale_id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", sale.Id);
                        cmd.Parameters.AddWithValue("@item_id", sale.item_id);
                        cmd.Parameters.AddWithValue("@sale_date", sale.Sale_date);
                        cmd.Parameters.AddWithValue("@sale_price", sale.Sale_price);
                        cmd.Parameters.AddWithValue("@client_id", sale.client_id);
                        cmd.Parameters.AddWithValue("@sold_by_employee_id", sale.sold_by_employee_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadSales();
                }
                else
                {
                    MessageBox.Show("Сохранение пока реализовано только для текущей таблицы.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==============================
        // ВСПОМОГАТЕЛЬНЫЕ
        // ==============================
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow?.Show();
            this.Close();
        }

        private void HighlightButton(Button activeButton)
        {
            var buttons = new[] {
                clientsButton,
                EmployeesButton,
                ItemsButton,
                RatesButton,
                ContractsButton,
                ExtensionsButton,
                RedemptionsButton,
                PurchasesButton,
                SalesButton
            };
            foreach (var btn in buttons)
                btn.Background = System.Windows.Media.Brushes.LightGray;
            activeButton.Background = System.Windows.Media.Brushes.LightBlue;
        }
    }
}