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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project.MVVM.View
{
    /// <summary>
    /// Logika interakcji dla klasy HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            Pokaz_wiadomosci.DataContext = "todo sprawdz ile nowych wiadomosci";
            Pokaz_wnioski.DataContext = "to do sprawdz ile wnioskow";
            Pokaz_wnioski.DataContext = "to do sprawdz czy pracuje";
        }

        private void Pokaz_Wiadomosci(object sender, RoutedEventArgs e)
        {
            ChatView dashboard = new ChatView();
            dashboard.Show();   
    


        }

        private void Pokaz_Wnioski(object sender, RoutedEventArgs e)
        {
            Pokaz_wnioski.DataContext = "todo pokaż wnioski";
        }

        private void Wyślij_Wiadomość(object sender, RoutedEventArgs e)
        {

        }
        private void Stwórz_Wniosek(object sender, RoutedEventArgs e)
        {

        }
        private void Status_Pracy(object sender, RoutedEventArgs e)
        {

        }
        private void Czy_Pracuje(object sender, RoutedEventArgs e)
        {
            if(Convert.ToString(Czy_pracuje.DataContext)=="Zakończ Prace")
            {
                Czy_pracuje.DataContext = "Rozpocznij Pracę";
            }
            else
            {
                Czy_pracuje.DataContext = "Zakończ Prace";
            }
            
        }
    }
}
