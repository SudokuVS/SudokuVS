﻿using System.ComponentModel.DataAnnotations;
using SudokuVS.Game.Users;

namespace SudokuVS.WebApi.Models;

/// <summary>
///     Identity of a user.
/// </summary>
public class UserIdentityDto
{
    /// <summary>
    ///     The unique identifier of the user.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the user.
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class UserIdentityMappingExtensions
{
    public static UserIdentityDto ToDto(this UserIdentity user) => new() { Id = user.ExternalId, Name = user.Name };
}
