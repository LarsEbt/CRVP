using System;
using System.Collections.Generic;
using System.Diagnostics;
using Google.OrTools.ConstraintSolver;
using CVRP;

namespace VapDevKVRT
{
    public class GoogleORSolver : ISolver
    {
        public CVRPInstance Instance { get; set; }
        private double TimeLimit;
        private int SolutionLimit;

        public GoogleORSolver(CVRPInstance instance, double timeLimit, int solutionLimit)
        {
            Instance = instance;
            TimeLimit = timeLimit;
            SolutionLimit = solutionLimit;
        }

        public CVRPSolution Solve()
        {
            Console.WriteLine($"Starte Google OR-Tools Solver für Instanz: {Instance.Name}");            
            int n = Instance.NodeCount; // Anzahl Knoten inkl. Depot (Knoten 0)
            int K = Instance.VehicleCount; // Anzahl Fahrzeuge
            double Q = Instance.VehicleCapacity; // Fahrzeugkapazität
            double[,] c = Instance.CostMatrix; // Kostenmatrix
            double[] d = Instance.Demands; // Nachfragen

            var sw = Stopwatch.StartNew();

            // OR-Tools benötigt ganzzahlige Kosten und Nachfragen
            long[,] costMatrix = new long[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    costMatrix[i, j] = (long)Math.Round(c[i, j]);
            long[] demands = d.Select(x => (long)Math.Round(x)).ToArray();
            long vehicleCapacity = (long)Math.Round(Q);

            // RoutingIndexManager verwaltet die Zuordnung von Knoten zu Indizes
            var manager = new RoutingIndexManager(n, K, 0); // Depot ist Knoten 0
            var routing = new RoutingModel(manager);

            // Kostenfunktion (Callback): Liefert die Kosten für eine Fahrt von fromNode zu toNode
            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                int fromNode = manager.IndexToNode(fromIndex);
                int toNode = manager.IndexToNode(toIndex);
                return costMatrix[fromNode, toNode];
            });
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Kapazitätsbedingung (Callback): Liefert die Nachfrage eines Knotens
            int demandCallbackIndex = routing.RegisterUnaryTransitCallback((long fromIndex) =>
            {
                int fromNode = manager.IndexToNode(fromIndex);
                return demands[fromNode];
            });
            routing.AddDimensionWithVehicleCapacity(
                demandCallbackIndex,
                0, // kein Kapazitätspuffer
                Enumerable.Repeat(vehicleCapacity, K).ToArray(), // Kapazität für jedes Fahrzeug
                true, // Startwert ist 0
                "Capacity");

            // Suchstrategie und Metaheuristik einstellen
            var searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc; // Startlösung: billigster Pfad
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch; // Verbesserung: Guided Local Search
            
            // TimeLimit setzen
            searchParameters.TimeLimit = new Google.Protobuf.WellKnownTypes.Duration { Seconds = (long)Math.Round(TimeLimit) };

            // Zusätzliche Parameter für früheren Abbruch
            searchParameters.SolutionLimit = SolutionLimit; // Maximal 1000 Lösungen

            Console.WriteLine($"Verwendes Zeitlimit: {TimeLimit:F1} Sekunden");

            // Löse das Problem
            var solution = routing.SolveWithParameters(searchParameters);

            // Ergebnis-Arrays vorbereiten
            var xSol = new double[K, n]; // xSol[k, i] = 1, falls Fahrzeug k Knoten i besucht
            var ySol = new double[K];    // ySol[k] = 1, falls Fahrzeug k genutzt wird
            double travelCosts = 0.0;    // Gesamtkosten

            if (solution != null)
            {   
                // Für jedes Fahrzeug die Route extrahieren
                for (int k = 0; k < K; k++)
                {
                    long index = routing.Start(k); // Startindex für Fahrzeug k
                    bool used = false; // Wurde das Fahrzeug genutzt?
                    var route = new List<int>(); // Liste der besuchten Knoten (außer Depot)
                    int prevNode = manager.IndexToNode(index);
                    while (!routing.IsEnd(index))
                    {
                        long nextIndex = solution.Value(routing.NextVar(index));
                        int fromNode = manager.IndexToNode(index);
                        int toNode = manager.IndexToNode(nextIndex);
                        if (toNode != 0)
                        {
                            used = true;
                            route.Add(toNode);
                        }
                        if (fromNode != toNode)
                            travelCosts += c[fromNode, toNode]; // Kosten aufsummieren
                        index = nextIndex;
                    }
                    if (used)
                    {
                        ySol[k] = 1; // Fahrzeug wurde genutzt
                        // Markiere alle besuchten Knoten für dieses Fahrzeug
                        foreach (var node in route)
                        {
                            if (node != 0) // Depot nicht markieren
                                xSol[k, node] = 1;
                        }
                    }
                }
                Console.WriteLine($"Gesamtkosten: {travelCosts:F2}");
                Console.WriteLine($"Genutzte Fahrzeuge: {ySol.Sum():F0}");
            }
            else
            {
                Console.WriteLine("Keine Lösung innerhalb des Zeitlimits gefunden!");
            }

            sw.Stop();
            double solutionTime = sw.Elapsed.TotalSeconds;
            
            Console.WriteLine($"Lösen abgeschlossen in {solutionTime:F2} Sekunden");

            // Rückgabe der Lösung im CVRPSolution-Format
            return new CVRPSolution(
                Instance.Name,
                "GoogleOR",
                travelCosts,
                solutionTime,
                xSol,
                ySol
            );
        }
    }
}
