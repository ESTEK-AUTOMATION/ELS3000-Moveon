using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using Common;

namespace Product
{
    public partial class ProductFormBarcode : Form
    {
        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryEvent([MarshalAs(UnmanagedType.LPStr)]string eventName, bool state);

        public string m_strmode = "Retry Barcode Form";
        BackgroundWorker m_bgwUserInterface;
        bool m_bExitWindowForm = false;

        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductProcessEvent m_ProductProcessEvent;// = new ProductProcessEvent();
        private ProductStateControl m_ProductStateControl;// = new ProductStateControl();
        
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
            set { m_ProductStateControl = value; }
        }
        public bool ExitForm
        {
            set
            {
                m_bExitWindowForm = value;
            }
        }
        #region Form Events

        public ProductFormBarcode()
        {
            InitializeComponent();
            //Initialize();
            textBoxBarcodeID.Focus();
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            try
            {
                #region Verify File
                if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                {
                    SetShareMemoryEvent("StartReset", true);
                    Thread.Sleep(500);
                    SetShareMemoryEvent("StartJob", true);
                }
                if (textBoxBarcodeID.Text != "")
                {
                    m_ProductShareVariables.strRawBarcodeID = textBoxBarcodeID.Text;
                    //ShareVariables.strRawBarcodeID = textBoxTileID.Text;
                    m_ProductShareVariables.strCurrentBarcodeID = GetExtractedBarcode(m_ProductShareVariables.strRawBarcodeID);
                    m_ProductProcessEvent.PCS_PCS_VerifyBarcodeDone.Reset();
                    m_ProductProcessEvent.PCS_PCS_StartVerifyBarcode.Set();
                    m_ProductProcessEvent.GUI_PCS_WaitingBarcodeConfirmation.Reset();
                    SetShareMemoryEvent("RTHD_GMAIN_IS_BARCODE_RETRY_SHOW", false);
                }
                else
                {
                    m_ProductProcessEvent.GUI_PCS_WaitingBarcodeConfirmation.Reset();
                    SetShareMemoryEvent("RTHD_GMAIN_IS_BARCODE_RETRY_SHOW", false);
                    SetShareMemoryEvent("RTHD_GMAIN_READ_BARCODE_START", true);
                }
                //if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                //{
                //    SetShareMemoryEvent("StartReset", true);
                //    Thread.Sleep(500);
                //    SetShareMemoryEvent("StartJob", true);
                //}
                #endregion
                
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

        private void buttonUnload_Click(object sender, EventArgs e)
        {
            try
            {
                SetShareMemoryEvent("RTHD_GMAIN_BARCODE_RESULT_NOT_VALID", true);
                SetShareMemoryEvent("RTHD_GMAIN_IS_BARCODE_RETRY_SHOW", false);
                if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                {
                    SetShareMemoryEvent("StartReset", true);
                    Thread.Sleep(500);
                    SetShareMemoryEvent("StartJob", true);
                }
                m_ProductProcessEvent.GUI_PCS_WaitingBarcodeConfirmation.Reset();
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

        #endregion Form Events

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
                    Thread.Sleep(1);
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
                if (m_bExitWindowForm == true)
                {
                    if (m_bgwUserInterface.WorkerSupportsCancellation == true)
                    {
                        m_bgwUserInterface.CancelAsync();
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
                Close();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        #endregion

        #region private

        private void AutoCreateFolderFile()
        {
            
        }

        private string GetExtractedBarcode(string rawBarcode)
        {
            int nError = 0;
            try
            {
                if (rawBarcode.IndexOf("-", 0, 1) == 0 || rawBarcode.IndexOf("_", 0, 1) == 0)
                {
                    return rawBarcode.Remove(0, 1);
                }
                else
                    return rawBarcode;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                nError = -1;
                return "";
            }
        }
        #endregion private

        #region public function
        virtual public void Initialize()
        {
            try
            {
                AutoCreateFolderFile();

                m_bgwUserInterface = new BackgroundWorker();
                m_bgwUserInterface.WorkerReportsProgress = true;
                m_bgwUserInterface.WorkerSupportsCancellation = true;
                m_bgwUserInterface.DoWork += new DoWorkEventHandler(bgwUserInterface_DoWork);
                m_bgwUserInterface.ProgressChanged += new ProgressChangedEventHandler(bgwUserInterface_UpdateInterface);
                m_bgwUserInterface.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwUserInterface_Complete);

                if (LoadSettings())
                {
                    UpdateGUI();
                }
                textBoxBarcodeID.Focus();
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
                return true;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public void UpdateGUI()
        {
            #region Turret Application
            #endregion
            pictureBoxLastReadImage.Image = m_ProductShareVariables.imgLastReadBarcode;
        }

        #endregion public function


    }
}
