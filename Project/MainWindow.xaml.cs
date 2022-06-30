using System.Windows;
using System.Windows.Input;

namespace Project
{
    public partial class MainWindow : Window
    {
        public MainWindow( )
        {
            InitializeComponent();
        }

        /* Funkcja wywoływana po naciśnięciu przycisku ma za zadanie zamknąć bierzące okno oraz wyłączyć aplikacje  */
        private void CloseIt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }
        /* Funkcja wywoływana po naciśnięciu lewego przycisku myszki i przytrzymanie go ma za zadanie umożliwić przesuwanie okna */
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginScreen dashboard = new LoginScreen();
            dashboard.Show();
            this.Close();
        }
    }
}
