namespace SudokuVS.Game.Exceptions;

public class GameNotFound : ObjectNotFound<SudokuGame>
{
    public GameNotFound(Guid gameId) : base(gameId)
    {
    }
}
