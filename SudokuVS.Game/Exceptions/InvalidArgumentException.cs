using System.Text;

namespace SudokuVS.Game.Exceptions;

public class InvalidArgumentException : DomainException
{
    public InvalidArgumentException(string argumentName, object argumentValue, string? message = null) : base(GetMessage(argumentName, argumentValue, message))
    {
    }

    static string GetMessage(string argumentName, object argumentValue, string? message)
    {
        StringBuilder builder = new();

        builder.AppendLine(string.IsNullOrWhiteSpace(message) ? "The provided argument value is invalid." : message);
        builder.AppendLine($"Argument name: {argumentName}");
        builder.AppendLine($"Actual value: {argumentValue}");

        return builder.ToString();
    }
}
