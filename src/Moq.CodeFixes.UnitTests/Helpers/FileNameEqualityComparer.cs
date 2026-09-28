using System.Collections.Generic;
using System.IO;

public class FileNameEqualityComparer : IEqualityComparer<string>
{
    public static IEqualityComparer<string> Default { get; } = new FileNameEqualityComparer();

    FileNameEqualityComparer() { }

    public bool Equals(string? x, string? y) => Path.GetFileName(x)?.Equals(Path.GetFileName(y)) == true;

    public int GetHashCode(string obj) => Path.GetFileName(obj).GetHashCode();
}