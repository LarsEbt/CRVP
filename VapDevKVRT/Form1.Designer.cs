namespace VAP
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            openFileDialog1 = new OpenFileDialog();
            bOpenFile = new Button();
            tbLösung = new TextBox();
            label1 = new Label();
            pVisualization = new Panel();
            label2 = new Label();
            richTextBox1 = new RichTextBox();
            label3 = new Label();
            label4 = new Label();
            textBox1 = new TextBox();
            label5 = new Label();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            label6 = new Label();
            textBox4 = new TextBox();
            label7 = new Label();
            textBox5 = new TextBox();
            label8 = new Label();
            label9 = new Label();
            button1 = new Button();
            dataGridView1 = new DataGridView();
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            textBox6 = new TextBox();
            label10 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // bOpenFile
            // 
            bOpenFile.BackColor = SystemColors.InactiveCaption;
            bOpenFile.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            bOpenFile.ForeColor = SystemColors.ActiveCaptionText;
            bOpenFile.Location = new Point(33, 57);
            bOpenFile.Margin = new Padding(3, 2, 3, 2);
            bOpenFile.Name = "bOpenFile";
            bOpenFile.Size = new Size(120, 35);
            bOpenFile.TabIndex = 0;
            bOpenFile.Text = "Lösung laden";
            bOpenFile.UseVisualStyleBackColor = false;
            bOpenFile.Click += bOpenFile_Click;
            // 
            // tbLösung
            // 
            tbLösung.BackColor = SystemColors.Menu;
            tbLösung.Location = new Point(299, 75);
            tbLösung.Margin = new Padding(3, 2, 3, 2);
            tbLösung.Name = "tbLösung";
            tbLösung.Size = new Size(247, 23);
            tbLösung.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(170, 75);
            label1.Name = "label1";
            label1.Size = new Size(111, 15);
            label1.TabIndex = 2;
            label1.Text = "Visualisierte Lösung";
            // 
            // pVisualization
            // 
            pVisualization.BorderStyle = BorderStyle.FixedSingle;
            pVisualization.Location = new Point(33, 115);
            pVisualization.Margin = new Padding(3, 2, 3, 2);
            pVisualization.Name = "pVisualization";
            pVisualization.Size = new Size(512, 408);
            pVisualization.TabIndex = 3;
            pVisualization.Paint += pVisualization_Paint;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(571, 75);
            label2.Name = "label2";
            label2.Size = new Size(122, 21);
            label2.TabIndex = 2;
            label2.Text = "Routen Details";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(571, 334);
            richTextBox1.Margin = new Padding(1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(339, 190);
            richTextBox1.TabIndex = 5;
            richTextBox1.Text = "";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Black", 20F, FontStyle.Bold);
            label3.ForeColor = SystemColors.HotTrack;
            label3.Location = new Point(33, 12);
            label3.Margin = new Padding(1, 0, 1, 0);
            label3.Name = "label3";
            label3.Size = new Size(200, 37);
            label3.TabIndex = 6;
            label3.Text = "CVRP SOLVER";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(582, 112);
            label4.Margin = new Padding(1, 0, 1, 0);
            label4.Name = "label4";
            label4.Size = new Size(42, 15);
            label4.TabIndex = 7;
            label4.Text = "Solver:";
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.ButtonFace;
            textBox1.Location = new Point(693, 112);
            textBox1.Margin = new Padding(3, 2, 3, 2);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(216, 23);
            textBox1.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(582, 144);
            label5.Margin = new Padding(1, 0, 1, 0);
            label5.Name = "label5";
            label5.Size = new Size(49, 15);
            label5.TabIndex = 7;
            label5.Text = "Lösung:";
            // 
            // textBox2
            // 
            textBox2.BackColor = SystemColors.ButtonFace;
            textBox2.Location = new Point(693, 141);
            textBox2.Margin = new Padding(3, 2, 3, 2);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(216, 23);
            textBox2.TabIndex = 1;
            // 
            // textBox3
            // 
            textBox3.BackColor = SystemColors.ButtonFace;
            textBox3.Location = new Point(693, 204);
            textBox3.Margin = new Padding(3, 2, 3, 2);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(216, 23);
            textBox3.TabIndex = 1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(582, 207);
            label6.Margin = new Padding(1, 0, 1, 0);
            label6.Name = "label6";
            label6.Size = new Size(64, 15);
            label6.TabIndex = 7;
            label6.Text = "Fehrzeuge:";
            // 
            // textBox4
            // 
            textBox4.BackColor = SystemColors.ButtonFace;
            textBox4.Location = new Point(693, 238);
            textBox4.Margin = new Padding(3, 2, 3, 2);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(216, 23);
            textBox4.TabIndex = 1;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(582, 240);
            label7.Margin = new Padding(1, 0, 1, 0);
            label7.Name = "label7";
            label7.Size = new Size(69, 15);
            label7.TabIndex = 7;
            label7.Text = "Auslastung:";
            // 
            // textBox5
            // 
            textBox5.BackColor = SystemColors.ButtonFace;
            textBox5.Location = new Point(693, 272);
            textBox5.Margin = new Padding(3, 2, 3, 2);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(216, 23);
            textBox5.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(582, 275);
            label8.Margin = new Padding(1, 0, 1, 0);
            label8.Name = "label8";
            label8.Size = new Size(103, 15);
            label8.TabIndex = 7;
            label8.Text = "Gesamtnachfrage:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(571, 307);
            label9.Name = "label9";
            label9.Size = new Size(98, 21);
            label9.TabIndex = 2;
            label9.Text = "Teil-Routen";
            // 
            // button1
            // 
            button1.BackColor = SystemColors.InactiveCaption;
            button1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ActiveCaptionText;
            button1.Location = new Point(938, 64);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(208, 35);
            button1.TabIndex = 0;
            button1.Text = "Lösungen vergleichen";
            button1.UseVisualStyleBackColor = false;
            button1.Click += bMultiLoad_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(931, 112);
            dataGridView1.Margin = new Padding(1);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 92;
            dataGridView1.Size = new Size(475, 216);
            dataGridView1.TabIndex = 8;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chart1.Legends.Add(legend1);
            chart1.Location = new Point(931, 334);
            chart1.Margin = new Padding(1);
            chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            chart1.Series.Add(series1);
            chart1.Size = new Size(475, 190);
            chart1.TabIndex = 9;
            chart1.Text = "chart1";
            chart1.Click += chart1_Click;
            // 
            // textBox6
            // 
            textBox6.BackColor = SystemColors.ButtonFace;
            textBox6.Location = new Point(693, 174);
            textBox6.Margin = new Padding(3, 2, 3, 2);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(216, 23);
            textBox6.TabIndex = 1;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(582, 176);
            label10.Margin = new Padding(1, 0, 1, 0);
            label10.Name = "label10";
            label10.Size = new Size(30, 15);
            label10.TabIndex = 7;
            label10.Text = "Zeit:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(1508, 542);
            Controls.Add(chart1);
            Controls.Add(dataGridView1);
            Controls.Add(label10);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(textBox6);
            Controls.Add(richTextBox1);
            Controls.Add(textBox5);
            Controls.Add(pVisualization);
            Controls.Add(textBox4);
            Controls.Add(label9);
            Controls.Add(label2);
            Controls.Add(textBox3);
            Controls.Add(label1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(tbLösung);
            Controls.Add(button1);
            Controls.Add(bOpenFile);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Visualisierung";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog openFileDialog1;
        private Button bOpenFile;
        private TextBox tbLösung;
        private Label label1;
        private Panel pVisualization;
        private Label label2;
        private RichTextBox richTextBox1;
        private Label label3;
        private Label label4;
        private TextBox textBox1;
        private Label label5;
        private TextBox textBox2;
        private TextBox textBox3;
        private Label label6;
        private TextBox textBox4;
        private Label label7;
        private TextBox textBox5;
        private Label label8;
        private Label label9;
        private Button button1;
        private DataGridView dataGridView1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private TextBox textBox6;
        private Label label10;
    }
}
