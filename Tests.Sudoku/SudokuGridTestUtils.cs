using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Serialization;

namespace Tests.Sudoku;

public static class SudokuGridTestUtils
{
    public static SudokuGrid RandomGrid => new SudokuGridEnumerableSerializer().FromEnumerable(Enumerable.Range(0, 81).Select(_ => Random.Shared.Next(1, 10)));
    public static SudokuGrid SolvedGrid => new SudokuGridStringSerializer().FromString("123456789456789123789123456234567891567891234891234567345678912678912345912345678");
}
