using System;
using System.Linq;
using System.Windows.Controls;


namespace Project.MVVM.View
{
    public partial class PracownicyView : UserControl
    {
        public PracownicyView()
        {
            InitializeComponent();
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
                                     }).ToList();
                    foreach (var p in id_finder)
                    {
                        if (db.praca.Where(c => c.Id_pracownika == p.Id && c.Data == DateTime.Today).Count() > 0)
                        {
                            var sprawdzacz_czy_pracuje = from c in db.praca where c.Id_pracownika == p.Id && c.Data == DateTime.Today select c;
                            var praca_checker = sprawdzacz_czy_pracuje.FirstOrDefault<praca>();
                            PracownicyGrid.Items.Add(new Pracownicy
                            {
                                Imie_pracownika = p.Imie,
                                Nazwisko_pracownika = p.Nazwisko,
                                Rola_pracownika = p.Rola,
                                Zarobki_pracownika = p.Zarobki,
                                Czy_pracuje = praca_checker.Czy_pracuje,
                              
                            }
                            );
                        }
                        else
                        {
                            var pracownik = db.Set<praca>();
                            db.praca.Add(new praca { Id_pracownika = p.Id, Data = DateTime.Today, Data_rozpoczecia = null, Data_zakonczenia = null, Czy_pracuje = "Poza Pracą" });
                            db.SaveChanges();
                            PracownicyGrid.Items.Add(new Pracownicy
                            {
                                Imie_pracownika = p.Imie,
                                Nazwisko_pracownika = p.Nazwisko,
                                Rola_pracownika = p.Rola,
                                Zarobki_pracownika = p.Zarobki,
                                Czy_pracuje = "Poza Pracą",
         
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
        }
    }
}
