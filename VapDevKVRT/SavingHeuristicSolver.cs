using CVRP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VapDevKVRT
{
    public class SavingHeuristicSolver : ISolver
    {
        public CVRPInstance Instance { get; set; }
        public double TimeLimit { get; set; } = 300; // Default 5 minutes

        public SavingHeuristicSolver(CVRPInstance instance)
        {
            Instance = instance;
        }

        public SavingHeuristicSolver(CVRPInstance instance, double timeLimit)
        {
            Instance = instance;
            TimeLimit = timeLimit;
        }

        public CVRPSolution Solve()
        {
            return Solve(TimeLimit);
        }

        public CVRPSolution Solve(double timeLimit)
        {
            var sw = Stopwatch.StartNew();
            
            try
            {
                int n = Instance.NodeCount;                // Gesamtzahl Knoten (Depot + Kunden)
                int cust = n - 1;                          // Anzahl Kunden
                double[,] c = Instance.CostMatrix;         // Kostenmatrix
                double[] demand = Instance.Demands;        // Nachfragen
                double Q = Instance.VehicleCapacity;       // Fahrzeugkapazität

                // Check time limit before starting
                if (sw.ElapsedMilliseconds / 1000.0 > timeLimit)
                {
                    sw.Stop();
                    return new CVRPSolution(
                        Instance.Name,
                        "SavingHeuristic",
                        0,
                        sw.Elapsed.TotalSeconds,
                        new double[Instance.VehicleCount, n],
                        new double[Instance.VehicleCount]
                    );
                }

                // 1. Savings berechnen
                List<(int i, int j, double saving)> savingsList = new();
                for (int i = 1; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        double saving = c[0, i] + c[0, j] - c[i, j];
                        savingsList.Add((i, j, saving));
                    }
                }
                savingsList.Sort((a, b) => b.saving.CompareTo(a.saving));

                // Check time limit after computing savings
                if (sw.ElapsedMilliseconds / 1000.0 > timeLimit)
                {
                    sw.Stop();
                    return new CVRPSolution(
                        Instance.Name,
                        "SavingHeuristic",
                        0,
                        sw.Elapsed.TotalSeconds,
                        new double[Instance.VehicleCount, n],
                        new double[Instance.VehicleCount]
                    );
                }

                // 2. Initialrouten: jede Nachfrage hat eigene Route
                Dictionary<int, List<int>> routes = new();
                Dictionary<int, double> routeLoads = new();
                Dictionary<int, (int start, int end)> routeEndpoints = new();

                for (int i = 1; i < n; i++)
                {
                    routes[i] = new List<int> { 0, i, 0 };
                    routeLoads[i] = demand[i];
                    routeEndpoints[i] = (i, i); // Start und Ende sind beide i
                }

                // 3. Routen kombinieren anhand Savings
                foreach (var (i, j, saving) in savingsList)
                {
                    // Check time limit during savings processing
                    if (sw.ElapsedMilliseconds / 1000.0 > timeLimit)
                    {
                        break;
                    }

                    int? routeI = null;
                    int? routeJ = null;

                    foreach (var kv in routeEndpoints)
                    {
                        if (kv.Value.start == i || kv.Value.end == i) routeI = kv.Key;
                        if (kv.Value.start == j || kv.Value.end == j) routeJ = kv.Key;
                        if (routeI != null && routeJ != null) break;
                    }

                    if (routeI == null || routeJ == null || routeI.Value == routeJ.Value) continue;
                    if (routeLoads[routeI.Value] + routeLoads[routeJ.Value] > Q)
                    {
                        continue;
                    }

                    var endpointsI = routeEndpoints[routeI.Value];
                    var endpointsJ = routeEndpoints[routeJ.Value];
                    var rI = routes[routeI.Value];
                    var rJ = routes[routeJ.Value];

                    List<int> newRoute = null;
                    (int start, int end) newEndpoints = (0, 0);

                    if (endpointsI.end == i && endpointsJ.start == j)
                    {
                        newRoute = new List<int>(rI);
                        newRoute.RemoveAt(newRoute.Count - 1);
                        var tempRJ = new List<int>(rJ);
                        tempRJ.RemoveAt(0);
                        newRoute.AddRange(tempRJ);
                        newEndpoints = (endpointsI.start, endpointsJ.end);
                    }
                    else if (endpointsI.end == i && endpointsJ.end == j)
                    {
                        newRoute = new List<int>(rI);
                        newRoute.RemoveAt(newRoute.Count - 1);
                        var tempRJ = new List<int>(rJ);
                        tempRJ.RemoveAt(tempRJ.Count - 1);
                        tempRJ.Reverse();
                        tempRJ.RemoveAt(0);
                        newRoute.AddRange(tempRJ);
                        newEndpoints = (endpointsI.start, endpointsJ.start);
                    }
                    else if (endpointsI.start == i && endpointsJ.start == j)
                    {
                        var tempRI = new List<int>(rI);
                        tempRI.RemoveAt(0);
                        tempRI.Reverse();
                        tempRI.RemoveAt(tempRI.Count - 1);
                        var tempRJ = new List<int>(rJ);
                        tempRJ.RemoveAt(0);
                        newRoute = new List<int> { 0 };
                        newRoute.AddRange(tempRI);
                        newRoute.AddRange(tempRJ);
                        newEndpoints = (endpointsI.end, endpointsJ.end);
                    }
                    else if (endpointsI.start == i && endpointsJ.end == j)
                    {
                        var tempRI = new List<int>(rI);
                        tempRI.RemoveAt(0);
                        tempRI.Reverse();
                        tempRI.RemoveAt(tempRI.Count - 1);
                        var tempRJ = new List<int>(rJ);
                        tempRJ.RemoveAt(tempRJ.Count - 1);
                        newRoute = new List<int> { 0 };
                        newRoute.AddRange(tempRJ);
                        newRoute.AddRange(tempRI);
                        newEndpoints = (endpointsJ.start, endpointsI.end);
                    }

                    if (newRoute != null)
                    {
                        while (newRoute.Count > 1 && newRoute[0] == 0 && newRoute[1] == 0)
                            newRoute.RemoveAt(0);

                        if (newRoute.Count == 0 || newRoute[0] != 0)
                            newRoute.Insert(0, 0);
                        if (newRoute[^1] != 0)
                            newRoute.Add(0);

                        routes[routeI.Value] = newRoute;
                        routeEndpoints[routeI.Value] = newEndpoints;
                        routeLoads[routeI.Value] += routeLoads[routeJ.Value];

                        routes.Remove(routeJ.Value);
                        routeLoads.Remove(routeJ.Value);
                        routeEndpoints.Remove(routeJ.Value);
                    }
                }

                // 4. Kosten berechnen und finale Routen erstellen
                var usedRoutes = routes.Values.ToList();
                var allCustomerIds = Enumerable.Range(1, cust);

                foreach (var id in allCustomerIds)
                {
                    bool alreadyIncluded = usedRoutes.Any(route => route.Contains(id));
                    if (!alreadyIncluded)
                    {
                        double demandOfCustomer = demand[id];
                        bool inserted = false;

                        // Versuche, den Kunden in eine bestehende Route einzufügen
                        for (int r = 0; r < usedRoutes.Count; r++)
                        {
                            var route = usedRoutes[r];
                            double currentLoad = route
                                .Where(x => x != 0)
                                .Sum(x => demand[x]);

                            if (currentLoad + demandOfCustomer <= Q)
                            {
                                int bestPos = -1;
                                double bestIncrease = double.MaxValue;

                                for (int pos = 1; pos < route.Count; pos++)
                                {
                                    int prev = route[pos - 1];
                                    int next = route[pos];
                                    double increase = c[prev, id] + c[id, next] - c[prev, next];

                                    if (increase < bestIncrease)
                                    {
                                        bestIncrease = increase;
                                        bestPos = pos;
                                    }
                                }

                                if (bestPos != -1)
                                {
                                    route.Insert(bestPos, id);
                                    inserted = true;
                                    break;
                                }
                            }
                        }

                        // Falls keine geeignete Route gefunden wurde
                        if (!inserted)
                        {
                            usedRoutes.Add(new List<int> { 0, id, 0 });
                        }
                    }
                }

                // 5. Kosten berechnen
                double totalCost = 0.0;
                foreach (var route in usedRoutes)
                {
                    for (int i = 0; i < route.Count - 1; i++)
                        totalCost += c[route[i], route[i + 1]];
                }

                // 6. CVRPSolution im erwarteten Format erstellen
                var xSol = new double[Instance.VehicleCount, n];
                var ySol = new double[Instance.VehicleCount];

                for (int k = 0; k < usedRoutes.Count && k < Instance.VehicleCount; k++)
                {
                    ySol[k] = 1; // Fahrzeug wird verwendet
                    var route = usedRoutes[k];
                    
                    foreach (int node in route)
                    {
                        if (node != 0) // Nicht das Depot
                        {
                            xSol[k, node] = 1; // Fahrzeug k besucht Knoten node
                        }
                    }
                }

                sw.Stop();
                return new CVRPSolution(
                    Instance.Name,
                    "SavingHeuristic",
                    totalCost,
                    sw.Elapsed.TotalSeconds,
                    xSol,
                    ySol
                );
            }
            catch (Exception ex)
            {
                sw.Stop();
                Console.WriteLine($"Error in SavingHeuristic solver: {ex.Message}");
                
                return new CVRPSolution(
                    Instance.Name,
                    "SavingHeuristic",
                    0,
                    sw.Elapsed.TotalSeconds,
                    new double[Instance.VehicleCount, Instance.NodeCount],
                    new double[Instance.VehicleCount]
                );
            }
        }
    }
}