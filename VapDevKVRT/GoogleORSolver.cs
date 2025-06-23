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

        public GoogleORSolver(CVRPInstance instance, double timeLimit)
        {
            Instance = instance;
            TimeLimit = timeLimit;
        }

        public CVRPSolution Solve()
        {
            Console.WriteLine($"Starting Google OR-Tools solver for instance: {Instance.Name}");            
            int n = Instance.NodeCount; // including depot as node 0
            int K = Instance.VehicleCount;
            double Q = Instance.VehicleCapacity;
            double[,] c = Instance.CostMatrix;
            double[] d = Instance.Demands;

            var sw = Stopwatch.StartNew();

            // OR-Tools requires integer costs and demands
            long[,] costMatrix = new long[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    costMatrix[i, j] = (long)Math.Round(c[i, j]);
            long[] demands = d.Select(x => (long)Math.Round(x)).ToArray();
            long vehicleCapacity = (long)Math.Round(Q);

            // Create Routing Index Manager
            var manager = new RoutingIndexManager(n, K, 0); // depot is 0
            var routing = new RoutingModel(manager);

            // Cost callback
            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                int fromNode = manager.IndexToNode(fromIndex);
                int toNode = manager.IndexToNode(toIndex);
                return costMatrix[fromNode, toNode];
            });
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Add capacity constraint
            int demandCallbackIndex = routing.RegisterUnaryTransitCallback((long fromIndex) =>
            {
                int fromNode = manager.IndexToNode(fromIndex);
                return demands[fromNode];
            });
            routing.AddDimensionWithVehicleCapacity(
                demandCallbackIndex,
                0, // null capacity slack
                Enumerable.Repeat(vehicleCapacity, K).ToArray(), // vehicle capacities
                true, // start cumul to zero
                "Capacity");

            // Setting first solution heuristic
            var searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Google.Protobuf.WellKnownTypes.Duration { Seconds = (long)Math.Round(TimeLimit) };

            // Solve
            var solution = routing.SolveWithParameters(searchParameters);

            // Prepare solution arrays
            var xSol = new double[K, n];
            var ySol = new double[K];
            double travelCosts = 0.0;

            if (solution != null)
            {   
                for (int k = 0; k < K; k++)
                {
                    long index = routing.Start(k);
                    bool used = false;
                    var route = new List<int>();
                    
                    while (!routing.IsEnd(index))
                    {
                        long nextIndex = solution.Value(routing.NextVar(index));
                        int fromNode = manager.IndexToNode(index);
                        int toNode = manager.IndexToNode(nextIndex);
                        
                        if (fromNode != 0 && toNode != 0)
                        {
                            xSol[k, toNode] = 1;
                        }
                        if (toNode != 0)
                        {
                            used = true;
                            route.Add(toNode);
                        }
                        if (fromNode != toNode)
                            travelCosts += c[fromNode, toNode];
                        index = nextIndex;
                    }
                    
                    if (used)
                    {
                        ySol[k] = 1;
                    }
                }
                
                Console.WriteLine($"Total travel cost: {travelCosts:F2}");
                Console.WriteLine($"Vehicles used: {ySol.Sum():F0}");
            }
            else
            {
                Console.WriteLine("No solution found within time limit!");
            }

            sw.Stop();
            double solutionTime = sw.Elapsed.TotalSeconds;
            
            Console.WriteLine($"Solving completed in {solutionTime:F2} seconds");

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
