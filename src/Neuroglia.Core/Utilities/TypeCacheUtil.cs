using System.Collections.Concurrent;
using System.Reflection;

namespace Neuroglia;

/// <summary>
/// Acts as an helper to find a filter types
/// </summary>
public static class TypeCacheUtil
{

    static readonly ConcurrentDictionary<string, IEnumerable<Type>> Cache = new();

    /// <summary>
    /// Find types filtered by a given predicate
    /// </summary>
    /// <param name="cacheKey">The cache key used to store the results</param>
    /// <param name="predicate">The predicate that filters the types</param>
    /// <param name="assemblies">An array containing the assemblies to scan</param>
    /// <returns>The filtered types</returns>
    public static IEnumerable<Type> FindFilteredTypes(string cacheKey, Func<Type, bool> predicate, params Assembly[] assemblies)
    {
        if (Cache.TryGetValue(cacheKey, out var matches)) return matches;
        matches = assemblies.SelectMany(a =>
        {
            try
            {
                return a.DefinedTypes;
            }
            catch
            {
                return Array.Empty<Type>().AsEnumerable();
            }
        }).Where(predicate).ToList();
        Cache.TryAdd(cacheKey, matches);
        return matches;
    }

    /// <summary>
    /// Find types filtered by a given predicate
    /// </summary>
    /// <param name="cacheKey">The cache key used to store the results</param>
    /// <param name="predicate">The predicate that filters the types</param>
    /// <returns>The filtered types</returns>
    public static IEnumerable<Type> FindFilteredTypes(string cacheKey, Func<Type, bool> predicate) => FindFilteredTypes(cacheKey, predicate, AssemblyLocator.GetAssemblies().ToArray());

}
