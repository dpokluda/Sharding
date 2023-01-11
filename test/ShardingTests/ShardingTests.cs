using System.Diagnostics;
using System.Text;
using Sharding;
using Sharding.HashAlgorithms;
using ShardingTests.Components;

namespace ShardingTests;

[TestClass]
public class ShardingTests
{
    [TestMethod]
    public void Construct()
    {
        Assert.IsNotNull(new ConsistentHashSharding());
    }

    [TestMethod]
    public void NumberOfReplicas()
    {
        var singleReplica = new TestableConsistentHashSharding(1);
        var circle = singleReplica.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(0, circle.Count);
        
        singleReplica.Add("one");
        circle = singleReplica.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(1, circle.Count);

        singleReplica.Add("two");
        circle = singleReplica.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(2, circle.Count);
        
        var twoReplicas = new TestableConsistentHashSharding(2);
        circle = twoReplicas.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(0, circle.Count);
        
        twoReplicas.Add("one");
        circle = twoReplicas.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(2, circle.Count);

        twoReplicas.Add("two");
        circle = twoReplicas.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(4, circle.Count);
        Trace.WriteLine(string.Join(";", circle.Select((pair, i) => $"{pair.Key}:{pair.Value}")));
    }
    
    [TestMethod]
    public void Add()
    {
        var sharding = new TestableConsistentHashSharding(2);
        var circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(0, circle.Count);
        
        sharding.Add("one");
        circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(2, circle.Count);

        sharding.Add("two");
        circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(4, circle.Count);
        
        sharding.Add("one"); // another server that has the same hash value
        circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(4, circle.Count);
    }    
    
    [TestMethod]
    public void Remove()
    {
        var sharding = new TestableConsistentHashSharding(2);
        var circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(0, circle.Count);
        
        sharding.Add("one");
        circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(2, circle.Count);

        sharding.Add("two");
        circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(4, circle.Count);
        
        // remove un-existing
        sharding.Remove("three");
        circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(4, circle.Count);
        
        sharding.Remove("one");
        circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(2, circle.Count);
        
        sharding.Remove("two");
        circle = sharding.GetCircle();
        Assert.IsNotNull(circle);
        Assert.AreEqual(0, circle.Count);
    }

    [TestMethod]
    public void EmptyGetNode()
    {
        var sharding = new TestableConsistentHashSharding(2);
        Assert.ThrowsException<IndexOutOfRangeException>(() => sharding.GetNode("one"));
    }

    [TestMethod]
    public void GetNode()
    {
        var sharding = new TestableConsistentHashSharding(2);
        sharding.Add("one");

        Assert.AreEqual("one", sharding.GetNode("zero"));
        Assert.AreEqual("one", sharding.GetNode("one"));
        Assert.AreEqual("one", sharding.GetNode("two"));
        
        sharding.Add("two");
        Assert.AreEqual("one", sharding.GetNode("one0"));
        Assert.AreEqual("one", sharding.GetNode("one1"));
        Assert.AreEqual("two", sharding.GetNode("two0"));
        Assert.AreEqual("two", sharding.GetNode("two1"));
    }

    [TestMethod]
    public void ComputeHash()
    {
        var function = new Murmur2();
        var hash = function.ComputeHash(Encoding.UTF8.GetBytes("one"));

        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(hash, function.ComputeHash(Encoding.UTF8.GetBytes("one")));
        }
    }
}