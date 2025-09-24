namespace Product
{
    partial class StateDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StateDisplay));
            this.labelStateDisplay = new System.Windows.Forms.Label();
            this.pictureBoxStateDisplay = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStateDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // labelStateDisplay
            // 
            this.labelStateDisplay.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.labelStateDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 51.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStateDisplay.ForeColor = System.Drawing.Color.Red;
            this.labelStateDisplay.Location = new System.Drawing.Point(3, 116);
            this.labelStateDisplay.Name = "labelStateDisplay";
            this.labelStateDisplay.Size = new System.Drawing.Size(1114, 198);
            this.labelStateDisplay.TabIndex = 0;
            this.labelStateDisplay.Text = "label1";
            this.labelStateDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxStateDisplay
            // 
            this.pictureBoxStateDisplay.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxStateDisplay.Image")));
            this.pictureBoxStateDisplay.Location = new System.Drawing.Point(446, 317);
            this.pictureBoxStateDisplay.Name = "pictureBoxStateDisplay";
            this.pictureBoxStateDisplay.Size = new System.Drawing.Size(215, 149);
            this.pictureBoxStateDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxStateDisplay.TabIndex = 1;
            this.pictureBoxStateDisplay.TabStop = false;
            // 
            // StateDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(20F, 39F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.Controls.Add(this.pictureBoxStateDisplay);
            this.Controls.Add(this.labelStateDisplay);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 25.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(9, 10, 9, 10);
            this.Name = "StateDisplay";
            this.Size = new System.Drawing.Size(1114, 482);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStateDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label labelStateDisplay;
        public System.Windows.Forms.PictureBox pictureBoxStateDisplay;
    }
}
