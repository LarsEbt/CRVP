using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CVRP
{
    public class CVRPSolution
    {
        public string InstanceName { get; set; }
        public string Solver { get; set; }
        public double TravelCosts { get; set; }
        public double SolutionTime { get; set; }
        public double[,] XSol { get; set; }
      
        public double[] YSol { get; set; }

        public CVRPSolution(
            string instanceName,
            string solver,
            double travelCosts,
            double solutionTime,
            double[,] xSol,
            double[] ySol)
        {
            InstanceName = instanceName;
            Solver = solver;
            TravelCosts = travelCosts;
            SolutionTime = solutionTime;
            XSol = xSol;
            YSol = ySol;
        }

        public void WriteToFile()
        {
            Console.WriteLine($"Writing solution to file");
            string path = @$"..\..\..\Solutions\{InstanceName}_{Solver}_CVRPSol.txt";
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine($"Name\n{InstanceName}\n");
                writer.WriteLine($"Solver\n{Solver}\n");
                writer.WriteLine($"Travel costs\n{TravelCosts:F0}\n");
                writer.WriteLine($"Solution time\n{SolutionTime}\n");

                writer.WriteLine("ySol");
                for (int k = 0; k < YSol.Length; k++)
                    writer.Write($"{YSol[k]} ");

                writer.WriteLine("\n\nxSol");
                for (int k = 0; k < XSol.GetLength(0); k++)
                {
                    for (int i = 0; i < XSol.GetLength(1); i++)
                        writer.Write($"{XSol[k, i]} ");
                    writer.WriteLine();
                }
            }
        }

        public static CVRPSolution ReadFromFile(string instanceName, string solverName)
        {
            var lines = File.ReadAllLines(@$"..\..\..\Solutions\{instanceName}_{solverName}_CVRPSol.txt")
                            .ToList();

            string name = lines[lines.IndexOf("Name") + 1];
            string solver = lines[lines.IndexOf("Solver") + 1];
            double travelCosts = double.Parse(lines[lines.IndexOf("Travel costs") + 1]);
            double solutionTime = double.Parse(lines[lines.IndexOf("Solution time") + 1]);

            var ySol = lines[lines.IndexOf("ySol") + 1]
                .Trim()
                .Split(' ')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(double.Parse)
                .ToArray();
            int vehicleCount = ySol.Length;

            int xStart = lines.IndexOf("xSol") + 1;
            int nodeCount = lines[xStart]
                .Trim()
                .Split(' ')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Count();

            var xSol = new double[vehicleCount, nodeCount];
            for (int k = 0; k < vehicleCount; k++)
            {
                var parts = lines[xStart + k]
                    .Trim()
                    .Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToArray();
                for (int i = 0; i < nodeCount; i++)
                    xSol[k, i] = double.Parse(parts[i]);
            }

            return new CVRPSolution(
                name,
                solver,
                travelCosts,
                solutionTime,
                xSol,
                ySol
            );
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Solution:");
            sb.AppendLine($"Instance: {InstanceName}");
            sb.AppendLine($"Solver:   {Solver}");
            sb.AppendLine($"Travel costs:  {TravelCosts:F0}");
            sb.AppendLine($"Solution time: {SolutionTime}");
            sb.AppendLine();
            sb.Append("ySol: ");
            for (int k = 0; k < YSol.Length; k++)
                sb.Append($"{YSol[k]} ");
            sb.AppendLine("\n");
            sb.AppendLine("xSol:");
            for (int k = 0; k < XSol.GetLength(0); k++)
            {
                for (int i = 0; i < XSol.GetLength(1); i++)
                    sb.Append($"{XSol[k, i]} ");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}