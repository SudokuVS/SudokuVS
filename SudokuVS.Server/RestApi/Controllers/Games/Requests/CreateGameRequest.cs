namespace SudokuVS.Server.RestApi.Controllers.Games.Requests;

/// <summary>
///     Create a new game.
/// </summary>
public class CreateGameRequest
{
    /// <summary>
    ///     The name of the game.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The number of hints that can be used by each player during the round.
    /// </summary>
    public int Hints { get; init; }
}
