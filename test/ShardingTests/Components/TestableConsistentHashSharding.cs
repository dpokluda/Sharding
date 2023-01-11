// -------------------------------------------------------------------------
// <copyright file="TestableConsistentHashSharding.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

using Sharding;

namespace ShardingTests.Components;

public class TestableConsistentHashSharding<T> : ConsistentHashSharding<T> where T : class
{
    public TestableConsistentHashSharding(IEnumerable<T> nodes, int numberOfReplicas)
        : base(nodes, numberOfReplicas)
    { }

    public TestableConsistentHashSharding(IEnumerable<T> nodes)
        : base(nodes)
    { }

    public TestableConsistentHashSharding(int numberOfReplicas)
        : base(numberOfReplicas)
    { }

    public TestableConsistentHashSharding()
    { }

    public SortedDictionary<int, T> GetCircle()
    {
        return Circle;
    }
}