using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DriverFinder.Models;
using DriverFinder.Algorithms;

namespace DriverFinder.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByParams)]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class AlgorithmsBenchmarks
{
    private Order _order = null!;
    private Dictionary<int, List<Driver>> _driversByCount = new();

    private BruteForce _bruteForce = null!;
    private Radius _radius = null!;
    private PartialQuickSelect _partialQuickSelect = null!;

    [Params(100, 1000, 10000)]
    public int DriverCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        const int gridWidth = 1000;
        const int gridHeight = 1000;

        _order = new Order { X = 500, Y = 500 };
        _driversByCount[100] = GenerateDrivers(100, gridWidth, gridHeight, random);
        _driversByCount[1000] = GenerateDrivers(1000, gridWidth, gridHeight, random);
        _driversByCount[10000] = GenerateDrivers(10000, gridWidth, gridHeight, random);

        _bruteForce = new BruteForce();
        _radius = new Radius();
        _partialQuickSelect = new PartialQuickSelect();
    }

    private static List<Driver> GenerateDrivers(int count, int gridWidth, int gridHeight, Random random)
    {
        var drivers = new List<Driver>(count);
        var allPositions = new List<(int X, int Y)>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                allPositions.Add((x, y));
            }
        }

        for (int i = allPositions.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (allPositions[i], allPositions[j]) = (allPositions[j], allPositions[i]);
        }

        for (int i = 0; i < count; i++)
        {
            var (x, y) = allPositions[i];
            drivers.Add(new Driver
            {
                Id = i + 1,
                X = x,
                Y = y
            });
        }

        return drivers;
    }

    [Benchmark]
    public List<Driver> BruteForce()
    {
        return _bruteForce.FindNearestDrivers(_order, _driversByCount[DriverCount], 5);
    }

    [Benchmark]
    public List<Driver> Radius()
    {
        return _radius.FindNearestDrivers(_order, _driversByCount[DriverCount], 5);
    }

    [Benchmark]
    public List<Driver> PartialQuickSelect()
    {
        return _partialQuickSelect.FindNearestDrivers(_order, _driversByCount[DriverCount], 5);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<AlgorithmsBenchmarks>();
    }
}