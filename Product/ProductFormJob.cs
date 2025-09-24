using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using Common;
using Machine;
using System.Text.RegularExpressions;
//using System.Threading;

namespace Product
{
    public class ProductFormJob : Machine.FormJob
    {
        groupboxCustomerInput m_groupboxCustomerInput = new groupboxCustomerInput();
        MainPagePanel m_MainPagePanel = new MainPagePanel();
        SettingsGroupbox m_SettingsGroupbox = new SettingsGroupbox();
        SummaryPanel m_SummaryPanel = new SummaryPanel();
        THKPanel m_THKPanel = new THKPanel();
        tabpageTurret m_tabpageTurret = new tabpageTurret();
        tabpageWaferSelection m_tabpageInputWaferSelection = new tabpageWaferSelection();
        tabpageWaferSelection m_tabpageOutputWaferSelection = new tabpageWaferSelection();
        GUIRightPanel m_GUIRightPanel = new GUIRightPanel();
        MappingDisplay m_MappingDisplay = new MappingDisplay();
        GroupBoxTray m_GroupboxTray = new GroupBoxTray();
        StateDisplay m_StateDisplay = new StateDisplay();
        TrayInformation m_TrayInformation = new TrayInformation();
        SummaryReportDisplay m_SummaryReportDisplay = new SummaryReportDisplay();
        YeildParetoChart m_YeildParetoCharty = new YeildParetoChart();
        AlarmParetoChart m_AlarmParetoCharty = new AlarmParetoChart();

        public TabPage tabPageInputWaferSelection = new TabPage();
        public TabPage tabPageOutputWaferSelection = new TabPage();
        public TabPage tabPageTurret = new TabPage();
        public TabPage tabpageTrayInfo = new TabPage();
        public TabPage tabpageSummaryReport = new TabPage();
        public TabPage tabpageYeildPareto = new TabPage();
        public TabPage tabpageAlarmPareto = new TabPage();

        public ToolStripButton toolStripButtonSetupCameraUpDownControl = new ToolStripButton();
        public ToolStripButton toolStripButtonReTeachMap = new ToolStripButton();
        public ToolStripButton toolStripButtonCycle = new ToolStripButton();
        public ToolStripButton toolStripButtonMaintainanceCount = new ToolStripButton();
        public ToolStripButton toolStripButtonReview = new ToolStripButton();
        public ToolStripButton toolStripButtonCalibration = new ToolStripButton();
        public ToolStripButton toolStripButtonOutputMotionMove = new ToolStripButton();

        public Button buttonContinueLot = new Button();
        public Button buttonEndLot = new Button();

        public ToolStripButton toolStripButtonLowYieldAlarm = new ToolStripButton();

        public ToolStripButton toolStripButtonPusherControl = new ToolStripButton();

        public ToolStripButton toolStripButtonLightingCalibration = new ToolStripButton();

        public ToolStripButton toolStripButtonAutoColletCalibration = new ToolStripButton();
        #region Windows Forms
        public ProductFormConversion m_formConversion;
        //public ProductFormNewLot m_formNewLot;
        public ProductFormBarcode m_formBarcode;
        public ProductFormTeachMap m_formTeachMap;
        public ProductFormConsumableParts m_FormConsumableParts;
        public ProductFormLowYieldParts m_FormLowYieldParts;
        public ProductFormPusherControl m_FormPusherControl;

        public ProductFormOutputAlignment m_FormOutputAlignment;

        public ProductFormAutoForceGaugeCalibration m_FormLightingCalibration;

        public ProductFormAutoColletCalibration m_FormAutoColletCalibration;

        public ProductFormCalibration m_FormCalibration;

        public ProductionOutputMotionMove m_FormOutputMotionMove;
        #endregion Windows Forms

        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductProcessEvent m_ProductProcessEvent;// = new ProductProcessEvent();
        
        private ProductStateControl m_ProductStateControl;// = new ProductStateControl();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess();        
        public ProductAlarmList m_ProductAlarmData;// = new ProductAlarmList();

        private ProductReportProcess m_ProductReportProcess;
        private ProductReportEvent m_ProductReportEvent;

        public ProductPreviousLotInfo m_ProductPreviousLotInfo = new ProductPreviousLotInfo();

        Button m_buttonPurgeHalfWayData = new Button();

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

        public ProductStateControl productStateControl
        {
            set
            {
                m_ProductStateControl = value;
                stateControl = value;
            }
        }

        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
                rtssProcess = value;
            }
        }

        public ProductReportProcess productReportProcess
        {
            set
            {
                m_ProductReportProcess = value;
                reportProcess = value;
            }
        }

        public ProductReportEvent productReportEvent
        {
            set
            {
                m_ProductReportEvent = value;
                
                // = value;
            }
        }

        private Keys keysPrevious;

        #region FrameSelection
        CheckBox[] m_checkBoxInputFrameSelection;
        CheckBox[] m_checkBoxOutputFrameSelection;
        string m_strFrameSelectionRecipe = string.Empty;
        private System.ComponentModel.IContainer components;
        private Timer timerMaintainenceCount = new Timer();
        public bool m_bUpdateFrameSelection = false;
        #endregion

//        #region Mapping
//#if InputXYTable
//        //Total Mapping count value
//        ToolTip t1 = new ToolTip();
//        //Get Top Left Position.
//        int Totol_Count;
//        //Get Top Left Position.
//        int Top_Left_Pos_X;
//        int Top_Left_Pos_Y;
//        //Get Top Right Position.
//        int Top_Right_Pos_X;
//        int Top_Right_Pos_Y;
//        //Get Botton Left Position.
//        int Bottom_Left_Pos_X;
//        int Bottom_Left_Pos_Y;
//        //Get Botton Right Position.
//        int Bottom_Right_Pos_X;
//        int Bottom_Right_Pos_Y;
//        int X_Cordinate = 10;
//        int Y_Cordinate = 10;
//        int Result_Count = 0;
//        int Rect_XY;
//        //Number of Row And Coloum
//        int X_Cell = 65;
//        int Y_Cell = 65;
//        //Magnify Glass
//        /// <summary>
//        /// Stores the zoomfactor of the picZoom picturebox
//        /// </summary>
//        //Store Picture 1 Mouse Location for Zoom In and Zoom Out used
//        int PB1_Image_Mouse_Location_X;
//        int PB1_Image_Mouse_Location_Y;
//        /// <summary>
//        /// Stores the color used to fill any areas not covered by an image
//        /// </summary>
//        private Color _BackColor;
//        int _ZoomFactor = 0;

//        ProductInputOutputFileFormat m_readInfo = new ProductInputOutputFileFormat();

