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
            using (var db = new DBPROJECT()) //otworzenie połączenia z baza danych i użycie jej
            {
                using (var contex = db.Database.BeginTransaction())  //stworzenie transakcji w bazie danych i użycie jej
                {
                    var passwdhashed = GetHashedText(txtPassword.Password);  // zmienna przechowująca zahashowane hasło, które zostało pobrane z password boxa 
                    if (db.users.Where(c => c.username == txtUsername.Text && c.password == passwdhashed).Count() > 0) // zapytanie do bazy danych, które sprawdza czy użytkownik o podanych danych pobranych z formularza
                    {
                        var id_finder = from c in db.users where c.username == txtUsername.Text select c; // zapytanie do bazy danych zwracające wszystkie rekordy pasujące do wymagań
                        var id_checker = id_finder.FirstOrDefault<users>();  //funkcja zwracająca pierwszy wiersz z wcześniejszego zapytania
                        DateTime thisDay = DateTime.Today; //zmienna zawierająca aktualną date
                        var czy_pracuje = db.praca.Where(x => x.Id_pracownika == id_checker.Id && x.Data == thisDay).Count(); // zapytanie do bazy danych, które zlicza ilość zwróconych rekordów
                        if (czy_pracuje == 0)  // jeżeli zapytanie zwróci 0 to wykona się zapisanie rekordu i commit transakcji - czyli dodanie rekordu do bazy danych
                        {
                            db.praca.Add(new praca { Id_pracownika = id_checker.Id, Data = thisDay, Data_rozpoczecia = null, Data_zakonczenia = null, Czy_pracuje = "Nie Pracuje" });
                            db.SaveChanges();
                            contex.Commit();
                        }
                        Application.Current.Properties["currect_user_username"] = txtUsername.Text; // słownik który służy do przechowywania przekazanej informacji z której moge korzystać z dowolnego miejsca
                        Application.Current.Properties["currect_user_id"] = id_checker.Id;
                        if (db.user_roles.Where(c => c.id_user == id_checker.Id && c.id_role == 1).Count() > 0) //zapytanie sprawdzajace czy użytkownik posiada uprawnienia
                        {
                            Application.Current.Properties["currect_user_admin"] = true;
                            MainWindow dashboard = new MainWindow();  //zainicjowanie klasy mainwindow
                            dashboard.Show(); //pokazanie okna mainwindow
                            this.Close();  // zamknięcie okna logowania
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
        private string GetHashedText(string inputData) //funkcja hashująca 
        {
            byte[] tmpSource;
            byte[] tmpData;
            tmpSource = ASCIIEncoding.ASCII.GetBytes(inputData); //przekonwertowuje do typu byte 
            tmpData = new MD5CryptoServiceProvider().ComputeHash(tmpSource); // użycie gotowej funkcji, która hashuje przekazaną tablice
            return Convert.ToBase64String(tmpData); //zahashowaną tablice przekonwertowywuje do stringa i następnia zwraca
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
