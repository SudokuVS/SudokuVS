using System.Text;

namespace SudokuVS.Game.Exceptions;

public class InvalidArgumentsException : DomainException
{
    public InvalidArgumentsException(string argumentName, object argumentValue, string? message = null) : base(GetMessage(argumentName, argumentValue, message))
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

public class InvalidKeyException<TObject> : DomainException
{
    public InvalidKeyException(object? key = null, string? message = null) : base(GetMessage(key, message))
    {
    }

    static string GetMessage(object? key, string? message)
    {
        StringBuilder builder = new();

        builder.AppendLine(string.IsNullOrWhiteSpace(message) ? $"Could not find object of type {typeof(TObject).FullName}." : message);

        if (key != null)
        {
            builder.AppendLine($"Key: {key}");
        }

        return builder.ToString();
    }
}
