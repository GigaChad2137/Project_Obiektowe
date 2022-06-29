using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    /// Logika interakcji dla klasy WorkRaportPdf.xaml
    /// </summary>
    public partial class WorkRaportPdf : Window
    {
        public WorkRaportPdf()
        {
            InitializeComponent();
            fillList();
        }
        private void fillList()
        {
            List<Pdf_view> items = new List<Pdf_view>();
            int id_currect_user = (int)Application.Current.Properties["currect_user_id"];
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-0);
            var last = month.AddDays(+29);
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
    public class Pdf_view
    {
        public string data { get; set; }
        public TimeSpan? czas_start { get; set; }
        public TimeSpan? czas_stop { get; set; }
        public string godziny { get; set; }
        public string kwota { get; set; }

    }
}
