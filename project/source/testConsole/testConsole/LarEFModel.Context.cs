﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace testConsole
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LibraryDemoEntities1 : DbContext
    {
        public LibraryDemoEntities1()
            : base("name=LibraryDemoEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Book> Books { get; set; }
        public DbSet<BookStatu> BookStatus { get; set; }
        public DbSet<BookType> BookTypes { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<RFIDTag> RFIDTags { get; set; }
        public DbSet<RFIDTagType> RFIDTagTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<TagStatu> TagStatus { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionStatu> TransactionStatus { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Writer> Writers { get; set; }
    }
}
