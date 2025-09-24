namespace Product.FormLowYieldPart
{
    partial class LowYieldCountingItem
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownLowYieldTotalCount = new System.Windows.Forms.NumericUpDown();
            this.progressBarLowYieldPercentage = new System.Windows.Forms.ProgressBar();
            this.numericUpDownLowYieldPassCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowYieldTotalCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowYieldPassCount)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.flowLayoutPanel2);
            this.panel1.Location = new System.Drawing.Point(3, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1695, 144);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.numericUpDownLowYieldTotalCount);
            this.panel2.Controls.Add(this.progressBarLowYieldPercentage);
            this.panel2.Controls.Add(this.numericUpDownLowYieldPassCount);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(361, 14);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1320, 123);
            this.panel2.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(267, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 25);
            this.label5.TabIndex = 10;
            this.label5.Text = "Total Count :";
            // 
            // numericUpDownLowYieldTotalCount
            // 
            this.numericUpDownLowYieldTotalCount.Enabled = false;
            this.numericUpDownLowYieldTotalCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownLowYieldTotalCount.Location = new System.Drawing.Point(441, 30);
            this.numericUpDownLowYieldTotalCount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDownLowYieldTotalCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownLowYieldTotalCount.Name = "numericUpDownLowYieldTotalCount";
            this.numericUpDownLowYieldTotalCount.Size = new System.Drawing.Size(171, 30);
            this.numericUpDownLowYieldTotalCount.TabIndex = 11;
            // 
            // progressBarLowYieldPercentage
            // 
            this.progressBarLowYieldPercentage.ForeColor = System.Drawing.Color.Lime;
            this.progressBarLowYieldPercentage.Location = new System.Drawing.Point(105, 78);
            this.progressBarLowYieldPercentage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.progressBarLowYieldPercentage.Name = "progressBarLowYieldPercentage";
            this.progressBarLowYieldPercentage.Size = new System.Drawing.Size(1136, 23);
            this.progressBarLowYieldPercentage.TabIndex = 6;
            // 
            // numericUpDownLowYieldPassCount
            // 
            this.numericUpDownLowYieldPassCount.Enabled = false;
            this.numericUpDownLowYieldPassCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownLowYieldPassCount.Location = new System.Drawing.Point(938, 30);
            this.numericUpDownLowYieldPassCount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDownLowYieldPassCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownLowYieldPassCount.Name = "numericUpDownLowYieldPassCount";
            this.numericUpDownLowYieldPassCount.Size = new System.Drawing.Size(171, 30);
            this.numericUpDownLowYieldPassCount.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(766, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Clean Count :";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label4);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(27, 18);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(329, 105);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(229, 46);
            this.label4.TabIndex = 9;
            this.label4.Text = "Bond Head";
            // 
            // LowYieldCountingItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "LowYieldCountingItem";
            this.Size = new System.Drawing.Size(1701, 150);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowYieldTotalCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowYieldPassCount)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown numericUpDownLowYieldTotalCount;
        public System.Windows.Forms.ProgressBar progressBarLowYieldPercentage;
        public System.Windows.Forms.NumericUpDown numericUpDownLowYieldPassCount;
    }
}
