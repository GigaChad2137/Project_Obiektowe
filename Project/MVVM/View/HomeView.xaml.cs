using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Threading;


namespace Project.MVVM.View
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            Pokaz_wiadomosci.DataContext = $"Witaj!";
            refresh_nowe_wnioski();
            load_home_content();
        }
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        /*Funkcja tworzy nowy wątek oraz sprawdza zapytaniem do bazy danych co zawiera okleślone pole i na jego podstawie
        Wyświetla odpowiedni tekst*/
        private void load_home_content()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Tick += new EventHandler(refresh_nowe_wiadomosciThread);
            dispatcherTimer.Tick += new EventHandler(refresh_nowe_wnioskiThread);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
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
                    else if (czy_pracuje.Czy_pracuje == "L4")
                    {
                        Czy_pracuje.DataContext = "L4";
                    }
                    else
                    {
                        Czy_pracuje.DataContext = czy_pracuje.Czy_pracuje;
                    }
                }
            }
        }
        /* Po kliknięciu przycisku funkcja Pokazuje nowe okno */
        private void Pokaz_Wiadomosci(object sender, RoutedEventArgs e)
        {
            ChatView dashboard = new ChatView();
            dashboard.Show();
        }
        private void refresh_nowe_wiadomosciThread(object source, EventArgs e)
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
                    else if (przeczytane == 1)
                    {
                        Pokaz_wiadomosci.DataContext = $"Masz {przeczytane} nową wiadomości";
                    }
                    else
                    {
                        Pokaz_wiadomosci.DataContext = $"Masz {przeczytane} nowe wiadomości";
                    }
                }
            }
        }
        private void refresh_nowe_wnioskiThread(object source,EventArgs e)
        {
            using (DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                    if ((bool)Application.Current.Properties["currect_user_admin"] == true)
                    {
                        var przeczytane = db.user_wnioski.Where(a =>a.Status_Wniosku == null).Count();
                        if (przeczytane == 0)
                        {
                            Pokaz_wnioski.DataContext = $"    Brak nowych {Environment.NewLine}      Wniosków";
                        }
                        else if(przeczytane ==1)
                        {
                            Pokaz_wnioski.DataContext = $"  {przeczytane} Nowy Wniosek";
                        }
                        else
                        {
                            Pokaz_wnioski.DataContext = $"  {przeczytane} Nowe Wnioski";
                        }
                    }
                    else
                    {
                        var przeczytane = db.user_wnioski.Where(a => a.id_pracownika == id_currect_user && a.noti_c < 1 && a.Status_Wniosku != null).Count();
                        if (przeczytane == 0)
                        {
                            Pokaz_wnioski.DataContext = $"    Brak nowych {Environment.NewLine}      Wniosków";
                        }
                        else if ( przeczytane == 1)
                        {
                            Pokaz_wnioski.DataContext = $"  {przeczytane} Nowy Status {Environment.NewLine}       Wniosku";
                        }
                        else
                        {
                            Pokaz_wnioski.DataContext = $"    {przeczytane} Nowe Statusy {Environment.NewLine}        Wniosków";
                        }
                    }
                }
            }
        }
        private void Pokaz_Wnioski(object sender, RoutedEventArgs e)
        {
            if ((bool)Application.Current.Properties["currect_user_admin"] == true)
            {
                WnioskiVIewAdmin dashboard = new WnioskiVIewAdmin();
                dashboard.Show();
            }
            else
            {
                using (DBPROJECT db = new DBPROJECT())
                {
                    using (var contex = db.Database.BeginTransaction())
                    {
                        int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                        var statusy = db.user_wnioski.Where(x => x.id_pracownika == id_currect_user && x.Status_Wniosku != null && x.noti_c <3).ToList();
                        var notificationManager = new NotificationManager();
                        var starytype = NotificationType.Success;
                        foreach (var status in  statusy)
                        {
                            if(status.noti_c > 0)
                            {
                                 starytype = NotificationType.Information;
                            }
                            var znajdz_status = db.wnioski.Where(x => x.id == status.id_wniosku).First();
                            if (status.Status_Wniosku == true)
                            {
                                if (status.kwota == null)
                                {
                                    notificationManager.Show(new NotificationContent
                                    {
                                        Title = $"Wniosek o {znajdz_status.typ_wniosku}",
                                        Message = $"{status.Data_rozpoczecia.ToShortDateString()}- {status.Data_zakonczenia.ToShortDateString()}  {Environment.NewLine}Został zaakceptowany",
                                        Type = starytype
                                    });
                                }
                                else
                                {
                                    notificationManager.Show(new NotificationContent
                                    {
                                        Title = $"Wniosek o {znajdz_status.typ_wniosku}",
                                        Message = $"Nowa kwota wynagrodzenia: {status.kwota}zł  {Environment.NewLine}został zaakceptowany",
                                        Type = starytype
                                    });
                                }
                            }
                            if (status.Status_Wniosku == false)
                            {
                                if (status.noti_c > 0)
                                {
                                    starytype = NotificationType.Information;
                                }
                                else
                                {
                                    starytype = NotificationType.Error;
                                }
                                if (status.kwota == null)
                                {
                                    notificationManager.Show(new NotificationContent
                                    {
                                        Title = $"Wniosek o {znajdz_status.typ_wniosku}",
                                        Message = $"{status.Data_rozpoczecia.ToShortDateString()}- {status.Data_zakonczenia.ToShortDateString()}  {Environment.NewLine}został odrzucony",
                                        Type = starytype
                                    });
                                }
                                else
                                {
                                    notificationManager.Show(new NotificationContent
                                    {
                                        Title = $"Wniosek o {znajdz_status.typ_wniosku}",
                                        Message = $"Nowa kwota wynagrodzenia: {status.kwota}zł  {Environment.NewLine}został odrzucony",
                                        Type = starytype
                                    });
                                }
                            }
                            status.noti_c = status.noti_c + 1;
                            db.SaveChanges();
                        }
                        contex.Commit();
                    }
                }
            }
        }
        private void refresh_nowe_wnioski()
        {
            using (DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                    if ((bool)Application.Current.Properties["currect_user_admin"] == true)
                    {
                        var przeczytane = db.user_wnioski.Where(a => a.Status_Wniosku == null).Count();
                        if (przeczytane == 0)
                        {
                            Pokaz_wnioski.DataContext = $"    Brak nowych {Environment.NewLine}      Wniosków";
                        }
                        else if (przeczytane == 1)
                        {
                            Pokaz_wnioski.DataContext = $"  {przeczytane} Nowy Wniosek";
                        }
                        else
                        {
                            Pokaz_wnioski.DataContext = $"  {przeczytane} Nowe Wnioski";
                        }
                    }
                    else
                    {
                        var przeczytane = db.user_wnioski.Where(a => a.id_pracownika == id_currect_user && a.noti_c < 1 && a.Status_Wniosku != null).Count();
                        if (przeczytane == 0)
                        {
                            Pokaz_wnioski.DataContext = $"    Brak nowych {Environment.NewLine}      Wniosków";
                        }
                        else if (przeczytane == 1)
                        {
                            Pokaz_wnioski.DataContext = $"  {przeczytane} Nowy Status {Environment.NewLine}       Wniosku";
                        }
                        else
                        {
                            Pokaz_wnioski.DataContext = $"    {przeczytane} Nowe Statusy {Environment.NewLine}        Wniosków";
                        }
                    }
                }
            }
        }
        /* Po kliknięciu przycisku funkcja Pokazuje nowe okno */
        private void Wyślij_Wiadomość(object sender, RoutedEventArgs e)
        {
            ChatView dashboard = new ChatView();
            dashboard.Show();
        }
        /* Po kliknięciu przycisku funkcja Pokazuje nowe okno */
        private void Stwórz_Wniosek(object sender, RoutedEventArgs e)
        {
            WnioskiVIew dashboard = new WnioskiVIew();
            dashboard.Show();
        }
        /* Po kliknięciu przycisku funkcja Pokazuje nowe okno */
        private void Status_Pracy(object sender, RoutedEventArgs e)
        {
            WorkRaportPdf dashboard = new WorkRaportPdf();
            dashboard.Show();
        }
        private void Czy_Pracuje(object sender, RoutedEventArgs e)
        {   
            using(DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    DateTime thisDay = DateTime.Today;
                    int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                    var czy_pracuje = db.praca.First(x => x.Id_pracownika == id_currect_user && x.Data == thisDay);
                    DateTime Date_with_time = DateTime.Now;
                    if (czy_pracuje.Czy_pracuje == "Pracuje")
                    {
                        Czy_pracuje.DataContext = "Rozpocznij Prace";
                        czy_pracuje.Data_zakonczenia = Date_with_time;
                        czy_pracuje.Czy_pracuje = "Nie Pracuje";
                       
                    }
                    else if(czy_pracuje.Czy_pracuje == "Nie Pracuje")
                    {
                        Czy_pracuje.DataContext = "Zakończ Prace";
                        czy_pracuje.Data_rozpoczecia = Date_with_time;
                        czy_pracuje.Czy_pracuje = "Pracuje";
                    }
                    db.SaveChanges();
                    contex.Commit();
                }
            }
        }
    }
}
