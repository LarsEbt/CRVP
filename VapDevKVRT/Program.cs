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


        }
    }
}