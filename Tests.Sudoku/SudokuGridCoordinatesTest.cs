﻿using FluentAssertions;
using SudokuVS.Sudoku.Utils;

namespace Tests.Sudoku;

[TestClass]
public class SudokuGridCoordinatesTest
{
    [DataTestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, 1)]
    [DataRow(0, 2, 2)]
    [DataRow(0, 3, 3)]
    [DataRow(0, 4, 4)]
    [DataRow(0, 5, 5)]
    [DataRow(0, 6, 6)]
    [DataRow(0, 7, 7)]
    [DataRow(0, 8, 8)]
    [DataRow(1, 0, 9)]
    [DataRow(1, 1, 10)]
    [DataRow(1, 2, 11)]
    [DataRow(1, 3, 12)]
    [DataRow(1, 4, 13)]
    [DataRow(1, 5, 14)]
    [DataRow(1, 6, 15)]
    [DataRow(1, 7, 16)]
    [DataRow(1, 8, 17)]
    [DataRow(2, 0, 18)]
    [DataRow(2, 1, 19)]
    [DataRow(2, 2, 20)]
    [DataRow(2, 3, 21)]
    [DataRow(2, 4, 22)]
    [DataRow(2, 5, 23)]
    [DataRow(2, 6, 24)]
    [DataRow(2, 7, 25)]
    [DataRow(2, 8, 26)]
    [DataRow(3, 0, 27)]
    [DataRow(3, 1, 28)]
    [DataRow(3, 2, 29)]
    [DataRow(3, 3, 30)]
    [DataRow(3, 4, 31)]
    [DataRow(3, 5, 32)]
    [DataRow(3, 6, 33)]
    [DataRow(3, 7, 34)]
    [DataRow(3, 8, 35)]
    [DataRow(4, 0, 36)]
    [DataRow(4, 1, 37)]
    [DataRow(4, 2, 38)]
    [DataRow(4, 3, 39)]
    [DataRow(4, 4, 40)]
    [DataRow(4, 5, 41)]
    [DataRow(4, 6, 42)]
    [DataRow(4, 7, 43)]
    [DataRow(4, 8, 44)]
    [DataRow(5, 0, 45)]
    [DataRow(5, 1, 46)]
    [DataRow(5, 2, 47)]
    [DataRow(5, 3, 48)]
    [DataRow(5, 4, 49)]
    [DataRow(5, 5, 50)]
    [DataRow(5, 6, 51)]
    [DataRow(5, 7, 52)]
    [DataRow(5, 8, 53)]
    [DataRow(6, 0, 54)]
    [DataRow(6, 1, 55)]
    [DataRow(6, 2, 56)]
    [DataRow(6, 3, 57)]
    [DataRow(6, 4, 58)]
    [DataRow(6, 5, 59)]
    [DataRow(6, 6, 60)]
    [DataRow(6, 7, 61)]
    [DataRow(6, 8, 62)]
    [DataRow(7, 0, 63)]
    [DataRow(7, 1, 64)]
    [DataRow(7, 2, 65)]
    [DataRow(7, 3, 66)]
    [DataRow(7, 4, 67)]
    [DataRow(7, 5, 68)]
    [DataRow(7, 6, 69)]
    [DataRow(7, 7, 70)]
    [DataRow(7, 8, 71)]
    [DataRow(8, 0, 72)]
    [DataRow(8, 1, 73)]
    [DataRow(8, 2, 74)]
    [DataRow(8, 3, 75)]
    [DataRow(8, 4, 76)]
    [DataRow(8, 5, 77)]
    [DataRow(8, 6, 78)]
    [DataRow(8, 7, 79)]
    [DataRow(8, 8, 80)]
    public void ShouldComputeFlatIndex(int row, int column, int expectedIndex)
    {
        int result = SudokuGridCoordinates.ComputeFlatIndex(row, column);

        result.Should().Be(expectedIndex);
    }

    [DataTestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(1, 0, 1)]
    [DataRow(2, 0, 2)]
    [DataRow(3, 0, 3)]
    [DataRow(4, 0, 4)]
    [DataRow(5, 0, 5)]
    [DataRow(6, 0, 6)]
    [DataRow(7, 0, 7)]
    [DataRow(8, 0, 8)]
    [DataRow(9, 1, 0)]
    [DataRow(10, 1, 1)]
    [DataRow(11, 1, 2)]
    [DataRow(12, 1, 3)]
    [DataRow(13, 1, 4)]
    [DataRow(14, 1, 5)]
    [DataRow(15, 1, 6)]
    [DataRow(16, 1, 7)]
    [DataRow(17, 1, 8)]
    [DataRow(18, 2, 0)]
    [DataRow(19, 2, 1)]
    [DataRow(20, 2, 2)]
    [DataRow(21, 2, 3)]
    [DataRow(22, 2, 4)]
    [DataRow(23, 2, 5)]
    [DataRow(24, 2, 6)]
    [DataRow(25, 2, 7)]
    [DataRow(26, 2, 8)]
    [DataRow(27, 3, 0)]
    [DataRow(28, 3, 1)]
    [DataRow(29, 3, 2)]
    [DataRow(30, 3, 3)]
    [DataRow(31, 3, 4)]
    [DataRow(32, 3, 5)]
    [DataRow(33, 3, 6)]
    [DataRow(34, 3, 7)]
    [DataRow(35, 3, 8)]
    [DataRow(36, 4, 0)]
    [DataRow(37, 4, 1)]
    [DataRow(38, 4, 2)]
    [DataRow(39, 4, 3)]
    [DataRow(40, 4, 4)]
    [DataRow(41, 4, 5)]
    [DataRow(42, 4, 6)]
    [DataRow(43, 4, 7)]
    [DataRow(44, 4, 8)]
    [DataRow(45, 5, 0)]
    [DataRow(46, 5, 1)]
    [DataRow(47, 5, 2)]
    [DataRow(48, 5, 3)]
    [DataRow(49, 5, 4)]
    [DataRow(50, 5, 5)]
    [DataRow(51, 5, 6)]
    [DataRow(52, 5, 7)]
    [DataRow(53, 5, 8)]
    [DataRow(54, 6, 0)]
    [DataRow(55, 6, 1)]
    [DataRow(56, 6, 2)]
    [DataRow(57, 6, 3)]
    [DataRow(58, 6, 4)]
    [DataRow(59, 6, 5)]
    [DataRow(60, 6, 6)]
    [DataRow(61, 6, 7)]
    [DataRow(62, 6, 8)]
    [DataRow(63, 7, 0)]
    [DataRow(64, 7, 1)]
    [DataRow(65, 7, 2)]
    [DataRow(66, 7, 3)]
    [DataRow(67, 7, 4)]
    [DataRow(68, 7, 5)]
    [DataRow(69, 7, 6)]
    [DataRow(70, 7, 7)]
    [DataRow(71, 7, 8)]
    [DataRow(72, 8, 0)]
    [DataRow(73, 8, 1)]
    [DataRow(74, 8, 2)]
    [DataRow(75, 8, 3)]
    [DataRow(76, 8, 4)]
    [DataRow(77, 8, 5)]
    [DataRow(78, 8, 6)]
    [DataRow(79, 8, 7)]
    [DataRow(80, 8, 8)]
    public void ShouldComputeCoordinates(int index, int expectedRow, int expectedColumn)
    {
        (int Row, int Column) result = SudokuGridCoordinates.ComputeCoordinates(index);

        result.Row.Should().Be(expectedRow);
        result.Column.Should().Be(expectedColumn);
    }
}
