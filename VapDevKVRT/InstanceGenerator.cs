using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapDevKVRT
{
    public class InstanceGenerator
    {
        public InstanceGenerator()
        {
            // Constructor logic can be added here if needed
        }

        public void GenerateInstances(int numberOfInstances, int numberOfVehicles, int numberOfDemandLocations)
        {
            Random random = new Random(69);

            for (int k = 0; k < numberOfInstances; k++)
            {
                int a = numberOfVehicles;
                int n = numberOfDemandLocations;

                string name = $"{k}-{a}-{n}";
                int q = 200; // maximum capacity of each vehicle
                double[] d = new double[n]; // demand at each location
                List<(double x, double y)> coordinatesCustomers = new List<(double x, double y)>(); // coordinates of customers

                var warehouse = (
                   x: random.NextDouble() * 1000,
                   y: random.NextDouble() * 1000
               );

                for (int i = 0; i < n; i++)
                {
                    d[i] = random.Next(10, 30); // Random demand between 10 and 30
                    coordinatesCustomers.Add((random.Next(0, 1001), random.Next(0, 1001))); // Random coordinates between 0 and 10000
                }

                double totalCapacity = n * q; // total capacity of all vehicles
                double totalDemand = d.Sum(); // total demand of all locations

                if (totalDemand > totalCapacity)
                {
                    Console.WriteLine($"Instance {name} has total demand {totalDemand} which exceeds the total capacity {totalCapacity}. Skipping instance generation.");
                    double increment = Math.Ceiling(totalDemand - totalCapacity) / q; // Calculate increment to adjust demand
                    a += (int)increment;
                }

                CVRPInstance instance = new CVRPInstance(name, a, n, d, warehouse, coordinatesCustomers);
                instance.WriteToFile();
            }
        }
    }
}
