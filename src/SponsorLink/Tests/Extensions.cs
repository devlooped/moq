using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Devlooped.Sponsors;

static class Extensions
{
    public static HashCode Add(this HashCode hash, params object[] items)
    {
        foreach (var item in items)
            hash.Add(item);

        return hash;
    }


    public static HashCode AddRange<T>(this HashCode hash, IEnumerable<T> items)
    {
        foreach (var item in items)
            hash.Add(item);

        return hash;
    }

    public static Array Cast(this Array array, Type elementType)
    {
        //Convert the object list to the destination array type.
        var result = Array.CreateInstance(elementType, array.Length);
        Array.Copy(array, result, array.Length);
        return result;
    }

    public static void Assert(this ILogger logger, [DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression(nameof(condition))] string? message = default, params object?[] args)
    {
        if (!condition)
        {
            //Debug.Assert(condition, message);
            logger.LogError(message, args);
            throw new InvalidOperationException(message);
        }
    }
}
