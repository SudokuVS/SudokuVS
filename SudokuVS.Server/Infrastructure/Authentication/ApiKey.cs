namespace SudokuVS.Server.Infrastructure.Authentication;

public class ApiKey
{
    public ApiKey(Guid id, DateTime creationDate, string? name, string token)
    {
        Id = id;
        CreationDate = creationDate;
        Name = name;
        Token = token;
    }

    public Guid Id { get; private set; }
    public DateTime CreationDate { get; set; }
    public string? Name { get; private set; }
    public string Token { get; private set; }
}
