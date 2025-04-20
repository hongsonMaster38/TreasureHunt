using Microsoft.EntityFrameworkCore;
using TreasureHunt.Models;

namespace TreasureHunt;

public class TreasureHuntContext : DbContext
{
    public TreasureHuntContext(DbContextOptions<TreasureHuntContext> options) : base(options) { }

    public DbSet<TreasureMapInput> TreasureMaps { get; set; }
    public DbSet<TreasureMapResult> TreasureResults { get; set; }
}

