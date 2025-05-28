using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapDevKVRT
{
    public class CVRPInstance
    {
        public string Name { get; set; }

        public int NumberOfVehicles { get; set; }

        public int NumberOfDemandLocations { get; set; }

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

        public void WriteToFile()
        {
            // Ensure the directory exists
            string relativePath = @"C:\Users\Lars\Desktop\VAP\Projekt\VapDevKVRT\VapDevKVRT\instances\";
            Directory.CreateDirectory(relativePath);

            // Use a file name based on the instance name
            string filePath = Path.Combine(relativePath, $"{Name}.txt");

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Name:\n{Name}");
                writer.WriteLine($"Number of Vehicles:\n{NumberOfVehicles}");
                writer.WriteLine($"Number of Demand Locations:\n{NumberOfDemandLocations}");
                writer.WriteLine("Demands:\n" + string.Join(", ", d));

                writer.WriteLine("Warehouse Coordinates:");
                writer.WriteLine($"{Warehouse.x}, {Warehouse.y}");
                writer.WriteLine();



                writer.WriteLine("Customer Coordinates:");
                foreach (var coord in CoordinatesCustomers)
                {
                    writer.WriteLine($"{coord.x}, {coord.y}");
                }
            }
        }




    }

    
    
}
