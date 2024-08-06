using System.Text;

namespace SudokuVS.Game.Exceptions;

public class InvalidKeyException : DomainException
{
    public InvalidKeyException(Type type, object? key = null, string? message = null) : base(GetMessage(type, key, message))
    {
    }

    static string GetMessage(Type type, object? key, string? message)
    {
        StringBuilder builder = new();

        builder.AppendLine(string.IsNullOrWhiteSpace(message) ? $"Could not find object of type {type.FullName}." : message);

        if (key != null)
        {
            builder.AppendLine($"Key: {key}");
        }

        return builder.ToString();
    }
}

public class InvalidKeyException<TObject> : InvalidKeyException
{
    public InvalidKeyException(object? key = null, string? message = null) : base(typeof(TObject), key, message)
    {
    }
}
