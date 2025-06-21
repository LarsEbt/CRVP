using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CVRP
{
    public class CVRPInstance
    {
        public string Name { get; set; }
        public int NodeCount { get; set; }
        public int VehicleCount { get; set; }
        public double VehicleCapacity { get; set; }
        public double[] Demands { get; set; }
        public double[,] CostMatrix { get; set; }
        public List<(double x, double y)> Coordinates { get; set; }

        public CVRPInstance(
            string name,
            int nodeCount,
            int vehicleCount,
            double vehicleCapacity,
            double[] demands,
            double[,] costMatrix,
            List<(double x, double y)> coordinates)
        {
            Name = name;
            NodeCount = nodeCount;
            VehicleCount = vehicleCount;
            VehicleCapacity = vehicleCapacity;
            Demands = demands;
            CostMatrix = costMatrix;
            Coordinates = coordinates;
        }

        public void WriteToFile()
        {
            var path = @$"..\..\..\Instances\{Name}.txt";
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine($"Name\n{Name}\n");
                writer.WriteLine($"nodeCount\n{NodeCount}\n");
                writer.WriteLine($"vehicleCount\n{VehicleCount}\n");
                writer.WriteLine($"vehicleCapacity\n{VehicleCapacity}\n");

                writer.WriteLine("demands");
                for (int i = 0; i < NodeCount; i++)
                    writer.Write($"{Demands[i]} ");

                writer.WriteLine("\n\nc");
                for (int i = 0; i < NodeCount; i++)
                {
                    for (int j = 0; j < NodeCount; j++)
                        writer.Write($"{CostMatrix[i, j]} ");
                    writer.WriteLine();
                }

                writer.WriteLine("\nCoordinates");
                writer.WriteLine("x y");
                for (int i = 0; i < NodeCount; i++)
                    writer.WriteLine($"{Coordinates[i].x} {Coordinates[i].y}");
            }
        }

        public static CVRPInstance ReadFromFile(string instanceName)
        {
            var lines = File.ReadAllLines(@$"..\..\..\Instances\{instanceName}.txt").ToList();

            string name = lines[1];
            int nodeCount = int.Parse(lines[lines.IndexOf("nodeCount") + 1]);
            int vehicleCount = int.Parse(lines[lines.IndexOf("vehicleCount") + 1]);
            double vehicleCapacity = double.Parse(lines[lines.IndexOf("vehicleCapacity") + 1]);

            double[] demands = lines[lines.IndexOf("demands") + 1]
                .Trim()
                .Split(' ')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(double.Parse)
                .ToArray();

            var costMatrix = new double[nodeCount, nodeCount];
            int cStart = lines.IndexOf("c") + 1;
            for (int i = 0; i < nodeCount; i++)
            {
                var parts = lines[cStart + i]
                    .Trim()
                    .Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToArray();
                for (int j = 0; j < nodeCount; j++)
                    costMatrix[i, j] = double.Parse(parts[j]);
            }

            var coordinates = new List<(double x, double y)>();
            int coordStart = lines.IndexOf("Coordinates") + 2;
            for (int i = 0; i < nodeCount; i++)
            {
                var parts = lines[coordStart + i]
                    .Trim()
                    .Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToArray();
                coordinates.Add((double.Parse(parts[0]), double.Parse(parts[1])));
            }

            return new CVRPInstance(
                name,
                nodeCount,
                vehicleCount,
                vehicleCapacity,
                demands,
                costMatrix,
                coordinates
            );
        }

        private static double CalculateDistance((double x, double y) p1, (double x, double y) p2)
        {
            double dx = p1.x - p2.x;
            double dy = p1.y - p2.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Ein kleines Beispiel mit 6 Knoten (Depot + 5 Kunden), 3 Fahrzeugen und Kapazität 100.
        /// </summary>
        public static CVRPInstance Example()
        {
            string name = "CVRPExample";
            int nodeCount = 6;
            int vehicleCount = 3;
            double vehicleCapacity = 100;

            // Nachfrage: Knoten 0 = Depot (0), dann Kunden
            double[] demands = new double[] { 0, 10, 15, 20, 25, 30 };

            // Koordinaten: (x,y) für Depot und Kunden
            var coords = new List<(double x, double y)>()
            {
                (0.0,  0.0),   // Depot
                (10.0, 0.0),
                (0.0,  10.0),
                (10.0, 10.0),
                (20.0, 10.0),
                (10.0, 20.0)
            };

            // Distanzmatrix berechnen
            var c = new double[nodeCount, nodeCount];
            for (int i = 0; i < nodeCount; i++)
                for (int j = 0; j < nodeCount; j++)
                    c[i, j] = CalculateDistance(coords[i], coords[j]);

            return new CVRPInstance(
                name,
                nodeCount,
                vehicleCount,
                vehicleCapacity,
                demands,
                c,
                coords
            );
        }
    }
}