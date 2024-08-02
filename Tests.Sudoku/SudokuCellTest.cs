using FluentAssertions;
using FluentAssertions.Events;
using SudokuVS.Sudoku.Models;

namespace Tests.Sudoku;

[TestClass]
public class SudokuCellTest
{
    [DataTestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, 0)]
    [DataRow(0, 2, 0)]
    [DataRow(1, 0, 0)]
    [DataRow(1, 1, 0)]
    [DataRow(1, 2, 0)]
    [DataRow(2, 0, 0)]
    [DataRow(2, 1, 0)]
    [DataRow(2, 2, 0)]
    [DataRow(0, 3, 1)]
    [DataRow(0, 4, 1)]
    [DataRow(0, 5, 1)]
    [DataRow(1, 3, 1)]
    [DataRow(1, 4, 1)]
    [DataRow(1, 5, 1)]
    [DataRow(2, 3, 1)]
    [DataRow(2, 4, 1)]
    [DataRow(2, 5, 1)]
    [DataRow(0, 6, 2)]
    [DataRow(0, 7, 2)]
    [DataRow(0, 8, 2)]
    [DataRow(1, 6, 2)]
    [DataRow(1, 7, 2)]
    [DataRow(1, 8, 2)]
    [DataRow(2, 6, 2)]
    [DataRow(2, 7, 2)]
    [DataRow(2, 8, 2)]
    [DataRow(3, 0, 3)]
    [DataRow(3, 1, 3)]
    [DataRow(3, 2, 3)]
    [DataRow(4, 0, 3)]
    [DataRow(4, 1, 3)]
    [DataRow(4, 2, 3)]
    [DataRow(5, 0, 3)]
    [DataRow(5, 1, 3)]
    [DataRow(5, 2, 3)]
    [DataRow(3, 3, 4)]
    [DataRow(3, 4, 4)]
    [DataRow(3, 5, 4)]
    [DataRow(4, 3, 4)]
    [DataRow(4, 4, 4)]
    [DataRow(4, 5, 4)]
    [DataRow(5, 3, 4)]
    [DataRow(5, 4, 4)]
    [DataRow(5, 5, 4)]
    [DataRow(3, 6, 5)]
    [DataRow(3, 7, 5)]
    [DataRow(3, 8, 5)]
    [DataRow(4, 6, 5)]
    [DataRow(4, 7, 5)]
    [DataRow(4, 8, 5)]
    [DataRow(5, 6, 5)]
    [DataRow(5, 7, 5)]
    [DataRow(5, 8, 5)]
    [DataRow(6, 0, 6)]
    [DataRow(6, 1, 6)]
    [DataRow(6, 2, 6)]
    [DataRow(7, 0, 6)]
    [DataRow(7, 1, 6)]
    [DataRow(7, 2, 6)]
    [DataRow(8, 0, 6)]
    [DataRow(8, 1, 6)]
    [DataRow(8, 2, 6)]
    [DataRow(6, 3, 7)]
    [DataRow(6, 4, 7)]
    [DataRow(6, 5, 7)]
    [DataRow(7, 3, 7)]
    [DataRow(7, 4, 7)]
    [DataRow(7, 5, 7)]
    [DataRow(8, 3, 7)]
    [DataRow(8, 4, 7)]
    [DataRow(8, 5, 7)]
    [DataRow(6, 6, 8)]
    [DataRow(6, 7, 8)]
    [DataRow(6, 8, 8)]
    [DataRow(7, 6, 8)]
    [DataRow(7, 7, 8)]
    [DataRow(7, 8, 8)]
    [DataRow(8, 6, 8)]
    [DataRow(8, 7, 8)]
    [DataRow(8, 8, 8)]
    public void ShouldComputeRegion(int row, int column, int expectedRegion)
    {
        SudokuCell cell = new(row, column, 1);

        cell.Region.Should().Be(expectedRegion);
    }

    [TestMethod]
    public void ShouldRaiseElementChanged_SetElement()
    {
        SudokuCell cell = new(0, 0, 1);
        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.Element = 2;

        monitor.Should().Raise("ElementChanged");
    }

    [TestMethod]
    public void ShouldNotRaiseElementChanged_SetSameElement()
    {
        SudokuCell cell = new(0, 0, 1);
        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.Element = 1;

        monitor.Should().NotRaise("ElementChanged");
    }

    [TestMethod]
    public void ShouldRaiseElementChanged_ClearElement()
    {
        SudokuCell cell = new(0, 0, 1);
        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.Element = null;

        monitor.Should().Raise("ElementChanged");
    }

    [TestMethod]
    public void ShouldRaiseAnnotationsChanged_Add()
    {
        SudokuCell cell = new(0, 0, 1);
        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.Annotations.Add(1);

        monitor.Should().Raise("AnnotationsChanged");
    }

    [TestMethod]
    public void ShouldNotRaiseAnnotationsChanged_AddExistingElement()
    {
        SudokuCell cell = new(0, 0, 1);
        cell.Annotations.Add(1);
        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.Annotations.Add(1);

        monitor.Should().NotRaise("AnnotationsChanged");
    }

    [TestMethod]
    public void ShouldRaiseAnnotationsChanged_Remove()
    {
        SudokuCell cell = new(0, 0, 1);
        cell.Annotations.Add(1);

        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.Annotations.Remove(1);

        monitor.Should().Raise("AnnotationsChanged");
    }

    [TestMethod]
    public void ShouldNotRaiseAnnotationsChanged_RemoveNonExistentElement()
    {
        SudokuCell cell = new(0, 0, 1);
        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.Annotations.Remove(1);

        monitor.Should().NotRaise("AnnotationsChanged");
    }

    [TestMethod]
    public void ShouldRaiseAnnotationsChanged_Clear()
    {
        SudokuCell cell = new(0, 0, 1);
        cell.Annotations.Add(1);

        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.Annotations.Clear();

        monitor.Should().Raise("AnnotationsChanged");
    }

    [TestMethod]
    public void ShouldRaiseLockChanged_SetIsLocked()
    {
        SudokuCell cell = new(0, 0, 1);

        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.IsLocked = true;

        monitor.Should().Raise("LockChanged");
    }

    [TestMethod]
    public void ShouldNotRaiseLockChanged_SetSameIsLocked()
    {
        SudokuCell cell = new(0, 0, 1);

        using IMonitor<SudokuCell>? monitor = cell.Monitor();

        cell.IsLocked = false;

        monitor.Should().NotRaise("LockChanged");
    }

    [TestMethod]
    public void ShouldClone()
    {
        SudokuCell cell = new(1, 2, 3) { Annotations = { 4, 5, 6 }, IsLocked = true };

        SudokuCell clonedCell = SudokuCell.Clone(cell);

        clonedCell.Should().NotBe(cell);
        clonedCell.Should().BeEquivalentTo(cell);
    }

    [DataTestMethod]
    [DataRow(-1)]
    [DataRow(9)]
    public void ShouldNotInstantiate_WhenRowOutOfRange(int value)
    {
        Action action = () => _ = new SudokuCell(value, 0, 1);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [DataTestMethod]
    [DataRow(-1)]
    [DataRow(9)]
    public void ShouldNotInstantiate_WhenColumnOutOfRange(int value)
    {
        Action action = () => _ = new SudokuCell(0, value, 1);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void ShouldNotInstantiate_WhenElementOutOfRange(int value)
    {
        Action action = () => _ = new SudokuCell(0, 0, value);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void ShouldNotSetElement_WhenElementOutOfRange(int value)
    {
        SudokuCell cell = new(0, 0, 1);
        Action action = () => cell.Element = value;

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void ShouldNotSetElement_WhenCellIsLocked()
    {
        SudokuCell cell = new(0, 0, 1) { IsLocked = true };

        Action action = () => cell.Element = 1;

        action.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void ShouldNotAddAnnotation_WhenCellIsLocked()
    {
        SudokuCell cell = new(0, 0, 1) { IsLocked = true };

        Action action = () => cell.Annotations.Add(1);

        action.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void ShouldNotRemoveAnnotation_WhenCellIsLocked()
    {
        SudokuCell cell = new(0, 0, 1) { IsLocked = true };

        Action action = () => cell.Annotations.Remove(1);

        action.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void ShouldNotClearAnnotation_WhenCellIsLocked()
    {
        SudokuCell cell = new(0, 0, 1) { IsLocked = true };

        Action action = () => cell.Annotations.Clear();

        action.Should().Throw<InvalidOperationException>();
    }
}
