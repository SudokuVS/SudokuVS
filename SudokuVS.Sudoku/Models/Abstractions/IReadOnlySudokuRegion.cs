﻿namespace SudokuVS.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuRegion
{
    IReadOnlySudokuCell this[int rowIndex, int colIndex] { get; }
}
