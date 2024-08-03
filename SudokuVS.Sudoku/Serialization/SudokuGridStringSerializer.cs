using System.Text;
using SudokuVS.Sudoku.Models;

namespace SudokuVS.Sudoku.Serialization;

public class SudokuGridStringSerializer
{
    /// <summary>
    ///     Max size of the string representation of a grid.
    ///     There are 81 cells, each one can be one char (the element) up to 13 chars:<br />
    ///     <br />
    ///     e.g. 0l|123456789;
    ///     <list>
    ///         <item>1st: any integer, the element of the cell</item>
    ///         <item>2nd: 'l' means that the cell is locked</item>
    ///         <item>3rd: '|' indicates that there are annotations</item>
    ///         <item>4th-12th: integers, the annotations of the cell, at most 9</item>
    ///         <item>13th: ';' cell separator</item>
    ///     </list>
    /// </summary>
    public const int SerializedStringMaxLength = 1053;

    readonly SudokuGridEnumerableSerializer _enumerableSerializer = new();

    public string ToString(SudokuGrid grid)
    {
        StringBuilder stringBuilder = new();

        foreach (SudokuCell cell in grid.Enumerate())
        {
            Write(stringBuilder, cell);
        }

        return stringBuilder.ToString();
    }

    public SudokuGrid FromString(string serialized)
    {
        using MemoryStream stream = new(Encoding.ASCII.GetBytes(serialized));
        using BinaryReader reader = new(stream, Encoding.ASCII);

        SudokuCell[,] cells = new SudokuCell[9, 9];

        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            cells[i, j] = Read(i, j, reader);
        }

        return new SudokuGrid(cells);
    }

    static void Write(StringBuilder builder, SudokuCell cell)
    {
        builder.Append(cell.Element ?? 0);

        if (cell.IsLocked)
        {
            builder.Append('l');
        }

        if (cell.HasAnnotations)
        {
            builder.Append('|');
            builder.AppendJoin("", cell.Annotations);
        }

        builder.Append(';');
    }

    static SudokuCell Read(int row, int col, BinaryReader reader)
    {
        char next = reader.ReadChar();
        if (IsInteger(next, out int element))
        {
            SudokuCell cell = new(row, col, element == 0 ? null : element);

            if (ReachedEnd(reader))
            {
                return cell;
            }

            bool locked = false;

            next = reader.ReadChar();

            if (next == 'l')
            {
                locked = true;
                next = reader.ReadChar();
            }

            if (next == '|')
            {
                next = reader.ReadChar();
                while (IsInteger(next, out int annotation))
                {
                    cell.Annotations.Add(annotation);
                    next = reader.ReadChar();
                }
            }

            if (next == ';')
            {
                cell.IsLocked = locked;
                return cell;
            }
        }

        throw new InvalidOperationException($"Unexpected character {next} at position {reader.BaseStream.Position - 1}");
    }

    static int ParseChar(char elementChar) => elementChar - '0';

    static bool IsInteger(char elementChar, out int element)
    {
        element = ParseChar(elementChar);
        return element is >= 0 and <= 9;
    }

    static bool ReachedEnd(BinaryReader reader) => reader.BaseStream.Position >= reader.BaseStream.Length;
}
