using FluentAssertions;
using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Serialization;

namespace Tests.Sudoku;

[TestClass]
public class SudokuGridStringSerializerTest
{
    SudokuGridStringSerializer _serializer = null!;

    [TestInitialize]
    public void Initialize() => _serializer = new SudokuGridStringSerializer();

    [TestMethod]
    public void ShouldSerializeGridToIntegers()
    {
        SudokuGrid grid = SudokuGridTestUtils.RandomGrid;

        string serialized = _serializer.ToString(grid);

        serialized.Should().HaveLength(81);
        for (int i = 0; i < 81; i++)
        {
            serialized[i].ToString().Should().Be(grid[i].Element.ToString());
        }
    }

    [TestMethod]
    public void ShouldSerializeEmptyCellToZero()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();

        string serialized = _serializer.ToString(grid);

        serialized.Should().HaveLength(81);
        for (int i = 0; i < 81; i++)
        {
            serialized[i].Should().Be('0');
        }
    }

    [TestMethod]
    public void ShouldDeserializeToGrid()
    {
        string values = string.Join("", Enumerable.Range(0, 81).Select(_ => 5));

        SudokuGrid grid = _serializer.FromString(values);

        grid.Enumerate().Should().AllSatisfy(x => x.Cell.Element.Should().Be(5));
    }

    [TestMethod]
    public void ShouldDeserializeZeroToEmptyCell()
    {
        string values = string.Join("", Enumerable.Range(0, 81).Select(_ => 0));

        SudokuGrid grid = _serializer.FromString(values);

        grid.Enumerate().Should().AllSatisfy(x => x.Cell.IsEmpty.Should().BeTrue());
    }
}
