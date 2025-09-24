using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using Common;

namespace Product
{
    public partial class ProductFormTeachMap : Form
    {
        public enum MotorAxis
        {
            PickAndPlace1XAxis = 107,
            PickAndPlace1YAxis = 108,
            PickAndPlace1ThetaAxis = 110,
            PickAndPlace1ZAxis = 109,

            PickAndPlace2XAxis = 111,
            PickAndPlace2YAxis = 112,
            PickAndPlace2ThetaAxis = 114,
            PickAndPlace2ZAxis = 113,

            InputVisionZAxis = 115,
            S2VisionZAxis = 116,
            S1VisionZAxis = 117,
            //SidewallLeftVisionZAxis = 117,
            //SidewallRightVisionZAxis = 118,
            //SidewallFrontVisionZAxis = 119,
            //SidewallRearVisionZAxis = 120,

            S3VisionZAxis = 121,
        }
        public enum VisionModule
        {
	        Unknown = 0,
            InputVision = 1,
            S2Vision = 2,
            SetupVision = 3,
            S1Vision = 4,
            S3Vision = 9,
            OutputVision = 10,
        };
        public string m_strmode = "Map Teaching Form";
        BackgroundWorker m_bgwUserInterface;
   
        ProductInputOutputFileFormat m_readInfo = new ProductInputOutputFileFormat();
        BinInfo arrayUnitInfo;
        BinInfo m_mapData;

        //UnitInfo[] arrayUnitInfo;
        //Start Point Selection
        int StartPointSelect;
        //BluUnitInfo[] arrayBluUnitInfo;

        public int m_nErrorCode = 0;
        public string m_strErrorMsg = "";
        public int m_nPosition = 0;

        bool bVisionZAxisForward = false;
        bool bVisionZAxisReverse = false;
        bool bPickAndPlaceZAxisForward = false;
        bool bPickAndPlaceZAxisReverse = false;
        bool bPickAndPlaceYForward = false;
        bool bPickAndPlaceXReserve = false;
        bool bPickAndPlaceXForward = false;
        bool bPickAndPlaceYReverse = false;        
        bool bPickAndPlaceThetaForward = false;
        bool bPickAndPlaceThetaReverse = false;
        
        int bUserSaveRecipeAndContinue = 0;
        bool m_bExitWindowForm = false;
        static ManualResetEvent eTeachPosition = new ManualResetEvent(false);
        string m_strMessageFromSequence = "";
        private StringBuilder m_strBMessageFromSequence = new StringBuilder(512);

        public ProductRecipeVisionSettings m_ProductRecipeVisionSetting = new ProductRecipeVisionSettings();
        public ProductRecipeInputSettings m_ProductRecipeInputSetting = new ProductRecipeInputSettings();
        HiPerfTimer m_HPTMouseDown = new HiPerfTimer();

        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductProcessEvent m_ProductProcessEvent;// = new ProductProcessEvent();

        private ProductStateControl m_ProductStateControl;// = new ProductStateControl();
        private ProductRTSSProcess m_ProductRTSSProcess;// = new ProductRTSSProcess();
        
        PositionSet m_nInitialPositionSet = new PositionSet();

        public ProductShareVariables productShareVariables
        {
            set { m_ProductShareVariables = value; }
        }

        public ProductProcessEvent productProcessEvent
        {
            set { m_ProductProcessEvent = value; }
        }

