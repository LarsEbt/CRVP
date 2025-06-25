using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using CVRP;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using VapDevKVRT;
using System.Windows.Forms.DataVisualization.Charting;

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
                var file = Path.GetFileNameWithoutExtension(solPath);
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
            // Hier bleibt dein bestehender Visualisierungs- und Ausgabetext
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

                LadeTabelleInsDiagramm();
            }
        }

        private void LadeTabelleInsDiagramm()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();

            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);
            area.AxisX.Interval = 1;

            var serieKosten = new Series("Kosten")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.SteelBlue
            };

            var serieFahrzeuge = new Series("Fahrzeuge")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.ForestGreen
            };

            var serieAuslastung = new Series("Auslastung (%)")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.DarkOrange
            };

            var serieNachfrage = new Series("Nachfrage")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Purple
            };

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string solverName = row.Cells["Solver"].Value?.ToString();
                if (string.IsNullOrEmpty(solverName)) continue;

                double kosten = Convert.ToDouble(row.Cells["Kosten"].Value);
                int fahrzeuge = Convert.ToInt32(row.Cells["Fahrzeuge"].Value);
                double auslastung = Convert.ToDouble(row.Cells["Auslastung"].Value);
                double nachfrage = Convert.ToDouble(row.Cells["Nachfrage"].Value);

                serieKosten.Points.AddXY(solverName, kosten);
                serieFahrzeuge.Points.AddXY(solverName, fahrzeuge);
                serieAuslastung.Points.AddXY(solverName, auslastung);
                serieNachfrage.Points.AddXY(solverName, nachfrage);
            }

            chart1.Series.Add(serieKosten);
            chart1.Series.Add(serieFahrzeuge);
            chart1.Series.Add(serieAuslastung);
            chart1.Series.Add(serieNachfrage);

            chart1.Titles.Add("Lösungsvergleich");
        }

        private void pVisualization_Paint(object sender, PaintEventArgs e)
        {
            // Hier bleibt dein bestehender Visualisierungscode
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Optional init code
        }

        private void chart1_Click(object sender, EventArgs e)
        {
        }
    }

    public class CVRPSolutionInfo
    {
        public string Dateiname { get; set; }
        public string Solver { get; set; }
        public double Kosten { get; set; }
        public int Fahrzeuge { get; set; }
        public double Auslastung { get; set; }
        public double Nachfrage { get; set; }
    }
}
