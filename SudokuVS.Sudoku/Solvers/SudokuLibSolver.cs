﻿using Sudoku;
using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Serialization;
using SudokuVS.Sudoku.Solvers.Abstractions;

namespace SudokuVS.Sudoku.Solvers;

public class SudokuLibSolver : ISudokuSolver
{
    readonly Solver _solver = new(HistoryType.None, SolveMethod.FindFirst);
    readonly SudokuGridEnumerableSerializer _serializer = new();

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
