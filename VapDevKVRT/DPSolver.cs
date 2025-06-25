using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VapDevKVRT;

namespace CVRP
{
    /// <summary>
    /// Dynamic Programming solver for the Capacitated Vehicle Routing Problem (CVRP).
    /// This solver uses a state-space approach where each state represents:
    /// - Current node (depot or customer)
    /// - Remaining capacity
    /// - Set of unvisited customers
    /// 
    /// The DP table stores the minimum cost to reach each state.
    /// This approach is suitable for small instances due to exponential state space.
    /// </summary>
    public class DPSolver : ISolver
    {
        public CVRPInstance Instance { get; set; }

        public DPSolver(CVRPInstance instance)
        {
            Instance = instance;
        }

        public CVRPSolution Solve()
        {
            var sw = Stopwatch.StartNew();

            int n = Instance.NodeCount;
            int K = Instance.VehicleCount;
            double Q = Instance.VehicleCapacity;
            double[,] c = Instance.CostMatrix;
            double[] d = Instance.Demands;

            // Check if instance is too large for DP (exponential complexity)
            if (n > 15)
            {
                throw new InvalidOperationException($"DP solver cannot handle instances with more than 15 nodes. Current: {n}");
            }

            // Create customer nodes (excluding depot)
            var customers = Enumerable.Range(1, n - 1).ToList();

            // Initialize DP table: [node][capacity][customer_set]
            var dp = new Dictionary<string, double>();
            var parent = new Dictionary<string, (int node, int capacity, int customerSet)>();

            // Base case: start at depot with full capacity and all customers unvisited
            string initialState = CreateState(0, (int)Q, (1 << (n - 1)) - 1);
            dp[initialState] = 0;

            // Process all states
            var queue = new Queue<string>();
            queue.Enqueue(initialState);

            while (queue.Count > 0)
            {
                string currentState = queue.Dequeue();
                var (currentNode, currentCapacity, customerSet) = ParseState(currentState);
                double currentCost = dp[currentState];

                // If we're at depot and no customers left, we're done
                if (currentNode == 0 && customerSet == 0)
                    continue;

                // If we're at depot, we can start a new route
                if (currentNode == 0)
                {
                    // Try visiting each unvisited customer
                    for (int i = 0; i < customers.Count; i++)
                    {
                        int customer = customers[i];
                        if ((customerSet & (1 << i)) != 0 && d[customer] <= currentCapacity)
                        {
                            int newCustomerSet = customerSet & ~(1 << i);
                            int newCapacity = (int)(currentCapacity - d[customer]);
                            double newCost = currentCost + c[currentNode, customer];

                            string newState = CreateState(customer, newCapacity, newCustomerSet);

                            if (!dp.ContainsKey(newState) || newCost < dp[newState])
                            {
                                dp[newState] = newCost;
                                parent[newState] = (currentNode, currentCapacity, customerSet);
                                queue.Enqueue(newState);
                            }
                        }
                    }
                }
                else
                {
                    // We're at a customer, can go to depot or other customers

                    // Option 1: Return to depot
                    if (currentCapacity >= 0)
                    {
                        double depotCost = currentCost + c[currentNode, 0];
                        string depotState = CreateState(0, (int)Q, customerSet);

                        if (!dp.ContainsKey(depotState) || depotCost < dp[depotState])
                        {
                            dp[depotState] = depotCost;
                            parent[depotState] = (currentNode, currentCapacity, customerSet);
                            queue.Enqueue(depotState);
                        }
                    }

                    // Option 2: Visit another customer
                    for (int i = 0; i < customers.Count; i++)
                    {
                        int customer = customers[i];
                        if ((customerSet & (1 << i)) != 0 && d[customer] <= currentCapacity)
                        {
                            int newCustomerSet = customerSet & ~(1 << i);
                            int newCapacity = (int)(currentCapacity - d[customer]);
                            double newCost = currentCost + c[currentNode, customer];

                            string newState = CreateState(customer, newCapacity, newCustomerSet);

                            if (!dp.ContainsKey(newState) || newCost < dp[newState])
                            {
                                dp[newState] = newCost;
                                parent[newState] = (currentNode, currentCapacity, customerSet);
                                queue.Enqueue(newState);
                            }
                        }
                    }
                }
            }

            // Find optimal solution
            double minCost = double.MaxValue;
            string bestState = "";

            foreach (var kvp in dp)
            {
                var (node, capacity, customerSet) = ParseState(kvp.Key);
                if (node == 0 && customerSet == 0 && kvp.Value < minCost)
                {
                    minCost = kvp.Value;
                    bestState = kvp.Key;
                }
            }

            if (minCost == double.MaxValue)
            {
                throw new InvalidOperationException("No feasible solution found");
            }

            // Reconstruct solution
            var (xSol, ySol) = ReconstructSolution(parent, bestState, n, K, c, d);

            sw.Stop();
            double solutionTime = sw.Elapsed.TotalSeconds;

            return new CVRPSolution(
                Instance.Name,
                "DP",
                minCost,
                solutionTime,
                xSol,
                ySol
            );
        }

        private string CreateState(int node, int capacity, int customerSet)
        {
            return $"{node},{capacity},{customerSet}";
        }

        private (int node, int capacity, int customerSet) ParseState(string state)
        {
            var parts = state.Split(',');
            return (int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }

        private (double[,] xSol, double[] ySol) ReconstructSolution(
            Dictionary<string, (int node, int capacity, int customerSet)> parent,
            string finalState,
            int n,
            int K,
            double[,] c,
            double[] d)
        {
            var xSol = new double[K, n];
            var ySol = new double[K];
            var routes = new List<List<int>>();
            var currentRoute = new List<int>();

            string currentState = finalState;
            var path = new List<(int node, int capacity, int customerSet)>();

            // Reconstruct path
            while (parent.ContainsKey(currentState))
            {
                var (node, capacity, customerSet) = ParseState(currentState);
                path.Add((node, capacity, customerSet));
                currentState = CreateState(parent[currentState].node, parent[currentState].capacity, parent[currentState].customerSet);
            }

            path.Reverse();

            // Build routes from path
            for (int i = 0; i < path.Count; i++)
            {
                var (node, capacity, customerSet) = path[i];

                if (node == 0 && i > 0)
                {
                    // End of route, start new one
                    if (currentRoute.Count > 0)
                    {
                        routes.Add(new List<int>(currentRoute));
                        currentRoute.Clear();
                    }
                }
                else if (node > 0)
                {
                    currentRoute.Add(node);
                }
            }

            // Add last route if not empty
            if (currentRoute.Count > 0)
            {
                routes.Add(currentRoute);
            }

            // Convert routes to solution format
            for (int k = 0; k < Math.Min(routes.Count, K); k++)
            {
                if (routes[k].Count > 0)
                {
                    ySol[k] = 1;
                    foreach (int customer in routes[k])
                    {
                        xSol[k, customer] = 1;
                    }
                }
            }

            return (xSol, ySol);
        }
    }
}