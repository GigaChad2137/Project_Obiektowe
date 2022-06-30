using Notifications.Wpf;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace Project
{
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var passwdhashed = GetHashedText(txtPassword.Password);
                    if (db.users.Where(c => c.username == txtUsername.Text && c.password == passwdhashed).Count() > 0)
                    {
                        var id_finder = from c in db.users where c.username == txtUsername.Text select c;
                        var id_checker = id_finder.FirstOrDefault<users>();
                        DateTime thisDay = DateTime.Today;
                        var czy_pracuje = db.praca.Where(x => x.Id_pracownika == id_checker.Id && x.Data == thisDay).Count();
                        if(czy_pracuje == 0)
                        {
                            db.praca.Add(new praca { Id_pracownika = id_checker.Id, Data = thisDay, Data_rozpoczecia = null, Data_zakonczenia = null, Czy_pracuje = "Nie Pracuje" });
                            db.SaveChanges();
                            contex.Commit();
                        }
                        Application.Current.Properties["currect_user_username"] = txtUsername.Text;
                        Application.Current.Properties["currect_user_id"] = id_checker.Id;
                        if (db.user_roles.Where(c => c.id_user == id_checker.Id && c.id_role == 1).Count() > 0)
                        {
                            Application.Current.Properties["currect_user_admin"] = true;
                            MainWindow dashboard = new MainWindow();    
                            dashboard.Show();
                            this.Close();
                        }
                        else
                        {
                            Application.Current.Properties["currect_user_admin"] = false;
                            UserMainWindow dashboard = new UserMainWindow();
                            dashboard.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        var notificationManager = new NotificationManager();
                        notificationManager.Show(new NotificationContent
                        {
                            Title = $"Błędne Login lub Hasło",
                            Message = $"Sprawdź swoje dane logowania! {Environment.NewLine}I spróbój ponownie",
                            Type = NotificationType.Error
                        });
                    }
                }
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
        /* Funkcja wywoływana po naciśnięciu przycisku ma za zadanie zamknąć bierzące okno oraz wyłączyć aplikacje  */
        private void CloseIt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }
        /* Funkcja wywoływana po naciśnięciu lewego przycisku myszki i przytrzymanie go ma za zadanie umożliwić przesuwanie okna */
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
