using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Notifications.Wpf;

namespace Project.MVVM.View
{
    public partial class DodajPracownikaVIew : UserControl
    {
        public DodajPracownikaVIew()
        {
            InitializeComponent();
        }
        /*Funkcja wywoływana po naciśnięciu przycisku hashuje hasło sprawdza wszystkie warunki i dodaje pracownika do bazy danych */
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
            var notificationManager = new NotificationManager();
            var reg_zarobki_check =Int32.TryParse(Register_Zarobki.Text, out int reg_zarobki);
            using (DBPROJECT db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    if (db.users.Where(c => c.username == reg_user).Count() > 0)
                    {
                        notificationManager.Show(new NotificationContent
                        {
                            Title = $" Błędna Nazwa użytkownika",
                            Message = $"Podany użytkownik już istnieje!{Environment.NewLine}",
                            Type = NotificationType.Error
                        });
                    }
                    else
                    {
                        if (reg_user != "" && reg_user.Length >= 4)
                        {
                            if (reg_passwd == reg_retype_passwd && reg_passwd != "" && reg_passwd.Length > 4 && reg_passwd.Length < 20 && reg_zarobki > 0 && reg_imie != "" && reg_nazwisko != "")
                            {
                                Source = ASCIIEncoding.ASCII.GetBytes(Register_retypePassword.Password);
                                hashed_Data = new MD5CryptoServiceProvider().ComputeHash(Source);
                                string passwd_hash = Convert.ToBase64String(hashed_Data);
                                users new_usr = new users { username = Register_username.Text, password = passwd_hash };
                                db.users.Add(new_usr);
                                if (Register_czy_szef.IsChecked == true)
                                {
                                    db.user_roles.Add(new user_roles { id_user = new_usr.Id, id_role = 1 });
                                }
                                else
                                {
                                    db.user_roles.Add(new user_roles { id_user = new_usr.Id, id_role = 2 });
                                }
                                db.informacje_personalne.Add(new informacje_personalne { Id_pracownika = new_usr.Id, Imie = reg_imie, Nazwisko = reg_nazwisko, Zarobki = reg_zarobki, Dni_urlopowe = 30, Data_zatrudnienia = today });
                                notificationManager.Show(new NotificationContent
                                {
                                    Title = $"Dodano Pracownika",
                                    Message = $"Pracownik {reg_imie} {reg_nazwisko}{Environment.NewLine}Pomyśnie Dodany",
                                    Type = NotificationType.Success
                                });
                            }
                            else if (reg_passwd == "")
                            {
                                notificationManager.Show(new NotificationContent
                                {
                                    Title = $" Błędna Długość Hasła",
                                    Message = $"Hasło musi zawierać się między 4-20 znaków",
                                    Type = NotificationType.Error
                                });
                            }
                            else if (reg_passwd.Length < 4 || reg_passwd.Length > 20)
                            {
                                notificationManager.Show(new NotificationContent
                                {
                                    Title = $" Błędna Długość Hasła",
                                    Message = $"Hasło musi zawierać się między 4-20 znaków",
                                    Type = NotificationType.Error
                                });
                            }
                            else if (reg_passwd != reg_retype_passwd)
                            {
                                notificationManager.Show(new NotificationContent
                                {
                                    Title = $" Hasła nie są identyczne!",
                                    Message = $"Oba hasła muszą być identyczne {Environment.NewLine} Spróbój Ponownie",
                                    Type = NotificationType.Error
                                });
                            }
                            else
                            {
                                notificationManager.Show(new NotificationContent
                                {
                                    Title = $"Nieprawidłowe Dane",
                                    Message = $"Upewnij się że wprowadzone dane są poprawne",
                                    Type = NotificationType.Error
                                });
                            }
                        }
                        else if (reg_user.Length <= 4 || reg_user.Length >= 20)
                        {
                            notificationManager.Show(new NotificationContent
                            {
                                Title = $" Błędna Długość nazwy użytkownika",
                                Message = $"Użytkownik musi zawierać się między 4-20 znaków",
                                Type = NotificationType.Error
                            });
                        }
                        else
                        {
                            notificationManager.Show(new NotificationContent
                            {
                                Title = $"Uzupełnij Wszystkie Pola",
                                Message = $"Każde pole jest Obowiązkowe",
                                Type = NotificationType.Error
                            });
                        }
                    }
                    db.SaveChanges();
                    contex.Commit();
                }
            }
        }
        /*Funkcja wymusza wpisywanie tylko cyfr */
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
