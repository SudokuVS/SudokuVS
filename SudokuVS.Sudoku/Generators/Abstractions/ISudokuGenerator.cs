using SudokuVS.Sudoku.Models;

namespace SudokuVS.Sudoku.Generators.Abstractions;

public interface ISudokuGenerator
{
    /// <summary>
    ///     Generate a new sudoku grid
    /// </summary>
    SudokuGrid? Generate();
}
