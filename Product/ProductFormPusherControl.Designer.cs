namespace Product
{
    partial class ProductFormPusherControl
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
            this.groupBoxSWPusher = new System.Windows.Forms.GroupBox();
            this.buttonAdditionalSWPusherDown = new System.Windows.Forms.Button();
            this.buttonAdditonalSWPusherUp = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBoxSWPusher.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSWPusher
            // 
            this.groupBoxSWPusher.Controls.Add(this.buttonAdditionalSWPusherDown);
            this.groupBoxSWPusher.Controls.Add(this.buttonAdditonalSWPusherUp);
            this.groupBoxSWPusher.Controls.Add(this.label2);
            this.groupBoxSWPusher.Controls.Add(this.label1);
            this.groupBoxSWPusher.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSWPusher.Location = new System.Drawing.Point(12, 12);
            this.groupBoxSWPusher.Name = "groupBoxSWPusher";
            this.groupBoxSWPusher.Size = new System.Drawing.Size(472, 255);
            this.groupBoxSWPusher.TabIndex = 0;
            this.groupBoxSWPusher.TabStop = false;
            this.groupBoxSWPusher.Text = "Side Wall Pusher Up and Down";
            // 
            // buttonAdditionalSWPusherDown
            // 
            this.buttonAdditionalSWPusherDown.Location = new System.Drawing.Point(276, 133);
            this.buttonAdditionalSWPusherDown.Name = "buttonAdditionalSWPusherDown";
            this.buttonAdditionalSWPusherDown.Size = new System.Drawing.Size(152, 47);
            this.buttonAdditionalSWPusherDown.TabIndex = 3;
            this.buttonAdditionalSWPusherDown.Text = "Down";
            this.buttonAdditionalSWPusherDown.UseVisualStyleBackColor = true;
            // 
            // buttonAdditonalSWPusherUp
            // 
            this.buttonAdditonalSWPusherUp.Location = new System.Drawing.Point(276, 46);
            this.buttonAdditonalSWPusherUp.Name = "buttonAdditonalSWPusherUp";
            this.buttonAdditonalSWPusherUp.Size = new System.Drawing.Size(152, 47);
            this.buttonAdditonalSWPusherUp.TabIndex = 2;
            this.buttonAdditonalSWPusherUp.Text = "Up";
            this.buttonAdditonalSWPusherUp.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "SW Pusher Down :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "SW Pusher Up :";
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.Location = new System.Drawing.Point(708, 426);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(125, 49);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // ProductFormPusherControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 487);
            this.ControlBox = false;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBoxSWPusher);
            this.Name = "ProductFormPusherControl";
            this.Text = "ProductFormPusherControl";
            this.groupBoxSWPusher.ResumeLayout(false);
            this.groupBoxSWPusher.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox groupBoxSWPusher;
        public System.Windows.Forms.Button buttonAdditionalSWPusherDown;
        public System.Windows.Forms.Button buttonAdditonalSWPusherUp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonClose;
    }
}