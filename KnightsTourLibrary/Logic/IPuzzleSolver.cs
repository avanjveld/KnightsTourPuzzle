namespace KnightsTourLibrary.Logic;

public interface IPuzzleSolver
{
    bool IsNeighbour(int? x, int? y, int xx, int yy);
    Tuple<bool, int[]> FindSolution();
    void Print(IReadOnlyList<int> a);
}