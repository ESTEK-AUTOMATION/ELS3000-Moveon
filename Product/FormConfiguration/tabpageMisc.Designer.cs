namespace Product.FormConfiguration
{
    partial class tabpageMisc
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
            this.checkBoxEnableSecsgem = new System.Windows.Forms.CheckBox();
            this.groupBoxKeyenceBarcodeReader = new System.Windows.Forms.GroupBox();
            this.panelKeyenceBarcodeReaderCommunicationInterface = new System.Windows.Forms.Panel();
            this.radioButtonKeyenceBarcodeReader_Ethernet = new System.Windows.Forms.RadioButton();
            this.radioButtonKeyenceBarcodeReader_RS232C = new System.Windows.Forms.RadioButton();
            this.checkBoxEnableKeyenceBarcodeReader = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableZebexScanner = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableCognexBarcodeReader = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableBarcodeReader = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableAutoInputLoading = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableAutoOutputLoading = new System.Windows.Forms.CheckBox();
            this.groupBoxKeyenceBarcodeReader.SuspendLayout();
            this.panelKeyenceBarcodeReaderCommunicationInterface.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxEnableSecsgem
            // 
            this.checkBoxEnableSecsgem.AutoSize = true;
            this.checkBoxEnableSecsgem.Location = new System.Drawing.Point(452, 12);
            this.checkBoxEnableSecsgem.Name = "checkBoxEnableSecsgem";
            this.checkBoxEnableSecsgem.Size = new System.Drawing.Size(174, 28);
            this.checkBoxEnableSecsgem.TabIndex = 15;
            this.checkBoxEnableSecsgem.Text = "Enable Secsgem";
            this.checkBoxEnableSecsgem.UseVisualStyleBackColor = true;
            // 
            // groupBoxKeyenceBarcodeReader
            // 
            this.groupBoxKeyenceBarcodeReader.Controls.Add(this.panelKeyenceBarcodeReaderCommunicationInterface);
            this.groupBoxKeyenceBarcodeReader.Controls.Add(this.checkBoxEnableKeyenceBarcodeReader);
            this.groupBoxKeyenceBarcodeReader.Location = new System.Drawing.Point(13, 114);
            this.groupBoxKeyenceBarcodeReader.Name = "groupBoxKeyenceBarcodeReader";
            this.groupBoxKeyenceBarcodeReader.Size = new System.Drawing.Size(330, 100);
            this.groupBoxKeyenceBarcodeReader.TabIndex = 14;
            this.groupBoxKeyenceBarcodeReader.TabStop = false;
            this.groupBoxKeyenceBarcodeReader.Text = "Keyence Barcode Reader";
            // 
            // panelKeyenceBarcodeReaderCommunicationInterface
            // 
            this.panelKeyenceBarcodeReaderCommunicationInterface.Controls.Add(this.radioButtonKeyenceBarcodeReader_Ethernet);
            this.panelKeyenceBarcodeReaderCommunicationInterface.Controls.Add(this.radioButtonKeyenceBarcodeReader_RS232C);
            this.panelKeyenceBarcodeReaderCommunicationInterface.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelKeyenceBarcodeReaderCommunicationInterface.Location = new System.Drawing.Point(3, 53);
            this.panelKeyenceBarcodeReaderCommunicationInterface.Name = "panelKeyenceBarcodeReaderCommunicationInterface";
            this.panelKeyenceBarcodeReaderCommunicationInterface.Size = new System.Drawing.Size(324, 39);
            this.panelKeyenceBarcodeReaderCommunicationInterface.TabIndex = 9;
            // 
            // radioButtonKeyenceBarcodeReader_Ethernet
            // 
            this.radioButtonKeyenceBarcodeReader_Ethernet.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButtonKeyenceBarcodeReader_Ethernet.Location = new System.Drawing.Point(100, 0);
            this.radioButtonKeyenceBarcodeReader_Ethernet.Name = "radioButtonKeyenceBarcodeReader_Ethernet";
            this.radioButtonKeyenceBarcodeReader_Ethernet.Size = new System.Drawing.Size(100, 39);
            this.radioButtonKeyenceBarcodeReader_Ethernet.TabIndex = 1;
            this.radioButtonKeyenceBarcodeReader_Ethernet.TabStop = true;
            this.radioButtonKeyenceBarcodeReader_Ethernet.Text = "Ethernet";
            this.radioButtonKeyenceBarcodeReader_Ethernet.UseVisualStyleBackColor = true;
            // 
            // radioButtonKeyenceBarcodeReader_RS232C
            // 
            this.radioButtonKeyenceBarcodeReader_RS232C.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButtonKeyenceBarcodeReader_RS232C.Location = new System.Drawing.Point(0, 0);
            this.radioButtonKeyenceBarcodeReader_RS232C.Name = "radioButtonKeyenceBarcodeReader_RS232C";
            this.radioButtonKeyenceBarcodeReader_RS232C.Size = new System.Drawing.Size(100, 39);
            this.radioButtonKeyenceBarcodeReader_RS232C.TabIndex = 0;
            this.radioButtonKeyenceBarcodeReader_RS232C.TabStop = true;
            this.radioButtonKeyenceBarcodeReader_RS232C.Text = "RS232C";
            this.radioButtonKeyenceBarcodeReader_RS232C.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableKeyenceBarcodeReader
            // 
            this.checkBoxEnableKeyenceBarcodeReader.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxEnableKeyenceBarcodeReader.Location = new System.Drawing.Point(3, 25);
            this.checkBoxEnableKeyenceBarcodeReader.Name = "checkBoxEnableKeyenceBarcodeReader";
            this.checkBoxEnableKeyenceBarcodeReader.Size = new System.Drawing.Size(324, 28);
            this.checkBoxEnableKeyenceBarcodeReader.TabIndex = 8;
            this.checkBoxEnableKeyenceBarcodeReader.Text = "Enable Keyence Barcode Reader";
            this.checkBoxEnableKeyenceBarcodeReader.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableZebexScanner
            // 
            this.checkBoxEnableZebexScanner.AutoSize = true;
            this.checkBoxEnableZebexScanner.Location = new System.Drawing.Point(13, 80);
            this.checkBoxEnableZebexScanner.Name = "checkBoxEnableZebexScanner";
            this.checkBoxEnableZebexScanner.Size = new System.Drawing.Size(225, 28);
            this.checkBoxEnableZebexScanner.TabIndex = 13;
            this.checkBoxEnableZebexScanner.Text = "Enable Zebex Scanner";
            this.checkBoxEnableZebexScanner.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableCognexBarcodeReader
            // 
            this.checkBoxEnableCognexBarcodeReader.AutoSize = true;
            this.checkBoxEnableCognexBarcodeReader.Location = new System.Drawing.Point(13, 46);
            this.checkBoxEnableCognexBarcodeReader.Name = "checkBoxEnableCognexBarcodeReader";
            this.checkBoxEnableCognexBarcodeReader.Size = new System.Drawing.Size(304, 28);
            this.checkBoxEnableCognexBarcodeReader.TabIndex = 12;
            this.checkBoxEnableCognexBarcodeReader.Text = "Enable Cognex Barcode Reader";
            this.checkBoxEnableCognexBarcodeReader.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableBarcodeReader
            // 
            this.checkBoxEnableBarcodeReader.AutoSize = true;
            this.checkBoxEnableBarcodeReader.Location = new System.Drawing.Point(13, 12);
            this.checkBoxEnableBarcodeReader.Name = "checkBoxEnableBarcodeReader";
            this.checkBoxEnableBarcodeReader.Size = new System.Drawing.Size(232, 28);
            this.checkBoxEnableBarcodeReader.TabIndex = 11;
            this.checkBoxEnableBarcodeReader.Text = "Enable Barcode Reader";
            this.checkBoxEnableBarcodeReader.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableAutoInputLoading
            // 
            this.checkBoxEnableAutoInputLoading.AutoSize = true;
            this.checkBoxEnableAutoInputLoading.Location = new System.Drawing.Point(13, 253);
            this.checkBoxEnableAutoInputLoading.Name = "checkBoxEnableAutoInputLoading";
            this.checkBoxEnableAutoInputLoading.Size = new System.Drawing.Size(252, 28);
            this.checkBoxEnableAutoInputLoading.TabIndex = 16;
            this.checkBoxEnableAutoInputLoading.Text = "Enable Auto Input Loading";
            this.checkBoxEnableAutoInputLoading.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableAutoOutputLoading
            // 
            this.checkBoxEnableAutoOutputLoading.AutoSize = true;
            this.checkBoxEnableAutoOutputLoading.Location = new System.Drawing.Point(13, 287);
            this.checkBoxEnableAutoOutputLoading.Name = "checkBoxEnableAutoOutputLoading";
            this.checkBoxEnableAutoOutputLoading.Size = new System.Drawing.Size(267, 28);
            this.checkBoxEnableAutoOutputLoading.TabIndex = 17;
            this.checkBoxEnableAutoOutputLoading.Text = "Enable Auto Output Loading";
            this.checkBoxEnableAutoOutputLoading.UseVisualStyleBackColor = true;
            // 
            // tabpageMisc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.checkBoxEnableAutoOutputLoading);
            this.Controls.Add(this.checkBoxEnableAutoInputLoading);
            this.Controls.Add(this.checkBoxEnableSecsgem);
            this.Controls.Add(this.groupBoxKeyenceBarcodeReader);
            this.Controls.Add(this.checkBoxEnableZebexScanner);
            this.Controls.Add(this.checkBoxEnableCognexBarcodeReader);
            this.Controls.Add(this.checkBoxEnableBarcodeReader);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "tabpageMisc";
            this.Size = new System.Drawing.Size(688, 417);
            this.groupBoxKeyenceBarcodeReader.ResumeLayout(false);
            this.panelKeyenceBarcodeReaderCommunicationInterface.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox checkBoxEnableSecsgem;
        public System.Windows.Forms.GroupBox groupBoxKeyenceBarcodeReader;
        public System.Windows.Forms.Panel panelKeyenceBarcodeReaderCommunicationInterface;
        public System.Windows.Forms.RadioButton radioButtonKeyenceBarcodeReader_Ethernet;
        public System.Windows.Forms.RadioButton radioButtonKeyenceBarcodeReader_RS232C;
        public System.Windows.Forms.CheckBox checkBoxEnableKeyenceBarcodeReader;
        public System.Windows.Forms.CheckBox checkBoxEnableZebexScanner;
        public System.Windows.Forms.CheckBox checkBoxEnableCognexBarcodeReader;
        public System.Windows.Forms.CheckBox checkBoxEnableBarcodeReader;
        public System.Windows.Forms.CheckBox checkBoxEnableAutoInputLoading;
        public System.Windows.Forms.CheckBox checkBoxEnableAutoOutputLoading;
    }
}
