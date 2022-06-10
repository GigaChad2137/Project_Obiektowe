﻿using System;
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
            Pokaz_wnioski.DataContext = "to do sprawdz czy pracuje";
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
           
            Pokaz_wnioski.DataContext = "todo pokaż wnioski";
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
