using Xunit.Abstractions;
using Xunit.Sdk;

namespace Neuroglia.UnitTests.Services;

internal class PriorityTestCaseOrderer
    : ITestCaseOrderer
{

    /// <inheritdoc/>
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        int priority;
        var sortedCases = new SortedDictionary<int, List<TTestCase>>();
        foreach (TTestCase testCase in testCases)
        {
            priority = 0;
            foreach (IAttributeInfo attr in testCase.TestMethod.Method.GetCustomAttributes((typeof(PriorityAttribute).AssemblyQualifiedName))) priority = attr.GetNamedArgument<int>("Priority");
            this.GetOrCreate(sortedCases, priority).Add(testCase);
        }
        foreach (var list in sortedCases.Keys.Select(p => sortedCases[p]))
        {
            list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
            foreach (TTestCase testCase in list) yield return testCase;
        }
    }

    private TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
    {
        if (dictionary.TryGetValue(key, out var result)) return result;
        result = new TValue();
        dictionary[key] = result;
        return result;
    }

}