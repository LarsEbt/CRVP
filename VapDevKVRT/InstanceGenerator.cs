using System;
using System.Collections.Generic;
using System.Linq;
using VapDevKVRT;

namespace CVRP
{
    public class CVRPInstanceGenerator
    {
        public void GenerateInstances(int numberOfInstances, int numberOfVehicles, int numberOfCustomers, double vehicleCapacity)
        {
            var rand = new Random(69);
            double capacityBuffer = 0.10; // 10% Puffer auf die benötigte Kapazität

            for (int k = 0; k < numberOfInstances; k++)
            {
                int A = numberOfVehicles;
                int n = numberOfCustomers;
                double Q = vehicleCapacity;

                // Name: z.B. "CVRP-A-n-k"
                string name = $"CVRP-{A}-{n}-{k}";

                // Koordinaten: Index 0 = Depot, 1..n = Kunden
                var coords = new List<(double x, double y)>();
                // Bedarfe (d): d[0]=0 fürs Depot
                double[] d = new double[n + 1];
                // Distanzmatrix (n+1)×(n+1)
                double[,] c = new double[n + 1, n + 1];

                // Depot zufällig im Quadrat [0,800]×[0,800]
                coords.Add((rand.Next(0, 1001), rand.Next(0, 1001)));

                // Generiere Kundennachfrage und Kundenkoordinaten
                for (int i = 1; i <= n; i++)
                {
                    d[i] = rand.Next(1, 11) * 50;                       // Nachfrage in {50,100,…,500}
                    coords.Add((rand.Next(0, 1001), rand.Next(0, 1001)));
                }

                // Fülle Distanzmatrix mit euklidischen Distanzen
                for (int i = 0; i <= n; i++)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        c[i, j] = CalculateDistance(coords[i], coords[j]);
                    }
                }

                // Prüfe, ob Gesamtbedarf in A Fahrzeuge mit Kapazität Q passt
                double totalDemand = d.Sum();
                if (totalDemand > A * Q * (1 + capacityBuffer))
                {
                    double newQ = Math.Ceiling(totalDemand / (A * (1 - capacityBuffer)));
                    Console.WriteLine($"Instance {name}: Gesamtbedarf {totalDemand} > A·Q·(1+{capacityBuffer}) → erhöhe Q auf {newQ}");
                    Q = newQ;
                }
                else
                {
                    Console.WriteLine($"Instance {name}: Gesamtbedarf {totalDemand} ≤ A·Q·(1+{capacityBuffer}) → Q bleibt bei {Q}");
                }

                // Instance erstellen und in Datei schreiben
                var instance = new CVRPInstance(
                    name: name,
                    nodeCount: n + 1,
                    vehicleCount: A,
                    vehicleCapacity: Q,
                    demands: d,
                    costMatrix: c,
                    coordinates: coords
                );
                instance.WriteToFile();
            }
        }

        private static double CalculateDistance((double x, double y) p1, (double x, double y) p2)
        {
            double dx = p1.x - p2.x;
            double dy = p1.y - p2.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}