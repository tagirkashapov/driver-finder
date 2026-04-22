using System;
using System.Collections.Generic;
using System.Linq;
using DriverFinder.Models;

namespace DriverFinder.Algorithms;

public class BruteForce : IAlgorithm
{
    public List<Driver> FindNearestDrivers(Order order, List<Driver> drivers, int count = 5)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        if (drivers == null) throw new ArgumentNullException(nameof(drivers));
        if (count <= 0) throw new ArgumentException("Count must be positive", nameof(count));
        if (drivers.Count == 0) return new List<Driver>();

        var driversWithDistance = drivers.Select(driver => new
        {
            Driver = driver,
            Distance = CalculateDistance(order.X, order.Y, driver.X, driver.Y)
        });

        return driversWithDistance
            .OrderBy(d => d.Distance)
            .Take(count)
            .Select(d => d.Driver)
            .ToList();
    }

    private static double CalculateDistance(int x1, int y1, int x2, int y2)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}