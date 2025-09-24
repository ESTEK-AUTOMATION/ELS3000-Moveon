namespace Product
{
    partial class ProductionOutputMotionMove
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
            this.label7 = new System.Windows.Forms.Label();
            this.nudYToMove = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nudXToMove = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.btnMoveX = new System.Windows.Forms.Button();
            this.btnMoveY = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudYToMove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXToMove)).BeginInit();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(435, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 24);
            this.label7.TabIndex = 45;
            this.label7.Text = "(um)";
            // 
            // nudYToMove
            // 
            this.nudYToMove.Location = new System.Drawing.Point(181, 70);
            this.nudYToMove.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudYToMove.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.nudYToMove.Name = "nudYToMove";
            this.nudYToMove.Size = new System.Drawing.Size(238, 29);
            this.nudYToMove.TabIndex = 44;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 24);
            this.label8.TabIndex = 43;
            this.label8.Text = "Y To Move:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(435, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 24);
            this.label6.TabIndex = 42;
            this.label6.Text = "(um)";
            // 
            // nudXToMove
            // 
            this.nudXToMove.Location = new System.Drawing.Point(181, 26);
            this.nudXToMove.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudXToMove.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.nudXToMove.Name = "nudXToMove";
            this.nudXToMove.Size = new System.Drawing.Size(238, 29);
            this.nudXToMove.TabIndex = 41;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 24);
            this.label5.TabIndex = 40;
            this.label5.Text = "X To Move:";
            // 
            // btnMoveX
            // 
            this.btnMoveX.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveX.Location = new System.Drawing.Point(18, 136);
            this.btnMoveX.Name = "btnMoveX";
            this.btnMoveX.Size = new System.Drawing.Size(82, 62);
            this.btnMoveX.TabIndex = 46;
            this.btnMoveX.Text = "Move X";
            this.btnMoveX.UseVisualStyleBackColor = true;
            this.btnMoveX.Click += new System.EventHandler(this.btnMoveX_Click);
            // 
            // btnMoveY
            // 
            this.btnMoveY.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveY.Location = new System.Drawing.Point(200, 136);
            this.btnMoveY.Name = "btnMoveY";
            this.btnMoveY.Size = new System.Drawing.Size(82, 62);
            this.btnMoveY.TabIndex = 47;
            this.btnMoveY.Text = "Move Y";
            this.btnMoveY.UseVisualStyleBackColor = true;
            this.btnMoveY.Click += new System.EventHandler(this.btnMoveY_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.Location = new System.Drawing.Point(402, 136);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(82, 62);
            this.buttonClose.TabIndex = 48;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // ProductionOutputMotionMove
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.ClientSize = new System.Drawing.Size(546, 239);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.btnMoveY);
            this.Controls.Add(this.btnMoveX);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.nudYToMove);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nudXToMove);
            this.Controls.Add(this.label5);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "ProductionOutputMotionMove";
            this.Text = "ProductionOutputMotionMove";
            ((System.ComponentModel.ISupportInitialize)(this.nudYToMove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudXToMove)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudYToMove;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudXToMove;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnMoveX;
        private System.Windows.Forms.Button btnMoveY;
        private System.Windows.Forms.Button buttonClose;
    }
}