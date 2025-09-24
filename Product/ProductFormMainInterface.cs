using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Common;
using System.Threading;

namespace Product
{
    public partial class ProductFormMainInterface : Machine.FormMainInterface
    {
        public new ProductFormJob m_formJob;// = new ProductFormJob();
        public new ProductFormRecipe m_formRecipe;// = new ProductFormRecipe();
        public new ProductFormReport m_formReport;// = new ProductFormReport();
        public new ProductFormTeachPoint m_formTeachPoint;
        public new ProductFormSetup m_formSetup;// = new ProductFormSetup();
        public new ProductFormMotionChart m_formMotionChart;
        public new ProductFormOption m_formOption;// = new ProductFormOption();
        public new ProductFormManual m_formManual;// = new ProductFormSetup();
        public new ProductFormConfiguration m_formConfiguration;// = new ProductFormConfiguration();

        private ProductShareVariables m_ProductShareVariables = new ProductShareVariables();
        private ProductProcessEvent m_ProductProcessEvent = new ProductProcessEvent();
        private ProductGUIEvent m_ProductGUIEvent = new ProductGUIEvent();

        private ProductStateControl m_ProductStateControl = new ProductStateControl();
        private ProductRTSSProcess m_ProductRTSSProcess = new ProductRTSSProcess();
        private ProductMainProcess m_ProductMainProcess = new ProductMainProcess();
        private ProductReportProcess m_ProductReportProcess = new ProductReportProcess();
        public string AssemblyVersion1 { set; get; }

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

        public ProductGUIEvent productGUIEvent
        {
            set
            {
                m_ProductGUIEvent = value;
                guiEvent = m_ProductGUIEvent;
            }
        }

        public ProductStateControl productStateControl
        {
            set
            {
                m_ProductStateControl = value;
                base.stateControl = m_ProductStateControl;
            }
        }     

        public ProductRTSSProcess productRTSSProcess
        {
            set
            {
                m_ProductRTSSProcess = value;
                base.rtssProcess = m_ProductRTSSProcess;
            }
        }

        public ProductMainProcess productMainProcess
        {
            set
            {
                m_ProductMainProcess = value;
                base.mainProcess = m_ProductMainProcess;
            }
        }

        public ProductReportProcess productReportProcess
        {
            set
            {
                m_ProductReportProcess = value;
                base.reportProcess = m_ProductReportProcess;
            }
        }
        public string ProductAssemblyName
        {
            set { AssemblyName = value; }
        }
        public string ProductAssemblyVersion
        {
            //get { return AssemblyVersion1; }
            set { AssemblyVersion = value; }
            //set { AssemblyVersion1 = value; }

        }
        public string ProductionAssemblyVersion1
        {
            get { return AssemblyVersion1; }
            //set { AssemblyVersion = value; }
            set { AssemblyVersion1 = value; }
        }
        public ProductFormMainInterface()
        {
            //base.Load += new System.EventHandler(Initialize);
        }
        
        override public void SetFormMainInterfaceVariables()
        {
            productShareVariables = m_ProductShareVariables;
            productProcessEvent = m_ProductProcessEvent;
            productGUIEvent = m_ProductGUIEvent;

            productStateControl = m_ProductStateControl;
            productRTSSProcess = m_ProductRTSSProcess;
            productMainProcess = m_ProductMainProcess;

            base.SetFormMainInterfaceVariables();
        }

