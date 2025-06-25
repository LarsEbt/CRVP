using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VapDevKVRT;

namespace CVRP
{
   
    public class RandomNextSolver : ISolver
    {
        public CVRPInstance Instance { get; set; }
        private Random random;

        public RandomNextSolver(CVRPInstance instance)
        {
            Instance = instance;
            random = new Random();
        }

        public CVRPSolution Solve()
        {
            int n = Instance.NodeCount;              
            int K = Instance.VehicleCount;           
            double Q = Instance.VehicleCapacity;     
            double[,] c = Instance.CostMatrix;       
            // Copy demands so we don't modify the original instance
            double[] d = new double[n];
            Array.Copy(Instance.Demands, d, n);

            // Initialize solutions
            var xSol = new double[K, n]; 
            var ySol = new double[K];    
            double travelCosts = 0.0;

            var sw = Stopwatch.StartNew();

            // For each vehicle k
            for (int k = 0; k < K; k++)
            {
                double remainingCap = Q;
                int current = 0;          // always start at depot
                var route = new List<int>();

                while (true)
                {
                    // Find all customers that can be served with remaining capacity
                    var availableCustomers = new List<int>();
                    for (int i = 1; i < n; i++)
                    {
                        if (d[i] > 0 && d[i] <= remainingCap)
                        {
                            availableCustomers.Add(i);
                        }
                    }

                    if (availableCustomers.Count == 0)
                        break;  // no more customers can be served

                    // Choose a random customer from available ones
                    int randomIndex = random.Next(availableCustomers.Count);
                    int next = availableCustomers[randomIndex];

                    // Travel to next customer
                    travelCosts += c[current, next];
                    remainingCap -= d[next];
                    d[next] = 0;               // customer served
                    xSol[k, next] = 1;
                    route.Add(next);
                    current = next;
                }

                if (route.Count > 0)
                {
                    // Vehicle k was used
                    ySol[k] = 1;
                    // Return to depot
                    travelCosts += c[current, 0];
                }
            }

            sw.Stop();
            double solutionTime = sw.Elapsed.TotalSeconds;

            return new CVRPSolution(
                Instance.Name,
                "RandomNext",
                travelCosts,
                solutionTime,
                xSol,
                ySol
            );
        }
    }
}
