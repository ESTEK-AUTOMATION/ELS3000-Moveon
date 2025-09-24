using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Product
{
    public partial class ProductFormCalibration : Form
    {
        public ProductShareVariables m_ProductShareVariables;
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess();
        private ProductProcessEvent m_ProductProcessEvent;
        bool FirstTime = false;
        public ProductRTSSProcess productRTSSProcess
        {
            set { m_ProductRTSSProcess = value; }
        }
        public ProductShareVariables productShareVariables
        {
            set { m_ProductShareVariables = value; }
        }
        public ProductProcessEvent productProcessEvent
        {
            set { m_ProductProcessEvent = value; }
        }
        public ProductFormCalibration()
        {
            InitializeComponent();
            //comboBoxGrayScaleCalibrationRecipeName.DataSource = m_ProductShareVariables.RecipeListForCalibration;
            //comboBoxGrayScaleCalibrationRecipeName.SelectedItem = m_ProductShareVariables.currentMainRecipeName;
            //comboBoxCalibrationType.SelectedIndex = 0;
            cmbPnPSelect.SelectedIndex = 0;
            FirstTime = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("JobStop", true);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_CALIBRATION", true);
            m_ProductShareVariables.bFormCalibrationEnable = true;
            timerReadRTS.Enabled = false;
            timerCheckOpen.Enabled = false;
            Dispose();
            Close();

        }

        private void timerReadRTS_Tick(object sender, EventArgs e)
        {
            if (FirstTime == false)
            {
                if (m_ProductRTSSProcess.GetEvent("GMAIN_RMAIN_STOP_CALIBRATION") == true)
                {

                    ButtonEnable(true);
                }
            }
        }
        void ButtonEnable(bool bolSet)
        {

            if (bolSet == true)
            {
                comboBoxCalibrationType.Enabled = true;
                comboBoxGrayScaleCalibrationRecipeName.Enabled = true;
                btnInputStationCalibration.Enabled = true;
                btnS1BTMStationCalibration.Enabled = true;
                btnS2S3StationCalibration.Enabled = true;
                btnOutputStationCalibration.Enabled = true;
            }
            else
            {
                comboBoxCalibrationType.Enabled = false;
                comboBoxGrayScaleCalibrationRecipeName.Enabled = false;
                btnInputStationCalibration.Enabled = false;
                btnS1BTMStationCalibration.Enabled = false;
                btnS2S3StationCalibration.Enabled = false;
                btnOutputStationCalibration.Enabled = false;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("JobStop", true);
        }

        private void timerCheckOpen_Tick(object sender, EventArgs e)
        {
            m_ProductShareVariables.bFormCalibrationEnable = false;
        }

        private void btnInputStationCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetProductionInt("StationSelectedForCalibration", 1);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_CALIBRATION", false);
            if(comboBoxCalibrationType.SelectedIndex == 0)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
            }
            else if (comboBoxCalibrationType.SelectedIndex == 1)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
            }
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }

        private void btnS1BTMStationCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetProductionInt("StationSelectedForCalibration", 2);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_CALIBRATION", false);
            if (comboBoxCalibrationType.SelectedIndex == 0)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
            }
            else if (comboBoxCalibrationType.SelectedIndex == 1)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
            }
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }

        private void btnS2S3StationCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetProductionInt("StationSelectedForCalibration", 3);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_CALIBRATION", false);
            if (comboBoxCalibrationType.SelectedIndex == 0)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
            }
            else if (comboBoxCalibrationType.SelectedIndex == 1)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
            }
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }

        private void btnOutputStationCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetProductionInt("StationSelectedForCalibration", 4);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_CALIBRATION", false);
            if (comboBoxCalibrationType.SelectedIndex == 0)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
            }
            else if (comboBoxCalibrationType.SelectedIndex == 1)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
            }
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }

        private void ProductFormDotGridCalibration_Load(object sender, EventArgs e)
        {
            comboBoxGrayScaleCalibrationRecipeName.DataSource = m_ProductShareVariables.RecipeListForCalibration;
            comboBoxGrayScaleCalibrationRecipeName.SelectedItem = m_ProductShareVariables.currentMainRecipeName;
            comboBoxCalibrationType.SelectedIndex = 0;
            timerCheckOpen.Enabled = true;
        }
        private void comboBoxGrayScaleCalibrationRecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGrayScaleCalibrationRecipeName.SelectedItem == null)
                return;
            m_ProductShareVariables.currentMainRecipeName = comboBoxGrayScaleCalibrationRecipeName.SelectedItem.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetProductionInt("StationSelectedForCalibration", 1);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_CALIBRATION", false);
            if (cmbPnPSelect.SelectedIndex == 0)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
                m_ProductRTSSProcess.SetProductionLong("PnP1AngleMaintainance", (int)nudAngle.Value);

            }
            else
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
                m_ProductRTSSProcess.SetProductionLong("PnP2AngleMaintainance", (int)nudAngle.Value);

            }
            m_ProductRTSSProcess.SetProductionLong("BottomXAxisoffsetMaintainance", (int)nudBottomXOffset.Value);
            m_ProductRTSSProcess.SetProductionLong("BottomYAxisoffsetMaintainance", (int)nudBottomYOffset.Value);
        }

        private void cmbPnPSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FirstTime == true)
            {
                m_ProductRTSSProcess.SetProductionLong("PnPCurrentAngle", (int)0);
            }
            
        }

        private void btnMoveToBottom_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetProductionInt("StationSelectedForCalibration", 2);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_CALIBRATION", false);
            if (cmbPnPSelect.SelectedIndex == 0)
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
                //m_ProductRTSSProcess.SetProductionLong("PnP1AngleMaintainance", (int)nudAngle.Value);

            }
            else
            {
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
                //m_ProductRTSSProcess.SetProductionLong("PnP2AngleMaintainance", (int)nudAngle.Value);

            }
        }

        private void btnVacuum1_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetProductionInt("StationSelectedForCalibration", 3);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_CALIBRATION", false);
            if (btnVacuum1.Text == "Vacuum On 1")
            {
                btnVacuum1.Text = "Vacuum Off 1";
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
                //m_ProductRTSSProcess.SetProductionLong("PnP1AngleMaintainance", (int)nudAngle.Value);

            }
            else
            {
                btnVacuum1.Text = "Vacuum On 1";
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
                //m_ProductRTSSProcess.SetProductionLong("PnP2AngleMaintainance", (int)nudAngle.Value);

            }
        }

        private void btnVacuum2_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetProductionInt("StationSelectedForCalibration", 4);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_CALIBRATION", false);
            if (btnVacuum2.Text == "Vacuum On 2")
            {
                btnVacuum2.Text = "Vacuum Off 2";
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
                //m_ProductRTSSProcess.SetProductionLong("PnP1AngleMaintainance", (int)nudAngle.Value);

            }
            else
            {
                btnVacuum2.Text = "Vacuum On 2";
                m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
                //m_ProductRTSSProcess.SetProductionLong("PnP2AngleMaintainance", (int)nudAngle.Value);

            }
        }
    }
}
