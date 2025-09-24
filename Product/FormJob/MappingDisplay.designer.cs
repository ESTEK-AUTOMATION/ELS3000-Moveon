namespace Product
{
    partial class MappingDisplay
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
            this.panelMapping = new System.Windows.Forms.Panel();
            this.labelCellLocation = new System.Windows.Forms.Label();
            this.pictureBoxMapping = new System.Windows.Forms.PictureBox();
            this.panelMapping.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMapping)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMapping
            // 
            this.panelMapping.Controls.Add(this.labelCellLocation);
            this.panelMapping.Controls.Add(this.pictureBoxMapping);
            this.panelMapping.Enabled = false;
            this.panelMapping.Location = new System.Drawing.Point(1, 2);
            this.panelMapping.Margin = new System.Windows.Forms.Padding(6);
            this.panelMapping.Name = "panelMapping";
            this.panelMapping.Size = new System.Drawing.Size(957, 565);
            this.panelMapping.TabIndex = 85;
            // 
            // labelCellLocation
            // 
            this.labelCellLocation.AutoSize = true;
            this.labelCellLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelCellLocation.Location = new System.Drawing.Point(5, 5);
            this.labelCellLocation.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelCellLocation.Name = "labelCellLocation";
            this.labelCellLocation.Size = new System.Drawing.Size(124, 20);
            this.labelCellLocation.TabIndex = 87;
            this.labelCellLocation.Text = "Row = 0, Col = 0";
            // 
            // pictureBoxMapping
            // 
            this.pictureBoxMapping.BackColor = System.Drawing.Color.LightCyan;
            this.pictureBoxMapping.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxMapping.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBoxMapping.Name = "pictureBoxMapping";
            this.pictureBoxMapping.Size = new System.Drawing.Size(957, 565);
            this.pictureBoxMapping.TabIndex = 27;
            this.pictureBoxMapping.TabStop = false;
            //this.pictureBoxMapping.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMapping_MouseDown);
            this.pictureBoxMapping.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMapping_MouseDown);
            this.pictureBoxMapping.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMapping_MouseUp);
            // 
            // MappingDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMapping);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "MappingDisplay";
            this.Size = new System.Drawing.Size(960, 570);
            this.panelMapping.ResumeLayout(false);
            this.panelMapping.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMapping)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panelMapping;
        public System.Windows.Forms.PictureBox pictureBoxMapping;
        public System.Windows.Forms.Label labelCellLocation;
    }
}
