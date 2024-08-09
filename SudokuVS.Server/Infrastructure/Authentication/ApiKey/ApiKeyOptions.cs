namespace SudokuVS.Server.Infrastructure.Authentication.ApiKey;

public class ApiKeyOptions
{
    public bool Enabled { get; set; }
    public string? Secret { get; set; }
}
