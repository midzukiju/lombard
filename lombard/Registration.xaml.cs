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
            // Получаем введенные данные
            string input = InputTextBox.Text.Trim(); // .Trim() убирает лишние пробелы
            string password = PasswordInput.Password;

            // 1. Проверка на пустоту
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(password))
            {
                string missingField = "";

                if (string.IsNullOrEmpty(input))
                {
                    missingField = InputLabel.Content.ToString();
                }
                else if (string.IsNullOrEmpty(password))
                {
                    missingField = "пароль";
                }

                // Показываем предупреждение
                MessageBox.Show($"Пожалуйста, {missingField} и пароль.",
                                "Не все поля заполнены",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return; // Прекращаем выполнение метода, если поля пусты
            }


            if (input == "admin" && password == "123")
            {
                // Если логин и пароль совпадают с данными администратора, открываем окно Admin
                Admin adminWindow = new Admin();
                adminWindow.Show();
                this.Close();
            }
            else
            {
                // Если это не администратор, то пытаемся открыть обычный аккаунт
                Account account = new Account();
                account.Show();
                this.Close();
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
