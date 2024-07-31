using System.Diagnostics;
using System.Reflection;

namespace SudokuBattle.App;

public static class Metadata
{
    public static string? Version { get; private set; }

    static Metadata()
    {
        Assembly assembly = typeof(Metadata).Assembly;
        Version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
    }
}
