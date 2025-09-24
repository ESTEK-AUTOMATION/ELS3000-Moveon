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
    public partial class ProductFormConversion : Form
    {
        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryEvent([MarshalAs(UnmanagedType.LPStr)]string eventName, bool enable);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern bool GetShareMemoryEvent([MarshalAs(UnmanagedType.LPStr)]string eventName);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryGeneralInt([MarshalAs(UnmanagedType.LPStr)]string generalName, int value);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetShareMemoryGeneralString([MarshalAs(UnmanagedType.LPStr)]string generalName);

        public string m_strmode = "Conversion Form";
        BackgroundWorker m_bgwUserInterface;
        
        public ProductFormConversion()
        {
            InitializeComponent();
            Initialize();
            //SetShareMemoryEvent("GMNL_RMNL_MANUAL_MODE", true);
        }

        #region Form Event
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

        private void buttonDone_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Done at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
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
        private void buttonTapeAndReelIndexMotorOn_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Tape And Reel Index Motor On at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (GetShareMemoryEvent("JobPause") == false)
                {
                    return;
                }
                SetShareMemoryEvent("StartTNRIndexMotorOn", true);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonTapeAndReelIndexMotorOff_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Tape And Reel Index Motor Off at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (GetShareMemoryEvent("JobPause") == false)
                {
                    return;
                }
                SetShareMemoryEvent("StartTNRIndexMotorOff", true);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonCutTape_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Cut Tape at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (GetShareMemoryEvent("JobPause") == false)
                {
                    return;
                }
                if (GetShareMemoryEvent("RTNR_RSEQ_CUT_TAPE_DONE") == false)
                {
                    return;
                }
                SetShareMemoryEvent("RSEQ_RTNR_CUT_TAPE_START", true);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonTapeJog_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Tape Jog Start at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (GetShareMemoryEvent("JobPause") == false)
                {
                    return;
                }
                if (GetShareMemoryEvent("GGUI_RPCS_TAPE_JOB_MODE") == true)
                {
                    return;
                }
                SetShareMemoryEvent("GGUI_RPCS_TAPE_JOB_MODE", true);
                //while (GetShareMemoryEvent("GGUI_RPCS_TAPE_JOB_MODE") == false)
                //{
                //    Application.DoEvents();
                //    Thread.Sleep(100);
                //}
                
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonTapeJobStop_Click(object sender, EventArgs e)
        {
            try
            {
                Machine.EventLogger.WriteLog(string.Format("{0} Tape Jog Stop at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                if (GetShareMemoryEvent("JobPause") == false)
                {
                    return;
                }
                //SetShareMemoryEvent("StartTNRIndexMotorOff", true);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }        

        #endregion Form Event

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

        #region Private

        private void Initialize()
        {
            try
            {
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
         
        #endregion Private       

              
    }
}
