using System.ComponentModel.DataAnnotations;

namespace TreasureHunt.Models;

public class TreasureMapResult
{
    [Key]
    public int Id { get; set; }

    public int InputId { get; set; }

    public double MinimumFuel { get; set; }

    public DateTime CalculatedAt { get; set; } = DateTime.Now;
}