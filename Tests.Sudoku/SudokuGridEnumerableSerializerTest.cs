using FluentAssertions;
using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Serialization;

namespace Tests.Sudoku;

[TestClass]
public class SudokuGridEnumerableSerializerTest
{
    SudokuGridEnumerableSerializer _serializer = null!;

    [TestInitialize]
    public void Initialize() => _serializer = new SudokuGridEnumerableSerializer();

    [TestMethod]
    public void ShouldSerializeGridToIntegers()
    {
        SudokuGrid grid = SudokuGridTestUtils.RandomGrid;

        int[] serialized = _serializer.ToEnumerable(grid).ToArray();

        serialized.Should().HaveCount(81);
        for (int i = 0; i < 81; i++)
        {
            serialized[i].Should().Be(grid[i].Element);
        }
    }

    [TestMethod]
    public void ShouldSerializeEmptyCellToZero()
    {
        SudokuGrid grid = SudokuGrid.CreateEmpty();

        int[] serialized = _serializer.ToEnumerable(grid).ToArray();

        serialized.Should().HaveCount(81);
        serialized.Should().AllSatisfy(e => e.Should().Be(0));
    }

    [TestMethod]
    public void ShouldDeserializeToGrid()
    {
        IEnumerable<int> values = Enumerable.Range(0, 81).Select(_ => 5);

        SudokuGrid grid = _serializer.FromEnumerable(values);

        grid.Enumerate().Should().AllSatisfy(x => x.Cell.Element.Should().Be(5));
    }

    [TestMethod]
    public void ShouldDeserializeZeroToEmptyCell()
    {
        IEnumerable<int> values = Enumerable.Range(0, 81).Select(_ => 0);

        SudokuGrid grid = _serializer.FromEnumerable(values);

        grid.Enumerate().Should().AllSatisfy(x => x.Cell.IsEmpty.Should().BeTrue());
    }
}
