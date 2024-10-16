namespace KnightsTourLibrary.Logic;

public interface IPuzzleSolver
{
    bool IsNeighbouringSquare(int? x, int? y, int targetX, int targetY);
    Tuple<bool, int[]> FindSolution();
    void PrintBoard(IReadOnlyList<int> board);
}