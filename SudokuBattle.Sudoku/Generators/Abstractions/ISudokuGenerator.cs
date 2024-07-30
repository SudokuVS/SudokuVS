using SudokuBattle.Sudoku.Models;

namespace SudokuBattle.Sudoku.Generators.Abstractions;

public interface ISudokuGenerator
{
    /// <summary>
    ///     Generate a new sudoku grid
    /// </summary>
    SudokuGrid? Generate();
}
