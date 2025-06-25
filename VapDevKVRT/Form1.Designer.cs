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
            label10 = new Label();
            textBox6 = new TextBox();
            button1 = new Button();
            dataGridView1 = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
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
            bOpenFile.Location = new Point(70, 141);
            bOpenFile.Margin = new Padding(6);
            bOpenFile.Name = "bOpenFile";
            bOpenFile.Size = new Size(257, 87);
            bOpenFile.TabIndex = 0;
            bOpenFile.Text = "Lösung laden";
            bOpenFile.UseVisualStyleBackColor = false;
            bOpenFile.Click += bOpenFile_Click;
            // 
            // tbLösung
            // 
            tbLösung.BackColor = SystemColors.Menu;
            tbLösung.Location = new Point(641, 185);
            tbLösung.Margin = new Padding(6);
            tbLösung.Name = "tbLösung";
            tbLösung.Size = new Size(524, 43);
            tbLösung.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(365, 185);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(252, 37);
            label1.TabIndex = 2;
            label1.Text = "Visualisierte Lösung";
            // 
            // pVisualization
            // 
            pVisualization.BorderStyle = BorderStyle.FixedSingle;
            pVisualization.Location = new Point(70, 284);
            pVisualization.Margin = new Padding(6);
            pVisualization.Name = "pVisualization";
            pVisualization.Size = new Size(1095, 1003);
            pVisualization.TabIndex = 3;
            pVisualization.Paint += pVisualization_Paint;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(1223, 180);
            label2.Margin = new Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new Size(267, 48);
            label2.TabIndex = 2;
            label2.Text = "Routen Details";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(1223, 823);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(722, 464);
            richTextBox1.TabIndex = 5;
            richTextBox1.Text = "";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Black", 20F, FontStyle.Bold);
            label3.ForeColor = SystemColors.HotTrack;
            label3.Location = new Point(70, 30);
            label3.Name = "label3";
            label3.Size = new Size(440, 81);
            label3.TabIndex = 6;
            label3.Text = "CVRP SOLVER";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1223, 287);
            label4.Name = "label4";
            label4.Size = new Size(96, 37);
            label4.TabIndex = 7;
            label4.Text = "Solver:";
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.ButtonFace;
            textBox1.Location = new Point(1486, 287);
            textBox1.Margin = new Padding(6);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(459, 43);
            textBox1.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(1223, 356);
            label5.Name = "label5";
            label5.Size = new Size(109, 37);
            label5.TabIndex = 7;
            label5.Text = "Lösung:";
            // 
            // textBox2
            // 
            textBox2.BackColor = SystemColors.ButtonFace;
            textBox2.Location = new Point(1486, 356);
            textBox2.Margin = new Padding(6);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(459, 43);
            textBox2.TabIndex = 1;
            // 
            // textBox3
            // 
            textBox3.BackColor = SystemColors.ButtonFace;
            textBox3.Location = new Point(1486, 426);
            textBox3.Margin = new Padding(6);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(459, 43);
            textBox3.TabIndex = 1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(1223, 426);
            label6.Name = "label6";
            label6.Size = new Size(145, 37);
            label6.TabIndex = 7;
            label6.Text = "Fehrzeuge:";
            // 
            // textBox4
            // 
            textBox4.BackColor = SystemColors.ButtonFace;
            textBox4.Location = new Point(1486, 498);
            textBox4.Margin = new Padding(6);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(459, 43);
            textBox4.TabIndex = 1;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(1223, 498);
            label7.Name = "label7";
            label7.Size = new Size(153, 37);
            label7.TabIndex = 7;
            label7.Text = "Auslastung:";
            // 
            // textBox5
            // 
            textBox5.BackColor = SystemColors.ButtonFace;
            textBox5.Location = new Point(1486, 576);
            textBox5.Margin = new Padding(6);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(459, 43);
            textBox5.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(1223, 576);
            label8.Name = "label8";
            label8.Size = new Size(230, 37);
            label8.TabIndex = 7;
            label8.Text = "Gesamtnachfrage:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(1223, 749);
            label9.Margin = new Padding(6, 0, 6, 0);
            label9.Name = "label9";
            label9.Size = new Size(214, 48);
            label9.TabIndex = 2;
            label9.Text = "Teil-Routen";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(1223, 653);
            label10.Name = "label10";
            label10.Size = new Size(162, 37);
            label10.TabIndex = 7;
            label10.Text = "Lösungszeit:";
            // 
            // textBox6
            // 
            textBox6.BackColor = SystemColors.ButtonFace;
            textBox6.Location = new Point(1486, 653);
            textBox6.Margin = new Padding(6);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(459, 43);
            textBox6.TabIndex = 1;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.InactiveCaption;
            button1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ActiveCaptionText;
            button1.Location = new Point(70, 1357);
            button1.Margin = new Padding(6);
            button1.Name = "button1";
            button1.Size = new Size(386, 87);
            button1.TabIndex = 0;
            button1.Text = "Lösungen vergleichen";
            button1.UseVisualStyleBackColor = false;
            button1.Click += bMultiLoad_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.BackgroundColor = SystemColors.Control;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn7 });
            dataGridView1.GridColor = SystemColors.Menu;
            dataGridView1.Location = new Point(70, 1474);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 92;
            dataGridView1.Size = new Size(1875, 361);
            dataGridView1.TabIndex = 100;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn1.HeaderText = "Instanz Name";
            dataGridViewTextBoxColumn1.MinimumWidth = 11;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn2.HeaderText = "Solver";
            dataGridViewTextBoxColumn2.MinimumWidth = 11;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn3.HeaderText = "Lösung";
            dataGridViewTextBoxColumn3.MinimumWidth = 11;
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewTextBoxColumn4.HeaderText = "Lösungszeit";
            dataGridViewTextBoxColumn4.MinimumWidth = 11;
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            dataGridViewTextBoxColumn4.Width = 205;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn5.HeaderText = "#Fahrzeuge";
            dataGridViewTextBoxColumn5.MinimumWidth = 11;
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn6.HeaderText = "Ges. Nachfrage";
            dataGridViewTextBoxColumn6.MinimumWidth = 11;
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewTextBoxColumn7.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn7.HeaderText = "Auslastung";
            dataGridViewTextBoxColumn7.MinimumWidth = 11;
            dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.InactiveCaption;
            button2.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.ForeColor = SystemColors.ActiveCaptionText;
            button2.Location = new Point(70, 162);
            button2.Margin = new Padding(6);
            button2.Name = "button2";
            button2.Size = new Size(283, 87);
            button2.TabIndex = 0;
            button2.Text = "Lösung laden";
            button2.UseVisualStyleBackColor = false;
            button2.Click += bOpenFile_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(15F, 37F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(2032, 1884);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
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
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label10);
            Controls.Add(textBox6);
            Controls.Add(dataGridView1);
            Margin = new Padding(6);
            Name = "Form1";
            Text = "Visualisierung";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
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
        private Label label10;
        private TextBox textBox6;
        private Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Button button2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    }
}
