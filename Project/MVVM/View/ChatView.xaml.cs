using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        public List<informacje_personalne> user { get; set; }
        private void BindUserlist()
        {
            using (var db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {

                    var item = db.informacje_personalne.ToList();
                    user = item;
                    DataContext = user;
                }
            }
        }

        private void Send_msg_Click(object sender, RoutedEventArgs e)
        {
            string username = (string)Application.Current.Properties["username"];
            var id_do_kogo = Send_do_kogo.SelectedValue;
            string tresc_wiadomosci = Send_Message.Text;

            
        }
    }
}
