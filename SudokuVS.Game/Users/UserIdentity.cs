namespace SudokuVS.Game.Users;

public class UserIdentity
{
    public required Guid ExternalId { get; init; }
    public required string Name { get; init; }
}
