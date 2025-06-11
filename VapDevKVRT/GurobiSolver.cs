using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VapDevKVRT
{
    public class GurobiSolver : ISolver
    {
        public CVRPInstance Instance { get; set; }

        public int TimeLimit { get; set; } // in Millisekunden

        public GurobiSolver(CVRPInstance instance, int timeLimit)
        {
            Instance = instance;
            TimeLimit = timeLimit; // Gurobi expects time limit 
        }

        public CVRPSolution Solve()
        {
            var sw = Stopwatch.StartNew();
            var n = Instance.NumberOfDemandLocations;
            var N = n + 1; // inkl. Depot (0)
            var d = Instance.d;
            var Q = 200.0; // Kapazität – fix oder aus Instance extrahieren
            var A = Instance.NumberOfVehicles;
            var c = Instance.DistanceMatrix;

            using GRBEnv env = new GRBEnv(true);
            //env.OutputFlag = 0; // Keine Konsolenausgabe
            env.TimeLimit = TimeLimit;
            env.Start();
            using GRBModel model = new GRBModel(env);

            // Entscheidungsvariablen: x[i,j] ∈ {0,1}
            GRBVar[,] x = new GRBVar[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i != j)
                        x[i, j] = model.AddVar(0, 1, c[i, j], GRB.BINARY, $"x_{i}_{j}");
                }
            }

            // Hilfsvariablen für MTZ-Subtour-Elimination (q[i])
            GRBVar[] q = new GRBVar[N];
            for (int i = 1; i < N; i++)
                q[i] = model.AddVar(d[i - 1], Q, 0, GRB.CONTINUOUS, $"q_{i}");

            // Zielfunktion: Minimiere Gesamtkosten
            model.ModelSense = GRB.MINIMIZE;

            // Nebenbedingungen

            // 1. Jeder Knoten hat genau eine Einfahrt
            for (int j = 1; j < N; j++)
            {
                GRBLinExpr expr = 0;
                for (int i = 0; i < N; i++)
                    if (i != j) expr.AddTerm(1, x[i, j]);
                model.AddConstr(expr == 1, $"in_{j}");
            }

            // 2. Jeder Knoten hat genau eine Ausfahrt
            for (int i = 1; i < N; i++)
            {
                GRBLinExpr expr = 0;
                for (int j = 0; j < N; j++)
                    if (i != j) expr.AddTerm(1, x[i, j]);
                model.AddConstr(expr == 1, $"out_{i}");
            }

            // 3. Anzahl der Fahrzeuge, die vom Depot starten
            GRBLinExpr depotExpr = 0;
            for (int j = 1; j < N; j++)
                depotExpr.AddTerm(1, x[0, j]);
            model.AddConstr(depotExpr <= A, "depot_start");

            // 4. MTZ: Subtour-Elimination
            for (int i = 1; i < N; i++)
            {
                for (int j = 1; j < N; j++)
                {
                    if (i == j) continue;
                    model.AddConstr(q[i] + d[j - 1] - Q * (1 - x[i, j]) <= q[j], $"mtz_{i}_{j}");
                }
            }

            model.Optimize();
            sw.Stop();

            // Lösung extrahieren
            List<List<int>> routes = new List<List<int>>();
            if (model.SolCount > 0)
            {
                bool[,] xVal = new bool[N, N];
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j && x[i, j].X > 0.5)
                            xVal[i, j] = true;
                    }
                }

                HashSet<int> visited = new();
                for (int k = 0; k < A; k++)
                {
                    List<int> route = new List<int> { 0 };
                    int current = 0;
                    while (true)
                    {
                        bool found = false;

                        for (int j = 1; j < N; j++) //bei 0 starten
                        {
                            if (xVal[current, j] && !visited.Contains(j)) //könnte weg contains

                                for (int i = 1; i < N; j++)
                                {
                                    if (xVal[current, j] && !visited.Contains(j))

                                    {
                                        route.Add(j);
                                        visited.Add(j);
                                        current = j;
                                        found = true;
                                        break;
                                    }
                                }
                            if (!found) break;
                        }
                        route.Add(0);
                        if (route.Count > 2) routes.Add(route);
                    }
                }

                return new CVRPSolution(
                    instanceName: Instance.Name,
                    solver: "Gurobi",
                    deliveryCosts: model.ObjVal,
                    solutiontime: sw.Elapsed.TotalSeconds,
                    numberOfVehicles: routes.Count
                )
                {
                    Routes = routes
                };
            }
        }
    }
}
