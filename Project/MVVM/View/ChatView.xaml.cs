using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Project.MVVM.View
{
    /// <summary>
    /// Logika interakcji dla klasy ChatView.xaml
    /// </summary>
    public partial class ChatView : Window
    {
        public ChatView()
        {
            InitializeComponent();
            BindUserlist();
        }

       public  List<Czat> user = new List<Czat>();
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
                        Trace.WriteLine(person.Nazwisko + " " + czy_istnieje);
                        if (czy_istnieje == 0 )
                        {
                            user.Add(new Czat {Id_pracownika=person.Id_pracownika, Imie = person.Imie, Nazwisko = person.Nazwisko, czy_nowa_wiadomosc = "" });
                        }
                        else
                        {

                           var co_zawiera = czy_nowa_wiadomosc.First(s => s.id_nadawcy == person.Id_pracownika && s.id_odbiorcy==id_currect_user);
                            Trace.WriteLine(person.Nazwisko + " " + co_zawiera.czy_przeczytane + " " + co_zawiera.Wiadomosc);
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
                            BindUserlist();
                        }
                    }
                }
            }
        }
        private void Send_do_kogo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            fast_refresh();
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(refresh);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
        }
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
                            if(string.IsNullOrWhiteSpace(wiadomosc.Wiadomosc))
                            { }
                            else
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
                }
            }
        }
        public void fast_refresh()
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
                            if (string.IsNullOrWhiteSpace(wiadomosc.Wiadomosc))
                            { }
                            else
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


    }

    public class Czat
    {
        public int Id_pracownika { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string czy_nowa_wiadomosc { get; set; }
    }

}
