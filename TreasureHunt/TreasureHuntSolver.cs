namespace TreasureHunt;

public class TreasureHuntSolver
{
    private readonly int _n;
    private readonly int _m;
    private readonly int _p;
    private readonly int[][] _matrix;

    // Store positions of each chest number
    private readonly Dictionary<int, List<(int row, int col)>> _chestPositions;

    public TreasureHuntSolver(int n, int m, int p, int[][] matrix)
    {
        _n = n;
        _m = m;
        _p = p;
        _matrix = matrix;

        // Initialize chest positions dictionary
        _chestPositions = new Dictionary<int, List<(int row, int col)>>();
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                int chestNumber = matrix[i][j];
                if (!_chestPositions.ContainsKey(chestNumber))
                {
                    _chestPositions[chestNumber] = new List<(int, int)>();
                }
                _chestPositions[chestNumber].Add((i, j));
            }
        }
    }

    public (double fuel, List<(int keyNumber, int row, int col)> path) Solve()
    {
        // Starting position is (0,0) in 0-indexed matrix with key 0
        // We need to find keys 1, 2, ..., p in sequence

        var path = new List<(int keyNumber, int row, int col)>();
        double totalFuel = 0;

        // Initial position (1,1) in 1-indexed or (0,0) in 0-indexed
        int currentRow = 0;
        int currentCol = 0;

        path.Add((0, currentRow, currentCol));

        // For each key we need to find (1 to p)
        for (int keyNeeded = 1; keyNeeded <= _p; keyNeeded++)
        {
            // Find all chests with the number keyNeeded
            if (!_chestPositions.ContainsKey(keyNeeded))
            {
                throw new InvalidOperationException($"No chest with number {keyNeeded} found in the matrix");
            }

            // Find the closest chest with the required key
            double minDistance = double.MaxValue;
            (int row, int col) nextChestPosition = (-1, -1);

            foreach (var position in _chestPositions[keyNeeded])
            {
                double distance = CalculateDistance(currentRow, currentCol, position.row, position.col);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nextChestPosition = position;
                }
            }

            // Move to the chest, add fuel used
            totalFuel += minDistance;
            currentRow = nextChestPosition.row;
            currentCol = nextChestPosition.col;

            // Add to path
            path.Add((keyNeeded, currentRow, currentCol));
        }

        return (totalFuel, path);
    }

    private double CalculateDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
    }
}
