using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Project
{
    /// <summary>
    /// Logika interakcji dla klasy LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                using (var db = new DBPROJECT())
                {
                    if (db.users.Where(c => c.username == txtUsername.Text && c.password == txtPassword.Password).Count() > 0)
                    {

                        var id_finder = from c in db.users where c.username == txtUsername.Text select c;
                        var id_checker = id_finder.FirstOrDefault<users>();
                        if (db.user_roles.Where(c => c.id_user == id_checker.Id && c.id_role == 1).Count() > 0)
                        {
                            MainWindow dashboard = new MainWindow();
                            dashboard.Show();
                            this.Close();
                        }
                        else
         
                        {
                            UserMainWindow dashboard = new UserMainWindow();
                            dashboard.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Użytkownik lub hasło niepoprawne");
                    }
                }

            }
            catch(Exception ea)
            {
                MessageBox.Show(ea.Message);
            }
        }
      
        private string GetHashedText(string inputData)
        {
            byte[] tmpSource;
            byte[] tmpData;
            tmpSource = ASCIIEncoding.ASCII.GetBytes(inputData);
            tmpData = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            return Convert.ToBase64String(tmpData);
        }
    }
}
