using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SudokuVS.Game;

namespace SudokuVS.App.Services;

public class GameTokenService
{
    public const string GameIdClaim = "game-id";
    public const string PlayerSideClaim = "player-side";

    readonly JwtSecurityTokenHandler _tokenHandler;
    readonly SigningCredentials _credentials;

    public GameTokenService(byte[] secretKey)
    {
        _credentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha512);
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public string Generate(Guid id, PlayerSide side)
    {
        Claim[] claims =
        [
            new Claim(GameIdClaim, id.ToString()),
            new Claim(PlayerSideClaim, SerializePlayerSide(side))
        ];

        JwtSecurityToken token = new(claims: claims, signingCredentials: _credentials);
        return _tokenHandler.WriteToken(token);
    }

    public bool Validate(string tokenString, out Guid gameId, out PlayerSide playerSide)
    {
        gameId = default;
        playerSide = default;

        if (!_tokenHandler.CanReadToken(tokenString))
        {
            return false;
        }

        JwtSecurityToken token = _tokenHandler.ReadJwtToken(tokenString);

        string gameIdStr = token.Claims.FirstOrDefault(c => c.Type == GameIdClaim)?.Value ?? "";
        string playerSideStr = token.Claims.FirstOrDefault(c => c.Type == PlayerSideClaim)?.Value ?? "";

        return Guid.TryParse(gameIdStr, out gameId) && TryParsePlayerSide(playerSideStr, out playerSide);
    }

    static string SerializePlayerSide(PlayerSide side) =>
        side switch
        {
            PlayerSide.Player1 => "player1",
            PlayerSide.Player2 => "player2",
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };

    static bool TryParsePlayerSide(string playerSide, out PlayerSide side)
    {
        switch (playerSide)
        {
            case "player1":
                side = PlayerSide.Player1;
                return true;
            case "player2":
                side = PlayerSide.Player2;
                return true;
            default:
                side = default;
                return false;
        }
    }
}
