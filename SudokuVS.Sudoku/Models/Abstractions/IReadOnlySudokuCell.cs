namespace SudokuVS.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuCell : IHiddenSudokuCell
{
    int? Element { get; }
    IReadOnlyCollection<int> Annotations { get; }
}
