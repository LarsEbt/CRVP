using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VapDevKVRT
{
    public class NearestNeighborSolver
    {
        public CVRPSolution Solve(CVRPInstance instance, int vehicleCapacity)
        {
            var stopwatch = Stopwatch.StartNew();

            int n = instance.NumberOfDemandLocations;
            bool[] visited = new bool[n]; // für Kunden 1 bis n (Index 0 bis n-1)
            var remainingDemand = instance.d.ToArray();

            var allRoutes = new List<List<int>>();
            double totalCost = 0;
            int vehiclesUsed = 0;

            while (visited.Any(v => !v))
            {
                var route = new List<int> { 0 }; // Start beim Depot
                double remainingCapacity = vehicleCapacity;
                int currentLocationIndex = 0; // Depot = 0
                double routeCost = 0;

                while (true)
                {
                    int nearest = -1;
                    double minDistance = double.MaxValue;


                    for (int i = 0; i < n; i++)
                    {
                        if (!visited[i] && remainingDemand[i] <= remainingCapacity)
                        {
                            double dist = instance.DistanceMatrix[currentLocationIndex, i + 1];

                            if (dist < minDistance)
                            {
                                minDistance = dist;
                                nearest = i;
                            }
                        }
                    }

                    if (nearest == -1)
                        break;

                    // Gehe zum Kunden
                    route.Add(nearest + 1); // +1, weil Kunde i+1 in Matrix
                    visited[nearest] = true;
                    remainingCapacity -= remainingDemand[nearest];
                    routeCost += instance.DistanceMatrix[currentLocationIndex, nearest + 1];
                    currentLocationIndex = nearest + 1;
                }

                // Rückfahrt zum Depot
                route.Add(0);
                routeCost += instance.DistanceMatrix[currentLocationIndex, 0];

                allRoutes.Add(route);
                totalCost += routeCost;
                vehiclesUsed++;
            }

            stopwatch.Stop();

            var solution = new CVRPSolution(
                instance.Name,
                "NearestNeighbor",
                totalCost,
                stopwatch.Elapsed.TotalSeconds,
                vehiclesUsed
            );
            solution.Routes = allRoutes;
            solution.WriteToFile();

            return solution;
        }
    }
}
