using lombard.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;

namespace lombard
{
    public partial class Admin : Window
    {
        private DateTime? GetSafeDateTime(MySqlDataReader reader, string fieldName)
        {
            try
            {
                if (reader.IsDBNull(fieldName))
                    return null;

                var value = reader.GetValue(fieldName);

                // Проверка на нулевые даты MySQL
                if (value is DateTime dt && (dt == DateTime.MinValue || dt.Year < 1900))
                    return null;

                // Попытка безопасного преобразования
                if (DateTime.TryParse(value.ToString(), out DateTime result))
                    return result;

                return null;
            }
            catch
            {
                return null;
            }
        }

        private string _userRole = "admin";
        public void SetUserRole(string role)
        {
            _userRole = role;
            ApplyRolePermissions();
        }

        // Добавьте этот метод в класс Admin
        private void ApplyRolePermissions()
        {
            if (_userRole == "appraiser")
            {
                // Скрываем кнопки, недоступные для оценщика
                EmployeesButton.Visibility = Visibility.Collapsed;
                clientsButton.Visibility = Visibility.Collapsed;
                ContractsButton.Visibility = Visibility.Collapsed;
                ExtensionsButton.Visibility = Visibility.Collapsed;
                RedemptionsButton.Visibility = Visibility.Collapsed;
                PurchasesButton.Visibility = Visibility.Collapsed;
                SalesButton.Visibility = Visibility.Collapsed;

                // Если сейчас открыта недоступная вкладка - открываем первую доступную
                if (_currentTable != "items" && _currentTable != "rates")
                {
                    LoadItems();
                }
            }
            // Для admin и других ролей оставляем все как есть
        }

        private const string ConnectionString =
            "Server=tompsons.beget.tech;Port=3306;Database=tompsons_stud03;User=tompsons_stud03;Password=10230901Sd;SslMode=Preferred;";

        private string _currentTable = "clients";

        // Храним текущие списки, чтобы не терять данные при добавлении
        private List<clients> _clients;
        private List<employees> _employees;
        private List<items> _items;
        private List<interest_rates> _rates;
        private List<contracts> _contracts;
        private List<extensions> _extensions;
        private List<redemptions> _redemptions;
        private List<purchases> _purchases;
        private List<sales> _sales;

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
            _clients = new List<clients>();
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
                var client = new clients();

                // Обработка ID
                client.client_id = reader.IsDBNull("client_id") ? 0 : reader.GetInt64("client_id");

                // Обработка ФИО
                client.last_name = reader.IsDBNull("last_name") ? "" : reader.GetString("last_name");
                client.first_name = reader.IsDBNull("first_name") ? "" : reader.GetString("first_name");
                client.patronymic = reader.IsDBNull("patronymic") ? "" : reader.GetString("patronymic");

                // Безопасная обработка дат
                client.date_of_birth = GetSafeDateTime(reader, "date_of_birth");
                client.passport_issue_date = GetSafeDateTime(reader, "passport_issue_date");
                client.created_on = GetSafeDateTime(reader, "created_on") ?? DateTime.Now;

                // Для серии паспорта
                client.passport_series = reader.IsDBNull("passport_series")
                    ? ""
                    : reader.GetInt32("passport_series").ToString();

                // Для номера паспорта
                client.passport_number = reader.IsDBNull("passport_number")
                    ? ""
                    : reader.GetInt32("passport_number").ToString();

                client.passport_issued_by = reader.IsDBNull("passport_issued_by") ? "" : reader.GetString("passport_issued_by");

                // Обработка контактов и адреса
                client.phone = reader.IsDBNull("phone") ? "" : reader.GetString("phone");
                client.email = reader.IsDBNull("email") ? "" : reader.GetString("email");
                client.city = reader.IsDBNull("city") ? "" : reader.GetString("city");
                client.street = reader.IsDBNull("street") ? "" : reader.GetString("street");
                client.house_number = reader.IsDBNull("house_number") ? 0 : reader.GetInt32("house_number");
                client.user_id = reader.IsDBNull("user_id") ? 0 : reader.GetInt32("user_id");

