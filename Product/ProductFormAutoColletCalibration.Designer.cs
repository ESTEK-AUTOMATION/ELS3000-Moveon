namespace Product
{
    partial class ProductFormAutoColletCalibration
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxAutoColletRecipeName = new System.Windows.Forms.ComboBox();
            this.buttonStartAutoColletCalibration = new System.Windows.Forms.Button();
            this.buttonCloseAutoColletCalibration = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Vision Recipe:";
            // 
            // comboBoxAutoColletRecipeName
            // 
            this.comboBoxAutoColletRecipeName.FormattingEnabled = true;
            this.comboBoxAutoColletRecipeName.Location = new System.Drawing.Point(162, 11);
            this.comboBoxAutoColletRecipeName.Name = "comboBoxAutoColletRecipeName";
            this.comboBoxAutoColletRecipeName.Size = new System.Drawing.Size(517, 33);
            this.comboBoxAutoColletRecipeName.TabIndex = 1;
            // 
            // buttonStartAutoColletCalibration
            // 
            this.buttonStartAutoColletCalibration.Location = new System.Drawing.Point(23, 63);
            this.buttonStartAutoColletCalibration.Name = "buttonStartAutoColletCalibration";
            this.buttonStartAutoColletCalibration.Size = new System.Drawing.Size(120, 78);
            this.buttonStartAutoColletCalibration.TabIndex = 2;
            this.buttonStartAutoColletCalibration.Text = "Start";
            this.buttonStartAutoColletCalibration.UseVisualStyleBackColor = true;
            // 
            // buttonCloseAutoColletCalibration
            // 
            this.buttonCloseAutoColletCalibration.Location = new System.Drawing.Point(575, 112);
            this.buttonCloseAutoColletCalibration.Name = "buttonCloseAutoColletCalibration";
            this.buttonCloseAutoColletCalibration.Size = new System.Drawing.Size(120, 78);
            this.buttonCloseAutoColletCalibration.TabIndex = 3;
            this.buttonCloseAutoColletCalibration.Text = "Close";
            this.buttonCloseAutoColletCalibration.UseVisualStyleBackColor = true;
            // 
            // ProductFormAutoColletCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.ClientSize = new System.Drawing.Size(707, 202);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCloseAutoColletCalibration);
            this.Controls.Add(this.buttonStartAutoColletCalibration);
            this.Controls.Add(this.comboBoxAutoColletRecipeName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ProductFormAutoColletCalibration";
            this.Text = "Auto Collet Calibration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboBoxAutoColletRecipeName;
        public System.Windows.Forms.Button buttonStartAutoColletCalibration;
        public System.Windows.Forms.Button buttonCloseAutoColletCalibration;
    }
}