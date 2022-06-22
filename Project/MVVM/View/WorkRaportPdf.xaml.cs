using System;
using System.Collections.Generic;
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
            List<Pdf_view> items = new List<Pdf_view>();
            items.Add(new Pdf_view() { Name = "John Doe", czas_start = DateTime.Now,czas_stop=DateTime.Now,godziny="4h",kwota="80zloty" });
            items.Add(new Pdf_view() { Name = "John Doe", czas_start = DateTime.Now, czas_stop = DateTime.Now, godziny = "4h", kwota = "80zloty" });
            items.Add(new Pdf_view() { Name = "John Doe", czas_start = DateTime.Now, czas_stop = DateTime.Now, godziny = "4h", kwota = "80zloty" });

            Listviewpdf.ItemsSource = items;
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
        public string Name { get; set; }
        public DateTime czas_start { get; set; }
        public DateTime czas_stop { get; set; }
        public string godziny { get; set; }
        public string kwota { get; set; }

    }
}
