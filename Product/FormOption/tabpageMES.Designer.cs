namespace Product
{
    partial class tabpageMES
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
            this.checkBoxEnableMES = new System.Windows.Forms.CheckBox();
            this.groupBoxMES = new System.Windows.Forms.GroupBox();
            this.textBoxMachineNo = new System.Windows.Forms.TextBox();
            this.textBoxEndJobURL = new System.Windows.Forms.TextBox();
            this.labelEndJobURL = new System.Windows.Forms.Label();
            this.labelMachineNo = new System.Windows.Forms.Label();
            this.textBoxOutputURL = new System.Windows.Forms.TextBox();
            this.labelOutputURL = new System.Windows.Forms.Label();
            this.textBoxInputURL = new System.Windows.Forms.TextBox();
            this.labelInputURL = new System.Windows.Forms.Label();
            this.groupBoxCountMethod = new System.Windows.Forms.GroupBox();
            this.radioButtonCountByInputUnitQuantity = new System.Windows.Forms.RadioButton();
            this.radioButtonCountByInputTrayQuantity = new System.Windows.Forms.RadioButton();
            this.groupBoxMES.SuspendLayout();
            this.groupBoxCountMethod.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxEnableMES
            // 
            this.checkBoxEnableMES.AutoSize = true;
            this.checkBoxEnableMES.Location = new System.Drawing.Point(3, 3);
            this.checkBoxEnableMES.Name = "checkBoxEnableMES";
            this.checkBoxEnableMES.Size = new System.Drawing.Size(135, 28);
            this.checkBoxEnableMES.TabIndex = 5;
            this.checkBoxEnableMES.Text = "Enable MES";
            this.checkBoxEnableMES.UseVisualStyleBackColor = true;
            // 
            // groupBoxMES
            // 
            this.groupBoxMES.Controls.Add(this.textBoxMachineNo);
            this.groupBoxMES.Controls.Add(this.textBoxEndJobURL);
            this.groupBoxMES.Controls.Add(this.labelEndJobURL);
            this.groupBoxMES.Controls.Add(this.labelMachineNo);
            this.groupBoxMES.Controls.Add(this.textBoxOutputURL);
            this.groupBoxMES.Controls.Add(this.labelOutputURL);
            this.groupBoxMES.Controls.Add(this.textBoxInputURL);
            this.groupBoxMES.Controls.Add(this.labelInputURL);
            this.groupBoxMES.Location = new System.Drawing.Point(3, 37);
            this.groupBoxMES.Name = "groupBoxMES";
            this.groupBoxMES.Size = new System.Drawing.Size(638, 429);
            this.groupBoxMES.TabIndex = 6;
            this.groupBoxMES.TabStop = false;
            this.groupBoxMES.Text = "MES";
            // 
            // textBoxMachineNo
            // 
            this.textBoxMachineNo.Location = new System.Drawing.Point(128, 25);
            this.textBoxMachineNo.Name = "textBoxMachineNo";
            this.textBoxMachineNo.Size = new System.Drawing.Size(112, 29);
            this.textBoxMachineNo.TabIndex = 11;
            // 
            // textBoxEndJobURL
            // 
            this.textBoxEndJobURL.Location = new System.Drawing.Point(128, 145);
            this.textBoxEndJobURL.Name = "textBoxEndJobURL";
            this.textBoxEndJobURL.Size = new System.Drawing.Size(479, 29);
            this.textBoxEndJobURL.TabIndex = 9;
            // 
            // labelEndJobURL
            // 
            this.labelEndJobURL.AutoSize = true;
            this.labelEndJobURL.Location = new System.Drawing.Point(6, 145);
            this.labelEndJobURL.Name = "labelEndJobURL";
            this.labelEndJobURL.Size = new System.Drawing.Size(122, 24);
            this.labelEndJobURL.TabIndex = 8;
            this.labelEndJobURL.Text = "EndJob URL:";
            // 
            // labelMachineNo
            // 
            this.labelMachineNo.AutoSize = true;
            this.labelMachineNo.Location = new System.Drawing.Point(6, 25);
            this.labelMachineNo.Name = "labelMachineNo";
            this.labelMachineNo.Size = new System.Drawing.Size(118, 24);
            this.labelMachineNo.TabIndex = 10;
            this.labelMachineNo.Text = "Machine No:";
            // 
            // textBoxOutputURL
            // 
            this.textBoxOutputURL.Location = new System.Drawing.Point(128, 105);
            this.textBoxOutputURL.Name = "textBoxOutputURL";
            this.textBoxOutputURL.Size = new System.Drawing.Size(479, 29);
            this.textBoxOutputURL.TabIndex = 7;
            // 
            // labelOutputURL
            // 
            this.labelOutputURL.AutoSize = true;
            this.labelOutputURL.Location = new System.Drawing.Point(6, 105);
            this.labelOutputURL.Name = "labelOutputURL";
            this.labelOutputURL.Size = new System.Drawing.Size(112, 24);
            this.labelOutputURL.TabIndex = 6;
            this.labelOutputURL.Text = "Output URL:";
            // 
            // textBoxInputURL
            // 
            this.textBoxInputURL.Location = new System.Drawing.Point(128, 65);
            this.textBoxInputURL.Name = "textBoxInputURL";
            this.textBoxInputURL.Size = new System.Drawing.Size(479, 29);
            this.textBoxInputURL.TabIndex = 5;
            // 
            // labelInputURL
            // 
            this.labelInputURL.AutoSize = true;
            this.labelInputURL.Location = new System.Drawing.Point(6, 65);
            this.labelInputURL.Name = "labelInputURL";
            this.labelInputURL.Size = new System.Drawing.Size(97, 24);
            this.labelInputURL.TabIndex = 3;
            this.labelInputURL.Text = "Input URL:";
            // 
            // groupBoxCountMethod
            // 
            this.groupBoxCountMethod.Controls.Add(this.radioButtonCountByInputTrayQuantity);
            this.groupBoxCountMethod.Controls.Add(this.radioButtonCountByInputUnitQuantity);
            this.groupBoxCountMethod.Location = new System.Drawing.Point(647, 37);
            this.groupBoxCountMethod.Name = "groupBoxCountMethod";
            this.groupBoxCountMethod.Size = new System.Drawing.Size(244, 122);
            this.groupBoxCountMethod.TabIndex = 7;
            this.groupBoxCountMethod.TabStop = false;
            this.groupBoxCountMethod.Text = "Count By";
            // 
            // radioButtonCountByInputUnitQuantity
            // 
            this.radioButtonCountByInputUnitQuantity.AutoSize = true;
            this.radioButtonCountByInputUnitQuantity.Location = new System.Drawing.Point(6, 28);
            this.radioButtonCountByInputUnitQuantity.Name = "radioButtonCountByInputUnitQuantity";
            this.radioButtonCountByInputUnitQuantity.Size = new System.Drawing.Size(179, 28);
            this.radioButtonCountByInputUnitQuantity.TabIndex = 0;
            this.radioButtonCountByInputUnitQuantity.TabStop = true;
            this.radioButtonCountByInputUnitQuantity.Text = "Input Unit Quantity";
            this.radioButtonCountByInputUnitQuantity.UseVisualStyleBackColor = true;
            // 
            // radioButtonCountByInputTrayQuantity
            // 
            this.radioButtonCountByInputTrayQuantity.AutoSize = true;
            this.radioButtonCountByInputTrayQuantity.Location = new System.Drawing.Point(6, 66);
            this.radioButtonCountByInputTrayQuantity.Name = "radioButtonCountByInputTrayQuantity";
            this.radioButtonCountByInputTrayQuantity.Size = new System.Drawing.Size(138, 28);
            this.radioButtonCountByInputTrayQuantity.TabIndex = 1;
            this.radioButtonCountByInputTrayQuantity.TabStop = true;
            this.radioButtonCountByInputTrayQuantity.Text = "Tray Quantity";
            this.radioButtonCountByInputTrayQuantity.UseVisualStyleBackColor = true;
            // 
            // tabpageMES
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxCountMethod);
            this.Controls.Add(this.groupBoxMES);
            this.Controls.Add(this.checkBoxEnableMES);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "tabpageMES";
            this.Size = new System.Drawing.Size(911, 540);
            this.groupBoxMES.ResumeLayout(false);
            this.groupBoxMES.PerformLayout();
            this.groupBoxCountMethod.ResumeLayout(false);
            this.groupBoxCountMethod.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.CheckBox checkBoxEnableMES;
        public System.Windows.Forms.GroupBox groupBoxMES;
        public System.Windows.Forms.TextBox textBoxEndJobURL;
        public System.Windows.Forms.Label labelEndJobURL;
        public System.Windows.Forms.TextBox textBoxOutputURL;
        public System.Windows.Forms.Label labelOutputURL;
        public System.Windows.Forms.TextBox textBoxInputURL;
        public System.Windows.Forms.Label labelInputURL;
        public System.Windows.Forms.TextBox textBoxMachineNo;
        public System.Windows.Forms.Label labelMachineNo;
        private System.Windows.Forms.GroupBox groupBoxCountMethod;
        public System.Windows.Forms.RadioButton radioButtonCountByInputTrayQuantity;
        public System.Windows.Forms.RadioButton radioButtonCountByInputUnitQuantity;
    }
}
