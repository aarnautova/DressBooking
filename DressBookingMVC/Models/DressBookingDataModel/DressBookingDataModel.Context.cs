﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DressBookingMVC.Models.DressBookingDataModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DressBookingDB1Entities : DbContext
    {
        public DressBookingDB1Entities()
            : base("name=DressBookingDB1Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Dress> Dresses { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
    }
}
