namespace SudokuVS.Game;

public static class SudokuGridCoordinates
{
    public static int ComputeFlatIndex(int row, int column) => row * 9 + column;
    public static (int Row, int Column) ComputeCoordinates(int index) => (index / 9, index % 9);
}
