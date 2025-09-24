using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Product
{
    public partial class ProductFormTeachPointGuide : Form
    {
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess(); 
        private ProductProcessEvent m_ProductProcessEvent;//= new ProductProcessEvent();
        private ProductFormTeachPoint m_ProductFormTeachPoint;

        BackgroundWorker m_bgwUserInterface;
        public ProductProcessEvent productProcessEvent
        {
            set
            {
                m_ProductProcessEvent = value;
            }
        }

        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
            }
        }

        string m_strmode="TeachPointGuide";
        int nNoOfSnap = 0;
        bool IsUnitIn30Degree = false;
        bool IsResetPressed = false;
        bool IsOneStepButtonPressed = false;
        public ProductFormTeachPointGuide()
        {
            InitializeComponent();
            
            IsUnitIn30Degree = false;
            IsResetPressed = false;
            buttonCenterRotationVisionSnap.Enabled = false;
            buttonCenterRotationRotate30Degree.Enabled = false;
            buttonCenterRotationVisionReSnap.Enabled = false;
            buttonCenterRotationGetData.Enabled = false;
            buttonCenterRotationResetAll.Enabled = false;
            buttonCenterRotationRotateNegative30Degree.Enabled = false;
            buttonCenterRotationMove.Enabled = false;
            buttonCenterRotationOneStep.Enabled = false;
            buttonCenterRotationVisionSnap.Click += ButtonCenterRotationVisionSnap_Click;
            buttonCenterRotationRotate30Degree.Click += ButtonCenterRotationRotate30Degree_Click;
            buttonCenterRotationVisionReSnap.Click += ButtonCenterRotationVisionReSnap_Click;
            buttonCenterRotationGetData.Click += ButtonCenterRotationGetData_Click;
            buttonCenterRotationRotateNegative30Degree.Click += ButtonCenterRotationRotateNegative30Degree_Click;
            buttonCenterRotationResetAll.Click += ButtonCenterRotationResetAll_Click;
            buttonCenterRotationMove.Click += ButtonCenterRotationMove_Click;
            buttonCenterRotationOneStep.Click += ButtonOneStep_Click;
            buttonFormClose.Click += ButtonFormClose_Click;
            buttonTurretIndex.Click += ButtonTurretIndex_Click;
            buttonTurretStandbyPosition.Click += ButtonTurretStandbyPosition_Click;
            m_bgwUserInterface = new BackgroundWorker();
            m_bgwUserInterface.WorkerReportsProgress = true;
            m_bgwUserInterface.WorkerSupportsCancellation = true;
            m_bgwUserInterface.DoWork += new DoWorkEventHandler(bgwUserInterface_DoWork);
            m_bgwUserInterface.ProgressChanged += new ProgressChangedEventHandler(bgwUserInterface_UpdateInterface);
            m_bgwUserInterface.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwUserInterface_Complete);

            m_bgwUserInterface.RunWorkerAsync();
        }
        
        public void bgwUserInterface_DoWork(Object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            int nProgress = 0;

            try
            {
                while (true)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        if (nProgress == 10)
                            nProgress = 0;
                        nProgress += 1;

                        worker.ReportProgress(nProgress);
                        Thread.Sleep(1);
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        public void bgwUserInterface_UpdateInterface(Object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Initialize_ACK.WaitOne(0) || m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Initialize_ACK.WaitOne(0))
                {
                    buttonCenterRotationVisionSnap.Enabled = true;
                    buttonCenterRotationRotate30Degree.Enabled = false;
                    buttonCenterRotationVisionReSnap.Enabled = false;
                    buttonCenterRotationGetData.Enabled = false;
                    buttonCenterRotationMove.Enabled = false;
                    buttonCenterRotationResetAll.Enabled = false;
                    buttonCenterRotationRotateNegative30Degree.Enabled = false;
                    buttonCenterRotationOneStep.Enabled = true;
                    IsUnitIn30Degree = false;
                    IsResetPressed = false;
                    IsOneStepButtonPressed = false;
                }
                if (m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Initialize_NAK.WaitOne(0) || m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Initialize_NAK.WaitOne(0))
                {
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Error: Aligner Vision SemiAuto Teach fail to Initialize." + Environment.NewLine);
                }
                if (m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Snap_ACK.WaitOne(0) || m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Snap_ACK.WaitOne(0))
                {
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_DONE", false);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_FAIL", false);
                    m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", 1);
                    m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", true);
                    m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", true);
                }
                if (m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Snap2_ACK.WaitOne(0) || m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Snap2_ACK.WaitOne(0))
                {
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_DONE", false);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_FAIL", false);
                    m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", 1);
                    m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", true);
                    m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", true);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_DONE") == true)
                {
                    if (nNoOfSnap == 1)
                    {
                        TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: SemiAuto Teach First Snap Done." + Environment.NewLine);
                        if (IsOneStepButtonPressed == false)
                        {
                            buttonCenterRotationVisionSnap.Enabled = false;
                            buttonCenterRotationRotate30Degree.Enabled = true;
                            buttonCenterRotationVisionReSnap.Enabled = false;
                            buttonCenterRotationGetData.Enabled = false;
                            buttonCenterRotationResetAll.Enabled = true;
                            
                        }
                        else
                        {
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_DONE", false);
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_FAIL", false);
                            m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", 2);// 2 for rotate 30 degree
                            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", true);
                            m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", true);
                            TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: Start Rotate 30 Degree." + Environment.NewLine);
                        }
                    }
                    else if (nNoOfSnap == 2)
                    {
                        TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: SemiAuto Teach Second Snap Done." + Environment.NewLine);
                        if (IsOneStepButtonPressed == false)
                        {
                            buttonCenterRotationVisionSnap.Enabled = false;
                            buttonCenterRotationRotate30Degree.Enabled = false;
                            buttonCenterRotationVisionReSnap.Enabled = false;
                            buttonCenterRotationGetData.Enabled = true;
                        }
                        else
                        {
                            TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: Start getting Result From Vision." + Environment.NewLine);
                            if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
                            {
                                m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_GetResult.Set();
                            }
                            else if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.WaitOne(0))
                            {
                                m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_GetResult.Set();
                            }
                        }
                    }
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_DONE", false);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_FAIL") == true)
                {
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Error: Vision fail to Snap Photo in SemiAuto Teach." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_FAIL", false);
                    if (nNoOfSnap == 1)
                    {
                        buttonCenterRotationVisionSnap.Enabled = true;
                    }
                    else if (nNoOfSnap == 2)
                    {
                        buttonCenterRotationVisionReSnap.Enabled = true;
                    }
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_DONE") == true)
                {
                    if (IsOneStepButtonPressed == false)
                    {
                        IsUnitIn30Degree = true;
                        buttonCenterRotationRotateNegative30Degree.Enabled = true;
                        buttonCenterRotationVisionSnap.Enabled = false;
                        buttonCenterRotationRotate30Degree.Enabled = false;
                        buttonCenterRotationVisionReSnap.Enabled = true;
                        buttonCenterRotationGetData.Enabled = false;
                    }
                    else
                    {
                        nNoOfSnap = 2;
                        TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: Vision Start Snap 2." + Environment.NewLine);
                        if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
                        {
                            m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Snap2.Set();
                        }
                        else if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.WaitOne(0))
                        {
                            m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Snap2.Set();
                        }
                    }
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: Rotate 30 Degree Done in SemiAuto Teach." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_DONE", false);
                    
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_FAIL") == true)
                {
                    buttonCenterRotationRotate30Degree.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Error: Fail to Rotate 30 Degree in SemiAuto Teach." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_FAIL", false);
                }
                if (m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_GetResult_Done.WaitOne(0))
                {
                    labelRotationCenterPositionXOffset.Text = m_ProductRTSSProcess.GetTeachPointLong("AlignerCenterPositionXOffset").ToString();
                    labelRotationCenterPositionYOffset.Text = m_ProductRTSSProcess.GetTeachPointLong("AlignerCenterPositionYOffset").ToString();
                    labelXPositionAfterOffset.Text = (m_ProductRTSSProcess.GetTeachPointLong("AlignerXAxisCenterPosition") - m_ProductRTSSProcess.GetTeachPointLong("AlignerCenterPositionXOffset")).ToString();
                    labelYPositionAfterOffset.Text = (m_ProductRTSSProcess.GetTeachPointLong("AlignerYAxisCenterPosition") + m_ProductRTSSProcess.GetTeachPointLong("AlignerCenterPositionYOffset")).ToString();
                    buttonCenterRotationVisionSnap.Enabled = true;
                    buttonCenterRotationOneStep.Enabled = true;
                    buttonCenterRotationMove.Enabled = true;
                    buttonCenterRotationRotate30Degree.Enabled = false;
                    buttonCenterRotationVisionReSnap.Enabled = false;
                    buttonCenterRotationGetData.Enabled = false;
                    buttonCenterRotationResetAll.Enabled = true;
                    buttonCenterRotationRotateNegative30Degree.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: Aligner SemiAuto Teach get result done." + Environment.NewLine);
                }
                if (m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_GetResult_Done.WaitOne(0))
                {
                    labelRotationCenterPositionXOffset.Text = m_ProductRTSSProcess.GetTeachPointLong("InputTableCenterPositionXOffset").ToString();
                    labelRotationCenterPositionYOffset.Text = m_ProductRTSSProcess.GetTeachPointLong("InputTableCenterPositionYOffset").ToString();
                    labelXPositionAfterOffset.Text = (m_ProductRTSSProcess.GetTeachPointLong("InputTableXAxisCenterPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableCenterPositionXOffset")).ToString();
                    labelYPositionAfterOffset.Text = (m_ProductRTSSProcess.GetTeachPointLong("InputTableYAxisCenterPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableCenterPositionYOffset")).ToString();
                    buttonCenterRotationVisionSnap.Enabled = true;
                    buttonCenterRotationOneStep.Enabled = true;
                    buttonCenterRotationMove.Enabled = true;
                    buttonCenterRotationRotate30Degree.Enabled = false;
                    buttonCenterRotationVisionReSnap.Enabled = false;
                    buttonCenterRotationGetData.Enabled = false;
                    buttonCenterRotationResetAll.Enabled = true;
                    buttonCenterRotationRotateNegative30Degree.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: Input Table SemiAuto Teach get result done." + Environment.NewLine);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_DONE") == true)
                {
                    IsUnitIn30Degree = false;
                    if (IsResetPressed == true)
                    {
                        buttonCenterRotationVisionSnap.Enabled = true;
                        buttonCenterRotationOneStep.Enabled = true;
                        buttonCenterRotationMove.Enabled = false;
                        buttonCenterRotationRotate30Degree.Enabled = false;
                        buttonCenterRotationVisionReSnap.Enabled = false;
                        buttonCenterRotationGetData.Enabled = false;
                        buttonCenterRotationResetAll.Enabled = false;
                        buttonCenterRotationRotateNegative30Degree.Enabled = false;
                        TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: Aligner SemiAuto Teach Reset done." + Environment.NewLine);
                    }
                    else
                    {
                        buttonCenterRotationVisionSnap.Enabled = true;
                        buttonCenterRotationRotate30Degree.Enabled = true;
                        buttonCenterRotationVisionReSnap.Enabled = true;
                        buttonCenterRotationGetData.Enabled = false;
                        TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: Aligner SemiAuto Teach rotate Negative 30 Degree done." + Environment.NewLine);
                    }
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_DONE", false);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_FAIL") == true)
                {
                    buttonCenterRotationRotateNegative30Degree.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Error: Fail to Rotate Negative 30 Degree in SemiAuto Teach." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_FAIL", false);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_MOVE_OFFSET_DONE") == true)
                {
                    buttonCenterRotationMove.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: SemiAuto Teach Move Offset Done." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_MOVE_OFFSET_DONE", false);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_MOVE_OFFSET_FAIL") == true)
                {
                    buttonCenterRotationMove.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Error: SemiAuto Teach Move Offset Fail." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_MOVE_OFFSET_FAIL", false);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_INDEX_DONE") == true)
                {
                    buttonTurretIndex.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: SemiAuto Turret Index Done." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_INDEX_DONE", false);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_INDEX_FAIL") == true)
                {
                    buttonTurretIndex.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Error: SemiAuto Turret Index Fail." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_INDEX_FAIL", false);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_STANDBY_POSITION_DONE") == true)
                {
                    buttonTurretStandbyPosition.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: SemiAuto Turret Move To Standby Position Done." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_STANDBY_POSITION_DONE", false);
                }
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_STANDBY_POSITION_FAIL") == true)
                {
                    buttonTurretStandbyPosition.Enabled = true;
                    TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Error: SemiAuto Turret Move To Standby Position Fail." + Environment.NewLine);
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_STANDBY_POSITION_FAIL", false);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        public void bgwUserInterface_Complete(Object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled == true)
                {
                    Machine.EventLogger.WriteLog(string.Format("{0} Form properly close at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                }
                else if (!(e.Error == null))
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0} bgwUserInterface Error at {1}." + e.Error.Message, DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                }
                else
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0} bgwUserInterface Done at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                }
                //ProcessEvent.PCS_PCS_Set_Vision_Close_Teach_Window.Set();
                Close();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void ButtonFormClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to close this form?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
                {
                    m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_End.Set();
                }
                else if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.WaitOne(0))
                {
                    m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_End.Set();
                }
                //Close();
                if (m_bgwUserInterface.WorkerSupportsCancellation == true)
                {
                    m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.Reset();
                    m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.Reset();
                    m_bgwUserInterface.CancelAsync();
                }
            }
        }
        private void ButtonCenterRotationVisionSnap_Click(object sender, EventArgs e)
        {
            nNoOfSnap = 1;
            buttonCenterRotationVisionSnap.Enabled = false;
            buttonCenterRotationRotate30Degree.Enabled = false;
            buttonCenterRotationVisionReSnap.Enabled = false;
            buttonCenterRotationGetData.Enabled = false;
            TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Vision Snap 1." + Environment.NewLine);
            if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Snap.Set();
            }
            else if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Snap.Set();
            }
        }
        private void ButtonCenterRotationRotate30Degree_Click(object sender, EventArgs e)
        {
            IsUnitIn30Degree = false;
            buttonCenterRotationVisionSnap.Enabled = false;
            buttonCenterRotationRotate30Degree.Enabled = false;
            buttonCenterRotationVisionReSnap.Enabled = false;
            buttonCenterRotationGetData.Enabled = false;
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_DONE", false);
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_FAIL", false);
            m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", 2);// 2 for rotate 30 degree
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", true);
            m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", true);
            TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Rotate 30 Degree." + Environment.NewLine);
            //m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Rotate_30_Start.Set();
        }
        private void ButtonCenterRotationVisionReSnap_Click(object sender, EventArgs e)
        {
            buttonCenterRotationVisionSnap.Enabled = false;
            buttonCenterRotationRotate30Degree.Enabled = false;
            buttonCenterRotationVisionReSnap.Enabled = false;
            buttonCenterRotationGetData.Enabled = false;
            nNoOfSnap = 2;
            TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Vision Snap 2." + Environment.NewLine);
            if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Snap2.Set();
            }
            else if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Snap2.Set();
            }
        }
        private void ButtonCenterRotationGetData_Click(object sender, EventArgs e)
        {
            buttonCenterRotationVisionSnap.Enabled = false;
            buttonCenterRotationRotate30Degree.Enabled = false;
            buttonCenterRotationVisionReSnap.Enabled = false;
            buttonCenterRotationGetData.Enabled = false;
            TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Get Result From Vision." + Environment.NewLine);
            if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_GetResult.Set();
            }
            else if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_GetResult.Set();
            }
        }
        private void ButtonCenterRotationResetAll_Click(object sender, EventArgs e)
        {
            if (IsUnitIn30Degree == true)
            {
                IsResetPressed = true;
                buttonCenterRotationVisionSnap.Enabled = false;
                buttonCenterRotationRotate30Degree.Enabled = false;
                buttonCenterRotationVisionReSnap.Enabled = false;
                buttonCenterRotationGetData.Enabled = false;
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_DONE", false);
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_FAIL", false);
                m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", 3);// 3 for rotate Negative 30 degree
                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", true);
                m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", true);
                TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Reset Button and waiting to Rotate Negative 30 Degree." + Environment.NewLine);
            }
            else
            {
                buttonCenterRotationVisionSnap.Enabled = true;
                buttonCenterRotationOneStep.Enabled = true;
                buttonCenterRotationMove.Enabled = false;
                buttonCenterRotationRotate30Degree.Enabled = false;
                buttonCenterRotationVisionReSnap.Enabled = false;
                buttonCenterRotationGetData.Enabled = false;
                buttonCenterRotationResetAll.Enabled = false;
                buttonCenterRotationRotateNegative30Degree.Enabled = false;
                TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Reset Button." + Environment.NewLine);
            }
        }
        private void ButtonCenterRotationRotateNegative30Degree_Click(object sender, EventArgs e)
        {
            buttonCenterRotationRotateNegative30Degree.Enabled = false;
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_DONE", false);
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_FAIL", false);
            m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", 3);// 3 for rotate Negative 30 degree
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", true);
            m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", true);
            TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Rotate Negative 30 Degree." + Environment.NewLine);
        }
        private void ButtonCenterRotationMove_Click(object sender, EventArgs e)
        {
            buttonCenterRotationMove.Enabled = false;
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_MOVE_OFFSET_DONE", false);
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_MOVE_OFFSET_FAIL", false);
            m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", 4);// 4 for Move Offset
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", true);
            m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", true);
            TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Move to Position." + Environment.NewLine);
        }
        private void ButtonOneStep_Click(object sender, EventArgs e)
        {
            nNoOfSnap = 1;
            IsOneStepButtonPressed = true;
            buttonCenterRotationVisionSnap.Enabled = false;
            buttonCenterRotationRotate30Degree.Enabled = false;
            buttonCenterRotationVisionReSnap.Enabled = false;
            buttonCenterRotationGetData.Enabled = false;
            buttonCenterRotationMove.Enabled = false;
            buttonCenterRotationResetAll.Enabled = false;
            buttonCenterRotationRotateNegative30Degree.Enabled = false;
            buttonCenterRotationOneStep.Enabled = false;
            TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Vision Snap 1." + Environment.NewLine);
            if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Snap.Set();
            }
            else if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Snap.Set();
            }
        }
        private void ButtonTurretStandbyPosition_Click(object sender, EventArgs e)
        {
            if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                buttonTurretStandbyPosition.Enabled = false;
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_STANDBY_POSITION_DONE", false);
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_STANDBY_POSITION_FAIL", false);
                m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", 5);
                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", true);
                m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", true);
                TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Move Turret to Standby Position." + Environment.NewLine);
            }
        }

        private void ButtonTurretIndex_Click(object sender, EventArgs e)
        {
            if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
            {
                buttonCenterRotationMove.Enabled = false;
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_INDEX_DONE", false);
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TURRET_INDEX_FAIL", false);
                m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", 6);
                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", true);
                m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", true);
                TeachPointGuideMessageTextBox.AppendText($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Process: User Pressed Turret Index." + Environment.NewLine);
            }
        }
        private void buttonTeachPosittion_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("", true);
        }
    }
}
