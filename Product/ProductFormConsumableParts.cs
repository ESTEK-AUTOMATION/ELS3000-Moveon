using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Product 
{
    public partial class ProductFormConsumableParts : Form
    {
        public MaintenanceSetting m_MaintenanceSetting = new MaintenanceSetting();
        public TurretFlipperMaintenance m_TurretFlipperMaintenance = new TurretFlipperMaintenance();
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductRTSSProcess m_ProductRTSSProcess;// =  new ProductRTSSProcess();
        private ProductProcessEvent m_ProductProcessEvent;
        public string m_strmode = "Maintenance Count Form";
        BackgroundWorker m_bgwUserInterface;
        bool bChanged = false;
        int nSortMode = 1; //0-> Name , 1-> by Current Count 
        int nTriggerMode = 1; //0-> Due , 1-> >Warning , 2-> Clean
        int nCloseMode = 1; //0-> Alway can Close , Current < Due count

        int PickUpHeadCleanCount = 0;
        int PickUpHeadWarningCount = 0;
        int PickUpHeadDueCount = 0;

        int nPickUpHeadSettingCleanCount = 0;

        int nCurrentItemIdex = 0;
        int nPreviousValue = 0;
        int nPreviousCleanCount = 0;
        int nPreviousMinCleanCount = 0;
        int[] nPreviousCleanCountBHArray = new int[2];
        int[] nPreviousMinCleanCountBHArray = new int[2];
        int[] nPreviousValueBHArray = new int[2];

        string UserName = "";
        string ResetItemName = "";
        bool bPressResetButton = false;
        bool bPressUndoButton = false;
        bool bPressUndoAndResetButton = false;
        bool bSaveReset = false;
        int nPreviousUndoValue = 0;
        string UndoItemName = "";

        int nPreviousPickUpHeadSettingCleanValue = 0;
        int nChangedPickUpHeadCleanValue = 0;
        int nPreviousPickUpHeadCleanValue = 0;

        bool bButtonDoneCleanPress = false;
        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
            }
        }

        public ProductShareVariables productShareVariables
        {
            set { m_ProductShareVariables = value; }
        }
        public ProductProcessEvent productProcessEvent
        {
            set { m_ProductProcessEvent = value; }
        }

        public class ItemInfo
        {
            public string ItemName { get; set; }
            public string ItemLocation { get; set; }
            public int CleanCount { get; set; }
            public int CurrentCount { get; set; }
            public int WarningCount { get; set; }
            public int DueCount { get; set; }
            public int SettingCleanCount { get; set; }
            public int MinimumCleanCount { get; set; }
        }

        public static List<ItemInfo> ItemList = new List<ItemInfo>();

        public ProductFormConsumableParts()
        {
            InitializeComponent();
            GenerateFormEvents();
            //this.FormBorderStyle = FormBorderStyle.None;
        }

        void GenerateFormEvents()
        {
            m_MaintenanceSetting.comboBoxComsumableItem.SelectedIndexChanged += new System.EventHandler(this.comboBoxComsumableItem_SelectedIndexChanged);
            m_MaintenanceSetting.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            m_MaintenanceSetting.buttonUndo.Click += new System.EventHandler(this.buttonUndo_Click);
            m_MaintenanceSetting.buttonDoneCleaning.Click += ButtonDoneCleaning_Click;
        }
        virtual public void Initialize(string Message)
        {
            try
            {
                m_MaintenanceSetting.buttonDoneCleaning.Enabled = true;
                m_MaintenanceSetting.buttonReset.Enabled = true;
                m_MaintenanceSetting.buttonUndo.Enabled = true;
                UpdateMessage(Message);
                UpdateSettingParameter();
                AddItemList();
                InitialSettingInterface();
                //SortItemList();
                UpdateMaintananceCountItemInterface();

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
                    UpdateCurrentItemList();
                    CheckCanFormClose();
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
                foreach (Product.FormConsumablePart.MaintenanceCountItem ControlItem in flowLayoutPanel1.Controls)
                {
                    bChanged = false;

                    var itemName = ControlItem.ItemName.Substring(ControlItem.ItemName.Length - 2);
                    if (int.TryParse(itemName, out int itemArrayNo))
                    {
                        itemArrayNo = itemArrayNo - 1;
                    }
                    if (ControlItem.ItemName.Contains("Pick Up Head"))
                    {
                        if (ControlItem.numericUpDownCurrentCount.Value != m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", itemArrayNo, ""))
                        {
                            ControlItem.numericUpDownCurrentCount.Value = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", itemArrayNo, "");
                            bChanged = true;
                        }
                        //if (ControlItem.numericUpDownCleanCount.Value != int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString()))
                        //{
                        //    ControlItem.numericUpDownCleanCount.Value = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString());
                        //}
                        if (ControlItem.numericUpDownCleanCount.Value != int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadSettingCleanCount", 1000).ToString()))
                        {
                            ControlItem.numericUpDownCleanCount.Value = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadSettingCleanCount", 1000).ToString());
                            bChanged = true;
                        }
                        if (ControlItem.numericUpDownWarningCount.Value != int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadWarningCount", 2000).ToString()))
                        {
                            ControlItem.numericUpDownWarningCount.Value = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadWarningCount", 2000).ToString());
                            bChanged = true;
                        }
                        if (ControlItem.numericUpDownDueCount.Value != int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadDueCount", 3000).ToString()))
                        {
                            ControlItem.numericUpDownDueCount.Value = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadDueCount", 3000).ToString());
                            bChanged = true;
                        }
                        //if (ControlItem.progressBarClean.Minimum!= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}MinCleanCount", 1000).ToString()))
                        //{
                        //    ControlItem.progressBarClean.Minimum = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}MinCleanCount", 1000).ToString());
                        //}
                        if (int.Parse(ControlItem.labelCleanCount.Text) !=
                            (m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", itemArrayNo, "") - int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}MinCleanCount", 1000).ToString())))
                        {
                            ControlItem.labelCleanCount.Text = (m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", itemArrayNo, "") - int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}MinCleanCount", 1000).ToString())).ToString();
                            bChanged = true;
                        }
                        if (ControlItem.progressBarWarningDue.Maximum != int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadDueCount", 3000).ToString()))
                        {
                            ControlItem.progressBarWarningDue.Maximum = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadDueCount", 3000).ToString());
                        }
                    }

                    if (ControlItem.progressBarClean.Minimum != int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}MinCleanCount", 1000).ToString()))
                    {
                        ControlItem.progressBarClean.Minimum = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}MinCleanCount", 1000).ToString());
                    }
                    if (ControlItem.progressBarClean.Maximum != int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString()))
                    {
                        ControlItem.progressBarClean.Maximum = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString());
                    }

                    if (bChanged == true)
                    {
                        if (ControlItem.numericUpDownCurrentCount.Value < int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString()) && ControlItem.numericUpDownCurrentCount.Value < ControlItem.numericUpDownWarningCount.Value)
                        {
                            ControlItem.progressBarClean.Value = (int)ControlItem.numericUpDownCurrentCount.Value;
                            ControlItem.progressBarWarningDue.Value = (int)ControlItem.numericUpDownCurrentCount.Value;
                            ControlItem.panel1.BackColor = Color.PaleTurquoise;
                        }
                        else if (ControlItem.numericUpDownCurrentCount.Value >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString()) && ControlItem.numericUpDownCurrentCount.Value < ControlItem.numericUpDownWarningCount.Value)
                        {
                            ControlItem.progressBarClean.Value = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString());
                            ControlItem.progressBarWarningDue.Value = (int)ControlItem.numericUpDownCurrentCount.Value;
                            if (!ControlItem.ItemName.Contains("Pick Up Head"))
                            {
                                ControlItem.panel1.BackColor = Color.Yellow;
                            }
                            else
                            {
                                ControlItem.panel1.BackColor = Color.PaleTurquoise;
                            }
                        }
                        else if (ControlItem.numericUpDownCurrentCount.Value >= ControlItem.numericUpDownWarningCount.Value && ControlItem.numericUpDownCurrentCount.Value < ControlItem.numericUpDownDueCount.Value && ControlItem.numericUpDownCurrentCount.Value < int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString()))
                        {
                            ControlItem.progressBarClean.Value = (int)ControlItem.numericUpDownCurrentCount.Value;
                            //ControlItem.progressBarWarningDue.Value = (int)ControlItem.numericUpDownWarningCount.Value;
                            ControlItem.progressBarWarningDue.Value = (int)ControlItem.numericUpDownCurrentCount.Value;
                            ControlItem.panel1.BackColor = Color.Orange;
                        }
                        else if (ControlItem.numericUpDownCurrentCount.Value >= ControlItem.numericUpDownWarningCount.Value && ControlItem.numericUpDownCurrentCount.Value < ControlItem.numericUpDownDueCount.Value && ControlItem.numericUpDownCurrentCount.Value >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString()))
                        {
                            ControlItem.progressBarClean.Value = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString());
                            //ControlItem.progressBarWarningDue.Value = (int)ControlItem.numericUpDownWarningCount.Value;
                            ControlItem.progressBarWarningDue.Value = (int)ControlItem.numericUpDownCurrentCount.Value;
                            //ControlItem.panel1.BackColor = Color.Orange;
                            if (!ControlItem.ItemName.Contains("Pick Up Head"))
                            {
                                ControlItem.panel1.BackColor = Color.Yellow;
                            }
                            else
                            {
                                ControlItem.panel1.BackColor = Color.Orange;
                            }
                        }
                        else if (ControlItem.numericUpDownCurrentCount.Value >= ControlItem.numericUpDownDueCount.Value && ControlItem.numericUpDownCurrentCount.Value < int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString()))
                        {
                            ControlItem.progressBarClean.Value = (int)ControlItem.numericUpDownCurrentCount.Value;
                            ControlItem.progressBarWarningDue.Value = (int)ControlItem.numericUpDownDueCount.Value;
                            ControlItem.panel1.BackColor = Color.Red;
                        }
                        else if (ControlItem.numericUpDownCurrentCount.Value >= ControlItem.numericUpDownDueCount.Value && ControlItem.numericUpDownCurrentCount.Value >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString()))
                        {
                            ControlItem.progressBarClean.Value = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ControlItem.ItemName}CleanCount", 1000).ToString());
                            ControlItem.progressBarWarningDue.Value = (int)ControlItem.numericUpDownDueCount.Value;
                            ControlItem.panel1.BackColor = Color.Red;
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

        void UpdateMessage(string Message)
        {
            labelMessage.Text = Message;
        }

        void UpdateSettingParameter()
        {
            //BondHeadCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("BondHeadCleanCount", 1000).ToString());
            for (int i = 1; i <= 2; i++)
            {
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"Pick Up Head {i}CleanCount", 1000).ToString()) < int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadSettingCleanCount", 1000).ToString()))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter($"Pick Up Head {i}CleanCount", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpSettingCleanCount", 1000).ToString()));
                }
                m_ProductShareVariables.RegKey.SetEstekKeyParameter($"Pick Up Head {i}MinCleanCount",
                    (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"Pick Up Head {i}CleanCount", 1000).ToString()) - int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadSettingCleanCount", 1000).ToString())));
                //int a = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"Bond Head {i}MinCleanCount", 0).ToString());
            }

            //int d = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"Aligner TipMinCleanCount", 0).ToString());
            PickUpHeadWarningCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadWarningCount", 2000).ToString());
            PickUpHeadDueCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadDueCount", 3000).ToString());

            nPickUpHeadSettingCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadSettingCleanCount", 1000).ToString());
            m_MaintenanceSetting.numericUpDownCleanCount.Value = nPickUpHeadSettingCleanCount;
            m_MaintenanceSetting.numericUpDownDueCount.Value = PickUpHeadDueCount;
            m_MaintenanceSetting.numericUpDownWarningCount.Value = PickUpHeadWarningCount;
            m_MaintenanceSetting.comboBoxComsumablePart.SelectedIndex = 0;
            //#region Turret&Flipper Maintenance
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH1.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 0);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH2.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 1);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH3.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 2);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH4.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 3);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH5.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 4);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH6.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 5);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH7.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 6);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH8.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 7);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH9.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 8);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH10.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 9);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH11.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 10);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH12.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 11);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH13.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 12);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH14.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 13);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH15.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 14);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH16.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 15);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH17.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 16);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH18.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 17);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH19.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 18);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH20.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 19);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH21.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 20);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH22.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 21);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH23.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 22);
            //m_TurretFlipperMaintenance.checkBoxMaintenancePUH24.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenancePUH", 23);

            //m_TurretFlipperMaintenance.checkBoxMaintenanceFH1.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenanceFH", 0);
            //m_TurretFlipperMaintenance.checkBoxMaintenanceFH2.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenanceFH", 1);
            //m_TurretFlipperMaintenance.checkBoxMaintenanceFH3.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenanceFH", 2);
            //m_TurretFlipperMaintenance.checkBoxMaintenanceFH4.Checked = m_ProductRTSSProcess.GetProductionArrayBool("MaintenanceFH", 3);

            //#endregion Turret&Flipper Maintenance
        }

        void UpdateCurrentCount()
        {
            for (int i = 0; i <= 1; i++)
            {
                m_ProductRTSSProcess.SetProductionArray("PickUpHeadCount", i, "", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head " + (i + 1), 1).ToString()));
            }
        }

        void UpdateCurrentItemList()
        {
            for (int i = 0; i <= 1; i++)
            {
                ItemList[i].CurrentCount = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "");
            }
        }

        void AddItemList()
        {
            UpdateCurrentCount();
            //nSortMode = m_MaintenanceSetting.comboBoxArrageBy.SelectedIndex;
            ItemList.Clear();
            ItemList.Add(new ItemInfo() { ItemName = "Pick Up Head 1", MinimumCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 1MinCleanCount", 0).ToString()), CleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 1CleanCount", 1000).ToString()), WarningCount = PickUpHeadWarningCount, DueCount = PickUpHeadDueCount, SettingCleanCount = nPickUpHeadSettingCleanCount, CurrentCount = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", 0, ""), ItemLocation = @"C:\Users\ASUS STRIX\Desktop\Item1.png" });
            ItemList.Add(new ItemInfo() { ItemName = "Pick Up Head 2", MinimumCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 2MinCleanCount", 0).ToString()), CleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 2CleanCount", 1000).ToString()), WarningCount = PickUpHeadWarningCount, DueCount = PickUpHeadDueCount, SettingCleanCount = nPickUpHeadSettingCleanCount, CurrentCount = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", 1, ""), ItemLocation = @"C:\Users\ASUS STRIX\Desktop\Item1.png" });
            //ItemList.Add(new ItemInfo() { ItemName = "Pick Up Head 3", MinimumCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 3MinCleanCount", 0).ToString()), CleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 3CleanCount", 1000).ToString()), WarningCount = PickUpHeadWarningCount, DueCount = PickUpHeadDueCount, SettingCleanCount = nPickUpHeadSettingCleanCount, CurrentCount = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", 2, ""), ItemLocation = @"C:\Users\ASUS STRIX\Desktop\Item1.png" });
            //ItemList.Add(new ItemInfo() { ItemName = "Pick Up Head 4", MinimumCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 4MinCleanCount", 0).ToString()), CleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 4CleanCount", 1000).ToString()), WarningCount = PickUpHeadWarningCount, DueCount = PickUpHeadDueCount, SettingCleanCount = nPickUpHeadSettingCleanCount, CurrentCount = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", 3, ""), ItemLocation = @"C:\Users\ASUS STRIX\Desktop\Item1.png" });
            //ItemList.Add(new ItemInfo() { ItemName = "Pick Up Head 5", MinimumCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 5MinCleanCount", 0).ToString()), CleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 5CleanCount", 1000).ToString()), WarningCount = PickUpHeadWarningCount, DueCount = PickUpHeadDueCount, SettingCleanCount = nPickUpHeadSettingCleanCount, CurrentCount = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", 4, ""), ItemLocation = @"C:\Users\ASUS STRIX\Desktop\Item1.png" });
            //ItemList.Add(new ItemInfo() { ItemName = "Pick Up Head 6", MinimumCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 6MinCleanCount", 0).ToString()), CleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head 6CleanCount", 1000).ToString()), WarningCount = PickUpHeadWarningCount, DueCount = PickUpHeadDueCount, SettingCleanCount = nPickUpHeadSettingCleanCount, CurrentCount = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", 5, ""), ItemLocation = @"C:\Users\ASUS STRIX\Desktop\Item1.png" });
        }

        void InitialSettingInterface()
        {
            //default
            // m_MaintenanceSetting.comboBoxComsumablePart.SelectedIndex = 0;
            //nSortMode = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nSortMode", 1).ToString());
            //nTriggerMode = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nTriggerMode", 0).ToString());
            //nCloseMode = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCloseMode", 0).ToString());
            //m_MaintenanceSetting.comboBoxArrageBy.SelectedIndex = nSortMode;
            //m_MaintenanceSetting.comboBoxTriggerMode.SelectedIndex = nTriggerMode;
            //m_MaintenanceSetting.comboBoxCloseMode.SelectedIndex = nCloseMode;
            foreach (var item in ItemList)
            {
                m_MaintenanceSetting.comboBoxComsumableItem.Items.Add(item.ItemName);
            }
            m_MaintenanceSetting.comboBoxComsumableItem.Items.Add("All Pick Up Head");
            //m_MaintenanceSetting.comboBoxComsumableItem.Items.Add("All Flipper Head");
            m_MaintenanceSetting.comboBoxComsumableItem.SelectedIndex = 0;
        }

        void SortItemList()
        {
            if (nSortMode == 1)
            {
                ItemList = ItemList.OrderByDescending(o => o.CurrentCount).ToList();
            }
        }

        void UpdateMaintananceCountItemInterface()
        {
            CheckCanFormClose();
            flowLayoutPanel1.Controls.Clear();
            foreach (var item in ItemList)
            {
                //var ItemContol = AddUsercontrol(item.CleanCount, item.WarningCount, item.DueCount, item.CurrentCount, item.ItemName, item.ItemLocation);
                var ItemContol = AddUsercontrol(item.MinimumCleanCount, item.CleanCount, item.WarningCount, item.DueCount, item.CurrentCount, item.SettingCleanCount, item.ItemName, item.ItemLocation);
                flowLayoutPanel1.Controls.Add(ItemContol);

                if (m_ProductShareVariables.nLoginAuthority > 1)
                {
                    panel1.Controls.Add(m_MaintenanceSetting);
                    buttonSave.Visible = true;
                    //panel2.Controls.Add(m_TurretFlipperMaintenance);
                }
                else
                {
                    tabControl1.TabPages.Remove(tabPage2);
                    buttonSave.Visible = false;
                    //tabControl1.TabPages.Remove(tabPagePerformMaintenance);
                }
            }
        }

        //Product.FormConsumablePart.MaintenanceCountItem AddUsercontrol(int CleanCountvalue, int WarningCountvalue, int DueCountvalue, int CurrentCountvalue, string Name, string ItemImageLocation)
        Product.FormConsumablePart.MaintenanceCountItem AddUsercontrol(int MinimumCleanCount, int CleanCountvalue, int WarningCountvalue, int DueCountvalue, int CurrentCountvalue, int nSettingCleanCountValue, string Name, string ItemImageLocation)
        {
            var Itemname = new Product.FormConsumablePart.MaintenanceCountItem();
            Itemname.ItemName = Name;
            Itemname.CleanCountValue = CleanCountvalue;
            Itemname.WarningCountValue = WarningCountvalue;
            Itemname.DueCountValue = DueCountvalue;
            Itemname.CurrentCountValue = CurrentCountvalue;
            Itemname.MinimumCleanCount = MinimumCleanCount;
            Itemname.nSettingCleanCountValue = nSettingCleanCountValue;
            Itemname.Dock = DockStyle.Top;
            Itemname.updateInterface();
            return Itemname;
        }


        private void comboBoxComsumableItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_MaintenanceSetting.buttonDoneCleaning.Enabled = true;
                m_MaintenanceSetting.buttonReset.Enabled = true;
                m_MaintenanceSetting.buttonUndo.Enabled = true;
                bPressResetButton = false;
                bPressUndoButton = false;
                bPressUndoAndResetButton = false;
                if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString() != "")
                {
                    if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("All") == false)
                    {
                        foreach (var item in ItemList)
                        {
                            if (item.ItemName == m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString())
                            {
                                m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value = item.CurrentCount;
                                break;
                            }
                        }
                    }
                    else if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("All Pick Up Head"))
                    {
                        m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", 0, "");
                    }
                }
                else
                {
                    MessageBox.Show("Please Select a valid Item");
                }
            }
            catch
            {
                MessageBox.Show("a");
            }
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
            if (m_bgwUserInterface.WorkerSupportsCancellation == true)
            {
                m_bgwUserInterface.CancelAsync();
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("All Pick Up Head"))
            {
                nPreviousValueBHArray = new int[2];
                for (int i = 0; i < nPreviousValueBHArray.Length; i++)
                {
                    nPreviousValueBHArray[i] = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "");
                }
            }
            if (bPressResetButton == false && bPressUndoAndResetButton == false && bSaveReset == false)
            {
                nCurrentItemIdex = (int)m_MaintenanceSetting.comboBoxComsumableItem.SelectedIndex;
                nPreviousValue = (int)m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value;
            }
            ResetItemName = m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString();
            m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value = 0;
            if (bPressUndoButton == false)
            {
                bPressResetButton = true;
            }
            else
            {
                bPressUndoAndResetButton = true;
            }
            m_ProductProcessEvent.PCS_GUI_Clean_Count_Alarm.Reset();
            m_ProductProcessEvent.PCS_GUI_Warning_Count_Alarm.Reset();
            m_ProductProcessEvent.PCS_GUI_Due_Count_Alarm.Reset();
            m_MaintenanceSetting.buttonReset.Enabled = false;
            m_MaintenanceSetting.buttonUndo.Enabled = true;
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            if (bPressUndoButton == false)
            {
                nPreviousUndoValue = (int)m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value;
            }
            UndoItemName = m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString();
            m_MaintenanceSetting.comboBoxComsumableItem.SelectedIndex = nCurrentItemIdex;
            m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value = nPreviousValue;
            if (bPressResetButton == false || ((bPressResetButton || bPressUndoAndResetButton) && bSaveReset))
            {
                bSaveReset = false;
                bPressUndoButton = true;
            }
            m_MaintenanceSetting.buttonReset.Enabled = true;
            m_MaintenanceSetting.buttonUndo.Enabled = false;
        }
        private void ButtonDoneCleaning_Click(object sender, EventArgs e)
        {
            m_ProductProcessEvent.PCS_GUI_Clean_Count_Alarm.Reset();
            m_ProductProcessEvent.PCS_GUI_Warning_Count_Alarm.Reset();
            m_ProductProcessEvent.PCS_GUI_Due_Count_Alarm.Reset();
            int changedCleanCount = 0;
            string ItemNameForClean = "";
            if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString() != "")
            {

                bButtonDoneCleanPress = true;

            }
            else
            {
                MessageBox.Show("Please Select a Valid Item");
            }
            m_MaintenanceSetting.buttonDoneCleaning.Enabled = false;
        }
        void CheckCanFormClose()
        {
            if (nCloseMode == 0)
            {
                buttonClose.Enabled = true;
            }
            if (nCloseMode == 1)
            {
                foreach (var item in ItemList)
                {
                    if (item.CurrentCount > item.DueCount)
                    {
                        buttonClose.Enabled = false;
                        break;
                    }
                    buttonClose.Enabled = true;
                }
            }
        }

        private void buttonSave_Click_1(object sender, EventArgs e)
        {
            int changedCleanCount = 0;
            string ItemNameForClean = "";

            m_MaintenanceSetting.buttonDoneCleaning.Enabled = true;
            m_MaintenanceSetting.buttonReset.Enabled = true;
            m_MaintenanceSetting.buttonUndo.Enabled = true;
            string NameItem = "";
            m_ProductProcessEvent.PCS_GUI_Clean_Count_Alarm.Reset();
            m_ProductProcessEvent.PCS_GUI_Warning_Count_Alarm.Reset();
            m_ProductProcessEvent.PCS_GUI_Due_Count_Alarm.Reset();
            UserName = m_ProductShareVariables.strLoginUserName;
            if ((bPressResetButton || bPressUndoAndResetButton) && nPreviousValue != m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value)
            {
                //bSaveReset = true;
                //bPressResetButton = false;
                //bPressUndoButton = false;
                //bPressUndoAndResetButton = false;
                string MainCountLogMessage = $"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User Press Reset Maintenance Count.";
                string MainCountLogMessage2 = $"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User Name: {UserName}.";
                string MainCountLogMessage3 = $"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Item Name: {ResetItemName}, Previous Count No: {nPreviousValue}, Count No Reset To: {m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value}.";
                WriteMaintCountLog(MainCountLogMessage, MainCountLogMessage2, MainCountLogMessage3);
            }
            else if (bPressUndoButton && bPressUndoAndResetButton == false && nPreviousUndoValue != m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value)
            {
                //bSaveReset = false;
                //bPressResetButton = false;
                //bPressUndoButton = false;
                //bPressUndoAndResetButton = false;
                string MainCountLogMessage = $"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User Press Undo Maintenance Count.";
                string MainCountLogMessage2 = $"{DateTime.Now.ToString("yyyyMMdd HHmmss")} User Name: {UserName}.";
                string MainCountLogMessage3 = $"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Item Name: {ResetItemName}, Previous Count No: {nPreviousUndoValue}, Count No Undo To: {m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value}.";
                WriteMaintCountLog(MainCountLogMessage, MainCountLogMessage2, MainCountLogMessage3);
            }
            else
            {
                //bSaveReset = false;
                //bPressResetButton = false;
                //bPressUndoButton = false;
                //bPressUndoAndResetButton = false;
            }
            var itemName = m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Substring(m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Length - 2);
            if (int.TryParse(itemName, out int itemArrayNo))
            {
                itemArrayNo = itemArrayNo - 1;
            }
            else
            {
                itemArrayNo = 0;
            }
            if (bPressResetButton || bPressUndoAndResetButton)
            {
                if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("All") == false)
                {
                    if (nPreviousValue != m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value)
                    {
                        if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("Pick Up Head"))
                        {
                            NameItem = "PickUpHead";
                            m_ProductRTSSProcess.SetProductionArray("PickUpHeadCount", itemArrayNo, "", 0);
                        }

                        m_ProductShareVariables.RegKey.SetEstekKeyParameter(m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString(), m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value);
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}WarningAlarm", "false");
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}CleanAlarm", "false");
                        nPreviousCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}CleanCount", 1000).ToString());
                        nPreviousMinCleanCount = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}MinCleanCount", 0).ToString());
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}CleanCount", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{NameItem}SettingCleanCount", 1000).ToString()));
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}MinCleanCount", 0);
                    }
                }
                else if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("All Pick Up Head"))
                {
                    //if (bPressResetButton || bPressUndoAndResetButton)
                    //if ((bPressResetButton || bPressUndoAndResetButton) && nPreviousValue != m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value)
                    //{
                    foreach (var item in ItemList)
                    {
                        if (item.ItemName.Contains("Pick Up Head"))
                        {
                            string itemNameLastDigit = item.ItemName.Substring(item.ItemName.Length - 2);
                            int No = int.Parse(itemNameLastDigit) - 1;

                            m_ProductShareVariables.RegKey.SetEstekKeyParameter(item.ItemName, m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value);
                            m_ProductRTSSProcess.SetProductionArray("PickUpHeadCount", No, "", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter(item.ItemName, 1).ToString()));

                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}WarningAlarm", "false");
                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}CleanAlarm", "false");

                            nPreviousCleanCountBHArray[No] = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{item.ItemName}CleanCount", 1000).ToString());
                            nPreviousMinCleanCountBHArray[No] = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{item.ItemName}MinCleanCount", 0).ToString());

                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}CleanCount", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadSettingCleanCount", 1000).ToString()));
                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}MinCleanCount", 0);
                        }
                    }
                    //for (int i = 0; i <= 23; i++)
                    //{
                    //    m_ProductRTSSProcess.SetProductionArray("BondHeadCount", i, "", 0);
                    //}
                    //}
                }
            }
            else if (bPressUndoButton && bPressUndoAndResetButton == false)
            {
                if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("All") == false)
                {
                    if (nPreviousUndoValue != m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value)
                    {
                        if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("Pick Up Head"))
                        {
                            NameItem = "PickUpHead";
                            m_ProductRTSSProcess.SetProductionArray("PickUpHeadCount", itemArrayNo, "", Convert.ToInt32(m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value));
                        }
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter(m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString(), m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value);

                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}WarningAlarm", "false");
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}CleanAlarm", "false");

                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}CleanCount", nPreviousCleanCount);
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}MinCleanCount", nPreviousMinCleanCount);

                    }
                }
                else if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("All Pick Up Head"))
                {
                    foreach (var item in ItemList)
                    {
                        if (item.ItemName.Contains("Pick Up Head"))
                        {
                            string itemNameLastDigit = item.ItemName.Substring(item.ItemName.Length - 2);
                            int No = int.Parse(itemNameLastDigit) - 1;

                            m_ProductShareVariables.RegKey.SetEstekKeyParameter(item.ItemName, nPreviousValueBHArray[No]);
                            m_ProductRTSSProcess.SetProductionArray("PickUpHeadCount", No, "", nPreviousValueBHArray[No]);

                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}WarningAlarm", "false");
                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}CleanAlarm", "false");

                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}CleanCount", nPreviousCleanCountBHArray[No]);
                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}MinCleanCount", nPreviousMinCleanCountBHArray[No]);
                        }
                    }
                    //for (int i = 0; i <= 23; i++)
                    //{
                    //    m_ProductRTSSProcess.SetProductionArray("BondHeadCount", i, "", 0);
                    //}
                    //}
                }
            }
            if (bButtonDoneCleanPress == true)
            {
                bButtonDoneCleanPress = false;
                if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("All") == false)
                {
                    //var itemName = m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Substring(m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Length - 2);
                    //if (int.TryParse(itemName, out int itemArrayNo))
                    //{
                    //    itemArrayNo = itemArrayNo - 1;
                    //}
                    //else
                    //{
                    //    itemArrayNo = 0;
                    //}
                    if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("Pick Up Head"))
                    {
                        ItemNameForClean = "PickUpHeadCount";
                        changedCleanCount = m_ProductRTSSProcess.GetProductionArray($"{ItemNameForClean}", itemArrayNo, "") + int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"PickUpHeadSettingCleanCount", 1000).ToString());
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}CleanCount", changedCleanCount);
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}MinCleanCount", m_ProductRTSSProcess.GetProductionArray($"{ItemNameForClean}", itemArrayNo, ""));
                    }
                    //else if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("Ejector"))
                    //{
                    //    MessageBox.Show("Ejector Needle do not have clean count. Please select another item.");
                    //    //ItemNameForClean = "EjectorNeedle";
                    //    //changedCleanCount = m_ProductRTSSProcess.GetProductionArray($"{ItemNameForClean}", itemArrayNo, "") + int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{ItemNameForClean}SettingCleanCount", 1000).ToString());
                    //    //m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}CleanCount", changedCleanCount);
                    //    //m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}MinCleanCount", m_ProductRTSSProcess.GetProductionArray($"{ItemNameForClean}", itemArrayNo, ""));
                    //}

                    m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}CleanAlarm", "false");

                    //int a = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString()}MinCleanCount", 0).ToString());
                    foreach (Product.FormConsumablePart.MaintenanceCountItem ControlItem in flowLayoutPanel1.Controls)
                    {
                        if (ControlItem.ItemName == m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString() &&
                            !m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("Ejector"))
                        {
                            ControlItem.progressBarClean.Minimum = m_ProductRTSSProcess.GetProductionArray($"{ItemNameForClean}", itemArrayNo, "");
                            ControlItem.progressBarClean.Maximum = changedCleanCount;
                        }
                    }
                }
                else if (m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString().Contains("All Pick Up Head"))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        changedCleanCount = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "") + int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadSettingCleanCount", 1000).ToString());
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"Pick Up Head {i + 1}CleanCount", changedCleanCount);
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"Pick Up Head {i + 1}MinCleanCount", m_ProductRTSSProcess.GetProductionArray($"PickUpHeadCount", i, ""));

                        foreach (Product.FormConsumablePart.MaintenanceCountItem ControlItem in flowLayoutPanel1.Controls)
                        {
                            if (ControlItem.ItemName == $"Pick Up Head {i + 1}")
                            {
                                ControlItem.progressBarClean.Minimum = m_ProductRTSSProcess.GetProductionArray($"PickUpHeadCount", i, "");
                                ControlItem.progressBarClean.Maximum = changedCleanCount;
                            }
                        }
                        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"Pick Up Head {i + 1}CleanAlarm", "false");

                    }
                }
            }

            nPickUpHeadSettingCleanCount = (int)m_MaintenanceSetting.numericUpDownCleanCount.Value;
            PickUpHeadWarningCount = (int)m_MaintenanceSetting.numericUpDownWarningCount.Value;

            if ((int)m_MaintenanceSetting.numericUpDownDueCount.Value < (int)m_MaintenanceSetting.numericUpDownWarningCount.Value)
            {
                if (MessageBox.Show("Due count should not less than Warning Count", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) == DialogResult.OK)
                {
                    PickUpHeadDueCount = (int)m_MaintenanceSetting.numericUpDownWarningCount.Value;
                    m_MaintenanceSetting.numericUpDownDueCount.Value = PickUpHeadDueCount;
                }
            }
            else
            {
                PickUpHeadDueCount = (int)m_MaintenanceSetting.numericUpDownDueCount.Value;
            }
            if ((int)m_MaintenanceSetting.numericUpDownDueCount.Value < (int)m_MaintenanceSetting.numericUpDownCleanCount.Value)
            {
                if (MessageBox.Show("Due count should not less than Clean Count", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) == DialogResult.OK)
                {
                    PickUpHeadDueCount = (int)m_MaintenanceSetting.numericUpDownCleanCount.Value;
                    m_MaintenanceSetting.numericUpDownDueCount.Value = PickUpHeadDueCount;
                }
            }
            else
            {
                PickUpHeadDueCount = (int)m_MaintenanceSetting.numericUpDownDueCount.Value;
            }
            if ((int)m_MaintenanceSetting.numericUpDownWarningCount.Value < (int)m_MaintenanceSetting.numericUpDownCleanCount.Value)
            {
                if (MessageBox.Show("Warning count should not less than Clean Count", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) == DialogResult.OK)
                {
                    PickUpHeadWarningCount = (int)m_MaintenanceSetting.numericUpDownCleanCount.Value;
                    m_MaintenanceSetting.numericUpDownWarningCount.Value = PickUpHeadWarningCount;
                }
            }
            else
            {
                PickUpHeadWarningCount = (int)m_MaintenanceSetting.numericUpDownWarningCount.Value;
            }
            nPreviousPickUpHeadSettingCleanValue = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadSettingCleanCount", 1000).ToString());
            nChangedPickUpHeadCleanValue = nPickUpHeadSettingCleanCount - nPreviousPickUpHeadSettingCleanValue;
            foreach (var item in ItemList)
            {
                if (item.ItemName.Contains("Pick Up Head"))
                {
                    nPreviousPickUpHeadCleanValue = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{item.ItemName}CleanCount", 1000).ToString());
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}CleanCount", nPreviousPickUpHeadCleanValue + nChangedPickUpHeadCleanValue);
                }
            }
            //m_ProductShareVariables.RegKey.SetEstekKeyParameter("BondHeadCleanCount", BondHeadCleanCount);
            m_ProductShareVariables.RegKey.SetEstekKeyParameter("PickUpHeadSettingCleanCount", nPickUpHeadSettingCleanCount);

            m_ProductShareVariables.RegKey.SetEstekKeyParameter("PickUpHeadWarningCount", PickUpHeadWarningCount);
            m_ProductShareVariables.RegKey.SetEstekKeyParameter("PickUpHeadDueCount", PickUpHeadDueCount);

            nPickUpHeadSettingCleanCount = (int)m_MaintenanceSetting.numericUpDownCleanCount.Value;

            PickUpHeadWarningCount = (int)m_MaintenanceSetting.numericUpDownWarningCount.Value;

            if ((int)m_MaintenanceSetting.numericUpDownDueCount.Value < (int)m_MaintenanceSetting.numericUpDownWarningCount.Value)
            {
                if (MessageBox.Show("Due count should not less than Warning Count", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) == DialogResult.OK)
                {
                    PickUpHeadDueCount = (int)m_MaintenanceSetting.numericUpDownWarningCount.Value;
                    m_MaintenanceSetting.numericUpDownDueCount.Value = PickUpHeadDueCount;
                }
            }
            else
            {
                PickUpHeadDueCount = (int)m_MaintenanceSetting.numericUpDownDueCount.Value;
            }
            nPreviousPickUpHeadSettingCleanValue = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadSettingCleanCount", 1000).ToString());
            nChangedPickUpHeadCleanValue = nPickUpHeadSettingCleanCount - nPreviousPickUpHeadSettingCleanValue;
            foreach (var item in ItemList)
            {
                if (item.ItemName.Contains("Pick Up Head"))
                {
                    nPreviousPickUpHeadCleanValue = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"{item.ItemName}CleanCount", 1000).ToString());
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter($"{item.ItemName}CleanCount", nPreviousPickUpHeadCleanValue + nChangedPickUpHeadCleanValue);
                }
            }
            //m_ProductShareVariables.RegKey.SetEstekKeyParameter("BondHeadCleanCount", BondHeadCleanCount);
            m_ProductShareVariables.RegKey.SetEstekKeyParameter("PickUpHeadSettingCleanCount", nPickUpHeadSettingCleanCount);

            m_ProductShareVariables.RegKey.SetEstekKeyParameter("PickUpHeadWarningCount", PickUpHeadWarningCount);
            m_ProductShareVariables.RegKey.SetEstekKeyParameter("PickUpHeadDueCount", PickUpHeadDueCount);

            //MessageBox.Show(m_MaintenanceSetting.comboBoxComsumableItem.SelectedItem.ToString() +" = "+ m_MaintenanceSetting.numericUpDownConsumableItemCurrentCount.Value);

            bSaveReset = false;
            bPressResetButton = false;
            bPressUndoButton = false;
            bPressUndoAndResetButton = false;
            //AddItemList();
            //SortItemList();
            //UpdateMaintananceCountItemInterface();
            //CheckCanFormClose();
            MessageBox.Show("Saved");
        }
        void WriteMaintCountLog(string Message1, string Message2, string Message3)
        {
            string MainCountLogDirectory = (@"..\Maint Count Log");
            string MainCountLogFile = ($@"{MainCountLogDirectory}\MaintCountLog_{DateTime.Now.ToString("yyyyMMdd")}.log");
            try
            {

                if (!Directory.Exists(MainCountLogDirectory))
                {
                    Directory.CreateDirectory(MainCountLogDirectory);
                }
                if (File.Exists(MainCountLogFile))
                {
                    using (StreamWriter sw = File.AppendText(MainCountLogFile))
                    {
                        sw.WriteLine("");
                        sw.WriteLine(Message1);
                        sw.WriteLine(Message2);
                        sw.WriteLine(Message3);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.CreateText(MainCountLogFile))
                    {
                        sw.WriteLine(Message1);
                        sw.WriteLine(Message2);
                        sw.WriteLine(Message3);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} .", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
    }
}
