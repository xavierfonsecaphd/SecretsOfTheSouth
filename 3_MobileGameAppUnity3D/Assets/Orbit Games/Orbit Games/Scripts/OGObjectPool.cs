using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class OGObjectPool<T>
{
    public const int FILL_RATE = 64;
    private static Stack<T> pool = new Stack<T>();
    private static Action<int> filler = null;

    public static void SetPoolFiller(Action<int> createBunchOfObjects)
    {
        filler = createBunchOfObjects;
    }

    private static T Create()
    {
        if (filler != null)
        {
            filler(FILL_RATE);
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
        }
        return Activator.CreateInstance<T>();
    }

    public static T Get()
    {
        lock (pool)
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
        }
        return Create();
    }

    public static void Put(ref T item)
    {
        Put(item);
        item = default(T);
    }

    public static void Put(T item)
    {
        lock (pool)
        {
            if (item == null) return;
            pool.Push(item);
        }
    }

    public static void Put(ICollection<T> items, Action<T> clearDelegate)
    {
        lock (pool)
        {
            foreach (var item in items)
            {
                if (item == null) continue;
                clearDelegate(item);
                pool.Push(item);
            }
            items.Clear();
        }
    }
}