using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapDevKVRT
{
    public class NNHImproved : ISolver
    {
        public CVRPInstance Instance { get; set; }
        public int TimeLimit { get; set; }
        public NNHImproved(CVRPInstance instance, int timeLimit)
        {
            Instance = instance;
            TimeLimit = timeLimit;
        }

        public CVRPSolution Solve()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var n = Instance.NumberOfDemandLocations;
            var N = n + 1; // including depot
            var d = Instance.d;
            var Q = 200.0;
            var c = Instance.DistanceMatrix;

            bool[] visited = new bool[N];
            visited[0] = true;
            List<List<int>> routes = new();
            int unvisited = n;

            while (unvisited > 0)
            {
                List<int> route = new() { 0 };
                double remainingCapacity = Q;
                int current = 0;

                while (true)
                {
                    double bestDist = double.MaxValue;
                    int next = -1;

                    for (int j = 1; j < N; j++)
                    {
                        if (!visited[j] && d[j - 1] <= remainingCapacity)
                        {
                            double dist = c[current, j];

                            // Heuristik: Start eines neuen Fahrzeugs, wenn zu weit weg
                            if (dist < bestDist && dist <= 1.2 * c[0, j])
                            {
                                bestDist = dist;
                                next = j;
                            }
                        }
                    }

                    if (next == -1)
                        break;

                    route.Add(next);
                    visited[next] = true;
                    unvisited--;
                    remainingCapacity -= d[next - 1];
                    current = next;
                }

                route.Add(0);
                route = InsideRoute(route, c);
                routes.Add(route);
            }

            double totalDistance = 0.0;
            foreach (var route in routes)
            {
                for (int i = 0; i < route.Count - 1; i++)
                    totalDistance += c[route[i], route[i + 1]];
            }

            sw.Stop();
            return new CVRPSolution(
                instanceName: Instance.Name,
                solver: "NNHImproved",
                deliveryCosts: totalDistance,
                solutiontime: sw.Elapsed.TotalSeconds,
                numberOfVehicles: routes.Count
            )
            {
                Routes = routes
            };
        }

        private List<int> InsideRoute(List<int> route, double[,] distanceMatrix)
        {
            bool improved = true;
            while (improved)
            {
                improved = false;
                for (int i = 1; i < route.Count - 2; i++)
                {
                    for (int j = i + 1; j < route.Count - 1; j++)
                    {
                        double before = distanceMatrix[route[i - 1], route[i]] + distanceMatrix[route[j], route[j + 1]];
                        double after = distanceMatrix[route[i - 1], route[j]] + distanceMatrix[route[i], route[j + 1]];

                        if (after < before)
                        {
                            route.Reverse(i, j - i + 1);
                            improved = true;
                        }
                    }
                }
            }
            return route;
        }
    }
}
