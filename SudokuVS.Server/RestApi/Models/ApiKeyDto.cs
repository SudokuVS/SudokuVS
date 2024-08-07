using SudokuVS.Server.Infrastructure.Authentication;
using SudokuVS.Server.Infrastructure.Authentication.ApiKey;

namespace SudokuVS.Server.RestApi.Models;

public class ApiKeyDto
{
    /// <summary>
    ///     The unique ID of the key.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    ///     The creation date of the key.
    /// </summary>
    public required DateTime CreationDate { get; set; }

    /// <summary>
    ///     The name of the key.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The token corresponding to the key. This is the value that is expected in the Authorization header.
    /// </summary>
    public required string Token { get; set; }
}

public static class ApiKeyMappingExtensions
{
    public static ApiKeyDto ToDto(this ApiKey apiKey) => new() { Id = apiKey.Id, CreationDate = apiKey.CreationDate, Name = apiKey.Name, Token = apiKey.Token };
}
