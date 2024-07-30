namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IHiddenSudokuCell
{
    bool Locked { get; }
    bool Empty { get; }
    bool HasAnnotations { get; }
}
