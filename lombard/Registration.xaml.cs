using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private readonly SolidColorBrush ActiveBrush;
        private readonly SolidColorBrush PhoneActiveBrush;
        private readonly SolidColorBrush InactiveBrush;
        public Registration()
        {
            InitializeComponent();

            ActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x7B, 0x6D));
            ActiveBrush.Opacity = 0.50;

            PhoneActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x7B, 0x6D));
            PhoneActiveBrush.Opacity = 0.50;

            InactiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDD, 0xDD, 0xDD));

           
            SetLoginMode();
        }
        

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text.Trim();
            string password = PasswordInput.Password;

            // Проверка на пустоту
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин (или телефон) и пароль.",
                                "Не все поля заполнены",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            const string ConnectionString =
                "Server=tompsons.beget.tech;Port=3306;Database=tompsons_stud03;User=tompsons_stud03;Password=10230901Sd;SslMode=Preferred;ConvertZeroDateTime=True;";

            try
            {
                using (var conn = new MySqlConnector.MySqlConnection(ConnectionString))
                {
                    conn.Open();

                    // Ищем пользователя по логину 
                    string query = @"
                SELECT Id, password, role_id 
                FROM users 
                WHERE (login = @input)
                LIMIT 1";

                    using (var cmd = new MySqlConnector.MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@input", input);

                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            string storedPassword = reader.GetString("password");
                            int roleId = reader.GetInt32("role_id");
                            int userId = reader.GetInt32("Id");

                            // Сравниваем пароль (временно — в открытом виде)
                            if (password == storedPassword)
                            {
                                reader.Close();

                                // Открываем нужную панель в зависимости от роли
                                switch (roleId)
                                {
                                    case 1: // Админ
                                        var adminWindow = new Admin();
                                        adminWindow.SetUserRole("admin");
                                        adminWindow.Show();
                                        break;

                                    case 2: // Оценщик
                                        var appraiserWindow = new Admin();
                                        appraiserWindow.SetUserRole("appraiser");
                                        appraiserWindow.Show();
                                        break;

                                    case 3: // Клиент
                                        var accountWindow = new Account();
                                        // Передай userId, если нужно для загрузки данных
                                        accountWindow.Show();
                                        break;

                                    default:
                                        MessageBox.Show("Неизвестная роль пользователя.",
                                                        "Ошибка",
                                                        MessageBoxButton.OK,
                                                        MessageBoxImage.Error);
                                        return;
                                }

                                this.Close();
                                return;
                            }
                        }

                        // Если дошли сюда — пользователь не найден или пароль неверен
                        MessageBox.Show("Неверный логин/телефон или пароль.",
                                        "Ошибка авторизации",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }



        private void SetLoginMode()
        {
            MailLoginButton.Background = ActiveBrush;

            PhoneButton.Background = InactiveBrush;

            InputLabel.Content = "Введите логин";
        }

        private void SetPhoneMode()
        {

            PhoneButton.Background = PhoneActiveBrush;

            MailLoginButton.Background = InactiveBrush;

            InputLabel.Content = "Введите номер";
        }

        private void MailLoginButton_Click(object sender, RoutedEventArgs e)
        {
            SetLoginMode();
        }

        private void PhoneButton_Click(object sender, RoutedEventArgs e)
        {
            SetPhoneMode();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Registration2 registration2 = new Registration2();
            registration2.Show();
            this.Close();
        }


    }

}
