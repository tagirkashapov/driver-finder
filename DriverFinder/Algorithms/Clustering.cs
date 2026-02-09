using System;
using System.Collections.Generic;
using System.Linq;
using DriverFinder.Models;

namespace DriverFinder.Algorithms;

public class Clustering : IAlgorithm
{
    private readonly int _clusterSize;

    public Clustering(int clusterSize = 20)
    {
        _clusterSize = clusterSize;
    }

    public List<Driver> FindNearestDrivers(Order order, List<Driver> drivers, int count = 5)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        if (drivers == null) throw new ArgumentNullException(nameof(drivers));
        if (count <= 0) throw new ArgumentException("Count must be positive", nameof(count));
        if (drivers.Count == 0) return new List<Driver>();

        var clusters = GroupIntoClusters(drivers);

        var orderCluster = GetClusterKey(order.X, order.Y);

        var nearestDrivers = new List<Driver>();
        if (clusters.ContainsKey(orderCluster))
        {
            var driversInCluster = clusters[orderCluster];
            nearestDrivers.AddRange(FindNearestInList(order, driversInCluster, count));
        }

        if (nearestDrivers.Count < count)
        {
            var neighborClusters = GetNeighborClusters(orderCluster);

            foreach (var clusterKey in neighborClusters)
            {
                if (clusters.ContainsKey(clusterKey))
                {
                    var driversInNeighbor = clusters[clusterKey];
                    nearestDrivers.AddRange(FindNearestInList(order, driversInNeighbor, count - nearestDrivers.Count));

                    if (nearestDrivers.Count >= count)
                        break;
                }
            }
        }

        if (nearestDrivers.Count < count && nearestDrivers.Count < drivers.Count)
        {
            var allDrivers = drivers.Where(d => !nearestDrivers.Contains(d)).ToList();
            nearestDrivers.AddRange(FindNearestInList(order, allDrivers, count - nearestDrivers.Count));
        }

        return nearestDrivers.Take(count).ToList();
    }

    private Dictionary<(int, int), List<Driver>> GroupIntoClusters(List<Driver> drivers)
    {
        var clusters = new Dictionary<(int, int), List<Driver>>();

        foreach (var driver in drivers)
        {
            var clusterKey = GetClusterKey(driver.X, driver.Y);

            if (!clusters.ContainsKey(clusterKey))
                clusters[clusterKey] = new List<Driver>();

            clusters[clusterKey].Add(driver);
        }

        return clusters;
    }

    private (int, int) GetClusterKey(int x, int y)
    {
        return (x / _clusterSize, y / _clusterSize);
    }

    private List<(int, int)> GetNeighborClusters((int, int) centerCluster)
    {
        var neighbors = new List<(int, int)>();
        var (centerX, centerY) = centerCluster;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                neighbors.Add((centerX + dx, centerY + dy));
            }
        }

        return neighbors;
    }

    private List<Driver> FindNearestInList(Order order, List<Driver> drivers, int count)
    {
        return drivers
            .Select(d => new
            {
                Driver = d,
                Distance = CalculateDistance(order.X, order.Y, d.X, d.Y)
            })
            .OrderBy(x => x.Distance)
            .Take(count)
            .Select(x => x.Driver)
            .ToList();
    }

    private static double CalculateDistance(int x1, int y1, int x2, int y2)
    {
        int dx = x2 - x1;
        int dy = y2 - y1;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}