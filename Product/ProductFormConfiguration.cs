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
    public partial class ProductFormConfiguration : Machine.FormConfiguration
    {
        FormConfiguration.tabpageMotionController m_tabPageMotionController = new FormConfiguration.tabpageMotionController();
        FormConfiguration.tabpageDriverFuji m_tabPageDriverFuji = new FormConfiguration.tabpageDriverFuji();
        FormConfiguration.tabpageVisionTurret m_tabPageVisionTurret = new FormConfiguration.tabpageVisionTurret();
        FormConfiguration.tabpageVision m_tabPageVisionInputXYTable = new FormConfiguration.tabpageVision();
        FormConfiguration.tabpageMisc m_tabPageMisc = new FormConfiguration.tabpageMisc();

        private ProductConfigurationSettings m_ProductConfigurationSettings = new ProductConfigurationSettings();

        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductProcessEvent m_ProductProcessEvent;// = new ProductProcessEvent();

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

        public ProductConfigurationSettings productConfigurationSettings
        {
            set
            {
                m_ProductConfigurationSettings = value;
                configurationSettings = m_ProductConfigurationSettings;
            }
        }

        public ProductFormConfiguration()
        {
            //base.Load += new System.EventHandler(InitializeGUI);
        }

        override public void Initialize()
        {
            base.Initialize();

            base.tabPageMotionController.Controls.Add(m_tabPageMotionController); 
            base.tapePageDriver.Controls.Add(m_tabPageDriverFuji);

            tabControlConfiguration.Height = 700;
            //base.tabPageVision.Controls.Add(m_tabPageVisionTurret);
            base.tabPageVision.Controls.Add(m_tabPageVisionInputXYTable);
            m_tabPageVisionTurret.Location = new System.Drawing.Point(0, 42);
            base.tabPageMisc.Controls.Add(m_tabPageMisc);
        }

        override public bool LoadSettings()
        {
            try
            {
                if (File.Exists(base.SettingFileName))
                {
                    m_ProductConfigurationSettings = Tools.Deserialize<ProductConfigurationSettings>(base.SettingFileName);                   
                    base.configurationSettings = m_ProductConfigurationSettings;
                    return true;
                }
                else
                {
                    updateRichTextBoxMessage("Configuration file not exist.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        override public void SaveSetting()
        {
            Tools.Serialize(base.SettingFileName, m_ProductConfigurationSettings);
        }

        override public void UpdateGUI()
        {
            #region Motion Controller
            m_tabPageMotionController.checkBoxEnableMotionController1.Checked = m_ProductConfigurationSettings.EnableMotionController1;
            m_tabPageMotionController.checkBoxEnableMotionController2.Checked = m_ProductConfigurationSettings.EnableMotionController2;
            m_tabPageMotionController.checkBoxEnableMotionController3.Checked = m_ProductConfigurationSettings.EnableMotionController3;
            m_tabPageMotionController.checkBoxEnableMotionController4.Checked = m_ProductConfigurationSettings.EnableMotionController4;
            m_tabPageMotionController.checkBoxEnableMotionController5.Checked = m_ProductConfigurationSettings.EnableMotionController5;
            m_tabPageMotionController.checkBoxEnableMotionController6.Checked = m_ProductConfigurationSettings.EnableMotionController6;
            m_tabPageMotionController.checkBoxEnableMotionController7.Checked = m_ProductConfigurationSettings.EnableMotionController7;
            #endregion Motion Controller

            #region Driver

            #region Input XY Table
            m_tabPageDriverFuji.checkBoxEnablePickAndPlace1XAxisMotor.Checked = m_ProductConfigurationSettings.EnablePickAndPlace1XAxisMotor;
            m_tabPageDriverFuji.checkBoxEnablePickAndPlace1YAxisMotor.Checked = m_ProductConfigurationSettings.EnablePickAndPlace1YAxisMotor;
            m_tabPageDriverFuji.checkBoxEnablePickAndPlace2XAxisMotor.Checked = m_ProductConfigurationSettings.EnablePickAndPlace2XAxisMotor;
            m_tabPageDriverFuji.checkBoxEnablePickAndPlace2YAxisMotor.Checked = m_ProductConfigurationSettings.EnablePickAndPlace2YAxisMotor;

            m_tabPageDriverFuji.checkBoxEnableInputTrayTableXAxisMotor.Checked = m_ProductConfigurationSettings.EnableInputTrayTableXAxisMotor;
            m_tabPageDriverFuji.checkBoxEnableInputTrayTableYAxisMotor.Checked = m_ProductConfigurationSettings.EnableInputTrayTableYAxisMotor;
            m_tabPageDriverFuji.checkBoxEnableInputTrayTableZAxisMotor.Checked = m_ProductConfigurationSettings.EnableInputTrayTableZAxisMotor;

            m_tabPageDriverFuji.checkBoxEnableOutputTrayTableXAxisMotor.Checked = m_ProductConfigurationSettings.EnableOutputTrayTableXAxisMotor;
            m_tabPageDriverFuji.checkBoxEnableOutputTrayTableYAxisMotor.Checked = m_ProductConfigurationSettings.EnableOutputTrayTableYAxisMotor;
            m_tabPageDriverFuji.checkBoxEnableOutputTrayTableZAxisMotor.Checked = m_ProductConfigurationSettings.EnableOutputTrayTableZAxisMotor;

            m_tabPageDriverFuji.checkBoxEnableInputVisionMotor.Checked = m_ProductConfigurationSettings.EnableInputVisionMotor;
            m_tabPageDriverFuji.checkBoxEnableS2VisionMotor.Checked = m_ProductConfigurationSettings.EnableS2VisionMotor;
            m_tabPageDriverFuji.checkBoxEnableS1VisionMotor.Checked = m_ProductConfigurationSettings.EnableS1VisionMotor;
            m_tabPageDriverFuji.checkBoxEnableS3VisionMotor.Checked = m_ProductConfigurationSettings.EnableS3VisionMotor;

            m_tabPageDriverFuji.checkBoxEnablePickAndPlace1Module.Checked = m_ProductConfigurationSettings.EnablePickAndPlace1Module;
            m_tabPageDriverFuji.checkBoxEnablePickAndPlace2Module.Checked = m_ProductConfigurationSettings.EnablePickAndPlace2Module;

            #endregion Input XY Table
            #endregion Driver

            #region Vision
            m_tabPageVisionInputXYTable.checkBoxEnableInputVision.Checked = m_ProductConfigurationSettings.EnableInputVisionModule;
            m_tabPageVisionInputXYTable.checkBoxEnableS2Vision.Checked = m_ProductConfigurationSettings.EnableS2VisionModule;
            m_tabPageVisionInputXYTable.checkBoxEnableS1Vision.Checked = m_ProductConfigurationSettings.EnableS1VisionModule;
            m_tabPageVisionInputXYTable.checkBoxEnableBottomVision.Checked = m_ProductConfigurationSettings.EnableBottomVisionModule;
            m_tabPageVisionInputXYTable.checkBoxEnableS3Vision.Checked = m_ProductConfigurationSettings.EnableS3VisionModule;
            m_tabPageVisionInputXYTable.checkBoxEnableOutputVision.Checked = m_ProductConfigurationSettings.EnableOutputVisionModule;

            
            #endregion Vision

            #region Misc
            m_tabPageMisc.checkBoxEnableSecsgem.Checked = m_ProductConfigurationSettings.EnableSecsgem;
            #region Input XY Table
#if InputXYTable
            m_tabPageMisc.checkBoxEnableBarcodeReader.Checked = m_ProductConfigurationSettings.EnableBarcodeReader;
            m_tabPageMisc.checkBoxEnableCognexBarcodeReader.Checked = m_ProductConfigurationSettings.EnableCognexBarcodeReader;
            m_tabPageMisc.checkBoxEnableZebexScanner.Checked = m_ProductConfigurationSettings.EnableZebexScanner;
            #region BarcodeReaderKeyence
            m_tabPageMisc.checkBoxEnableKeyenceBarcodeReader.Checked = m_ProductConfigurationSettings.EnableKeyenceBarcodeReader;

            if (m_ProductConfigurationSettings.EnableKeyenceBarcodeReader)
            {
                if (m_ProductConfigurationSettings.KeyenceBarcodeReaderCommunicationInterface == 0)
                    m_tabPageMisc.radioButtonKeyenceBarcodeReader_RS232C.Checked = true;
                else if (m_ProductConfigurationSettings.KeyenceBarcodeReaderCommunicationInterface == 1)
                    m_tabPageMisc.radioButtonKeyenceBarcodeReader_Ethernet.Checked = true;
                else updateRichTextBoxMessage("Unknown communication interface");
            }

            #endregion
            m_tabPageMisc.checkBoxEnableAutoInputLoading.Checked = m_ProductConfigurationSettings.EnableAutoInputLoading;
            m_tabPageMisc.checkBoxEnableAutoOutputLoading.Checked = m_ProductConfigurationSettings.EnableAutoOutputLoading;
#endif
            #endregion Input XY Table
            #endregion Misc
        }

        override public bool VerifyConfiguration()
        {
            if (base.VerifyConfiguration() == false)
                return false;

            bool bIsNeedToRestartSequenceSoftware = false;

            #region Verify Configuration
            #region Motion Controller
            if (m_ProductConfigurationSettings.EnableMotionController1 != m_tabPageMotionController.checkBoxEnableMotionController1.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Motion Controller 1? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableMotionController1 = m_tabPageMotionController.checkBoxEnableMotionController1.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableMotionController2 != m_tabPageMotionController.checkBoxEnableMotionController2.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Motion Controller 2? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableMotionController2 = m_tabPageMotionController.checkBoxEnableMotionController2.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableMotionController3 != m_tabPageMotionController.checkBoxEnableMotionController3.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Motion Controller 3? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableMotionController3 = m_tabPageMotionController.checkBoxEnableMotionController3.Checked;
                }
                else
                    return false;
            }

            if (m_ProductConfigurationSettings.EnableMotionController4 != m_tabPageMotionController.checkBoxEnableMotionController4.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Motion Controller 4? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableMotionController4 = m_tabPageMotionController.checkBoxEnableMotionController4.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableMotionController5 != m_tabPageMotionController.checkBoxEnableMotionController5.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Motion Controller 5? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableMotionController5 = m_tabPageMotionController.checkBoxEnableMotionController5.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableMotionController6 != m_tabPageMotionController.checkBoxEnableMotionController6.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Motion Controller 6? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableMotionController6 = m_tabPageMotionController.checkBoxEnableMotionController6.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableMotionController7 != m_tabPageMotionController.checkBoxEnableMotionController7.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Motion Controller 7? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableMotionController7 = m_tabPageMotionController.checkBoxEnableMotionController7.Checked;
                }
                else
                    return false;
            }
            #endregion Motion Controller

            #region Driver
            if (m_ProductConfigurationSettings.EnablePickAndPlace1XAxisMotor != m_tabPageDriverFuji.checkBoxEnablePickAndPlace1XAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Pick And Place 1 X Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnablePickAndPlace1XAxisMotor = m_tabPageDriverFuji.checkBoxEnablePickAndPlace1XAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnablePickAndPlace1YAxisMotor != m_tabPageDriverFuji.checkBoxEnablePickAndPlace1YAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Pick And Place 1 Y Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnablePickAndPlace1YAxisMotor = m_tabPageDriverFuji.checkBoxEnablePickAndPlace1YAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnablePickAndPlace2XAxisMotor != m_tabPageDriverFuji.checkBoxEnablePickAndPlace2XAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Pick And Place 2 X Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnablePickAndPlace2XAxisMotor = m_tabPageDriverFuji.checkBoxEnablePickAndPlace2XAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnablePickAndPlace2YAxisMotor != m_tabPageDriverFuji.checkBoxEnablePickAndPlace2YAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Pick And Place 2 X Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnablePickAndPlace2YAxisMotor = m_tabPageDriverFuji.checkBoxEnablePickAndPlace2YAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableInputTrayTableXAxisMotor != m_tabPageDriverFuji.checkBoxEnableInputTrayTableXAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Input Tray Table X Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableInputTrayTableXAxisMotor = m_tabPageDriverFuji.checkBoxEnableInputTrayTableXAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableInputTrayTableYAxisMotor != m_tabPageDriverFuji.checkBoxEnableInputTrayTableYAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Input Tray Table Y Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableInputTrayTableYAxisMotor = m_tabPageDriverFuji.checkBoxEnableInputTrayTableYAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableInputTrayTableZAxisMotor != m_tabPageDriverFuji.checkBoxEnableInputTrayTableZAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Input Tray Table Z Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableInputTrayTableZAxisMotor = m_tabPageDriverFuji.checkBoxEnableInputTrayTableZAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableOutputTrayTableXAxisMotor != m_tabPageDriverFuji.checkBoxEnableOutputTrayTableXAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Output Tray Table X Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableOutputTrayTableXAxisMotor = m_tabPageDriverFuji.checkBoxEnableOutputTrayTableXAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableOutputTrayTableYAxisMotor != m_tabPageDriverFuji.checkBoxEnableOutputTrayTableYAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Output Tray Table Y Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableOutputTrayTableYAxisMotor = m_tabPageDriverFuji.checkBoxEnableOutputTrayTableYAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableOutputTrayTableZAxisMotor != m_tabPageDriverFuji.checkBoxEnableOutputTrayTableZAxisMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Output Tray Table Z Axis Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableOutputTrayTableZAxisMotor = m_tabPageDriverFuji.checkBoxEnableOutputTrayTableZAxisMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableInputVisionMotor != m_tabPageDriverFuji.checkBoxEnableInputVisionMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Input Vision Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableInputVisionMotor = m_tabPageDriverFuji.checkBoxEnableInputVisionMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableS2VisionMotor != m_tabPageDriverFuji.checkBoxEnableS2VisionMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable S2 Vision Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableS2VisionMotor = m_tabPageDriverFuji.checkBoxEnableS2VisionMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableS1VisionMotor != m_tabPageDriverFuji.checkBoxEnableS1VisionMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Sidewall Left Vision Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableS1VisionMotor = m_tabPageDriverFuji.checkBoxEnableS1VisionMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnableS3VisionMotor != m_tabPageDriverFuji.checkBoxEnableS3VisionMotor.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable S3 Vision Motor? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnableS3VisionMotor = m_tabPageDriverFuji.checkBoxEnableS3VisionMotor.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnablePickAndPlace1Module != m_tabPageDriverFuji.checkBoxEnablePickAndPlace1Module.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Pick And Place 1 Module? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnablePickAndPlace1Module = m_tabPageDriverFuji.checkBoxEnablePickAndPlace1Module.Checked;
                }
                else
                    return false;
            }
            if (m_ProductConfigurationSettings.EnablePickAndPlace2Module != m_tabPageDriverFuji.checkBoxEnablePickAndPlace2Module.Checked)
            {
                if (MessageBox.Show("Are you sure you want to enable/disable Pick And Place 2 Module? Sequence software will be restarted after changes has been made.", "Confirm Apply and Save",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bIsNeedToRestartSequenceSoftware = true;
                    m_ProductConfigurationSettings.EnablePickAndPlace2Module = m_tabPageDriverFuji.checkBoxEnablePickAndPlace2Module.Checked;
                }
                else
                    return false;
            }

            if (m_tabPageMotionController.checkBoxEnableMotionController1.Checked == false)
            {
                if (m_tabPageDriverFuji.checkBoxEnablePickAndPlace1YAxisMotor.Checked == true)
                {
                    base.richTextBoxMessage.Text += "Pick And Place 1 Y Axis Motor will not be enabled because MC1 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnablePickAndPlace1YAxisMotor = false;
                }
                if (m_tabPageDriverFuji.checkBoxEnableInputTrayTableXAxisMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "Input Tray Table X Axis Motor will not be enabled because MC1 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableInputTrayTableXAxisMotor = false;
                }
                if (m_tabPageDriverFuji.checkBoxEnableInputTrayTableYAxisMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "Input Tray Table Y Axis Motor will not be enabled because MC1 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableInputTrayTableYAxisMotor = false;
                }
                if (m_tabPageDriverFuji.checkBoxEnableInputTrayTableZAxisMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "Input Tray Table Z Axis Motor will not be enabled because MC1 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableInputTrayTableZAxisMotor = false;
                }
                
            }
            if (m_tabPageMotionController.checkBoxEnableMotionController2.Checked == false)
            {
                if (m_tabPageDriverFuji.checkBoxEnablePickAndPlace2YAxisMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "Pick And Place 2 Y Axis Motor will not be enabled because MC2 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnablePickAndPlace2YAxisMotor = false;
                }
                if (m_tabPageDriverFuji.checkBoxEnableOutputTrayTableXAxisMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "Output Tray Table X Axis Motor will not be enabled because MC2 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableOutputTrayTableXAxisMotor = false;
                }
                if (m_tabPageDriverFuji.checkBoxEnableOutputTrayTableYAxisMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "Output Tray Table Y Axis Motor will not be enabled because MC2 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableOutputTrayTableYAxisMotor = false;
                }
                if (m_tabPageDriverFuji.checkBoxEnableOutputTrayTableZAxisMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "Output Tray Table Z Axis Motor will not be enabled because MC2 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableOutputTrayTableZAxisMotor = false;
                }
            }
            if (m_tabPageMotionController.checkBoxEnableMotionController3.Checked == false)
            {
                if (m_tabPageDriverFuji.checkBoxEnableInputVisionMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "Input Vision Motor will not be enabled because MC3 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableInputVisionMotor = false;
                }
                if (m_tabPageDriverFuji.checkBoxEnableS2VisionMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "S2 Vision Motor will not be enabled because MC3 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableS2VisionMotor = false;
                }
                if (m_tabPageDriverFuji.checkBoxEnableS1VisionMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "S1 Vision Motor will not be enabled because MC3 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableS1VisionMotor = false;
                }
                if (m_tabPageDriverFuji.checkBoxEnableS3VisionMotor.Checked == true)
                {
                    richTextBoxMessage.Text += "S3 Vision Motor will not be enabled because MC3 is not enabled\n" + Environment.NewLine;
                    m_ProductConfigurationSettings.EnableS3VisionMotor = false;
                }
            }
            if (m_tabPageMotionController.checkBoxEnableMotionController4.Checked == false)
            {
                
            }
            #endregion Driver

            #region Vision
            if (m_ProductConfigurationSettings.EnableInputVisionModule != m_tabPageVisionInputXYTable.checkBoxEnableInputVision.Checked)
            {
                m_ProductConfigurationSettings.EnableInputVisionModule = m_tabPageVisionInputXYTable.checkBoxEnableInputVision.Checked;
            }
            if (m_ProductConfigurationSettings.EnableS2VisionModule != m_tabPageVisionInputXYTable.checkBoxEnableS2Vision.Checked)
            {
                m_ProductConfigurationSettings.EnableS2VisionModule = m_tabPageVisionInputXYTable.checkBoxEnableS2Vision.Checked;
            }
            if (m_ProductConfigurationSettings.EnableS1VisionModule != m_tabPageVisionInputXYTable.checkBoxEnableS1Vision.Checked)
            {
                m_ProductConfigurationSettings.EnableS1VisionModule = m_tabPageVisionInputXYTable.checkBoxEnableS1Vision.Checked;
            }
            if (m_ProductConfigurationSettings.EnableBottomVisionModule != m_tabPageVisionInputXYTable.checkBoxEnableBottomVision.Checked)
            {
                m_ProductConfigurationSettings.EnableBottomVisionModule = m_tabPageVisionInputXYTable.checkBoxEnableBottomVision.Checked;
            }
            if (m_ProductConfigurationSettings.EnableS3VisionModule != m_tabPageVisionInputXYTable.checkBoxEnableS3Vision.Checked)
            {
                m_ProductConfigurationSettings.EnableS3VisionModule = m_tabPageVisionInputXYTable.checkBoxEnableS3Vision.Checked;
            }
            if (m_ProductConfigurationSettings.EnableOutputVisionModule != m_tabPageVisionInputXYTable.checkBoxEnableOutputVision.Checked)
            {
                m_ProductConfigurationSettings.EnableOutputVisionModule = m_tabPageVisionInputXYTable.checkBoxEnableOutputVision.Checked;
            }
            #endregion Vision

            #region Misc
            m_ProductConfigurationSettings.EnableSecsgem = m_tabPageMisc.checkBoxEnableSecsgem.Checked;
            
            if (m_ProductConfigurationSettings.EnableBarcodeReader != m_tabPageMisc.checkBoxEnableBarcodeReader.Checked)
            {
                m_ProductConfigurationSettings.EnableBarcodeReader = m_tabPageMisc.checkBoxEnableBarcodeReader.Checked;
            }
            if (m_ProductConfigurationSettings.EnableCognexBarcodeReader != m_tabPageMisc.checkBoxEnableCognexBarcodeReader.Checked)
            {
                m_ProductConfigurationSettings.EnableCognexBarcodeReader = m_tabPageMisc.checkBoxEnableCognexBarcodeReader.Checked;
            }
            if (m_ProductConfigurationSettings.EnableZebexScanner != m_tabPageMisc.checkBoxEnableZebexScanner.Checked)
            {
                m_ProductConfigurationSettings.EnableZebexScanner = m_tabPageMisc.checkBoxEnableZebexScanner.Checked;
            }
            #region BarcodeReaderKeyence
            if (m_ProductConfigurationSettings.EnableKeyenceBarcodeReader != m_tabPageMisc.checkBoxEnableKeyenceBarcodeReader.Checked)
                m_ProductConfigurationSettings.EnableKeyenceBarcodeReader = m_tabPageMisc.checkBoxEnableKeyenceBarcodeReader.Checked;

            if (m_tabPageMisc.checkBoxEnableKeyenceBarcodeReader.Checked)
            {
                if (m_tabPageMisc.radioButtonKeyenceBarcodeReader_RS232C.Checked)
                {
                    m_ProductConfigurationSettings.KeyenceBarcodeReaderCommunicationInterface = 0;
                }
                else if (m_tabPageMisc.radioButtonKeyenceBarcodeReader_Ethernet.Checked)
                {
                    m_ProductConfigurationSettings.KeyenceBarcodeReaderCommunicationInterface = 1;
                }
                else
                {
                    updateRichTextBoxMessage("Please select communication interface!");
                    return false;
                }
            }
            #endregion
                        
            if (m_ProductConfigurationSettings.EnableAutoInputLoading != m_tabPageMisc.checkBoxEnableAutoInputLoading.Checked)
            {
                m_ProductConfigurationSettings.EnableAutoInputLoading = m_tabPageMisc.checkBoxEnableAutoInputLoading.Checked;
            }
            if (m_ProductConfigurationSettings.EnableAutoOutputLoading != m_tabPageMisc.checkBoxEnableAutoOutputLoading.Checked)
            {
                m_ProductConfigurationSettings.EnableAutoOutputLoading = m_tabPageMisc.checkBoxEnableAutoOutputLoading.Checked;
            }
            #endregion Misc
            #endregion
            return true;
        }
    }
}
