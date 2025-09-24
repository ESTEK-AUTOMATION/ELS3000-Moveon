namespace Customer
{
    partial class CustomerFormNewLot
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
            this.panelBottom = new System.Windows.Forms.Panel();
            this.richTextBoxMessage = new System.Windows.Forms.RichTextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxOperatorID = new System.Windows.Forms.TextBox();
            this.labelOperatorID = new System.Windows.Forms.Label();
            this.textBoxLotID1 = new System.Windows.Forms.TextBox();
            this.label1MESLot = new System.Windows.Forms.Label();
            this.textBoxLotID2 = new System.Windows.Forms.TextBox();
            this.textBoxLotID3 = new System.Windows.Forms.TextBox();
            this.textBoxBuild = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPartName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownTotalOutputQuantity = new System.Windows.Forms.NumericUpDown();
            this.textBoxPartNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownInputQuantity1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownInputQuantity2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownInputQuantity3 = new System.Windows.Forms.NumericUpDown();
            this.buttonVerifyMatchLot = new System.Windows.Forms.Button();
            this.numericUpDownInputTrayQuantity1 = new System.Windows.Forms.NumericUpDown();
            this.labelTrayQty1 = new System.Windows.Forms.Label();
            this.labelUnitQty1 = new System.Windows.Forms.Label();
            this.groupBoxLotID1 = new System.Windows.Forms.GroupBox();
            this.groupBoxLotID3 = new System.Windows.Forms.GroupBox();
            this.label3MESLot = new System.Windows.Forms.Label();
            this.labelTrayQty3 = new System.Windows.Forms.Label();
            this.labelUnitQty3 = new System.Windows.Forms.Label();
            this.numericUpDownInputTrayQuantity3 = new System.Windows.Forms.NumericUpDown();
            this.groupBoxLotID2 = new System.Windows.Forms.GroupBox();
            this.label2MESLot = new System.Windows.Forms.Label();
            this.labelTrayQty2 = new System.Windows.Forms.Label();
            this.labelUnitQty2 = new System.Windows.Forms.Label();
            this.numericUpDownInputTrayQuantity2 = new System.Windows.Forms.NumericUpDown();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalOutputQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputQuantity1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputQuantity2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputQuantity3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputTrayQuantity1)).BeginInit();
            this.groupBoxLotID1.SuspendLayout();
            this.groupBoxLotID3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputTrayQuantity3)).BeginInit();
            this.groupBoxLotID2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputTrayQuantity2)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.richTextBoxMessage);
            this.panelBottom.Controls.Add(this.buttonOK);
            this.panelBottom.Controls.Add(this.buttonCancel);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 642);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1284, 100);
            this.panelBottom.TabIndex = 103;
            // 
            // richTextBoxMessage
            // 
            this.richTextBoxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxMessage.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxMessage.Margin = new System.Windows.Forms.Padding(6);
            this.richTextBoxMessage.MaxLength = 2000;
            this.richTextBoxMessage.Name = "richTextBoxMessage";
            this.richTextBoxMessage.Size = new System.Drawing.Size(1084, 100);
            this.richTextBoxMessage.TabIndex = 69;
            this.richTextBoxMessage.Text = "";
            this.richTextBoxMessage.TextChanged += new System.EventHandler(this.richTextBoxMessage_TextChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOK.Location = new System.Drawing.Point(1084, 0);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(5);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 100);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(1184, 0);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(5);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 100);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxOperatorID
            // 
            this.textBoxOperatorID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOperatorID.Location = new System.Drawing.Point(184, 12);
            this.textBoxOperatorID.Name = "textBoxOperatorID";
            this.textBoxOperatorID.Size = new System.Drawing.Size(451, 26);
            this.textBoxOperatorID.TabIndex = 0;
            this.textBoxOperatorID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxOperatorID_KeyUp);
            // 
            // labelOperatorID
            // 
            this.labelOperatorID.AutoSize = true;
            this.labelOperatorID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOperatorID.Location = new System.Drawing.Point(13, 15);
            this.labelOperatorID.Name = "labelOperatorID";
            this.labelOperatorID.Size = new System.Drawing.Size(101, 20);
            this.labelOperatorID.TabIndex = 112;
            this.labelOperatorID.Text = "Operator ID :";
            // 
            // textBoxLotID1
            // 
            this.textBoxLotID1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLotID1.Location = new System.Drawing.Point(161, 28);
            this.textBoxLotID1.Name = "textBoxLotID1";
            this.textBoxLotID1.Size = new System.Drawing.Size(451, 26);
            this.textBoxLotID1.TabIndex = 1;
            this.textBoxLotID1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxLotID1_KeyUp);
            // 
            // label1MESLot
            // 
            this.label1MESLot.AutoSize = true;
            this.label1MESLot.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1MESLot.Location = new System.Drawing.Point(6, 34);
            this.label1MESLot.Name = "label1MESLot";
            this.label1MESLot.Size = new System.Drawing.Size(61, 20);
            this.label1MESLot.TabIndex = 118;
            this.label1MESLot.Text = "Lot ID :";
            // 
            // textBoxLotID2
            // 
            this.textBoxLotID2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLotID2.Location = new System.Drawing.Point(161, 28);
            this.textBoxLotID2.Name = "textBoxLotID2";
            this.textBoxLotID2.Size = new System.Drawing.Size(451, 26);
            this.textBoxLotID2.TabIndex = 6;
            this.textBoxLotID2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxLotID2_KeyUp);
            // 
            // textBoxLotID3
            // 
            this.textBoxLotID3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLotID3.Location = new System.Drawing.Point(161, 28);
            this.textBoxLotID3.Name = "textBoxLotID3";
            this.textBoxLotID3.Size = new System.Drawing.Size(451, 26);
            this.textBoxLotID3.TabIndex = 7;
            this.textBoxLotID3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxLotID3_KeyUp);
            // 
            // textBoxBuild
            // 
            this.textBoxBuild.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxBuild.Location = new System.Drawing.Point(812, 115);
            this.textBoxBuild.Name = "textBoxBuild";
            this.textBoxBuild.Size = new System.Drawing.Size(451, 26);
            this.textBoxBuild.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(641, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 20);
            this.label1.TabIndex = 131;
            this.label1.Text = "Build :";
            // 
            // textBoxPartName
            // 
            this.textBoxPartName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPartName.Location = new System.Drawing.Point(812, 12);
            this.textBoxPartName.Name = "textBoxPartName";
            this.textBoxPartName.Size = new System.Drawing.Size(451, 26);
            this.textBoxPartName.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(641, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 20);
            this.label2.TabIndex = 130;
            this.label2.Text = "Part Name :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(641, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 20);
            this.label3.TabIndex = 133;
            this.label3.Text = "Output Total Quantity:";
            this.label3.Visible = false;
            // 
            // numericUpDownTotalOutputQuantity
            // 
            this.numericUpDownTotalOutputQuantity.Location = new System.Drawing.Point(812, 169);
            this.numericUpDownTotalOutputQuantity.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownTotalOutputQuantity.Name = "numericUpDownTotalOutputQuantity";
            this.numericUpDownTotalOutputQuantity.Size = new System.Drawing.Size(120, 29);
            this.numericUpDownTotalOutputQuantity.TabIndex = 5;
            this.numericUpDownTotalOutputQuantity.Visible = false;
            // 
            // textBoxPartNumber
            // 
            this.textBoxPartNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPartNumber.Location = new System.Drawing.Point(812, 63);
            this.textBoxPartNumber.Name = "textBoxPartNumber";
            this.textBoxPartNumber.Size = new System.Drawing.Size(451, 26);
            this.textBoxPartNumber.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(641, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 20);
            this.label4.TabIndex = 136;
            this.label4.Text = "Part # :";
            // 
            // numericUpDownInputQuantity1
            // 
            this.numericUpDownInputQuantity1.Location = new System.Drawing.Point(161, 95);
            this.numericUpDownInputQuantity1.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownInputQuantity1.Name = "numericUpDownInputQuantity1";
            this.numericUpDownInputQuantity1.Size = new System.Drawing.Size(120, 29);
            this.numericUpDownInputQuantity1.TabIndex = 137;
            this.numericUpDownInputQuantity1.Visible = false;
            // 
            // numericUpDownInputQuantity2
            // 
            this.numericUpDownInputQuantity2.Location = new System.Drawing.Point(161, 95);
            this.numericUpDownInputQuantity2.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownInputQuantity2.Name = "numericUpDownInputQuantity2";
            this.numericUpDownInputQuantity2.Size = new System.Drawing.Size(120, 29);
            this.numericUpDownInputQuantity2.TabIndex = 138;
            // 
            // numericUpDownInputQuantity3
            // 
            this.numericUpDownInputQuantity3.Location = new System.Drawing.Point(161, 95);
            this.numericUpDownInputQuantity3.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownInputQuantity3.Name = "numericUpDownInputQuantity3";
            this.numericUpDownInputQuantity3.Size = new System.Drawing.Size(120, 29);
            this.numericUpDownInputQuantity3.TabIndex = 139;
            // 
            // buttonVerifyMatchLot
            // 
            this.buttonVerifyMatchLot.Location = new System.Drawing.Point(593, 27);
            this.buttonVerifyMatchLot.Name = "buttonVerifyMatchLot";
            this.buttonVerifyMatchLot.Size = new System.Drawing.Size(19, 27);
            this.buttonVerifyMatchLot.TabIndex = 140;
            this.buttonVerifyMatchLot.Text = "*";
            this.buttonVerifyMatchLot.UseVisualStyleBackColor = true;
            this.buttonVerifyMatchLot.Click += new System.EventHandler(this.buttonVerifyMatchLot_Click);
            // 
            // numericUpDownInputTrayQuantity1
            // 
            this.numericUpDownInputTrayQuantity1.Location = new System.Drawing.Point(161, 60);
            this.numericUpDownInputTrayQuantity1.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownInputTrayQuantity1.Name = "numericUpDownInputTrayQuantity1";
            this.numericUpDownInputTrayQuantity1.Size = new System.Drawing.Size(120, 29);
            this.numericUpDownInputTrayQuantity1.TabIndex = 141;
            // 
            // labelTrayQty1
            // 
            this.labelTrayQty1.AutoSize = true;
            this.labelTrayQty1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrayQty1.Location = new System.Drawing.Point(5, 69);
            this.labelTrayQty1.Name = "labelTrayQty1";
            this.labelTrayQty1.Size = new System.Drawing.Size(75, 20);
            this.labelTrayQty1.TabIndex = 142;
            this.labelTrayQty1.Text = "Tray Qty :";
            // 
            // labelUnitQty1
            // 
            this.labelUnitQty1.AutoSize = true;
            this.labelUnitQty1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUnitQty1.Location = new System.Drawing.Point(7, 104);
            this.labelUnitQty1.Name = "labelUnitQty1";
            this.labelUnitQty1.Size = new System.Drawing.Size(74, 20);
            this.labelUnitQty1.TabIndex = 143;
            this.labelUnitQty1.Text = "Unit Qty :";
            this.labelUnitQty1.Visible = false;
            // 
            // groupBoxLotID1
            // 
            this.groupBoxLotID1.Controls.Add(this.label1MESLot);
            this.groupBoxLotID1.Controls.Add(this.buttonVerifyMatchLot);
            this.groupBoxLotID1.Controls.Add(this.labelTrayQty1);
            this.groupBoxLotID1.Controls.Add(this.labelUnitQty1);
            this.groupBoxLotID1.Controls.Add(this.textBoxLotID1);
            this.groupBoxLotID1.Controls.Add(this.numericUpDownInputTrayQuantity1);
            this.groupBoxLotID1.Controls.Add(this.numericUpDownInputQuantity1);
            this.groupBoxLotID1.Location = new System.Drawing.Point(17, 44);
            this.groupBoxLotID1.Name = "groupBoxLotID1";
            this.groupBoxLotID1.Size = new System.Drawing.Size(618, 132);
            this.groupBoxLotID1.TabIndex = 144;
            this.groupBoxLotID1.TabStop = false;
            this.groupBoxLotID1.Text = "Lot ID 1";
            // 
            // groupBoxLotID3
            // 
            this.groupBoxLotID3.Controls.Add(this.label3MESLot);
            this.groupBoxLotID3.Controls.Add(this.labelTrayQty3);
            this.groupBoxLotID3.Controls.Add(this.labelUnitQty3);
            this.groupBoxLotID3.Controls.Add(this.numericUpDownInputQuantity3);
            this.groupBoxLotID3.Controls.Add(this.numericUpDownInputTrayQuantity3);
            this.groupBoxLotID3.Controls.Add(this.textBoxLotID3);
            this.groupBoxLotID3.Location = new System.Drawing.Point(17, 320);
            this.groupBoxLotID3.Name = "groupBoxLotID3";
            this.groupBoxLotID3.Size = new System.Drawing.Size(618, 132);
            this.groupBoxLotID3.TabIndex = 145;
            this.groupBoxLotID3.TabStop = false;
            this.groupBoxLotID3.Text = "Lot ID 3";
            this.groupBoxLotID3.Visible = false;
            // 
            // label3MESLot
            // 
            this.label3MESLot.AutoSize = true;
            this.label3MESLot.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3MESLot.Location = new System.Drawing.Point(6, 34);
            this.label3MESLot.Name = "label3MESLot";
            this.label3MESLot.Size = new System.Drawing.Size(61, 20);
            this.label3MESLot.TabIndex = 118;
            this.label3MESLot.Text = "Lot ID :";
            // 
            // labelTrayQty3
            // 
            this.labelTrayQty3.AutoSize = true;
            this.labelTrayQty3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrayQty3.Location = new System.Drawing.Point(7, 65);
            this.labelTrayQty3.Name = "labelTrayQty3";
            this.labelTrayQty3.Size = new System.Drawing.Size(75, 20);
            this.labelTrayQty3.TabIndex = 142;
            this.labelTrayQty3.Text = "Tray Qty :";
            // 
            // labelUnitQty3
            // 
            this.labelUnitQty3.AutoSize = true;
            this.labelUnitQty3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUnitQty3.Location = new System.Drawing.Point(7, 100);
            this.labelUnitQty3.Name = "labelUnitQty3";
            this.labelUnitQty3.Size = new System.Drawing.Size(74, 20);
            this.labelUnitQty3.TabIndex = 143;
            this.labelUnitQty3.Text = "Unit Qty :";
            // 
            // numericUpDownInputTrayQuantity3
            // 
            this.numericUpDownInputTrayQuantity3.Location = new System.Drawing.Point(161, 60);
            this.numericUpDownInputTrayQuantity3.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownInputTrayQuantity3.Name = "numericUpDownInputTrayQuantity3";
            this.numericUpDownInputTrayQuantity3.Size = new System.Drawing.Size(120, 29);
            this.numericUpDownInputTrayQuantity3.TabIndex = 141;
            // 
            // groupBoxLotID2
            // 
            this.groupBoxLotID2.Controls.Add(this.label2MESLot);
            this.groupBoxLotID2.Controls.Add(this.labelTrayQty2);
            this.groupBoxLotID2.Controls.Add(this.labelUnitQty2);
            this.groupBoxLotID2.Controls.Add(this.numericUpDownInputTrayQuantity2);
            this.groupBoxLotID2.Controls.Add(this.numericUpDownInputQuantity2);
            this.groupBoxLotID2.Controls.Add(this.textBoxLotID2);
            this.groupBoxLotID2.Location = new System.Drawing.Point(17, 182);
            this.groupBoxLotID2.Name = "groupBoxLotID2";
            this.groupBoxLotID2.Size = new System.Drawing.Size(618, 132);
            this.groupBoxLotID2.TabIndex = 145;
            this.groupBoxLotID2.TabStop = false;
            this.groupBoxLotID2.Text = "Lot ID 2";
            this.groupBoxLotID2.Visible = false;
            // 
            // label2MESLot
            // 
            this.label2MESLot.AutoSize = true;
            this.label2MESLot.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2MESLot.Location = new System.Drawing.Point(6, 34);
            this.label2MESLot.Name = "label2MESLot";
            this.label2MESLot.Size = new System.Drawing.Size(61, 20);
            this.label2MESLot.TabIndex = 118;
            this.label2MESLot.Text = "Lot ID :";
            // 
            // labelTrayQty2
            // 
            this.labelTrayQty2.AutoSize = true;
            this.labelTrayQty2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrayQty2.Location = new System.Drawing.Point(6, 65);
            this.labelTrayQty2.Name = "labelTrayQty2";
            this.labelTrayQty2.Size = new System.Drawing.Size(75, 20);
            this.labelTrayQty2.TabIndex = 142;
            this.labelTrayQty2.Text = "Tray Qty :";
            // 
            // labelUnitQty2
            // 
            this.labelUnitQty2.AutoSize = true;
            this.labelUnitQty2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUnitQty2.Location = new System.Drawing.Point(6, 100);
            this.labelUnitQty2.Name = "labelUnitQty2";
            this.labelUnitQty2.Size = new System.Drawing.Size(74, 20);
            this.labelUnitQty2.TabIndex = 143;
            this.labelUnitQty2.Text = "Unit Qty :";
            // 
            // numericUpDownInputTrayQuantity2
            // 
            this.numericUpDownInputTrayQuantity2.Location = new System.Drawing.Point(161, 60);
            this.numericUpDownInputTrayQuantity2.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownInputTrayQuantity2.Name = "numericUpDownInputTrayQuantity2";
            this.numericUpDownInputTrayQuantity2.Size = new System.Drawing.Size(120, 29);
            this.numericUpDownInputTrayQuantity2.TabIndex = 141;
            // 
            // CustomerFormNewLot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 742);
            this.ControlBox = false;
            this.Controls.Add(this.groupBoxLotID2);
            this.Controls.Add(this.groupBoxLotID3);
            this.Controls.Add(this.groupBoxLotID1);
            this.Controls.Add(this.textBoxPartNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownTotalOutputQuantity);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxBuild);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPartName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxOperatorID);
            this.Controls.Add(this.labelOperatorID);
            this.Controls.Add(this.panelBottom);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomerFormNewLot";
            this.Text = "New Lot";
            this.TopMost = true;
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalOutputQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputQuantity1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputQuantity2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputQuantity3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputTrayQuantity1)).EndInit();
            this.groupBoxLotID1.ResumeLayout(false);
            this.groupBoxLotID1.PerformLayout();
            this.groupBoxLotID3.ResumeLayout(false);
            this.groupBoxLotID3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputTrayQuantity3)).EndInit();
            this.groupBoxLotID2.ResumeLayout(false);
            this.groupBoxLotID2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputTrayQuantity2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.RichTextBox richTextBoxMessage;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelOperatorID;
        private System.Windows.Forms.TextBox textBoxLotID1;
        private System.Windows.Forms.Label label1MESLot;
        private System.Windows.Forms.TextBox textBoxLotID2;
        private System.Windows.Forms.TextBox textBoxLotID3;
        public System.Windows.Forms.TextBox textBoxOperatorID;
        private System.Windows.Forms.TextBox textBoxBuild;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBoxPartName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownTotalOutputQuantity;
        public System.Windows.Forms.TextBox textBoxPartNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownInputQuantity1;
        private System.Windows.Forms.NumericUpDown numericUpDownInputQuantity2;
        private System.Windows.Forms.NumericUpDown numericUpDownInputQuantity3;
        private System.Windows.Forms.Button buttonVerifyMatchLot;
        private System.Windows.Forms.NumericUpDown numericUpDownInputTrayQuantity1;
        private System.Windows.Forms.Label labelTrayQty1;
        private System.Windows.Forms.Label labelUnitQty1;
        private System.Windows.Forms.GroupBox groupBoxLotID1;
        private System.Windows.Forms.GroupBox groupBoxLotID3;
        private System.Windows.Forms.Label label3MESLot;
        private System.Windows.Forms.Label labelTrayQty3;
        private System.Windows.Forms.Label labelUnitQty3;
        private System.Windows.Forms.NumericUpDown numericUpDownInputTrayQuantity3;
        private System.Windows.Forms.GroupBox groupBoxLotID2;
        private System.Windows.Forms.Label label2MESLot;
        private System.Windows.Forms.Label labelTrayQty2;
        private System.Windows.Forms.Label labelUnitQty2;
        private System.Windows.Forms.NumericUpDown numericUpDownInputTrayQuantity2;
    }
}