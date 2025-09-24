using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Common;

namespace Product
{
    public class ProductFormRecipe : Machine.FormRecipe
    {
        public TabPage tabPageInput = new TabPage();
        public TabPage tabPageOutput = new TabPage();
        public TabPage tabPageMotorPosition = new TabPage();
        public TabPage tabPageDelay = new TabPage();
        public TabPage tabPageInputCassette = new TabPage();
        public TabPage tabPageOutputCassette = new TabPage();
        public TabPage tabPageOutputFile = new TabPage();
        public TabPage tabPageVision = new TabPage();
        //public TabPage tabPageSorting = new TabPage();
        public TabPage tabPagePickUpHead = new TabPage();
        //public UserControl SortingSubPage = new UserControl();
        public groupboxRecipeControl groupboxMainRecipeControl = new groupboxRecipeControl();

        public tabpageMainInputXYTable m_tabpageMainRecipe = new tabpageMainInputXYTable();
        public tabpageInput m_tabpageInput = new tabpageInput();
        public tabpageOutput m_tabpageOutput = new tabpageOutput();
        public tabpageMotorPosition m_tabpageMotorPosition = new tabpageMotorPosition();
        public tabpageDelay m_tabpageDelay = new tabpageDelay();
        //public tabpageCassette m_tabpageInputCassette = new tabpageCassette();
        //public tabpageCassette m_tabpageOutputCassette = new tabpageCassette();
        public tabpageOutputFile m_tabpageOutputFile = new tabpageOutputFile();
        public tabpageVisionRecipe m_tabpageVisionRecipe = new tabpageVisionRecipe();
        //public tabpageSortingRecipe m_tabpageSortingRecipe = new tabpageSortingRecipe();
        public tabpagePickUpHead m_tabpagePickUpHeadRecipe = new tabpagePickUpHead();

        
        public string m_strRecipeMainPath = "..\\Recipe\\Main\\";
        public string m_strRecipeInputPath = "..\\Recipe\\Input\\";
        public string m_strRecipeOutputPath = "..\\Recipe\\Output\\";
        public string m_strRecipeDelayPath = "..\\Recipe\\Delay\\";
        public string m_strRecipeMotorPositionPath = "..\\Recipe\\MotorPosition\\";
        public string m_strRecipeOutputFilePath = "..\\Recipe\\OutputFile\\";
        
        #region Recipe File Path
        public string m_strRecipeInputCassettePath = "..\\Recipe\\Input Cassette\\";
        public string m_strRecipeOutputCassettePath = "..\\Recipe\\Output Cassette\\";
        public string m_strRecipeVisionPath = "..\\Recipe\\Vision\\";
        public string m_strRecipeSortingPath = "..\\Recipe\\Sorting\\";
        public string m_strRecipePickUpHeadPath = "..\\Recipe\\PickUpHead\\";
        public string m_strRecipeBarcodePath = "..\\Recipe\\Barcode\\";
        #endregion Recipe File Path        

        public ProductRecipeMainSettings m_ProductRecipeMainSettings = new ProductRecipeMainSettings();
        public ProductRecipeInputSettings m_ProductRecipeInputSettings = new ProductRecipeInputSettings();
        public ProductRecipeOutputSettings m_ProductRecipeOutputSettings = new ProductRecipeOutputSettings();
        public ProductRecipeDelaySettings m_ProductRecipeDelaySettings;// = new ProductRecipeDelaySettings();
        public ProductRecipeMotorPositionSettings m_ProductRecipeMotorPositionSettings = new ProductRecipeMotorPositionSettings();
        public ProductRecipeOutputFileSettings m_ProductRecipeOutputFileSettings = new ProductRecipeOutputFileSettings();

        #region Recipe Setting

        public ProductRecipeCassetteSettings m_ProductRecipeInputCassetteSettings = new ProductRecipeCassetteSettings();
        public ProductRecipeCassetteSettings m_ProductRecipeOutputCassetteSettings = new ProductRecipeCassetteSettings();
        public ProductRecipeVisionSettings m_ProductRecipeVisionSettings = new ProductRecipeVisionSettings();
        public ProductRecipeSortingSetting m_ProductRecipeSortingSetting = new ProductRecipeSortingSetting();
        public ProductRecipePickUpHeadSeting m_ProductRecipePickUpHeadSetting = new ProductRecipePickUpHeadSeting();
        public InfoSorting m_ProductRecipeSortingSetting_ListInfo = new InfoSorting();
        #endregion Recipe Setting

        private TextBox m_textboxEditListViewMotorPosition = new TextBox();
        private TextBox m_textboxEditListViewDelay = new TextBox();
        ListView.SelectedListViewItemCollection selectedItem;

        public int m_nLastSelectedDelayItem = -1;

        Task UploadrecipeTask;

        private ProductShareVariables m_ProductShareVariables;
        private ProductProcessEvent m_ProductProcessEvent;// = new ProductProcessEvent();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess();

        public ProductShareVariables productShareVariables
        {
            set
            {
                m_ProductShareVariables = value;
                shareVariables = m_ProductShareVariables;
            }
        }

