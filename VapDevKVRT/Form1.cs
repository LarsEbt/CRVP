using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using CVRP;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using VapDevKVRT;

namespace VAP
{
    public partial class Form1 : Form
    {
        private CVRPInstance currentInstance;
        private CVRPSolution currentSolution;
        private readonly Color[] routeColors = new Color[]
        {
            Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Purple, Color.Brown, Color.Magenta, Color.Cyan, Color.DarkGoldenrod, Color.DarkTurquoise
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void bOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "solutions");
            openFileDialog1.Filter = "CVRP Solution Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string solPath = openFileDialog1.FileName;
                tbLösung.Text = Path.GetFileName(solPath);
                // Parse instance and solver name from file name
                var file = Path.GetFileNameWithoutExtension(solPath);
                // Expected: CVRP-5-10-0_GoogleOR_CVRPSol
                var parts = file.Split('_');
                if (parts.Length < 3)
                {
                    MessageBox.Show("Dateiname entspricht nicht dem erwarteten Muster.");
                    return;
                }
                string instanceName = parts[0];
                string solverName = parts[1];
                try
                {
                    currentInstance = CVRPInstance.ReadFromFile(instanceName);
                    currentSolution = CVRPSolution.ReadFromFile(instanceName, solverName);
                    pVisualization.Invalidate();
                    PopulateRouteList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden: {ex.Message}");
                }
            }
        }

        private void PopulateRouteList()
        {
            richTextBox1.Clear();
            if (currentInstance == null || currentSolution == null)
                return;

            int n = currentInstance.NodeCount;
            int K = currentInstance.VehicleCount;
            var coords = currentInstance.Coordinates;
            var demands = currentInstance.Demands;

            // Summary variables
            double totalSolutionDistance = 0;
            int vehiclesUsed = 0;
            int totalCustomers = 0;
            double totalDemand = 0;

            for (int k = 0; k < K; k++)
            {
                if (currentSolution.YSol[k] < 0.5) continue;

                vehiclesUsed++;

                // Build route: always starts at depot (0)
                var route = new List<int> { 0 };
                var visited = new bool[n];
                visited[0] = true;
                int last = 0;

                while (true)
                {
                    int next = -1;
                    for (int i = 1; i < n; i++)
                    {
                        if (!visited[i] && currentSolution.XSol[k, i] > 0.5)
                        {
                            next = i;
                            break;
                        }
                    }
                    if (next == -1) break;
                    route.Add(next);
                    visited[next] = true;
                    last = next;
                }
                route.Add(0); // return to depot

                // Calculate route statistics
                double totalDistance = 0;
                double routeDemand = 0;
                int customerCount = 0;

                for (int i = 0; i < route.Count - 1; i++)
                {
                    int from = route[i];
                    int to = route[i + 1];
                    totalDistance += currentInstance.CostMatrix[from, to];

                    if (from > 0) // Not depot
                    {
                        routeDemand += demands[from];
                        customerCount++;
                    }
                }

                totalSolutionDistance += totalDistance;
                totalCustomers += customerCount;
                totalDemand += routeDemand;

                double utilization = (routeDemand / currentInstance.VehicleCapacity) * 100;

                // Format route string
                string routeString = string.Join(" -> ", route);
                string routeInfo = $"=== Fahrzeug {k + 1} ===\n" +
                                 $"Route: {routeString}\n" +
                                 $"Distanz: {totalDistance:F2}\n" +
                                 $"Kunden: {customerCount}\n" +
                                 $"Auslastung: {utilization:F1}%\n" +
                                 $"Nachfrage: {routeDemand:F1}/{currentInstance.VehicleCapacity:F1}\n" +
                                 $"-------------------------------------------------------------\n"; 

                richTextBox1.AppendText(routeInfo);
            }

            // Add summary section
            double averageUtilization = vehiclesUsed > 0 ? (totalDemand / (vehiclesUsed * currentInstance.VehicleCapacity)) * 100 : 0;
            
            // Populate text fields with summary information
            textBox1.Text = currentSolution.Solver;
            textBox2.Text = $"{currentSolution.TravelCosts:F2}";
            textBox3.Text = $"{vehiclesUsed}";
            textBox4.Text = $"{averageUtilization:F1}%";
            textBox5.Text = $"{totalDemand:F1}";

            // Update label
            label2.Text = $"Routen Details ({vehiclesUsed} Fahrzeuge)";
        }

        private void pVisualization_Paint(object sender, PaintEventArgs e)
        {
            if (currentInstance == null || currentSolution == null)
                return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var coords = currentInstance.Coordinates;
            int n = currentInstance.NodeCount;
            int K = currentInstance.VehicleCount;
            // Find bounds for scaling
            double minX = coords.Min(c => c.x), maxX = coords.Max(c => c.x);
            double minY = coords.Min(c => c.y), maxY = coords.Max(c => c.y);
            float margin = 40;
            float panelW = pVisualization.Width - 2 * margin;
            float panelH = pVisualization.Height - 2 * margin;
            float scaleX = (float)(panelW / (maxX - minX));
            float scaleY = (float)(panelH / (maxY - minY));
            Func<double, double, PointF> map = (x, y) => new PointF(
                margin + (float)((x - minX) * scaleX),
                margin + (float)(panelH - (y - minY) * scaleY)
            );
            // Draw routes
            for (int k = 0; k < K; k++)
            {
                if (currentSolution.YSol[k] < 0.5) continue;
                var color = routeColors[k % routeColors.Length];
                var pen = new Pen(color, 3);
                // Build route: always starts at depot (0)
                var route = new System.Collections.Generic.List<int> { 0 };
                var visited = new bool[n];
                visited[0] = true;
                int last = 0;
                while (true)
                {
                    int next = -1;
                    for (int i = 1; i < n; i++)
                    {
                        if (!visited[i] && currentSolution.XSol[k, i] > 0.5)
                        {
                            next = i;
                            break;
                        }
                    }
                    if (next == -1) break;
                    route.Add(next);
                    visited[next] = true;
                    last = next;
                }
                route.Add(0); // return to depot
                // Draw lines
                for (int i = 0; i < route.Count - 1; i++)
                {
                    var p1 = map(coords[route[i]].x, coords[route[i]].y);
                    var p2 = map(coords[route[i + 1]].x, coords[route[i + 1]].y);
                    g.DrawLine(pen, p1, p2);
                }
            }

            // Draw nodes
            for (int i = 0; i < n; i++)
            {
                var pt = map(coords[i].x, coords[i].y);
                Brush b = i == 0 ? Brushes.Black : Brushes.White;
                g.FillEllipse(b, pt.X - 8, pt.Y - 8, 16, 16);
                g.DrawEllipse(Pens.Black, pt.X - 8, pt.Y - 8, 16, 16);
                if (i == 0)
                    g.DrawString("Depot", this.Font, Brushes.Black, pt.X + 10, pt.Y - 10);
                else
                    g.DrawString(i.ToString(), this.Font, Brushes.Black, pt.X + 10, pt.Y - 10);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Optional: set default state
        }


        private void bMultiLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "solutions");
            openFileDialog1.Filter = "CVRP Solution Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var files = openFileDialog1.FileNames;
                List<CVRPSolutionInfo> solutionInfos = new List<CVRPSolutionInfo>();

                foreach (var path in files)
                {
                    try
                    {
                        var file = Path.GetFileNameWithoutExtension(path);
                        var parts = file.Split('_');
                        if (parts.Length < 3)
                            continue;

                        string instanceName = parts[0];
                        string solverName = parts[1];

                        var instance = CVRPInstance.ReadFromFile(instanceName);
                        var solution = CVRPSolution.ReadFromFile(instanceName, solverName);

                        int vehiclesUsed = solution.YSol.Count(y => y > 0.5);
                        double demand = 0;
                        for (int i = 0; i < instance.NodeCount; i++)
                        {
                            if (i > 0)
                                demand += instance.Demands[i];
                        }

                        double avgUtil = (vehiclesUsed > 0) ? (demand / (vehiclesUsed * instance.VehicleCapacity)) * 100 : 0;

                        solutionInfos.Add(new CVRPSolutionInfo
                        {
                            Dateiname = Path.GetFileName(path),
                            Solver = solution.Solver,
                            Kosten = solution.TravelCosts,
                            Fahrzeuge = vehiclesUsed,
                            Auslastung = avgUtil,
                            Nachfrage = demand
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fehler bei Datei {path}: {ex.Message}");
                    }
                }

                dataGridView1.DataSource = solutionInfos;
            }
        }
    }
}
