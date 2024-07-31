namespace SudokuBattle.Game;

public enum PlayerSide
{
    Player1,
    Player2
}

public static class PlayerSideExtensions
{
    public static PlayerSide Other(this PlayerSide side) =>
        side switch { PlayerSide.Player1 => PlayerSide.Player2, PlayerSide.Player2 => PlayerSide.Player1, _ => throw new ArgumentOutOfRangeException(nameof(side), side, null) };

    public static string Format(this PlayerSide side) => side switch { PlayerSide.Player1 => "Player 1", PlayerSide.Player2 => "Player 2", _ => "Player ?" };
}