        #region FormJob
        override public void CreateFormJob()
        {
            try
            {
                m_formJob = new ProductFormJob();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void SetFormJob()
        {
            try
            {
                base.m_formJob = m_formJob;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void SetFormJobVariables()
        {
            try
            {
                m_formJob.productShareVariables = m_ProductShareVariables;
                m_formJob.productProcessEvent = m_ProductProcessEvent;
                m_formJob.productStateControl = m_ProductStateControl;
                m_formJob.productRTSSProcess = m_ProductRTSSProcess;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        override public void InitializeFormJob()
        {
            try
            {
                //base.InitializeFormJob();
                m_formJob.Initialize();
                //
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        #endregion Form Job

        #region Form Recipe
        override public void CreateFormRecipe()
        {
            try
            {
                m_formRecipe = new ProductFormRecipe();
                //base.m_formRecipe = m_formRecipe;

               // m_formRecipe.productShareVariables = m_ProductShareVariables;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void SetFormRecipe()
        {
            base.m_formRecipe = m_formRecipe;
        }

        override public void SetFormRecipeVariables()
        {
            m_formRecipe.productShareVariables = m_ProductShareVariables;
            m_formRecipe.productProcessEvent = m_ProductProcessEvent;
            m_formRecipe.productRTSSProcess = m_ProductRTSSProcess;
        }

        override public void InitializeFormRecipe()
        {
            m_formRecipe.Initialize();
        }
        #endregion Form Recipe

        #region Form Report
        override public void CreateFormReport()
        {
            try
            {
                m_formReport = new ProductFormReport();
                //base.m_formReport = m_formReport;

                //m_formReport.productShareVariables = m_ProductShareVariables;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void SetFormReport()
        {
            base.m_formReport = m_formReport;
        }

        override public void SetFormReportVariables()
        {
            m_formReport.productShareVariables = m_ProductShareVariables;
            m_formReport.reportProcess = reportProcess;
        }

        override public void InitializeFormReport()
        {
            m_formReport.Initialize();
        }
        #endregion Form Report     

        #region Form IO
        override public void CreateFormIO()
        {
            m_formIO = new ProductFormIO();
        }

        override public void SetFormIO()
        {
            base.m_formIO = m_formIO;
        }
        override public void SetFormIOVariables()
        {
            m_formIO.shareVariables = m_ProductShareVariables;
            m_formIO.guiEvent = m_ProductGUIEvent;
            m_formIO.rtssProcess = m_ProductRTSSProcess;
        }

        //override public void InitializeFormIO()
        //{
        //}
        #endregion Form IO

        #region Form Teach Point
        override public void CreateFormTeachPoint()
        {
            m_formTeachPoint = new ProductFormTeachPoint();
        }

        override public void SetFormTeachPoint()
        {
            base.m_formTeachPoint = m_formTeachPoint;
        }

        override public void SetFormTeachPointVariables()
        {
            m_formTeachPoint.productShareVariables = m_ProductShareVariables;
            m_formTeachPoint.productProcessEvent = m_ProductProcessEvent;
            m_formTeachPoint.productRTSSProcess = m_ProductRTSSProcess;
        }

        override public void InitializeFormTeachPoint()
        {
            m_formTeachPoint.Initialize();
            m_formTeachPoint.InitializeGUI();
        }
        #endregion Form Teach Point

        #region FormManual
        override public void CreateFormManual()
        {
            try
            {
                m_formManual = new ProductFormManual();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void SetFormManual()
        {
            base.m_formManual = m_formManual;
        }

        override public void SetFormManualVariables()
        {
            m_formManual.productShareVariables = m_ProductShareVariables;
            m_formManual.productProcessEvent = m_ProductProcessEvent;
            m_formManual.productGUIEvent = m_ProductGUIEvent;
            m_formManual.productRTSSProcess = m_ProductRTSSProcess;
        }

        override public void InitializeFormManual()
        {
            m_formManual.Initialize();
        }
        #endregion FormManual

        #region Form Motion Chart
        override public void CreateFormMotionChart()
        {
            m_formMotionChart = new ProductFormMotionChart();
        }

        override public void SetFormMotionChart()
        {
            base.m_formMotionChart = m_formMotionChart;
        }

        override public void SetFormMotionChartVariables()
        {
            m_formMotionChart.shareVariables = m_ProductShareVariables;
        }

        override public void InitializeFormMotionChart()
        {
            m_formMotionChart.Initialize();
        }
        #endregion Form Motion Chart

        #region FormOption
        override public void CreateFormOption()
        {
            try
            {
                m_formOption = new ProductFormOption();
                //base.m_formOption = m_formOption;

                //m_formOption.productProcessEvent = m_ProductProcessEvent;
                //m_formOption.productShareVariables = m_ProductShareVariables;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void SetFormOption()
        {
            base.m_formOption = m_formOption;
        }

        override public void SetFormOptionVariables()
        {
            m_formOption.productProcessEvent = m_ProductProcessEvent;
            m_formOption.productShareVariables = m_ProductShareVariables;
        }

        override public void InitializeFormOption()
        {
            m_formOption.Initialize();
        }

        #endregion FormOption

        #region FormSetup
        override public void CreateFormSetup()
        {
            try
            {
                m_formSetup = new ProductFormSetup();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void SetFormSetup()
        {
            base.m_formSetup = m_formSetup;
        }

        override public void SetFormSetupVariables()
        {
            m_formSetup.productProcessEvent = m_ProductProcessEvent;
            m_formSetup.productShareVariables = m_ProductShareVariables;
            m_formSetup.productRTSSProcess = m_ProductRTSSProcess;
        }

        override public void InitializeFormSetup()
        {
            m_formSetup.Initialize();
        }
        #endregion FormSetup

        #region FormConfiguration
        override public void CreateFormConfiguration()
        {
            try
            {
                m_formConfiguration = new ProductFormConfiguration();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        override public void SetFormConfiguration()
        {
            base.m_formConfiguration = m_formConfiguration;
        }

        override public void SetFormConfigurationVariables()
        {
            m_formConfiguration.productShareVariables = m_ProductShareVariables;
            m_formConfiguration.productProcessEvent = m_ProductProcessEvent;
        }

        override public void InitializeFormConfiguration()
        {
            m_formConfiguration.Initialize();
        }
        #endregion FormConfiguration

        public override void OnLoginAsAdministrator()
        {            
            base.OnLoginAsAdministrator();
            //buttonManual.Visible = false;
        }
        public override void OnLoginAsVendor()
        {
            base.OnLoginAsVendor();
            //buttonManual.Visible = false;
        }
        override public void UpdateGUIInBackgroundworker()
        {

            if (m_ProductProcessEvent.PCS_GUI_LaunchVisionSoftware.WaitOne(0))
            {
                if (m_ProductShareVariables.productOptionSettings.EnableLaunchVisionSoftware == true)
                {
                    if (System.IO.Directory.Exists("Z:") == false)
                    {
                        MessageBox.Show("Vision Directory Not Connected");
                    }
                    if (System.IO.File.Exists("Z:\\EnVision.exe") == false)
                    {
                        MessageBox.Show("Vision software not exist");
                    }
                    System.Diagnostics.ProcessStartInfo pcsStartInfo = new ProcessStartInfo("Z:\\EnVision.exe");
                    string a = pcsStartInfo.FileName;
                    pcsStartInfo.UseShellExecute = true;
                    pcsStartInfo.Domain = "VISION-PC";
                    Process.Start(pcsStartInfo);
                }
            }
            if (m_ProductProcessEvent.PCS_PCS_Start_Motion_Controller.WaitOne(0))
            {
                buttonTeachPoint.Enabled = false;
                buttonManual.Enabled = false;
            }
            if (m_ProductProcessEvent.PCS_GUI_GET_MACHINE_VERSION.WaitOne(0))
            {
                m_ProductShareVariables.MachineVersion = ProductionAssemblyVersion1;
            }
            if (m_ProductProcessEvent.PCS_PCS_Motion_Controller_Ready.WaitOne(0))
            {
                m_ProductProcessEvent.PCS_PCS_Start_Motion_Controller.Reset();
                if (m_formJob == null)
                {
                    buttonTeachPoint.Enabled = true;
                    buttonManual.Enabled = true;
                }
            }
            
        }
        override public void OnMainFormExit()
        {
            m_ProductProcessEvent.GUI_PCS_Force_Generate_Last_Data.Reset();
            bool StartExitForm = true;
            HiPerfTimer ExitSoftwareTimer = new HiPerfTimer();
            try
            {
                base.OnMainFormExit();
                //if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_UNIT_PLACED_ON_OUTPUT_FRAME") == true)
                //{
                //m_ProductProcessEvent.PCS_PCS_Send_Vision_EndTile.Set();
                m_ProductProcessEvent.PCS_PCS_Send_Vision_EndLot.Set();
                //    ExitSoftwareTimer.Start();
                //    //if(!m_ProductProcessEvent.PCS_GUI_Writing_File_Start.WaitOne(0))
                //    if (m_ProductProcessEvent.PCS_GUI_Writing_File_Start.WaitOne(0))
                //    {
                //        m_ProductProcessEvent.PCS_GUI_Writing_File_Start.Reset(); 
                //    }
                //    else
                //    {
                //        m_ProductProcessEvent.GUI_PCS_Force_Generate_Last_Data.Set();
                //    }
                //    while (StartExitForm == true)
                //    {
                //        ExitSoftwareTimer.Elapse();
                //        if (m_ProductProcessEvent.PCS_GUI_Last_Data_Generated.WaitOne(0))
                //        {
                //            if (m_ProductProcessEvent.PCS_GUI_Copy_Task_Complete.WaitOne(0))
                //            {
                //                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} : Purge Half Way Data Completed in" + (ExitSoftwareTimer.ElapseMiliSecond() / 1000).ToString() + "s");
                //                StartExitForm = false;
                //            }
                //            else if (m_ProductProcessEvent.PCS_GUI_Error_Copy_Task.WaitOne(0))
                //            {
                //                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} : Handler Copy Previous Lot Image Not Complete");
                //                StartExitForm = false;
                //            }
                //        }
                //        else if (ExitSoftwareTimer.ElapseMiliSecond() >= 60000)
                //        {
                //            Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} : Purge Half Way Data Timeout in {(ExitSoftwareTimer.ElapseMiliSecond() / 1000).ToString()}");
                //            StartExitForm = false;
                //        }
                //        Thread.Sleep(1);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ProductFormMainInterface
            // 
            this.ClientSize = new System.Drawing.Size(1898, 1029);
            this.Name = "ProductFormMainInterface";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        override public void Initialize()
        {
            try
            {
                base.Initialize();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
    }
}
