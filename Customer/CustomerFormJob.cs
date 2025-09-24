using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Common;
using System.Windows.Forms;
using Machine;

namespace Customer
{
    public class CustomerFormJob : Product.ProductFormJob
    {
        //public CustomerFormConversion m_formConversion;
        public CustomerFormNewLot m_formNewLot;
        //public CustomerFormBarcode m_formBarcode;
        public CustomerFormTeachMap m_formTeachMap;

        private CustomerShareVariables m_CustomerShareVariables;// = new CustomerShareVariables();
        private CustomerProcessEvent m_CustomerProcessEvent;// = new CustomerProcessEvent();

        private CustomerStateControl m_CustomerStateControl;// = new ProductStateControl();
        private CustomerRTSSProcess m_CustomerRTSSProcess;// =  new ProductRTSSProcess();  
        public new CustomerAlarmList m_alarmData = new CustomerAlarmList();

        private CustomerReportProcess m_CustomerReportProcess;
        private CustomerReportEvent m_CustomerReportEvent;

        private CustomerPreviousLotInfo m_CustomerPreviousLotInfo;
        public CustomerShareVariables customerShareVariables
        {
            set
            {
                m_CustomerShareVariables = value;
                productShareVariables = m_CustomerShareVariables;
            }
        }

        public CustomerProcessEvent customerProcessEvent
        {
            set
            {
                m_CustomerProcessEvent = value;
                productProcessEvent = m_CustomerProcessEvent;
            }
        }

        public CustomerStateControl customerStateControl
        {
            set
            {
                m_CustomerStateControl = value;
                productStateControl = m_CustomerStateControl;
            }
        }

        public CustomerRTSSProcess customerRTSSProcess
        {
            set
            {
                m_CustomerRTSSProcess = value;
                productRTSSProcess = m_CustomerRTSSProcess;
            }
        }

        public CustomerReportProcess customerReportProcess
        {
            set
            {
                m_CustomerReportProcess = value;
                productReportProcess = value;
            }
        }

        public CustomerReportEvent customerReportEvent
        {
            set
            {
                m_CustomerReportEvent = value;
                productReportEvent = value;
            }
        }
        override public void Initialize()
        {
            base.Initialize();
            buttonNewLot.Size = new System.Drawing.Size(100, 71);
            //buttonEndLot.Size = new System.Drawing.Size(100, 71);
            labelLotID.Visible = false;
            labelLotID.Location = new System.Drawing.Point(16, 20);
            textBoxLotID.Visible = false;
            textBoxLotID.Location = new System.Drawing.Point(187, 15);
            labelOperatorID.Visible = false;
            textBoxOperatorID.Visible = false;
            //tabControlLeftPanel.TabPages[0].Controls.Add(m_LotDetailDisplay);
            //panelLeft.Controls.Add(m_LotDetailDisplay);
        }
        override public void InitializeAlarmList()
        {
            if (File.Exists(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strAlarmFile))
            {
                m_alarmData = Tools.Deserialize<CustomerAlarmList>(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strAlarmFile);
                Tools.Serialize(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strAlarmFile, m_alarmData);
                UpdateAlarmList();
            }
            else
            {
                Tools.Serialize(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strAlarmFile, m_alarmData);
                m_alarmData = Tools.Deserialize<CustomerAlarmList>(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strAlarmFile);
                UpdateAlarmList();
            }
        }

        override public void UpdateAlarmList()
        {
            FieldInfo[] fields = typeof(CustomerAlarmList).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo _fields in fields)
            {
                m_alarmList.Add((Machine.Platform.Alarm)_fields.GetValue(m_alarmData));
            }

        }

