﻿using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;

namespace SudokuVS.Server.RestApi.Models;

/// <summary>
///     State of a player.
/// </summary>
public class SudokuGamePlayerStateDto
{
    /// <summary>
    ///     The user corresponding to the player.
    /// </summary>
    [Required]
    public required UserIdentityDto User { get; set; }

    /// <summary>
    ///     The grid as seen by the player.
    /// </summary>
    [Required]
    public required SudokuPlayerGridDto Grid { get; set; }
}

static class SudokuGamePlayerStateMappingExtensions
{
    public static SudokuGamePlayerStateDto ToPlayerStateDto(this PlayerState state, UserIdentityDto user) =>
        new()
        {
            User = user,
            Grid = state.Grid.ToPlayerGridDto(state)
        };
}
