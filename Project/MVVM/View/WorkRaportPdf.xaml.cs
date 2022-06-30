using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Project.MVVM.View
{
    public partial class WorkRaportPdf : Window
    {
        public WorkRaportPdf()
        {
            InitializeComponent();
            fillList();
        }
        /* Funkcja ma uzupełnić Listview w wyświetlanym oknie zaczyna od utworzenia listy pobrania id obecnie zalogowanego usera
         * wykonuje operacje znajdując pierwszy i ostatni dzień poprzedniego miesiąca następnie znajduje w bazie danych informacje
         o wynagrodzeniu użytkownika i wylicza jego wynagrodzenie na godzine dalej znajduje wszystkie dni z poprzedniego miesiąca
        ktore "przepracował czyli data rozpoczęcia i zakończenia nie były null i w pętli zaczyna liczyć dla każdego dnia który został
        zwrócony ile godzin pracował łącznie a potem sume ile zarobił w dany dzień (w międzyczasie liczy ile zarobił za cały miesiąc)
        a następnie dodaje nowy  obiekt do listy którą binduje do Listview*/
        private void fillList()
        {
            List<Pdf_view> items = new List<Pdf_view>();
            int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);
            double suma_miesiac = 0;
            using (var db = new DBPROJECT())
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var dane_usera = db.informacje_personalne.Where(x => x.Id_pracownika == id_currect_user).First();
                    double zarobek_na_godzine = dane_usera.Zarobki / 160;
                    var miesiac_rozliczenia = db.praca.Where(x => x.Id_pracownika == id_currect_user && x.Data >= first && x.Data <= last && x.Data_rozpoczecia != null && x.Data_zakonczenia != null).ToList();
                    foreach (var dzien in miesiac_rozliczenia)
                    {
                        double suma_dzien = 0;
                        TimeSpan? godziny_przepracowane = dzien.Data_zakonczenia - dzien.Data_rozpoczecia;
                        suma_dzien = Math.Round(godziny_przepracowane.Value.TotalHours * zarobek_na_godzine, 2);
                        items.Add(new Pdf_view { data = dzien.Data.ToString(), czas_start = dzien.Data_rozpoczecia.Value.TimeOfDay, czas_stop = dzien.Data_zakonczenia.Value.TimeOfDay, godziny = $"{Math.Round(godziny_przepracowane.Value.TotalHours, 2)}h", kwota = $"{suma_dzien}zł" });
                        suma_miesiac = suma_miesiac + suma_dzien;
                    }
                    Listviewpdf.ItemsSource = items;
                    Data_stworzenia_pdfa.Text = $"{DateTime.Today.ToShortDateString()}";
                    Podsumowanie_wyplaty.Text = $"{suma_miesiac}zł";
                    imie_nazwisko_pracownika.Text = $"{dane_usera.Imie} {dane_usera.Nazwisko}";
                    Data_rozliczenia.Text = $"{first.ToShortDateString()}-{last.ToShortDateString()}";
                }
            }
        }
        /* Funkcja która wywołuje się po kliknięciu otwiera pole do konfiguracji drukowania  */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(print, "invoice");
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
    }

    public class Pdf_view //klasa stworzona na potrzeby uzupełnienia Listview 
    {
        public string data { get; set; }
        public TimeSpan? czas_start { get; set; }
        public TimeSpan? czas_stop { get; set; }
        public string godziny { get; set; }
        public string kwota { get; set; }
    }
}
