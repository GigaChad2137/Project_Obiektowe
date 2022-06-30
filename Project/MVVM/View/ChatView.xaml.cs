using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Project.MVVM.View
{
    public partial class ChatView : Window
    {
        public ChatView()
        {
            InitializeComponent();
            BindUserlist();

        }

       public  List<Czat> user = new List<Czat>(); //utworzenie listy z Klasy Czat

        /*  Funkcja czyści liste user następnie Wyciąga wszystkich użytkowników z bazy danych i za pomocą pętli  wysyła  zapytania
         *  sprawdzając czy ostatnia wysłana przez nich wiadomośc Istnieje lub  jest  odczytana
         *  i w zależności od wyniku Dodaje odpowiedni rekord do wcześniej utworzonej listy */
        private void BindUserlist() 
        {
            user.Clear();
            int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
            using (var db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var imiona = (from users in db.users
                                join informacje_personalne in db.informacje_personalne
                                on users.Id equals informacje_personalne.Id_pracownika
                                select new
                                {
                                    informacje_personalne.Id_pracownika,
                                    informacje_personalne.Imie,
                                    informacje_personalne.Nazwisko,
                             
                                }).ToList();
                    var czy_nowa_wiadomosc = db.wiadomosci.OrderByDescending(u => u.Id);
                    foreach (var person in imiona)
                    {
                        int czy_istnieje = czy_nowa_wiadomosc.Where(s => s.id_nadawcy == person.Id_pracownika && s.id_odbiorcy == id_currect_user).Count();
                        if (czy_istnieje == 0 )
                        {
                            user.Add(new Czat {Id_pracownika=person.Id_pracownika, Imie = person.Imie, Nazwisko = person.Nazwisko, czy_nowa_wiadomosc = "" });
                        }
                        else
                        {
                            var co_zawiera = czy_nowa_wiadomosc.First(s => s.id_nadawcy == person.Id_pracownika && s.id_odbiorcy == id_currect_user);
                            if (co_zawiera.czy_przeczytane == false)
                            {
                                var ilosc = czy_nowa_wiadomosc.Where(s => s.id_nadawcy == person.Id_pracownika && s.id_odbiorcy == id_currect_user && s.czy_przeczytane==false).Count();
                                user.Add(new Czat { Id_pracownika = person.Id_pracownika, Imie = person.Imie, Nazwisko = person.Nazwisko, czy_nowa_wiadomosc = $" **" });
                            }
                            else
                            {
                                user.Add(new Czat { Id_pracownika = person.Id_pracownika, Imie = person.Imie, Nazwisko = person.Nazwisko, czy_nowa_wiadomosc = "" });
                            }
                        }
                    }
                    DataContext = user;
                }
            }
        }
        /* Funkcja wywołuje się po kliknięciu przycisku "wyślij" Sprawdza czy zaznaczono jakiegoś użytkownika z Combobox'a
         * Jeśli użytkownik został wybrany.  Pobiera id i username obecnie zalogowanego użytkownika oraz id użytkownka wybranego z Combobox'a
         Następnie sprawdza czy wiadomości ma długość większą od 0 oraz czy wiadomość nie jest samymi pustymi spacjami
        Jeśli nie jst następuje wysłanie wiadomości czyli dodanie jej do bazy danych odświeżenie okienka wyświetlającego wiadomości
        i zresetowanie tekstu który był w textboxie do którego wpisujemy wiadomość*/
        private void Send_msg_Click(object sender, RoutedEventArgs e)
        {
            if (Send_do_kogo.SelectedValue != null)
            {
                int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                string username_currect_user = (string)Application.Current.Properties["currect_user_username"];
                int id_do_kogo = Convert.ToInt32(Send_do_kogo.SelectedValue.ToString());
                string tresc_wiadomosci = Send_Message.Text;
                using (var db = new DBPROJECT())
                {
                    using (var contex = db.Database.BeginTransaction())
                    {
                        if (string.IsNullOrWhiteSpace(tresc_wiadomosci) || tresc_wiadomosci.Length < 1) { }
                        else
                        {
                            db.wiadomosci.Add(new wiadomosci { id_nadawcy = id_currect_user, id_odbiorcy = id_do_kogo, Wiadomosc = tresc_wiadomosci, czy_przeczytane = false });
                            db.SaveChanges();
                            contex.Commit();
                            Send_Message.Text = "";
                            fast_refresh();
                        }
                    }
                }
            }
        }
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer(); //zaainicjowie klasy dzięki której można używać wielu wątków
        /* Funkcja Wywoływana po zmianie użytkownika W Combobox'ie zatrzymuje wcześniej uruchomiony wątek i inicjuje nowy który wykonuje się co 5 sekund  */
        private void Send_do_kogo_SelectionChanged(object sender, SelectionChangedEventArgs e)

        {
            dispatcherTimer.Stop();
            fast_refresh();
            dispatcherTimer.Tick += new EventHandler(refresh);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
        }
        /* Funkcja używana jako wątek ma za zadanie wyciągnąc z bazy danych informacje i wiadomości które były wysłane przez obecnie zalogowanego użytkownika  do tego wybranego z listy
        * I vice versa  następnie segreguje wiadomosci od najstarszych i za pomocą pętli wstawia je do umieszczonego na stronie RichtextBox'a
         *  */
        private void refresh(object source, EventArgs e)
        {
            int id_do_kogo = Convert.ToInt32(Send_do_kogo.SelectedValue.ToString());
            Read_messages.Document.Blocks.Clear();
            using (DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                    var czat = (from users in db.users
                                join wiadomosci in db.wiadomosci
               on users.Id equals wiadomosci.id_nadawcy
                                where wiadomosci.id_nadawcy == id_currect_user && wiadomosci.id_odbiorcy == id_do_kogo || wiadomosci.id_nadawcy == id_do_kogo && wiadomosci.id_odbiorcy == id_currect_user
                                orderby wiadomosci.Id ascending
                                select new
                                {
                                    users.username,
                                    wiadomosci.Wiadomosc
                                }).ToList();
                    int czy_1_linia = 0;
                    foreach (var wiadomosc in czat)
                    {
                        if (wiadomosc.Wiadomosc.Length >= 1)
                            if(!string.IsNullOrWhiteSpace(wiadomosc.Wiadomosc))
                            {
                                if (czy_1_linia == 0)
                                {
                                    Read_messages.AppendText($"{wiadomosc.username}: {wiadomosc.Wiadomosc}  ");
                                    czy_1_linia = 1;
                                }
                                else
                                {
                                    Read_messages.AppendText($"{Environment.NewLine}{wiadomosc.username}: {wiadomosc.Wiadomosc}");
                                }
                            }
                        Read_messages.ScrollToEnd();
                    }
                }
            }
        }
        public void fast_refresh()
        /* Funkcja używana jako szybkie przeładowanie wiadomości aby uniknąć czekania na wątek ma za zadanie wyciągnąc z bazy danych informacje i wiadomości które były wysłane przez obecnie zalogowanego użytkownika  do tego wybranego z listy
    * I vice versa  następnie segreguje wiadomosci od najstarszych i za pomocą pętli wstawia je do umieszczonego na stronie RichtextBox'a
     *  */
        {
            int id_do_kogo = Convert.ToInt32(Send_do_kogo.SelectedValue.ToString());
            Read_messages.Document.Blocks.Clear();
            using (DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
                    var przeczytane = db.wiadomosci.Where(a => a.id_odbiorcy == id_currect_user && a.id_nadawcy == id_do_kogo).ToList();
                    przeczytane.ForEach(a => a.czy_przeczytane = true);
                    db.SaveChanges();
                    var czat = (from users in db.users
                                join wiadomosci in db.wiadomosci
                                on users.Id equals wiadomosci.id_nadawcy
                                where wiadomosci.id_nadawcy == id_currect_user && wiadomosci.id_odbiorcy == id_do_kogo || wiadomosci.id_nadawcy == id_do_kogo && wiadomosci.id_odbiorcy == id_currect_user
                                orderby wiadomosci.Id ascending
                                select new
                                {
                                    users.username,
                                    wiadomosci.Wiadomosc
                                }).ToList();
                    int czy_1_linia = 0;
                    foreach (var wiadomosc in czat)
                    {
                        if (wiadomosc.Wiadomosc.Length >= 1)
                            if (!string.IsNullOrWhiteSpace(wiadomosc.Wiadomosc))
                            {
                                if (czy_1_linia == 0)
                                {
                                    Read_messages.AppendText($"{wiadomosc.username}: {wiadomosc.Wiadomosc}  ");
                                    czy_1_linia = 1;
                                }
                                else
                                {
                                    Read_messages.AppendText($"{Environment.NewLine}{wiadomosc.username}: {wiadomosc.Wiadomosc}");
                                    var converter = new System.Windows.Media.BrushConverter();
                                }
                            }
                        Read_messages.ScrollToEnd();
                    }
                    contex.Commit();
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

    public class Czat
    {
        public int Id_pracownika { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string czy_nowa_wiadomosc { get; set; }
    }
}
