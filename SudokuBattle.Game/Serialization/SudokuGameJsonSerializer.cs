﻿using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuBattle.Sudoku.Models;
using SudokuBattle.Sudoku.Serialization;

namespace SudokuBattle.Game.Serialization;

public class SudokuGameJsonSerializer
{
    readonly SudokuGridEnumerableSerializer _gridSerializer = new();
    readonly JsonSerializerOptions _serializerOptions;

    public SudokuGameJsonSerializer(bool indented = false)
    {
        _serializerOptions = new JsonSerializerOptions
            { Converters = { new JsonStringEnumConverter() }, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = indented };
    }

    public string Serialize(SudokuGame game)
    {
        SerializedSudokuGame serialized = new()
        {
            Id = game.Id,
            Name = game.Name,
            Options = game.Options,
            InitialGrid = _gridSerializer.ToEnumerable(game.InitialGrid).ToArray(),
            SolvedGrid = _gridSerializer.ToEnumerable(game.SolvedGrid).ToArray(),
            Player1 = game.Player1 == null ? null : Serialize(game.Player1),
            Player2 = game.Player2 == null ? null : Serialize(game.Player2),
            StartDate = game.StartDate,
            EndDate = game.EndDate,
            Winner = game.Winner
        };

        return JsonSerializer.Serialize(serialized, _serializerOptions);
    }

    public SudokuGame? Deserialize(string serialized)
    {
        SerializedSudokuGame? deserialized = JsonSerializer.Deserialize<SerializedSudokuGame>(serialized, _serializerOptions);
        if (deserialized == null)
        {
            return null;
        }

        SudokuGrid initialGrid = _gridSerializer.FromEnumerable(deserialized.InitialGrid);
        initialGrid.LockNonEmptyCells();

        SudokuGrid solvedGrid = _gridSerializer.FromEnumerable(deserialized.SolvedGrid);
        solvedGrid.LockNonEmptyCells();

        SudokuGame game = SudokuGame.Load(deserialized.Id, deserialized.Name, initialGrid, solvedGrid, deserialized.Options);

        PlayerState? player1 = deserialized.Player1 == null ? null : Deserialize(game, PlayerSide.Player1, deserialized.Player1);
        PlayerState? player2 = deserialized.Player2 == null ? null : Deserialize(game, PlayerSide.Player2, deserialized.Player2);

        game.Restore(player1, player2, deserialized.StartDate, deserialized.EndDate, deserialized.Winner);

        return game;
    }

    static SerializedPlayerState Serialize(PlayerState player) =>
        new()
        {
            PlayerName = player.PlayerName,
            Grid = Serialize(player.Grid),
            Hints = player.Hints.Select(x => ComputeFlatIndex(x.Row, x.Column)).ToArray()
        };

    static Dictionary<int, SerializedCell> Serialize(SudokuGrid grid) =>
        grid.Enumerate()
            .Where(x => x.Cell.Element.HasValue || x.Cell.Annotations.Count > 0 || x.Cell.Locked)
            .ToDictionary(
                x => ComputeFlatIndex(x.Row, x.Column),
                x => new SerializedCell
                {
                    Elements = x.Cell.Element,
                    Annotations = x.Cell.Annotations.Count == 0 ? null : x.Cell.Annotations.ToArray(),
                    Locked = x.Cell.Locked ? true : null
                }
            );

    static PlayerState Deserialize(SudokuGame game, PlayerSide side, SerializedPlayerState player)
    {
        SudokuGrid grid = Deserialize(player.Grid);
        PlayerState state = new(game, grid, side, player.PlayerName);

        IEnumerable<(int Row, int Column)> hints = player.Hints.Select(ComputeCoordinates);
        state.Restore(hints);

        return state;
    }

    static SudokuGrid Deserialize(Dictionary<int, SerializedCell> grid)
    {
        SudokuCell[,] cells = new SudokuCell[9, 9];
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
            cells[i, j] = new SudokuCell(i, j);

            int flatIndex = ComputeFlatIndex(i, j);
            if (!grid.TryGetValue(flatIndex, out SerializedCell? cell))
            {
                continue;
            }

            cells[i, j].Element = cell.Elements;

            if (cell.Annotations != null)
            {
                foreach (int annotation in cell.Annotations)
                {
                    cells[i, j].Annotations.Add(annotation);
                }
            }

            cells[i, j].Locked = cell.Locked ?? false;
        }

        return new SudokuGrid(cells);
    }

    static int ComputeFlatIndex(int row, int column) => row * 9 + column;
    static (int Row, int Column) ComputeCoordinates(int index) => (index / 9, index % 9);

    class SerializedSudokuGame
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required SudokuGameOptions Options { get; init; }
        public required int[] InitialGrid { get; init; }
        public required int[] SolvedGrid { get; init; }
        public SerializedPlayerState? Player1 { get; init; }
        public SerializedPlayerState? Player2 { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public PlayerSide? Winner { get; init; }
    }

    class SerializedPlayerState
    {
        public required string PlayerName { get; init; }
        public required Dictionary<int, SerializedCell> Grid { get; init; }
        public required int[] Hints { get; init; }
    }

    class SerializedCell
    {
        public int? Elements { get; init; }
        public int[]? Annotations { get; init; }
        public bool? Locked { get; init; }
    }
}
