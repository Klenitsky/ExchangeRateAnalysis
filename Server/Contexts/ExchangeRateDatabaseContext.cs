using Microsoft.EntityFrameworkCore;
using Server.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Contexts
{
    public class ExchangeRateDatabaseContext: DbContext
    {
        public DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;

        public ExchangeRateDatabaseContext()
        {
            Database.EnsureCreated();
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ExchangeRate>(pc =>
        //    {
        //        pc.HasNoKey();
        //    });
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ExchangeRateDB;Trusted_Connection=True;");
        }

    }
}
