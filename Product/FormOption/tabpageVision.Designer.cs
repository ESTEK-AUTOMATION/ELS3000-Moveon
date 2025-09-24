namespace Product
{
    partial class tabpageVision
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
            this.groupBoxVision = new System.Windows.Forms.GroupBox();
            this.checkBoxEnableVisionWaitResult = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableS3Vision = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableSWFRVision = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableSWLRVision = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableSetupVision = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableS2Vision = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableInputVision = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableOutputVision = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableBottomVision = new System.Windows.Forms.CheckBox();
            this.groupBoxInputVision = new System.Windows.Forms.GroupBox();
            this.numericUpDownInputVisionFOVWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownOutputVisionFOVHeight = new System.Windows.Forms.NumericUpDown();
            this.textBoxInputVisionUnitOfMeasurement = new System.Windows.Forms.TextBox();
            this.checkBoxEnableLaunchVisionSoftware = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableVision = new System.Windows.Forms.CheckBox();
            this.groupBoxVision.SuspendLayout();
            this.groupBoxInputVision.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputVisionFOVWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOutputVisionFOVHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxVision
            // 
            this.groupBoxVision.Controls.Add(this.checkBoxEnableVisionWaitResult);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableS3Vision);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableSWFRVision);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableSWLRVision);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableSetupVision);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableS2Vision);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableInputVision);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableOutputVision);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableBottomVision);
            this.groupBoxVision.Controls.Add(this.groupBoxInputVision);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableLaunchVisionSoftware);
            this.groupBoxVision.Controls.Add(this.checkBoxEnableVision);
            this.groupBoxVision.Location = new System.Drawing.Point(3, 3);
            this.groupBoxVision.Name = "groupBoxVision";
            this.groupBoxVision.Size = new System.Drawing.Size(700, 661);
            this.groupBoxVision.TabIndex = 1;
            this.groupBoxVision.TabStop = false;
            this.groupBoxVision.Text = "Vision";
            // 
            // checkBoxEnableVisionWaitResult
            // 
            this.checkBoxEnableVisionWaitResult.AutoSize = true;
            this.checkBoxEnableVisionWaitResult.Location = new System.Drawing.Point(6, 65);
            this.checkBoxEnableVisionWaitResult.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxEnableVisionWaitResult.Name = "checkBoxEnableVisionWaitResult";
            this.checkBoxEnableVisionWaitResult.Size = new System.Drawing.Size(492, 28);
            this.checkBoxEnableVisionWaitResult.TabIndex = 125;
            this.checkBoxEnableVisionWaitResult.Text = "Enable Wait Vision Inspection Result During Production";
            this.checkBoxEnableVisionWaitResult.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableS3Vision
            // 
            this.checkBoxEnableS3Vision.AutoSize = true;
            this.checkBoxEnableS3Vision.Location = new System.Drawing.Point(3, 324);
            this.checkBoxEnableS3Vision.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxEnableS3Vision.Name = "checkBoxEnableS3Vision";
            this.checkBoxEnableS3Vision.Size = new System.Drawing.Size(173, 28);
            this.checkBoxEnableS3Vision.TabIndex = 124;
            this.checkBoxEnableS3Vision.Text = "Enable S3 Vision";
            this.checkBoxEnableS3Vision.UseVisualStyleBackColor = true;
            this.checkBoxEnableS3Vision.Visible = false;
            // 
            // checkBoxEnableSWFRVision
            // 
            this.checkBoxEnableSWFRVision.AutoSize = true;
            this.checkBoxEnableSWFRVision.Location = new System.Drawing.Point(3, 279);
            this.checkBoxEnableSWFRVision.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxEnableSWFRVision.Name = "checkBoxEnableSWFRVision";
            this.checkBoxEnableSWFRVision.Size = new System.Drawing.Size(323, 28);
            this.checkBoxEnableSWFRVision.TabIndex = 123;
            this.checkBoxEnableSWFRVision.Text = "Enable Sidewall Top Bottom Vision";
            this.checkBoxEnableSWFRVision.UseVisualStyleBackColor = true;
            this.checkBoxEnableSWFRVision.Visible = false;
            // 
            // checkBoxEnableSWLRVision
            // 
            this.checkBoxEnableSWLRVision.AutoSize = true;
            this.checkBoxEnableSWLRVision.Location = new System.Drawing.Point(3, 234);
            this.checkBoxEnableSWLRVision.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxEnableSWLRVision.Name = "checkBoxEnableSWLRVision";
            this.checkBoxEnableSWLRVision.Size = new System.Drawing.Size(303, 28);
            this.checkBoxEnableSWLRVision.TabIndex = 122;
            this.checkBoxEnableSWLRVision.Text = "Enable Sidewall Left Right Vision";
            this.checkBoxEnableSWLRVision.UseVisualStyleBackColor = true;
            this.checkBoxEnableSWLRVision.Visible = false;
            // 
            // checkBoxEnableSetupVision
            // 
            this.checkBoxEnableSetupVision.AutoSize = true;
            this.checkBoxEnableSetupVision.Location = new System.Drawing.Point(3, 189);
            this.checkBoxEnableSetupVision.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxEnableSetupVision.Name = "checkBoxEnableSetupVision";
            this.checkBoxEnableSetupVision.Size = new System.Drawing.Size(200, 28);
            this.checkBoxEnableSetupVision.TabIndex = 121;
            this.checkBoxEnableSetupVision.Text = "Enable Setup Vision";
            this.checkBoxEnableSetupVision.UseVisualStyleBackColor = true;
            this.checkBoxEnableSetupVision.Visible = false;
            // 
            // checkBoxEnableS2Vision
            // 
            this.checkBoxEnableS2Vision.AutoSize = true;
            this.checkBoxEnableS2Vision.Location = new System.Drawing.Point(3, 144);
            this.checkBoxEnableS2Vision.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxEnableS2Vision.Name = "checkBoxEnableS2Vision";
            this.checkBoxEnableS2Vision.Size = new System.Drawing.Size(173, 28);
            this.checkBoxEnableS2Vision.TabIndex = 120;
            this.checkBoxEnableS2Vision.Text = "Enable S2 Vision";
            this.checkBoxEnableS2Vision.UseVisualStyleBackColor = true;
            this.checkBoxEnableS2Vision.Visible = false;
            // 
            // checkBoxEnableInputVision
            // 
            this.checkBoxEnableInputVision.AutoSize = true;
            this.checkBoxEnableInputVision.Location = new System.Drawing.Point(3, 99);
            this.checkBoxEnableInputVision.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxEnableInputVision.Name = "checkBoxEnableInputVision";
            this.checkBoxEnableInputVision.Size = new System.Drawing.Size(192, 28);
            this.checkBoxEnableInputVision.TabIndex = 113;
            this.checkBoxEnableInputVision.Text = "Enable Input Vision";
            this.checkBoxEnableInputVision.UseVisualStyleBackColor = true;
            this.checkBoxEnableInputVision.Visible = false;
            // 
            // checkBoxEnableOutputVision
            // 
            this.checkBoxEnableOutputVision.AutoSize = true;
            this.checkBoxEnableOutputVision.Location = new System.Drawing.Point(3, 414);
            this.checkBoxEnableOutputVision.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxEnableOutputVision.Name = "checkBoxEnableOutputVision";
            this.checkBoxEnableOutputVision.Size = new System.Drawing.Size(207, 28);
            this.checkBoxEnableOutputVision.TabIndex = 119;
            this.checkBoxEnableOutputVision.Text = "Enable Output Vision";
            this.checkBoxEnableOutputVision.UseVisualStyleBackColor = true;
            this.checkBoxEnableOutputVision.Visible = false;
            // 
            // checkBoxEnableBottomVision
            // 
            this.checkBoxEnableBottomVision.AutoSize = true;
            this.checkBoxEnableBottomVision.Location = new System.Drawing.Point(3, 369);
            this.checkBoxEnableBottomVision.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxEnableBottomVision.Name = "checkBoxEnableBottomVision";
            this.checkBoxEnableBottomVision.Size = new System.Drawing.Size(209, 28);
            this.checkBoxEnableBottomVision.TabIndex = 115;
            this.checkBoxEnableBottomVision.Text = "Enable Bottom Vision";
            this.checkBoxEnableBottomVision.UseVisualStyleBackColor = true;
            this.checkBoxEnableBottomVision.Visible = false;
            // 
            // groupBoxInputVision
            // 
            this.groupBoxInputVision.Controls.Add(this.numericUpDownInputVisionFOVWidth);
            this.groupBoxInputVision.Controls.Add(this.label1);
            this.groupBoxInputVision.Controls.Add(this.label2);
            this.groupBoxInputVision.Controls.Add(this.label3);
            this.groupBoxInputVision.Controls.Add(this.numericUpDownOutputVisionFOVHeight);
            this.groupBoxInputVision.Controls.Add(this.textBoxInputVisionUnitOfMeasurement);
            this.groupBoxInputVision.Location = new System.Drawing.Point(6, 456);
            this.groupBoxInputVision.Name = "groupBoxInputVision";
            this.groupBoxInputVision.Size = new System.Drawing.Size(469, 197);
            this.groupBoxInputVision.TabIndex = 8;
            this.groupBoxInputVision.TabStop = false;
            this.groupBoxInputVision.Text = "Input Vision";
            this.groupBoxInputVision.Visible = false;
            // 
            // numericUpDownInputVisionFOVWidth
            // 
            this.numericUpDownInputVisionFOVWidth.Location = new System.Drawing.Point(299, 56);
            this.numericUpDownInputVisionFOVWidth.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownInputVisionFOVWidth.Name = "numericUpDownInputVisionFOVWidth";
            this.numericUpDownInputVisionFOVWidth.Size = new System.Drawing.Size(159, 29);
            this.numericUpDownInputVisionFOVWidth.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(206, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input Vision FOV Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(213, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "Input Vision FOV Height";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(286, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Input Vision Unit of Measurement";
            // 
            // numericUpDownOutputVisionFOVHeight
            // 
            this.numericUpDownOutputVisionFOVHeight.Location = new System.Drawing.Point(299, 95);
            this.numericUpDownOutputVisionFOVHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownOutputVisionFOVHeight.Name = "numericUpDownOutputVisionFOVHeight";
            this.numericUpDownOutputVisionFOVHeight.Size = new System.Drawing.Size(159, 29);
            this.numericUpDownOutputVisionFOVHeight.TabIndex = 6;
            // 
            // textBoxInputVisionUnitOfMeasurement
            // 
            this.textBoxInputVisionUnitOfMeasurement.Location = new System.Drawing.Point(299, 135);
            this.textBoxInputVisionUnitOfMeasurement.Name = "textBoxInputVisionUnitOfMeasurement";
            this.textBoxInputVisionUnitOfMeasurement.Size = new System.Drawing.Size(159, 29);
            this.textBoxInputVisionUnitOfMeasurement.TabIndex = 7;
            // 
            // checkBoxEnableLaunchVisionSoftware
            // 
            this.checkBoxEnableLaunchVisionSoftware.AutoSize = true;
            this.checkBoxEnableLaunchVisionSoftware.Location = new System.Drawing.Point(6, 62);
            this.checkBoxEnableLaunchVisionSoftware.Name = "checkBoxEnableLaunchVisionSoftware";
            this.checkBoxEnableLaunchVisionSoftware.Size = new System.Drawing.Size(291, 28);
            this.checkBoxEnableLaunchVisionSoftware.TabIndex = 1;
            this.checkBoxEnableLaunchVisionSoftware.Text = "Enable Launch Vision Software";
            this.checkBoxEnableLaunchVisionSoftware.UseVisualStyleBackColor = true;
            this.checkBoxEnableLaunchVisionSoftware.Visible = false;
            // 
            // checkBoxEnableVision
            // 
            this.checkBoxEnableVision.AutoSize = true;
            this.checkBoxEnableVision.Location = new System.Drawing.Point(6, 28);
            this.checkBoxEnableVision.Name = "checkBoxEnableVision";
            this.checkBoxEnableVision.Size = new System.Drawing.Size(146, 28);
            this.checkBoxEnableVision.TabIndex = 0;
            this.checkBoxEnableVision.Text = "Enable Vision";
            this.checkBoxEnableVision.UseVisualStyleBackColor = true;
            // 
            // tabpageVision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBoxVision);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "tabpageVision";
            this.Size = new System.Drawing.Size(722, 674);
            this.groupBoxVision.ResumeLayout(false);
            this.groupBoxVision.PerformLayout();
            this.groupBoxInputVision.ResumeLayout(false);
            this.groupBoxInputVision.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputVisionFOVWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOutputVisionFOVHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox groupBoxVision;
        public System.Windows.Forms.CheckBox checkBoxEnableLaunchVisionSoftware;
        public System.Windows.Forms.CheckBox checkBoxEnableVision;
        public System.Windows.Forms.TextBox textBoxInputVisionUnitOfMeasurement;
        public System.Windows.Forms.NumericUpDown numericUpDownOutputVisionFOVHeight;
        public System.Windows.Forms.NumericUpDown numericUpDownInputVisionFOVWidth;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxInputVision;
        public System.Windows.Forms.CheckBox checkBoxEnableOutputVision;
        public System.Windows.Forms.CheckBox checkBoxEnableBottomVision;
        public System.Windows.Forms.CheckBox checkBoxEnableInputVision;
        public System.Windows.Forms.CheckBox checkBoxEnableSWFRVision;
        public System.Windows.Forms.CheckBox checkBoxEnableSWLRVision;
        public System.Windows.Forms.CheckBox checkBoxEnableSetupVision;
        public System.Windows.Forms.CheckBox checkBoxEnableS2Vision;
        public System.Windows.Forms.CheckBox checkBoxEnableS3Vision;
        public System.Windows.Forms.CheckBox checkBoxEnableVisionWaitResult;
    }
}
