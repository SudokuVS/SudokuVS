namespace SudokuVS.Sudoku.Models.Abstractions;

public interface IHiddenSudokuCell
{
    int Row { get; }
    int Column { get; }
    int Region { get; }
    bool IsLocked { get; }
    bool IsEmpty { get; }
    bool HasAnnotations { get; }
}
