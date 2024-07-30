using SudokuBattle.Sudoku.Models;

namespace SudokuBattle.Sudoku.Serialization;

public class SudokuArraySerializer
{
    public IEnumerable<int> ToEnumerable(SudokuGrid grid)
    {
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            yield return grid[i, j].Element ?? 0;
        }
    }

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
