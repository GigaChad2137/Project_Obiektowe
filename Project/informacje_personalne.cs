//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project
{
    using System;
    using System.Collections.Generic;
    
    public partial class informacje_personalne
    {
        public int Id_pracownika { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int Zarobki { get; set; }
        public int Dni_urlopowe { get; set; }
        public System.DateTime Data_zatrudnienia { get; set; }
    
        public virtual users users { get; set; }
    }
}