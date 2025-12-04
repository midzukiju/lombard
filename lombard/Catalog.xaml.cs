using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lombard
{
    /// <summary>
    /// Логика взаимодействия для Catalog.xaml
    /// </summary>
    public partial class Catalog : Window
    {
      
        private const string PlaceholderText = "Изделие, бренд...";


        private readonly SolidColorBrush PlaceholderBrush = new SolidColorBrush(Color.FromArgb(0xAD, 0x70, 0x7B, 0x6D));
        
        private readonly SolidColorBrush TextBrush = new SolidColorBrush(Color.FromRgb(0x26, 0x2D, 0x24));

        public Catalog()
        {
            InitializeComponent();

            SearchTextBox.Text = PlaceholderText;
            SearchTextBox.Foreground = PlaceholderBrush;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == PlaceholderText)
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = TextBrush;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = PlaceholderText;
                SearchTextBox.Foreground = PlaceholderBrush;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Close();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Basket basket = new Basket();
            basket.Show();
            this.Close();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Favourites favourites = new Favourites();
            favourites.Show();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}