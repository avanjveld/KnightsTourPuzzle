namespace KnightsTourLibrary.Models;

/// <summary>
/// Represents a cell in the Knight's Tour puzzle.
/// </summary>
internal class Cell
{
    /// <summary>
    /// Gets or sets the X-coordinate of the cell.
    /// </summary>
    public int X;

    /// <summary>
    /// Gets or sets the Y-coordinate of the cell.
    /// </summary>
    public int Y;

    /// <summary>
    /// Initializes a new instance of the <see cref="Cell"/> class.
    /// </summary>
    /// <param name="x">The X-coordinate of the cell.</param>
    /// <param name="y">The Y-coordinate of the cell.</param>
    public Cell(int x, int y)
    {
        X = x;
        Y = y;
    }
}