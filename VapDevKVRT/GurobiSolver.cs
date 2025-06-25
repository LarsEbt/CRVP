using CVRP;
using Gurobi;
using System;
using System.Diagnostics;

namespace VapDevKVRT
{
    public class GurobiSolver : ISolver
    {
        public CVRPInstance Instance { get; set; }
        public double TimeLimit { get; set; } = 300;

        public GurobiSolver(CVRPInstance instance)
        {
            Instance = instance;
        }

        public GurobiSolver(CVRPInstance instance, double timeLimit)
        {
            Instance = instance;
            TimeLimit = timeLimit;
        }

        public CVRPSolution Solve()
        {
            Console.WriteLine($"Starte Gurobi-Solver für Instanz: {Instance.Name}");
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Erzeuge Gurobi-Umgebung und Modell
                using var env = new GRBEnv();
                using var model = new GRBModel(env);

                int n = Instance.NodeCount;  // Anzahl der Knoten (inklusive Depot)
                int K = Instance.VehicleCount;  // Anzahl der Fahrzeuge
                int depot = 0;  // Depot ist immer Knoten 0

                // Entscheidungsvariablen
                // x[k,i,j] = 1, falls Fahrzeug k von Knoten i nach j fährt, sonst 0
                var x = new GRBVar[K, n, n];
                // y[k] = 1, falls Fahrzeug k genutzt wird, sonst 0
                var y = new GRBVar[K];

                // Variablen erzeugen
                for (int k = 0; k < K; k++)
                {
                    y[k] = model.AddVar(0, 1, 0, GRB.BINARY, $"y_{k}");
                    
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i != j)
                            {
                                x[k, i, j] = model.AddVar(0, 1, Instance.CostMatrix[i, j], GRB.BINARY, $"x_{k}_{i}_{j}");
                            }
                        }
                    }
                }

                // Zielfunktion: Minimierung der Gesamtreisekosten
                GRBLinExpr objective = 0;
                for (int k = 0; k < K; k++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i != j)
                            {
                                objective.AddTerm(Instance.CostMatrix[i, j], x[k, i, j]);
                            }
                        }
                    }
                }
                model.SetObjective(objective, GRB.MINIMIZE);

                // Nebenbedingungen

                // 1. Jeder Kunde muss genau einmal besucht werden
                for (int j = 1; j < n; j++)  // Depot überspringen
                {
                    GRBLinExpr sum = 0;
                    for (int k = 0; k < K; k++)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            if (i != j)
                            {
                                sum.AddTerm(1, x[k, i, j]);
                            }
                        }
                    }
                    model.AddConstr(sum == 1, $"visit_customer_{j}");
                }

                // 2. Flusserhaltung: Für jedes Fahrzeug und jeden Knoten
                for (int k = 0; k < K; k++)
                {
                    for (int h = 0; h < n; h++)
                    {
                        GRBLinExpr inflow = 0;
                        GRBLinExpr outflow = 0;
                        
                        for (int i = 0; i < n; i++)
                        {
                            if (i != h)
                            {
                                inflow.AddTerm(1, x[k, i, h]);
                            }
                        }
                        
                        for (int j = 0; j < n; j++)
                        {
                            if (h != j)
                            {
                                outflow.AddTerm(1, x[k, h, j]);
                            }
                        }
                        
                        model.AddConstr(inflow == outflow, $"flow_{k}_{h}");
                    }
                }

                // 3. Jedes Fahrzeug muss am Depot starten und enden
                for (int k = 0; k < K; k++)
                {
                    GRBLinExpr depotOutflow = 0;
                    GRBLinExpr depotInflow = 0;
                    
                    for (int j = 1; j < n; j++)  // Vom Depot zu den Kunden
                    {
                        depotOutflow.AddTerm(1, x[k, depot, j]);
                    }
                    
                    for (int i = 1; i < n; i++)  // Von Kunden zum Depot
                    {
                        depotInflow.AddTerm(1, x[k, i, depot]);
                    }
                    
                    model.AddConstr(depotOutflow == y[k], $"depot_start_{k}");
                    model.AddConstr(depotInflow == y[k], $"depot_end_{k}");
                }

                // 4. Kapazitätsrestriktionen (mit MTZ-Formulierung)
                // u[i] = Position von Knoten i in der Tour
                var u = new GRBVar[n];
                for (int i = 0; i < n; i++)
                {
                    u[i] = model.AddVar(0, n, 0, GRB.CONTINUOUS, $"u_{i}");
                }

                // Depot ist immer an Position 0
                model.AddConstr(u[depot] == 0, "depot_position");

                // MTZ-Bedingungen: u[j] >= u[i] + 1 - n(1 - x[k,i,j])
                for (int k = 0; k < K; k++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 1; j < n; j++)  // j != depot
                        {
                            if (i != j)
                            {
                                model.AddConstr(u[j] >= u[i] + 1 - n * (1 - x[k, i, j]), $"mtz_{k}_{i}_{j}");
                            }
                        }
                    }
                }

                // Kapazitätsrestriktionen: Summe der Nachfragen auf der Route <= Fahrzeugkapazität
                for (int k = 0; k < K; k++)
                {
                    GRBLinExpr routeDemand = 0;
                    for (int i = 1; i < n; i++)  // Depot überspringen
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i != j)
                            {
                                routeDemand.AddTerm(Instance.Demands[i], x[k, i, j]);
                            }
                        }
                    }
                    model.AddConstr(routeDemand <= Instance.VehicleCapacity * y[k], $"capacity_{k}");
                }

                // Setze Solver-Parameter
                model.Set(GRB.DoubleParam.TimeLimit, TimeLimit);  // Zeitlimit
                model.Set(GRB.DoubleParam.MIPGap, 0.01);    // 1% Optimalitätslücke
                model.Set(GRB.IntParam.Threads, 0);         // Alle verfügbaren Threads nutzen
                model.Set(GRB.IntParam.LogToConsole, 0);    // Keine Ausgabe in der Konsole

                // Löse das Modell
                model.Optimize();

                stopwatch.Stop();
                double solutionTime = stopwatch.ElapsedMilliseconds / 1000.0;

                // Extrahiere Lösung
                double[,] xSol = new double[K, n];
                double[] ySol = new double[K];
                double totalCost = 0;

                if (model.Status == GRB.Status.OPTIMAL || model.Status == GRB.Status.TIME_LIMIT)
                {
                    // Extrahiere y-Werte
                    for (int k = 0; k < K; k++)
                    {
                        ySol[k] = y[k].X;
                    }

                    // Extrahiere x-Werte und berechne Gesamtkosten
                    for (int k = 0; k < K; k++)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            for (int j = 0; j < n; j++)
                            {
                                if (i != j)
                                {
                                    double value = x[k, i, j].X;
                                    xSol[k, i] += value;  // Summe aller ausgehenden Kanten von Knoten i für Fahrzeug k
                                    totalCost += value * Instance.CostMatrix[i, j];
                                }
                            }
                        }
                    }

                    return new CVRPSolution(
                        Instance.Name,
                        "Gurobi",
                        totalCost,
                        solutionTime,
                        xSol,
                        ySol
                    );
                }
                else
                {
                    // Falls keine Lösung gefunden wurde, gib leere Lösung zurück
                    return new CVRPSolution(
                        Instance.Name,
                        "Gurobi",
                        0,
                        solutionTime,
                        new double[K, n],
                        new double[K]
                    );
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Fehler im Gurobi-Solver: {ex.Message}");
                
                return new CVRPSolution(
                    Instance.Name,
                    "Gurobi",
                    0,
                    stopwatch.ElapsedMilliseconds / 1000.0,
                    new double[Instance.VehicleCount, Instance.NodeCount],
                    new double[Instance.VehicleCount]
                );
            }
        }
    }
}
