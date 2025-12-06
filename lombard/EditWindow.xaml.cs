using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace lombard
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow(INotifyPropertyChanged item, string title)
        {
            InitializeComponent();

            var vm = new ItemEditorViewModel(item, title);
            this.DataContext = vm;

            this.Title = vm.WindowTitle;
        }
    }
    #region 1.12 Вспомогательная модель для динамической формы
      
        /// <summary>
        /// Описывает одно поле (свойство) сущности для динамической генерации формы.
        /// </summary>
        public class FieldDefinition
            {
                public string Label { get; set; }
                public string PropertyName { get; set; }
                public Type DataType { get; set; }
                public bool IsRequired { get; set; }
            }

    #endregion


    #region 2.12 Универсальная ViewModel для редактирования

    public class ItemEditorViewModel : BaseViewModel
    {
        public INotifyPropertyChanged CurrentItem { get; private set; }
        public ObservableCollection<FieldDefinition> FieldDefinitions { get; private set; }
        public string WindowTitle { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ItemEditorViewModel(INotifyPropertyChanged item, string title)
        {
            CurrentItem = item;
            WindowTitle = title;
            FieldDefinitions = new ObservableCollection<FieldDefinition>();

            GenerateFields(item);

            // Команды, привязанные к кнопкам "Сохранить" и "Отмена" в EditWindow
            SaveCommand = new RelayCommand(p => OnSave(p), p => true);
            CancelCommand = new RelayCommand(p => OnCancel(p), p => true);
        }

        private void GenerateFields(object item)
        {
            // *** Логика для ГЕНЕРАЦИИ полей в зависимости от типа ***

            if (item is Employee emp)
            {
                // Поля для Сотрудника (Employee)
                FieldDefinitions.Add(new FieldDefinition { Label = "ID:", PropertyName = nameof(emp.Id), DataType = typeof(int) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Фамилия:", PropertyName = nameof(emp.LastName), DataType = typeof(string), IsRequired = true });
                FieldDefinitions.Add(new FieldDefinition { Label = "Имя:", PropertyName = nameof(emp.FirstName), DataType = typeof(string), IsRequired = true });
                FieldDefinitions.Add(new FieldDefinition { Label = "Отчество:", PropertyName = nameof(emp.Patronymic), DataType = typeof(string), IsRequired = false });
                FieldDefinitions.Add(new FieldDefinition { Label = "Телефон:", PropertyName = nameof(emp.Number), DataType = typeof(string), IsRequired = true });
                FieldDefinitions.Add(new FieldDefinition { Label = "Почта:", PropertyName = nameof(emp.Email), DataType = typeof(string), IsRequired = true });
                FieldDefinitions.Add(new FieldDefinition { Label = "ID пользователя:", PropertyName = nameof(emp.UserId), DataType = typeof(string), IsRequired = true });
                FieldDefinitions.Add(new FieldDefinition { Label = "Создан:", PropertyName = nameof(emp.created_on), DataType = typeof(string), IsRequired = true });
            }
            else if (item is Client client)
            {
                // Поля для Клиента (Client)
                FieldDefinitions.Add(new FieldDefinition { Label = "ID:", PropertyName = nameof(client.Id), DataType = typeof(int) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Фамилия:", PropertyName = nameof(client.LastName), DataType = typeof(string), IsRequired = true });
                FieldDefinitions.Add(new FieldDefinition { Label = "Имя:", PropertyName = nameof(client.FirstName), DataType = typeof(string), IsRequired = true });
                FieldDefinitions.Add(new FieldDefinition { Label = "Отчество:", PropertyName = nameof(emp.Patronymic), DataType = typeof(string), IsRequired = false });
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата рождения:",
                    PropertyName = nameof(client.DateOfBirth), // ИСПОЛЬЗУЙТЕ ТОЧНОЕ ИМЯ СВОЙСТВА
                    DataType = typeof(DateTime?),              // Тип: DateTime с возможностью NULL (рекомендуется)
                    IsRequired = true                          // Сделайте поле обязательным
                });
                FieldDefinitions.Add(new FieldDefinition { Label = "Номер паспорта:", PropertyName = nameof(client.PassportNumber), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Серия паспорта:", PropertyName = nameof(client.PassportNumber), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Паспорт выдан:", PropertyName = nameof(client.PassportIssuedBy), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Дата выдачи:", PropertyName = nameof(client.PassportIssueDate), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Телефон:", PropertyName = nameof(client.Phone), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Почта:", PropertyName = nameof(client.Email), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "ID пользователя:", PropertyName = nameof(client.UserId), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Город:", PropertyName = nameof(client.City), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Улица:", PropertyName = nameof(client.Street), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Дом:", PropertyName = nameof(client.HouseNumber), DataType = typeof(string) });
                FieldDefinitions.Add(new FieldDefinition { Label = "Создан:", PropertyName = nameof(emp.created_on), DataType = typeof(string), IsRequired = true });
            }
            else if (item is Item inventoryItem)
            {
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Товара:",
                    PropertyName = nameof(inventoryItem.item_id),
                    DataType = typeof(int)
                });

                // ID Категории (ввод числом, без ComboBox)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Категории:",
                    PropertyName = nameof(inventoryItem.item_category_id),
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Название товара
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Название:",
                    PropertyName = nameof(inventoryItem.item_name),
                    DataType = typeof(string),
                    IsRequired = true
                });

                //описание
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Описание:",
                    PropertyName = nameof(inventoryItem.item_description),
                    DataType = typeof(string),
                    IsRequired = false
                });

                // Оценочная стоимость (Deciaml/Money)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Оценочная цена:",
                    PropertyName = nameof(inventoryItem.item_estimated_price),
                    DataType = typeof(decimal),
                    IsRequired = true
                });

                // Рыночная стоимость (Decimal/Money)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Рыночная цена:",
                    PropertyName = nameof(inventoryItem.item_market_price),
                    DataType = typeof(decimal),
                    IsRequired = true
                });

                //изображение
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Путь к изображению:",
                    PropertyName = nameof(inventoryItem.item_image),
                    DataType = typeof(string),
                    IsRequired = false
                });

                // Дата создания/регистрации товара (DateTime)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата создания:",
                    PropertyName = nameof(inventoryItem.created_on),
                    DataType = typeof(DateTime?), // Используем nullable DateTime
                    IsRequired = false
                });

            }
            else if (item is Rate rate)
            {

                // ID Ставки (только для чтения)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Ставки:",
                    PropertyName = nameof(rate.rate_id),
                    DataType = typeof(int)
                });

                // ID категории
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Контракта:",
                    PropertyName = nameof(rate.category_id),
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Мин. дней
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата начала действия:",
                    PropertyName = nameof(rate.min_days),
                    DataType = typeof(DateTime?),
                    IsRequired = true
                });

                // Макс. дней
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата начала действия:",
                    PropertyName = nameof(rate.max_days),
                    DataType = typeof(DateTime?),
                    IsRequired = true
                });

                // Значение процента (Decimal/double)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Процентная ставка (%):",
                    PropertyName = nameof(rate.interest_rate),
                    DataType = typeof(decimal), // Используем decimal для точности
                    IsRequired = true
                });
            }
            else if (item is Contract contract)
            {
                // *** БЛОК ДЛЯ КОНТРАКТА (Contract) ***

                // ID Контракта (только для чтения)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Контракта:",
                    PropertyName = nameof(contract.contract_id),
                    DataType = typeof(int)
                });

                // ID Клиента (ForeignKey)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Клиента:",
                    PropertyName = nameof(contract.client_id),
                    DataType = typeof(int),
                    IsRequired = true
                });

                // ID Сотрудника (ForeignKey)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Сотрудника:",
                    PropertyName = nameof(contract.employee_id),
                    DataType = typeof(int),
                    IsRequired = true
                });

                // ID Товара (ForeignKey)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Товара:",
                    PropertyName = nameof(contract.item_id),
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Номер контракта
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Номер контракта:",
                    PropertyName = nameof(contract.contract_number),
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Сумма контракта (Decimal/Money)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Сумма контракта:",
                    PropertyName = nameof(contract.pawn_value),
                    DataType = typeof(decimal),
                    IsRequired = true
                });

                // Сумма выкупа (Decimal/Money)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Сумма выкупа:",
                    PropertyName = nameof(contract.loan_amount),
                    DataType = typeof(decimal),
                    IsRequired = true
                });

                // Дата выдачи/начала действия контракта
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата выдачи:",
                    PropertyName = nameof(contract.issue_date),
                    DataType = typeof(DateTime?),
                    IsRequired = true
                });

                // Дата истечения срока действия контракта
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата истечения:",
                    PropertyName = nameof(contract.maturity_date),
                    DataType = typeof(DateTime?),
                    IsRequired = true
                });

                // Статус контракта (например, "Активен", "Закрыт")
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Статус:",
                    PropertyName = nameof(contract.status),
                    DataType = typeof(string),
                    IsRequired = true
                });

                // Дата создания/регистрации товара (DateTime)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата создания:",
                    PropertyName = nameof(inventoryItem.created_on),
                    DataType = typeof(DateTime?), // Используем nullable DateTime
                    IsRequired = false
                });
            }
            else if (item is Extension extension)
            {

                // ID Продления (только для чтения)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Продления:",
                    PropertyName = nameof(extension.extension_id), 
                    DataType = typeof(int)
                });

                // ID Контракта (Внешний ключ)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Контракта:",
                    PropertyName = nameof(extension.contract_id), 
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Старая дата погашения
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Старая дата погашения:",
                    PropertyName = nameof(extension.old_maturity_date), 
                    DataType = typeof(DateTime?),
                    IsRequired = true
                });

                // Новая дата погашения
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Новая дата погашения:",
                    PropertyName = nameof(extension.new_maturity_date), 
                    DataType = typeof(DateTime?),
                    IsRequired = true
                });

                // Дата создания записи о продлении
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата создания записи:",
                    PropertyName = nameof(extension.created_on), 
                    DataType = typeof(DateTime?),
                    IsRequired = false
                });

                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Сотрудника (создатель):",
                    PropertyName = nameof(extension.employee_id), // Убедитесь, что это свойство существует в Extension
                    DataType = typeof(int),
                    IsRequired = true
                });
            }
            else if (item is Redemption redemption)
            {

                // ID Выкупа (только для чтения)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Выкупа:",
                    PropertyName = nameof(redemption.redemption_id), // Используем _redemptionId
                    DataType = typeof(int)
                });

                // ID Контракта (Внешний ключ)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Контракта:",
                    PropertyName = nameof(redemption.contract_id), // Используем _contractId
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Дата фактического выкупа
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата выкупа:",
                    PropertyName = nameof(redemption.redemption_date), // Используем _redemptionDate
                    DataType = typeof(DateTime?),
                    IsRequired = true
                });

                // Общая сумма оплаты
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Общая сумма оплаты:",
                    PropertyName = nameof(redemption.total_paid), // Используем _totalPaid
                    DataType = typeof(decimal),
                    IsRequired = true
                });

                // ID Сотрудника, принявшего платеж (Аудит)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Сотрудника (принявшего платеж):",
                    PropertyName = nameof(redemption.redeemed_by_employee_id), // Используем _employeeId
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Дата создания записи
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата создания записи:",
                    PropertyName = nameof(redemption.created_on), // Используем _createdOn
                    DataType = typeof(DateTime?),
                    IsRequired = false
                });
            }
            else if (item is Buy purchase) // Используем 'purchase' для ясности
            {
                // *** БЛОК ДЛЯ ПОКУПКИ (Buy) ***

                // ID Покупки (только для чтения)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Покупки:",
                    PropertyName = nameof(purchase.buy_id), // Используем _buyId
                    DataType = typeof(int)
                });

                // ID Товара (Внешний ключ)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Товара:",
                    PropertyName = nameof(purchase.item_id), // Используем _itemId
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Цена, за которую ломбард купил товар
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Цена покупки:",
                    PropertyName = nameof(purchase.buy_price), // Используем _buyPrice
                    DataType = typeof(decimal),
                    IsRequired = true
                });

                // Дата покупки
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата покупки:",
                    PropertyName = nameof(purchase.buy_date), // Используем _buyDate
                    DataType = typeof(DateTime?),
                    IsRequired = true
                });

                // ID Клиента (тот, кто продал товар ломбарду)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Клиента (продавца):",
                    PropertyName = nameof(purchase.client_id), // Используем _clientId
                    DataType = typeof(int),
                    IsRequired = true
                });

                // ID Сотрудника (тот, кто оформил покупку)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Сотрудника (оформителя):",
                    PropertyName = nameof(purchase.buy_by_employee_id), // Используем _employeeId
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Дата создания записи (аудит)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата создания записи:",
                    PropertyName = nameof(purchase.created_on), // Используем _createdOn
                    DataType = typeof(DateTime?),
                    IsRequired = false
                });
            }
            else if (item is Sale sale)
            {
                // *** БЛОК ДЛЯ ПРОДАЖИ (Sale) ***

                // 1. ID Продажи (только для чтения)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Продажи:",
                    PropertyName = nameof(sale.sale_id),
                    DataType = typeof(int)
                });

                // 2. ID Товара (Внешний ключ)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Товара:",
                    PropertyName = nameof(sale.item_id),
                    DataType = typeof(int),
                    IsRequired = true
                });

                // 3. Дата продажи
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата продажи:",
                    PropertyName = nameof(sale.sale_date),
                    DataType = typeof(DateTime?), // Используем DateTime? для DatePicker
                    IsRequired = true
                });

                // 4. Цена продажи
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Цена продажи:",
                    PropertyName = nameof(sale.sale_price),
                    DataType = typeof(decimal),
                    IsRequired = true
                });

                // 5. ID Клиента (покупателя) - может быть null
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Клиента (покупатель):",
                    PropertyName = nameof(sale.client_id),
                    DataType = typeof(int?), // Важно: для nullable int
                    IsRequired = false
                });

                // 6. ID Сотрудника (тот, кто оформил продажу)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Сотрудника (оформителя):",
                    PropertyName = nameof(sale.sold_by_employee_id),
                    DataType = typeof(int),
                    IsRequired = true
                });

                // 7. Дата создания записи (аудит)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата создания записи:",
                    PropertyName = nameof(sale.created_on),
                    DataType = typeof(DateTime?),
                    IsRequired = false
                });
            }
            else if (item is Request request) // Используем 'application' для Заявки
            {
                // *** БЛОК ДЛЯ ЗАЯВКИ (Application) ***

                // ID Заявки (только для чтения)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Заявки:",
                    PropertyName = nameof(request.request_id), // Используем _requestId
                    DataType = typeof(int)
                });

                // ID Услуги (Внешний ключ)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "ID Услуги:",
                    PropertyName = nameof(request.service_id), // Используем _serviceId
                    DataType = typeof(int),
                    IsRequired = true
                });

                // Фамилия заявителя
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Фамилия заявителя:",
                    PropertyName = nameof(request.requester_last_name), // Используем _requesterLastName
                    DataType = typeof(string),
                    IsRequired = true
                });

                // Имя заявителя
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Имя заявителя:",
                    PropertyName = nameof(request.requester_first_name), // Используем _requesterFirstName
                    DataType = typeof(string),
                    IsRequired = true
                });

                // Отчество заявителя
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Отчество заявителя:",
                    PropertyName = nameof(request.requester_patronymic), // Используем _requesterPatronymic
                    DataType = typeof(string),
                    IsRequired = false // Отчество обычно необязательно
                });

                // Контактный номер заявителя
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Контактный номер:",
                    PropertyName = nameof(request.requester_number), // Используем _requesterNumber
                    DataType = typeof(string),
                    IsRequired = true
                });

                // Город заявителя
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Город заявителя:",
                    PropertyName = nameof(request.requester_city), // Используем _requesterCity
                    DataType = typeof(string),
                    IsRequired = false
                });

                // Дата создания записи (аудит)
                FieldDefinitions.Add(new FieldDefinition
                {
                    Label = "Дата создания записи:",
                    PropertyName = nameof(request.created_on), // Используем _createdOn
                    DataType = typeof(DateTime?),
                    IsRequired = false
                });
            }

        }

        private void OnSave(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void OnCancel(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }

    #endregion
}
