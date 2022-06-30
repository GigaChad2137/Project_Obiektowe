using System.Windows;
using System.Windows.Input;

namespace Project
{
    public partial class UserMainWindow : Window
    {
       
        public UserMainWindow()
        {
            InitializeComponent();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginScreen dashboard = new LoginScreen();
            dashboard.Show();
            this.Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        private void CloseIt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }

    }
}
