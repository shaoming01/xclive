namespace Frame.Ext;

public static class ListExt
{
    public static T RadomOne<T>(this List<T> list)
    {
        if (!list.Has<T>())
            return default(T);
        int index = new Random().Next(0, list.Count);
        return list[index];
    }

    /// <summary>合并集合</summary>
    public static List<T> MergeList<T>(this List<T> list, params List<T>[] listArray)
    {
        listArray = listArray ?? new List<T>[0];
        List<T> objList = new List<T>(list.Count +
                                      ((IEnumerable<List<T>>)listArray).Sum<List<T>>(
                                          (Func<List<T>, int>)(x => x.Count)));
        objList.AddRange((IEnumerable<T>)list);
        listArray.ForEach<List<T>>(new Action<List<T>>(objList.AddRange));
        return objList;
    }

    /// <summary>合并集合</summary>
    public static List<T> MergeList<T>(this T[] list, params T[][] listArray)
    {
        listArray = listArray ?? new T[0][];
        List<T> objList =
            new List<T>(list.Length + ((IEnumerable<T[]>)listArray).Sum<T[]>((Func<T[], int>)(x => x.Length)));
        objList.AddRange((IEnumerable<T>)list);
        listArray.ForEach<T[]>(new Action<T[]>(objList.AddRange));
        return objList;
    }

    /// <summary>合并集合</summary>
    public static List<T> MergeList<T>(this IEnumerable<T> list, params IEnumerable<T>[] listArray)
    {
        List<T> objList = new List<T>(list);
        if (((IEnumerable<IEnumerable<T>>)listArray).Has<IEnumerable<T>>())
            listArray.ForEach<IEnumerable<T>>(new Action<IEnumerable<T>>(objList.AddRange));
        return objList;
    }
}