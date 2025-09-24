namespace Product
{
    partial class ProductFormCalibration
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxGrayScaleCalibrationRecipeName = new System.Windows.Forms.ComboBox();
            this.buttonStop = new System.Windows.Forms.Button();
            this.btnInputStationCalibration = new System.Windows.Forms.Button();
            this.btnS2S3StationCalibration = new System.Windows.Forms.Button();
            this.btnS1BTMStationCalibration = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.timerReadRTS = new System.Windows.Forms.Timer(this.components);
            this.timerCheckOpen = new System.Windows.Forms.Timer(this.components);
            this.btnOutputStationCalibration = new System.Windows.Forms.Button();
            this.comboBoxCalibrationType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudAngle = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nudBottomXOffset = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.nudBottomYOffset = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.cmbPnPSelect = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnMoveToBottom = new System.Windows.Forms.Button();
            this.btnVacuum1 = new System.Windows.Forms.Button();
            this.btnVacuum2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBottomXOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBottomYOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(567, 406);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Vision Recipe:";
            this.label1.Visible = false;
            // 
            // comboBoxGrayScaleCalibrationRecipeName
            // 
            this.comboBoxGrayScaleCalibrationRecipeName.FormattingEnabled = true;
            this.comboBoxGrayScaleCalibrationRecipeName.Location = new System.Drawing.Point(729, 400);
            this.comboBoxGrayScaleCalibrationRecipeName.Name = "comboBoxGrayScaleCalibrationRecipeName";
            this.comboBoxGrayScaleCalibrationRecipeName.Size = new System.Drawing.Size(517, 32);
            this.comboBoxGrayScaleCalibrationRecipeName.TabIndex = 2;
            this.comboBoxGrayScaleCalibrationRecipeName.Visible = false;
            // 
            // buttonStop
            // 
            this.buttonStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStop.Location = new System.Drawing.Point(572, 628);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(82, 62);
            this.buttonStop.TabIndex = 22;
            this.buttonStop.Text = "STOP";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Visible = false;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // btnInputStationCalibration
            // 
            this.btnInputStationCalibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInputStationCalibration.Location = new System.Drawing.Point(572, 532);
            this.btnInputStationCalibration.Name = "btnInputStationCalibration";
            this.btnInputStationCalibration.Size = new System.Drawing.Size(96, 90);
            this.btnInputStationCalibration.TabIndex = 19;
            this.btnInputStationCalibration.Text = "Input";
            this.btnInputStationCalibration.UseVisualStyleBackColor = true;
            this.btnInputStationCalibration.Visible = false;
            this.btnInputStationCalibration.Click += new System.EventHandler(this.btnInputStationCalibration_Click);
            // 
            // btnS2S3StationCalibration
            // 
            this.btnS2S3StationCalibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnS2S3StationCalibration.Location = new System.Drawing.Point(888, 532);
            this.btnS2S3StationCalibration.Name = "btnS2S3StationCalibration";
            this.btnS2S3StationCalibration.Size = new System.Drawing.Size(96, 90);
            this.btnS2S3StationCalibration.TabIndex = 18;
            this.btnS2S3StationCalibration.Text = "S2 S3";
            this.btnS2S3StationCalibration.UseVisualStyleBackColor = true;
            this.btnS2S3StationCalibration.Visible = false;
            this.btnS2S3StationCalibration.Click += new System.EventHandler(this.btnS2S3StationCalibration_Click);
            // 
            // btnS1BTMStationCalibration
            // 
            this.btnS1BTMStationCalibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnS1BTMStationCalibration.Location = new System.Drawing.Point(724, 532);
            this.btnS1BTMStationCalibration.Name = "btnS1BTMStationCalibration";
            this.btnS1BTMStationCalibration.Size = new System.Drawing.Size(96, 90);
            this.btnS1BTMStationCalibration.TabIndex = 17;
            this.btnS1BTMStationCalibration.Text = "S1 BTM";
            this.btnS1BTMStationCalibration.UseVisualStyleBackColor = true;
            this.btnS1BTMStationCalibration.Visible = false;
            this.btnS1BTMStationCalibration.Click += new System.EventHandler(this.btnS1BTMStationCalibration_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.Location = new System.Drawing.Point(616, 302);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(82, 62);
            this.buttonClose.TabIndex = 23;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // timerReadRTS
            // 
            this.timerReadRTS.Enabled = true;
            this.timerReadRTS.Tick += new System.EventHandler(this.timerReadRTS_Tick);
            // 
            // timerCheckOpen
            // 
            this.timerCheckOpen.Enabled = true;
            this.timerCheckOpen.Tick += new System.EventHandler(this.timerCheckOpen_Tick);
            // 
            // btnOutputStationCalibration
            // 
            this.btnOutputStationCalibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOutputStationCalibration.Location = new System.Drawing.Point(1049, 532);
            this.btnOutputStationCalibration.Name = "btnOutputStationCalibration";
            this.btnOutputStationCalibration.Size = new System.Drawing.Size(96, 90);
            this.btnOutputStationCalibration.TabIndex = 27;
            this.btnOutputStationCalibration.Text = "Output";
            this.btnOutputStationCalibration.UseVisualStyleBackColor = true;
            this.btnOutputStationCalibration.Visible = false;
            this.btnOutputStationCalibration.Click += new System.EventHandler(this.btnOutputStationCalibration_Click);
            // 
            // comboBoxCalibrationType
            // 
            this.comboBoxCalibrationType.FormattingEnabled = true;
            this.comboBoxCalibrationType.Items.AddRange(new object[] {
            "DotGrid",
            "GrayScale"});
            this.comboBoxCalibrationType.Location = new System.Drawing.Point(729, 448);
            this.comboBoxCalibrationType.Name = "comboBoxCalibrationType";
            this.comboBoxCalibrationType.Size = new System.Drawing.Size(255, 32);
            this.comboBoxCalibrationType.TabIndex = 30;
            this.comboBoxCalibrationType.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(567, 454);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 20);
            this.label2.TabIndex = 29;
            this.label2.Text = "Calibration Type:";
            this.label2.Visible = false;
            // 
            // nudAngle
            // 
            this.nudAngle.Location = new System.Drawing.Point(179, 24);
            this.nudAngle.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudAngle.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.nudAngle.Name = "nudAngle";
            this.nudAngle.Size = new System.Drawing.Size(238, 29);
            this.nudAngle.TabIndex = 31;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 24);
            this.label3.TabIndex = 32;
            this.label3.Text = "Angle:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(433, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 24);
            this.label4.TabIndex = 33;
            this.label4.Text = "(md)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 24);
            this.label5.TabIndex = 34;
            this.label5.Text = "Bottom X Offset:";
            // 
            // nudBottomXOffset
            // 
            this.nudBottomXOffset.Location = new System.Drawing.Point(179, 68);
            this.nudBottomXOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudBottomXOffset.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.nudBottomXOffset.Name = "nudBottomXOffset";
            this.nudBottomXOffset.Size = new System.Drawing.Size(238, 29);
            this.nudBottomXOffset.TabIndex = 35;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(433, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 24);
            this.label6.TabIndex = 36;
            this.label6.Text = "(um)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(433, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 24);
            this.label7.TabIndex = 39;
            this.label7.Text = "(um)";
            // 
            // nudBottomYOffset
            // 
            this.nudBottomYOffset.Location = new System.Drawing.Point(179, 112);
            this.nudBottomYOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudBottomYOffset.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.nudBottomYOffset.Name = "nudBottomYOffset";
            this.nudBottomYOffset.Size = new System.Drawing.Size(238, 29);
            this.nudBottomYOffset.TabIndex = 38;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 24);
            this.label8.TabIndex = 37;
            this.label8.Text = "Bottom Y Offset:";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(16, 302);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(82, 62);
            this.btnStart.TabIndex = 40;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // cmbPnPSelect
            // 
            this.cmbPnPSelect.FormattingEnabled = true;
            this.cmbPnPSelect.Items.AddRange(new object[] {
            "1",
            "2"});
            this.cmbPnPSelect.Location = new System.Drawing.Point(179, 160);
            this.cmbPnPSelect.Name = "cmbPnPSelect";
            this.cmbPnPSelect.Size = new System.Drawing.Size(55, 32);
            this.cmbPnPSelect.TabIndex = 41;
            this.cmbPnPSelect.SelectedIndexChanged += new System.EventHandler(this.cmbPnPSelect_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 163);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(111, 24);
            this.label9.TabIndex = 42;
            this.label9.Text = "PnP Sleelct:";
            // 
            // btnMoveToBottom
            // 
            this.btnMoveToBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveToBottom.Location = new System.Drawing.Point(308, 302);
            this.btnMoveToBottom.Name = "btnMoveToBottom";
            this.btnMoveToBottom.Size = new System.Drawing.Size(82, 62);
            this.btnMoveToBottom.TabIndex = 43;
            this.btnMoveToBottom.Text = "Move Bottom";
            this.btnMoveToBottom.UseVisualStyleBackColor = true;
            this.btnMoveToBottom.Click += new System.EventHandler(this.btnMoveToBottom_Click);
            // 
            // btnVacuum1
            // 
            this.btnVacuum1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVacuum1.Location = new System.Drawing.Point(494, 7);
            this.btnVacuum1.Name = "btnVacuum1";
            this.btnVacuum1.Size = new System.Drawing.Size(103, 62);
            this.btnVacuum1.TabIndex = 44;
            this.btnVacuum1.Text = "Vacuum On 1";
            this.btnVacuum1.UseVisualStyleBackColor = true;
            this.btnVacuum1.Click += new System.EventHandler(this.btnVacuum1_Click);
            // 
            // btnVacuum2
            // 
            this.btnVacuum2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVacuum2.Location = new System.Drawing.Point(603, 7);
            this.btnVacuum2.Name = "btnVacuum2";
            this.btnVacuum2.Size = new System.Drawing.Size(103, 62);
            this.btnVacuum2.TabIndex = 45;
            this.btnVacuum2.Text = "Vacuum On 2";
            this.btnVacuum2.UseVisualStyleBackColor = true;
            this.btnVacuum2.Click += new System.EventHandler(this.btnVacuum2_Click);
            // 
            // ProductFormCalibration
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.ClientSize = new System.Drawing.Size(718, 380);
            this.ControlBox = false;
            this.Controls.Add(this.btnVacuum2);
            this.Controls.Add(this.btnVacuum1);
            this.Controls.Add(this.btnMoveToBottom);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmbPnPSelect);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.nudBottomYOffset);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nudBottomXOffset);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudAngle);
            this.Controls.Add(this.comboBoxCalibrationType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOutputStationCalibration);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.btnInputStationCalibration);
            this.Controls.Add(this.btnS2S3StationCalibration);
            this.Controls.Add(this.btnS1BTMStationCalibration);
            this.Controls.Add(this.comboBoxGrayScaleCalibrationRecipeName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ProductFormCalibration";
            this.Text = "Dot Grid Calibration";
            this.Load += new System.EventHandler(this.ProductFormDotGridCalibration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBottomXOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBottomYOffset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboBoxGrayScaleCalibrationRecipeName;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button btnInputStationCalibration;
        private System.Windows.Forms.Button btnS2S3StationCalibration;
        private System.Windows.Forms.Button btnS1BTMStationCalibration;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Timer timerReadRTS;
        private System.Windows.Forms.Timer timerCheckOpen;
        private System.Windows.Forms.Button btnOutputStationCalibration;
        public System.Windows.Forms.ComboBox comboBoxCalibrationType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudAngle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudBottomXOffset;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudBottomYOffset;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ComboBox cmbPnPSelect;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnMoveToBottom;
        private System.Windows.Forms.Button btnVacuum1;
        private System.Windows.Forms.Button btnVacuum2;
    }
}