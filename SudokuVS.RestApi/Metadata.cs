using System.Diagnostics;
using System.Reflection;
using Semver;

namespace SudokuVS.RestApi;

static class Metadata
{
    public static SemVersion? Version { get; private set; }

    static Metadata()
    {
        Assembly assembly = typeof(Metadata).Assembly;
        string? versionStr = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
        Version = versionStr != null ? SemVersion.Parse(versionStr, SemVersionStyles.Any) : null;
    }
}
