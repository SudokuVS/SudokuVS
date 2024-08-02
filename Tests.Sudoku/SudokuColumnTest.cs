using FluentAssertions;
using SudokuVS.Sudoku.Models;

namespace Tests.Sudoku;

[TestClass]
public class SudokuColumnTest
{
    const int ColumnIndex = 1;

    SudokuGrid _grid = null!;
    SudokuColumn _column = null!;

    [TestInitialize]
    public void Initialize()
    {
        _grid = SudokuGrid.CreateEmpty();
        _column = _grid.Columns[ColumnIndex];
    }

    [TestMethod]
    public void ShouldIndexGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            _column[i].Should().BeSameAs(_grid[i, ColumnIndex]);
        }
    }

    [TestMethod]
    public void ShouldBeCompleted_WhenAllCellsAreSet()
    {
        for (int i = 0; i < 9; i++)
        {
            _column[i].Element = 1;
        }

        _column.IsCompleted.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldNotBeCompleted_WhenSomeCellsAreNotSet() => _column.IsCompleted.Should().BeFalse();

    [TestMethod]
    public void ShouldBeValid_WhenAllCellsHaveDifferentElements()
    {
        for (int i = 0; i < 9; i++)
        {
            _column[i].Element = i + 1;
        }

        _column.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldBeValid_WhenAllCellsHaveDifferentElements_EvenIfCellsAreEmpty() => _column.IsValid.Should().BeTrue();

    [TestMethod]
    public void ShouldNotBeValid_WhenSomeCellsHaveSameElement()
    {
        _column[0].Element = 1;
        _column[1].Element = 1;

        _column.IsValid.Should().BeFalse();
    }
}
