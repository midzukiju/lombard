using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lombard
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        private Button _activeButton;

        // Определяем цвета (Кисти)
        private readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xAE, 0xB7, 0xAB)); // #FFAEB7AB (Клиенты)
        private readonly SolidColorBrush InactiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x7B, 0x6D));
        public Admin()
        {
            InitializeComponent();
            Button clientsButton = FindName("clientsButton") as Button; 
            if (clientsButton != null)
            {
                _activeButton = clientsButton;
            }
        }

        
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            
            Button clickedButton = sender as Button;


            if (clickedButton != null && clickedButton.Content != null)
            {
                // 1. Сброс цвета предыдущей активной кнопки
                if (_activeButton != null)
                {
                    _activeButton.Background = InactiveBrush;
                }

                // 2. Установка активного цвета для новой кнопки
                clickedButton.Background = ActiveBrush;

                // 3. Сохранение новой активной кнопки
                _activeButton = clickedButton;

                // Изменение заголовка
                string newTitle = clickedButton.Content.ToString();
                TitleLabel.Content = newTitle;
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

        }
    }
}