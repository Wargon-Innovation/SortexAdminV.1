using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortexAdminV._1.Models
{
    public class SortexDBContext : DbContext
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandImage> BrandImages { get; set; }
        public DbSet<BrandTagMM> BrandTagMMs { get; set; }
        public DbSet<Fraction> Fractions { get; set; }
        public DbSet<Modeboard> Modeboards { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Trend> Trends { get; set; }
        public DbSet<TrendImage> TrendImages { get; set; }
        public DbSet<TrendImageMM> TrendImageMMs { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<WargonBrands> WargonBrands { get; set;}

        public SortexDBContext(DbContextOptions options) : base (options)
        {

        }



    }
}
