using FluentAssertions;
using SudokuVS.Sudoku.Models;

namespace Tests.Sudoku;

[TestClass]
public class SudokuRowTest
{
    const int RowIndex = 1;

    SudokuGrid _grid = null!;
    SudokuRow _row = null!;

    [TestInitialize]
    public void Initialize()
    {
        _grid = SudokuGrid.CreateEmpty();
        _row = _grid.Rows[RowIndex];
    }

    [TestMethod]
    public void ShouldIndexGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            _row[i].Should().BeSameAs(_grid[RowIndex, i]);
        }
    }

    [TestMethod]
    public void ShouldBeCompleted_WhenAllCellsAreSet()
    {
        for (int i = 0; i < 9; i++)
        {
            _row[i].Element = 1;
        }

        _row.IsCompleted.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldNotBeCompleted_WhenSomeCellsAreNotSet() => _row.IsCompleted.Should().BeFalse();

    [TestMethod]
    public void ShouldBeValid_WhenAllCellsHaveDifferentElements()
    {
        for (int i = 0; i < 9; i++)
        {
            _row[i].Element = i + 1;
        }

        _row.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldBeValid_WhenAllCellsHaveDifferentElements_EvenIfCellsAreEmpty() => _row.IsValid.Should().BeTrue();

    [TestMethod]
    public void ShouldNotBeValid_WhenSomeCellsHaveSameElement()
    {
        _row[0].Element = 1;
        _row[1].Element = 1;

        _row.IsValid.Should().BeFalse();
    }
}