        override public void LaunchNewLotForm()
        {
            Machine.EventLogger.WriteLog(string.Format("{0} Click new lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
            if (m_formNewLot != null)
            {
                if (!m_formNewLot.IsDisposed)
                {
                    m_formNewLot.Focus();
                    return;
                }
            }
            CreateFormNewLot();
            SetFormNewLot();
            SetFormNewLotVariables();
            InitializeFormNewLot();

            m_formNewLot.Show();
            m_formNewLot.textBoxOperatorID.Focus();
        }

        #region Form New Lot
        override public void CreateFormNewLot()
        {
            m_formNewLot = new CustomerFormNewLot();
        }

        override public void SetFormNewLot()
        {
            //base.m_formNewLot = m_formNewLot;
        }

        override public void SetFormNewLotVariables()
        {
            m_formNewLot.customerShareVariables = m_CustomerShareVariables;
            m_formNewLot.customerProcessEvent = m_CustomerProcessEvent;
            m_formNewLot.customerRTSSProcess = m_CustomerRTSSProcess;
            m_formNewLot.customerReportProcess = m_CustomerReportProcess;
            m_formNewLot.customerReportEvent = m_CustomerReportEvent;
            m_formNewLot.customerPreviousLotInfo = m_CustomerPreviousLotInfo;
        }

        override public void InitializeFormNewLot()
        {
            m_formNewLot.Initialize();
        }
        #endregion Form Teach Map
        
        #region Form Teach Map
        override public void CreateFormTeachMap()
        {
            m_formTeachMap = new CustomerFormTeachMap();
        }

        override public void SetFormTeachMap()
        {
            base.m_formTeachMap = m_formTeachMap;
        }

        override public void SetFormTeachMapVariables()
        {
            m_formTeachMap.customerShareVariables = m_CustomerShareVariables;
            m_formTeachMap.customerProcessEvent = m_CustomerProcessEvent;

            m_formTeachMap.customerStateControl = m_CustomerStateControl;
            m_formTeachMap.customerRTSSProcess = m_CustomerRTSSProcess;
        }

        override public void InitializeFormTeachMap()
        {
            m_formTeachMap.Initialize();
        }
        #endregion Form Teach Map

        override public void UpdateGUIInBackgroundworker()
        {
            base.UpdateGUIInBackgroundworker();
        }

        override public int OnAlarmAssist()
        {
            m_CustomerRTSSProcess.SetEvent("RMAIN_RTHD_ALARM_ASSIST_START", true);
            return 0;
        }
        override public int OnAlarmAssist(int alarmID, int alarmType)
        {
            int nError = 0;            
            base.OnAlarmAssist(alarmID, alarmType);
            //if (alarmID == 0)
            //if(false)
            {
                //m_CustomerReportProcess.AddRecord(new ReportProcess.Record { dateTime = DateTime.Now, eventID = m_CustomerShareVariables.customerReportEvent.EventUnloadMaterialTime.EventID, eventName = m_CustomerShareVariables.customerReportEvent.EventUnloadMaterialTime.EventName, lotID = m_CustomerShareVariables.strucInputProductInfo.LotID, alarmID = 0, alarmType = 0 });
            }
            return nError;
        }
        public override void OnChangeToOperatorAuthority()
        {
            base.OnChangeToOperatorAuthority();
            btnProductionStop.Visible = true;
        }

        override public void OnClearAlarmDone(string alarmID)
        {
            if(alarmID == "RemoveTray")
            {
                //m_CustomerReportProcess.AddRecord(new ReportProcess.Record { dateTime = DateTime.Now, eventID = m_CustomerShareVariables.customerReportEvent.EventUnloadMaterialTime.EventID, eventName = m_CustomerShareVariables.customerReportEvent.EventUnloadMaterialTime.EventName, lotID = m_CustomerShareVariables.strucInputProductInfo.LotID, alarmID = 0, alarmType = 0 });
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
            // comboBoxStartingFrameOrTile
            // 
            this.comboBoxStartingFrameOrTile.Size = new System.Drawing.Size(121, 24);
            // 
            // CustomerFormJob
            // 
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Name = "CustomerFormJob";
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