        public ProductProcessEvent productProcessEvent
        {
            set { m_ProductProcessEvent = value;
                processEvent = m_ProductProcessEvent;
            }
        }

        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
            }
        }
        public ProductRecipeMainSettings productRecipeMainSettings
        {
            set
            {
                m_ProductRecipeMainSettings = value;
            }
        }

        public ProductRecipeInputSettings productRecipeInputSettings
        {
            set
            {
                m_ProductRecipeInputSettings = value;
            }
        }

        public ProductRecipeOutputSettings productRecipeOutputSettings
        {
            set
            {
                m_ProductRecipeOutputSettings = value;
            }
        }

        public ProductRecipeDelaySettings productRecipeDelaySettings
        {
            set
            {
                m_ProductRecipeDelaySettings = value;
            }
        }

        public ProductRecipeMotorPositionSettings productRecipeMotorPositionSettings
        {
            set
            {
                m_ProductRecipeMotorPositionSettings = value;
            }
        }

        public ProductRecipeOutputFileSettings productRecipeOutputFileSettings
        {
            set
            {
                m_ProductRecipeOutputFileSettings = value;
            }
        }

        public ProductRecipeCassetteSettings productRecipeInputCassetteSettings
        {
            set
            {
                m_ProductRecipeInputCassetteSettings = value;
            }
        }

        public ProductRecipeCassetteSettings productRecipeOutputCassetteSettings
        {
            set
            {
                m_ProductRecipeOutputCassetteSettings = value;
            }
        }

        public ProductRecipeVisionSettings productRecipeVisionSettings
        {
            set
            {
                m_ProductRecipeVisionSettings = value;
            }
        }

        public ProductRecipeSortingSetting productRecipeSortingSettings
        {
            set
            {
                m_ProductRecipeSortingSetting = value;
            }
        }

        public ProductRecipePickUpHeadSeting productRecipePickUpHeadSettings
        {
            set
            {
                m_ProductRecipePickUpHeadSetting = value;
            }
        }

        public ProductFormRecipe()
        {

        }

        override public void Initialize()
        {
            base.Initialize();
            tabPageInput.BackColor = System.Drawing.Color.LightCyan;
            tabPageInput.Controls.Add(m_tabpageInput);
            tabPageInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabPageInput.Location = new System.Drawing.Point(4, 22);
            tabPageInput.Name = "tabPageInput";
            tabPageInput.Padding = new System.Windows.Forms.Padding(3);
            //tabPageInput.Size = new System.Drawing.Size(1896, 816);
            tabPageInput.TabIndex = 1;
            tabPageInput.Text = "Input";

            tabPageOutput.BackColor = System.Drawing.Color.LightCyan;
            tabPageOutput.Controls.Add(m_tabpageOutput);
            tabPageOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabPageOutput.Location = new System.Drawing.Point(4, 22);
            tabPageOutput.Name = "tabPageInput";
            tabPageOutput.Padding = new System.Windows.Forms.Padding(3);
            //tabPageOutput.Size = new System.Drawing.Size(1896, 816);
            tabPageOutput.TabIndex = 2;
            tabPageOutput.Text = "Output";

            tabPageMotorPosition.BackColor = System.Drawing.Color.LightCyan;
            tabPageMotorPosition.Controls.Add(m_tabpageMotorPosition);
            tabPageMotorPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabPageMotorPosition.Location = new System.Drawing.Point(4, 22);
            tabPageMotorPosition.Name = "tabPageMotorPosition";
            tabPageMotorPosition.Padding = new System.Windows.Forms.Padding(3);
            //tabPageMotorPosition.Size = new System.Drawing.Size(1896, 816);
            tabPageMotorPosition.TabIndex = 3;
            tabPageMotorPosition.Text = "Motor Position";

            tabPageDelay.BackColor = System.Drawing.Color.LightCyan;
            tabPageDelay.Controls.Add(m_tabpageDelay);
            tabPageDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabPageDelay.Location = new System.Drawing.Point(4, 22);
            tabPageDelay.Name = "tabPageMotorDelay";
            tabPageDelay.Padding = new System.Windows.Forms.Padding(3);
            //tabPageDelay.Size = new System.Drawing.Size(1896, 816);
            tabPageDelay.TabIndex = 4;
            tabPageDelay.Text = "Delay";

            //tabPageInputCassette.BackColor = System.Drawing.Color.LightCyan;
            //tabPageInputCassette.Controls.Add(m_tabpageInputCassette);
            //tabPageInputCassette.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //tabPageInputCassette.Location = new System.Drawing.Point(4, 22);
            //tabPageInputCassette.Name = "tabPageCassette";
            //tabPageInputCassette.Padding = new System.Windows.Forms.Padding(3);
            ////tabPageCassette.Size = new System.Drawing.Size(1896, 816);
            //tabPageInputCassette.TabIndex = 5;
            //tabPageInputCassette.Text = "Input Cassette";

            //tabPageOutputCassette.BackColor = System.Drawing.Color.LightCyan;
            //tabPageOutputCassette.Controls.Add(m_tabpageOutputCassette);
            //tabPageOutputCassette.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //tabPageOutputCassette.Location = new System.Drawing.Point(4, 22);
            //tabPageOutputCassette.Name = "tabPageOutputCassette";
            //tabPageOutputCassette.Padding = new System.Windows.Forms.Padding(3);
            ////tabPageOutputCassette.Size = new System.Drawing.Size(1896, 816);
            //tabPageOutputCassette.TabIndex = 6;
            //tabPageOutputCassette.Text = "Output Cassette";

            tabPageVision.BackColor = System.Drawing.Color.LightCyan;
            tabPageVision.Controls.Add(m_tabpageVisionRecipe);
            tabPageVision.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabPageVision.Location = new System.Drawing.Point(4, 22);
            tabPageVision.Name = "tabPageVision";
            tabPageVision.Padding = new System.Windows.Forms.Padding(3);
            //tabPageVision.Size = new System.Drawing.Size(1896, 816);
            tabPageVision.TabIndex = 5;
            tabPageVision.Text = "Vision";

            tabPageOutputFile.BackColor = System.Drawing.Color.LightCyan;
            tabPageOutputFile.Controls.Add(m_tabpageOutputFile);
            tabPageOutputFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabPageOutputFile.Location = new System.Drawing.Point(4, 22);
            tabPageOutputFile.Name = "tabPageOutputFile";
            tabPageOutputFile.Padding = new System.Windows.Forms.Padding(3);
            //tabPageOutputFile.Size = new System.Drawing.Size(1896, 816);
            tabPageOutputFile.TabIndex = 6;
            tabPageOutputFile.Text = "Output File";

            //tabPageSorting.BackColor = System.Drawing.Color.LightCyan;
            //tabPageSorting.Controls.Add(m_tabpageSortingRecipe);
            //tabPageSorting.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //tabPageSorting.Location = new System.Drawing.Point(4, 22);
            //tabPageSorting.Name = "tabPageSorting";
            //tabPageSorting.Padding = new System.Windows.Forms.Padding(3);
            ////tabPageOutputFile.Size = new System.Drawing.Size(1896, 816);
            //tabPageSorting.TabIndex = 7;
            //tabPageSorting.Text = "Sorting";

            tabPagePickUpHead.BackColor = System.Drawing.Color.LightCyan;
            tabPagePickUpHead.Controls.Add(m_tabpagePickUpHeadRecipe);
            tabPagePickUpHead.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabPagePickUpHead.Location = new System.Drawing.Point(4, 22);
            tabPagePickUpHead.Name = "tabPagePickUpHead";
            tabPagePickUpHead.Padding = new System.Windows.Forms.Padding(3);
            //tabPageOutputFile.Size = new System.Drawing.Size(1896, 816);
            tabPagePickUpHead.TabIndex = 8;
            tabPagePickUpHead.Text = "Pick Up Head";

            base.tabPageMain.Controls.Add(groupboxMainRecipeControl);
            base.tabPageMain.Controls.Add(m_tabpageMainRecipe);
            base.tabControl.TabPages.Add(tabPageInput);
            base.tabControl.TabPages.Add(tabPageOutput);
            base.tabControl.TabPages.Add(tabPageMotorPosition);
            base.tabControl.TabPages.Add(tabPageDelay);
            //base.tabControl.TabPages.Add(tabPageInputCassette);
            //base.tabControl.TabPages.Add(tabPageOutputCassette);
            base.tabControl.TabPages.Add(tabPageVision);
            base.tabControl.TabPages.Add(tabPageOutputFile);
            //base.tabControl.TabPages.Add(tabPageSorting);
            base.tabControl.TabPages.Add(tabPagePickUpHead);
            //m_tabpageVisionRecipe.tabControlVision.Controls.Remove(m_tabpageVisionRecipe.tabPageOutputVision);
            m_tabpageMainRecipe.Location = new System.Drawing.Point(0, 220);
        }

        override public int GenerateFormEvent()
        {
            int nError = 0;

            groupboxMainRecipeControl.buttonLoadRecipeMain.Click += new System.EventHandler(buttonLoadRecipeMain_Click);
            groupboxMainRecipeControl.buttonApplyAndSaveRecipeMain.Click += new System.EventHandler(buttonApplyAndSaveRecipeMain_Click);
            groupboxMainRecipeControl.buttonSaveAsRecipeMain.Click += new System.EventHandler(buttonSaveAsRecipeMain_Click);
            groupboxMainRecipeControl.buttonDeleteRecipeMain.Click += new System.EventHandler(buttonDeleteRecipeMain_Click);
            groupboxMainRecipeControl.comboBoxRecipeMain.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeMain_SelectedIndexChanged);

            m_tabpageInput.richTextBoxMessageRecipeInput.TextChanged += new EventHandler(richTextBoxMessageRecipeInput_TextChanged);
            m_tabpageInput.buttonLoadRecipeInput.Click += new System.EventHandler(buttonLoadRecipeInput_Click);
            m_tabpageInput.buttonApplyAndSaveRecipeInput.Click += new System.EventHandler(buttonApplyAndSaveRecipeInput_Click);
            m_tabpageInput.buttonSaveAsRecipeInput.Click += new System.EventHandler(buttonSaveAsRecipeInput_Click);
            m_tabpageInput.buttonDeleteRecipeInput.Click += new System.EventHandler(buttonDeleteRecipeInput_Click);
            m_tabpageInput.comboBoxRecipeInput.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeInput_SelectedIndexChanged);
            //m_tabpageInput.checkBoxVisionCameraBarcode.CheckedChanged += new System.EventHandler(checkBoxVisionCameraBarcode_CheckedChanged);
            m_tabpageInput.checkBoxEnableCheckmptyUnit.CheckedChanged += new System.EventHandler(checkBoxEnableCheckmptyUnit_CheckedChanged);

            m_tabpageOutput.richTextBoxMessageRecipeOutput.TextChanged += new EventHandler(richTextBoxMessageRecipeOutput_TextChanged);
            m_tabpageOutput.buttonLoadRecipeOutput.Click += new System.EventHandler(buttonLoadRecipeOutput_Click);
            m_tabpageOutput.buttonApplyAndSaveRecipeOutput.Click += new System.EventHandler(buttonApplyAndSaveRecipeOutput_Click);
            m_tabpageOutput.buttonSaveAsRecipeOutput.Click += new System.EventHandler(buttonSaveAsRecipeOutput_Click);
            m_tabpageOutput.buttonDeleteRecipeOutput.Click += new System.EventHandler(buttonDeleteRecipeOutput_Click);
            m_tabpageOutput.comboBoxRecipeOutput.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeOutput_SelectedIndexChanged);
            m_tabpageOutput.checkBoxGoodSampling.CheckedChanged += new System.EventHandler(checkBoxGoodSampling_CheckedChanged);

            m_tabpageMotorPosition.richTextBoxMessageRecipeMotorPosition.TextChanged += new EventHandler(richTextBoxMessageRecipeMotorPosition_TextChanged);
            m_tabpageMotorPosition.buttonLoadRecipeMotorPosition.Click += new System.EventHandler(buttonLoadRecipeMotorPosition_Click);
            m_tabpageMotorPosition.buttonApplyAndSaveRecipeMotorPosition.Click += new System.EventHandler(buttonApplyAndSaveRecipeMotorPosition_Click);
            m_tabpageMotorPosition.buttonSaveAsRecipeMotorPosition.Click += new System.EventHandler(buttonSaveAsRecipeMotorPosition_Click);
            m_tabpageMotorPosition.buttonDeleteRecipeMotorPosition.Click += new System.EventHandler(buttonDeleteRecipeMotorPosition_Click);
            m_tabpageMotorPosition.listViewMotorPosition.Click += new System.EventHandler(listViewMotorPosition_Click);            
            m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeMotorPosition_SelectedIndexChanged);
           
            m_tabpageMotorPosition.listViewMotorPosition.MouseWheel += new System.Windows.Forms.MouseEventHandler(textboxEditListViewMotorPosition_ScrollLostFocus);
            //m_tabpageMotorPosition.listViewMotorPosition.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(textboxEditListViewMotorPosition_SelectedItemChanged);
            //m_tabpageMotorPosition.listViewMotorPosition.LostFocus += new EventHandler(textboxEditListViewMotorPosition_LostFocus);

            m_textboxEditListViewMotorPosition.Parent = m_tabpageMotorPosition.listViewMotorPosition;
            m_textboxEditListViewMotorPosition.Hide();
            m_textboxEditListViewMotorPosition.LostFocus += new EventHandler(textboxEditListViewMotorPosition_LostFocus);
            m_textboxEditListViewMotorPosition.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textboxEditListViewMotorPosition_KeyPress);

            m_textboxEditListViewMotorPosition.MouseLeave += new EventHandler(textboxEditListViewMotorPosition_LostFocus);

            m_tabpageDelay.richTextBoxMessageRecipeDelay.TextChanged += new EventHandler(richTextBoxMessageRecipeDelay_TextChanged);
            m_tabpageDelay.buttonLoadRecipeDelay.Click += new System.EventHandler(buttonLoadRecipeDelay_Click);
            m_tabpageDelay.buttonApplyAndSaveRecipeDelay.Click += new System.EventHandler(buttonApplyAndSaveRecipeDelay_Click);
            m_tabpageDelay.buttonSaveAsRecipeDelay.Click += new System.EventHandler(buttonSaveAsRecipeDelay_Click);
            m_tabpageDelay.buttonDeleteRecipeDelay.Click += new System.EventHandler(buttonDeleteRecipeDelay_Click);
            m_tabpageDelay.listViewDelay.MouseClick += new System.Windows.Forms.MouseEventHandler(listViewDelay_MouseClick);            
            m_tabpageDelay.comboBoxRecipeDelay.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeDelay_SelectedIndexChanged);

            m_tabpageDelay.listViewDelay.MouseWheel += new System.Windows.Forms.MouseEventHandler(textboxEditListViewDelay_ScrollLostFocus);
            //m_tabpageDelay.listViewDelay.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(textboxEditListViewDelay_SelectedItemChanged);
            //m_tabpageDelay.listViewDelay.LostFocus += new EventHandler(textboxEditListViewDelay_LostFocus);

            m_textboxEditListViewDelay.Parent = m_tabpageDelay.listViewDelay;
            m_textboxEditListViewDelay.Hide();
            m_textboxEditListViewDelay.LostFocus += new EventHandler(textboxEditListViewDelay_LostFocus);
            m_textboxEditListViewDelay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textboxEditListViewDelay_KeyPress);

            m_textboxEditListViewDelay.MouseLeave += new EventHandler(textboxEditListViewDelay_LostFocus);

            m_textboxEditListViewDelay.TextAlign = HorizontalAlignment.Center;
            m_tabpageDelay.listViewDelay.Columns[1].TextAlign = HorizontalAlignment.Center;
            m_textboxEditListViewMotorPosition.TextAlign = HorizontalAlignment.Center;
            m_tabpageMotorPosition.listViewMotorPosition.Columns[1].TextAlign = HorizontalAlignment.Center;

            //m_tabpageInputCassette.buttonLoadRecipeCassette.Click += new System.EventHandler(buttonLoadRecipeInputCassette_Click);
            //m_tabpageInputCassette.buttonApplyAndSaveRecipeCassette.Click += new System.EventHandler(buttonApplyAndSaveRecipeInputCassette_Click);
            //m_tabpageInputCassette.buttonSaveAsRecipeCassette.Click += new System.EventHandler(buttonSaveAsRecipeInputCassette_Click);
            //m_tabpageInputCassette.buttonDeleteRecipeCassette.Click += new System.EventHandler(buttonDeleteRecipeInputCassette_Click);
            //m_tabpageInputCassette.richTextBoxMessageRecipeCassette.TextChanged += new System.EventHandler(richTextBoxMessageRecipeInputCassette_TextChanged);
            //m_tabpageInputCassette.comboBoxRecipeCassette.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeInputCassette_SelectedIndexChanged);

            //m_tabpageOutputCassette.buttonLoadRecipeCassette.Click += new System.EventHandler(buttonLoadRecipeOutputCassette_Click);
            //m_tabpageOutputCassette.buttonApplyAndSaveRecipeCassette.Click += new System.EventHandler(buttonApplyAndSaveRecipeOutputCassette_Click);
            //m_tabpageOutputCassette.buttonSaveAsRecipeCassette.Click += new System.EventHandler(buttonSaveAsRecipeOutputCassette_Click);
            //m_tabpageOutputCassette.buttonDeleteRecipeCassette.Click += new System.EventHandler(buttonDeleteRecipeOutputCassette_Click);
            //m_tabpageOutputCassette.richTextBoxMessageRecipeCassette.TextChanged += new System.EventHandler(richTextBoxMessageRecipeOutputCassette_TextChanged);
            //m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeOutputCassette_SelectedIndexChanged);

            m_tabpageVisionRecipe.buttonLoadRecipeVision.Click += new System.EventHandler(buttonLoadRecipeVision_Click);
            m_tabpageVisionRecipe.buttonApplyAndSaveRecipeVision.Click += new System.EventHandler(buttonApplyAndSaveRecipeVision_Click);
            m_tabpageVisionRecipe.buttonSaveAsRecipeVision.Click += new System.EventHandler(buttonSaveAsRecipeVision_Click);
            m_tabpageVisionRecipe.buttonDeleteRecipeVision.Click += new System.EventHandler(buttonDeleteRecipeVision_Click);
            m_tabpageVisionRecipe.richTextBoxMessageRecipeVision.TextChanged += new System.EventHandler(richTextBoxMessageRecipeVision_TextChanged);
            m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeVision_SelectedIndexChanged);
            
            m_tabpageOutputFile.buttonLoadRecipeOutputFile.Click += new System.EventHandler(buttonLoadRecipeOutputFile_Click);
            m_tabpageOutputFile.buttonApplyAndSaveRecipeOutputFile.Click += new System.EventHandler(buttonApplyAndSaveRecipeOutputFile_Click);
            m_tabpageOutputFile.buttonSaveAsRecipeOutputFile.Click += new System.EventHandler(buttonSaveAsRecipeOutputFile_Click);
            m_tabpageOutputFile.buttonDeleteRecipeOutputFile.Click += new System.EventHandler(buttonDeleteRecipeOutputFile_Click);
            m_tabpageOutputFile.richTextBoxMessageRecipeOutputFile.TextChanged += new System.EventHandler(richTextBoxRecipeOutputFile_TextChanged);
            m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeOutputFile_SelectedIndexChanged);


            //m_tabpageOutputFile.buttonBrowseInputFilePath.Click += new System.EventHandler(this.buttonBrowseInputFilePath_Click);
            m_tabpageOutputFile.buttonBrowseOutputFile.Click += new System.EventHandler(this.buttonBrowseOutputFile_Click);
            m_tabpageOutputFile.buttonBrowseOutputLocalFile.Click += new System.EventHandler(this.buttonBrowseOutputLocalFile_Click);

            //m_tabpageSortingRecipe.buttonLoadRecipeSorting.Click += new System.EventHandler(buttonLoadRecipeSorting_Click);
            //m_tabpageSortingRecipe.buttonApplyAndSaveRecipeSorting.Click += new System.EventHandler(buttonApplyAndSaveRecipeSorting_Click);
            //m_tabpageSortingRecipe.buttonSaveAsRecipeSorting.Click += new System.EventHandler(buttonSaveAsRecipeSorting_Click);
            //m_tabpageSortingRecipe.buttonDeleteRecipeSorting.Click += new System.EventHandler(buttonDeleteRecipeSorting_Click);
            //m_tabpageSortingRecipe.richTextBoxMessageRecipeSorting.TextChanged += new System.EventHandler(richTextBoxMessageRecipeSorting_TextChanged);
            //m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedIndexChanged += new System.EventHandler(comboBoxRecipeSorting_SelectedIndexChanged);

            //m_tabpageSortingRecipe.radioButtonByRecipe.CheckedChanged += new System.EventHandler(this.radioButtonByRecipe_CheckedChanged);
            //m_tabpageSortingRecipe.radioButtonByInputFile.CheckedChanged += new System.EventHandler(this.radioButtonByInputFile_CheckedChanged);
            //m_tabpageSortingRecipe.buttonBrowseSortingFilePath.Click += new System.EventHandler(this.buttonBrowseSortingFilePath_Click);

            //m_tabpageSortingRecipe.buttonLoadSortingBarcode.Click += new System.EventHandler(buttonLoadSortingBarcode_Click);
            //m_tabpageSortingRecipe.buttonRemoveSortingBarcode.Click += new System.EventHandler(buttonRemoveSortingBarcode_Click);
            //m_tabpageSortingRecipe.buttonAddSortingBarcode.Click += new System.EventHandler(buttonAddSortingBarcode_Click);
            //m_tabpageSortingRecipe.comboBoxSortingBarcode.SelectedIndexChanged += new System.EventHandler(comboBoxSortingBarcode_SelectedIndexChanged);

            m_tabpageOutputFile.comboBoxDefectColor.SelectedIndexChanged += new System.EventHandler(this.comboBoxDefectColor_SelectedIndexChanged);
            m_tabpageOutputFile.buttonAddDefect.Click += new System.EventHandler(this.buttonAddDefect_Click);
            m_tabpageOutputFile.buttonRemoveDefect.Click += new System.EventHandler(this.buttonRemoveDefect_Click);
            m_tabpageOutputFile.listViewDefect.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewDefect_MouseClick);
            m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.SelectedIndexChanged += new System.EventHandler(this.comboBoxEmptyUnitDefectColor_SelectedIndexChanged);
            m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.SelectedIndexChanged += new System.EventHandler(this.comboBoxUnitSlantedDefectColor_SelectedIndexChanged);

            m_tabpagePickUpHeadRecipe.buttonLoadRecipePickUpHead.Click += new System.EventHandler(buttonLoadRecipePickUpHead_Click);
            m_tabpagePickUpHeadRecipe.buttonApplyAndSaveRecipePickUpHead.Click += new System.EventHandler(buttonApplyAndSaveRecipePickUpHead_Click);
            m_tabpagePickUpHeadRecipe.buttonSaveAsRecipePickUpHead.Click += new System.EventHandler(buttonSaveAsRecipePickUpHead_Click);
            m_tabpagePickUpHeadRecipe.buttonDeleteRecipePickUpHead.Click += new System.EventHandler(buttonDeleteRecipePickUpHead_Click);
            m_tabpagePickUpHeadRecipe.richTextBoxMessageRecipePickUpHead.TextChanged += new System.EventHandler(richTextBoxMessageRecipePickUpHead_TextChanged);
            m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedIndexChanged += new System.EventHandler(comboBoxRecipePickUpHead_SelectedIndexChanged);


            m_tabpageVisionRecipe.buttonAddInputSnap.Click += ButtonAddInputSnap_Click;
            m_tabpageVisionRecipe.buttonRemoveInputSnap.Click += ButtonRemoveInputSnap_Click;
            m_tabpageVisionRecipe.listViewInputSnap.MouseClick += ListViewInputSnap_MouseClick;

            m_tabpageVisionRecipe.buttonAddS1Snap.Click += ButtonAddS1Snap_Click;
            m_tabpageVisionRecipe.buttonRemoveS1Snap.Click += ButtonRemoveS1Snap_Click;
            m_tabpageVisionRecipe.listViewS1Snap.MouseClick += ListViewS1Snap_MouseClick;

            m_tabpageVisionRecipe.buttonAddS2Snap.Click += ButtonAddS2Snap_Click;
            m_tabpageVisionRecipe.buttonRemoveS2Snap.Click += ButtonRemoveS2Snap_Click;
            m_tabpageVisionRecipe.listViewS2Snap.MouseClick += ListViewS2Snap_MouseClick;

            m_tabpageVisionRecipe.buttonAddS2FacetSnap.Click += ButtonAddS2FacetSnap_Click;
            m_tabpageVisionRecipe.buttonRemoveS2FacetSnap.Click += ButtonRemoveS2FacetSnap_Click;
            m_tabpageVisionRecipe.listViewS2FacetSnap.MouseClick += ListViewS2FacetSnap_MouseClick;

            m_tabpageVisionRecipe.buttonAddBottomSnap.Click += ButtonAddBottomSnap_Click;
            m_tabpageVisionRecipe.buttonRemoveBottomSnap.Click += ButtonRemoveBottomSnap_Click;
            m_tabpageVisionRecipe.listViewBottomSnap.MouseClick += ListViewBottomSnap_MouseClick;

            m_tabpageVisionRecipe.buttonAddOutputSnap.Click += ButtonAddOutputSnap_Click;
            m_tabpageVisionRecipe.buttonRemoveOutputSnap.Click += ButtonRemoveOutputSnap_Click;
            m_tabpageVisionRecipe.listViewOutputSnap.MouseClick += ListViewOutputSnap_MouseClick;
            //RefreshBarcodeRecipeList();
            m_tabpageOutput.buttonAddSortingTrayConfiguration.Click += ButtonAddSortingTrayConfiguration_Click;
            m_tabpageOutput.buttonRemoveSortTrayConfiguration.Click += ButtonRemoveSortTrayConfiguration_Click;
            m_tabpageOutput.listViewSortTrayConfiguration.MouseClick += ListViewSortTrayConfiguration_MouseClick;
            return nError;
        }

        #region Form Event
        private void buttonLoadRecipeMain_Click(object sender, EventArgs e)
        {
            try
            {
                if (groupboxMainRecipeControl.comboBoxRecipeMain.SelectedIndex != -1)
                {
                    if (LoadMainSettings())
                    {
                        RefreshMainRecipeParameter();
                        updateRichTextBoxMessageRecipeMain("Load recipe done.");
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Load recipe fail.");
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Please select recipe before click load.");
                }
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeMain(ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonApplyAndSaveRecipeMain_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (groupboxMainRecipeControl.comboBoxRecipeMain.SelectedIndex != -1)
                        {
                            if (VerifyMainRecipe())
                            {
                                if(SaveMainSettings())
                                    updateRichTextBoxMessageRecipeMain("Apply and save recipe succesfully.");
                                else
                                    updateRichTextBoxMessageRecipeMain("Apply and save recipe fail.");
                            }
                            OnSaveMainRecipeDone();
                        }
                        else
                        {
                            updateRichTextBoxMessageRecipeMain("Please select recipe before click Apply and Save.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeMain(ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonSaveAsRecipeMain_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (VerifyMainRecipe())
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeMainPath);
                        saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
                        saveFileDialogRecipe.FileName = "";
                        if (directoryInfo.Exists == false)
                            Directory.CreateDirectory(directoryInfo.FullName);
                        if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
                        {
                            if (SaveAsMainSettings())
                            {
                                string[] stringSeparators = new string[] { "\\" };
                                string[] result;
                                string newRecipeName = "";
                                result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
                                foreach (string i in result)
                                {
                                    if (i.ToLower().EndsWith(".xml"))
                                    {
                                        newRecipeName = i.Substring(0, i.Length - ".xml".Length);
                                    }
                                }
                                RefreshMainRecipeList();
                                groupboxMainRecipeControl.comboBoxRecipeMain.SelectedItem = newRecipeName;
                                updateRichTextBoxMessageRecipeMain("Save recipe settings to file success.");

                                OnSaveMainRecipeDone();
                            }
                            else
                                updateRichTextBoxMessageRecipeMain("Save recipe settings to file fail.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonDeleteRecipeMain_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string filepath = "";

                        if (groupboxMainRecipeControl.comboBoxRecipeMain.SelectedItem != null)
                        {
                            filepath = m_strRecipeMainPath + groupboxMainRecipeControl.comboBoxRecipeMain.SelectedItem.ToString() + m_strRecipeExtension;
                            if (File.Exists(filepath) == true)
                            {
                                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Main Recipe Named: {groupboxMainRecipeControl.comboBoxRecipeMain.SelectedItem.ToString()}.");
                                File.Delete(filepath);
                                RefreshMainRecipeList();
                                updateRichTextBoxMessageRecipeMain("Recipe deleted.");
                            }
                            else
                            {
                                updateRichTextBoxMessageRecipeMain("Recipe to be deleted not exists.");
                            }
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonLoadRecipeInput_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_tabpageInput.comboBoxRecipeInput.SelectedIndex != -1)
                {
                    if (LoadInputSettings())
                    {
                        RefreshInputRecipeParameter();
                        updateRichTextBoxMessageRecipeInput("Load recipe done.");
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeInput("Load recipe fail.");
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Please select recipe before click load.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonApplyAndSaveRecipeInput_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (m_tabpageInput.comboBoxRecipeInput.SelectedIndex != -1)
                        {
                            if (VerifyInputRecipe())
                            {
                                if (SaveInputSettings())
                                {                                    
                                    updateRichTextBoxMessageRecipeInput("Apply and save recipe succesfully.");
                                    OnSaveInputRecipeDone();
                                }
                                else
                                {
                                    updateRichTextBoxMessageRecipeInput("Apply and save recipe fail.");
                                }
                            }
                        }
                        else
                        {
                            updateRichTextBoxMessageRecipeInput("Please select recipe before click Apply and Save.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonSaveAsRecipeInput_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (VerifyInputRecipe())
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeInputPath);
                        saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
                        saveFileDialogRecipe.FileName = "";
                        if (directoryInfo.Exists == false)
                            Directory.CreateDirectory(directoryInfo.FullName);
                        if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
                        {
                            if (SaveAsInputSettings())
                            {

                                string[] stringSeparators = new string[] { "\\" };
                                string[] result;
                                string newRecipeName = "";
                                result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
                                foreach (string i in result)
                                {
                                    //i.ToLower();                                
                                    //if (i.Contains(".xml"))
                                    if (i.ToLower().EndsWith(".xml"))
                                    {
                                        //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
                                        newRecipeName = i.Substring(0, i.Length - ".xml".Length);
                                    }
                                }
                                RefreshInputRecipeList();
                                m_tabpageInput.comboBoxRecipeInput.SelectedItem = newRecipeName;
                                updateRichTextBoxMessageRecipeInput("Save recipe settings to file success.");

                                OnSaveInputRecipeDone();
                            }
                            else
                                updateRichTextBoxMessageRecipeInput("Save recipe settings to file fail.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonDeleteRecipeInput_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string filepath = "";

                        if (m_tabpageInput.comboBoxRecipeInput.SelectedItem != null)
                        {
                            Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Input Recipe Named: {m_tabpageInput.comboBoxRecipeInput.SelectedItem.ToString()}.");
                            filepath = m_strRecipeInputPath + m_tabpageInput.comboBoxRecipeInput.SelectedItem.ToString() + m_strRecipeExtension;
                            if (File.Exists(filepath) == true)
                            {
                                File.Delete(filepath);
                                RefreshInputRecipeList();
                                updateRichTextBoxMessageRecipeInput("Recipe deleted.");
                            }
                            else
                            {
                                updateRichTextBoxMessageRecipeInput("Recipe to be deleted not exists.");
                            }
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        //private void buttonLoadRecipeSorting_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedIndex != -1)
        //        {
        //            if (LoadSortingSettings())
        //            {
        //                RefreshSortingRecipeParameter();
        //                updateRichTextBoxMessageRecipeSorting("Load recipe done.");
        //            }
        //            else
        //            {
        //                updateRichTextBoxMessageRecipeSorting("Load recipe fail.");
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeSorting("Please select recipe before click load.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonApplyAndSaveRecipeSorting_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
        //            || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
        //        {
        //            if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
        //               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        //            {
        //                if (m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedIndex != -1)
        //                    {
        //                    if (VerifySortingRecipe())
        //                    {
        //                        if (SaveSortingSettings())
        //                        {
        //                            updateRichTextBoxMessageRecipeSorting("Apply and save recipe succesfully.");
        //                            OnSaveSortingRecipeDone();
        //                        }
        //                        else
        //                        {
        //                            updateRichTextBoxMessageRecipeSorting("Apply and save recipe fail.");
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    updateRichTextBoxMessageRecipeSorting("Please select recipe before click Apply and Save.");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeSorting("Only work if system in Idle or Setup mode.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonSaveAsRecipeSorting_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
        //            || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
        //        {
        //            if (VerifySortingRecipe())
        //            {
        //                DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeSortingPath);
        //                saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
        //                saveFileDialogRecipe.FileName = "";
        //                if (directoryInfo.Exists == false)
        //                    Directory.CreateDirectory(directoryInfo.FullName);
        //                if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
        //                {
        //                    if (SaveAsSortingSettings())
        //                    {

        //                        string[] stringSeparators = new string[] { "\\" };
        //                        string[] result;
        //                        string newRecipeName = "";
        //                        result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
        //                        foreach (string i in result)
        //                        {
        //                            //i.ToLower();                                
        //                            //if (i.Contains(".xml"))
        //                            if (i.ToLower().EndsWith(".xml"))
        //                            {
        //                                //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
        //                                newRecipeName = i.Substring(0, i.Length - ".xml".Length);
        //                            }
        //                        }
        //                        RefreshSortingRecipeList();
        //                        m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedItem = newRecipeName;
        //                        updateRichTextBoxMessageRecipeSorting("Save recipe settings to file success.");

        //                        OnSaveSortingRecipeDone();
        //                    }
        //                    else
        //                        updateRichTextBoxMessageRecipeSorting("Save recipe settings to file fail.");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeSorting("Only work if system in Idle or Setup mode.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonDeleteRecipeSorting_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
        //            || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
        //        {
        //            if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
        //               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        //            {
        //                string filepath = "";

        //                if (m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedItem != null)
        //                {
        //                    Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Input Recipe Named: {m_tabpageInput.comboBoxRecipeInput.SelectedItem.ToString()}.");
        //                    filepath = m_strRecipeSortingPath + m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedItem.ToString() + m_strRecipeExtension;
        //                    if (File.Exists(filepath) == true)
        //                    {
        //                        File.Delete(filepath);
        //                        RefreshSortingRecipeList();
        //                        updateRichTextBoxMessageRecipeSorting("Recipe deleted.");
        //                    }
        //                    else
        //                    {
        //                        updateRichTextBoxMessageRecipeSorting("Recipe to be deleted not exists.");
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeSorting("Only work if system in Idle or Setup mode.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}
        private void buttonAddDefect_Click(object sender, EventArgs e)
        {
            try
            {
                int nNo = -1;

                if (Int32.TryParse(m_tabpageOutputFile.textBoxDefectNo.Text, out nNo) == false)
                {
                    updateRichTextBoxMessageRecipeOutputFile("Invalid defect no.");
                    return;
                }
                if (nNo <= 0)
                {
                    updateRichTextBoxMessageRecipeOutputFile("Defect no must be greater than 0.");
                    return;
                }
                if (m_tabpageOutputFile.textBoxDefectCode.Text == "")
                {
                    updateRichTextBoxMessageRecipeOutputFile("Please key in defect code.");
                    return;
                }
                if (m_tabpageOutputFile.textBoxDefectDescription.Text == "")
                {
                    updateRichTextBoxMessageRecipeOutputFile("Please key in defect description.");
                    return;
                }
                if (m_tabpageOutputFile.comboBoxDefectColor.SelectedIndex == -1)
                {
                    updateRichTextBoxMessageRecipeOutputFile("Please select defect color.");
                    return;
                }
                if (m_tabpageOutputFile.comboBoxDefectDestination.SelectedIndex == -1)
                {
                    updateRichTextBoxMessageRecipeOutputFile("Please select defect enable pick.");
                    return;
                }
                foreach (ListViewItem item in m_tabpageOutputFile.listViewDefect.Items)
                {
                    if (item.SubItems[0].Text == m_tabpageOutputFile.textBoxDefectNo.Text
                        )
                    {
                        updateRichTextBoxMessageRecipeOutputFile("Defect number already exist.");
                        return;
                    }
                    if (item.SubItems[1].Text == m_tabpageOutputFile.textBoxDefectCode.Text
                            && item.SubItems[2].Text == m_tabpageOutputFile.textBoxDefectDescription.Text
                        )
                    {
                        updateRichTextBoxMessageRecipeOutputFile("Defect code and description already exist.");
                        return;
                    }
                }
                string strEnableDefectInReport = "";
                if (m_tabpageOutputFile.checkBoxEnableInReport.Checked == true)
                    strEnableDefectInReport = "Yes";
                else
                    strEnableDefectInReport = "No";

                string[] strArrayItem = new string[5] { nNo.ToString(), m_tabpageOutputFile.textBoxDefectCode.Text, m_tabpageOutputFile.textBoxDefectDescription.Text, m_tabpageOutputFile.comboBoxDefectDestination.SelectedItem.ToString(), m_tabpageOutputFile.comboBoxDefectColor.SelectedItem.ToString() }; //, strEnableDefectInReport};
                ListViewItem listViewItem = new ListViewItem(strArrayItem);
                m_tabpageOutputFile.listViewDefect.Items.Add(listViewItem);
                m_tabpageOutputFile.listViewDefect.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageOutputFile.labelTotalDefect.Text = "Total : " + m_tabpageOutputFile.listViewDefect.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonRemoveDefect_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem _listViewItem in m_tabpageOutputFile.listViewDefect.SelectedItems)
                {
                    m_tabpageOutputFile.listViewDefect.Items.Remove(_listViewItem);
                }
                m_tabpageOutputFile.labelTotalDefect.Text = "Total : " + m_tabpageOutputFile.listViewDefect.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void comboBoxDefectColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_tabpageOutputFile.pictureBoxDefectColor.BackColor = System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxDefectColor.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void comboBoxEmptyUnitDefectColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_tabpageOutputFile.pictureBoxEmptyUnitDefectColor.BackColor = System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void comboBoxUnitSlantedDefectColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_tabpageOutputFile.pictureBoxUnitSlantedDefectColor.BackColor = System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void listViewDefect_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItem = m_tabpageOutputFile.listViewDefect.SelectedItems;
                foreach (ListViewItem item in selectedItem)
                {
                    m_tabpageOutputFile.textBoxDefectNo.Text = item.SubItems[0].Text;
                    m_tabpageOutputFile.textBoxDefectCode.Text = item.SubItems[1].Text;
                    m_tabpageOutputFile.textBoxDefectDescription.Text = item.SubItems[2].Text;
                    m_tabpageOutputFile.comboBoxDefectDestination.SelectedItem = item.SubItems[3].Text;
                    //if (item.SubItems[4].Text.ToUpper() == "YES")
                    //    m_tabpageOutputFile.checkBoxEnableInReport.Checked = true;
                    //else
                    //    m_tabpageOutputFile.checkBoxEnableInReport.Checked = false;
                    m_tabpageOutputFile.pictureBoxDefectColor.BackColor = System.Drawing.Color.FromName(item.SubItems[4].Text);
                    m_tabpageOutputFile.comboBoxDefectColor.SelectedItem = item.SubItems[4].Text;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void buttonLoadRecipePickUpHead_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedIndex != -1)
                {
                    if (LoadPickUpHeadSettings())
                    {
                        RefreshPickUpHeadRecipeParameter();
                        updateRichTextBoxMessageRecipePickUpHead("Load recipe done.");
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipePickUpHead("Load recipe fail.");
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipePickUpHead("Please select recipe before click load.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonApplyAndSaveRecipePickUpHead_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedIndex != -1)
                        {
                            if (VerifyPickUpHeadRecipe())
                            {
                                if (SavePickUpHeadSettings())
                                {
                                    updateRichTextBoxMessageRecipePickUpHead("Apply and save recipe succesfully.");
                                    OnSavePickUpHeadRecipeDone();
                                }
                                else
                                {
                                    updateRichTextBoxMessageRecipePickUpHead("Apply and save recipe fail.");
                                }
                            }
                        }
                        else
                        {
                            updateRichTextBoxMessageRecipePickUpHead("Please select recipe before click Apply and Save.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipePickUpHead("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonSaveAsRecipePickUpHead_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (VerifyPickUpHeadRecipe())
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipePickUpHeadPath);
                        saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
                        saveFileDialogRecipe.FileName = "";
                        if (directoryInfo.Exists == false)
                            Directory.CreateDirectory(directoryInfo.FullName);
                        if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
                        {
                            if (SaveAsPickUpHeadSettings())
                            {

                                string[] stringSeparators = new string[] { "\\" };
                                string[] result;
                                string newRecipeName = "";
                                result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
                                foreach (string i in result)
                                {
                                    //i.ToLower();                                
                                    //if (i.Contains(".xml"))
                                    if (i.ToLower().EndsWith(".xml"))
                                    {
                                        //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
                                        newRecipeName = i.Substring(0, i.Length - ".xml".Length);
                                    }
                                }
                                RefreshPickUpHeadRecipeList();
                                m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedItem = newRecipeName;
                                updateRichTextBoxMessageRecipePickUpHead("Save recipe settings to file success.");

                                OnSavePickUpHeadRecipeDone();
                            }
                            else
                                updateRichTextBoxMessageRecipePickUpHead("Save recipe settings to file fail.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipePickUpHead("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonDeleteRecipePickUpHead_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string filepath = "";

                        if (m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedItem != null)
                        {
                            Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Input Recipe Named: {m_tabpageInput.comboBoxRecipeInput.SelectedItem.ToString()}.");
                            filepath = m_strRecipePickUpHeadPath + m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedItem.ToString() + m_strRecipeExtension;
                            if (File.Exists(filepath) == true)
                            {
                                File.Delete(filepath);
                                RefreshPickUpHeadRecipeList();
                                updateRichTextBoxMessageRecipePickUpHead("Recipe deleted.");
                            }
                            else
                            {
                                updateRichTextBoxMessageRecipePickUpHead("Recipe to be deleted not exists.");
                            }
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipePickUpHead("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void richTextBoxMessageRecipeInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpageInput.richTextBoxMessageRecipeInput);
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeInput(ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void comboBoxRecipeInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                {
                    if (m_tabpageInput.comboBoxRecipeInput.SelectedIndex != -1)
                    {
                        if (LoadInputSettings() == false)
                        {
                            updateRichTextBoxMessageRecipeInput("Fail to load recipe.");
                        }
                        else
                            RefreshInputRecipeParameter();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeInput("Please select recipe before click load.");
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void checkBoxEnableCheckmptyUnit_CheckedChanged(object sender, EventArgs e)
        {
            if (m_tabpageInput.checkBoxEnableCheckmptyUnit.Checked == true)
            {
                m_tabpageInput.labelEmptyUnit.Visible = true;
                m_tabpageInput.numericUpDownEmptyUnit.Visible = true;
            }
            else
            {
                m_tabpageInput.labelEmptyUnit.Visible = false;
                m_tabpageInput.numericUpDownEmptyUnit.Visible = false;
            }
        }

        private void buttonLoadRecipeOutput_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_tabpageOutput.comboBoxRecipeOutput.SelectedIndex != -1)
                {
                    if (LoadOutputSettings())
                    {
                        RefreshOutputRecipeParameter();
                        updateRichTextBoxMessageRecipeInput("Load recipe done.");
                    }
                    else
                        updateRichTextBoxMessageRecipeInput("Load recipe fail.");
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutput("Please select recipe before click load.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonApplyAndSaveRecipeOutput_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (m_tabpageOutput.comboBoxRecipeOutput.SelectedIndex != -1)
                        {
                            if (VerifyOutputRecipe())
                            {
                                if (SaveOutputSettings())
                                {
                                    updateRichTextBoxMessageRecipeOutput("Apply and save recipe succesfully.");

                                    OnSaveOutputRecipeDone();
                                }
                                else
                                    updateRichTextBoxMessageRecipeOutput("Apply and save recipe fail.");
                            }
                        }
                        else
                        {
                            updateRichTextBoxMessageRecipeOutput("Please select recipe before click Apply and Save.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutput("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonSaveAsRecipeOutput_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (VerifyOutputRecipe())
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeOutputPath);
                        saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
                        saveFileDialogRecipe.FileName = "";
                        if (directoryInfo.Exists == false)
                            Directory.CreateDirectory(directoryInfo.FullName);
                        if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
                        {
                            if (SaveAsOutputSettings())
                            {                                
                                string[] stringSeparators = new string[] { "\\" };
                                string[] result;
                                string newRecipeName = "";
                                result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
                                foreach (string i in result)
                                {
                                    //i.ToLower();                                
                                    //if (i.Contains(".xml"))
                                    if (i.ToLower().EndsWith(".xml"))
                                    {
                                        //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
                                        newRecipeName = i.Substring(0, i.Length - ".xml".Length);
                                    }
                                }
                                RefreshOutputRecipeList();
                                m_tabpageOutput.comboBoxRecipeOutput.SelectedItem = newRecipeName;
                                updateRichTextBoxMessageRecipeOutput("Save recipe settings to file success.");

                                OnSaveOutputRecipeDone();
                            }
                            else
                                updateRichTextBoxMessageRecipeOutput("Save recipe settings to file fail.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutput("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonDeleteRecipeOutput_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string filepath = "";

                        if (m_tabpageOutput.comboBoxRecipeOutput.SelectedItem != null)
                        {
                            filepath = m_strRecipeOutputPath + m_tabpageOutput.comboBoxRecipeOutput.SelectedItem.ToString() + m_strRecipeExtension;
                            if (File.Exists(filepath) == true)
                            {
                                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Output Recipe Named: {m_tabpageOutput.comboBoxRecipeOutput.SelectedItem.ToString()}.");
                                File.Delete(filepath);
                                RefreshOutputRecipeList();
                                updateRichTextBoxMessageRecipeOutput("Recipe deleted.");
                            }
                            else
                            {
                                updateRichTextBoxMessageRecipeOutput("Recipe to be deleted not exists.");
                            }
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutput("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void richTextBoxMessageRecipeOutput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpageOutput.richTextBoxMessageRecipeOutput);
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeOutput(ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void comboBoxRecipeOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                {
                    if (m_tabpageOutput.comboBoxRecipeOutput.SelectedIndex != -1)
                    {
                        if (LoadOutputSettings() == false)
                        {
                            updateRichTextBoxMessageRecipeOutput("Fail to load recipe.");
                        }
                        else
                            RefreshOutputRecipeParameter();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeOutput("Please select recipe before click load.");
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void checkBoxGoodSampling_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_tabpageOutput.checkBoxGoodSampling.Checked == true)
                {
                    if(m_tabpageOutput.comboBoxRejectTray.Items.Contains("1"))
                    {
                        m_tabpageOutput.comboBoxRejectTray.Items.Remove("1");
                    }
                    foreach(ListViewItem _SortItem in m_tabpageOutput.listViewSortTrayConfiguration.Items)
                    {
                        if(_SortItem.SubItems[0].Text == "1")
                        {
                            _SortItem.SubItems[0].Text = "";
                        }
                    }
                    //if (m_tabpageOutput.comboBoxInputOutputRejectTray.Items.Contains("Reject Tray 1"))
                    //{ 
                    //    m_tabpageOutput.comboBoxInputOutputRejectTray.Items.Remove("Reject Tray 1");
                    //}
                    //if (m_tabpageOutput.comboBoxS2RejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxS2RejectTray.Items.Remove("Reject Tray 1");
                    //}
                    //if (m_tabpageOutput.comboBoxSetupRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSetupRejectTray.Items.Remove("Reject Tray 1");
                    //}
                    //if (m_tabpageOutput.comboBoxS1RejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxS1RejectTray.Items.Remove("Reject Tray 1");
                    //}
                    //if (m_tabpageOutput.comboBoxSidewallLeftRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSidewallLeftRejectTray.Items.Remove("Reject Tray 1");
                    //}
                    //if (m_tabpageOutput.comboBoxSidewallRightRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSidewallRightRejectTray.Items.Remove("Reject Tray 1");
                    //}
                    //if (m_tabpageOutput.comboBoxSidewallFrontRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSidewallFrontRejectTray.Items.Remove("Reject Tray 1");
                    //}
                    //if (m_tabpageOutput.comboBoxSidewallRearRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSidewallRearRejectTray.Items.Remove("Reject Tray 1");
                    //}
                    //if (m_tabpageOutput.comboBoxS3RejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxS3RejectTray.Items.Remove("Reject Tray 1");
                    //}
                }
                else
                {
                    if (!m_tabpageOutput.comboBoxRejectTray.Items.Contains("1"))
                    {
                        m_tabpageOutput.comboBoxRejectTray.Items.Insert(0,"1");
                    }
                    //if(!m_tabpageOutput.comboBoxInputOutputRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxInputOutputRejectTray.Items.Insert(0,"Reject Tray 1");
                    //}
                    //if (!m_tabpageOutput.comboBoxS2RejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxS2RejectTray.Items.Insert(0, "Reject Tray 1");
                    //}
                    //if (!m_tabpageOutput.comboBoxSetupRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSetupRejectTray.Items.Insert(0, "Reject Tray 1");
                    //}
                    //if (!m_tabpageOutput.comboBoxS1RejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxS1RejectTray.Items.Insert(0, "Reject Tray 1");
                    //}
                    //if (!m_tabpageOutput.comboBoxSidewallLeftRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSidewallLeftRejectTray.Items.Insert(0, "Reject Tray 1");
                    //}
                    //if (!m_tabpageOutput.comboBoxSidewallRightRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSidewallRightRejectTray.Items.Insert(0, "Reject Tray 1");
                    //}
                    //if (!m_tabpageOutput.comboBoxSidewallFrontRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSidewallFrontRejectTray.Items.Insert(0, "Reject Tray 1");
                    //}
                    //if (!m_tabpageOutput.comboBoxSidewallRearRejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxSidewallRearRejectTray.Items.Insert(0, "Reject Tray 1");
                    //}
                    //if (!m_tabpageOutput.comboBoxS3RejectTray.Items.Contains("Reject Tray 1"))
                    //{
                    //    m_tabpageOutput.comboBoxS3RejectTray.Items.Insert(0, "Reject Tray 1");
                    //}
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        
        private void buttonLoadRecipeDelay_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_tabpageDelay.comboBoxRecipeDelay.SelectedIndex != -1)
                {
                    if (LoadDelaySettings())
                    {
                        RefreshDelayRecipeParameter();
                        updateRichTextBoxMessageRecipeDelay("Load recipe done.");
                    }
                    else
                        updateRichTextBoxMessageRecipeDelay("Load recipe fail.");
                }
                else
                {
                    updateRichTextBoxMessageRecipeDelay("Please select recipe before click load.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonApplyAndSaveRecipeDelay_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (m_tabpageDelay.comboBoxRecipeDelay.SelectedIndex != -1)
                        {
                            if (VerifyDelayRecipe())
                            {
                                if (SaveDelaySettings())
                                {                                    
                                    updateRichTextBoxMessageRecipeDelay("Apply and save recipe succesfully.");

                                    OnSaveDelayRecipeDone();
                                }
                                else
                                    updateRichTextBoxMessageRecipeDelay("Apply and save recipe fail.");
                            }
                        }
                        else
                        {
                            updateRichTextBoxMessageRecipeDelay("Please select recipe before click Apply and Save.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeDelay("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonSaveAsRecipeDelay_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (VerifyDelayRecipe())
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeDelayPath);
                        saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
                        saveFileDialogRecipe.FileName = "";
                        if (directoryInfo.Exists == false)
                            Directory.CreateDirectory(directoryInfo.FullName);
                        if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
                        {
                            if (SaveAsDelaySettings())
                            {                                
                                string[] stringSeparators = new string[] { "\\" };
                                string[] result;
                                string newRecipeName = "";
                                result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
                                foreach (string i in result)
                                {
                                    //i.ToLower();                                
                                    //if (i.Contains(".xml"))
                                    if (i.ToLower().EndsWith(".xml"))
                                    {
                                        //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
                                        newRecipeName = i.Substring(0, i.Length - ".xml".Length);
                                    }
                                }
                                RefreshDelayRecipeList();
                                m_tabpageDelay.comboBoxRecipeDelay.SelectedItem = newRecipeName;
                                updateRichTextBoxMessageRecipeDelay("Save recipe settings to file success.");

                                OnSaveDelayRecipeDone();
                            }
                            else
                                updateRichTextBoxMessageRecipeDelay("Save recipe settings to file fail.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeDelay("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonDeleteRecipeDelay_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string filepath = "";

                        if (m_tabpageDelay.comboBoxRecipeDelay.SelectedItem != null)
                        {
                            filepath = m_strRecipeDelayPath + m_tabpageDelay.comboBoxRecipeDelay.SelectedItem.ToString() + m_strRecipeExtension;
                            if (File.Exists(filepath) == true)
                            {
                                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Delay Recipe Named: {m_tabpageDelay.comboBoxRecipeDelay.SelectedItem.ToString()}.");
                                File.Delete(filepath);
                                RefreshDelayRecipeList();
                                updateRichTextBoxMessageRecipeDelay("Recipe deleted.");
                            }
                            else
                            {
                                updateRichTextBoxMessageRecipeDelay("Recipe to be deleted not exists.");
                            }
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeDelay("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void richTextBoxMessageRecipeDelay_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpageDelay.richTextBoxMessageRecipeDelay);
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeDelay(ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void listViewDelay_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                selectedItem = m_tabpageDelay.listViewDelay.SelectedItems;
                if (selectedItem.Count != 0)
                {
                    foreach (ListViewItem item in selectedItem)
                    {
                        m_textboxEditListViewDelay.Bounds = item.SubItems[1].Bounds;
                        m_textboxEditListViewDelay.Text = item.SubItems[1].Text;
                        m_textboxEditListViewDelay.Focus();
                        m_textboxEditListViewDelay.Show();                        
                    }
                }
                else
                    updateRichTextBoxMessageRecipeDelay("Please select delay item.");
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void textboxEditListViewDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (selectedItem != null)
                {
                    if (selectedItem.Count != 0)
                    {
                        foreach (ListViewItem item in selectedItem)
                        {
                            item.SubItems[1].Text = m_textboxEditListViewDelay.Text;
                        }
                        m_textboxEditListViewDelay.Hide();
                    }
                }
            }
        }

        private void comboBoxRecipeDelay_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                {
                    if (m_tabpageDelay.comboBoxRecipeDelay.SelectedIndex != -1)
                    {
                        if (LoadDelaySettings() == false)
                        {
                            //m_ProductRecipeDelaySettings = Tools.Deserialize<ProductRecipeDelaySettings>(m_strRecipeDelayPath + m_tabpageDelay.comboBoxRecipeDelay.SelectedItem.ToString() + m_strRecipeExtension);
                            updateRichTextBoxMessageRecipeDelay("Fail to load recipe.");
                        }
                        else
                            RefreshDelayRecipeParameter();                        
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeDelay("Please select recipe before click load.");
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textboxEditListViewDelay_LostFocus(object sender, EventArgs e)
        {
            try
            {
                if (selectedItem.Count != 0)
                {
                    foreach (ListViewItem item in selectedItem)
                    {
                        item.SubItems[1].Text = m_textboxEditListViewDelay.Text;
                    }
                    m_textboxEditListViewDelay.Hide();
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        
        private void textboxEditListViewDelay_ScrollLostFocus(object sender, EventArgs e)
        {
            try
            {
                if (selectedItem.Count != 0)
                {
                    foreach (ListViewItem item in selectedItem)
                    {
                        item.SubItems[1].Text = m_textboxEditListViewDelay.Text;
                    }
                    m_textboxEditListViewDelay.Hide();
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void textboxEditListViewDelay_SelectedItemChanged(object sender, EventArgs e)
        {
            try
            {
                int nCurrentSelectedDelayItem = -1;
                if (selectedItem.Count != 0)
                {
                    foreach (ListViewItem item in selectedItem)
                    {
                        //item.SubItems[1].Text = m_textboxEditListViewDelay.Text;
                        break;
                    }
                    //selectedItem.IndexOf(m_tabpageDelay.listViewDelay.);
                    nCurrentSelectedDelayItem++;
                }
                if (m_nLastSelectedDelayItem != nCurrentSelectedDelayItem)
                    m_textboxEditListViewDelay.Hide();
                m_nLastSelectedDelayItem = nCurrentSelectedDelayItem;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        
        private void textboxEditListViewMotorPosition_SelectedItemChanged(object sender, EventArgs e)
        {
            try
            {
                int nCurrentSelectedMotorPositionItem = -1;
                if (selectedItem.Count != 0)
                {
                    foreach (ListViewItem item in selectedItem)
                    {
                        //item.SubItems[1].Text = m_textboxEditListViewDelay.Text;
                        break;
                    }
                    //selectedItem.IndexOf(m_tabpageDelay.listViewDelay.);
                    nCurrentSelectedMotorPositionItem++;
                }
                if (m_nLastSelectedDelayItem != nCurrentSelectedMotorPositionItem)
                    m_textboxEditListViewMotorPosition.Hide();
                m_nLastSelectedDelayItem = nCurrentSelectedMotorPositionItem;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void buttonLoadRecipeMotorPosition_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedIndex != -1)
                {
                    if (LoadMotorPositionSettings())
                    {                        
                        RefreshMotorPositionRecipeParameter();
                        updateRichTextBoxMessageRecipeMotorPosition("Load recipe done.");
                    }
                    else
                        updateRichTextBoxMessageRecipeMotorPosition("Load recipe fail.");
                }
                else
                {
                    updateRichTextBoxMessageRecipeMotorPosition("Please select recipe before click load.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonApplyAndSaveRecipeMotorPosition_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedIndex != -1)
                        {
                            if (VerifyMotorPositionRecipe())
                            {
                                if (SaveMotorPositionSettings())
                                {                                    
                                    updateRichTextBoxMessageRecipeMotorPosition("Apply and save recipe succesfully.");

                                    OnSaveMotorPositionRecipeDone();
                                }
                                else
                                    updateRichTextBoxMessageRecipeMotorPosition("Apply and save recipe fail.");
                            }
                        }
                        else
                        {
                            updateRichTextBoxMessageRecipeMotorPosition("Please select recipe before click Apply and Save.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMotorPosition("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonSaveAsRecipeMotorPosition_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (VerifyMotorPositionRecipe())
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeMotorPositionPath);
                        saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
                        saveFileDialogRecipe.FileName = "";
                        if (directoryInfo.Exists == false)
                            Directory.CreateDirectory(directoryInfo.FullName);
                        if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
                        {
                            if (SaveAsMotorPositionSettings())
                            {                                
                                string[] stringSeparators = new string[] { "\\" };
                                string[] result;
                                string newRecipeName = "";
                                result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
                                foreach (string i in result)
                                {
                                    //i.ToLower();                                
                                    //if (i.Contains(".xml"))
                                    if (i.ToLower().EndsWith(".xml"))
                                    {
                                        //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
                                        newRecipeName = i.Substring(0, i.Length - ".xml".Length);
                                    }
                                }
                                RefreshMotorPositionRecipeList();
                                m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedItem = newRecipeName;
                                updateRichTextBoxMessageRecipeMotorPosition("Save recipe settings to file success.");

                                OnSaveMotorPositionRecipeDone();
                            }
                            else
                                updateRichTextBoxMessageRecipeMotorPosition("Save recipe settings to file fail.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMotorPosition("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonDeleteRecipeMotorPosition_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string filepath = "";

                        if (m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedItem != null)
                        {
                            filepath = m_strRecipeMotorPositionPath + m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedItem.ToString() + m_strRecipeExtension;
                            if (File.Exists(filepath) == true)
                            {
                                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Motor Position Recipe Named: {m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedItem.ToString()}.");
                                File.Delete(filepath);
                                RefreshMotorPositionRecipeList();
                                updateRichTextBoxMessageRecipeMotorPosition("Recipe deleted.");
                            }
                            else
                            {
                                updateRichTextBoxMessageRecipeMotorPosition("Recipe to be deleted not exists.");
                            }
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMotorPosition("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void richTextBoxMessageRecipeMotorPosition_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpageMotorPosition.richTextBoxMessageRecipeMotorPosition);
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeMotorPosition(ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void listViewMotorPosition_Click(object sender, EventArgs e)
        {
            try
            {
                selectedItem = m_tabpageMotorPosition.listViewMotorPosition.SelectedItems;
                if (selectedItem.Count != 0)
                {
                    foreach (ListViewItem item in selectedItem)
                    {
                        m_textboxEditListViewMotorPosition.Bounds = item.SubItems[1].Bounds;
                        m_textboxEditListViewMotorPosition.Text = item.SubItems[1].Text;
                        m_textboxEditListViewMotorPosition.Focus();
                        m_textboxEditListViewMotorPosition.Show();
                    }
                }
                else
                    updateRichTextBoxMessageRecipeMotorPosition("Please select Motor Position item.");
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void textboxEditListViewMotorPosition_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (selectedItem != null)
                {
                    if (selectedItem.Count != 0)
                    {
                        foreach (ListViewItem item in selectedItem)
                        {
                            item.SubItems[1].Text = m_textboxEditListViewMotorPosition.Text;
                        }
                        m_textboxEditListViewMotorPosition.Hide();
                    }
                }

            }
        }

        private void comboBoxRecipeMotorPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                {
                    if (m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedIndex != -1)
                    {
                        if(LoadMotorPositionSettings() == false)
                        {
                            updateRichTextBoxMessageRecipeMotorPosition("Fail to load recipe.");
                        }
                        else
                            RefreshMotorPositionRecipeParameter();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMotorPosition("Please select recipe before click load.");
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textboxEditListViewMotorPosition_LostFocus(object sender, EventArgs e)
        {
            try
            {
                if (selectedItem.Count != 0)
                {
                    foreach (ListViewItem item in selectedItem)
                    {
                        item.SubItems[1].Text = m_textboxEditListViewMotorPosition.Text;
                    }
                    m_textboxEditListViewMotorPosition.Hide();
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        
        private void textboxEditListViewMotorPosition_ScrollLostFocus(object sender, EventArgs e)
        {
            try
            {
                if (selectedItem.Count != 0)
                {
                    foreach (ListViewItem item in selectedItem)
                    {
                        item.SubItems[1].Text = m_textboxEditListViewMotorPosition.Text;
                    }
                    m_textboxEditListViewMotorPosition.Hide();
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonLoadRecipeOutputFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedIndex != -1)
                {
                    if (LoadOutputFileSettings())
                    {
                        RefreshOutputFileRecipeParameter();
                        updateRichTextBoxMessageRecipeOutputFile("Load recipe done.");
                    }
                    else
                        updateRichTextBoxMessageRecipeOutputFile("Load recipe fail.");
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutputFile("Please select recipe before click load.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonApplyAndSaveRecipeOutputFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedIndex != -1)
                        {
                            if (VerifyOutputFileRecipe())
                            {
                                if (SaveOutputFileSettings())
                                {                                    
                                    updateRichTextBoxMessageRecipeOutputFile("Apply and save recipe succesfully.");

                                    OnSaveOutputFileRecipeDone();
                                }
                                else
                                    updateRichTextBoxMessageRecipeOutputFile("Apply and save recipe fail.");
                            }
                        }
                        else
                        {
                            updateRichTextBoxMessageRecipeOutputFile("Please select recipe before click Apply and Save.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutputFile("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonSaveAsRecipeOutputFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (VerifyOutputFileRecipe())
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeOutputFilePath);
                        saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
                        saveFileDialogRecipe.FileName = "";
                        if (directoryInfo.Exists == false)
                            Directory.CreateDirectory(directoryInfo.FullName);
                        if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
                        {
                            if (SaveAsOutputFileSettings())
                            {                                
                                string[] stringSeparators = new string[] { "\\" };
                                string[] result;
                                string newRecipeName = "";
                                result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
                                foreach (string i in result)
                                {
                                    //i.ToLower();                                
                                    //if (i.Contains(".xml"))
                                    if (i.ToLower().EndsWith(".xml"))
                                    {
                                        //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
                                        newRecipeName = i.Substring(0, i.Length - ".xml".Length);
                                    }
                                }
                                RefreshOutputFileRecipeList();
                                m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedItem = newRecipeName;
                                updateRichTextBoxMessageRecipeOutputFile("Save recipe settings to file success.");

                                OnSaveOutputFileRecipeDone();
                            }
                            else
                                updateRichTextBoxMessageRecipeOutputFile("Save recipe settings to file fail.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutputFile("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void buttonDeleteRecipeOutputFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string filepath = "";

                        if (m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedItem != null)
                        {
                            filepath = m_strRecipeOutputFilePath + m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedItem.ToString() + m_strRecipeExtension;
                            if (File.Exists(filepath) == true)
                            {
                                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Output Recipe Named: {m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedItem.ToString()}.");
                                File.Delete(filepath);
                                RefreshOutputFileRecipeList();
                                updateRichTextBoxMessageRecipeOutputFile("Recipe deleted.");
                            }
                            else
                            {
                                updateRichTextBoxMessageRecipeOutputFile("Recipe to be deleted not exists.");
                            }
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutputFile("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void richTextBoxRecipeOutputFile_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpageOutputFile.richTextBoxMessageRecipeOutputFile);
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeOutputFile(ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void comboBoxRecipeOutputFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                {
                    if (m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedIndex != -1)
                    {
                        if (LoadOutputFileSettings() == false)
                        {
                            updateRichTextBoxMessageRecipeOutputFile("Fail to load recipe.");
                        }
                        else
                            RefreshOutputFileRecipeParameter();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeOutputFile("Please select recipe before click load.");
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        //private void richTextBoxMessageRecipeSorting_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpageSortingRecipe.richTextBoxMessageRecipeSorting);
        //    }
        //    catch (Exception ex)
        //    {
        //        updateRichTextBoxMessageRecipeSorting(ex.ToString());
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void comboBoxRecipeSorting_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
        //        {
        //            if (m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedIndex != -1)
        //            {
        //                if (LoadSortingSettings() == false)
        //                {
        //                    updateRichTextBoxMessageRecipeSorting("Fail to load recipe.");
        //                }
        //                else
        //                    RefreshSortingRecipeParameter();
        //            }
        //            else
        //            {
        //                updateRichTextBoxMessageRecipeSorting("Please select recipe before click load.");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        private void richTextBoxMessageRecipePickUpHead_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpagePickUpHeadRecipe.richTextBoxMessageRecipePickUpHead);
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipePickUpHead(ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void comboBoxRecipePickUpHead_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                {
                    if (m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedIndex != -1)
                    {
                        if (LoadPickUpHeadSettings() == false)
                        {
                            updateRichTextBoxMessageRecipePickUpHead("Fail to load recipe.");
                        }
                        else
                            RefreshPickUpHeadRecipeParameter();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipePickUpHead("Please select recipe before click load.");
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        //private void comboBoxSortingBarcode_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductRecipeSortingSetting.lstSortings.Count > 0)
        //        {
        //            //m_tabpageSortingRecipe.comboBoxSortingBarcode.Items.Clear();
        //            //for (int i = 0; i < m_ProductRecipeSortingSetting.lstSortings.Count; i++)
        //            //{
        //            //    m_tabpageSortingRecipe.comboBoxSortingBarcode.Items.Add(m_ProductRecipeSortingSetting.lstSortings[i].strSortingBarcode);
        //            //}
        //            m_tabpageSortingRecipe.comboBoxSortingBarcode.Text = m_tabpageSortingRecipe.comboBoxSortingBarcode.Items[m_tabpageSortingRecipe.comboBoxSortingBarcode.SelectedIndex].ToString();
        //            m_tabpageSortingRecipe.labelNoOfWords.Text = m_ProductRecipeSortingSetting.lstSortings[m_tabpageSortingRecipe.comboBoxSortingBarcode.SelectedIndex].intBarcodeLength.ToString();
        //            m_tabpageSortingRecipe.numericUpDownCharacterNo.Value = m_ProductRecipeSortingSetting.lstSortings[m_tabpageSortingRecipe.comboBoxSortingBarcode.SelectedIndex].intBarcodeLength;
        //            if (m_ProductRecipeSortingSetting.lstSortings[m_tabpageSortingRecipe.comboBoxSortingBarcode.SelectedIndex].SortDataCorrect.Count > 0)
        //            {
        //                m_tabpageSortingRecipe.flowLayoutPanel1.Controls.Clear();
        //                foreach (var item in m_ProductRecipeSortingSetting.lstSortings[m_tabpageSortingRecipe.comboBoxSortingBarcode.SelectedIndex].SortDataCorrect)
        //                {
        //                    //var ItemContol = AddUsercontrol(item.CleanCount, item.WarningCount, item.DueCount, item.CurrentCount, item.ItemName, item.ItemLocation);
        //                    var ItemContol = AddUsercontrol(item.strName, item.intWordStartNo, item.intWordEndNo, item.intSelectType, item.strRangeStart, item.strRangeEnd);
        //                    m_tabpageSortingRecipe.flowLayoutPanel1.Controls.Add(ItemContol);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonLoadRecipeInputCassette_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_tabpageInputCassette.comboBoxRecipeCassette.SelectedIndex != -1)
        //        {
        //            if (LoadInputCassetteSettings())
        //            {                        
        //                RefreshInputCassetteRecipeParameter();
        //                updateRichTextBoxMessageRecipeInputCassette("Load recipe done.");
        //            }
        //            else
        //                updateRichTextBoxMessageRecipeInputCassette("Load recipe fail.");

        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeInputCassette("Please select recipe before click load.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonApplyAndSaveRecipeInputCassette_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
        //            || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
        //        {
        //            if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
        //               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        //            {
        //                if (m_tabpageInputCassette.comboBoxRecipeCassette.SelectedIndex != -1)
        //                {
        //                    if (VerifyInputCassetteRecipe())
        //                    {
        //                        if (SaveInputCassetteSettings())
        //                        {                                    
        //                            updateRichTextBoxMessageRecipeInputCassette("Apply and save recipe succesfully.");
        //                        }
        //                        else
        //                            updateRichTextBoxMessageRecipeInputCassette("Apply and save recipe fail.");
        //                    }
        //                }
        //                else
        //                {
        //                    updateRichTextBoxMessageRecipeInputCassette("Please select recipe before click Apply and Save.");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeInputCassette("Only work if system in Idle or Setup mode.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonSaveAsRecipeInputCassette_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
        //            || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
        //        {
        //            if (VerifyInputCassetteRecipe())
        //            {
        //                DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeInputCassettePath);
        //                saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
        //                saveFileDialogRecipe.FileName = "";
        //                if (directoryInfo.Exists == false)
        //                    Directory.CreateDirectory(directoryInfo.FullName);
        //                if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
        //                {
        //                    if (SaveAsInputCassetteSettings())
        //                    {                                
        //                        string[] stringSeparators = new string[] { "\\" };
        //                        string[] result;
        //                        string newRecipeName = "";
        //                        result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
        //                        foreach (string i in result)
        //                        {
        //                            //i.ToLower();                                
        //                            //if (i.Contains(".xml"))
        //                            if (i.ToLower().EndsWith(".xml"))
        //                            {
        //                                //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
        //                                newRecipeName = i.Substring(0, i.Length - ".xml".Length);
        //                            }
        //                        }
        //                        RefreshInputCassetteRecipeList();
        //                        m_tabpageInputCassette.comboBoxRecipeCassette.SelectedItem = newRecipeName;
        //                        updateRichTextBoxMessageRecipeInputCassette("Save recipe settings to file success.");
        //                    }
        //                    else
        //                        updateRichTextBoxMessageRecipeInputCassette("Save recipe settings to file fail.");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeInputCassette("Only work if system in Idle or Setup mode.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonDeleteRecipeInputCassette_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
        //            || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
        //        {
        //            if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
        //               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        //            {
        //                string filepath = "";

        //                if (m_tabpageInputCassette.comboBoxRecipeCassette.SelectedItem != null)
        //                {
        //                    filepath = m_strRecipeInputCassettePath + m_tabpageInputCassette.comboBoxRecipeCassette.SelectedItem.ToString() + m_strRecipeExtension;
        //                    if (File.Exists(filepath) == true)
        //                    {
        //                        Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Input Cassette Recipe Named: {m_tabpageInputCassette.comboBoxRecipeCassette.SelectedItem.ToString()}.");
        //                        File.Delete(filepath);
        //                        RefreshInputCassetteRecipeList();
        //                        updateRichTextBoxMessageRecipeInputCassette("Recipe deleted.");
        //                    }
        //                    else
        //                    {
        //                        updateRichTextBoxMessageRecipeInputCassette("Recipe to be deleted not exists.");
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeInputCassette("Only work if system in Idle or Setup mode.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void richTextBoxMessageRecipeInputCassette_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpageInputCassette.richTextBoxMessageRecipeCassette);
        //    }
        //    catch (Exception ex)
        //    {
        //        updateRichTextBoxMessageRecipeInputCassette(ex.ToString());
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void comboBoxRecipeInputCassette_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
        //        {
        //            if (m_tabpageInputCassette.comboBoxRecipeCassette.SelectedIndex != -1)
        //            {
        //                if (LoadInputCassetteSettings() == false)
        //                {
        //                    updateRichTextBoxMessageRecipeInputCassette("Fail to load recipe.");
        //                }
        //                else
        //                    RefreshInputCassetteRecipeParameter();
        //            }
        //            else
        //            {
        //                updateRichTextBoxMessageRecipeInputCassette("Please select recipe before click load.");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonLoadRecipeOutputCassette_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedIndex != -1)
        //        {
        //            if (LoadOutputCassetteSettings())
        //            {
        //                RefreshOutputCassetteRecipeParameter();
        //                updateRichTextBoxMessageRecipeOutputCassette("Load recipe done.");
        //            }
        //            else
        //                updateRichTextBoxMessageRecipeOutputCassette("Load recipe fail.");

        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeOutputCassette("Please select recipe before click load.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonApplyAndSaveRecipeOutputCassette_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
        //            || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
        //        {
        //            if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
        //               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        //            {
        //                if (m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedIndex != -1)
        //                {
        //                    if (VerifyOutputCassetteRecipe())
        //                    {
        //                        if (SaveOutputCassetteSettings())
        //                        {
        //                            updateRichTextBoxMessageRecipeOutputCassette("Apply and save recipe succesfully.");
        //                        }
        //                        else
        //                            updateRichTextBoxMessageRecipeOutputCassette("Apply and save recipe fail.");
        //                    }
        //                }
        //                else
        //                {
        //                    updateRichTextBoxMessageRecipeOutputCassette("Please select recipe before click Apply and Save.");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeOutputCassette("Only work if system in Idle or Setup mode.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonSaveAsRecipeOutputCassette_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
        //            || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
        //        {
        //            if (VerifyOutputCassetteRecipe())
        //            {
        //                DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeOutputCassettePath);
        //                saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
        //                saveFileDialogRecipe.FileName = "";
        //                if (directoryInfo.Exists == false)
        //                    Directory.CreateDirectory(directoryInfo.FullName);
        //                if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
        //                {
        //                    if (SaveAsOutputCassetteSettings())
        //                    {
        //                        string[] stringSeparators = new string[] { "\\" };
        //                        string[] result;
        //                        string newRecipeName = "";
        //                        result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
        //                        foreach (string i in result)
        //                        {
        //                            //i.ToLower();                                
        //                            //if (i.Contains(".xml"))
        //                            if (i.ToLower().EndsWith(".xml"))
        //                            {
        //                                //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
        //                                newRecipeName = i.Substring(0, i.Length - ".xml".Length);
        //                            }
        //                        }
        //                        RefreshOutputCassetteRecipeList();
        //                        m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedItem = newRecipeName;
        //                        updateRichTextBoxMessageRecipeOutputCassette("Save recipe settings to file success.");
        //                    }
        //                    else
        //                        updateRichTextBoxMessageRecipeOutputCassette("Save recipe settings to file fail.");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeOutputCassette("Only work if system in Idle or Setup mode.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonDeleteRecipeOutputCassette_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
        //            || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
        //        {
        //            if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
        //               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        //            {
        //                string filepath = "";

        //                if (m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedItem != null)
        //                {
        //                    filepath = m_strRecipeOutputCassettePath + m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedItem.ToString() + m_strRecipeExtension;
        //                    if (File.Exists(filepath) == true)
        //                    {
        //                        Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Output Cassette Recipe Named: {m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedItem.ToString()}.");
        //                        File.Delete(filepath);
        //                        RefreshOutputCassetteRecipeList();
        //                        updateRichTextBoxMessageRecipeOutputCassette("Recipe deleted.");
        //                    }
        //                    else
        //                    {
        //                        updateRichTextBoxMessageRecipeOutputCassette("Recipe to be deleted not exists.");
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeOutputCassette("Only work if system in Idle or Setup mode.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void richTextBoxMessageRecipeOutputCassette_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpageOutputCassette.richTextBoxMessageRecipeCassette);
        //    }
        //    catch (Exception ex)
        //    {
        //        updateRichTextBoxMessageRecipeInputCassette(ex.ToString());
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void comboBoxRecipeOutputCassette_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
        //        {
        //            if (m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedIndex != -1)
        //            {
        //                if(LoadOutputCassetteSettings() == false)
        //                {
        //                    updateRichTextBoxMessageRecipeOutputCassette("Fail to load recipe.");
        //                }
        //                else
        //                    RefreshOutputCassetteRecipeParameter();
        //            }
        //            else
        //            {
        //                updateRichTextBoxMessageRecipeOutputCassette("Please select recipe before click load.");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        private void buttonLoadRecipeVision_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedIndex != -1)
                {
                    if (LoadVisionSettings())
                    {                        
                        RefreshVisionRecipeParameter();
                        updateRichTextBoxMessageRecipeVision("Load recipe done.");
                    }
                    else
                        updateRichTextBoxMessageRecipeVision("Load recipe fail.");
                }
                else
                {
                    updateRichTextBoxMessageRecipeVision("Please select recipe before click load.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonApplyAndSaveRecipeVision_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedIndex != -1)
                        {
                            if (VerifyVisionRecipe())
                            {
                                if (SaveVisionSettings())
                                {                                    
                                    updateRichTextBoxMessageRecipeVision("Apply and save recipe succesfully.");
                                }
                                else
                                    updateRichTextBoxMessageRecipeVision("Apply and save recipe fail.");
                            }
                        }
                        else
                        {
                            updateRichTextBoxMessageRecipeVision("Please select recipe before click Apply and Save.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeVision("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonSaveAsRecipeVision_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (VerifyVisionRecipe())
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(m_strRecipeVisionPath);
                        saveFileDialogRecipe.InitialDirectory = directoryInfo.FullName;
                        saveFileDialogRecipe.FileName = "";
                        if (directoryInfo.Exists == false)
                            Directory.CreateDirectory(directoryInfo.FullName);
                        if (saveFileDialogRecipe.ShowDialog() == DialogResult.OK)
                        {
                            if (SaveAsVisionSettings())
                            {                                
                                string[] stringSeparators = new string[] { "\\" };
                                string[] result;
                                string newRecipeName = "";
                                result = saveFileDialogRecipe.FileName.Split(stringSeparators, StringSplitOptions.None);
                                foreach (string i in result)
                                {
                                    //i.ToLower();                                
                                    //if (i.Contains(".xml"))
                                    if (i.ToLower().EndsWith(".xml"))
                                    {
                                        //newRecipeName = i.TrimEnd(m_strRecipeExtension.ToCharArray());
                                        newRecipeName = i.Substring(0, i.Length - ".xml".Length);
                                    }
                                }
                                RefreshVisionRecipeList();
                                m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedItem = newRecipeName;
                                updateRichTextBoxMessageRecipeVision("Save recipe settings to file success.");
                            }
                            else
                                updateRichTextBoxMessageRecipeVision("Save recipe settings to file fail.");
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeVision("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonDeleteRecipeVision_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Idle
                    || m_ProductShareVariables.CurrentSystemState == Machine.Global.SystemState.Setup)
                {
                    if (MessageBox.Show("Are you sure you want to Delete recipe?", "Confirm Delete",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string filepath = "";

                        if (m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedItem != null)
                        {
                            filepath = m_strRecipeVisionPath + m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedItem.ToString() + m_strRecipeExtension;
                            if (File.Exists(filepath) == true)
                            {
                                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User: {m_ProductShareVariables.strLoginUserName}, deleted Vision Recipe Named: {m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedItem.ToString()}.");
                                File.Delete(filepath);
                                RefreshVisionRecipeList();
                                updateRichTextBoxMessageRecipeVision("Recipe deleted.");
                            }
                            else
                            {
                                updateRichTextBoxMessageRecipeVision("Recipe to be deleted not exists.");
                            }
                        }
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeVision("Only work if system in Idle or Setup mode.");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void richTextBoxMessageRecipeVision_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabpageVisionRecipe.richTextBoxMessageRecipeVision);
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeVision(ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void comboBoxRecipeVision_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                {
                    if (m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedIndex != -1)
                    {
                        if(LoadVisionSettings() == false)
                        {
                            updateRichTextBoxMessageRecipeVision("Fail to load recipe.");
                        }
                        else
                            RefreshVisionRecipeParameter();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeVision("Please select recipe before click load.");
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        //private void buttonBrowseInputFilePath_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DialogResult result = folderBrowserDialog1.ShowDialog();

        //        if (result == DialogResult.OK)
        //        {
        //            m_tabpageOutputFile.textBoxInputFilePath.Text = folderBrowserDialog1.SelectedPath;
        //            //txtVisFileFolder.Text = InvisFileFolderName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void buttonBrowseSortingFilePath_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        OpenFileDialog result = new OpenFileDialog();

        //        if (result.ShowDialog() == DialogResult.OK)
        //        {
        //            m_tabpageSortingRecipe.textBoxSortingFilePath.Text = result.FileName;
        //            //txtVisFileFolder.Text = InvisFileFolderName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        private void buttonBrowseOutputFile_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = folderBrowserDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    m_tabpageOutputFile.textBoxOutputFilePath.Text = folderBrowserDialog1.SelectedPath;
                    //txtVisFileFolder.Text = InvisFileFolderName;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void buttonBrowseOutputLocalFile_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = folderBrowserDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    m_tabpageOutputFile.textBoxOutputLocalFilePath.Text = folderBrowserDialog1.SelectedPath;
                    //txtVisFileFolder.Text = InvisFileFolderName;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void buttonServerSummaryFilePath_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = folderBrowserDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    //m_tabpageOutputFile.textBoxServerSummaryFilePath.Text = folderBrowserDialog1.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        //private void radioButtonByRecipe_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (m_tabpageSortingRecipe.radioButtonByRecipe.Checked == true)
        //    {
        //        m_tabpageSortingRecipe.groupBoxSortingByInputFile.Visible = false;
        //        m_tabpageSortingRecipe.groupBoxSortingByRecipe.Visible = true;
        //    }
        //}

        //private void radioButtonByInputFile_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (m_tabpageSortingRecipe.radioButtonByInputFile.Checked == true)
        //    {
        //        m_tabpageSortingRecipe.groupBoxSortingByInputFile.Visible = true;
        //        m_tabpageSortingRecipe.groupBoxSortingByRecipe.Visible = false;
        //    }
        //}

        //private void buttonLoadSortingBarcode_Click(object sender, EventArgs e)
        //{
        //    int length = 0;
        //    m_ProductRecipeSortingSetting_ListInfo.lstInfoSortings.Clear();
        //    length = m_tabpageSortingRecipe.comboBoxSortingBarcode.Text.Length;
        //    m_tabpageSortingRecipe.labelNoOfWords.Text = $"{length.ToString()}";
        //    string last = "";
        //    string text = "";
        //    bool start = false;
        //    if (m_tabpageSortingRecipe.numericUpDownCharacterNo.Value != length)
        //    {
        //        MessageBox.Show("No Of Character & Filled Charactor Length Not Match");
        //        return;
        //    }
        //    for (int i = 0; i < length; i++)
        //    {
        //        if (m_tabpageSortingRecipe.comboBoxSortingBarcode.Text[i].ToString() != "*")
        //        {
        //            if (m_tabpageSortingRecipe.comboBoxSortingBarcode.Text[i].ToString() == last)
        //            {
        //                text += m_tabpageSortingRecipe.comboBoxSortingBarcode.Text[i].ToString();
        //            }
        //            else
        //            {
        //                if (start == true)
        //                {
        //                    m_ProductRecipeSortingSetting_ListInfo.lstInfoSortings.Add(new listInfo { StartNo = i - (text.Length - 1), EndNo = i, words = text });
        //                    text = "";
        //                }
        //                text = m_tabpageSortingRecipe.comboBoxSortingBarcode.Text[i].ToString();
        //            }
        //            last = m_tabpageSortingRecipe.comboBoxSortingBarcode.Text[i].ToString();
        //            start = true;
        //            if (i == m_tabpageSortingRecipe.comboBoxSortingBarcode.Text.Length - 1)
        //            {
        //                m_ProductRecipeSortingSetting_ListInfo.lstInfoSortings.Add(new listInfo { StartNo = (i + 1) - (text.Length - 1), EndNo = i + 1, words = text });
        //                text = "";
        //            }
        //        }
        //        else if(m_tabpageSortingRecipe.comboBoxSortingBarcode.Text[i].ToString() == "*" && text != "")
        //        {
        //            m_ProductRecipeSortingSetting_ListInfo.lstInfoSortings.Add(new listInfo { StartNo = (i) - (text.Length - 1), EndNo = i, words = text });
        //            text = "";
        //            last = "";
        //            start = false;
        //        }
        //    }
        //    UpdateMaintananceCountItemInterface();
        //}

        //private void buttonRemoveSortingBarcode_Click(object sender, EventArgs e)
        //{
        //    for (int i = 0; i < m_ProductRecipeSortingSetting.lstSortings.Count; i++)
        //    {
        //        if (m_tabpageSortingRecipe.comboBoxSortingBarcode.Text == m_ProductRecipeSortingSetting.lstSortings[i].strSortingBarcode)
        //        {
        //            m_ProductRecipeSortingSetting.lstSortings.RemoveAt(i);
        //            break;
        //        }
        //    }
        //    m_tabpageSortingRecipe.comboBoxSortingBarcode.Items.Clear();
        //    if (m_ProductRecipeSortingSetting.lstSortings.Count > 0)
        //    {
        //        for (int i = 0; i < m_ProductRecipeSortingSetting.lstSortings.Count; i++)
        //        {
        //            m_tabpageSortingRecipe.comboBoxSortingBarcode.Items.Add(m_ProductRecipeSortingSetting.lstSortings[i].strSortingBarcode);
        //        }
        //        m_tabpageSortingRecipe.comboBoxSortingBarcode.Text = m_tabpageSortingRecipe.comboBoxSortingBarcode.Items[0].ToString();
        //        m_tabpageSortingRecipe.labelNoOfWords.Text = m_ProductRecipeSortingSetting.lstSortings[0].intBarcodeLength.ToString();
        //        if (m_ProductRecipeSortingSetting.lstSortings[0].SortDataCorrect.Count > 0)
        //        {
        //            m_tabpageSortingRecipe.flowLayoutPanel1.Controls.Clear();
        //            foreach (var item in m_ProductRecipeSortingSetting.lstSortings[0].SortDataCorrect)
        //            {
        //                //var ItemContol = AddUsercontrol(item.CleanCount, item.WarningCount, item.DueCount, item.CurrentCount, item.ItemName, item.ItemLocation);
        //                var ItemContol = AddUsercontrol(item.strName, item.intWordStartNo, item.intWordEndNo, item.intSelectType, item.strRangeStart, item.strRangeEnd);
        //                m_tabpageSortingRecipe.flowLayoutPanel1.Controls.Add(ItemContol);
        //            }
        //        }
        //    }
        //}

        //private void buttonAddSortingBarcode_Click(object sender, EventArgs e)
        //{
        //    m_tabpageSortingRecipe.comboBoxSortingBarcode.Items.Add(m_tabpageSortingRecipe.comboBoxSortingBarcode.Text);
        //    AddRecipe();
        //}

        //void AddRecipe()
        //{
        //    int j = m_ProductRecipeSortingSetting.lstSortings.Count;
        //    m_tabpageSortingRecipe.comboBoxSortingBarcode.SelectedIndex = j;
        //    m_ProductRecipeSortingSetting.lstSortings.Add(new Sorting());
        //    m_ProductRecipeSortingSetting.lstSortings[j].strSortingBarcode = m_tabpageSortingRecipe.comboBoxSortingBarcode.Text;
        //    m_ProductRecipeSortingSetting.lstSortings[j].intBarcodeLength = Convert.ToInt16(m_tabpageSortingRecipe.labelNoOfWords.Text);
        //    m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect = new List<SortData>();
        //    int i = 0;
        //    foreach (SortingSubPage ControlItem in m_tabpageSortingRecipe.flowLayoutPanel1.Controls)
        //    {
        //        m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect.Add(new SortData());
        //        m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].strName = ControlItem.Name;
        //        m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].intSelectType = ControlItem.SelectType;
        //        m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].intWordStartNo = ControlItem.StartNo;
        //        m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].intWordEndNo = ControlItem.EndNo;
        //        m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].strRangeEnd = ControlItem.RangeEnd;
        //        m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].strRangeStart = ControlItem.RangeStart;
        //        i++;
        //    }
        //}

        //void SaveRecipe()
        //{
        //    int j = m_tabpageSortingRecipe.comboBoxSortingBarcode.SelectedIndex;
        //    if (j < 0)
        //    {
        //        m_ProductRecipeSortingSetting.lstSortings.Clear();
        //    }
        //    else
        //    {
        //        m_ProductRecipeSortingSetting.lstSortings[j].strSortingBarcode = m_tabpageSortingRecipe.comboBoxSortingBarcode.Text;
        //        m_ProductRecipeSortingSetting.lstSortings[j].intBarcodeLength = Convert.ToInt16(m_tabpageSortingRecipe.labelNoOfWords.Text);
        //        m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect = new List<SortData>();
        //        int i = 0;
        //        foreach (SortingSubPage ControlItem in m_tabpageSortingRecipe.flowLayoutPanel1.Controls)
        //        {
        //            m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect.Add(new SortData());
        //            m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].strName = ControlItem.Name;
        //            m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].intSelectType = ControlItem.SelectType;
        //            m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].intWordStartNo = ControlItem.StartNo;
        //            m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].intWordEndNo = ControlItem.EndNo;
        //            m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].strRangeEnd = ControlItem.RangeEnd;
        //            m_ProductRecipeSortingSetting.lstSortings[j].SortDataCorrect[i].strRangeStart = ControlItem.RangeStart;
        //            i++;
        //        }
        //    }
        //    //m_clsSorting.lstSortings.Add(new clsSorting.Sorting());
        //}

        //void UpdateMaintananceCountItemInterface()
        //{
        //    m_tabpageSortingRecipe.flowLayoutPanel1.Controls.Clear();
        //    foreach (var item in m_ProductRecipeSortingSetting_ListInfo.lstInfoSortings)
        //    {
        //        var ItemContol = AddUsercontrol(item.words, item.StartNo, item.EndNo, 0, "", "");
        //        m_tabpageSortingRecipe.flowLayoutPanel1.Controls.Add(ItemContol);
        //    }
        //}
        //SortingSubPage AddUsercontrol(string Index, int startNo, int EndNo, int SelectType, string RangeStart, string RangeEnd)
        //{
        //    var Itemname = new SortingSubPage();
        //    Itemname.Name = Index;
        //    Itemname.StartNo = startNo;
        //    Itemname.EndNo = EndNo;
        //    Itemname.SelectType = SelectType;
        //    Itemname.RangeStart = RangeStart;
        //    Itemname.RangeEnd = RangeEnd;
        //    Itemname.updateInterface();
        //    return Itemname;
        //}

        public void updateRichTextBoxMessageRecipeMain(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(groupboxMainRecipeControl.richTextBoxMessageRecipeMain, message);
        }

        public void updateRichTextBoxMessageRecipePickUpHead(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpagePickUpHeadRecipe.richTextBoxMessageRecipePickUpHead, message);
        }

        public void updateRichTextBoxMessageRecipeInput(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpageInput.richTextBoxMessageRecipeInput, message);
        }
        //public void updateRichTextBoxMessageRecipeSorting(string message)
        //{
        //    Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpageSortingRecipe.richTextBoxMessageRecipeSorting, message);
        //}

        public void updateRichTextBoxMessageRecipeOutput(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpageOutput.richTextBoxMessageRecipeOutput, message);
        }

        public void updateRichTextBoxMessageRecipeMotorPosition(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpageMotorPosition.richTextBoxMessageRecipeMotorPosition, message);
        }

        public void updateRichTextBoxMessageRecipeDelay(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpageDelay.richTextBoxMessageRecipeDelay, message);
        }

        //public void updateRichTextBoxMessageRecipeInputCassette(string message)
        //{
        //    Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpageInputCassette.richTextBoxMessageRecipeCassette, message);
        //}

        //public void updateRichTextBoxMessageRecipeOutputCassette(string message)
        //{
        //    Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpageOutputCassette.richTextBoxMessageRecipeCassette, message);
        //}

        public void updateRichTextBoxMessageRecipeVision(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpageVisionRecipe.richTextBoxMessageRecipeVision, message);
        }

        public void updateRichTextBoxMessageRecipeOutputFile(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(m_tabpageOutputFile.richTextBoxMessageRecipeOutputFile, message);
        }

        private void comboBoxRecipeMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                {
                    if (groupboxMainRecipeControl.comboBoxRecipeMain.SelectedIndex != -1)
                    {
                        if(LoadMainSettings() == false)
                        {
                            updateRichTextBoxMessageRecipeMain("Fail to load recipe.");
                        }
                        else
                            RefreshMainRecipeParameter();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Please select recipe before click load.");
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonServelPnPOutputPath_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = folderBrowserDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    //m_tabpageOutputFile.textBoxPnPLabelOutputFilePath.Text = folderBrowserDialog1.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonPnpAdditionPath_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = folderBrowserDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    //m_tabpageOutputFile.textBoxPnpAdditionalFile.Text = folderBrowserDialog1.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }


        //private void checkBoxVisionCameraBarcode_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (m_tabpageOutputFile.checkBoxVisionCameraBarcode.Checked == true)
        //    {
        //        m_tabpageOutputFile.panelZPositionLoadUnloadPosition.Visible = true;
        //    }
        //    else
        //    {
        //        panelZPositionLoadUnloadPosition.Visible = false;
        //    }
        //}
        private void ButtonAddInputSnap_Click(object sender, EventArgs e)
        {
            try
            {
                int nNo = -1;

                if (int.TryParse(m_tabpageVisionRecipe.numericUpDownInputSnapNo.Text,out nNo) == false)
                {
                    updateRichTextBoxMessageRecipeVision("Invalid Input Snap number.");
                    return;
                }
                if (nNo <= 0)
                {
                    updateRichTextBoxMessageRecipeVision("Input Snap number must be greater than 0.");
                    return;
                }
                if (m_tabpageVisionRecipe.textBoxInputSnapDescription.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Input Snap description.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownInputSnapXOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Input Snap X Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownInputSnapYOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Input Snap Y Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownInputSnapZOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Input Snap Z Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownInputSnapThetaOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Input Snap Theta Offset.");
                    return;
                }

                foreach (ListViewItem item in m_tabpageVisionRecipe.listViewInputSnap.Items)
                {
                    if (item.SubItems[0].Text == m_tabpageVisionRecipe.numericUpDownInputSnapNo.Value.ToString())
                    {
                        updateRichTextBoxMessageRecipeVision("Snap number already exist.");
                        return;
                    }
                    if (item.SubItems[1].Text == m_tabpageVisionRecipe.textBoxInputSnapDescription.Text)
                    {
                        updateRichTextBoxMessageRecipeVision("Input Snap description already exist.");
                        return;
                    }
                }
                string strEnableSnap = "";
                if (m_tabpageVisionRecipe.checkBoxEnableInputSnap.Checked == true)
                    strEnableSnap = "Yes";
                else
                    strEnableSnap = "No";

                string[] strArrayItem = new string[7] { nNo.ToString(), m_tabpageVisionRecipe.textBoxInputSnapDescription.Text, strEnableSnap,
                    m_tabpageVisionRecipe.numericUpDownInputSnapXOffset.Value.ToString(),m_tabpageVisionRecipe.numericUpDownInputSnapYOffset.Value.ToString(),
                    m_tabpageVisionRecipe.numericUpDownInputSnapZOffset.Value.ToString(), m_tabpageVisionRecipe.numericUpDownInputSnapThetaOffset.Value.ToString() };
                ListViewItem listViewItem = new ListViewItem(strArrayItem);
                m_tabpageVisionRecipe.listViewInputSnap.Items.Add(listViewItem);
                m_tabpageVisionRecipe.listViewInputSnap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalInputSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewInputSnap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ButtonRemoveInputSnap_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem _listViewItem in m_tabpageVisionRecipe.listViewInputSnap.SelectedItems)
                {
                    m_tabpageVisionRecipe.listViewInputSnap.Items.Remove(_listViewItem);
                }
                m_tabpageVisionRecipe.labelTotalInputSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewInputSnap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ListViewInputSnap_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItem = m_tabpageVisionRecipe.listViewInputSnap.SelectedItems;
                foreach (ListViewItem item in selectedItem)
                {
                    m_tabpageVisionRecipe.numericUpDownInputSnapNo.Value = int.Parse(item.SubItems[0].Text);
                    m_tabpageVisionRecipe.textBoxInputSnapDescription.Text = item.SubItems[1].Text;
                    if (item.SubItems[2].Text.ToUpper() == "YES")
                        m_tabpageVisionRecipe.checkBoxEnableInputSnap.Checked = true;
                    else
                        m_tabpageVisionRecipe.checkBoxEnableInputSnap.Checked = false;

                    m_tabpageVisionRecipe.numericUpDownInputSnapXOffset.Value = int.Parse(item.SubItems[3].Text);
                    m_tabpageVisionRecipe.numericUpDownInputSnapYOffset.Value = int.Parse(item.SubItems[4].Text);
                    m_tabpageVisionRecipe.numericUpDownInputSnapZOffset.Value = int.Parse(item.SubItems[5].Text);
                    m_tabpageVisionRecipe.numericUpDownInputSnapThetaOffset.Value = int.Parse(item.SubItems[6].Text);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

       
        private void ButtonAddS1Snap_Click(object sender, EventArgs e)
        {
            try
            {
                int nNo = -1;

                if (int.TryParse(m_tabpageVisionRecipe.numericUpDownS1SnapNo.Text, out nNo) == false)
                {
                    updateRichTextBoxMessageRecipeVision("Invalid S1 Snap number.");
                    return;
                }
                if (nNo <= 0)
                {
                    updateRichTextBoxMessageRecipeVision("S1 Snap number must be greater than 0.");
                    return;
                }
                if (m_tabpageVisionRecipe.textBoxS1SnapDescription.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S1 Snap description.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS1SnapXOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S1 Snap X Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS1SnapYOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S1 Snap Y Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS1SnapZOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S1 Snap Z Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS1SnapThetaOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S1 Snap Theta Offset.");
                    return;
                }

                foreach (ListViewItem item in m_tabpageVisionRecipe.listViewS1Snap.Items)
                {
                    if (item.SubItems[0].Text == m_tabpageVisionRecipe.numericUpDownS1SnapNo.Value.ToString())
                    {
                        updateRichTextBoxMessageRecipeVision("Snap number already exist.");
                        return;
                    }
                    if (item.SubItems[1].Text == m_tabpageVisionRecipe.textBoxS1SnapDescription.Text)
                    {
                        updateRichTextBoxMessageRecipeVision("S1 Snap description already exist.");
                        return;
                    }
                }
                string strEnableSnap = "";
                if (m_tabpageVisionRecipe.checkBoxEnableS1Snap.Checked == true)
                    strEnableSnap = "Yes";
                else
                    strEnableSnap = "No";

                string[] strArrayItem = new string[7] { nNo.ToString(), m_tabpageVisionRecipe.textBoxS1SnapDescription.Text, strEnableSnap,
                    m_tabpageVisionRecipe.numericUpDownS1SnapXOffset.Value.ToString(),m_tabpageVisionRecipe.numericUpDownS1SnapYOffset.Value.ToString(),
                    m_tabpageVisionRecipe.numericUpDownS1SnapZOffset.Value.ToString(), m_tabpageVisionRecipe.numericUpDownS1SnapThetaOffset.Value.ToString() };
                ListViewItem listViewItem = new ListViewItem(strArrayItem);
                m_tabpageVisionRecipe.listViewS1Snap.Items.Add(listViewItem);
                m_tabpageVisionRecipe.listViewS1Snap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalS1SnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewS1Snap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ButtonRemoveS1Snap_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem _listViewItem in m_tabpageVisionRecipe.listViewS1Snap.SelectedItems)
                {
                    m_tabpageVisionRecipe.listViewS1Snap.Items.Remove(_listViewItem);
                }
                m_tabpageVisionRecipe.labelTotalS1SnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewS1Snap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ListViewS1Snap_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItem = m_tabpageVisionRecipe.listViewS1Snap.SelectedItems;
                foreach (ListViewItem item in selectedItem)
                {
                    m_tabpageVisionRecipe.numericUpDownS1SnapNo.Value = int.Parse(item.SubItems[0].Text);
                    m_tabpageVisionRecipe.textBoxS1SnapDescription.Text = item.SubItems[1].Text;
                    if (item.SubItems[2].Text.ToUpper() == "YES")
                        m_tabpageVisionRecipe.checkBoxEnableS1Snap.Checked = true;
                    else
                        m_tabpageVisionRecipe.checkBoxEnableS1Snap.Checked = false;

                    m_tabpageVisionRecipe.numericUpDownS1SnapXOffset.Value = int.Parse(item.SubItems[3].Text);
                    m_tabpageVisionRecipe.numericUpDownS1SnapYOffset.Value = int.Parse(item.SubItems[4].Text);
                    m_tabpageVisionRecipe.numericUpDownS1SnapZOffset.Value = int.Parse(item.SubItems[5].Text);
                    m_tabpageVisionRecipe.numericUpDownS1SnapThetaOffset.Value = int.Parse(item.SubItems[6].Text);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void ButtonAddS2Snap_Click(object sender, EventArgs e)
        {
            try
            {
                int nNo = -1;

                if (int.TryParse(m_tabpageVisionRecipe.numericUpDownS2SnapNo.Text, out nNo) == false)
                {
                    updateRichTextBoxMessageRecipeVision("Invalid S2 Snap number.");
                    return;
                }
                if (nNo <= 0)
                {
                    updateRichTextBoxMessageRecipeVision("S2 Snap number must be greater than 0.");
                    return;
                }
                if (m_tabpageVisionRecipe.textBoxS2SnapDescription.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2 Snap description.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS2SnapXOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2 Snap X Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS2SnapYOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2 Snap Y Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS2SnapZOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2 Snap Z Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS2SnapThetaOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2 Snap Theta Offset.");
                    return;
                }

                foreach (ListViewItem item in m_tabpageVisionRecipe.listViewS2Snap.Items)
                {
                    if (item.SubItems[0].Text == m_tabpageVisionRecipe.numericUpDownS2SnapNo.Value.ToString())
                    {
                        updateRichTextBoxMessageRecipeVision("S2 Snap number already exist.");
                        return;
                    }
                    if (item.SubItems[1].Text == m_tabpageVisionRecipe.textBoxS2SnapDescription.Text)
                    {
                        updateRichTextBoxMessageRecipeVision("S2 Snap description already exist.");
                        return;
                    }
                }
                string strEnableSnap = "";
                if (m_tabpageVisionRecipe.checkBoxEnableS2Snap.Checked == true)
                    strEnableSnap = "Yes";
                else
                    strEnableSnap = "No";

                string[] strArrayItem = new string[7] { nNo.ToString(), m_tabpageVisionRecipe.textBoxS2SnapDescription.Text, strEnableSnap,
                    m_tabpageVisionRecipe.numericUpDownS2SnapXOffset.Value.ToString(),m_tabpageVisionRecipe.numericUpDownS2SnapYOffset.Value.ToString(),
                    m_tabpageVisionRecipe.numericUpDownS2SnapZOffset.Value.ToString(), m_tabpageVisionRecipe.numericUpDownS2SnapThetaOffset.Value.ToString() };
                ListViewItem listViewItem = new ListViewItem(strArrayItem);
                m_tabpageVisionRecipe.listViewS2Snap.Items.Add(listViewItem);
                m_tabpageVisionRecipe.listViewS2Snap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalS2SnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewS2Snap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ButtonRemoveS2Snap_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem _listViewItem in m_tabpageVisionRecipe.listViewS2Snap.SelectedItems)
                {
                    m_tabpageVisionRecipe.listViewS2Snap.Items.Remove(_listViewItem);
                }
                m_tabpageVisionRecipe.labelTotalS2SnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewS2Snap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void ListViewS2Snap_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItem = m_tabpageVisionRecipe.listViewS2Snap.SelectedItems;
                foreach (ListViewItem item in selectedItem)
                {
                    m_tabpageVisionRecipe.numericUpDownS2SnapNo.Value = int.Parse(item.SubItems[0].Text);
                    m_tabpageVisionRecipe.textBoxS2SnapDescription.Text = item.SubItems[1].Text;
                    if (item.SubItems[2].Text.ToUpper() == "YES")
                        m_tabpageVisionRecipe.checkBoxEnableS2Snap.Checked = true;
                    else
                        m_tabpageVisionRecipe.checkBoxEnableS2Snap.Checked = false;

                    m_tabpageVisionRecipe.numericUpDownS2SnapXOffset.Value = int.Parse(item.SubItems[3].Text);
                    m_tabpageVisionRecipe.numericUpDownS2SnapYOffset.Value = int.Parse(item.SubItems[4].Text);
                    m_tabpageVisionRecipe.numericUpDownS2SnapZOffset.Value = int.Parse(item.SubItems[5].Text);
                    m_tabpageVisionRecipe.numericUpDownS2SnapThetaOffset.Value = int.Parse(item.SubItems[6].Text);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void ButtonAddS2FacetSnap_Click(object sender, EventArgs e)
        {
            try
            {
                int nNo = -1;

                if (int.TryParse(m_tabpageVisionRecipe.numericUpDownS2FacetSnapNo.Text, out nNo) == false)
                {
                    updateRichTextBoxMessageRecipeVision("Invalid S2Facet Snap number.");
                    return;
                }
                if (nNo <= 0)
                {
                    updateRichTextBoxMessageRecipeVision("S2Facet Snap number must be greater than 0.");
                    return;
                }
                if (m_tabpageVisionRecipe.textBoxS2FacetSnapDescription.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2Facet Snap description.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS2FacetSnapXOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2Facet Snap X Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS2FacetSnapYOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2Facet Snap Y Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS2FacetSnapZOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2Facet Snap Z Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownS2FacetSnapThetaOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in S2Facet Snap Theta Offset.");
                    return;
                }

                foreach (ListViewItem item in m_tabpageVisionRecipe.listViewS2FacetSnap.Items)
                {
                    if (item.SubItems[0].Text == m_tabpageVisionRecipe.numericUpDownS2FacetSnapNo.Value.ToString())
                    {
                        updateRichTextBoxMessageRecipeVision("Snap number already exist.");
                        return;
                    }
                    if (item.SubItems[1].Text == m_tabpageVisionRecipe.textBoxS2FacetSnapDescription.Text)
                    {
                        updateRichTextBoxMessageRecipeVision("S2Facet Snap description already exist.");
                        return;
                    }
                }
                string strEnableSnap = "";
                if (m_tabpageVisionRecipe.checkBoxEnableS2FacetSnap.Checked == true)
                    strEnableSnap = "Yes";
                else
                    strEnableSnap = "No";

                string[] strArrayItem = new string[7] { nNo.ToString(), m_tabpageVisionRecipe.textBoxS2FacetSnapDescription.Text, strEnableSnap,
                    m_tabpageVisionRecipe.numericUpDownS2FacetSnapXOffset.Value.ToString(),m_tabpageVisionRecipe.numericUpDownS2FacetSnapYOffset.Value.ToString(),
                    m_tabpageVisionRecipe.numericUpDownS2FacetSnapZOffset.Value.ToString(), m_tabpageVisionRecipe.numericUpDownS2FacetSnapThetaOffset.Value.ToString() };
                ListViewItem listViewItem = new ListViewItem(strArrayItem);
                m_tabpageVisionRecipe.listViewS2FacetSnap.Items.Add(listViewItem);
                m_tabpageVisionRecipe.listViewS2FacetSnap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalS2FacetSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewS2FacetSnap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ButtonRemoveS2FacetSnap_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem _listViewItem in m_tabpageVisionRecipe.listViewS2FacetSnap.SelectedItems)
                {
                    m_tabpageVisionRecipe.listViewS2FacetSnap.Items.Remove(_listViewItem);
                }
                m_tabpageVisionRecipe.labelTotalS2FacetSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewS2FacetSnap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ListViewS2FacetSnap_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItem = m_tabpageVisionRecipe.listViewS2FacetSnap.SelectedItems;
                foreach (ListViewItem item in selectedItem)
                {
                    m_tabpageVisionRecipe.numericUpDownS2FacetSnapNo.Value = int.Parse(item.SubItems[0].Text);
                    m_tabpageVisionRecipe.textBoxS2FacetSnapDescription.Text = item.SubItems[1].Text;
                    if (item.SubItems[2].Text.ToUpper() == "YES")
                        m_tabpageVisionRecipe.checkBoxEnableS2FacetSnap.Checked = true;
                    else
                        m_tabpageVisionRecipe.checkBoxEnableS2FacetSnap.Checked = false;

                    m_tabpageVisionRecipe.numericUpDownS2FacetSnapXOffset.Value = int.Parse(item.SubItems[3].Text);
                    m_tabpageVisionRecipe.numericUpDownS2FacetSnapYOffset.Value = int.Parse(item.SubItems[4].Text);
                    m_tabpageVisionRecipe.numericUpDownS2FacetSnapZOffset.Value = int.Parse(item.SubItems[5].Text);
                    m_tabpageVisionRecipe.numericUpDownS2FacetSnapThetaOffset.Value = int.Parse(item.SubItems[6].Text);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }


        private void ButtonAddBottomSnap_Click(object sender, EventArgs e)
        {
            try
            {
                int nNo = -1;

                if (int.TryParse(m_tabpageVisionRecipe.numericUpDownBottomSnapNo.Text, out nNo) == false)
                {
                    updateRichTextBoxMessageRecipeVision("Invalid Bottom Snap number.");
                    return;
                }
                if (nNo <= 0)
                {
                    updateRichTextBoxMessageRecipeVision("Bottom Snap number must be greater than 0.");
                    return;
                }
                if (m_tabpageVisionRecipe.textBoxBottomSnapDescription.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Bottom Snap description.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownBottomSnapXOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Bottom Snap X Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownBottomSnapYOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Bottom Snap Y Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownBottomSnapZOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Bottom Snap Z Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownBottomSnapThetaOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Bottom Snap Theta Offset.");
                    return;
                }

                foreach (ListViewItem item in m_tabpageVisionRecipe.listViewBottomSnap.Items)
                {
                    if (item.SubItems[0].Text == m_tabpageVisionRecipe.numericUpDownBottomSnapNo.Value.ToString())
                    {
                        updateRichTextBoxMessageRecipeVision("Bottom Snap number already exist.");
                        return;
                    }
                    if (item.SubItems[1].Text == m_tabpageVisionRecipe.textBoxBottomSnapDescription.Text
                        )
                    {
                        updateRichTextBoxMessageRecipeVision("Bottom Snap description already exist.");
                        return;
                    }
                }
                string strEnableSnap = "";
                if (m_tabpageVisionRecipe.checkBoxEnableBottomSnap.Checked == true)
                    strEnableSnap = "Yes";
                else
                    strEnableSnap = "No";
                string strEnableDiffuser = "";
                if (m_tabpageVisionRecipe.checkBoxEnableDiffuserActuator.Checked == true)
                    strEnableDiffuser = "Yes";
                else
                    strEnableDiffuser = "No";
                string[] strArrayItem = new string[8] { nNo.ToString(), m_tabpageVisionRecipe.textBoxBottomSnapDescription.Text, strEnableSnap,
                    m_tabpageVisionRecipe.numericUpDownBottomSnapXOffset.Value.ToString(),m_tabpageVisionRecipe.numericUpDownBottomSnapYOffset.Value.ToString(),
                    m_tabpageVisionRecipe.numericUpDownBottomSnapZOffset.Value.ToString(), m_tabpageVisionRecipe.numericUpDownBottomSnapThetaOffset.Value.ToString(),strEnableDiffuser };
                ListViewItem listViewItem = new ListViewItem(strArrayItem);
                m_tabpageVisionRecipe.listViewBottomSnap.Items.Add(listViewItem);
                m_tabpageVisionRecipe.listViewBottomSnap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalBottomSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewBottomSnap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ButtonRemoveBottomSnap_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem _listViewItem in m_tabpageVisionRecipe.listViewBottomSnap.SelectedItems)
                {
                    m_tabpageVisionRecipe.listViewBottomSnap.Items.Remove(_listViewItem);
                }
                m_tabpageVisionRecipe.labelTotalBottomSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewBottomSnap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ListViewBottomSnap_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItem = m_tabpageVisionRecipe.listViewBottomSnap.SelectedItems;
                foreach (ListViewItem item in selectedItem)
                {
                    m_tabpageVisionRecipe.numericUpDownBottomSnapNo.Value = int.Parse(item.SubItems[0].Text);
                    m_tabpageVisionRecipe.textBoxBottomSnapDescription.Text = item.SubItems[1].Text;
                    if (item.SubItems[2].Text.ToUpper() == "YES")
                        m_tabpageVisionRecipe.checkBoxEnableBottomSnap.Checked = true;
                    else
                        m_tabpageVisionRecipe.checkBoxEnableBottomSnap.Checked = false;

                    m_tabpageVisionRecipe.numericUpDownBottomSnapXOffset.Value = int.Parse(item.SubItems[3].Text);
                    m_tabpageVisionRecipe.numericUpDownBottomSnapYOffset.Value = int.Parse(item.SubItems[4].Text);
                    m_tabpageVisionRecipe.numericUpDownBottomSnapZOffset.Value = int.Parse(item.SubItems[5].Text);
                    m_tabpageVisionRecipe.numericUpDownBottomSnapThetaOffset.Value = int.Parse(item.SubItems[6].Text);
                    if (item.SubItems[7].Text.ToUpper() == "YES")
                        m_tabpageVisionRecipe.checkBoxEnableDiffuserActuator.Checked = true;
                    else
                        m_tabpageVisionRecipe.checkBoxEnableDiffuserActuator.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }


        #endregion Form Event

        private void ButtonAddOutputSnap_Click(object sender, EventArgs e)
        {
            try
            {
                int nNo = -1;

                if (int.TryParse(m_tabpageVisionRecipe.numericUpDownOutputSnapNo.Text, out nNo) == false)
                {
                    updateRichTextBoxMessageRecipeVision("Invalid Output Snap number.");
                    return;
                }
                if (nNo <= 0)
                {
                    updateRichTextBoxMessageRecipeVision("Output Snap number must be greater than 0.");
                    return;
                }
                if (m_tabpageVisionRecipe.textBoxOutputSnapDescription.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Output Snap description.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownOutputSnapXOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Output Snap X Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownOutputSnapYOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Output Snap Y Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownOutputSnapZOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Output Snap Z Offset.");
                    return;
                }
                if (m_tabpageVisionRecipe.numericUpDownOutputSnapThetaOffset.Text == "")
                {
                    updateRichTextBoxMessageRecipeVision("Please key in Output Snap Theta Offset.");
                    return;
                }

                foreach (ListViewItem item in m_tabpageVisionRecipe.listViewOutputSnap.Items)
                {
                    if (item.SubItems[0].Text == m_tabpageVisionRecipe.numericUpDownOutputSnapNo.Text)
                    {
                        updateRichTextBoxMessageRecipeVision("Output Snap number already exist.");
                        return;
                    }
                    if (item.SubItems[1].Text == m_tabpageVisionRecipe.textBoxOutputSnapDescription.Text)
                    {
                        updateRichTextBoxMessageRecipeVision("Output Snap description already exist.");
                        return;
                    }
                }
                string strEnableSnap = "";
                if (m_tabpageVisionRecipe.checkBoxEnableOutputSnap.Checked == true)
                    strEnableSnap = "Yes";
                else
                    strEnableSnap = "No";

                string[] strArrayItem = new string[7] { nNo.ToString(), m_tabpageVisionRecipe.textBoxOutputSnapDescription.Text, strEnableSnap,
                    m_tabpageVisionRecipe.numericUpDownOutputSnapXOffset.Value.ToString(),m_tabpageVisionRecipe.numericUpDownOutputSnapYOffset.Value.ToString(),
                    m_tabpageVisionRecipe.numericUpDownOutputSnapZOffset.Value.ToString(), m_tabpageVisionRecipe.numericUpDownOutputSnapThetaOffset.Value.ToString() };
                ListViewItem listViewItem = new ListViewItem(strArrayItem);
                m_tabpageVisionRecipe.listViewOutputSnap.Items.Add(listViewItem);
                m_tabpageVisionRecipe.listViewOutputSnap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalOutputSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewOutputSnap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ButtonRemoveOutputSnap_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem _listViewItem in m_tabpageVisionRecipe.listViewOutputSnap.SelectedItems)
                {
                    m_tabpageVisionRecipe.listViewOutputSnap.Items.Remove(_listViewItem);
                }
                m_tabpageVisionRecipe.labelTotalOutputSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewOutputSnap.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void ListViewOutputSnap_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItem = m_tabpageVisionRecipe.listViewOutputSnap.SelectedItems;
                foreach (ListViewItem item in selectedItem)
                {
                    m_tabpageVisionRecipe.numericUpDownOutputSnapNo.Value = int.Parse(item.SubItems[0].Text);
                    m_tabpageVisionRecipe.textBoxOutputSnapDescription.Text = item.SubItems[1].Text;
                    if (item.SubItems[2].Text.ToUpper() == "YES")
                        m_tabpageVisionRecipe.checkBoxEnableOutputSnap.Checked = true;
                    else
                        m_tabpageVisionRecipe.checkBoxEnableOutputSnap.Checked = false;

                    m_tabpageVisionRecipe.numericUpDownOutputSnapXOffset.Value = int.Parse(item.SubItems[3].Text);
                    m_tabpageVisionRecipe.numericUpDownOutputSnapYOffset.Value = int.Parse(item.SubItems[4].Text);
                    m_tabpageVisionRecipe.numericUpDownOutputSnapZOffset.Value = int.Parse(item.SubItems[5].Text);
                    m_tabpageVisionRecipe.numericUpDownOutputSnapThetaOffset.Value = int.Parse(item.SubItems[6].Text);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void AutoCreateFolderFile()
        {
            if (!Directory.Exists(m_strRecipeMainPath))
                Directory.CreateDirectory(m_strRecipeMainPath);
            if (!Directory.Exists(m_strRecipeInputPath))
                Directory.CreateDirectory(m_strRecipeInputPath);
            if (!Directory.Exists(m_strRecipeOutputPath))
                Directory.CreateDirectory(m_strRecipeOutputPath);
            if (!Directory.Exists(m_strRecipeDelayPath))
                Directory.CreateDirectory(m_strRecipeDelayPath);
            if (!Directory.Exists(m_strRecipeMotorPositionPath))
                Directory.CreateDirectory(m_strRecipeMotorPositionPath);
            if (!Directory.Exists(m_strRecipeOutputFilePath))
                Directory.CreateDirectory(m_strRecipeOutputFilePath);

            #region Turret  Application
//#if TurretApplication
//            if (!Directory.Exists(m_strRecipeRotaryAndAlignerAPath))
//                Directory.CreateDirectory(m_strRecipeRotaryAndAlignerAPath);
//            if (!Directory.Exists(m_strRecipeRotaryAndAlignerBPath))
//                Directory.CreateDirectory(m_strRecipeRotaryAndAlignerBPath);
//            if (!Directory.Exists(m_strRecipeSortingPath))
//                Directory.CreateDirectory(m_strRecipeSortingPath);
//#endif
            #endregion Turret Application
            #region Input XY Table
#if InputXYTable
            if (!Directory.Exists(m_strRecipeInputCassettePath))
                Directory.CreateDirectory(m_strRecipeInputCassettePath);

            if (!Directory.Exists(m_strRecipeOutputCassettePath))
                Directory.CreateDirectory(m_strRecipeOutputCassettePath);

            if (!Directory.Exists(m_strRecipeVisionPath))
                Directory.CreateDirectory(m_strRecipeVisionPath);

            if (!Directory.Exists(m_strRecipeSortingPath))
                Directory.CreateDirectory(m_strRecipeSortingPath);

            if (!Directory.Exists(m_strRecipePickUpHeadPath))
                Directory.CreateDirectory(m_strRecipePickUpHeadPath);
#endif
            #endregion Input XY Table
        }

        override public int UpdateGUIControl()
        {
            int nError = 0;
            #region Application Code

            #region Group Box
//#if TurretApplication == false
//            //groupBoxMainTurretApplication.Visible = false;
//            m_tabpageInput.groupBoxInputTurretApplication.Visible = false;
//#endif

            #endregion Group Box

            #region ListView           

            #region Listview Motor Position

            m_tabpageMotorPosition.listViewMotorPosition.Groups.Clear();
            m_tabpageMotorPosition.listViewMotorPosition.Items.Clear();
            FieldInfo[] fields = typeof(ProductRecipeMotorPositionSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo _fields in fields)
            {
                m_tabpageMotorPosition.listViewMotorPosition.Items.Add(new ListViewItem(new string[] { GetSeparateWordFromCapitalString(_fields.Name), "0" }, -1));
            }
            #endregion Listview Motor Position

            #region Listview Delay

            m_tabpageDelay.listViewDelay.Groups.Clear();
            m_tabpageDelay.listViewDelay.Items.Clear();
            FieldInfo[] fields2 = typeof(ProductRecipeDelaySettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo _fields in fields2)
            {
                m_tabpageDelay.listViewDelay.Items.Add(new ListViewItem(new string[] { GetSeparateWordFromCapitalString(_fields.Name), "0" }, -1));
            }
            

            #endregion Listview Motor Position
            #endregion Listview
            
            #endregion Application Code
            return nError;
        }

        override public void RefreshAllRecipeList()
        {
            RefreshMainRecipeList();
            RefreshInputRecipeList();
            RefreshOutputRecipeList();
            RefreshDelayRecipeList();
            RefreshMotorPositionRecipeList();
            RefreshOutputFileRecipeList();
            
            //RefreshInputCassetteRecipeList();
           // RefreshOutputCassetteRecipeList();
            RefreshVisionRecipeList();
            RefreshInspectionRecipeList();
            //RefreshSortingRecipeList();
            RefreshPickUpHeadRecipeList();
            if (m_ProductRTSSProcess.GetEvent("JobMode") == true)
            {
                if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                {
                    if (groupboxMainRecipeControl.comboBoxRecipeMain.Items.Contains(m_ProductShareVariables.currentMainRecipeName))
                    {
                        groupboxMainRecipeControl.comboBoxRecipeMain.SelectedItem = m_ProductShareVariables.currentMainRecipeName;
                        if (LoadMainSettings(m_ProductShareVariables.currentMainRecipeName) == false)
                        {
                            updateRichTextBoxMessageRecipeMain("Fail to load recipe.");
                        }
                        else
                            RefreshMainRecipeParameter();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Recipe not exist.");
                    }
                }
            }
        }

        override public void InitializeRecipesSettings()
        {

        }

        override public bool OnClose()
        {
            base.OnClose();
            if(m_ProductRTSSProcess.GetEvent("JobMode") == true)
                m_ProductProcessEvent.GUI_PCS_UpdateSettingShareMemory.Set();
            return true;
        }

        virtual public bool LoadMainSettings(string mainRecipeFilename)
        {
            try
            {

                if (File.Exists(m_strRecipeMainPath + mainRecipeFilename + m_strRecipeExtension))
                {
                    m_ProductRecipeMainSettings = Tools.Deserialize<ProductRecipeMainSettings>(m_strRecipeMainPath + mainRecipeFilename + m_strRecipeExtension);
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool LoadMainSettings()
        {
            try
            {

                if (File.Exists(m_strRecipeMainPath + groupboxMainRecipeControl.comboBoxRecipeMain.SelectedItem.ToString() + m_strRecipeExtension))
                {
                    m_ProductRecipeMainSettings = Tools.Deserialize<ProductRecipeMainSettings>(m_strRecipeMainPath + groupboxMainRecipeControl.comboBoxRecipeMain.SelectedItem.ToString() + m_strRecipeExtension);
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveMainSettings()
        {
            try
            {
                Tools.Serialize(m_strRecipeMainPath + groupboxMainRecipeControl.comboBoxRecipeMain.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeMainSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveAsMainSettings()
        {
            try
            {
                Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeMainSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool LoadInputSettings()
        {
            try
            {

                if (File.Exists(m_strRecipeInputPath + m_tabpageInput.comboBoxRecipeInput.SelectedItem.ToString() + m_strRecipeExtension))
                {
                    m_ProductRecipeInputSettings = Tools.Deserialize<ProductRecipeInputSettings>(m_strRecipeInputPath + m_tabpageInput.comboBoxRecipeInput.SelectedItem.ToString() + m_strRecipeExtension);
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveInputSettings()
        {
            try
            {
                Tools.Serialize(m_strRecipeInputPath + m_tabpageInput.comboBoxRecipeInput.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeInputSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveAsInputSettings()
        {
            try
            {
                Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeInputSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool LoadOutputSettings()
        {
            try
            {

                if (File.Exists(m_strRecipeOutputPath + m_tabpageOutput.comboBoxRecipeOutput.SelectedItem.ToString() + m_strRecipeExtension))
                {
                    m_ProductRecipeOutputSettings = Tools.Deserialize<ProductRecipeOutputSettings>(m_strRecipeOutputPath + m_tabpageOutput.comboBoxRecipeOutput.SelectedItem.ToString() + m_strRecipeExtension);
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveOutputSettings()
        {
            try
            {
                Tools.Serialize(m_strRecipeOutputPath + m_tabpageOutput.comboBoxRecipeOutput.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeOutputSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveAsOutputSettings()
        {
            try
            {
                Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeOutputSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool LoadDelaySettings()
        {
            try
            {

                if (File.Exists(m_strRecipeDelayPath + m_tabpageDelay.comboBoxRecipeDelay.SelectedItem.ToString() + m_strRecipeExtension))
                {
                    m_ProductRecipeDelaySettings = Tools.Deserialize<ProductRecipeDelaySettings>(m_strRecipeDelayPath + m_tabpageDelay.comboBoxRecipeDelay.SelectedItem.ToString() + m_strRecipeExtension);
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveDelaySettings()
        {
            try
            {
                Tools.Serialize(m_strRecipeDelayPath + m_tabpageDelay.comboBoxRecipeDelay.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeDelaySettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveAsDelaySettings()
        {
            try
            {
                Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeDelaySettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool LoadMotorPositionSettings()
        {
            try
            {
                if (File.Exists(m_strRecipeMotorPositionPath + m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedItem.ToString() + m_strRecipeExtension))
                {
                    m_ProductRecipeMotorPositionSettings = Tools.Deserialize<ProductRecipeMotorPositionSettings>(m_strRecipeMotorPositionPath + m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedItem.ToString() + m_strRecipeExtension);
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveMotorPositionSettings()
        {
            try
            {
                Tools.Serialize(m_strRecipeMotorPositionPath + m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeMotorPositionSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveAsMotorPositionSettings()
        {
            try
            {
                Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeMotorPositionSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool LoadOutputFileSettings()
        {
            try
            {
                if (File.Exists(m_strRecipeOutputFilePath + m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedItem.ToString() + m_strRecipeExtension))
                {
                    m_ProductRecipeOutputFileSettings = Tools.Deserialize<ProductRecipeOutputFileSettings>(m_strRecipeOutputFilePath + m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedItem.ToString() + m_strRecipeExtension);
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveOutputFileSettings()
        {
            try
            {
                Tools.Serialize(m_strRecipeOutputFilePath + m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeOutputFileSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveAsOutputFileSettings()
        {
            try
            {
                Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeOutputFileSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        //virtual public bool LoadSortingSettings()
        //{
        //    try
        //    {
        //        if (File.Exists(m_strRecipeSortingPath + m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedItem.ToString() + m_strRecipeExtension))
        //        {
        //            m_ProductRecipeSortingSetting = Tools.Deserialize<ProductRecipeSortingSetting>(m_strRecipeSortingPath + m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedItem.ToString() + m_strRecipeExtension);
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        //virtual public bool SaveSortingSettings()
        //{
        //    try
        //    {
        //        Tools.Serialize(m_strRecipeSortingPath + m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeSortingSetting);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        //virtual public bool SaveAsSortingSettings()
        //{
        //    try
        //    {
        //        Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeSortingSetting);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        virtual public bool LoadPickUpHeadSettings()
        {
            try
            {
                if (File.Exists(m_strRecipePickUpHeadPath + m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedItem.ToString() + m_strRecipeExtension))
                {
                    m_ProductRecipePickUpHeadSetting = Tools.Deserialize<ProductRecipePickUpHeadSeting>(m_strRecipePickUpHeadPath + m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedItem.ToString() + m_strRecipeExtension);
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SavePickUpHeadSettings()
        {
            try
            {
                Tools.Serialize(m_strRecipePickUpHeadPath + m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipePickUpHeadSetting);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveAsPickUpHeadSettings()
        {
            try
            {
                Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipePickUpHeadSetting);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public void RefreshMainRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeMainPath, m_strRecipeExtension);
                if (recipeList != null)
                {
                    groupboxMainRecipeControl.comboBoxRecipeMain.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        groupboxMainRecipeControl.comboBoxRecipeMain.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshInputRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeInputPath, m_strRecipeExtension);
                if (recipeList != null)
                {
                    m_tabpageMainRecipe.comboBoxInput.Items.Clear();
                    m_tabpageInput.comboBoxRecipeInput.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        m_tabpageMainRecipe.comboBoxInput.Items.Add(recipeList[i]);
                        m_tabpageInput.comboBoxRecipeInput.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshOutputRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeOutputPath, m_strRecipeExtension);
                if (recipeList != null)
                {
                    m_tabpageMainRecipe.comboBoxOutput.Items.Clear();
                    m_tabpageOutput.comboBoxRecipeOutput.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        m_tabpageMainRecipe.comboBoxOutput.Items.Add(recipeList[i]);
                        m_tabpageOutput.comboBoxRecipeOutput.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshDelayRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeDelayPath, m_strRecipeExtension);
                if (recipeList != null)
                {
                    m_tabpageMainRecipe.comboBoxDelay.Items.Clear();
                    m_tabpageDelay.comboBoxRecipeDelay.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        m_tabpageMainRecipe.comboBoxDelay.Items.Add(recipeList[i]);
                        m_tabpageDelay.comboBoxRecipeDelay.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshMotorPositionRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeMotorPositionPath, m_strRecipeExtension);
                if (recipeList != null)
                {
                    m_tabpageMainRecipe.comboBoxMotorPosition.Items.Clear();
                    m_tabpageMotorPosition.comboBoxRecipeMotorPosition.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        m_tabpageMainRecipe.comboBoxMotorPosition.Items.Add(recipeList[i]);
                        m_tabpageMotorPosition.comboBoxRecipeMotorPosition.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshOutputFileRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeOutputFilePath, m_strRecipeExtension);
                if (recipeList != null)
                {
                    m_tabpageMainRecipe.comboBoxOutputFile.Items.Clear();
                    m_tabpageOutputFile.comboBoxRecipeOutputFile.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        m_tabpageMainRecipe.comboBoxOutputFile.Items.Add(recipeList[i]);
                        m_tabpageOutputFile.comboBoxRecipeOutputFile.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        //virtual public bool LoadInputCassetteSettings()
        //{
        //    try
        //    {
        //        if (File.Exists(m_strRecipeInputCassettePath + m_tabpageInputCassette.comboBoxRecipeCassette.SelectedItem.ToString() + m_strRecipeExtension))
        //        {
        //            m_ProductRecipeInputCassetteSettings = Tools.Deserialize<ProductRecipeCassetteSettings>(m_strRecipeInputCassettePath + m_tabpageInputCassette.comboBoxRecipeCassette.SelectedItem.ToString() + m_strRecipeExtension);
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        //virtual public bool SaveInputCassetteSettings()
        //{
        //    try
        //    {
        //        Tools.Serialize(m_strRecipeInputCassettePath + m_tabpageInputCassette.comboBoxRecipeCassette.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeInputCassetteSettings);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        //virtual public bool SaveAsInputCassetteSettings()
        //{
        //    try
        //    {
        //        Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeInputCassetteSettings);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        //virtual public bool LoadOutputCassetteSettings()
        //{
        //    try
        //    {
        //        if (File.Exists(m_strRecipeOutputCassettePath + m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedItem.ToString() + m_strRecipeExtension))
        //        {
        //            m_ProductRecipeOutputCassetteSettings = Tools.Deserialize<ProductRecipeCassetteSettings>(m_strRecipeOutputCassettePath + m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedItem.ToString() + m_strRecipeExtension);
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        //virtual public bool SaveOutputCassetteSettings()
        //{
        //    try
        //    {
        //        Tools.Serialize(m_strRecipeOutputCassettePath + m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeOutputCassetteSettings);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        //virtual public bool SaveAsOutputCassetteSettings()
        //{
        //    try
        //    {
        //        Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeOutputCassetteSettings);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        virtual public bool LoadVisionSettings()
        {
            try
            {
                if (File.Exists(m_strRecipeVisionPath + m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedItem.ToString() + m_strRecipeExtension))
                {
                    m_ProductRecipeVisionSettings = Tools.Deserialize<ProductRecipeVisionSettings>(m_strRecipeVisionPath + m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedItem.ToString() + m_strRecipeExtension);
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Recipe file not exist.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveVisionSettings()
        {
            try
            {
                Tools.Serialize(m_strRecipeVisionPath + m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedItem.ToString() + m_strRecipeExtension, m_ProductRecipeVisionSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool SaveAsVisionSettings()
        {
            try
            {
                Tools.Serialize(saveFileDialogRecipe.FileName, m_ProductRecipeVisionSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        //virtual public void RefreshInputCassetteRecipeList()
        //{
        //    try
        //    {
        //        List<string> recipeList;
        //        int length;

        //        recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeInputCassettePath, m_strRecipeExtension);
        //        if (recipeList != null)
        //        {
        //            m_tabpageMainRecipe.comboBoxInputCassette.Items.Clear();
        //            m_tabpageInputCassette.comboBoxRecipeCassette.Items.Clear();
        //            length = recipeList.Count;
        //            for (int i = 0; i != length; i++)
        //            {
        //                m_tabpageMainRecipe.comboBoxInputCassette.Items.Add(recipeList[i]);
        //                m_tabpageInputCassette.comboBoxRecipeCassette.Items.Add(recipeList[i]);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        updateRichTextBoxMessageRecipeMain("Recipe display fail.");
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //virtual public void RefreshOutputCassetteRecipeList()
        //{
        //    try
        //    {
        //        List<string> recipeList;
        //        int length;

        //        recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeOutputCassettePath, m_strRecipeExtension);
        //        if (recipeList != null)
        //        {
        //            m_tabpageMainRecipe.comboBoxOutputCassette.Items.Clear();
        //            m_tabpageOutputCassette.comboBoxRecipeCassette.Items.Clear();
        //            length = recipeList.Count;
        //            for (int i = 0; i != length; i++)
        //            {
        //                m_tabpageMainRecipe.comboBoxOutputCassette.Items.Add(recipeList[i]);
        //                m_tabpageOutputCassette.comboBoxRecipeCassette.Items.Add(recipeList[i]);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        updateRichTextBoxMessageRecipeMain("Recipe display fail.");
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //virtual public void RefreshSortingRecipeList()
        //{
        //    try
        //    {
        //        List<string> recipeList;
        //        int length;

        //        recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeSortingPath, m_strRecipeExtension);
        //        if (recipeList != null)
        //        {
        //            m_tabpageMainRecipe.comboBoxSorting.Items.Clear();
        //            m_tabpageSortingRecipe.comboBoxRecipeSorting.Items.Clear();
        //            length = recipeList.Count;
        //            for (int i = 0; i != length; i++)
        //            {
        //                m_tabpageMainRecipe.comboBoxSorting.Items.Add(recipeList[i]);
        //                m_tabpageSortingRecipe.comboBoxRecipeSorting.Items.Add(recipeList[i]);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        updateRichTextBoxMessageRecipeMain("Recipe display fail.");
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        virtual public void RefreshPickUpHeadRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipePickUpHeadPath, m_strRecipeExtension);
                if (recipeList != null)
                {
                    m_tabpageMainRecipe.comboBoxPickUpHead.Items.Clear();
                    m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        m_tabpageMainRecipe.comboBoxPickUpHead.Items.Add(recipeList[i]);
                        m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeMain("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshVisionRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeVisionPath, m_strRecipeExtension);
                if (recipeList != null)
                {
                    m_tabpageMainRecipe.comboBoxVision.Items.Clear();
                    m_tabpageVisionRecipe.comboBoxRecipeVision.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        m_tabpageMainRecipe.comboBoxVision.Items.Add(recipeList[i]);
                        m_tabpageVisionRecipe.comboBoxRecipeVision.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeMain("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshInspectionRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                if (Directory.Exists(m_ProductShareVariables.productOptionSettings.VisionRecipeFolderPath) == false)
                {
                    updateRichTextBoxMessageRecipeMain("Inspection Recipe folder not exist");
                    return;
                }
                recipeList = Tools.GetFilesNamesFromDirectory(m_ProductShareVariables.productOptionSettings.VisionRecipeFolderPath, m_strRecipeExtension);
                if (recipeList != null)
                {
                    m_tabpageMainRecipe.comboBoxInspectionRecipe.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        m_tabpageMainRecipe.comboBoxInspectionRecipe.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeMain("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshBarcodeRecipeList()
        {
            try
            {
                List<string> recipeList;
                int length;

                //if (Directory.Exists(m_ProductShareVariables.productOptionSettings.VisionRecipeFolderPath) == false)
                //{
                //    updateRichTextBoxMessageRecipeMain("Inspection Recipe folder not exist");
                //    return;
                //}
                recipeList = Tools.GetFilesNamesFromDirectory(m_strRecipeBarcodePath, ".PTC");
                if (recipeList != null)
                {
                    m_tabpageInput.comboBoxBarcodeRecipe.Items.Clear();
                    length = recipeList.Count;
                    for (int i = 0; i != length; i++)
                    {
                        m_tabpageInput.comboBoxBarcodeRecipe.Items.Add(recipeList[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeMain("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }


        virtual public void RefreshMainRecipeParameter()
        {
            try
            {
                if (Tools.IsFileExist(m_strRecipeInputPath, m_ProductRecipeMainSettings.InputRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxInput.Items.Contains(m_ProductRecipeMainSettings.InputRecipeName))
                {
                    m_tabpageMainRecipe.comboBoxInput.SelectedItem = m_ProductRecipeMainSettings.InputRecipeName;
                    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                    {
                        m_tabpageInput.comboBoxRecipeInput.SelectedItem = m_ProductRecipeMainSettings.InputRecipeName;
                    }
                }
                else
                {
                    m_tabpageMainRecipe.comboBoxInput.SelectedIndex = -1;
                    updateRichTextBoxMessageRecipeMain("Input recipe not exist.");
                    return;
                }

                if (Tools.IsFileExist(m_strRecipeOutputPath, m_ProductRecipeMainSettings.OutputRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxOutput.Items.Contains(m_ProductRecipeMainSettings.OutputRecipeName))
                {
                    m_tabpageMainRecipe.comboBoxOutput.SelectedItem = m_ProductRecipeMainSettings.OutputRecipeName;
                    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                    {
                        m_tabpageOutput.comboBoxRecipeOutput.SelectedItem = m_ProductRecipeMainSettings.OutputRecipeName;
                    }
                }
                else
                {
                    m_tabpageMainRecipe.comboBoxOutput.SelectedIndex = -1;
                    updateRichTextBoxMessageRecipeMain("Output recipe not exist.");
                    return;
                }

                if (Tools.IsFileExist(m_strRecipeDelayPath, m_ProductRecipeMainSettings.DelayRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxDelay.Items.Contains(m_ProductRecipeMainSettings.DelayRecipeName))
                {
                    m_tabpageMainRecipe.comboBoxDelay.SelectedItem = m_ProductRecipeMainSettings.DelayRecipeName;
                    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                    {
                        m_tabpageDelay.comboBoxRecipeDelay.SelectedItem = m_ProductRecipeMainSettings.DelayRecipeName;
                    }
                }
                else
                {
                    m_tabpageMainRecipe.comboBoxDelay.SelectedIndex = -1;
                    updateRichTextBoxMessageRecipeMain("Delay recipe not exist.");
                    return;
                }
                if (Tools.IsFileExist(m_strRecipeMotorPositionPath, m_ProductRecipeMainSettings.MotorPositionRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxMotorPosition.Items.Contains(m_ProductRecipeMainSettings.MotorPositionRecipeName))
                {
                    m_tabpageMainRecipe.comboBoxMotorPosition.SelectedItem = m_ProductRecipeMainSettings.MotorPositionRecipeName;
                    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                    {
                        m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedItem = m_ProductRecipeMainSettings.MotorPositionRecipeName;
                    }
                }
                else
                {
                    m_tabpageMainRecipe.comboBoxMotorPosition.SelectedIndex = -1;
                    updateRichTextBoxMessageRecipeMain("Motor Position recipe not exist.");
                    return;
                }

                if (Tools.IsFileExist(m_strRecipeOutputFilePath, m_ProductRecipeMainSettings.OutputFileRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxOutputFile.Items.Contains(m_ProductRecipeMainSettings.OutputFileRecipeName))
                {
                    m_tabpageMainRecipe.comboBoxOutputFile.SelectedItem = m_ProductRecipeMainSettings.OutputFileRecipeName;
                    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                    {
                        m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedItem = m_ProductRecipeMainSettings.OutputFileRecipeName;
                    }
                }
                else
                {
                    m_tabpageMainRecipe.comboBoxOutputFile.SelectedIndex = -1;
                    updateRichTextBoxMessageRecipeMain("Output File recipe not exist.");
                    return;
                }

                #region Input XY Table
                //if (Tools.IsFileExist(m_strRecipeInputCassettePath, m_ProductRecipeMainSettings.InputCassetteRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxInputCassette.Items.Contains(m_ProductRecipeMainSettings.InputCassetteRecipeName))
                //{
                //    m_tabpageMainRecipe.comboBoxInputCassette.SelectedItem = m_ProductRecipeMainSettings.InputCassetteRecipeName;
                //    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                //    {
                //        m_tabpageInputCassette.comboBoxRecipeCassette.SelectedItem = m_ProductRecipeMainSettings.InputCassetteRecipeName;
                //    }
                //}
                //else
                //{
                //    m_tabpageMainRecipe.comboBoxInputCassette.SelectedIndex = -1;
                //    updateRichTextBoxMessageRecipeMain("Input Cassette recipe not exist.");
                //    return;
                //}

                //if (Tools.IsFileExist(m_strRecipeOutputCassettePath, m_ProductRecipeMainSettings.OutputCassetteRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxOutputCassette.Items.Contains(m_ProductRecipeMainSettings.OutputCassetteRecipeName))
                //{
                //    m_tabpageMainRecipe.comboBoxOutputCassette.SelectedItem = m_ProductRecipeMainSettings.OutputCassetteRecipeName;
                //    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                //    {
                //        m_tabpageOutputCassette.comboBoxRecipeCassette.SelectedItem = m_ProductRecipeMainSettings.OutputCassetteRecipeName;
                //    }
                //}
                //else
                //{
                //    m_tabpageMainRecipe.comboBoxOutputCassette.SelectedIndex = -1;
                //    updateRichTextBoxMessageRecipeMain("Output Cassette recipe not exist.");
                //    return;
                //}

                if (Tools.IsFileExist(m_strRecipeVisionPath, m_ProductRecipeMainSettings.VisionRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxVision.Items.Contains(m_ProductRecipeMainSettings.VisionRecipeName))
                {
                    m_tabpageMainRecipe.comboBoxVision.SelectedItem = m_ProductRecipeMainSettings.VisionRecipeName;
                    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                    {
                        m_tabpageVisionRecipe.comboBoxRecipeVision.SelectedItem = m_ProductRecipeMainSettings.VisionRecipeName;
                    }
                }
                else
                {
                    m_tabpageMainRecipe.comboBoxVision.SelectedIndex = -1;
                    updateRichTextBoxMessageRecipeMain("Vision recipe not exist.");
                    return;
                }

                if (Tools.IsFileExist(m_ProductShareVariables.productOptionSettings.VisionRecipeFolderPath, m_ProductRecipeMainSettings.InspectionRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxInspectionRecipe.Items.Contains(m_ProductRecipeMainSettings.InspectionRecipeName))
                {
                    m_tabpageMainRecipe.comboBoxInspectionRecipe.SelectedItem = m_ProductRecipeMainSettings.InspectionRecipeName;
                }
                else
                {
                    m_tabpageMainRecipe.comboBoxInspectionRecipe.SelectedIndex = -1;
                    updateRichTextBoxMessageRecipeMain("Inspection recipe not exist.");
                    return;
                }
                //if (Tools.IsFileExist(m_strRecipeSortingPath, m_ProductRecipeMainSettings.SortingRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxSorting.Items.Contains(m_ProductRecipeMainSettings.SortingRecipeName))
                //{
                //    m_tabpageMainRecipe.comboBoxSorting.SelectedItem = m_ProductRecipeMainSettings.SortingRecipeName;
                //    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                //    {
                //        m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedItem = m_ProductRecipeMainSettings.SortingRecipeName;
                //    }
                //}
                //else
                //{
                //    m_tabpageMainRecipe.comboBoxSorting.SelectedIndex = -1;
                //    updateRichTextBoxMessageRecipeMain("Sorting recipe not exist.");
                //    return;
                //}
                if (Tools.IsFileExist(m_strRecipePickUpHeadPath, m_ProductRecipeMainSettings.PickUpHeadRecipeName, m_strRecipeExtension) && m_tabpageMainRecipe.comboBoxPickUpHead.Items.Contains(m_ProductRecipeMainSettings.PickUpHeadRecipeName))
                {
                    m_tabpageMainRecipe.comboBoxPickUpHead.SelectedItem = m_ProductRecipeMainSettings.PickUpHeadRecipeName;
                    if (m_ProductShareVariables.optionSettings.EnableAutoLoadRecipe == true)
                    {
                        m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedItem = m_ProductRecipeMainSettings.PickUpHeadRecipeName;
                    }
                }
                else
                {
                    m_tabpageMainRecipe.comboBoxPickUpHead.SelectedIndex = -1;
                    updateRichTextBoxMessageRecipeMain("PickUp Head recipe not exist.");
                    return;
                }
                #endregion

                updateRichTextBoxMessageRecipeMain("Recipe loaded successfully.");
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeMain("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshInputRecipeParameter()
        {
            try
            {
                m_tabpageInput.numericUpDownDeviceXPitchInput.Value = (int)m_ProductRecipeInputSettings.DeviceXPitchInput;
                m_tabpageInput.numericUpDownDeviceYPitchInput.Value = (int)m_ProductRecipeInputSettings.DeviceYPitchInput;
                m_tabpageInput.numericUpDownNoOfDeviceInRowInput.Value = (int)m_ProductRecipeInputSettings.NoOfDeviceInRowInput;
                m_tabpageInput.numericUpDownNoOfDeviceInColInput.Value = (int)m_ProductRecipeInputSettings.NoOfDeviceInColInput;
                m_tabpageInput.numericUpDowPocketDepthInput_um.Value = (int)m_ProductRecipeInputSettings.InputPocketDepth_um;
                m_tabpageInput.numericUpDownPickingCenterXOffsetInput.Value = m_ProductRecipeInputSettings.PickingCenterXOffsetInput;
                m_tabpageInput.numericUpDownPickingCenterYOffsetInput.Value = m_ProductRecipeInputSettings.PickingCenterYOffsetInput;
                m_tabpageInput.numericUpDownUnitPlacementRotationOffsetInput.Value = m_ProductRecipeInputSettings.UnitPlacementRotationOffsetInput;
                m_tabpageInput.numericUpDownBottomVisionXOffset.Value = m_ProductRecipeInputSettings.BottomVisionXOffset;
                m_tabpageInput.numericUpDownContinuouslyEmptyPocket.Value = m_ProductRecipeInputSettings.ContinuouslyEmptyPocket;
                
                m_tabpageInput.checkBoxEnableCheckmptyUnit.Checked = (bool)m_ProductRecipeInputSettings.EnableCheckEmptyUnit;
                m_tabpageInput.checkBoxEnableInputTableVacuum.Checked = (bool)m_ProductRecipeInputSettings.EnableInputTableVacuum;
                m_tabpageInput.checkBoxEnablePurging.Checked = (bool)m_ProductRecipeInputSettings.EnablePurging;
                if (m_ProductRecipeInputSettings.EnableCheckEmptyUnit)
                {
                    m_tabpageInput.numericUpDownEmptyUnit.Visible = true;
                    m_tabpageInput.labelEmptyUnit.Visible = true;
                }
                else
                {
                    m_tabpageInput.numericUpDownEmptyUnit.Visible = false;
                    m_tabpageInput.labelEmptyUnit.Visible = false;
                }
                m_tabpageInput.numericUpDownDeviceThickness_um.Value = m_ProductRecipeInputSettings.UnitThickness_um;
                m_tabpageInput.numericUpDownEmptyUnit.Value = (int)m_ProductRecipeInputSettings.EmptyUnit;
                //RefreshBarcodeRecipeList();
                //m_tabpageInput.comboBoxBarcodeRecipe.Text = m_ProductRecipeInputSettings.BarcodeRecipe;

                #region BarcodeReaderKeyence
                //if (m_ProductRecipeInputSettings.BarcodeReaderKeyence_Recipe == 1)
                //{
                //    m_tabpageInput.comboBoxBarcodeReaderKeyence_Recipe.SelectedIndex = 0;
                //}
                //else if (m_ProductRecipeInputSettings.BarcodeReaderKeyence_Recipe == 2)
                //{
                //    m_tabpageInput.comboBoxBarcodeReaderKeyence_Recipe.SelectedIndex = 1;
                //}
                //else if (m_ProductRecipeInputSettings.BarcodeReaderKeyence_Recipe == 3)
                //{
                //    m_tabpageInput.comboBoxBarcodeReaderKeyence_Recipe.SelectedIndex = 2;
                //}
                //else
                //{
                //    m_tabpageInput.comboBoxBarcodeReaderKeyence_Recipe.SelectedIndex = -1;
                //    updateRichTextBoxMessageRecipeInput("Unknown Barcode Reader Keyence Recipe.");
                //    return;
                //}
                #endregion
                updateRichTextBoxMessageRecipeInput("Recipe loaded successfully.");

            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeInput("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshOutputRecipeParameter()
        {
            try
            {
                m_tabpageOutput.numericUpDownDeviceXPitchOutput.Value = m_ProductRecipeOutputSettings.DeviceXPitchOutput;
                m_tabpageOutput.numericUpDownDeviceYPitchOutput.Value = m_ProductRecipeOutputSettings.DeviceYPitchOutput;
                m_tabpageOutput.numericUpDownNoOfDeviceInRowOutput.Value = m_ProductRecipeOutputSettings.NoOfDeviceInRowOutput;
                m_tabpageOutput.numericUpDownNoOfDeviceInColOutput.Value = m_ProductRecipeOutputSettings.NoOfDeviceInColOutput;
                m_tabpageOutput.numericUpDownOutputTrayThickness_um.Value = m_ProductRecipeOutputSettings.OutputTrayThickness;
                m_tabpageOutput.numericUpDownPocketDepthOutput_um.Value = m_ProductRecipeOutputSettings.OutputPocketDepth_um;
                m_tabpageOutput.numericUpDownTableXOffsetOutput.Value = m_ProductRecipeOutputSettings.TableXOffsetOutput;
                m_tabpageOutput.numericUpDownTableYOffsetOutput.Value = m_ProductRecipeOutputSettings.TableYOffsetOutput;
                m_tabpageOutput.numericUpDownUnitPlacementRotationOffsetOutput.Value = m_ProductRecipeOutputSettings.UnitPlacementRotationOffsetOutput;
                m_tabpageOutput.numericUpDownLowYieldAlarm.Value = m_ProductRecipeOutputSettings.LowYieldAlarm;
                m_tabpageOutput.numericUpDownYieldPercentage.Value = (decimal)m_ProductRecipeOutputSettings.YieldPercentage;

                if (!m_tabpageOutput.comboBoxRejectTray.Items.Contains("1"))
                {
                    m_tabpageOutput.comboBoxRejectTray.Items.Insert(0, "1");
                }

                m_tabpageOutput.comboBoxBarcodeLabelSize.SelectedIndex = (int)m_ProductRecipeOutputSettings.BarcodeLabelSize;

                //m_tabpageOutput.checkBoxGoodSampling.Checked = (bool)m_ProductRecipeOutputSettings.EnableGoodSamplingSequence;

                //m_tabpageOutput.checkBoxEnableBarcodeTray.Checked = (bool)m_ProductRecipeOutputSettings.EnableScanBarcodeOnOutputTray;
                m_tabpageOutput.checkBoxCheckContinuousDefectCode.Checked = (bool)m_ProductRecipeOutputSettings.EnableCheckContinuousDefectCode;
                m_tabpageOutput.checkBoxEnableOutputTableVacuum.Checked = (bool)m_ProductRecipeOutputSettings.EnableOutputTableVacuum;
                m_tabpageOutput.numericUpDownCheckContinuousDefectCode.Value = m_ProductRecipeOutputSettings.CheckContinuousDefectCode;
                m_tabpageOutput.listViewSortTrayConfiguration.Items.Clear();
                foreach (SortTrayInfo _SortTrayInfo in m_ProductRecipeOutputSettings.listSortTrayInfo)
                {
                    string[] strArrayItem = new string[2] {_SortTrayInfo.RejectTray.ToString(),_SortTrayInfo.DefectCode};
                    ListViewItem listViewItem = new ListViewItem(strArrayItem);
                    m_tabpageOutput.listViewSortTrayConfiguration.Items.Add(listViewItem);

                }
                m_tabpageOutput.listViewSortTrayConfiguration.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageOutput.labelTotalSortingTrayConfiguration.Text = "Total : " + m_tabpageOutput.listViewSortTrayConfiguration.Items.Count.ToString();
                updateRichTextBoxMessageRecipeOutput("Recipe loaded successfully.");
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeOutput("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshDelayRecipeParameter()
        {
            try
            {
                ListViewItem lvItem;

                FieldInfo[] fields = typeof(ProductRecipeDelaySettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo _fields in fields)
                {
                    lvItem = m_tabpageDelay.listViewDelay.FindItemWithText(GetSeparateWordFromCapitalString(_fields.Name));
                    lvItem.SubItems[1].Text = (_fields.GetValue(m_ProductRecipeDelaySettings)).ToString();
                }

                //lvItem = m_tabpageDelay.listViewDelay.FindItemWithText(m_ProductRecipeDelaySettings.DelayBeforePickupPusherGoingDown_ms.);
                //lvItem.SubItems[1].Text = m_ProductRecipeDelaySettings.DelayBeforePickupPusherGoingDown_ms.ToString();                

                updateRichTextBoxMessageRecipeDelay("Recipe loaded successfully.");
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeDelay("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshMotorPositionRecipeParameter()
        {
            try
            {
                ListViewItem lvItem;

                FieldInfo[] fields = typeof(ProductRecipeMotorPositionSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo _fields in fields)
                {
                    lvItem = m_tabpageMotorPosition.listViewMotorPosition.FindItemWithText(GetSeparateWordFromCapitalString(_fields.Name));
                    lvItem.SubItems[1].Text = (_fields.GetValue(m_ProductRecipeMotorPositionSettings)).ToString();
                }

                //lvItem = m_tabpageMotorPosition.listViewMotorPosition.FindItemWithText("Input Pusher Down Distance From Touching Position");
                //lvItem.SubItems[1].Text = m_RecipeMotorPosition.InputPusherDownDistance.ToString();

                updateRichTextBoxMessageRecipeMotorPosition("Recipe loaded successfully.");
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeMotorPosition("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void RefreshOutputFileRecipeParameter()
        {
            try
            {
                m_tabpageOutputFile.listViewDefect.Items.Clear();
                foreach (DefectProperty _Defect in m_ProductRecipeOutputFileSettings.listDefect)
                {
                    //string strEnableInReport = "No";
                    //if (_Defect.EnableInReport == true)
                    //    strEnableInReport = "Yes";
                    //Color a = ColorTranslator.FromOle(Convert.ToInt32(_Defect.ColorInHex));
                    //Color b = ColorTranslator.FromWin32(Convert.ToInt32(_Defect.ColorInHex));
                    System.Drawing.Color x = System.Drawing.ColorTranslator.FromHtml(_Defect.ColorInHex);
                    System.Drawing.Color a = (System.Drawing.Color)System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Drawing.Color)).ConvertFromString(_Defect.ColorInHex);
                    //string name = a.Name;
                    string[] strArrayItem = new string[5] { _Defect.No.ToString(), _Defect.Code, _Defect.Description, _Defect.Pick, ((System.Drawing.Color)System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Drawing.Color)).ConvertFromString(_Defect.ColorInHex)).Name };//, strEnableInReport, ((System.Drawing.Color)System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Drawing.Color)).ConvertFromString(_Defect.ColorInHex)).Name };
                    ListViewItem listViewItem = new ListViewItem(strArrayItem);
                    m_tabpageOutputFile.listViewDefect.Items.Add(listViewItem);

                }
                m_tabpageOutputFile.listViewDefect.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageOutputFile.labelTotalDefect.Text = "Total : " + m_tabpageOutputFile.listViewDefect.Items.Count.ToString();

                //m_tabpageOutputFile.checkBoxEnableInputFile.Checked = m_ProductRecipeOutputFileSettings.bEnableInputFile;

                //m_tabpageOutputFile.checkBoxEnableGoodMapFile.Checked = m_ProductRecipeOutputFileSettings.EnableGoodMapFile;
                //m_tabpageOutputFile.checkBoxEnableRejectedMapFile.Checked = m_ProductRecipeOutputFileSettings.EnableRejectedMapFile;
                //m_tabpageOutputFile.checkBoxEnableSortingMapFile.Checked = m_ProductRecipeOutputFileSettings.EnableSortingMapFile;
                //m_tabpageOutputFile.checkBoxEnableXMLFIle.Checked = m_ProductRecipeOutputFileSettings.EnableXMLFile;

                //m_tabpageOutputFile.textBoxInputFilePath.Text = m_ProductRecipeOutputFileSettings.strInputFilePath;
                m_tabpageOutputFile.textBoxOutputFilePath.Text = m_ProductRecipeOutputFileSettings.strOutputFilePath;
                m_tabpageOutputFile.textBoxOutputLocalFilePath.Text = m_ProductRecipeOutputFileSettings.strOutputLocalFilePath;
               
                //m_tabpageOutputFile.textBoxServerSummaryFilePath.Text = m_ProductRecipeOutputFileSettings.strSummaryFilePath;
                //m_tabpageOutputFile.textBoxPnPLabelOutputFilePath.Text = m_ProductRecipeOutputFileSettings.strPnPOutputFilePath;
                //m_tabpageOutputFile.textBoxPnpAdditionalFile.Text = m_ProductRecipeOutputFileSettings.strPnPAdditionFilePath;
                m_tabpageOutputFile.comboBoxDefectColor.Items.Clear();
                m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.Items.Clear();
                m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.Items.Clear();
                foreach (System.Reflection.PropertyInfo _propertyInfo in typeof(System.Drawing.Color).GetProperties())
                {
                    if (_propertyInfo.PropertyType == typeof(System.Drawing.Color))
                    {
                        m_tabpageOutputFile.comboBoxDefectColor.Items.Add(_propertyInfo.Name);
                        m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.Items.Add(_propertyInfo.Name);
                        m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.Items.Add(_propertyInfo.Name);
                    }
                }
                m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.SelectedItem = ((System.Drawing.Color)System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Drawing.Color)).ConvertFromString(m_ProductRecipeOutputFileSettings.EmptyUnitColorInHex)).Name;
                m_tabpageOutputFile.pictureBoxEmptyUnitDefectColor.BackColor = System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.SelectedItem.ToString());
                m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.SelectedItem = ((System.Drawing.Color)System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Drawing.Color)).ConvertFromString(m_ProductRecipeOutputFileSettings.UnitSlantedColorInHex)).Name;
                m_tabpageOutputFile.pictureBoxUnitSlantedDefectColor.BackColor = System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.SelectedItem.ToString());
                updateRichTextBoxMessageRecipeOutputFile("Recipe loaded successfully.");
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeOutputFile("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        
        //virtual public void RefreshInputCassetteRecipeParameter()
        //{
        //    try
        //    {
        //        m_tabpageInputCassette.numericUpDownCassetteTotalSlot.Value = m_ProductRecipeInputCassetteSettings.CassetteTotalSlot;
        //        m_tabpageInputCassette.numericUpDownCassetteSlotPitch_um.Value = m_ProductRecipeInputCassetteSettings.CassetteSlotPitch_um;
        //        m_tabpageInputCassette.numericUpDownCassetteFirstSlotOffset_um.Value = m_ProductRecipeInputCassetteSettings.CassetteFirstSlotOffset_um;
        //        m_tabpageInputCassette.numericUpDownCassetteUnloadOffset_um.Value = m_ProductRecipeInputCassetteSettings.CassetteUnloadOffset_um;

        //        updateRichTextBoxMessageRecipeInputCassette("Recipe loaded successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        updateRichTextBoxMessageRecipeInputCassette("Recipe display fail.");
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //virtual public void RefreshOutputCassetteRecipeParameter()
        //{
        //    try
        //    {
        //        m_tabpageOutputCassette.numericUpDownCassetteTotalSlot.Value = m_ProductRecipeOutputCassetteSettings.CassetteTotalSlot;
        //        m_tabpageOutputCassette.numericUpDownCassetteSlotPitch_um.Value = m_ProductRecipeOutputCassetteSettings.CassetteSlotPitch_um;
        //        m_tabpageOutputCassette.numericUpDownCassetteFirstSlotOffset_um.Value = m_ProductRecipeOutputCassetteSettings.CassetteFirstSlotOffset_um;
        //        m_tabpageOutputCassette.numericUpDownCassetteUnloadOffset_um.Value = m_ProductRecipeOutputCassetteSettings.CassetteUnloadOffset_um;

        //        updateRichTextBoxMessageRecipeOutputCassette("Recipe loaded successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        updateRichTextBoxMessageRecipeOutputCassette("Recipe display fail.");
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        virtual public void RefreshVisionRecipeParameter()
        {
            try
            {
                #region Input Vision
                m_tabpageVisionRecipe.checkBoxEnableInputVision.Checked = m_ProductRecipeVisionSettings.EnableInputVision;

                m_tabpageVisionRecipe.numericUpDownInputVisionInspectionCountInCol.Value = m_ProductRecipeVisionSettings.InputVisionInspectionCountInCol;
                m_tabpageVisionRecipe.numericUpDownInputVisionInspectionCountInRow.Value = m_ProductRecipeVisionSettings.InputVisionInspectionCountInRow;

                m_tabpageVisionRecipe.checkBoxEnableTeachUnitAtVision.Checked = m_ProductRecipeVisionSettings.EnableTeachUnitAtVision;

                m_tabpageVisionRecipe.numericUpDownInputVisionRetryCountAfterFail.Value = m_ProductRecipeVisionSettings.InputVisionRetryCountAfterFail;
                m_tabpageVisionRecipe.numericUpDownInputVisionContinuousFailCountToTriggerAlarm.Value = m_ProductRecipeVisionSettings.InputVisionContinuousFailCountToTriggerAlarm;

                m_tabpageVisionRecipe.numericUpDownInputVisionUnitThetaOffset.Value = m_ProductRecipeVisionSettings.InputVisionUnitThetaOffset;

                m_tabpageVisionRecipe.listViewInputSnap.Items.Clear();
                foreach (VisionSnapInfo _Snap in m_ProductRecipeVisionSettings.listInputVisionSnap)
                {
                    string strEnableSnap = "NO";
                    if (_Snap.EnableSnap == true)
                        strEnableSnap = "YES";
                    string[] strArrayItem = new string[7] { _Snap.SnapNo.ToString(), _Snap.Description, strEnableSnap,
                         _Snap.VisionXAxisOffset.ToString(), _Snap.VisionYAxisOffset.ToString(),
                        _Snap.VisionZAxisOffset.ToString(), _Snap.VisionThetaAxisOffset.ToString() };
                    ListViewItem listViewItem = new ListViewItem(strArrayItem);
                    m_tabpageVisionRecipe.listViewInputSnap.Items.Add(listViewItem);

                }
                m_tabpageVisionRecipe.listViewInputSnap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalInputSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewInputSnap.Items.Count.ToString();
                #endregion Input Vision

                #region Bottom Vision
                m_tabpageVisionRecipe.checkBoxEnableBottomVision.Checked = m_ProductRecipeVisionSettings.EnableBottomVision;
                m_tabpageVisionRecipe.numericUpDownBTMVisionInspectionCountInCol.Value = m_ProductRecipeVisionSettings.BTMVisionInspectionCountInCol;
                m_tabpageVisionRecipe.numericUpDownBTMVisionInspectionCountInRow.Value = m_ProductRecipeVisionSettings.BTMVisionInspectionCountInRow;

                m_tabpageVisionRecipe.numericUpDownBottomVisionRetryCountAfterFail.Value = m_ProductRecipeVisionSettings.BottomVisionRetryCountAfterFail;
                m_tabpageVisionRecipe.numericUpDownBottomVisionContinuousFailCountToTriggerAlarm.Value = m_ProductRecipeVisionSettings.BottomVisionContinuousFailCountToTriggerAlarm;

                m_tabpageVisionRecipe.numericUpDownBottomVisionUnitThetaOffset.Value = m_ProductRecipeVisionSettings.BottomVisionUnitThetaOffset;

                m_tabpageVisionRecipe.listViewBottomSnap.Items.Clear();
                foreach (VisionSnapInfo _Snap in m_ProductRecipeVisionSettings.listBottomVisionSnap)
                {
                    string strEnableSnap = "NO";
                    if (_Snap.EnableSnap == true)
                        strEnableSnap = "YES";
                    string strEnableDiffuser = "NO";
                    if (_Snap.EnableDiffuser == true)
                        strEnableDiffuser = "YES";
                    string[] strArrayItem = new string[8] { _Snap.SnapNo.ToString(), _Snap.Description, strEnableSnap,
                         _Snap.VisionXAxisOffset.ToString(), _Snap.VisionYAxisOffset.ToString(),
                        _Snap.VisionZAxisOffset.ToString(), _Snap.VisionThetaAxisOffset.ToString(),strEnableDiffuser };
                    ListViewItem listViewItem = new ListViewItem(strArrayItem);
                    m_tabpageVisionRecipe.listViewBottomSnap.Items.Add(listViewItem);

                }
                m_tabpageVisionRecipe.listViewBottomSnap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalBottomSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewBottomSnap.Items.Count.ToString();

                #endregion Bottom Vision

                #region Setup Vision
                m_tabpageVisionRecipe.checkBoxEnableSetupVision.Checked = m_ProductRecipeVisionSettings.EnableSetupVision;

                m_tabpageVisionRecipe.numericUpDownSetupVisionRetryCountAfterFail.Value = m_ProductRecipeVisionSettings.SetupVisionRetryCountAfterFail;
                m_tabpageVisionRecipe.numericUpDownSetupVisionContinuousFailCountToTriggerAlarm.Value = m_ProductRecipeVisionSettings.SetupVisionContinuousFailCountToTriggerAlarm;

                m_tabpageVisionRecipe.numericUpDownSetupVisionUnitThetaOffset.Value = m_ProductRecipeVisionSettings.SetupVisionUnitThetaOffset;
                #endregion Input Vision

                #region S1 Vision
                m_tabpageVisionRecipe.checkBoxEnableS1Vision.Checked = m_ProductRecipeVisionSettings.EnableS1Vision;

                m_tabpageVisionRecipe.numericUpDownS1VisionRetryCountAfterFail.Value = m_ProductRecipeVisionSettings.S1VisionRetryCountAfterFail;
                m_tabpageVisionRecipe.numericUpDownS1VisionContinuousFailCountToTriggerAlarm.Value = m_ProductRecipeVisionSettings.S1VisionContinuousFailCountToTriggerAlarm;

                m_tabpageVisionRecipe.numericUpDownS1VisionUnitThetaOffset.Value = m_ProductRecipeVisionSettings.S1VisionUnitThetaOffset;

                m_tabpageVisionRecipe.listViewS1Snap.Items.Clear();
                foreach (VisionSnapInfo _Snap in m_ProductRecipeVisionSettings.listS1VisionSnap)
                {
                    string strEnableSnap = "NO";
                    if (_Snap.EnableSnap == true)
                        strEnableSnap = "YES";
                    string[] strArrayItem = new string[7] { _Snap.SnapNo.ToString(), _Snap.Description, strEnableSnap,
                         _Snap.VisionXAxisOffset.ToString(), _Snap.VisionYAxisOffset.ToString(),
                        _Snap.VisionZAxisOffset.ToString(), _Snap.VisionThetaAxisOffset.ToString() };
                    ListViewItem listViewItem = new ListViewItem(strArrayItem);
                    m_tabpageVisionRecipe.listViewS1Snap.Items.Add(listViewItem);

                }
                m_tabpageVisionRecipe.listViewS1Snap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalS1SnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewS1Snap.Items.Count.ToString();
                #endregion S1 Vision

                #region S2 Parting
                m_tabpageVisionRecipe.checkBoxEnableS2Vision.Checked = m_ProductRecipeVisionSettings.EnableS2Vision;

                m_tabpageVisionRecipe.numericUpDownS2VisionRetryCountAfterFail.Value = m_ProductRecipeVisionSettings.S2VisionRetryCountAfterFail;
                m_tabpageVisionRecipe.numericUpDownS2VisionContinuousFailCountToTriggerAlarm.Value = m_ProductRecipeVisionSettings.S2VisionContinuousFailCountToTriggerAlarm;

                m_tabpageVisionRecipe.numericUpDownS2VisionUnitThetaOffset.Value = m_ProductRecipeVisionSettings.S2VisionUnitThetaOffset;

                m_tabpageVisionRecipe.listViewS2Snap.Items.Clear();
                foreach (VisionSnapInfo _Snap in m_ProductRecipeVisionSettings.listS2VisionSnap)
                {
                    string strEnableSnap = "NO";
                    if (_Snap.EnableSnap == true)
                        strEnableSnap = "YES";
                    string[] strArrayItem = new string[7] { _Snap.SnapNo.ToString(), _Snap.Description, strEnableSnap,
                         _Snap.VisionXAxisOffset.ToString(), _Snap.VisionYAxisOffset.ToString(),
                        _Snap.VisionZAxisOffset.ToString(), _Snap.VisionThetaAxisOffset.ToString() };
                    ListViewItem listViewItem = new ListViewItem(strArrayItem);
                    m_tabpageVisionRecipe.listViewS2Snap.Items.Add(listViewItem);

                }
                m_tabpageVisionRecipe.listViewS2Snap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalS2SnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewS2Snap.Items.Count.ToString();
                #endregion S2 Parting

                #region S2 Facet
                m_tabpageVisionRecipe.numericUpDownS2FacetRetryCountAfterFail.Value = m_ProductRecipeVisionSettings.S2FacetVisionRetryCountAfterFail;
                m_tabpageVisionRecipe.numericUpDownS2FacetContinuousFailCountToTriggerAlarm.Value = m_ProductRecipeVisionSettings.S2FacetVisionContinuousFailCountToTriggerAlarm;

                m_tabpageVisionRecipe.numericUpDownS2FacetUnitThetaOffset.Value = m_ProductRecipeVisionSettings.S2FacetVisionUnitThetaOffset;
                m_tabpageVisionRecipe.checkBoxS2S3BothSnap.Checked = m_ProductRecipeVisionSettings.EnableS2S3BothSnapping;

                m_tabpageVisionRecipe.listViewS2FacetSnap.Items.Clear();
                foreach (VisionSnapInfo _Snap in m_ProductRecipeVisionSettings.listS2FacetVisionSnap)
                {
                    string strEnableSnap = "NO";
                    if (_Snap.EnableSnap == true)
                        strEnableSnap = "YES";
                    string[] strArrayItem = new string[7] { _Snap.SnapNo.ToString(), _Snap.Description, strEnableSnap,
                         _Snap.VisionXAxisOffset.ToString(), _Snap.VisionYAxisOffset.ToString(),
                        _Snap.VisionZAxisOffset.ToString(), _Snap.VisionThetaAxisOffset.ToString() };
                    ListViewItem listViewItem = new ListViewItem(strArrayItem);
                    m_tabpageVisionRecipe.listViewS2FacetSnap.Items.Add(listViewItem);

                }
                m_tabpageVisionRecipe.listViewS2FacetSnap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalS2SnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewS2FacetSnap.Items.Count.ToString();
                #endregion S2 Facet

                #region Output Vision
                m_tabpageVisionRecipe.checkBoxEnableOutputVision.Checked = m_ProductRecipeVisionSettings.EnableOutputVision;
                m_tabpageVisionRecipe.checkBoxEnableOutputVision2ndSnap.Checked = m_ProductRecipeVisionSettings.EnableOutputVision2ndPostAlign;
                m_tabpageVisionRecipe.numericUpDownOutputVisionInspectionCountInCol.Value = m_ProductRecipeVisionSettings.OutputVisionInspectionCountInCol;
                m_tabpageVisionRecipe.numericUpDownOutputVisionInspectionCountInRow.Value = m_ProductRecipeVisionSettings.OutputVisionInspectionCountInRow;

                m_tabpageVisionRecipe.numericUpDownOutputVisionRetryCountAfterFail.Value = m_ProductRecipeVisionSettings.OutputVisionRetryCountAfterFail;
                m_tabpageVisionRecipe.numericUpDownOutputVisionContinuousFailCountToTriggerAlarm.Value = m_ProductRecipeVisionSettings.OutputVisionContinuousFailCountToTriggerAlarm;

                m_tabpageVisionRecipe.numericUpDownOutputVisionUnitThetaOffset.Value = m_ProductRecipeVisionSettings.OutputVisionUnitThetaOffset;

                m_tabpageVisionRecipe.listViewOutputSnap.Items.Clear();
                foreach (VisionSnapInfo _Snap in m_ProductRecipeVisionSettings.listOutputVisionSnap)
                {
                    string strEnableSnap = "NO";
                    if (_Snap.EnableSnap == true)
                        strEnableSnap = "YES";
                    string[] strArrayItem = new string[7] { _Snap.SnapNo.ToString(), _Snap.Description, strEnableSnap,
                         _Snap.VisionXAxisOffset.ToString(), _Snap.VisionYAxisOffset.ToString(),
                        _Snap.VisionZAxisOffset.ToString(), _Snap.VisionThetaAxisOffset.ToString() };
                    ListViewItem listViewItem = new ListViewItem(strArrayItem);
                    m_tabpageVisionRecipe.listViewOutputSnap.Items.Add(listViewItem);

                }
                m_tabpageVisionRecipe.listViewOutputSnap.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageVisionRecipe.labelTotalOutputSnapNo.Text = "Total : " + m_tabpageVisionRecipe.listViewOutputSnap.Items.Count.ToString();
                #endregion Output Vision

                updateRichTextBoxMessageRecipeVision("Recipe loaded successfully.");
            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeVision("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        //virtual public void RefreshSortingRecipeParameter()
        //{
        //    try
        //    {
        //        m_tabpageSortingRecipe.checkBoxEnableSortingMode.Checked = (bool)m_ProductRecipeSortingSetting.EnableSortingMode;
        //        if (m_ProductRecipeSortingSetting.SortingMode == 0)
        //        {
        //            m_tabpageSortingRecipe.radioButtonByInputFile.Checked = true;
        //            m_tabpageSortingRecipe.radioButtonByRecipe.Checked = false;
        //        }
        //        if (m_ProductRecipeSortingSetting.SortingMode == 1)
        //        {
        //            m_tabpageSortingRecipe.radioButtonByInputFile.Checked = false;
        //            m_tabpageSortingRecipe.radioButtonByRecipe.Checked = true;
        //        }
        //        if (m_ProductRecipeSortingSetting.EnableSortingMode == true)
        //        {
        //            //m_tabpageSortingRecipe.flowLayoutPanel1.Visible = true;
        //            if (m_ProductRecipeSortingSetting.SortingMode == 0)
        //            {
        //                m_tabpageSortingRecipe.textBoxSortingFilePath.Text = m_ProductRecipeSortingSetting.SortingInputFilePath;
        //            }

        //            if (m_ProductRecipeSortingSetting.SortingMode == 1)
        //            {
        //                if (m_ProductRecipeSortingSetting.lstSortings.Count > 0)
        //                {
        //                    m_tabpageSortingRecipe.comboBoxSortingBarcode.Items.Clear();
        //                    for (int i = 0; i < m_ProductRecipeSortingSetting.lstSortings.Count; i++)
        //                    {
        //                        m_tabpageSortingRecipe.comboBoxSortingBarcode.Items.Add(m_ProductRecipeSortingSetting.lstSortings[i].strSortingBarcode);
        //                    }
        //                    m_tabpageSortingRecipe.comboBoxSortingBarcode.Text = m_tabpageSortingRecipe.comboBoxSortingBarcode.Items[0].ToString();
        //                    m_tabpageSortingRecipe.labelNoOfWords.Text = m_ProductRecipeSortingSetting.lstSortings[0].intBarcodeLength.ToString();
        //                    m_tabpageSortingRecipe.numericUpDownCharacterNo.Value = m_ProductRecipeSortingSetting.lstSortings[0].intBarcodeLength;
        //                    if (m_ProductRecipeSortingSetting.lstSortings[0].SortDataCorrect.Count > 0)
        //                    {
        //                        m_tabpageSortingRecipe.flowLayoutPanel1.Controls.Clear();
        //                        foreach (var item in m_ProductRecipeSortingSetting.lstSortings[0].SortDataCorrect)
        //                        {
        //                            //var ItemContol = AddUsercontrol(item.CleanCount, item.WarningCount, item.DueCount, item.CurrentCount, item.ItemName, item.ItemLocation);
        //                            var ItemContol = AddUsercontrol(item.strName, item.intWordStartNo, item.intWordEndNo, item.intSelectType, item.strRangeStart, item.strRangeEnd);
        //                            m_tabpageSortingRecipe.flowLayoutPanel1.Controls.Add(ItemContol);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //             //m_tabpageSortingRecipe.flowLayoutPanel1.Visible = false;
        //        }
        //        updateRichTextBoxMessageRecipeInput("Recipe loaded successfully.");

        //    }
        //    catch (Exception ex)
        //    {
        //        updateRichTextBoxMessageRecipeInput("Recipe display fail.");
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        virtual public void RefreshPickUpHeadRecipeParameter()
        {
            try
            {
                m_tabpagePickUpHeadRecipe.checkBoxEnableSoftLanding.Checked = m_ProductRecipePickUpHeadSetting.EnablePickSoftlanding;
                m_tabpagePickUpHeadRecipe.checkBoxEnablePlaceSoftLanding.Checked = m_ProductRecipePickUpHeadSetting.EnablePlaceSoftlanding;

                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead1FlowRate.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead1FlowRate;
                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead1Pressure.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead1Pressure;
                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead1Force.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead1Force;

                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead2FlowRate.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead2FlowRate;
                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead2Pressure.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead2Pressure;
                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead2Force.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead2Force;
                
                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace1FlowRate.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead1PlaceFlowRate;
                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace1Pressure.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead1PlacePressure;
                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace1Force.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead1PlaceForce;

                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace2FlowRate.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead2PlaceFlowRate;
                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace2Pressure.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead2PlacePressure;
                m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace2Force.Value = (decimal)m_ProductRecipePickUpHeadSetting.PickUpHead2PlaceForce;

                m_tabpagePickUpHeadRecipe.checkBoxEnableSafetyPnPMovePickStation.Checked = m_ProductRecipePickUpHeadSetting.EnableSafetyPnPMovePickStation;

                updateRichTextBoxMessageRecipePickUpHead("Recipe loaded successfully.");

            }
            catch (Exception ex)
            {
                updateRichTextBoxMessageRecipeInput("Recipe display fail.");
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }


        virtual public bool VerifyMainRecipe()
        {
            try
            {
                #region Verify Recipe
                if (m_tabpageMainRecipe.comboBoxInput.SelectedIndex != -1)
                {
                    if (Tools.IsFileExist(m_strRecipeInputPath, m_tabpageMainRecipe.comboBoxInput.SelectedItem.ToString(), m_strRecipeExtension))
                    {
                        m_ProductRecipeMainSettings.InputRecipeName = m_tabpageMainRecipe.comboBoxInput.SelectedItem.ToString();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Input recipe not exist.");
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Please select input recipe.");
                    return false;
                }

                if (m_tabpageMainRecipe.comboBoxOutput.SelectedIndex != -1)
                {
                    if (Tools.IsFileExist(m_strRecipeOutputPath, m_tabpageMainRecipe.comboBoxOutput.SelectedItem.ToString(), m_strRecipeExtension))
                    {
                        m_ProductRecipeMainSettings.OutputRecipeName = m_tabpageMainRecipe.comboBoxOutput.SelectedItem.ToString();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Output recipe not exist.");
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Please select output recipe.");
                    return false;
                }
                if (m_tabpageMainRecipe.comboBoxDelay.SelectedIndex != -1)
                {
                    if (Tools.IsFileExist(m_strRecipeDelayPath, m_tabpageMainRecipe.comboBoxDelay.SelectedItem.ToString(), m_strRecipeExtension))
                    {
                        m_ProductRecipeMainSettings.DelayRecipeName = m_tabpageMainRecipe.comboBoxDelay.SelectedItem.ToString();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Delay recipe not exist.");
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Please select delay recipe.");
                    return false;
                }
                if (m_tabpageMainRecipe.comboBoxMotorPosition.SelectedIndex != -1)
                {
                    if (Tools.IsFileExist(m_strRecipeMotorPositionPath, m_tabpageMainRecipe.comboBoxMotorPosition.SelectedItem.ToString(), m_strRecipeExtension))
                    {
                        m_ProductRecipeMainSettings.MotorPositionRecipeName = m_tabpageMainRecipe.comboBoxMotorPosition.SelectedItem.ToString();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Motor Position recipe not exist.");
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Please select Motor Position recipe.");
                    return false;
                }

                if (m_tabpageMainRecipe.comboBoxOutputFile.SelectedIndex != -1)
                {
                    if (Tools.IsFileExist(m_strRecipeOutputFilePath, m_tabpageMainRecipe.comboBoxOutputFile.SelectedItem.ToString(), m_strRecipeExtension))
                    {
                        m_ProductRecipeMainSettings.OutputFileRecipeName = m_tabpageMainRecipe.comboBoxOutputFile.SelectedItem.ToString();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Output File recipe not exist.");
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Please select Output File recipe.");
                    return false;
                }

                //if (m_tabpageMainRecipe.comboBoxInputCassette.SelectedIndex != -1)
                //{
                //    if (Tools.IsFileExist(m_strRecipeInputCassettePath, m_tabpageMainRecipe.comboBoxInputCassette.SelectedItem.ToString(), m_strRecipeExtension))
                //    {
                //        m_ProductRecipeMainSettings.InputCassetteRecipeName = m_tabpageMainRecipe.comboBoxInputCassette.SelectedItem.ToString();
                //    }
                //    else
                //    {
                //        updateRichTextBoxMessageRecipeMain("Input Cassette recipe not exist.");
                //        return false;
                //    }
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipeMain("Please select Input Cassette recipe.");
                //    return false;
                //}

                //if (m_tabpageMainRecipe.comboBoxOutputCassette.SelectedIndex != -1)
                //{
                //    if (Tools.IsFileExist(m_strRecipeOutputCassettePath, m_tabpageMainRecipe.comboBoxOutputCassette.SelectedItem.ToString(), m_strRecipeExtension))
                //    {
                //        m_ProductRecipeMainSettings.OutputCassetteRecipeName = m_tabpageMainRecipe.comboBoxOutputCassette.SelectedItem.ToString();
                //    }
                //    else
                //    {
                //        updateRichTextBoxMessageRecipeMain("Output Cassette recipe not exist.");
                //        return false;
                //    }
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipeMain("Please select Output Cassette recipe.");
                //    return false;
                //}

                if (m_tabpageMainRecipe.comboBoxVision.SelectedIndex != -1)
                {
                    if (Tools.IsFileExist(m_strRecipeVisionPath, m_tabpageMainRecipe.comboBoxVision.SelectedItem.ToString(), m_strRecipeExtension))
                    {
                        m_ProductRecipeMainSettings.VisionRecipeName = m_tabpageMainRecipe.comboBoxVision.SelectedItem.ToString();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Vision recipe not exist.");
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Please select Vision recipe.");
                    return false;
                }

                if (m_tabpageMainRecipe.comboBoxInspectionRecipe.SelectedIndex != -1)
                {
                    if (Tools.IsFileExist(m_ProductShareVariables.productOptionSettings.VisionRecipeFolderPath, m_tabpageMainRecipe.comboBoxInspectionRecipe.SelectedItem.ToString(), m_strRecipeExtension))
                    {
                        m_ProductRecipeMainSettings.InspectionRecipeName = m_tabpageMainRecipe.comboBoxInspectionRecipe.SelectedItem.ToString();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Inspection recipe not exist.");
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Please select Inspection recipe.");
                    return false;
                }

                //if (m_tabpageMainRecipe.comboBoxSorting.SelectedIndex != -1)
                //{
                //    if (Tools.IsFileExist(m_strRecipeSortingPath, m_tabpageMainRecipe.comboBoxSorting.SelectedItem.ToString(), m_strRecipeExtension))
                //    {
                //        m_ProductRecipeMainSettings.SortingRecipeName = m_tabpageMainRecipe.comboBoxSorting.SelectedItem.ToString();
                //    }
                //    else
                //    {
                //        updateRichTextBoxMessageRecipeMain("Sorting recipe not exist.");
                //        return false;
                //    }
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipeMain("Please select Sorting recipe.");
                //    return false;
                //}

                if (m_tabpageMainRecipe.comboBoxPickUpHead.SelectedIndex != -1)
                {
                    if (Tools.IsFileExist(m_strRecipePickUpHeadPath, m_tabpageMainRecipe.comboBoxPickUpHead.SelectedItem.ToString(), m_strRecipeExtension))
                    {
                        m_ProductRecipeMainSettings.PickUpHeadRecipeName = m_tabpageMainRecipe.comboBoxPickUpHead.SelectedItem.ToString();
                    }
                    else
                    {
                        updateRichTextBoxMessageRecipeMain("Pick Up Head recipe not exist.");
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessageRecipeMain("Please select Pick Up Head recipe.");
                    return false;
                }
                #endregion Verify Recipe

                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool VerifyInputRecipe()
        {
            try
            {
                #region Verify Recipe
                m_ProductRecipeInputSettings.NoOfDeviceInColInput = (uint)m_tabpageInput.numericUpDownNoOfDeviceInColInput.Value;
                m_ProductRecipeInputSettings.NoOfDeviceInRowInput = (uint)m_tabpageInput.numericUpDownNoOfDeviceInRowInput.Value;
                m_ProductRecipeInputSettings.DeviceXPitchInput = (uint)m_tabpageInput.numericUpDownDeviceXPitchInput.Value;
                m_ProductRecipeInputSettings.DeviceYPitchInput = (uint)m_tabpageInput.numericUpDownDeviceYPitchInput.Value;

                m_ProductRecipeInputSettings.UnitThickness_um = (uint)m_tabpageInput.numericUpDownDeviceThickness_um.Value;
                m_ProductRecipeInputSettings.InputPocketDepth_um = (uint)m_tabpageInput.numericUpDowPocketDepthInput_um.Value;
                
                m_ProductRecipeInputSettings.PickingCenterXOffsetInput = (int)m_tabpageInput.numericUpDownPickingCenterXOffsetInput.Value;
                m_ProductRecipeInputSettings.PickingCenterYOffsetInput = (int)m_tabpageInput.numericUpDownPickingCenterYOffsetInput.Value;
                m_ProductRecipeInputSettings.UnitPlacementRotationOffsetInput = (int)m_tabpageInput.numericUpDownUnitPlacementRotationOffsetInput.Value;
                m_ProductRecipeInputSettings.InputTrayThickness = (uint)m_tabpageInput.numericUpDownInputTrayThickness_um.Value;

                m_ProductRecipeInputSettings.BottomVisionXOffset = (int)m_tabpageInput.numericUpDownBottomVisionXOffset.Value;
                m_ProductRecipeInputSettings.BottomVisionYOffset = (int)m_tabpageInput.numericUpDownBottomVisionYOffset.Value;
                m_ProductRecipeInputSettings.ContinuouslyEmptyPocket = (int)m_tabpageInput.numericUpDownContinuouslyEmptyPocket.Value;
                
                m_ProductRecipeInputSettings.EnableCheckEmptyUnit = (bool)m_tabpageInput.checkBoxEnableCheckmptyUnit.Checked;
                m_ProductRecipeInputSettings.EnableInputTableVacuum = (bool)m_tabpageInput.checkBoxEnableInputTableVacuum.Checked;
                m_ProductRecipeInputSettings.EmptyUnit = (int)m_tabpageInput.numericUpDownEmptyUnit.Value;
                m_ProductRecipeInputSettings.BarcodeRecipe = m_tabpageInput.comboBoxBarcodeRecipe.Text;

                m_ProductRecipeInputSettings.EnablePurging = (bool)m_tabpageInput.checkBoxEnablePurging.Checked;
                #region BarcodeReaderKeyence
                //if (m_tabpageInput.comboBoxBarcodeReaderKeyence_Recipe.SelectedItem != null)
                //{
                //    if (m_tabpageInput.comboBoxBarcodeReaderKeyence_Recipe.SelectedIndex == 0)
                //    {
                //        m_ProductRecipeInputSettings.BarcodeReaderKeyence_Recipe = 1;
                //    }
                //    else if (m_tabpageInput.comboBoxBarcodeReaderKeyence_Recipe.SelectedIndex == 1)
                //    {
                //        m_ProductRecipeInputSettings.BarcodeReaderKeyence_Recipe = 2;
                //    }
                //    else if (m_tabpageInput.comboBoxBarcodeReaderKeyence_Recipe.SelectedIndex == 2)
                //    {
                //        m_ProductRecipeInputSettings.BarcodeReaderKeyence_Recipe = 3;
                //    }
                //    else
                //    {
                //        updateRichTextBoxMessageRecipeInput("Unknown Barcode Reader Keyence Recipe.");
                //        return false;
                //    }
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipeInput("Please Select Barcode Reader Keyence Recipe!");
                //    return false;
                //}
               
                #endregion
                #endregion Verify Recipe
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool VerifyOutputRecipe()
        {
            try
            {
                #region Verify Recipe
                m_ProductRecipeOutputSettings.NoOfDeviceInColOutput = (uint)m_tabpageOutput.numericUpDownNoOfDeviceInColOutput.Value;
                m_ProductRecipeOutputSettings.NoOfDeviceInRowOutput = (uint)m_tabpageOutput.numericUpDownNoOfDeviceInRowOutput.Value;
                m_ProductRecipeOutputSettings.DeviceXPitchOutput = (uint)m_tabpageOutput.numericUpDownDeviceXPitchOutput.Value;
                m_ProductRecipeOutputSettings.DeviceYPitchOutput = (uint)m_tabpageOutput.numericUpDownDeviceYPitchOutput.Value;
                m_ProductRecipeOutputSettings.OutputTrayThickness = (uint)m_tabpageOutput.numericUpDownOutputTrayThickness_um.Value;
                m_ProductRecipeOutputSettings.OutputPocketDepth_um = (uint)m_tabpageOutput.numericUpDownPocketDepthOutput_um.Value;

                m_ProductRecipeOutputSettings.TableXOffsetOutput = (int)m_tabpageOutput.numericUpDownTableXOffsetOutput.Value;
                m_ProductRecipeOutputSettings.TableYOffsetOutput = (int)m_tabpageOutput.numericUpDownTableYOffsetOutput.Value;
                m_ProductRecipeOutputSettings.UnitPlacementRotationOffsetOutput = (int)m_tabpageOutput.numericUpDownUnitPlacementRotationOffsetOutput.Value;
                //m_ProductRecipeOutputSettings.EnableGoodSamplingSequence = (bool)m_tabpageOutput.checkBoxGoodSampling.Checked;
                //m_ProductRecipeOutputSettings.EnableScanBarcodeOnOutputTray = (bool)m_tabpageOutput.checkBoxEnableBarcodeTray.Checked;
                m_ProductRecipeOutputSettings.EnableCheckContinuousDefectCode = (bool)m_tabpageOutput.checkBoxCheckContinuousDefectCode.Checked;
                m_ProductRecipeOutputSettings.EnableOutputTableVacuum = (bool)m_tabpageOutput.checkBoxEnableOutputTableVacuum.Checked;
                m_ProductRecipeOutputSettings.CheckContinuousDefectCode = (uint)m_tabpageOutput.numericUpDownCheckContinuousDefectCode.Value;

                m_ProductRecipeOutputSettings.LowYieldAlarm = (uint)m_tabpageOutput.numericUpDownLowYieldAlarm.Value;
                m_ProductRecipeOutputSettings.YieldPercentage = (double)m_tabpageOutput.numericUpDownYieldPercentage.Value;
                //if (m_tabpageOutput.comboBoxInputOutputRejectTray.SelectedIndex != -1)
                //{
                //    m_ProductRecipeOutputSettings.InputOutputDefectRejectTray = (uint)(m_tabpageOutput.comboBoxInputOutputRejectTray.SelectedIndex + 1);
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipePickUpHead("Please select valid Reject Tray For Input Output Reject Tray.");
                //    return false;
                //}
                //if (m_tabpageOutput.comboBoxS2RejectTray.SelectedIndex != -1)
                //{
                //    m_ProductRecipeOutputSettings.S2DefectRejectTray = (uint)(m_tabpageOutput.comboBoxS2RejectTray.SelectedIndex + 1);
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipePickUpHead("Please select valid Reject Tray For S2 Reject Tray.");
                //    return false;
                //}
                //if (m_tabpageOutput.comboBoxSetupRejectTray.SelectedIndex != -1)
                //{
                //    m_ProductRecipeOutputSettings.SetupDefectRejectTray = (uint)(m_tabpageOutput.comboBoxSetupRejectTray.SelectedIndex + 1);
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipePickUpHead("Please select valid Reject Tray For Setup Reject Tray.");
                //    return false;
                //}
                //if (m_tabpageOutput.comboBoxS1RejectTray.SelectedIndex != -1)
                //{
                //    m_ProductRecipeOutputSettings.S1DefectRejectTray = (uint)(m_tabpageOutput.comboBoxS1RejectTray.SelectedIndex + 1);
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipePickUpHead("Please select valid Reject Tray For S1 Reject Tray.");
                //    return false;
                //}
                //if (m_tabpageOutput.comboBoxSidewallLeftRejectTray.SelectedIndex != -1)
                //{
                //    m_ProductRecipeOutputSettings.SidewallLeftDefectRejectTray = (uint)(m_tabpageOutput.comboBoxSidewallLeftRejectTray.SelectedIndex + 1);
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipePickUpHead("Please select valid Reject Tray For Sidewall Left Reject Tray.");
                //    return false;
                //}
                //if (m_tabpageOutput.comboBoxSidewallRightRejectTray.SelectedIndex != -1)
                //{
                //    m_ProductRecipeOutputSettings.SidewallRightDefectRejectTray = (uint)(m_tabpageOutput.comboBoxSidewallRightRejectTray.SelectedIndex + 1);
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipePickUpHead("Please select valid Reject Tray For Sidewall Right Reject Tray.");
                //    return false;
                //}
                //if (m_tabpageOutput.comboBoxS3RejectTray.SelectedIndex != -1)
                //{
                //    m_ProductRecipeOutputSettings.S3DefectRejectTray = (uint)(m_tabpageOutput.comboBoxS3RejectTray.SelectedIndex + 1);
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipePickUpHead("Please select valid Reject Tray For S3 Reject Tray.");
                //    return false;
                //}
                //if (m_tabpageOutput.comboBoxSidewallFrontRejectTray.SelectedIndex != -1)
                //{
                //    m_ProductRecipeOutputSettings.SidewallFrontDefectRejectTray = (uint)(m_tabpageOutput.comboBoxSidewallFrontRejectTray.SelectedIndex + 1);
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipePickUpHead("Please select valid Reject Tray For Sidewall Front Reject Tray.");
                //    return false;
                //}
                //if (m_tabpageOutput.comboBoxSidewallRearRejectTray.SelectedIndex != -1)
                //{
                //    m_ProductRecipeOutputSettings.SidewallRearDefectRejectTray = (uint)(m_tabpageOutput.comboBoxSidewallRearRejectTray.SelectedIndex + 1);
                //}
                //else
                //{
                //    updateRichTextBoxMessageRecipePickUpHead("Please select valid Reject Tray For Sidewall Rear Reject Tray.");
                //    return false;
                //}
                if (m_tabpageOutput.comboBoxBarcodeLabelSize.SelectedIndex != -1)
                {
                    m_ProductRecipeOutputSettings.BarcodeLabelSize = (uint)(m_tabpageOutput.comboBoxBarcodeLabelSize.SelectedIndex);
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutput("Please select valid Barcode Printer Label Size.");
                    return false;
                }
                m_ProductRecipeOutputSettings.listSortTrayInfo.Clear();
                foreach (ListViewItem _listViewSortTrayConfiguration in m_tabpageOutput.listViewSortTrayConfiguration.Items)
                {
                    if(_listViewSortTrayConfiguration.SubItems[0].Text == "" || _listViewSortTrayConfiguration.SubItems[0].Text == null)
                    {
                        updateRichTextBoxMessageRecipeOutput("Please fill in the reject tray for ."+_listViewSortTrayConfiguration.SubItems[1].Text);
                        m_ProductRecipeOutputSettings.listSortTrayInfo.Clear();
                        return false;
                    }
                    m_ProductRecipeOutputSettings.listSortTrayInfo.Add(new SortTrayInfo
                    {
                        DefectCode = _listViewSortTrayConfiguration.SubItems[1].Text,
                        RejectTray = int.Parse(_listViewSortTrayConfiguration.SubItems[0].Text)
                    });
                }
                #endregion Verify Recipe
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool VerifyDelayRecipe()
        {
            try
            {
                #region Verify Recipe
                UInt32 unValue = 0;
                ListViewItem lvItem;

                FieldInfo[] fields = typeof(ProductRecipeDelaySettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo _fields in fields)
                {
                    lvItem = m_tabpageDelay.listViewDelay.FindItemWithText(GetSeparateWordFromCapitalString(_fields.Name));
                    if (UInt32.TryParse(lvItem.SubItems[1].Text, out unValue) == true)
                    {
                        _fields.SetValue(m_ProductRecipeDelaySettings, (uint)unValue);
                    }
                }

                //lvItem = m_tabpageDelay.listViewDelay.FindItemWithText("Input Pusher Delay Before Move Down");
                //if (UInt32.TryParse(lvItem.SubItems[1].Text, out unValue) == true)
                //{
                //    m_RecipeDelay.DelayBeforeInputPusherMoveDown = unValue;
                //}


                #endregion Verify Recipe
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool VerifyMotorPositionRecipe()
        {
            try
            {
                #region Verify Recipe
                int nValue = 0;
                ListViewItem lvItem;

                FieldInfo[] fields = typeof(ProductRecipeMotorPositionSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo _fields in fields)
                {
                    lvItem = m_tabpageMotorPosition.listViewMotorPosition.FindItemWithText(GetSeparateWordFromCapitalString(_fields.Name));
                    if (Int32.TryParse(lvItem.SubItems[1].Text, out nValue) == true)
                    {
                        _fields.SetValue(m_ProductRecipeMotorPositionSettings, (int)nValue);
                    }
                }

                //lvItem = m_tabpageMotorPosition.listViewMotorPosition.FindItemWithText("Input Pusher Down Distance From Touching Position");
                //if (Int32.TryParse(lvItem.SubItems[1].Text, out nValue) == true)
                //{
                //    //m_RecipeMotorPosition.InputPusherDownDistance = nValue;
                //}

                #endregion Verify Recipe
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public bool VerifyOutputFileRecipe()
        {
            try
            {
                #region Verify Recipe
                //m_ProductRecipeOutputFileSettings.listCoordinateSPC.Clear();
                //foreach (ListViewItem _listViewSPC in m_tabpageOutputFile.listViewSPC.Items)
                //{
                //    int nRow = -1;
                //    int nColumn = -1;
                //    if (Int32.TryParse(_listViewSPC.SubItems[0].Text, out nRow) == true
                //        && Int32.TryParse(_listViewSPC.SubItems[1].Text, out nColumn) == true
                //        )
                //    {
                //        m_ProductRecipeOutputFileSettings.listCoordinateSPC.Add(new Coordinate { Row = nRow, Column = nColumn });
                //    }

                //}
                m_ProductRecipeOutputFileSettings.listDefect.Clear();
                foreach (ListViewItem _listViewDefect in m_tabpageOutputFile.listViewDefect.Items)
                {
                    int nNo = -1;
                    //bool bEnableInReport = false;
                    bool bEnableInReport = true;
                    string strColorInHex = "";
                    //if (_listViewDefect.SubItems[4].Text.ToUpper() == "YES")
                    //    bEnableInReport = true;
                    //if (_listViewDefect.SubItems[4].Text.ToUpper() == "YES" || _listViewDefect.SubItems[4].Text.ToUpper() == "NO")
                    //{
                    //    bEnableInReport = true;
                    //}
                    strColorInHex = string.Format("#{0:X2}{1:X2}{2:X2}", System.Drawing.Color.FromName(_listViewDefect.SubItems[4].Text).R, System.Drawing.Color.FromName(_listViewDefect.SubItems[4].Text).G, System.Drawing.Color.FromName(_listViewDefect.SubItems[4].Text).B);
                    if (Int32.TryParse(_listViewDefect.SubItems[0].Text, out nNo) == true
                        )
                    {
                        m_ProductRecipeOutputFileSettings.listDefect.Add(new DefectProperty
                        {
                            No = nNo,
                            Code = _listViewDefect.SubItems[1].Text,
                            Description = _listViewDefect.SubItems[2].Text,
                            Pick = _listViewDefect.SubItems[3].Text,
                            EnableInReport = bEnableInReport,
                            ColorInHex = strColorInHex
                        });
                    }

                }
                    //m_ProductRecipeOutputFileSettings.listAdditionalFileDefectCode.Clear();
                    //foreach (ListViewItem item in m_tabpageOutputFile.listViewAdditionFileDefectCode.Items)
                    //{
                    //    m_ProductRecipeOutputFileSettings.listAdditionalFileDefectCode.Add(item.Text);
                    //}
                    //m_ProductRecipeOutputFileSettings.listSpcDefectCode.Clear();
                    //foreach (ListViewItem _listViewSpcDefectCode in m_tabpageOutputFile.listViewSpcDefectCode.Items)
                    //{
                    //    m_ProductRecipeOutputFileSettings.listSpcDefectCode.Add(_listViewSpcDefectCode.Text);
                    //}
                    m_ProductRecipeOutputFileSettings.strOutputFilePath = m_tabpageOutputFile.textBoxOutputFilePath.Text;
                    m_ProductRecipeOutputFileSettings.strOutputLocalFilePath = m_tabpageOutputFile.textBoxOutputLocalFilePath.Text;

                //m_ProductRecipeOutputFileSettings.strSummaryFilePath = m_tabpageOutputFile.textBoxServerSummaryFilePath.Text;
                //m_ProductRecipeOutputFileSettings.strPnPOutputFilePath = m_tabpageOutputFile.textBoxPnPLabelOutputFilePath.Text;
                //m_ProductRecipeOutputFileSettings.strPnPAdditionFilePath = m_tabpageOutputFile.textBoxPnpAdditionalFile.Text;
                if (m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.SelectedIndex != -1)
                {
                    m_ProductRecipeOutputFileSettings.EmptyUnitColorInHex = string.Format("#{0:X2}{1:X2}{2:X2}", System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.SelectedItem.ToString()).R, System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.SelectedItem.ToString()).G, System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxEmptyUnitDefectColor.SelectedItem.ToString()).B); 
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutputFile("Please select defect color for empty unit defect code.");
                    return false;
                }
                if (m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.SelectedIndex != -1)
                {
                    m_ProductRecipeOutputFileSettings.UnitSlantedColorInHex = string.Format("#{0:X2}{1:X2}{2:X2}", System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.SelectedItem.ToString()).R, System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.SelectedItem.ToString()).G, System.Drawing.Color.FromName(m_tabpageOutputFile.comboBoxUnitSlantedDefectColor.SelectedItem.ToString()).B); 
                }
                else
                {
                    updateRichTextBoxMessageRecipeOutputFile("Please select defect color for unit slanted defect code.");
                    return false;
                }
                #endregion Verify Recipe
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }
        
        //virtual public bool VerifyInputCassetteRecipe()
        //{
        //    try
        //    {
        //        #region Verify Recipe
        //        m_ProductRecipeInputCassetteSettings.CassetteTotalSlot = (uint)m_tabpageInputCassette.numericUpDownCassetteTotalSlot.Value;
        //        m_ProductRecipeInputCassetteSettings.CassetteSlotPitch_um = (uint)m_tabpageInputCassette.numericUpDownCassetteSlotPitch_um.Value;

        //        m_ProductRecipeInputCassetteSettings.CassetteFirstSlotOffset_um = (int)m_tabpageInputCassette.numericUpDownCassetteFirstSlotOffset_um.Value;
        //        m_ProductRecipeInputCassetteSettings.CassetteUnloadOffset_um = (int)m_tabpageInputCassette.numericUpDownCassetteUnloadOffset_um.Value;
        //        #endregion Verify Recipe
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        //virtual public bool VerifyOutputCassetteRecipe()
        //{
        //    try
        //    {
        //        #region Verify Recipe
        //        m_ProductRecipeOutputCassetteSettings.CassetteTotalSlot = (uint)m_tabpageOutputCassette.numericUpDownCassetteTotalSlot.Value;
        //        m_ProductRecipeOutputCassetteSettings.CassetteSlotPitch_um = (uint)m_tabpageOutputCassette.numericUpDownCassetteSlotPitch_um.Value;

        //        m_ProductRecipeOutputCassetteSettings.CassetteFirstSlotOffset_um = (int)m_tabpageOutputCassette.numericUpDownCassetteFirstSlotOffset_um.Value;
        //        m_ProductRecipeOutputCassetteSettings.CassetteUnloadOffset_um = (int)m_tabpageOutputCassette.numericUpDownCassetteUnloadOffset_um.Value;
        //        #endregion Verify Recipe
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        virtual public bool VerifyVisionRecipe()
        {
            try
            {
                #region Verify Recipe

                #region Input Vision
                m_ProductRecipeVisionSettings.EnableInputVision = m_tabpageVisionRecipe.checkBoxEnableInputVision.Checked;
                m_ProductRecipeVisionSettings.InputVisionInspectionCountInCol = (uint)m_tabpageVisionRecipe.numericUpDownInputVisionInspectionCountInCol.Value;
                m_ProductRecipeVisionSettings.InputVisionInspectionCountInRow = (uint)m_tabpageVisionRecipe.numericUpDownInputVisionInspectionCountInRow.Value;
                m_ProductRecipeVisionSettings.InputVisionRetryCountAfterFail = (int)m_tabpageVisionRecipe.numericUpDownInputVisionRetryCountAfterFail.Value;
                m_ProductRecipeVisionSettings.InputVisionContinuousFailCountToTriggerAlarm = (int)m_tabpageVisionRecipe.numericUpDownInputVisionContinuousFailCountToTriggerAlarm.Value;
                m_ProductRecipeVisionSettings.EnableTeachUnitAtVision = m_tabpageVisionRecipe.checkBoxEnableTeachUnitAtVision.Checked;

                m_ProductRecipeVisionSettings.InputVisionUnitThetaOffset = (int)m_tabpageVisionRecipe.numericUpDownInputVisionUnitThetaOffset.Value;

                m_ProductRecipeVisionSettings.listInputVisionSnap.Clear();
                foreach (ListViewItem _listViewInputSnap in m_tabpageVisionRecipe.listViewInputSnap.Items)
                {
                    int nNo = -1;
                    bool bEnableSnap = false;
                    if (_listViewInputSnap.SubItems[2].Text.ToUpper() == "YES" )//|| _listViewInputSnap.SubItems[2].Text.ToUpper() == "NO")
                    {
                        bEnableSnap = true;
                    }
                    if (Int32.TryParse(_listViewInputSnap.SubItems[0].Text, out nNo) == true
                        )
                    {
                        m_ProductRecipeVisionSettings.listInputVisionSnap.Add(new VisionSnapInfo
                        {
                            SnapNo = nNo,
                            Description = _listViewInputSnap.SubItems[1].Text,
                            EnableSnap = bEnableSnap,
                            VisionXAxisOffset = int.Parse(_listViewInputSnap.SubItems[3].Text),
                            VisionYAxisOffset = int.Parse(_listViewInputSnap.SubItems[4].Text),
                            VisionZAxisOffset = int.Parse(_listViewInputSnap.SubItems[5].Text),
                            VisionThetaAxisOffset = int.Parse(_listViewInputSnap.SubItems[6].Text)
                        });
                    }

                }
                #endregion Input Vision

                #region Bottom Vision
                m_ProductRecipeVisionSettings.EnableBottomVision = m_tabpageVisionRecipe.checkBoxEnableBottomVision.Checked;
                m_ProductRecipeVisionSettings.BottomVisionRetryCountAfterFail = (int)m_tabpageVisionRecipe.numericUpDownBottomVisionRetryCountAfterFail.Value;
                m_ProductRecipeVisionSettings.BottomVisionContinuousFailCountToTriggerAlarm = (int)m_tabpageVisionRecipe.numericUpDownBottomVisionContinuousFailCountToTriggerAlarm.Value;
                m_ProductRecipeVisionSettings.BTMVisionInspectionCountInCol = (uint)m_tabpageVisionRecipe.numericUpDownBTMVisionInspectionCountInCol.Value;
                m_ProductRecipeVisionSettings.BTMVisionInspectionCountInRow = (uint)m_tabpageVisionRecipe.numericUpDownBTMVisionInspectionCountInRow.Value;

                m_ProductRecipeVisionSettings.BottomVisionUnitThetaOffset = (int)m_tabpageVisionRecipe.numericUpDownBottomVisionUnitThetaOffset.Value;
                m_ProductRecipeVisionSettings.listBottomVisionSnap.Clear();
                foreach (ListViewItem _listViewBottomSnap in m_tabpageVisionRecipe.listViewBottomSnap.Items)
                {
                    int nNo = -1;
                    bool bEnableSnap = false;
                    bool bEnableDiffuser = false;
                    if (_listViewBottomSnap.SubItems[2].Text.ToUpper() == "YES")//|| _listViewBottomSnap.SubItems[2].Text.ToUpper() == "NO")
                    {
                        bEnableSnap = true;
                    }
                    if (_listViewBottomSnap.SubItems[7].Text.ToUpper() == "YES")
                    {
                        bEnableDiffuser = true;
                    }
                    if (Int32.TryParse(_listViewBottomSnap.SubItems[0].Text, out nNo) == true
                        )
                    {
                        m_ProductRecipeVisionSettings.listBottomVisionSnap.Add(new VisionSnapInfo
                        {
                            SnapNo = nNo,
                            Description = _listViewBottomSnap.SubItems[1].Text,
                            EnableSnap = bEnableSnap,
                            VisionXAxisOffset = int.Parse(_listViewBottomSnap.SubItems[3].Text),
                            VisionYAxisOffset = int.Parse(_listViewBottomSnap.SubItems[4].Text),
                            VisionZAxisOffset = int.Parse(_listViewBottomSnap.SubItems[5].Text),
                            VisionThetaAxisOffset = int.Parse(_listViewBottomSnap.SubItems[6].Text),
                            EnableDiffuser = bEnableDiffuser
                        });
                    }

                }
                #endregion Bottom Vision

                #region Setup Vision
                m_ProductRecipeVisionSettings.EnableSetupVision = m_tabpageVisionRecipe.checkBoxEnableSetupVision.Checked;
                m_ProductRecipeVisionSettings.SetupVisionRetryCountAfterFail = (int)m_tabpageVisionRecipe.numericUpDownSetupVisionRetryCountAfterFail.Value;
                m_ProductRecipeVisionSettings.SetupVisionContinuousFailCountToTriggerAlarm = (int)m_tabpageVisionRecipe.numericUpDownSetupVisionContinuousFailCountToTriggerAlarm.Value;

                m_ProductRecipeVisionSettings.SetupVisionUnitThetaOffset = (int)m_tabpageVisionRecipe.numericUpDownSetupVisionUnitThetaOffset.Value;
                #endregion S1 Setup

                #region S1 Vision
                m_ProductRecipeVisionSettings.EnableS1Vision = m_tabpageVisionRecipe.checkBoxEnableS1Vision.Checked;
                m_ProductRecipeVisionSettings.S1VisionRetryCountAfterFail = (int)m_tabpageVisionRecipe.numericUpDownS1VisionRetryCountAfterFail.Value;
                m_ProductRecipeVisionSettings.S1VisionContinuousFailCountToTriggerAlarm = (int)m_tabpageVisionRecipe.numericUpDownS1VisionContinuousFailCountToTriggerAlarm.Value;

                m_ProductRecipeVisionSettings.S1VisionUnitThetaOffset = (int)m_tabpageVisionRecipe.numericUpDownS1VisionUnitThetaOffset.Value;

                m_ProductRecipeVisionSettings.listS1VisionSnap.Clear();
                foreach (ListViewItem _listViewS1Snap in m_tabpageVisionRecipe.listViewS1Snap.Items)
                {
                    int nNo = -1;
                    bool bEnableSnap = false;
                    if (_listViewS1Snap.SubItems[2].Text.ToUpper() == "YES")//|| _listViewS1Snap.SubItems[2].Text.ToUpper() == "NO")
                    {
                        bEnableSnap = true;
                    }
                    if (Int32.TryParse(_listViewS1Snap.SubItems[0].Text, out nNo) == true
                        )
                    {
                        m_ProductRecipeVisionSettings.listS1VisionSnap.Add(new VisionSnapInfo
                        {
                            SnapNo = nNo,
                            Description = _listViewS1Snap.SubItems[1].Text,
                            EnableSnap = bEnableSnap,
                            VisionXAxisOffset = int.Parse(_listViewS1Snap.SubItems[3].Text),
                            VisionYAxisOffset = int.Parse(_listViewS1Snap.SubItems[4].Text),
                            VisionZAxisOffset = int.Parse(_listViewS1Snap.SubItems[5].Text),
                            VisionThetaAxisOffset = int.Parse(_listViewS1Snap.SubItems[6].Text)
                        });
                    }

                }
                #endregion S1 Vision

                #region S2 Parting
                m_ProductRecipeVisionSettings.EnableS2Vision = m_tabpageVisionRecipe.checkBoxEnableS2Vision.Checked;
                m_ProductRecipeVisionSettings.S2VisionRetryCountAfterFail = (int)m_tabpageVisionRecipe.numericUpDownS2VisionRetryCountAfterFail.Value;
                m_ProductRecipeVisionSettings.S2VisionContinuousFailCountToTriggerAlarm = (int)m_tabpageVisionRecipe.numericUpDownS2VisionContinuousFailCountToTriggerAlarm.Value;

                m_ProductRecipeVisionSettings.S2VisionUnitThetaOffset = (int)m_tabpageVisionRecipe.numericUpDownS2VisionUnitThetaOffset.Value;

                m_ProductRecipeVisionSettings.listS2VisionSnap.Clear();
                foreach (ListViewItem _listViewS2Snap in m_tabpageVisionRecipe.listViewS2Snap.Items)
                {
                    int nNo = -1;
                    bool bEnableSnap = false;
                    if (_listViewS2Snap.SubItems[2].Text.ToUpper() == "YES")//|| _listViewS2Snap.SubItems[2].Text.ToUpper() == "NO")
                    {
                        bEnableSnap = true;
                    }
                    if (Int32.TryParse(_listViewS2Snap.SubItems[0].Text, out nNo) == true
                        )
                    {
                        m_ProductRecipeVisionSettings.listS2VisionSnap.Add(new VisionSnapInfo
                        {
                            SnapNo = nNo,
                            Description = _listViewS2Snap.SubItems[1].Text,
                            EnableSnap = bEnableSnap,
                            VisionXAxisOffset = int.Parse(_listViewS2Snap.SubItems[3].Text),
                            VisionYAxisOffset = int.Parse(_listViewS2Snap.SubItems[4].Text),
                            VisionZAxisOffset = int.Parse(_listViewS2Snap.SubItems[5].Text),
                            VisionThetaAxisOffset = int.Parse(_listViewS2Snap.SubItems[6].Text)
                        });
                    }

                }
                #endregion S2 Parting

                #region S2 Facet
                m_ProductRecipeVisionSettings.S2FacetVisionRetryCountAfterFail = (int)m_tabpageVisionRecipe.numericUpDownS2FacetRetryCountAfterFail.Value;
                m_ProductRecipeVisionSettings.S2FacetVisionContinuousFailCountToTriggerAlarm = (int)m_tabpageVisionRecipe.numericUpDownS2FacetContinuousFailCountToTriggerAlarm.Value;

                m_ProductRecipeVisionSettings.S2FacetVisionUnitThetaOffset = (int)m_tabpageVisionRecipe.numericUpDownS2FacetUnitThetaOffset.Value;
                m_ProductRecipeVisionSettings.EnableS2S3BothSnapping = (bool)m_tabpageVisionRecipe.checkBoxS2S3BothSnap.Checked;
                m_ProductRecipeVisionSettings.listS2FacetVisionSnap.Clear();
                foreach (ListViewItem _listViewS2Snap in m_tabpageVisionRecipe.listViewS2FacetSnap.Items)
                {
                    int nNo = -1;
                    bool bEnableSnap = false;
                    if (_listViewS2Snap.SubItems[2].Text.ToUpper() == "YES")//|| _listViewS2Snap.SubItems[2].Text.ToUpper() == "NO")
                    {
                        bEnableSnap = true;
                    }
                    if (Int32.TryParse(_listViewS2Snap.SubItems[0].Text, out nNo) == true
                        )
                    {
                        m_ProductRecipeVisionSettings.listS2FacetVisionSnap.Add(new VisionSnapInfo
                        {
                            SnapNo = nNo,
                            Description = _listViewS2Snap.SubItems[1].Text,
                            EnableSnap = bEnableSnap,
                            VisionXAxisOffset = int.Parse(_listViewS2Snap.SubItems[3].Text),
                            VisionYAxisOffset = int.Parse(_listViewS2Snap.SubItems[4].Text),
                            VisionZAxisOffset = int.Parse(_listViewS2Snap.SubItems[5].Text),
                            VisionThetaAxisOffset = int.Parse(_listViewS2Snap.SubItems[6].Text)
                        });
                    }

                }
                #endregion S2 Facet

                #region Output Vision
                m_ProductRecipeVisionSettings.EnableOutputVision = m_tabpageVisionRecipe.checkBoxEnableOutputVision.Checked;
                m_ProductRecipeVisionSettings.EnableOutputVision2ndPostAlign = m_tabpageVisionRecipe.checkBoxEnableOutputVision2ndSnap.Checked;
                m_ProductRecipeVisionSettings.OutputVisionRetryCountAfterFail = (int)m_tabpageVisionRecipe.numericUpDownOutputVisionRetryCountAfterFail.Value;
                m_ProductRecipeVisionSettings.OutputVisionContinuousFailCountToTriggerAlarm = (int)m_tabpageVisionRecipe.numericUpDownOutputVisionContinuousFailCountToTriggerAlarm.Value;
                m_ProductRecipeVisionSettings.OutputVisionInspectionCountInCol = (uint)m_tabpageVisionRecipe.numericUpDownOutputVisionInspectionCountInCol.Value;
                m_ProductRecipeVisionSettings.OutputVisionInspectionCountInRow = (uint)m_tabpageVisionRecipe.numericUpDownOutputVisionInspectionCountInRow.Value;

                m_ProductRecipeVisionSettings.OutputVisionUnitThetaOffset = (int)m_tabpageVisionRecipe.numericUpDownOutputVisionUnitThetaOffset.Value;

                m_ProductRecipeVisionSettings.listOutputVisionSnap.Clear();
                foreach (ListViewItem _listViewOutputSnap in m_tabpageVisionRecipe.listViewOutputSnap.Items)
                {
                    int nNo = -1;
                    bool bEnableSnap = false;
                    if (_listViewOutputSnap.SubItems[2].Text.ToUpper() == "YES")//|| _listViewOutputSnap.SubItems[2].Text.ToUpper() == "NO")
                    {
                        bEnableSnap = true;
                    }
                    if (Int32.TryParse(_listViewOutputSnap.SubItems[0].Text, out nNo) == true
                        )
                    {
                        m_ProductRecipeVisionSettings.listOutputVisionSnap.Add(new VisionSnapInfo
                        {
                            SnapNo = nNo,
                            Description = _listViewOutputSnap.SubItems[1].Text,
                            EnableSnap = bEnableSnap,
                            VisionXAxisOffset = int.Parse(_listViewOutputSnap.SubItems[3].Text),
                            VisionYAxisOffset = int.Parse(_listViewOutputSnap.SubItems[4].Text),
                            VisionZAxisOffset = int.Parse(_listViewOutputSnap.SubItems[5].Text),
                            VisionThetaAxisOffset = int.Parse(_listViewOutputSnap.SubItems[6].Text)
                        });
                    }

                }
                #endregion Output Vision

                #endregion Verify Recipe
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        //virtual public bool VerifySortingRecipe()
        //{
        //    try
        //    {
        //        #region Verify Recipe
        //        //m_tabpageSortingRecipe.checkBoxEnableSortingMode.Checked = (bool)m_ProductRecipeSortingSetting.EnableSortingMode;
        //        m_ProductRecipeSortingSetting.EnableSortingMode = (bool)m_tabpageSortingRecipe.checkBoxEnableSortingMode.Checked;
        //        if (m_tabpageSortingRecipe.radioButtonByInputFile.Checked == true)
        //        {
        //            m_ProductRecipeSortingSetting.SortingMode = 0;
        //            m_ProductRecipeSortingSetting.SortingInputFilePath = m_tabpageSortingRecipe.textBoxSortingFilePath.Text;

        //        }
        //        if (m_tabpageSortingRecipe.radioButtonByRecipe.Checked == true)
        //        {
        //            m_ProductRecipeSortingSetting.SortingMode = 1;
        //            SaveRecipe();
        //        }
        //        #endregion Verify Recipe
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //        return false;
        //    }
        //}

        virtual public bool VerifyPickUpHeadRecipe()
        {
            try
            {
                #region Verify Recipe
                m_ProductRecipePickUpHeadSetting.EnablePickSoftlanding = m_tabpagePickUpHeadRecipe.checkBoxEnableSoftLanding.Checked;
                m_ProductRecipePickUpHeadSetting.EnablePlaceSoftlanding = m_tabpagePickUpHeadRecipe.checkBoxEnablePlaceSoftLanding.Checked;

                //m_tabpageSortingRecipe.checkBoxEnableSortingMode.Checked = (bool)m_ProductRecipeSortingSetting.EnableSortingMode;
                m_ProductRecipePickUpHeadSetting.PickUpHead1FlowRate = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead1FlowRate.Value;
                m_ProductRecipePickUpHeadSetting.PickUpHead1Pressure = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead1Pressure.Value;
                m_ProductRecipePickUpHeadSetting.PickUpHead1Force = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead1Force.Value;

                m_ProductRecipePickUpHeadSetting.PickUpHead2FlowRate = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead2FlowRate.Value;
                m_ProductRecipePickUpHeadSetting.PickUpHead2Pressure = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead2Pressure.Value;
                m_ProductRecipePickUpHeadSetting.PickUpHead2Force = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHead2Force.Value;
                
                m_ProductRecipePickUpHeadSetting.PickUpHead1PlaceFlowRate = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace1FlowRate.Value;
                m_ProductRecipePickUpHeadSetting.PickUpHead1PlacePressure = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace1Pressure.Value;
                m_ProductRecipePickUpHeadSetting.PickUpHead1PlaceForce = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace1Force.Value;

                m_ProductRecipePickUpHeadSetting.PickUpHead2PlaceFlowRate = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace2FlowRate.Value;
                m_ProductRecipePickUpHeadSetting.PickUpHead2PlacePressure = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace2Pressure.Value;
                m_ProductRecipePickUpHeadSetting.PickUpHead2PlaceForce = (double)m_tabpagePickUpHeadRecipe.numericUpDownPickUpHeadPlace2Force.Value;

                m_ProductRecipePickUpHeadSetting.EnableSafetyPnPMovePickStation = m_tabpagePickUpHeadRecipe.checkBoxEnableSafetyPnPMovePickStation.Checked;

                #endregion Verify Recipe
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        #region Secsgem
        virtual public void OnSaveMainRecipeDone()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                string FileFullPath = @"D:\Estek\Recipe\Main" + @"\" + groupboxMainRecipeControl.comboBoxRecipeMain.SelectedItem.ToString() + ".xml";
                if (UploadrecipeTask == null || UploadrecipeTask.IsCompleted)
                {
                    UploadrecipeTask = Task.Run(() => UploadRecipeMain(FileFullPath));
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Uploading Previous Recipe to Secgem host, Please try again later");
                }
            }
        }

        virtual public void OnSaveInputRecipeDone()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                string FileFullPath = @"D:\Estek\Recipe\Tray" + @"\" + m_tabpageInput.comboBoxRecipeInput.SelectedItem.ToString() + ".xml";
                if (UploadrecipeTask == null || UploadrecipeTask.IsCompleted)
                {
                    UploadrecipeTask = Task.Run(() => UploadRecipeInput(FileFullPath));
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Uploading Previous Recipe to Secgem host, Please try again later");
                }
            }
        }

        //virtual public void OnSaveSortingRecipeDone()
        //{
        //    if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
        //    {
        //        string FileFullPath = @"D:\Estek\Recipe\Sorting" + @"\" + m_tabpageSortingRecipe.comboBoxRecipeSorting.SelectedItem.ToString() + ".xml";
        //        if (UploadrecipeTask == null || UploadrecipeTask.IsCompleted)
        //        {
        //            UploadrecipeTask = Task.Run(() => UploadRecipeSorting(FileFullPath));
        //        }
        //        else
        //        {
        //            updateRichTextBoxMessageRecipeSorting("Uploading Previous Recipe to Secgem host, Please try again later");
        //        }
        //    }
        //}

        virtual public void OnSavePickUpHeadRecipeDone()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                string FileFullPath = @"D:\Estek\Recipe\PickUpHead" + @"\" + m_tabpagePickUpHeadRecipe.comboBoxRecipePickUpHead.SelectedItem.ToString() + ".xml";
                if (UploadrecipeTask == null || UploadrecipeTask.IsCompleted)
                {
                    UploadrecipeTask = Task.Run(() => UploadRecipePickupHead(FileFullPath));
                }
                else
                {
                    updateRichTextBoxMessageRecipePickUpHead("Uploading Previous Recipe to Secgem host, Please try again later");
                }
            }
        }

        virtual public void OnSaveOutputRecipeDone()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                string FileFullPath = @"D:\Estek\Recipe\Output" + @"\" + m_tabpageOutput.comboBoxRecipeOutput.SelectedItem.ToString() + ".xml";
                if (UploadrecipeTask == null || UploadrecipeTask.IsCompleted)
                {
                    UploadrecipeTask = Task.Run(() => UploadRecipeOutput(FileFullPath));
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Uploading Previous Recipe to Secgem host, Please try again later");
                }
            }
        }

        virtual public void OnSaveDelayRecipeDone()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                string FileFullPath = @"D:\Estek\Recipe\Delay" + @"\" + m_tabpageDelay.comboBoxRecipeDelay.SelectedItem.ToString() + ".xml";
                if (UploadrecipeTask == null || UploadrecipeTask.IsCompleted)
                {
                    UploadrecipeTask = Task.Run(() => UploadRecipeDelay(FileFullPath));
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Uploading Previous Recipe to Secgem host, Please try again later");
                }
            }
        }

        virtual public void OnSaveMotorPositionRecipeDone()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                string FileFullPath = @"D:\Estek\Recipe\MotorPosition" + @"\" + m_tabpageMotorPosition.comboBoxRecipeMotorPosition.SelectedItem.ToString() + ".xml";
                if (UploadrecipeTask == null || UploadrecipeTask.IsCompleted)
                {
                    UploadrecipeTask = Task.Run(() => UploadRecipeMotorPosition(FileFullPath));
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Uploading Previous Recipe to Secgem host, Please try again later");
                }
            }
        }

        virtual public void OnSaveOutputFileRecipeDone()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                string FileFullPath = @"D:\Estek\Recipe\OutputFile" + @"\" + m_tabpageOutputFile.comboBoxRecipeOutputFile.SelectedItem.ToString() + ".xml";
                if (UploadrecipeTask == null || UploadrecipeTask.IsCompleted)
                {
                    UploadrecipeTask = Task.Run(() => UploadRecipeOutputFile(FileFullPath));
                }
                else
                {
                    updateRichTextBoxMessageRecipeInput("Uploading Previous Recipe to Secgem host, Please try again later");
                }
            }
        }

        virtual public void UploadRecipeMain(string FullPath)
        {
            int nError = 0;
            nError = Machine.Platform.SecsgemControl.UploadRecipe(FullPath);
            if (nError != 0)
            {
                updateRichTextBoxMessageRecipeMain("Send Recipe to Server Fail.");
            }
            else
            {
                updateRichTextBoxMessageRecipeMain("Send Recipe to Server Success.");
            }
        }

        virtual public void UploadRecipeInput(string FullPath)
        {
            int nError = 0;
            nError = Machine.Platform.SecsgemControl.UploadRecipe(FullPath);
            if (nError != 0)
            {
                updateRichTextBoxMessageRecipeInput("Send Recipe to Server Fail.");
            }
            else
            {
                updateRichTextBoxMessageRecipeInput("Send Recipe to Server Success.");
            }
        }

        //virtual public void UploadRecipeSorting(string FullPath)
        //{
        //    int nError = 0;
        //    nError = Machine.Platform.SecsgemControl.UploadRecipe(FullPath);
        //    if (nError != 0)
        //    {
        //        updateRichTextBoxMessageRecipeSorting("Send Recipe to Server Fail.");
        //    }
        //    else
        //    {
        //        updateRichTextBoxMessageRecipeSorting("Send Recipe to Server Success.");
        //    }
        //}
        virtual public void UploadRecipePickupHead(string FullPath)
        {
            int nError = 0;
            nError = Machine.Platform.SecsgemControl.UploadRecipe(FullPath);
            if (nError != 0)
            {
                updateRichTextBoxMessageRecipePickUpHead("Send Recipe to Server Fail.");
            }
            else
            {
                updateRichTextBoxMessageRecipePickUpHead("Send Recipe to Server Success.");
            }
        }
        virtual public void UploadRecipeOutput(string FullPath)
        {
            int nError = 0;
            nError = Machine.Platform.SecsgemControl.UploadRecipe(FullPath);
            if (nError != 0)
            {
                updateRichTextBoxMessageRecipeOutput("Send Recipe to Server Fail.");
            }
            else
            {
                updateRichTextBoxMessageRecipeOutput("Send Recipe to Server Success.");
            }
        }

        virtual public void UploadRecipeMotorPosition(string FullPath)
        {
            int nError = 0;
            nError = Machine.Platform.SecsgemControl.UploadRecipe(FullPath);
            if (nError != 0)
            {
                updateRichTextBoxMessageRecipeMotorPosition("Send Recipe to Server Fail.");
            }
            else
            {
                updateRichTextBoxMessageRecipeMotorPosition("Send Recipe to Server Success.");
            }
            //return nError;
        }

        virtual public void UploadRecipeDelay(string FullPath)
        {
            int nError = 0;
            nError = Machine.Platform.SecsgemControl.UploadRecipe(FullPath);
            if (nError != 0)
            {
                //SetShareMemoryGeneralInt("AlarmID", 7002);
                updateRichTextBoxMessageRecipeDelay("Send Recipe to Server Fail.");
            }
            else
            {
                updateRichTextBoxMessageRecipeDelay("Send Recipe to Server Success.");
            }
        }

        virtual public void UploadRecipeOutputFile(string FullPath)
        {
            int nError = 0;
            nError = Machine.Platform.SecsgemControl.UploadRecipe(FullPath);
            if (nError != 0)
            {
                updateRichTextBoxMessageRecipeOutputFile("Send Recipe to Server Fail.");
            }
            else
            {
                updateRichTextBoxMessageRecipeOutputFile("Send Recipe to Server Success.");
            }
        }
        #endregion Secsgem

        virtual public string GetSeparateWordFromCapitalString(string input)
        {
            string strInput = input;
            int nUnderscoreIndex = strInput.IndexOf("_");
            if (nUnderscoreIndex > 0)
            {
                strInput = strInput.Substring(0, nUnderscoreIndex);
            }
            List<int> listOfChar = new List<int>();
            int charNo = 0;
            foreach (char ch in strInput)
            {
                if (char.IsUpper(ch))
                {
                    listOfChar.Add(charNo);
                }
                charNo++;
            }
            listOfChar.Reverse();
            foreach (int no in listOfChar)
            {
                if (no == 0)
                    break;
                strInput = strInput.Insert(no, " ");
            }
            return strInput;
        }

        private void ButtonAddSortingTrayConfiguration_Click(object sender, EventArgs e)
        {
            try
            {
                if(m_tabpageOutput.textBoxDefectCodeName.Text == "")
                {
                    updateRichTextBoxMessageRecipeOutput("Please key in the defect code.");
                    return;
                }
                if(m_tabpageOutput.comboBoxRejectTray.SelectedIndex == -1)
                {
                    updateRichTextBoxMessageRecipeOutput("Please select the reject tray.");
                    return;
                }
                foreach (ListViewItem item in m_tabpageOutput.listViewSortTrayConfiguration.Items)
                {
                    if (item.SubItems[1].Text == m_tabpageOutput.textBoxDefectCodeName.Text)
                    {
                        updateRichTextBoxMessageRecipeVision("Defect Code already exist.");
                        return;
                    }
                }
                string[] strArrayItem = new string[2] { m_tabpageOutput.comboBoxRejectTray.SelectedItem.ToString(), m_tabpageOutput.textBoxDefectCodeName.Text};
                ListViewItem listViewItem = new ListViewItem(strArrayItem);
                m_tabpageOutput.listViewSortTrayConfiguration.Items.Add(listViewItem);
                m_tabpageOutput.listViewSortTrayConfiguration.ListViewItemSorter = new Machine.ListViewComparer(0, SortOrder.Ascending);
                m_tabpageOutput.labelTotalSortingTrayConfiguration.Text = "Total : " + m_tabpageOutput.listViewSortTrayConfiguration.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ButtonRemoveSortTrayConfiguration_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem _listViewItem in m_tabpageOutput.listViewSortTrayConfiguration.SelectedItems)
                {
                    m_tabpageOutput.listViewSortTrayConfiguration.Items.Remove(_listViewItem);
                }
                m_tabpageOutput.labelTotalSortingTrayConfiguration.Text = "Total : " + m_tabpageOutput.listViewSortTrayConfiguration.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ListViewSortTrayConfiguration_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItem = m_tabpageOutput.listViewSortTrayConfiguration.SelectedItems;
                foreach (ListViewItem item in selectedItem)
                {
                    m_tabpageOutput.textBoxDefectCodeName.Text = item.SubItems[1].Text;
                    m_tabpageOutput.comboBoxRejectTray.SelectedIndex = m_tabpageOutput.comboBoxRejectTray.FindStringExact(item.SubItems[0].Text);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
    }
}
