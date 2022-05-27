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

            //To do : zmienic SqlConnection na framework Entity
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database.mdf;Integrated Security=True");
            SqlTransaction transaction;

            if (sqlCon.State == System.Data.ConnectionState.Closed)
                sqlCon.Open();

            try
            {
                transaction = sqlCon.BeginTransaction();
                string query = "SELECT COUNT(1) from users Where username=@username";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon, transaction);
                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@username", Register_username.Text);
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                if (count == 1)
                {
                    MessageBox.Show("Podany użytkownik już istnieje");
                }
                else
                {
                    if (reg_user != "" && reg_user.Length > 4)
                    {
                        if (reg_passwd == Register_retypePassword.Password && reg_passwd != "" && reg_passwd.Length > 4 && reg_passwd.Length < 20)
                        {
                            Source = ASCIIEncoding.ASCII.GetBytes(Register_retypePassword.Password);
                            hashed_Data = new MD5CryptoServiceProvider().ComputeHash(Source);
                            string passwd_hash = Convert.ToBase64String(hashed_Data);



                            string query_user = "Insert Into users (username,password) values(@username,@password)";
                            SqlCommand sql_register = new SqlCommand(query_user, sqlCon, transaction);
                            sql_register.CommandType = System.Data.CommandType.Text;
                            sql_register.Parameters.AddWithValue("@username", Register_username.Text);
                            sql_register.Parameters.AddWithValue("@password", passwd_hash);
                            sql_register.ExecuteNonQuery();
                            transaction.Commit();

                            string query_for_id = "Select Id from users where username=@username";
                            SqlCommand sql_id = new SqlCommand(query_for_id, sqlCon, transaction);
                            sql_id.CommandType = System.Data.CommandType.Text;
                            sql_id.Parameters.AddWithValue("@username", Register_username.Text);
                            int To = Convert.ToInt32(sql_id.ExecuteScalar());


                            string query_rola = "Insert Into user_roles (id_user,id_role) values(@id,@rola)";
                            SqlCommand sql_rola = new SqlCommand(query_rola, sqlCon);
                            sql_rola.Parameters.AddWithValue("@id", To);
                            if (Register_czy_szef.IsChecked == true)
                            {
                                int rola = 2;
                                sql_rola.Parameters.AddWithValue("@rola", rola);
                            }
                            else
                            {
                                int rola = 1;
                                sql_rola.Parameters.AddWithValue("@rola", rola);
                            }
                            sql_rola.ExecuteNonQuery();
                            MessageBox.Show("Pracownik dodany");


                        }
                        else if (reg_passwd == "")
                        {
                            MessageBox.Show("Hasło nie może być puste");
                        }
                        else if (reg_passwd.Length < 4 || reg_passwd.Length > 20)
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
                        MessageBox.Show("Hasło nie może być puste");
                    }

                }


            }
            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);

            }
            finally
            {
                sqlCon.Close();
            }

        }
    }
}
