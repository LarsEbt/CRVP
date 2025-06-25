using System;
using System.Collections.Generic;
using System.Diagnostics;
using VapDevKVRT;
using System.Linq;

namespace CVRP
{
    /// <summary>
    /// Einfache Nearest‐Neighbour‐Heuristik für das CVRP.
    /// Fahrzeuge starten im Depot (0) und besuchen wiederholt den nächsten 
    /// noch offenen Kunden, solange die Restkapazität reicht.
    /// </summary>
    public class NearestNeighbourSolver : ISolver
    {
        public CVRPInstance Instance { get; set; }

        public NearestNeighbourSolver(CVRPInstance instance)
        {
            Instance = instance;
        }

        public CVRPSolution Solve()
        {
            int n = Instance.NodeCount;              
            int K = Instance.VehicleCount;           
            double Q = Instance.VehicleCapacity;     
            double[,] c = Instance.CostMatrix;       
            // Kopiere Nachfragen, damit Instance nicht geändert wird
            double[] demandsCopy = new double[n];
            Array.Copy(Instance.Demands, demandsCopy, n);

            var xSol = new double[K, n]; 
            var ySol = new double[K];    
            double travelCosts = 0.0;

            var sw = Stopwatch.StartNew();

            // Für jedes Fahrzeug k
            for (int k = 0; k < K; k++)
            {
                double remainingCap = Q;
                int currentNode = 0; // starte immer im Depot
                var route = new List<int>();

                while (true)
                {
                    // Finde nächsten Kunden (Knoten > 0), dessen Nachfrage ≤ remainingCap
                    int next = -1;
                    //bestDist ist die kleinste Distanz zu einem noch offenen Kunden
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

                    // Fahre zu nächstem Kunden (next)
                    travelCosts += bestDist;
                    remainingCap -= demandsCopy[next];
                    demandsCopy[next] = 0; // Kunde wurde bedient
                    xSol[k, next] = 1; // Kunde wurde besucht
                    route.Add(next); // Kunde wurde zur Route hinzugefügt
                    currentNode = next; // Fahrzeug fährt zum nächsten Kunden
                }

                // Wenn die Route nicht leer ist, fahre zum Depot
                if (route.Count > 0)
                {
                    ySol[k] = 1; // Fahrzeug wurde genutzt
                    travelCosts += c[currentNode, 0]; // Rückkehr zum Depot
                }

                // Optional: Wenn alle Kunden bedient sind, kann abgebrochen werden
                if (demandsCopy.Sum() == 0)
                    break;
            }

            sw.Stop();
            double solutionTime = sw.Elapsed.TotalSeconds;

            return new CVRPSolution(
                Instance.Name,
                "NearestNeighbour",
                travelCosts,
                solutionTime,
                xSol,
                ySol
            );
        }
    }
}