using FluentAssertions;
using FluentAssertions.Events;
using SudokuVS.Sudoku.Models;

namespace Tests.Sudoku;

[TestClass]
public class SudokuGridTest
{
    [TestMethod]
    public void ShouldCreateGridFromValues()
    {
        int[,] values = new int[9, 9];
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            values[i, j] = 5;
        }

        SudokuGrid grid = new(values);

        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            grid[i, j].Element.Should().Be(5);
            grid[i, j].IsLocked.Should().BeFalse();
        }
    }

    [TestMethod]
    public void ShouldCreateGridFromCells()
    {
        SudokuCell[,] cells = new SudokuCell[9, 9];
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            cells[i, j] = new SudokuCell(i, j);
        }

        SudokuGrid grid = new(cells);

        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            grid[i, j].Should().BeSameAs(cells[i, j]);
        }
    }

    [TestMethod]
    public void ShouldBeCompleted_WhenAllCellsAreSet()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();

        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            grid[i, j].Element = 1;
        }

        grid.IsCompleted.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldBeValid_WhenGridIsValid()
    {
        SudokuGrid grid = SudokuGridTestUtils.SolvedGrid;

        grid.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldBeValid_WhenGridIsValid_EvenIfSomeCellsAreEmpty()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();

        grid.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldRaiseCellElementChanged_SetElement()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();
        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].Element = 1;

        monitor.Should().Raise("CellElementChanged").WithArgs<(int Row, int Column)>(args => args.Row == 1 && args.Column == 2);
    }

    [TestMethod]
    public void ShouldNotRaiseCellElementChanged_SetSameElement()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();
        grid[1, 2].Element = 1;
        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].Element = 1;

        monitor.Should().NotRaise("CellElementChanged");
    }

    [TestMethod]
    public void ShouldRaiseCellElementChanged_ClearElement()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();
        grid[1, 2].Element = 1;
        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].Element = null;

        monitor.Should().Raise("CellElementChanged").WithArgs<(int Row, int Column)>(args => args.Row == 1 && args.Column == 2);
    }

    [TestMethod]
    public void ShouldRaiseCellAnnotationsChanged_Add()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();
        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].Annotations.Add(1);

        monitor.Should().Raise("CellAnnotationsChanged").WithArgs<(int Row, int Column)>(args => args.Row == 1 && args.Column == 2);
    }

    [TestMethod]
    public void ShouldNotRaiseCellAnnotationsChanged_AddExistingElement()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();
        grid[1, 2].Annotations.Add(1);
        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].Annotations.Add(1);

        monitor.Should().NotRaise("CellAnnotationsChanged");
    }

    [TestMethod]
    public void ShouldRaiseCellAnnotationsChanged_Remove()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();
        grid[1, 2].Annotations.Add(1);

        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].Annotations.Remove(1);

        monitor.Should().Raise("CellAnnotationsChanged").WithArgs<(int Row, int Column)>(args => args.Row == 1 && args.Column == 2);
    }

    [TestMethod]
    public void ShouldNotRaiseCellAnnotationsChanged_RemoveNonExistentElement()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();
        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].Annotations.Remove(1);

        monitor.Should().NotRaise("CellAnnotationsChanged");
    }

    [TestMethod]
    public void ShouldRaiseCellAnnotationsChanged_Clear()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();
        grid[1, 2].Annotations.Add(1);

        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].Annotations.Clear();

        monitor.Should().Raise("CellAnnotationsChanged").WithArgs<(int Row, int Column)>(args => args.Row == 1 && args.Column == 2);
    }

    [TestMethod]
    public void ShouldRaiseCellLockChanged_SetIsLocked()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();

        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].IsLocked = true;

        monitor.Should().Raise("CellLockChanged").WithArgs<(int Row, int Column)>(args => args.Row == 1 && args.Column == 2);
    }

    [TestMethod]
    public void ShouldNotRaiseCellLockChanged_SetSameIsLocked()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();

        using IMonitor<SudokuGrid>? monitor = grid.Monitor();

        grid[1, 2].IsLocked = false;

        monitor.Should().NotRaise("CellLockChanged");
    }

    [TestMethod]
    public void ShouldLockNonEmptyCells()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();

        grid[0, 0].Element = 1;
        grid.LockNonEmptyCells();

        grid[0, 0].IsLocked.Should().BeTrue();

        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            if (i == 0 && j == 0)
            {
                continue;
            }

            grid[i, j].IsLocked.Should().BeFalse();
        }
    }

    [TestMethod]
    public void ShouldClone()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();

        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            grid[i, j].Element = (i + j) % 9 + 1;
            grid[i, j].IsLocked = i > j;
        }

        SudokuGrid clonedGrid = SudokuGrid.Clone(grid);

        clonedGrid.Should().NotBeSameAs(grid);
        clonedGrid.Should().BeEquivalentTo(grid);
    }
}