//#endif
//        #endregion Mapping


        #region Form Events
        private void buttonNewLot_Click(object sender, EventArgs e)
        {
            Machine.EventLogger.WriteLog(string.Format("{0} Click New Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));

            //NewLot();
            if (NewLot() == 0)
            {
                m_ProductProcessEvent.PCS_GUI_Launch_NewLot.Set();
                //m_ProductProcessEvent.GUI_PCS_Need_NewLot.Set();
                //m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", 0);
                //m_ProductRTSSProcess.SetProductionInt("nCurrentOutputTrayNo", 0);
                //m_ProductRTSSProcess.SetProductionInt("nCurrentSortingTrayNo", 0);
                //m_ProductRTSSProcess.SetProductionInt("nCurrentRejectTrayNo", 0);
                //m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"));
                //m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentOutputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo"));
                //m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentSortingTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentSortingTrayNo"));
                //m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentRejectTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentRejectTrayNo"));
            }

        }
        private void ButtonEndLot_Click(object sender, EventArgs e)
        {
            Machine.EventLogger.WriteLog(string.Format("{0} Click End Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
            if (MessageBox.Show("Are you sure to perform End Lot", "Confirmation",
                        MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Click Ok  For EndLot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_ENDLOT", true);
                m_ProductRTSSProcess.SetProductionInt("nInputRunningState",0);
                m_ProductRTSSProcess.SetProductionInt("nPNPRunningState", 0);
                m_ProductRTSSProcess.SetProductionInt("nOutputRunningState", 0);
                m_ProductProcessEvent.GUI_PCS_NewLotDone2.Reset();
                if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                {
                    m_ProductRTSSProcess.SetEvent("StartReset", true);
                    System.Threading.Thread.Sleep(500);
                    m_ProductRTSSProcess.SetEvent("StartJob", true);
                }

            }  
        }
        //private void ButtonContinueLot_Click(object sender, EventArgs e)
        //{
        //    try
        //    { 
        //        Machine.EventLogger.WriteLog(string.Format("{0} Click Continue Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
        //        if (MessageBox.Show("Are you sure to perform Continue Lot", "Confirmation",
        //                MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1) == DialogResult.OK)
        //        {
        //            if (File.Exists("..//System//PreviousLotInfo.xml"))
        //            {
        //                m_ProductPreviousLotInfo = Tools.Deserialize<ProductPreviousLotInfo>("..//System//PreviousLotInfo.xml");
        //                m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
        //                m_ProductProcessEvent.GUI_PCS_NewLotDone.Set();
        //            }
        //            else
        //            {
        //                updateRichTextBoxLogDisplay("Previous Lot Info Does Not Exist.");
        //                Machine.EventLogger.WriteLog(string.Format("{0}", "Previous Lot Info Does Not Exist."));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}
        private void textBoxProductPartNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Click Scan New Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (NewLot()==0)
                m_ProductProcessEvent.GUI_PCS_Need_NewLot.Set();
            }
            keysPrevious = e.KeyData;
        }

        #region Top Tool Buttons

        private void toolStripButtonConversion_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Click conversion at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (m_formConversion != null)
                {
                    if (!m_formConversion.IsDisposed)
                    {
                        m_formConversion.Focus();
                        return;
                    }
                }

                m_formConversion = new ProductFormConversion();
                m_formConversion.MdiParent = this;
                m_formConversion.Dock = DockStyle.Fill;
                //m_formConversion.SetParameter(ShareVariables.strRecipeMainPath, ShareVariables.strRecipeExtension);
                //SwitchLanguageFormJob();
                m_formConversion.Show();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void toolStripButtonOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(m_ShareVariables.optionSettings.LocalOutputPath + "\\Summary");
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void toolStripButtonOpenFolder2_Click(object sender, EventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(m_ShareVariables.optionSettings.LocalOutputPath + "\\Output");
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void toolStripButtonOpenFolder3_Click(object sender, EventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(m_ShareVariables.optionSettings.LocalOutputPath + "\\PP Lot");
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void toolStripButtonGrayscaleVerification_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you want to perform Gray Scale verification/calibration.", "Proceed Confirmation",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
                    m_ProductRTSSProcess.SetEvent("StartSetup", true);
                    Machine.EventLogger.WriteLog(string.Format("{0} Click perform Gray Scale verification/calibration at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                }
                else
                {
                    Machine.EventLogger.WriteLog(string.Format("{0} Click Not perform Gray Scale verification/calibration at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void toolStripButtonDotGridsVerification_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you want to perform Dot Grids verification/calibration.", "Proceed Confirmation",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
                    m_ProductRTSSProcess.SetEvent("StartSetup", true);
                    Machine.EventLogger.WriteLog(string.Format("{0} Click perform Dot Grids verification/calibration at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                }
                else
                {
                    Machine.EventLogger.WriteLog(string.Format("{0} Click Not perform Dot Grids verification/calibration at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void toolStripButtonSetupCameraUpDownControl_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolStripButtonSetupCameraUpDownControl.Text == "Setup\rCamera\rDown")
                {
                    //if (MessageBox.Show("Are you want to move setup camera down.", "Proceed Confirmation",
                    //       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_SETUP_CAMERA_DOWN", true);
                        toolStripButtonSetupCameraUpDownControl.Text = "Setup\rCamera\rUp";
                        Machine.EventLogger.WriteLog(string.Format("{0} Click setup camera down at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    }
                    //else
                    //{
                    //    Machine.EventLogger.WriteLog(string.Format("{0} Click Not down setup camera at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    //}
                }
                else if (toolStripButtonSetupCameraUpDownControl.Text == "Setup\rCamera\rUp")
                {
                    //if (MessageBox.Show("Are you want to move setup camera up.", "Proceed Confirmation",
                    //       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_SETUP_CAMERA_UP", true);
                        toolStripButtonSetupCameraUpDownControl.Text = "Setup\rCamera\rDown";
                        Machine.EventLogger.WriteLog(string.Format("{0} Click setup camera up at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    }
                    //else
                    //{
                    //    Machine.EventLogger.WriteLog(string.Format("{0} Click Not up setup camera at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    //}
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void toolStripButtonMaintainanceCount_Click(object sender, EventArgs e)
        {
            if (m_FormConsumableParts != null)
            {
                if (!m_FormConsumableParts.IsDisposed)
                {
                    m_FormConsumableParts.Focus();
                    return;
                }
            }
            CreateFormConsumableParts();
            SetFormConsumableParts();
            SetFormConsumablePartsVariables();
            InitializeConsumableParts();
            m_FormConsumableParts.Show();
        }
        private void ToolStripButtonLowYieldAlarm_Click(object sender, EventArgs e)
        {
            if (m_FormLowYieldParts != null)
            {
                if (!m_FormLowYieldParts.IsDisposed)
                {
                    m_FormLowYieldParts.Focus();
                    return;
                }
            }
            CreateFormLowYieldParts();
            SetFormLowYieldParts();
            SetFormLowYieldPartsVariables();
            InitializeLowYieldParts();
            m_FormLowYieldParts.Show();
        }
        private void ToolStripButtonPusherControl_Click(object sender, EventArgs e)
        {
            if (m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherDown") == false && m_ProductRTSSProcess.GetModuleStatusBool("IsSidewallPusherZAxisPusherUp") == true)
            {
                if (m_FormPusherControl != null)
                {
                    if (!m_FormPusherControl.IsDisposed)
                    {
                        m_FormPusherControl.Focus();
                        return;
                    }
                }
                CreateFormPusherControlParts();
                //SetFormPusherControlParts();
                SetFormPusherControlVariables();
                InitializePusherControlParts();
                m_FormPusherControl.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Sidewall Pusher is at down position.");
            }
        }
        private void ToolStripButtonLightingCalibration_Click(object sender, EventArgs e)
        {
            if (m_FormLightingCalibration != null)
            {
                if (!m_FormLightingCalibration.IsDisposed)
                {
                    m_FormLightingCalibration.Focus();
                    return;
                }
            }
            CreateFormLightingCalibration();
            SetFormLightingCalibrationVariables();
            //InitializeLightingCalibration();
            m_ProductShareVariables.bFormLightingCalibrationEnable = false;
            m_FormLightingCalibration.Show();
        }
        private void ToolStripButtonAutoColletCalibration_Click(object sender, EventArgs e)
        {
            if (m_FormAutoColletCalibration != null)
            {
                if (!m_FormAutoColletCalibration.IsDisposed)
                {
                    m_FormAutoColletCalibration.Focus();
                    return;
                }
            }
            CreateFormLightingCalibration();
            SetFormLightingCalibrationVariables();
            //InitializeLightingCalibration();
            m_ProductShareVariables.bFormLightingCalibrationEnable = false;
            m_FormLightingCalibration.Show();
        }
        private void toolStripButtonReTeachMap_Click(object sender, EventArgs e)
        {
            try
            {
                m_ProductRTSSProcess.SetEvent("RTHD_RMAIN_RETEACH_MAP_ENABLE", false);
                toolStripButtonReTeachMap.Enabled = false;
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_RETEACH_MAP_START", true);
                //if (toolStripButtonSetupCameraUpDownControl.Text == "Setup\rCamera\rDown")
                //{
                //    //if (MessageBox.Show("Are you want to move setup camera down.", "Proceed Confirmation",
                //    //       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                //    {
                //        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_SETUP_CAMERA_DOWN", true);
                //        toolStripButtonSetupCameraUpDownControl.Text = "Setup\rCamera\rUp";
                //        Machine.EventLogger.WriteLog(string.Format("{0} Click setup camera down at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //    }
                //    //else
                //    //{
                //    //    Machine.EventLogger.WriteLog(string.Format("{0} Click Not down setup camera at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //    //}
                //}
                //else if (toolStripButtonSetupCameraUpDownControl.Text == "Setup\rCamera\rUp")
                //{
                //    //if (MessageBox.Show("Are you want to move setup camera up.", "Proceed Confirmation",
                //    //       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                //    {
                //        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_SETUP_CAMERA_UP", true);
                //        toolStripButtonSetupCameraUpDownControl.Text = "Setup\rCamera\rDown";
                //        Machine.EventLogger.WriteLog(string.Format("{0} Click setup camera up at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //    }
                //    //else
                //    //{
                //    //    Machine.EventLogger.WriteLog(string.Format("{0} Click Not up setup camera at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //    //}
                //}
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        
        private void toolStripButtonReview_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Click Review at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (m_ProductShareVariables.bFormProductionReview == false)
                {
                    m_ProductShareVariables.bFormProductionReview = true;
                    toolStripButtonReview.BackColor = System.Drawing.Color.LimeGreen;
                    //m_ProductRTSSProcess.SetEvent("JobStep", true);
                    m_ProductRTSSProcess.SetEvent("ReviewMode", true);
                }
                else
                {
                    m_ProductShareVariables.bFormProductionReview = false;
                    toolStripButtonReview.BackColor = System.Drawing.Color.LightCyan;
                    m_ProductRTSSProcess.SetEvent("ReviewMode", false);
                    //m_ProductRTSSProcess.SetEvent("JobStep", false);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void toolStripButtonCycle_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Click Cycle at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (m_ProductShareVariables.bFormProductionCycle == false)
                {
                    m_ProductShareVariables.bFormProductionCycle = true;
                    toolStripButtonCycle.BackColor = System.Drawing.Color.LimeGreen;
                    //m_ProductRTSSProcess.SetEvent("JobStep", true);
                    m_ProductRTSSProcess.SetEvent("CycleMode", true);
                }
                else
                {
                    m_ProductShareVariables.bFormProductionCycle = false;
                    toolStripButtonCycle.BackColor = System.Drawing.Color.LightCyan;
                    m_ProductRTSSProcess.SetEvent("CycleMode", false);
                    //m_ProductRTSSProcess.SetEvent("JobStep", false);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void toolStripButtonCalibration_Click(object sender, EventArgs e)
        {
            if (m_FormCalibration != null)
            {
                if (!m_FormCalibration.IsDisposed)
                {
                    m_FormCalibration.Focus();
                    return;
                }
            }
            CreateFormCalibration();
            SetFormCalibrationVariables();
            //InitializeLightingCalibration();
            m_ProductShareVariables.bFormCalibrationEnable = false;
            m_FormCalibration.Show();
        }
        private void toolStripButtonOutputMotionMove_Click(object sender, EventArgs e)
        {
            if (m_FormOutputMotionMove != null)
            {
                if (!m_FormOutputMotionMove.IsDisposed)
                {
                    m_FormOutputMotionMove.Focus();
                    return;
                }
            }
            CreateFormOutputMotionMove();
            SetFormOutputMotionMovesVariables();
            m_FormOutputMotionMove.Show();
        }
        #endregion Top Tool Buttons
        private void buttonPurgeHalfWayData_Click(object sender, EventArgs e)
        {
            Machine.EventLogger.WriteLog(string.Format("{0} Click Purge Half Way Data at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
            //m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_OUTPUT_FRAME_UNLOAD_COMPLETE", true);
            m_ProductProcessEvent.GUI_PCS_Force_Generate_Last_Data.Set();
        }
        #endregion Form Event

        #region public
        override public void Initialize()
        {
            //base.m_alarmData = new ProductAlarmList();

            LoadPreviousProductionSetting();
            //m_ProductProcessEvent.PCS_PCS_Send_Vision_NewLot.Set();
            m_ProductProcessEvent.PCS_GUI_Launch_NewLot.Reset();
            m_ProductProcessEvent.GUI_PCS_Need_NewLot.Reset();

            groupBoxUserInput.Controls.Add(m_groupboxCustomerInput);
            tabPageMainPage.Controls.Add(m_MainPagePanel);
            panelRight.Controls.Add(m_GUIRightPanel);
            panelLeft.Controls.Add(m_THKPanel);
            //groupBoxDisplaySetting.Controls.Add(panelLeft);
            m_MappingDisplay.productShareVariables = m_ProductShareVariables;

            m_MainPagePanel.panelScrollable.Controls.Add(m_MappingDisplay);
            //m_MainPagePanel.panelScrollable.Controls.Add(m_GroupboxTray);
            //tabPageTrayInfo.Controls.Add(m_TrayInformation);

            #region JobPage
            tabpageTrayInfo.BackColor = System.Drawing.Color.LightCyan;
            tabpageTrayInfo.Controls.Add(m_TrayInformation);
            tabpageTrayInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabpageTrayInfo.Location = new System.Drawing.Point(4, 22);
            tabpageTrayInfo.Margin = new System.Windows.Forms.Padding(3);
            tabpageTrayInfo.Name = "tabpageTrayInfo";
            tabpageTrayInfo.Text = "Tray Info";

            //tabControlJobPage.Controls.Add(tabpageTrayInfo);


            tabpageSummaryReport.BackColor = System.Drawing.Color.LightCyan;
            tabpageSummaryReport.Controls.Add(m_SummaryReportDisplay);
            tabpageSummaryReport.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabpageSummaryReport.Location = new System.Drawing.Point(4, 22);
            tabpageSummaryReport.Margin = new System.Windows.Forms.Padding(3);
            tabpageSummaryReport.Name = "tabpageSummaryReport";
            tabpageSummaryReport.Text = "Summary Report";

            tabControlJobPage.Controls.Add(tabpageSummaryReport);

            tabpageYeildPareto.BackColor = System.Drawing.Color.LightCyan;
            tabpageYeildPareto.Controls.Add(m_YeildParetoCharty);
            tabpageYeildPareto.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabpageYeildPareto.Location = new System.Drawing.Point(4, 22);
            tabpageYeildPareto.Margin = new System.Windows.Forms.Padding(3);
            tabpageYeildPareto.Name = "tabpageYeildPareto";
            tabpageYeildPareto.Text = "Yield Pareto";

            tabControlJobPage.Controls.Add(tabpageYeildPareto);

            tabpageAlarmPareto.BackColor = System.Drawing.Color.LightCyan;
            tabpageAlarmPareto.Controls.Add(m_AlarmParetoCharty);
            tabpageAlarmPareto.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabpageAlarmPareto.Location = new System.Drawing.Point(4, 22);
            tabpageAlarmPareto.Margin = new System.Windows.Forms.Padding(3);
            tabpageAlarmPareto.Name = "tabpageAlarmPareto";
            tabpageAlarmPareto.Text = "Alarm Pareto";

            tabControlJobPage.Controls.Add(tabpageAlarmPareto);

            m_GUIRightPanel.Dock = DockStyle.Right;
            m_GUIRightPanel.Controls.Add(m_SummaryPanel);
            m_GUIRightPanel.BringToFront();

            groupBoxSummary.Visible = false;
            m_SummaryPanel.Location = new System.Drawing.Point(5, 100);
            m_SummaryPanel.Name = "groupBoxSummary1";
            m_SummaryPanel.Size = new System.Drawing.Size(250, 190);
            m_SummaryPanel.Visible = true;
            m_SummaryPanel.Dock = System.Windows.Forms.DockStyle.Top;
            m_GUIRightPanel.Controls.Add(labelMachineID);
            m_GUIRightPanel.Controls.Add(groupBoxPerformance);
            labelMachineID.Dock = DockStyle.Top;

            m_GUIRightPanel.groupBoxSecsgem.SendToBack();
            m_GUIRightPanel.groupBoxStatus.SendToBack();
            groupBoxPerformance.SendToBack();
            m_SummaryPanel.SendToBack();
            labelMachineID.SendToBack();

           

            toolStripButtonSetupCameraUpDownControl.AutoSize = false;
            toolStripButtonSetupCameraUpDownControl.Enabled = true;
            toolStripButtonSetupCameraUpDownControl.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonSetupCameraUpDownControl.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonSetupCameraUpDownControl.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonSetupCameraUpDownControl.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonSetupCameraUpDownControl.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonSetupCameraUpDownControl.Name = "toolStripButtonSetupCameraUpDownControl";
            toolStripButtonSetupCameraUpDownControl.Size = new System.Drawing.Size(85, 85);
            toolStripButtonSetupCameraUpDownControl.Text = "Setup\rCamera";
            toolStripButtonSetupCameraUpDownControl.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonSetupCameraUpDownControl.Visible = false;

            toolStripButtonMaintainanceCount.AutoSize = false;
            toolStripButtonMaintainanceCount.Enabled = true;
            toolStripButtonMaintainanceCount.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonMaintainanceCount.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonMaintainanceCount.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonMaintainanceCount.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonMaintainanceCount.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonMaintainanceCount.Name = "toolStripButtonMaintainanceCount";
            toolStripButtonMaintainanceCount.Size = new System.Drawing.Size(85, 85);
            toolStripButtonMaintainanceCount.Text = "Main.\rCount";
            toolStripButtonMaintainanceCount.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonMaintainanceCount.Visible = true;

            toolStripButtonLowYieldAlarm.AutoSize = false;
            toolStripButtonLowYieldAlarm.Enabled = true;
            toolStripButtonLowYieldAlarm.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonLowYieldAlarm.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonLowYieldAlarm.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonLowYieldAlarm.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonLowYieldAlarm.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonLowYieldAlarm.Name = "toolStripButtonLowYieldAlarm";
            toolStripButtonLowYieldAlarm.Size = new System.Drawing.Size(85, 85);
            toolStripButtonLowYieldAlarm.Text = "Low Yield\rAlarm";
            toolStripButtonLowYieldAlarm.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonLowYieldAlarm.Visible = false;

            toolStripButtonPusherControl.AutoSize = false;
            toolStripButtonPusherControl.Enabled = true;
            toolStripButtonPusherControl.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonPusherControl.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonPusherControl.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonPusherControl.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonPusherControl.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonPusherControl.Name = "toolStripButtonPusherControl";
            toolStripButtonPusherControl.Size = new System.Drawing.Size(85, 85);
            toolStripButtonPusherControl.Text = "Pusher\rControl";
            toolStripButtonPusherControl.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonPusherControl.Visible = false;

            toolStripButtonLightingCalibration.AutoSize = false;
            toolStripButtonLightingCalibration.Enabled = true;
            toolStripButtonLightingCalibration.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonLightingCalibration.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonLightingCalibration.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonLightingCalibration.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonLightingCalibration.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonLightingCalibration.Name = "toolStripButtonLightingCalibration";
            toolStripButtonLightingCalibration.Size = new System.Drawing.Size(85, 85);
            toolStripButtonLightingCalibration.Text = "ACH\rCalibration";
            toolStripButtonLightingCalibration.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonLightingCalibration.Visible = false;

            toolStripButtonAutoColletCalibration.AutoSize = false;
            toolStripButtonAutoColletCalibration.Enabled = true;
            toolStripButtonAutoColletCalibration.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonAutoColletCalibration.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonAutoColletCalibration.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonAutoColletCalibration.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonAutoColletCalibration.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonAutoColletCalibration.Name = "toolStripButtonAutoColletCalibration";
            toolStripButtonAutoColletCalibration.Size = new System.Drawing.Size(85, 85);
            toolStripButtonAutoColletCalibration.Text = "Collet\rCalibration";
            toolStripButtonAutoColletCalibration.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonAutoColletCalibration.Visible = false;

            toolStripButtonReTeachMap.AutoSize = false;
            toolStripButtonReTeachMap.Enabled = true;
            toolStripButtonReTeachMap.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonReTeachMap.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonReTeachMap.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonReTeachMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonReTeachMap.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonReTeachMap.Name = "toolStripButtonReTeachMap";
            toolStripButtonReTeachMap.Size = new System.Drawing.Size(85, 85);
            toolStripButtonReTeachMap.Text = "ReTeach\nMap";
            toolStripButtonReTeachMap.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonReTeachMap.Visible = false;

            toolStripButtonCycle.AutoSize = false;
            toolStripButtonCycle.Enabled = true;
            toolStripButtonCycle.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonCycle.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonCycle.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonCycle.ImageTransparentColor = System.Drawing.Color.Magenta;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Machine.FormJob));
            toolStripButtonCycle.Image = ((System.Drawing.Image)(resources.GetObject("btnProductionStep.Image")));
            toolStripButtonCycle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            toolStripButtonCycle.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonCycle.Name = "toolStripButtonCycle";
            toolStripButtonCycle.Size = new System.Drawing.Size(85, 85);
            toolStripButtonCycle.Text = "Cycle";
            toolStripButtonCycle.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonCycle.Visible = true;

            toolStripButtonReview.AutoSize = false;
            toolStripButtonReview.Enabled = true;
            toolStripButtonReview.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonReview.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonReview.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonReview.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonReview.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonReview.Name = "toolStripButtonReview";
            toolStripButtonReview.Size = new System.Drawing.Size(85, 85);
            toolStripButtonReview.Text = "Review";
            toolStripButtonReview.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonReview.Visible = true;

            toolStripButtonCalibration.AutoSize = false;
            toolStripButtonCalibration.Enabled = true;
            toolStripButtonCalibration.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonCalibration.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonCalibration.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonCalibration.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonCalibration.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonCalibration.Name = "toolStripButtonCalibration";
            toolStripButtonCalibration.Size = new System.Drawing.Size(85, 85);
            toolStripButtonCalibration.Text = "Calibration";
            toolStripButtonCalibration.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonCalibration.Visible = true;

            toolStripButtonOutputMotionMove.AutoSize = false;
            toolStripButtonOutputMotionMove.Enabled = true;
            toolStripButtonOutputMotionMove.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            toolStripButtonOutputMotionMove.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripButtonOutputMotionMove.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            toolStripButtonOutputMotionMove.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonOutputMotionMove.Margin = new System.Windows.Forms.Padding(5);
            toolStripButtonOutputMotionMove.Name = "toolStripButtonOutputMotionMove";
            toolStripButtonOutputMotionMove.Size = new System.Drawing.Size(85, 85);
            toolStripButtonOutputMotionMove.Text = "Output M.Move";
            toolStripButtonOutputMotionMove.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            toolStripButtonOutputMotionMove.Visible = true;
            //buttonContinueLot.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //buttonContinueLot.Location = new System.Drawing.Point(730, 8);
            //buttonContinueLot.Margin = new System.Windows.Forms.Padding(5);
            //buttonContinueLot.Name = "buttonContinueLot";
            //buttonContinueLot.Size = new System.Drawing.Size(100, 71);
            //buttonContinueLot.TabIndex = 72;
            //buttonContinueLot.Text = "Continue Lot";
            //buttonContinueLot.UseVisualStyleBackColor = true;
            //buttonContinueLot.Visible = true;
            //groupBoxUserInput.Controls.Add(buttonContinueLot);
            //groupBoxUserInput.Controls.SetChildIndex(buttonContinueLot, 0);

            buttonEndLot.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            buttonEndLot.Location = new System.Drawing.Point(730, 8);
            //buttonEndLot.Location = new System.Drawing.Point(840, 8);
            buttonEndLot.Margin = new System.Windows.Forms.Padding(5);
            buttonEndLot.Name = "buttonEndLot";
            buttonEndLot.Size = new System.Drawing.Size(100, 71);
            buttonEndLot.TabIndex = 72;
            buttonEndLot.Text = "End Lot";
            buttonEndLot.UseVisualStyleBackColor = true;
            buttonEndLot.Visible = true;
            groupBoxUserInput.Controls.Add(buttonEndLot);
            groupBoxUserInput.Controls.SetChildIndex(buttonEndLot, 0);

            toolStripBottom.Items.Add(toolStripButtonLightingCalibration);
            toolStripBottom.Items.Add(toolStripButtonAutoColletCalibration);
            toolStripBottom.Items.Add(toolStripButtonMaintainanceCount);
            toolStripBottom.Items.Add(toolStripButtonReview);
            toolStripBottom.Items.Add(toolStripButtonCalibration);
            toolStripBottom.Items.Add(toolStripButtonOutputMotionMove);
            //toolStripBottom.Items.Add(toolStripButtonLowYieldAlarm);

            int nIndex = RightToolStrip.Items.IndexOf(btnProductionStep);

            RightToolStrip.Items.Insert(nIndex, toolStripButtonCycle);
            #endregion
            m_buttonPurgeHalfWayData.Location = new System.Drawing.Point(890, 8);
            m_buttonPurgeHalfWayData.Name = "buttonPurgeHalfWayData";
            m_buttonPurgeHalfWayData.Size = new System.Drawing.Size(78, 66);
            m_buttonPurgeHalfWayData.TabIndex = 200;
            m_buttonPurgeHalfWayData.Text = "Purge Half Way Data";
            m_buttonPurgeHalfWayData.UseVisualStyleBackColor = true;
            m_buttonPurgeHalfWayData.Visible = false;
            //m_buttonPurgeHalfWayData.SendToBack();
            //panelOption.Controls.Add(m_buttonPurgeHalfWayData);
            groupBoxUserInput.Controls.Add(m_buttonPurgeHalfWayData);
            groupBoxUserInput.Controls.SetChildIndex(m_buttonPurgeHalfWayData, 0);

            btnDryRun.Visible = false;

            GenerateFormEvents();

            //m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_CHECK_SETUP_CAMERA_STATUS_START", true);

            timerMaintainenceCount.Enabled = true;
            timerMaintainenceCount.Interval = 1000;
            m_ProductProcessEvent.PCS_GUI_Due_Count_Alarm.Reset();
            m_ProductProcessEvent.PCS_GUI_Warning_Count_Alarm.Reset();
            m_ProductProcessEvent.PCS_GUI_Clean_Count_Alarm.Reset();

            m_THKPanel.Dock = DockStyle.Left;
            m_THKPanel.BringToFront();

            labelUPH.Text = "Throughput :";
            labelCycleTime_ms.Text = "Throughput Cycle Time :\n(ms)";
            labelCycleTime_ms.Size = new System.Drawing.Size(100,40);
            labelCurrentUPHTitle.Location = new System.Drawing.Point(6, 85);
            labelCurrentCycleTimeTitle.Location = new System.Drawing.Point(6, 108);
            labelCurrentUPH.Location = new System.Drawing.Point(192, 22);
            labelCurrentCycleTime_ms.Location = new System.Drawing.Point(192, 42);
            labelCurrentUnitUPHValue.Location = new System.Drawing.Point(192, 85);
            labelCurrentUnitCycleTimeValue.Location = new System.Drawing.Point(192, 108);
            base.Initialize();
            RefreshRecipeListForCalibration();
        }

        override public void InitializeAlarmList()
        {
            if (File.Exists(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strAlarmFile))
            {
                m_ProductAlarmData = Tools.Deserialize<ProductAlarmList>(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strAlarmFile);
                Tools.Serialize(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strAlarmFile, m_ProductAlarmData);
                UpdateAlarmList();
            }
            else
            {
                Tools.Serialize(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strAlarmFile, m_ProductAlarmData);
                m_ProductAlarmData = Tools.Deserialize<ProductAlarmList>(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strAlarmFile);
                UpdateAlarmList();
            }
        }

        override public void UpdateAlarmList()
        {
            FieldInfo[] fields = typeof(ProductAlarmList).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo _fields in fields)
            {
                m_alarmList.Add((Machine.Platform.Alarm)_fields.GetValue(m_ProductAlarmData));
            }
        }

        virtual public void GenerateFormEvents()
        {
            int nError = 0;

            m_groupboxCustomerInput.textBoxProductPartNumber.KeyUp += new System.Windows.Forms.KeyEventHandler(textBoxProductPartNumber_KeyUp);

            //m_MainPagePanel.pictureBoxMapping.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMapping_MouseDown);
            //m_MainPagePanel.pictureBoxMapping.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMapping_MouseUp);

            toolStripButtonConversion.Click += new System.EventHandler(this.toolStripButtonConversion_Click);
            toolStripButtonConversion.Click += new System.EventHandler(this.toolStripButtonConversion_Click);
            toolStripButtonOpenFolder.Click += new System.EventHandler(this.toolStripButtonOpenFolder_Click);
            toolStripButtonGrayscaleVerification.Click += new System.EventHandler(this.toolStripButtonGrayscaleVerification_Click);
            toolStripButtonDotGridsVerification.Click += new System.EventHandler(this.toolStripButtonDotGridsVerification_Click);
            buttonNewLot.Click += new System.EventHandler(this.buttonNewLot_Click);
            buttonNewLot.Visible = true;
            buttonEndLot.Click += ButtonEndLot_Click;
            buttonEndLot.Visible = true;
            //buttonContinueLot.Click += ButtonContinueLot_Click;
            //buttonContinueLot.Visible = true;
            toolStripButtonSetupCameraUpDownControl.Click += new EventHandler(toolStripButtonSetupCameraUpDownControl_Click);

            toolStripButtonMaintainanceCount.Click += new EventHandler(toolStripButtonMaintainanceCount_Click);

            toolStripButtonReview.Click += new EventHandler(toolStripButtonReview_Click);

            toolStripButtonCalibration.Click += new EventHandler(toolStripButtonCalibration_Click);

            toolStripButtonOutputMotionMove.Click += new EventHandler(toolStripButtonOutputMotionMove_Click);

            toolStripButtonLowYieldAlarm.Click += ToolStripButtonLowYieldAlarm_Click;

            toolStripButtonReTeachMap.Click += new EventHandler(toolStripButtonReTeachMap_Click);

            toolStripButtonCycle.Click += new EventHandler(toolStripButtonCycle_Click);

            m_buttonPurgeHalfWayData.Click += new System.EventHandler(buttonPurgeHalfWayData_Click);

            toolStripButtonPusherControl.Click += ToolStripButtonPusherControl_Click;

            toolStripButtonLightingCalibration.Click += ToolStripButtonLightingCalibration_Click;

            toolStripButtonAutoColletCalibration.Click += ToolStripButtonAutoColletCalibration_Click;
            timerMaintainenceCount.Tick += new System.EventHandler(timerMaintainenceCount_Tick);

            m_MappingDisplay.pictureBoxMapping.MouseDown += new System.Windows.Forms.MouseEventHandler(m_MappingDisplay.pictureBoxMapping_MouseDown);
            m_MappingDisplay.pictureBoxMapping.MouseUp += new System.Windows.Forms.MouseEventHandler(m_MappingDisplay.pictureBoxMapping_MouseUp);
        }



        override public void UpdateGUI()
        {
            try
            {
                base.UpdateGUI();

                if (m_ProductShareVariables.bFormProductionCycle == false)
                {
                    toolStripButtonCycle.BackColor = System.Drawing.Color.LightCyan;
                    m_ProductRTSSProcess.SetEvent("CycleMode", false);
                    //m_ProductRTSSProcess.SetEvent("JobStep", false);
                }
                else
                {
                    toolStripButtonCycle.BackColor = System.Drawing.Color.LimeGreen;
                    //m_ProductRTSSProcess.SetEvent("JobStep", true);
                    m_ProductRTSSProcess.SetEvent("CycleMode", true);
                }
                if (m_ProductShareVariables.bFormProductionReview == false)
                {
                    toolStripButtonReview.BackColor = System.Drawing.Color.LightCyan;
                    m_ProductRTSSProcess.SetEvent("ReviewMode", false);
                    //m_ProductRTSSProcess.SetEvent("JobStep", false);
                }
                else
                {
                    toolStripButtonReview.BackColor = System.Drawing.Color.LimeGreen;
                    //m_ProductRTSSProcess.SetEvent("JobStep", true);
                    m_ProductRTSSProcess.SetEvent("ReviewMode", true);
                }
                // m_tabpageTurret.labelOutputUnit.Text = m_ProductRTSSProcess.GetProductionInt("nUnitHasBeenPlacedOnOutputTable").ToString();
                //m_tabpageTurret.labelQuantityInPurgeBin.Text = m_ProductRTSSProcess.GetProductionInt("nUnitHasBeenPlacedInPurgeBucket").ToString();
                // m_tabpageTurret.labelQuantityInFlipperPurgeBin.Text = m_ProductRTSSProcess.GetProductionInt("nUnitHasBeenPlacedInFlipperPurgeBucket").ToString();
                // if(m_ProductRTSSProcess.GetProductionInt("nUnitToBeInspectedOnInputTable") - 1 < 0)
                //     m_tabpageTurret.labelQuantityInspectedOnInputTable.Text = (0).ToString();
                // else
                //     m_tabpageTurret.labelQuantityInspectedOnInputTable.Text = (m_ProductRTSSProcess.GetProductionInt("nUnitToBeInspectedOnInputTable") - 1).ToString();
                //comboBoxStartingFrameOrTile.Items.Clear();
                //for (int i = 1; i < m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot; i++)
                //{
                //    comboBoxStartingFrameOrTile.Items.Add(i.ToString());
                //}
                labelMachineID.Text = m_ProductShareVariables.optionSettings.MachineID;
                if (m_ProductShareVariables.StateMain == Machine.StateControl.InitializeDoneState)
                {
                    m_ProductStateControl.GUIUpdateInInitializeDoneState();
                }
                else if (m_ProductShareVariables.StateMain == Machine.StateControl.IdleDoneState)
                {
                    comboBoxRecipe.SelectedItem = m_ProductShareVariables.currentMainRecipeName;
                    m_ProductStateControl.GUIUpdateInIdleDoneState();
                }
                if (m_ProductShareVariables.StateMain == Machine.StateControl.PauseDoneState)
                {
                    comboBoxRecipe.SelectedItem = m_ProductShareVariables.currentMainRecipeName;
                    m_ProductStateControl.GUIUpdateInPauseDoneState();
                }

                // Tray Selection
                //#region Tray Selection
                ////input tray
                //if (m_ProductShareVariables.productRecipeInputSettings.InputTrayType == "Jedec Tray")
                //{
                //    m_GroupboxTray.UpdateInputTrayImage("Jedec Tray");
                //}
                //else if (m_ProductShareVariables.productRecipeInputSettings.InputTrayType == "Soft Tray")
                //{
                //    m_GroupboxTray.UpdateInputTrayImage("Soft Tray");
                //}
                ////output tray
                //if (m_ProductShareVariables.productRecipeSortingSettings.EnableSortingMode != true)
                //{
                //    if (m_ProductShareVariables.productRecipeInputSettings.OutputTrayType == "Jedec Tray")
                //    {
                //        m_GroupboxTray.UpdateOutputTrayImage("Jedec Tray");
                //    }
                //    else if (m_ProductShareVariables.productRecipeInputSettings.OutputTrayType == "Soft Tray")
                //    {
                //        m_GroupboxTray.UpdateOutputTrayImage("Soft Tray");
                //    }
                //    else if (m_ProductShareVariables.productRecipeInputSettings.OutputTrayType == "Special Carrier")
                //    {
                //        m_GroupboxTray.UpdateOutputTrayImage("Special Carrier");
                //    }
                //}
                //else
                //{//Sorting Table Selection
                //    if (m_ProductShareVariables.productRecipeInputSettings.OutputTrayType == "Jedec Tray")
                //    {

                //        if (m_ProductShareVariables.productRecipeInputSettings.SortingTrayType == "Soft Tray")
                //        {
                //            m_GroupboxTray.updateSortingTray("Jedec Tray", "Soft Tray");
                //        }
                //        else if (m_ProductShareVariables.productRecipeInputSettings.SortingTrayType == "Special Carrier")
                //        {
                //            m_GroupboxTray.updateSortingTray("Jedec Tray", "Special Carrier");
                //        }
                //    }
                //    else if (m_ProductShareVariables.productRecipeInputSettings.OutputTrayType == "Soft Tray")
                //    {


                //        if (m_ProductShareVariables.productRecipeInputSettings.SortingTrayType == "Jedec Tray")
                //        {
                //            m_GroupboxTray.updateSortingTray("Soft Tray", "Jedec Tray");
                //        }
                //        else if (m_ProductShareVariables.productRecipeInputSettings.SortingTrayType == "Special Carrier")
                //        {
                //            m_GroupboxTray.updateSortingTray("Soft Tray", "Special Carrier");
                //        }
                //    }
                //    else if (m_ProductShareVariables.productRecipeInputSettings.OutputTrayType == "Special Carrier")
                //    {

                //        if (m_ProductShareVariables.productRecipeInputSettings.SortingTrayType == "Jedec Tray")
                //        {
                //            m_GroupboxTray.updateSortingTray("Special Carrier", "Jedec Tray");
                //        }
                //        else if (m_ProductShareVariables.productRecipeInputSettings.SortingTrayType == "Soft Tray")
                //        {
                //            m_GroupboxTray.updateSortingTray("Special Carrier", "Soft Tray");
                //        }

                //    }
                //}

                //#endregion
                #region Secsgem
                m_GUIRightPanel.groupBoxSecsgem.Visible = m_ProductShareVariables.productConfigurationSettings.EnableSecsgem;
                if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
                {
                    if (Machine.Platform.SecsgemControl.CommunicationState == true)
                        m_GUIRightPanel.labelCommunicationState.Text = "Communicating";
                    else
                        m_GUIRightPanel.labelCommunicationState.Text = "Not Communicating";
                    if (Machine.Platform.SecsgemControl.ControlState == 0)
                        m_GUIRightPanel.labelControlState.Text = "Offline";
                    else if (Machine.Platform.SecsgemControl.ControlState == 1)
                        m_GUIRightPanel.labelControlState.Text = "Online Local";
                    else if (Machine.Platform.SecsgemControl.ControlState == 2)
                        m_GUIRightPanel.labelControlState.Text = "Online Remote";
                }
                #endregion Secsgem
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public void LoadPreviousProductionSetting()
        {
            try
            {
                m_ProductShareVariables.strucInputProductInfo.LotID = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotID", "Unknown").ToString();
                m_ProductShareVariables.LotID = m_ProductShareVariables.strucInputProductInfo.LotID;
                m_ProductShareVariables.strucInputProductInfo.LotIDOutput = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotIDOutput", "Unknown").ToString();
                m_ProductShareVariables.strucInputProductInfo.LotID2 = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotIDInput2", "Unknown").ToString();
                m_ProductShareVariables.strucInputProductInfo.LotIDOutput2 = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotIDOutput2", "Unknown").ToString();
                m_ProductShareVariables.strucInputProductInfo.WorkOrder = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("WorkOrder", "Unknown").ToString();
                m_ProductShareVariables.strucInputProductInfo.Recipe = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("Recipe", "Unknown").ToString();
                m_ProductShareVariables.strucInputProductInfo.WaferBin = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("WaferBin", "Unknown").ToString();
                m_ProductShareVariables.strucInputProductInfo.PPLot = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("PPLot", "Unknown").ToString();
                m_ProductShareVariables.strucInputProductInfo.OperatorID = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("OperatorID", "Unknown").ToString();
                m_ProductShareVariables.strucInputProductInfo.Shift = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("Shift", "Unknown").ToString();
                m_ProductShareVariables.dtProductionStartTime = DateTime.Parse(m_ProductShareVariables.RegKey.GetJobDataKeyParameter("dtProductionStartTime", DateTime.Now.ToString()).ToString());
                m_ProductShareVariables.strCurrentBarcodeID = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("strCurrentTileID", "Unknown").ToString();

                m_ProductShareVariables.strCurrentBarcodeID2 = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("strCurrentTileID2", "Unknown").ToString();
                m_ProductShareVariables.currentMainRecipeName = m_ProductShareVariables.strucInputProductInfo.Recipe;

                m_ProductShareVariables.PartName = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("PartName", "Unknown").ToString();
                m_ProductShareVariables.PartNumber = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("PartNumber", "Unknown").ToString();
                m_ProductShareVariables.BuildName = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("BuildName", "Unknown").ToString();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void UpdateGUIInBackgroundworker()
        {
            try
            {
                //continue lot button
                if (m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0) == true && buttonNewLot.Text != "Continue Lot")
                {
                    buttonNewLot.Text = "Continue Lot";
                }
                if (m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0) == false && buttonNewLot.Text != "New Lot")
                {
                    buttonNewLot.Text = "New Lot";
                }
                if (m_ProductShareVariables.StateMain == Machine.StateControl.HomingState)
                {
                    if (m_ProductShareVariables.IsStateDisplayChanged == false)
                    {
                        m_StateDisplay.labelStateDisplay.Text = "HOMING IN PROGRESS";
                        m_MainPagePanel.panelScrollable.Controls.Remove(m_MappingDisplay);
                        m_MainPagePanel.panelScrollable.Controls.Add(m_StateDisplay);
                        m_ProductShareVariables.IsStateDisplayChanged = true;
                    }
                }
                if (m_ProductShareVariables.StateMain != Machine.StateControl.HomingState)
                {
                    if (m_ProductShareVariables.IsStateDisplayChanged == true)
                    {
                        m_StateDisplay.labelStateDisplay.Text = "";
                        m_MainPagePanel.panelScrollable.Controls.Remove(m_StateDisplay);
                        m_MainPagePanel.panelScrollable.Controls.Add(m_MappingDisplay);
                        m_ProductShareVariables.IsStateDisplayChanged = false;
                    }
                }
                if(m_ProductProcessEvent.PCS_GUI_SettingChanged.WaitOne(0))
                {
                    m_MainPagePanel.panelLegend.Controls.Clear();
                    Panel panel = new Panel();
                    panel.Height = 40;
                    panel.Width = 200;
                    Label label = new Label();
                    label.Height = 20;
                    label.Width = 150;
                    label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Height = 30;
                    pictureBox.Width = 30;
                    panel.Controls.Add(label);
                    panel.Controls.Add(pictureBox);
                    pictureBox.Location = new System.Drawing.Point(5, 5);
                    label.Location = new System.Drawing.Point(40, 10);

                    m_MainPagePanel.panelLegend.Controls.Add(panel);
                    panel.Dock = DockStyle.Top;
                    label.Text = "Pass";
                    pictureBox.BackColor = Color.Lime;
                    panel.BringToFront();

                    panel = new Panel();
                    panel.Height = 40;
                    panel.Width = 200;
                    label = new Label();
                    label.Height = 20;
                    label.Width = 150;
                    label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    pictureBox = new PictureBox();
                    pictureBox.Height = 30;
                    pictureBox.Width = 30;
                    panel.Controls.Add(label);
                    panel.Controls.Add(pictureBox);
                    pictureBox.Location = new System.Drawing.Point(5, 5);
                    label.Location = new System.Drawing.Point(40, 10);

                    m_MainPagePanel.panelLegend.Controls.Add(panel);
                    panel.Dock = DockStyle.Top;
                    label.Text = "In Progress";
                    pictureBox.BackColor = Color.Yellow;
                    panel.BringToFront();

                    foreach (DefectProperty _defectProperty in m_ProductShareVariables.productRecipeOutputFileSettings.listDefect)
                    {
                        panel = new Panel();
                        panel.Height = 40;
                        panel.Width = 200;
                        label = new Label();
                        label.Height = 20;
                        label.Width = 150;
                        label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        pictureBox = new PictureBox();
                        pictureBox.Height = 30;
                        pictureBox.Width = 30;
                        panel.Controls.Add(label);
                        panel.Controls.Add(pictureBox);
                        pictureBox.Location = new System.Drawing.Point(5, 5);
                        label.Location = new System.Drawing.Point(40, 10);

                        m_MainPagePanel.panelLegend.Controls.Add(panel);
                        panel.Dock = DockStyle.Top;
                        label.Text = _defectProperty.Code;
                        pictureBox.BackColor = ColorTranslator.FromHtml(_defectProperty.ColorInHex);
                        panel.BringToFront();
                    }

                    panel = new Panel();
                    panel.Height = 40;
                    panel.Width = 200;
                    label = new Label();
                    label.Height = 20;
                    label.Width = 150;
                    label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    pictureBox = new PictureBox();
                    pictureBox.Height = 30;
                    pictureBox.Width = 30;
                    panel.Controls.Add(label);
                    panel.Controls.Add(pictureBox);
                    pictureBox.Location = new System.Drawing.Point(5, 5);
                    label.Location = new System.Drawing.Point(40, 10);

                    m_MainPagePanel.panelLegend.Controls.Add(panel);
                    panel.Dock = DockStyle.Top;
                    label.Text = "EP";
                    pictureBox.BackColor = ColorTranslator.FromHtml(m_ProductShareVariables.productRecipeOutputFileSettings.EmptyUnitColorInHex);
                    panel.BringToFront();

                    panel = new Panel();
                    panel.Height = 40;
                    panel.Width = 200;
                    label = new Label();
                    label.Height = 20;
                    label.Width = 150;
                    label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    pictureBox = new PictureBox();
                    pictureBox.Height = 30;
                    pictureBox.Width = 30;
                    panel.Controls.Add(label);
                    panel.Controls.Add(pictureBox);
                    pictureBox.Location = new System.Drawing.Point(5, 5);
                    label.Location = new System.Drawing.Point(40, 10);

                    m_MainPagePanel.panelLegend.Controls.Add(panel);
                    panel.Dock = DockStyle.Top;
                    label.Text = "UP";
                    pictureBox.BackColor = ColorTranslator.FromHtml(m_ProductShareVariables.productRecipeOutputFileSettings.UnitSlantedColorInHex);
                    panel.BringToFront();

                    panel = new Panel();
                    panel.Height = 40;
                    panel.Width = 200;
                    label = new Label();
                    label.Height = 20;
                    label.Width = 150;
                    label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    pictureBox = new PictureBox();
                    pictureBox.Height = 30;
                    pictureBox.Width = 30;
                    panel.Controls.Add(label);
                    panel.Controls.Add(pictureBox);
                    pictureBox.Location = new System.Drawing.Point(5, 5);
                    label.Location = new System.Drawing.Point(40, 10);

                    m_MainPagePanel.panelLegend.Controls.Add(panel);
                    panel.Dock = DockStyle.Top;
                    label.Text = "Fail";
                    pictureBox.BackColor = Color.Red;
                    panel.BringToFront();
                }
                //if(m_ProductShareVariables.StateMain == Machine.StateControl.ProductioningState)
                //{
                //    m_ProductShareVariables.IsSendVisionInfo = true;
                //}
                //if (m_ProductShareVariables.StateMain != Machine.StateControl.ProductioningState)
                //{
                //    m_ProductShareVariables.IsSendVisionInfo = false;
                //}
                //if (m_ProductShareVariables.StateMain == Machine.StateControl.HomeDoneState && m_ProductShareVariables.IsNewLotDone == true)
                //{
                //    btnProductionStart.Enabled = true;
                //}
                //else if (m_ProductShareVariables.StateMain == Machine.StateControl.HomeDoneState && m_ProductShareVariables.IsNewLotDone == false)
                //{
                //    btnProductionStart.Enabled = false;
                //}
                m_ProductShareVariables.nCurrentUnitCycleTime = m_ProductRTSSProcess.GetProductionArray("nTime", 0, "");
                //if (m_ProductShareVariables.bFormProductionProductPartNumberEnable != comboBoxRecipe.Enabled)
                //{
                //    //m_groupboxCustomerInput.textBoxProductPartNumber.Enabled = m_ProductShareVariables.bFormProductionProductPartNumberEnable;
                //    comboBoxRecipe.Enabled = m_ProductShareVariables.bFormProductionProductPartNumberEnable;
                //    textBoxLotID.Enabled = m_ProductShareVariables.bFormProductionProductPartNumberEnable;
                //    textBoxOperatorID.Enabled = m_ProductShareVariables.bFormProductionProductPartNumberEnable;
                //}
                if (m_ProductShareVariables.bFormProductionProductRecipeEnable != comboBoxRecipe.Enabled)
                {
                    //m_groupboxCustomerInput.textBoxProductPartNumber.Enabled = m_ProductShareVariables.bFormProductionProductPartNumberEnable;
                    comboBoxRecipe.Enabled = m_ProductShareVariables.bFormProductionProductRecipeEnable;
                }
                //if (m_ProductShareVariables.bFormProductionProductRecipeEnable != buttonNewLot.Enabled)
                //{
                //    buttonNewLot.Enabled = m_ProductShareVariables.bFormProductionProductRecipeEnable;
                //}
                //if (m_ProductShareVariables.bFormProductionProductRecipeEnable != buttonContinueLot.Enabled)
                //{
                //    buttonContinueLot.Enabled = m_ProductShareVariables.bFormProductionProductRecipeEnable;
                //}

                //button lot button
                if (m_ProductShareVariables.bFormProductionProductButtonLotEnable != buttonNewLot.Enabled)
                {
                    //m_groupboxCustomerInput.textBoxProductPartNumber.Enabled = m_ProductShareVariables.bFormProductionProductPartNumberEnable;
                    //comboBoxRecipe.Enabled = m_ProductShareVariables.bFormProductionProductRecipeEnable;
                    buttonNewLot.Enabled = m_ProductShareVariables.bFormProductionProductButtonLotEnable;
                }

                if (m_ProductShareVariables.bFormProductionProductEndLotEnable != buttonEndLot.Enabled)
                {
                    buttonEndLot.Enabled = m_ProductShareVariables.bFormProductionProductEndLotEnable;
                }
                //if (m_ProductShareVariables.bFormProductionMaintenanceEnable != toolStripButtonDotGridsVerification.Enabled)
                //{
                //    toolStripButtonDotGridsVerification.Enabled = m_ProductShareVariables.bFormProductionMaintenanceEnable;
                //    toolStripButtonGrayscaleVerification.Enabled = m_ProductShareVariables.bFormProductionMaintenanceEnable;
                //}
                if (m_ProductShareVariables.bFormProductionReviewEnable != toolStripButtonReview.Enabled)
                {
                    toolStripButtonReview.Enabled = m_ProductShareVariables.bFormProductionReviewEnable;
                }
                if (m_ProductShareVariables.bFormProductionCycleEnable != toolStripButtonCycle.Enabled)
                {
                    toolStripButtonCycle.Enabled = m_ProductShareVariables.bFormProductionCycleEnable;
                }
                if (m_ProductShareVariables.bFormCalibrationEnable != toolStripButtonCalibration.Enabled)
                {
                    toolStripButtonCalibration.Enabled = m_ProductShareVariables.bFormCalibrationEnable;
                }
                if (m_ProductShareVariables.bFormOutputMotionMoveEnable != toolStripButtonOutputMotionMove.Enabled)
                {
                    toolStripButtonOutputMotionMove.Enabled = m_ProductShareVariables.bFormOutputMotionMoveEnable;
                }
                //if (m_ProductShareVariables.bFormLightingCalibrationEnable != toolStripButtonLightingCalibration.Enabled)
                //{
                //    toolStripButtonLightingCalibration.Enabled = m_ProductShareVariables.bFormLightingCalibrationEnable;
                //}
                //if (m_GroupboxTray.labelInputPickedUnitQuantity.Text != ((int)m_ProductRTSSProcess.GetProductionInt("nUnitPickOnInputTable")).ToString())
                //{
                //    m_GroupboxTray.labelInputPickedUnitQuantity.Text = ((int)m_ProductRTSSProcess.GetProductionInt("nUnitPickOnInputTable")).ToString();
                //}

                //if (m_GroupboxTray.labelInputTrayNo.Text != m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString())
                //{
                //    m_GroupboxTray.labelInputTrayNo.Text = m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString();
                //    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"));
                //}

                if (m_TrayInformation.labelInputQuantity.Text != m_ProductRTSSProcess.GetProductionInt("nCurrentTotalInputUnitDone").ToString())
                {
                    m_TrayInformation.labelInputQuantity.Text = m_ProductRTSSProcess.GetProductionInt("nCurrentTotalInputUnitDone").ToString();
                }
                if (m_TrayInformation.labelCurrentInputQuantity.Text != m_ProductRTSSProcess.GetProductionInt("nCurrentInputUnitOnTray").ToString())
                {
                    m_TrayInformation.labelCurrentInputQuantity.Text = m_ProductRTSSProcess.GetProductionInt("nCurrentInputUnitOnTray").ToString();
                }
                if (m_TrayInformation.labelOutputQuantity.Text != m_ProductRTSSProcess.GetProductionInt("nCurrentTotalUnitDone").ToString())
                {
                    m_TrayInformation.labelOutputQuantity.Text = m_ProductRTSSProcess.GetProductionInt("nCurrentTotalUnitDone").ToString();
                }
                if (m_TrayInformation.labelCurrentOutput.Text != m_ProductRTSSProcess.GetProductionInt("nCurrentOutputUnitOnTray").ToString())
                {
                    m_TrayInformation.labelCurrentOutput.Text = m_ProductRTSSProcess.GetProductionInt("nCurrentOutputUnitOnTray").ToString();
                }
                if (m_TrayInformation.labelRejectTray1Quantity.Text != m_ProductRTSSProcess.GetProductionInt("nCurrentRejectUnitOnTray").ToString())
                {
                    m_TrayInformation.labelRejectTray1Quantity.Text = m_ProductRTSSProcess.GetProductionInt("nCurrentRejectUnitOnTray").ToString();
                }
                if(m_TrayInformation.labelInputLotID.Text != m_ProductShareVariables.strucInputProductInfo.LotID)
                {
                    m_TrayInformation.labelInputLotID.Text = m_ProductShareVariables.strucInputProductInfo.LotID;
                }
                if(m_TrayInformation.labelInputLotQuantity.Text != m_ProductShareVariables.InputLotQuantity[m_ProductShareVariables.nLotIDNumber].ToString())
                {
                    m_TrayInformation.labelInputLotQuantity.Text = m_ProductShareVariables.InputLotQuantity[m_ProductShareVariables.nLotIDNumber].ToString();
                }
                if (m_TrayInformation.labelInputTrayQuantity.Text != m_ProductShareVariables.InputLotTrayQuantity[m_ProductShareVariables.nLotIDNumber].ToString())
                {
                    m_TrayInformation.labelInputTrayQuantity.Text = m_ProductShareVariables.InputLotTrayQuantity[m_ProductShareVariables.nLotIDNumber].ToString();
                }
                if (m_TrayInformation.labelCurrentTrayNo.Text != (m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun")+1).ToString())
                {
                    m_TrayInformation.labelCurrentTrayNo.Text = (m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun") + 1).ToString();
                }
                if (m_THKPanel.labelTHK1PressureValue.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentPressureValue")).ToString())
                {
                    m_THKPanel.labelTHK1PressureValue.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentPressureValue")).ToString();
                }
                if (m_THKPanel.labelTHK1ForceValue.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentForceValue")).ToString())
                {
                    m_THKPanel.labelTHK1ForceValue.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentForceValue")).ToString();
                }
                if (m_THKPanel.labelTHK1FlowRate.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentFlowRate")).ToString())
                {
                    m_THKPanel.labelTHK1FlowRate.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentFlowRate")).ToString();
                }

                if (m_THKPanel.labelTHK2PressureValue.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentPressureValue")).ToString())
                {
                    m_THKPanel.labelTHK2PressureValue.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentPressureValue")).ToString();
                }
                if (m_THKPanel.labelTHK2ForceValue.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentForceValue")).ToString())
                {
                    m_THKPanel.labelTHK2ForceValue.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentForceValue")).ToString();
                }
                if (m_THKPanel.labelTHK2FlowRate.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentFlowRate")).ToString())
                {
                    m_THKPanel.labelTHK2FlowRate.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentFlowRate")).ToString();
                }


                if (m_THKPanel.labelShiftATotalQuantityValue.Text != m_ProductShareVariables.ShiftATotalQuantity.ToString())
                {
                    m_THKPanel.labelShiftATotalQuantityValue.Text = m_ProductShareVariables.ShiftATotalQuantity.ToString();
                }
                if (m_THKPanel.labelShiftATotalDefectValue.Text != m_ProductShareVariables.ShiftATotalDefect.ToString())
                {
                    m_THKPanel.labelShiftATotalDefectValue.Text = m_ProductShareVariables.ShiftATotalDefect.ToString();
                }
                if (m_ProductShareVariables.ShiftATotalQuantity > 0)
                {
                    if (m_THKPanel.labelShiftATotalYieldValue.Text != ((double)(m_ProductShareVariables.ShiftATotalOutput) / (double)(m_ProductShareVariables.ShiftATotalQuantity) * 100.0).ToString("0.00"));
                    {
                        m_THKPanel.labelShiftATotalYieldValue.Text = ((double)(m_ProductShareVariables.ShiftATotalOutput) / (double)(m_ProductShareVariables.ShiftATotalQuantity) * 100.0).ToString("0.00");
                    }
                }
                else
                {
                    if (m_THKPanel.labelShiftATotalYieldValue.Text != "0.00")
                    {
                        m_THKPanel.labelShiftATotalYieldValue.Text ="0.00";
                    }
                }
                if (m_THKPanel.labelShiftBTotalQuantityValue.Text != m_ProductShareVariables.ShiftBTotalQuantity.ToString())
                {
                    m_THKPanel.labelShiftBTotalQuantityValue.Text = m_ProductShareVariables.ShiftBTotalQuantity.ToString();
                }
                if (m_THKPanel.labelShiftBTotalDefectValue.Text != m_ProductShareVariables.ShiftBTotalDefect.ToString())
                {
                    m_THKPanel.labelShiftBTotalDefectValue.Text = m_ProductShareVariables.ShiftBTotalDefect.ToString();
                }
                if (m_ProductShareVariables.ShiftBTotalQuantity > 0)
                {
                    if (m_THKPanel.labelShiftBTotalYieldValue.Text != ((double)(m_ProductShareVariables.ShiftBTotalOutput) / (double)(m_ProductShareVariables.ShiftBTotalQuantity) * 100.0).ToString("0.00"))
                    {
                        m_THKPanel.labelShiftBTotalYieldValue.Text = ((double)(m_ProductShareVariables.ShiftBTotalOutput) / (double)(m_ProductShareVariables.ShiftBTotalQuantity) * 100.0).ToString("0.00");
                    }
                }
                else
                {
                    if (m_THKPanel.labelShiftBTotalYieldValue.Text != "0.00")
                    {
                        m_THKPanel.labelShiftBTotalYieldValue.Text = "0.00";
                    }
                }
                if (m_THKPanel.labelOverallTotalQuantityValue.Text != m_ProductShareVariables.OverallTotalQuantity.ToString())
                {
                    m_THKPanel.labelOverallTotalQuantityValue.Text = m_ProductShareVariables.OverallTotalQuantity.ToString();
                }
                if (m_THKPanel.labelOverallTotalOutputValue.Text != m_ProductShareVariables.OverallTotalOutput.ToString())
                {
                    m_THKPanel.labelOverallTotalOutputValue.Text = m_ProductShareVariables.OverallTotalOutput.ToString();
                }
                if (m_THKPanel.labelOverallTotalDefectValue.Text != m_ProductShareVariables.OverallTotalDefect.ToString())
                {
                    m_THKPanel.labelOverallTotalDefectValue.Text = m_ProductShareVariables.OverallTotalDefect.ToString();
                }
                if (m_ProductShareVariables.OverallTotalQuantity > 0)
                {
                    if (m_THKPanel.labelOverallTotalYieldValue.Text != ((double)(m_ProductShareVariables.OverallTotalOutput) / (double)(m_ProductShareVariables.OverallTotalQuantity) * 100.0).ToString("0.00"))
                    {
                        m_THKPanel.labelOverallTotalYieldValue.Text = ((double)(m_ProductShareVariables.OverallTotalOutput) / (double)(m_ProductShareVariables.OverallTotalQuantity) * 100.0).ToString("0.00");
                    }
                }
                else
                {
                    if (m_THKPanel.labelOverallTotalYieldValue.Text != "0.00")
                    {
                        m_THKPanel.labelOverallTotalYieldValue.Text = "0.00";
                    }
                }
                if (m_ProductShareVariables.strVisionConnection != m_GUIRightPanel.labelVisionConnectionStatus.Text)
                {
                    m_GUIRightPanel.labelVisionConnectionStatus.Text = m_ProductShareVariables.strVisionConnection;
                }
                if (m_ProductShareVariables.strState != m_GUIRightPanel.labelState.Text)
                {
                    m_GUIRightPanel.labelState.Text = m_ProductShareVariables.strState;
                }
                
                if (m_ProductShareVariables.strCurrentBarcodeID != "")
                {
                    if (m_SummaryPanel.labelCurrentBarcodeID.Text != m_ProductShareVariables.strCurrentBarcodeID)
                    {
                        m_SummaryPanel.labelCurrentBarcodeID.Text = m_ProductShareVariables.strCurrentBarcodeID;
                    }
                }
                if (m_ProductShareVariables.strucInputProductInfo.LotIDOutput != m_SummaryPanel.labelCurrentLotID.Text)
                {
                    m_SummaryPanel.labelCurrentLotID.Text = m_ProductShareVariables.strucInputProductInfo.LotIDOutput;
                }
                if (m_ProductShareVariables.strucInputProductInfo.LotIDOutput != m_SummaryPanel.labelLotID2.Text)
                {
                    m_SummaryPanel.labelLotID2.Text = m_ProductShareVariables.strucInputProductInfo.LotIDOutput;
                }
                if (m_ProductShareVariables.strucInputProductInfo.WorkOrder != m_SummaryPanel.labelCurrentWO.Text)
                {
                    m_SummaryPanel.labelCurrentWO.Text = m_ProductShareVariables.strucInputProductInfo.WorkOrder;
                }
                if (m_ProductShareVariables.PartName != m_SummaryPanel.labelCurrentPartName.Text)
                {
                    m_SummaryPanel.labelCurrentPartName.Text = m_ProductShareVariables.PartName;
                }
                if (m_ProductShareVariables.BuildName != m_SummaryPanel.labelCurrentBuild.Text)
                {
                    m_SummaryPanel.labelCurrentBuild.Text = m_ProductShareVariables.BuildName;
                }
                if (m_ProductShareVariables.strucInputProductInfo.OperatorID != m_SummaryPanel.labelCurrentOperatorID.Text)
                {
                    m_SummaryPanel.labelCurrentOperatorID.Text = m_ProductShareVariables.strucInputProductInfo.OperatorID;
                }
                if (m_ProductShareVariables.nUnitNo.ToString() != m_SummaryPanel.labelCurrentUnitQuantity.Text)
                {
                    m_SummaryPanel.labelCurrentUnitQuantity.Text = m_ProductShareVariables.nUnitNo.ToString();
                }
                if(labelCurrentUnitUPHValue.Text != m_ProductShareVariables.Throughput)
                {
                    m_ProductShareVariables.Throughput = labelCurrentUnitUPHValue.Text;
                }
                if (m_ProductProcessEvent.GUI_PCS_NewLotCancel.WaitOne(0) == true)
                {
                    Machine.EventLogger.WriteLog(string.Format("{0} New Lot cancel at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    if (m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0) == false)
                    {
                        m_ProductShareVariables.bFormProductionProductPartNumberEnable = true;
                        m_ProductShareVariables.bFormProductionProductRecipeEnable = true;
                        m_ProductShareVariables.bFormProductionProductButtonLotEnable = true;
                        //textBoxProductPartNumber.Enabled = true;
                        comboBoxRecipe.SelectedIndex = -1;
                    }
                }
                if (m_ProductProcessEvent.GUI_PCS_NewLotDone.WaitOne(0) == true)
                {
                    Machine.EventLogger.WriteLog(string.Format("{0} New Lot done at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));

                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("LotID", m_ProductShareVariables.strucInputProductInfo.LotID);
                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("LotIDOutput", m_ProductShareVariables.strucInputProductInfo.LotIDOutput);

                    //m_ProductShareVariables.RegKey.SetJobDataKeyParameter("LotIDInput2", m_ProductShareVariables.strucInputProductInfo.LotID2);
                    //m_ProductShareVariables.RegKey.SetJobDataKeyParameter("LotIDOutput2", m_ProductShareVariables.strucInputProductInfo.LotIDOutput2);

                    //m_ProductShareVariables.RegKey.SetJobDataKeyParameter("WorkOrder", m_ProductShareVariables.strucInputProductInfo.WorkOrder);
                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("Recipe", m_ProductShareVariables.strucInputProductInfo.Recipe);
                    //m_ProductShareVariables.RegKey.SetJobDataKeyParameter("WaferBin", m_ProductShareVariables.strucInputProductInfo.WaferBin);
                    //m_ProductShareVariables.RegKey.SetJobDataKeyParameter("PPLot", m_ProductShareVariables.strucInputProductInfo.PPLot);
                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("OperatorID", m_ProductShareVariables.strucInputProductInfo.OperatorID);
                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("Shift", m_ProductShareVariables.strucInputProductInfo.Shift);
                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("dtProductionStartTime", m_ProductShareVariables.dtProductionStartTime);

                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("PartName", m_ProductShareVariables.PartName);
                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("PartNumber", m_ProductShareVariables.PartNumber);
                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("BuildName", m_ProductShareVariables.BuildName);
                    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("nLotIDNumber", m_ProductShareVariables.nLotIDNumber);
                    if(m_ProductShareVariables.nLotIDNumber == 0)
                    {
                        m_ProductShareVariables.RegKey.SetJobDataKeyParameter("LotID1", m_ProductShareVariables.strucInputProductInfo.LotID);

                    }
                    else if (m_ProductShareVariables.nLotIDNumber == 1)
                    {
                        m_ProductShareVariables.RegKey.SetJobDataKeyParameter("LotID2", m_ProductShareVariables.strucInputProductInfo.LotID);
                    }
                    else
                    {
                        m_ProductShareVariables.RegKey.SetJobDataKeyParameter("LotID3", m_ProductShareVariables.strucInputProductInfo.LotID);
                    }
                    // Output Quantity (WC)
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("TotalOutputUnitQuantity", ProductRTSSProcess.GetShareMemorySettingUInt("TotalOutputUnitQuantity"));
                    //input lot quantity
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nInputLotQuantity", m_ProductShareVariables.InputLotQuantity[m_ProductShareVariables.nLotIDNumber]);

                }
                if (m_ProductProcessEvent.PCS_GUI_Initial_Map.WaitOne(0))
                {
                    m_SummaryPanel.labelCurrentBarcodeID.Text = "";
                    m_MappingDisplay.panelMapping.Enabled = true;
                    //if ((m_ProductShareVariables.nInputTrayNo % 2) == 1)
                    {
                        m_MappingDisplay.Generate_Map_Image(m_ProductShareVariables.MultipleMappingInfo[m_ProductShareVariables.nInputTrayNo], 0, 1);
                    }
                    //else
                    //{
                    //    m_MappingDisplay.Generate_Map_Image(m_ProductShareVariables.mappingInfo2, 0, 1);
                    //}
                }

                if (m_ProductProcessEvent.PCS_GUI_Update_Map.WaitOne(0))
                {
                    m_MappingDisplay.Update_Map_Image(m_ProductShareVariables.MultipleMappingInfo[m_ProductShareVariables.nInputTrayNo], 0, 1);
                }

                if (m_ProductProcessEvent.PCS_GUI_Update_Summary_Report.WaitOne(0))
                {
                    m_SummaryReportDisplay.LoadDataToDisplay(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotID, 
                                                            "Summary-" + m_ProductShareVariables.strucInputProductInfo.LotID + ".txt");
                }
                if (m_ProductProcessEvent.PCS_GUI_Update_Yeild_Pareto.WaitOne(0))
                {
                    m_YeildParetoCharty.LoadDataToDisplay(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotID,
                                                            "Summary-" + m_ProductShareVariables.strucInputProductInfo.LotID + ".txt");
                }

                if (m_ProductProcessEvent.PCS_GUI_Update_Summary_Report_EndLot.WaitOne(0))
                {
                    m_SummaryReportDisplay.LoadDataToDisplay(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotID,
                                                            "Summary-" + m_ProductShareVariables.strucInputProductInfo.LotID + ".txt");
                }
                if (m_ProductProcessEvent.PCS_GUI_Update_Yeild_Pareto_EndLot.WaitOne(0))
                {
                    m_YeildParetoCharty.LoadDataToDisplay(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotID,
                                                            "Summary-" + m_ProductShareVariables.strucInputProductInfo.LotID  + ".txt");
                }
                if (m_ProductProcessEvent.PCS_GUI_Update_Alarm_Pareto_EndLot.WaitOne(0))
                {
                    m_AlarmParetoCharty.LoadDataToDisplay(m_ProductShareVariables.LotID,m_alarmList);
                }
                if (m_ProductProcessEvent.PCS_GUI_EndLot.WaitOne(0))
                {
                    m_ProductShareVariables.ShiftATotalQuantity = 0;
                    m_ProductShareVariables.ShiftATotalDefect = 0;
                    m_ProductShareVariables.ShiftATotalOutput = 0;
                    m_ProductShareVariables.ShiftBTotalQuantity = 0;
                    m_ProductShareVariables.ShiftBTotalDefect = 0;
                    m_ProductShareVariables.ShiftBTotalOutput = 0;
                    m_ProductShareVariables.OverallTotalQuantity = 0;
                    m_ProductShareVariables.OverallTotalOutput = 0;
                    m_ProductShareVariables.OverallTotalDefect = 0;

                    string strSummaryAllLine = "";
                    string[] strArraySummaryLine;
                    string[] strArraySummaryData;
                    string FolderName = "..\\LotDetail";
                    string FileName = "DetailByShift.txt";

                    TimeSpan ShiftStartTime = new TimeSpan();
                    TimeSpan Shift2StartTime = new TimeSpan();
                    TimeSpan Shift3StartTime = new TimeSpan();
                    TimeSpan Midnight = new TimeSpan();
                    TimeSpan AfterMidnight = new TimeSpan();
                    DateTime ShiftStart = new DateTime();
                    DateTime ShiftEnd = new DateTime();

                    DateTime Shift1Start = new DateTime();
                    DateTime Shift1End = new DateTime();
                    DateTime Shift2Start = new DateTime();
                    DateTime Shift2End = new DateTime();
                    DateTime Shift3Start = new DateTime();
                    DateTime Shift3End = new DateTime();

                    DateTime TimeNow = new DateTime();
                    TimeSpan TimeTemp = new TimeSpan();
                    TimeSpan TimeDiff = new TimeSpan();
                    TimeSpan TimeDiff2 = new TimeSpan();
                    m_ProductShareVariables.LotDetail.Clear();

                    if (m_ProductShareVariables.m_reportSetting.NoOfShift > -1)
                    {
                        ShiftStartTime = new TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1StartHour, m_ProductShareVariables.m_reportSetting.Shift1StartMinutes, 0);
                        if (m_ProductShareVariables.m_reportSetting.NoOfShift > 0)
                        {
                            Shift2StartTime = new TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2StartHour, m_ProductShareVariables.m_reportSetting.Shift2StartMinutes, 0);
                        }
                        if (m_ProductShareVariables.m_reportSetting.NoOfShift > 1)
                        {
                            Shift3StartTime = new TimeSpan(m_ProductShareVariables.m_reportSetting.Shift3StartHour, m_ProductShareVariables.m_reportSetting.Shift3StartMinutes, 0);
                        }
                        Midnight = new TimeSpan(23, 59, 59);
                        AfterMidnight = new TimeSpan(0, 0, 0);


                        TimeNow = DateTime.Now;
                        if (TimeNow.TimeOfDay < Midnight && TimeNow.TimeOfDay > ShiftStartTime)
                        {
                            ShiftStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_ProductShareVariables.m_reportSetting.Shift1StartHour, m_ProductShareVariables.m_reportSetting.Shift1StartMinutes, 0);
                            
                            Shift1Start = ShiftStart;

                            if (m_ProductShareVariables.m_reportSetting.NoOfShift == 0)
                            {
                                ShiftEnd = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0));
                                Shift1End = ShiftEnd;
                            }
                            else if (m_ProductShareVariables.m_reportSetting.NoOfShift == 1)
                            {
                                TimeTemp = new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1StartHour + m_ProductShareVariables.m_reportSetting.Shift1OperationHour,0,0);
                                TimeDiff = new System.TimeSpan(Shift2StartTime.Hours - TimeTemp.Hours, 0, 0);
                                ShiftEnd = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff
                                    + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0));

                                Shift1End = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0));
                                Shift2Start = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff);
                                Shift2End = ShiftEnd;
                            }
                            else if (m_ProductShareVariables.m_reportSetting.NoOfShift == 2)
                            {
                                TimeTemp = new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1StartHour + m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0);
                                TimeDiff = new System.TimeSpan(Shift2StartTime.Hours - TimeTemp.Hours, 0, 0);
                                TimeTemp = new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2StartHour + m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0);
                                TimeDiff2 = new System.TimeSpan(Shift3StartTime.Hours - TimeTemp.Hours, 0, 0);
                                ShiftEnd = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff
                                    + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0) + TimeDiff2
                                    + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift3OperationHour, 0, 0));
                                
                                Shift1End = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0));
                                Shift2Start = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff);
                                Shift2End = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff
                                    + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0));
                                Shift3Start = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff
                                    + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0) + TimeDiff2);
                                Shift3End = ShiftEnd;
                            }
                        }
                        else
                        {
                            if (TimeNow.TimeOfDay >= AfterMidnight && TimeNow.TimeOfDay < ShiftStartTime)
                            {
                                ShiftStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, m_ProductShareVariables.m_reportSetting.Shift1StartHour, m_ProductShareVariables.m_reportSetting.Shift1StartMinutes, 0);
                            }
                            else
                            {
                                ShiftStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_ProductShareVariables.m_reportSetting.Shift1OperationHour, m_ProductShareVariables.m_reportSetting.Shift1StartMinutes, 0);
                            }
                            Shift1Start = ShiftStart;
                            if (m_ProductShareVariables.m_reportSetting.NoOfShift == 0)
                            {
                                ShiftEnd = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0));
                                Shift1End = ShiftEnd;
                            }
                            else if (m_ProductShareVariables.m_reportSetting.NoOfShift == 1)
                            {
                                TimeTemp = new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1StartHour + m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0);
                                TimeDiff = new System.TimeSpan(Shift2StartTime.Hours - TimeTemp.Hours, 0, 0);
                                ShiftEnd = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff
                                + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0));

                                Shift2Start = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff);
                                Shift1End = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0));
                                Shift2End = ShiftEnd;
                            }
                            else if (m_ProductShareVariables.m_reportSetting.NoOfShift == 2)
                            {
                                Shift3Start = ShiftStart;
                                TimeTemp = new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1StartHour + m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0);
                                TimeDiff = new System.TimeSpan(Shift2StartTime.Hours - TimeTemp.Hours, 0, 0);
                                TimeTemp = new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2StartHour + m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0);
                                TimeDiff2 = new System.TimeSpan(Shift3StartTime.Hours - TimeTemp.Hours, 0, 0);
                                ShiftEnd = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff
                                    + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0) + TimeDiff2
                                    + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift3OperationHour, 0, 0));

                                Shift1End = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0));
                                Shift2Start = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff);
                                Shift2End = ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff
                                    + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0));
                                Shift3Start= ShiftStart.Add(new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift1OperationHour, 0, 0) + TimeDiff
                                    + new System.TimeSpan(m_ProductShareVariables.m_reportSetting.Shift2OperationHour, 0, 0) + TimeDiff2);
                                Shift3End = ShiftEnd;
                            }
                        }

                        if (Directory.Exists(FolderName) == false)
                        {
                            Directory.CreateDirectory(FolderName);
                        }
                        if (File.Exists(FolderName + "\\" + FileName))
                        {
                            using (StreamReader srSummary = new StreamReader(FolderName + "\\" + FileName))
                            {
                                strSummaryAllLine = srSummary.ReadToEnd();
                                if (strSummaryAllLine != "" || strSummaryAllLine != null)
                                {
                                    strArraySummaryLine = Regex.Split(strSummaryAllLine, "\r\n");
                                    foreach (string _strSummaryLine in strArraySummaryLine)
                                    {
                                        strArraySummaryData = _strSummaryLine.Split('\t');
                                        if (strArraySummaryData.Length > 3)
                                        {
                                            DateTime CurrentTime = Convert.ToDateTime(strArraySummaryData[0]);

                                            if (CurrentTime > ShiftStart && CurrentTime < ShiftEnd)
                                            {
                                                m_ProductShareVariables.LotDetail.Add(new ProductShareVariables.DetailInfo
                                                {
                                                    TimeTaken = Convert.ToDateTime(strArraySummaryData[0]),
                                                    TotalQuantity = int.Parse(strArraySummaryData[1]),
                                                    TotalOutput = int.Parse(strArraySummaryData[2]),
                                                    TotalDefect = int.Parse(strArraySummaryData[3]),
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        m_ProductShareVariables.LotDetail.Add(new ProductShareVariables.DetailInfo
                        {
                            TimeTaken = DateTime.Now,
                            TotalQuantity = m_ProductShareVariables.TotalInputQuantityByTray,
                            TotalOutput = m_ProductShareVariables.TotalOutputQuantityByTray,
                            TotalDefect = m_ProductShareVariables.TotalDefectQuantityByTray
                        });
                        using (StreamWriter writer = new StreamWriter(FolderName + "\\" + FileName))
                        {
                            foreach (var m_LotDetail in m_ProductShareVariables.LotDetail.ToList())
                            {
                                if (m_LotDetail.TimeTaken >= Shift1Start
                                    && m_LotDetail.TimeTaken < Shift1End)
                                {
                                    m_ProductShareVariables.ShiftATotalQuantity += m_LotDetail.TotalQuantity;
                                    m_ProductShareVariables.ShiftATotalDefect += m_LotDetail.TotalDefect;
                                    m_ProductShareVariables.ShiftATotalOutput += m_LotDetail.TotalOutput;
                                }
                                else if (m_ProductShareVariables.m_reportSetting.NoOfShift > 0
                                    && m_LotDetail.TimeTaken >= Shift2Start
                                    && m_LotDetail.TimeTaken < Shift2End)
                                {
                                    m_ProductShareVariables.ShiftBTotalQuantity += m_LotDetail.TotalQuantity;
                                    m_ProductShareVariables.ShiftBTotalDefect += m_LotDetail.TotalDefect;
                                    m_ProductShareVariables.ShiftBTotalOutput += m_LotDetail.TotalOutput;
                                }
                                else if (m_ProductShareVariables.m_reportSetting.NoOfShift > 1
                                   && m_LotDetail.TimeTaken >= Shift3Start
                                   && m_LotDetail.TimeTaken < Shift3End)
                                {
                                    m_ProductShareVariables.ShiftCTotalQuantity += m_LotDetail.TotalQuantity;
                                    m_ProductShareVariables.ShiftCTotalDefect += m_LotDetail.TotalDefect;
                                    m_ProductShareVariables.ShiftCTotalOutput += m_LotDetail.TotalOutput;
                                }

                                m_ProductShareVariables.OverallTotalQuantity += m_LotDetail.TotalQuantity;
                                m_ProductShareVariables.OverallTotalOutput += m_LotDetail.TotalOutput;
                                m_ProductShareVariables.OverallTotalDefect += m_LotDetail.TotalDefect;

                                writer.WriteLine(m_LotDetail.TimeTaken.ToString() + "\t" + m_LotDetail.TotalQuantity.ToString()
                                    + "\t" + m_LotDetail.TotalOutput.ToString() + "\t" + m_LotDetail.TotalDefect.ToString());
                            }
                        }
                    }
                    //labelLotID.Text = "-";
                    //m_groupboxCustomerInput.textBoxProductPartNumber.Text = "";
                    if (m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0) == false)
                    {
                        comboBoxRecipe.SelectedIndex = -1;
                        m_ProductShareVariables.bFormProductionProductPartNumberEnable = true;
                        m_ProductShareVariables.bFormProductionProductRecipeEnable = true;
                        m_ProductShareVariables.bFormProductionProductButtonLotEnable = true;
                    }
                    //textBoxProductPartNumber.Enabled = true;
                    //comboBoxRecipe.SelectedIndex = -1;
                }
                if (m_ProductProcessEvent.PCS_GUI_Launch_NewLot.WaitOne(0))
                {
                    LaunchNewLotForm();
                }
                if (m_ProductProcessEvent.PCS_GUI_Close_Remaining_Active_Form.WaitOne(0))
                {
                    if (m_formTeachMap != null)
                    {
                        if (!m_formTeachMap.IsDisposed)
                        {
                            m_formTeachMap.ExitForm = true;
                        }
                    }
                    if (m_formBarcode != null)
                    {
                        if (!m_formBarcode.IsDisposed)
                        {
                            m_formBarcode.ExitForm = true;
                        }
                    }
                }
                             
                //if (m_tabpageTurret.labelFH0.Text != string.Format("{0:0.0}", RoundTo4(GetFHIndex(m_ProductRTSSProcess.GetProductionInt("nCurrentFlipperThetaCycle")))))
                //    m_tabpageTurret.labelFH0.Text = string.Format("{0:0.0}", RoundTo4(GetFHIndex(m_ProductRTSSProcess.GetProductionInt("nCurrentFlipperThetaCycle"))));
                //if(m_tabpageTurret.labelFH1.Text != string.Format("{0:0.0}", RoundTo4(GetFHIndex(m_ProductRTSSProcess.GetProductionInt("nCurrentFlipperThetaCycle")) + 1)))
                //    m_tabpageTurret.labelFH1.Text = string.Format("{0:0.0}", RoundTo4(GetFHIndex(m_ProductRTSSProcess.GetProductionInt("nCurrentFlipperThetaCycle")) + 1));
                //if(m_tabpageTurret.labelFH2.Text != string.Format("{0:0.0}", RoundTo4(GetFHIndex(m_ProductRTSSProcess.GetProductionInt("nCurrentFlipperThetaCycle")) + 2)))
                //    m_tabpageTurret.labelFH2.Text = string.Format("{0:0.0}", RoundTo4(GetFHIndex(m_ProductRTSSProcess.GetProductionInt("nCurrentFlipperThetaCycle")) + 2));
                //if(m_tabpageTurret.labelFH3.Text != string.Format("{0:0.0}", RoundTo4(GetFHIndex(m_ProductRTSSProcess.GetProductionInt("nCurrentFlipperThetaCycle")) + 3)))
                //    m_tabpageTurret.labelFH3.Text = string.Format("{0:0.0}", RoundTo4(GetFHIndex(m_ProductRTSSProcess.GetProductionInt("nCurrentFlipperThetaCycle")) + 3));

                //if (m_ProductShareVariables.TurretStationResult[0].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 0, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[0].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 0, "UnitPresent");
                //    m_tabpageTurret.labelStation0.Text = m_ProductShareVariables.TurretStationResult[0].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[1].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 1, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[1].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 1, "UnitPresent");
                //    m_tabpageTurret.labelStation1.Text = m_ProductShareVariables.TurretStationResult[1].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[2].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 2, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[2].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 2, "UnitPresent");
                //    m_tabpageTurret.labelStation2.Text = m_ProductShareVariables.TurretStationResult[2].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[3].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 3, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[3].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 3, "UnitPresent");
                //    m_tabpageTurret.labelStation3.Text = m_ProductShareVariables.TurretStationResult[3].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[4].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 4, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[4].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 4, "UnitPresent");
                //    m_tabpageTurret.labelStation4.Text = m_ProductShareVariables.TurretStationResult[4].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[5].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 5, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[5].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 5, "UnitPresent");
                //    m_tabpageTurret.labelStation5.Text = m_ProductShareVariables.TurretStationResult[5].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[6].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 6, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[6].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 6, "UnitPresent");
                //    m_tabpageTurret.labelStation6.Text = m_ProductShareVariables.TurretStationResult[6].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[7].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 7, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[7].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 7, "UnitPresent");
                //    m_tabpageTurret.labelStation7.Text = m_ProductShareVariables.TurretStationResult[7].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[8].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 8, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[8].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 8, "UnitPresent");
                //    m_tabpageTurret.labelStation8.Text = m_ProductShareVariables.TurretStationResult[8].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[9].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 9, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[9].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 9, "UnitPresent");
                //    m_tabpageTurret.labelStation9.Text = m_ProductShareVariables.TurretStationResult[9].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[10].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 10, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[10].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 10, "UnitPresent");
                //    m_tabpageTurret.labelStation10.Text = m_ProductShareVariables.TurretStationResult[10].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[11].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 11, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[11].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 11, "UnitPresent");
                //    m_tabpageTurret.labelStation11.Text = m_ProductShareVariables.TurretStationResult[11].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[12].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 12, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[12].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 12, "UnitPresent");
                //    m_tabpageTurret.labelStation12.Text = m_ProductShareVariables.TurretStationResult[12].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[13].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 13, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[13].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 13, "UnitPresent");
                //    m_tabpageTurret.labelStation13.Text = m_ProductShareVariables.TurretStationResult[13].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[14].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 14, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[14].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 14, "UnitPresent");
                //    m_tabpageTurret.labelStation14.Text = m_ProductShareVariables.TurretStationResult[14].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[15].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 15, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[15].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 15, "UnitPresent");
                //    m_tabpageTurret.labelStation15.Text = m_ProductShareVariables.TurretStationResult[15].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[16].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 16, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[16].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 16, "UnitPresent");
                //    m_tabpageTurret.labelStation16.Text = m_ProductShareVariables.TurretStationResult[16].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[17].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 17, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[17].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 17, "UnitPresent");
                //    m_tabpageTurret.labelStation17.Text = m_ProductShareVariables.TurretStationResult[17].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[18].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 18, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[18].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 18, "UnitPresent");
                //    m_tabpageTurret.labelStation18.Text = m_ProductShareVariables.TurretStationResult[18].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[19].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 19, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[19].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 19, "UnitPresent");
                //    m_tabpageTurret.labelStation19.Text = m_ProductShareVariables.TurretStationResult[19].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[20].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 20, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[20].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 20, "UnitPresent");
                //    m_tabpageTurret.labelStation20.Text = m_ProductShareVariables.TurretStationResult[20].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[21].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 21, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[21].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 21, "UnitPresent");
                //    m_tabpageTurret.labelStation21.Text = m_ProductShareVariables.TurretStationResult[21].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[22].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 22, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[22].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 22, "UnitPresent");
                //    m_tabpageTurret.labelStation22.Text = m_ProductShareVariables.TurretStationResult[22].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.TurretStationResult[23].UnitPresent != m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 23, "UnitPresent"))
                //{
                //    m_ProductShareVariables.TurretStationResult[23].UnitPresent = m_ProductRTSSProcess.GetProductionArray("TurretStationResult", 23, "UnitPresent");
                //    m_tabpageTurret.labelStation23.Text = m_ProductShareVariables.TurretStationResult[23].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.FlipperStationResult[0].UnitPresent != m_ProductRTSSProcess.GetProductionArray("FlipperStationResult", 0, "UnitPresent"))
                //{
                //    m_ProductShareVariables.FlipperStationResult[0].UnitPresent = m_ProductRTSSProcess.GetProductionArray("FlipperStationResult", 0, "UnitPresent");
                //    m_tabpageTurret.labelFlipperStation0.Text = m_ProductShareVariables.FlipperStationResult[0].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.FlipperStationResult[1].UnitPresent != m_ProductRTSSProcess.GetProductionArray("FlipperStationResult", 1, "UnitPresent"))
                //{
                //    m_ProductShareVariables.FlipperStationResult[1].UnitPresent = m_ProductRTSSProcess.GetProductionArray("FlipperStationResult", 1, "UnitPresent");
                //    m_tabpageTurret.labelFlipperStation1.Text = m_ProductShareVariables.FlipperStationResult[1].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.FlipperStationResult[2].UnitPresent != m_ProductRTSSProcess.GetProductionArray("FlipperStationResult", 2, "UnitPresent"))
                //{
                //    m_ProductShareVariables.FlipperStationResult[2].UnitPresent = m_ProductRTSSProcess.GetProductionArray("FlipperStationResult", 2, "UnitPresent");
                //    m_tabpageTurret.labelFlipperStation2.Text = m_ProductShareVariables.FlipperStationResult[2].UnitPresent.ToString();
                //}
                //if (m_ProductShareVariables.FlipperStationResult[3].UnitPresent != m_ProductRTSSProcess.GetProductionArray("FlipperStationResult", 3, "UnitPresent"))
                //{
                //    m_ProductShareVariables.FlipperStationResult[3].UnitPresent = m_ProductRTSSProcess.GetProductionArray("FlipperStationResult", 3, "UnitPresent");
                //    m_tabpageTurret.labelFlipperStation3.Text = m_ProductShareVariables.FlipperStationResult[3].UnitPresent.ToString();
                //}

                //if (m_tabpageTurret.labelOutputUnit.Text != m_ProductRTSSProcess.GetProductionInt("nUnitHasBeenPlacedOnOutputTable").ToString())
                //{
                //    m_tabpageTurret.labelOutputUnit.Text = m_ProductRTSSProcess.GetProductionInt("nUnitHasBeenPlacedOnOutputTable").ToString();
                //}
                //if (m_tabpageTurret.labelQuantityInPurgeBin.Text != m_ProductRTSSProcess.GetProductionInt("nUnitHasBeenPlacedInPurgeBucket").ToString())
                //{
                //    m_tabpageTurret.labelQuantityInPurgeBin.Text = m_ProductRTSSProcess.GetProductionInt("nUnitHasBeenPlacedInPurgeBucket").ToString();
                //}
                //if (m_tabpageTurret.labelQuantityInFlipperPurgeBin.Text != m_ProductRTSSProcess.GetProductionInt("nUnitHasBeenPlacedInFlipperPurgeBucket").ToString())
                //{
                //    m_tabpageTurret.labelQuantityInFlipperPurgeBin.Text = m_ProductRTSSProcess.GetProductionInt("nUnitHasBeenPlacedInFlipperPurgeBucket").ToString();
                //}
                //int j = m_ProductRTSSProcess.GetProductionInt("nUnitToBeInspectedOnInputTable") - 1;
                //if (j < 0)
                //    j = 0;
                //if (m_tabpageTurret.labelQuantityInspectedOnInputTable.Text != j.ToString())
                //{
                //    m_tabpageTurret.labelQuantityInspectedOnInputTable.Text = j.ToString();
                //}

                #region FrameSelection
                if (m_ProductProcessEvent.PCS_GUI_CreateFrameSelection.WaitOne(0))
                {
                    //FrameSelection_CreateFrameSelection();
                }
                if (m_ProductProcessEvent.PCS_GUI_SelectSlotToRun.WaitOne(0))
                {
                    //FrameSelection_SelectSlotToRun();
                }
                //if (m_ProductProcessEvent.PCS_GUI_UpdateInputBarcodeID.WaitOne(0))
                //{
                //    m_checkBoxInputFrameSelection[(m_ProductRTSSProcess.GetProductionInt("nCurrentInputCassetteSlot") - 1)].Text = m_ProductShareVariables.strCurrentBarcodeID;
                //    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("CurrentTileID" + (m_ProductRTSSProcess.GetProductionInt("nCurrentInputCassetteSlot") - 1), m_ProductShareVariables.strCurrentBarcodeID);
                //}
                //if (m_ProductProcessEvent.PCS_GUI_UpdateOutputBarcodeID.WaitOne(0))
                //{
                //    m_checkBoxOutputFrameSelection[(m_ProductRTSSProcess.GetProductionInt("nCurrentOutputCassetteSlot") - 1)].Text = m_ProductShareVariables.strCurrentBarcodeID2;
                //    m_ProductShareVariables.RegKey.SetJobDataKeyParameter("CurrentTileIDOutput" + (m_ProductRTSSProcess.GetProductionInt("nCurrentOutputCassetteSlot") - 1), m_ProductShareVariables.strCurrentBarcodeID2);
                //}
                if (m_ProductProcessEvent.PCS_GUI_EnableFrameSelection.WaitOne(0))
                {
                    m_tabpageInputWaferSelection.splitContainerFrameSelection.Enabled = true;
                    m_tabpageInputWaferSelection.radioButtonFS_AllSlotToRun.Checked = true;
                    m_tabpageOutputWaferSelection.splitContainerFrameSelection.Enabled = true;
                    m_tabpageOutputWaferSelection.radioButtonFS_AllSlotToRun.Checked = true;
                }
                //if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CHECK_INPUT_FRAME_SLOT_DONE") == true)
                //{
                //    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CHECK_INPUT_FRAME_SLOT_DONE", false);
                //    if (m_ProductRTSSProcess.GetProductionArray("nArrayInputCassetteSlotToRun", m_ProductRTSSProcess.GetProductionInt("nCurrentInputCassetteSlot") - 1, "") == 0)
                //    {
                //        m_checkBoxInputFrameSelection[(m_ProductRTSSProcess.GetProductionInt("nCurrentInputCassetteSlot") - 1)].Text = "Empty";
                //        m_ProductShareVariables.RegKey.SetJobDataKeyParameter("CurrentTileID" + (m_ProductRTSSProcess.GetProductionInt("nCurrentInputCassetteSlot") - 1), "Empty");

                //    }
                //    else if (m_ProductRTSSProcess.GetProductionArray("nArrayInputCassetteSlotToRun", m_ProductRTSSProcess.GetProductionInt("nCurrentInputCassetteSlot") - 1, "") == 1)
                //    {
                //        m_checkBoxInputFrameSelection[(m_ProductRTSSProcess.GetProductionInt("nCurrentInputCassetteSlot") - 1)].Text = "Frame Present";
                //        m_ProductShareVariables.RegKey.SetJobDataKeyParameter("CurrentTileID" + (m_ProductRTSSProcess.GetProductionInt("nCurrentInputCassetteSlot") - 1), "Frame Or Tile Present");
                //        OnLoadFrameOrTile();

                //        m_ProductProcessEvent.PCS_PCS_Start_Map_Drive.Set();
                //    }
                //}
                //if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CHECK_OUTPUT_FRAME_SLOT_DONE") == true)
                //{
                //    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CHECK_OUTPUT_FRAME_SLOT_DONE", false);
                //    if (m_ProductRTSSProcess.GetProductionArray("nArrayOutputCassetteSlotToRun", m_ProductRTSSProcess.GetProductionInt("nCurrentOutputCassetteSlot") - 1, "") == 0)
                //    {
                //        m_checkBoxOutputFrameSelection[(m_ProductRTSSProcess.GetProductionInt("nCurrentOutputCassetteSlot") - 1)].Text = "Empty";
                //        m_ProductShareVariables.RegKey.SetJobDataKeyParameter("CurrentTileIDOutput" + (m_ProductRTSSProcess.GetProductionInt("nCurrentOutputCassetteSlot") - 1), "Empty");

                //    }
                //    else if (m_ProductRTSSProcess.GetProductionArray("nArrayOutputCassetteSlotToRun", m_ProductRTSSProcess.GetProductionInt("nCurrentOutputCassetteSlot") - 1, "") == 1)
                //    {
                //        m_checkBoxOutputFrameSelection[(m_ProductRTSSProcess.GetProductionInt("nCurrentOutputCassetteSlot") - 1)].Text = "Frame Present";
                //        m_ProductShareVariables.RegKey.SetJobDataKeyParameter("CurrentTileIDOutput" + (m_ProductRTSSProcess.GetProductionInt("nCurrentOutputCassetteSlot") - 1), "Frame Or Tile Present");
                //        //OnLoadFrameOrTile();

                //        //m_ProductProcessEvent.PCS_PCS_Start_Map_Drive.Set();
                //    }
                //}
                #endregion

                //           if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_VISION_VERIFICATION_RETRY_REQUEST") == true)
                //           {
                //               m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VISION_VERIFICATION_RETRY_REQUEST", false);
                //               if (MessageBox.Show("Are you want to retry vision verification/calibration.", "Retry Confirmation",
                //                   MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                //               {
                //                   m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_VISION_VERIFICATION_RETRY", true);
                //                   Machine.EventLogger.WriteLog(string.Format("{0} Click RETRY vision verification/calibration at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //               }
                //               else
                //               {
                //                   m_ProductRTSSProcess.SetEvent("JobStop", true);
                //                   Machine.EventLogger.WriteLog(string.Format("{0} Click NOT RETRY vision verification/calibration at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //               }
                //           }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_TEACH_VISION_START") == true)// && m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_RETEACH_MAP_START") == false)
                {
                    
                    Machine.EventLogger.WriteLog(string.Format("{0} Teach Vision is Launched.", DateTime.Now.ToString("yyyyMMdd HHmmss")));

                    //if (m_ProductShareVariables.productRecipeVisionSettings.EnableTeachUnitAtVision)// || m_ProductRTSSProcess.GetProductionBool("ResumePickAndPlace") == true)
                    {
                        Machine.EventLogger.WriteLog(string.Format("{0} Teach Vision is Launched 2.", DateTime.Now.ToString("yyyyMMdd HHmmss")));

                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TEACHING_VISION", true);
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_TEACH_VISION_START", false);
                        if (m_formTeachMap != null)
                        {
                            if (!m_formTeachMap.IsDisposed)
                            {
                                m_formTeachMap.Focus();
                                return;
                            }
                        }

                        CreateFormTeachMap();
                        SetFormTeachMap();
                        SetFormTeachMapVariables();
                        InitializeFormTeachMap();

                        //if (m_ProductRTSSProcess.GetProductionBool("ResumePickAndPlace") == true)
                        //{
                        //    m_formTeachMap.SetAllocateButtonVisible = true;
                        //}
                        //m_formTeachMap = new ProductFormTeachMap();

                        //m_formTeachMap.MdiParent = this;
                        //m_formTeachMap.Dock = DockStyle.Fill;
                        //m_formTeachMap.SetParameter(ShareVariables.strRecipeMainPath, ShareVariables.strXmlExtension);
                        m_formTeachMap.Show();
                    }
                    //else
                    //{

                    //}
                }

                //           if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_READY_TEACH_MAP") == true && m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_RETEACH_MAP_START") == true)
                //           {
                //               Machine.EventLogger.WriteLog(string.Format("{0} Thor debug: Teach Map is trigger 3.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                //               m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_RETEACH_MAP_UNLOAD", false);
                //               toolStripButtonReTeachMap.Enabled = false;
                //               //if (m_ProductShareVariables.productRecipeVisionSettings.TriggerTeachAlignment)
                //               {
                //                   m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_READY_TEACH_MAP", false);
                //                   m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TEACHING_MAP", true);
                //                   if (m_formTeachMap != null)
                //                   {
                //                       if (!m_formTeachMap.IsDisposed)
                //                       {
                //                           m_formTeachMap.Focus();
                //                           return;
                //                       }
                //                   }
                //                   //m_formJob = new FormJob();
                //                   CreateFormTeachMap();
                //                   SetFormTeachMap();
                //                   SetFormTeachMapVariables();
                //                   InitializeFormTeachMap();
                //                   m_formTeachMap.SetAllocateButtonVisible = true;
                //                   m_formTeachMap.SetOutputGroupBoxVisible = false;
                //                   //m_formTeachMap = new ProductFormTeachMap();

                //                   //m_formTeachMap.MdiParent = this;
                //                   //m_formTeachMap.Dock = DockStyle.Fill;
                //                   //m_formTeachMap.SetParameter(ShareVariables.strRecipeMainPath, ShareVariables.strXmlExtension);
                //                   //SwitchLanguageFormJob();
                //                   m_formTeachMap.Show();
                //               }
                //               //else
                //               {

                //               }
                //           }

                //           if (toolStripButtonReTeachMap.Enabled != m_ProductRTSSProcess.GetEvent("RTHD_RMAIN_RETEACH_MAP_ENABLE"))
                //           {
                //               toolStripButtonReTeachMap.Enabled = m_ProductRTSSProcess.GetEvent("RTHD_RMAIN_RETEACH_MAP_ENABLE");
                //           }

                //           if (m_ProductRTSSProcess.GetEvent("RTHD_RMAIN_CHECK_SETUP_CAMERA_STATUS_DONE") == true)
                //           {
                //               m_ProductRTSSProcess.SetEvent("RTHD_RMAIN_CHECK_SETUP_CAMERA_STATUS_DONE", false);
                //               if (m_ProductRTSSProcess.GetEvent("RTHD_RMAIN_CHECK_SETUP_CAMERA_IS_UP") == true)
                //               {
                //                   toolStripButtonSetupCameraUpDownControl.Text = "Setup\rCamera\rDown";
                //               }
                //               else
                //               {
                //                   toolStripButtonSetupCameraUpDownControl.Text = "Setup\rCamera\rUp";
                //               }
                //           }


                //           if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_BOTTOM_VISION_LEARN_UNIT_START") == true)
                //           {
                //               m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_BOTTOM_VISION_LEARN_UNIT_START", false);
                //               if (MessageBox.Show("Please learn Bottom Vision Unit. After done, click ok", "Proceed Confirmation",
                //                      MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                //               {
                //                   m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BOTTOM_VISION_LEARN_UNIT_DONE", true);
                //                   Machine.EventLogger.WriteLog(string.Format("{0} Click learn bottom vision done {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //               }
                //           }



                //           if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_OUTPUT_VISION_LEARN_UNIT_START") == true)
                //           {
                //               m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_OUTPUT_VISION_LEARN_UNIT_START", false);
                //               if (MessageBox.Show("Please learn Output Vision Unit. After done, click ok", "Proceed Confirmation",
                //                      MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                //               {
                //                   m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUTPUT_VISION_LEARN_UNIT_DONE", true);
                //                   Machine.EventLogger.WriteLog(string.Format("{0} Click learn Output vision done {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //               }
                //           }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_START_REAL_TIME_UPH") == true)
                {
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_START_REAL_TIME_UPH", false);
                    m_ProductShareVariables.dtLotStartTime = DateTime.Now;
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_UPDATE_REAL_TIME_YEILD") == true)
                {
                    if (m_THKPanel.labelLotDetailsTotalInputQuantityValue.Text != m_ProductRTSSProcess.GetProductionInt("nCurrentInputQuantity").ToString())
                    {
                        m_THKPanel.labelLotDetailsTotalInputQuantityValue.Text = m_ProductRTSSProcess.GetProductionInt("nCurrentInputQuantity").ToString();
                    }
                    if (m_THKPanel.labelLotDetailsTotalOutputQuantityValue.Text != m_ProductRTSSProcess.GetProductionInt("nCurrentOutputQuantity").ToString())
                    {
                        m_THKPanel.labelLotDetailsTotalOutputQuantityValue.Text = m_ProductRTSSProcess.GetProductionInt("nCurrentOutputQuantity").ToString();
                    }

                    if (m_THKPanel.labelLotDetailsTotalRejectQuantityValue.Text != m_ProductRTSSProcess.GetProductionInt("nCurrentRejectQuantity").ToString())
                    {
                        m_THKPanel.labelLotDetailsTotalRejectQuantityValue.Text = m_ProductRTSSProcess.GetProductionInt("nCurrentRejectQuantity").ToString();
                    }

                    if (m_THKPanel.labelLotDetailsTotalYieldValue.Text != ((double)((m_ProductRTSSProcess.GetProductionInt("nCurrentOutputQuantity")) / (double)(m_ProductRTSSProcess.GetProductionInt("nCurrentInputQuantity"))) * 100.0).ToString("0.00")
                        + " (" + m_ProductShareVariables.productRecipeOutputSettings.YieldPercentage.ToString("0.00") + ")")
                    {
                        m_THKPanel.labelLotDetailsTotalYieldValue.Text = ((double)((m_ProductRTSSProcess.GetProductionInt("nCurrentOutputQuantity")) / (double)(m_ProductRTSSProcess.GetProductionInt("nCurrentInputQuantity"))) * 100.0).ToString("0.00")
                            + " (" + m_ProductShareVariables.productRecipeOutputSettings.YieldPercentage.ToString("0.00") + ")";
                        if (((double)((m_ProductRTSSProcess.GetProductionInt("nCurrentOutputQuantity")) / (double)(m_ProductRTSSProcess.GetProductionInt("nCurrentInputQuantity"))) * 100.0)
                            <= m_ProductShareVariables.productRecipeOutputSettings.YieldPercentage)
                        {
                            m_THKPanel.labelLotDetailsTotalYieldValue.ForeColor = Color.Red;
                        }
                        else
                        {
                            m_THKPanel.labelLotDetailsTotalYieldValue.ForeColor = Color.Green;
                        }
                    }

                    if (m_THKPanel.labelTrayDetailsTotalYieldValue.Text != ((double)((double)((m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColInput * m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput) - m_ProductRTSSProcess.GetProductionInt("nCurrentRejectQuantityBasedOnInputTray")) / (m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColInput * m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput)) * 100.0).ToString("0.00")
                       + " (" + m_ProductShareVariables.productRecipeOutputSettings.YieldPercentage.ToString("0.00") + ")")
                    {
                        m_THKPanel.labelTrayDetailsTotalYieldValue.Text = ((double)((double)((m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColInput * m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput) - m_ProductRTSSProcess.GetProductionInt("nCurrentRejectQuantityBasedOnInputTray")) / (m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColInput * m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput)) * 100.0).ToString("0.00")
                            + " (" + m_ProductShareVariables.productRecipeOutputSettings.YieldPercentage.ToString("0.00") + ")";
                        if (((double)((double)((m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColInput * m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput) - m_ProductRTSSProcess.GetProductionInt("nCurrentRejectQuantityBasedOnInputTray")) / (m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColInput * m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput)) * 100.0)
                            <= m_ProductShareVariables.productRecipeOutputSettings.YieldPercentage)
                        {
                            m_THKPanel.labelTrayDetailsTotalYieldValue.ForeColor = Color.Red;
                        }
                        else
                        {
                            m_THKPanel.labelTrayDetailsTotalYieldValue.ForeColor = Color.Green;
                        }
                    }

                    if (m_ProductRTSSProcess.GetProductionInt("nCurrentInputQuantity") != 0)
                    {
                        double dtCycleTime = (DateTime.Now - m_ProductShareVariables.dtLotStartTime).TotalMilliseconds;
                        m_THKPanel.labelLotDetailsTotalInputUPH.Text = ((double)(m_ProductRTSSProcess.GetProductionInt("nCurrentInputQuantity") / (dtCycleTime / 3600000))).ToString("0.00");
                    }
                    if ((m_ProductRTSSProcess.GetProductionInt("nCurrentOutputQuantity") + m_ProductRTSSProcess.GetProductionInt("nCurrentRejectQuantity")) != 0)
                    {
                        double dtCycleTime = (DateTime.Now - m_ProductShareVariables.dtLotStartTime).TotalMilliseconds;
                        TimeSpan tsEfficiency = DateTime.Now - m_ProductShareVariables.dtLotStartTime;
                        double elapsedHours = tsEfficiency.TotalHours;
                        double idealOutput = 900 * elapsedHours;
                        int TotalOutputRejectUnit = m_ProductRTSSProcess.GetProductionInt("nCurrentOutputQuantity") + m_ProductRTSSProcess.GetProductionInt("nCurrentRejectQuantity");
                        m_THKPanel.labelLotDetailsTotalOutputUPH.Text = ((double)(TotalOutputRejectUnit / (dtCycleTime / 3600000))).ToString("0.00");
                        m_THKPanel.labelLotDetailsTotalEfficiency.Text = ((double)(((double)(TotalOutputRejectUnit / idealOutput)) * 100)).ToString("0.00");
                    }
                    if (m_ProductRTSSProcess.GetProductionInt("nCurrentLowYieldAlarmQuantity") >= m_ProductShareVariables.productRecipeOutputSettings.LowYieldAlarm
                        && m_ProductRTSSProcess.GetEvent("RMAIN_GMAIN_LOW_YIELD_ALARM_TRIGGER") == false)
                    {
                        if (((double)((m_ProductRTSSProcess.GetProductionInt("nCurrentOutputQuantity")) / (double)(m_ProductRTSSProcess.GetProductionInt("nCurrentInputQuantity"))) * 100.0)
                           <= (double)m_ProductShareVariables.productRecipeOutputSettings.YieldPercentage)
                        {
                            Machine.SequenceControl.SetAlarm(60214);
                            m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_LOW_YIELD_ALARM_TRIGGER", true);
                        }                        
                    }
                }
                if(m_ProductRTSSProcess.GetEvent("RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_START") ==true)
                {
                    m_ProductRTSSProcess.SetEvent("RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_START", false);
                    if (MessageBox.Show("Input Alignment Offset Out Of Range.\r\n Press OK after fine tune vision recipe to resnap the unit" +
                            "\r\n Press Cancel to skip the unit and continue sequence", "Confirmation",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_DONE", true);
                        Machine.EventLogger.WriteLog(string.Format("{0} Click OK After Fine Tune The Vision Recipe To Resnap The Unit at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    }
                    else
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_RINT_INPUT_FINE_TUNE_OR_SKIP_FAIL", true);
                        Machine.EventLogger.WriteLog(string.Format("{0} Click Cancel skip the unit and continue sequence at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    }
                }
                if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_IS_REMAIN_OR_UNLOADTRAY") == true)
                {
                    m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_IS_REMAIN_OR_UNLOADTRAY", false);
                    if (MessageBox.Show("Remained Units On Tray After MBB Lot Ended, choose to remain tray or unload tray.\r\n Press OK To Remain Tray" +
                            "\r\n Press Cancel to Unload Tray", "Confirmation",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_IS_REMAINTRAY", true);
                        Machine.EventLogger.WriteLog(string.Format("{0} Click OK to remain tray at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    }
                    else
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_IS_UNLOADTRAY", true);
                        Machine.EventLogger.WriteLog(string.Format("{0} Click Cancel to unload tray at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    }
                }
                if (m_ProductRTSSProcess.GetEvent("RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_START") == true)
                {
                    m_ProductRTSSProcess.SetEvent("RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_START", false);
                    if (MessageBox.Show("Output Post Offset Out Of Range.\r\n Press OK after fine tune vision recipe to resnap the unit" +
                            "\r\n Press Cancel to skip the unit and continue sequence", "Confirmation",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_DONE", true);
                        Machine.EventLogger.WriteLog(string.Format("{0} Click OK After Fine Tune The Vision Recipe To Resnap The Output Unit at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    }
                    else
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_ROUT_OUTPUT_POST_FINE_TUNE_OR_SKIP_FAIL", true);
                        Machine.EventLogger.WriteLog(string.Format("{0} Click Cancel skip the Output unit and continue sequence at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    int c = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "");
                    int b = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadCount", 3000).ToString());
                    if (m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "") >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadDueCount", 3000).ToString()))
                    {
                        m_ProductProcessEvent.PCS_GUI_Due_Count_Alarm.Set();
                        //break;
                    }
                    if (m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "") >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadWarningCount", 2000).ToString()))
                    {
                        m_ProductProcessEvent.PCS_GUI_Warning_Count_Alarm.Set();
                        //break;
                    }
                    if (m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "") >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"Pick Up Head {i + 1}CleanCount", 1000).ToString()))
                    {
                        m_ProductProcessEvent.PCS_GUI_Clean_Count_Alarm.Set();
                        //break;
                    }
                }

                #region Secsgem
                if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
                {
                    if (Machine.Platform.SecsgemControl.ReceiveMessageFromHost.WaitOne(0))
                        MessageBox.Show(Machine.Platform.SecsgemControl.MessageFromHost);
                    if (Machine.Platform.SecsgemControl.CommunicationStateChange.WaitOne(0))
                    {
                        if (Machine.Platform.SecsgemControl.CommunicationState == true)
                            m_GUIRightPanel.labelCommunicationState.Text = "Communicating";
                        else
                            m_GUIRightPanel.labelCommunicationState.Text = "Not Communicating";
                    }
                    if (Machine.Platform.SecsgemControl.HostTriggerControlStateOffline.WaitOne(0))
                        m_GUIRightPanel.labelControlState.Text = "Offline";
                    if (Machine.Platform.SecsgemControl.HostTriggerControlStateOnlineLocal.WaitOne(0))
                        m_GUIRightPanel.labelControlState.Text = "Online Local";
                    if (Machine.Platform.SecsgemControl.HostTriggerControlStateOnlineRemote.WaitOne(0))
                        m_GUIRightPanel.labelControlState.Text = "Online Remote";
                }
                if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
                {
                    Machine.Platform.SecsgemControl.MTBA = string.Format("{0:0.00}", nMTBATemp);
                    Machine.Platform.SecsgemControl.MTBF = string.Format("{0:0.00}", nMTBFTemp);
                }
                if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
                {
                    Machine.Platform.SecsgemControl.UPH = string.Format("{0:0.00}", (3600000.0 / m_ProductShareVariables.nCurrentCycleTime));
                }
                #endregion Secsgem
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        void ClearAlarm(string AlarmId)
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.ClearAlarm(AlarmId) != 0)
                {
                    Machine.SequenceControl.SetAlarm(7008);
                }
            }
        }

        #region Input XY Table
#if InputXYTable
        virtual public int NewLot()
        {
            try
            {
                //if (m_groupboxCustomerInput.textBoxProductPartNumber.Text == "")
                //{
                //    MessageBox.Show("Please enter part number");
                //    return 1;
                //}
                if (comboBoxRecipe.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select recipe");
                    return 1;
                }
                //if (!Directory.Exists(m_ProductShareVariables.strRecipeMainPath))
                //{
                //    MessageBox.Show("Recipe directory " + m_ProductShareVariables.strRecipeMainPath + " not exist");
                //    return 2;
                //}
                //DirectoryInfo directoryInfo = new DirectoryInfo(m_ProductShareVariables.strRecipeMainPath);
                //FileInfo[] fileInfo = directoryInfo.GetFiles();
                //if (fileInfo.Length == 0)
                //{
                //    MessageBox.Show("No recipe found in directory " + m_ProductShareVariables.strRecipeMainPath);
                //    return 3;
                //}
                //bool bRecipeFound = false;
                //foreach (FileInfo _fileInfo in fileInfo)
                //{
                //    if ((_fileInfo.Name) == m_groupboxCustomerInput.textBoxProductPartNumber.Text + m_ProductShareVariables.strXmlExtension)
                //    {
                //        bRecipeFound = true;
                //        break;
                //    }
                //}
                //if (bRecipeFound == false)
                //{
                //    MessageBox.Show("Recipe " + m_groupboxCustomerInput.textBoxProductPartNumber.Text + m_ProductShareVariables.strXmlExtension + " not exist");
                //    return 4;
                //}
                //comboBoxRecipe.SelectedIndex = -1;
                //comboBoxRecipe.SelectedItem = m_groupboxCustomerInput.textBoxProductPartNumber.Text;

                //m_ProductShareVariables.bFormProductionProductPartNumberEnable = false;
                //textBoxProductPartNumber.Enabled = false;

                //EventLogger.WriteLog(string.Format("{0} Click new lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //if (m_formNewLot != null)
                //{
                //    if (!m_formNewLot.IsDisposed)
                //    {
                //        m_formNewLot.Focus();
                //        return;
                //    }
                //}

                //m_formNewLot = new FormNewLot();
                ////m_formNewLot.MdiParent = this;
                ////m_formNewLot.Dock = DockStyle.Fill;
                ////m_formConversion.SetParameter(ShareVariables.strRecipeMainPath, ShareVariables.strRecipeExtension);
                ////SwitchLanguageFormJob();
                //m_formNewLot.Show();
                return 0;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        virtual public void LaunchNewLotForm()
        {

        }

        

        #region FrameSelection
        private void frameSelection_CreatePanel(DockStyle dockStyle, int height,
            int width, out Panel panel)
        {
            Panel p = new Panel();
            p.AutoScroll = false;
            p.AutoSize = false;
            p.Dock = dockStyle;
            p.Height = height;
            p.Visible = true;
            p.Width = width;
            panel = p;
        }

        private void frameSelection_CreateLabel(DockStyle dockStyle, int height,
            string text, int width, out Label label)
        {
            Label l = new Label();
            l.AutoSize = false;
            l.Dock = dockStyle;
            l.Height = height;
            l.Text = text;
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Visible = true;
            l.Width = width;
            label = l;
        }

        private void InputframeSelection_CreateCheckBox(DockStyle dockStyle, int height,
            int i, string text, ref CheckBox checkBox)
        {
            CheckBox c = new CheckBox();
            c.Appearance = Appearance.Button;
            c.AutoSize = false;
            c.BackColor = Color.Gray;
            c.Checked = false;
            c.Dock = dockStyle;
            c.Height = height;
            c.Name = "Frame " + (i + 1).ToString();
            c.Text = text;
            c.TextAlign = ContentAlignment.MiddleCenter;
            c.Visible = true;
            c.Click += InputFrameSelection_CheckBox_Click;
            c.CheckedChanged += checkBox_CheckedChanged;
            //m_checkBoxInputFrameSelection[i] = c;
            checkBox = c;
        }

        private void OutputframeSelection_CreateCheckBox(DockStyle dockStyle, int height,
            int i, string text, ref CheckBox checkBox)
        {
            CheckBox c = new CheckBox();
            c.Appearance = Appearance.Button;
            c.AutoSize = false;
            c.BackColor = Color.Gray;
            c.Checked = false;
            c.Dock = dockStyle;
            c.Height = height;
            c.Name = "Frame " + (i + 1).ToString();
            c.Text = text;
            c.TextAlign = ContentAlignment.MiddleCenter;
            c.Visible = true;
            c.Click += OutputFrameSelection_CheckBox_Click;
            c.CheckedChanged += checkBox_CheckedChanged;
            //m_checkBoxInputFrameSelection[i] = c;
            checkBox = c;
        }

        private void InputFrameSelection_CheckBox_Click(object sender, EventArgs e)
        {
            if (m_tabpageInputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked)
            {
                CheckBox c = (CheckBox)sender;
                for (int i = 0; i < m_checkBoxInputFrameSelection.Length; i++)
                    m_checkBoxInputFrameSelection[i].Checked = false;
                int r = 0;
                String[] s = c.Name.Split(' ');
                Int32.TryParse(s[1], out r);
                r--;
                m_ProductShareVariables.RegKey.SetJobDataKeyParameter("StartFromSelectedSlot", r.ToString());
                for (int i = r; i < m_checkBoxInputFrameSelection.Length; i++)
                    m_checkBoxInputFrameSelection[i].Checked = true;
            }
        }

        private void OutputFrameSelection_CheckBox_Click(object sender, EventArgs e)
        {
            if (m_tabpageOutputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked)
            {
                CheckBox c = (CheckBox)sender;
                for (int i = 0; i < m_checkBoxOutputFrameSelection.Length; i++)
                    m_checkBoxOutputFrameSelection[i].Checked = false;
                int r = 0;
                String[] s = c.Name.Split(' ');
                Int32.TryParse(s[1], out r);
                r--;
                m_ProductShareVariables.RegKey.SetJobDataKeyParameter("StartFromSelectedSlotOutput", r.ToString());
                for (int i = r; i < m_checkBoxOutputFrameSelection.Length; i++)
                    m_checkBoxOutputFrameSelection[i].Checked = true;
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox c = (CheckBox)sender;
                if (c.Checked) c.BackColor = Color.Green;
                else c.BackColor = Color.Gray;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void frameSelection_CreateHeader(Panel panel)
        {
            try
            {
                Panel p;
                Label l;
                frameSelection_CreatePanel(DockStyle.Top, 30, 0, out p);
                frameSelection_CreateLabel(DockStyle.Left, p.Height, "Slot", 80, out l);
                p.Controls.Add(l);
                frameSelection_CreateLabel(DockStyle.Fill, p.Height, "Frame (s)", 80, out l);
                p.Controls.Add(l);
                l.BringToFront();
                panel.Controls.Add(p);
                p.SendToBack();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void InputFrameSelection_CreateFrameOfTiles(int height, int i, Panel panel, ref CheckBox checkBox)
        {
            try
            {
                Panel p;
                Label l;
                //CheckBox c;
                frameSelection_CreatePanel(DockStyle.Top, height, 0, out p);
                frameSelection_CreateLabel(DockStyle.Left, p.Height, (i + 1).ToString(), 80, out l);
                InputframeSelection_CreateCheckBox(DockStyle.Fill, p.Height, i, "Unknown", ref checkBox);
                p.Controls.Add(l);
                p.Controls.Add(checkBox);
                checkBox.BringToFront();
                panel.Controls.Add(p);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void OutputFrameSelection_CreateFrameOfTiles(int height, int i, Panel panel, ref CheckBox checkBox)
        {
            try
            {
                Panel p;
                Label l;
                //CheckBox c;
                frameSelection_CreatePanel(DockStyle.Top, height, 0, out p);
                frameSelection_CreateLabel(DockStyle.Left, p.Height, (i + 1).ToString(), 80, out l);
                OutputframeSelection_CreateCheckBox(DockStyle.Fill, p.Height, i, "Unknown", ref checkBox);
                p.Controls.Add(l);
                p.Controls.Add(checkBox);
                checkBox.BringToFront();
                panel.Controls.Add(p);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void FrameSelection_CreateFrameSelection()
        {
            try
            {
                //splitContainerFrameSelection.Enabled = true;
                if (m_strFrameSelectionRecipe != m_ProductShareVariables.currentMainRecipeName)
                {
                    m_strFrameSelectionRecipe = m_ProductShareVariables.currentMainRecipeName;
                    Panel p1 = new Panel();
                    Panel p2 = new Panel();
                    int nPanelHeight = 0;
                    #region Input
                    m_checkBoxInputFrameSelection = new CheckBox[(int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot];

                    nPanelHeight = (m_tabpageInputWaferSelection.splitContainerFrameSelection.Panel2.Height - 30) /
                        (int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot;
                    if (nPanelHeight <= 38) nPanelHeight = 38;

                    m_tabpageInputWaferSelection.splitContainerFrameSelection.Panel2.Controls.Clear();

                    if ((int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot > 0 &&
                        (int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot <= 13)
                    {
                        frameSelection_CreatePanel(DockStyle.Fill, 0, 0, out p1);
                        m_tabpageInputWaferSelection.splitContainerFrameSelection.Panel2.Controls.Add(p1);
                    }
                    else if ((int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot > 13 &&
                        (int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot <= 25)
                    {
                        int width = (m_tabpageInputWaferSelection.splitContainerFrameSelection.Panel2.Width / 2);
                        frameSelection_CreatePanel(DockStyle.Left, 0, width, out p1);
                        frameSelection_CreatePanel(DockStyle.Left, 0, width, out p2);
                        m_tabpageInputWaferSelection.splitContainerFrameSelection.Panel2.Controls.Add(p1);
                        m_tabpageInputWaferSelection.splitContainerFrameSelection.Panel2.Controls.Add(p2);
                        p2.BringToFront();
                    }

                    //for (int i = 0; i < 100; i++)
                    //    m_ProductRTSSProcess.SetProductionArray("nArrayCassetteSlotToRun", i, "", 0);

                    for (int i = 0; i < (int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot; i++)
                    {
                        if (i <= 12) InputFrameSelection_CreateFrameOfTiles(nPanelHeight, i, p1, ref m_checkBoxInputFrameSelection[i]);
                        else InputFrameSelection_CreateFrameOfTiles(nPanelHeight, i, p2, ref m_checkBoxInputFrameSelection[i]);
                    }

                    if ((int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot > 0 &&
                        (int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot <= 13)
                    {
                        frameSelection_CreateHeader(p1);
                    }
                    else if ((int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot > 13 &&
                        (int)m_ProductShareVariables.productRecipeInputCassetteSettings.CassetteTotalSlot <= 25)
                    {
                        frameSelection_CreateHeader(p1);
                        frameSelection_CreateHeader(p2);
                    }
                    #endregion Input

                    #region Output
                    p1 = new Panel();
                    p2 = new Panel();
                    m_checkBoxOutputFrameSelection = new CheckBox[(int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot];

                    nPanelHeight = (m_tabpageOutputWaferSelection.splitContainerFrameSelection.Panel2.Height - 30) /
                        (int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot;
                    if (nPanelHeight <= 38) nPanelHeight = 38;

                    m_tabpageOutputWaferSelection.splitContainerFrameSelection.Panel2.Controls.Clear();

                    if ((int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot > 0 &&
                        (int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot <= 13)
                    {
                        frameSelection_CreatePanel(DockStyle.Fill, 0, 0, out p1);
                        m_tabpageOutputWaferSelection.splitContainerFrameSelection.Panel2.Controls.Add(p1);
                    }
                    else if ((int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot > 13 &&
                        (int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot <= 25)
                    {
                        int width = (m_tabpageOutputWaferSelection.splitContainerFrameSelection.Panel2.Width / 2);
                        frameSelection_CreatePanel(DockStyle.Left, 0, width, out p1);
                        frameSelection_CreatePanel(DockStyle.Left, 0, width, out p2);
                        m_tabpageOutputWaferSelection.splitContainerFrameSelection.Panel2.Controls.Add(p1);
                        m_tabpageOutputWaferSelection.splitContainerFrameSelection.Panel2.Controls.Add(p2);
                        p2.BringToFront();
                    }

                    //for (int i = 0; i < 100; i++)
                    //    m_ProductRTSSProcess.SetProductionArray("nArrayCassetteSlotToRun", i, "", 0);

                    for (int i = 0; i < (int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot; i++)
                    {
                        if (i <= 12) OutputFrameSelection_CreateFrameOfTiles(nPanelHeight, i, p1, ref m_checkBoxOutputFrameSelection[i]);
                        else OutputFrameSelection_CreateFrameOfTiles(nPanelHeight, i, p2, ref m_checkBoxOutputFrameSelection[i]);
                    }

                    if ((int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot > 0 &&
                        (int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot <= 13)
                    {
                        frameSelection_CreateHeader(p1);
                    }
                    else if ((int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot > 13 &&
                        (int)m_ProductShareVariables.productRecipeOutputCassetteSettings.CassetteTotalSlot <= 25)
                    {
                        frameSelection_CreateHeader(p1);
                        frameSelection_CreateHeader(p2);
                    }
                    #endregion Output

                    if (m_bUpdateFrameSelection)
                    {
                        m_bUpdateFrameSelection = false;

                        string allSlotToRun = "true";
                        string fromSelectedSlotToRun = "true";
                        string selectedSlotToRun = "true";
                        #region Input
                        if (allSlotToRun == m_ProductShareVariables.RegKey.GetJobDataKeyParameter("AllSlotToRun", "false").ToString())
                        {
                            m_tabpageInputWaferSelection.radioButtonFS_AllSlotToRun.Checked = true;
                            m_tabpageInputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked = false;
                            m_tabpageInputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked = false;
                        }
                        else if (fromSelectedSlotToRun == m_ProductShareVariables.RegKey.GetJobDataKeyParameter("FromSelectedSlotToRun", "false").ToString())
                        {
                            m_tabpageInputWaferSelection.radioButtonFS_AllSlotToRun.Checked = false;
                            m_tabpageInputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked = true;
                            m_tabpageInputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked = false;
                        }
                        else if (selectedSlotToRun == m_ProductShareVariables.RegKey.GetJobDataKeyParameter("SelectedSlotToRun", "false").ToString())
                        {
                            m_tabpageInputWaferSelection.radioButtonFS_AllSlotToRun.Checked = false;
                            m_tabpageInputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked = false;
                            m_tabpageInputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked = true;
                        }

                        for (int i = 0; i < m_checkBoxInputFrameSelection.Length; i++)
                        {
                            if (m_checkBoxInputFrameSelection[i].Text != m_ProductShareVariables.RegKey.GetJobDataKeyParameter("CurrentTileID" + i.ToString(), "Unknown").ToString())
                            {
                                m_checkBoxInputFrameSelection[i].Text = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("CurrentTileID" + i.ToString(), "Unknown").ToString();
                                if (m_tabpageInputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked)
                                    m_checkBoxInputFrameSelection[i].Checked = true;
                            }
                        }

                        if (m_tabpageInputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked)
                        {
                            string start = m_ProductShareVariables.RegKey.GetJobDataKeyParameter(
                                "StartFromSelectedSlot", "0").ToString();
                            for (int i = 0; i < m_checkBoxInputFrameSelection.Length; i++)
                                m_checkBoxInputFrameSelection[i].Checked = false;
                            for (int i = Convert.ToInt32(start); i < m_checkBoxInputFrameSelection.Length; i++)
                                m_checkBoxInputFrameSelection[i].Checked = true;
                        }
                        #endregion Input

                        #region Output
                        if (allSlotToRun == m_ProductShareVariables.RegKey.GetJobDataKeyParameter("AllSlotToRunOutput", "false").ToString())
                        {
                            m_tabpageOutputWaferSelection.radioButtonFS_AllSlotToRun.Checked = true;
                            m_tabpageOutputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked = false;
                            m_tabpageOutputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked = false;
                        }
                        else if (fromSelectedSlotToRun == m_ProductShareVariables.RegKey.GetJobDataKeyParameter("FromSelectedSlotToRunOutput", "false").ToString())
                        {
                            m_tabpageOutputWaferSelection.radioButtonFS_AllSlotToRun.Checked = false;
                            m_tabpageOutputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked = true;
                            m_tabpageOutputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked = false;
                        }
                        else if (selectedSlotToRun == m_ProductShareVariables.RegKey.GetJobDataKeyParameter("SelectedSlotToRunOutput", "false").ToString())
                        {
                            m_tabpageOutputWaferSelection.radioButtonFS_AllSlotToRun.Checked = false;
                            m_tabpageOutputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked = false;
                            m_tabpageOutputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked = true;
                        }

                        for (int i = 0; i < m_checkBoxOutputFrameSelection.Length; i++)
                        {
                            if (m_checkBoxOutputFrameSelection[i].Text != m_ProductShareVariables.RegKey.GetJobDataKeyParameter("CurrentTileIDOutput" + i.ToString(), "Unknown").ToString())
                            {
                                m_checkBoxOutputFrameSelection[i].Text = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("CurrentTileIDOutput" + i.ToString(), "Unknown").ToString();
                                if (m_tabpageOutputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked)
                                    m_checkBoxOutputFrameSelection[i].Checked = true;
                            }
                        }

                        if (m_tabpageOutputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked)
                        {
                            string start = m_ProductShareVariables.RegKey.GetJobDataKeyParameter(
                                "StartFromSelectedSlotOutput", "0").ToString();
                            for (int i = 0; i < m_checkBoxOutputFrameSelection.Length; i++)
                                m_checkBoxOutputFrameSelection[i].Checked = false;
                            for (int i = Convert.ToInt32(start); i < m_checkBoxOutputFrameSelection.Length; i++)
                                m_checkBoxOutputFrameSelection[i].Checked = true;
                        }
                        #endregion Output
                        //splitContainerFrameSelection.Enabled = false;
                    }
                    else
                    {
                        m_tabpageInputWaferSelection.radioButtonFS_AllSlotToRun.Checked = true;
                        m_tabpageInputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked = false;
                        m_tabpageInputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked = false;

                        m_tabpageOutputWaferSelection.radioButtonFS_AllSlotToRun.Checked = true;
                        m_tabpageOutputWaferSelection.radioButtonFS_FromSelectedSlotToRun.Checked = false;
                        m_tabpageOutputWaferSelection.radioButtonFS_SelectedSlotToRun.Checked = false;
                    }

                    tabControlJobPage.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        #endregion

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                LaunchNewLotForm();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
#endif
        #endregion

        #region Form New Lot
        virtual public void CreateFormNewLot()
        {
            //m_formNewLot = new ProductFormNewLot();
        }

        virtual public void SetFormNewLot()
        {

        }

        virtual public void SetFormNewLotVariables()
        {
            //m_formNewLot.productShareVariables = m_ProductShareVariables;
            //m_formNewLot.productProcessEvent = m_ProductProcessEvent;
        }

        virtual public void InitializeFormNewLot()
        {
            //m_formNewLot.Initialize();
        }
        #endregion Form Teach Map

        #region Form Retry Barcode
        virtual public void CreateFormRetryBarcode()
        {
            m_formBarcode = new ProductFormBarcode();
        }

        virtual public void SetFormRetryBarcode()
        {

        }

        virtual public void SetFormRetryBarcodeVariables()
        {
            m_formBarcode.productShareVariables = m_ProductShareVariables;
            m_formBarcode.productProcessEvent = m_ProductProcessEvent;

            m_formBarcode.productStateControl = m_ProductStateControl;
        }

        virtual public void InitializeFormRetryBarcode()
        {
            m_formBarcode.Initialize();
        }
        virtual public void CreateFormConsumableParts()
        {
            m_FormConsumableParts = new ProductFormConsumableParts();
            //m_FormConsumableParts.TopMost = true;
        }

        virtual public void SetFormConsumableParts()
        {

        }

        virtual public void SetFormConsumablePartsVariables()
        {
            m_FormConsumableParts.productShareVariables = m_ProductShareVariables;
            m_FormConsumableParts.productRTSSProcess = m_ProductRTSSProcess;
            m_FormConsumableParts.productProcessEvent = m_ProductProcessEvent;
        }
        virtual public void InitializeConsumableParts()
        {
            m_FormConsumableParts.Initialize("Warning! Some of your parts need to replace or Clean.");
        }
        #endregion Form Teach Map
        #region Low Yield
        virtual public void CreateFormLowYieldParts()
        {
            m_FormLowYieldParts = new ProductFormLowYieldParts();
            //m_FormConsumableParts.TopMost = true;
        }

        virtual public void SetFormLowYieldParts()
        {

        }

        virtual public void SetFormLowYieldPartsVariables()
        {
            //MessageBox.Show("a");
            m_FormLowYieldParts.productProcessEvent = m_ProductProcessEvent;
            m_FormLowYieldParts.productShareVariables = m_ProductShareVariables;
            m_FormLowYieldParts.productRTSSProcess = m_ProductRTSSProcess;
            //m_FormConsumableParts.Show();
        }

        virtual public void InitializeLowYieldParts()
        {
            m_FormLowYieldParts.Initialize("Warning! Some of your parts need to replace or Clean.");
        }
        #endregion Low Yield
        #region Pusher Control
        virtual public void CreateFormPusherControlParts()
        {
            m_FormPusherControl = new ProductFormPusherControl();
            //m_FormConsumableParts.TopMost = true;
        }
        
        virtual public void SetFormPusherControlVariables()
        {
            m_FormPusherControl.productProcessEvent = m_ProductProcessEvent;
            m_FormPusherControl.productShareVariables = m_ProductShareVariables;
            m_FormPusherControl.productRTSSProcess = m_ProductRTSSProcess;
            //m_FormConsumableParts.Show();
        }
        virtual public void InitializePusherControlParts()
        {
            m_FormPusherControl.Initialize();
        }
        #endregion Pusher Control
        #region Auto Collet Calibration
        virtual public void CreateFormAutoColletCalibration()
        {
            m_FormAutoColletCalibration = new ProductFormAutoColletCalibration();
            //m_FormLightingCalibration.TopMost = true;
        }

        virtual public void SetFormAutoColletCalibrationVariables()
        {
            m_FormLightingCalibration.productProcessEvent = m_ProductProcessEvent;
            m_FormLightingCalibration.productShareVariables = m_ProductShareVariables;
            m_FormLightingCalibration.productRTSSProcess = m_ProductRTSSProcess;
        }
        virtual public void InitializeAutoColletCalibration()
        {
            //m_FormLightingCalibration.Initialize();
        }
        #endregion Auto Collet Calibration


        #region Lighting Calibration
        virtual public void CreateFormLightingCalibration()
        {
            m_FormLightingCalibration = new ProductFormAutoForceGaugeCalibration();
            //m_FormLightingCalibration.TopMost = true;
        }

        virtual public void SetFormLightingCalibrationVariables()
        {
            //MessageBox.Show("a");
            m_FormLightingCalibration.productProcessEvent = m_ProductProcessEvent;
            m_FormLightingCalibration.productShareVariables = m_ProductShareVariables;
            m_FormLightingCalibration.productRTSSProcess = m_ProductRTSSProcess;
            //m_FormConsumableParts.Show();
        }
        virtual public void InitializeLightingCalibration()
        {
            //m_FormLightingCalibration.Initialize();
        }
        #endregion Lighting Calibration

        #region Calibration
        virtual public void CreateFormCalibration()
        {
            m_FormCalibration = new ProductFormCalibration();
            //m_FormLightingCalibration.TopMost = true;
        }

        virtual public void SetFormCalibrationVariables()
        {
            //MessageBox.Show("a");
            m_FormCalibration.productProcessEvent = m_ProductProcessEvent;
            m_FormCalibration.productShareVariables = m_ProductShareVariables;
            m_FormCalibration.productRTSSProcess = m_ProductRTSSProcess;
            //m_FormConsumableParts.Show();
        }
        virtual public void InitializeCalibration()
        {
            //m_FormLightingCalibration.Initialize();
        }
        #endregion Lighting Calibration

        #region Move Output Table
        virtual public void CreateFormOutputMotionMove()
        {
            m_FormOutputMotionMove = new ProductionOutputMotionMove();
            //m_FormLightingCalibration.TopMost = true;
        }
        virtual public void SetFormOutputMotionMovesVariables()
        {
            //MessageBox.Show("a");
            m_FormOutputMotionMove.productProcessEvent = m_ProductProcessEvent;
            m_FormOutputMotionMove.productShareVariables = m_ProductShareVariables;
            m_FormOutputMotionMove.productRTSSProcess = m_ProductRTSSProcess;
            //m_FormConsumableParts.Show();
        }

        #endregion

        #region Form Teach Map
        virtual public void CreateFormTeachMap()
        {
            m_formTeachMap = new ProductFormTeachMap();
        }

        virtual public void SetFormTeachMap()
        {

        }

        virtual public void SetFormTeachMapVariables()
        {           
            m_formTeachMap.productShareVariables = m_ProductShareVariables;
            m_formTeachMap.productProcessEvent = m_ProductProcessEvent;

            m_formTeachMap.productStateControl = m_ProductStateControl;
            m_formTeachMap.productRTSSProcess = m_ProductRTSSProcess;
        }

        virtual public void InitializeFormTeachMap()
        {
            m_formTeachMap.Initialize();
        }
        #endregion Form Teach Map
        
        override public void OnChangeToUnknownAuthority()
        {
            toolStripButtonSetupCameraUpDownControl.Visible = false;
            toolStripButtonReTeachMap.Visible = false;
		
            toolStripButtonCycle.Visible = false;
			toolStripButtonMaintainanceCount.Visible = true;
            m_buttonPurgeHalfWayData.Visible = false;

            toolStripButtonReview.Visible = false;
      
            toolStripButtonLowYieldAlarm.Visible = false;

            toolStripButtonPusherControl.Visible = false;
            m_ProductShareVariables.bEnableContinueLot = false;
            panelLeft.Controls.Remove(m_THKPanel);
        }

        override public void OnChangeToOperatorAuthority()
        {
            toolStripButtonSetupCameraUpDownControl.Visible = false;
            toolStripButtonReTeachMap.Visible = false;
	
            toolStripButtonCycle.Visible = false;
            toolStripButtonMaintainanceCount.Visible = true;
            m_buttonPurgeHalfWayData.Visible = false;
    
            toolStripButtonLowYieldAlarm.Visible = false;
      
            toolStripButtonPusherControl.Visible = false;

            toolStripButtonReview.Visible = false;
  
            //toolStripButtonLightingCalibration.Visible = false;
            m_ProductShareVariables.bEnableContinueLot = false;
            panelLeft.Controls.Remove(m_THKPanel);
        }

        override public void OnChangeToTechnicianAuthority()
        {
            toolStripButtonSetupCameraUpDownControl.Visible = true;
            toolStripButtonReTeachMap.Visible = true;

            btnProductionStep.Visible = false;
            toolStripButtonCycle.Visible = true;
            toolStripButtonMaintainanceCount.Visible = true;
            m_buttonPurgeHalfWayData.Visible = true;

            toolStripButtonReview.Visible = true;

            toolStripButtonLowYieldAlarm.Visible = false;

            toolStripButtonPusherControl.Visible = false;

            //toolStripButtonLightingCalibration.Visible = true;
            m_ProductShareVariables.bEnableContinueLot = false;
            panelLeft.Controls.Remove(m_THKPanel);
        }

        override public void OnChangeToEngineerAuthority()
        {
            toolStripButtonSetupCameraUpDownControl.Visible = true;
            toolStripButtonReTeachMap.Visible = true;

            btnProductionStep.Visible = false;
            toolStripButtonCycle.Visible = true;
            toolStripButtonMaintainanceCount.Visible = true;
            m_buttonPurgeHalfWayData.Visible = true;

            toolStripButtonReview.Visible = true;

            toolStripButtonLowYieldAlarm.Visible = false;

            toolStripButtonPusherControl.Visible = false;
            m_ProductShareVariables.bEnableContinueLot = false;

            //toolStripButtonLightingCalibration.Visible = true;
            panelLeft.Controls.Add(m_THKPanel);
            m_THKPanel.Dock = DockStyle.Left;
            m_THKPanel.BringToFront();
        }

        override public void OnChangeToAdministratorAuthority()
        {
            toolStripButtonSetupCameraUpDownControl.Visible = true;
            toolStripButtonReTeachMap.Visible = true;

            btnProductionStep.Visible = false;
            toolStripButtonCycle.Visible = true;
            toolStripButtonMaintainanceCount.Visible = true;
            m_buttonPurgeHalfWayData.Visible = true;

            toolStripButtonLowYieldAlarm.Visible = true;

            toolStripButtonPusherControl.Visible = true;

            toolStripButtonReview.Visible = true;
            m_ProductShareVariables.bEnableContinueLot = false;

            //toolStripButtonLightingCalibration.Visible = true;

            panelLeft.Controls.Add(m_THKPanel);
            m_THKPanel.Dock = DockStyle.Left;
            m_THKPanel.BringToFront();
        }

        override public void OnChangeToVendorAuthority()
        {
            toolStripButtonSetupCameraUpDownControl.Visible = true;
            toolStripButtonReTeachMap.Visible = true;

            btnProductionStep.Visible = false;
            //toolStripButtonCycle.Visible = true;
            toolStripButtonMaintainanceCount.Visible = true;
            m_buttonPurgeHalfWayData.Visible = true;

            toolStripButtonLowYieldAlarm.Visible = false;

            toolStripButtonPusherControl.Visible = false;

            //toolStripButtonLightingCalibration.Visible = true;
            m_ProductShareVariables.bEnableContinueLot = false;

            panelLeft.Controls.Add(m_THKPanel);
            m_THKPanel.Dock = DockStyle.Left;
            m_THKPanel.BringToFront();
        }

        override public void OnChangeToSoftwareDesignerAuthority()
        {
            toolStripButtonSetupCameraUpDownControl.Visible = true;
            toolStripButtonReTeachMap.Visible = true;

            btnProductionStep.Visible = true;
            //toolStripButtonCycle.Visible = true;
            toolStripButtonMaintainanceCount.Visible = true;
            m_buttonPurgeHalfWayData.Visible = true;

            toolStripButtonLowYieldAlarm.Visible = true;

            toolStripButtonPusherControl.Visible = true;
            m_ProductShareVariables.bEnableContinueLot = false;

            //toolStripButtonLightingCalibration.Visible = true;
            panelLeft.Controls.Add(m_THKPanel);
            m_THKPanel.Dock = DockStyle.Left;
            m_THKPanel.BringToFront();
        }
        public override int OnClickStopButton()
        {
            int nError = 0;
            nError = base.OnClickStopButton();
            if (nError != 0)
            {
                return nError;
            }
            m_ProductProcessEvent.PCS_GUI_Close_Remaining_Active_Form.Set();
            return nError;
        }
        override public void OnRecipeChangeDone(string recipeName)
        {
            //m_bUpdateFrameSelection = true;
            m_ProductProcessEvent.PCS_PCS_Need_Vision_Setting.Set();
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (comboBoxRecipe.SelectedIndex != -1)
                {
                    Task DownloadMainRecipeTaks = Task.Run(() => DownloadMainRecipe(recipeName));

                }
            }
            //m_ProductShareVariables.RegKey.SetJobDataKeyParameter("Recipe", m_ProductShareVariables.strucInputProductInfo.Recipe);
        }

        virtual public void DownloadMainRecipe(string RecipePath)
        {
            List<string> m_ListOfRecipePath = new List<string>();
            ProductRecipeMainSettings m_RecipeMain = new ProductRecipeMainSettings();
            string pathName = "";

            if (Machine.Platform.SecsgemControl.DownloadRecipe(RecipePath) != 0)//DownloadRecipe(string RecipeFullPath)
            {
                updateRichTextBoxLogDisplay("Download Main Recipe From Secgem Host Fail");
            }
            else
            {
                m_RecipeMain = Tools.Deserialize<ProductRecipeMainSettings>(RecipePath);
                if (m_RecipeMain.InputRecipeName != "")
                {
                    pathName = @"D:\Estek\Recipe\Tray" + @"\" + m_RecipeMain.InputRecipeName + ".xml";
                    m_ListOfRecipePath.Add(pathName);
                }
                //if (m_RecipeMain.OutputRecipeName != "")
                //{
                //    pathName = @"D:\Estek\Recipe\Output" + @"\" + m_RecipeMain.OutputRecipeName + ".xml";
                //    m_ListOfRecipePath.Add(pathName);
                //}
                if (m_RecipeMain.DelayRecipeName != "")
                {
                    pathName = @"D:\Estek\Recipe\Delay" + @"\" + m_RecipeMain.DelayRecipeName + ".xml";
                    m_ListOfRecipePath.Add(pathName);
                }
                if (m_RecipeMain.MotorPositionRecipeName != "")
                {
                    pathName = @"D:\Estek\Recipe\MotorPosition" + @"\" + m_RecipeMain.MotorPositionRecipeName + ".xml";
                    m_ListOfRecipePath.Add(pathName);
                }
                if (m_RecipeMain.OutputFileRecipeName != "")
                {
                    pathName = @"D:\Estek\Recipe\OutputFile" + @"\" + m_RecipeMain.OutputFileRecipeName + ".xml";
                    m_ListOfRecipePath.Add(pathName);
                }
                //if (m_RecipeMain.InputCassetteRecipeName != "")
                //{
                //    pathName = @"D:\Estek\Recipe\Cassette" + @"\" + m_RecipeMain.InputCassetteRecipeName + ".xml";
                //    m_ListOfRecipePath.Add(pathName);
                //}
                if (m_RecipeMain.VisionRecipeName != "")
                {
                    pathName = @"D:\Estek\Recipe\Vision" + @"\" + m_RecipeMain.VisionRecipeName + ".xml";
                    m_ListOfRecipePath.Add(pathName);
                }
                if (m_RecipeMain.InspectionRecipeName != "")
                {
                    pathName = @"D:\Estek\Recipe\Inspection" + @"\" + m_RecipeMain.InspectionRecipeName + ".xml";
                    m_ListOfRecipePath.Add(pathName);
                }
            }

            if (Machine.Platform.SecsgemControl.DownloadMainRecipe(m_ListOfRecipePath) != 0)
            {
                updateRichTextBoxLogDisplay("Download Sub-recipe From Secgem Host Fail");
            }
            else
            {
                m_bUpdateFrameSelection = true;
                m_ProductProcessEvent.GUI_PCS_UpdateSettingShareMemory.Set();
                updateRichTextBoxLogDisplay("Download Sub-recipe From Secgem Host Done");
            }
        }

        override public void OnClearAlarmDone(string alarmID)
        {
            //m_ProductShareVariables.bAlarmStart = false;
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                Task ClearAlarmTask = Task.Run(() => ClearAlarm(alarmID));
            }
        }

        virtual public void OnLoadFrameOrTile()
        {
            #region Secsgem
            Task TriggerEventLoadFrameOrTileTask = Task.Run(() => TriggerEventLoadFrameOrTile());
            #endregion Secsgem
        }
        
        void TriggerEventLoadFrameOrTile()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.SetEventLoadFrameOrTile() != 0)
                {
                    Machine.SequenceControl.SetAlarm(7005);
                }
            }
        }
        
        virtual public double GetFHIndex(int currentCycle)
        {
            double currentCycleIn8 = currentCycle % 8;
            return currentCycleIn8 / 2.0 + 1.0;
        }

        virtual public double RoundTo4(double indexNo)
        {
            if (indexNo >= 5.0)
                indexNo -= 4.0;
            return indexNo;
        }

        override public void OnDryRunModeOn()
        {
            m_ProductProcessEvent.PCS_PCS_Send_Vision_DryRun_Mode_On.Set();
        }

        override public void OnDryRunModeOff()
        {
            m_ProductProcessEvent.PCS_PCS_Send_Vision_DryRun_Mode_Off.Set();
        }
        
        override public int OnAlarmAssist()
        {
            m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_ALARM_ASSIST_START", true);

            //if(alarmID == 0)
            //{
            //    reportProcess.AddRecord(new ReportProcess.Record { dateTime = DateTime.Now, eventID = m_ProductShareVariables.productReportEvent.EventUnloadMaterialTime.EventID, eventName = m_ProductShareVariables.productReportEvent.EventUnloadMaterialTime.EventName, lotID = m_ProductShareVariables.LotID, alarmID = 0, alarmType = 0 });
            //}
            return 0;
        }

        override public int OnAlarmMessage()
        {
            m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_ALARM_MESSAGE_START", true);
            return 0;
        }
        override public int OnAlarmFailure()
        {
            m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_ALARM_FAILURE_START", true);
            return 0;
        }

        override public int OnAlarmAssist(int alarmID, int alarmType)
        {
            int nError = 0;
            base.OnAlarmAssist(alarmID, alarmType);
            m_ProductShareVariables.CurrentDownTimeCounterMES.Add(new MoveonMESAPI.DownTime {dt_code = alarmID.ToString(),stime=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), });
            m_ProductShareVariables.CurrentDownTimeNo++;
            m_ProductShareVariables.AlarmStart = 1;
            return nError;
        }
        override public int OnAlarmMessage(int alarmID, int alarmType)
        {
            int nError = 0;
            base.OnAlarmMessage(alarmID, alarmType);
            return nError;
        }
        override public int OnAlarmFailure(int alarmID, int alarmType)
        {
            int nError = 0;
            base.OnAlarmFailure(alarmID, alarmType);
            m_ProductShareVariables.CurrentDownTimeCounterMES.Add(new MoveonMESAPI.DownTime { dt_code = alarmID.ToString(), stime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), });
            m_ProductShareVariables.CurrentDownTimeNo++;
            m_ProductShareVariables.AlarmStart = 1;
            return nError;
        }
        
        #endregion Public



        private void timerMaintainenceCount_Tick(object sender, EventArgs e)
        {
            if (m_ProductProcessEvent.PCS_GUI_Due_Count_Alarm.WaitOne(0))
            {
                if (toolStripButtonMaintainanceCount.BackColor != Color.Red)
                    toolStripButtonMaintainanceCount.BackColor = Color.Red;

                else 
                    toolStripButtonMaintainanceCount.BackColor = Color.LightCyan;
            }
            //if (m_ProductProcessEvent.PCS_GUI_Clean_Count_Alarm.WaitOne(0) && !m_ProductProcessEvent.PCS_GUI_Warning_Count_Alarm.WaitOne(0) && !m_ProductProcessEvent.PCS_GUI_Due_Count_Alarm.WaitOne(0))
            else if (m_ProductProcessEvent.PCS_GUI_Warning_Count_Alarm.WaitOne(0))
            {
                if (toolStripButtonMaintainanceCount.BackColor != Color.Orange)
                    toolStripButtonMaintainanceCount.BackColor = Color.Orange;

                else
                    toolStripButtonMaintainanceCount.BackColor = Color.LightCyan;
            }
            else if (m_ProductProcessEvent.PCS_GUI_Clean_Count_Alarm.WaitOne(0))
            {
                if (toolStripButtonMaintainanceCount.BackColor != Color.Yellow)
                    toolStripButtonMaintainanceCount.BackColor = Color.Yellow;

                else 
                    toolStripButtonMaintainanceCount.BackColor = Color.LightCyan;
            }
           
            else
                toolStripButtonMaintainanceCount.BackColor = Color.LightCyan;
        }

        private void RefreshRecipeListForCalibration()
        {
            m_ProductShareVariables.RecipeListForCalibration.Clear();
            foreach(string Recipe in comboBoxRecipe.Items)
            {
                m_ProductShareVariables.RecipeListForCalibration.Add(Recipe);
            }
        }

        private void InitializeComponent()
        {
            this.panelJob.SuspendLayout();
            this.groupBoxPerformance.SuspendLayout();
            this.groupBoxSummary.SuspendLayout();
            this.groupBoxUserInput.SuspendLayout();
            this.panelAlarm.SuspendLayout();
            this.groupBoxInputXYTable.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelLogDisplay.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelUserInput.SuspendLayout();
            this.panelLogDisplayHeader.SuspendLayout();
            this.tabControlJobPage.SuspendLayout();
            this.panelOption.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelJob
            // 
            this.panelJob.Size = new System.Drawing.Size(1461, 884);
            // 
            // panelAlarm
            // 
            this.panelAlarm.Location = new System.Drawing.Point(0, 562);
            this.panelAlarm.Size = new System.Drawing.Size(1461, 127);
            // 
            // labelAlarmMessage
            // 
            this.labelAlarmMessage.Size = new System.Drawing.Size(1461, 79);
            // 
            // groupBoxInputXYTable
            // 
            this.groupBoxInputXYTable.Size = new System.Drawing.Size(251, 167);
            // 
            // buttonNewLot
            // 
            this.buttonNewLot.Enabled = false;
            this.buttonNewLot.Size = new System.Drawing.Size(100, 71);
            // 
            // panelLogDisplay
            // 
            this.panelLogDisplay.Location = new System.Drawing.Point(0, 689);
            this.panelLogDisplay.Size = new System.Drawing.Size(1461, 100);
            // 
            // richTextBoxLogDisplay
            // 
            this.richTextBoxLogDisplay.Size = new System.Drawing.Size(1461, 75);
            // 
            // panelRight
            // 
            this.panelRight.Location = new System.Drawing.Point(1204, 87);
            this.panelRight.Size = new System.Drawing.Size(257, 475);
            // 
            // panelLeft
            // 
            this.panelLeft.Size = new System.Drawing.Size(295, 475);
            // 
            // panelUserInput
            // 
            this.panelUserInput.Size = new System.Drawing.Size(1461, 87);
            // 
            // panelLogDisplayHeader
            // 
            this.panelLogDisplayHeader.Size = new System.Drawing.Size(1461, 25);
            // 
            // tabControlJobPage
            // 
            this.tabControlJobPage.Size = new System.Drawing.Size(909, 475);
            // 
            // tabPageMainPage
            // 
            this.tabPageMainPage.Size = new System.Drawing.Size(901, 442);
            // 
            // ProductFormJob
            // 
            this.ClientSize = new System.Drawing.Size(1556, 884);
            this.Name = "ProductFormJob";
            this.panelJob.ResumeLayout(false);
            this.groupBoxPerformance.ResumeLayout(false);
            this.groupBoxPerformance.PerformLayout();
            this.groupBoxSummary.ResumeLayout(false);
            this.groupBoxUserInput.ResumeLayout(false);
            this.groupBoxUserInput.PerformLayout();
            this.panelAlarm.ResumeLayout(false);
            this.panelAlarm.PerformLayout();
            this.groupBoxInputXYTable.ResumeLayout(false);
            this.groupBoxInputXYTable.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelLogDisplay.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelUserInput.ResumeLayout(false);
            this.panelLogDisplayHeader.ResumeLayout(false);
            this.panelLogDisplayHeader.PerformLayout();
            this.tabControlJobPage.ResumeLayout(false);
            this.panelOption.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
