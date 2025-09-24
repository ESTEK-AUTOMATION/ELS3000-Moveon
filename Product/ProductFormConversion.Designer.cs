namespace Product
{
    partial class ProductFormConversion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductFormConversion));
            this.buttonClose = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.richTextBoxMessage = new System.Windows.Forms.RichTextBox();
            this.richTextBoxTapeAndReelConversion = new System.Windows.Forms.RichTextBox();
            this.buttonDone = new System.Windows.Forms.Button();
            this.buttonTapeAndReelIndexMotorOn = new System.Windows.Forms.Button();
            this.buttonTapeAndReelIndexMotorOff = new System.Windows.Forms.Button();
            this.buttonCutTape = new System.Windows.Forms.Button();
            this.buttonTapeJog = new System.Windows.Forms.Button();
            this.buttonTapeJobStop = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelBottom.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.Location = new System.Drawing.Point(1270, 0);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(5);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(100, 100);
            this.buttonClose.TabIndex = 21;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.richTextBoxMessage);
            this.panelBottom.Controls.Add(this.buttonClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 650);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1370, 100);
            this.panelBottom.TabIndex = 23;
            // 
            // richTextBoxMessage
            // 
            this.richTextBoxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxMessage.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxMessage.Margin = new System.Windows.Forms.Padding(6);
            this.richTextBoxMessage.MaxLength = 2000;
            this.richTextBoxMessage.Name = "richTextBoxMessage";
            this.richTextBoxMessage.Size = new System.Drawing.Size(1270, 100);
            this.richTextBoxMessage.TabIndex = 69;
            this.richTextBoxMessage.Text = "Message";
            // 
            // richTextBoxTapeAndReelConversion
            // 
            this.richTextBoxTapeAndReelConversion.Location = new System.Drawing.Point(19, 29);
            this.richTextBoxTapeAndReelConversion.Name = "richTextBoxTapeAndReelConversion";
            this.richTextBoxTapeAndReelConversion.Size = new System.Drawing.Size(336, 189);
            this.richTextBoxTapeAndReelConversion.TabIndex = 25;
            this.richTextBoxTapeAndReelConversion.Text = resources.GetString("richTextBoxTapeAndReelConversion.Text");
            // 
            // buttonDone
            // 
            this.buttonDone.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDone.Location = new System.Drawing.Point(647, 97);
            this.buttonDone.Name = "buttonDone";
            this.buttonDone.Size = new System.Drawing.Size(95, 95);
            this.buttonDone.TabIndex = 26;
            this.buttonDone.Text = "Done";
            this.buttonDone.UseVisualStyleBackColor = true;
            this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
            // 
            // buttonTapeAndReelIndexMotorOn
            // 
            this.buttonTapeAndReelIndexMotorOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTapeAndReelIndexMotorOn.Location = new System.Drawing.Point(361, 136);
            this.buttonTapeAndReelIndexMotorOn.Name = "buttonTapeAndReelIndexMotorOn";
            this.buttonTapeAndReelIndexMotorOn.Size = new System.Drawing.Size(95, 95);
            this.buttonTapeAndReelIndexMotorOn.TabIndex = 27;
            this.buttonTapeAndReelIndexMotorOn.Text = "Index Motor On";
            this.buttonTapeAndReelIndexMotorOn.UseVisualStyleBackColor = true;
            this.buttonTapeAndReelIndexMotorOn.Click += new System.EventHandler(this.buttonTapeAndReelIndexMotorOn_Click);
            // 
            // buttonTapeAndReelIndexMotorOff
            // 
            this.buttonTapeAndReelIndexMotorOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTapeAndReelIndexMotorOff.Location = new System.Drawing.Point(361, 289);
            this.buttonTapeAndReelIndexMotorOff.Name = "buttonTapeAndReelIndexMotorOff";
            this.buttonTapeAndReelIndexMotorOff.Size = new System.Drawing.Size(95, 95);
            this.buttonTapeAndReelIndexMotorOff.TabIndex = 28;
            this.buttonTapeAndReelIndexMotorOff.Text = "Index Motor Off";
            this.buttonTapeAndReelIndexMotorOff.UseVisualStyleBackColor = true;
            this.buttonTapeAndReelIndexMotorOff.Click += new System.EventHandler(this.buttonTapeAndReelIndexMotorOff_Click);
            // 
            // buttonCutTape
            // 
            this.buttonCutTape.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCutTape.Location = new System.Drawing.Point(361, 402);
            this.buttonCutTape.Name = "buttonCutTape";
            this.buttonCutTape.Size = new System.Drawing.Size(95, 95);
            this.buttonCutTape.TabIndex = 29;
            this.buttonCutTape.Text = "Cut Tape";
            this.buttonCutTape.UseVisualStyleBackColor = true;
            this.buttonCutTape.Click += new System.EventHandler(this.buttonCutTape_Click);
            // 
            // buttonTapeJog
            // 
            this.buttonTapeJog.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTapeJog.Location = new System.Drawing.Point(481, 402);
            this.buttonTapeJog.Name = "buttonTapeJog";
            this.buttonTapeJog.Size = new System.Drawing.Size(95, 95);
            this.buttonTapeJog.TabIndex = 30;
            this.buttonTapeJog.Text = "Tape Jog Start";
            this.buttonTapeJog.UseVisualStyleBackColor = true;
            this.buttonTapeJog.Click += new System.EventHandler(this.buttonTapeJog_Click);
            // 
            // buttonTapeJobStop
            // 
            this.buttonTapeJobStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTapeJobStop.Location = new System.Drawing.Point(613, 402);
            this.buttonTapeJobStop.Name = "buttonTapeJobStop";
            this.buttonTapeJobStop.Size = new System.Drawing.Size(95, 95);
            this.buttonTapeJobStop.TabIndex = 31;
            this.buttonTapeJobStop.Text = "Tape Jog Stop";
            this.buttonTapeJobStop.UseVisualStyleBackColor = true;
            this.buttonTapeJobStop.Click += new System.EventHandler(this.buttonTapeJobStop_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBoxTapeAndReelConversion);
            this.groupBox1.Controls.Add(this.buttonTapeAndReelIndexMotorOn);
            this.groupBox1.Controls.Add(this.buttonDone);
            this.groupBox1.Controls.Add(this.buttonTapeAndReelIndexMotorOff);
            this.groupBox1.Controls.Add(this.buttonCutTape);
            this.groupBox1.Controls.Add(this.buttonTapeJog);
            this.groupBox1.Controls.Add(this.buttonTapeJobStop);
            this.groupBox1.Location = new System.Drawing.Point(971, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(762, 536);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            this.groupBox1.Visible = false;
            // 
            // FormConversion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 750);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelBottom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConversion";
            this.Text = "Conversion";
            this.panelBottom.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.RichTextBox richTextBoxMessage;
        private System.Windows.Forms.RichTextBox richTextBoxTapeAndReelConversion;
        private System.Windows.Forms.Button buttonDone;
        private System.Windows.Forms.Button buttonTapeAndReelIndexMotorOn;
        private System.Windows.Forms.Button buttonTapeAndReelIndexMotorOff;
        private System.Windows.Forms.Button buttonCutTape;
        private System.Windows.Forms.Button buttonTapeJog;
        private System.Windows.Forms.Button buttonTapeJobStop;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}