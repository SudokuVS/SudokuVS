﻿using System.ComponentModel.DataAnnotations;
using SudokuVS.Game;

namespace SudokuVS.Server.RestApi.Models;

/// <summary>
///     State of the opponent of a player.
/// </summary>
public class SudokuGameHiddenPlayerStateDto
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
    public required SudokuHiddenPlayerGridDto Grid { get; set; }
}

static class SudokuGameHiddenPlayerStateMappingExtensions
{
    public static SudokuGameHiddenPlayerStateDto ToHiddenPlayerStateDto(this IHiddenPlayerState state, UserIdentityDto user) =>
        new()
        {
            User = user,
            Grid = state.Grid.ToHiddenPlayerGridDto(state)
        };
}
