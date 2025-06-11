namespace VapDevKVRT
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
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
            //1. Instanz generieren
            Console.WriteLine("Running pre-GUI setup...");
            InstanceGenerator generator = new InstanceGenerator();
            generator.GenerateInstances(10, 5, 20); // Example parameters: 10 instances, 5 vehicles, 20 demand locations

            //2. Instanzname definieren
            string instanceName = "0-5-20"; // Example instance name

            //3. Instancz laden
            CVRPInstance instance = CVRPInstance.ReadFromFile(instanceName);

            //4. Sovler wählen
            ISolver sovler = new GurobiSolver(instance, 5);

            //5. Instanz lösen
            CVRPSolution solution = sovler.Solve();


            // 6. Lösung speichern
            solution.WriteToFile();

            // 7. Ausgabe in Konsole
            Console.WriteLine("----- Lösung abgeschlossen -----");
            Console.WriteLine($"Solver: {solution.Solver}");
            Console.WriteLine($"Kosten: {solution.DeliveryCosts:F2}");
            Console.WriteLine($"Fahrzeuge: {solution.NumberOfVehicles}");
            Console.WriteLine($"Lösungszeit: {solution.Solutiontime:F3} Sekunden");
            Console.WriteLine("--------------------------------");


        }
    }
}