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
    public partial class ProductFormLowYieldParts : Form
    {
        public LowYieldSetting m_LowYieldSetting = new LowYieldSetting();
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess(); 
        private ProductProcessEvent m_ProductProcessEvent;
        public List<LowYieldItemInformation> LowYieldItemList = new List<LowYieldItemInformation>();
        public List<ContinuousLowYieldItemInfo> ContinuousLowYieldItemList = new List<ContinuousLowYieldItemInfo>();

        public string m_strmode = "Low Yield Form";
        BackgroundWorker m_bgwUserInterface;

        int nCurrentLYResetIndex = 0;
        int nCurrentLYResetValue = 0;
        int nCurrentLYResetPassValue = 0;
        int nCurrentLYResetTotalValue = 0;
        int nCurrentContinuousLYResetIndex = 0;
        int nCurrentContinuousLYResetFailValue = 0;
        int nCurrentContinuousLYResetTotalValue = 0;
        bool bPressLYReset = false;
        bool bPressLYUndo = false;
        bool bPressContinuousLYReset = false;
        bool bPressContinuousLYUndo = false;
        bool bChanged = false;
        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
            }
        }
        public ProductProcessEvent productProcessEvent
        {
            set
            {
                m_ProductProcessEvent = value;
            }
        }

        public ProductShareVariables productShareVariables
        {
            set { m_ProductShareVariables = value; }
        }

        public class LowYieldItemInformation
        {
            public string ItemName { get; set; }
            public int TotalCount { get; set; }
            public int PassCount { get; set; }

        }
        public class ContinuousLowYieldItemInfo
        {
            public string ItemName { get; set; }
            public int ContinuousFailCount { get; set; }
            public int ContinuousTotalCount { get; set; }
        }
        public ProductFormLowYieldParts()
        {
            InitializeComponent();
            FormEvent();
        }
        void FormEvent()
        {
            m_LowYieldSetting.comboBoxContinuousLowYieldCountReset.SelectedIndexChanged += ComboBoxContinuousLowYieldCountReset_SelectedIndexChanged;
            m_LowYieldSetting.comboBoxLowYieldCountReset.SelectedIndexChanged += ComboBoxLowYieldCountReset_SelectedIndexChanged;
            m_LowYieldSetting.buttonContinuousLYCountReset.Click += ButtonContinuousLYCountReset_Click;
            m_LowYieldSetting.buttonContinuousLYCountUndo.Click += ButtonContinuousLYCountUndo_Click;
            m_LowYieldSetting.buttonLYCountReset.Click += ButtonLYCountReset_Click;
            m_LowYieldSetting.buttonLYCountUndo.Click += ButtonLYCountUndo_Click;
        }

        virtual public void Initialize(string Message)
        {
            try
            {
                UpdateMessage(Message);
                AddItemList();
                UpdateSettingParameter();
                UpdateLowYieldCountItemInterface();
                
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
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} .", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
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
                foreach (Product.FormLowYieldPart.LowYieldCountingItem ControlItem in flowLayoutPanel2.Controls)
                {
                    bChanged = false;
                    var itemName = ControlItem.ItemName.Substring(ControlItem.ItemName.Length - 2);
                    var itemArrayNo = Convert.ToInt32(itemName) - 1;
                    if (ControlItem.ItemName.Contains("Bond"))
                    {
                        if (ControlItem.numericUpDownLowYieldPassCount.Value != m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadPassCount", itemArrayNo, ""))
                        {
                            ControlItem.numericUpDownLowYieldPassCount.Value = m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadPassCount", itemArrayNo, "");
                            ControlItem.nLowYieldPassCountValue = m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadPassCount", itemArrayNo, "");
                            bChanged = true;
                        }
                        if (ControlItem.numericUpDownLowYieldTotalCount.Value != m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadTotalCount", itemArrayNo, ""))
                        {
                            ControlItem.numericUpDownLowYieldTotalCount.Value = m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadTotalCount", itemArrayNo, "");
                            ControlItem.nLowYieldTotalCountValue = m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadTotalCount", itemArrayNo, "");
                            bChanged = true;
                        }
                    }
                    else if (ControlItem.ItemName.Contains("Flipper"))
                    {
                        if (ControlItem.numericUpDownLowYieldPassCount.Value != m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadPassCount", itemArrayNo, ""))
                        {
                            ControlItem.numericUpDownLowYieldPassCount.Value = m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadPassCount", itemArrayNo, "");
                            ControlItem.nLowYieldPassCountValue = m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadPassCount", itemArrayNo, "");
                            bChanged = true;
                        }
                        if (ControlItem.numericUpDownLowYieldTotalCount.Value != m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadTotalCount", itemArrayNo, ""))
                        {
                            ControlItem.numericUpDownLowYieldTotalCount.Value = m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadTotalCount", itemArrayNo, "");
                            ControlItem.nLowYieldTotalCountValue = m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadTotalCount", itemArrayNo, "");
                            bChanged = true;
                        }
                    }
                    if (bChanged == true)
                    {
                        if (ControlItem.numericUpDownLowYieldTotalCount.Value > 0)
                        {
                            ControlItem.nValueDiffer = Convert.ToDouble(ControlItem.numericUpDownLowYieldPassCount.Value) / Convert.ToDouble(ControlItem.numericUpDownLowYieldTotalCount.Value);
                            ControlItem.nLowYieldPercent = (int)(ControlItem.nValueDiffer * 100);

                        }
                        else
                        {
                            ControlItem.nValueDiffer = 0;
                            ControlItem.nLowYieldPercent = 100;
                        }
                        ControlItem.progressBarLowYieldPercentage.Value = ControlItem.nLowYieldPercent;
                    }
                }
                foreach (Product.FormLowYieldPart.ContinuousLowYield ContinuousControl in flowLayoutPanelContinuousLowYield.Controls)
                {
                    var ContinuousItemName = ContinuousControl.ItemName.Substring(ContinuousControl.ItemName.Length - 2);
                    var ContinuousArrayNo = Convert.ToInt32(ContinuousItemName) - 1;

                    if (ContinuousControl.ItemName.Contains("Bond"))
                    {
                        if (ContinuousControl.numericUpDownContinuousLowYieldFailCount.Value != m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldBondHeadFailCount", ContinuousArrayNo, ""))
                        {
                            ContinuousControl.numericUpDownContinuousLowYieldFailCount.Value = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldBondHeadFailCount", ContinuousArrayNo, "");
                            ContinuousControl.nContinuousLowYieldFailCountValue = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldBondHeadFailCount", ContinuousArrayNo, "");
                        }
                        if (ContinuousControl.numericUpDownContinuousLowYieldTotalCount.Value != m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldBondHeadTotalCount", ContinuousArrayNo, ""))
                        {
                            ContinuousControl.numericUpDownContinuousLowYieldTotalCount.Value = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldBondHeadTotalCount", ContinuousArrayNo, "");
                            ContinuousControl.nContinuousLowYieldTotalCountValue = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldBondHeadTotalCount", ContinuousArrayNo, "");
                        }
                    }
                    else if (ContinuousControl.ItemName.Contains("Flipper"))
                    {
                        if (ContinuousControl.numericUpDownContinuousLowYieldFailCount.Value != m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldFlipperHeadFailCount", ContinuousArrayNo, ""))
                        {
                            ContinuousControl.numericUpDownContinuousLowYieldFailCount.Value = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldFlipperHeadFailCount", ContinuousArrayNo, "");
                            ContinuousControl.nContinuousLowYieldFailCountValue = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldFlipperHeadFailCount", ContinuousArrayNo, "");
                        }
                        if (ContinuousControl.numericUpDownContinuousLowYieldTotalCount.Value != m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldFlipperHeadTotalCount", ContinuousArrayNo, ""))
                        {
                            ContinuousControl.numericUpDownContinuousLowYieldTotalCount.Value = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldFlipperHeadTotalCount", ContinuousArrayNo, "");
                            ContinuousControl.nContinuousLowYieldTotalCountValue = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldFlipperHeadTotalCount", ContinuousArrayNo, "");
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
        //private void ComboBoxLowYieldItemName_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (m_LowYieldSetting.comboBoxLowYieldItemName.SelectedIndex == 0)
        //    {
        //        m_LowYieldSetting.numericUpDownLowYieldMinCleanCount.Value = nBondHeadLowYieldCleanCount;
        //        m_LowYieldSetting.numericUpDownLowYieldMaxLowYieldPercent.Value = nBondHeadLowYieldWarningCount;
        //        m_LowYieldSetting.numericUpDownLowYieldMaxContinuousFailCount.Value = nBondHeadLowYieldAlarmCount;
        //    }
        //    else if (m_LowYieldSetting.comboBoxLowYieldItemName.SelectedIndex == 1)
        //    {
        //        m_LowYieldSetting.numericUpDownLowYieldMinCleanCount.Value = nFlipperHeadLowYieldCleanCount;
        //        m_LowYieldSetting.numericUpDownLowYieldMaxLowYieldPercent.Value = nFlipperHeadLowYieldWarningCount;
        //        m_LowYieldSetting.numericUpDownLowYieldMaxContinuousFailCount.Value = nFlipperHeadLowYieldAlarmCount;
        //    }
        //}
        void UpdateMessage(string Message)
        {
            labelLowYieldMessage.Text = Message;
        }

        void UpdateSettingParameter()
        {
            foreach (var item in LowYieldItemList)
            {
                m_LowYieldSetting.comboBoxLowYieldCountReset.Items.Add(item.ItemName);
            }
            foreach (var item2 in ContinuousLowYieldItemList)
            {
                m_LowYieldSetting.comboBoxContinuousLowYieldCountReset.Items.Add(item2.ItemName);
            }
            m_LowYieldSetting.comboBoxContinuousLowYieldCountReset.SelectedIndex = 0;
            m_LowYieldSetting.comboBoxLowYieldCountReset.SelectedIndex = 0;
        }
        
        public void AddItemList()
        {
            //nSortMode = m_MaintenanceSetting.comboBoxArrageBy.SelectedIndex;
            LowYieldItemList.Clear();
            for (int i = 0; i < 24; i++)
            {
                LowYieldItemList.Add(new LowYieldItemInformation() { ItemName = $"Bond Head {i+1}", TotalCount = m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadTotalCount", i, ""),  PassCount = m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadPassCount", i, "") });
            }

            for (int j = 0; j < 4; j++)
            {
                LowYieldItemList.Add(new LowYieldItemInformation() { ItemName = $"Flipper Head {j+1}", TotalCount = m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadTotalCount", j, ""), PassCount = m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadPassCount", j, "") });
            }
            ContinuousLowYieldItemList.Clear();
            for (int i=0; i<24; i++)
            {
                ContinuousLowYieldItemList.Add(new ContinuousLowYieldItemInfo() { ItemName = $"Bond Head {i + 1}", ContinuousTotalCount = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldBondHeadTotalCount", i, ""), ContinuousFailCount = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldBondHeadFailCount", i, "") });
            }
            for (int j = 0; j < 4; j++)
            {
                ContinuousLowYieldItemList.Add(new ContinuousLowYieldItemInfo() { ItemName = $"Flipper Head {j + 1}", ContinuousTotalCount = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldFlipperHeadTotalCount", j, ""), ContinuousFailCount = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldFlipperHeadFailCount", j, "") });
            }
        }
        public void UpdateLowYieldCountItemInterface()
        {
            flowLayoutPanel2.Controls.Clear();
            flowLayoutPanelContinuousLowYield.Controls.Clear();
            panel1.Controls.Clear();
            foreach (var item in LowYieldItemList)
            {
                var ItemControl = AddUsercontrol(item.TotalCount, item.PassCount, item.ItemName);
                flowLayoutPanel2.Controls.Add(ItemControl);
               // panelSetting.Controls.Add(m_LowYieldSetting);
            }
            foreach(var ContinuousItem in ContinuousLowYieldItemList)
            {
                var ContinuousItemControl = AddContinuousUserControl(ContinuousItem.ContinuousTotalCount, ContinuousItem.ContinuousFailCount, ContinuousItem.ItemName);
                flowLayoutPanelContinuousLowYield.Controls.Add(ContinuousItemControl);
            }
            panel1.Controls.Add(m_LowYieldSetting);
        }
        Product.FormLowYieldPart.LowYieldCountingItem AddUsercontrol(int TotalCountvalue, int PassCountvalue, string Name)
        {
            var LowYieldItemName = new Product.FormLowYieldPart.LowYieldCountingItem();
            LowYieldItemName.ItemName = Name;
            LowYieldItemName.nLowYieldPassCountValue = PassCountvalue;
            LowYieldItemName.nLowYieldTotalCountValue = TotalCountvalue;
            LowYieldItemName.Dock = DockStyle.Top;
            LowYieldItemName.updateInterface();
            return LowYieldItemName;
        }
        Product.FormLowYieldPart.ContinuousLowYield AddContinuousUserControl(int ContinuousTotalCount, int ContinuousFailCountValue, string Name)
        {
            var LowYieldItemName = new Product.FormLowYieldPart.ContinuousLowYield();
            LowYieldItemName.ItemName = Name;
            LowYieldItemName.nContinuousLowYieldFailCountValue = ContinuousFailCountValue;
            LowYieldItemName.nContinuousLowYieldTotalCountValue = ContinuousTotalCount;
            LowYieldItemName.Dock = DockStyle.Top;
            LowYieldItemName.updateInterface();
            return LowYieldItemName;
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            //Close();
            if (m_bgwUserInterface.WorkerSupportsCancellation == true)
            {
                m_bgwUserInterface.CancelAsync();
            }
        }
        private void ComboBoxLowYieldCountReset_SelectedIndexChanged(object sender, EventArgs e)
        {
            bPressLYReset = false;
            bPressLYUndo = false;
            try
            {
                if (m_LowYieldSetting.comboBoxLowYieldCountReset.SelectedItem.ToString() != "")
                {
                    foreach (var item in LowYieldItemList)
                    {
                        if (item.ItemName == m_LowYieldSetting.comboBoxLowYieldCountReset.SelectedItem.ToString())
                        {
                            if (item.TotalCount > 0)
                            {
                                m_LowYieldSetting.numericUpDownLowYieldResetCurrentPercent.Value = (int)(Convert.ToDouble(Convert.ToDouble( item.PassCount) / Convert.ToDouble(item.TotalCount)) * 100);
                            }
                            else
                            {
                                m_LowYieldSetting.numericUpDownLowYieldResetCurrentPercent.Value = 100;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("c");
                }
            }
            catch
            {
                MessageBox.Show("a");
            }
        }

        private void ComboBoxContinuousLowYieldCountReset_SelectedIndexChanged(object sender, EventArgs e)
        {
            bPressLYReset = false;
            bPressLYUndo = false;
            try
            {
                if (m_LowYieldSetting.comboBoxContinuousLowYieldCountReset.SelectedItem.ToString() != "")
                {
                    foreach (var item in ContinuousLowYieldItemList)
                    {
                        if (item.ItemName == m_LowYieldSetting.comboBoxContinuousLowYieldCountReset.SelectedItem.ToString())
                        {
                            m_LowYieldSetting.numericUpDownContinuousLowYieldResetCurrentCount.Value = item.ContinuousFailCount;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("c");
                }
            }
            catch
            {
                MessageBox.Show("a");
            }
        }
        private void ButtonLYCountReset_Click(object sender, EventArgs e)
        {
            bPressLYUndo = false;
            if (bPressLYReset == false)
            {
                nCurrentLYResetIndex = m_LowYieldSetting.comboBoxLowYieldCountReset.SelectedIndex;
                nCurrentLYResetValue = (int)m_LowYieldSetting.numericUpDownLowYieldResetCurrentPercent.Value;
                m_LowYieldSetting.numericUpDownLowYieldResetCurrentPercent.Value = 100;
                string ItemFullName = m_LowYieldSetting.comboBoxLowYieldCountReset.SelectedItem.ToString();
                string lastWord = ItemFullName.Substring(ItemFullName.LastIndexOf(" ") + 1);
                if (int.TryParse(lastWord, out int No))
                {
                    if (ItemFullName.Contains("Bond"))
                    {
                        nCurrentLYResetTotalValue = m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadTotalCount", No - 1, "");
                        nCurrentLYResetPassValue = m_ProductRTSSProcess.GetProductionArray("LowYieldBondHeadPassCount", No - 1, ""); ;
                        m_ProductRTSSProcess.SetProductionArray("LowYieldBondHeadTotalCount", No - 1, "", 0);
                        m_ProductRTSSProcess.SetProductionArray("LowYieldBondHeadPassCount", No - 1, "", 0);
                    }
                    else
                    {
                        nCurrentLYResetTotalValue = m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadTotalCount", No - 1, "");
                        nCurrentLYResetPassValue = m_ProductRTSSProcess.GetProductionArray("LowYieldFlipperHeadPassCount", No - 1, ""); ;
                        m_ProductRTSSProcess.SetProductionArray("LowYieldFlipperHeadTotalCount", No - 1, "", 0);
                        m_ProductRTSSProcess.SetProductionArray("LowYieldFlipperHeadPassCount", No - 1, "", 0);
                    }
                }
            }
            bPressLYReset = true;
        }
        private void ButtonLYCountUndo_Click(object sender, EventArgs e)
        {
            bPressLYReset = false;
            if (bPressLYUndo == false)
            {
                m_LowYieldSetting.comboBoxLowYieldCountReset.SelectedIndex = nCurrentLYResetIndex;
                m_LowYieldSetting.numericUpDownLowYieldResetCurrentPercent.Value = nCurrentLYResetValue;
                string ItemFullName = m_LowYieldSetting.comboBoxLowYieldCountReset.SelectedItem.ToString();
                string lastWord = ItemFullName.Substring(ItemFullName.LastIndexOf(" ") + 1);
                if (int.TryParse(lastWord, out int No))
                {
                    if (ItemFullName.Contains("Bond"))
                    {
                        m_ProductRTSSProcess.SetProductionArray("LowYieldBondHeadTotalCount", No - 1, "", nCurrentLYResetTotalValue);
                        m_ProductRTSSProcess.SetProductionArray("LowYieldBondHeadPassCount", No - 1, "", nCurrentLYResetPassValue);
                    }
                    else
                    {
                        m_ProductRTSSProcess.SetProductionArray("LowYieldFlipperHeadTotalCount", No - 1, "", nCurrentLYResetTotalValue);
                        m_ProductRTSSProcess.SetProductionArray("LowYieldFlipperHeadPassCount", No - 1, "", nCurrentLYResetPassValue);
                    }
                }
            }
            bPressLYUndo = true;
        }
        private void ButtonContinuousLYCountReset_Click(object sender, EventArgs e)
        {
            bPressContinuousLYUndo = false;
            if (bPressContinuousLYReset == false)
            {
                nCurrentContinuousLYResetIndex = m_LowYieldSetting.comboBoxContinuousLowYieldCountReset.SelectedIndex;
                nCurrentContinuousLYResetFailValue = (int)m_LowYieldSetting.numericUpDownContinuousLowYieldResetCurrentCount.Value;
                m_LowYieldSetting.numericUpDownContinuousLowYieldResetCurrentCount.Value = 0;
                string ItemFullName = m_LowYieldSetting.comboBoxContinuousLowYieldCountReset.SelectedItem.ToString();
                string lastWord = ItemFullName.Substring(ItemFullName.LastIndexOf(" ") + 1);
                if (int.TryParse(lastWord, out int No))
                {
                    if (ItemFullName.Contains("Bond"))
                    {
                        nCurrentContinuousLYResetTotalValue = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldBondHeadTotalCount", No - 1, "");
                        m_ProductRTSSProcess.SetProductionArray("ContinuousLowYieldBondHeadTotalCount", No - 1, "", 0);
                        m_ProductRTSSProcess.SetProductionArray("ContinuousLowYieldBondHeadFailCount", No - 1, "", 0);
                    }
                    else
                    {
                        nCurrentContinuousLYResetTotalValue = m_ProductRTSSProcess.GetProductionArray("ContinuousLowYieldFlipperHeadTotalCount", No - 1, "");
                        m_ProductRTSSProcess.SetProductionArray("ContinuousLowYieldFlipperHeadTotalCount", No - 1, "", 0);
                        m_ProductRTSSProcess.SetProductionArray("ContinuousLowYieldFlipperHeadFailCount", No - 1, "", 0);
                    }
                }
            }
            bPressContinuousLYReset = true;
        }
        private void ButtonContinuousLYCountUndo_Click(object sender, EventArgs e)
        {
            bPressContinuousLYReset = false;
            if (bPressContinuousLYUndo == false)
            {
                m_LowYieldSetting.comboBoxContinuousLowYieldCountReset.SelectedIndex = nCurrentContinuousLYResetIndex;
                m_LowYieldSetting.numericUpDownContinuousLowYieldResetCurrentCount.Value = nCurrentContinuousLYResetFailValue;
                string ItemFullName = m_LowYieldSetting.comboBoxContinuousLowYieldCountReset.SelectedItem.ToString();
                string lastWord = ItemFullName.Substring(ItemFullName.LastIndexOf(" ") + 1);
                if (int.TryParse(lastWord, out int No))
                {
                    if (ItemFullName.Contains("Bond"))
                    {
                        m_ProductRTSSProcess.SetProductionArray("ContinuousLowYieldBondHeadTotalCount", No - 1, "", nCurrentContinuousLYResetTotalValue);
                        m_ProductRTSSProcess.SetProductionArray("ContinuousLowYieldBondHeadFailCount", No - 1, "", nCurrentContinuousLYResetFailValue);
                    }
                    else
                    {
                        m_ProductRTSSProcess.SetProductionArray("ContinuousLowYieldFlipperHeadTotalCount", No - 1, "", nCurrentContinuousLYResetTotalValue);
                        m_ProductRTSSProcess.SetProductionArray("ContinuousLowYieldFlipperHeadFailCount", No - 1, "", nCurrentContinuousLYResetFailValue);
                    }
                }
            }
            bPressContinuousLYUndo = true;
        }
    }
}
