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
            Console.WriteLine("Running pre-GUI setup...");
            InstanceGenerator generator = new InstanceGenerator();
            generator.GenerateInstances(10, 5, 20); // Example parameters: 10 instances, 5 vehicles, 20 demand locations


        }
    }
}