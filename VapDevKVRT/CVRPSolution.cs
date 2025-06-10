using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace VapDevKVRT
{
    public class CVRPSolution
    {
        public string InstanceName {  get; set; }

        public string Solver {  get; set; }

        public double DeliveryCosts { get; set; }

        public double Solutiontime { get; set; }

        public List<List<int>> Routes { get; set; } = new List<List<int>>(); //z.B. [[0, 1, 2], [0, 3, 4]] für zwei Routen, wobei 0 das Depot ist

        public int NumberOfVehicles { get; set; }

        public CVRPSolution(string instanceName, string solver, double deliveryCosts, double solutiontime, int numberOfVehicles)
        {
            InstanceName = instanceName;
            Solver = solver;
            DeliveryCosts = deliveryCosts;
            Solutiontime = solutiontime;
            NumberOfVehicles = numberOfVehicles;
        }

        public void WriteToFile()
        {
            StreamWriter writer = new StreamWriter($@"..\..\..\solutions\{InstanceName}_{Solver}_Sol.txt");

            writer.WriteLine($"Name\n{InstanceName}\n\nSolver\n{Solver}\n\nDelivery costs\n{DeliveryCosts}\n\nSolution time\n{Solutiontime}\n\nNumber of vehicles\n{NumberOfVehicles}\n");

            writer.WriteLine("\nRoutes:");
            foreach (var route in Routes)
            {
                writer.WriteLine(string.Join(" -> ", route)); 
            }

            writer.Close();
        }

        public static CVRPSolution ReadFromFile(string instanceName, string solverName)
        {
            var lines = File.ReadAllLines($@"..\..\..\solutions\{instanceName}_{solverName}_Sol.txt").ToList();

            int id = lines.IndexOf("Name");
            string name = lines[id + 1];

            string solver = lines[lines.IndexOf("Solver") + 1];
            double deliveryCosts = double.Parse(lines[lines.IndexOf("Delivery costs") + 1]);
            double Solutiontime = double.Parse(lines[lines.IndexOf("Solution time") + 1]);
            int NumberOfVehicles = int.Parse(lines[lines.IndexOf("Number of vehicles") + 1]);
            //Reihenfolge fehlt noch!!!
              
            return new CVRPSolution(instanceName, solver, deliveryCosts, Solutiontime, NumberOfVehicles);

        }
    }
}
