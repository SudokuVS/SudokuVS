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
        SudokuGrid grid = SudokuGridTestUtils.Repeat(1, 2, 3, 4, 5, 6, 7, 8, 9);

        string serialized = _serializer.ToString(grid);

        serialized.Should()
            .Be(
                "1;2;3;4;5;6;7;8;9;1;2;3;4;5;6;7;8;9;1;2;3;4;5;6;7;8;9;1;2;3;4;5;6;7;8;9;1;2;3;4;5;6;7;8;9;1;2;3;4;5;6;7;8;9;1;2;3;4;5;6;7;8;9;1;2;3;4;5;6;7;8;9;1;2;3;4;5;6;7;8;9;"
            );
    }

    [TestMethod]
    public void ShouldDeserializeToGrid()
    {
        string values = string.Join(";", Enumerable.Range(0, 81).Select(_ => 5));

        SudokuGrid grid = _serializer.FromString(values);

        grid.Enumerate().Should().AllSatisfy(cell => cell.Element.Should().Be(5));
    }

    [TestMethod]
    public void ShouldSerializeEmptyCellToZero()
    {
        SudokuGrid grid = SudokuGridTestUtils.Repeat(0);

        string serialized = _serializer.ToString(grid);

        serialized.Should().StartWith("0;");
    }

    [TestMethod]
    public void ShouldDeserializeZeroToEmptyCell()
    {
        string values = string.Join(";", Enumerable.Range(0, 81).Select(_ => 0));

        SudokuGrid grid = _serializer.FromString(values);

        grid[0, 0].IsEmpty.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldSerializeLock()
    {
        SudokuGrid grid = SudokuGridTestUtils.Repeat(0);
        grid[0, 0].IsLocked = true;

        string serialized = _serializer.ToString(grid);

        serialized.Should().StartWith("0l;");
    }

    [TestMethod]
    public void ShouldDeserializeLock()
    {
        string values = "0l;" + string.Join(";", Enumerable.Range(0, 80).Select(_ => 0));

        SudokuGrid grid = _serializer.FromString(values);

        grid[0, 0].IsLocked.Should().BeTrue();
    }

    [TestMethod]
    public void ShouldSerializeAnnotations()
    {
        SudokuGrid grid = SudokuGridTestUtils.Repeat(0);
        grid[0, 0].Annotations.Add(1);
        grid[0, 1].Annotations.Add(2);
        grid[0, 1].Annotations.Add(3);

        string serialized = _serializer.ToString(grid);

        serialized.Should().StartWith("0|1;0|23;");
    }

    [TestMethod]
    public void ShouldDeserializeAnnotations()
    {
        string values = "0|1;0|23;" + string.Join(";", Enumerable.Range(0, 79).Select(_ => 0));

        SudokuGrid grid = _serializer.FromString(values);

        grid[0, 0].Annotations.Should().BeEquivalentTo([1]);
        grid[0, 1].Annotations.Should().BeEquivalentTo([2, 3]);
    }
}
