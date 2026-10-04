using Microsoft.EntityFrameworkCore;
using SonGun.Models;

namespace SonGun.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<News> News => Set<News>();
}
