using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;


namespace Project.MVVM.View
{
    /// <summary>
    /// Logika interakcji dla klasy DodajPracownikaVIew.xaml
    /// </summary>
    public partial class DodajPracownikaVIew : UserControl
    {
        public DodajPracownikaVIew()
        {
            InitializeComponent();
        }
        private void Register_btnclick_Click(object sender, RoutedEventArgs e)
        {
            byte[] Source;
            byte[] hashed_Data;
            var reg_user = Register_username.Text;
            var reg_passwd = Register_password.Password;
            var reg_retype_passwd = Register_retypePassword.Password;
            var reg_imie = Register_Imie.Text;
            var reg_nazwisko = Register_Nazwisko.Text;
            DateTime today = DateTime.Today;
            var reg_zarobki_check =Int32.TryParse(Register_Zarobki.Text, out int reg_zarobki);
            using (DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                   

               
                      
                        if (db.users.Where(c => c.username == reg_user).Count() > 0)
                        {
                            MessageBox.Show("Podany użytkownik już istnieje");
                        }
                        else
                        {
                            if (reg_user != "" && reg_user.Length > 4)
                            {
                                if (reg_passwd == reg_retype_passwd && reg_passwd != "" && reg_passwd.Length > 4 && reg_passwd.Length < 20 && reg_zarobki > 0 && reg_imie != null && reg_nazwisko != null)
                                {
                                    Source = ASCIIEncoding.ASCII.GetBytes(Register_retypePassword.Password);
                                    hashed_Data = new MD5CryptoServiceProvider().ComputeHash(Source);
                                    string passwd_hash = Convert.ToBase64String(hashed_Data);
                                    users new_usr = new users { username = Register_username.Text, password = passwd_hash };
                                    db.users.Add(new_usr);
                                    

                                   
                                 
                                    if (Register_czy_szef.IsChecked == true)
                                    {
                                        db.user_roles.Add(new user_roles { id_user = new_usr.Id, id_role = 2 });
                                    }
                                    else
                                    {
                                        db.user_roles.Add(new user_roles { id_user = new_usr.Id, id_role = 1 });
                                    }
                                    db.informacje_personalne.Add(new informacje_personalne { Id_pracownika = new_usr.Id, Imie = reg_imie, Nazwisko = reg_nazwisko, Zarobki = reg_zarobki,Dni_urlopowe=30,Data_zatrudnienia= today });
                                    MessageBox.Show("Pracownik dodany");
                                    

                                }
                                else if (reg_passwd == "")
                                {
                                    MessageBox.Show("Hasło nie może być puste xd");
                                }
                                else if (reg_passwd.Length <= 4 || reg_passwd.Length > 20)
                                {
                                    MessageBox.Show("Hasło musi zawierać się miedzy 4 a 20 znaków.");
                                }
                                else
                                {
                                    MessageBox.Show("Hasła nie są takie same");
                                }

                            }
                            else if (reg_user.Length < 4 || reg_user.Length > 20)
                            {
                                MessageBox.Show("Użytkownik musi zawierać się miedzy 4 a 20 znaków.");
                            }
                            else
                            {
                                MessageBox.Show("Uzupełnij wszystkie pola");
                            }

                    }
                    db.SaveChanges();
                    contex.Commit();

                }
                   
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
