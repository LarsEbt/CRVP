using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VapDevKVRT
{
    public class CVRPInstance
    {
        public string Name { get; set; }

        public int NumberOfVehicles { get; set; }

        public int NumberOfDemandLocations { get; set; }

        public double[,] DistanceMatrix { get; set; }

        public double[] d { get; set; }

        public (double x, double y) Warehouse { get; set; }

        public List<(double x, double y)> CoordinatesCustomers { get; set; }

        public CVRPInstance(string name, int numberOfVehicles, int numberOfDemandLocations, double[] d, (double x, double y) warehouse, List<(double x, double y)> coordinatesCustomers)
        {
            Name = name;
            NumberOfVehicles = numberOfVehicles;
            NumberOfDemandLocations = numberOfDemandLocations;
            this.d = d;
            Warehouse = warehouse;
            CoordinatesCustomers = coordinatesCustomers;
        }

        public void ComputeDistanceMatrix()
        {
            int totalPoints = CoordinatesCustomers.Count + 1;
            DistanceMatrix = new double[totalPoints, totalPoints];

            List<(double x, double y)> allPoints = new List<(double x, double y)> { Warehouse };
            allPoints.AddRange(CoordinatesCustomers);

            for (int i = 0; i < totalPoints; i++)
            {
                for (int j = 0; j < totalPoints; j++)
                {
                    double dx = allPoints[i].x - allPoints[j].x;
                    double dy = allPoints[i].y - allPoints[j].y;
                    DistanceMatrix[i, j] = Math.Sqrt(dx * dx + dy * dy);
                }
            }
        }

        public void WriteToFile()
        {
            string relativePath = @"..\..\..\instances\";
            Directory.CreateDirectory(relativePath);
            string filePath = Path.Combine(relativePath, $"{Name}.txt");

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Name:\n{Name}");
                writer.WriteLine($"Number of Vehicles:\n{NumberOfVehicles}");
                writer.WriteLine($"Number of Demand Locations:\n{NumberOfDemandLocations}");
                writer.WriteLine("Demands:\n" + string.Join(", ", d));

                writer.WriteLine("Warehouse Coordinates:");
                writer.WriteLine($"{Warehouse.x}, {Warehouse.y}");
                writer.WriteLine(); // ✅ Fehler vorher: fehlendes Semikolon

                writer.WriteLine("Customer Coordinates:");
                foreach (var coord in CoordinatesCustomers)
                {
                    writer.WriteLine($"{coord.x}, {coord.y}");
                }

                writer.WriteLine();
                writer.WriteLine("Distance Matrix:");
                for (int i = 0; i < DistanceMatrix.GetLength(0); i++)
                {
                    string line = "";
                    for (int j = 0; j < DistanceMatrix.GetLength(1); j++)
                    {
                        line += DistanceMatrix[i, j].ToString("F2") + (j < DistanceMatrix.GetLength(1) - 1 ? ", " : "");
                    }
                    writer.WriteLine(line);
                }
            }
        }

        public static CVRPInstance ReadFromFile(string fileName)
        {
            string filePath = Path.Combine(@"..\..\..\instances\", $"{fileName}.txt");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Instance file '{filePath}' not found.");
            }

            var lines = File.ReadAllLines(filePath).ToList();

            string name = lines[1].Trim();
            int numberOfVehicles = int.Parse(lines[3].Trim());
            int numberOfDemandLocations = int.Parse(lines[5].Trim());
            double[] d = lines[7].Split(',').Select(double.Parse).ToArray();

            var warehouseCoords = lines[9].Split(',').Select(double.Parse).ToArray();
            (double x, double y) warehouse = (warehouseCoords[0], warehouseCoords[1]);

            List<(double x, double y)> coordinatesCustomers = new List<(double x, double y)>();
            int customerStartLine = 12; 
            for (int i = 0; i < numberOfDemandLocations; i++)
            {
                var coords = lines[customerStartLine + i].Split(',').Select(double.Parse).ToArray();
                coordinatesCustomers.Add((coords[0], coords[1]));
            }


            int matrixStart = lines.FindIndex(l => l.StartsWith("Distance Matrix:")) + 1;
            int matrixSize = numberOfDemandLocations + 1;

            double[,] distanceMatrix = new double[matrixSize, matrixSize];
            for (int i = 0; i < matrixSize; i++)
            {
                var rowValues = lines[matrixStart + i].Split(',').Select(s => double.Parse(s)).ToArray();
                for (int j = 0; j < matrixSize; j++)
                {
                    distanceMatrix[i, j] = rowValues[j];
                }
            }

            CVRPInstance instance = new CVRPInstance(name, numberOfVehicles, numberOfDemandLocations, d, warehouse, coordinatesCustomers);
            instance.DistanceMatrix = distanceMatrix;
            return instance;
        }
    }
}





