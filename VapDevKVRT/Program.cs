using CVRP;
using Google.OrTools.ConstraintSolver;
using System.Security.Cryptography.X509Certificates;
using VAP;

namespace VapDevKVRT
{

    internal static class Program
    {


        [STAThread]
        static void Main()
        {
            RunBeforeGUI();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }


        public static void RunBeforeGUI()
        {
            // Parameter zentral definieren
            int numberOfInstances = 5;
            int numberOfVehicles = 5;
            int numberOfCustomers = 12;
            int vehicleCapacity = 2000;
            int instanceIndex = 0;
            int timeLimit = 1; 

            // 1) Instanz-Generator ausführen:
            var generator = new CVRPInstanceGenerator();
            // Erzeuge 5 Instanzen mit je 3 Fahrzeugen, 10 Kunden und Kapazität 100
            generator.GenerateInstances(numberOfInstances,
                                        numberOfVehicles,
                                        numberOfCustomers,
                                        vehicleCapacity);

            // 2) Eine Instanz einlesen "{Fahrzeuge}-{Kunden}-{Index}", z.B. "CVRP-3-10-0")
            var instanceFileName = CVRPInstance.ReadFromFile($"CVRP-{numberOfVehicles}-{numberOfCustomers}-{instanceIndex}");



            ////// --- Nearest Neighbour lösen ---
            

            // --- Saving Heuristic lösen ---
            ISolver savingSolver = new SavingHeuristicSolver(instanceFileName);
            var savingSolution = savingSolver.Solve();
            savingSolution.WriteToFile();
            Console.WriteLine("SavingHeuristic:   " + savingSolution);

            //--- Clarke-Wright lösen ---
            ISolver randomNextSolver = new RandomNextSolver(instanceFileName);
            var randomNextSolution = randomNextSolver.Solve();
            randomNextSolution.WriteToFile();
            Console.WriteLine("RandomNext:   " + randomNextSolution);

            // --- Gurobi lösen 
            ISolver gurobiSolver = new GurobiSolver(instanceFileName, timeLimit);
            var gurobiSolution = gurobiSolver.Solve();
            gurobiSolution.WriteToFile();
            Console.WriteLine("Gurobi:   " + gurobiSolution);

            //--- Google OR Tools lösen ---
            ISolver googleSolver = new GoogleORSolver(instanceFileName, 1);
            var googleSolution = googleSolver.Solve();
            googleSolution.WriteToFile();
            Console.WriteLine("Google OR Tools:   " + googleSolution);

            //---Dynamic Programming lösen ---
            ISolver dpsolver = new DPSolver(instanceFileName);
            CVRPSolution sol = dpsolver.Solve();
            sol.WriteToFile();
            Console.WriteLine("Dynamic Programming:   " + sol);
        }
    }
}