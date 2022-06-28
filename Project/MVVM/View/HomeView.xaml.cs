﻿using Notifications.Wpf;
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
    /// <summary>
    /// Logika interakcji dla klasy HomeView.xaml
    /// </summary>

    public partial class HomeView : UserControl
    {
       

        public HomeView()
        {
            InitializeComponent();
            Pokaz_wiadomosci.DataContext = $"Witaj!";
         //   load_home_content();
         //   refresh_nowe_wnioski();



        }

        private void load_home_content()
        {
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
                    else if (czy_pracuje.Czy_pracuje == "L4")
                    {
                        Czy_pracuje.DataContext = "L4";
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
        private void refresh_nowe_wnioski()
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
                     
                        var przeczytane = db.user_wnioski.Where(a => a.id_pracownika == id_currect_user && a.noti_c < 1).Count();
                        if (przeczytane == 0)
                        {
                            Pokaz_wnioski.DataContext = $"Wnioski";
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
        private void Wyślij_Wiadomość(object sender, RoutedEventArgs e)
        {
            ChatView dashboard = new ChatView();
            dashboard.Show();
        }
        private void Stwórz_Wniosek(object sender, RoutedEventArgs e)
        {
            WnioskiVIew dashboard = new WnioskiVIew();
            dashboard.Show();

        }
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
