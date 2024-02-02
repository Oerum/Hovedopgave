﻿using Auth.Database.Model;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace Auth.Database
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        public DbSet<ActiveLicensesDbModel> ActiveLicenses { get; set; }
        public DbSet<UserDbModel> User { get; set; }
        public DbSet<OrderDbModel> Order { get; set; }
        //public DbSet<MakeDatabase> MakeDatabase { get; set; }

        //public DbSet<WalletDbModel> Wallet { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelbuilder)
        //{
        //    base.OnModelCreating(modelbuilder);

        //    //modelbuilder.Entity<UserDbModel>(e =>
        //    //{
        //    //    //e.HasNoKey();
        //    //    //e.Property(o => o.Id).HasColumnType("int").HasConversion<int>();
        //    //    //e.Property(o => o.Bmi).HasColumnType("float").HasConversion<double>();
        //    //    //e.Property(o => o.Result).HasColumnType("varchar(50)").HasConversion<string>();
        //    //    //e.Property(o => o.DateModified).HasColumnType("timestamp").HasConversion<DateTime>();
        //    //});
        //}
    }
}