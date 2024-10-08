﻿using System.Diagnostics.CodeAnalysis;
using SudokuVS.Sudoku.Models;
using SudokuVS.Sudoku.Models.Abstractions;
using SudokuVS.Sudoku.Utils;

namespace SudokuVS.Game;

public class PlayerState : IHiddenPlayerState
{
    readonly HashSet<(int Row, int Column)> _hints = [];

    public PlayerState(SudokuGame game, SudokuGrid grid, PlayerSide side, string username)
    {
        Game = game;
        Grid = grid;
        Side = side;
        Username = username;
    }

    public SudokuGame Game { get; }
    public SudokuGrid Grid { get; }
    IHiddenSudokuGrid IHiddenPlayerState.Grid => Grid;
    public PlayerSide Side { get; }
    public string Username { get; }
    public IReadOnlyCollection<(int Row, int Column)> Hints => _hints;
    public int RemainingHints => Math.Max(0, Game.Options.MaxHints - Hints.Count);

    public event EventHandler? HintAdded;

    public void SetElement(int row, int column, int element) => Grid[row, column].Element = element;
    public void ClearElement(int row, int column) => Grid[row, column].Element = null;

    public void AddAnnotation(int row, int column, int element) => Grid[row, column].Annotations.Add(element);
    public void RemoveAnnotation(int row, int column, int element) => Grid[row, column].Annotations.Remove(element);
    public void ClearAnnotations(int row, int column) => Grid[row, column].Annotations.Clear();

    public bool TryUseHint(int row, int column, [NotNullWhen(false)] out string? whyNot)
    {
        if (_hints.Contains((row, column)))
        {
            whyNot = "Cell is already a hint";
            return false;
        }

        SudokuCell cell = Grid[row, column];
        if (cell.IsLocked)
        {
            whyNot = "Cell is locked";
            return false;
        }

        _hints.Add((row, column));
        cell.Element = Game.SolvedGrid[row, column].Element;
        cell.IsLocked = true;

        HintAdded?.Invoke(this, EventArgs.Empty);

        whyNot = null;
        return true;
    }

    public void Restore(IEnumerable<(int, int)> hints)
    {
        _hints.Clear();
        foreach ((int, int) hint in hints)
        {
            _hints.Add(hint);
        }
    }
}

public static class PlayerStateExtensions
{
    public static void SetElement(this PlayerState state, int index, int element)
    {
        (int row, int column) = SudokuGridCoordinates.ComputeCoordinates(index);
        state.SetElement(row, column, element);
    }

    public static void ClearElement(this PlayerState state, int index)
    {
        (int row, int column) = SudokuGridCoordinates.ComputeCoordinates(index);
        state.ClearElement(row, column);
    }

    public static void AddAnnotation(this PlayerState state, int index, int annotation)
    {
        (int row, int column) = SudokuGridCoordinates.ComputeCoordinates(index);
        state.AddAnnotation(row, column, annotation);
    }

    public static void RemoveAnnotation(this PlayerState state, int index, int annotation)
    {
        (int row, int column) = SudokuGridCoordinates.ComputeCoordinates(index);
        state.RemoveAnnotation(row, column, annotation);
    }

    public static void ClearAnnotations(this PlayerState state, int index)
    {
        (int row, int column) = SudokuGridCoordinates.ComputeCoordinates(index);
        state.ClearAnnotations(row, column);
    }

    public static bool TryUseHint(this PlayerState state, int index, [NotNullWhen(false)] out string? whyNot)
    {
        (int row, int column) = SudokuGridCoordinates.ComputeCoordinates(index);
        return state.TryUseHint(row, column, out whyNot);
    }

    public static void ToggleAnnotation(this PlayerState state, int row, int column, int element)
    {
        SudokuCell cell = state.Grid[row, column];
        if (!cell.Annotations.Remove(element))
        {
            cell.Annotations.Add(element);
        }
    }

    public static void ToggleAnnotation(this PlayerState state, int index, int element)
    {
        (int row, int column) = SudokuGridCoordinates.ComputeCoordinates(index);
        state.ToggleAnnotation(row, column, element);
    }
}
