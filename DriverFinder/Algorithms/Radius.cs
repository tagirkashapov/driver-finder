using System;
using System.Collections.Generic;
using System.Linq;
using DriverFinder.Models;

namespace DriverFinder.Algorithms;

public class Radius : IAlgorithm
{
    private readonly int _initialRadius;

    public Radius(int initialRadius = 50)
    {
        _initialRadius = initialRadius;
    }

    public List<Driver> FindNearestDrivers(Order order, List<Driver> drivers, int count = 5)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        if (drivers == null) throw new ArgumentNullException(nameof(drivers));
        if (count <= 0) throw new ArgumentException("Count must be positive", nameof(count));
        if (drivers.Count == 0) return new List<Driver>();

        if (count >= drivers.Count)
        {
            return SortByDistanceSquared(order, drivers, drivers.Count);
        }

        List<Driver> result = new List<Driver>();
        int radiusSq = _initialRadius * _initialRadius;

        while (result.Count < count * 2 && radiusSq < int.MaxValue / 4)
        {
            result.Clear();
            int foundCount = 0;

            for (int i = 0; i < drivers.Count; i++)
            {
                var driver = drivers[i];
                int dx = driver.X - order.X;
                int dy = driver.Y - order.Y;
                int distanceSq = dx * dx + dy * dy;

                if (distanceSq <= radiusSq)
                {
                    result.Add(driver);
                    foundCount++;
                }
            }

            if (foundCount >= count)
            {
                break;
            }

            radiusSq *= 4;
        }

        return SortByDistanceSquared(order, result, count);
    }

    private List<Driver> SortByDistanceSquared(Order order, List<Driver> drivers, int count)
    {
        var tempArray = new (Driver driver, int distanceSq)[drivers.Count];

        for (int i = 0; i < drivers.Count; i++)
        {
            var driver = drivers[i];
            int dx = driver.X - order.X;
            int dy = driver.Y - order.Y;
            tempArray[i] = (driver, dx * dx + dy * dy);
        }

        Array.Sort(tempArray, (a, b) => a.distanceSq.CompareTo(b.distanceSq));

        var result = new List<Driver>(count);
        for (int i = 0; i < Math.Min(count, tempArray.Length); i++)
        {
            result.Add(tempArray[i].driver);
        }

        return result;
    }
}