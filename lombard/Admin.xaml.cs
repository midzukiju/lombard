using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
// Убедитесь, что это пространство имен указывает на папку, где находятся ваши классы Models
using lombard.Models;

namespace lombard
{
    // =======================================================
    // 1. Базовые классы (RelayCommand, BaseViewModel)
    // =======================================================
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
    }

    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public ICommand SaveCommand { get; protected set; }
        public ICommand DeleteCommand { get; protected set; }
        public ICommand AddCommand { get; protected set; }

        public abstract void LoadData();
    }

    // =======================================================
    // 2. AdminViewModel (Главный навигатор)
    // =======================================================
    public class AdminViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => Set(ref _currentViewModel, value);
        }

        private int _currentUserId = 1;
        public int CurrentUserId
        {
            get => _currentUserId;
            set => Set(ref _currentUserId, value);
        }

        public ICommand NavigateCommand { get; }

        public AdminViewModel()
        {
            NavigateCommand = new RelayCommand(NavigateToView);
            // Инициализация первой ViewModel по умолчанию
            CurrentViewModel = new ClientViewModel();

            // Убедимся, что база данных существует
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }
        }

        private void NavigateToView(object parameter)
        {
            string viewName = parameter as string;
            if (viewName == null) return;

            BaseViewModel newViewModel = viewName switch
            {
                "Клиенты" => new ClientViewModel(),
                "Сотрудники" => new EmployeeViewModel(),
                "Товары" => new ItemViewModel(),
                "Проценты" => new InterestRateViewModel(),
                "Контракты" => new ContractViewModel(),
                "Продление" => new ExtensionViewModel(),
                "Выкупы" => new RedemptionViewModel(),
                "Покупки" => new PurchaseViewModel(),
                "Продажи" => new SaleViewModel(),
                "Заявки" => new RequestViewModel(),
                _ => CurrentViewModel
            };
            CurrentViewModel = newViewModel;
        }
        public override void LoadData() { }
    }

    // =======================================================
    // 3. ClientViewModel (Клиенты)
    // =======================================================
    public class ClientViewModel : BaseViewModel
    {
        private ObservableCollection<Client> _clients;
        public ObservableCollection<Client> Clients { get => _clients; set => Set(ref _clients, value); }
        private Client _selectedClient;
        public Client SelectedClient { get => _selectedClient; set => Set(ref _selectedClient, value); }

        public ClientViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedClient != null && SelectedClient.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }

        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Clients = new ObservableCollection<Client>(db.Clients.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Client
            {
                LastName = "Новая Фамилия",
                Created_on = DateTime.Now, // PascalCase
                PassportIssueDate = DateTime.Now,
                DateOfBirth = DateTime.Now,
                City = "Город",
                Street = "Улица",
                House_Number = 1
            };
            Clients.Add(newItem);
            SelectedClient = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Clients.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.Clients.Add(item);
                    else db.Clients.Update(item);
                }
                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены успешно!", "Успех");
                    LoadData();
                }
                catch (DbUpdateException ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД");
                }
            }
        }
        private void DeleteItem()
        {
            if (SelectedClient == null || SelectedClient.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedClient.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.Clients.Find(SelectedClient.Id);
                    if (itemToDelete != null)
                    {
                        db.Clients.Remove(itemToDelete);
                        db.SaveChanges();
                        Clients.Remove(SelectedClient);
                        SelectedClient = null;
                    }
                }
            }
        }
    }

    // =======================================================
    // 4. EmployeeViewModel (Сотрудники)
    // =======================================================
    public class EmployeeViewModel : BaseViewModel
    {
        private ObservableCollection<Employee> _employees;
        public ObservableCollection<Employee> Employees { get => _employees; set => Set(ref _employees, value); }
        private Employee _selectedEmployee;
        public Employee SelectedEmployee { get => _selectedEmployee; set => Set(ref _selectedEmployee, value); }

        public EmployeeViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedEmployee != null && SelectedEmployee.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }
        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Employees = new ObservableCollection<Employee>(db.Employees.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Employee { LastName = "Новый Сотрудник", created_on = DateTime.Now }; // snake_case
            Employees.Add(newItem);
            SelectedEmployee = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Employees.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.Employees.Add(item);
                    else db.Employees.Update(item);
                }
                try { db.SaveChanges(); MessageBox.Show("Данные сохранены успешно!", "Успех"); LoadData(); }
                catch (DbUpdateException ex) { MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД"); }
            }
        }
        private void DeleteItem()
        {
            if (SelectedEmployee == null || SelectedEmployee.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedEmployee.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.Employees.Find(SelectedEmployee.Id);
                    if (itemToDelete != null)
                    {
                        db.Employees.Remove(itemToDelete);
                        db.SaveChanges();
                        Employees.Remove(SelectedEmployee);
                        SelectedEmployee = null;
                    }
                }
            }
        }
    }

    // =======================================================
    // 5. ItemViewModel (Товары)
    // =======================================================
    public class ItemViewModel : BaseViewModel
    {
        private ObservableCollection<Item> _items;
        public ObservableCollection<Item> Items { get => _items; set => Set(ref _items, value); }
        private Item _selectedItem;
        public Item SelectedItem { get => _selectedItem; set => Set(ref _selectedItem, value); }

        public ItemViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedItem != null && SelectedItem.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }
        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Items = new ObservableCollection<Item>(db.Items.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Item
            {
                item_category_id = 1,
                item_name = "Новый товар",
                item_description = "Описание товара",
                item_estimated_price = 100m,
                item_market_price = 150m,
                item_image = "/Images/default.png",
                created_on = DateTime.Now // snake_case
            };
            Items.Add(newItem);
            SelectedItem = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Items.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.Items.Add(item);
                    else db.Items.Update(item);
                }
                try { db.SaveChanges(); MessageBox.Show("Данные сохранены успешно!", "Успех"); LoadData(); }
                catch (DbUpdateException ex) { MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД"); }
            }
        }
        private void DeleteItem()
        {
            if (SelectedItem == null || SelectedItem.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedItem.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.Items.Find(SelectedItem.Id);
                    if (itemToDelete != null)
                    {
                        db.Items.Remove(itemToDelete);
                        db.SaveChanges();
                        Items.Remove(SelectedItem);
                        SelectedItem = null;
                    }
                }
            }
        }
    }

    // =======================================================
    // 6. InterestRateViewModel (Проценты)
    // =======================================================
    public class InterestRateViewModel : BaseViewModel
    {
        private ObservableCollection<Interest_rate> _rates;
        public ObservableCollection<Interest_rate> Rates { get => _rates; set => Set(ref _rates, value); }
        private Interest_rate _selectedRate;
        public Interest_rate SelectedRate { get => _selectedRate; set => Set(ref _selectedRate, value); }

        public InterestRateViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedRate != null && SelectedRate.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }
        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                // ИСПРАВЛЕНО: DbSet теперь называется InterestRates
                Rates = new ObservableCollection<Interest_rate>(db.InterestRates.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Interest_rate
            {
                Category_id = 1,
                Min_days = 1,
                Max_days = 30,
                Daily_rate_percent = 0.5m // snake_case
            };
            Rates.Add(newItem);
            SelectedRate = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Rates.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.InterestRates.Add(item);
                    else db.InterestRates.Update(item);
                }
                try { db.SaveChanges(); MessageBox.Show("Данные сохранены успешно!", "Успех"); LoadData(); }
                catch (DbUpdateException ex) { MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД"); }
            }
        }
        private void DeleteItem()
        {
            if (SelectedRate == null || SelectedRate.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedRate.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.InterestRates.Find(SelectedRate.Id);
                    if (itemToDelete != null)
                    {
                        db.InterestRates.Remove(itemToDelete);
                        db.SaveChanges();
                        Rates.Remove(SelectedRate);
                        SelectedRate = null;
                    }
                }
            }
        }
    }

    // =======================================================
    // 7. ContractViewModel (Контракты)
    // =======================================================
    public class ContractViewModel : BaseViewModel
    {
        private ObservableCollection<Contract> _contracts;
        public ObservableCollection<Contract> Contracts { get => _contracts; set => Set(ref _contracts, value); }
        private Contract _selectedContract;
        public Contract SelectedContract { get => _selectedContract; set => Set(ref _selectedContract, value); }

        public ContractViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedContract != null && SelectedContract.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }
        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Contracts = new ObservableCollection<Contract>(db.Contracts.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Contract
            {
                Contract_status = "Новый",
                Contract_date = DateTime.Now,
                Due_date = DateTime.Now.AddMonths(1),
                Pawn_amount = 0m,
                Redemption_amount = 0m,
                Created_on = DateTime.Now, // PascalCase
                Contract_number = "0000",
                client_id = 1, // snake_case
                employee_id = 1, // snake_case
                item_id = 1 // snake_case
            };
            Contracts.Add(newItem);
            SelectedContract = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Contracts.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.Contracts.Add(item);
                    else db.Contracts.Update(item);
                }
                try { db.SaveChanges(); MessageBox.Show("Данные сохранены успешно!", "Успех"); LoadData(); }
                catch (DbUpdateException ex) { MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД"); }
            }
        }
        private void DeleteItem()
        {
            if (SelectedContract == null || SelectedContract.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedContract.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.Contracts.Find(SelectedContract.Id);
                    if (itemToDelete != null)
                    {
                        db.Contracts.Remove(itemToDelete);
                        db.SaveChanges();
                        Contracts.Remove(SelectedContract);
                        SelectedContract = null;
                    }
                }
            }
        }
    }

    // =======================================================
    // 8. ExtensionViewModel (Продление)
    // =======================================================
    public class ExtensionViewModel : BaseViewModel
    {
        private ObservableCollection<Extension> _extensions;
        public ObservableCollection<Extension> Extensions { get => _extensions; set => Set(ref _extensions, value); }
        private Extension _selectedExtension;
        public Extension SelectedExtension { get => _selectedExtension; set => Set(ref _selectedExtension, value); }

        public ExtensionViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedExtension != null && SelectedExtension.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }
        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Extensions = new ObservableCollection<Extension>(db.Extensions.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Extension
            {
                contract_id = 1, // snake_case
                Old_due_date = DateTime.Now.AddDays(30),
                New_due_date = DateTime.Now.AddDays(60),
                Extension_fee = 10.0m,
                Extended_by_employee_id = 1, // PascalCase
                Created_on = DateTime.Now // PascalCase
            };
            Extensions.Add(newItem);
            SelectedExtension = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Extensions.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.Extensions.Add(item);
                    else db.Extensions.Update(item);
                }
                try { db.SaveChanges(); MessageBox.Show("Данные сохранены успешно!", "Успех"); LoadData(); }
                catch (DbUpdateException ex) { MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД"); }
            }
        }
        private void DeleteItem()
        {
            if (SelectedExtension == null || SelectedExtension.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedExtension.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.Extensions.Find(SelectedExtension.Id);
                    if (itemToDelete != null)
                    {
                        db.Extensions.Remove(itemToDelete);
                        db.SaveChanges();
                        Extensions.Remove(SelectedExtension);
                        SelectedExtension = null;
                    }
                }
            }
        }
    }

    // =======================================================
    // 9. RedemptionViewModel (Выкупы)
    // =======================================================
    public class RedemptionViewModel : BaseViewModel
    {
        private ObservableCollection<Redemption> _redemptions;
        public ObservableCollection<Redemption> Redemptions { get => _redemptions; set => Set(ref _redemptions, value); }
        private Redemption _selectedRedemption;
        public Redemption SelectedRedemption { get => _selectedRedemption; set => Set(ref _selectedRedemption, value); }

        public RedemptionViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedRedemption != null && SelectedRedemption.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }
        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Redemptions = new ObservableCollection<Redemption>(db.Redemptions.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Redemption
            {
                contract_id = 1, // snake_case
                Redemption_date = DateTime.Now, // PascalCase
                Total_paid = 0m, // PascalCase
                redeemed_by_employee_id = 1, // snake_case
                Created_on = DateTime.Now // PascalCase
            };
            Redemptions.Add(newItem);
            SelectedRedemption = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Redemptions.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.Redemptions.Add(item);
                    else db.Redemptions.Update(item);
                }
                try { db.SaveChanges(); MessageBox.Show("Данные сохранены успешно!", "Успех"); LoadData(); }
                catch (DbUpdateException ex) { MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД"); }
            }
        }
        private void DeleteItem()
        {
            if (SelectedRedemption == null || SelectedRedemption.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedRedemption.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.Redemptions.Find(SelectedRedemption.Id);
                    if (itemToDelete != null)
                    {
                        db.Redemptions.Remove(itemToDelete);
                        db.SaveChanges();
                        Redemptions.Remove(SelectedRedemption);
                        SelectedRedemption = null;
                    }
                }
            }
        }
    }

    // =======================================================
    // 10. PurchaseViewModel (Покупки)
    // =======================================================
    public class PurchaseViewModel : BaseViewModel
    {
        private ObservableCollection<Purchase> _purchases;
        public ObservableCollection<Purchase> Buys { get => _purchases; set => Set(ref _purchases, value); }
        private Purchase _selectedPurchase;
        public Purchase SelectedBuy { get => _selectedPurchase; set => Set(ref _selectedPurchase, value); }

        public PurchaseViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedBuy != null && SelectedBuy.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }
        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Buys = new ObservableCollection<Purchase>(db.Purchases.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Purchase
            {
                item_id = 1, // snake_case
                Buy_price = 100m, // PascalCase
                Buy_date = DateTime.Now, // PascalCase
                client_id = 1, // snake_case
                buy_by_employee_id = 1, // snake_case
                Created_on = DateTime.Now // PascalCase
            };
            Buys.Add(newItem);
            SelectedBuy = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Buys.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.Purchases.Add(item);
                    else db.Purchases.Update(item);
                }
                try { db.SaveChanges(); MessageBox.Show("Данные сохранены успешно!", "Успех"); LoadData(); }
                catch (DbUpdateException ex) { MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД"); }
            }
        }
        private void DeleteItem()
        {
            if (SelectedBuy == null || SelectedBuy.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedBuy.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.Purchases.Find(SelectedBuy.Id);
                    if (itemToDelete != null)
                    {
                        db.Purchases.Remove(itemToDelete);
                        db.SaveChanges();
                        Buys.Remove(SelectedBuy);
                        SelectedBuy = null;
                    }
                }
            }
        }
    }

    // =======================================================
    // 11. SaleViewModel (Продажи)
    // =======================================================
    public class SaleViewModel : BaseViewModel
    {
        private ObservableCollection<Sale> _sales;
        public ObservableCollection<Sale> Sales { get => _sales; set => Set(ref _sales, value); }
        private Sale _selectedSale;
        public Sale SelectedSale { get => _selectedSale; set => Set(ref _selectedSale, value); }

        public SaleViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedSale != null && SelectedSale.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }
        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Sales = new ObservableCollection<Sale>(db.Sales.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Sale
            {
                item_id = 1, // snake_case
                Sale_price = 0m, // PascalCase
                Sale_date = DateTime.Now, // PascalCase
                client_id = 1, // snake_case
                sold_by_employee_id = 1, // snake_case
                Created_on = DateTime.Now // PascalCase
            };
            Sales.Add(newItem);
            SelectedSale = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Sales.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.Sales.Add(item);
                    else db.Sales.Update(item);
                }
                try { db.SaveChanges(); MessageBox.Show("Данные сохранены успешно!", "Успех"); LoadData(); }
                catch (DbUpdateException ex) { MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД"); }
            }
        }
        private void DeleteItem()
        {
            if (SelectedSale == null || SelectedSale.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedSale.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.Sales.Find(SelectedSale.Id);
                    if (itemToDelete != null)
                    {
                        db.Sales.Remove(itemToDelete);
                        db.SaveChanges();
                        Sales.Remove(SelectedSale);
                        SelectedSale = null;
                    }
                }
            }
        }
    }

    // =======================================================
    // 12. RequestViewModel (Заявки)
    // =======================================================
    public class RequestViewModel : BaseViewModel
    {
        private ObservableCollection<Request> _requests;
        public ObservableCollection<Request> Requests { get => _requests; set => Set(ref _requests, value); }
        private Request _selectedRequest;
        public Request SelectedRequest { get => _selectedRequest; set => Set(ref _selectedRequest, value); }

        public RequestViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveChanges());
            DeleteCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedRequest != null && SelectedRequest.Id != 0);
            AddCommand = new RelayCommand(_ => AddNewItem());
            LoadData();
        }
        public override void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Requests = new ObservableCollection<Request>(db.Requests.AsNoTracking().ToList());
            }
        }
        private void AddNewItem()
        {
            var newItem = new Request
            {
                Service_id = 1, // PascalCase
                Requester_last_name = "Новый",
                Requester_first_name = "Заявитель",
                Requester_patronymic = "Отчество",
                Requester_number = "123456789",
                Requester_city = "Город",
                Created_on = DateTime.Now // PascalCase
            };
            Requests.Add(newItem);
            SelectedRequest = newItem;
        }
        private void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in Requests.Where(i => i.Id == 0 || db.Entry(i).State == EntityState.Modified))
                {
                    if (item.Id == 0) db.Requests.Add(item);
                    else db.Requests.Update(item);
                }
                try { db.SaveChanges(); MessageBox.Show("Данные сохранены успешно!", "Успех"); LoadData(); }
                catch (DbUpdateException ex) { MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД"); }
            }
        }
        private void DeleteItem()
        {
            if (SelectedRequest == null || SelectedRequest.Id == 0) return;
            var result = MessageBox.Show($"Удалить запись ID: {SelectedRequest.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var itemToDelete = db.Requests.Find(SelectedRequest.Id);
                    if (itemToDelete != null)
                    {
                        db.Requests.Remove(itemToDelete);
                        db.SaveChanges();
                        Requests.Remove(SelectedRequest);
                        SelectedRequest = null;
                    }
                }
            }
        }
    }
}