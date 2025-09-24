namespace Product
{
    partial class AlarmParetoChart
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tabPageAlarmList = new System.Windows.Forms.TabPage();
            this.tabPageChart = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AlarmChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listViewAlarmList = new System.Windows.Forms.ListView();
            this.AlarmId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AlarmMessageEnglish = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageAlarmList.SuspendLayout();
            this.tabPageChart.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlarmChart)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageAlarmList
            // 
            this.tabPageAlarmList.Controls.Add(this.panel2);
            this.tabPageAlarmList.Location = new System.Drawing.Point(4, 22);
            this.tabPageAlarmList.Name = "tabPageAlarmList";
            this.tabPageAlarmList.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAlarmList.Size = new System.Drawing.Size(952, 544);
            this.tabPageAlarmList.TabIndex = 1;
            this.tabPageAlarmList.Text = "Alarm List";
            this.tabPageAlarmList.UseVisualStyleBackColor = true;
            // 
            // tabPageChart
            // 
            this.tabPageChart.Controls.Add(this.panel1);
            this.tabPageChart.Location = new System.Drawing.Point(4, 22);
            this.tabPageChart.Name = "tabPageChart";
            this.tabPageChart.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageChart.Size = new System.Drawing.Size(952, 544);
            this.tabPageChart.TabIndex = 0;
            this.tabPageChart.Text = "Chart";
            this.tabPageChart.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageChart);
            this.tabControl1.Controls.Add(this.tabPageAlarmList);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(960, 570);
            this.tabControl1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.AlarmChart);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(946, 538);
            this.panel1.TabIndex = 0;
            // 
            // AlarmChart
            // 
            chartArea1.Name = "ChartArea1";
            this.AlarmChart.ChartAreas.Add(chartArea1);
            this.AlarmChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.AlarmChart.Legends.Add(legend1);
            this.AlarmChart.Location = new System.Drawing.Point(0, 0);
            this.AlarmChart.Name = "AlarmChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.AlarmChart.Series.Add(series1);
            this.AlarmChart.Size = new System.Drawing.Size(946, 538);
            this.AlarmChart.TabIndex = 1;
            this.AlarmChart.Text = "chart1";
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.listViewAlarmList);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(946, 538);
            this.panel2.TabIndex = 0;
            // 
            // listViewAlarmList
            // 
            this.listViewAlarmList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AlarmId,
            this.AlarmMessageEnglish});
            this.listViewAlarmList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAlarmList.HideSelection = false;
            this.listViewAlarmList.Location = new System.Drawing.Point(0, 0);
            this.listViewAlarmList.Name = "listViewAlarmList";
            this.listViewAlarmList.Size = new System.Drawing.Size(946, 538);
            this.listViewAlarmList.TabIndex = 1;
            this.listViewAlarmList.UseCompatibleStateImageBehavior = false;
            this.listViewAlarmList.View = System.Windows.Forms.View.Details;
            // 
            // AlarmId
            // 
            this.AlarmId.Text = "AlarmId";
            this.AlarmId.Width = 80;
            // 
            // AlarmMessageEnglish
            // 
            this.AlarmMessageEnglish.Text = "AlarmMessageEnglish";
            this.AlarmMessageEnglish.Width = 300;
            // 
            // AlarmParetoChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "AlarmParetoChart";
            this.Size = new System.Drawing.Size(960, 570);
            this.tabPageAlarmList.ResumeLayout(false);
            this.tabPageAlarmList.PerformLayout();
            this.tabPageChart.ResumeLayout(false);
            this.tabPageChart.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AlarmChart)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPageAlarmList;
        private System.Windows.Forms.TabPage tabPageChart;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart AlarmChart;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView listViewAlarmList;
        private System.Windows.Forms.ColumnHeader AlarmId;
        private System.Windows.Forms.ColumnHeader AlarmMessageEnglish;
    }
}
