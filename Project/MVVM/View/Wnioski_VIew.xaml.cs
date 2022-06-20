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
    public partial class WnioskiVIew : Window
    {
        public WnioskiVIew()
        {
            InitializeComponent();
            BindUserlist();
           
           
           // Send_Message.Visibility = Visibility.Hidden;
        }


        public List<wnioski> wniosek { get; set; }
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
        private void Send_wniosek_Click(object sender, RoutedEventArgs e)
        {
            int typ_wniosku = (int)Send_do_kogo.SelectedValue;
            int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
            string username_currect_user = (string)Application.Current.Properties["currect_user_username"];
            int id_do_kogo =Convert.ToInt32(Send_do_kogo.SelectedValue.ToString());
            string tresc_wiadomosci = Notka.Text;
            using (var db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {

                    var testow = db.wnioski.First(x => x.id == typ_wniosku);
                    if (testow.typ_wniosku == "Wynagrodzenie")
                    {
                        var data_start = Data_Start.SelectedDate.Value.Date;
                        var data_end = Data_koniec.SelectedDate.Value.Date;
                        string notka = Notka.Text;
                        db.user_wnioski.Add(new user_wnioski { id_pracownika = id_currect_user, id_wniosku = typ_wniosku, Data_rozpoczecia = DateTime.Today, Data_zakonczenia = DateTime.Today, Notka = notka });
                        db.SaveChanges();

                        Notka.Text = "";
                     

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

                    }
                    contex.Commit();

                }
            }
        }
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
    }
}
