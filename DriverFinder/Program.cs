using System;
using System.Collections.Generic;
using System.Linq;
using DriverFinder.Algorithms;
using DriverFinder.Models;

namespace DriverFinder;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Система подбора водителей на заказ DriverFinder\n");

        var demoConfig = new DemoConfig
        {
            GridWidth = 100,
            GridHeight = 100,
            OrderX = 50,
            OrderY = 50,
            DriversCount = 10,
            NearestDriversToFind = 5
        };

        RunAlgorithmsDemo(demoConfig);

        PrintProjectCommands();
    }

    private class DemoConfig
    {
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public int OrderX { get; set; }
        public int OrderY { get; set; }
        public int DriversCount { get; set; }
        public int NearestDriversToFind { get; set; }
    }

    private static void RunAlgorithmsDemo(DemoConfig config)
    {
        Console.WriteLine($"Демонстрация работы алгоритмов:\n");
        Console.WriteLine($"Размер карты: {config.GridWidth} x {config.GridHeight}");
        Console.WriteLine($"Заказ в точке: ({config.OrderX}, {config.OrderY})");
        Console.WriteLine($"Количество водителей: {config.DriversCount}");
        Console.WriteLine($"Ищем ближайших: {config.NearestDriversToFind}\n");

        var order = new Order { X = config.OrderX, Y = config.OrderY };
        var drivers = GenerateDrivers(config.DriversCount, config.GridWidth, config.GridHeight);
        var algorithms = GetAvailableAlgorithms();

        foreach (var algorithm in algorithms)
        {
            RunAlgorithmDemo(algorithm, order, drivers, config.NearestDriversToFind);
        }
    }

    private static List<Driver> GenerateDrivers(int count, int gridWidth, int gridHeight)
    {
        int totalCells = gridWidth * gridHeight;
        if (count > totalCells)
        {
            Console.WriteLine($"Предупреждение: невозможно разместить {count} водителей на карте {gridWidth}x{gridHeight}");
            Console.WriteLine($"Будет создано максимально возможное количество: {totalCells}");
            count = totalCells;
        }

        var random = new Random();
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

    private static List<IAlgorithm> GetAvailableAlgorithms()
    {
        return new List<IAlgorithm>
        {
            new BruteForce(),
        };
    }

    private static void RunAlgorithmDemo(IAlgorithm algorithm, Order order, List<Driver> drivers, int count)
    {
        var algorithmName = algorithm.GetType().Name;
        Console.WriteLine($"Алгоритм: {algorithmName}");

        try
        {
            var startTime = DateTime.Now;
            var nearestDrivers = algorithm.FindNearestDrivers(order, drivers, count);
            var elapsed = DateTime.Now - startTime;

            Console.WriteLine($"Время выполнения: {elapsed.TotalMilliseconds:F2} мс");

            if (nearestDrivers.Any())
            {
                Console.WriteLine("Ближайшие водители:");
                foreach (var driver in nearestDrivers)
                {
                    var distance = CalculateDistance(order.X, order.Y, driver.X, driver.Y);
                    Console.WriteLine($"ID: {driver.Id} | Координаты: ({driver.X}, {driver.Y}) | Расстояние: {distance:F2}");
                }
            }
            else
            {
                Console.WriteLine("Водители не найдены");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        Console.WriteLine();
    }

    private static double CalculateDistance(int x1, int y1, int x2, int y2)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static void PrintProjectCommands()
    {
        Console.WriteLine("Команды:");
        Console.WriteLine("Сборка решения: dotnet build");
        Console.WriteLine("Запуск демонстрации: dotnet run --project DriverFinder");
    }
}