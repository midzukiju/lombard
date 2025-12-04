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
    /// Логика взаимодействия для Request.xaml
    /// </summary>
    public partial class Request : Window
    {
        public Request()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            string surn = FTextBox.Text.Trim();   
            string name = ITextBox.Text.Trim();  
            string patr = PTextBox.Text.Trim();   
            string phone = PhoneTextBox.Text.Trim();
            string city = CityTextBox.Text.Trim();

            string fText = FTextBox.Text.Trim();
            string iText = ITextBox.Text.Trim();
            string pText = PTextBox.Text.Trim();

            if (string.IsNullOrEmpty(fText) ||
                string.IsNullOrEmpty(iText) ||
                string.IsNullOrEmpty(pText) ||
                string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(city))
            {
                
                MessageBox.Show("Пожалуйста, заполните все поля заявки (ФИО, Телефон, Город).",
                                "Предупреждение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return; 
            }

            MessageBox.Show("Заявка успешно отправлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            FTextBox.Text = string.Empty;
            ITextBox.Text = string.Empty;
            PTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            CityTextBox.Text = string.Empty;

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }

    }
}
