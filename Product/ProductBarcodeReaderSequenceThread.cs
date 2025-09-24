using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using Common;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

namespace Product
{
    public class ProductBarcodeReaderSequenceThread
    {
        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern bool GetShareMemoryEvent([MarshalAs(UnmanagedType.LPStr)]string eventName);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryEvent([MarshalAs(UnmanagedType.LPStr)]string eventName, bool state);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern int GetShareMemoryProductionLong([MarshalAs(UnmanagedType.LPStr)]string parameterName);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryProductionLong([MarshalAs(UnmanagedType.LPStr)]string parameterName, int parameterValue);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern int GetShareMemoryProductionInt([MarshalAs(UnmanagedType.LPStr)]string parameterName);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern bool GetShareMemoryCustomizeBool([MarshalAs(UnmanagedType.LPStr)]string parameterName);

        [DllImport("RtxLibrary.dll", CharSet = CharSet.Auto)]
        public static extern void SetShareMemoryCustomizeBool([MarshalAs(UnmanagedType.LPStr)]string customizeName, bool enable);

        public bool m_bRunThread = true;
        public bool m_bAbortThread = false;
        public static string m_strBarcodeScannerResponse = "";
        private int m_nError = 0;

        private ProductShareVariables m_ProductShareVariables;
        private ProductProcessEvent m_ProductProcessEvent;
        private ProductRTSSProcess m_ProductRTSSProcess;

        public ProductShareVariables productShareVariables
        {
            set
            {
                m_ProductShareVariables = value;
            }
        }

        public ProductProcessEvent productProcessEvent
        {
            set { m_ProductProcessEvent = value; }
        }

        public ProductRTSSProcess productRTSSProcess
        {
            set { m_ProductRTSSProcess = value; }
        }

