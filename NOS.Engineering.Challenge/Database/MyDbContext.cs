using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Database
{
    public class MyDbContext : DbContext
    {
        public DbSet<Content> Contents { get; set; } = null!;

        public MyDbContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;User Id=root;Password=;Database=nosi-dotnet-engineering-challenge", MySqlServerVersion.Parse("10.4.32"));
        }
    }
}
