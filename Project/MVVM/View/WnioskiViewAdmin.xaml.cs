using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Project.MVVM.View
{
    public partial class WnioskiVIewAdmin : Window
    {
        public WnioskiVIewAdmin()
        {
            InitializeComponent();
            BindUserlist();
        }

        public List<Rozpatrz_wnioski> rozpatrz_wnioski = new List<Rozpatrz_wnioski>(); //lista stworzona dla Combobox'a

        /*  Funkcja czyści liste rozpatrz_wnioski następnie Wyciąga wszystkich użytkowników oraz wniosek który wysłali
         *  z warunkiem który wymaga aby wniosek miał status null i wstawia je do wcześniej utworzonej listy       */

        private void BindUserlist()
        {
            rozpatrz_wnioski.Clear();
            using (var db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var wnioski_do_rozpatrzenia = (from users in db.users
                                  join informacje_personalne in db.informacje_personalne
                                  on users.Id equals informacje_personalne.Id_pracownika
                                  join user_wnioski in db.user_wnioski on users.Id equals user_wnioski.id_pracownika
                                  join wnioski in db.wnioski on user_wnioski.id_wniosku equals wnioski.id
                                  where user_wnioski.Status_Wniosku == null
                                  select new
                                  {
                                     user_wnioski.Id,
                                     user_wnioski.id_pracownika,
                                     wnioski.typ_wniosku,
                                    informacje_personalne.Imie,
                                    informacje_personalne.Nazwisko,
                                    user_wnioski.Data_rozpoczecia,
                                    user_wnioski.Data_zakonczenia,
                                    user_wnioski.Notka,
                                    user_wnioski.kwota
                                  }).ToList();
                    foreach (var wniosek in wnioski_do_rozpatrzenia)
                    {
                        rozpatrz_wnioski.Add(new Rozpatrz_wnioski { id_wniosku = wniosek.Id, id_pracownika = wniosek.id_pracownika, typ_wniosku = wniosek.typ_wniosku, imie = wniosek.Imie, nazwisko = wniosek.Nazwisko, data_start = wniosek.Data_rozpoczecia, data_end = wniosek.Data_zakonczenia, notka = wniosek.Notka, kwota = wniosek.kwota });
                    }
                    DataContext = rozpatrz_wnioski;
                    Send_do_kogo.ItemsSource = rozpatrz_wnioski;
                    Send_do_kogo.SelectedValuePath = "id_wniosku";
                }
            }
        }
        /*Funkcja wywoływana po naciśnięciu odpowiedniego przycisku wyciąga informacje o obecnie zalogowanym użytkowniku oraz wybranym z ComboBox'a
         * Następnie wykonuje zapytytanie zwracający 1 napotkany rekord Status wniosku zmienia wartość na true i zapisuje do bazy
         Jeśli pole kwota było null Tworzy się tablica  która zawiera wszystkie daty które znajdują się pomiędzy wyciągniętymi wcześniej
        z zapytania Po tym wyciągany jest typ wniosku i zaczyna się petlą która idze po każdej dacie z utworzonej tablicy i w zależności
        od tego czy rekord istniał podmienia wartośći kolumn lub tworzy nowe rekordy  Następnie wniosek znika a lista combobox'a jest odświeżana*/
        private void Send_akceptuj_wniosek_Click(object sender, RoutedEventArgs e)
        {
            if (Send_do_kogo.SelectedValue!= null)
            {
                int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                string username_currect_user = (string)Application.Current.Properties["currect_user_username"];
                int id_do_kogo = Convert.ToInt32(Send_do_kogo.SelectedValue.ToString());
                int id_wniosku = (int)Send_do_kogo.SelectedValue;
                using (var db = new DBPROJECT())
                {
                    using (var contex = db.Database.BeginTransaction())
                    {
                        var find_wniosek = db.user_wnioski.Where(x => x.Id == id_wniosku).First();
                        var start = find_wniosek.Data_rozpoczecia;
                        var end = find_wniosek.Data_zakonczenia;
                        find_wniosek.Status_Wniosku = true;
                        Send_do_kogo.DataContext = null;
                        db.SaveChanges();
                        if (find_wniosek.kwota == null)
                        {
                            DateTime[] zakres_nieobecnosci = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                 .Select(offset => start.AddDays(offset))
                                                 .ToArray();
                            var czy_pracuje = db.wnioski.Where(x => x.id == find_wniosek.id_wniosku).First();
                            foreach (var dzien in zakres_nieobecnosci)
                            {
                                var check_date = db.praca.Where(x => x.Id_pracownika == find_wniosek.id_pracownika && x.Data == dzien.Date).FirstOrDefault();
                                if (check_date != null)
                                {
                                    check_date.Data_rozpoczecia = null;
                                    check_date.Data_zakonczenia = null;
                                    check_date.Czy_pracuje = czy_pracuje.typ_wniosku;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    db.praca.Add(new praca { Id_pracownika = find_wniosek.id_pracownika, Data = dzien.Date, Data_rozpoczecia = null, Data_zakonczenia = null, Czy_pracuje = czy_pracuje.typ_wniosku });
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            var find_user = db.informacje_personalne.Where(x => x.Id_pracownika == find_wniosek.id_pracownika).First();
                            find_user.Zarobki = Convert.ToInt32(find_wniosek.kwota);
                            db.SaveChanges();
                        }
                        contex.Commit();
                    }
                }
                Notka.Text = "";
                Send_do_kogo.SelectedIndex = -1;
                Send_do_kogo.ItemsSource = null;
                rozpatrz_wnioski.Clear();
                BindUserlist();
                Send_do_kogo.Items.Refresh();
            }
        }
        /*Funkcja wywoływana po naciśnięciu odpowiedniego przycisku wyciąga informacje o obecnie zalogowanym użytkowniku oraz wybranym z ComboBox'a
          * Następnie wykonuje zapytytanie zwracający 1 napotkany rekord Status wniosku zmienia wartość na false i zapisuje do bazy
         i odświeża liste Combobox'a */
       private void Send_odrzuc_wniosek_Click(object sender, RoutedEventArgs e)
       {
           if(Send_do_kogo.SelectedValue != null)
           {
               int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
               string username_currect_user = (string)Application.Current.Properties["currect_user_username"];
               int id_do_kogo = Convert.ToInt32(Send_do_kogo.SelectedValue.ToString());
               int id_wniosku = (int)Send_do_kogo.SelectedValue;
               using (var db = new DBPROJECT())
               {
                   using (var contex = db.Database.BeginTransaction())
                   {
                       var find_wniosek = db.user_wnioski.Where(x => x.Id == id_wniosku).First();
                       find_wniosek.Status_Wniosku = false;
                       Notka.Text = "";
                       db.SaveChanges();
                       contex.Commit();
                   }
               }
               Send_do_kogo.SelectedIndex = -1;
               Send_do_kogo.ItemsSource = null;
               rozpatrz_wnioski.Clear();
               BindUserlist();
               Send_do_kogo.Items.Refresh();
           }
       }
        /* Funkcja wywoływana przy zmianie opcji w combobox'ie Ma za zadanie wyciągnąć przypisany do opcji wniosek z bazy danych
         * I w zależności od spełnionych warunków uzupełnić RichTextboxa*/
       private void Send_do_kogo_SelectionChanged(object sender, SelectionChangedEventArgs e)
       {
           if (Send_do_kogo.SelectedValue != null)
           {
               int id_wniosku = (int)Send_do_kogo.SelectedValue;

               using (var db = new DBPROJECT())
               {
                   using (var contex = db.Database.BeginTransaction())
                   {

                       var find_wniosek = db.user_wnioski.Where(x => x.Id == id_wniosku).First();
                       string newline = Environment.NewLine;
                       if (find_wniosek.kwota != null)
                       {

                           Notka.Text = $"Kwota proszonej podwyżki: {find_wniosek.kwota} {newline}  {newline}--------------------------Załączona wiadomość-------------------------{newline}{find_wniosek.Notka}";
                       }
                       else
                       {
                           Notka.Text = $"Zakres nieobecności:  {find_wniosek.Data_rozpoczecia.Date.ToShortDateString()}- {find_wniosek.Data_zakonczenia.Date.ToShortDateString()}{newline}{newline}--------------------------Załączona wiadomość-------------------------{newline}{find_wniosek.Notka}";
                       }

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
    }
    /* Klasa stworzona na potrzeby utworzenia specjalnej listy z potrzebnymi informacjami która się łączy z Combobox'em  */
    public class Rozpatrz_wnioski
    {
        public int id_wniosku { get; set; }
        public int id_pracownika { get; set; }
        public string typ_wniosku { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public  DateTime data_start { get; set; }
        public DateTime data_end { get; set; }
        public string notka { get; set; }
        public Nullable<int> kwota { get; set; }
    }

}
