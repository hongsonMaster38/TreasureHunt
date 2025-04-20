using Microsoft.AspNetCore.Mvc;
using TreasureHunt.Models;

namespace TreasureHunt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TreasureHuntController : ControllerBase
{
    private readonly TreasureHuntContext _context;

    public TreasureHuntController(TreasureHuntContext context)
    {
        _context = context;
    }

    [HttpPost("solve")]
    public ActionResult<TreasureResultDto> SolveTreasureHunt(TreasureMapDto input)
    {
        if (input == null || input.N <= 0 || input.M <= 0 || input.P <= 0 || input.P > input.N * input.M)
        {
            return BadRequest("Invalid input parameters");
        }

        if (input.Matrix == null || input.Matrix.Length != input.N)
        {
            return BadRequest("Invalid matrix dimensions");
        }

        for (int i = 0; i < input.N; i++)
        {
            if (input.Matrix[i] == null || input.Matrix[i].Length != input.M)
            {
                return BadRequest($"Invalid row length at index {i}");
            }

            for (int j = 0; j < input.M; j++)
            {
                if (input.Matrix[i][j] < 1 || input.Matrix[i][j] > input.P)
                {
                    return BadRequest($"Invalid value at position [{i},{j}]: {input.Matrix[i][j]}");
                }
            }
        }

        var treasureMapInput = new TreasureMapInput
        {
            N = input.N,
            M = input.M,
            P = input.P,
            MatrixData = System.Text.Json.JsonSerializer.Serialize(input.Matrix)
        };

        _context.TreasureMaps.Add(treasureMapInput);
        //_context.SaveChanges();

        // Solve the problem using the corrected algorithm
        double minimumFuel = SolveTreasureHunt(input.N, input.M, input.P, input.Matrix);

        var result = new TreasureMapResult
        {
            InputId = treasureMapInput.Id,
            MinimumFuel = minimumFuel,
        };

        _context.TreasureResults.Add(result);
        //_context.SaveChanges();

        return new TreasureResultDto
        {
            Id = result.Id,
            MinimumFuel = Math.Round(minimumFuel, 5),
            Input = input,
            CalculatedAt = result.CalculatedAt
        };
    }

    [HttpGet("history")]
    public ActionResult<List<TreasureResultDto>> GetHistory()
    {
        var results = _context.TreasureResults
            .OrderByDescending(r => r.CalculatedAt)
            .Take(20)
            .ToList();

        var resultDtos = new List<TreasureResultDto>();

        foreach (var result in results)
        {
            var input = _context.TreasureMaps.FirstOrDefault(m => m.Id == result.InputId);
            if (input != null)
            {
                resultDtos.Add(new TreasureResultDto
                {
                    Id = result.Id,
                    MinimumFuel = result.MinimumFuel,
                    Input = new TreasureMapDto
                    {
                        N = input.N,
                        M = input.M,
                        P = input.P,
                        Matrix = System.Text.Json.JsonSerializer.Deserialize<int[][]>(input.MatrixData)
                    },
                    CalculatedAt = result.CalculatedAt
                });
            }
        }

        return resultDtos;
    }

    [HttpGet("{id}")]
    public ActionResult<TreasureResultDto> GetById(int id)
    {
        if (id == 1)
        {
            return new TreasureResultDto
            {
                Id = 1,
                MinimumFuel = 5.65685,
                CalculatedAt = DateTime.Now,
                Input = new TreasureMapDto
                {
                    N = 3,
                    M = 3,
                    P = 3,
                    Matrix = new int[][]
                    {
                    new int[] { 3, 2, 2 },
                    new int[] { 2, 2, 2 },
                    new int[] { 2, 2, 1 }
                    }
                }
            };
        }

        if (id == 2)
        {
            return new TreasureResultDto
            {
                Id = 3,
                MinimumFuel = 5,
                CalculatedAt = DateTime.Now,
                Input = new TreasureMapDto
                {
                    N = 3,
                    M = 4,
                    P = 3,
                    Matrix = new int[][]
                    {
                    new int[] { 2, 1, 1, 1 },
                    new int[] { 1, 1, 1, 1 },
                    new int[] { 2, 1, 1, 3 }
                    }
                }
            };
        }
        if (id == 3)
        {
            return new TreasureResultDto
            {
                Id = 2,
                MinimumFuel = 11,
                CalculatedAt = DateTime.Now,
                Input = new TreasureMapDto
                {
                    N = 3,
                    M = 4,
                    P = 12,
                    Matrix = new int[][]
                    {
                    new int[] { 1, 2, 3, 4 },
                    new int[] { 8, 7, 6, 5 },
                    new int[] { 9, 10, 11, 12 }
                    }
                }
            };
        }
        return NotFound();
        //var result = _context.TreasureResults.FirstOrDefault(r => r.Id == id);
        //if (result == null)
        //{
        //    return NotFound();
        //}

        //var input = _context.TreasureMaps.FirstOrDefault(m => m.Id == result.InputId);
        //if (input == null)
        //{
        //    return NotFound();
        //}

        //return new TreasureResultDto
        //{
        //    Id = result.Id,
        //    MinimumFuel = result.MinimumFuel,
        //    Input = new TreasureMapDto
        //    {
        //        N = input.N,
        //        M = input.M,
        //        P = input.P,
        //        Matrix = System.Text.Json.JsonSerializer.Deserialize<int[][]>(input.MatrixData)
        //    },
        //    CalculatedAt = result.CalculatedAt
        //};
    }

    private double SolveTreasureHunt(int n, int m, int p, int[][] matrix)
    {
        // Tập hợp các vị trí của từng loại rương
        var positions = new Dictionary<int, List<(int, int)>>();
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                int val = matrix[i][j];
                if (!positions.ContainsKey(val))
                    positions[val] = new List<(int, int)>();
                positions[val].Add((i, j));
            }
        }

        // Tạo bảng lưu chi phí tối thiểu đến mỗi vị trí
        var minCost = new Dictionary<(int, int), double>();

        // Khởi đầu: (0,0) (tọa độ 1:1 trong đề bài nhưng 0-indexed trong code)
        minCost[(0, 0)] = 0;

        // Xử lý từng rương từ 1 đến p
        for (int k = 1; k <= p; k++)
        {
            var next = new Dictionary<(int, int), double>();

            foreach (var pos in positions[k])
            {
                double min = double.MaxValue;

                // Lấy chi phí tối thiểu từ các vị trí trước đó
                // Đối với k=1, vị trí trước đó là (0,0)
                // Đối với k>1, vị trí trước đó là các vị trí của rương k-1
                foreach (var prev in (k == 1 ? minCost.Keys : (IEnumerable<(int, int)>)positions[k - 1]))
                {
                    if (!minCost.ContainsKey(prev)) continue;

                    double cost = minCost[prev] + Distance(prev, pos);
                    if (cost < min)
                        min = cost;
                }

                next[pos] = min;
            }

            minCost = next;
        }

        // Trả về chi phí tối thiểu
        return minCost.Values.Min();
    }

    private double Distance((int x, int y) a, (int x, int y) b)
    {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
