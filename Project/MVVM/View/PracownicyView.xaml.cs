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
using System.Diagnostics;

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

            using (DBPROJECT db = new DBPROJECT())
            {
                using(var contex = db.Database.BeginTransaction())
                {
                    var id_finder = (from users in db.users
                                     join user_roles in db.user_roles on users.Id equals user_roles.id_user
                                     join roles in db.roles on user_roles.id_role equals roles.Id
                                     join inf_p in db.informacje_personalne on users.Id equals inf_p.Id_pracownika
                                     orderby users.Id descending
                                     select new
                                     {
                                         Id = users.Id,
                                         Imie = inf_p.Imie,
                                         Nazwisko = inf_p.Nazwisko,
                                         Rola = roles.role,
                                         Zarobki = inf_p.Zarobki,
                                         Dni_urlopu = inf_p.Dni_urlopowe
                                     }).ToList();
                    foreach (var p in id_finder)
                    {
                        DateTime thisDay = DateTime.Now;
                        if (db.praca.Where(c => c.Id_pracownika == p.Id && c.Data == thisDay).Count() > 0)
                        {
                            var sprawdzacz_czy_pracuje = from c in db.praca where c.Id_pracownika == p.Id && c.Data == thisDay select c;
                            var praca_checker = sprawdzacz_czy_pracuje.FirstOrDefault<praca>();


                            PracownicyGrid.Items.Add(new Pracownicy
                            {
                                Imie_pracownika = p.Imie,
                                Nazwisko_pracownika = p.Nazwisko,
                                Rola_pracownika = p.Rola,
                                Zarobki_pracownika = p.Zarobki,
                                Czy_pracuje = praca_checker.Czy_pracuje,
                                Urlop_pracownika = p.Dni_urlopu
                            }
                            );

                        }
                        else
                        {
                            var pracownik = db.Set<praca>();
                            db.praca.Add(new praca { Id_pracownika = p.Id, Data = default, Data_rozpoczecia = null, Data_zakonczenia = null, Czy_pracuje = "Poza Pracą" });
                            db.SaveChanges();

                            PracownicyGrid.Items.Add(new Pracownicy
                            {
                                Imie_pracownika = p.Imie,
                                Nazwisko_pracownika = p.Nazwisko,
                                Rola_pracownika = p.Rola,
                                Zarobki_pracownika = p.Zarobki,
                                Czy_pracuje = "Poza Pracą",
                                Urlop_pracownika = p.Dni_urlopu
                            });
                            
                        }
                    }
                    contex.Commit();
                }
            }
        }
        public class Pracownicy
        {

            public string Imie_pracownika { get; set; }
            public string Nazwisko_pracownika { get; set; }
            public string Rola_pracownika { get; set; }
            public int Zarobki_pracownika { get; set; }
            public string Czy_pracuje { get; set; }
          
            public int Urlop_pracownika { get; set; }
        }

        private void PracownicyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
