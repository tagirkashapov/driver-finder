using System;
using System.Collections.Generic;
using System.Linq;
using DriverFinder.Models;

namespace DriverFinder.Algorithms;

public class PartialQuickSelect : IAlgorithm
{
    public List<Driver> FindNearestDrivers(Order order, List<Driver> drivers, int count = 5)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        if (drivers == null) throw new ArgumentNullException(nameof(drivers));
        if (count <= 0) throw new ArgumentException("Count must be positive", nameof(count));
        if (drivers.Count == 0) return new List<Driver>();

        if (count >= drivers.Count * 0.8)
        {
            return SimpleSort(order, drivers, count);
        }

        var distances = new double[drivers.Count];
        for (int i = 0; i < drivers.Count; i++)
        {
            distances[i] = CalculateDistanceSq(order, drivers[i]);
        }

        double threshold = QuickSelect(distances, count - 1);

        var candidates = new List<(Driver Driver, double DistanceSq)>();

        for (int i = 0; i < drivers.Count && candidates.Count < count * 2; i++)
        {
            if (distances[i] <= threshold * 1.1)
            {
                candidates.Add((drivers[i], distances[i]));
            }
        }

        return candidates
            .OrderBy(x => x.DistanceSq)
            .Take(count)
            .Select(x => x.Driver)
            .ToList();
    }

    private double QuickSelect(double[] array, int k)
    {
        var arr = (double[])array.Clone();
        return QuickSelectRecursive(arr, 0, arr.Length - 1, k);
    }

    private double QuickSelectRecursive(double[] arr, int left, int right, int k)
    {
        if (left == right) return arr[left];

        int pivotIndex = Partition(arr, left, right);

        if (k == pivotIndex)
        {
            return arr[k];
        }
        else if (k < pivotIndex)
        {
            return QuickSelectRecursive(arr, left, pivotIndex - 1, k);
        }
        else
        {
            return QuickSelectRecursive(arr, pivotIndex + 1, right, k);
        }
    }

    private int Partition(double[] arr, int left, int right)
    {
        double pivot = arr[right];
        int i = left - 1;

        for (int j = left; j < right; j++)
        {
            if (arr[j] <= pivot)
            {
                i++;
                Swap(arr, i, j);
            }
        }

        Swap(arr, i + 1, right);
        return i + 1;
    }

    private void Swap(double[] arr, int i, int j)
    {
        double temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }

    private List<Driver> SimpleSort(Order order, List<Driver> drivers, int count)
    {
        return drivers
            .Select(d => new
            {
                Driver = d,
                DistanceSq = CalculateDistanceSq(order, d)
            })
            .OrderBy(x => x.DistanceSq)
            .Take(count)
            .Select(x => x.Driver)
            .ToList();
    }

    private static double CalculateDistanceSq(Order order, Driver driver)
    {
        int dx = driver.X - order.X;
        int dy = driver.Y - order.Y;
        return dx * dx + dy * dy;
    }
}