using System;
using System.Collections.Generic;
using NUnit.Framework;
using DriverFinder.Models;
using DriverFinder.Algorithms;

namespace DriverFinder.Tests;

[TestFixture]
public class PartialQuickSelectTests
{
    private PartialQuickSelect _algorithm;
    private Order _testOrder;
    private List<Driver> _testDrivers;

    [SetUp]
    public void Setup()
    {
        _algorithm = new PartialQuickSelect();
        _testOrder = new Order { X = 0, Y = 0 };
        _testDrivers = new List<Driver>
        {
            new Driver { Id = 1, X = 10, Y = 10 },
            new Driver { Id = 2, X = 5, Y = 5 },
            new Driver { Id = 3, X = 15, Y = 15 },
            new Driver { Id = 4, X = 20, Y = 20 },
            new Driver { Id = 5, X = 25, Y = 25 }
        };
    }

    [Test]
    public void FindNearestDrivers_ReturnsCorrectNumberOfDrivers()
    {
        var result = _algorithm.FindNearestDrivers(_testOrder, _testDrivers, 2);
        Assert.That(result.Count, Is.EqualTo(2));

        result = _algorithm.FindNearestDrivers(_testOrder, _testDrivers, 3);
        Assert.That(result.Count, Is.EqualTo(3));

        result = _algorithm.FindNearestDrivers(_testOrder, _testDrivers, 5);
        Assert.That(result.Count, Is.EqualTo(5));
    }

    [Test]
    public void FindNearestDrivers_ReturnsDriversSortedByDistance()
    {
        var result = _algorithm.FindNearestDrivers(_testOrder, _testDrivers, 5);

        Assert.That(result[0].Id, Is.EqualTo(2));
        Assert.That(result[1].Id, Is.EqualTo(1));
        Assert.That(result[2].Id, Is.EqualTo(3));
        Assert.That(result[3].Id, Is.EqualTo(4));
        Assert.That(result[4].Id, Is.EqualTo(5));
    }

    [Test]
    public void FindNearestDrivers_RequestMoreDriversThanAvailable_ReturnsAllDrivers()
    {
        var result = _algorithm.FindNearestDrivers(_testOrder, _testDrivers, 10);
        Assert.That(result.Count, Is.EqualTo(5));
    }

    [Test]
    public void FindNearestDrivers_EmptyDriverList_ReturnsEmptyList()
    {
        var emptyDrivers = new List<Driver>();
        var result = _algorithm.FindNearestDrivers(_testOrder, emptyDrivers, 5);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void FindNearestDrivers_RequestZeroDrivers_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _algorithm.FindNearestDrivers(_testOrder, _testDrivers, 0));
    }

    [Test]
    public void FindNearestDrivers_UsesSimpleSortForLargePercentage()
    {
        var manyDrivers = new List<Driver>();
        for (int i = 0; i < 10; i++)
        {
            manyDrivers.Add(new Driver { Id = i, X = i * 10, Y = i * 10 });
        }

        var order = new Order { X = 0, Y = 0 };
        var result = _algorithm.FindNearestDrivers(order, manyDrivers, 8);

        Assert.That(result.Count, Is.EqualTo(8));
    }

    [Test]
    public void FindNearestDrivers_UsesQuickSelectForSmallPercentage()
    {
        var manyDrivers = new List<Driver>();
        for (int i = 0; i < 10; i++)
        {
            manyDrivers.Add(new Driver { Id = i, X = i * 10, Y = i * 10 });
        }

        var order = new Order { X = 0, Y = 0 };
        var result = _algorithm.FindNearestDrivers(order, manyDrivers, 2);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo(0));
        Assert.That(result[1].Id, Is.EqualTo(1));
    }

    [Test]
    public void FindNearestDrivers_WorksWithThreshold10Percent()
    {
        var drivers = new List<Driver>
        {
            new Driver { Id = 1, X = 10, Y = 10 },
            new Driver { Id = 2, X = 11, Y = 11 },
            new Driver { Id = 3, X = 12, Y = 12 },
            new Driver { Id = 4, X = 50, Y = 50 },
            new Driver { Id = 5, X = 60, Y = 60 }
        };

        var order = new Order { X = 0, Y = 0 };
        var result = _algorithm.FindNearestDrivers(order, drivers, 2);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo(1));
        Assert.That(result[1].Id, Is.EqualTo(2));
    }
}