        public ProductStateControl productStateControl
        {
            set
            {
                m_ProductStateControl = value;
            }
        }

        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
            }
        }

        public ProductRecipeVisionSettings productRecipeVisionSettings
        {
            set
            {
                m_ProductRecipeVisionSetting = value;
            }
        }

        public ProductRecipeInputSettings productRecipeInputSettings
        {
            set
            {
                m_ProductRecipeInputSetting = value;
            }
        }

        public bool ExitForm
        {
            set
            {
                m_bExitWindowForm = value;
            }
        }

        #region Form Teach Map

        public ProductFormTeachMap()
        {
            InitializeComponent();
            //Initialize();
            
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Close at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (m_bgwUserInterface.WorkerSupportsCancellation == true)
                {
                    m_bgwUserInterface.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        #region BackgroundWorker User Interface

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
                    Thread.Sleep(100);
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
                if (m_ProductShareVariables.bFormMainInterfaceButtonExit == false)
                {
                    if (bVisionZAxisForward == true || bVisionZAxisReverse == true)
                    {
                        if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                            MoveRelative(MotorAxis.InputVisionZAxis, m_nPosition);
                        else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                            MoveRelative(MotorAxis.S2VisionZAxis, m_nPosition);
                        else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                            MoveRelative(MotorAxis.S3VisionZAxis, m_nPosition);
                        else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision
                            || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision
                            || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                        {
                            if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                                MoveRelative(MotorAxis.PickAndPlace1ZAxis, m_nPosition);
                            else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                                MoveRelative(MotorAxis.PickAndPlace2ZAxis, m_nPosition);
                        }
                    }
                    if (bPickAndPlaceZAxisForward == true || bPickAndPlaceZAxisReverse == true)
                    {
                        if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                            MoveRelative(MotorAxis.PickAndPlace1ZAxis, m_nPosition);
                        else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                            MoveRelative(MotorAxis.PickAndPlace2ZAxis, m_nPosition);
                    }
                    if (bPickAndPlaceXForward == true || bPickAndPlaceXReserve == true)
                    {
                        if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                            MoveRelative(MotorAxis.PickAndPlace1XAxis, m_nPosition);
                        else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                            MoveRelative(MotorAxis.PickAndPlace2XAxis, m_nPosition);
                    }
                    if (bPickAndPlaceYForward == true || bPickAndPlaceYReverse == true)
                    {
                        if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                            MoveRelative(MotorAxis.PickAndPlace1YAxis, m_nPosition);
                        else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                            MoveRelative(MotorAxis.PickAndPlace2YAxis, m_nPosition);
                    }
                    if (bPickAndPlaceThetaForward == true || bPickAndPlaceThetaReverse == true)
                    {
                        if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                            MoveRelative(MotorAxis.PickAndPlace1ThetaAxis, m_nPosition);
                        else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                            MoveRelative(MotorAxis.PickAndPlace2ThetaAxis, m_nPosition);
                    }

                    if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                    {
                        if (labelXAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1XAxisEncoderPosition").ToString())
                            labelXAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1XAxisEncoderPosition").ToString();
                        if (labelYAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1YAxisEncoderPosition").ToString())
                            labelYAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1YAxisEncoderPosition").ToString();
                        if (labelThetaAbsolute2_mD.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ThetaAxisEncoderPosition").ToString())
                            labelThetaAbsolute2_mD.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ThetaAxisEncoderPosition").ToString();

                        if (labelPZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition").ToString())
                            labelPZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition").ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                    {
                        if (labelXAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace2XAxisEncoderPosition").ToString())
                            labelXAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2XAxisEncoderPosition").ToString();
                        if (labelYAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace2YAxisEncoderPosition").ToString())
                            labelYAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2YAxisEncoderPosition").ToString();
                        if (labelThetaAbsolute2_mD.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ThetaAxisEncoderPosition").ToString())
                            labelThetaAbsolute2_mD.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ThetaAxisEncoderPosition").ToString();

                        if (labelPZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition").ToString())
                            labelPZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition").ToString();
                    }

                    if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                    {
                        if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition").ToString())
                            labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition").ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                    {
                        if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("S2VisionZAxisEncoderPosition").ToString())
                            labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("S2VisionZAxisEncoderPosition").ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                    {
                        if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("S1VisionZAxisEncoderPosition").ToString())
                            labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("S1VisionZAxisEncoderPosition").ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                    {
                        if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("S3VisionZAxisEncoderPosition").ToString())
                            labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("S3VisionZAxisEncoderPosition").ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision
                        || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                    {
                        if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                        {
                            if (labelPZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition").ToString())
                                labelPZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition").ToString();
                        }
                        else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                        {
                            if (labelPZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition").ToString())
                                labelPZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition").ToString();
                        }
                    }

                    if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                    {
                        if (labelXReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1XAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceXPosition).ToString())
                            labelXReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1XAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceXPosition).ToString();
                        if (labelYReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1YAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceYPosition).ToString())
                            labelYReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1YAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceYPosition).ToString();
                        if (labelThetaReference2_mD.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ThetaAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceThetaPosition).ToString())
                            labelThetaReference2_mD.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ThetaAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceThetaPosition).ToString();                     
                        if (labelPZReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceZPosition).ToString())
                            labelPZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceZPosition).ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                    {
                        if (labelXReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2XAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceXPosition).ToString())
                            labelXReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2XAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceXPosition).ToString();
                        if (labelYReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2YAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceYPosition).ToString())
                            labelYReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2YAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceYPosition).ToString();
                        if (labelThetaReference2_mD.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ThetaAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceThetaPosition).ToString())
                            labelThetaReference2_mD.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ThetaAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceThetaPosition).ToString();
                        if (labelPZReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceZPosition).ToString())
                            labelPZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition") - m_nInitialPositionSet.PickAndPlaceZPosition).ToString();
                    }

                    if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                    {
                        if (labelVZReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString())
                            labelVZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                    {
                        if (labelVZReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("S2VisionZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString())
                            labelVZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("S2VisionZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                    {
                        if (labelVZReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("S1VisionZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString())
                            labelVZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("S1VisionZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                    {
                        if (labelVZReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("S3VisionZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString())
                            labelVZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("S3VisionZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision
                        || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                    {
                        if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                        {
                            if (labelVZReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString())
                                labelVZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString();
                        }
                        else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                        {
                            if (labelVZReference2_um.Text != (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString())
                                labelVZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition") - m_nInitialPositionSet.VisionFocusPosition).ToString();
                        }
                    }

                    if(m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_VERIFY_POS_DONE") == true)
                    {
                        comboBoxTeachItem.Enabled = true;
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VERIFY_POS_DONE", false);
                        updateRichTextBoxMessageInBackgroundworker(string.Format("Move to position done"));
                    }
                    //if ((m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_VERIFY_FIDUCIAL_DONE") == true) && (m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_VERIFY_FIDUCIAL_START") == false)// && bTeachPosition == true)
                    //    && m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_TEACH_MAP_POS"))
                    //{
                    //    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VERIFY_FIDUCIAL_DONE", false);
                    //    int nPositionX_um, nPositionY_um, nPositionTheta_mDegree, nPositionVisionZ_um, nPositionUnitSupportZ_um, nPositionVisionOffsetX_um, nPositionVisionOffsetY_um, nPositionVisionOffsetTheta_mDegree;

                    //    labelXAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("InputVisionXAxisEncoderPosition").ToString();
                    //    labelYAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("InputTableYAxisEncoderPosition").ToString();
                    //    labelThetaAbsolute2_mD.Text = m_ProductRTSSProcess.GetProductionLong("InputTableThetaAxisEncoderPosition").ToString();
                    //    //labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition").ToString();
                    //    labelPZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PepperPotZAxisEncoderPosition").ToString();

                    //    labelXReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("InputVisionXAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableXAxisCenterPosition")).ToString();
                    //    labelYReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("InputTableYAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableYAxisCenterPosition")).ToString();
                    //    labelThetaReference2_mD.Text = (m_ProductRTSSProcess.GetProductionLong("InputTableThetaAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableThetaAxisCenterPosition")
                    //        - m_ProductRTSSProcess.GetSettingLong("InspectionOrientation_mDegree")).ToString();
                    //    //labelVZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputVisionZAxisVisionFocusPosition")).ToString();
                    //    //labelUZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("UnitSupportZAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("UnitSupportUpPosition")).ToString();
                    //    labelVisionOffsetX.Text = m_ProductRTSSProcess.GetProductionLong("nVisionXOffset").ToString();
                    //    labelVisionOffsetY.Text = m_ProductRTSSProcess.GetProductionLong("nVisionYOffset").ToString();
                    //    labelVisionOffsetTheta.Text = m_ProductRTSSProcess.GetProductionLong("nVisionThetaOffset").ToString();

                    //    if (Int32.TryParse(labelXReference2_um.Text, out nPositionX_um)
                    //        && Int32.TryParse(labelYReference2_um.Text, out nPositionY_um)
                    //        && Int32.TryParse(labelThetaReference2_mD.Text, out nPositionTheta_mDegree)
                    //        && Int32.TryParse(labelVZReference2_um.Text, out nPositionVisionZ_um)
                    //        && Int32.TryParse(labelPZReference2_um.Text, out nPositionUnitSupportZ_um)
                    //        && Int32.TryParse(labelVisionOffsetX.Text, out nPositionVisionOffsetX_um)
                    //        && Int32.TryParse(labelVisionOffsetY.Text, out nPositionVisionOffsetY_um)
                    //        && Int32.TryParse(labelVisionOffsetTheta.Text, out nPositionVisionOffsetTheta_mDegree)
                    //        )
                    //    {
                    //        //m_ProductRecipeInputSetting.InputTableZAxisUpOffset_um = nPositionVisionZ_um;
                    //        //m_ProductRecipeInputSetting.PepperPotOffset_um = nPositionUnitSupportZ_um;
                    //        //m_ProductRecipeVisionSetting.FiducialData[comboBoxTeachItem.SelectedIndex].PositionX_um = nPositionX_um;
                    //        //m_ProductRecipeVisionSetting.FiducialData[comboBoxTeachItem.SelectedIndex].PositionY_um = nPositionY_um;
                    //        //m_ProductRecipeVisionSetting.FiducialData[comboBoxTeachItem.SelectedIndex].PositionTheta_mDegree = nPositionTheta_mDegree;
                    //        //m_ProductRecipeVisionSetting.FiducialData[comboBoxTeachItem.SelectedIndex].VisionOffsetX_um = nPositionVisionOffsetX_um;
                    //        //m_ProductRecipeVisionSetting.FiducialData[comboBoxTeachItem.SelectedIndex].VisionOffsetY_um = nPositionVisionOffsetY_um;
                    //        //m_ProductRecipeVisionSetting.FiducialData[comboBoxTeachItem.SelectedIndex].VisionOffsetTheta_mDegree = nPositionVisionOffsetTheta_mDegree;

                    //        labelCenterX.Text = nPositionX_um.ToString();
                    //        labelCenterY.Text = nPositionY_um.ToString();
                    //        labelCenterTheta.Text = nPositionTheta_mDegree.ToString();
                    //        labelAbsoluteX.Text = (nPositionX_um + m_ProductRTSSProcess.GetTeachPointLong("InputTableXAxisCenterPosition")).ToString();
                    //        labelAbsoluteY.Text = (nPositionY_um + m_ProductRTSSProcess.GetTeachPointLong("InputTableYAxisCenterPosition")).ToString();
                    //        labelAbsoluteTheta.Text = (nPositionTheta_mDegree + m_ProductRTSSProcess.GetTeachPointLong("InputTableThetaAxisCenterPosition") + m_ProductRTSSProcess.GetSettingLong("InspectionOrientation_mDegree")).ToString();
                    //        labelVisionOffsetX.Text = nPositionVisionOffsetX_um.ToString();
                    //        labelVisionOffsetY.Text = nPositionVisionOffsetY_um.ToString();
                    //        labelVisionOffsetTheta.Text = nPositionVisionOffsetTheta_mDegree.ToString();

                    //        updateRichTextBoxMessageInBackgroundworker(string.Format("Receive from vision X offset = {0} um,Y offset = {1} um", m_ProductRTSSProcess.GetProductionLong("nVisionXOffset"), m_ProductRTSSProcess.GetProductionLong("nVisionYOffset")));

                    //        updateRichTextBoxMessageInBackgroundworker(string.Format("Teach Fiducial {0} done", m_ProductShareVariables.TeachItem));
                    //    }
                    //    comboBoxTeachType.Enabled = true;
                    //    comboBoxTeachItem.Enabled = true;
                    //    Application.DoEvents();
                    //    Thread.Sleep(100);
                    //}
                    //else if ((m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_VERIFY_FIDUCIAL_DONE") == true) && (m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_VERIFY_FIDUCIAL_START") == false)// && bTeachPosition == false)
                    //    && (m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_TEACH_MAP_POS") == false))
                    //{
                    //    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VERIFY_FIDUCIAL_DONE", false);
                    //    int nPositionX_um, nPositionY_um, nPositionTheta_mDegree, nPositionVisionZ_um, nPositionUnitSupportZ_um, nPositionVisionOffsetX_um, nPositionVisionOffsetY_um, nPositionVisionOffsetTheta_mDegree;
                    //    if (Int32.TryParse(labelVisionOffsetX.Text, out nPositionVisionOffsetX_um)
                    //        && Int32.TryParse(labelVisionOffsetY.Text, out nPositionVisionOffsetY_um)
                    //        )
                    //    {
                    //        updateRichTextBoxMessageInBackgroundworker(string.Format("Receive from vision X offset = {0} um,Y offset = {1} um", m_ProductRTSSProcess.GetProductionLong("nVisionXOffset"), m_ProductRTSSProcess.GetProductionLong("nVisionYOffset")));

                    //        updateRichTextBoxMessageInBackgroundworker(string.Format("Taught vision X offset = {0} um,Y offset = {1} um", nPositionVisionOffsetX_um, nPositionVisionOffsetY_um));
                    //        //if (m_ProductRTSSProcess.GetProductionLong("nVisionXOffset") - nPositionVisionOffsetX_um <= m_ProductRecipeVisionSetting.MaximumAllowableXYOffsetBetweenFiducial_um
                    //        //    && m_ProductRTSSProcess.GetProductionLong("nVisionYOffset") - nPositionVisionOffsetY_um <= m_ProductRecipeVisionSetting.MaximumAllowableXYOffsetBetweenFiducial_um)
                    //        //{
                    //        //    updateRichTextBoxMessageInBackgroundworker(string.Format("Verify Fiducial {0} pass within tolerance {1}", m_ProductShareVariables.TeachItem, m_ProductRecipeVisionSetting.MaximumAllowableXYOffsetBetweenFiducial_um));
                    //        //}
                    //        //else
                    //        //{
                    //        //    updateRichTextBoxMessageInBackgroundworker(string.Format("Verify Fiducial {0} fail, out of tolerance {1}", m_ProductShareVariables.TeachItem, m_ProductRecipeVisionSetting.MaximumAllowableXYOffsetBetweenFiducial_um));
                    //        //}
                    //    }
                    //    comboBoxTeachType.Enabled = true;
                    //    comboBoxTeachItem.Enabled = true;

                    //    Application.DoEvents();
                    //    Thread.Sleep(100);
                    //}

                    //if ((m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_VERIFY_FIRST_POS_DONE")  == true) && (m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_VERIFY_FIRST_POS_START") == false)// && bTeachPosition == true)
                    //    //&& (eTeachPosition.WaitOne(0) == true))
                    //    && m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_TEACH_MAP_POS"))
                    //{
                    //    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VERIFY_FIRST_POS_DONE", false);
                    //    int nPositionX_um, nPositionY_um, nPositionTheta_mDegree, nPositionVisionZ_um, nPositionUnitSupportZ_um;

                    //    labelXAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("InputVisionXAxisEncoderPosition").ToString();
                    //    labelYAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("InputTableYAxisEncoderPosition").ToString();
                    //    labelThetaAbsolute2_mD.Text = m_ProductRTSSProcess.GetProductionLong("InputTableThetaAxisEncoderPosition").ToString();
                    //    //labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition").ToString();
                    //    labelPZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PepperPotZAxisEncoderPosition").ToString();

                    //    labelXReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("InputVisionXAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableXAxisCenterPosition")).ToString();
                    //    labelYReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("InputTableYAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableYAxisCenterPosition")).ToString();
                    //    labelThetaReference2_mD.Text = (m_ProductRTSSProcess.GetProductionLong("InputTableThetaAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableThetaAxisCenterPosition")
                    //        - m_ProductRTSSProcess.GetSettingLong("InspectionOrientation_mDegree")).ToString();
                    //    //labelVZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputVisionZAxisVisionFocusPosition")).ToString();
                    //    labelPZReference2_um.Text = (m_ProductRTSSProcess.GetProductionLong("PepperPotZAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("PepperPotTouchingPosition")).ToString();

                    //    if (Int32.TryParse(labelXReference2_um.Text, out nPositionX_um)
                    //    && Int32.TryParse(labelYReference2_um.Text, out nPositionY_um)
                    //    && Int32.TryParse(labelThetaReference2_mD.Text, out nPositionTheta_mDegree)
                    //    && Int32.TryParse(labelVZReference2_um.Text, out nPositionVisionZ_um)
                    //    && Int32.TryParse(labelPZReference2_um.Text, out nPositionUnitSupportZ_um)
                    //    )
                    //    {
                    //        //m_ProductRecipeInputSetting.InputTableZAxisUpOffset_um = nPositionVisionZ_um;
                    //        //m_ProductRecipeInputSetting.PepperPotOffset_um = nPositionUnitSupportZ_um;

                    //        //m_ProductRecipeVisionSetting.FirstUnitXPosition_um = nPositionX_um;
                    //        //m_ProductRecipeVisionSetting.FirstUnitYPosition_um = nPositionY_um;
                    //        //m_ProductRecipeVisionSetting.FirstUnitThetaPosition_mDegree = nPositionTheta_mDegree;

                    //        //m_ProductRecipeVisionSetting.FirstUnitXVisionOffset_um = m_ProductRTSSProcess.GetProductionLong("nVisionXOffset");
                    //        //m_ProductRecipeVisionSetting.FirstUnitYVisionOffset_um = m_ProductRTSSProcess.GetProductionLong("nVisionYOffset");
                    //        //m_ProductRecipeVisionSetting.FirstUnitThetaVisionOffset_mDegree = m_ProductRTSSProcess.GetProductionLong("nVisionThetaOffset");

                    //        labelCenterX.Text = nPositionX_um.ToString();
                    //        labelCenterY.Text = nPositionY_um.ToString();
                    //        labelCenterTheta.Text = nPositionTheta_mDegree.ToString();
                    //        labelAbsoluteX.Text = (nPositionX_um + m_ProductRTSSProcess.GetTeachPointLong("InputTableXAxisCenterPosition")).ToString();
                    //        labelAbsoluteY.Text = (nPositionY_um + m_ProductRTSSProcess.GetTeachPointLong("InputTableYAxisCenterPosition")).ToString();
                    //        labelAbsoluteTheta.Text = (nPositionTheta_mDegree + m_ProductRTSSProcess.GetTeachPointLong("InputTableThetaAxisCenterPosition") + m_ProductRTSSProcess.GetSettingLong("InspectionOrientation_mDegree")).ToString();

                    //        labelVisionOffsetX.Text = m_ProductRTSSProcess.GetProductionLong("nVisionXOffset").ToString();
                    //        labelVisionOffsetY.Text = m_ProductRTSSProcess.GetProductionLong("nVisionYOffset").ToString();
                    //        labelVisionOffsetTheta.Text = m_ProductRTSSProcess.GetProductionLong("nVisionThetaOffset").ToString();

                    //        updateRichTextBoxMessageInBackgroundworker(string.Format("Receive from vision X offset = {0} um,Y offset = {1} um, Theta offset = {2} mDegree", m_ProductRTSSProcess.GetProductionLong("nVisionXOffset"), m_ProductRTSSProcess.GetProductionLong("nVisionYOffset"), m_ProductRTSSProcess.GetProductionLong("nVisionThetaOffset")));

                    //        updateRichTextBoxMessageInBackgroundworker(string.Format("Teach First position done"));
                    //    }
                    //    comboBoxTeachType.Enabled = true;
                    //    comboBoxTeachItem.Enabled = true;
                    //    Application.DoEvents();
                    //    Thread.Sleep(100);
                    //}
                    //else if ((m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_VERIFY_FIRST_POS_DONE") == true) && (m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_VERIFY_FIRST_POS_START") == false)// && bTeachPosition == false)
                    //    && (m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_TEACH_MAP_POS") == false))
                    //{
                    //    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VERIFY_FIRST_POS_DONE", false);
                    //    updateRichTextBoxMessageInBackgroundworker(string.Format("Receive from vision X offset = {0} um,Y offset = {1} um, Theta offset = {2} mDegree", m_ProductRTSSProcess.GetProductionLong("nVisionXOffset"), m_ProductRTSSProcess.GetProductionLong("nVisionYOffset"), m_ProductRTSSProcess.GetProductionLong("nVisionThetaOffset")));
                    //    updateRichTextBoxMessageInBackgroundworker(string.Format("Move offset to Theta = {0} mDegree ", m_ProductRTSSProcess.GetProductionLong("nVisionThetaOffset") * -1));

                    //    //updateRichTextBoxMessageInBackgroundworker(string.Format("Taught vision X offset = {0} um,Y offset = {1} um", m_ProductRecipeVisionSetting.FirstUnitXVisionOffset_um, m_ProductRecipeVisionSetting.FirstUnitYVisionOffset_um));

                    //    //if (m_ProductRTSSProcess.GetProductionLong("nVisionXOffset") - m_ProductRecipeVisionSetting.FirstUnitXVisionOffset_um <= m_ProductRecipeVisionSetting.MaximumAllowableXYOffsetBetweenFiducial_um
                    //    //    && m_ProductRTSSProcess.GetProductionLong("nVisionYOffset") - m_ProductRecipeVisionSetting.FirstUnitYVisionOffset_um <= m_ProductRecipeVisionSetting.MaximumAllowableXYOffsetBetweenFiducial_um)
                    //    //{
                    //    //    updateRichTextBoxMessageInBackgroundworker(string.Format("Verify First Position {0} pass within tolerance {1}", m_ProductShareVariables.TeachItem, m_ProductRecipeVisionSetting.MaximumAllowableXYOffsetBetweenFiducial_um));
                    //    //}
                    //    //else
                    //    //{
                    //    //    updateRichTextBoxMessageInBackgroundworker(string.Format("Verify First Position {0} fail, out of tolerance {1}", m_ProductShareVariables.TeachItem, m_ProductRecipeVisionSetting.MaximumAllowableXYOffsetBetweenFiducial_um));
                    //    //}
                    //    comboBoxTeachType.Enabled = true;
                    //    comboBoxTeachItem.Enabled = true;
                    //    Application.DoEvents();
                    //    Thread.Sleep(100);
                    //}

                    //if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_ALLOCATE_UNIT_DONE") == true && m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_ALLOCATE_UNIT_START") == false)
                    //{
                    //    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_ALLOCATE_UNIT_DONE", false);
                    //    updateRichTextBoxMessageInBackgroundworker(string.Format("Receive from vision X offset = {0} um,Y offset = {1} um, Theta offset = {2} mDegree", m_ProductRTSSProcess.GetProductionLong("nVisionXOffset"), m_ProductRTSSProcess.GetProductionLong("nVisionYOffset"), m_ProductRTSSProcess.GetProductionLong("nVisionThetaOffset")));
                    //    updateRichTextBoxMessageInBackgroundworker(string.Format("Allocate Unit Done."));
                    //    Application.DoEvents();
                    //    Thread.Sleep(100);
                    //}

                    if (m_ProductRTSSProcess.GetGeneralInt("updateMessage") == 1)
                    {
                        m_ProductRTSSProcess.SetGeneralInt("updateMessage", 0);
                        m_ProductRTSSProcess.GetGeneralString("MessageStatus", m_strBMessageFromSequence, m_strBMessageFromSequence.Capacity);
                        m_strMessageFromSequence = Tools.GetDecodedStringFromNativeChar(Convert.ToString(m_strBMessageFromSequence));
                        updateRichTextBoxMessageInBackgroundworker(m_strMessageFromSequence);
                    }

                    if(m_bExitWindowForm == true)
                    {
                        if (m_bgwUserInterface.WorkerSupportsCancellation == true)
                        {
                            m_bgwUserInterface.CancelAsync();
                        }
                    }
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
                if ((e.Cancelled == true))
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

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void rdbTopLeft_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdbTopRight_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void rdbBottomLeft_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void rdbBottomRight_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void richTextBoxMessage_TextChanged(object sender, EventArgs e)
        {
            richTextBoxMessage.SelectionStart = richTextBoxMessage.TextLength;
            richTextBoxMessage.ScrollToCaret();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Add at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxTeachType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int m_nError = -1;
            //try
            //{
            //    //if (comboBoxTeachType.SelectedIndex == 0)
            //    if (comboBoxTeachType.SelectedItem.ToString() == "Fiducial")
            //    {
            //        checkBoxEnableTeachItem.Visible = true;
            //    }
            //    //else if (comboBoxTeachType.SelectedIndex == 1)
            //    else if(comboBoxTeachType.SelectedItem.ToString() == "First Position")
            //    {
            //        comboBoxTeachItem.Items.Clear();
            //        comboBoxTeachItem.SelectedIndex = -1;
            //        comboBoxTeachItem.Text = "";
            //        comboBoxTeachItem.Enabled = false;
            //        textBoxRemarkTeachItem.Enabled = false;

            //        checkBoxEnableTeachItem.Visible = false;
            //        textBoxRemarkTeachItem.Text = "First Position";

            //        buttonVisionTeach.Text = "Teach First Pos.";
            //    }
            //    else if (comboBoxTeachType.SelectedIndex == 2)
            //    {

            //    }
            //}
            //catch (Exception ex)
            //{
            //    m_nError = -1;
            //    richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
            //    Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            //}
            return;
        }

        private void buttonTeach_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} User click teach at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                int nPositionX_um, nPositionY_um, nPositionTheta_mDegree, nPositionVisionFocusZ_um, nPositionZ_um, nPositionVisionOffsetX_um, nPositionVisionOffsetY_um, nPositionVisionOffsetTheta_mDegree;
                //comboBoxTeachType.Enabled = false;
                //comboBoxTeachItem.Enabled = false;
                m_ProductShareVariables.TeachItem = 0;

                if(comboBoxTeachItem.SelectedIndex == -1)
                {
                    updateRichTextBoxMessage("Please select teach item.");
                }
                if (Int32.TryParse(labelXReference2_um.Text, out nPositionX_um)
                                && Int32.TryParse(labelYReference2_um.Text, out nPositionY_um)
                                && Int32.TryParse(labelThetaReference2_mD.Text, out nPositionTheta_mDegree)
                                && Int32.TryParse(labelVZReference2_um.Text, out nPositionVisionFocusZ_um)
                                && Int32.TryParse(labelPZReference2_um.Text, out nPositionZ_um)
                                //&& Int32.TryParse(labelVisionOffsetX.Text, out nPositionVisionOffsetX_um)
                                //&& Int32.TryParse(labelVisionOffsetY.Text, out nPositionVisionOffsetY_um)
                                //&& Int32.TryParse(labelVisionOffsetTheta.Text, out nPositionVisionOffsetTheta_mDegree)
                                )
                {
                    if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                    {
                        int nCurrentCount = m_ProductRecipeVisionSetting.listInputVisionSnap.Count;
                        int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                        VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                        if (nDesireCount <= nCurrentCount)
                        {
                            visionSnapInfo = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex];
                            visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                            visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                            visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                            visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                            visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                            visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                            m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                        }
                        else
                        {
                            if (MessageBox.Show("Are you sure you want add on no of inspection position from " + nCurrentCount.ToString() + " " + nDesireCount.ToString() + "?", "Confirm Add Teach Position",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                for (int i = nCurrentCount; i < nDesireCount; i++)
                                {
                                    m_ProductRecipeVisionSetting.listInputVisionSnap.Add(new VisionSnapInfo { SnapNo = i, EnableSnap = false, Description = "NA", VisionXAxisOffset = 0, VisionYAxisOffset = 0, VisionThetaAxisOffset = 0, VisionZAxisOffset = 0 });
                                }
                                visionSnapInfo = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex];
                                visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                                visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                                visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                                visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                                visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                                visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                                m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            }
                        }
                        labelXReference_um.Text = nPositionX_um.ToString();
                        labelYReference_um.Text = nPositionY_um.ToString();
                        labelThetaReference_mDegree.Text = nPositionTheta_mDegree.ToString();
                        labelVZReference_um.Text = nPositionVisionFocusZ_um.ToString();
                        labelPZReference_um.Text = nPositionZ_um.ToString();

                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                    {
                        int nCurrentCount = m_ProductRecipeVisionSetting.listS2VisionSnap.Count;
                        int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                        VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                        if (nDesireCount <= nCurrentCount)
                        {
                            visionSnapInfo = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex];
                            visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                            visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                            visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                            visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                            visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                            visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                            m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                        }
                        else
                        {
                            if (MessageBox.Show("Are you sure you want add on no of inspection position from " + nCurrentCount.ToString() + " " + nDesireCount.ToString() + "?", "Confirm Add Teach Position",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                for (int i = nCurrentCount; i < nDesireCount; i++)
                                {
                                    m_ProductRecipeVisionSetting.listS2VisionSnap.Add(new VisionSnapInfo { SnapNo = i, EnableSnap = false, Description = "NA", VisionXAxisOffset = 0, VisionYAxisOffset = 0, VisionThetaAxisOffset = 0, VisionZAxisOffset = 0 });
                                }
                                visionSnapInfo = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex];
                                visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                                visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                                visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                                visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                                visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                                visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                                m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            }
                        }
                        labelXReference_um.Text = nPositionX_um.ToString();
                        labelYReference_um.Text = nPositionY_um.ToString();
                        labelThetaReference_mDegree.Text = nPositionTheta_mDegree.ToString();
                        labelVZReference_um.Text = nPositionVisionFocusZ_um.ToString();
                        labelPZReference_um.Text = nPositionZ_um.ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                    {
                        int nCurrentCount = m_ProductRecipeVisionSetting.listS1VisionSnap.Count;
                        int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                        VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                        if (nDesireCount <= nCurrentCount)
                        {
                            visionSnapInfo = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex];
                            visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                            visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                            visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                            visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                            visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                            visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                            m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                        }
                        else
                        {
                            if (MessageBox.Show("Are you sure you want add on no of inspection position from " + nCurrentCount.ToString() + " " + nDesireCount.ToString() + "?", "Confirm Add Teach Position",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                for (int i = nCurrentCount; i < nDesireCount; i++)
                                {
                                    m_ProductRecipeVisionSetting.listS1VisionSnap.Add(new VisionSnapInfo { SnapNo = i, EnableSnap = false, Description = "NA", VisionXAxisOffset = 0, VisionYAxisOffset = 0, VisionThetaAxisOffset = 0, VisionZAxisOffset = 0 });
                                }
                                visionSnapInfo = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex];
                                visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                                visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                                visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                                visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                                visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                                visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                                m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            }
                        }
                        labelXReference_um.Text = nPositionX_um.ToString();
                        labelYReference_um.Text = nPositionY_um.ToString();
                        labelThetaReference_mDegree.Text = nPositionTheta_mDegree.ToString();
                        labelVZReference_um.Text = nPositionVisionFocusZ_um.ToString();
                        labelPZReference_um.Text = nPositionZ_um.ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                    {
                        int nCurrentCount = m_ProductRecipeVisionSetting.listS3VisionSnap.Count;
                        int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                        VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                        if (nDesireCount <= nCurrentCount)
                        {
                            visionSnapInfo = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex];
                            visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                            visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                            visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                            visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                            visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                            visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                            m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                        }
                        else
                        {
                            if (MessageBox.Show("Are you sure you want add on no of inspection position from " + nCurrentCount.ToString() + " " + nDesireCount.ToString() + "?", "Confirm Add Teach Position",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                for (int i = nCurrentCount; i < nDesireCount; i++)
                                {
                                    m_ProductRecipeVisionSetting.listS3VisionSnap.Add(new VisionSnapInfo { SnapNo = i, EnableSnap = false, Description = "NA", VisionXAxisOffset = 0, VisionYAxisOffset = 0, VisionThetaAxisOffset = 0, VisionZAxisOffset = 0 });
                                }
                                visionSnapInfo = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex];
                                visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                                visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                                visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                                visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                                visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                                visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                                m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            }
                        }
                        labelXReference_um.Text = nPositionX_um.ToString();
                        labelYReference_um.Text = nPositionY_um.ToString();
                        labelThetaReference_mDegree.Text = nPositionTheta_mDegree.ToString();
                        labelVZReference_um.Text = nPositionVisionFocusZ_um.ToString();
                        labelPZReference_um.Text = nPositionZ_um.ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision)
                    {

                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                    {
                        int nCurrentCount = m_ProductRecipeVisionSetting.listBottomVisionSnap.Count;
                        int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                        VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                        if (nDesireCount <= nCurrentCount)
                        {
                            visionSnapInfo = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex];
                            visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                            visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                            visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                            visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                            visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                            visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                            m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                        }
                        else
                        {
                            if (MessageBox.Show("Are you sure you want add on no of inspection position from " + nCurrentCount.ToString() + " " + nDesireCount.ToString() + "?", "Confirm Add Teach Position",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                for (int i = nCurrentCount; i < nDesireCount; i++)
                                {
                                    m_ProductRecipeVisionSetting.listBottomVisionSnap.Add(new VisionSnapInfo { SnapNo = i, EnableSnap = false, Description = "NA", VisionXAxisOffset = 0, VisionYAxisOffset = 0, VisionThetaAxisOffset = 0, VisionZAxisOffset = 0 });
                                }
                                visionSnapInfo = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex];
                                visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                                visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                                visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                                visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                                visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                                visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                                m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            }
                        }
                        labelXReference_um.Text = nPositionX_um.ToString();
                        labelYReference_um.Text = nPositionY_um.ToString();
                        labelThetaReference_mDegree.Text = nPositionTheta_mDegree.ToString();
                        labelVZReference_um.Text = nPositionVisionFocusZ_um.ToString();
                        labelPZReference_um.Text = nPositionZ_um.ToString();
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                    {
                        int nCurrentCount = m_ProductRecipeVisionSetting.listOutputVisionSnap.Count;
                        int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                        VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                        if (nDesireCount <= nCurrentCount)
                        {
                            visionSnapInfo = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex];
                            visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                            visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                            visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                            visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                            visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                            visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                            m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                        }
                        else
                        {
                            if (MessageBox.Show("Are you sure you want add on no of inspection position from " + nCurrentCount.ToString() + " " + nDesireCount.ToString() + "?", "Confirm Add Teach Position",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                for (int i = nCurrentCount; i < nDesireCount; i++)
                                {
                                    m_ProductRecipeVisionSetting.listOutputVisionSnap.Add(new VisionSnapInfo { SnapNo = i, EnableSnap = false, Description = "NA", VisionXAxisOffset = 0, VisionYAxisOffset = 0, VisionThetaAxisOffset = 0, VisionZAxisOffset = 0 });
                                }
                                visionSnapInfo = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex];
                                visionSnapInfo.VisionXAxisOffset = nPositionX_um;
                                visionSnapInfo.VisionYAxisOffset = nPositionY_um;
                                visionSnapInfo.VisionThetaAxisOffset = nPositionTheta_mDegree;
                                visionSnapInfo.VisionZAxisOffset = nPositionVisionFocusZ_um;
                                visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                                visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                                m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                            }
                        }
                        labelXReference_um.Text = nPositionX_um.ToString();
                        labelYReference_um.Text = nPositionY_um.ToString();
                        labelThetaReference_mDegree.Text = nPositionTheta_mDegree.ToString();
                        labelVZReference_um.Text = nPositionVisionFocusZ_um.ToString();
                        labelPZReference_um.Text = nPositionZ_um.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonVerify_Click(object sender, EventArgs e)
        {
            Machine.EventLogger.WriteLog(string.Format("{0} User click verify at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
            int nPositionX_um, nPositionY_um, nPositionTheta_mDegree, nPositionVisionFocusZ_um, nPositionZ_um, nPositionVisionOffsetX_um, nPositionVisionOffsetY_um, nPositionVisionOffsetTheta_mDegree;
            //comboBoxTeachType.Enabled = false;
            comboBoxTeachItem.Enabled = false;

            //if (comboBoxTeachType.SelectedItem.ToString() == "Fiducial")
            //{
            //    if (Int32.TryParse(buttonVisionTeach.Text.Substring(buttonVisionTeach.Text.Length - 2, 2), out m_ProductShareVariables.TeachItem) == false)
            //    {
            //        updateRichTextBoxMessage("Fail to convert Fiducial no");
            //    }
            //}
            //else if (comboBoxTeachType.SelectedItem.ToString() == "First Position")
            //{
            //    m_ProductShareVariables.TeachItem = 11;
            //}
            //else if (comboBoxTeachType.SelectedIndex == 2)
            //{

            //}
            //if (comboBoxTeachType.SelectedItem.ToString() == "Fiducial")
            //{

            //}
            //else if (comboBoxTeachType.SelectedItem.ToString() == "First Position")
            //{
            //    //if (m_RecipeVision.FiducialData != null)
            //    {
            //        //if (comboBoxTeachItem.SelectedIndex >= 0 && comboBoxTeachItem.SelectedIndex < m_RecipeVision.FiducialData.Length)
            //        {
            //            //labelVisionOffsetX.Text = "NA".ToString();
            //            //labelVisionOffsetY.Text = "NA".ToString();
            //            //labelVisionOffsetTheta.Text = "NA".ToString();
            //            if (Int32.TryParse(labelCenterX.Text, out nPositionX_um)
            //                && Int32.TryParse(labelCenterY.Text, out nPositionY_um)
            //                && Int32.TryParse(labelCenterTheta.Text, out nPositionTheta_mDegree)
            //                && Int32.TryParse(labelVZReference2_um.Text, out nPositionVisionZ_um)
            //                && Int32.TryParse(labelPZReference2_um.Text, out nPositionUnitSupportZ_um)
            //                && Int32.TryParse(labelVisionOffsetX.Text, out nPositionVisionOffsetX_um)
            //                && Int32.TryParse(labelVisionOffsetY.Text, out nPositionVisionOffsetY_um)
            //                && Int32.TryParse(labelVisionOffsetTheta.Text, out nPositionVisionOffsetTheta_mDegree)
            //                )
            //            {
            //                if (VerifyFirstPosition(labelAbsoluteX.Text, labelAbsoluteY.Text, labelAbsoluteTheta.Text) != 0)
            //                {
            //                    comboBoxTeachType.Enabled = true;
            //                    comboBoxTeachItem.Enabled = true;
            //                    updateRichTextBoxMessage("Verify first position fail");
            //                    return;
            //                }
            //                eTeachPosition.Reset();
            //                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TEACH_MAP_POS", false);
            //                Machine.EventLogger.WriteLog(string.Format("{0} Verifying first position at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
            //            }
            //            else
            //            {
            //                updateRichTextBoxMessage("Position is not an integer");
            //            }
            //        }
            //    }

            //}
            //else if (comboBoxTeachType.SelectedIndex == 2)
            //{

            //}

            if (Int32.TryParse(labelXReference_um.Text, out nPositionX_um)
                && Int32.TryParse(labelYReference_um.Text, out nPositionY_um)
                && Int32.TryParse(labelThetaReference_mDegree.Text, out nPositionTheta_mDegree)
                && Int32.TryParse(labelPZReference_um.Text, out nPositionZ_um)
                && Int32.TryParse(labelVZReference_um.Text, out nPositionVisionFocusZ_um)
                )
            {
                if (VerifyPosition(nPositionX_um + m_nInitialPositionSet.PickAndPlaceXPosition, nPositionY_um + m_nInitialPositionSet.PickAndPlaceYPosition
                    , nPositionTheta_mDegree + m_nInitialPositionSet.PickAndPlaceThetaPosition, nPositionZ_um + m_nInitialPositionSet.PickAndPlaceZPosition
                    , nPositionVisionFocusZ_um + m_nInitialPositionSet.VisionFocusPosition) != 0)
                {
                    //comboBoxTeachType.Enabled = true;
                    comboBoxTeachItem.Enabled = true;
                    updateRichTextBoxMessage("Verify first position fail");
                    return;
                }
                eTeachPosition.Reset();
                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_VERIFY_POS_START", true);
                //m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_VERIFY_POS_DONE", true);
                Machine.EventLogger.WriteLog(string.Format("{0} Verifying first position at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
            }
            else
            {
                updateRichTextBoxMessage("Position is not an integer");
            }
        }

        private void comboBoxTeachItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            int m_nError = -1;
            try
            {

                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                {
                    if (comboBoxTeachItem.SelectedIndex + 1 > m_ProductRecipeVisionSetting.listInputVisionSnap.Count)
                    { 
                        updateRichTextBoxMessage("Please teach if new position is require.");
                        return;
                    }
                    labelXReference_um.Text = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].VisionXAxisOffset.ToString();
                    labelYReference_um.Text = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].VisionYAxisOffset.ToString();
                    labelThetaReference_mDegree.Text = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].VisionThetaAxisOffset.ToString();
                    labelVZReference_um.Text = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].VisionZAxisOffset.ToString();
                    checkBoxEnableTeachItem.Checked = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap;
                    textBoxRemarkTeachItem.Text = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].Description;
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                {
                    if (comboBoxTeachItem.SelectedIndex + 1 > m_ProductRecipeVisionSetting.listS2VisionSnap.Count)
                    { 
                        updateRichTextBoxMessage("Please teach if new position is require.");
                        return;
                    }
                    labelXReference_um.Text = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex].VisionXAxisOffset.ToString();
                    labelYReference_um.Text = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex].VisionYAxisOffset.ToString();
                    labelThetaReference_mDegree.Text = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex].VisionThetaAxisOffset.ToString();
                    labelVZReference_um.Text = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex].VisionZAxisOffset.ToString();
                    checkBoxEnableTeachItem.Checked = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap;
                    textBoxRemarkTeachItem.Text = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex].Description;
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                {
                    if (comboBoxTeachItem.SelectedIndex + 1 > m_ProductRecipeVisionSetting.listS1VisionSnap.Count)
                    {
                        updateRichTextBoxMessage("Please teach if new position is require.");
                        return;
                    }
                    labelXReference_um.Text = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex].VisionXAxisOffset.ToString();
                    labelYReference_um.Text = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex].VisionYAxisOffset.ToString();
                    labelThetaReference_mDegree.Text = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex].VisionThetaAxisOffset.ToString();
                    labelVZReference_um.Text = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex].VisionZAxisOffset.ToString();
                    checkBoxEnableTeachItem.Checked = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap;
                    textBoxRemarkTeachItem.Text = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex].Description;
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                {
                    if (comboBoxTeachItem.SelectedIndex + 1 > m_ProductRecipeVisionSetting.listS3VisionSnap.Count)
                    {
                        updateRichTextBoxMessage("Please teach if new position is require.");
                        return;
                    }
                    labelXReference_um.Text = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex].VisionXAxisOffset.ToString();
                    labelYReference_um.Text = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex].VisionYAxisOffset.ToString();
                    labelThetaReference_mDegree.Text = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex].VisionThetaAxisOffset.ToString();
                    labelVZReference_um.Text = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex].VisionZAxisOffset.ToString();
                    checkBoxEnableTeachItem.Checked = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap;
                    textBoxRemarkTeachItem.Text = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex].Description;
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision)
                {

                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                {
                    if (comboBoxTeachItem.SelectedIndex + 1 > m_ProductRecipeVisionSetting.listBottomVisionSnap.Count)
                    {
                        updateRichTextBoxMessage("Please teach if new position is require.");
                        return;
                    }
                    labelXReference_um.Text = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex].VisionXAxisOffset.ToString();
                    labelYReference_um.Text = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex].VisionYAxisOffset.ToString();
                    labelThetaReference_mDegree.Text = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex].VisionThetaAxisOffset.ToString();
                    labelVZReference_um.Text = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex].VisionZAxisOffset.ToString();
                    checkBoxEnableTeachItem.Checked = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap;
                    textBoxRemarkTeachItem.Text = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex].Description;
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                {
                    if (comboBoxTeachItem.SelectedIndex + 1 > m_ProductRecipeVisionSetting.listOutputVisionSnap.Count)
                    {
                        updateRichTextBoxMessage("Please teach if new position is require.");
                        return;
                    }
                    labelXReference_um.Text = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex].VisionXAxisOffset.ToString();
                    labelYReference_um.Text = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex].VisionYAxisOffset.ToString();
                    labelThetaReference_mDegree.Text = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex].VisionThetaAxisOffset.ToString();
                    labelVZReference_um.Text = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex].VisionZAxisOffset.ToString();
                    checkBoxEnableTeachItem.Checked = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap;
                    textBoxRemarkTeachItem.Text = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex].Description;
                }
            }
            catch (Exception ex)
            {
                m_nError = -1;
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return;
        }

        private void checkBoxEnableTeachItem_Click(object sender, EventArgs e)
        {
            int m_nError = -1;
            try
            {
                //if (comboBoxTeachType.SelectedItem.ToString() == "Fiducial")
                //{

                //}
                //else if (comboBoxTeachType.SelectedItem.ToString() == "First Position")
                //{

                //}
                //else if (comboBoxTeachType.SelectedIndex == 2)
                //{

                //}
            }
            catch (Exception ex)
            {
                m_nError = -1;
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return;
        }

        private void textBoxRemarkTeachItem_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxTeachItem.SelectedIndex == -1)
                {
                    return;
                }
                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listInputVisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                        m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listS2VisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                        m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listS1VisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                        m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listS3VisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                        m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision)
                {

                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listBottomVisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                        m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                        //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listOutputVisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.Description = textBoxRemarkTeachItem.Text;
                        m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                        //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            int m_nError = -1;
            try
            {
                if (MessageBox.Show("Are you sure you want to Apply and Save recipe?", "Confirm Apply and Save",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    //updateRichTextBoxMessage("");
                    //if (comboBoxRecipeVision.SelectedIndex != -1)
                    {
                        //if (VerifyVisionRecipe())
                        {
                            if (SaveSettings())
                            {
                                LoadSettingToShareMemory();
                                updateRichTextBoxMessage("Save recipe succesfully.");
                            }
                            else
                            {
                                updateRichTextBoxMessage("Save recipe fail");
                            }
                            //updateRichTextBoxMessageRecipeVision("Apply and save recipe succesfully.");
                        }

                        //}
                        //else
                        //{
                        //    updateRichTextBoxMessageRecipeVision("Please select recipe before click Apply and Save.");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                m_nError = -1;
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return;
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            //if (radioButtonAlignmentByEnvelop.Checked == true)
            //{
            //    //m_ProductRTSSProcess.SetSettingBool("EnablePositioningByEnvelope", true);
            //}

            //m_ProductRTSSProcess.SetProductionInt("nEdgeDieCoordinateY", (int)numericUpDownStartRow.Value);
            //m_ProductRTSSProcess.SetProductionInt("nEdgeDieCoordinateX", (int)numericUpDownStartColumn.Value);

           
            //if (checkBoxDisableVerifyFirstUnit.Visible == true && checkBoxDisableVerifyFirstUnit.Enabled == true)
            //{
            //    if (checkBoxDisableVerifyFirstUnit.Checked == true)
            //    {
            //        m_ProductRTSSProcess.SetProductionBool("DisableVerifyFirstPosition", true);
            //    }
            //    else
            //    {
            //        m_ProductRTSSProcess.SetProductionBool("DisableVerifyFirstPosition", false);
            //    }
            //}
            updateRichTextBoxMessage("Continue production.");
            m_ProductProcessEvent.PCS_PCS_Set_Vision_Close_Teach_Window.Set();
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TEACH_VISION_DONE", true);

            if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
            {
                m_ProductRTSSProcess.SetEvent("StartReset", true);
                Thread.Sleep(1000);
                m_ProductRTSSProcess.SetEvent("StartJob", true);
            }

            if (m_bgwUserInterface.WorkerSupportsCancellation == true)
            {
                m_bgwUserInterface.CancelAsync();
            }
        }


        private void buttonUnload_Click(object sender, EventArgs e)
        {
            //m_ProductRTSSProcess.SetEvent("JobStop", true);
            //if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
            //{
            //    m_ProductRTSSProcess.SetEvent("StartReset", true);
            //    Thread.Sleep(500);
            //    m_ProductRTSSProcess.SetEvent("StartJob", true);
            //}
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_RETEACH_MAP_START") == false)
            {
                m_ProductRTSSProcess.SetEvent("JobStop", true);
                if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                {
                    m_ProductRTSSProcess.SetEvent("StartReset", true);
                    Thread.Sleep(500);
                    m_ProductRTSSProcess.SetEvent("StartJob", true);
                }
            }
            else
            {
                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_RETEACH_MAP_UNLOAD", true); 
            }
            updateRichTextBoxMessage("Unload frame or tile.");
            m_ProductProcessEvent.PCS_PCS_Set_Vision_Close_Teach_Window.Set();

            if (m_bgwUserInterface.WorkerSupportsCancellation == true)
            {
                m_bgwUserInterface.CancelAsync();
            }
        }

        private void buttonBackup_Click(object sender, EventArgs e)
        {

        }

        private void buttonRestore_Click(object sender, EventArgs e)
        {
            int m_nError = -1;
            try
            {
                if (MessageBox.Show("Are you sure you want to restore recipe?", "Confirm Restore",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    updateRichTextBoxMessage("Restore recipe succesfully.");
                    if (LoadSettings())
                    {
                        updateRichTextBoxMessage("Restore recipe succesfully.");
                        UpdateGUI();
                        updateRichTextBoxMessage("Refresh GUI done.");
                    }
                }
            }
            catch (Exception ex)
            {
                m_nError = -1;
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return;
        }

        private void buttonInputVisionZAxisForward_Click(object sender, EventArgs e)
        {

        }

        private void buttonInputVisionZAxisReverse_Click(object sender, EventArgs e)
        {

        }

        private void buttonUnitSupportZAxisForward_Click(object sender, EventArgs e)
        {

        }

        private void buttonUnitSupportZAxisReverse_Click(object sender, EventArgs e)
        {

        }

        private void buttonInputTableYForward_Click(object sender, EventArgs e)
        {

        }

        private void buttonInputTableXReserve_Click(object sender, EventArgs e)
        {

        }

        private void buttonInputTableXForward_Click(object sender, EventArgs e)
        {

        }

        private void buttonInputTableYReverse_Click(object sender, EventArgs e)
        {

        }

        private void buttonInputTableThetaReverse_Click(object sender, EventArgs e)
        {

        }

        private void buttonInputTableThetaForward_Click(object sender, EventArgs e)
        {

        }

        private void buttonVisionZAxisForward_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.InputVisionZAxis, Math.Abs(nPosition));
                        m_nPosition = Math.Abs(nPosition);
                        bVisionZAxisForward = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonVisionZAxisForward_MouseUp(object sender, MouseEventArgs e)
        {
            bVisionZAxisForward = false;
            Thread.Sleep(100);
        }

        private void buttonVisionZAxisReverse_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.InputVisionZAxis, Math.Abs(nPosition) * -1);
                        m_nPosition = m_nPosition = Math.Abs(nPosition) * -1;
                        bVisionZAxisReverse = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonVisionZAxisReverse_MouseUp(object sender, MouseEventArgs e)
        {
            bVisionZAxisReverse = false;
            Thread.Sleep(100);
        }

        private void buttonPickAndPlaceZAxisForward_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.UnitSupportZAxis, Math.Abs(nPosition));
                        m_nPosition = Math.Abs(nPosition);
                        bPickAndPlaceZAxisForward = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonPickAndPlaceZAxisForward_MouseUp(object sender, MouseEventArgs e)
        {
            bPickAndPlaceZAxisForward = false;
            Thread.Sleep(100);
        }

        private void buttonPickAndPlaceZAxisReverse_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.UnitSupportZAxis, Math.Abs(nPosition) * -1);
                        m_nPosition = Math.Abs(nPosition) * -1;
                        bPickAndPlaceZAxisReverse = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonUnitSupportZAxisReverse_MouseUp(object sender, MouseEventArgs e)
        {
            bPickAndPlaceZAxisReverse = false;
            Thread.Sleep(100);
        }

        private void buttonInputTableYForward_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.InputTableYAxis, Math.Abs(nPosition));
                        m_nPosition = Math.Abs(nPosition);
                        bPickAndPlaceYForward = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonInputTableYForward_MouseUp(object sender, MouseEventArgs e)
        {
            bPickAndPlaceYForward = false;
            Thread.Sleep(100);
        }

        private void buttonInputTableXReserve_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.InputTableXAxis, Math.Abs(nPosition) * -1);
                        m_nPosition = Math.Abs(nPosition) * -1;
                        bPickAndPlaceXReserve = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonInputTableXReserve_MouseUp(object sender, MouseEventArgs e)
        {
            bPickAndPlaceXReserve = false;
            Thread.Sleep(100);
        }

        private void buttonInputTableYReverse_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.InputTableYAxis, Math.Abs(nPosition) * -1);
                        m_nPosition = Math.Abs(nPosition) * -1;
                        bPickAndPlaceYReverse = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonInputTableYReverse_MouseUp(object sender, MouseEventArgs e)
        {
            bPickAndPlaceYReverse = false;
            Thread.Sleep(100);
        }

        private void buttonInputTableXForward_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.InputTableXAxis, Math.Abs(nPosition));
                        m_nPosition = Math.Abs(nPosition);
                        bPickAndPlaceXForward = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonInputTableXForward_MouseUp(object sender, MouseEventArgs e)
        {
            bPickAndPlaceXForward = false;
            Thread.Sleep(100);
        }

        private void buttonInputTableThetaReverse_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.InputTableThetaAxis, Math.Abs(nPosition) * -1);
                        m_nPosition = Math.Abs(nPosition) * -1;
                        bPickAndPlaceThetaReverse = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonInputTableThetaReverse_MouseUp(object sender, MouseEventArgs e)
        {
            bPickAndPlaceThetaReverse = false;
            Thread.Sleep(100);
        }

        private void buttonInputTableThetaForward_MouseDown(object sender, MouseEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                int nPosition;
                if (comboBoxMoveStep_um_mD != null)
                {
                    if (Int32.TryParse(comboBoxMoveStep_um_mD.Text, out nPosition) == true)
                    {
                        Thread.Sleep(100);
                        //MoveRelative(MotorAxis.InputTableThetaAxis, Math.Abs(nPosition));
                        m_nPosition = Math.Abs(nPosition);
                        bPickAndPlaceThetaForward = true;
                        m_HPTMouseDown.Reset();
                        m_HPTMouseDown.Start();
                    }
                    else
                    {
                        updateRichTextBoxMessage("Invalid move step size");
                    }
                }
                else
                {
                    updateRichTextBoxMessage("Invalid move step size");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonInputTableThetaForward_MouseUp(object sender, MouseEventArgs e)
        {
            bPickAndPlaceThetaForward = false;
            Thread.Sleep(100);
        }

        private void buttonStopMovement_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                bVisionZAxisForward = false;
                bVisionZAxisReverse = false;
                bPickAndPlaceZAxisForward = false;
                bPickAndPlaceZAxisReverse = false;
                bPickAndPlaceYForward = false;
                bPickAndPlaceXReserve = false;
                bPickAndPlaceYReverse = false;
                bPickAndPlaceXForward = false;
                bPickAndPlaceThetaReverse = false;
                bPickAndPlaceThetaForward = false;
                m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_STOP", true);
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonSetVisionLiveOn_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                m_ProductProcessEvent.PCS_PCS_Set_Vision_Live_On.Set();
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonSetVisionLiveOff_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                m_ProductProcessEvent.PCS_PCS_Set_Vision_Live_Off.Set();
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonBackLightUp_Click(object sender, EventArgs e)
        {

        }

        private void buttonBackLightDown_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxTeachItem_KeyDown(object sender, KeyEventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                if (e.KeyValue == 13)
                {
                    //if (comboBoxTeachType.SelectedIndex == 0)
                    //if (comboBoxTeachType.SelectedItem.ToString() == "Fiducial")
                    //{
                    //if (m_ProductRecipeVisionSetting. != null)
                    //{
                    //    if (comboBoxTeachItem.SelectedIndex >= 0 && comboBoxTeachItem.SelectedIndex < m_ProductRecipeVisionSetting.FiducialData.Length)
                    //    {
                    //        m_ProductRecipeVisionSetting.FiducialData[comboBoxTeachItem.SelectedIndex].FiducialName = comboBoxTeachItem.Text;
                    //        comboBoxTeachItem.Items[comboBoxTeachItem.SelectedIndex] = comboBoxTeachItem.Text;
                    //        comboBoxTeachItem.SelectedItem = m_ProductRecipeVisionSetting.FiducialData[comboBoxTeachItem.SelectedIndex].FiducialName;
                    //    }
                    //}
                    //}
                    //else if (comboBoxTeachType.SelectedIndex == 1)
                    //else if (comboBoxTeachType.SelectedItem.ToString() == "First Position")
                    //{

                    //}
                    //else if (comboBoxTeachType.SelectedIndex == 2)
                    //{

                    //}


                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonVisionTeach_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                m_ProductShareVariables.TeachItem = 0;
                //if (buttonVisionTeach.Text.Contains("Fiducial"))
                //{
                //    if (Int32.TryParse(buttonVisionTeach.Text.Substring(buttonVisionTeach.Text.Length - 2, 2), out ShareVariables.TeachItem) == false)
                //    {
                //        updateRichTextBoxMessage("Fail to convert Fiducial no");
                //    }
                //}
                //else if (buttonVisionTeach.Text.Contains("First"))
                //{
                //    ShareVariables.TeachItem = 11;
                //}
                //if (comboBoxTeachType.SelectedIndex == 0)
                if (comboBoxTeachType.SelectedItem.ToString() == "Fiducial")
                {
                    if (Int32.TryParse(buttonVisionTeach.Text.Substring(buttonVisionTeach.Text.Length - 2, 2), out m_ProductShareVariables.TeachItem) == false)
                    {
                        updateRichTextBoxMessage("Fail to convert Fiducial no");
                    }
                }
                //else if (comboBoxTeachType.SelectedIndex == 1)
                else if (comboBoxTeachType.SelectedItem.ToString() == "First Position")
                {
                    m_ProductShareVariables.TeachItem = 11;
                }
                else if (comboBoxTeachType.SelectedIndex == 2)
                {

                }

                m_ProductProcessEvent.PCS_PCS_Set_Vision_Teach.Set();
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void buttonLaunchVisionTeachWindow_Click(object sender, EventArgs e)
        {
            m_ProductProcessEvent.PCS_PCS_Set_Vision_Launch_Teach_Window.Set();
        }

        private void buttonVisionTeachTheta_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;
            try
            {
                m_ProductShareVariables.TeachItem = 12;
                m_ProductProcessEvent.PCS_PCS_Set_Vision_Teach.Set();
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
            }
        }

        private void checkBoxEnablePositioningByFiducial_CheckedChanged(object sender, EventArgs e)
        {
            //m_recipeVisionSetting.EnablePositioningByFiducial = checkBoxEnablePositioningByFiducial.Checked;
        }

        private void checkBoxTriggerTeachAlignment_CheckedChanged(object sender, EventArgs e)
        {
            m_ProductRecipeVisionSetting.EnableTeachUnitAtVision = checkBoxTriggerLearnVision.Checked;
        }
        
        private void buttonInputTableYReverse_MouseLeave(object sender, EventArgs e)
        {
            //updateRichTextBoxMessage("mouse leave");
        }

        private void buttonClose_Click(object sender, FormClosedEventArgs e)
        {

        }

        private void FormTeachMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TEACHING_VISION", false);
        }
        #endregion

        #endregion Form Teach Map

        #region Private        

        public void updateRichTextBoxMessage(string message)
        {
            Machine.GeneralTools.UpdateRichTextBoxMessage(richTextBoxMessage, message);
            Machine.EventLogger.WriteLog(string.Format("{0} " + message + " at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
        }

        #endregion Private

        #region Public
        virtual public void Initialize()
        {
            try
            {
                LoadSettings();
                UpdateGUI();
                GetInitialPosition(out m_nInitialPositionSet);
                m_ProductProcessEvent.PCS_PCS_Set_Vision_Launch_Teach_Window.Set();
                m_bgwUserInterface = new BackgroundWorker();
                m_bgwUserInterface.WorkerReportsProgress = true;
                m_bgwUserInterface.WorkerSupportsCancellation = true;
                m_bgwUserInterface.DoWork += new DoWorkEventHandler(bgwUserInterface_DoWork);
                m_bgwUserInterface.ProgressChanged += new ProgressChangedEventHandler(bgwUserInterface_UpdateInterface);
                m_bgwUserInterface.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwUserInterface_Complete);

                m_bgwUserInterface.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        virtual public bool LoadSettings()
        {
            try
            {
                if (Tools.IsFileExist(m_ProductShareVariables.strRecipeVisionPath, m_ProductShareVariables.productRecipeMainSettings.VisionRecipeName, m_ProductShareVariables.strXmlExtension))
                {
                    m_ProductRecipeVisionSetting = Tools.Deserialize<ProductRecipeVisionSettings>(m_ProductShareVariables.strRecipeVisionPath + m_ProductShareVariables.productRecipeMainSettings.VisionRecipeName + m_ProductShareVariables.strXmlExtension);
                }
                else
                {
                    updateRichTextBoxMessage("Vision recipe not exist");
                }
                if (File.Exists(m_ProductShareVariables.strRecipeInputPath + m_ProductShareVariables.recipeMainSettings.InputRecipeName + m_ProductShareVariables.strXmlExtension))
                {
                    m_ProductRecipeInputSetting = Tools.Deserialize<ProductRecipeInputSettings>(m_ProductShareVariables.strRecipeInputPath + m_ProductShareVariables.recipeMainSettings.InputRecipeName + m_ProductShareVariables.strXmlExtension);
                }
                else
                {
                    updateRichTextBoxMessage("Input recipe not exist");
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

        virtual public bool SaveSettings()
        {
            try
            {
                Tools.Serialize(m_ProductShareVariables.strRecipeVisionPath + m_ProductShareVariables.productRecipeMainSettings.VisionRecipeName + m_ProductShareVariables.strXmlExtension, m_ProductRecipeVisionSetting);
                Tools.Serialize(m_ProductShareVariables.strRecipeInputPath + m_ProductShareVariables.recipeMainSettings.InputRecipeName + m_ProductShareVariables.strXmlExtension, m_ProductRecipeInputSetting);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }
        virtual public bool GetInitialPosition(out PositionSet positionSet)
        {
            positionSet = new PositionSet();
            try
            {                
                positionSet.PickAndPlaceXPosition = 0;
                positionSet.PickAndPlaceYPosition = 0;
                positionSet.PickAndPlaceThetaPosition = 0;
                positionSet.PickAndPlaceZPosition = 0;
                positionSet.VisionFocusPosition = 0;

                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                {
                    positionSet.PickAndPlaceXPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1XAxisEncoderPosition");
                    positionSet.PickAndPlaceYPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1YAxisEncoderPosition");
                    positionSet.PickAndPlaceThetaPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ThetaAxisEncoderPosition");
                    positionSet.PickAndPlaceZPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition");
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                {
                    positionSet.PickAndPlaceXPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2XAxisEncoderPosition");
                    positionSet.PickAndPlaceYPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2YAxisEncoderPosition");
                    positionSet.PickAndPlaceThetaPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ThetaAxisEncoderPosition");
                    positionSet.PickAndPlaceZPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition");
                }

                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                {
                    positionSet.VisionFocusPosition = m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition");
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                {
                    positionSet.VisionFocusPosition = m_ProductRTSSProcess.GetProductionLong("S2VisionZAxisEncoderPosition");
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                {
                    positionSet.VisionFocusPosition = m_ProductRTSSProcess.GetProductionLong("S1VisionZAxisEncoderPosition");
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                {
                    positionSet.VisionFocusPosition = m_ProductRTSSProcess.GetProductionLong("S3VisionZAxisEncoderPosition");
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision
                   
                    || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                {
                    if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                    {
                        positionSet.VisionFocusPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition");
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                    {
                        positionSet.VisionFocusPosition = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }
        
        #endregion Public
        
        virtual public int UpdateGUI()
        {
            int m_nError = 0;
            try
            {
                //comboBoxTeachType.SelectedIndex = 0;
                //if (m_ProductRecipeVisionSetting.FiducialData != null)
                //{
                //    comboBoxTeachItem.Items.Clear();

                //    for (int i = 0; i < m_ProductRecipeVisionSetting.FiducialData.Length; i++)
                //    {
                //        if (m_ProductRecipeVisionSetting.FiducialData[i] != null)
                //        {
                //            comboBoxTeachItem.Items.Add(m_ProductRecipeVisionSetting.FiducialData[i].FiducialName);
                //        }
                //        else
                //        {
                //            m_ProductRecipeVisionSetting.FiducialData[i] = new FiducialSettings();
                //            comboBoxTeachItem.Items.Add(m_ProductRecipeVisionSetting.FiducialData[i].FiducialName);
                //        }
                //    }
                //    comboBoxTeachItem.SelectedIndex = 0;
                //}
                comboBoxTeachItem.Items.Clear();
                for (int i = 0; i < 10;i++)
                {
                    comboBoxTeachItem.Items.Add((i+1).ToString());
                }
                comboBoxTeachItem.SelectedIndex = 0;
                System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
                
                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                    UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace1XAxis);
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                    UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace2XAxis);
                sWatch.Start();
                while (m_ProductShareVariables.bFormMainInterfaceButtonExit == false && (sWatch.ElapsedMilliseconds < 100 && labelXReference2_um.Text == "0000000"))
                {
                    Thread.Sleep(1);
                }
                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                    UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace1YAxis);
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                    UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace2YAxis);
                sWatch.Restart();
                while (m_ProductShareVariables.bFormMainInterfaceButtonExit == false && (sWatch.ElapsedMilliseconds < 100 && labelYReference2_um.Text == "0000000"))
                {
                    Thread.Sleep(1);
                }
                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                    UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace1ThetaAxis);
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                    UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace2ThetaAxis);
                sWatch.Restart();
                while (m_ProductShareVariables.bFormMainInterfaceButtonExit == false && (sWatch.ElapsedMilliseconds < 100 && labelThetaReference2_mD.Text == "0000000"))
                {
                    Thread.Sleep(1);
                }
                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                    UpdateMemoryOnAxisChange(MotorAxis.InputVisionZAxis);
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                    UpdateMemoryOnAxisChange(MotorAxis.S2VisionZAxis);                
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                    UpdateMemoryOnAxisChange(MotorAxis.S1VisionZAxis);
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                    UpdateMemoryOnAxisChange(MotorAxis.S3VisionZAxis);
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision
                    || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision
                    || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                {
                    if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                        UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace1ZAxis);
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                        UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace2ZAxis);
                }

                sWatch.Restart();
                while (m_ProductShareVariables.bFormMainInterfaceButtonExit == false && (sWatch.ElapsedMilliseconds < 100 && labelVZReference2_um.Text == "0000000"))
                {
                    Thread.Sleep(1);
                }
                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                    UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace1ZAxis);
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                    UpdateMemoryOnAxisChange(MotorAxis.PickAndPlace2ZAxis);
                sWatch.Restart();
                while (m_ProductShareVariables.bFormMainInterfaceButtonExit == false && (sWatch.ElapsedMilliseconds < 100 && labelPZReference2_um.Text == "0000000"))
                {
                    Thread.Sleep(1);
                }
                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                {
                    this.Text = "Teach Input Vision";
                    groupBoxPickAndPlace.Enabled = false;
                    groupBoxPickAndPlaceZAxis.Enabled = false;
                    groupBoxVisionFocusAxis.Enabled = true;

                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                {
                    this.Text = "Teach S2 Vision";
                    groupBoxPickAndPlace.Enabled = false;
                    groupBoxPickAndPlaceZAxis.Enabled = false;
                    groupBoxVisionFocusAxis.Enabled = true;

                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                {
                    this.Text = "Teach S1 Vision";
                    groupBoxPickAndPlace.Enabled = false;
                    groupBoxPickAndPlaceZAxis.Enabled = false;
                    groupBoxVisionFocusAxis.Enabled = true;

                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                {
                    this.Text = "Teach S3 Vision";
                    groupBoxPickAndPlace.Enabled = true;
                    groupBoxPickAndPlaceZAxis.Enabled = false;
                    groupBoxVisionFocusAxis.Enabled = true;
                }
                else if (
                    m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision
                    )
                {
                    this.Text = "Teach S1 Vision";
                    groupBoxPickAndPlace.Enabled = true;
                    groupBoxPickAndPlaceZAxis.Enabled = false;
                    groupBoxVisionFocusAxis.Enabled = true;
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision)
                {
                    this.Text = "Teach Setup Vision";
                    groupBoxPickAndPlace.Enabled = false;
                    groupBoxPickAndPlaceZAxis.Enabled = false;
                    groupBoxVisionFocusAxis.Enabled = false;
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                {
                    this.Text = "Teach Output Vision";
                    groupBoxPickAndPlace.Enabled = false;
                    groupBoxPickAndPlaceZAxis.Enabled = false;
                    groupBoxVisionFocusAxis.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                m_nError = -1;
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return m_nError;
        }
 
        virtual public void updateRichTextBoxMessageInBackgroundworker(string message)
        {
            richTextBoxMessage.Text += DateTime.Now.ToString("yyyyMMdd HHmmss") + ": "+ message + Environment.NewLine;
            Machine.EventLogger.WriteLog(string.Format("{0} " + message + " at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
        }

        virtual public int UpdateMemoryOnAxisChange(MotorAxis motorAxis)
        {
            m_nErrorCode = 0;
            try
            {
                m_ProductRTSSProcess.SetGeneralInt("nTeachPointAxis", (int)Enum.Parse(typeof(MotorAxis), Enum.GetName(typeof(MotorAxis), motorAxis)));
                return m_nErrorCode;
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return m_nErrorCode = -1;
            }
        }

        virtual public int MoveRelative(MotorAxis motorAxis, int position)
        {
            m_nErrorCode = 0;
            try
            {
                if (IsMoveDone())
                {
                    m_ProductRTSSProcess.SetGeneralInt("nTeachPointAxis", (int)Enum.Parse(typeof(MotorAxis), Enum.GetName(typeof(MotorAxis), motorAxis)));
                    //RTSSProcess.SetShareMemoryGeneralInt("nMotorMovePosition", position);
                    //RTSSProcess.SetShareMemoryEvent("GTCH_RTCH_MOTOR_MV_REL", true);



                    //if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                    //{
                    //    if (labelXAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1XAxisEncoderPosition").ToString())
                    //        labelXAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1XAxisEncoderPosition").ToString();
                    //    if (labelYAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1YAxisEncoderPosition").ToString())
                    //        labelYAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1YAxisEncoderPosition").ToString();
                    //    if (labelThetaAbsolute2_mD.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ThetaAxisEncoderPosition").ToString())
                    //        labelThetaAbsolute2_mD.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ThetaAxisEncoderPosition").ToString();

                    //    if (labelPZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition").ToString())
                    //        labelPZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition").ToString();
                    //}
                    //else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                    //{
                    //    if (labelXAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace2XAxisEncoderPosition").ToString())
                    //        labelXAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2XAxisEncoderPosition").ToString();
                    //    if (labelYAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace2YAxisEncoderPosition").ToString())
                    //        labelYAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2YAxisEncoderPosition").ToString();
                    //    if (labelThetaAbsolute2_mD.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ThetaAxisEncoderPosition").ToString())
                    //        labelThetaAbsolute2_mD.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ThetaAxisEncoderPosition").ToString();

                    //    if (labelPZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition").ToString())
                    //        labelPZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition").ToString();
                    //}

                    //if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                    //{
                    //    if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition").ToString())
                    //        labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition").ToString();
                    //}
                    //else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                    //{
                    //    if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("S2VisionZAxisEncoderPosition").ToString())
                    //        labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("S2VisionZAxisEncoderPosition").ToString();
                    //}
                    //else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SidewallLeftVision)
                    //{
                    //    if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("SidewallLeftVisionZAxisEncoderPosition").ToString())
                    //        labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("SidewallLeftVisionZAxisEncoderPosition").ToString();
                    //}
                    //else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SidewallRightVision)
                    //{
                    //    if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("SidewallRightVisionZAxisEncoderPosition").ToString())
                    //        labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("SidewallRightVisionZAxisEncoderPosition").ToString();
                    //}
                    //else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SidewallFrontVision)
                    //{
                    //    if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("SidewallFrontVisionZAxisEncoderPosition").ToString())
                    //        labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("SidewallFrontVisionZAxisEncoderPosition").ToString();
                    //}
                    //else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SidewallRearVision)
                    //{
                    //    if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("SidewallRearVisionZAxisEncoderPosition").ToString())
                    //        labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("SidewallRearVisionZAxisEncoderPosition").ToString();
                    //}
                    //else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                    //{
                    //    if (labelVZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("S3VisionZAxisEncoderPosition").ToString())
                    //        labelVZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("S3VisionZAxisEncoderPosition").ToString();
                    //}
                    //else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision
                    //    || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision
                    //    || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                    //{
                    //    if (labelPZAbsolute2_um.Text != m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition").ToString())
                    //        labelPZAbsolute2_um.Text = m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition").ToString();
                    //}


                    switch (motorAxis)
                    {
                        case MotorAxis.PickAndPlace1XAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("PickAndPlace1XAxisEncoderPosition"));
                            break;
                        case MotorAxis.PickAndPlace1YAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("PickAndPlace1YAxisEncoderPosition"));
                            break;
                        case MotorAxis.PickAndPlace1ThetaAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ThetaAxisEncoderPosition"));
                            break;
                        case MotorAxis.PickAndPlace1ZAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("PickAndPlace1ZAxisEncoderPosition"));
                            break;

                        case MotorAxis.PickAndPlace2XAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("PickAndPlace2XAxisEncoderPosition"));
                            break;
                        case MotorAxis.PickAndPlace2YAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("PickAndPlace2YAxisEncoderPosition"));
                            break;
                        case MotorAxis.PickAndPlace2ThetaAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ThetaAxisEncoderPosition"));
                            break;
                        case MotorAxis.PickAndPlace2ZAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("PickAndPlace2ZAxisEncoderPosition"));
                            break;

                        case MotorAxis.InputVisionZAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("InputVisionZAxisEncoderPosition"));
                            break;
                        case MotorAxis.S2VisionZAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("S2VisionZAxisEncoderPosition"));
                            break;
                        case MotorAxis.S3VisionZAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("S3VisionZAxisEncoderPosition"));
                            break;

                        case MotorAxis.S1VisionZAxis:
                            m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position + m_ProductRTSSProcess.GetProductionLong("S1VisionZAxisEncoderPosition"));
                            break;
                        default:
                            MessageBox.Show("Error");
                            return 2;
                            break;
                    }
                    m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_MV_ABS", true);
                    return m_nErrorCode;
                }
                else
                {
                    m_nErrorCode = 1;
                    return m_nErrorCode;
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return m_nErrorCode = -1;
            }
        }

        virtual public int MoveAbsolute(MotorAxis motorAxis, int position)
        {
            m_nErrorCode = 0;
            try
            {
                if (IsMoveDone())
                {
                    m_ProductRTSSProcess.SetGeneralInt("nTeachPointAxis", (int)Enum.Parse(typeof(MotorAxis), Enum.GetName(typeof(MotorAxis), motorAxis)));
                    m_ProductRTSSProcess.SetGeneralInt("nMotorMovePosition", position);
                    m_ProductRTSSProcess.SetEvent("GTCH_RTCH_MOTOR_MV_ABS", true);
                    return m_nErrorCode;
                }
                else
                {
                    m_nErrorCode = 1;
                    return m_nErrorCode;
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return m_nErrorCode = -1;
            }
        }

        virtual public bool IsMoveDone()
        {
            m_nErrorCode = 0;
            try
            {
                if (m_ProductRTSSProcess.GetEvent("GTCH_RTCH_MOTOR_MV_REL") == false && m_ProductRTSSProcess.GetEvent("GTCH_RTCH_MOTOR_MV_ABS") == false
                    //&& m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_VERIFY_FIDUCIAL_START") == false && m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_VERIFY_FIRST_POS_START") == false
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                m_nErrorCode = -1;
                return false;
            }
        }

        virtual public int VerifyFiducial(string xPosition, string yPosition)
        {
            m_nErrorCode = 0;
            
            try
            {                 
                //ProcessEvent.PCS_PCS_Set_Vision_Live_Off.Set();

                //System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
                //updateRichTextBoxMessage(string.Format("Start move to X = {0},Y = {1} position", xPosition, yPosition));
                ////Move Input Table X                
                //if (IsMoveDone() == false)
                //{
                //    updateRichTextBoxMessage("Previous movement not yet done");
                //    m_nErrorCode = 1;
                //    return m_nErrorCode;
                //}

                //m_ProductRTSSProcess.SetProductionLong("InputTableXAxisMovePosition", Int32.Parse(xPosition));
                //m_ProductRTSSProcess.SetProductionLong("InputTableYAxisMovePosition", Int32.Parse(yPosition));
                ////RTSSProcess.SetShareMemoryEvent("RTHD_GMAIN_VERIFY_FIDUCIAL_DONE", false);
                //m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_VERIFY_FIDUCIAL_START", true);

                return m_nErrorCode;
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return m_nErrorCode = -1;
            }
        }

        virtual public int VerifyPosition(int xPosition, int yPosition, int thetaPosition, int zPosition, int focusPosition)
        {
            m_nErrorCode = 0;

            try
            {
                //ProcessEvent.PCS_PCS_Set_Vision_Live_Off.Set();

                System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();

                updateRichTextBoxMessage(string.Format("Start move to X = {0},Y = {1}, Theta = {2} position, Z = {3} position", xPosition, yPosition, thetaPosition, zPosition));

                //Move Input Table X                
                if (IsMoveDone() == false)
                {
                    updateRichTextBoxMessage("Previous movement not yet done");
                    return 1;
                }
                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                {
                    m_ProductRTSSProcess.SetProductionLong("PickAndPlace1XAxisMovePosition", xPosition);
                    m_ProductRTSSProcess.SetProductionLong("PickAndPlace1YAxisMovePosition", yPosition);
                    m_ProductRTSSProcess.SetProductionLong("PickAndPlace1ThetaAxisMovePosition", thetaPosition);
                    m_ProductRTSSProcess.SetProductionLong("PickAndPlace1ZAxisMovePosition", zPosition);
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                {
                    m_ProductRTSSProcess.SetProductionLong("PickAndPlace2XAxisMovePosition", xPosition);
                    m_ProductRTSSProcess.SetProductionLong("PickAndPlace2YAxisMovePosition", yPosition);
                    m_ProductRTSSProcess.SetProductionLong("PickAndPlace2ThetaAxisMovePosition", thetaPosition);
                    m_ProductRTSSProcess.SetProductionLong("PickAndPlace2ZAxisMovePosition", zPosition);
                }

                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                {
                    m_ProductRTSSProcess.SetProductionLong("InputVisionModuleMovePosition", focusPosition);
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                {
                    m_ProductRTSSProcess.SetProductionLong("S2VisionModuleMovePosition", focusPosition);
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                {
                    m_ProductRTSSProcess.SetProductionLong("S1VisionModuleMovePosition", focusPosition);                    
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                {
                    m_ProductRTSSProcess.SetProductionLong("S3VisionModuleMovePosition", focusPosition);
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision
                    || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision
                    || m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                {
                    if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 1)
                    {
                        m_ProductRTSSProcess.SetProductionLong("PickAndPlace1ZAxisMovePosition", focusPosition);
                    }
                    else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionPickAndPlace") == 2)
                    {
                        m_ProductRTSSProcess.SetProductionLong("PickAndPlace2ZAxisMovePosition", focusPosition);
                    }
                }
                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_VERIFY_POS_START", true);

                return m_nErrorCode;
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return m_nErrorCode = -1;
            }
        }

        virtual public int AllocateUnit(string xPosition, string yPosition, string thetaPosition)
        {
            m_nErrorCode = 0;

            try
            {
                //System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();

                //updateRichTextBoxMessage(string.Format("Start move to X = {0},Y = {1}, Theta = {2} position", xPosition, yPosition, thetaPosition));

                ////Move Input Table X                
                //if (IsMoveDone() == false)
                //{
                //    updateRichTextBoxMessage("Previous movement not yet done");
                //    return 1;
                //}

                //m_ProductRTSSProcess.SetProductionLong("InputTableXAxisMovePosition", Int32.Parse(xPosition));
                //m_ProductRTSSProcess.SetProductionLong("InputTableYAxisMovePosition", Int32.Parse(yPosition));
                //m_ProductRTSSProcess.SetProductionLong("InputTableThetaAxisMovePosition", Int32.Parse(thetaPosition));
                //m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_ALLOCATE_UNIT_START", true);

                return m_nErrorCode;
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return m_nErrorCode = -1;
            }
        }

        virtual public void LoadSettingToShareMemory()
        {
            m_ProductRTSSProcess.LoadInputRecipeToSettingShareMemory();
            m_ProductRTSSProcess.LoadVisionRecipeToSettingShareMemory();
        }
                
        private void buttonConfirmEnvelop_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;

            try
            {
                //int nMaxX = 0;
                //int nMaxY = 0;
                //int rangeX = 0;
                //int rangeY = 0;
                //int nColumnMax = 0;
                //int nRowMax = 0;
                //if (m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMaxX_um") > m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMinX_um"))
                //{
                //    nMaxX = m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMinX_um");
                //    m_ProductRTSSProcess.SetSettingLong("EnvelopLimitMinX_um", m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMaxX_um"));
                //    m_ProductRTSSProcess.SetSettingLong("EnvelopLimitMaxX_um", nMaxX);
                //}
                //if (m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMaxY_um") > m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMinY_um"))
                //{
                //    nMaxY = m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMinY_um");
                //    m_ProductRTSSProcess.SetSettingLong("EnvelopLimitMinY_um", m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMaxY_um"));
                //    m_ProductRTSSProcess.SetSettingLong("EnvelopLimitMaxY_um", nMaxY);
                //}
                ////nColumnMax = Math.Abs(m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMaxX_um") - m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMinX_um"))/ (int)m_ProductRecipeInputSetting.DeviceXPitch + 5;
                ////nRowMax = Math.Abs(m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMaxY_um") - m_ProductRTSSProcess.GetSettingLong("EnvelopLimitMinY_um"))/ (int)m_ProductRecipeInputSetting.DeviceYPitch + 5;
                ////ProductInputOutputFileFormat outputFileFormat = new ProductInputOutputFileFormat();
                ////m_ProductShareVariables.mappingInfo.arrayUnitInfo = new UnitInfo[nColumnMax * nRowMax];
                ////m_ProductShareVariables.mappingInfo.Col_Max = nColumnMax;
                ////m_ProductShareVariables.mappingInfo.Row_Max = nRowMax;
                ////outputFileFormat.InitializeMapArrayData(ref m_ProductShareVariables.mappingInfo, 0);
                //updateRichTextBoxMessage("Envelope is formed");
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                //return m_nErrorCode = -1;
            }
        }

        private void buttonAlignTheta_Click(object sender, EventArgs e)
        {
            //m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_ALIGN_THETA_GET_PITCH_START", true);
        }

        private void buttonSetEnvelop1_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;

            try
            {
                //m_ProductRTSSProcess.SetSettingLong("EnvelopLimitMaxX_um", m_ProductRTSSProcess.GetProductionLong("InputVisionXAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableXAxisCenterPosition"));
                //m_ProductRTSSProcess.SetSettingLong("EnvelopLimitMaxY_um", m_ProductRTSSProcess.GetProductionLong("InputTableYAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableYAxisCenterPosition"));
                //updateRichTextBoxMessage("Set envelope 1 done");
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                //return m_nErrorCode = -1;
            }
        }

        private void buttonSetEnvelop2_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;

            try
            {
                //m_ProductRTSSProcess.SetSettingLong("EnvelopLimitMinX_um", m_ProductRTSSProcess.GetProductionLong("InputVisionXAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableXAxisCenterPosition"));
                //m_ProductRTSSProcess.SetSettingLong("EnvelopLimitMinY_um", m_ProductRTSSProcess.GetProductionLong("InputTableYAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableYAxisCenterPosition"));
                //updateRichTextBoxMessage("Set envelope 2 done");
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                //return m_nErrorCode = -1;
            }
        }

        private void buttonGetPitch_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;

            try
            {
                //m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_GET_VISION_XY_START", true);
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                //return m_nErrorCode = -1;
            }
        }

        private void radioButtonAlignmentByEnvelop_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void buttonSetFirstUnit_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;

            try
            {
                //m_ProductRecipeVisionSetting.EnvelopFirstUnitXPosition_um = m_ProductRTSSProcess.GetProductionLong("InputVisionXAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableXAxisCenterPosition");
                //m_ProductRecipeVisionSetting.EnvelopFirstUnitYPosition_um = m_ProductRTSSProcess.GetProductionLong("InputTableYAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableYAxisCenterPosition");
                //m_ProductRecipeVisionSetting.EnvelopFirstUnitThetaPosition_mDegree = m_ProductRTSSProcess.GetProductionLong("InputTableThetaAxisEncoderPosition") - m_ProductRTSSProcess.GetTeachPointLong("InputTableThetaAxisCenterPosition")
                    //- m_ProductRTSSProcess.GetSettingLong("InspectionOrientation_mDegree");
            }
            catch (Exception ex)
            {
                m_strErrorMsg += DateTime.Now.ToString("yyyyMMdd HHmmss ") + ex.ToString();
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                //return m_nErrorCode = -1;
            }
        }

        private void radioButtonAlignmentByFiducial_CheckedChanged(object sender, EventArgs e)
        {
         
        }

        private void radioButtonAlignmentByXYTheta_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonAllocateUnit_Click(object sender, EventArgs e)
        {
            m_nErrorCode = 0;

            try
            {
                //Machine.EventLogger.WriteLog(string.Format("{0} User click Allocate Unit at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //int nPositionX_um, nPositionY_um, nPositionTheta_mDegree, nPositionVisionZ_um, nPositionUnitSupportZ_um;

                //if (Int32.TryParse(labelXReference2_um.Text, out nPositionX_um)
                //    && Int32.TryParse(labelYReference2_um.Text, out nPositionY_um)
                //    && Int32.TryParse(labelThetaReference2_mD.Text, out nPositionTheta_mDegree)
                //    && Int32.TryParse(labelVZReference2_um.Text, out nPositionVisionZ_um)
                //    && Int32.TryParse(labelPZReference2_um.Text, out nPositionUnitSupportZ_um)
                //    )
                //{
                //    if (AllocateUnit(labelXAbsolute2_um.Text, labelYAbsolute2_um.Text, labelThetaAbsolute2_mD.Text) != 0)
                //    {
                //        updateRichTextBoxMessage("Allocate unit fail");
                //        return;
                //    }
                //    Machine.EventLogger.WriteLog(string.Format("{0} Teaching first position at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //}
                //else
                //{
                //    updateRichTextBoxMessage("Position is not an integer");
                //}
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void comboBoxEnvelopType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownCircleEnvelopRadius_um_ValueChanged(object sender, EventArgs e)
        {
            //m_ProductRecipeVisionSetting.EnvelopCircleRadius_um = (int)numericUpDownCircleEnvelopRadius_um.Value;
        }

        private void checkBoxEnableTeachItem_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxTeachItem.SelectedIndex == -1)
                {
                    return;
                }
                if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.InputVision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listInputVisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                        m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S2Vision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listS2VisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                        m_ProductRecipeVisionSetting.listS2VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listS1VisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                        m_ProductRecipeVisionSetting.listS1VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S3Vision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listS3VisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                        m_ProductRecipeVisionSetting.listS3VisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.SetupVision)
                {

                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.S1Vision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listBottomVisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                        m_ProductRecipeVisionSetting.listBottomVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                        //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                    }
                }
                else if (m_ProductRTSSProcess.GetProductionInt("TeachVisionStation") == (int)VisionModule.OutputVision)
                {
                    int nCurrentCount = m_ProductRecipeVisionSetting.listOutputVisionSnap.Count;
                    int nDesireCount = comboBoxTeachItem.SelectedIndex + 1;

                    VisionSnapInfo visionSnapInfo = new VisionSnapInfo();
                    if (nDesireCount <= nCurrentCount)
                    {
                        visionSnapInfo = m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex];
                        visionSnapInfo.EnableSnap = checkBoxEnableTeachItem.Checked;
                        m_ProductRecipeVisionSetting.listOutputVisionSnap[comboBoxTeachItem.SelectedIndex] = visionSnapInfo;
                        //m_ProductRecipeVisionSetting.listInputVisionSnap[comboBoxTeachItem.SelectedIndex].EnableSnap = true;
                    }
                }
                            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
    }

    public class PositionSet
    {
        public int PickAndPlaceXPosition = 0;
        public int PickAndPlaceYPosition = 0;
        public int PickAndPlaceThetaPosition = 0;
        public int PickAndPlaceZPosition = 0;
        public int VisionFocusPosition = 0;
    }
}
