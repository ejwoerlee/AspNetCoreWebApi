using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// To enable Migration: open the Package Manager Console
// Add-Migration CityInfoDbInitialMigration
// Update-Database

namespace CityInfo.API.Entities
{
    using Microsoft.EntityFrameworkCore;

    public class CityInfoContext: DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options): base(options)
        {
            Database.Migrate(); // Voert het commando Update-Databse() uit zoals dit gedaan zou worden vanuit de Package Manage Console..
            // Database.EnsureCreated();
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("connectionstring");

        //    base.OnConfiguring(optionsBuilder);
        //}

    }
}
