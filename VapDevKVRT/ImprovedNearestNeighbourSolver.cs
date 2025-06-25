using CVRP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapDevKVRT
{
    /// Verbesserte Nearest-Neighbour-Heuristik für das CVRP.
    /// Basiert auf der einfachen NN-Heuristik, fügt aber lokale Optimierung hinzu:
    /// - 2-opt Optimierung für einzelne Routen
    /// - Austausch von Kunden zwischen Routen
    /// - Verschiebung von Kunden zwischen Routen
    public class ImprovedNearestNeighbourSolver : ISolver
    {
        public CVRPInstance Instance { get; set; }

        public ImprovedNearestNeighbourSolver(CVRPInstance instance)
        {
            Instance = instance;
        }

        public CVRPSolution Solve()
        {
            int n = Instance.NodeCount;              
            int K = Instance.VehicleCount;           
            double Q = Instance.VehicleCapacity;     
            double[,] c = Instance.CostMatrix;       
            // Kopiere Nachfragen, damit wir Instance nicht ändern
            double[] d = new double[n];
            Array.Copy(Instance.Demands, d, n);

            var sw = Stopwatch.StartNew();

            // Phase 1: Erstelle initiale Lösung mit Nearest Neighbour
            var routes = CreateInitialSolution(n, K, Q, c, d);

            // Phase 2: Optimiere einzelne Routen mit 2-opt
            for (int k = 0; k < routes.Count; k++)
            {
                routes[k] = OptimizeRoute2Opt(routes[k], c);
            }

            // Phase 3: Optimiere zwischen Routen (Swap und Relocate)
            OptimizeBetweenRoutes(routes, c, d, Q);

            // Konvertiere Routen in Solution-Format
            var (xSol, ySol, travelCosts) = ConvertRoutesToSolution(routes, K, n, c);

            sw.Stop();
            double solutionTime = sw.Elapsed.TotalSeconds;

            return new CVRPSolution(
                Instance.Name,
                "ImprovedNearestNeighbour",
                travelCosts,
                solutionTime,
                xSol,
                ySol
            );
        }

        /// Erstellt eine initiale Lösung mit der Nearest Neighbour Heuristik
        private List<List<int>> CreateInitialSolution(int n, int K, double Q, double[,] c, double[] d)
        {
            var routes = new List<List<int>>();
            var demandsCopy = new double[n];
            Array.Copy(d, demandsCopy, n);

            // Für jedes Fahrzeug k
            for (int k = 0; k < K; k++)
            {
                double remainingCap = Q;
                int currentNode = 0; // starte immer im Depot
                var route = new List<int> { 0 }; // Route beginnt im Depot

                while (true)
                {
                    // Finde nächsten Kunden, dessen Nachfrage ≤ remainingCap
                    int next = -1;
                    double bestDist = double.MaxValue;
                    for (int i = 1; i < n; i++) // i=0 ist das Depot
                    {
                        if (demandsCopy[i] > 0 && demandsCopy[i] <= remainingCap)
                        {
                            if (c[currentNode, i] < bestDist)
                            {
                                bestDist = c[currentNode, i];
                                next = i;
                            }
                        }
                    }
                    if (next < 0)
                        break;  // kein weiterer Kunde passend

                    // Fahre zu next
                    remainingCap -= demandsCopy[next];
                    demandsCopy[next] = 0; // Kunde bedient
                    route.Add(next);
                    currentNode = next;
                }

                if (route.Count > 1) // Route enthält mehr als nur Depot
                {
                    route.Add(0); // Rückkehr zum Depot
                    routes.Add(route);
                }

                // Wenn alle Kunden bedient sind, kann abgebrochen werden
                if (demandsCopy.Sum() == 0)
                    break;
            }

            return routes;
        }

        /// Optimiert eine einzelne Route mit 2-opt: Tausche zwei Kunden in der Route und prüfe, ob die Gesamtkosten reduziert werden
        private List<int> OptimizeRoute2Opt(List<int> route, double[,] c)
        {
            bool improved = true;
            while (improved)
            {
                improved = false;
                for (int i = 1; i < route.Count - 2; i++)
                {
                    for (int j = i + 1; j < route.Count - 1; j++)
                    {
                        // Berechne Kosten vor und nach 2-opt Tausch (j und i tauschen)
                        double before = c[route[i - 1], route[i]] + c[route[j], route[j + 1]];
                        double after = c[route[i - 1], route[j]] + c[route[i], route[j + 1]];

                        if (after < before)
                        {
                            // Führe 2-opt Tausch durch (umkehren von Teilroute von i bis j)
                            route.Reverse(i, j - i + 1);
                            improved = true;
                        }
                    }
                }
            }
            return route;
        }

        /// Optimiert zwischen Routen durch Swap und Relocate Operationen
        private void OptimizeBetweenRoutes(List<List<int>> routes, double[,] c, double[] d, double Q)
        {
            bool improved = true;
            while (improved)
            {
                improved = false;

                // Swap: Tausche Kunden zwischen zwei Routen
                for (int r1 = 0; r1 < routes.Count; r1++)
                {
                    for (int r2 = r1 + 1; r2 < routes.Count; r2++)
                    {
                        if (TrySwapCustomers(routes[r1], routes[r2], c, d, Q))
                            improved = true;
                    }
                }

                // Relocate: Verschiebe einen Kunden von einer Route in eine andere
                for (int r1 = 0; r1 < routes.Count; r1++)
                {
                    for (int r2 = 0; r2 < routes.Count; r2++)
                    {
                        if (r1 != r2 && TryRelocateCustomer(routes[r1], routes[r2], c, d, Q))
                            improved = true;
                    }
                }
            }
        }

        /// Versucht zwei Kunden zwischen Routen zu tauschen
        private bool TrySwapCustomers(List<int> route1, List<int> route2, double[,] c, double[] d, double Q)
        {
            for (int i = 1; i < route1.Count - 1; i++)
            {
                for (int j = 1; j < route2.Count - 1; j++)
                {
                    int cust1 = route1[i];
                    int cust2 = route2[j];

                    // Prüfe Kapazitätsbeschränkungen
                    double demand1 = d[cust1];
                    double demand2 = d[cust2];
                    double cap1 = route1.Skip(1).Take(route1.Count - 2).Sum(k => d[k]);
                    double cap2 = route2.Skip(1).Take(route2.Count - 2).Sum(k => d[k]);

                    if (cap1 - demand1 + demand2 <= Q && cap2 - demand2 + demand1 <= Q)
                    {
                        // Berechne Kosten vor und nach Tausch
                        double before = c[route1[i - 1], cust1] + c[cust1, route1[i + 1]] +
                                      c[route2[j - 1], cust2] + c[cust2, route2[j + 1]];
                        double after = c[route1[i - 1], cust2] + c[cust2, route1[i + 1]] +
                                     c[route2[j - 1], cust1] + c[cust1, route2[j + 1]];

                        if (after < before)
                        {
                            // Führe Tausch durch
                            route1[i] = cust2;
                            route2[j] = cust1;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// Versucht einen Kunden von einer Route in eine andere zu verschieben
        private bool TryRelocateCustomer(List<int> route1, List<int> route2, double[,] c, double[] d, double Q)
        {
            for (int i = 1; i < route1.Count - 1; i++)
            {
                int customer = route1[i];
                double demand = d[customer];

                // Prüfe Kapazität der Zielroute
                double cap2 = route2.Skip(1).Take(route2.Count - 2).Sum(k => d[k]);
                if (cap2 + demand > Q) continue;

                for (int j = 1; j < route2.Count; j++)
                {
                    // Berechne Kosten vor und nach Verschiebung
                    double oldCost = c[route1[i - 1], customer] + c[customer, route1[i + 1]] +
                                   c[route2[j - 1], route2[j]];
                    double newCost = c[route1[i - 1], route1[i + 1]] +
                                   c[route2[j - 1], customer] + c[customer, route2[j]];

                    if (newCost < oldCost)
                    {
                        // Führe Verschiebung durch
                        route1.RemoveAt(i);
                        route2.Insert(j, customer);
                        return true;
                    }
                }
            }
            return false;
        }

        /// Konvertiert die Routen in das Solution-Format
        private (double[,] xSol, double[] ySol, double travelCosts) ConvertRoutesToSolution(
            List<List<int>> routes, int K, int n, double[,] c)
        {
            var xSol = new double[K, n];
            var ySol = new double[K];
            double travelCosts = 0.0;

            for (int k = 0; k < routes.Count && k < K; k++)
            {
                var route = routes[k];
                ySol[k] = 1; // Fahrzeug wird verwendet

                // Markiere alle besuchten Knoten (außer Depot)
                for (int i = 1; i < route.Count - 1; i++)
                {
                    xSol[k, route[i]] = 1;
                }

                // Berechne Gesamtkosten der Route
                for (int i = 0; i < route.Count - 1; i++)
                {
                    travelCosts += c[route[i], route[i + 1]];
                }
            }

            return (xSol, ySol, travelCosts);
        }
    }
}
