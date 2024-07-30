using Sudoku;
using SudokuBattle.Sudoku.Models;
using SudokuBattle.Sudoku.Serialization;
using SudokuBattle.Sudoku.Solvers.Abstractions;

namespace SudokuBattle.Sudoku.Solvers;

public class SudokuLibSolver : ISudokuSolver
{
    readonly Solver _solver = new(HistoryType.None, SolveMethod.FindFirst);
    readonly SudokuArraySerializer _serializer = new();

    public SudokuGrid? Solve(SudokuGrid grid)
    {
        int[] flattened = _serializer.ToEnumerable(grid).ToArray();
        SudokuResult? result = _solver.Solve(flattened);
        if (result == null)
        {
            return null;
        }

        IEnumerable<int> solution = Enumerable.Range(0, 81).Select(i => result[i]);
        return _serializer.FromEnumerable(solution);
    }
}
