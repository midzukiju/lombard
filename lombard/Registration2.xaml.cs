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
                string inputType = _isEmailMode ? "логин/email" : "телефон";
                MessageBox.Show($"Пожалуйста, введите {inputType} и пароль.",
                                "Не все поля заполнены",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        // === 1. Проверка: не занят ли логин/телефон ===
                        string checkQuery = _isEmailMode
                            ? "SELECT COUNT(*) FROM users WHERE login = @contact"
                            : "SELECT COUNT(*) FROM users WHERE phone = @contact";

                        using (var checkCmd = new MySqlCommand(checkQuery, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@contact", contactInfo);
                            if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                            {
                                string inputType = _isEmailMode ? "логин" : "телефон";
                                MessageBox.Show($"Этот {inputType} уже зарегистрирован.",
                                                "Ошибка регистрации",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                                return;
                            }
                        }

                        // === 2. Вставка в таблицу USERS ===
                        string insertUserQuery = _isEmailMode
                            ? "INSERT INTO users (login, password, role_id) VALUES (@login, @password, 3); SELECT LAST_INSERT_ID();"
                            : "INSERT INTO users (phone, password, role_id) VALUES (@phone, @password, 3); SELECT LAST_INSERT_ID();";

                        long userId;
                        using (var userCmd = new MySqlCommand(insertUserQuery, conn, transaction))
                        {
                            if (_isEmailMode)
                            {
                                userCmd.Parameters.AddWithValue("@login", contactInfo);
                            }
                            else
                            {
                                userCmd.Parameters.AddWithValue("@phone", contactInfo);
                            }
                            userCmd.Parameters.AddWithValue("@password", password); // ← временно в открытом виде
                            

                            userId = Convert.ToInt64(userCmd.ExecuteScalar());
                        }

                        // === 3. Вставка в CLIENTS с user_id ===
                        string insertClientQuery = @"
                    INSERT INTO clients (
                        last_name, first_name, patronymic, date_of_birth,
                        user_id, created_on
                    ) VALUES (
                        @last_name, @first_name, @patronymic, @date_of_birth,
                        @user_id, @created_on
                    )";

                        using (var clientCmd = new MySqlCommand(insertClientQuery, conn, transaction))
                        {
                            clientCmd.Parameters.AddWithValue("@last_name", surname);
                            clientCmd.Parameters.AddWithValue("@first_name", name);
                            clientCmd.Parameters.AddWithValue("@patronymic", string.IsNullOrEmpty(patronymic) ? (object)DBNull.Value : patronymic);
                            clientCmd.Parameters.AddWithValue("@date_of_birth", dateOfBirth.Value);
                            clientCmd.Parameters.AddWithValue("@user_id", userId);
                            clientCmd.Parameters.AddWithValue("@created_on", DateTime.Now);

                            clientCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                MessageBox.Show("Регистрация успешно завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Переход в личный кабинет
                var account = new Account();
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