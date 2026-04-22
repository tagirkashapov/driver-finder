using System.Collections.Generic;
using DriverFinder.Models;

namespace DriverFinder.Algorithms;

public interface IAlgorithm
{
    List<Driver> FindNearestDrivers(Order order, List<Driver> drivers, int count = 5);
}