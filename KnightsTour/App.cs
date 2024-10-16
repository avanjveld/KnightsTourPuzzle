using KnightsTourLibrary.Logic;

namespace KnightsTour;

public class App
{
    private readonly IPuzzleSolver _puzzleSolver;

    public App(IPuzzleSolver puzzleSolver)
    {
        _puzzleSolver = puzzleSolver;
    }

    /// <example> KnightsTour.exe -boardSize=8</example>
    /// <param name="args">Board size</param>
    public void Run(string[] args)
    {
        var succeed = false;
        var attemptNo = 0;
        
        while (!succeed)
        {
            attemptNo++;
            (succeed, var result) = _puzzleSolver.FindSolution();

            if (!succeed)
            {
                Console.WriteLine($"Puzzle could not be solved (Attempt {attemptNo}) .. Trying again");
                continue;
            }

            _puzzleSolver.PrintBoard(result);
            Console.WriteLine($"Puzzle solved in {attemptNo} attempts.");
        }
    }
}