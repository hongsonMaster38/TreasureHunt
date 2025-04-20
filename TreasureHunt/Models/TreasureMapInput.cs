using System.ComponentModel.DataAnnotations;

namespace TreasureHunt.Models;

public class TreasureMapInput
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Range(1, 500)]
    public int N { get; set; }

    [Required]
    [Range(1, 500)]
    public int M { get; set; }

    [Required]
    public int P { get; set; }

    [Required]
    public string MatrixData { get; set; } // Stored as JSON string

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
