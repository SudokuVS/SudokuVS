namespace SudokuVS.Sudoku.Models.Abstractions;

public interface IHiddenSudokuCell
{
    int Row { get; }
    int Column { get; }
    int Region { get; }
    bool Locked { get; }
    bool Empty { get; }
    bool HasAnnotations { get; }
}