        public void BarcodeReaderThread()
        {
            try
            {
                Equipment.BarcodeReaderFactory m_barcodeReaderFactory = new Equipment.BarcodeReaderFactory();
                bool bTriggerRead = false;
                int nBarcode = 0;
                int nRetry = 0;
                HiPerfTimer timerReadBarcode = new HiPerfTimer();

                Equipment.BarcodeReader barcodeReader1;
                Equipment.BarcodeReader barcodeReader2;
                Equipment.BarcodeReader barcodeReader3;
                Equipment.BarcodeReader barcodeReader4;

                while (m_bRunThread && m_ProductShareVariables.StateMain <= Machine.StateControl.InitializeDoneState)
                {
                    Thread.Sleep(1);
                }
                barcodeReader1 = m_barcodeReaderFactory.Create("Cognex");
                barcodeReader2 = m_barcodeReaderFactory.Create("Zebex");
                barcodeReader3 = m_barcodeReaderFactory.Create("Keyence");
                barcodeReader4 = m_barcodeReaderFactory.Create("Vision");

                if (m_ProductShareVariables.productConfigurationSettings.EnableCognexBarcodeReader == true)
                {
                    //ShareVariables.barcodeReader = new BarcodeReaderCognex();
                    //barcodeReader1.Initialize(true);
                    barcodeReader1.Initialize(true, "192.168.5.2");
                    m_ProductShareVariables.barcodeReader = barcodeReader1;
                    m_ProductShareVariables.barcodeReader.Connect("192.168.5.2");
                }
                if (m_ProductShareVariables.productConfigurationSettings.EnableZebexScanner == true)
                {
                    //ShareVariables.barcodeReader = new BarcodeReaderZebex();
                    barcodeReader2.Initialize(true);
                    m_ProductShareVariables.barcodeReader = barcodeReader2;
                    m_ProductShareVariables.barcodeReader.Connect();
                }
                #region BarcodeReaderKeyence
                if (m_ProductShareVariables.productConfigurationSettings.EnableKeyenceBarcodeReader)
                {
                    //ShareVariables.barcodeReader = new BarcodeReaderKeyence();
                    barcodeReader3.ConfigureBarcodeReader(0,
                        m_ProductShareVariables.productConfigurationSettings.KeyenceBarcodeReaderCommunicationInterface);
                    barcodeReader3.Initialize(true);
                    m_ProductShareVariables.barcodeReader = barcodeReader3;
                    m_ProductShareVariables.barcodeReader.ConfigureBarcodeReader(3, 0);//new add on enable trigger readby io
                    m_ProductShareVariables.barcodeReader.Initialize(true, "192.168.5.2");//new add on
                    m_ProductShareVariables.barcodeReader.Connect();
                }
                if (m_ProductShareVariables.Barcode2DMode == 2)
                {
                    barcodeReader4.barcodeDevice = m_ProductShareVariables.barcodeDevice;
                    barcodeReader4.Initialize(true);
                    m_ProductShareVariables.barcodeReader = barcodeReader4;
                    m_ProductShareVariables.barcodeReader.Connect();
                }
                #endregion
                //ShareVariables.barcodeReader = new Cognex_Scanner_Connection();
                //m_ProductProcessEvent.GUI_PCS_ConnectBarcodeReader.Set();

                while (m_bRunThread)
                {
                    if (m_ProductProcessEvent.GUI_PCS_ConnectBarcodeReader.WaitOne(0))
                    {
                        if (m_ProductShareVariables.barcodeReader.IsDisconnected())
                        {
                            m_ProductShareVariables.barcodeReader.Disconnect();
                        }
                        if (m_ProductShareVariables.productConfigurationSettings.EnableBarcodeReader)
                        {
                            if (m_ProductShareVariables.productConfigurationSettings.EnableKeyenceBarcodeReader)
                            {
                                m_ProductShareVariables.barcodeReader.Connect();
                            }
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_UpdateRecipe.WaitOne(0)) // detect tile or frame 
                    {
                        if (m_ProductShareVariables.productConfigurationSettings.EnableZebexScanner == true)
                        {
                            barcodeReader2.Initialize(true);
                            m_ProductShareVariables.barcodeReader = barcodeReader2;
                        }
                        //string fileName = m_ProductShareVariables.productRecipeInputSettings.BarcodeRecipe + ".PTC";
                        //string sourcePath = @"D:\Estek\Recipe\Barcode\";
                        //string targetPath = "ftp://192.168.5.2/CONFIG/";

                        //string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                        //string destFile = System.IO.Path.Combine(targetPath, fileName);

                        //System.IO.File.Copy(sourceFile, destFile, true);

                        //if (System.IO.File.Exists(targetPath + "CONFIG1.PTC") == true)
                        //{
                        //    System.IO.File.Delete(targetPath + "CONFIG1.PTC");
                        //}
                        //System.IO.File.Move(m_ProductShareVariables.productRecipeInputSettings.BarcodeRecipe + ".PTC", "CONFIG1.PTC");
                        MoveBarcodeRecipeToBarcodeReader();
                        Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Send barcode configure(1,1)");
                        if (m_ProductShareVariables.barcodeReader.ConfigureBarcodeReader(1, 0) != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} {1} during update recipe for barcode reader.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.GetErrorMsg(m_ProductShareVariables.barcodeReader.GetLastErrorCode())));
                            m_ProductShareVariables.barcodeReader.ClearError();
                        }
                        //                  if (m_ProductShareVariables.productRecipeInputSettings.InputTrayType == "6 Inches Frame" || m_ProductShareVariables.productRecipeInputSettings.InputTrayType == "8 Inches Frame")
                        //                  {
                        //                      //m_ProductShareVariables.barcodeReader = barcodeReader1;
                        //                      //if (m_ProductShareVariables.barcodeReader.ConfigureBarcodeReader(1, m_ProductShareVariables.productRecipeInputSettings.BarcodeReaderKeyence_Recipe) != 0)
                        //                      //{
                        //                      //    Machine.DebugLogger.WriteLog(string.Format("{0} {1} during update recipe for barcode reader.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.GetErrorMsg(m_ProductShareVariables.barcodeReader.GetLastErrorCode())));
                        //                      //    m_ProductShareVariables.barcodeReader.ClearError();
                        //                      //}
                        //                      if (m_ProductShareVariables.Barcode2DMode == 1)
                        //                      {
                        //                          Equipment.BarcodeReader newBarcodeReader = m_barcodeReaderFactory.Create("Cognex");
                        //                          m_ProductShareVariables.barcodeReader = newBarcodeReader;
                        //                          m_ProductShareVariables.barcodeReader = barcodeReader1;
                        //                          if (m_ProductShareVariables.barcodeReader.IsDisconnected())
                        //                          {
                        //                              if (m_ProductShareVariables.barcodeReader.Connect("192.168.5.2") != 0)
                        //                              {
                        //                                  Machine.DebugLogger.WriteLog(string.Format("{0} {1} during connect barcode reader.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.GetErrorMsg(m_ProductShareVariables.barcodeReader.GetLastErrorCode())));
                        //                                  m_ProductShareVariables.barcodeReader.ClearError();
                        //                              }
                        //                          }
                        //                      }
                        //                      //else
                        //                      //{
                        //                      //    Equipment.BarcodeReader newBarcodeReader = m_barcodeReaderFactory.Create("Vision");
                        //                      //    m_ProductShareVariables.barcodeReader = newBarcodeReader;
                        //                      //    newBarcodeReader.barcodeDevice = m_ProductShareVariables.barcodeDevice;
                        //                      //    m_ProductShareVariables.barcodeReader = barcodeReader4;
                        //                      //    if (m_ProductShareVariables.barcodeReader.IsDisconnected())
                        //                      //    {
                        //                      //        if (m_ProductShareVariables.barcodeReader.Connect() != 0)
                        //                      //        {
                        //                      //            Machine.DebugLogger.WriteLog(string.Format("{0} {1} during connect barcode reader.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.GetErrorMsg(m_ProductShareVariables.barcodeReader.GetLastErrorCode())));
                        //                      //            m_ProductShareVariables.barcodeReader.ClearError();
                        //                      //        }
                        //                      //    }
                        //                      //}


                        //                  }
                        //if (m_ProductShareVariables.productRecipeInputSettings.InputTrayType == "Frame")
                        //{
                        //    //m_ProductShareVariables.barcodeReader = barcodeReader3;
                        //    //if (m_ProductShareVariables.barcodeReader.ConfigureBarcodeReader(1, m_ProductShareVariables.productRecipeInputSettings.BarcodeReaderKeyence_Recipe) != 0)
                        //    //{
                        //    //    Machine.DebugLogger.WriteLog(string.Format("{0} {1} during update recipe for barcode reader.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.GetErrorMsg(m_ProductShareVariables.barcodeReader.GetLastErrorCode())));
                        //    //    m_ProductShareVariables.barcodeReader.ClearError();
                        //    //}

                        //    Equipment.BarcodeReader newBarcodeReader = m_barcodeReaderFactory.Create("Keyence");
                        //    m_ProductShareVariables.barcodeReader = newBarcodeReader;
                        //    m_ProductShareVariables.barcodeReader = barcodeReader3;
                        //    if (m_ProductShareVariables.barcodeReader.IsDisconnected())
                        //    {
                        //        if (m_ProductShareVariables.barcodeReader.Connect() != 0)
                        //        {
                        //            Machine.DebugLogger.WriteLog(string.Format("{0} {1} during connect barcode reader.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.GetErrorMsg(m_ProductShareVariables.barcodeReader.GetLastErrorCode())));
                        //            m_ProductShareVariables.barcodeReader.ClearError();
                        //        }
                        //    }
                        //    if (m_ProductShareVariables.barcodeReader.ConfigureBarcodeReader(1, m_ProductShareVariables.productRecipeInputSettings.BarcodeReaderKeyence_Recipe) != 0)
                        //    {
                        //        Machine.DebugLogger.WriteLog(string.Format("{0} {1} during update recipe for barcode reader.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.GetErrorMsg(m_ProductShareVariables.barcodeReader.GetLastErrorCode())));
                        //        m_ProductShareVariables.barcodeReader.ClearError();
                        //    }
                        //}
                    }
                    if (m_ProductProcessEvent.GUI_PCS_TriggerBarcodeReader.WaitOne(0) || m_ProductRTSSProcess.GetEvent("RMAIN_GTHD_TRIGGER_BARCODE") ==true)
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_GTHD_TRIGGER_BARCODE", false);
                        try
                        {
                            if (m_ProductShareVariables.productConfigurationSettings.EnableBarcodeReader == true)
                            {
                                m_nError = 0;
                                if (m_ProductShareVariables.barcodeReader.IsDisconnected())
                                {
                                    Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), $"Error Code: {m_ProductShareVariables.barcodeReader.m_nErrorCode}."));
                                    Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Barcode reader is disconnected and connect again"));

                                    if (m_ProductShareVariables.productConfigurationSettings.EnableKeyenceBarcodeReader == true)
                                    {
                                        m_ProductShareVariables.barcodeReader.Connect();
                                    }
                                    else
                                        m_nError = m_ProductShareVariables.barcodeReader.Connect();
                                }
                                if (m_nError == 0)
                                {
                                    if (m_ProductShareVariables.barcodeReader.ReadID() != 0)
                                    //if (nError == 0)// || !m_ProductShareVariables.barcodeReader.m_strErrorMsg.Contains("no data"))
                                    {
                                        Machine.SequenceControl.SetAlarm(7001);
                                        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.GetErrorMsg(m_ProductShareVariables.barcodeReader.GetLastErrorCode())));
                                        m_ProductShareVariables.barcodeReader.ClearError();
                                       // m_ProductProcessEvent.PCS_GUI_ShowBarcodeRetry.Set();
                                    }
                                    else
                                    {
                                        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Input Barcode Trigger to read ID"));
                                        if (m_ProductShareVariables.barcodeReader.m_strErrorMsg.Contains("no data"))
                                        {
                                            Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} " +
                                                $"{m_ProductShareVariables.barcodeReader.m_strErrorMsg}.");

