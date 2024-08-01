using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Models.Abstractions;

namespace SudokuVS.Sudoku.Serialization;

public class SudokuGridEnumerableSerializer
{
    public IEnumerable<int> ToEnumerable(IReadOnlySudokuGrid grid) => Enumerable.Range(0, 9).SelectMany(i => Enumerable.Range(0, 9).Select(j => grid[i, j].Element ?? 0));

    public SudokuGrid FromEnumerable(IEnumerable<int> values)
    {
        int[,] grid = new int[9, 9];
        int row = 0;
        int col = 0;

        foreach (int value in values)
        {
            grid[row, col] = value;

            col++;
            if (col > 8)
            {
                col = 0;
                row++;
            }
        }

        return new SudokuGrid(grid);
    }
}
