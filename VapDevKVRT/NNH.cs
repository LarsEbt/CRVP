using System;               
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapDevKVRT
{
    public class NNH : ISolver
    {
        public CVRPInstance Instance { get; set; }
        public int TimeLimit { get; set; } // in Millisekunden
        public NNH(CVRPInstance instance, int timeLimit)
        {
            Instance = instance;
            TimeLimit = timeLimit; // NNH expects time limit 
        }
        public CVRPSolution Solve()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var n = Instance.NumberOfDemandLocations;
            var N = n + 1; // including depot (0)
            var d = Instance.d;
            var Q = 200.0; // Capacity – fixed or extracted from Instance
            var A = Instance.NumberOfVehicles;
            var c = Instance.DistanceMatrix;

            bool[] visited = new bool[N]; // Daten vorbereiten
            visited[0] = true;
            List<List<int>> routes = new();

            int vehicleUsed = 0;
            while (visited.Any(v => v == false)) // Für jeden Lkw
            {
                List<int> route = new() { 0 };
                double remainingCapacity = Q; 
                int currentNode = 0;

                while (true)
                {
                    double minDist = double.MaxValue;
                    int next = -1;

                    for (int j = 1; j < N; j++) // Nur Kunden
                    {
                        if (!visited[j] && d[j - 1] <= remainingCapacity && c[currentNode, j] < minDist)
                        {
                            minDist = c[currentNode, j];
                            next = j;
                        }
                    }

                    if (next == -1) break; // kein passender Kunde mehr

                    route.Add(next);
                    visited[next] = true;
                    remainingCapacity -= d[next - 1];
                    currentNode = next;
                }
                route.Add(0); // Zurück zum Depot
                route = InsideOpt(route, c); // 2-opt-Optimierung
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
                solver: "NNH",
                deliveryCosts: totalDistance,
                solutiontime: sw.Elapsed.TotalSeconds, // Time taken for the solution
                numberOfVehicles: routes.Count
            )
            {
                Routes = routes
            };

        }
        private List<int> InsideOpt(List<int> route, double[,] c)
        {
            if (route.Count <= 4) return route;

            bool improved = true;
            int maxIterations = 1000;
            int iteration = 0;

            while (improved && iteration < maxIterations)
            {
                improved = false;
                iteration++;

                for (int i = 1; i < route.Count - 2; i++)
                {
                    for (int k = i + 1; k < route.Count - 1; k++)
                    {
                        double delta =
                            c[route[i - 1], route[k]] +
                            c[route[i], route[k + 1]] -
                            c[route[i - 1], route[i]] -
                            c[route[k], route[k + 1]];

                        if (delta < -1e-3) // größerer Schwellenwert
                        {
                            route.Reverse(i, k - i + 1);
                            improved = true;
                        }
                    }
                }
            }

            return route;
        }

    }
}
