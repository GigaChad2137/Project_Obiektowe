﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBPROJECT : DbContext
    {
        public DBPROJECT()
            : base("name=DBPROJECT")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<roles> roles { get; set; }
        public virtual DbSet<user_roles> user_roles { get; set; }
        public virtual DbSet<users> users { get; set; }
        public virtual DbSet<wiadomosci> wiadomosci { get; set; }
        public virtual DbSet<informacje_personalne> informacje_personalne { get; set; }
        public virtual DbSet<praca> praca { get; set; }
    }
}