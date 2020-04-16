using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGListPool<T> 
{
    private static Stack<List<T>> pool = new Stack<List<T>>();

    public static List<T> Get()
    {
        lock (pool)
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
        }
        return new List<T>();
    }

    public static List<T> Get(IEnumerable<T> copyFrom)
    {
        lock (pool)
        {
            if (pool.Count > 0)
            {
                var obtained = pool.Pop();
                obtained.AddRange(copyFrom);
                return obtained;
            }
        }
        return new List<T>(copyFrom);
    }
     
    public static void ReplaceCopy(ref List<T> variable, IEnumerable<T> copyFrom)
    {
        Put(ref variable);
        variable = Get(copyFrom);
    }

    public static void Replace(ref List<T> variable)
    {
        Put(ref variable);
        variable = Get();
    }

    public static void Put(ref List<T> list)
    {
        Put(list);
        list = null;
    }

    public static void Put(List<List<T>> listOfLists)
    {
        foreach (var list in listOfLists)
            Put(list);
        listOfLists.Clear();
    }

    public static void Put(List<T> list)
    {
        lock (pool)
        {
            if (list == null) return;
            list.Clear();
            pool.Push(list);
        }
    }
}
