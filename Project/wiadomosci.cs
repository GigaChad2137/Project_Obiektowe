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
    
    public partial class wiadomosci
    {
        public int Id { get; set; }
        public int id_nadawcy { get; set; }
        public int id_odbiorcy { get; set; }
        public string Wiadomosc { get; set; }
        public bool czy_przeczytane { get; set; }
    
        public virtual users users { get; set; }
        public virtual users users1 { get; set; }
    }
}
