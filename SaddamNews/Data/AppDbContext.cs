using Microsoft.EntityFrameworkCore;
using SaddamNews.Models;

namespace SaddamNews.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<News> News => Set<News>();
}
