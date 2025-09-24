namespace Product
{
    partial class ProductFormAutoForceGaugeCalibration
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCalibration = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.btnOutputSpecailCarrierCalibration = new System.Windows.Forms.Button();
            this.btnOutputSoftTrayCalibration = new System.Windows.Forms.Button();
            this.btnInputSoftTrayCalibration = new System.Windows.Forms.Button();
            this.btnOutputJedecTrayCalibration = new System.Windows.Forms.Button();
            this.btnInputJedecTrayCalibration = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxPickUpHeadChoosing = new System.Windows.Forms.ComboBox();
            this.timerReadRTS = new System.Windows.Forms.Timer(this.components);
            this.timerCheckOpen = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCalibration);
            this.panel1.Controls.Add(this.buttonStop);
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Controls.Add(this.btnOutputSpecailCarrierCalibration);
            this.panel1.Controls.Add(this.btnOutputSoftTrayCalibration);
            this.panel1.Controls.Add(this.btnInputSoftTrayCalibration);
            this.panel1.Controls.Add(this.btnOutputJedecTrayCalibration);
            this.panel1.Controls.Add(this.btnInputJedecTrayCalibration);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comboBoxPickUpHeadChoosing);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(691, 383);
            this.panel1.TabIndex = 0;
            // 
            // btnCalibration
            // 
            this.btnCalibration.Location = new System.Drawing.Point(28, 12);
            this.btnCalibration.Name = "btnCalibration";
            this.btnCalibration.Size = new System.Drawing.Size(124, 90);
            this.btnCalibration.TabIndex = 17;
            this.btnCalibration.Text = "Calibration";
            this.btnCalibration.UseVisualStyleBackColor = true;
            this.btnCalibration.Click += new System.EventHandler(this.btnCalibration_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(474, 148);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(82, 62);
            this.buttonStop.TabIndex = 16;
            this.buttonStop.Text = "STOP";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Visible = false;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(234, 26);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(82, 62);
            this.buttonClose.TabIndex = 15;
            this.buttonClose.Text = "CLOSE";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // btnOutputSpecailCarrierCalibration
            // 
            this.btnOutputSpecailCarrierCalibration.Location = new System.Drawing.Point(562, 216);
            this.btnOutputSpecailCarrierCalibration.Name = "btnOutputSpecailCarrierCalibration";
            this.btnOutputSpecailCarrierCalibration.Size = new System.Drawing.Size(96, 90);
            this.btnOutputSpecailCarrierCalibration.TabIndex = 13;
            this.btnOutputSpecailCarrierCalibration.Text = "OPSCT";
            this.btnOutputSpecailCarrierCalibration.UseVisualStyleBackColor = true;
            this.btnOutputSpecailCarrierCalibration.Visible = false;
            this.btnOutputSpecailCarrierCalibration.Click += new System.EventHandler(this.btnOutputSpecailCarrierCalibration_Click);
            // 
            // btnOutputSoftTrayCalibration
            // 
            this.btnOutputSoftTrayCalibration.Location = new System.Drawing.Point(428, 216);
            this.btnOutputSoftTrayCalibration.Name = "btnOutputSoftTrayCalibration";
            this.btnOutputSoftTrayCalibration.Size = new System.Drawing.Size(96, 90);
            this.btnOutputSoftTrayCalibration.TabIndex = 12;
            this.btnOutputSoftTrayCalibration.Text = "OPSFT";
            this.btnOutputSoftTrayCalibration.UseVisualStyleBackColor = true;
            this.btnOutputSoftTrayCalibration.Visible = false;
            this.btnOutputSoftTrayCalibration.Click += new System.EventHandler(this.btnOutputSoftTrayCalibration_Click);
            // 
            // btnInputSoftTrayCalibration
            // 
            this.btnInputSoftTrayCalibration.Location = new System.Drawing.Point(28, 216);
            this.btnInputSoftTrayCalibration.Name = "btnInputSoftTrayCalibration";
            this.btnInputSoftTrayCalibration.Size = new System.Drawing.Size(96, 90);
            this.btnInputSoftTrayCalibration.TabIndex = 11;
            this.btnInputSoftTrayCalibration.Text = "IPSFT";
            this.btnInputSoftTrayCalibration.UseVisualStyleBackColor = true;
            this.btnInputSoftTrayCalibration.Visible = false;
            this.btnInputSoftTrayCalibration.Click += new System.EventHandler(this.btnInputSoftTrayCalibration_Click);
            // 
            // btnOutputJedecTrayCalibration
            // 
            this.btnOutputJedecTrayCalibration.Location = new System.Drawing.Point(292, 216);
            this.btnOutputJedecTrayCalibration.Name = "btnOutputJedecTrayCalibration";
            this.btnOutputJedecTrayCalibration.Size = new System.Drawing.Size(96, 90);
            this.btnOutputJedecTrayCalibration.TabIndex = 10;
            this.btnOutputJedecTrayCalibration.Text = "OPJDT";
            this.btnOutputJedecTrayCalibration.UseVisualStyleBackColor = true;
            this.btnOutputJedecTrayCalibration.Visible = false;
            this.btnOutputJedecTrayCalibration.Click += new System.EventHandler(this.btnOutputJedecTrayCalibration_Click);
            // 
            // btnInputJedecTrayCalibration
            // 
            this.btnInputJedecTrayCalibration.Location = new System.Drawing.Point(160, 216);
            this.btnInputJedecTrayCalibration.Name = "btnInputJedecTrayCalibration";
            this.btnInputJedecTrayCalibration.Size = new System.Drawing.Size(96, 90);
            this.btnInputJedecTrayCalibration.TabIndex = 9;
            this.btnInputJedecTrayCalibration.Text = "IPJDT";
            this.btnInputJedecTrayCalibration.UseVisualStyleBackColor = true;
            this.btnInputJedecTrayCalibration.Visible = false;
            this.btnInputJedecTrayCalibration.Click += new System.EventHandler(this.btnInputJedecTrayCalibration_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 8;
            this.label1.Text = "Pick Up Head:";
            this.label1.Visible = false;
            // 
            // comboBoxPickUpHeadChoosing
            // 
            this.comboBoxPickUpHeadChoosing.FormattingEnabled = true;
            this.comboBoxPickUpHeadChoosing.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.comboBoxPickUpHeadChoosing.Location = new System.Drawing.Point(221, 164);
            this.comboBoxPickUpHeadChoosing.Name = "comboBoxPickUpHeadChoosing";
            this.comboBoxPickUpHeadChoosing.Size = new System.Drawing.Size(79, 32);
            this.comboBoxPickUpHeadChoosing.TabIndex = 7;
            this.comboBoxPickUpHeadChoosing.Visible = false;
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
            // ProductFormAutoForceGaugeCalibration
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.ClientSize = new System.Drawing.Size(691, 383);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ProductFormAutoForceGaugeCalibration";
            this.Text = "Auto Cross Hair Calibration";
            this.Load += new System.EventHandler(this.ProductFormAutoForceGaugeCalibration_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOutputSpecailCarrierCalibration;
        private System.Windows.Forms.Button btnOutputSoftTrayCalibration;
        private System.Windows.Forms.Button btnInputSoftTrayCalibration;
        private System.Windows.Forms.Button btnOutputJedecTrayCalibration;
        private System.Windows.Forms.Button btnInputJedecTrayCalibration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxPickUpHeadChoosing;
        private System.Windows.Forms.Timer timerReadRTS;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Timer timerCheckOpen;
        private System.Windows.Forms.Button btnCalibration;
    }
}