using SudokuVS.Sudoku.Models;

namespace SudokuVS.Sudoku.Solvers.Abstractions;

public interface ISudokuSolver
{
    /// <summary>
    ///     Return a solved copy of the input grid
    /// </summary>
    SudokuGrid? Solve(SudokuGrid grid);
}
