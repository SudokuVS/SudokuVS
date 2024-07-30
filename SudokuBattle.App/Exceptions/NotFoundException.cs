namespace SudokuBattle.App.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class NotFoundException<T> : NotFoundException
{
    public NotFoundException(object identifier) : base($"Could not find {typeof(T).Name} {identifier}")
    {
    }
}
