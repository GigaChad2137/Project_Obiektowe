using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

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
            Pokaz_wiadomosci.DataContext = $"Witaj!";
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(refresh_nowe_wiadomosci);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
            Pokaz_wnioski.DataContext = "to do sprawdz ile wnioskow";
            using (DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    DateTime thisDay = DateTime.Today;
                    int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                    var czy_pracuje = db.praca.First(x => x.Id_pracownika == id_currect_user && x.Data == thisDay);
                    if (czy_pracuje.Czy_pracuje == "Pracuje")
                    {
                        Czy_pracuje.DataContext = "Zakończ Prace";
                    }
                    else if (czy_pracuje.Czy_pracuje == "Urlop")
                    {
                        Czy_pracuje.DataContext = "Urlop";
                    }
                    else if (czy_pracuje.Czy_pracuje == "Nie Pracuje")
                    {
                        Czy_pracuje.DataContext = "Rozpocznij Prace";
                    }
                }
            }
        }

        private void Pokaz_Wiadomosci(object sender, RoutedEventArgs e)
        {
            ChatView dashboard = new ChatView();
            dashboard.Show();
        }
        private void refresh_nowe_wiadomosci(object source, EventArgs e)
        {
            using (DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                    var przeczytane = db.wiadomosci.Where(a => a.id_odbiorcy == id_currect_user && a.czy_przeczytane==false).Count();
                    if(przeczytane == 0)
                    {
                        Pokaz_wiadomosci.DataContext = $"Masz {przeczytane} nowych wiadomości";
                    }
                    else
                    {
                        Pokaz_wiadomosci.DataContext = $"Masz {przeczytane} nowe wiadomości";
                    }
                }
            }
          
        }

        private void Pokaz_Wnioski(object sender, RoutedEventArgs e)
        {

            WnioskiVIew dashboard = new WnioskiVIew();
            dashboard.Show();
        }

        private void Wyślij_Wiadomość(object sender, RoutedEventArgs e)
        {
            ChatView dashboard = new ChatView();
            dashboard.Show();
        }
        private void Stwórz_Wniosek(object sender, RoutedEventArgs e)
        {

        }
        private void Status_Pracy(object sender, RoutedEventArgs e)
        {

        }
        private void Czy_Pracuje(object sender, RoutedEventArgs e)
        {   
            using(DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    DateTime thisDay = DateTime.Today;
                    Trace.WriteLine(thisDay);
                    int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                    var czy_pracuje = db.praca.First(x => x.Id_pracownika == id_currect_user && x.Data == thisDay);
                    DateTime Date_with_time = DateTime.Now;
                    Trace.WriteLine(Date_with_time);
                    if (czy_pracuje.Czy_pracuje == "Pracuje")
                    {
                        Czy_pracuje.DataContext = "Rozpocznij Prace";
                        czy_pracuje.Data_zakonczenia = Date_with_time;
                        Trace.WriteLine(" pracuje if");
                        Trace.WriteLine("Data roz" + czy_pracuje.Data_rozpoczecia);
                        Trace.WriteLine("Data zak" + czy_pracuje.Data_zakonczenia);
                        czy_pracuje.Czy_pracuje = "Nie Pracuje";
                       
                    }
                    else if(czy_pracuje.Czy_pracuje == "Urlop")
                    {
                        Czy_pracuje.DataContext = "Urlop";
                    }
                    
                    else if(czy_pracuje.Czy_pracuje == "Nie Pracuje")
                    {
                        Czy_pracuje.DataContext = "Zakończ Prace";
                        czy_pracuje.Data_rozpoczecia = Date_with_time;
                        Trace.WriteLine("Nie pracuje if");
                        Trace.WriteLine("Data roz"+czy_pracuje.Data_rozpoczecia);
                        Trace.WriteLine("Data zak"+czy_pracuje.Data_zakonczenia);
                        czy_pracuje.Czy_pracuje = "Pracuje";
                  

                    }
                    db.SaveChanges();
                    contex.Commit();

                }
            }
           
            
        }
    }
}
