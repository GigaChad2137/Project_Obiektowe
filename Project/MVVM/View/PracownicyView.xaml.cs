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
    /// Logika interakcji dla klasy PracownicyView.xaml
    /// </summary>
    public partial class PracownicyView : UserControl
    {
        public PracownicyView()
        {
            InitializeComponent();
            // Todo: podlaczyc baze danych i zaladowac modele
            //DBPROJECT db = new DBPROJECT();


            //var id_finder = (from users in db.users
            //                 join user_roles in db.user_roles on users.Id equals user_roles.id_user
            //                 join roles in db.roles on user_roles.id_role equals roles.Id
            //                 join zarobki in db.zarobki on users.Id equals zarobki.Id_pracownika
            //                 select new
            //                 {
            //                     Id = users.Id,
            //                     Name = users.username,
            //                     Rola = roles.role,
            //                     Zarobas = zarobki.Zarobki1,


            //                 }).ToList();
            //foreach (var p in id_finder)
            //{

            //    DateTime thisDay = DateTime.Today;
            //    if (db.praca.Where(c => c.Id_pracownika == p.Id && c.Data == thisDay).Count() > 0)
            //    {
            //        var sprawdzacz_czy_pracuje = from c in db.praca where c.Id_pracownika == p.Id && c.Data == thisDay select c;
            //        var praca_checker = sprawdzacz_czy_pracuje.FirstOrDefault<praca>();


            //        PracownicyGrid.Items.Add(new Pracownicy
            //        {
            //            id_pracownika = p.Id,
            //            Nazwa_pracownika = p.Name,
            //            Rola_pracownika = p.Rola,
            //            Czy_pracuje = (bool)praca_checker.Czy_pracuje,
            //            Zarobki_Pracownika = p.Zarobas
            //        }
            //        );
            //    }
            //    else
            //    {

            //        var pracownik = db.Set<praca>();
            //        db.praca.Add(new praca { Id_pracownika = p.Id, Data = default, Data_rozpoczecia = null, Data_zakonczenia = null, Czy_pracuje = false });
            //        db.SaveChanges();

            //        PracownicyGrid.Items.Add(new Pracownicy
            //        {
            //            id_pracownika = p.Id,
            //            Nazwa_pracownika = p.Name,
            //            Rola_pracownika = p.Rola,
            //            Czy_pracuje = false,
            //            Zarobki_Pracownika = p.Zarobas
            //        });
            //    }



            //}


        }


        public class Pracownicy
        {

            public int id_pracownika { get; set; }
            public string Nazwa_pracownika { get; set; }
            public string Rola_pracownika { get; set; }
            public bool Czy_pracuje { get; set; }
            public int Zarobki_Pracownika { get; set; }
        }

        private void PracownicyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
