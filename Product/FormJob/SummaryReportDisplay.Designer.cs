namespace Product
{
    partial class SummaryReportDisplay
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
            this.panelSummaryReport = new System.Windows.Forms.Panel();
            this.richTextBoxSummaryReport = new System.Windows.Forms.RichTextBox();
            this.panelSummaryReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSummaryReport
            // 
            this.panelSummaryReport.Controls.Add(this.richTextBoxSummaryReport);
            this.panelSummaryReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSummaryReport.Location = new System.Drawing.Point(0, 0);
            this.panelSummaryReport.Name = "panelSummaryReport";
            this.panelSummaryReport.Size = new System.Drawing.Size(960, 570);
            this.panelSummaryReport.TabIndex = 0;
            // 
            // richTextBoxSummaryReport
            // 
            this.richTextBoxSummaryReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxSummaryReport.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.richTextBoxSummaryReport.Location = new System.Drawing.Point(1, 2);
            this.richTextBoxSummaryReport.Name = "richTextBoxSummaryReport";
            this.richTextBoxSummaryReport.ReadOnly = true;
            this.richTextBoxSummaryReport.Size = new System.Drawing.Size(956, 565);
            this.richTextBoxSummaryReport.TabIndex = 0;
            this.richTextBoxSummaryReport.Text = "";
            this.richTextBoxSummaryReport.WordWrap = false;
            // 
            // SummaryReportDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.Controls.Add(this.panelSummaryReport);
            this.Name = "SummaryReportDisplay";
            this.Size = new System.Drawing.Size(960, 570);
            this.panelSummaryReport.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSummaryReport;
        private System.Windows.Forms.RichTextBox richTextBoxSummaryReport;
    }
}
