using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Serialization;

namespace Tests.Sudoku;

public static class SudokuGridTestUtils
{
    public static SudokuGrid RandomGrid => new SudokuGridEnumerableSerializer().FromEnumerable(Enumerable.Range(0, 81).Select(_ => Random.Shared.Next(1, 10)));

    public static SudokuGrid SolvedGrid =>
        new SudokuGridStringSerializer().FromString(
            "1;2;3;4;5;6;7;8;9;4;5;6;7;8;9;1;2;3;7;8;9;1;2;3;4;5;6;2;3;4;5;6;7;8;9;1;5;6;7;8;9;1;2;3;4;8;9;1;2;3;4;5;6;7;3;4;5;6;7;8;9;1;2;6;7;8;9;1;2;3;4;5;9;1;2;3;4;5;6;7;8"
        );

    public static SudokuGrid Repeat(params int[] values)
    {
        int[,] cells = new int[9, 9];
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            int flatIndex = i * 9 + j;
            cells[i, j] = values[flatIndex % values.Length];
        }

        return new SudokuGrid(cells);
    }
}
