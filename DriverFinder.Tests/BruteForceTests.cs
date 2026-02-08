using System;
using System.Collections.Generic;
using NUnit.Framework;
using DriverFinder.Models;
using DriverFinder.Algorithms;

namespace DriverFinder.Tests;

[TestFixture]
public class BruteForceTests
{
    private BruteForce _algorithm;
    private Order _testOrder;
    private List<Driver> _testDrivers;

    [SetUp]
    public void Setup()
    {
        _algorithm = new BruteForce();
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
}