                _clients.Add(client);
            }
            MainDataGrid.ItemsSource = _clients;
            SetupColumns("clients");
            _currentTable = "clients";
            HighlightButton(clientsButton);
        }

        private void LoadEmployees()
        {
            _employees = new List<employees>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT employee_id, last_name, first_name, patronymic, phone, email, user_id, created_on
                FROM employees", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _employees.Add(new employees
                {
                    employee_id = reader.GetInt64("employee_id"),
                    last_name = reader.IsDBNull("last_name") ? "" : reader.GetString("last_name"),
                    first_name = reader.IsDBNull("first_name") ? "" : reader.GetString("first_name"),
                    patronymic = reader.IsDBNull("patronymic") ? "" : reader.GetString("patronymic"),
                    phone = reader.IsDBNull("phone") ? "" : reader.GetString("phone"),
                    email = reader.IsDBNull("email") ? "" : reader.GetString("email"),
                    user_id = reader.IsDBNull("user_id") ? 0 : reader.GetInt32("user_id"),
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
            _items = new List<items>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT item_id, category_id, name, description, estimated_value, market_value, img_path, created_on
                FROM items", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _items.Add(new items
                {
                    item_id = reader.GetInt64("item_id"),
                    category_id = reader.IsDBNull("category_id") ? 0 : reader.GetInt32("category_id"),
                    name = reader.IsDBNull("name") ? "" : reader.GetString("name"),
                    description = reader.IsDBNull("description") ? "" : reader.GetString("description"),
                    estimated_price = reader.IsDBNull("estimated_value") ? 0m : reader.GetDecimal("estimated_value"),
                    market_price = reader.IsDBNull("market_value") ? 0m : reader.GetDecimal("market_value"),
                    image = reader.IsDBNull("img_path") ? "" : reader.GetString("img_path"),
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
            _rates = new List<interest_rates>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT rate_id, category_id, min_days, max_days, daily_rate_percent
                FROM interest_rates", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _rates.Add(new interest_rates
                {
                    rate_id = reader.GetInt32("rate_id"),
                    category_id = reader.IsDBNull("category_id") ? 0 : reader.GetInt32("category_id"),
                    min_days = reader.IsDBNull("min_days") ? 0 : reader.GetInt32("min_days"),
                    max_days = reader.IsDBNull("max_days") ? 0 : reader.GetInt32("max_days"),
                    daily_rate_percent = reader.IsDBNull("daily_rate_percent") ? 0m : reader.GetDecimal("daily_rate_percent")
                });
            }
            MainDataGrid.ItemsSource = _rates;
            SetupColumns("rates");
            _currentTable = "rates";
            HighlightButton(RatesButton);
        }

        private void LoadContracts()
        {
            _contracts = new List<contracts>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT contract_id, client_id, employee_id, item_id, contract_number, pawn_amount, redemption_amount,
                       contract_date, due_date, status_id, created_on
                FROM contracts", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _contracts.Add(new contracts
                {
                    contract_id = reader.GetInt64("contract_id"),
                    client_id = reader.IsDBNull("client_id") ? 0L : reader.GetInt64("client_id"),
                    employee_id = reader.IsDBNull("employee_id") ? 0L : reader.GetInt64("employee_id"),
                    item_id = reader.IsDBNull("item_id") ? 0L : reader.GetInt64("item_id"),
                    contract_number = reader.IsDBNull("contract_number") ? 0 : reader.GetInt32("contract_number"),
                    pawn_amount = reader.IsDBNull("pawn_amount") ? 0m : reader.GetDecimal("pawn_amount"),
                    redemption_amount = reader.IsDBNull("redemption_amount") ? 0m : reader.GetDecimal("redemption_amount"),
                    contract_date = reader.GetDateTime("contract_date"),
                    due_date = reader.GetDateTime("due_date"),
                    status_id = reader.IsDBNull("status_id") ? 0 : reader.GetInt32("status_id"),
                    created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _contracts;
            SetupColumns("contracts");
            _currentTable = "contracts";
            HighlightButton(ContractsButton);
        }

        private void LoadExtensions()
        {
            _extensions = new List<extensions>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT extension_id, contract_id, old_due_date, new_due_date, extension_fee, extended_by_employee_id, created_on
                FROM extensions", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _extensions.Add(new extensions
                {
                    extension_id = reader.GetInt64("extension_id"),
                    contract_id = reader.IsDBNull("contract_id") ? 0L : reader.GetInt64("contract_id"),
                    old_due_date = reader.GetDateTime("old_due_date"),
                    new_due_date = reader.GetDateTime("new_due_date"),
                    extension_fee = reader.IsDBNull("extension_fee") ? 0m : reader.GetDecimal("extension_fee"),
                    extended_by_employee_id = reader.IsDBNull("extended_by_employee_id") ? 0L : reader.GetInt64("extended_by_employee_id"),
                    created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _extensions;
            SetupColumns("extensions");
            _currentTable = "extensions";
            HighlightButton(ExtensionsButton);
        }

        private void LoadRedemptions()
        {
            _redemptions = new List<redemptions>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT redemption_id, contract_id, redemption_date, total_paid, redeemed_by_employee_id, created_on
                FROM redemptions", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _redemptions.Add(new redemptions
                {
                    redemption_id = reader.GetInt64("redemption_id"),
                    contract_id = reader.IsDBNull("contract_id") ? 0L : reader.GetInt64("contract_id"),
                    redemption_date = reader.GetDateTime("redemption_date"),
                    total_paid = reader.IsDBNull("total_paid") ? 0m : reader.GetDecimal("total_paid"),
                    redeemed_by_employee_id = reader.IsDBNull("redeemed_by_employee_id") ? 0L : reader.GetInt64("redeemed_by_employee_id"),
                    created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _redemptions;
            SetupColumns("redemptions");
            _currentTable = "redemptions";
            HighlightButton(RedemptionsButton);
        }

        private void LoadPurchases()
        {
            _purchases = new List<purchases>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT buy_id, item_id, buy_price, buy_date, client_id, buy_by_employee_id, created_on
                FROM purchases", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _purchases.Add(new purchases
                {
                    purchase_id = reader.GetInt64("buy_id"),
                    item_id = reader.IsDBNull("item_id") ? 0L : reader.GetInt64("item_id"),
                    buy_price = reader.IsDBNull("buy_price") ? 0m : reader.GetDecimal("buy_price"),
                    buy_date = reader.GetDateTime("buy_date"),
                    client_id = reader.IsDBNull("client_id") ? 0L : reader.GetInt64("client_id"),
                    buy_by_employee_id = reader.IsDBNull("buy_by_employee_id") ? 0L : reader.GetInt64("buy_by_employee_id"),
                    created_on = reader.GetDateTime("created_on")
                });
            }
            MainDataGrid.ItemsSource = _purchases;
            SetupColumns("purchases");
            _currentTable = "purchases";
            HighlightButton(PurchasesButton);
        }

        private void LoadSales()
        {
            _sales = new List<sales>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(@"
                SELECT sale_id, item_id, sale_date, sale_price, client_id, sold_by_employee_id, created_on
                FROM sales", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _sales.Add(new sales
                {
                    sale_id = reader.GetInt64("sale_id"),
                    item_id = reader.IsDBNull("item_id") ? 0L : reader.GetInt64("item_id"),
                    sale_date = reader.GetDateTime("sale_date"),
                    sale_price = reader.IsDBNull("sale_price") ? 0m : reader.GetDecimal("sale_price"),
                    client_id = reader.IsDBNull("client_id") ? 0L : reader.GetInt64("client_id"),
                    sold_by_employee_id = reader.IsDBNull("sold_by_employee_id") ? 0L : reader.GetInt64("sold_by_employee_id"),
                    created_on = reader.GetDateTime("created_on")
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
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("client_id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Фамилия", Binding = new System.Windows.Data.Binding("last_name"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Имя", Binding = new System.Windows.Data.Binding("first_name"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Отчество", Binding = new System.Windows.Data.Binding("patronymic"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Паспорт (серия)", Binding = new System.Windows.Data.Binding("passport_series"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Паспорт (номер)", Binding = new System.Windows.Data.Binding("passport_number"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Телефон", Binding = new System.Windows.Data.Binding("phone"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "email", Binding = new System.Windows.Data.Binding("email"), Width = 120 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Город", Binding = new System.Windows.Data.Binding("city"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Улица", Binding = new System.Windows.Data.Binding("street"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дом", Binding = new System.Windows.Data.Binding("house_number"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "User ID", Binding = new System.Windows.Data.Binding("user_id"), Width = 70 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "employees":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("employee_id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Фамилия", Binding = new System.Windows.Data.Binding("last_name"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Имя", Binding = new System.Windows.Data.Binding("first_name"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Отчество", Binding = new System.Windows.Data.Binding("patronymic"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Телефон", Binding = new System.Windows.Data.Binding("phone"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "email", Binding = new System.Windows.Data.Binding("email"), Width = 120 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "User ID", Binding = new System.Windows.Data.Binding("user_id"), Width = 70 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "items":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("item_id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Категория ID", Binding = new System.Windows.Data.Binding("category_id"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new System.Windows.Data.Binding("name"), Width = 120 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Описание", Binding = new System.Windows.Data.Binding("description"), Width = 150 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Оценка", Binding = new System.Windows.Data.Binding("estimated_price") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Рынок", Binding = new System.Windows.Data.Binding("market_price") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Картинка", Binding = new System.Windows.Data.Binding("image"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "rates":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("rate_id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Категория ID", Binding = new System.Windows.Data.Binding("category_id"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Мин. дней", Binding = new System.Windows.Data.Binding("min_days"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Макс. дней", Binding = new System.Windows.Data.Binding("max_days"), Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Ставка (%)", Binding = new System.Windows.Data.Binding("daily_rate_percent") { StringFormat = "F2" }, Width = 100 });
                    break;
                case "contracts":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("contract_id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Клиент ID", Binding = new System.Windows.Data.Binding("client_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Товар ID", Binding = new System.Windows.Data.Binding("item_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Номер", Binding = new System.Windows.Data.Binding("contract_number"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Залог", Binding = new System.Windows.Data.Binding("pawn_amount") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Выкуп", Binding = new System.Windows.Data.Binding("redemption_amount") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата контракта", Binding = new System.Windows.Data.Binding("contract_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Срок до", Binding = new System.Windows.Data.Binding("due_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Статус ID", Binding = new System.Windows.Data.Binding("status_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "extensions":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("extension_id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Контракт ID", Binding = new System.Windows.Data.Binding("contract_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Старый срок", Binding = new System.Windows.Data.Binding("old_due_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Новый срок", Binding = new System.Windows.Data.Binding("new_due_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Плата", Binding = new System.Windows.Data.Binding("extension_fee") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("extended_by_employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "redemptions":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("redemption_id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Контракт ID", Binding = new System.Windows.Data.Binding("contract_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата выкупа", Binding = new System.Windows.Data.Binding("redemption_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сумма", Binding = new System.Windows.Data.Binding("total_paid") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("redeemed_by_employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "purchases":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("purchase_id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Товар ID", Binding = new System.Windows.Data.Binding("item_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Цена покупки", Binding = new System.Windows.Data.Binding("buy_price") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата покупки", Binding = new System.Windows.Data.Binding("buy_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Клиент ID", Binding = new System.Windows.Data.Binding("client_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("buy_by_employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
                    break;
                case "sales":
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("sale_id"), Width = 60 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Товар ID", Binding = new System.Windows.Data.Binding("item_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата продажи", Binding = new System.Windows.Data.Binding("sale_date") { StringFormat = "yyyy-MM-dd" }, Width = 100 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Цена продажи", Binding = new System.Windows.Data.Binding("sale_price") { StringFormat = "C2" }, Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Клиент ID", Binding = new System.Windows.Data.Binding("client_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник ID", Binding = new System.Windows.Data.Binding("sold_by_employee_id"), Width = 80 });
                    MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Создано", Binding = new System.Windows.Data.Binding("created_on") { StringFormat = "yyyy-MM-dd" }, Width = 90 });
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
                var newClient = new clients { created_on = DateTime.Now };
                _clients.Add(newClient);
                RefreshGrid(_clients);

                var foundClient = _clients.FirstOrDefault(c => c.created_on == newClient.created_on && c.client_id == 0);
                if (foundClient != null)
                {
                    MainDataGrid.SelectedItem = foundClient;
                    MainDataGrid.ScrollIntoView(foundClient);
                }
            }
            else if (_currentTable == "employees")
            {
                var newEmp = new employees { created_on = DateTime.Now };
                _employees.Add(newEmp);
                RefreshGrid(_employees);

                var foundEmp = _employees.FirstOrDefault(e => e.created_on == newEmp.created_on && e.employee_id == 0);
                if (foundEmp != null)
                {
                    MainDataGrid.SelectedItem = foundEmp;
                    MainDataGrid.ScrollIntoView(foundEmp);
                }
            }
            else if (_currentTable == "items")
            {
                var newItem = new items { created_on = DateTime.Now };
                _items.Add(newItem);
                RefreshGrid(_items);

                var foundItem = _items.FirstOrDefault(i => i.created_on == newItem.created_on && i.item_id == 0);
                if (foundItem != null)
                {
                    MainDataGrid.SelectedItem = foundItem;
                    MainDataGrid.ScrollIntoView(foundItem);
                }
            }
            else if (_currentTable == "rates")
            {
                var newRate = new interest_rates();
                _rates.Add(newRate);
                RefreshGrid(_rates);

                var foundRate = _rates.FirstOrDefault(r => r.rate_id == 0);
                if (foundRate != null)
                {
                    MainDataGrid.SelectedItem = foundRate;
                    MainDataGrid.ScrollIntoView(foundRate);
                }
            }
            else if (_currentTable == "contracts")
            {
                var newContract = new contracts { created_on = DateTime.Now };
                _contracts.Add(newContract);
                RefreshGrid(_contracts);

                var foundContract = _contracts.FirstOrDefault(c => c.created_on == newContract.created_on && c.contract_id == 0);
                if (foundContract != null)
                {
                    MainDataGrid.SelectedItem = foundContract;
                    MainDataGrid.ScrollIntoView(foundContract);
                }
            }
            else if (_currentTable == "extensions")
            {
                var newExt = new extensions { created_on = DateTime.Now };
                _extensions.Add(newExt);
                RefreshGrid(_extensions);

                var foundExt = _extensions.FirstOrDefault(ex => ex.created_on == newExt.created_on && ex.extension_id == 0);
                if (foundExt != null)
                {
                    MainDataGrid.SelectedItem = foundExt;
                    MainDataGrid.ScrollIntoView(foundExt);
                }
            }
            else if (_currentTable == "redemptions")
            {
                var newRed = new redemptions { created_on = DateTime.Now };
                _redemptions.Add(newRed);
                RefreshGrid(_redemptions);

                var foundRed = _redemptions.FirstOrDefault(r => r.created_on == newRed.created_on && r.redemption_id == 0);
                if (foundRed != null)
                {
                    MainDataGrid.SelectedItem = foundRed;
                    MainDataGrid.ScrollIntoView(foundRed);
                }
            }
            else if (_currentTable == "purchases")
            {
                var newPur = new purchases { created_on = DateTime.Now };
                _purchases.Add(newPur);
                RefreshGrid(_purchases);

                var foundPur = _purchases.FirstOrDefault(p => p.created_on == newPur.created_on && p.purchase_id == 0);
                if (foundPur != null)
                {
                    MainDataGrid.SelectedItem = foundPur;
                    MainDataGrid.ScrollIntoView(foundPur);
                }
            }
            else if (_currentTable == "sales")
            {
                var newSale = new sales { created_on = DateTime.Now };
                _sales.Add(newSale);
                RefreshGrid(_sales);

                var foundSale = _sales.FirstOrDefault(s => s.created_on == newSale.created_on && s.sale_id == 0);
                if (foundSale != null)
                {
                    MainDataGrid.SelectedItem = foundSale;
                    MainDataGrid.ScrollIntoView(foundSale);
                }
            }
        }

        private void RefreshGrid<T>(List<T> list)
        {
            MainDataGrid.ItemsSource = null;
            MainDataGrid.ItemsSource = list;
            MainDataGrid.Items.Refresh();
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTable == "clients" && MainDataGrid.SelectedItem is clients client && client.client_id > 0)
            {
                if (MessageBox.Show($"Удалить клиента {client.first_name}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM clients WHERE client_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", client.client_id);
                    cmd.ExecuteNonQuery();
                    LoadClients();
                }
            }
            else if (_currentTable == "employees" && MainDataGrid.SelectedItem is employees employee && employee.employee_id > 0)
            {
                if (MessageBox.Show($"Удалить сотрудника {employee.first_name}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM employees WHERE employee_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", employee.employee_id);
                    cmd.ExecuteNonQuery();
                    LoadEmployees();
                }
            }
            else if (_currentTable == "items" && MainDataGrid.SelectedItem is items item && item.item_id > 0)
            {
                if (MessageBox.Show($"Удалить товар {item.name}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM items WHERE item_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", item.item_id);
                    cmd.ExecuteNonQuery();
                    LoadItems();
                }
            }
            else if (_currentTable == "rates" && MainDataGrid.SelectedItem is interest_rates rate && rate.rate_id > 0)
            {
                if (MessageBox.Show($"Удалить процентную ставку {rate.daily_rate_percent}%?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM interest_rates WHERE rate_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", rate.rate_id);
                    cmd.ExecuteNonQuery();
                    LoadRates();
                }
            }
            else if (_currentTable == "contracts" && MainDataGrid.SelectedItem is contracts contract && contract.contract_id > 0)
            {
                if (MessageBox.Show($"Удалить контракт №{contract.contract_number}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM contracts WHERE contract_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", contract.contract_id);
                    cmd.ExecuteNonQuery();
                    LoadContracts();
                }
            }
            else if (_currentTable == "extensions" && MainDataGrid.SelectedItem is extensions extension && extension.extension_id > 0)
            {
                if (MessageBox.Show($"Удалить продление ID={extension.extension_id}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM extensions WHERE extension_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", extension.extension_id);
                    cmd.ExecuteNonQuery();
                    LoadExtensions();
                }
            }
            else if (_currentTable == "redemptions" && MainDataGrid.SelectedItem is redemptions redemption && redemption.redemption_id > 0)
            {
                if (MessageBox.Show($"Удалить выкуп ID={redemption.redemption_id}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM redemptions WHERE redemption_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", redemption.redemption_id);
                    cmd.ExecuteNonQuery();
                    LoadRedemptions();
                }
            }
            else if (_currentTable == "purchases" && MainDataGrid.SelectedItem is purchases purchase && purchase.purchase_id > 0)
            {
                if (MessageBox.Show($"Удалить покупку ID={purchase.purchase_id}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM purchases WHERE buy_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", purchase.purchase_id);
                    cmd.ExecuteNonQuery();
                    LoadPurchases();
                }
            }
            else if (_currentTable == "sales" && MainDataGrid.SelectedItem is sales sale && sale.sale_id > 0)
            {
                if (MessageBox.Show($"Удалить продажу ID={sale.sale_id}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM sales WHERE sale_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", sale.sale_id);
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
                if (_currentTable == "clients" && MainDataGrid.SelectedItem is clients client)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (client.client_id == 0)
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

                        // Обязательные поля
                        cmd.Parameters.AddWithValue("@last_name", client.last_name ?? "");
                        cmd.Parameters.AddWithValue("@first_name", client.first_name ?? "");
                        cmd.Parameters.AddWithValue("@patronymic", client.patronymic ?? "");
                        cmd.Parameters.AddWithValue("@date_of_birth", client.date_of_birth ?? (object)DBNull.Value);

                        // Опциональные поля (могут быть NULL)
                        cmd.Parameters.AddWithValue("@passport_series",
                            string.IsNullOrEmpty(client.passport_series) ? (object)DBNull.Value : (object)client.passport_series);
                        cmd.Parameters.AddWithValue("@passport_number",
                            string.IsNullOrEmpty(client.passport_number) ? (object)DBNull.Value : (object)client.passport_number);
                        cmd.Parameters.AddWithValue("@passport_issued_by",
                            string.IsNullOrEmpty(client.passport_issued_by) ? (object)DBNull.Value : client.passport_issued_by);
                        cmd.Parameters.AddWithValue("@passport_issue_date",
                            client.passport_issue_date.HasValue ? (object)client.passport_issue_date.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@phone",
                            string.IsNullOrEmpty(client.phone) ? (object)DBNull.Value : client.phone);
                        cmd.Parameters.AddWithValue("@email",
                            string.IsNullOrEmpty(client.email) ? (object)DBNull.Value : client.email);
                        cmd.Parameters.AddWithValue("@city",
                            string.IsNullOrEmpty(client.city) ? (object)DBNull.Value : client.city);
                        cmd.Parameters.AddWithValue("@street",
                            string.IsNullOrEmpty(client.street) ? (object)DBNull.Value : client.street);
                        cmd.Parameters.AddWithValue("@house_number",
                            client.house_number == 0 ? (object)DBNull.Value : client.house_number);
                        cmd.Parameters.AddWithValue("@user_id",
                            client.client_id == 0 ? (object)DBNull.Value : client.client_id);
                        cmd.Parameters.AddWithValue("@created_on", client.created_on);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Новый клиент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        // UPDATE запрос (аналогично с проверками на пустые значения)
                        // ... (тот же код с обработкой NULL для опциональных полей)
                    }
                    LoadClients();
                }
                else if (_currentTable == "employees" && MainDataGrid.SelectedItem is employees employee)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (employee.employee_id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO employees (
                                last_name, first_name, patronymic, phone, email, user_id, created_on
                            ) VALUES (
                                @last_name, @first_name, @patronymic, @phone, @email, @user_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@last_name", employee.last_name ?? "");
                        cmd.Parameters.AddWithValue("@first_name", employee.first_name ?? "");
                        cmd.Parameters.AddWithValue("@patronymic", employee.patronymic ?? "");
                        cmd.Parameters.AddWithValue("@phone", employee.phone ?? "");
                        cmd.Parameters.AddWithValue("@email", employee.email ?? "");
                        cmd.Parameters.AddWithValue("@user_id", employee.user_id);
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
                        cmd.Parameters.AddWithValue("@id", employee.employee_id);
                        cmd.Parameters.AddWithValue("@last_name", employee.last_name ?? "");
                        cmd.Parameters.AddWithValue("@first_name", employee.first_name ?? "");
                        cmd.Parameters.AddWithValue("@patronymic", employee.patronymic ?? "");
                        cmd.Parameters.AddWithValue("@phone", employee.phone ?? "");
                        cmd.Parameters.AddWithValue("@email", employee.email ?? "");
                        cmd.Parameters.AddWithValue("@user_id", employee.user_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadEmployees();
                }
                else if (_currentTable == "items" && MainDataGrid.SelectedItem is items item)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (item.item_id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO items (
                                category_id, name, description, estimated_value, market_value, img_path, created_on
                            ) VALUES (
                                @category_id, @name, @description, @estimated_value, @market_value, @img_path, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@category_id", item.category_id);
                        cmd.Parameters.AddWithValue("@name", item.name ?? "");
                        cmd.Parameters.AddWithValue("@description", item.description ?? "");
                        cmd.Parameters.AddWithValue("@estimated_value", item.estimated_price);
                        cmd.Parameters.AddWithValue("@market_value", item.market_price);
                        cmd.Parameters.AddWithValue("@img_path", item.image ?? "");
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
                        cmd.Parameters.AddWithValue("@id", item.item_id);
                        cmd.Parameters.AddWithValue("@category_id", item.category_id);
                        cmd.Parameters.AddWithValue("@name", item.name ?? "");
                        cmd.Parameters.AddWithValue("@description", item.description ?? "");
                        cmd.Parameters.AddWithValue("@estimated_value", item.estimated_price);
                        cmd.Parameters.AddWithValue("@market_value", item.market_price);
                        cmd.Parameters.AddWithValue("@img_path", item.image ?? "");
                        cmd.ExecuteNonQuery();
                    }
                    LoadItems();
                }
                else if (_currentTable == "rates" && MainDataGrid.SelectedItem is interest_rates rate)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (rate.rate_id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO interest_rates (
                                category_id, min_days, max_days, daily_rate_percent
                            ) VALUES (
                                @category_id, @min_days, @max_days, @daily_rate_percent
                            )", conn);
                        cmd.Parameters.AddWithValue("@category_id", rate.category_id);
                        cmd.Parameters.AddWithValue("@min_days", rate.min_days);
                        cmd.Parameters.AddWithValue("@max_days", rate.max_days);
                        cmd.Parameters.AddWithValue("@daily_rate_percent", rate.daily_rate_percent);
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
                        cmd.Parameters.AddWithValue("@id", rate.rate_id);
                        cmd.Parameters.AddWithValue("@category_id", rate.category_id);
                        cmd.Parameters.AddWithValue("@min_days", rate.min_days);
                        cmd.Parameters.AddWithValue("@max_days", rate.max_days);
                        cmd.Parameters.AddWithValue("@daily_rate_percent", rate.daily_rate_percent);
                        cmd.ExecuteNonQuery();
                    }
                    LoadRates();
                }
                else if (_currentTable == "contracts" && MainDataGrid.SelectedItem is contracts contract)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (contract.contract_id == 0)
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
                        cmd.Parameters.AddWithValue("@contract_number", contract.contract_number);
                        cmd.Parameters.AddWithValue("@pawn_amount", contract.pawn_amount);
                        cmd.Parameters.AddWithValue("@redemption_amount", contract.redemption_amount);
                        cmd.Parameters.AddWithValue("@contract_date", contract.contract_date);
                        cmd.Parameters.AddWithValue("@due_date", contract.due_date);
                        cmd.Parameters.AddWithValue("@status_id", contract.status_id);
                        cmd.Parameters.AddWithValue("@created_on", contract.created_on);
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
                        cmd.Parameters.AddWithValue("@id", contract.contract_id);
                        cmd.Parameters.AddWithValue("@client_id", contract.client_id);
                        cmd.Parameters.AddWithValue("@employee_id", contract.employee_id);
                        cmd.Parameters.AddWithValue("@item_id", contract.item_id);
                        cmd.Parameters.AddWithValue("@contract_number", contract.contract_number);
                        cmd.Parameters.AddWithValue("@pawn_amount", contract.pawn_amount);
                        cmd.Parameters.AddWithValue("@redemption_amount", contract.redemption_amount);
                        cmd.Parameters.AddWithValue("@contract_date", contract.contract_date);
                        cmd.Parameters.AddWithValue("@due_date", contract.due_date);
                        cmd.Parameters.AddWithValue("@status_id", contract.status_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadContracts();
                }
                else if (_currentTable == "extensions" && MainDataGrid.SelectedItem is extensions extension)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (extension.extension_id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO extensions (
                                contract_id, old_due_date, new_due_date, extension_fee, extended_by_employee_id, created_on
                            ) VALUES (
                                @contract_id, @old_due_date, @new_due_date, @extension_fee, @extended_by_employee_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@contract_id", extension.contract_id);
                        cmd.Parameters.AddWithValue("@old_due_date", extension.old_due_date);
                        cmd.Parameters.AddWithValue("@new_due_date", extension.new_due_date);
                        cmd.Parameters.AddWithValue("@extension_fee", extension.extension_fee);
                        cmd.Parameters.AddWithValue("@extended_by_employee_id", extension.extended_by_employee_id);
                        cmd.Parameters.AddWithValue("@created_on", extension.created_on);
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
                        cmd.Parameters.AddWithValue("@id", extension.extension_id);
                        cmd.Parameters.AddWithValue("@contract_id", extension.contract_id);
                        cmd.Parameters.AddWithValue("@old_due_date", extension.old_due_date);
                        cmd.Parameters.AddWithValue("@new_due_date", extension.new_due_date);
                        cmd.Parameters.AddWithValue("@extension_fee", extension.extension_fee);
                        cmd.Parameters.AddWithValue("@extended_by_employee_id", extension.extended_by_employee_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadExtensions();
                }
                else if (_currentTable == "redemptions" && MainDataGrid.SelectedItem is redemptions redemption)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (redemption.redemption_id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO redemptions (
                                contract_id, redemption_date, total_paid, redeemed_by_employee_id, created_on
                            ) VALUES (
                                @contract_id, @redemption_date, @total_paid, @redeemed_by_employee_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@contract_id", redemption.contract_id);
                        cmd.Parameters.AddWithValue("@redemption_date", redemption.redemption_date);
                        cmd.Parameters.AddWithValue("@total_paid", redemption.total_paid);
                        cmd.Parameters.AddWithValue("@redeemed_by_employee_id", redemption.redeemed_by_employee_id);
                        cmd.Parameters.AddWithValue("@created_on", redemption.created_on);
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
                        cmd.Parameters.AddWithValue("@id", redemption.redemption_id);
                        cmd.Parameters.AddWithValue("@contract_id", redemption.contract_id);
                        cmd.Parameters.AddWithValue("@redemption_date", redemption.redemption_date);
                        cmd.Parameters.AddWithValue("@total_paid", redemption.total_paid);
                        cmd.Parameters.AddWithValue("@redeemed_by_employee_id", redemption.redeemed_by_employee_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadRedemptions();
                }
                else if (_currentTable == "purchases" && MainDataGrid.SelectedItem is purchases purchase)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (purchase.purchase_id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO purchases (
                                item_id, buy_price, buy_date, client_id, buy_by_employee_id, created_on
                            ) VALUES (
                                @item_id, @buy_price, @buy_date, @client_id, @buy_by_employee_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@item_id", purchase.item_id);
                        cmd.Parameters.AddWithValue("@buy_price", purchase.buy_price);
                        cmd.Parameters.AddWithValue("@buy_date", purchase.buy_date);
                        cmd.Parameters.AddWithValue("@client_id", purchase.client_id);
                        cmd.Parameters.AddWithValue("@buy_by_employee_id", purchase.buy_by_employee_id);
                        cmd.Parameters.AddWithValue("@created_on", purchase.created_on);
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
                                buy_by_employee_id = @buy_by_employee_id
                            WHERE buy_id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", purchase.purchase_id);
                        cmd.Parameters.AddWithValue("@item_id", purchase.item_id);
                        cmd.Parameters.AddWithValue("@buy_price", purchase.buy_price);
                        cmd.Parameters.AddWithValue("@buy_date", purchase.buy_date);
                        cmd.Parameters.AddWithValue("@client_id", purchase.client_id);
                        cmd.Parameters.AddWithValue("@buy_by_employee_id", purchase.buy_by_employee_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadPurchases();
                }
                else if (_currentTable == "sales" && MainDataGrid.SelectedItem is sales sale)
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (sale.sale_id == 0)
                    {
                        using var cmd = new MySqlCommand(@"
                            INSERT INTO sales (
                                item_id, sale_date, sale_price, client_id, sold_by_employee_id, created_on
                            ) VALUES (
                                @item_id, @sale_date, @sale_price, @client_id, @sold_by_employee_id, @created_on
                            )", conn);
                        cmd.Parameters.AddWithValue("@item_id", sale.item_id);
                        cmd.Parameters.AddWithValue("@sale_date", sale.sale_date);
                        cmd.Parameters.AddWithValue("@sale_price", sale.sale_price);
                        cmd.Parameters.AddWithValue("@client_id", sale.client_id);
                        cmd.Parameters.AddWithValue("@sold_by_employee_id", sale.sold_by_employee_id);
                        cmd.Parameters.AddWithValue("@created_on", sale.created_on);
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
                        cmd.Parameters.AddWithValue("@id", sale.sale_id);
                        cmd.Parameters.AddWithValue("@item_id", sale.item_id);
                        cmd.Parameters.AddWithValue("@sale_date", sale.sale_date);
                        cmd.Parameters.AddWithValue("@sale_price", sale.sale_price);
                        cmd.Parameters.AddWithValue("@client_id", sale.client_id);
                        cmd.Parameters.AddWithValue("@sold_by_employee_id", sale.sold_by_employee_id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadSales();
                }
                else
                {
                    MessageBox.Show("Сохранено успешно!");
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

            // Цвет для обычных кнопок: #FF707B6D
            var normalColor = System.Windows.Media.Brushes.LightGray; // Временный цвет, заменим на стиль в XAML

            foreach (var btn in buttons)
                btn.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF707B6D"));

            // Цвет для выбранной кнопки - более насыщенный (70%) вариант #FF707B6D
            activeButton.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF4A573D"));
        }
    }
}