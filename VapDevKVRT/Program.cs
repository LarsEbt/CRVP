using CVRP;
using Google.OrTools.ConstraintSolver;
using VAP;

namespace VapDevKVRT
{

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            RunBeforeGUI();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        public static void RunBeforeGUI()
        {
            // Parameter zentral definieren
            int numberOfInstances = 1;
            int numberOfVehicles = 5;
            int numberOfCustomers = 10;
            int vehicleCapacity = 1000;
            int instanceIndex = 0;
            int timeLimit = 10; 

            // 1) Instanz-Generator ausführen:
            var generator = new CVRPInstanceGenerator();
            generator.GenerateInstances(numberOfInstances,
                                        numberOfVehicles,
                                        numberOfCustomers,
                                        vehicleCapacity);

            // 2) Eine Instanz einlesen
            var instanceFileName = CVRPInstance.ReadFromFile($"CVRP-{numberOfVehicles}-{numberOfCustomers}-{instanceIndex}");


            // --- Nearest Neighbour lösen ---
            ISolver nnSolver = new NearestNeighbourSolver(instanceFileName);
            var nnSolution = nnSolver.Solve();
            nnSolution.WriteToFile();
            Console.WriteLine("NearestNeighbour:   " + nnSolution);
            Console.WriteLine("----------------------------------");

            // --- Improved Nearest Neighbour Heuristic lösen ---
            ISolver innSolver = new ImprovedNearestNeighbourSolver(instanceFileName);
            var innSolution = innSolver.Solve();
            innSolution.WriteToFile();
            Console.WriteLine("Improved NearestNeighbour:   " + innSolution);
            Console.WriteLine("----------------------------------");

            // --- Saving Heuristic lösen ---
            ISolver savingSolver = new SavingHeuristicSolver(instanceFileName);
            var savingSolution = savingSolver.Solve();
            savingSolution.WriteToFile();
            Console.WriteLine("SavingHeuristic:   " + savingSolution);
            Console.WriteLine("----------------------------------");

            //--- Random Next lösen ---
            ISolver randomNextSolver = new RandomNextSolver(instanceFileName);
            var randomNextSolution = randomNextSolver.Solve();
            randomNextSolution.WriteToFile();
            Console.WriteLine("RandomNext:   " + randomNextSolution);
            Console.WriteLine("----------------------------------");

            // --- Gurobi lösen 
            ISolver gurobiSolver = new GurobiSolver(instanceFileName, timeLimit);
            var gurobiSolution = gurobiSolver.Solve();
            gurobiSolution.WriteToFile();
            Console.WriteLine("Gurobi:   " + gurobiSolution);
            Console.WriteLine("----------------------------------");

            //--- Google OR Tools lösen ---
            ISolver googleSolver = new GoogleORSolver(instanceFileName, timeLimit, 1000);
            var googleSolution = googleSolver.Solve();
            googleSolution.WriteToFile();
            Console.WriteLine("Google OR Tools:   " + googleSolution);
            Console.WriteLine("----------------------------------");

            //---Dynamic Programming lösen ---
            if (numberOfCustomers < 15)
            {
                ISolver dpsolver = new DPSolver(instanceFileName);
                CVRPSolution sol = dpsolver.Solve();
                sol.WriteToFile();
                Console.WriteLine("Dynamic Programming:   " + sol);
                Console.WriteLine("----------------------------------");
            }
        }
    }
}