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
                // Schreibe Metadaten der Instanz
                writer.WriteLine($"Name\n{Name}\n");
                writer.WriteLine($"nodeCount\n{NodeCount}\n");
                writer.WriteLine($"vehicleCount\n{VehicleCount}\n");
                writer.WriteLine($"vehicleCapacity\n{VehicleCapacity}\n");

                // Schreibe Nachfrage-Array (alle Nachfragen in einer Zeile)
                writer.WriteLine("demands");
                for (int i = 0; i < NodeCount; i++)
                    writer.Write($"{Demands[i]} ");

                // Schreibe Kostenmatrix (jede Zeile entspricht einer Zeile der Matrix)
                writer.WriteLine("\n\nc");
                for (int i = 0; i < NodeCount; i++)
                {
                    for (int j = 0; j < NodeCount; j++)
                        writer.Write($"{CostMatrix[i, j]} ");
                    writer.WriteLine();
                }

                // Schreibe Koordinaten (jede Zeile enthält x- und y-Koordinate eines Knotens)
                writer.WriteLine("\nCoordinates");
                writer.WriteLine("x y");
                for (int i = 0; i < NodeCount; i++)
                    writer.WriteLine($"{Coordinates[i].x} {Coordinates[i].y}");
            }
        }

        public static CVRPInstance ReadFromFile(string instanceName)
        {
            // Lese alle Zeilen der Instanz-Datei
            var lines = File.ReadAllLines(@$"..\..\..\Instances\{instanceName}.txt").ToList();

            // Parse Metadaten
            string name = lines[1];
            int nodeCount = int.Parse(lines[lines.IndexOf("nodeCount") + 1]);
            int vehicleCount = int.Parse(lines[lines.IndexOf("vehicleCount") + 1]);
            double vehicleCapacity = double.Parse(lines[lines.IndexOf("vehicleCapacity") + 1]);

            // Parse Nachfrage-Array: Teile die Zeile in einzelne Zahlen auf
            double[] demands = lines[lines.IndexOf("demands") + 1]
                .Trim()
                .Split(' ')
                .Where(s => !string.IsNullOrWhiteSpace(s))  // Ignoriert leere Strings
                .Select(double.Parse)
                .ToArray();

            // Parse Kostenmatrix: Jede Zeile nach "c" entspricht einer Zeile der Matrix
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

            // Parse Koordinaten: Jede Zeile nach "Coordinates" enthält x- und y-Koordinate
            var coordinates = new List<(double x, double y)>();
            int coordStart = lines.IndexOf("Coordinates") + 2;  // "Coordinates" und "x y" Zeile
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
    }
}