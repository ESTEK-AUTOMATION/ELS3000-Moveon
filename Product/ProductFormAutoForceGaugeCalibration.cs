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
    public partial class ProductFormAutoForceGaugeCalibration : Form
    {
        public ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess();
        private ProductProcessEvent m_ProductProcessEvent;
        bool FirstTime = true;

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
        public ProductFormAutoForceGaugeCalibration()
        {
            InitializeComponent();
            comboBoxPickUpHeadChoosing.SelectedIndex = 0;
            FirstTime = true;
        }

        private void btnInputJedecTrayCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            m_ProductRTSSProcess.SetProductionInt("CalibrationPickUpHeadNo", comboBoxPickUpHeadChoosing.SelectedIndex);
            m_ProductRTSSProcess.SetProductionInt("CalibrationInOutTrayNo", 0);
            m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_START_PICK_UP_HEAD_AFG_CAL", true);
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TAKING_PICK_UP_AFG_CAL_DONE", false);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_AUTO_CALIBRATION", false);
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }

        private void btnOutputJedecTrayCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            m_ProductRTSSProcess.SetProductionInt("CalibrationPickUpHeadNo", comboBoxPickUpHeadChoosing.SelectedIndex);
            m_ProductRTSSProcess.SetProductionInt("CalibrationInOutTrayNo", 1);
            m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 2);
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_START_PICK_UP_HEAD_AFG_CAL", true);
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TAKING_PICK_UP_AFG_CAL_DONE", false);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_AUTO_CALIBRATION", false);
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }

        private void btnInputSoftTrayCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            m_ProductRTSSProcess.SetProductionInt("CalibrationPickUpHeadNo", comboBoxPickUpHeadChoosing.SelectedIndex);
            m_ProductRTSSProcess.SetProductionInt("CalibrationInOutTrayNo", 2);
            m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 3);
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_START_PICK_UP_HEAD_AFG_CAL", true);
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TAKING_PICK_UP_AFG_CAL_DONE", false);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_AUTO_CALIBRATION", false);
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }

        private void btnOutputSoftTrayCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            m_ProductRTSSProcess.SetProductionInt("CalibrationPickUpHeadNo", comboBoxPickUpHeadChoosing.SelectedIndex);
            m_ProductRTSSProcess.SetProductionInt("CalibrationInOutTrayNo", 3);
            m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 4);
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_START_PICK_UP_HEAD_AFG_CAL", true);
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TAKING_PICK_UP_AFG_CAL_DONE", false);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_AUTO_CALIBRATION", false);
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }

        private void btnOutputSpecailCarrierCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            m_ProductRTSSProcess.SetProductionInt("CalibrationPickUpHeadNo", comboBoxPickUpHeadChoosing.SelectedIndex);
            m_ProductRTSSProcess.SetProductionInt("CalibrationInOutTrayNo", 4);
            m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 5);
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_START_PICK_UP_HEAD_AFG_CAL", true);
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TAKING_PICK_UP_AFG_CAL_DONE", false);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_AUTO_CALIBRATION", false);
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }

        private void timerReadRTS_Tick(object sender, EventArgs e)
        {
            if (FirstTime == false)
            {
                if (m_ProductRTSSProcess.GetEvent("GMAIN_RMAIN_STOP_AUTO_CALIBRATION") == true )
                {

                    ButtonEnable(true);
                }
                //else
                //{
                //    btnInputJedecTrayCalibration.Enabled = false;
                //    btnInputSoftTrayCalibration.Enabled = false;
                //    btnOutputJedecTrayCalibration.Enabled = false;
                //    btnOutputSoftTrayCalibration.Enabled = false;
                //    btnOutputSpecailCarrierCalibration.Enabled = false;
                //}
            }
        }

        void ButtonEnable (bool bolSet)
        {

            if (bolSet == true)
            {
                btnInputJedecTrayCalibration.Enabled = true;
                btnInputSoftTrayCalibration.Enabled = true;
                btnOutputJedecTrayCalibration.Enabled = true;
                btnOutputSoftTrayCalibration.Enabled = true;
                btnOutputSpecailCarrierCalibration.Enabled = true;
                btnCalibration.Enabled = true;
            }
            else
            {
                btnInputJedecTrayCalibration.Enabled = false;
                btnInputSoftTrayCalibration.Enabled = false;
                btnOutputJedecTrayCalibration.Enabled = false;
                btnOutputSoftTrayCalibration.Enabled = false;
                btnOutputSpecailCarrierCalibration.Enabled = false;
                btnCalibration.Enabled = false;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_AUTO_CALIBRATION", true);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_AUTO_CALIBRATION", true);
            timerReadRTS.Enabled = false;
            timerCheckOpen.Enabled = false;
            m_ProductShareVariables.bFormLightingCalibrationEnable = true;
            Dispose();
            Close();
        }

        private void ProductFormAutoForceGaugeCalibration_Load(object sender, EventArgs e)
        {
            timerCheckOpen.Enabled = true;
        }

        private void timerCheckOpen_Tick(object sender, EventArgs e)
        {
            m_ProductShareVariables.bFormLightingCalibrationEnable = false;
        }

        private void btnCalibration_Click(object sender, EventArgs e)
        {
            FirstTime = false;
            //m_ProductRTSSProcess.SetProductionInt("CalibrationPickUpHeadNo", comboBoxPickUpHeadChoosing.SelectedIndex);
            //m_ProductRTSSProcess.SetProductionInt("CalibrationInOutTrayNo", 0);
            m_ProductRTSSProcess.SetGeneralInt("MaintenanceID", 1);
            m_ProductRTSSProcess.SetEvent("StartSetup", true);
            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_START_PICK_UP_HEAD_AFG_CAL", true);
            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_TAKING_PICK_UP_AFG_CAL_DONE", false);
            m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_STOP_AUTO_CALIBRATION", false);
            ButtonEnable(false);
            timerReadRTS.Enabled = true;
        }
    }
}
