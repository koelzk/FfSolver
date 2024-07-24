namespace BoardExtractor;

public static class EnumerableExtensions
{
    /// <summary>
    /// Returns elements from a sequence until a specified condition is no longer true.
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    /// <param name="source">A sequence to return elements from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>An System.Collections.Generic.IEnumerable`1 that contains the elements from the input sequence that occur before the element at which the test no longer passes.</returns>
    /// <exception cref="ArgumentNullException">source or predicate is null</exception>
    public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (var element in source)
        {
            yield return element;

            if (!predicate(element))
            {
                yield break;
            }
        }
    }
}
