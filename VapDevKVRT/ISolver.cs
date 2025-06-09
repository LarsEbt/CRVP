using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapDevKVRT
{
    public class NearestNeighborSolver
    {
        public CVRPSolution Solve(CVRPInstance instance, int vehicleCapacity)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            int n = instance.NumberOfDemandLocations;
            bool[] visited = new bool[n];
            var remainingDemand = instance.d.ToArray();

            var warehouse = instance.warehouse;
            var customers = instance.CoordinatesCustomers;

            var allRoutes = new List<List<int>>();
            double totalCost = 0;
            int vehiclesUsed = 0;

            while (visited.Any(v => !v))
            {
                var route = new List<int>();
                double remainingCapacity = vehicleCapacity;
                var currentLocation = warehouse;
                double routeCost = 0;

                while (true)
                {
                    int nearest = -1;
                    double minDistance = double.MaxValue;

                    for (int i = 0; i < n; i++)
                    {
                        if (!visited[i] && remainingDemand[i] <= remainingCapacity)
                        {
                            double dist = Distance(currentLocation, customers[i]);
                            if (dist < minDistance)
                            {
                                minDistance = dist;
                                nearest = i;
                            }
                        }
                    }

                    if (nearest == -1)
                        break; // Keine passenden Kunden mehr für diese Tour

                    // Besuch einplanen
                    route.Add(nearest);
                    visited[nearest] = true;
                    remainingCapacity -= remainingDemand[nearest];
                    routeCost += Distance(currentLocation, customers[nearest]);
                    currentLocation = customers[nearest];
                }

                // Rückfahrt zum Lager
                routeCost += Distance(currentLocation, warehouse);

                totalCost += routeCost;
                allRoutes.Add(route);
                vehiclesUsed++;
            }

            stopwatch.Stop();

            var solution = new CVRPSolution(
                instance.Name,
                "Nearest Neighbor",
                totalCost,
                stopwatch.Elapsed.TotalSeconds,
                vehiclesUsed,
                allRoutes // optional, falls CVRPSolution das unterstützt
            );

            solution.WriteToFile();
            return solution;
        }

        private double Distance((double x, double y) a, (double x, double y) b)
        {
            double dx = a.x - b.x;
            double dy = a.y - b.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public interface ISolver
    {
        CVRPInstance Instance { get; set; }

        public CVRPSolution Solve();
    }
}
