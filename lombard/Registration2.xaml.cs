using System;
using System.Windows;
using System.Windows.Media;
using MySqlConnector;

namespace lombard
{
    public partial class Registration2 : Window
    {
        private readonly SolidColorBrush ActiveBrush;
        private readonly SolidColorBrush PhoneActiveBrush;
        private readonly SolidColorBrush InactiveBrush;

        // Строка подключения к базе данных
        private const string ConnectionString =
            "Server=tompsons.beget.tech;Port=3306;Database=tompsons_stud03;User=tompsons_stud03;Password=10230901Sd;SslMode=Preferred;ConvertZeroDateTime=True;";

        // Флаг режима ввода (email или телефон)
        private bool _isEmailMode = true;

        public Registration2()
        {
            InitializeComponent();

            ActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x7B, 0x6D));
            ActiveBrush.Opacity = 0.50;

            PhoneActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x7B, 0x6D));
            PhoneActiveBrush.Opacity = 0.50;

            InactiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDD, 0xDD, 0xDD));

            SetLoginMode();
        }

        private void SetLoginMode()
        {
            MailLoginButton.Background = ActiveBrush;
            PhoneButton.Background = InactiveBrush;
            InputLabel.Content = "Введите логин (email)";
            _isEmailMode = true;
        }

        private void SetPhoneMode()
        {
            PhoneButton.Background = PhoneActiveBrush;
            MailLoginButton.Background = InactiveBrush;
            InputLabel.Content = "Введите номер телефона";
            _isEmailMode = false;
        }

        private void MailLoginButton_Click(object sender, RoutedEventArgs e)
        {
            SetLoginMode();
        }

        private void PhoneButton_Click(object sender, RoutedEventArgs e)
        {
            SetPhoneMode();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string surname = SurnameTextBox.Text.Trim();
            string patronymic = PatronymicTextBox.Text.Trim();
            DateTime? dateOfBirth = DateOfBirthPicker.SelectedDate;
            string contactInfo = InputTextBox.Text.Trim();
            string password = PasswordInput.Password;

            // Проверка обязательных полей
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(surname) ||
                dateOfBirth == null)
            {
                MessageBox.Show("Пожалуйста, заполните имя, фамилию и укажите дату рождения.",
                                "Не все поля заполнены",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(contactInfo) || string.IsNullOrEmpty(password))
            {
                string inputType = _isEmailMode ? "email" : "телефон";
                MessageBox.Show($"Пожалуйста, введите {inputType} и пароль.",
                                "Не все поля заполнены",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Проверка на уникальность email/телефона
                if (!IsContactInfoUnique(contactInfo))
                {
                    string inputType = _isEmailMode ? "email" : "телефон";
                    MessageBox.Show($"Этот {inputType} уже зарегистрирован. Пожалуйста, используйте другой {inputType}.",
                                    "Ошибка регистрации",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                // Сохранение пользователя в базу данных
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();

                    // SQL-запрос для вставки данных
                    string query = _isEmailMode ?
                        @"INSERT INTO clients (first_name, last_name, patronymic, date_of_birth, email, created_on) 
                          VALUES (@first_name, @last_name, @patronymic, @date_of_birth, @email, @created_on)" :
                        @"INSERT INTO clients (first_name, last_name, patronymic, date_of_birth, phone, created_on) 
                          VALUES (@first_name, @last_name, @patronymic, @date_of_birth, @phone, @created_on)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@first_name", name);
                        cmd.Parameters.AddWithValue("@last_name", surname);
                        cmd.Parameters.AddWithValue("@patronymic", string.IsNullOrEmpty(patronymic) ? (object)DBNull.Value : patronymic);
                        cmd.Parameters.AddWithValue("@date_of_birth", dateOfBirth.Value);

                        if (_isEmailMode)
                        {
                            cmd.Parameters.AddWithValue("@email", contactInfo);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@phone", contactInfo);
                        }

                        cmd.Parameters.AddWithValue("@created_on", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Регистрация успешно завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Переход в личный кабинет
                Account account = new Account();
                account.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private bool IsContactInfoUnique(string contactInfo)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = _isEmailMode ?
                        "SELECT COUNT(*) FROM clients WHERE email = @contactInfo" :
                        "SELECT COUNT(*) FROM clients WHERE phone = @contactInfo";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@contactInfo", contactInfo);
                        return Convert.ToInt32(cmd.ExecuteScalar()) == 0;
                    }
                }
            }
            catch
            {
                // В случае ошибки подключения считаем, что проверка не пройдена
                return false;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }
    }
}