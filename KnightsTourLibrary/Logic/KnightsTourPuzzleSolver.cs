using KnightsTourLibrary.Models;
using Microsoft.Extensions.Logging;

namespace KnightsTourLibrary.Logic;

/// <summary>
/// Solve the Knight’s tour problem using Warnsdorff’s algorithm
/// </summary>
public class KnightsTourPuzzleSolver : IPuzzleSolver
{
	
	private readonly ILogger<KnightsTourPuzzleSolver> _log;

	private const int _boardSize = 8;

	// Move pattern on basis of the change of
	// x coordinates and y coordinates respectively
	public int[] _cx = {1, 1, 2, 2, -1, -1, -2, -2};
	public int[] _cy = {2, -2, 1, -1, 2, -2, 1, -1};

	public KnightsTourPuzzleSolver(ILogger<KnightsTourPuzzleSolver> log)
	{
		_log = log ?? throw new ArgumentNullException(nameof(log));
	}
	
	/// <summary>
	/// function restricts the knight to remain within the 8x8 chessboard 
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private static bool limits(int x, int y)
	{
		return x >= 0 && y >= 0 && x < _boardSize && y < _boardSize;
	}

	/// <summary>
	/// Checks whether a square is valid and empty or not 
	/// </summary>
	/// <param name="a"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private static bool isempty(IReadOnlyList<int> a, int x, int y)
	{
		return limits(x, y) && (a[y * _boardSize + x] < 0);
	}

	/// <summary>
	/// Returns the number of empty squares adjacent to (x, y) 
	/// </summary>
	/// <param name="a"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private int getDegree(IReadOnlyList<int> a, int x, int y)
	{
		int count = 0;
		for (int i = 0; i < _boardSize; ++i)
			if (isempty(a, (x + _cx[i]), (y + _cy[i])))
				count++;

		return count;
	}
	
	/// <summary>
	/// Picks next point using Warnsdorff's heuristic.
	/// Returns false if it is not possible to pick next point. 
	/// </summary>
	/// <param name="a"></param>
	/// <param name="cell"></param>
	/// <returns></returns>
	private Cell? nextMove(int[] a, Cell cell)
	{
		_log.LogDebug("From [{x},{y}]:", cell.x, cell.y);
		int min_deg_idx = -1;
		int min_deg = (_boardSize + 1);

		min_deg_idx = FindAdjacentMinDegree(a, out int nx, out int ny, cell, min_deg, min_deg_idx);

		// If we could not find a next cell
		if (min_deg_idx == -1)
			return null;

		// Store coordinates of next point
		nx = cell.x + _cx[min_deg_idx];
		ny = cell.y + _cy[min_deg_idx];

		// Mark next move
		a[ny * _boardSize + nx] = a[cell.y * _boardSize + cell.x] + 1;

		// Update next point
		cell.x = nx;
		cell.y = ny;

		_log.LogDebug("To [{x},{y}]:", cell.x, cell.y);
		return cell;
	}

	/// <summary>
	/// Try all N adjacent of (*x, *y) starting from a random adjacent. Find the adjacent with minimum degree.
	/// </summary>
	/// <param name="a"></param>
	/// <param name="nx"></param>
	/// <param name="ny"></param>
	/// <param name="cell"></param>
	/// <param name="min_deg"></param>
	/// <param name="min_deg_idx"></param>
	/// <returns></returns>
	private int FindAdjacentMinDegree(IReadOnlyList<int> a, out int nx, out int ny, Cell cell, int min_deg, int min_deg_idx)
	{
		nx = 0;
		ny = 0;

		Random random = new();
		int start = random.Next(0, 1000);
		for (int count = 0; count < _boardSize; ++count)
		{
			int i = (start + count) % _boardSize;
			nx = cell.x + _cx[i];
			ny = cell.y + _cy[i];
			int c;
			if ((!isempty(a, nx, ny)) ||
			    (c = getDegree(a, nx, ny)) >= min_deg) continue;
			min_deg_idx = i;
			min_deg = c;
		}

		return min_deg_idx;
	}

	/// <summary>
	/// Displays the chessboard with all the legal knight's moves 
	/// </summary>
	/// <param name="a"></param>
	public void Print(IReadOnlyList<int> a)
	{
		for (int i = 0; i < _boardSize; ++i)
		{
			for (int j = 0; j < _boardSize; ++j)
				Console.Write(a[j * _boardSize + i]+"\t");
			Console.Write("\n\n\n");
		}
	}
	
	/// <summary>
	/// Checks its neighbouring squares 
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="xx"></param>
	/// <param name="yy"></param>
	/// <returns>Returns true if the knight ends on a square that is oneknight's move from the beginning square,
	/// then tour is closed
	/// </returns>
	public bool IsNeighbour(int? x, int? y, int xx, int yy)
	{
		for (int i = 0; i < _boardSize; ++i)
			if (((x + _cx[i]) == xx) &&
				((y + _cy[i]) == yy))
				return true;

		return false;
	}

	/// <summary>
	/// Generates the legal moves using warnsdorff's heuristics.  
	/// </summary>
	/// <returns>Returns false if not possible</returns>
	public Tuple<bool, int[]> FindSolution()
	{
		
		// Filling up the chessboard matrix with -1's
		int[] a = new int[_boardSize * _boardSize];
		for (int i = 0; i < _boardSize * _boardSize; ++i)
			a[i] = -1;

		// initial position
		const int sx = 3;
		const int sy = 2;

		// Current points are same as initial points
		Cell cell = new(sx, sy);

		a[cell.y * _boardSize + cell.x] = 1; // Mark first move.

		// Keep picking next points using Warnsdorff's heuristic
		Cell? ret = null;
		for (int i = 0; i < _boardSize * _boardSize - 1; ++i)
		{
			ret = nextMove(a, cell);
			if (ret == null)
				return new Tuple<bool, int[]>(false, a);
		}

		// Check if tour is closed (Can end at starting point)
		return !IsNeighbour(ret?.x, ret?.y, sx, sy) 
			? new Tuple<bool, int[]>(false, a) 
			: new Tuple<bool, int[]>(true, a);
	}

}

