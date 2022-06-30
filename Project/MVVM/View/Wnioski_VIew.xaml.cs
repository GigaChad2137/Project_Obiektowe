using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Project.MVVM.View
{
    public partial class WnioskiVIew : Window
    {
        public WnioskiVIew()
        {
            InitializeComponent();
            BindUserlist();
        }
        public List<wnioski> wniosek { get; set; } //lista utworzona do Combobox'a
        /*Funkcja bindująca tabele wnioski do combobox'a  */
        private void BindUserlist()
        {
            using (var db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var item = db.wnioski.ToList();
                    wniosek = item;
                    //       DataContext = wniosek;
                    Send_do_kogo.ItemsSource = wniosek;
                    Send_do_kogo.DisplayMemberPath = "typ_wniosku";
                    Send_do_kogo.SelectedValuePath = "id";
                }
            }
        }
        /*Funkcja wykonuje się po kliknięciu w przycisk  Pobiera informacje o obecnie zalogowanym użytkowniku oraz Informacje o wybranym wniosku z Combobox'a
         *W zależności od wybranego wniosku i spełnienia warunków wykonuje się odpowiednia operacja dodania do bazy danych oraz zwrócenia notifikacji */
        private void Send_wniosek_Click(object sender, RoutedEventArgs e)
        {
            if (Send_do_kogo.SelectedValue != null )
            {
                int typ_wniosku = (int)Send_do_kogo.SelectedValue;
                int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                string username_currect_user = (string)Application.Current.Properties["currect_user_username"];
                int id_do_kogo = Convert.ToInt32(Send_do_kogo.SelectedValue.ToString());
                string tresc_wiadomosci = Notka.Text;
                var notificationManager = new NotificationManager();
                using (var db = new DBPROJECT())
                {
                    using (var contex = db.Database.BeginTransaction())
                    {
                        var testow = db.wnioski.First(x => x.id == typ_wniosku);
                        if (testow.typ_wniosku == "Wynagrodzenie")
                        {
                            if (kwota.Text != "")
                            {
                                string notka = Notka.Text;
                                db.user_wnioski.Add(new user_wnioski { id_pracownika = id_currect_user, id_wniosku = typ_wniosku, Data_rozpoczecia = DateTime.Today, Data_zakonczenia = DateTime.Today, Notka = notka, kwota = Convert.ToInt32(kwota.Text) });
                                db.SaveChanges();
                                Notka.Text = "";
                                notificationManager.Show(new NotificationContent
                                {
                                    Title = $"Wniosek Wysłany",
                                    Message = $"Wniosek został wysłany poczekaj na rozpatrzenie",
                                    Type = NotificationType.Success
                                });
                            }
                            else
                            {
                                notificationManager.Show(new NotificationContent
                                {
                                    Title = $"Brak Wymaganego Pola",
                                    Message = $"Upewnij się że z Wynagrodzeniem jest uzupełnione",
                                    Type = NotificationType.Error
                                });
                            }

                        }
                        else
                        {
                            if (Data_Start.SelectedDate == null || Data_koniec.SelectedDate == null || Data_Start.SelectedDate > Data_koniec.SelectedDate)
                            {
                              
                                notificationManager.Show(new NotificationContent
                                {
                                    Title = $"Brak Wymaganego Pola",
                                    Message = $"Upewnij się że Pola z datami są poprawnie  uzupełnione",
                                    Type = NotificationType.Error
                                });
                            }
                            else
                            {
                                var data_start = Data_Start.SelectedDate.Value.Date;
                                var data_end = Data_koniec.SelectedDate.Value.Date;
                                string notka = Notka.Text;
                                db.user_wnioski.Add(new user_wnioski { id_pracownika = id_currect_user, id_wniosku = typ_wniosku, Data_rozpoczecia = data_start, Data_zakonczenia = data_end, Notka = notka });
                                db.SaveChanges();
                                Notka.Text = "";
                                Data_Start.SelectedDate = null;
                                Data_Start.DisplayDate = DateTime.Today;
                                Data_koniec.SelectedDate = null;
                                Data_koniec.DisplayDate = DateTime.Today;
                                notificationManager.Show(new NotificationContent
                                {
                                    Title = $"Wniosek Wysłany",
                                    Message = $"Wniosek został wysłany poczekaj na rozpatrzenie",
                                    Type = NotificationType.Success
                                });
                            }
                        }
                        contex.Commit();
                    }
                }
            }
        }
        /* Funkcja wywoływana po naciśnięciu przycisku ma za zadanie zamknąć bierzące okno  */
        private void CloseIt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /* Funkcja wywoływana po naciśnięciu lewego przycisku myszki i przytrzymanie go ma za zadanie umożliwić przesuwanie okna */
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        /* Funkcja wywoływana przy zmianie opcji w Comboboxi'e w zależności od opcji pokazuje lub chowa różne 'inputy' */
        private void Send_do_kogo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int typ_wniosku = (int)Send_do_kogo.SelectedValue;
            using (var db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var testow = db.wnioski.First(x => x.id == typ_wniosku);
                    if (testow.typ_wniosku == "Urlop")
                    {
                        Data_Start1.Visibility = Visibility.Visible;
                        Data_Start.Visibility = Visibility.Visible;
                        Data_koniecl.Visibility = Visibility.Visible;
                        Data_koniec.Visibility = Visibility.Visible;
                        kwotal.Visibility = Visibility.Hidden;
                        kwota.Visibility = Visibility.Hidden;
                    }
                    else if (testow.typ_wniosku == "L4")
                    {
                        Data_Start1.Visibility = Visibility.Visible;
                        Data_Start.Visibility = Visibility.Visible;
                        Data_koniecl.Visibility = Visibility.Visible;
                        Data_koniec.Visibility = Visibility.Visible;
                        kwotal.Visibility = Visibility.Hidden;
                        kwota.Visibility = Visibility.Hidden;
                    }
                    else if (testow.typ_wniosku == "Wynagrodzenie")
                    {
                        kwotal.Visibility = Visibility.Visible;
                        kwota.Visibility = Visibility.Visible;
                        Data_Start1.Visibility = Visibility.Hidden;
                        Data_Start.Visibility = Visibility.Hidden;
                        Data_koniecl.Visibility = Visibility.Hidden;
                        Data_koniec.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        /*Funkcja wymusza wpisywanie tylko cyfr */
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
