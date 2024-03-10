using Microsoft.EntityFrameworkCore;

namespace Udup.WebApp.EF;

public partial class DatabaseContext : DbContext
{
    public DbSet<Sample> Samples { get; set; }
    
}