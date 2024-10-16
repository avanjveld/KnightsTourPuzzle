using KnightsTourLibrary.Models;
using Microsoft.Extensions.Logging;

namespace KnightsTourLibrary.Logic;

/// <summary>
/// Solve the Knight’s tour problem using Warnsdorff’s algorithm
/// </summary>
public class KnightsTourPuzzleSolver : IPuzzleSolver
{
    private readonly ILogger<KnightsTourPuzzleSolver> _logger;

    private const int BoardSize = 8;

    // Move pattern on basis of the change of
    // x coordinates and y coordinates respectively
    private readonly int[] _xMoves = {1, 1, 2, 2, -1, -1, -2, -2};
    private readonly int[] _yMoves = {2, -2, 1, -1, 2, -2, 1, -1};

    public KnightsTourPuzzleSolver(ILogger<KnightsTourPuzzleSolver> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Restricts the knight to remain within the 8x8 chessboard
    /// </summary>
    private static bool IsWithinBoardLimits(int x, int y)
    {
        return x >= 0 && y >= 0 && x < BoardSize && y < BoardSize;
    }

    /// <summary>
    /// Checks whether a square is valid and empty or not
    /// </summary>
    private static bool IsValidAndEmpty(IReadOnlyList<int> board, int x, int y)
    {
        return IsWithinBoardLimits(x, y) && (board[y * BoardSize + x] < 0);
    }

    /// <summary>
    /// Returns the number of empty squares adjacent to (x, y)
    /// </summary>
    private int GetAdjacentEmptySquareCount(IReadOnlyList<int> board, int x, int y)
    {
        var count = 0;
        for (var i = 0; i < BoardSize; ++i)
            if (IsValidAndEmpty(board, (x + _xMoves[i]), (y + _yMoves[i])))
                count++;

        return count;
    }

    /// <summary>
    /// Picks next point using Warnsdorff's heuristic.
    /// Returns false if it is not possible to pick next point.
    /// </summary>
    private Cell? GetNextMove(int[] board, Cell currentCell)
    {
        _logger.LogDebug("From [{x},{y}]:", currentCell.X, currentCell.Y);
        var minDegreeIndex = -1;
        const int minDegree = (BoardSize + 1);

        minDegreeIndex = FindMinDegreeAdjacentCell(board, out var nextX, out var nextY, currentCell, minDegree, minDegreeIndex);

        // If we could not find a next cell
        if (minDegreeIndex == -1)
            return null;

        // Store coordinates of next point
        nextX = currentCell.X + _xMoves[minDegreeIndex];
        nextY = currentCell.Y + _yMoves[minDegreeIndex];

        // Mark next move
        board[nextY * BoardSize + nextX] = board[currentCell.Y * BoardSize + currentCell.X] + 1;

        // Update next point
        currentCell.X = nextX;
        currentCell.Y = nextY;

        _logger.LogDebug("To [{x},{y}]:", currentCell.X, currentCell.Y);
        return currentCell;
    }

    /// <summary>
    /// Try all N adjacent of (*x, *y) starting from a random adjacent. Find the adjacent with minimum degree.
    /// </summary>
    private int FindMinDegreeAdjacentCell(IReadOnlyList<int> board, out int nextX, out int nextY, Cell currentCell, int minDegree, int minDegreeIndex)
    {
        nextX = 0;
        nextY = 0;

        Random random = new();
        var start = random.Next(0, 1000);
        for (var count = 0; count < BoardSize; ++count)
        {
            var i = (start + count) % BoardSize;
            nextX = currentCell.X + _xMoves[i];
            nextY = currentCell.Y + _yMoves[i];
            int degree;
            if ((!IsValidAndEmpty(board, nextX, nextY)) ||
                (degree = GetAdjacentEmptySquareCount(board, nextX, nextY)) >= minDegree) continue;
            minDegreeIndex = i;
            minDegree = degree;
        }

        return minDegreeIndex;
    }

    /// <summary>
    /// Displays the chessboard with all the legal knight's moves
    /// </summary>
    public void PrintBoard(IReadOnlyList<int> board)
    {
        for (var i = 0; i < BoardSize; ++i)
        {
            for (var j = 0; j < BoardSize; ++j)
                Console.Write(board[j * BoardSize + i] + "\t");
            Console.Write("\n\n\n");
        }
    }

    /// <summary>
    /// Checks its neighbouring squares
    /// </summary>
    public bool IsNeighbouringSquare(int? x, int? y, int targetX, int targetY)
    {
        for (var i = 0; i < BoardSize; ++i)
            if (((x + _xMoves[i]) == targetX) &&
                ((y + _yMoves[i]) == targetY))
                return true;

        return false;
    }

    /// <summary>
    /// Generates the legal moves using Warnsdorff's heuristics.
    /// </summary>
    public Tuple<bool, int[]> FindSolution()
    {
        // Filling up the chessboard matrix with -1's
        var board = new int[BoardSize * BoardSize];
        for (var i = 0; i < BoardSize * BoardSize; ++i)
            board[i] = -1;

        // initial position
        const int startX = 3;
        const int startY = 2;

        // Current points are same as initial points
        Cell currentCell = new(startX, startY);

        board[currentCell.Y * BoardSize + currentCell.X] = 1; // Mark first move.

        // Keep picking next points using Warnsdorff's heuristic
        Cell? nextCell = null;
        for (var i = 0; i < BoardSize * BoardSize - 1; ++i)
        {
            nextCell = GetNextMove(board, currentCell);
            if (nextCell == null)
                return new Tuple<bool, int[]>(false, board);
        }

        // Check if tour is closed (Can end at starting point)
        return !IsNeighbouringSquare(nextCell?.X, nextCell?.Y, startX, startY)
            ? new Tuple<bool, int[]>(false, board)
            : new Tuple<bool, int[]>(true, board);
    }
}