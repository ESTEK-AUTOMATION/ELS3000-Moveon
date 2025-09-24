namespace Product
{
    partial class MaintenanceSetting
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownCleanCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownWarningCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownDueCount = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxComsumableItem = new System.Windows.Forms.ComboBox();
            this.numericUpDownConsumableItemCurrentCount = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonUndo = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxComsumablePart = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonDoneCleaning = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCleanCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWarningCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDueCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownConsumableItemCurrentCount)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Clean Count";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(22, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Warning Count";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(22, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Due Count";
            // 
            // numericUpDownCleanCount
            // 
            this.numericUpDownCleanCount.Location = new System.Drawing.Point(198, 71);
            this.numericUpDownCleanCount.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericUpDownCleanCount.Name = "numericUpDownCleanCount";
            this.numericUpDownCleanCount.Size = new System.Drawing.Size(128, 22);
            this.numericUpDownCleanCount.TabIndex = 5;
            // 
            // numericUpDownWarningCount
            // 
            this.numericUpDownWarningCount.Location = new System.Drawing.Point(198, 113);
            this.numericUpDownWarningCount.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericUpDownWarningCount.Name = "numericUpDownWarningCount";
            this.numericUpDownWarningCount.Size = new System.Drawing.Size(128, 22);
            this.numericUpDownWarningCount.TabIndex = 6;
            // 
            // numericUpDownDueCount
            // 
            this.numericUpDownDueCount.Location = new System.Drawing.Point(198, 157);
            this.numericUpDownDueCount.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericUpDownDueCount.Name = "numericUpDownDueCount";
            this.numericUpDownDueCount.Size = new System.Drawing.Size(128, 22);
            this.numericUpDownDueCount.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(22, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 16);
            this.label8.TabIndex = 16;
            this.label8.Text = "Consumable Item";
            // 
            // comboBoxComsumableItem
            // 
            this.comboBoxComsumableItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxComsumableItem.FormattingEnabled = true;
            this.comboBoxComsumableItem.Location = new System.Drawing.Point(198, 37);
            this.comboBoxComsumableItem.Name = "comboBoxComsumableItem";
            this.comboBoxComsumableItem.Size = new System.Drawing.Size(128, 24);
            this.comboBoxComsumableItem.TabIndex = 15;
            // 
            // numericUpDownConsumableItemCurrentCount
            // 
            this.numericUpDownConsumableItemCurrentCount.Enabled = false;
            this.numericUpDownConsumableItemCurrentCount.Location = new System.Drawing.Point(198, 74);
            this.numericUpDownConsumableItemCurrentCount.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericUpDownConsumableItemCurrentCount.Name = "numericUpDownConsumableItemCurrentCount";
            this.numericUpDownConsumableItemCurrentCount.Size = new System.Drawing.Size(128, 22);
            this.numericUpDownConsumableItemCurrentCount.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(22, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 16);
            this.label9.TabIndex = 17;
            this.label9.Text = "Current Count";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(198, 117);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(56, 27);
            this.buttonReset.TabIndex = 19;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            // 
            // buttonUndo
            // 
            this.buttonUndo.Location = new System.Drawing.Point(270, 117);
            this.buttonUndo.Name = "buttonUndo";
            this.buttonUndo.Size = new System.Drawing.Size(56, 27);
            this.buttonUndo.TabIndex = 20;
            this.buttonUndo.Text = "Undo";
            this.buttonUndo.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxComsumablePart);
            this.groupBox1.Controls.Add(this.numericUpDownCleanCount);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericUpDownWarningCount);
            this.groupBox1.Controls.Add(this.numericUpDownDueCount);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(33, 274);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(418, 213);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Count Setting";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "Consumable Item";
            // 
            // comboBoxComsumablePart
            // 
            this.comboBoxComsumablePart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxComsumablePart.FormattingEnabled = true;
            this.comboBoxComsumablePart.Items.AddRange(new object[] {
            "Pick Up Head"});
            this.comboBoxComsumablePart.Location = new System.Drawing.Point(198, 34);
            this.comboBoxComsumablePart.Name = "comboBoxComsumablePart";
            this.comboBoxComsumablePart.Size = new System.Drawing.Size(128, 24);
            this.comboBoxComsumablePart.TabIndex = 17;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.buttonDoneCleaning);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.comboBoxComsumableItem);
            this.groupBox2.Controls.Add(this.buttonUndo);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.buttonReset);
            this.groupBox2.Controls.Add(this.numericUpDownConsumableItemCurrentCount);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(33, 33);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(418, 212);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Consumable Part";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(22, 161);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(166, 34);
            this.label10.TabIndex = 22;
            this.label10.Text = "Please Press this button \r\nonly when done Cleaning";
            // 
            // buttonDoneCleaning
            // 
            this.buttonDoneCleaning.Location = new System.Drawing.Point(198, 161);
            this.buttonDoneCleaning.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDoneCleaning.Name = "buttonDoneCleaning";
            this.buttonDoneCleaning.Size = new System.Drawing.Size(127, 38);
            this.buttonDoneCleaning.TabIndex = 21;
            this.buttonDoneCleaning.Text = "Done Cleaning";
            this.buttonDoneCleaning.UseVisualStyleBackColor = true;
            // 
            // MaintenanceSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MaintenanceSetting";
            this.Size = new System.Drawing.Size(1158, 640);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCleanCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWarningCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDueCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownConsumableItemCurrentCount)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown numericUpDownCleanCount;
        public System.Windows.Forms.NumericUpDown numericUpDownWarningCount;
        public System.Windows.Forms.NumericUpDown numericUpDownDueCount;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.ComboBox comboBoxComsumableItem;
        public System.Windows.Forms.NumericUpDown numericUpDownConsumableItemCurrentCount;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.Button buttonReset;
        public System.Windows.Forms.Button buttonUndo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.Button buttonDoneCleaning;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboBoxComsumablePart;
    }
}
