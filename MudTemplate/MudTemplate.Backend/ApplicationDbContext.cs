using Microsoft.EntityFrameworkCore;
using MudTemplate.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Backend
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<SiteUser> SiteUsers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
