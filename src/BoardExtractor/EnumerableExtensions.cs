public static class EnumerableExtensions
{
    public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> elements, Func<T, bool> predicate)
    {
        foreach (var element in elements)
        {
            yield return element;

            if (!predicate(element))
            {
                yield break;
            }
        }
    }
}