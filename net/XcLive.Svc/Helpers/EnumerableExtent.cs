#nullable disable
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SchemaBuilder.Svc.Helpers;

public static class EnumerableExtent
{
    public static bool Has<TSource>([NotNullWhen(true)] this IEnumerable<TSource> source)
    {
        return source != null && source.Any();
    }

    public static bool Has<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        return source != null && source.Any<TSource>(predicate);
    }

    public static void ForEach<T>(this IEnumerable<T> query, Action<T> method)
    {
        foreach (T obj in query)
            method(obj);
    }

    public static void ForEach<T>(this T[] arr, Action<T> method)
    {
        foreach (T obj in arr)
            method(obj);
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        return source.Where<TSource>((Func<TSource, bool>) (element => seenKeys.Add(keySelector(element))));
    }

    public static List<List<T>> Split<T>(this IEnumerable<T> query, int pageSize)
    {
        List<List<T>> objListList = new List<List<T>>();
        if (query == null)
            return objListList;
        List<T> list = query.ToList<T>();
        int num = list.Count / pageSize + (list.Count % pageSize == 0 ? 0 : 1);
        for (int index = 0; index < num; ++index)
        {
            int count = pageSize;
            if (list.Count < (index + 1) * pageSize)
                count = list.Count - index * pageSize;
            objListList.Add(list.GetRange(index * pageSize, count));
        }
        return objListList;
    }

    public static bool ContainsIgnoreCase(this IEnumerable<string> source, string value)
    {
        return source.Contains<string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public static bool Any(this IEnumerable source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof (source));
        using (EnumerableExtent.DisposableEnumerator disposableEnumerator = source.GetDisposableEnumerator())
        {
            if (disposableEnumerator.MoveNext())
                return true;
        }
        return false;
    }

    public static object First(this IEnumerable source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof (source));
        if (source is IList list)
        {
            if (list.Count > 0)
                return list[0];
        }
        else
        {
            using (EnumerableExtent.DisposableEnumerator disposableEnumerator = source.GetDisposableEnumerator())
            {
                if (disposableEnumerator.MoveNext())
                    return disposableEnumerator.Current;
            }
        }
        throw new InvalidOperationException("Sequence contains no elements");
    }

    public static object FirstOrNull(this IEnumerable source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof (source));
        if (source is IList list)
        {
            if (list.Count > 0)
                return list[0];
        }
        else
        {
            using (EnumerableExtent.DisposableEnumerator disposableEnumerator = source.GetDisposableEnumerator())
            {
                if (disposableEnumerator.MoveNext())
                    return disposableEnumerator.Current;
            }
        }
        return (object) null;
    }

    private static EnumerableExtent.DisposableEnumerator GetDisposableEnumerator(
        this IEnumerable source)
    {
        return new EnumerableExtent.DisposableEnumerator(source);
    }

    internal class DisposableEnumerator : IDisposable, IEnumerator
    {
        private readonly IEnumerator _wrapped;

        public DisposableEnumerator(IEnumerable source) => this._wrapped = source.GetEnumerator();

        public void Dispose()
        {
            if (!(this._wrapped is IDisposable wrapped))
                return;
            wrapped.Dispose();
        }

        public bool MoveNext() => this._wrapped.MoveNext();

        public void Reset() => this._wrapped.Reset();

        public object Current => this._wrapped.Current;
    }
}