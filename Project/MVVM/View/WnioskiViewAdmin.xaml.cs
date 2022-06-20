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
    public partial class WnioskiVIewAdmin : Window
    {
        public WnioskiVIewAdmin()
        {
            InitializeComponent();
            BindUserlist();
           
           
           // Send_Message.Visibility = Visibility.Hidden;
        }


        public List<Rozpatrz_wnioski> rozpatrz_wnioski = new List<Rozpatrz_wnioski>();
        private void BindUserlist()
        {
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
        private void Send_akceptuj_wniosek_Click(object sender, RoutedEventArgs e)
        {
            int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
            string username_currect_user = (string)Application.Current.Properties["currect_user_username"];
            int id_do_kogo =Convert.ToInt32(Send_do_kogo.SelectedValue.ToString());
            int id_wniosku = (int)Send_do_kogo.SelectedValue;
            using (var db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var find_wniosek = db.user_wnioski.Where(x => x.Id == id_wniosku).First();
                    find_wniosek.Status_Wniosku = true;
                    Send_do_kogo.DataContext = null;
                    db.SaveChanges();
                    if (find_wniosek.kwota != null)
                    {

                    }

                        contex.Commit();
                }
            }
            Send_do_kogo.SelectedIndex = -1;
            Send_do_kogo.ItemsSource = null;
            rozpatrz_wnioski.Clear();
            BindUserlist();
            Send_do_kogo.Items.Refresh();
        }
        private void Send_odrzuc_wniosek_Click(object sender, RoutedEventArgs e)
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
        private void Send_do_kogo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Send_do_kogo.SelectedValue != null)
            {
                int id_wniosku = (int)Send_do_kogo.SelectedValue;
                Trace.WriteLine(typ_wniosku);
                using (var db = new DBPROJECT())
                {
                    using (var contex = db.Database.BeginTransaction())
                    {

                        var find_wniosek = db.user_wnioski.Where(x => x.Id == id_wniosku).First();
                        string newline = Environment.NewLine;
                        if (find_wniosek.kwota != null)
                        {

                            Notka.Text = $"Kwota proszonej podwyżki: {find_wniosek.kwota} {newline}  {newline}Załączona wiadomość:{newline}{find_wniosek.Notka}";
                        }
                        else
                        {
                            Notka.Text = $"Zakres nieobecności:  {find_wniosek.Data_rozpoczecia.Date.ToShortDateString()}- {find_wniosek.Data_zakonczenia.Date.ToShortDateString()}{newline}{newline}---------------------------------------------Załączona wiadomość---------------------------------------------{newline}{newline}{find_wniosek.Notka}";
                        }

                    }
                }
            }
            


           

        }
       
   


    }
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
