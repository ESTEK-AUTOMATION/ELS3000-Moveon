namespace Product
{
    partial class groupboxCustomerInput
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelProductPartNumber = new System.Windows.Forms.Label();
            this.textBoxProductPartNumber = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelProductPartNumber);
            this.groupBox1.Controls.Add(this.textBoxProductPartNumber);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox1.Size = new System.Drawing.Size(604, 45);
            this.groupBox1.TabIndex = 74;
            this.groupBox1.TabStop = false;
            // 
            // labelProductPartNumber
            // 
            this.labelProductPartNumber.AutoSize = true;
            this.labelProductPartNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProductPartNumber.Location = new System.Drawing.Point(3, 13);
            this.labelProductPartNumber.Name = "labelProductPartNumber";
            this.labelProductPartNumber.Size = new System.Drawing.Size(165, 20);
            this.labelProductPartNumber.TabIndex = 7;
            this.labelProductPartNumber.Text = "Product Part Number :";
            this.labelProductPartNumber.Visible = false;
            // 
            // textBoxProductPartNumber
            // 
            this.textBoxProductPartNumber.Visible = false;
            this.textBoxProductPartNumber.Enabled = false;
            this.textBoxProductPartNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxProductPartNumber.Location = new System.Drawing.Point(177, 13);
            this.textBoxProductPartNumber.Name = "textBoxProductPartNumber";
            this.textBoxProductPartNumber.Size = new System.Drawing.Size(416, 26);
            this.textBoxProductPartNumber.TabIndex = 8;
            // 
            // groupboxCustomerInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.Controls.Add(this.groupBox1);
            this.Name = "groupboxCustomerInput";
            this.Size = new System.Drawing.Size(611, 53);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Label labelProductPartNumber;
        public System.Windows.Forms.TextBox textBoxProductPartNumber;
    }
}
