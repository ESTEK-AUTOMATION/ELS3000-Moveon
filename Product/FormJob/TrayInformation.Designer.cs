namespace Product
{
    partial class TrayInformation
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
            this.label1 = new System.Windows.Forms.Label();
            this.labelOutputQuantity = new System.Windows.Forms.Label();
            this.labelRejectTray1Quantity = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelCurrentOutput = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelCurrentTrayNo = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelCurrentInputQuantity = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelInputQuantity = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelInputTrayQuantity = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelInputLotQuantity = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.labelInputLotID = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(246, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total Output Quantity Done: ";
            // 
            // labelOutputQuantity
            // 
            this.labelOutputQuantity.AutoSize = true;
            this.labelOutputQuantity.Location = new System.Drawing.Point(389, 30);
            this.labelOutputQuantity.Name = "labelOutputQuantity";
            this.labelOutputQuantity.Size = new System.Drawing.Size(60, 24);
            this.labelOutputQuantity.TabIndex = 1;
            this.labelOutputQuantity.Text = "label2";
            // 
            // labelRejectTray1Quantity
            // 
            this.labelRejectTray1Quantity.AutoSize = true;
            this.labelRejectTray1Quantity.Location = new System.Drawing.Point(389, 100);
            this.labelRejectTray1Quantity.Name = "labelRejectTray1Quantity";
            this.labelRejectTray1Quantity.Size = new System.Drawing.Size(60, 24);
            this.labelRejectTray1Quantity.TabIndex = 3;
            this.labelRejectTray1Quantity.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(255, 24);
            this.label3.TabIndex = 2;
            this.label3.Text = "Current Reject Tray Quantity: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelCurrentOutput);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.labelOutputQuantity);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.labelRejectTray1Quantity);
            this.groupBox1.Location = new System.Drawing.Point(3, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(455, 313);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output Info";
            // 
            // labelCurrentOutput
            // 
            this.labelCurrentOutput.AutoSize = true;
            this.labelCurrentOutput.Location = new System.Drawing.Point(389, 65);
            this.labelCurrentOutput.Name = "labelCurrentOutput";
            this.labelCurrentOutput.Size = new System.Drawing.Size(60, 24);
            this.labelCurrentOutput.TabIndex = 13;
            this.labelCurrentOutput.Text = "label2";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 65);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(289, 24);
            this.label10.TabIndex = 12;
            this.label10.Text = "Output Quantity On Current Tray: ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelCurrentTrayNo);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.labelCurrentInputQuantity);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.labelInputQuantity);
            this.groupBox2.Location = new System.Drawing.Point(464, 171);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(455, 169);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input Info";
            // 
            // labelCurrentTrayNo
            // 
            this.labelCurrentTrayNo.AutoSize = true;
            this.labelCurrentTrayNo.Location = new System.Drawing.Point(389, 100);
            this.labelCurrentTrayNo.Name = "labelCurrentTrayNo";
            this.labelCurrentTrayNo.Size = new System.Drawing.Size(60, 24);
            this.labelCurrentTrayNo.TabIndex = 17;
            this.labelCurrentTrayNo.Text = "label2";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 100);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(149, 24);
            this.label14.TabIndex = 16;
            this.label14.Text = "Current Tray No:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(242, 24);
            this.label6.TabIndex = 14;
            this.label6.Text = "Total Input Quantity Picked: ";
            // 
            // labelCurrentInputQuantity
            // 
            this.labelCurrentInputQuantity.AutoSize = true;
            this.labelCurrentInputQuantity.Location = new System.Drawing.Point(389, 65);
            this.labelCurrentInputQuantity.Name = "labelCurrentInputQuantity";
            this.labelCurrentInputQuantity.Size = new System.Drawing.Size(60, 24);
            this.labelCurrentInputQuantity.TabIndex = 15;
            this.labelCurrentInputQuantity.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(355, 24);
            this.label2.TabIndex = 12;
            this.label2.Text = "Input Quantity Picked From Current Tray: ";
            // 
            // labelInputQuantity
            // 
            this.labelInputQuantity.AutoSize = true;
            this.labelInputQuantity.Location = new System.Drawing.Point(389, 30);
            this.labelInputQuantity.Name = "labelInputQuantity";
            this.labelInputQuantity.Size = new System.Drawing.Size(60, 24);
            this.labelInputQuantity.TabIndex = 13;
            this.labelInputQuantity.Text = "label2";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelInputTrayQuantity);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.labelInputLotQuantity);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.labelInputLotID);
            this.groupBox3.Location = new System.Drawing.Point(464, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(455, 138);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Lot Info";
            // 
            // labelInputTrayQuantity
            // 
            this.labelInputTrayQuantity.AutoSize = true;
            this.labelInputTrayQuantity.Location = new System.Drawing.Point(389, 100);
            this.labelInputTrayQuantity.Name = "labelInputTrayQuantity";
            this.labelInputTrayQuantity.Size = new System.Drawing.Size(60, 24);
            this.labelInputTrayQuantity.TabIndex = 17;
            this.labelInputTrayQuantity.Text = "label2";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 100);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(243, 24);
            this.label13.TabIndex = 16;
            this.label13.Text = "Input Tray Quantity Entered:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 24);
            this.label8.TabIndex = 14;
            this.label8.Text = "Lot ID:";
            // 
            // labelInputLotQuantity
            // 
            this.labelInputLotQuantity.AutoSize = true;
            this.labelInputLotQuantity.Location = new System.Drawing.Point(389, 65);
            this.labelInputLotQuantity.Name = "labelInputLotQuantity";
            this.labelInputLotQuantity.Size = new System.Drawing.Size(60, 24);
            this.labelInputLotQuantity.TabIndex = 15;
            this.labelInputLotQuantity.Text = "label2";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 65);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(238, 24);
            this.label12.TabIndex = 12;
            this.label12.Text = "Input Unit Quantity Entered:";
            // 
            // labelInputLotID
            // 
            this.labelInputLotID.Location = new System.Drawing.Point(74, 30);
            this.labelInputLotID.Name = "labelInputLotID";
            this.labelInputLotID.Size = new System.Drawing.Size(375, 24);
            this.labelInputLotID.TabIndex = 13;
            this.labelInputLotID.Text = "label2";
            this.labelInputLotID.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // TrayInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "TrayInformation";
            this.Size = new System.Drawing.Size(928, 350);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label labelOutputQuantity;
        public System.Windows.Forms.Label labelRejectTray1Quantity;
        public System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label labelInputQuantity;
        public System.Windows.Forms.Label labelCurrentOutput;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label labelCurrentInputQuantity;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.Label labelInputLotQuantity;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.Label labelInputLotID;
        public System.Windows.Forms.Label labelInputTrayQuantity;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.Label labelCurrentTrayNo;
        private System.Windows.Forms.Label label14;
    }
}
