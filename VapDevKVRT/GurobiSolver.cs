using CVRP;
using Gurobi;
using System;
using System.Diagnostics;

namespace VapDevKVRT
{
    public class GurobiSolver : ISolver
    {
        public CVRPInstance Instance { get; set; }
        public double TimeLimit { get; set; } = 300;

        public GurobiSolver(CVRPInstance instance)
        {
            Instance = instance;
        }

        public GurobiSolver(CVRPInstance instance, double timeLimit)
        {
            Instance = instance;
            TimeLimit = timeLimit;
        }

        public CVRPSolution Solve()
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Create Gurobi environment and model
                using var env = new GRBEnv();
                using var model = new GRBModel(env);

                int n = Instance.NodeCount;  // Number of nodes (including depot)
                int K = Instance.VehicleCount;  // Number of vehicles
                int depot = 0;  // Depot is always node 0

                // Decision variables
                // x[k,i,j] = 1 if vehicle k travels from node i to node j, 0 otherwise
                var x = new GRBVar[K, n, n];
                // y[k] = 1 if vehicle k is used, 0 otherwise
                var y = new GRBVar[K];

                // Create variables
                for (int k = 0; k < K; k++)
                {
                    y[k] = model.AddVar(0, 1, 0, GRB.BINARY, $"y_{k}");
                    
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i != j)
                            {
                                x[k, i, j] = model.AddVar(0, 1, Instance.CostMatrix[i, j], GRB.BINARY, $"x_{k}_{i}_{j}");
                            }
                        }
                    }
                }

                // Objective: Minimize total travel cost
                GRBLinExpr objective = 0;
                for (int k = 0; k < K; k++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i != j)
                            {
                                objective.AddTerm(Instance.CostMatrix[i, j], x[k, i, j]);
                            }
                        }
                    }
                }
                model.SetObjective(objective, GRB.MINIMIZE);

                // Constraints

                // 1. Each customer must be visited exactly once
                for (int j = 1; j < n; j++)  // Skip depot
                {
                    GRBLinExpr sum = 0;
                    for (int k = 0; k < K; k++)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            if (i != j)
                            {
                                sum.AddTerm(1, x[k, i, j]);
                            }
                        }
                    }
                    model.AddConstr(sum == 1, $"visit_customer_{j}");
                }

                // 2. Flow conservation: for each vehicle and each node
                for (int k = 0; k < K; k++)
                {
                    for (int h = 0; h < n; h++)
                    {
                        GRBLinExpr inflow = 0;
                        GRBLinExpr outflow = 0;
                        
                        for (int i = 0; i < n; i++)
                        {
                            if (i != h)
                            {
                                inflow.AddTerm(1, x[k, i, h]);
                            }
                        }
                        
                        for (int j = 0; j < n; j++)
                        {
                            if (h != j)
                            {
                                outflow.AddTerm(1, x[k, h, j]);
                            }
                        }
                        
                        model.AddConstr(inflow == outflow, $"flow_{k}_{h}");
                    }
                }

                // 3. Each vehicle must start and end at the depot
                for (int k = 0; k < K; k++)
                {
                    GRBLinExpr depotOutflow = 0;
                    GRBLinExpr depotInflow = 0;
                    
                    for (int j = 1; j < n; j++)  // From depot to customers
                    {
                        depotOutflow.AddTerm(1, x[k, depot, j]);
                    }
                    
                    for (int i = 1; i < n; i++)  // From customers to depot
                    {
                        depotInflow.AddTerm(1, x[k, i, depot]);
                    }
                    
                    model.AddConstr(depotOutflow == y[k], $"depot_start_{k}");
                    model.AddConstr(depotInflow == y[k], $"depot_end_{k}");
                }

                // 4. Capacity constraints using MTZ formulation
                // u[i] = position of node i in the tour
                var u = new GRBVar[n];
                for (int i = 0; i < n; i++)
                {
                    u[i] = model.AddVar(0, n, 0, GRB.CONTINUOUS, $"u_{i}");
                }

                // u[depot] = 0
                model.AddConstr(u[depot] == 0, "depot_position");

                // MTZ constraints: u[j] >= u[i] + 1 - n(1 - x[k,i,j])
                for (int k = 0; k < K; k++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 1; j < n; j++)  // j != depot
                        {
                            if (i != j)
                            {
                                model.AddConstr(u[j] >= u[i] + 1 - n * (1 - x[k, i, j]), $"mtz_{k}_{i}_{j}");
                            }
                        }
                    }
                }

                // Capacity constraints: sum of demands on the route <= vehicle capacity
                for (int k = 0; k < K; k++)
                {
                    GRBLinExpr routeDemand = 0;
                    for (int i = 1; i < n; i++)  // Skip depot
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i != j)
                            {
                                routeDemand.AddTerm(Instance.Demands[i], x[k, i, j]);
                            }
                        }
                    }
                    model.AddConstr(routeDemand <= Instance.VehicleCapacity * y[k], $"capacity_{k}");
                }

                // 5. Subtour elimination constraints (optional, MTZ should handle this)
                // But we can add some additional constraints to strengthen the formulation

                // Set solver parameters
                model.Set(GRB.DoubleParam.TimeLimit, TimeLimit);  // Use configurable time limit
                model.Set(GRB.DoubleParam.MIPGap, 0.01);    // 1% optimality gap
                model.Set(GRB.IntParam.Threads, 0);         // Use all available threads

                // Solve the model
                model.Optimize();

                stopwatch.Stop();
                double solutionTime = stopwatch.ElapsedMilliseconds / 1000.0;

                // Extract solution
                double[,] xSol = new double[K, n];
                double[] ySol = new double[K];
                double totalCost = 0;

                if (model.Status == GRB.Status.OPTIMAL || model.Status == GRB.Status.TIME_LIMIT)
                {
                    // Extract y values
                    for (int k = 0; k < K; k++)
                    {
                        ySol[k] = y[k].X;
                    }

                    // Extract x values and calculate total cost
                    for (int k = 0; k < K; k++)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            for (int j = 0; j < n; j++)
                            {
                                if (i != j)
                                {
                                    double value = x[k, i, j].X;
                                    xSol[k, i] += value;  // Sum of all outgoing arcs from node i for vehicle k
                                    totalCost += value * Instance.CostMatrix[i, j];
                                }
                            }
                        }
                    }

                    return new CVRPSolution(
                        Instance.Name,
                        "Gurobi",
                        totalCost,
                        solutionTime,
                        xSol,
                        ySol
                    );
                }
                else
                {
                    // If no solution found, return empty solution
                    return new CVRPSolution(
                        Instance.Name,
                        "Gurobi",
                        0,
                        solutionTime,
                        new double[K, n],
                        new double[K]
                    );
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Error in Gurobi solver: {ex.Message}");
                
                return new CVRPSolution(
                    Instance.Name,
                    "Gurobi",
                    0,
                    stopwatch.ElapsedMilliseconds / 1000.0,
                    new double[Instance.VehicleCount, Instance.NodeCount],
                    new double[Instance.VehicleCount]
                );
            }
        }
    }
}
