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
    /// Логика взаимодействия для Registration2.xaml
    /// </summary>
    public partial class Registration2 : Window
    {
        private readonly SolidColorBrush ActiveBrush;
        private readonly SolidColorBrush PhoneActiveBrush;
        private readonly SolidColorBrush InactiveBrush;
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
            string input = InputTextBox.Text.Trim(); 
            string password = PasswordInput.Password;


            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(patronymic) ||
                dateOfBirth == null)
            {
                MessageBox.Show("Пожалуйста, заполните полностью ФИО и укажите дату рождения.",
                                "Не все поля заполнены",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

       
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(password))
            {
             
                string inputType = InputLabel.Content.ToString();

                MessageBox.Show($"Пожалуйста, {inputType} и пароль.",
                                "Не все поля заполнены",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Регистрация успешно завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            Account account = new Account();
            account.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }
    }
}
