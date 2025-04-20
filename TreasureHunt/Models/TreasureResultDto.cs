namespace TreasureHunt.Models;

public class TreasureResultDto
{
    public int Id { get; set; }
    public double MinimumFuel { get; set; }
    public TreasureMapDto Input { get; set; }
    public string Path { get; set; }
    public DateTime CalculatedAt { get; set; }
}
