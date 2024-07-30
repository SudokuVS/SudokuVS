using System.Diagnostics.CodeAnalysis;

namespace SudokuBattle.Sudoku.Models.Abstractions;

public interface IReadOnlySudokuCell
{
    int? Element { get; }
    bool Locked { get; }

    [MemberNotNullWhen(false, nameof(Element))]
    bool Empty { get; }
}
