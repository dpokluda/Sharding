// -------------------------------------------------------------------------
// <copyright file="ConsistentHashSharding.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

using System.Text;
using Sharding.HashAlgorithms;

namespace Sharding;

public class ConsistentHashSharding<T> : ISharding<T> where T : class
{
    private const int DefaultNumberOfReplicas = 100;
    private static HashFunction _hash = new Murmur2();
    
    protected readonly SortedDictionary<int, T> Circle;
    private int _numberOfReplicas;
    //cache the ordered keys for better performance
    private int[] _keys;

    public ConsistentHashSharding(IEnumerable<T> nodes, int numberOfReplicas)
    {
        Circle = new SortedDictionary<int, T>();
        _numberOfReplicas = numberOfReplicas;
        foreach (T node in nodes)
        {
            AddNodeToCircle(node);
        }

        _keys = Circle.Keys.ToArray();
    }

    public ConsistentHashSharding(IEnumerable<T> nodes)
        : this(nodes, DefaultNumberOfReplicas)
    { }

    public ConsistentHashSharding(int numberOfReplicas)
    {
        Circle = new SortedDictionary<int, T>();
        _numberOfReplicas = numberOfReplicas;
        _keys = Array.Empty<int>();
    }

    public ConsistentHashSharding()
        : this(DefaultNumberOfReplicas)
    { }

    public void Add(T node)
    {
        AddNodeToCircle(node);
        _keys = Circle.Keys.ToArray();
    }

    private void AddNodeToCircle(T node)
    {
        for (int i = 0; i < _numberOfReplicas; i++)
        {
            int hash = _hash.ComputeHash(ToBytes(node.GetHashCode().ToString() + i));
            Circle[hash] = node;
        }
    }

    public void Remove(T node)
    {
        for (int i = 0; i < _numberOfReplicas; i++)
        {
            int hash = _hash.ComputeHash(ToBytes(node.GetHashCode().ToString() + i));
            Circle.Remove(hash);
        }
        
        _keys = Circle.Keys.ToArray();
    }

    public T GetNode(string key)
    {
        int hash = _hash.ComputeHash(ToBytes(key));
        int first = FindFirstNode(_keys, hash);
        if (_keys.Length > 0 && first >= _keys.Length)
        {
            first %= _keys.Length;
        }

        return Circle[_keys[first]];
    }

    private int FindFirstNode(int[] ay, int val)
    {
        int result = Array.BinarySearch(ay, 0, ay.Length, val);
        if (result < 0)
        {
            // not found; let's return the first value greater than our value
            return ~result;
        }

        return result;
    }
    
    protected virtual byte[] ToBytes(string element)
    {
        return Encoding.UTF8.GetBytes(element!);
    }
}