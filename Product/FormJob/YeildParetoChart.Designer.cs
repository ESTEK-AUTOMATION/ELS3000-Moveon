namespace Product
{
    partial class YeildParetoChart
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
            this.panelParetoChart = new System.Windows.Forms.Panel();
            this.chartPareto = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panelParetoChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPareto)).BeginInit();
            this.SuspendLayout();
            // 
            // panelParetoChart
            // 
            this.panelParetoChart.AutoScroll = true;
            this.panelParetoChart.Controls.Add(this.chartPareto);
            this.panelParetoChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelParetoChart.Location = new System.Drawing.Point(0, 0);
            this.panelParetoChart.Name = "panelParetoChart";
            this.panelParetoChart.Size = new System.Drawing.Size(960, 570);
            this.panelParetoChart.TabIndex = 0;
            // 
            // chartPareto
            // 
            chartArea1.Name = "ChartArea1";
            this.chartPareto.ChartAreas.Add(chartArea1);
            this.chartPareto.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartPareto.Legends.Add(legend1);
            this.chartPareto.Location = new System.Drawing.Point(0, 0);
            this.chartPareto.Name = "chartPareto";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartPareto.Series.Add(series1);
            this.chartPareto.Size = new System.Drawing.Size(960, 570);
            this.chartPareto.TabIndex = 0;
            this.chartPareto.Text = "chart1";
            // 
            // YeildParetoChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelParetoChart);
            this.Name = "YeildParetoChart";
            this.Size = new System.Drawing.Size(960, 570);
            this.panelParetoChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartPareto)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panelParetoChart;
        public System.Windows.Forms.DataVisualization.Charting.Chart chartPareto;
    }
}
