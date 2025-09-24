namespace Product
{
    partial class ProductFormBarcode
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
            this.buttonRetry = new System.Windows.Forms.Button();
            this.buttonUnload = new System.Windows.Forms.Button();
            this.textBoxBarcodeID = new System.Windows.Forms.TextBox();
            this.labelBarcodeID = new System.Windows.Forms.Label();
            this.pictureBoxLastReadImage = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLastReadImage)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.richTextBoxMessage);
            this.panelBottom.Controls.Add(this.buttonRetry);
            this.panelBottom.Controls.Add(this.buttonUnload);
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
            this.richTextBoxMessage.Text = "Message";
            // 
            // buttonRetry
            // 
            this.buttonRetry.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonRetry.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRetry.Location = new System.Drawing.Point(1084, 0);
            this.buttonRetry.Margin = new System.Windows.Forms.Padding(5);
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.Size = new System.Drawing.Size(100, 100);
            this.buttonRetry.TabIndex = 70;
            this.buttonRetry.Text = "Retry";
            this.buttonRetry.UseVisualStyleBackColor = true;
            this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
            // 
            // buttonUnload
            // 
            this.buttonUnload.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonUnload.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUnload.Location = new System.Drawing.Point(1184, 0);
            this.buttonUnload.Margin = new System.Windows.Forms.Padding(5);
            this.buttonUnload.Name = "buttonUnload";
            this.buttonUnload.Size = new System.Drawing.Size(100, 100);
            this.buttonUnload.TabIndex = 21;
            this.buttonUnload.Text = "Unload";
            this.buttonUnload.UseVisualStyleBackColor = true;
            this.buttonUnload.Click += new System.EventHandler(this.buttonUnload_Click);
            // 
            // textBoxBarcodeID
            // 
            this.textBoxBarcodeID.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBoxBarcodeID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxBarcodeID.Location = new System.Drawing.Point(448, 30);
            this.textBoxBarcodeID.Name = "textBoxBarcodeID";
            this.textBoxBarcodeID.Size = new System.Drawing.Size(451, 26);
            this.textBoxBarcodeID.TabIndex = 105;
            // 
            // labelBarcodeID
            // 
            this.labelBarcodeID.AutoSize = true;
            this.labelBarcodeID.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelBarcodeID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBarcodeID.Location = new System.Drawing.Point(350, 30);
            this.labelBarcodeID.Name = "labelBarcodeID";
            this.labelBarcodeID.Size = new System.Drawing.Size(98, 20);
            this.labelBarcodeID.TabIndex = 104;
            this.labelBarcodeID.Text = "Barcode ID :";
            // 
            // pictureBoxLastReadImage
            // 
            this.pictureBoxLastReadImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxLastReadImage.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLastReadImage.Name = "pictureBoxLastReadImage";
            this.pictureBoxLastReadImage.Size = new System.Drawing.Size(1284, 556);
            this.pictureBoxLastReadImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxLastReadImage.TabIndex = 106;
            this.pictureBoxLastReadImage.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxBarcodeID);
            this.panel1.Controls.Add(this.labelBarcodeID);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(350, 30, 350, 30);
            this.panel1.Size = new System.Drawing.Size(1284, 86);
            this.panel1.TabIndex = 107;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.pictureBoxLastReadImage);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 86);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1284, 556);
            this.panel2.TabIndex = 108;
            // 
            // ProductFormBarcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 742);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelBottom);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductFormBarcode";
            this.Text = "Input Barcode ID Confirmation";
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLastReadImage)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.RichTextBox richTextBoxMessage;
        private System.Windows.Forms.Button buttonRetry;
        private System.Windows.Forms.Button buttonUnload;
        private System.Windows.Forms.TextBox textBoxBarcodeID;
        private System.Windows.Forms.Label labelBarcodeID;
        private System.Windows.Forms.PictureBox pictureBoxLastReadImage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}