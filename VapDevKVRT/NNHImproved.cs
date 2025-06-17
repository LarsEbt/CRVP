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

                OutsideRoute(routes, c, d, Q);
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

        private void OutsideRoute(List<List<int>> routes, double[,] c, double[] d, double Q)
        {
            bool improved = true;
            while (improved)
            {
                improved = false;

                // === SWAP-KUNDENTAUSCH ZWISCHEN ROUTEN ===
                for (int r1 = 0; r1 < routes.Count; r1++)
                {
                    for (int r2 = r1 + 1; r2 < routes.Count; r2++)
                    {
                        var route1 = routes[r1];
                        var route2 = routes[r2];

                        for (int i = 1; i < route1.Count - 1; i++)
                        {
                            for (int j = 1; j < route2.Count - 1; j++)
                            {
                                int cust1 = route1[i];
                                int cust2 = route2[j];

                                double demand1 = d[cust1 - 1];
                                double demand2 = d[cust2 - 1];

                                double cap1 = route1.Skip(1).Take(route1.Count - 2).Sum(k => d[k - 1]);
                                double cap2 = route2.Skip(1).Take(route2.Count - 2).Sum(k => d[k - 1]);

                                if (cap1 - demand1 + demand2 <= Q && cap2 - demand2 + demand1 <= Q)
                                {
                                    double before =
                                        c[route1[i - 1], route1[i]] + c[route1[i], route1[i + 1]] +
                                        c[route2[j - 1], route2[j]] + c[route2[j], route2[j + 1]];

                                    double after =
                                        c[route1[i - 1], cust2] + c[cust2, route1[i + 1]] +
                                        c[route2[j - 1], cust1] + c[cust1, route2[j + 1]];

                                    if (after < before)
                                    {
                                        route1[i] = cust2;
                                        route2[j] = cust1;
                                        improved = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // === RELOCATE — KUNDE VON EINER ROUTE IN EINE ANDERE ===
                for (int r1 = 0; r1 < routes.Count; r1++)
                {
                    for (int r2 = 0; r2 < routes.Count; r2++)
                    {
                        if (r1 == r2) continue;

                        var route1 = routes[r1];
                        var route2 = routes[r2];

                        for (int i = 1; i < route1.Count - 1; i++)
                        {
                            int cust = route1[i];
                            double demand = d[cust - 1];

                            double cap2 = route2.Skip(1).Take(route2.Count - 2).Sum(k => d[k - 1]);
                            if (cap2 + demand > Q) continue;

                            // Versuche, cust an allen Positionen in route2 einzufügen
                            for (int j = 1; j < route2.Count; j++)
                            {
                                double oldCost =
                                    c[route1[i - 1], route1[i]] + c[route1[i], route1[i + 1]] +
                                    c[route2[j - 1], route2[j]];

                                double newCost =
                                    c[route1[i - 1], route1[i + 1]] + // cust wird entfernt
                                    c[route2[j - 1], cust] + c[cust, route2[j]]; // cust wird eingefügt

                                if (newCost < oldCost)
                                {
                                    route1.RemoveAt(i);
                                    route2.Insert(j, cust);
                                    improved = true;
                                    break;
                                }
                            }

                            if (improved)
                                break;
                        }

                        if (improved)
                            break;
                    }

                    if (improved)
                        break;
                }
            }
        }

    }
}
