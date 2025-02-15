using InteractiveServer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InteractiveServer.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<AppEntity> AppEntities { get; set; } = default!;
    }
}