                                            m_ProductShareVariables.barcodeReader.ClearError();
                                            //m_ProductShareVariables.barcodeReader.m_strErrorMsg = "";
                                            m_ProductProcessEvent.GUI_PCS_TriggerBarcodeReader.Set();
                                        }
                                    }
                                }
                                else
                                {
                                    Machine.SequenceControl.SetAlarm(7002);
                                    Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.GetErrorMsg(m_ProductShareVariables.barcodeReader.GetLastErrorCode())));
                                    m_ProductShareVariables.barcodeReader.ClearError();
                                    //m_ProductProcessEvent.PCS_GUI_ShowBarcodeRetry.Set();
                                }
                            }
                            timerReadBarcode = new HiPerfTimer();
                            timerReadBarcode.Reset();
                            timerReadBarcode.Start();
                            bTriggerRead = true;
                        }
                        catch (Exception ex)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                        }
                    }
                    if (m_ProductShareVariables.barcodeReader != null)
                    {
                        try
                        {
                            //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} BarcodeReader.IsReadDone() = {m_ProductShareVariables.barcodeReader.IsReadDone()}.");
                            if (m_ProductShareVariables.barcodeReader.BarcodeID != null && m_ProductShareVariables.barcodeReader.IsReadDone())
                            {
                                m_strBarcodeScannerResponse = m_strBarcodeScannerResponse + "Scan Data =" + m_ProductShareVariables.barcodeReader.BarcodeID;
                                m_ProductShareVariables.imgLastReadBarcode = m_ProductShareVariables.barcodeReader.GetLastReadImage();
                                if (m_ProductShareVariables.barcodeReader.BarcodeID.Contains("\r") == true && m_ProductShareVariables.barcodeReader.BarcodeID.Contains("ERROR")==false)
                                {
                                    int subindex = 0;
                                    subindex = m_ProductShareVariables.barcodeReader.BarcodeID.IndexOf("\r");
                                    m_ProductShareVariables.OutputTrayIDFromBarcode = m_ProductShareVariables.barcodeReader.BarcodeID.Substring(0,subindex);
                                    //Machine.EventLogger.WriteLog(string.Format("{0} Barcode :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.BarcodeID));
                                    //Machine.DebugLogger.WriteLog(string.Format("{0} Barcode read :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.BarcodeID));
                                    Machine.DebugLogger.WriteLog(string.Format("{0} Barcode read Output Tray ID :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.OutputTrayIDFromBarcode));
                                    //int nBarcodeCount = 0;// = m_ProductRTSSProcess.GetProductionInt("nBarcodeTriggerCount");
                                    //string BarcodeIDTemp = "";
                                    //BarcodeIDTemp = m_ProductShareVariables.barcodeReader.BarcodeID;
                                    //Retry:
                                    //if (BarcodeIDTemp.Contains("OK,BLOAD"))
                                    //{
                                    //    BarcodeIDTemp = BarcodeIDTemp.Replace("OK,BLOAD\r", "");
                                    //    goto Retry;
                                    //}
                                    //if (BarcodeIDTemp.Contains("ER,BLOAD,12"))
                                    //{
                                    //    BarcodeIDTemp = BarcodeIDTemp.Replace("ER,BLOAD,12\r", "");
                                    //    goto Retry;
                                    //}
                                    //string[] array = Regex.Split(BarcodeIDTemp, "\r");
                                    //int nPickupHead = 0;
                                    //if (array.Length > 0)
                                    //{
                                    //    if (array.Length < nBarcodeCount)
                                    //    {
                                    //        m_ProductProcessEvent.GUI_PCS_TriggerBarcodeReader.Set();
                                    //        //break;
                                    //    }
                                    //    for (int i = 0; i < nBarcodeCount; i++)
                                    //    {
                                    //        Machine.EventLogger.WriteLog(string.Format("{0} Barcode :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), array[i]));
                                    //        //Machine.DebugLogger.WriteLog(string.Format("{0} Barcode end with slash r slash n :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), array[i]));

                                    //        for (int j = nPickupHead; j < 6; j++)
                                    //        {
                                    //            if (m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "UnitPresent") == 1)
                                    //            {
                                    //                nPickupHead = j + 1;
                                    //                bool bolReturn = false;
                                    //                if (m_ProductShareVariables.lstBarcode.Count < 0)
                                    //                {
                                    //                    m_ProductShareVariables.lstBarcode.Add(new TempBarcodeAdd() { Row = m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputRow"), Col = m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputColumn"), UnitID = array[i] });
                                    //                    Machine.DebugLogger.WriteLog(string.Format("{0} Barcode: add m_ProductShareVariables.lstBarcode.Count :{1},{2},{3}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), array[i], m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputRow"), m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputColumn")));
                                    //                }
                                    //                else
                                    //                {
                                    //                    for (int k = 0; k < m_ProductShareVariables.lstBarcode.Count; k++)
                                    //                    {
                                    //                        if (m_ProductShareVariables.lstBarcode[k].Row == m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputRow")
                                    //                            && m_ProductShareVariables.lstBarcode[k].Col == m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputColumn"))
                                    //                        {
                                    //                            bolReturn = true;
                                    //                            m_ProductShareVariables.lstBarcode[k] = new TempBarcodeAdd { Row = m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputRow"), Col = m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputColumn"), UnitID = array[i] };
                                    //                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode: bolReturn = true m_ProductShareVariables.lstBarcode.Count :{1},{2},{3}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), array[i], m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputRow"), m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputColumn")));
                                    //                            break;
                                    //                        }
                                    //                        bolReturn = false;
                                    //                    }
                                    //                    if (bolReturn == false)
                                    //                    {
                                    //                        m_ProductShareVariables.lstBarcode.Add(new TempBarcodeAdd() { Row = m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputRow"), Col = m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputColumn"), UnitID = array[i] });
                                    //                        Machine.DebugLogger.WriteLog(string.Format("{0} Barcode: bolReturn = false m_ProductShareVariables.lstBarcode.Count :{1},{2},{3}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), array[i], m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputRow"), m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", j, "InputColumn")));
                                    //                    }
                                    //                }
                                    //                m_ProductShareVariables.ArrayCurrentBarcodeID[i] = array[i];
                                    //                break;
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    SetShareMemoryEvent("RTHD_GMAIN_GET_BARCODE_DONE", true);

                                    //m_ProductShareVariables.ArrayCurrentBarcodeID[nBarcode - 1] = m_ProductShareVariables.strCurrentBarcodeID;
                                    //Machine.EventLogger.WriteLog(string.Format("{0} Barcode 1 :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.ArrayCurrentBarcodeID[0]));
                                    //Machine.EventLogger.WriteLog(string.Format("{0} Barcode 2 :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.ArrayCurrentBarcodeID[1]));
                                    //Machine.EventLogger.WriteLog(string.Format("{0} Barcode 3 :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.ArrayCurrentBarcodeID[2]));
                                    //Machine.EventLogger.WriteLog(string.Format("{0} Barcode 4 :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.ArrayCurrentBarcodeID[3]));
                                    //Machine.EventLogger.WriteLog(string.Format("{0} Barcode 5 :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.ArrayCurrentBarcodeID[4]));
                                    //Machine.EventLogger.WriteLog(string.Format("{0} Barcode 6 :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.ArrayCurrentBarcodeID[5]));
                                    //Machine.EventLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strBarcodeScannerResponse));
                                    //if (ShareVariables.strCurrentTileID.IndexOf("-", 0, 1) == 0 || ShareVariables.strCurrentTileID.IndexOf("_", 0, 1) == 0)
                                    //{
                                    //    ShareVariables.strCurrentTileID = ShareVariables.strCurrentTileID.Remove(0, 1);
                                    //}
                                    //m_ProductProcessEvent.PCS_PCS_VerifyBarcodeDone.Reset();
                                    //m_ProductProcessEvent.PCS_PCS_StartVerifyBarcode.Set();
                                    m_ProductShareVariables.barcodeReader.StopRead();
                                    //Machine.DebugLogger.WriteLog(string.Format("{0} Barcode end with slash r slash n :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.BarcodeID));
                                }
                                else if (m_ProductShareVariables.barcodeReader.BarcodeID == "")
                                {
                                    {
                                        //m_ProductProcessEvent.PCS_PCS_VerifyBarcodeDone.Reset();
                                        //m_ProductProcessEvent.PCS_PCS_StartVerifyBarcode.Set();
                                        m_ProductShareVariables.barcodeReader.StopRead();
                                        m_ProductShareVariables.OutputTrayIDFromBarcode = (m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo")+1).ToString();
                                        //ShareVariables.barcodeReader.Trigger("TRIGGER OFF");
                                        //ShareVariables.barcodeReader.m_nStatus = 0;
                                        SetShareMemoryEvent("RTHD_GMAIN_GET_BARCODE_DONE", true);
                                        Machine.DebugLogger.WriteLog(string.Format("{0} Barcode empty :{1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductShareVariables.barcodeReader.BarcodeID));
                                    }
                                }
                                else
                                {
                                    //m_ProductShareVariables.strRawBarcodeID = m_ProductShareVariables.barcodeReader.BarcodeID;
                                    //m_ProductShareVariables.strCurrentBarcodeID = GetExtractedBarcode(m_ProductShareVariables.strRawBarcodeID);

                                    //m_ProductProcessEvent.PCS_PCS_VerifyBarcodeDone.Reset();
                                    if (nRetry <= 2)
                                    {
                                        m_ProductProcessEvent.GUI_PCS_TriggerBarcodeReader.Set();
                                        m_ProductShareVariables.barcodeReader.StopRead();
                                        Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} m_ProductShareVariables.barcodeReader.StopRead();.");
                                        nRetry++;
                                    }
                                    else
                                    {
                                        m_ProductShareVariables.barcodeReader.StopRead();
                                        Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} m_ProductShareVariables.barcodeReader.StopRead();.");
                                        m_ProductShareVariables.OutputTrayIDFromBarcode = (m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo")+1).ToString();
                                        SetShareMemoryEvent("RTHD_GMAIN_GET_BARCODE_FAIL", true);
                                        nRetry = 0;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                        }
                    }
                    if (bTriggerRead == true && m_ProductShareVariables.productConfigurationSettings.EnableBarcodeReader == false)
                    {
                        bTriggerRead = false;
                        m_ProductShareVariables.strCurrentBarcodeID = "";
                        m_ProductProcessEvent.PCS_PCS_VerifyBarcodeDone.Reset();
                        m_ProductProcessEvent.PCS_PCS_StartVerifyBarcode.Set();
                        Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} bTriggerRead == true and m_ProductShareVariables.productConfigurationSettings.EnableBarcodeReader == false.");
                    }
                    Thread.Sleep(1);
                }
                if (m_ProductShareVariables.barcodeReader != null)
                {
                    m_ProductShareVariables.barcodeReader.Disconnect();
                    Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} m_ProductShareVariables.barcodeReader.Disconnect()");
                }
                m_bAbortThread = true;
            }
            catch (Exception ex)
            {
                m_bAbortThread = true;
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }

        private static string GetExtractedBarcode(string rawBarcode)
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

        void MoveBarcodeRecipeToBarcodeReader()
        {
            _InputParameter.Username = "admin";
            _InputParameter.Password = "admin";
            _InputParameter.Server = "ftp://192.168.5.2/CONFIG";
            _InputParameter.FileName = m_ProductShareVariables.productRecipeInputSettings.BarcodeRecipe + ".PTC"; 
            _InputParameter.FullName = @"D:\Estek\Recipe\Barcode\" + m_ProductShareVariables.productRecipeInputSettings.BarcodeRecipe + ".PTC";

            string filename = _InputParameter.FileName;//((FtpSetting)e.Argument).FileName;
            string fullname = _InputParameter.FullName;//((FtpSetting)e.Argument).FullName;
            string userName = _InputParameter.Username;//((FtpSetting)e.Argument).Username;
            string password = _InputParameter.Password;//((FtpSetting)e.Argument).Password;
            string server = _InputParameter.Server;//((FtpSetting)e.Argument).Server;

            string targetPath = @"D:\Temp Barcode";
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            if (File.Exists(targetPath + "/CONFIG1.PTC"))
            {
                File.Delete(targetPath + "/CONFIG1.PTC");
            }
            string sourceFile = System.IO.Path.Combine(fullname);
            string destFile = System.IO.Path.Combine(targetPath, filename);

            System.IO.File.Copy(sourceFile, destFile, true);

            System.IO.FileInfo fi = new System.IO.FileInfo(destFile);
            if (fi.Exists)
            {
                // Move file with a new name. Hence renamed.  
                fi.MoveTo(targetPath + "/CONFIG1.PTC");
                // Console.WriteLine("File Renamed.");
            }
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", server, "CONFIG1.PTC")));
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(userName, password);
            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                   
                }
            }
            catch (Exception ex)
            {

            }
            fullname = targetPath + @"\CONFIG1.PTC";
            request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", server, "CONFIG1.PTC")));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(userName, password);
            Stream ftpStream = request.GetRequestStream();
            FileStream fs = File.OpenRead(fullname);
            byte[] buffer = new byte[1024];
            double total = (double)fs.Length;
            int byteRead = 0;
            double read = 0;

            do
            {
                    byteRead = fs.Read(buffer, 0, 1024);
                    ftpStream.Write(buffer, 0, byteRead);
                    read += (double)byteRead;
                    double percentage = read / total * 100;
            }
            while (byteRead != 0);

            fs.Close();
            ftpStream.Close();
            if (System.IO.File.Exists(fullname))
            {
                System.IO.File.Delete(fullname);
            }
        }

        struct FtpSetting
        {
            public string Server { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string FileName { get; set; }
            public string FullName { get; set; }

        }

        FtpSetting _InputParameter;
    }
}
