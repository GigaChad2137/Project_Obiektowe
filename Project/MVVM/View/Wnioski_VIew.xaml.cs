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
            kwotal.Visibility = Visibility.Visible;
            Data_Start1.Visibility = Visibility.Hidden;
            Data_Start.Visibility = Visibility.Hidden;
            Data_koniecl.Visibility = Visibility.Hidden;
            Data_koniec.Visibility = Visibility.Hidden;
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
                    DataContext = wniosek;
                }
            }
        }
        private void Send_wniosek_Click(object sender, RoutedEventArgs e)
        {
            int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
            string username_currect_user = (string)Application.Current.Properties["currect_user_username"];
            int id_do_kogo =Convert.ToInt32(Send_do_kogo.SelectedValue.ToString());
            string tresc_wiadomosci = Notka.Text;
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
                        Notka.Text = "";
                        BindUserlist();
                    }
                }
            }
        }
        private void Send_do_kogo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string typ_wniosku = Send_do_kogo.SelectedIndex.ToString();
            if (typ_wniosku == "Urlop")
            {

            }

        }
       
   


    }



}
