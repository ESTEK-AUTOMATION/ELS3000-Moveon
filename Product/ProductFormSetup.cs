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
    public class ProductFormSetup : Machine.FormSetup
    {
        public FormSetup.tabPageSetup m_tabPageSetup = new FormSetup.tabPageSetup();

        public string m_MsgPreviousCommunicationResponse = "";
        public string m_MsgPreviousBarcodeReaderResponse = "";

        private ProductShareVariables m_ProductShareVariables;
        private ProductProcessEvent m_ProductProcessEvent;
        private ProductRTSSProcess m_ProductRTSSProcess;

        delegate void SetTextCallback(string text);
        delegate void SetTextCallback2(string text);

        Task DownloadRecipeInMainRecipeTask;
        Task UploadRecipeInMainRecipeTask;
        Task DownloadSingleRecipeTask;
        Task UploadSingleRecipeTask;
        Task DownloadAllRecipeTask;
        Task UploadAllRecipeTask;
        Task SetHeartBeatTask;

        ProductSetupSettings m_ProductSetupSettings = new ProductSetupSettings();
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
            }
        }

        public ProductSetupSettings productSetupSettings
        {
            set
            {
                m_ProductSetupSettings = value;
            }
        }

        public ProductFormSetup()
        {

        }

        override public bool LoadSettings()
        {
            try
            {
                if (File.Exists(m_strSetupPath + m_strFile))
                {
                    m_ProductSetupSettings = Tools.Deserialize<ProductSetupSettings>(m_strSetupPath + m_strFile);
                    setupSettings = m_ProductSetupSettings;
                    return true;
                }
                else
                {
                    updateRichTextBoxMessage("Setup file not exist.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        override public bool SaveSetupSettings()
        {
            try
            {
                Tools.Serialize(m_strSetupPath + m_strFile, m_ProductSetupSettings);
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        override public void Initialize()
        {
            base.Initialize();
            base.Controls.Add(m_tabPageSetup);
            //m_tabPageSetup.tabControlSetup.Controls.Remove(m_tabPageSetup.tabPageBarcodeReader);
            m_tabPageSetup.buttonConnectCommunication.Click += new System.EventHandler(buttonConnectCommunication_Click);
            m_tabPageSetup.buttonCommunicationSend.Click += new System.EventHandler(buttonCommunicationSend_Click);
            m_tabPageSetup.buttonCommunicationDisconnect.Click += new System.EventHandler(buttonCommunicationDisconnect_Click);
            m_tabPageSetup.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            m_tabPageSetup.buttonTrigger.Click += new System.EventHandler(this.buttonTrigger_Click);
            m_tabPageSetup.buttonConnect2.Click += new System.EventHandler(this.buttonConnect2_Click);
            m_tabPageSetup.buttonTrigger2.Click += new System.EventHandler(this.buttonTrigger2_Click);
            m_tabPageSetup.buttonCommunicationSend.Click += new System.EventHandler(buttonCommunicationSend_Click);
            m_tabPageSetup.buttonSendMessageToHost.Click += new System.EventHandler(buttonSendMessageToHost_Click);
            m_tabPageSetup.buttonEventBarcode.Click += new System.EventHandler(buttonEventBarcode_Click);
            m_tabPageSetup.buttonEventUnload.Click += new System.EventHandler(buttonEventUnload_Click);
            m_tabPageSetup.buttonEventEndLoad.Click += new System.EventHandler(buttonEventEndLoad_Click);
            m_tabPageSetup.buttonEventLoad.Click += new System.EventHandler(buttonEventLoad_Click);
            m_tabPageSetup.buttonEventStartLoad.Click += new System.EventHandler(EventStartLoad_Click);
            m_tabPageSetup.buttonSetAlarm.Click += new System.EventHandler(buttonSetAlarm_Click);
            m_tabPageSetup.buttonClearAlarm.Click += new System.EventHandler(buttonClearAlarm_Click);
            m_tabPageSetup.buttonDownloadRecipe.Click += new System.EventHandler(buttonDownloadRecipe_Click);
            m_tabPageSetup.buttonUploadRecipe.Click += new System.EventHandler(buttonUploadRecipe_Click);
            m_tabPageSetup.buttonDownloadAllRecipe.Click += new System.EventHandler(buttonDownloadAllRecipe_Click);
            m_tabPageSetup.buttonUploadAllRecipe.Click += new System.EventHandler(buttonUploadAllRecipe_Click);
            m_tabPageSetup.buttonDownloadMainRecipe.Click += new System.EventHandler(buttonDownloadMainRecipe_Click);
            m_tabPageSetup.buttonUploadMainRecipe.Click += new System.EventHandler(buttonUploadMainRecipe_Click);
            m_tabPageSetup.buttonS1F1.Click += new System.EventHandler(buttonS1F1_Click);
            m_tabPageSetup.buttonS1F13.Click += new System.EventHandler(buttonS1F13_Click);
            m_tabPageSetup.radioButtonDisableHeartBeat.MouseClick += new System.Windows.Forms.MouseEventHandler(radioButtonDisableHeartBeat_MouseClick);
            m_tabPageSetup.radioButtonEnableHeartBeat.MouseClick += new System.Windows.Forms.MouseEventHandler(radioButtonEnableHeartBeat_MouseClick);

            m_tabPageSetup.buttonBrowseOutputData.Click += new System.EventHandler(buttonBrowseOutputDataFilePath_Click);
            m_tabPageSetup.buttonBrowseEndJobData.Click += new System.EventHandler(buttonBrowseEndJobDataFilePath_Click);

            m_tabPageSetup.buttonSendOutputDataMES.Click += new System.EventHandler(buttonSendOutputDataMES_Click);
            m_tabPageSetup.buttonSendEndJobDataMES.Click += new System.EventHandler(buttonSendEndJobDataMES_Click);

            m_tabPageSetup.richTextBoxMESStatus.TextChanged += new System.EventHandler(richTextBoxMESMessage_TextChanged);
//#if TurretApplication == false
//            m_tabPageSetup.tabControlSetup.TabPages.Remove(m_tabPageSetup.tabPageTurretApplication);
//#endif
        }

        override public void UpdateUserInterface()
        {
            if (ProductBarcodeReaderSequenceThread.m_strBarcodeScannerResponse != m_MsgPreviousBarcodeReaderResponse)
            {
                base.updateRichTextBoxMessage(ProductBarcodeReaderSequenceThread.m_strBarcodeScannerResponse);
                m_MsgPreviousBarcodeReaderResponse = ProductBarcodeReaderSequenceThread.m_strBarcodeScannerResponse;
            }
            if (ProductCommunicationSequenceThread.m_strResponse != m_MsgPreviousCommunicationResponse && ProductCommunicationSequenceThread.m_strResponse != "")
            {
                m_tabPageSetup.labelCommunicationReceive.Text = ProductCommunicationSequenceThread.m_strResponse;
                m_MsgPreviousCommunicationResponse = ProductCommunicationSequenceThread.m_strResponse;
            }
        }

        override public void UpdateGUI()
        {
            if (m_SetupSettings.EnableSecsgemHeartbeat == true)
            {
                m_tabPageSetup.radioButtonEnableHeartBeat.Checked = true;
                m_tabPageSetup.radioButtonDisableHeartBeat.Checked = false;
            }
            else
            {
                m_tabPageSetup.radioButtonEnableHeartBeat.Checked = false;
                m_tabPageSetup.radioButtonDisableHeartBeat.Checked = true;
            }
            #region Turret Application
            //checkBoxEnablePUH24.Checked = m_Option.EnablePUH24;

            //numericUpDownSortingTubePitch_um.Value = (decimal) m_Option.SortingTubePitch_um;
            #endregion
            RefreshRecipeFolderList();
        }

        #region Form Event
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
#if CognexInstalled == false
                m_ProductProcessEvent.GUI_PCS_ConnectBarcodeReader.Set();
#endif
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonTrigger_Click(object sender, EventArgs e)
        {
            try
            {
                m_ProductProcessEvent.GUI_PCS_TriggerBarcodeReader.Set();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonConnect2_Click(object sender, EventArgs e)
        {
            try
            {
#if CognexInstalled == false
                m_ProductProcessEvent.GUI_PCS_ConnectBarcodeReader2.Set();
#endif
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonTrigger2_Click(object sender, EventArgs e)
        {
            try
            {
#if CognexInstalled == false
                m_ProductProcessEvent.GUI_PCS_TriggerBarcodeReader2.Set();
#endif
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonConnectCommunication_Click(object sender, EventArgs e)
        {
            m_ProductProcessEvent.GUI_PCS_ConnectCommunication.Set();
        }

        private void buttonCommunicationDisconnect_Click(object sender, EventArgs e)
        {
            m_ProductProcessEvent.GUI_PCS_DisconnectCommunication.Set();
        }

        private void buttonCommunicationSend_Click(object sender, EventArgs e)
        {
            ProductCommunicationSequenceThread.m_strSend = m_tabPageSetup.textBoxCommuncationSend.Text;
            m_ProductProcessEvent.GUI_PCS_CommunicationSend.Set();
        }

        private void buttonSendMessageToHost_Click(object sender, EventArgs e)
        {
            Task SendMessageTask = Task.Run(() => SendMessage());
        }

        private void buttonEventBarcode_Click(object sender, EventArgs e)
        {
            Task SetEventBarcodeTask = Task.Run(() => SetEventBarcode());
        }

        private void buttonEventUnload_Click(object sender, EventArgs e)
        {
            Task TriggerEventUnloadTask = Task.Run(() => TriggerEventUnload());
        }

        private void buttonEventEndLoad_Click(object sender, EventArgs e)
        {
            Task TriggerEventEndLoadTask = Task.Run(() => TriggerEventEndLoad());
        }

        private void buttonEventLoad_Click(object sender, EventArgs e)
        {
            Task TriggerEventLoadTask = Task.Run(() => TriggerEventLoad());
            Task TriggerEventUnloadTask = Task.Run(() => TriggerEventUnload());
        }

        private void EventStartLoad_Click(object sender, EventArgs e)
        {
            Task TriggerEventStartLoadTask = Task.Run(() => TriggerEventStartLoad(m_ProductShareVariables.strucInputProductInfo));
        }

        private void buttonSetAlarm_Click(object sender, EventArgs e)
        {
            Task SetAlarmTask = Task.Run(() => SetAlarm(m_ProductRTSSProcess.GetGeneralInt("AlarmID").ToString()));
        }

        private void buttonClearAlarm_Click(object sender, EventArgs e)
        {
            Task ClearAlarmTask = Task.Run(() => ClearAlarm());
        }

        private void buttonDownloadRecipe_Click(object sender, EventArgs e)
        {
            string StrRecipeName;
            textBoxStatusText("Downloading Recipe...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (m_tabPageSetup.comboBoxRecipeList.SelectedIndex != -1)
                {
                    StrRecipeName = m_tabPageSetup.comboBoxRecipeList.SelectedItem.ToString();
                    if (DownloadSingleRecipeTask == null || DownloadSingleRecipeTask.IsCompleted)
                    {
                        DownloadSingleRecipeTask = Task.Run(() => DownloadSingleRecipe(StrRecipeName));
                    }
                    else
                    {
                        textBoxStatusText("Uploading Previous Recipe to Secgem host, Please try again later");
                    }
                }
            }
        }

        private void buttonUploadRecipe_Click(object sender, EventArgs e)
        {
            string StrRecipeName;
            textBoxStatusText("Uploading Recipe...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (m_tabPageSetup.comboBoxRecipeList.SelectedIndex != -1)
                {
                    StrRecipeName = m_tabPageSetup.comboBoxRecipeList.SelectedItem.ToString();
                    //UploadSingleRecipeTask = Task.Run(() => UploadSingleRecipe(StrRecipeName));
                    //UploadSingleRecipeTask = Task.Run(();
                    if (UploadSingleRecipeTask == null || UploadSingleRecipeTask.IsCompleted)
                    {
                        UploadSingleRecipeTask = Task.Run(() => UploadSingleRecipe(StrRecipeName));
                    }
                    else
                    {
                        textBoxStatusText("Uploading Previous Recipe to Secgem host, Please try again later");
                    }
                }
            }
        }

        private void buttonDownloadAllRecipe_Click(object sender, EventArgs e)
        {
            textBoxStatusText("Downloading All Recipe...");
            if (DownloadAllRecipeTask == null || DownloadAllRecipeTask.IsCompleted)
            {
                DownloadAllRecipeTask = Task.Run(() => DownloadAllRecipe());
            }
            else
            {
                textBoxStatusText("Uploading Previous Recipe to Secgem host, Please try again later");
            }
        }

        private void buttonUploadAllRecipe_Click(object sender, EventArgs e)
        {
            textBoxStatusText("Uploading All Recipe...");
            if (UploadAllRecipeTask == null || UploadAllRecipeTask.IsCompleted)
            {
                UploadAllRecipeTask = Task.Run(() => UploadAllRecipe());
            }
            else
            {
                textBoxStatusText("Uploading Previous Recipe to Secgem host, Please try again later");
            }
        }

        private void buttonDownloadMainRecipe_Click(object sender, EventArgs e)
        {
            string StrRecipeName;

            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                try
                {
                    textBoxStatusText("Downloading Sub-Recipe in Main Recipe...");
                    richTextBoxUploadDownloadRecipeText("");
                    if (m_tabPageSetup.comboBoxMainRecipe.SelectedIndex != -1)
                    {
                        StrRecipeName = m_tabPageSetup.comboBoxMainRecipe.SelectedItem.ToString();
                        if (DownloadRecipeInMainRecipeTask == null || DownloadRecipeInMainRecipeTask.IsCompleted)
                        {
                            DownloadRecipeInMainRecipeTask = Task.Run(() => DownloadRecipeInMainRecipe(StrRecipeName));
                        }
                        else
                        {
                            textBoxStatusText("Download Previous Recipe to Secgem host, Please try again later");
                        }
                    }
                    else
                    {
                        textBoxStatusText("Please Select Main Recipe" + "\r\n");
                    }
                }
                catch (Exception ex)
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                }
            }
        }

        private void buttonUploadMainRecipe_Click(object sender, EventArgs e)
        {
            string StrRecipeName;

            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                try
                {
                    textBoxStatusText("Uploading Sub-Recipe in Main Recipe...");
                    richTextBoxUploadDownloadRecipeText("");

                    if (m_tabPageSetup.comboBoxMainRecipe.SelectedIndex != -1)
                    {
                        StrRecipeName = m_tabPageSetup.comboBoxMainRecipe.SelectedItem.ToString();
                        if (UploadRecipeInMainRecipeTask == null || UploadRecipeInMainRecipeTask.IsCompleted)
                        {
                            UploadRecipeInMainRecipeTask = Task.Run(() => UploadRecipeInMainRecipe(StrRecipeName));
                        }
                        else
                        {
                            textBoxStatusText("Uploading Previous Recipe to Secgem host, Please try again later");
                        }
                    }
                    else
                    {
                        textBoxStatusText("Please Select Main Recipe" + "\r\n");
                    }
                }
                catch (Exception ex)
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                }
            }
        }

        private void buttonS1F1_Click(object sender, EventArgs e)
        {
            Task S1F1_AreYouThereTask = Task.Run(() => S1F1_AreYouThere());
        }

        private void buttonS1F13_Click(object sender, EventArgs e)
        {
            Task CommunicationRequestTask = Task.Run(() => CommunicationRequest());
        }

        private void radioButtonDisableHeartBeat_MouseClick(object sender, MouseEventArgs e)
        {
            m_SetupSettings.EnableSecsgemHeartbeat = false;
            if (SetHeartBeatTask == null || SetHeartBeatTask.IsCompleted == true)
            {
                SetHeartBeatTask = Task.Run(() => SetHeartBeat(0));
            }
            else
            {
                textBoxStatusText("Enable Heartbeat, Please try again later");
            }
            //radioButtonEnableHeartBeat.Checked = false;
        }

        private void radioButtonEnableHeartBeat_MouseClick(object sender, MouseEventArgs e)
        {
            m_SetupSettings.EnableSecsgemHeartbeat = true;

            if (SetHeartBeatTask == null || SetHeartBeatTask.IsCompleted == true)
            {
                SetHeartBeatTask = Task.Run(() => SetHeartBeat(5)); ;
            }
            else
            {
                textBoxStatusText("Disable Heartbeat, Please try again later");
            }
            //radioButtonDisableHeartBeat.Checked = false;
        }

        #endregion Form Event

        public void SendMessage()
        {
            textBoxStatusText("Sending...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.SendMessageToHost(m_tabPageSetup.textBoxMessageToHost.Text) != 0)
                {
                    textBoxStatusText("Send Message Fail");
                }
                else
                {
                    textBoxStatusText("Send Message Done");
                }
            }
            else
            {
                textBoxStatusText("Secsgem Is Not Enabled");
                updateRichTextBoxMessage("Secsgem Is Not Enabled");
            }
        }

        public void textBoxStatusText(string text)
        {
            if (m_tabPageSetup.textBoxStatus.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(textBoxStatusText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                m_tabPageSetup.textBoxStatus.Text = text;
            }
        }

        public void richTextBoxUploadDownloadRecipeText(string text)
        {

            if (m_tabPageSetup.richTextBoxUploadDownloadRecipe.InvokeRequired)
            {
                SetTextCallback2 d2 = new SetTextCallback2(richTextBoxUploadDownloadRecipeText);
                m_tabPageSetup.richTextBoxUploadDownloadRecipe.Invoke(d2, new object[] { text });
            }
            else
            {
                if (text == "")
                {
                    m_tabPageSetup.richTextBoxUploadDownloadRecipe.Text = text;
                }
                else
                {
                    m_tabPageSetup.richTextBoxUploadDownloadRecipe.Text += text;
                }
            }
        }

        void SetEventBarcode()
        {
            textBoxStatusText("Triggering Event Barcode...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (!(m_ProductShareVariables.strCurrentBarcodeID == "" || m_ProductShareVariables.strCurrentBarcodeID == null))
                    Machine.Platform.SecsgemControl.SetEventBarcodeID(m_ProductShareVariables.strCurrentBarcodeID);
                else
                {
                    if (Machine.Platform.SecsgemControl.SetEventBarcodeID("Dummy") != 0)
                    {
                        textBoxStatusText("Trigger Barcode Event Fail");
                    }
                    else
                    {
                        textBoxStatusText("Trigger Barcode Event Alarm Done");
                    }
                }
            }
            else
            {
                textBoxStatusText("Secsgem Is Not Enabled");
                updateRichTextBoxMessage("Secsgem Is Not Enabled");
            }
        }

        void TriggerEventUnload()
        {
            textBoxStatusText("Triggering Event Unload...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.SetEventUnloadFrameOrTile() != 0)
                {
                    textBoxStatusText("Trigger Event Unload Fail");
                }
                else
                {
                    textBoxStatusText("Trigger Event Unload Done");
                }
            }
            else
            {
                textBoxStatusText("Secsgem Is Not Enabled");
                updateRichTextBoxMessage("Secsgem Is Not Enabled");
            }
        }
        
        void TriggerEventEndLoad()
        {
            //textBoxStatus.Text = "";
            textBoxStatusText("Triggering Event End...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.SetEventEndLot() != 0)
                {
                    textBoxStatusText("Trigger Event End Fail");
                }
                else
                {
                    textBoxStatusText("Trigger Event End Done");
                }
            }
            else
            {
                textBoxStatusText("Secsgem Is Not Enabled");
                updateRichTextBoxMessage("Secsgem Is Not Enabled");
            }
        }
        
        void TriggerEventLoad()
        {
            textBoxStatusText("Triggering Event End...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.SetEventLoadFrameOrTile() != 0)
                {
                    //SetShareMemoryGeneralInt("AlarmID", 7005);
                    textBoxStatusText("Trigger Event Load Fail");
                }
                else
                {
                    textBoxStatusText("Trigger Event Load Done");
                }
            }
            else
            {
                textBoxStatusText("Secsgem Is Not Enabled");
                updateRichTextBoxMessage("Secsgem Is Not Enabled");
            }
        }
        
        void TriggerEventStartLoad(Input_Product_Info InputProductInfo)
        {
            textBoxStatusText("Triggering Event Start...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (!(InputProductInfo.LotID == ""
                    || InputProductInfo.WorkOrder == ""
                    || InputProductInfo.Recipe == ""
                    || InputProductInfo.WaferBin == ""
                    || InputProductInfo.PPLot == ""
                    || InputProductInfo.OperatorID == ""
                    || InputProductInfo.Shift == ""
                    || InputProductInfo.LotID == null
                    || InputProductInfo.WorkOrder == null
                    || InputProductInfo.Recipe == null
                    || InputProductInfo.WaferBin == null
                    || InputProductInfo.PPLot == null
                    || InputProductInfo.OperatorID == null
                    || InputProductInfo.Shift == null
                    ))
                {

                    if (Machine.Platform.SecsgemControl.SetEventLotInfo(InputProductInfo.LotID, InputProductInfo.WorkOrder,
                                        InputProductInfo.Recipe, InputProductInfo.WaferBin, InputProductInfo.PPLot,
                                        InputProductInfo.OperatorID, InputProductInfo.Shift) != 0)
                    {
                        textBoxStatusText("Trigger Event Start Fail");
                    }
                    else
                    {
                        textBoxStatusText("Trigger Event Start Done");
                    }
                }
                else
                {
                    if (Machine.Platform.SecsgemControl.SetEventLotInfo("lotIDdummy1", "WOdummy2",
                                        "recipedummy3", "waferBindummy4", "pplotdummy5",
                                        "opdummy6", "shiftdummy7") != 0)
                    {
                        textBoxStatusText("Trigger Event Start Fail");
                    }
                    else
                    {
                        textBoxStatusText("Trigger Event Start Done");
                    }
                }
            }
            else
            {
                textBoxStatusText("Secsgem Is Not Enabled");
                updateRichTextBoxMessage("Secsgem Is Not Enabled");
            }

        }

        void SetAlarm(string AlarmId)
        {
            textBoxStatusText("Set Alarm...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (AlarmId != "0")
                {
                    if (Machine.Platform.SecsgemControl.SetAlarm("2002") != 0)
                    {
                        textBoxStatusText("Set Alarm Fail");
                    }
                    else
                    {
                        textBoxStatusText("Set Alarm Done");
                    }
                }
                else
                {
                    if (Machine.Platform.SecsgemControl.SetAlarm("2002") != 0)
                    {
                        textBoxStatusText("Set Alarm Fail");
                    }
                    else
                    {
                        textBoxStatusText("Set Alarm Done");
                    }
                }
            }
            else
            {
                textBoxStatusText("Secsgem Is Not Enabled");
                updateRichTextBoxMessage("Secsgem Is Not Enabled");
            }
        }                

        void ClearAlarm()
        {
            textBoxStatusText("Clear Alarm...");
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (m_ProductRTSSProcess.GetGeneralInt("AlarmID").ToString() != "0")
                {
                    if (Machine.Platform.SecsgemControl.ClearAlarm("2002") != 0)
                    {
                        textBoxStatusText("Clear Alarm Fail");
                    }
                    else
                    {
                        textBoxStatusText("Clear Alarm Done");
                    }
                }
                else
                {
                    if (Machine.Platform.SecsgemControl.ClearAlarm("2002") != 0)
                    {
                        textBoxStatusText("Clear Alarm Fail");
                    }
                    else
                    {
                        textBoxStatusText("Clear Alarm Done");
                    }
                }
            }
            else
            {
                textBoxStatusText("Secsgem Is Not Enabled");
                updateRichTextBoxMessage("Secsgem Is Not Enabled");
            }
        }
        
        public void RefreshRecipeFolderList()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                string path = @"D:\Estek\Recipe";
                string MainRecipePath = @"D:\Estek\Recipe\Main";
                List<string> FolderList = new List<string>();

                foreach (string s in Directory.GetDirectories(path))
                {
                    FolderList.Add(s);
                }

                foreach (var RecipeFolderPath in FolderList)
                {
                    string[] recipelist = Directory.GetFiles(RecipeFolderPath, "*.xml");

                    if (recipelist.Length != 0)
                    {
                        foreach (var fullRecipeXmlPath in recipelist)
                        {
                            m_tabPageSetup.comboBoxRecipeList.Items.Add(fullRecipeXmlPath);
                        }
                    }
                }
                foreach (string s in Directory.GetFiles(MainRecipePath))
                {
                    m_tabPageSetup.comboBoxMainRecipe.Items.Add(s.ToString());
                }
            }
        }

        void DownloadSingleRecipe(string RecipeName)
        {
            if (Machine.Platform.SecsgemControl.DownloadRecipe(RecipeName) != 0)
            {
                textBoxStatusText("Download Fail");
            }
            else
            {
                textBoxStatusText("Download Done");
            }
        }
        
        void UploadSingleRecipe(string RecipeName)
        {
            if (Machine.Platform.SecsgemControl.UploadRecipe(RecipeName) != 0)
            {
                textBoxStatusText("Upload fail");
            }
            else
            {
                textBoxStatusText("Upload Done");
            }
        }

        void DownloadAllRecipe()
        {
            string FolderPath = @"D:\Estek\Recipe";
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.DownloadAllRecipe(FolderPath) != 0)
                {
                    textBoxStatusText("Download All Recipe Fail");
                }
                else
                {
                    textBoxStatusText("Download All Recipe Done");
                }
            }
        }

        void UploadAllRecipe()
        {
            string FolderPath = @"D:\Estek\Recipe";
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.UploadAllRecipe(FolderPath) != 0)
                {
                    textBoxStatusText("Upload All Recipe Fail");
                }
                else
                {
                    textBoxStatusText("Upload All Recipe Done");
                }
            }
        }
        
        void DownloadRecipeInMainRecipe(string RecipePath)
        {
            List<string> m_ListOfRecipePath = new List<string>();
            ProductRecipeMainSettings m_RecipeMain = new ProductRecipeMainSettings();
            string pathName = "";

            textBoxStatusText("Downloading Main Recipe");
            if (Machine.Platform.SecsgemControl.DownloadRecipe(RecipePath) != 0)//DownloadRecipe(string RecipeFullPath)
            {
                textBoxStatusText("Download Main Recipe Fail");
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
                richTextBoxUploadDownloadRecipeText("");
                foreach (var item in m_ListOfRecipePath)
                {
                    richTextBoxUploadDownloadRecipeText(item + "\r\n");
                }

                textBoxStatusText("Downloading Recipe...");

                if (Machine.Platform.SecsgemControl.DownloadMainRecipe(m_ListOfRecipePath) != 0)
                {
                    textBoxStatusText("Download Recipe Fail");
                }
                else
                {
                    textBoxStatusText("Download Recipe Done");
                }
            }
        }

        void UploadRecipeInMainRecipe(string RecipeName)
        {

            List<string> m_ListOfRecipePath = new List<string>();
            ProductRecipeMainSettings m_RecipeMain = new ProductRecipeMainSettings();
            string pathName = "";

            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                try
                {
                    m_RecipeMain = Tools.Deserialize<ProductRecipeMainSettings>(RecipeName);

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
                    foreach (var item in m_ListOfRecipePath)
                    {
                        richTextBoxUploadDownloadRecipeText(item + "\r\n");
                    }
                    textBoxStatusText("Uploading Recipe...");
                    if (Machine.Platform.SecsgemControl.UploadMainRecipe(m_ListOfRecipePath) != 0)
                    {
                        textBoxStatusText("Upload All fail");
                    }
                    else
                    {
                        textBoxStatusText("Upload All Done");
                    }
                }
                catch (Exception ex)
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                }
            }
        }
        
        void S1F1_AreYouThere()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.S1F1_AreYouThere() != 0)
                {
                    textBoxStatusText("Are You There Request Fail");
                }
                else
                {
                    textBoxStatusText("Are You There Request Done");
                }
            }
            else
                textBoxStatusText("Secsgem Is Not Enabled");
        }
        
        void CommunicationRequest()
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.CommunicationRequest() != 0)
                {
                    textBoxStatusText("Communication Request Fail");
                }
                else
                {
                    textBoxStatusText("Communication Request Done");
                }
            }
            else
                textBoxStatusText("Secsgem Is Not Enabled");
        }

        void SetHeartBeat(int TimeInSecond)
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.SetHeartBeat(TimeInSecond) != 0)
                {
                    if (TimeInSecond != 0)
                    {
                        textBoxStatusText("Set Heartbeat Fail");
                    }
                    else
                    {
                        textBoxStatusText("Disable Heatbeat Fail");
                    }
                }
                else
                {
                    if (TimeInSecond != 0)
                    {
                        textBoxStatusText("Set Heartbeat Done");
                    }
                    else
                    {
                        textBoxStatusText("Disable Heatbeat Done");
                    }
                }
            }
            else
                textBoxStatusText("Secsgem Is Not Enabled");
        }

        private void buttonBrowseOutputDataFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Json files (*.json)|*.json";
            if(openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                m_tabPageSetup.textBoxOutputDataFilePath.Text = openFileDialog1.FileName;
            }
        }
        private void buttonBrowseEndJobDataFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Json files (*.json)|*.json";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                m_tabPageSetup.textBoxEndJobDataPath.Text = openFileDialog1.FileName;
            }
        }
        private void buttonSendOutputDataMES_Click(object sender, EventArgs e)
        {
            try
            {
                MoveonMESAPI.OutputData outputData = new MoveonMESAPI.OutputData();
                if (m_tabPageSetup.textBoxOutputDataFilePath.Text!="")
                {
                    outputData = Newtonsoft.Json.JsonConvert.DeserializeObject<MoveonMESAPI.OutputData>(File.ReadAllText(m_tabPageSetup.textBoxOutputDataFilePath.Text));
                }
                else
                {
                    m_tabPageSetup.richTextBoxMESStatus.Text += string.Format("{0}.","Please browse output data file path");
                    return;
                }
                int nError = 0;
                nError = MoveonMESAPI.MESAPI.Post(outputData);
                if(nError !=0)
                {
                    m_tabPageSetup.richTextBoxMESStatus.Text += string.Format("{0}, {1}.", "Fail to send output data to MES",MoveonMESAPI.MESAPI.StatusMessage);
                }
                else
                {
                    m_tabPageSetup.richTextBoxMESStatus.Text += string.Format("{0}, {1}.", "Send output data to MES Done", MoveonMESAPI.MESAPI.StatusMessage);
                }

            }
            catch (Exception ex)
            {
                m_tabPageSetup.richTextBoxMESStatus.Text += string.Format("{0}.", ex.ToString());
            }
        }
        private void buttonSendEndJobDataMES_Click(object sender, EventArgs e)
        {
            try
            {
                MoveonMESAPI.EndJobData endjobData = new MoveonMESAPI.EndJobData();
                if (m_tabPageSetup.textBoxEndJobDataPath.Text != "")
                {
                    endjobData = Newtonsoft.Json.JsonConvert.DeserializeObject<MoveonMESAPI.EndJobData>(File.ReadAllText(m_tabPageSetup.textBoxEndJobDataPath.Text));
                }
                else
                {
                    m_tabPageSetup.richTextBoxMESStatus.Text += string.Format("{0}.", "Please browse endjob data file path");
                    return;
                }
                int nError = 0;
                nError = MoveonMESAPI.MESAPI.Post(endjobData);
                if (nError != 0)
                {
                    m_tabPageSetup.richTextBoxMESStatus.Text += string.Format("{0}, {1}.", "Fail to send endjob data to MES", MoveonMESAPI.MESAPI.StatusMessage);
                }
                else
                {
                    m_tabPageSetup.richTextBoxMESStatus.Text += string.Format("{0}, {1}.", "Send endjob data to MES Done", MoveonMESAPI.MESAPI.StatusMessage);
                }

            }
            catch (Exception ex)
            {
                m_tabPageSetup.richTextBoxMESStatus.Text += string.Format("{0}.", ex.ToString());
            }
        }

        private void richTextBoxMESMessage_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Machine.GeneralTools.RichTextBoxScrollToCaret(m_tabPageSetup.richTextBoxMESStatus);
            }
            catch (Exception ex)
            {
                m_tabPageSetup.richTextBoxMESStatus.Text += string.Format("{0}  {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString());
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
    }
}
