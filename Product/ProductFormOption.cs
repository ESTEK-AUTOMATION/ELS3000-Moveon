using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Common;

namespace Product
{
    public class ProductFormOption : Machine.FormOption
    {
        public tabpageVision m_tabpageVision = new tabpageVision();
        public tabpageServer m_tabpageServer = new tabpageServer();
        public tabpageFilePath m_tabpageFilePath = new tabpageFilePath();
        private ProductRTSSProcess m_ProductRTSSProcess;
        public tabpagePickupHeadtApplication m_tabpagePickupHeadApplication = new tabpagePickupHeadtApplication();
        public tabpageLowYieldSetting m_tabpageLowYieldSetting = new tabpageLowYieldSetting();
        public tabpageAdditionalSystem m_tabpageAdditionalSystem = new tabpageAdditionalSystem();
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductProcessEvent m_ProductProcessEvent;// = new ProductProcessEvent();
        public TabPage tabPageMES = new TabPage();
        public ProductOptionSettings m_ProductOptionSettings = new ProductOptionSettings();
        public tabpageMES m_tabPageMES = new tabpageMES();

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
            set
            {
                m_ProductProcessEvent = value;
                processEvent = m_ProductProcessEvent;
            }
        }

        public ProductOptionSettings productOptionSettings
        {
            set
            {
                m_ProductOptionSettings = value;
                optionSettings = m_ProductOptionSettings;
            }
        }
        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
            }
        }
        public ProductFormOption()
        {
            //base.Load += new System.EventHandler(InitializeGUI);
        }

        override public void Initialize()
        {
            base.Initialize();
            InitializeComponent();
            base.tabPageVision.Controls.Add(m_tabpageVision);
            base.tapePageServer.Controls.Add(m_tabpageServer);
            base.tabPageFilePath.Controls.Add(m_tabpageFilePath);
            base.tabPageTurretApplication.Controls.Add(m_tabpagePickupHeadApplication);
            base.tabPageTurretApplication.Text = "Pickup Head Setting";
            //tabPageTurretApplication.Controls.Add(m_tabpageLowYieldSetting);
            //m_tabpageLowYieldSetting.Location = new System.Drawing.Point(800, 0);
            //m_tabpageLowYieldSetting.BringToFront();

    
            tabPageSystem.Controls.Add(m_tabpageAdditionalSystem);
            m_tabpageAdditionalSystem.Location = new System.Drawing.Point(10, 355);
            m_tabpageAdditionalSystem.BringToFront();
            

            m_tabpageFilePath.btnInVisFileBrowse.Click += new System.EventHandler(this.btnInVisFileBrowse_Click);
            m_tabpageFilePath.btnInBlueFileBrowse.Click += new System.EventHandler(this.btnInBlueFileBrowse_Click);
            m_tabpageFilePath.btnOutvisFileFolder.Click += new System.EventHandler(this.btnOutvisFileFolder_Click);
            m_tabpageFilePath.btnOutBluFileFolder.Click += new System.EventHandler(this.btnOutBluFileFolder_Click);
            m_tabpageFilePath.btnOutPPLogFileFolder.Click += new System.EventHandler(this.btnOutPPLogFileFolder_Click);
            m_tabpageFilePath.btnOutSummaryFileFoldr.Click += new System.EventHandler(this.btnOutSummaryFileFoldr_Click);
            m_tabpageFilePath.btnOutMapFileFolder.Click += new System.EventHandler(this.btnOutMapFileFolder_Click);
            m_tabpageFilePath.buttonVisionRecipePath.Click += new System.EventHandler(this.buttonVisionRecipePath_Click);

            //m_tabpageTurretApplication.comboBoxInputTableCompensationFlipperHeadSelection.SelectedIndex = -1;

            //m_tabpageTurretApplication.comboBoxOutputTableCompensationBondHeadSelection.SelectedIndex = -1;

            
            m_tabpageAdditionalSystem.numericUpDownSoftTrayPresentSensorOffTimeBeforeAlarm_ms.ValueChanged += numericUpDownSoftTrayPresentSensorOffTimeBeforeAlarm_ms_ValueChanged;

            m_tabpageAdditionalSystem.numericUpDownNoOfCycleForPickupHeadOffsetChecking.ValueChanged += NumericUpDownNoOfCycleForPickupHeadOffsetChecking_ValueChanged;
            m_tabpageAdditionalSystem.numericUpDownNoOfCycleToPerformTableOffsetChecking.ValueChanged += NumericUpDownNoOfCycleToPerformTableOffsetChecking_ValueChanged;
            m_tabpageAdditionalSystem.numericUpDownNoOfCycleToPerformAutoHoming.ValueChanged += NumericUpDownNoOfCycleToPerformAutoHoming_ValueChanged;

            m_tabpageAdditionalSystem.buttonPrintTest.Click += new System.EventHandler(this.buttonPrintTest_Click);
            tabPageMES.Controls.Add(m_tabPageMES);
            m_tabPageMES.checkBoxEnableMES.CheckedChanged += new System.EventHandler(this.checkBoxEnableMES_CheckedChanged);
        }


        override public void UpdateGUI()
        {
            base.UpdateGUI();


            m_tabpageVision.checkBoxEnableVision.Checked = m_ProductOptionSettings.EnableVision;
            m_tabpageVision.checkBoxEnableLaunchVisionSoftware.Checked = m_ProductOptionSettings.EnableLaunchVisionSoftware;

            m_tabpageServer.checkBoxEnableOnlineMode.Checked = m_ProductOptionSettings.EnableOnlineMode;
            m_tabpageServer.groupBoxSecsgem.Visible = m_ProductShareVariables.productConfigurationSettings.EnableSecsgem;

            m_tabpagePickupHeadApplication.checkBoxEnablePH1.Checked = m_ProductOptionSettings.EnablePH1;
            m_tabpagePickupHeadApplication.checkBoxEnablePH2.Checked = m_ProductOptionSettings.EnablePH2;
            
            m_tabpagePickupHeadApplication.comboBoxInputTableCompensationFlipperHeadSelection.SelectedIndex = 0;

            m_tabpagePickupHeadApplication.comboBoxOutputTableCompensationBondHeadSelection.SelectedIndex = 0;

            m_tabpageFilePath.textBoxVisionRecipePath.Text = m_ProductOptionSettings.VisionRecipeFolderPath;
            m_tabpageFilePath.textBoxServerVisPath.Text = m_ProductOptionSettings.ServerVisPath;
            m_tabpageFilePath.textBoxLocalOutputPath.Text = m_ProductOptionSettings.LocalOutputPath;
            m_tabpageFilePath.textBoxVisionOutputPath.Text = m_ProductOptionSettings.VisionOutputPath;

            m_tabpageFilePath.checkBoxEnableGenerateOutputFilesInVisionPC.Checked = m_ProductOptionSettings.EnableGenerateOutputFilesInVisionPC;

            m_tabpagePickupHeadApplication.checkBoxEnableRetryPicking.Checked = m_ProductOptionSettings.EnablePickupHeadRetryPickingNo;
            m_tabpagePickupHeadApplication.numericUpDownPickupHeadRetryPicking.Value = m_ProductOptionSettings.PickupHeadRetryPickingNo;

            m_tabpagePickupHeadApplication.checkBoxEnableRetryPlacing.Checked = m_ProductOptionSettings.EnablePickupHeadRetryPlacingNo;
            m_tabpagePickupHeadApplication.numericUpDownPickupHeadRetryPlacing.Value = m_ProductOptionSettings.PickupHeadRetryPlacingNo;

            m_tabpagePickupHeadApplication.numericUpDownPulseWidth.Value = m_ProductOptionSettings.PulseWidth;

            m_tabpagePickupHeadApplication.numericUpDownPH1XOffset.Value = m_ProductOptionSettings.PickUpHeadCompensationXOffset[0];
            m_tabpagePickupHeadApplication.numericUpDownPH1YOffset.Value = m_ProductOptionSettings.PickUpHeadCompensationYOffset[0];

            m_tabpagePickupHeadApplication.numericUpDownPH2XOffset.Value = m_ProductOptionSettings.PickUpHeadCompensationXOffset[1];
            m_tabpagePickupHeadApplication.numericUpDownPH2YOffset.Value = m_ProductOptionSettings.PickUpHeadCompensationYOffset[1];
            

            m_tabpagePickupHeadApplication.numericUpDownPH1HeadXOffset.Value = m_ProductOptionSettings.PickUpHeadHeadCompensationXOffset[0];
            m_tabpagePickupHeadApplication.numericUpDownPH1HeadYOffset.Value = m_ProductOptionSettings.PickUpHeadHeadCompensationYOffset[0];

            m_tabpagePickupHeadApplication.numericUpDownPH2HeadXOffset.Value = m_ProductOptionSettings.PickUpHeadHeadCompensationXOffset[1];
            m_tabpagePickupHeadApplication.numericUpDownPH2HeadYOffset.Value = m_ProductOptionSettings.PickUpHeadHeadCompensationYOffset[1];
            

            m_tabpagePickupHeadApplication.numericUpDownPH1RotationXOffset.Value = m_ProductOptionSettings.PickUpHeadRotationXOffset[0];
            m_tabpagePickupHeadApplication.numericUpDownPH1RotationYOffset.Value = m_ProductOptionSettings.PickUpHeadRotationYOffset[0];

            m_tabpagePickupHeadApplication.numericUpDownPH2RotationXOffset.Value = m_ProductOptionSettings.PickUpHeadRotationXOffset[1];
            m_tabpagePickupHeadApplication.numericUpDownPH2RotationYOffset.Value = m_ProductOptionSettings.PickUpHeadRotationYOffset[1];
            

            m_tabpagePickupHeadApplication.numericUpDownPH1PlacementXOffset.Value = m_ProductOptionSettings.PickUpHeadOutputCompensationXOffset[0];
            m_tabpagePickupHeadApplication.numericUpDownPH2PlacementXOffset.Value = m_ProductOptionSettings.PickUpHeadOutputCompensationXOffset[1];

            m_tabpagePickupHeadApplication.numericUpDownPH1PlacementYOffset.Value = m_ProductOptionSettings.PickUpHeadOutputCompensationYOffset[0];
            m_tabpagePickupHeadApplication.numericUpDownPH2PlacementYOffset.Value = m_ProductOptionSettings.PickUpHeadOutputCompensationYOffset[1];

            m_tabpagePickupHeadApplication.numericUpDownPH1PlacementThetaOffset.Value = m_ProductOptionSettings.PickUpHeadOutputCompensationThetaOffset[0];
            m_tabpagePickupHeadApplication.numericUpDownPH2PlacementThetaOffset.Value = m_ProductOptionSettings.PickUpHeadOutputCompensationThetaOffset[1];

            #region Secsgem
            m_tabpageServer.groupBoxSecsgem.Visible = m_ProductShareVariables.productConfigurationSettings.EnableSecsgem;
            if (m_ProductShareVariables.productOptionSettings.SecsgemMode == 0)
                m_tabpageServer.radioButtonSecsgemOffline.Checked = true;
            else if (m_ProductShareVariables.productOptionSettings.SecsgemMode == 1)
                m_tabpageServer.radioButtonSecsgemOnlineLocal.Checked = true;
            else if (m_ProductShareVariables.productOptionSettings.SecsgemMode == 2)
                m_tabpageServer.radioButtonSecsgemOnlineRemote.Checked = true;
            if (m_ProductShareVariables.productOptionSettings.EnableSpoolingConfig == true)
            {
                m_tabpageServer.radioButtonSpoolConfigEnable.Checked = true;
                m_tabpageServer.radioButtonSpoolConfigDisable.Checked = false;
            }
            else
            {
                m_tabpageServer.radioButtonSpoolConfigEnable.Checked = false;
                m_tabpageServer.radioButtonSpoolConfigDisable.Checked = true;
            }
            if (m_ProductShareVariables.productOptionSettings.EnableSpoolingOverwrite == true)
            {
                m_tabpageServer.radioButtonSpoolOverwriteTrue.Checked = true;
                m_tabpageServer.radioButtonSpoolOverwriteFalse.Checked = false;
            }
            else
            {
                m_tabpageServer.radioButtonSpoolOverwriteTrue.Checked = false;
                m_tabpageServer.radioButtonSpoolOverwriteFalse.Checked = true;
            }

            m_tabpageServer.textBoxSecsgemIPAddress.Text = m_ProductShareVariables.productOptionSettings.SecsgemIPAddress;
            m_tabpageServer.textBoxSecsgemIPAddress.Text = m_ProductShareVariables.productOptionSettings.SecsgemPortNumber.ToString();
            #endregion Secsgem
            m_tabpageVision.checkBoxEnableVisionWaitResult.Checked = m_ProductOptionSettings.EnableVisionWaitResult;

            m_tabpageVision.checkBoxEnableInputVision.Checked = m_ProductOptionSettings.EnableInputVision;
            m_tabpageVision.checkBoxEnableS2Vision.Checked = m_ProductOptionSettings.EnableS2Vision;
            m_tabpageVision.checkBoxEnableSetupVision.Checked = m_ProductOptionSettings.EnableSetupVision;
            m_tabpageVision.checkBoxEnableBottomVision.Checked = m_ProductOptionSettings.EnableBottomVision;
            m_tabpageVision.checkBoxEnableSWLRVision.Checked = m_ProductOptionSettings.EnableSWLRVision;
            m_tabpageVision.checkBoxEnableSWFRVision.Checked = m_ProductOptionSettings.EnableSWFRVision;
            m_tabpageVision.checkBoxEnableS3Vision.Checked = m_ProductOptionSettings.EnableS3Vision;
            m_tabpageVision.checkBoxEnableOutputVision.Checked = m_ProductOptionSettings.EnableOutputVision;

            m_tabpageLowYieldSetting.comboBoxLowYieldItemName.SelectedIndex = 0;
            m_tabpageAdditionalSystem.checkBoxCheckBarcodeID.Checked = m_ProductOptionSettings.bEnableCheckingBarcodeID;
            m_tabpageAdditionalSystem.checkBoxEnableBarcodePrinter.Checked = m_ProductOptionSettings.bEnableBarcodePrinter;
            m_tabpageLowYieldSetting.comboBoxContinuousLowYield.SelectedIndex = 0;

            m_tabpageAdditionalSystem.comboBoxBarcodeRibbonSensor.SelectedIndex = (int)m_ProductOptionSettings.BarcodePrinterRibbonSensor;

            m_tabpageAdditionalSystem.numericUpDownSoftTrayPresentSensorOffTimeBeforeAlarm_ms.Value = m_ProductOptionSettings.TrayPresentSensorOffTimeBeforeAlarm_ms;

            m_tabpageAdditionalSystem.checkBoxEnableInputTableVacuum.Checked = m_ProductOptionSettings.EnableInputTableVacuum;

            m_tabPageMES.checkBoxEnableMES.Checked = m_ProductOptionSettings.EnableMES;

            m_tabPageMES.textBoxInputURL.Text = m_ProductOptionSettings.MESInputURL;
            m_tabPageMES.textBoxOutputURL.Text = m_ProductOptionSettings.MESOutputURL;
            m_tabPageMES.textBoxEndJobURL.Text = m_ProductOptionSettings.MESEndJobURL;
            m_tabPageMES.textBoxMachineNo.Text = m_ProductOptionSettings.MachineNo;
            if(m_ProductOptionSettings.EnableCountDownByInputQuantity == true)
            {
                m_tabPageMES.radioButtonCountByInputUnitQuantity.Checked = true;
                m_tabPageMES.radioButtonCountByInputTrayQuantity.Checked = false;
            }
            else if (m_ProductOptionSettings.EnableCountDownByInputTrayNo == true)
            {
                m_tabPageMES.radioButtonCountByInputUnitQuantity.Checked = false;
                m_tabPageMES.radioButtonCountByInputTrayQuantity.Checked = true;
            }
            //m_tabpageAdditionalSystem.checkBoxEnableGoodSamplingSeq.Checked = m_ProductOptionSettings.EnableGoodSamplingSequence;

            //m_tabpageAdditionalSystem.numericUpDownNoOfCycleForPickupHeadOffsetChecking.Value = m_ProductOptionSettings.NoOfCycleToPerformPickupHeadOffsetChecking;
            //m_tabpageAdditionalSystem.numericUpDownNoOfCycleToPerformTableOffsetChecking.Value = m_ProductOptionSettings.NoOfCycleToPerformTrayTableOffsetChecking;
            //m_tabpageAdditionalSystem.numericUpDownNoOfCycleToPerformAutoHoming.Value = m_ProductOptionSettings.NoOfCycleToPerformAutoHoming;
        }

        override public bool VerifyOption()
        {
            if (base.VerifyOption() == false)
                return false;
            
            #region Verify Option
            m_ProductOptionSettings.EnableVision = m_tabpageVision.checkBoxEnableVision.Checked;
            m_ProductOptionSettings.EnableLaunchVisionSoftware = m_tabpageVision.checkBoxEnableLaunchVisionSoftware.Checked;

            m_ProductOptionSettings.EnableOnlineMode = m_tabpageServer.checkBoxEnableOnlineMode.Checked;

            m_ProductOptionSettings.EnablePH1 = m_tabpagePickupHeadApplication.checkBoxEnablePH1.Checked;
            m_ProductOptionSettings.EnablePH2 = m_tabpagePickupHeadApplication.checkBoxEnablePH2.Checked;
            
            m_ProductOptionSettings.VisionRecipeFolderPath = m_tabpageFilePath.textBoxVisionRecipePath.Text;
            m_ProductOptionSettings.EnableGenerateOutputFilesInVisionPC = m_tabpageFilePath.checkBoxEnableGenerateOutputFilesInVisionPC.Checked;
            m_ProductOptionSettings.ServerVisPath = m_tabpageFilePath.textBoxServerVisPath.Text;
            m_ProductOptionSettings.LocalOutputPath = m_tabpageFilePath.textBoxLocalOutputPath.Text;
            m_ProductOptionSettings.VisionOutputPath = m_tabpageFilePath.textBoxVisionOutputPath.Text;

            m_ProductOptionSettings.EnablePickupHeadRetryPickingNo = m_tabpagePickupHeadApplication.checkBoxEnableRetryPicking.Checked;
            m_ProductOptionSettings.PickupHeadRetryPickingNo = (uint)m_tabpagePickupHeadApplication.numericUpDownPickupHeadRetryPicking.Value;

            m_ProductOptionSettings.PulseWidth = (uint)m_tabpagePickupHeadApplication.numericUpDownPulseWidth.Value;

            m_ProductOptionSettings.PickUpHeadCompensationXOffset[0] = (int)m_tabpagePickupHeadApplication.numericUpDownPH1XOffset.Value;
            m_ProductOptionSettings.PickUpHeadCompensationYOffset[0] = (int)m_tabpagePickupHeadApplication.numericUpDownPH1YOffset.Value;

            m_ProductOptionSettings.PickUpHeadCompensationXOffset[1] = (int)m_tabpagePickupHeadApplication.numericUpDownPH2XOffset.Value;
            m_ProductOptionSettings.PickUpHeadCompensationYOffset[1] = (int)m_tabpagePickupHeadApplication.numericUpDownPH2YOffset.Value;
            //m_ProductOptionSettings.UnitOfMeasurement = double.Parse(m_tabpageVision.textBoxInputVisionUnitOfMeasurement.Text);

            m_ProductOptionSettings.PickUpHeadHeadCompensationXOffset[0] = (int)m_tabpagePickupHeadApplication.numericUpDownPH1HeadXOffset.Value;
            m_ProductOptionSettings.PickUpHeadHeadCompensationYOffset[0] = (int)m_tabpagePickupHeadApplication.numericUpDownPH1HeadYOffset.Value;

            m_ProductOptionSettings.PickUpHeadHeadCompensationXOffset[1] = (int)m_tabpagePickupHeadApplication.numericUpDownPH2HeadXOffset.Value;
            m_ProductOptionSettings.PickUpHeadHeadCompensationYOffset[1] = (int)m_tabpagePickupHeadApplication.numericUpDownPH2HeadYOffset.Value;

            
            m_ProductOptionSettings.PickUpHeadRotationXOffset[0] = (int)m_tabpagePickupHeadApplication.numericUpDownPH1RotationXOffset.Value;
            m_ProductOptionSettings.PickUpHeadRotationYOffset[0] = (int)m_tabpagePickupHeadApplication.numericUpDownPH1RotationYOffset.Value;

            m_ProductOptionSettings.PickUpHeadRotationXOffset[1] = (int)m_tabpagePickupHeadApplication.numericUpDownPH2RotationXOffset.Value;
            m_ProductOptionSettings.PickUpHeadRotationYOffset[1] = (int)m_tabpagePickupHeadApplication.numericUpDownPH2RotationYOffset.Value;

            m_ProductOptionSettings.PickUpHeadOutputCompensationXOffset[0] = (int)m_tabpagePickupHeadApplication.numericUpDownPH1PlacementXOffset.Value;
            m_ProductOptionSettings.PickUpHeadOutputCompensationXOffset[1] = (int)m_tabpagePickupHeadApplication.numericUpDownPH2PlacementXOffset.Value;

            m_ProductOptionSettings.PickUpHeadOutputCompensationYOffset[0] = (int)m_tabpagePickupHeadApplication.numericUpDownPH1PlacementYOffset.Value;
            m_ProductOptionSettings.PickUpHeadOutputCompensationYOffset[1] = (int)m_tabpagePickupHeadApplication.numericUpDownPH2PlacementYOffset.Value;

            m_ProductOptionSettings.PickUpHeadOutputCompensationThetaOffset[0] = (int)m_tabpagePickupHeadApplication.numericUpDownPH1PlacementThetaOffset.Value;
            m_ProductOptionSettings.PickUpHeadOutputCompensationThetaOffset[1] = (int)m_tabpagePickupHeadApplication.numericUpDownPH2PlacementThetaOffset.Value;

            m_ProductOptionSettings.EnableInputVision = m_tabpageVision.checkBoxEnableInputVision.Checked;
            m_ProductOptionSettings.EnableS2Vision = m_tabpageVision.checkBoxEnableS2Vision.Checked;
            m_ProductOptionSettings.EnableSetupVision = m_tabpageVision.checkBoxEnableSetupVision.Checked;
            m_ProductOptionSettings.EnableBottomVision = m_tabpageVision.checkBoxEnableBottomVision.Checked;
            m_ProductOptionSettings.EnableSWLRVision = m_tabpageVision.checkBoxEnableSWLRVision.Checked;
            m_ProductOptionSettings.EnableSWFRVision = m_tabpageVision.checkBoxEnableSWFRVision.Checked;
            m_ProductOptionSettings.EnableS3Vision = m_tabpageVision.checkBoxEnableS3Vision.Checked;
            m_ProductOptionSettings.EnableOutputVision = m_tabpageVision.checkBoxEnableOutputVision.Checked;

            m_ProductOptionSettings.bEnableCheckingBarcodeID = m_tabpageAdditionalSystem.checkBoxCheckBarcodeID.Checked;
            m_ProductOptionSettings.bEnableBarcodePrinter = m_tabpageAdditionalSystem.checkBoxEnableBarcodePrinter.Checked;

            m_ProductOptionSettings.EnableVisionWaitResult = m_tabpageVision.checkBoxEnableVisionWaitResult.Checked;

            m_ProductOptionSettings.EnablePickupHeadRetryPlacingNo = m_tabpagePickupHeadApplication.checkBoxEnableRetryPlacing.Checked;
            m_ProductOptionSettings.PickupHeadRetryPlacingNo = (uint)m_tabpagePickupHeadApplication.numericUpDownPickupHeadRetryPlacing.Value;

            //m_ProductOptionSettings.EnableGoodSamplingSequence = m_tabpageAdditionalSystem.checkBoxEnableGoodSamplingSeq.Checked;

            if (m_tabpageAdditionalSystem.comboBoxBarcodeRibbonSensor.SelectedIndex != -1)
            {
                m_ProductOptionSettings.BarcodePrinterRibbonSensor = (uint)m_tabpageAdditionalSystem.comboBoxBarcodeRibbonSensor.SelectedIndex;
            }
            else
            {
                m_ProductOptionSettings.BarcodePrinterRibbonSensor = (uint)0;
            }

            m_ProductOptionSettings.EnableInputTableVacuum = m_tabpageAdditionalSystem.checkBoxEnableInputTableVacuum.Checked;

            m_ProductOptionSettings.EnableMES = m_tabPageMES.checkBoxEnableMES.Checked;

            m_ProductOptionSettings.MESInputURL = m_tabPageMES.textBoxInputURL.Text;
            m_ProductOptionSettings.MESOutputURL = m_tabPageMES.textBoxOutputURL.Text;
            m_ProductOptionSettings.MESEndJobURL = m_tabPageMES.textBoxEndJobURL.Text;
            m_ProductOptionSettings.MachineNo = m_tabPageMES.textBoxMachineNo.Text;
            if (m_tabPageMES.radioButtonCountByInputUnitQuantity.Checked == true)
            {
                m_ProductOptionSettings.EnableCountDownByInputQuantity = true;
                m_ProductOptionSettings.EnableCountDownByInputTrayNo = false;
            }
            else if(m_tabPageMES.radioButtonCountByInputTrayQuantity.Checked == true)
            {
                m_ProductOptionSettings.EnableCountDownByInputQuantity = false;
                m_ProductOptionSettings.EnableCountDownByInputTrayNo = true;
            }
            #region Secsgem
            if (m_tabpageServer.radioButtonSecsgemOffline.Checked == true)
                m_ProductOptionSettings.SecsgemMode = 0;
            else if (m_tabpageServer.radioButtonSecsgemOnlineLocal.Checked == true)
                m_ProductOptionSettings.SecsgemMode = 1;
            else if (m_tabpageServer.radioButtonSecsgemOnlineRemote.Checked == true)
                m_ProductOptionSettings.SecsgemMode = 2;

            if (m_tabpageServer.radioButtonSpoolConfigEnable.Checked == true)
            {
                m_ProductOptionSettings.EnableSpoolingConfig = true;
                Task SetSpoolingConfigTask = Task.Run(() => SetSpoolingConfig(true));
            }
            else if (m_tabpageServer.radioButtonSpoolConfigDisable.Checked == true)
            {
                m_ProductOptionSettings.EnableSpoolingConfig = false;
                Task SetSpoolingConfigTask = Task.Run(() => SetSpoolingConfig(false));
            }

            if (m_tabpageServer.radioButtonSpoolOverwriteTrue.Checked == true)
            {
                m_ProductOptionSettings.EnableSpoolingOverwrite = true;
                Task SetSpoolingOverwriteTask = Task.Run(() => SetSpoolingOverwrite(true));
            }
            else if (m_tabpageServer.radioButtonSpoolOverwriteFalse.Checked == true)
            {
                m_ProductOptionSettings.EnableSpoolingOverwrite = false;
                Task SetSpoolingOverwriteTask = Task.Run(() => SetSpoolingOverwrite(false));
            }
            
            //IP Address
            //Port no
            #endregion Secsgem
            #endregion
            return true;
        }

        override public bool LoadSettings()
        {
            try
            {
                if (File.Exists(m_strOptionPath + m_strFile))
                {
                    m_ProductOptionSettings = Tools.Deserialize<ProductOptionSettings>(m_strOptionPath + m_strFile);
                    optionSettings = m_ProductOptionSettings;
                }
                else
                {
                    updateRichTextBoxMessage("Option file not exist.");
                    return false;
                }
                if (File.Exists(m_strOptionPath + m_strReportFile))
                {
                    m_ReportSetting = Tools.Deserialize<Machine.ReportSetting>(m_strOptionPath + m_strReportFile);
                }
                else
                {
                    updateRichTextBoxMessage("Report Setting file not exist.");
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

        override public bool SaveOptionSettings()
        {
            try
            {
                Tools.Serialize(m_strOptionPath + m_strFile, m_ProductOptionSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        private void btnInVisFileBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                m_tabpageFilePath.txtVisFileFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private void btnInBlueFileBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                m_tabpageFilePath.txtBluFileFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private void btnOutvisFileFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                m_tabpageFilePath.textBoxServerVisPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private void btnOutBluFileFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                m_tabpageFilePath.txtOutbluFileFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private void btnOutPPLogFileFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                m_tabpageFilePath.textBoxServerPPLotPath.Text = folderBrowserDialog1.SelectedPath;

            }
        }
        private void btnOutSummaryFileFoldr_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                m_tabpageFilePath.textBoxLocalOutputPath.Text = folderBrowserDialog1.SelectedPath;

            }
        }

        private void btnOutMapFileFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                m_tabpageFilePath.textBoxVisionOutputPath.Text = folderBrowserDialog1.SelectedPath;

            }
        }

        private void buttonVisionRecipePath_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                m_tabpageFilePath.textBoxVisionRecipePath.Text = folderBrowserDialog1.SelectedPath;

            }
        }
        private void buttonPrintTest_Click(object sender, EventArgs e)
        {
            //m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_TRIGGER_BARCODE_PRINTER", true);
            m_ProductShareVariables.TemporaryNumber = m_tabpageAdditionalSystem.textBoxTemporaryNumber.Text;
            m_ProductShareVariables.PrintTemporary = true;
        }
        private void checkBoxEnableMES_CheckedChanged(object sender,EventArgs e)
        {

        }
        private void numericUpDownSoftTrayPresentSensorOffTimeBeforeAlarm_ms_ValueChanged(object sender, EventArgs e)
        {
            m_ProductOptionSettings.TrayPresentSensorOffTimeBeforeAlarm_ms = (int)m_tabpageAdditionalSystem.numericUpDownSoftTrayPresentSensorOffTimeBeforeAlarm_ms.Value;
        }
        private void NumericUpDownNoOfCycleForPickupHeadOffsetChecking_ValueChanged(object sender, EventArgs e)
        {
            //m_ProductOptionSettings.NoOfCycleToPerformPickupHeadOffsetChecking = (int)m_tabpageAdditionalSystem.numericUpDownNoOfCycleForPickupHeadOffsetChecking.Value;
        }
        private void NumericUpDownNoOfCycleToPerformTableOffsetChecking_ValueChanged(object sender, EventArgs e)
        {
           // m_ProductOptionSettings.NoOfCycleToPerformTrayTableOffsetChecking = (int)m_tabpageAdditionalSystem.numericUpDownNoOfCycleToPerformTableOffsetChecking.Value;
        }
        private void NumericUpDownNoOfCycleToPerformAutoHoming_ValueChanged(object sender, EventArgs e)
        {
            //m_ProductOptionSettings.NoOfCycleToPerformAutoHoming = (int)m_tabpageAdditionalSystem.numericUpDownNoOfCycleToPerformAutoHoming.Value;
        }

        void SetSpoolingConfig(bool truetoEnable)
        {
            try
            {
                if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
                {
                    Machine.Platform.SecsgemControl.SetSpoolingConfig(truetoEnable);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        void SetSpoolingOverwrite(bool truetoEnable)
        {
            try
            {
                if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
                {
                    Machine.Platform.SecsgemControl.SetSpoolingOverwrite(truetoEnable);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void InitializeComponent()
        {
            this.tabPageMES = new System.Windows.Forms.TabPage();
            this.tabControlOption.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlOption
            // 
            this.tabControlOption.Controls.Add(this.tabPageMES);
            this.tabControlOption.Margin = new System.Windows.Forms.Padding(4);
            this.tabControlOption.Size = new System.Drawing.Size(1210, 599);
            this.tabControlOption.Controls.SetChildIndex(this.tabPageMES, 0);
            this.tabControlOption.Controls.SetChildIndex(this.tabPageTurretApplication, 0);
            this.tabControlOption.Controls.SetChildIndex(this.tabPageFilePath, 0);
            this.tabControlOption.Controls.SetChildIndex(this.tapePageServer, 0);
            this.tabControlOption.Controls.SetChildIndex(this.tabPageSystem, 0);
            this.tabControlOption.Controls.SetChildIndex(this.tabReportSetting, 0);
            this.tabControlOption.Controls.SetChildIndex(this.tabPageVision, 0);
            // 
            // tabPageTurretApplication
            // 
            this.tabPageTurretApplication.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageTurretApplication.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageTurretApplication.Size = new System.Drawing.Size(1202, 562);
            this.tabPageTurretApplication.Text = "Pickup Head Setting";
            // 
            // tapePageServer
            // 
            this.tapePageServer.Margin = new System.Windows.Forms.Padding(2);
            this.tapePageServer.Padding = new System.Windows.Forms.Padding(2);
            this.tapePageServer.Size = new System.Drawing.Size(1532, 708);
            // 
            // tabPageVision
            // 
            this.tabPageVision.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageVision.Size = new System.Drawing.Size(1532, 708);
            // 
            // tabPageFilePath
            // 
            this.tabPageFilePath.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageFilePath.Size = new System.Drawing.Size(1532, 708);
            // 
            // tabReportSetting
            // 
            this.tabReportSetting.Margin = new System.Windows.Forms.Padding(2);
            this.tabReportSetting.Size = new System.Drawing.Size(1532, 708);
            // 
            // tabPageSystem
            // 
            this.tabPageSystem.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageSystem.Size = new System.Drawing.Size(1202, 562);
            // 
            // tabPageMES
            // 
            this.tabPageMES.Location = new System.Drawing.Point(4, 33);
            this.tabPageMES.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageMES.Name = "tabPageMES";
            this.tabPageMES.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageMES.Size = new System.Drawing.Size(1202, 562);
            this.tabPageMES.TabIndex = 6;
            this.tabPageMES.Text = "MES";
            this.tabPageMES.UseVisualStyleBackColor = true;
            // 
            // ProductFormOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.ClientSize = new System.Drawing.Size(1210, 699);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ProductFormOption";
            this.tabControlOption.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
