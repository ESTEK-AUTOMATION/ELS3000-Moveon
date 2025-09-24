using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Common;
using MotionLibrary;

namespace Product
{
    public class ProductFormTeachPoint : Machine.FormTeachPoint
    {
        private ProductShareVariables m_ProductShareVariables;
        private ProductProcessEvent m_ProductProcessEvent;
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess(); 

        public ProductTeachPointSettings m_ProductTeachPointSettings = new ProductTeachPointSettings();
        
        public ProductFormTeachPointGuide productFormTeachPointGuide;// = new ProductFormTeachPointGuide();
                                                                  
        public System.Windows.Forms.Label labelTitleTHKPressureValue = new System.Windows.Forms.Label();
        public System.Windows.Forms.Label labelTitleTHKForceValue = new System.Windows.Forms.Label();
        public System.Windows.Forms.Label labelTitleTHKFlowRate = new System.Windows.Forms.Label();
        public System.Windows.Forms.Label labelTHKPressureValue = new System.Windows.Forms.Label();
        public System.Windows.Forms.Label labelTHKForceValue = new System.Windows.Forms.Label();
        public System.Windows.Forms.Label labelTHKFlowRate = new System.Windows.Forms.Label();

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

        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
                rtssProcess = m_ProductRTSSProcess;
            }
        }

        public ProductTeachPointSettings productTeachPointSettings
        {
            set
            {
                m_ProductTeachPointSettings = value;
                teachPointSettings = m_ProductTeachPointSettings;
            }
        }

        override public bool LoadSettings()
        {
            try
            {
                if (File.Exists(m_strTeachPointPath + m_strFile))
                {
                    m_ProductTeachPointSettings = Tools.Deserialize<ProductTeachPointSettings>(m_strTeachPointPath + m_strFile);
                    base.m_TeachPointSettings = m_ProductTeachPointSettings;
                    m_listMotorProfile.Clear();

                    if (AddAllTeachpoint() == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Teach Point file not exist." + Environment.NewLine);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        override public bool SaveSettings()
        {
            try
            {
                Tools.Serialize(m_strTeachPointPath + m_strFile, m_ProductTeachPointSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        override public int UpdateAllTeachpoint()
        {
            int nError = 0;
            FieldInfo[] fields = typeof(ProductTeachPointSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo _fields in fields)
            {
                foreach (MotionLibrary.Motion.MotorProfile _motionProfile in m_listMotorProfile)
                {
                    if (_fields.FieldType.Name == "MotorProfile")
                    {
                        if (((MotionLibrary.Motion.MotorProfile)_fields.GetValue(m_ProductTeachPointSettings)).Index == _motionProfile.Index)
                        {
                            _fields.SetValue(m_ProductTeachPointSettings, _motionProfile);
                            break;
                        }
                    }
                }
            }


            #region Turret Application
            //#if TurretApplication
            //                UpdateListTeachPoint(ref m_TeachPointSettings.TurretHomeOffsetPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.InputPusherTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.MarkingVisionPusherTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.RotaryAndAlignerAPusherTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.RotaryAndAlignerBPusherTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.FiveSPusherTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.SortingPusherTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.TapeAndReelPusherTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.UnitSingulatorMotorRetractPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.UnitSingulatorMotorExtendPosition);

            //                UpdateListTeachPoint(ref m_TeachPointSettings.MarkingVisionMotorHomeOffsetPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.RotaryAndAlignerAMotorHomeOffsetPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.RotaryAndAlignerBMotorHomeOffsetPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.SortingMotorHomeOffsetPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.TapeAndReelSealMotorTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.TapeAndReelSealMotorBlockUpPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.TapeAndReelSealMotorDownPosition);

            //                UpdateListTeachPoint(ref m_TeachPointSettings.TestSiteATouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.TestSiteBTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.TestSiteCTouchingPosition);
            //                UpdateListTeachPoint(ref m_TeachPointSettings.TestSiteDTouchingPosition);
            //#endif
            #endregion Turret Application

            #region Input XY Table
#if InputXYTable
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableXAxisCenterPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableXAxisLoadUnloadPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableYAxisCenterPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableYAxisLoadUnloadPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableThetaAxisCenterPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableThetaAxisLoadUnloadPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputVisionZAxisUpPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputVisionZAxisVisionFocusPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.Elevator4InchesTileFirstSlotLoadPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.TransferArmElevatorStandbyPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.TransferArm4InchesTileInsertTileStandbyPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.TransferArm4InchesTileInsertTilePosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.TransferArm4InchesTileLoadUnloadPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.UnitSupportDownPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.UnitSupportUpPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableXAxisGrayScalePosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableYAxisGrayScalePosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputVisionZAxisGrayScalePosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableXAxisDotGridsPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputTableYAxisDotGridsPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.InputVisionZAxisDotGridsPosition);
            ////UpdateListTeachPoint(ref m_TeachPointSettings.TransferArmFrameOrTileEnteringCassettePosition);

            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.Elevator6InchesWaferFrameFirstSlotLoadPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.TransferArm6InchesWaferFrameInsertFrameStandbyPosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.TransferArm6InchesWaferFrameInsertFramePosition);
            //UpdateListTeachPoint(ref m_ProductTeachPointSettings.TransferArm6InchesWaferFrameLoadUnloadPosition);

#endif
            #endregion Input XY Table
            return nError;
        }

        override public int AddAllTeachpoint()
        {
            int nError = 0;
            FieldInfo[] fields = typeof(ProductTeachPointSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo _fields in fields)
            {
                if (_fields.FieldType.Name == "MotorProfile")
                    m_listMotorProfile.Add((MotionLibrary.Motion.MotorProfile)_fields.GetValue(m_ProductTeachPointSettings));
            }
            #region Turret Application
            //#if TurretApplication
            //                AddListTeachPoint(m_TeachPointSettings.TurretHomeOffsetPosition);
            //                AddListTeachPoint(m_TeachPointSettings.InputPusherTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.MarkingVisionPusherTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.RotaryAndAlignerAPusherTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.RotaryAndAlignerBPusherTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.FiveSPusherTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.SortingPusherTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.TapeAndReelPusherTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.UnitSingulatorMotorRetractPosition);
            //                AddListTeachPoint(m_TeachPointSettings.UnitSingulatorMotorExtendPosition);

            //                AddListTeachPoint(m_TeachPointSettings.MarkingVisionMotorHomeOffsetPosition);
            //                AddListTeachPoint(m_TeachPointSettings.RotaryAndAlignerAMotorHomeOffsetPosition);
            //                AddListTeachPoint(m_TeachPointSettings.RotaryAndAlignerBMotorHomeOffsetPosition);
            //                AddListTeachPoint(m_TeachPointSettings.SortingMotorHomeOffsetPosition);
            //                AddListTeachPoint(m_TeachPointSettings.TapeAndReelSealMotorTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.TapeAndReelSealMotorBlockUpPosition);
            //                AddListTeachPoint(m_TeachPointSettings.TapeAndReelSealMotorDownPosition);

            //                AddListTeachPoint(m_TeachPointSettings.TestSiteATouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.TestSiteBTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.TestSiteCTouchingPosition);
            //                AddListTeachPoint(m_TeachPointSettings.TestSiteDTouchingPosition);
            //#endif
            #endregion Turret Application
            //            #region Input Table XY
            //#if InputXYTable
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableXAxisCenterPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableXAxisLoadUnloadPosition);

            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableYAxisCenterPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableYAxisLoadUnloadPosition);

            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableThetaAxisCenterPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableThetaAxisLoadUnloadPosition);

            //            AddListTeachPoint(m_ProductTeachPointSettings.InputVisionZAxisUpPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputVisionZAxisVisionFocusPosition);

            //            AddListTeachPoint(m_ProductTeachPointSettings.Elevator4InchesTileFirstSlotLoadPosition);

            //            AddListTeachPoint(m_ProductTeachPointSettings.TransferArmElevatorStandbyPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.TransferArm4InchesTileInsertTileStandbyPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.TransferArm4InchesTileInsertTilePosition);

            //            AddListTeachPoint(m_ProductTeachPointSettings.TransferArm4InchesTileLoadUnloadPosition);

            //            AddListTeachPoint(m_ProductTeachPointSettings.UnitSupportDownPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.UnitSupportUpPosition);

            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableXAxisGrayScalePosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableYAxisGrayScalePosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputVisionZAxisGrayScalePosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableXAxisDotGridsPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputTableYAxisDotGridsPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.InputVisionZAxisDotGridsPosition);
            //            //AddListTeachPoint(m_ProductTeachPointSettings.TransferArmFrameOrTileEnteringCassettePosition);

            //            AddListTeachPoint(m_ProductTeachPointSettings.Elevator6InchesWaferFrameFirstSlotLoadPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.TransferArm6InchesWaferFrameInsertFrameStandbyPosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.TransferArm6InchesWaferFrameInsertFramePosition);
            //            AddListTeachPoint(m_ProductTeachPointSettings.TransferArm6InchesWaferFrameLoadUnloadPosition);
            //#endif
            //            #endregion Input XY Table
            return nError;
        }

        override public int GetPreviousTeachPointSettings(out List<MotionLibrary.Motion.MotorProfile> listMotionProfile)
        {
            int nError = 0;
            listMotionProfile = new List<MotionLibrary.Motor.MotorProfile>();
            ProductTeachPointSettings m_PreviousTeachPointSettings = new ProductTeachPointSettings();
            m_PreviousTeachPointSettings = Tools.Deserialize<ProductTeachPointSettings>(m_strTeachPointPath + m_strFile);
            FieldInfo[] fields = typeof(ProductTeachPointSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo _fields in fields)
            {
                //if (_fields.Name != "ListTeachPoint")
                if (_fields.FieldType.Name == "MotorProfile")
                {
                    MotionLibrary.Motion.MotorProfile a = (MotionLibrary.Motion.MotorProfile)_fields.GetValue(m_PreviousTeachPointSettings);
                    listMotionProfile.Add((MotionLibrary.Motion.MotorProfile)_fields.GetValue(m_PreviousTeachPointSettings));
                }
            }
            return nError;
        }

        override public int LaunchSemiAutoTeach()
        {
            try
            {
                for (int i = 0; i < m_listMotorProfile.Count; i++)
                {
                    if (comboBoxModule.SelectedItem != null)
                    {
                        if (comboBoxModule.SelectedItem.ToString() == m_listMotorProfile[i].Module
                            && comboBoxAxis.SelectedItem.ToString() == m_listMotorProfile[i].Axis
                            && comboBoxTeachPointName.SelectedItem.ToString() == m_listMotorProfile[i].TeachPointName
                            //&& comboBoxProfileName.SelectedItem.ToString() == m_listMotorProfile[i].ProfileName
                            )
                        {
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_DONE", false);
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_SNAP_FAIL", false);
                            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_CENTER_ROTATION_TEACH_START", false);
                            m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_SEMI_TEACH", false);
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_DONE", false);
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_30_DEGREE_FAIL", false);
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_DONE", false);
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_ROTATE_NEGATIVE_30_DEGREE_FAIL", false);
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_MOVE_OFFSET_DONE", false);
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CENTER_ROTATION_TEACH_MOVE_OFFSET_FAIL", false);

                            if (productFormTeachPointGuide != null)
                            {
                                if (!productFormTeachPointGuide.IsDisposed)
                                {
                                    if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.WaitOne(0))
                                    {
                                        m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_End.Set();
                                    }
                                    else if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.WaitOne(0))
                                    {
                                        m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_End.Set();
                                    }
                                    productFormTeachPointGuide.Focus();
                                    goto EndOfShowTeachPointGuide;
                                }
                            }
                            productFormTeachPointGuide = new ProductFormTeachPointGuide();
                            productFormTeachPointGuide.productRTSSProcess = m_ProductRTSSProcess;
                            productFormTeachPointGuide.productProcessEvent = m_ProductProcessEvent;
                            productFormTeachPointGuide.Show();
                        EndOfShowTeachPointGuide:;
                            //productFormTeachPointGuide.TopMost = true;
                            m_ProductRTSSProcess.SetGeneralInt("nTeachPointAxis", m_listMotorProfile[i].AxisNo);
                            //m_ProductRTSSProcess.SetGeneralInt("nTeachPointIndexNo", m_listMotorProfile[i].Index);
                            //SetShareMemoryGeneralInt("nMotorMovePosition", (int)numericUpDownTeachPointPosition.Value);
                            //SetShareMemoryEvent("GTCH_RTCH_MOTOR_MV_ABS", true);
                            //updateRichTextBoxMessage(m_listMotorProfile[i].Module + " " + m_listMotorProfile[i].Axis + " Axis moving to teach position " + m_listMotorProfile[i].TeachPointName + Environment.NewLine);
                            //SetShareMemoryEvent("GPCS_RSEQ_ABORT", true);
                            if (comboBoxModule.SelectedItem.ToString().Contains("Input"))
                            {
                                m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.Set();
                                m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Initialize.Set();
                                m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.Reset();
                                m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Initialize.Reset();
                                productFormTeachPointGuide.groupBoxAdditional.Visible = false;
                            }
                            else if (comboBoxModule.SelectedItem.ToString().Contains("Aligner"))
                            {
                                m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Choose.Reset();
                                m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Initialize.Reset();
                                m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Choose.Set();
                                m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Initialize.Set();
                                productFormTeachPointGuide.groupBoxAdditional.Visible = true;

                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
            return m_nErrorCode;
        }

        private void InitializeComponent()
        {
            this.panel1.SuspendLayout();
            this.groupBoxMotorStatus.SuspendLayout();
            this.groupBoxControlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProfileSpeed)).BeginInit();
            this.groupBoxProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProfileDeceleration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProfileAcceleration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTeachPointPosition)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMotorAlarm
            // 
            this.labelMotorAlarm.Location = new System.Drawing.Point(467, 29);
            // 
            // ProductFormTeachPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(1904, 750);
            this.Name = "ProductFormTeachPoint";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBoxMotorStatus.ResumeLayout(false);
            this.groupBoxMotorStatus.PerformLayout();
            this.groupBoxControlButtons.ResumeLayout(false);
            this.groupBoxControlButtons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProfileSpeed)).EndInit();
            this.groupBoxProfile.ResumeLayout(false);
            this.groupBoxProfile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProfileDeceleration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProfileAcceleration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTeachPointPosition)).EndInit();
            this.ResumeLayout(false);

        }

        public virtual int InitializeGUI()
        {
            labelMotorAlarm.Location = new System.Drawing.Point(467, 29);
            groupBoxMotorStatus.Controls.Add(labelTitleTHKPressureValue);
            groupBoxMotorStatus.Controls.Add(labelTitleTHKForceValue);
            groupBoxMotorStatus.Controls.Add(labelTitleTHKFlowRate);
            groupBoxMotorStatus.Controls.Add(labelTHKPressureValue);
            groupBoxMotorStatus.Controls.Add(labelTHKForceValue);
            groupBoxMotorStatus.Controls.Add(labelTHKFlowRate);

            labelTitleTHKPressureValue.AutoSize = true;
            labelTitleTHKPressureValue.Location = new System.Drawing.Point(630, 29);
            labelTitleTHKPressureValue.Name = "labelTitleTHKPressureValue";
            labelTitleTHKPressureValue.Size = new System.Drawing.Size(157, 24);
            //labelTitleTHKPressureValue.TabIndex = 10;

            labelTitleTHKForceValue.AutoSize = true;
            labelTitleTHKForceValue.Location = new System.Drawing.Point(630, 55);
            labelTitleTHKForceValue.Name = "labelTitleTHKForceValue";
            labelTitleTHKForceValue.Size = new System.Drawing.Size(157, 24);
            //labelTitleTHKForceValue.TabIndex = 10;

            labelTitleTHKFlowRate.AutoSize = true;
            labelTitleTHKFlowRate.Location = new System.Drawing.Point(630, 83);
            labelTitleTHKFlowRate.Name = "labelTitleTHKFlowRate";
            labelTitleTHKFlowRate.Size = new System.Drawing.Size(157, 24);
            //labelTitleTHKFlowRate.TabIndex = 10;

            labelTHKPressureValue.AutoSize = true;
            labelTHKPressureValue.Location = new System.Drawing.Point(810, 29);
            labelTHKPressureValue.Name = "labelTHKPressureValue";
            labelTHKPressureValue.Size = new System.Drawing.Size(157, 24);
            //labelTHKPressureValue.TabIndex = 10;

            labelTHKForceValue.AutoSize = true;
            labelTHKForceValue.Location = new System.Drawing.Point(810, 55);
            labelTHKForceValue.Name = "labelTHKForceValue";
            labelTHKForceValue.Size = new System.Drawing.Size(157, 24);
            //labelTHKForceValue.TabIndex = 10;

            labelTHKFlowRate.AutoSize = true;
            labelTHKFlowRate.Location = new System.Drawing.Point(810, 83);
            labelTHKFlowRate.Name = "labelTHKFlowRate";
            labelTHKFlowRate.Size = new System.Drawing.Size(157, 24);
            //labelTHKFlowRate.TabIndex = 10;

            labelTitleTHKPressureValue.Text = "Pressure (kPA) : ";
            labelTitleTHKForceValue.Text = "Force (N) : ";
            labelTitleTHKFlowRate.Text = "FlowRate (L/min) : ";
            return 0;
        }

        public override void UpdateGUIInBackgroundworker()
        {
            try
            {
                base.UpdateGUIInBackgroundworker();
                if (comboBoxModule.SelectedItem != null && comboBoxAxis.SelectedItem != null)
                {
                    if ((comboBoxModule.SelectedItem.ToString() == "Pick And Place 1")
                        && (comboBoxAxis.SelectedItem.ToString() == "Z Axis" || comboBoxAxis.SelectedItem.ToString() == "Theta Axis")
                        )
                    {
                        //buttonStopMovement.Visible = false;
                        labelTitleTHKPressureValue.Visible = true;
                        labelTitleTHKForceValue.Visible = true;
                        labelTitleTHKFlowRate.Visible = true;
                        labelTHKPressureValue.Visible = true;
                        labelTHKForceValue.Visible = true;
                        labelTHKFlowRate.Visible = true;
                        if (labelTHKPressureValue.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentPressureValue")).ToString())
                        {
                            labelTHKPressureValue.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentPressureValue")).ToString();
                        }

                        if (labelTHKForceValue.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentForceValue")).ToString())
                        {
                            labelTHKForceValue.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentForceValue")).ToString();
                        }
                        if (labelTHKFlowRate.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentFlowRate")).ToString())
                        {
                            labelTHKFlowRate.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK1CurrentFlowRate")).ToString();
                        }
                    }
                    else if ((comboBoxModule.SelectedItem.ToString() == "Pick And Place 2")
                        && (comboBoxAxis.SelectedItem.ToString() == "Z Axis" || comboBoxAxis.SelectedItem.ToString() == "Theta Axis")
                        )
                    {
                        //buttonStopMovement.Visible = false;
                        labelTitleTHKPressureValue.Visible = true;
                        labelTitleTHKForceValue.Visible = true;
                        labelTitleTHKFlowRate.Visible = true;
                        labelTHKPressureValue.Visible = true;
                        labelTHKForceValue.Visible = true;
                        labelTHKFlowRate.Visible = true;
                        if (labelTHKPressureValue.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentPressureValue")).ToString())
                        {
                            labelTHKPressureValue.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentPressureValue")).ToString();
                        }
                        if (labelTHKForceValue.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentForceValue")).ToString())
                        {
                            labelTHKForceValue.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentForceValue")).ToString();
                        }
                        if (labelTHKFlowRate.Text != ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentFlowRate")).ToString())
                        {
                            labelTHKFlowRate.Text = ((double)m_ProductRTSSProcess.GetProductionDouble("THK2CurrentFlowRate")).ToString();
                        }
                    }
                    else
                    {
                        labelTitleTHKPressureValue.Visible = false;
                        labelTitleTHKForceValue.Visible = false;
                        labelTitleTHKFlowRate.Visible = false;
                        labelTHKPressureValue.Visible = false;
                        labelTHKForceValue.Visible = false;
                        labelTHKFlowRate.Visible = false;
                    }
                    if (comboBoxAxis.SelectedItem.ToString() == "Theta Axis")
                    {
                        labelUnitTeachPoint.Text = "mDegree";
                        labelUnit.Text = "mDegree";
                    }
                    else
                    {
                        labelUnitTeachPoint.Text = "um";
                        labelUnit.Text = "um";

                    }
                }
            }

            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                //return false;
            }
        }

        override public int OnUpdateNumericUpDownTeachPointPosition(List<Motion.MotorProfile> listMotorProfile, Motor.MotorProfile motorProfile)
        {
            int nError = 0;
            try
            {
                bool foundPosition = false;
                if (motorProfile.Module == "Module" && motorProfile.Axis == "Axis" && motorProfile.TeachPointName == "TeachPointName")
                {
                    foreach (Motion.MotorProfile _motorProfile in m_listMotorProfile)
                    {
                        if (motorProfile.Module == "TargetModule"
                            && motorProfile.Axis == "TargetAxis"
                            && motorProfile.TeachPointName == "TargetTeachPointName")
                        {
                            numericUpDownTeachPointPosition.Value = motorProfile.TeachPoint + 200;
                        }
                    }
                }
                else
                    base.OnUpdateNumericUpDownTeachPointPosition(listMotorProfile, motorProfile);
            }
            catch (Exception ex)
            {
                nError = -1;
                //MessageBox.Show(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return nError;
        }
    }
}
