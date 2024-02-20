using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalSystem.Models;

namespace RentalSystem.Data
{
    public class RentalSystemContext : DbContext
    {
        public RentalSystemContext (DbContextOptions<RentalSystemContext> options)
            : base(options)
        {
        }

        public DbSet<RentalSystem.Models.Product> Product { get; set; } = default!;
        public DbSet<RentalSystem.Models.User> Users { get; set; } = default!;
        public DbSet<RentalSystem.Models.Prod> Prod { get; set; } = default!;
    }
}
