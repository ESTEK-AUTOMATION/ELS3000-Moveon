namespace Product
{
    partial class tabpageDelay
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Input Pusher", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Marking Vision Pusher", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Rotary And Aligner A Pusher", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Rotary And Aligner B Pusher", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Five S Pusher", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Sorting Pusher", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Tape And Reel Pusher", System.Windows.Forms.HorizontalAlignment.Left);
            this.listViewDelay = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelDelayRecipeManagement = new System.Windows.Forms.Panel();
            this.labelMessageRecipeDelay = new System.Windows.Forms.Label();
            this.buttonSaveAsRecipeDelay = new System.Windows.Forms.Button();
            this.labelRecipeDelay = new System.Windows.Forms.Label();
            this.richTextBoxMessageRecipeDelay = new System.Windows.Forms.RichTextBox();
            this.buttonDeleteRecipeDelay = new System.Windows.Forms.Button();
            this.buttonApplyAndSaveRecipeDelay = new System.Windows.Forms.Button();
            this.comboBoxRecipeDelay = new System.Windows.Forms.ComboBox();
            this.buttonLoadRecipeDelay = new System.Windows.Forms.Button();
            this.panelDelayRecipeManagement.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewDelay
            // 
            this.listViewDelay.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderValue});
            this.listViewDelay.FullRowSelect = true;
            listViewGroup1.Header = "Input Pusher";
            listViewGroup1.Name = "ListViewGroupInputPusher";
            listViewGroup2.Header = "Marking Vision Pusher";
            listViewGroup2.Name = "ListViewGroupMarkingVisionPusher";
            listViewGroup3.Header = "Rotary And Aligner A Pusher";
            listViewGroup3.Name = "listViewGroupRotaryAndAlignerAPusher";
            listViewGroup4.Header = "Rotary And Aligner B Pusher";
            listViewGroup4.Name = "listViewGroupRotaryAndAlignerBPusher";
            listViewGroup5.Header = "Five S Pusher";
            listViewGroup5.Name = "listViewGroupFiveSPusher";
            listViewGroup6.Header = "Sorting Pusher";
            listViewGroup6.Name = "listViewGroupSortingPusher";
            listViewGroup7.Header = "Tape And Reel Pusher";
            listViewGroup7.Name = "listViewGroupTapeAndReelPusher";
            this.listViewDelay.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7});
            this.listViewDelay.HideSelection = false;
            this.listViewDelay.Location = new System.Drawing.Point(0, 272);
            this.listViewDelay.Name = "listViewDelay";
            this.listViewDelay.Size = new System.Drawing.Size(904, 385);
            this.listViewDelay.TabIndex = 26;
            this.listViewDelay.UseCompatibleStateImageBehavior = false;
            this.listViewDelay.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 721;
            // 
            // columnHeaderValue
            // 
            this.columnHeaderValue.Text = "Value (ms)";
            this.columnHeaderValue.Width = 178;
            // 
            // panelDelayRecipeManagement
            // 
            this.panelDelayRecipeManagement.Controls.Add(this.labelMessageRecipeDelay);
            this.panelDelayRecipeManagement.Controls.Add(this.buttonSaveAsRecipeDelay);
            this.panelDelayRecipeManagement.Controls.Add(this.labelRecipeDelay);
            this.panelDelayRecipeManagement.Controls.Add(this.richTextBoxMessageRecipeDelay);
            this.panelDelayRecipeManagement.Controls.Add(this.buttonDeleteRecipeDelay);
            this.panelDelayRecipeManagement.Controls.Add(this.buttonApplyAndSaveRecipeDelay);
            this.panelDelayRecipeManagement.Controls.Add(this.comboBoxRecipeDelay);
            this.panelDelayRecipeManagement.Controls.Add(this.buttonLoadRecipeDelay);
            this.panelDelayRecipeManagement.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDelayRecipeManagement.Location = new System.Drawing.Point(0, 0);
            this.panelDelayRecipeManagement.Name = "panelDelayRecipeManagement";
            this.panelDelayRecipeManagement.Size = new System.Drawing.Size(961, 214);
            this.panelDelayRecipeManagement.TabIndex = 25;
            // 
            // labelMessageRecipeDelay
            // 
            this.labelMessageRecipeDelay.AutoSize = true;
            this.labelMessageRecipeDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessageRecipeDelay.Location = new System.Drawing.Point(0, 0);
            this.labelMessageRecipeDelay.Name = "labelMessageRecipeDelay";
            this.labelMessageRecipeDelay.Size = new System.Drawing.Size(97, 24);
            this.labelMessageRecipeDelay.TabIndex = 0;
            this.labelMessageRecipeDelay.Text = "Message :";
            // 
            // buttonSaveAsRecipeDelay
            // 
            this.buttonSaveAsRecipeDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSaveAsRecipeDelay.Location = new System.Drawing.Point(686, 136);
            this.buttonSaveAsRecipeDelay.Name = "buttonSaveAsRecipeDelay";
            this.buttonSaveAsRecipeDelay.Size = new System.Drawing.Size(100, 70);
            this.buttonSaveAsRecipeDelay.TabIndex = 14;
            this.buttonSaveAsRecipeDelay.Text = "Save As";
            this.buttonSaveAsRecipeDelay.UseVisualStyleBackColor = true;
            // 
            // labelRecipeDelay
            // 
            this.labelRecipeDelay.AutoSize = true;
            this.labelRecipeDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRecipeDelay.Location = new System.Drawing.Point(1, 101);
            this.labelRecipeDelay.Name = "labelRecipeDelay";
            this.labelRecipeDelay.Size = new System.Drawing.Size(80, 24);
            this.labelRecipeDelay.TabIndex = 3;
            this.labelRecipeDelay.Text = "Recipe :";
            // 
            // richTextBoxMessageRecipeDelay
            // 
            this.richTextBoxMessageRecipeDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxMessageRecipeDelay.Location = new System.Drawing.Point(4, 27);
            this.richTextBoxMessageRecipeDelay.Name = "richTextBoxMessageRecipeDelay";
            this.richTextBoxMessageRecipeDelay.ReadOnly = true;
            this.richTextBoxMessageRecipeDelay.Size = new System.Drawing.Size(900, 65);
            this.richTextBoxMessageRecipeDelay.TabIndex = 1;
            this.richTextBoxMessageRecipeDelay.Text = "";
            // 
            // buttonDeleteRecipeDelay
            // 
            this.buttonDeleteRecipeDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDeleteRecipeDelay.Location = new System.Drawing.Point(804, 136);
            this.buttonDeleteRecipeDelay.Name = "buttonDeleteRecipeDelay";
            this.buttonDeleteRecipeDelay.Size = new System.Drawing.Size(100, 70);
            this.buttonDeleteRecipeDelay.TabIndex = 15;
            this.buttonDeleteRecipeDelay.Text = "Delete";
            this.buttonDeleteRecipeDelay.UseVisualStyleBackColor = true;
            // 
            // buttonApplyAndSaveRecipeDelay
            // 
            this.buttonApplyAndSaveRecipeDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonApplyAndSaveRecipeDelay.Location = new System.Drawing.Point(568, 136);
            this.buttonApplyAndSaveRecipeDelay.Name = "buttonApplyAndSaveRecipeDelay";
            this.buttonApplyAndSaveRecipeDelay.Size = new System.Drawing.Size(100, 70);
            this.buttonApplyAndSaveRecipeDelay.TabIndex = 17;
            this.buttonApplyAndSaveRecipeDelay.Text = "Apply and Save";
            this.buttonApplyAndSaveRecipeDelay.UseVisualStyleBackColor = true;
            // 
            // comboBoxRecipeDelay
            // 
            this.comboBoxRecipeDelay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRecipeDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxRecipeDelay.FormattingEnabled = true;
            this.comboBoxRecipeDelay.Location = new System.Drawing.Point(87, 98);
            this.comboBoxRecipeDelay.Name = "comboBoxRecipeDelay";
            this.comboBoxRecipeDelay.Size = new System.Drawing.Size(817, 32);
            this.comboBoxRecipeDelay.TabIndex = 2;
            // 
            // buttonLoadRecipeDelay
            // 
            this.buttonLoadRecipeDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoadRecipeDelay.Location = new System.Drawing.Point(453, 136);
            this.buttonLoadRecipeDelay.Name = "buttonLoadRecipeDelay";
            this.buttonLoadRecipeDelay.Size = new System.Drawing.Size(100, 70);
            this.buttonLoadRecipeDelay.TabIndex = 16;
            this.buttonLoadRecipeDelay.Text = "Load";
            this.buttonLoadRecipeDelay.UseVisualStyleBackColor = true;
            // 
            // tabpageDelay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCyan;
            this.Controls.Add(this.listViewDelay);
            this.Controls.Add(this.panelDelayRecipeManagement);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "tabpageDelay";
            this.Size = new System.Drawing.Size(961, 707);
            this.panelDelayRecipeManagement.ResumeLayout(false);
            this.panelDelayRecipeManagement.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.ListView listViewDelay;
        public System.Windows.Forms.ColumnHeader columnHeaderName;
        public System.Windows.Forms.ColumnHeader columnHeaderValue;
        public System.Windows.Forms.Panel panelDelayRecipeManagement;
        public System.Windows.Forms.Label labelMessageRecipeDelay;
        public System.Windows.Forms.Button buttonSaveAsRecipeDelay;
        public System.Windows.Forms.Label labelRecipeDelay;
        public System.Windows.Forms.RichTextBox richTextBoxMessageRecipeDelay;
        public System.Windows.Forms.Button buttonDeleteRecipeDelay;
        public System.Windows.Forms.Button buttonApplyAndSaveRecipeDelay;
        public System.Windows.Forms.ComboBox comboBoxRecipeDelay;
        public System.Windows.Forms.Button buttonLoadRecipeDelay;
    }
}
