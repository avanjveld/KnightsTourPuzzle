using KnightsTourLibrary.Logic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace KnightsTourTests.Logic;

public class KnightsTourPuzzleSolverTests
{
    [Theory]
    [InlineData(5, 3, 3, 2, true)]
    [InlineData(5, 3, 3, 1, false)]
    public void IsNeighbour_True(int x, int y, int xx, int yy, bool expected)
    {
        ILogger<KnightsTourPuzzleSolver> logger = new NullLogger<KnightsTourPuzzleSolver>();
        IPuzzleSolver solver = new KnightsTourPuzzleSolver(logger);
        bool actual = solver.IsNeighbouringSquare(x, y, xx, yy);
        Assert.Equal(expected, actual);
    }
}