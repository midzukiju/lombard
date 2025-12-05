using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace lombard
{
    // ====================================================================
    // 0. ВСПОМОГАТЕЛЬНЫЕ КЛАССЫ
    // ====================================================================

    #region 0.1 RelayCommand - Реализация ICommand

    /// <summary>
    /// Вспомогательный класс для реализации ICommand, позволяющий привязывать методы из ViewModel к элементам управления View.
    /// </summary>
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

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    #endregion

    #region 0.2 BaseViewModel - База для всех ViewModels

    /// <summary>
    /// Базовый класс для всех ViewModels, реализующий интерфейс INotifyPropertyChanged.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    // ====================================================================
    // 1. МОДЕЛИ ДАННЫХ (Models)
    // ====================================================================

    #region 1.1 Модель Клиент (Client Model)

    /// <summary>
    /// Представляет клиента ломбарда.
    /// </summary>
    public class Client : INotifyPropertyChanged
    {
        // Приватные поля (snake_case)
        private int _clientId;
        private string _lastName;
        private string _firstName;
        private string _patronymic;
        private DateTime? _dateOfBirth;
        private string _passportSeries;
        private string _passportNumber;
        private string _passportIssuedBy;
        private DateTime? _passportIssueDate;
        private string _registrationAddress;
        private string _phone;
        private string _email;
        private int _userId;
        private string _city;
        private string _street;
        private int? _houseNumber;
        private DateTime? _createdOn;

        // Публичные свойства (PascalCase, как в XAML)
        public int Id { get => _clientId; set { _clientId = value; OnPropertyChanged(nameof(Id)); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(nameof(LastName)); } }
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(nameof(FirstName)); } }
        public string Patronymic { get => _patronymic; set { _patronymic = value; OnPropertyChanged(nameof(Patronymic)); } }
        public DateTime? DateOfBirth { get => _dateOfBirth; set { _dateOfBirth = value; OnPropertyChanged(nameof(DateOfBirth)); } }
        public string PassportSeries { get => _passportSeries; set { _passportSeries = value; OnPropertyChanged(nameof(PassportSeries)); } }
        public string PassportNumber { get => _passportNumber; set { _passportNumber = value; OnPropertyChanged(nameof(PassportNumber)); } }
        public string PassportIssuedBy { get => _passportIssuedBy; set { _passportIssuedBy = value; OnPropertyChanged(nameof(PassportIssuedBy)); } }
        public DateTime? PassportIssueDate { get => _passportIssueDate; set { _passportIssueDate = value; OnPropertyChanged(nameof(PassportIssueDate)); } }
        public string RegistrationAddress { get => _registrationAddress; set { _registrationAddress = value; OnPropertyChanged(nameof(RegistrationAddress)); } }
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(nameof(Phone)); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(nameof(Email)); } }
        public int UserId { get => _userId; set { _userId = value; OnPropertyChanged(nameof(UserId)); } }
        public string City { get => _city; set { _city = value; OnPropertyChanged(nameof(City)); } }
        public string Street { get => _street; set { _street = value; OnPropertyChanged(nameof(Street)); } }
        public int? HouseNumber { get => _houseNumber; set { _houseNumber = value; OnPropertyChanged(nameof(HouseNumber)); } }
        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }

        // Вычисляемые свойства для XAML
        public string FullName => $"{LastName} {FirstName} {Patronymic}";
        public string PassportData => $"{PassportSeries} {PassportNumber}";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion

    #region 1.2 Модель Сотрудник (Employee Model)

    /// <summary>
    /// Представляет сотрудника ломбарда.
    /// </summary>
    public class Employee : INotifyPropertyChanged
    {
        private int _employeeId;
        private string _lastName;
        private string _firstName;
        private string _patronymic;
        private string _position;
        private string _number;
        private string _email;
        private int _userId; // Для привязки к User (если есть отдельная таблица пользователей)
        private DateTime? _createdOn;
        // 1. Для колонки "ID Сотрудника" (Id)
        public int Id { get => _employeeId; set { _employeeId = value; OnPropertyChanged(nameof(Id)); } }

        // 2. Части Ф.И.О.
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(nameof(LastName)); } }
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(nameof(FirstName)); } }
        public string Patronymic { get => _patronymic; set { _patronymic = value; OnPropertyChanged(nameof(Patronymic)); } }

        // 3. Вычисляемое свойство для колонки "Ф.И.О."
        public string FullName => $"{LastName} {FirstName} {Patronymic}";

        // 4. Для колонки "Должность" (Position)
        public string Position { get => _position; set { _position = value; OnPropertyChanged(nameof(Position)); } }

        // 5. Для колонки "Телефон" (Number)
        public string Number { get => _number; set { _number = value; OnPropertyChanged(nameof(Number)); } }

        // 6. Для колонки "Почта" (Email)
        public string Email { get => _email; set { _email = value; OnPropertyChanged(nameof(Email)); } }

        // 7. Для колонки "User id" (UserId)
        public int UserId { get => _userId; set { _userId = value; OnPropertyChanged(nameof(UserId)); } }

        // 8. Для колонки "Создано" (created_on)
        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region 1.3 Модель Товар (Item Model)

    /// <summary>
    /// Представляет товар, принятый ломбардом (залог или скупка).
    /// </summary>
    public class Item : INotifyPropertyChanged
    {
        // Приватные поля (для хранения данных)
        private int _itemId;
        private int _itemCategoryId;
        private string _itemName;
        private string _itemDescription;
        private decimal _itemEstimatedPrice;
        private decimal _itemMarketPrice;
        private byte[] _itemImage;
        private DateTime? _createdOn;

        // Публичные свойства (snake_case - как в XAML)

        public int item_id { get => _itemId; set { _itemId = value; OnPropertyChanged(nameof(item_id)); } }

        public int item_category_id { get => _itemCategoryId; set { _itemCategoryId = value; OnPropertyChanged(nameof(item_category_id)); } }

        public string item_name { get => _itemName; set { _itemName = value; OnPropertyChanged(nameof(item_name)); } }

        public string item_description { get => _itemDescription; set { _itemDescription = value; OnPropertyChanged(nameof(item_description)); } }

        public decimal item_estimated_price { get => _itemEstimatedPrice; set { _itemEstimatedPrice = value; OnPropertyChanged(nameof(item_estimated_price)); } }

        public decimal item_market_price { get => _itemMarketPrice; set { _itemMarketPrice = value; OnPropertyChanged(nameof(item_market_price)); } }

        public byte[] item_image { get => _itemImage; set { _itemImage = value; OnPropertyChanged(nameof(item_image)); } }

        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region 1.4 Модель Процентная Ставка (Rate Model)

    /// <summary>
    /// Представляет процентную ставку, зависящую от категории товара и срока.
    /// </summary>
    public class Rate : INotifyPropertyChanged
    {
        // Приватные поля (для хранения данных)
        private int _rateId;
        private int _categoryId;
        private int _minDays;
        private int _maxDays;
        private decimal _interestRate;
        private DateTime? _createdOn;

        // Публичные свойства (snake_case - как в XAML)

        // 1. ID Процента
        public int rate_id { get => _rateId; set { _rateId = value; OnPropertyChanged(nameof(rate_id)); } }

        // 2. ID Категории
        public int category_id { get => _categoryId; set { _categoryId = value; OnPropertyChanged(nameof(category_id)); } }

        // 3. Минимальное количество дней
        public int min_days { get => _minDays; set { _minDays = value; OnPropertyChanged(nameof(min_days)); } }

        // 4. Максимальное количество дней
        public int max_days { get => _maxDays; set { _maxDays = value; OnPropertyChanged(nameof(max_days)); } }

        // 5. Ставка (%)
        public decimal interest_rate { get => _interestRate; set { _interestRate = value; OnPropertyChanged(nameof(interest_rate)); } }

        // 6. Создано
        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region 1.5 Модель Контракт (Contract Model)

    /// <summary>
    /// Представляет договор залога или скупки, заключенный с клиентом.
    /// </summary>
    public class Contract : INotifyPropertyChanged
    {
        // Приватные поля (для хранения данных)
        private int _contractId;
        private int _clientId;        // Внешний ключ: Client
        private int _employeeId;      // Внешний ключ: Employee (тот, кто оформил)
        private int _itemId;          // Внешний ключ: Item
        private int _rateId;          // Внешний ключ: Rate
        private DateTime _issueDate;    // Дата выдачи/заключения
        private DateTime _maturityDate; // Дата погашения/возврата (срок)
        private decimal _loanAmount;    // Сумма займа (выданная клиенту)
        private decimal _pawnValue;     // Сумма оценки залога (для расчета)
        private string _status;         // Текущий статус: 'Активен', 'Просрочен', 'Выкуплен', 'Продан'
        private string _type;           // Тип контракта: 'Залог', 'Скупка'
        private DateTime? _createdOn;

        // Публичные свойства (snake_case - как в XAML)

        // 1. ID Контракта
        public int contract_id { get => _contractId; set { _contractId = value; OnPropertyChanged(nameof(contract_id)); } }

        // 2. Внешние ключи
        public int client_id { get => _clientId; set { _clientId = value; OnPropertyChanged(nameof(client_id)); } }
        public int employee_id { get => _employeeId; set { _employeeId = value; OnPropertyChanged(nameof(employee_id)); } }
        public int item_id { get => _itemId; set { _itemId = value; OnPropertyChanged(nameof(item_id)); } }
        public int rate_id { get => _rateId; set { _rateId = value; OnPropertyChanged(nameof(rate_id)); } }

        // 3. Основные даты и суммы
        public DateTime issue_date { get => _issueDate; set { _issueDate = value; OnPropertyChanged(nameof(issue_date)); } }
        public DateTime maturity_date { get => _maturityDate; set { _maturityDate = value; OnPropertyChanged(nameof(maturity_date)); } }
        public decimal loan_amount { get => _loanAmount; set { _loanAmount = value; OnPropertyChanged(nameof(loan_amount)); } }
        public decimal pawn_value { get => _pawnValue; set { _pawnValue = value; OnPropertyChanged(nameof(pawn_value)); } }

        // 4. Статус и тип
        public string status { get => _status; set { _status = value; OnPropertyChanged(nameof(status)); } }
        public string type { get => _type; set { _type = value; OnPropertyChanged(nameof(type)); } }

        // 5. Дата создания записи
        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region 1.6 Модель Продление (Extension Model)

    /// <summary>
    /// Представляет операцию продления срока действия контракта.
    /// </summary>
    public class Extension : INotifyPropertyChanged
    {
        // Приватные поля (для хранения данных)
        private int _extensionId;
        private int _contractId;        // Внешний ключ: Contract, который продлевается
        private DateTime _oldMaturityDate; // Старая дата погашения
        private DateTime _newMaturityDate; // Новая дата погашения
        private decimal _interestPaid;      // Сумма уплаченных процентов за предыдущий период
        private DateTime? _createdOn;

        // Публичные свойства (snake_case - как в XAML)

        // 1. ID Продления
        public int extension_id { get => _extensionId; set { _extensionId = value; OnPropertyChanged(nameof(extension_id)); } }

        // 2. ID Контракта
        public int contract_id { get => _contractId; set { _contractId = value; OnPropertyChanged(nameof(contract_id)); } }

        // 3. Даты
        public DateTime old_maturity_date { get => _oldMaturityDate; set { _oldMaturityDate = value; OnPropertyChanged(nameof(old_maturity_date)); } }
        public DateTime new_maturity_date { get => _newMaturityDate; set { _newMaturityDate = value; OnPropertyChanged(nameof(new_maturity_date)); } }

        // 4. Оплаченные проценты
        public decimal interest_paid { get => _interestPaid; set { _interestPaid = value; OnPropertyChanged(nameof(interest_paid)); } }

        // 5. Дата создания записи
        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region 1.7 Модель Выкуп (Redemption Model)

    /// <summary>
    /// Представляет операцию выкупа товара по контракту (погашение займа).
    /// </summary>
    public class Redemption : INotifyPropertyChanged
    {
        // Приватные поля (для хранения данных)
        private int _redemptionId;
        private int _contractId;        // Внешний ключ: Contract
        private DateTime _redemptionDate; // Дата фактического выкупа
        private decimal _principalPaid;   // Сумма, уплаченная за тело займа
        private decimal _interestPaid;    // Сумма, уплаченная в виде процентов
        private decimal _penaltyPaid;     // Сумма, уплаченная в виде штрафов (если были просрочки)
        private decimal _totalPaid;       // Общая сумма оплаты
        private int _employeeId;        // Внешний ключ: Employee (тот, кто принял платеж)
        private DateTime? _createdOn;

        // Публичные свойства (snake_case - как в XAML)

        // 1. ID Выкупа
        public int redemption_id { get => _redemptionId; set { _redemptionId = value; OnPropertyChanged(nameof(redemption_id)); } }

        // 2. ID Контракта
        public int contract_id { get => _contractId; set { _contractId = value; OnPropertyChanged(nameof(contract_id)); } }

        // 3. Дата выкупа
        public DateTime redemption_date { get => _redemptionDate; set { _redemptionDate = value; OnPropertyChanged(nameof(redemption_date)); } }

        // 4. Суммы оплаты (Общая сумма, скорее всего, привязана в XAML)
        // В вашем XAML есть "Сумма оплаты" — я привяжу ее к TotalPaid
        public decimal total_paid { get => _totalPaid; set { _totalPaid = value; OnPropertyChanged(nameof(total_paid)); } }

        // 5. ID Сотрудника
        public int redeemed_by_employee_id { get => _employeeId; set { _employeeId = value; OnPropertyChanged(nameof(redeemed_by_employee_id)); } }

        // 6. Дата создания записи
        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region 1.8 Модель Покупка (Buy Model)

    /// <summary>
    /// Представляет операцию скупки товара ломбардом у клиента.
    /// </summary>
    public class Buy : INotifyPropertyChanged
    {
        // Приватные поля (для хранения данных)
        private int _buyId;
        private int _itemId;           // Внешний ключ: Item (купленный товар)
        private decimal _buyPrice;     // Цена, за которую ломбард купил товар
        private DateTime _buyDate;      // Дата покупки
        private int _clientId;         // Внешний ключ: Client (тот, кто продал товар ломбарду)
        private int _employeeId;       // Внешний ключ: Employee (тот, кто оформил покупку)
        private DateTime? _createdOn;

        // Публичные свойства (snake_case - как в XAML)

        // 1. ID Покупки
        public int buy_id { get => _buyId; set { _buyId = value; OnPropertyChanged(nameof(buy_id)); } }

        // 2. ID Товара
        public int item_id { get => _itemId; set { _itemId = value; OnPropertyChanged(nameof(item_id)); } }

        // 3. Цена покупки
        public decimal buy_price { get => _buyPrice; set { _buyPrice = value; OnPropertyChanged(nameof(buy_price)); } }

        // 4. Дата покупки
        public DateTime buy_date { get => _buyDate; set { _buyDate = value; OnPropertyChanged(nameof(buy_date)); } }

        // 5. ID Клиента
        public int client_id { get => _clientId; set { _clientId = value; OnPropertyChanged(nameof(client_id)); } }

        // 6. ID Сотрудника
        public int buy_by_employee_id { get => _employeeId; set { _employeeId = value; OnPropertyChanged(nameof(buy_by_employee_id)); } }

        // 7. Дата создания записи
        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region 1.9 Модель Продажа (Sale Model)

    /// <summary>
    /// Представляет операцию продажи товара ломбардом.
    /// </summary>
    public class Sale : INotifyPropertyChanged
    {
        // Приватные поля (для хранения данных)
        private int _saleId;
        private int _itemId;           // Внешний ключ: Item (проданный товар)
        private DateTime _saleDate;     // Дата продажи
        private decimal _salePrice;    // Цена, за которую товар был продан
        private int? _clientId;         // Внешний ключ: Client (покупатель, может быть null)
        private int _employeeId;       // Внешний ключ: Employee (тот, кто оформил продажу)
        private DateTime? _createdOn;

        // Публичные свойства (snake_case - как в XAML)

        // 1. ID Продажи
        public int sale_id { get => _saleId; set { _saleId = value; OnPropertyChanged(nameof(sale_id)); } }

        // 2. ID Товара
        public int item_id { get => _itemId; set { _itemId = value; OnPropertyChanged(nameof(item_id)); } }

        // 3. Дата продажи
        public DateTime sale_date { get => _saleDate; set { _saleDate = value; OnPropertyChanged(nameof(sale_date)); } }

        // 4. Цена продажи
        public decimal sale_price { get => _salePrice; set { _salePrice = value; OnPropertyChanged(nameof(sale_price)); } }

        // 5. ID Клиента (покупателя)
        public int? client_id { get => _clientId; set { _clientId = value; OnPropertyChanged(nameof(client_id)); } }

        // 6. ID Сотрудника
        public int sold_by_employee_id { get => _employeeId; set { _employeeId = value; OnPropertyChanged(nameof(sold_by_employee_id)); } }

        // 7. Дата создания записи
        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region 1.10 Модель Заявка (Request Model)

    /// <summary>
    /// Представляет заявку клиента на услугу ломбарда (оценка, консультация и т.д.).
    /// </summary>
    public partial class Request : INotifyPropertyChanged
    {
        // Приватные поля (для хранения данных)
        private int _requestId;
        private int _serviceId;          // Внешний ключ: Service (тип услуги)
        private string _requesterLastName;
        private string _requesterFirstName;
        private string _requesterPatronymic;
        private string _requesterNumber;
        private string _requesterCity;
        private string _status;              // Статус заявки: "Новая", "Обработана", "Отклонена"
        private DateTime? _createdOn;

        // Публичные свойства (snake_case - как в XAML)

        // 1. ID Заявки
        public int request_id { get => _requestId; set { _requestId = value; OnPropertyChanged(nameof(request_id)); } }

        // 2. ID Услуги
        public int service_id { get => _serviceId; set { _serviceId = value; OnPropertyChanged(nameof(service_id)); } }

        // 3. ФИО Заявителя
        public string requester_last_name { get => _requesterLastName; set { _requesterLastName = value; OnPropertyChanged(nameof(requester_last_name)); } }
        public string requester_first_name { get => _requesterFirstName; set { _requesterFirstName = value; OnPropertyChanged(nameof(requester_first_name)); } }
        public string requester_patronymic { get => _requesterPatronymic; set { _requesterPatronymic = value; OnPropertyChanged(nameof(requester_patronymic)); } }

        // 4. Телефон и Город
        public string requester_number { get => _requesterNumber; set { _requesterNumber = value; OnPropertyChanged(nameof(requester_number)); } }
        public string requester_city { get => _requesterCity; set { _requesterCity = value; OnPropertyChanged(nameof(requester_city)); } }

        // 5. Статус
        public string status { get => _status; set { _status = value; OnPropertyChanged(nameof(status)); } }

        // 6. Дата создания
        public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region 1.11 Модель Категория (Category Model)

/// <summary>
/// Представляет категорию товара (например, Золото, Электроника).
/// </summary>
public class Category : INotifyPropertyChanged
{
    // Приватные поля (для хранения данных)
    private int _categoryId;
    private string _categoryName;
    private string _categoryDescription;
    private DateTime? _createdOn;

    // Публичные свойства (snake_case - как в XAML)

    // 1. ID Категории
    public int category_id { get => _categoryId; set { _categoryId = value; OnPropertyChanged(nameof(category_id)); } }

    // 2. Название Категории
    public string category_name { get => _categoryName; set { _categoryName = value; OnPropertyChanged(nameof(category_name)); } }

    // 3. Описание Категории
    public string category_description { get => _categoryDescription; set { _categoryDescription = value; OnPropertyChanged(nameof(category_description)); } }

    // 4. Дата создания
    public DateTime? created_on { get => _createdOn; set { _createdOn = value; OnPropertyChanged(nameof(created_on)); } }


    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

#endregion
    // ====================================================================
    // 2. VIEW MODELS (Логика данных и команд для таблиц)
    // ====================================================================

    #region 2.1 ViewModel Клиентов (ClientsViewModel)

    public class ClientsViewModel : BaseViewModel
    {
        public ObservableCollection<Client> Clients { get; }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
                // Обновление состояния кнопки "Удалить"
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public ClientsViewModel()
        {
            // Инициализация коллекции и тестовые данные
            Clients = new ObservableCollection<Client>
    {
        // Использование свойств в snake_case, как в модели Client
        new Client { Id = 1, LastName = "Иванов", FirstName = "Иван", Patronymic = "Иванович", PassportSeries = "1234", PassportNumber = "567890", created_on = DateTime.Now },
        new Client { Id = 2, LastName = "Петров", FirstName = "Петр", Patronymic = "Петрович", PassportSeries = "9876", PassportNumber = "543210", created_on = DateTime.Now }
    };

            AddCommand = new RelayCommand(AddClient);
            DeleteCommand = new RelayCommand(DeleteClient, CanDeleteClient);
            SaveCommand = new RelayCommand(SaveClients);
        }

        private void AddClient(object parameter)
        {
            // Вычисляем временный Id (для отображения, реальный Id будет от БД)
            int newId = Clients.Any() ? Clients.Max(c => c.Id) + 1 : 1;
            Clients.Add(new Client { Id = newId, LastName = "Новый клиент", PassportNumber = "000000" });
        }

        private void DeleteClient(object parameter)
        {
            if (SelectedClient != null)
            {
                Clients.Remove(SelectedClient);
            }
        }

        private bool CanDeleteClient(object parameter)
        {
            return SelectedClient != null;
        }

        private void SaveClients(object parameter)
        {
            // !!! СЮДА ДОБАВИТЬ ЛОГИКУ ВЗАИМОДЕЙСТВИЯ С БАЗОЙ ДАННЫХ !!!
            MessageBox.Show("Данные клиентов сохранены (эмуляция сохранения в БД).", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    #endregion

    #region 2.2 ViewModel Сотрудников (EmployeesViewModel)

    public class EmployeesViewModel : BaseViewModel
    {
        public ObservableCollection<Employee> Employees { get; }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public EmployeesViewModel()
        {
            // Инициализация коллекции и тестовые данные
            Employees = new ObservableCollection<Employee>
    {
        // Использование свойств в PascalCase, как в модели Employee
        new Employee { Id = 1, FirstName = "Елена", LastName = "Смирнова", Patronymic = "Александровна", Position = "Кассир" },
        new Employee { Id = 2, FirstName = "Максим", LastName = "Кузнецов", Patronymic = "Иванович", Position = "Оценщик" }
    };

            AddCommand = new RelayCommand(AddEmployee);
            DeleteCommand = new RelayCommand(DeleteEmployee, CanDeleteEmployee);
            SaveCommand = new RelayCommand(SaveEmployees);
        }

        private void AddEmployee(object parameter)
        {
            int newId = Employees.Any() ? Employees.Max(c => c.Id) + 1 : 1;
            Employees.Add(new Employee { Id = newId, LastName = "Новый", FirstName = "сотрудник", Position = "Не назначен" });
        }

        private void DeleteEmployee(object parameter)
        {
            if (SelectedEmployee != null)
            {
                Employees.Remove(SelectedEmployee);
            }
        }

        private bool CanDeleteEmployee(object parameter)
        {
            return SelectedEmployee != null;
        }

        private void SaveEmployees(object parameter)
        {
            // !!! СЮДА ДОБАВИТЬ ЛОГИКУ ВЗАИМОДЕЙСТВИЯ С БАЗОЙ ДАННЫХ !!!
            MessageBox.Show("Данные сотрудников сохранены (эмуляция сохранения в БД).", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    #endregion

    #region 2.3 ViewModel Товаров (ItemsViewModel)

    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

        private Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public ItemsViewModel()
        {
            Items.Add(new Item { item_id = 101, item_name = "Смартфон", item_estimated_price = 5000M, item_market_price = 7000M, created_on = DateTime.Now });

            AddCommand = new RelayCommand(AddItem);
            DeleteCommand = new RelayCommand(DeleteItem, CanDeleteItem);
            SaveCommand = new RelayCommand(SaveItems);
        }

        private void AddItem(object parameter)
        {
            int newId = Items.Any() ? Items.Max(c => c.item_id) + 1 : 1;
            Items.Add(new Item { item_id = newId, item_name = "Новый товар", item_estimated_price = 0M, item_market_price = 0M });
        }

        private void DeleteItem(object parameter)
        {
            if (SelectedItem != null) Items.Remove(SelectedItem);
        }

        private bool CanDeleteItem(object parameter) => SelectedItem != null;

        private void SaveItems(object parameter) => MessageBox.Show("Данные товаров сохранены.", "Сохранение");
    }

    #endregion

    #region 2.4 ViewModel Процентов (RatesViewModel)

    public class RatesViewModel : BaseViewModel
    {
        public ObservableCollection<Rate> Rates { get; } = new ObservableCollection<Rate>();

        private Rate _selectedRate;
        public Rate SelectedRate
        {
            get => _selectedRate;
            set
            {
                _selectedRate = value;
                OnPropertyChanged(nameof(SelectedRate));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public RatesViewModel()
        {
            Rates.Add(new Rate { rate_id = 1, category_id = 1, min_days = 1, max_days = 30, interest_rate = 0.5M, created_on = DateTime.Now });

            AddCommand = new RelayCommand(AddRate);
            DeleteCommand = new RelayCommand(DeleteRate, CanDeleteRate);
            SaveCommand = new RelayCommand(SaveRates);
        }

        private void AddRate(object parameter)
        {
            int newId = Rates.Any() ? Rates.Max(c => c.rate_id) + 1 : 1;
            Rates.Add(new Rate { rate_id = newId, category_id = 1, min_days = 1, max_days = 1, interest_rate = 0M });
        }

        private void DeleteRate(object parameter)
        {
            if (SelectedRate != null) Rates.Remove(SelectedRate);
        }

        private bool CanDeleteRate(object parameter) => SelectedRate != null;

        private void SaveRates(object parameter) => MessageBox.Show("Данные процентов сохранены.", "Сохранение");
    }

    #endregion

    #region 2.5 ViewModel Контрактов (ContractsViewModel)

    public class ContractsViewModel : BaseViewModel
    {
        public ObservableCollection<Contract> Contracts { get; } = new ObservableCollection<Contract>();

        private Contract _selectedContract;
        public Contract SelectedContract
        {
            get => _selectedContract;
            set
            {
                _selectedContract = value;
                OnPropertyChanged(nameof(SelectedContract));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public ContractsViewModel()
        {
            Contracts.Add(new Contract { contract_id = 501, client_id = 1, employee_id = 1, item_id = 101, issue_date = DateTime.Now, maturity_date = DateTime.Now.AddDays(30), loan_amount = 5000M, status = "Активен" });

            AddCommand = new RelayCommand(AddContract);
            DeleteCommand = new RelayCommand(DeleteContract, CanDeleteContract);
            SaveCommand = new RelayCommand(SaveContracts);
        }

        private void AddContract(object parameter)
        {
            int newId = Contracts.Any() ? Contracts.Max(c => c.contract_id) + 1 : 1;
            Contracts.Add(new Contract { contract_id = newId, client_id = 1, employee_id = 1, item_id = 1, issue_date = DateTime.Now, maturity_date = DateTime.Now.AddDays(1), loan_amount = 0M, status = "Черновик" });
        }

        private void DeleteContract(object parameter)
        {
            if (SelectedContract != null) Contracts.Remove(SelectedContract);
        }

        private bool CanDeleteContract(object parameter) => SelectedContract != null;

        private void SaveContracts(object parameter) => MessageBox.Show("Данные контрактов сохранены.", "Сохранение");
    }

    #endregion

    #region 2.6 ViewModel Продлений (ExtensionsViewModel)

    public class ExtensionsViewModel : BaseViewModel
    {
        public ObservableCollection<Extension> Extensions { get; } = new ObservableCollection<Extension>();

        private Extension _selectedExtension;
        public Extension SelectedExtension
        {
            get => _selectedExtension;
            set
            {
                _selectedExtension = value;
                OnPropertyChanged(nameof(SelectedExtension));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public ExtensionsViewModel()
        {
            Extensions.Add(new Extension { extension_id = 1, contract_id = 501, old_maturity_date = DateTime.Now.AddDays(30), new_maturity_date = DateTime.Now.AddDays(60), interest_paid = 500M });

            AddCommand = new RelayCommand(AddExtension);
            DeleteCommand = new RelayCommand(DeleteExtension, CanDeleteExtension);
            SaveCommand = new RelayCommand(SaveExtensions);
        }

        private void AddExtension(object parameter)
        {
            int newId = Extensions.Any() ? Extensions.Max(c => c.extension_id) + 1 : 1;
            Extensions.Add(new Extension { extension_id = newId, contract_id = 1, old_maturity_date = DateTime.Now, new_maturity_date = DateTime.Now.AddDays(1), interest_paid = 0M });
        }

        private void DeleteExtension(object parameter)
        {
            if (SelectedExtension != null) Extensions.Remove(SelectedExtension);
        }

        private bool CanDeleteExtension(object parameter) => SelectedExtension != null;

        private void SaveExtensions(object parameter) => MessageBox.Show("Данные продлений сохранены.", "Сохранение");
    }

    #endregion

    #region 2.7 ViewModel Выкупов (RedemptionViewModel)

    public class RedemptionViewModel : BaseViewModel
    {
        public ObservableCollection<Redemption> Redemptions { get; } = new ObservableCollection<Redemption>();

        private Redemption _selectedRedemption;
        public Redemption SelectedRedemption
        {
            get => _selectedRedemption;
            set
            {
                _selectedRedemption = value;
                OnPropertyChanged(nameof(SelectedRedemption));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public RedemptionViewModel()
        {
            Redemptions.Add(new Redemption { redemption_id = 1, contract_id = 502, redemption_date = DateTime.Now, total_paid = 6000M });

            AddCommand = new RelayCommand(AddRedemption);
            DeleteCommand = new RelayCommand(DeleteRedemption, CanDeleteRedemption);
            SaveCommand = new RelayCommand(SaveRedemptions);
        }

        private void AddRedemption(object parameter)
        {
            int newId = Redemptions.Any() ? Redemptions.Max(c => c.redemption_id) + 1 : 1;
            Redemptions.Add(new Redemption { redemption_id = newId, contract_id = 1, redemption_date = DateTime.Now, total_paid = 0M });
        }

        private void DeleteRedemption(object parameter)
        {
            if (SelectedRedemption != null) Redemptions.Remove(SelectedRedemption);
        }

        private bool CanDeleteRedemption(object parameter) => SelectedRedemption != null;

        private void SaveRedemptions(object parameter) => MessageBox.Show("Данные выкупов сохранены.", "Сохранение");
    }

    #endregion

    #region 2.8 ViewModel Покупки (BuyViewModel)

    public class BuyViewModel : BaseViewModel
    {
        public ObservableCollection<Buy> Buys { get; } = new ObservableCollection<Buy>();

        private Buy _selectedBuy;
        public Buy SelectedBuy
        {
            get => _selectedBuy;
            set
            {
                _selectedBuy = value;
                OnPropertyChanged(nameof(SelectedBuy));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public BuyViewModel()
        {
            Buys.Add(new Buy { buy_id = 1, item_id = 102, buy_price = 1000M, buy_date = DateTime.Now, client_id = 3 });

            AddCommand = new RelayCommand(AddBuy);
            DeleteCommand = new RelayCommand(DeleteBuy, CanDeleteBuy);
            SaveCommand = new RelayCommand(SaveBuys);
        }

        private void AddBuy(object parameter)
        {
            int newId = Buys.Any() ? Buys.Max(c => c.buy_id) + 1 : 1;
            Buys.Add(new Buy { buy_id = newId, item_id = 1, buy_price = 0M, buy_date = DateTime.Now, client_id = 1 });
        }

        private void DeleteBuy(object parameter)
        {
            if (SelectedBuy != null) Buys.Remove(SelectedBuy);
        }

        private bool CanDeleteBuy(object parameter) => SelectedBuy != null;

        private void SaveBuys(object parameter) => MessageBox.Show("Данные покупок сохранены.", "Сохранение");
    }

    #endregion

    #region 2.9 ViewModel Продажи (SaleViewModel)

    public class SaleViewModel : BaseViewModel
    {
        public ObservableCollection<Sale> Sales { get; } = new ObservableCollection<Sale>();

        private Sale _selectedSale;
        public Sale SelectedSale
        {
            get => _selectedSale;
            set
            {
                _selectedSale = value;
                OnPropertyChanged(nameof(SelectedSale));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public SaleViewModel()
        {
            Sales.Add(new Sale { sale_id = 1, item_id = 102, sale_price = 1500M, sale_date = DateTime.Now, sold_by_employee_id = 1 });

            AddCommand = new RelayCommand(AddSale);
            DeleteCommand = new RelayCommand(DeleteSale, CanDeleteSale);
            SaveCommand = new RelayCommand(SaveSales);
        }

        private void AddSale(object parameter)
        {
            int newId = Sales.Any() ? Sales.Max(c => c.sale_id) + 1 : 1;
            Sales.Add(new Sale { sale_id = newId, item_id = 1, sale_price = 0M, sale_date = DateTime.Now, sold_by_employee_id = 1 });
        }

        private void DeleteSale(object parameter)
        {
            if (SelectedSale != null) Sales.Remove(SelectedSale);
        }

        private bool CanDeleteSale(object parameter) => SelectedSale != null;

        private void SaveSales(object parameter) => MessageBox.Show("Данные продаж сохранены.", "Сохранение");
    }

    #endregion

    #region 2.10 ViewModel Заявки (RequestViewModel)

    public class RequestViewModel : BaseViewModel
    {
        public ObservableCollection<Request> Requests { get; } = new ObservableCollection<Request>();

        private Request _selectedRequest;
        public Request SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                _selectedRequest = value;
                OnPropertyChanged(nameof(SelectedRequest));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public RequestViewModel()
        {
            Requests.Add(new Request { request_id = 1, service_id = 1, requester_last_name = "Смирнов", requester_number = "89001234567", status = "Новая" });

            AddCommand = new RelayCommand(AddRequest);
            DeleteCommand = new RelayCommand(DeleteRequest, CanDeleteRequest);
            SaveCommand = new RelayCommand(SaveRequests);
        }

        private void AddRequest(object parameter)
        {
            int newId = Requests.Any() ? Requests.Max(c => c.request_id) + 1 : 1;
            Requests.Add(new Request { request_id = newId, service_id = 1, requester_last_name = "Новая", status = "Новая" });
        }

        private void DeleteRequest(object parameter)
        {
            if (SelectedRequest != null) Requests.Remove(SelectedRequest);
        }

        private bool CanDeleteRequest(object parameter) => SelectedRequest != null;

        private void SaveRequests(object parameter) => MessageBox.Show("Данные заявок сохранены.", "Сохранение");
    }

    #endregion

    #region 2.11 ViewModel Категории (CategoriesViewModel)

    public class CategoriesViewModel : BaseViewModel
    {
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public CategoriesViewModel()
        {
            Categories.Add(new Category { category_id = 1, category_name = "Золото", category_description = "Ювелирные изделия из золота" });

            AddCommand = new RelayCommand(AddCategory);
            DeleteCommand = new RelayCommand(DeleteCategory, CanDeleteCategory);
            SaveCommand = new RelayCommand(SaveCategories);
        }

        private void AddCategory(object parameter)
        {
            int newId = Categories.Any() ? Categories.Max(c => c.category_id) + 1 : 1;
            Categories.Add(new Category { category_id = newId, category_name = "Новая категория", category_description = "" });
        }

        private void DeleteCategory(object parameter)
        {
            if (SelectedCategory != null) Categories.Remove(SelectedCategory);
        }

        private bool CanDeleteCategory(object parameter) => SelectedCategory != null;

        private void SaveCategories(object parameter) => MessageBox.Show("Данные категорий сохранены.", "Сохранение");
    }

    #endregion

    // ====================================================================
    // 3. ГЛАВНАЯ VIEW MODEL (Управление навигацией)
    // ====================================================================

    #region 3.1 AdminViewModel - Навигация между ViewModels


    public class AdminViewModel : BaseViewModel
    {
        private int _currentUserId; // Поле для хранения ID вошедшего сотрудника
        public int CurrentUserId
        {
            get => _currentUserId;
            set
            {
                _currentUserId = value;
                OnPropertyChanged(nameof(CurrentUserId));
            }
        }

        private BaseViewModel _currentViewModel;
        // Используем readonly для словаря, который инициализируется только один раз
        private readonly Dictionary<string, BaseViewModel> _viewModels;

        /// <summary>
        /// Активная ViewModel, отображаемая в ContentControl.
        /// </summary>
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            private set // Приватный сеттер, чтобы менять только через метод ChangeView
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public AdminViewModel()
        {
            // Инициализация всех ViewModels для вкладок
            _viewModels = new Dictionary<string, BaseViewModel>
    {
        {"Клиенты", new ClientsViewModel()},
        {"Сотрудники", new EmployeesViewModel()},
        {"Товары", new ItemsViewModel()},
        {"Проценты", new RatesViewModel()},
        {"Контракты", new ContractsViewModel()},
        {"Продление", new ExtensionsViewModel()},
        {"Выкупы", new RedemptionViewModel()},
        {"Покупки", new BuyViewModel()},
        {"Продажи", new SaleViewModel()},
        {"Заявки", new RequestViewModel()},
        // Категории обычно не выводятся на отдельную вкладку, но если нужно:
        // {"Категории", new CategoriesViewModel()}, 

        
    };

            // Установка начальной активной вкладки
            CurrentViewModel = _viewModels["Клиенты"];
        }

        /// <summary>
        /// Переключает активное представление по имени вкладки.
        /// </summary>
        public void ChangeView(string viewName)
        {
            if (_viewModels.TryGetValue(viewName, out BaseViewModel viewModel))
            {
                CurrentViewModel = viewModel;
            }
        }
    }

    #endregion

    // ====================================================================
    // 4. CODE-BEHIND (Логика окна Admin.xaml.cs)
    // ====================================================================

    #region 4.1 Admin - Окно (Code-Behind)

    /// <summary>
    /// Логика взаимодействия для Admin.xaml (Code-Behind)
    /// </summary>
    public partial class Admin : Window
    {
        private Button _activeButton;
        private readonly AdminViewModel _viewModel; // Ссылка на главную ViewModel

        // Цвета (Кисти)
        private readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xAE, 0xB7, 0xAB));
        private readonly SolidColorBrush InactiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x7B, 0x6D));

        public Admin()
        {
            InitializeComponent();

            // Создание и установка главной ViewModel (DataContext)
            _viewModel = new AdminViewModel();
            this.DataContext = _viewModel;

            // Инициализация активной кнопки меню при старте
            Button clientsButton = FindName("clientsButton") as Button;
            if (clientsButton != null)
            {
                _activeButton = clientsButton;
                // Принудительно устанавливаем активный цвет для начальной кнопки
                _activeButton.Background = ActiveBrush;
            }
        }

        /// <summary>
        /// Обработчик клика по кнопкам меню (Клиенты, Сотрудники и т.д.)
        /// </summary>
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton && clickedButton.Content is string newTitle)
            {
                // 1. Изменение активной ViewModel (смена таблицы)
                _viewModel.ChangeView(newTitle);

                // 2. Сброс цвета предыдущей активной кнопки
                if (_activeButton != null)
                {
                    _activeButton.Background = InactiveBrush;
                }

                // 3. Установка активного цвета для новой кнопки и сохранение
                clickedButton.Background = ActiveBrush;
                _activeButton = clickedButton;

                // 4. Изменение заголовка
                TitleLabel.Content = newTitle;
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // Логика кнопки "Выход"
            Application.Current.MainWindow.Show();
            this.Close();
        }
    }

    #endregion
}