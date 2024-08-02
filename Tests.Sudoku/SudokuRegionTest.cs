using FluentAssertions;
using SudokuVS.Sudoku.Models;

namespace Tests.Sudoku;

[TestClass]
public class SudokuRegionTest
{
    const int RegionIndex = 4;

    SudokuGrid _grid = null!;
    SudokuRegion _region = null!;

    [TestInitialize]
    public void Initialize()
    {
        _grid = SudokuGrid.CreateEmpty();
        _region = _grid.Regions[RegionIndex];
    }

    [TestMethod]
    public void ShouldIndexGrid()
    {
        _region[0, 0].Should().BeSameAs(_grid[3, 3]);
        _region[0, 1].Should().BeSameAs(_grid[3, 4]);
        _region[0, 2].Should().BeSameAs(_grid[3, 5]);
        _region[1, 0].Should().BeSameAs(_grid[4, 3]);
        _region[1, 1].Should().BeSameAs(_grid[4, 4]);
        _region[1, 2].Should().BeSameAs(_grid[4, 5]);
        _region[2, 0].Should().BeSameAs(_grid[5, 3]);
        _region[2, 1].Should().BeSameAs(_grid[5, 4]);
        _region[2, 2].Should().BeSameAs(_grid[5, 5]);
    }

    [TestMethod]
    public void ShouldIndexGrid_Flat()
    {
        _region[0].Should().BeSameAs(_grid[3, 3]);
        _region[1].Should().BeSameAs(_grid[3, 4]);
        _region[2].Should().BeSameAs(_grid[3, 5]);
        _region[3].Should().BeSameAs(_grid[4, 3]);
        _region[4].Should().BeSameAs(_grid[4, 4]);
        _region[5].Should().BeSameAs(_grid[4, 5]);
        _region[6].Should().BeSameAs(_grid[5, 3]);
        _region[7].Should().BeSameAs(_grid[5, 4]);
        _region[8].Should().BeSameAs(_grid[5, 5]);
    }

    [TestMethod]
    public void ShouldBeCompleted_WhenAllCellsAreSet()
    {
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
        {
            _region[i, j].Element = 1;
        }

        _region.IsCompleted.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldNotBeCompleted_WhenSomeCellsAreNotSet() => _region.IsCompleted.Should().BeFalse();

    [TestMethod]
    public void ShouldBeValid_WhenAllCellsHaveDifferentElements()
    {
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
        {
            _region[i, j].Element = i * 3 + j + 1;
        }

        _region.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldBeValid_WhenAllCellsHaveDifferentElements_EvenIfCellsAreEmpty() => _region.IsValid.Should().BeTrue();

    [TestMethod]
    public void ShouldNotBeValid_WhenSomeCellsHaveSameElement()
    {
        _region[0, 0].Element = 1;
        _region[0, 1].Element = 1;

        _region.IsValid.Should().BeFalse();
    }
}
