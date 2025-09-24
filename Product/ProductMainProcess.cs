using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Common;
using System.Text.RegularExpressions;
using Machine;
using MoveonMESAPI;
//using TimerResolutionSet;
namespace Product
{
    public class ProductMainProcess : Machine.MainProcess
    {

        public Thread m_thdCommunication;
        public Thread m_thdBarcodeReader;
        public Thread m_thdBarcodeReader2;
        public Thread m_thdBarcodePrinter;
        public ProductCommunicationSequenceThread m_CommunicationSequenceThread = new ProductCommunicationSequenceThread();
        public ProductBarcodeReaderSequenceThread m_BarcodeReaderSequenceThread = new ProductBarcodeReaderSequenceThread();
        public ProductBarcodePrinter m_BarcodePrinterThread = new ProductBarcodePrinter();
        //public ProductBarcodeReader2SequenceThread m_BarcodeReader2SequenceThread = new ProductBarcodeReader2SequenceThread();

        private ProductShareVariables m_ProductShareVariables;
        private ProductProcessEvent m_ProductProcessEvent;

        private ProductStateControl m_ProductStateControl;
        private ProductRTSSProcess m_ProductRTSSProcess;

        ProductInputOutputFileFormat readInfo = new ProductInputOutputFileFormat();

        int[] nBondHeadCount = new int[24];
        int[] nPickUpHeadCount = new int[2];
        int[] nFlipperHeadCount = new int[4];
        int[] nEjectorNeedle = new int[1];
        int[] nAlignerTip = new int[1];

        int[] nLowYieldBondHeadCount = new int[2];
        int[] nLowYieldFlipperHeadCount = new int[4];
        string[] SortingBarcodeArray;
        string BarcodeSplitData;
        bool bolSetPnPLookUpTable = true;
        List<LookUpTableOffsetData> PnP1LookUpTable;
        List<LookUpTableOffsetData> PnP2LookUpTable;
        public ProductOptionSettings m_ProductOptionSettings = new ProductOptionSettings();
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

        public ProductInputOutputFileFormat productInputOutputFileFormat
        {
            set
            {
                readInfo = value;
            }
        }

        override public int InitializeThread()
        {
            int nError = 0;
            try
            {
                readInfo.productShareVariables = m_ProductShareVariables;
                m_ProductStateControl.productShareVariables = m_ProductShareVariables;
                m_ProductStateControl.productRTSSProcess = m_ProductRTSSProcess;
                //m_ProductStateControl.productreportProcess = m_ReportProcess;
                m_thdStateControl = new Thread(m_ProductStateControl.StateThread);
                m_thdStateControl.Start(0);
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int InitializeThreadAfterStateInitializeDone()
        {
            int nError = 0;
            try
            {
                base.InitializeThreadAfterStateInitializeDone();

                //TimerResolutionFunction.timeBeginPeriod(1);

                m_CommunicationSequenceThread.productShareVariables = m_ProductShareVariables;
                m_CommunicationSequenceThread.productProcessEvent = m_ProductProcessEvent;
                m_CommunicationSequenceThread.productRTSSProcess = m_ProductRTSSProcess;                
                m_thdCommunication = new Thread(m_CommunicationSequenceThread.CommunicationThread);

                m_thdCommunication.Priority = ThreadPriority.Highest;
                m_thdCommunication.Start();

                m_BarcodeReaderSequenceThread.productShareVariables = m_ProductShareVariables;
                m_BarcodeReaderSequenceThread.productProcessEvent = m_ProductProcessEvent;
                m_BarcodeReaderSequenceThread.productRTSSProcess = m_ProductRTSSProcess;
                m_thdBarcodeReader = new Thread(m_BarcodeReaderSequenceThread.BarcodeReaderThread);
                m_thdBarcodeReader.Start();

                m_BarcodePrinterThread.productShareVariables = m_ProductShareVariables;
                m_BarcodePrinterThread.productProcessEvent = m_ProductProcessEvent;
                m_BarcodePrinterThread.productRTSSProcess = m_ProductRTSSProcess;
                m_thdBarcodePrinter = new Thread(m_BarcodePrinterThread.BarcodePrinterThread);
                m_thdBarcodePrinter.Start();
                //m_BarcodeReader2SequenceThread.productShareVariables = m_ProductShareVariables;
                //m_BarcodeReader2SequenceThread.productProcessEvent = m_ProductProcessEvent;
                //m_thdBarcodeReader2 = new Thread(m_BarcodeReader2SequenceThread.BarcodeReaderThread);
                //m_thdBarcodeReader2.Start();

                UpdatedInitialMaintenanceCount();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int OnShutdowning()
        {
            int nError = 0;
            try
            {
                m_CommunicationSequenceThread.m_bRunThread = false;
                m_BarcodeReaderSequenceThread.m_bRunThread = false;
                m_BarcodePrinterThread.m_bRunThread = false;
               // m_BarcodeReader2SequenceThread.m_bRunThread = false;
                if (m_CommunicationSequenceThread.m_Server != null)
                {
                    m_CommunicationSequenceThread.m_Server.Kill();
                    //Communication.m_Server.Disconnect();
                    //Communication.m_Server = null;
                }
                while (m_CommunicationSequenceThread.m_bAbortThread == false)
                    Thread.Sleep(1);
                while (m_BarcodeReaderSequenceThread.m_bAbortThread == false)
                    Thread.Sleep(1);
                while (m_BarcodePrinterThread.m_bAbortThread == false)
                    Thread.Sleep(1);

                //TimerResolutionFunction.timeEndPeriod(1);
                //while (m_BarcodeReader2SequenceThread.m_bAbortThread == false)
                //    Thread.Sleep(1);
                #region Secsgem
                if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
                {
                    Machine.Platform.SecsgemControl.ShutDown();
                }
                #endregion Secsgem
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int OnInitializeDone()
        {
            int nError = 0;
            try
            {
                #region Secsgem
                if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
                {
                    Machine.Platform.SecsgemControl.Initialize(m_ProductShareVariables.productOptionSettings.SecsgemMode, GetType().Assembly.GetName().Name, GetType().Assembly.GetName().Version.ToString());
                }
                #endregion Secsgem
                m_ProductProcessEvent.PCS_PCS_Start_Motion_Controller.Set();
                m_ProductProcessEvent.PCS_GUI_GET_MACHINE_VERSION.Set();
                m_ProductShareVariables.IsStateDisplayChanged = false;
                m_ProductProcessEvent.PCS_PCS_Start_Map_Drive.Set();
                //m_ProductShareVariables.IsNewLotDone = false;
                //m_ProductShareVariables.IsSendVisionInfo = false;

                base.OnInitializeDone();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int OnLoadOptionDone()
        {
            int nError = 0;
            #region Secsgem
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                Machine.Platform.SecsgemControl.SetSecsgemMode(m_ProductShareVariables.productOptionSettings.SecsgemMode);
                Machine.Platform.SecsgemControl.ControlState = m_ProductShareVariables.productOptionSettings.SecsgemMode;
            }
            #endregion
            return nError;
        }

        override public int OnUpdateSettingDone()
        {
            int nError = 0;
            //if (m_ProductProcessEvent.PCS_PCS_Need_Vision_Setting.WaitOne(0))
            //{
            m_ProductProcessEvent.PCS_PCS_Is_Sending_Vision_Setting.Set();
            
            //m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();



            //}
            //#region FrameSelection
            //m_ProductProcessEvent.PCS_GUI_CreateFrameSelection.Set();
            //#endregion
            //if (m_ProductProcessEvent.GUI_PCS_Need_NewLot.WaitOne(0))
            //{
            //    m_ProductProcessEvent.PCS_GUI_Launch_NewLot.Set();
            //    SetShareMemoryProductionInt("nCurrentInputTrayNo", 0);
            //    SetShareMemoryProductionInt("nCurrentOutputTrayNo", 0);
            //    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"));
            //    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentOutputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo"));
            //}
            return nError;
        }

        override public int OnReadBarcodeID()
        {
            int nError = 0;
            #region Secsgem
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                Machine.Platform.SecsgemControl.SetEventBarcodeID(m_ProductShareVariables.strCurrentBarcodeID);
            }
            #endregion Secsgem
            return nError;
        }

        override public int OnUnloadFrameOrTile()
        {
            int nError = 0;
            #region Secsgem
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                Machine.Platform.SecsgemControl.SetEventUnloadFrameOrTile();
            }
            #endregion Secsgem
            return nError;
        }

        override public int OnLotUnloadComplete()
        {
            int nError = 0;
            #region Secsgem
            //if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            //{
            //    string[] variableName = new string[11]
            //    {
            //        "InputLotID",
            //        "OperatorID",
            //        "TotalUnitDone",
            //        "StartTime",
            //        "EndTime",
            //        "TotalRejectedUnits",
            //        "OutputLotID",
            //        "UPH",
            //        "Throughput",
            //        "CurrentLotMTBA",
            //        "CurrentLotMTBF"
            //    };
            //    string[] variableValue = new string[11]
            //    {
            //        m_ProductShareVariables.strucInputProductInfo.LotID,
            //        m_ProductShareVariables.strucInputProductInfo.OperatorID,
            //        m_ProductRTSSProcess.GetProductionInt("nCurrentTotalUnitDone").ToString(),
            //        m_ProductShareVariables.dtProductionStartTime.ToString(),
            //        m_ProductShareVariables.dtProductionEndTime.ToString(),
            //        m_ProductRTSSProcess.GetProductionInt("nCurrentTotalRejectUnit").ToString(),
            //        m_ProductShareVariables.strucInputProductInfo.LotIDOutput,
            //        m_ProductShareVariables.UPH,
            //        m_ProductShareVariables.Throughput,
            //        m_ProductShareVariables.nCurrentLotMTBA.ToString(),
            //        m_ProductShareVariables.nCurrentLotMTBF.ToString()
            //    };
            //    Machine.Platform.SecsgemControl.SetEvent("101",variableName,variableValue);
            //}
            #endregion Secsgem
            return nError;
        }

        override public int LoadConfiguration()
        {
            try
            {
                if (Tools.IsFileExist(m_ProductShareVariables.strSystemPath, m_ProductShareVariables.strConfigurationFile.Remove(m_ProductShareVariables.strConfigurationFile.Length - m_ProductShareVariables.strXmlExtension.Length), m_ProductShareVariables.strXmlExtension))
                {
                    m_ProductShareVariables.productConfigurationSettings = Tools.Deserialize<ProductConfigurationSettings>(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strConfigurationFile);
                    //shareVariables.configurationSettings = m_ProductShareVariables.productConfigurationSettings;
                }
                else
                {
                    //stripStatusLabel.Text += "Main recipe not exist.";
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }

            return 0;
        }
        override public int LoadOption()
        {
            try
            {
                if (Tools.IsFileExist(m_ProductShareVariables.strSystemPath, m_ProductShareVariables.strOptionFile.Remove(m_ProductShareVariables.strOptionFile.Length - m_ProductShareVariables.strXmlExtension.Length), m_ProductShareVariables.strXmlExtension))
                {
                    m_ProductShareVariables.productOptionSettings = Tools.Deserialize<ProductOptionSettings>(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strOptionFile);
                }
                else
                {
                    //stripStatusLabel.Text += "Main recipe not exist.";
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }

            return 0;
        }
        override public int LoadSetting()
        {
            int nError = 0;
            if (Tools.IsFileExist(m_ProductShareVariables.strRecipeInputPath, m_ProductShareVariables.recipeMainSettings.InputRecipeName, m_ProductShareVariables.strXmlExtension))
            {
                m_ProductShareVariables.productRecipeInputSettings = Tools.Deserialize<ProductRecipeInputSettings>(m_ProductShareVariables.strRecipeInputPath + m_ProductShareVariables.recipeMainSettings.InputRecipeName + m_ProductShareVariables.strXmlExtension);
                m_ProductShareVariables.recipeInputSettings = m_ProductShareVariables.productRecipeInputSettings;
            }
            else
            {
                nError = 1;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            //if (Tools.IsFileExist(m_ProductShareVariables.strRecipeOutputPath, m_ProductShareVariables.recipeMainSettings.OutputRecipeName, m_ProductShareVariables.strXmlExtension))
            //{
            //    m_ProductShareVariables.productRecipeOutputSettings = Tools.Deserialize<ProductRecipeOutputSettings>(m_ProductShareVariables.strRecipeOutputPath + m_ProductShareVariables.recipeMainSettings.OutputRecipeName + m_ProductShareVariables.strXmlExtension);
            //    m_ProductShareVariables.recipeOutputSettings = m_ProductShareVariables.productRecipeOutputSettings;
            //}
            //else
            //{
            //    nError = 2;
            //    return nError;
            //    //stripStatusLabel.Text += "Main recipe not exist.";
            //}
            if (Tools.IsFileExist(m_ProductShareVariables.strRecipeDelayPath, m_ProductShareVariables.recipeMainSettings.DelayRecipeName, m_ProductShareVariables.strXmlExtension))
            {
                m_ProductShareVariables.productRecipeDelaySettings = Tools.Deserialize<ProductRecipeDelaySettings>(m_ProductShareVariables.strRecipeDelayPath + m_ProductShareVariables.recipeMainSettings.DelayRecipeName + m_ProductShareVariables.strXmlExtension);
                m_ProductShareVariables.recipeDelaySettings = m_ProductShareVariables.productRecipeDelaySettings;
            }
            else
            {
                nError = 3;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            if (Tools.IsFileExist(m_ProductShareVariables.strRecipeMotorPositionPath, m_ProductShareVariables.recipeMainSettings.MotorPositionRecipeName, m_ProductShareVariables.strXmlExtension))
            {
                m_ProductShareVariables.productRecipeMotorPositionSettings = Tools.Deserialize<ProductRecipeMotorPositionSettings>(m_ProductShareVariables.strRecipeMotorPositionPath + m_ProductShareVariables.recipeMainSettings.MotorPositionRecipeName + m_ProductShareVariables.strXmlExtension);
                m_ProductShareVariables.recipeMotorPositionSettings = m_ProductShareVariables.productRecipeMotorPositionSettings;
            }
            else
            {
                nError = 4;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            if (Tools.IsFileExist(m_ProductShareVariables.strRecipeOutputFilePath, m_ProductShareVariables.recipeMainSettings.OutputFileRecipeName, m_ProductShareVariables.strXmlExtension))
            {
                m_ProductShareVariables.productRecipeOutputFileSettings = Tools.Deserialize<ProductRecipeOutputFileSettings>(m_ProductShareVariables.strRecipeOutputFilePath + m_ProductShareVariables.recipeMainSettings.OutputFileRecipeName + m_ProductShareVariables.strXmlExtension);
                m_ProductShareVariables.recipeOutputFileSettings = m_ProductShareVariables.productRecipeOutputFileSettings;
            }
            else
            {
                nError = 5;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            //if (Tools.IsFileExist(m_ProductShareVariables.strRecipeInputCassettePath, m_ProductShareVariables.productRecipeMainSettings.InputCassetteRecipeName, m_ProductShareVariables.strXmlExtension))
            //{
            //    m_ProductShareVariables.productRecipeInputCassetteSettings = Tools.Deserialize<ProductRecipeCassetteSettings>(m_ProductShareVariables.strRecipeInputCassettePath + m_ProductShareVariables.productRecipeMainSettings.InputCassetteRecipeName + m_ProductShareVariables.strXmlExtension);
            //}
            //else
            //{
            //    nError = 6;
            //    return nError;
            //    //stripStatusLabel.Text += "Main recipe not exist.";
            //}
            //if (Tools.IsFileExist(m_ProductShareVariables.strRecipeOutputCassettePath, m_ProductShareVariables.productRecipeMainSettings.OutputCassetteRecipeName, m_ProductShareVariables.strXmlExtension))
            //{
            //    m_ProductShareVariables.productRecipeOutputCassetteSettings = Tools.Deserialize<ProductRecipeCassetteSettings>(m_ProductShareVariables.strRecipeOutputCassettePath + m_ProductShareVariables.productRecipeMainSettings.OutputCassetteRecipeName + m_ProductShareVariables.strXmlExtension);
            //}
            //else
            //{
            //    nError = 7;
            //    return nError;
            //    //stripStatusLabel.Text += "Main recipe not exist.";
            //}
            if (Tools.IsFileExist(m_ProductShareVariables.strRecipeVisionPath, m_ProductShareVariables.productRecipeMainSettings.VisionRecipeName, m_ProductShareVariables.strXmlExtension))
            {
                m_ProductShareVariables.productRecipeVisionSettings = Tools.Deserialize<ProductRecipeVisionSettings>(m_ProductShareVariables.strRecipeVisionPath + m_ProductShareVariables.productRecipeMainSettings.VisionRecipeName + m_ProductShareVariables.strXmlExtension);
            }
            else
            {
                nError = 8;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            if (Tools.IsFileExist(m_ProductShareVariables.strRecipeSortingPath, m_ProductShareVariables.productRecipeMainSettings.SortingRecipeName, m_ProductShareVariables.strXmlExtension))
            {
                m_ProductShareVariables.productRecipeSortingSettings = Tools.Deserialize<ProductRecipeSortingSetting>(m_ProductShareVariables.strRecipeSortingPath + m_ProductShareVariables.productRecipeMainSettings.SortingRecipeName + m_ProductShareVariables.strXmlExtension);
            }
            else
            {
                nError = 9;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            if (Tools.IsFileExist(m_ProductShareVariables.strRecipePickUpHeadPath, m_ProductShareVariables.productRecipeMainSettings.PickUpHeadRecipeName, m_ProductShareVariables.strXmlExtension))
            {
                m_ProductShareVariables.productRecipePickUpHeadSettings = Tools.Deserialize<ProductRecipePickUpHeadSeting>(m_ProductShareVariables.strRecipePickUpHeadPath + m_ProductShareVariables.productRecipeMainSettings.PickUpHeadRecipeName + m_ProductShareVariables.strXmlExtension);
            }
            else
            {
                nError = 10;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }

            return nError;
        }

        override public int UpdateSetting()
        {
            int nError = 0;
            try
            {
                nError = base.UpdateSetting();
                if (nError == 0)
                {
                    m_ProductShareVariables.outputFileOption.listDefect = m_ProductShareVariables.productRecipeOutputFileSettings.listDefect;

                    #region BarcodeReaderKeyence
                    if (m_ProductShareVariables.productConfigurationSettings.EnableBarcodeReader == true)
                    {
                        if (m_ProductShareVariables.productConfigurationSettings.EnableKeyenceBarcodeReader == true)
                        {
                            //m_ProductProcessEvent.PCS_PCS_UpdateRecipe.Set();
                            m_ProductProcessEvent.PCS_PCS_UpdateRecipe2.Set();
                        }
                    }
                    #endregion
                }
                else
                    return nError;
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public void OnRunningMainThread()
        {
            if(m_ProductRTSSProcess.GetEvent("START_PNP_Calibration") == true)
            {
                m_ProductShareVariables.PnPCalibrationInfo = new List<LookUpTableOffsetData>();
                m_ProductRTSSProcess.SetEvent("START_PNP_Calibration", false);
                m_ProductShareVariables.strucInputProductInfo.LotID = "PNP_CALIBRATION";
                m_ProductShareVariables.strCurrentBarcodeID = "PNP"+ m_ProductRTSSProcess.GetProductionInt("PickUpHeadNoForCalibration").ToString();
                m_ProductProcessEvent.PCS_PCS_Resend_Vision_Setting.Set();
            }
            if (m_ProductRTSSProcess.GetEvent("PNP_CALIBRATION_WRITE_REPORT") == true)
            {
                m_ProductRTSSProcess.SetEvent("PNP_CALIBRATION_WRITE_REPORT", false);
                if (readInfo.GeneratePNPCalibrationReport(m_ProductShareVariables.PnPCalibrationInfo, m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotID,
                         m_ProductShareVariables.strCurrentBarcodeID +  DateTime.Now.ToString("yyyyMMdd") + ".csv") == 0)
                {
                    m_ProductShareVariables.PnPCalibrationInfo.Clear();
                    m_ProductShareVariables.PnPCalibrationInfo = null;
                }
            }
            if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_CONNECT_TO_CONTROLLER_DONE") == true)
            {
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_CONNECT_TO_CONTROLLER_DONE", false);
                m_ProductProcessEvent.PCS_PCS_Motion_Controller_Ready.Set();
            }
            if (bolSetPnPLookUpTable == true)
            {
                bolSetPnPLookUpTable = false;
                PnP1LookUpTable = new List<LookUpTableOffsetData>();
                PnP2LookUpTable = new List<LookUpTableOffsetData>();
                PnP1Data(ref PnP1LookUpTable);
                PnP2Data(ref PnP2LookUpTable);
                m_ProductRTSSProcess.SetPnPDataOffsetToShareMemory(PnP1LookUpTable, PnP2LookUpTable);
            }
            //if (GetShareMemoryEvent("RTHD_GMAIN_READ_BARCODE_START") == true)
            //{
            //    try
            //    {
            //        SetShareMemoryEvent("RTHD_GMAIN_READ_BARCODE_START", false);
            //        m_ProductShareVariables.nBarcodeIDNo = 0;
            //        m_ProductShareVariables.strCurrentBarcodeID = "";
            //        for (int i = 0; i < m_ProductShareVariables.ArrayCurrentBarcodeID.Length; i++)
            //        {
            //            m_ProductShareVariables.ArrayCurrentBarcodeID[i] = "";
            //        }
            //        m_ProductProcessEvent.GUI_PCS_TriggerBarcodeReader.Set();

            //    }
            //    catch (Exception ex)
            //    {
            //        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            //    }
            //}
            if (m_ProductProcessEvent.PCS_PCS_StartVerifyBarcode.WaitOne(0))
            {
                if (m_ProductRTSSProcess.GetSettingBool("EnableOnlineMode") == true)
                {
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_BARCODE_RESULT_VALID", true);
                    m_ProductShareVariables.tileIDInfo = new Product.TileIdInfo();
                    m_ProductShareVariables.strCurrentBarcodeID = "NA";
                    m_ProductShareVariables.tileIDInfo.TileId = m_ProductShareVariables.strCurrentBarcodeID;
                   m_ProductProcessEvent.PCS_PCS_Send_Vision_TrayNo.Set();
                }
                else
                {
                    //if (ProcessEvent.GUI_PCS_WaitingBarcodeConfirmation.WaitOne(0))
                    //{
                    //    ProcessEvent.PCS_PCS_VerifyBarcodeDone.Set();
                    //    continue;
                    //}
                    if (m_ProductRTSSProcess.GetEvent("GGUI_RSEQ_DRY_RUN_MODE") == true)
                    {
                        if (m_ProductRTSSProcess.GetSettingBool("EnableVision") == false)
                            m_ProductShareVariables.strCurrentBarcodeID = "dryrun";
                        else
                        {
                            string strPreviousTileID = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("strCurrentTileID", "Unknown").ToString();
                            if (strPreviousTileID.Length >= "dryrun".Length)
                            {
                                if (strPreviousTileID.Substring(0, "dryrun".Length) == "dryrun")
                                {
                                    string test = strPreviousTileID.Substring(0, "dryrun".Length);
                                    string test1 = strPreviousTileID.Substring("dryrun".Length, strPreviousTileID.Length - "dryrun".Length);
                                    int nCurrentDryRunNo = 0;
                                    if (Int32.TryParse(strPreviousTileID.Substring("dryrun".Length, strPreviousTileID.Length - "dryrun".Length), out nCurrentDryRunNo))
                                    {
                                        nCurrentDryRunNo++;
                                    }
                                    m_ProductShareVariables.strCurrentBarcodeID = "dryrun" + nCurrentDryRunNo.ToString();
                                }
                                else
                                    m_ProductShareVariables.strCurrentBarcodeID = "dryrun";
                            }
                            else
                                m_ProductShareVariables.strCurrentBarcodeID = "dryrun";
                        }
                    }
                    //if (m_ProductShareVariables.strCurrentBarcodeID == "")// && ProcessEvent.GUI_PCS_WaitingBarcodeConfirmation.WaitOne(0))
                    //{
                    //    Machine.SequenceControl.SetAlarm(7001);
                    //    m_ProductProcessEvent.PCS_GUI_ShowBarcodeRetry.Set();
                    //    //ProcessEvent.PCS_PCS_VerifyBarcodeDone.Set();
                    //    //SetShareMemoryEvent("RTHD_GMAIN_READ_BARCODE_START", true);                                
                    //    goto EndVerifyBarcode;
                    //}
                    ProcessInputFile();
                    
                EndVerifyBarcode:;
                }
            }

            if (m_ProductProcessEvent.PCS_PCS_StartReadInputFile.WaitOne(0))
            {
                ProcessInputFile();
                //readInfo.InitializeMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductShareVariables.nInputTrayNo], (int)m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput, (int)m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColInput, 0, 1);
                m_ProductProcessEvent.PCS_GUI_Initial_Map.Set();
                if (m_ProductShareVariables.UserChooseContinueLot == false)
                {
                    if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput))//&& m_ProductShareVariables.bolFirstTimeCreateOutputFile == true)
                    {
                        for (int i = 1; i < 100; i++)
                        {
                            if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString()) == false)
                            {
                                RenameFolder(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                                    m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                                break;
                            }
                        }
                    }
                    if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput))//&& m_ProductShareVariables.bolFirstTimeCreateOutputFile == true)
                    {
                        for (int i = 1; i < 100; i++)
                        {
                            if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString()) == false)
                            {
                                RenameFolder(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                                    m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                                break;
                            }
                        }
                    }
                }
                m_ProductShareVariables.bolFirstTimeCreateOutputFile = false;
            }
            if (m_ProductProcessEvent.PCS_PCS_Set_Mapping_Result.WaitOne(0))
            {
                //if ((m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") % 2) == 1)
                {
                    for (int j = 0; j < m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo")-1].arrayUnitInfo.Length; j++)
                    {
                        if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn")
                            && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"))
                        {
                            if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputResult == "P"
                                && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SetupResult == "P"
                                && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S2Result == "P"
                                && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S1Result == "P"
                                && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWLeftResult == "P"
                                && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRightResult == "P"
                                && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWFrontResult == "P"
                                && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRearResult == "P"
                                && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S3Result == "P"
                                && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].OutputResult == "P"
                                )
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn, "P", true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputResult != "P")
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "Input";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputResult, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputResult)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputResult, qty = 1 });
                                    }
                                }
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SetupResult != "P")
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "Setup";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SetupResult, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SetupResult)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SetupResult, qty = 1 });
                                    }
                                }
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S2Result != "P")
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "S2";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S2Result, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S2Result)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S2Result, qty = 1 });
                                    }
                                }
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S1Result != "P")
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "S1";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S1Result, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S1Result)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S1Result, qty = 1 });
                                    }
                                }
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWLeftResult != "P")
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "Sidewall Left";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWLeftResult, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWLeftResult)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWLeftResult, qty = 1 });
                                    }
                                }
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRightResult != "P")
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "Sidewall Right";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRightResult, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRightResult)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRightResult, qty = 1 });
                                    }
                                }
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWFrontResult != "P")
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "Sidewall Front";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWFrontResult, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWFrontResult)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWFrontResult, qty = 1 });
                                    }
                                }
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRearResult != "P")
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "Sidewall Rear";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRearResult, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRearResult)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRearResult, qty = 1 });
                                    }
                                }
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S3Result != "P")
                            {
                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "S3";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S3Result, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S3Result)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S3Result, qty = 1 });
                                    }
                                }
                            }
                            else if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].OutputResult != "P")
                            {
                                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Output Result Not equal to P");
                                Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Row = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow}" +
                                    $"Col = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn}");
                                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} INP = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputResult}," +
                                //    $"S2 = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S2Result}" +
                                //    $"Setup = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SetupResult}" +
                                //    $"S2 = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S1Result}" +
                                //    $"SWLeft = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWLeftResult}" +
                                //    $"SWRight = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRightResult}" +
                                //    $"S3 = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].S3Result}" +
                                //    $"SWFront = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWFrontResult}" +
                                //    $"SWRear = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].SWRearResult}" +
                                //    $"Ouptut = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].OutputResult}");

                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination = "Output";
                                readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1],
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].OutputResult, true,
                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].Destination);
                                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                                {
                                    bool IsDefectiveMESExist = false;
                                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                                    {
                                        if (_defective.def_code == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].OutputResult)
                                        {
                                            _defective.qty++;
                                            IsDefectiveMESExist = true;
                                            break;
                                        }
                                    }
                                    if (IsDefectiveMESExist == false)
                                    {
                                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].OutputResult, qty = 1 });
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                if (m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") == m_ProductShareVariables.nInputTrayNo + 1)
                {
                    m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                }
                if (m_ProductProcessEvent.GUI_GUI_GET_DATA.WaitOne(0))
                {
                    m_ProductProcessEvent.GUI_GUI_GET_DATA.Reset();
                    //m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_GET_OUTPUT_STATION_DONE", true);
                }
            }
            if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_UPDATE_IN_PROGRESS_MAPPING") == true)
            {
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_UPDATE_IN_PROGRESS_MAPPING", false);
                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputRow"), m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputColumn"))))
                {
                    //int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputRow"), m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputColumn"))];
                    //    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputTrayNo") - 1],
                    //               m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputTrayNo") - 1].arrayUnitInfo[curindex].InputRow,
                    //               m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputTrayNo") - 1].arrayUnitInfo[curindex].InputColumn,
                    //               "---", true);
                    if (m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputTrayNo") == m_ProductShareVariables.nInputTrayNo + 1)
                    {
                        m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                    }
                }
            }
            if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_UPDATE_INPUT_MAPPING") == true)
            {
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_UPDATE_INPUT_MAPPING", false);
                m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
            }

            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START", false);
                for (int j = 0; j < m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo.Length; j++)
                {
                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn == m_ProductRTSSProcess.GetProductionArray("InputTableResult",0, "InputColumn")
                        && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow == m_ProductRTSSProcess.GetProductionArray("InputTableResult",0, "InputRow"))
                    {

                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult",0, "InputTrayNo") - 1],
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                   "PF", false);
                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputResult = "PF";
                        //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                        if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                        {
                            bool IsDefectiveMESExist = false;
                            foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                            {
                                if (_defective.def_code == "UP")
                                {
                                    _defective.qty++;
                                    IsDefectiveMESExist = true;
                                    break;
                                }
                            }
                            if (IsDefectiveMESExist == false)
                            {
                                m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = "UP", qty = 1 });
                            }
                        }
                        //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Update data");
                    }
                }
                if (m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") == m_ProductShareVariables.nInputTrayNo + 1)
                {
                    //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Come in update");
                    m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                }
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE", true);
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START_NOMES") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_START_NOMES", false);
                for (int j = 0; j < m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo.Length; j++)
                {
                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn == m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn")
                        && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow == m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow"))
                    {

                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1],
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                   "UP", true, "");
                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputResult = "UP";
                        if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                        {
                            bool IsDefectiveMESExist = false;
                            foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                            {
                                if (_defective.def_code == "UP")
                                {
                                    _defective.qty++;
                                    IsDefectiveMESExist = true;
                                    break;
                                }
                            }
                            if (IsDefectiveMESExist == false)
                            {
                                m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = "UP", qty = 1 });
                            }
                        }
                    }
                }
                if (m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") == m_ProductShareVariables.nInputTrayNo + 1)
                {
                    //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Come in update");
                    m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                }
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_SKIP_UNIT_DONE_NOMES", true);
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_S1_MISSING_UNIT") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_S1_MISSING_UNIT", false);
                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Come in");
                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} UdpateMappingHead={m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead")}");
                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} TrayNo={m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputTrayNo") }");
                for (int j = 0; j < m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo.Length; j++)
                {
                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn")
                        && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[j].InputRow == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow"))
                    {

                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1],
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                   "UP", true, "");
                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[j].InputResult = "UP";
                        //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                        if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                        {
                            bool IsDefectiveMESExist = false;
                            foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                            {
                                if (_defective.def_code == "UP")
                                {
                                    _defective.qty++;
                                    IsDefectiveMESExist = true;
                                    break;
                                }
                            }
                            if (IsDefectiveMESExist == false)
                            {
                                m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = "UP", qty = 1 });
                            }
                        }
                        //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Update data");
                    }
                }
                if (m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") == m_ProductShareVariables.nInputTrayNo + 1)
                {
                    //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Come in update");
                    m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                }
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_S3_MISSING_UNIT") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_S3_MISSING_UNIT", false);
                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Come in");
                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} UdpateMappingHead={m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead")}");
                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} TrayNo={m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputTrayNo") }");
                for (int j = 0; j < m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo.Length; j++)
                {
                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn")
                        && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[j].InputRow == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow"))
                    {

                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1],
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                   "UP", true, "");
                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[j].InputResult = "UP";
                        //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                        if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                        {
                            bool IsDefectiveMESExist = false;
                            foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                            {
                                if (_defective.def_code == "UP")
                                {
                                    _defective.qty++;
                                    IsDefectiveMESExist = true;
                                    break;
                                }
                            }
                            if (IsDefectiveMESExist == false)
                            {
                                m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = "UP", qty = 1 });
                            }
                        }
                        //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Update data");
                    }
                }
                if (m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") == m_ProductShareVariables.nInputTrayNo + 1)
                {
                    //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Come in update");
                    m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                }
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_OUTPUT_MISSING_UNIT") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_OUTPUT_MISSING_UNIT", false);
                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Come in");
                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} UdpateMappingHead={m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead")}");
                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} TrayNo={m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("UpdateMappingProgressHead"), "InputTrayNo") }");
                for (int j = 0; j < m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputTrayNo") - 1].arrayUnitInfo.Length; j++)
                {
                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputColumn")
                        && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputTrayNo") - 1].arrayUnitInfo[j].InputRow == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputRow"))
                    {

                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputTrayNo") - 1],
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputTrayNo") - 1].arrayUnitInfo[j].InputRow,
                                   m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn,
                                   "UP", true, "");
                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputTrayNo") - 1].arrayUnitInfo[j].InputResult = "UP";
                        //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                        if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                        {
                            bool IsDefectiveMESExist = false;
                            foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                            {
                                if (_defective.def_code == "UP")
                                {
                                    _defective.qty++;
                                    IsDefectiveMESExist = true;
                                    break;
                                }
                            }
                            if (IsDefectiveMESExist == false)
                            {
                                m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = "UP", qty = 1 });
                            }
                        }
                        //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Update data");
                    }
                }
                if (m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput"), "InputTrayNo") == m_ProductShareVariables.nInputTrayNo + 1)
                {
                    //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Come in update");
                    m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                }
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_OUTPUT_POST_MISSING_UNIT") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MAP_AFTER_OUTPUT_POST_MISSING_UNIT", false);

                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))))
                {
                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))];
                    
                    m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                }
                else
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn")));
                }

            }
            if (m_ProductProcessEvent.GUI_PCS_Force_Generate_Last_Data.WaitOne(0))
            {
                m_ProductProcessEvent.PCS_PCS_Start_Write_AllFile.Set();
                Machine.LogDisplay.AddLogDisplay("Process", "Force generate last available data");
                if (m_ProductShareVariables.UserChooseContinueLot == false)
                {
                    //if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput)
                    //    && (m_ProductShareVariables.bolFirstTimeCreateOutputFile == true || m_ProductShareVariables.bolFirstTimeCreateOutputFileAfterCombineLot == true))
                    //{
                    //    for (int i = 1; i < 100; i++)
                    //    {
                    //        if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString()) == false)
                    //        {
                    //            RenameFolder(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                    //                m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                    //            break;
                    //        }
                    //    }
                    //}
                    //if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput)
                    //    && (m_ProductShareVariables.bolFirstTimeCreateOutputFile == true || m_ProductShareVariables.bolFirstTimeCreateOutputFileAfterCombineLot == true))
                    //{
                    //    for (int i = 1; i < 100; i++)
                    //    {
                    //        if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString()) == false)
                    //        {
                    //            RenameFolder(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                    //                m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                    //            break;
                    //        }
                    //    }
                    //}
                }

                string strFolderName = @m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotID;
                string strFileName = "Summary-" + m_ProductShareVariables.strucInputProductInfo.LotID;
                string strCSVFileName = "Map-" + m_ProductShareVariables.strucInputProductInfo.dtStartDate.ToString("yyyyMMdd") + "-" + m_ProductShareVariables.strucInputProductInfo.LotID;


                if (File.Exists(strFolderName + "\\" + strFileName + ".txt"))
                {
                    for (int i = 1; i < 100; i++)
                    {
                        if (File.Exists(strFolderName + "\\" + strFileName + "_Backup_" + i.ToString() + ".txt") == false)
                        {
                            File.Move(strFolderName + "\\" + strFileName + ".txt", strFolderName + "\\" + strFileName + "_Backup_" + i.ToString() + ".txt");
                            //RenameFolder(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                            //    m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                            break;
                        }
                    }
                }

                if (File.Exists(strFolderName + "\\" + strCSVFileName + ".csv"))
                {
                    for (int i = 1; i < 100; i++)
                    {
                        if (File.Exists(strFolderName + "\\" + strCSVFileName + "_Backup_" + i.ToString() + ".csv") == false)
                        {
                            File.Move(strFolderName + "\\" + strCSVFileName + ".csv", strFolderName + "\\" + strCSVFileName + "_Backup_" + i.ToString() + ".csv");
                            //RenameFolder(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                            //    m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                            break;
                        }
                    }
                }

                m_ProductShareVariables.bolFirstTimeCreateOutputFileAfterCombineLot = false;
                m_ProductShareVariables.bolFirstTimeCreateOutputFile = false;
                WriteUpdateOutputFile();
                WriteAllOutputFile();
            }

            //if (GetShareMemoryEvent("RTHD_GMAIN_IPT_TRAY_EMPTY_POSSIBLE_ENDLOT") == true)
            //{
            //    SetShareMemoryEvent("RTHD_GMAIN_IPT_TRAY_EMPTY_POSSIBLE_ENDLOT", false);
            //    m_ProductShareVariables.bFormProductionProductEndLotEnable = true;
            //}
            if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_START_GET_OUTPUT_STATION") == true)
            {
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_START_GET_OUTPUT_STATION", false);
                try
                {
                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))))
                    {
                        int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))];
                        if(m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "CurrentOutputTableNo") == 5)//Output
                        {
                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputTrayNo = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputTrayNo");
                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputTrayType = "OUTPUT";
                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputRow = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow");
                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputColumn = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn");
                        }
                        else //Reject
                        {
                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputTrayNo = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectTrayNo");
                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputTrayType = "REJECT";
                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputRow = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow");
                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputColumn = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn");
                        }
                        m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                    }
                    else
                    {
                        Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn")));
                    }
                }
                catch(Exception ex)
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                }
                finally
                {
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_GET_OUTPUT_STATION_DONE", true);
                }
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_UPDATE_MES_DATA") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MES_DATA", false);
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MES_DATA_DONE", true);

            }
            if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_LOT_UNLOAD_COMPLETE") == true)
            {
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_LOT_UNLOAD_COMPLETE", false);

                //WriteUpdateOutputFile();
                //WriteAllOutputFile();
                OnLotUnloadComplete();
                m_ProductShareVariables.nInputTrayNo++;
                ProcessInputFile();
                if (m_ProductRTSSProcess.GetEvent("GMAIN_RTHD_ENDLOT") == true)
                {
                    if(m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_THR_IS_REMAINTRAY") ==true)
                    {
                        m_ProductShareVariables.LotIDForTrayRemainedOnInputTrayTable = m_ProductShareVariables.strucInputProductInfo.LotID;
                    }
                    //m_ProductShareVariables.IsPassYieldMES = true;
                    //int TotalQuantityRun = m_ProductRTSSProcess.GetProductionInt("nTotalInputUnitDone");
                    //int TotalPassQty = m_ProductRTSSProcess.GetProductionInt("nCurrentTotalUnitDone");
                    //m_ProductShareVariables.LotYield = (double)TotalPassQty / (double)TotalQuantityRun;
                    m_ProductProcessEvent.GUI_PCS_NewLotDone2.Reset();
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_UPDATE_REAL_TIME_YEILD", false);
                    m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_ENDLOT", false);
                    m_ProductShareVariables.dtProductionEndTime = DateTime.Now;
                    //if (m_ProductRTSSProcess.GetProductionBool("IsUpdateMESAgain") == false)
                    //{
                    //    m_ProductShareVariables.CurrentLotMES.good_qty = m_ProductRTSSProcess.GetProductionInt("nCurrentLotGoodQuantity");
                    //    if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                    //    {
                    //        m_ProductShareVariables.CurrentLotMES.ng_qty = m_ProductRTSSProcess.GetProductionInt("nCurrentLotNotGoodQuantity");
                    //    }
                    //    else if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputTrayNo == true)
                    //    {
                    //        if (m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun") >= m_ProductRTSSProcess.GetProductionInt("nInputLotTrayNo"))
                    //        {
                    //            m_ProductShareVariables.CurrentLotMES.ng_qty = m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") - m_ProductRTSSProcess.GetProductionInt("nCurrentLotGoodQuantity");
                    //            int LeftUnit = m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") - m_ProductRTSSProcess.GetProductionInt("nCurrentLotGoodQuantity") - m_ProductRTSSProcess.GetProductionInt("nCurrentLotNotGoodQuantity");
                    //            if (LeftUnit > 0)
                    //            {
                    //                //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                    //                {
                    //                    bool IsDefectiveMESExist = false;
                    //                    foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                    //                    {
                    //                        if (_defective.def_code == "UP")
                    //                        {
                    //                            _defective.qty = _defective.qty + LeftUnit;
                    //                            IsDefectiveMESExist = true;
                    //                            break;
                    //                        }
                    //                    }
                    //                    if (IsDefectiveMESExist == false)
                    //                    {
                    //                        m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = "UP", qty = LeftUnit });
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            m_ProductShareVariables.CurrentLotMES.ng_qty = m_ProductRTSSProcess.GetProductionInt("nCurrentLotNotGoodQuantity");
                    //        }
                    //    }
                    //    m_ProductShareVariables.CurrentLotMES.defective = m_ProductShareVariables.CurrentDefectCounterMES.ToArray();
                    //    m_ProductShareVariables.TotalLotMES.Add(m_ProductShareVariables.CurrentLotMES);
                    //}
                    //else
                    //{
                    //    m_ProductRTSSProcess.SetProductionBool("IsUpdateMESAgain",false);
                    //}
                    //WriteUpdateOutputFile();
                    m_ProductProcessEvent.PCS_PCS_Send_Vision_EndLot.Set();
                    m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.Reset();
                    m_ProductProcessEvent.PCS_GUI_EndLot.Set();
                    m_ProductShareVariables.nLotIDNumber = 0;
                    m_ProductShareVariables.PreviousReportLotID = "";
                    //m_ProductShareVariables.PreviousReportTrayNo = 0;
                }
            }
            if (m_ProductShareVariables.IsPassYieldMESDone == true)
            {
                m_ProductShareVariables.IsPassYieldMESDone = false;
                m_ProductProcessEvent.PCS_PCS_SEND_OUTPUT_DATA_MES.Set();
                m_ProductProcessEvent.PCS_PCS_SEND_ENDJOB_DATA_MES.Set();
            }
            //if(m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_ADD_TRAY_TO_WRITE_REPORT") == true)
            //{
            //    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_ADD_TRAY_TO_WRITE_REPORT", false);
            //    m_ProductShareVariables.ListTrayToWriteReport.Add(m_ProductRTSSProcess.GetProductionInt("WriteReportTrayNoOnOutput"));
            //}
            if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_ALL_UNITS_PROCESSED_CURRENT_TRAY") == true)
            {
                m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_ALL_UNITS_PROCESSED_CURRENT_TRAY", false);
                //foreach(int trayno in m_ProductShareVariables.ListTrayToWriteReport)
                //{
                //    m_ProductRTSSProcess.SetProductionInt("WriteReportTrayNo", trayno);
                //}
                WriteUpdateOutputFile();
                //m_ProductShareVariables.ListTrayToWriteReport.Clear();
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_CHANGE_MAPPING") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_CHANGE_MAPPING", false);
                //readInfo.InitializeMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductShareVariables.nInputTrayNo], (int)m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput, (int)m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColInput, 0, 1);
                m_ProductProcessEvent.PCS_GUI_Initial_Map.Set();
            }
            if(m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_UPDATE_MES_LOT_DATA")==true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_UPDATE_MES_LOT_DATA", false);
                m_ProductShareVariables.CurrentLotMES.good_qty = m_ProductRTSSProcess.GetProductionInt("nCurrentLotGoodQuantity");
                if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                {
                    m_ProductShareVariables.CurrentLotMES.ng_qty = m_ProductRTSSProcess.GetProductionInt("nCurrentLotNotGoodQuantity");
                }
                else if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputTrayNo==true)
                {
                    m_ProductShareVariables.CurrentLotMES.ng_qty = m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") -m_ProductRTSSProcess.GetProductionInt("nCurrentLotGoodQuantity");
                    int LeftUnit = m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") - m_ProductRTSSProcess.GetProductionInt("nCurrentLotGoodQuantity") - m_ProductRTSSProcess.GetProductionInt("nCurrentLotNotGoodQuantity");
                    if (LeftUnit > 0)
                    {
                        //if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                        {
                            bool IsDefectiveMESExist = false;
                            foreach (MoveonMESAPI.Defective _defective in m_ProductShareVariables.CurrentDefectCounterMES)
                            {
                                if (_defective.def_code == "UP")
                                {
                                    _defective.qty=_defective.qty+LeftUnit;
                                    IsDefectiveMESExist = true;
                                    break;
                                }
                            }
                            if (IsDefectiveMESExist == false)
                            {
                                m_ProductShareVariables.CurrentDefectCounterMES.Add(new MoveonMESAPI.Defective { def_code = "UP", qty = LeftUnit });
                            }
                        }
                    }
                }
                m_ProductShareVariables.CurrentLotMES.defective = m_ProductShareVariables.CurrentDefectCounterMES.ToArray();
                m_ProductShareVariables.TotalLotMES.Add(m_ProductShareVariables.CurrentLotMES);
                m_ProductShareVariables.UserChooseContinueLot = true;
                //WriteUpdateOutputFile();
                m_ProductProcessEvent.PCS_GUI_EndLot.Set();
                //m_ProductShareVariables.nInputTrayNo = 0;
                //m_ProductProcessEvent.PCS_PCS_StartReadInputFile.Set();
                //m_ProductShareVariables.PreviousReportLotID = "";
                //m_ProductShareVariables.PreviousReportTrayNo = 0;
                //m_ProductProcessEvent.GUI_GUI_GET_DATA.Reset();
            }
            for (int i = 0; i < 2; i++)
            {
                if (nPickUpHeadCount[i] != m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, ""))
                {
                    nPickUpHeadCount[i] = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "");
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("Pick Up Head " + (i + 1), m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, ""));
                }
            }
            //if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"))
            //{
            //    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"));
            //}
            //if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentOutputTrayNo", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo"))
            //{
            //    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentOutputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo"));
            //}

            //if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentSortingTrayNo", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentSortingTrayNo"))
            //{
            //    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentSortingTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentSortingTrayNo"));
            //}
            //if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentRejectTrayNo", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentRejectTrayNo"))
            //{
            //    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentRejectTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentRejectTrayNo"));
            //}
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_START_RUNNING") == true)
            {
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"));
                }
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentOutputTrayNo", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentOutputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo"));
                }
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentTotalUnitDone", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentTotalUnitDone"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentTotalUnitDone", m_ProductRTSSProcess.GetProductionInt("nCurrentTotalUnitDone"));
                }
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentRejectTrayNo", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentRejectTrayNo"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentRejectTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentRejectTrayNo"));
                }
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrrentTotalUnitDoneByLot", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrrentTotalUnitDoneByLot"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrrentTotalUnitDoneByLot", m_ProductRTSSProcess.GetProductionInt("nCurrrentTotalUnitDoneByLot"));
                }
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentTotalInputUnitDone", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentTotalInputUnitDone"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentTotalInputUnitDone", m_ProductRTSSProcess.GetProductionInt("nCurrentTotalInputUnitDone"));
                }
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputLotQuantityRun", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotQuantityRun"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentInputLotQuantityRun", m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotQuantityRun"));
                }
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nTotalInputUnitDone", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nTotalInputUnitDone"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nTotalInputUnitDone", m_ProductRTSSProcess.GetProductionInt("nTotalInputUnitDone"));
                }
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentLotNotGoodQuantity", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentLotNotGoodQuantity"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentLotNotGoodQuantity", m_ProductRTSSProcess.GetProductionInt("nCurrentLotNotGoodQuantity"));
                }
                if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentLotGoodQuantity", 0).ToString()) != m_ProductRTSSProcess.GetProductionInt("nCurrentLotGoodQuantity"))
                {
                    m_ProductShareVariables.RegKey.SetEstekKeyParameter("nCurrentLotGoodQuantity", m_ProductRTSSProcess.GetProductionInt("nCurrentLotGoodQuantity"));
                }
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_READ_PREVIOUS_LOTID")==true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_READ_PREVIOUS_LOTID", false);
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_WRITE_PREVIOUS_LOTID") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_WRITE_PREVIOUS_LOTID", false);
            }
            if(m_ProductRTSSProcess.GetEvent("RMAIN_GMAIN_LOADING_CALIBRATION_RECIPE") ==true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_LOADING_CALIBRATION_RECIPE", false);
                m_ProductProcessEvent.PCS_PCS_Send_Vision_Calibration_Setting.Set();
            }
            if(m_ProductRTSSProcess.GetEvent("RMAIN_GMAIN_SAVE_UNFINISHED_LOT")==true)
            {
                bool IsDataExist = false;
                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_SAVE_UNFINISHED_LOT", false);
                foreach(LotDetail _Lot in m_ProductShareVariables.productContinueLotInfo.PreviousLotInfo)
                {
                    if(_Lot.LotID == m_ProductShareVariables.strucInputProductInfo.LotID)
                    {
                        _Lot.Row = m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX");
                        _Lot.Column = m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateY");
                        _Lot.InputBalance = m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") - m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotQuantityRun");
                        _Lot.nCurrentInputTrayNo = m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo");
                        if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputTrayNo == true && m_ProductRTSSProcess.GetProductionInt("nInputLotTrayNo") >= m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun"))
                        {
                            _Lot.InputTrayNo = m_ProductRTSSProcess.GetProductionInt("nInputLotTrayNo") - m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun");
                        }
                        IsDataExist = true;
                        break;
                    }
                }
                if (IsDataExist == false)
                {
                    if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputTrayNo == true && m_ProductRTSSProcess.GetProductionInt("nInputLotTrayNo") >= m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun"))
                    {
                        m_ProductShareVariables.productContinueLotInfo.PreviousLotInfo.Add(new LotDetail() { LotID = m_ProductShareVariables.strucInputProductInfo.LotID, Row = m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX"), Column = m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateY"), InputBalance = m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") - m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotQuantityRun"), InputTrayNo = m_ProductRTSSProcess.GetProductionInt("nInputLotTrayNo") - m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun"), nCurrentInputTrayNo = m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo") });
                    }
                    else
                    {
                        m_ProductShareVariables.productContinueLotInfo.PreviousLotInfo.Add(new LotDetail() { LotID = m_ProductShareVariables.strucInputProductInfo.LotID, Row = m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX"), Column = m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateY"), InputBalance = m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") - m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotQuantityRun"), nCurrentInputTrayNo = m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo") });
                    }
                }
                SaveContinueLotInfo();
            }
            if(m_ProductRTSSProcess.GetEvent("RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED") ==true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED", false);
                try
                {
                    string nRow = m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString();
                    string nCol = m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString();
                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", nRow, nCol)))
                    {
                        int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                        if(true
                            && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].InputResult == "P"
                            && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].BottomResult == "P"
                            && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].SetupResult == "P"
                            && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S1Result == "P"
                            && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S2PartingResult == "P"
                            && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3PartingResult == "P"
                            && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S2Result == "P"
                            && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3Result == "P"
                            )
                        {
                            m_ProductRTSSProcess.SetProductionInt("nCurrentProcessRejectTrayNo", 5);
                        }
                        else
                        {
                            m_ProductRTSSProcess.SetProductionInt("nCurrentProcessRejectTrayNo", 0);
                        }
                    }
                    else
                    {
                        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), $"RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED: Row = {nRow} Col = {nCol} Not Found"));
                    }
                }
                catch (Exception ex)
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                }
                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_DONE", true);
            }
            if (m_ProductRTSSProcess.GetEvent("RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_POST") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_POST", false);
                for (int j = 0; j < m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo.Length; j++)
                {
                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputColumn == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn")
                        && m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].InputRow == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"))
                    {
                        if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].OutputResult != "P")
                        {
                            bool IsMatch = false;
                            foreach (SortTrayInfo _SortTrayInfo in m_ProductShareVariables.productRecipeOutputSettings.listSortTrayInfo)
                            {
                                if (_SortTrayInfo.DefectCode == m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[j].OutputResult)
                                {
                                    m_ProductRTSSProcess.SetProductionInt("nCurrentProcessRejectTrayNo", (5 - _SortTrayInfo.RejectTray));
                                    IsMatch = true;
                                    break;
                                }
                            }
                            if (IsMatch == false)
                            {
                                m_ProductRTSSProcess.SetProductionInt("nCurrentProcessRejectTrayNo", 0);
                            }
                        }
                        break;
                    }
                }
                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_READ_TABLE_TO_BE_PLACED_POST_DONE", true);

            }

            if (m_ProductRTSSProcess.GetEvent("RMAIN_GMAIN_RESET_S2S3_VISION_RESULT") == true)
            {
                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_RESET_S2S3_VISION_RESULT", false);

                for (int i = 0; i < 10; i++)
                {
                    m_ProductShareVariables.S2FacetVisionResult[i] = "F";
                    m_ProductShareVariables.S3FacetVisionResult[i] = "F";
                }
                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_RESET_S2S3_VISION_RESULT_DONE", true);
            }

            if (m_thdCommunication == null || !m_thdCommunication.IsAlive)
            {
                m_thdCommunication = new Thread(m_CommunicationSequenceThread.CommunicationThread);
                m_thdCommunication.Start();
            }
            #region Secsgem
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.HostTriggerRemoteControlStart.WaitOne(0))
                {
                    if (m_ProductRTSSProcess.GetEvent("JobMode") == true)
                    {
                        if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                        {
                            m_ProductRTSSProcess.SetEvent("StartReset", true);
                            Thread.Sleep(500);
                            m_ProductRTSSProcess.SetEvent("StartJob", true);
                        }
                    }
                }
                if (Machine.Platform.SecsgemControl.HostTriggerRemoteControlPause.WaitOne(0))
                {
                    if (m_ProductRTSSProcess.GetEvent("JobMode") == true)
                    {
                        if (m_ProductStateControl.IsCurrentStateCanTriggerPause())
                            m_ProductRTSSProcess.SetEvent("StartPause", true);
                    }
                }
                if (Machine.Platform.SecsgemControl.HostTriggerRemoteControlStop.WaitOne(0))
                {
                    if (m_ProductRTSSProcess.GetEvent("JobMode") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                        if (m_ProductStateControl.IsCurrentStateCanTriggerResume())
                            m_ProductRTSSProcess.SetEvent("StartJob", true);
                    }
                }
                if (Machine.Platform.SecsgemControl.EventStartUp.WaitOne(0))
                {
                    Machine.Platform.SecsgemControl.SetVariable(SecsgemCategory.OTHERS, "EquipmentModel", GetType().Assembly.GetName().Name);
                    Machine.Platform.SecsgemControl.SetVariable(SecsgemCategory.OTHERS, "SoftwareRevision", GetType().Assembly.GetName().Version.ToString());
                    Machine.Platform.SecsgemControl.SetSecsgemMode(m_ProductShareVariables.productOptionSettings.SecsgemMode);
                    Machine.Platform.SecsgemControl.ControlState = m_ProductShareVariables.productOptionSettings.SecsgemMode;
                }
            }
            #endregion Secsgem

            #region MES
            if(m_ProductProcessEvent.PCS_PCS_SEND_OUTPUT_DATA_MES.WaitOne(0))
            {
                SettingURL();
                ConfigureMESURL(m_ProductShareVariables.MESInputURLUsed, m_ProductShareVariables.MESOutputURLUsed, m_ProductShareVariables.MESEndJobURLUsed);
                OutputData outputData = new OutputData()
                {
                    pcode = "HD4",
                    shf=m_ProductShareVariables.strucInputProductInfo.Shift,
                    emp_no = m_ProductShareVariables.strucInputProductInfo.OperatorID,
                    mc_no = m_ProductShareVariables.productOptionSettings.MachineNo,
                    yield = m_ProductShareVariables.LotYield,
                    mbb_pkg_lot_no = m_ProductShareVariables.strucInputProductInfo.LotIDOutput,
                    details = m_ProductShareVariables.TotalLotMES.ToArray(),
                    downtime = m_ProductShareVariables.CurrentDownTimeCounterMES.ToArray(),
                };
                string jsonStringOutput = Newtonsoft.Json.JsonConvert.SerializeObject(outputData, Newtonsoft.Json.Formatting.Indented);
                if (!Directory.Exists("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput))
                {
                    Directory.CreateDirectory("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput);
                }
                else
                {
                    for (int i = 1; i < 100; i++)
                    {
                        if (Directory.Exists("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString()) == false)
                        {
                            RenameFolder("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                                "F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                            Directory.CreateDirectory("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput);
                            break;
                        }
                    }
                }
                File.WriteAllText("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "\\outputData.json", jsonStringOutput);
                if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                {
                    SendOutputDataMES(outputData);
                }
            }
            if(m_ProductProcessEvent.PCS_PCS_SEND_ENDJOB_DATA_MES.WaitOne(0))
            {
                SettingURL();
                ConfigureMESURL(m_ProductShareVariables.MESInputURLUsed, m_ProductShareVariables.MESOutputURLUsed, m_ProductShareVariables.MESEndJobURLUsed);
                EndJobData endjobData = new EndJobData()
                {
                    pcode="HD4",
                    shf = m_ProductShareVariables.strucInputProductInfo.Shift,
                    mc_no = m_ProductShareVariables.productOptionSettings.MachineNo,
                    stime = m_ProductShareVariables.dtProductionStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    etime = m_ProductShareVariables.dtProductionEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    yield = m_ProductShareVariables.LotYield,
                };
                string jsonStringEndJob = Newtonsoft.Json.JsonConvert.SerializeObject(endjobData, Newtonsoft.Json.Formatting.Indented);
                //if (!Directory.Exists("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput))
                //{
                //    Directory.CreateDirectory("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput);
                //}
                //else
                //{
                //    for (int i = 1; i < 100; i++)
                //    {
                //        if (Directory.Exists("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString()) == false)
                //        {
                //            RenameFolder("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                //                "F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                //            Directory.CreateDirectory("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput);
                //            break;
                //        }
                //    }
                //}
                File.WriteAllText("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "\\endjobData.json", jsonStringEndJob);
                if (m_ProductShareVariables.productOptionSettings.EnableMES == true)
                {
                    SendEndJobDataMES(endjobData);
                }
            }
            #endregion MES

            if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_AvailableDiskSpaceLow.WaitOne(0) == true)
            {
                m_ProductProcessEvent.PCS_PCS_Vision_Receive_AvailableDiskSpaceLow.Reset();
                Machine.SequenceControl.SetAlarm(60002);
            }
        }

        override public void OnRunningSecondThread()
        {
            base.OnRunningSecondThread();

            if (m_ProductProcessEvent.PCS_PCS_Start_Map_Drive.WaitOne(0))
            {
                //if (m_ShareVariables.configurationSettings.EnableOffline == false)
                {
                    if (Directory.Exists(@"D:\Estek\Map Drive"))
                    {
                        if (File.Exists(@"D:\Estek\Map Drive\Map Drive.bat") == true)
                        {
                            System.Diagnostics.Process process = new System.Diagnostics.Process();
                            process.StartInfo.FileName = @"D:\Estek\Map Drive\Map Drive.bat";
                            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            process.Start();
                        }
                    }

                }
            }
            //if (GetShareMemoryEvent("RTHD_GMAIN_FRAME_INSPECTION_COMPLETE") == true)
            //{
            //    Machine.LogDisplay.AddLogDisplay("Process", "Inspection finish");
            //    SetShareMemoryEvent("RTHD_GMAIN_FRAME_INSPECTION_COMPLETE", false);
            //    m_ProductProcessEvent.PCS_PCS_Send_Vision_EndTile.Set();
            //    //CustomizeOutputFile();
            //}
        }
        void UpdatedInitialMaintenanceCount()
        {
            //Maintenance Count 
            for (int i = 0; i < 2; i++)
            {
                m_ProductRTSSProcess.SetProductionArray("PickUpHeadCount", i, "", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Pick Up Head " + (i + 1), 1).ToString()));
                nPickUpHeadCount[i] = m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "");
            }
            //for (int i = 0; i <= 3; i++)//FlipperHeadCount
            //{
            //    m_ProductRTSSProcess.SetProductionArray("FlipperHeadCount", i, "", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Flipper Head " + (i + 1), 1).ToString()));
            //    nFlipperHeadCount[i] = m_ProductRTSSProcess.GetProductionArray("FlipperHeadCount", i, "");
            //}
            //m_ProductRTSSProcess.SetProductionArray("EjectorNeedle", 0, "", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Ejector Needle", 1).ToString()));
            //nEjectorNeedle[0] = m_ProductRTSSProcess.GetProductionArray("EjectorNeedle", 0, "");
            //m_ProductRTSSProcess.SetProductionArray("AlignerTip", 0, "", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Aligner Tip", 1).ToString()));
            //nAlignerTip[0] = m_ProductRTSSProcess.GetProductionArray("AlignerTip", 0, "");
        }

        override public void OnRunningThirdThread()
        {
            base.OnRunningThirdThread();
        }
        
        virtual public int ProcessInputFile()
        {
            int nError = 0;
            int nNextIROW = 0;
            int nNextICOL = 0;
            int nNextOROW = 0;
            int nNextOCOL = 0;
            try
            {
                m_ProductRTSSProcess.SetProductionBool("ResumePickAndPlace", false);
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_RETEACH_MAP_VERIFICATION", false);
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_RETEACH_MAP_ALLOCATE_UNIT", false);
                //if (Directory.Exists(m_ProductShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_ProductShareVariables.strCurrentBarcodeID))
                //{
                //    if (m_ProductShareVariables.nLoginAuthority > 1)
                //    {
                //        Machine.SequenceControl.SetAlarm(60402);
                //        DialogResult Result = MessageBox.Show("Do You Want to Continue Running This Frame and Combine With Previous Report File?\r\n" +
                //            "Press 'Yes' for Combine Frame\n" + "Press 'No' for Rerun Frame\n" + "Press 'Cancel' to Abort This Particular Frame",
                //            "Resume Pick and Place", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);
                //        if (Result == DialogResult.Yes)
                //        {
                //            if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                //            {
                //                SetShareMemoryEvent("StartReset", true);
                //                Thread.Sleep(500);
                //                SetShareMemoryEvent("StartJob", true);
                //            }
                //            if (CheckPreviousFile(m_ProductShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_ProductShareVariables.strCurrentBarcodeID
                //                + "\\" + "Report-" + m_ProductShareVariables.strCurrentBarcodeID + ".txt",
                //                out nNextIROW, out nNextICOL, out nNextOROW, out nNextOCOL) == 0)
                //            {
                //                SetShareMemoryEvent("GMAIN_RTHD_TEACH_MAP_DONE", false);
                //                //m_ProductProcessEvent.PCS_GUI_Initial_Map.Set();
                //                SetShareMemoryEvent("GMAIN_RTHD_RESUME_PNP_CONTINUE_FRAME", true);
                //                m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                //                m_ProductRTSSProcess.SetProductionBool("ResumePickAndPlace", true);
                //                SetShareMemoryEvent("RMAIN_RTHD_RETEACH_MAP_VERIFICATION", true);
                //                SetShareMemoryProductionInt("ResumePnPInputFirstRowNo", nNextIROW);
                //                SetShareMemoryProductionInt("ResumePnPInputFirstColNo", nNextICOL);
                //                SetShareMemoryProductionInt("ResumePnPOutputFirstRowNo", nNextOROW);
                //                SetShareMemoryProductionInt("ResumePnPOutputFirstColNo", nNextOCOL);
                //            }
                //            else
                //            {
                //                Machine.SequenceControl.SetAlarm(60403);
                //                Machine.LogDisplay.AddLogDisplay("Error", "Error During Extracting Data From Previous Report File.");
                //                SetShareMemoryEvent("JobStop", true);
                //            }
                //        }
                //        else if (Result == DialogResult.Cancel)
                //        {
                //            if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                //            {
                //                SetShareMemoryEvent("StartReset", true);
                //                Thread.Sleep(500);
                //                SetShareMemoryEvent("StartJob", true);
                //            }
                //            SetShareMemoryEvent("GMAIN_RTHD_RESUME_PNP_ABORT_FRAME", true);
                //            //goto EndProcessInputFile;
                //        }
                //        else if (Result == DialogResult.No)
                //        {
                //            if (m_ProductStateControl.IsCurrentStateCanTriggerResume() == true)
                //            {
                //                SetShareMemoryEvent("StartReset", true);
                //                Thread.Sleep(500);
                //                SetShareMemoryEvent("StartJob", true);
                //            }
                //            SetShareMemoryEvent("GMAIN_RTHD_RESUME_PNP_CONTINUE_FRAME", true);
                //        }
                //    }
                //    else
                //    {
                //        Machine.SequenceControl.SetAlarm(60401);
                //        Machine.LogDisplay.AddLogDisplay("Assist", "Output file exist. Please Refer Higher Authority Whether to Combine Report or Rerun.");
                //        SetShareMemoryEvent("GMAIN_RTHD_RESUME_PNP_ABORT_FRAME", true);
                //    }
                //}
                //else
                //{
                //    SetShareMemoryEvent("GMAIN_RTHD_RESUME_PNP_CONTINUE_FRAME", true);
                //}
                //EndProcessInputFile:;
            }
            catch (Exception ex)
            {
                nError = -1;
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return nError;
        }
        override public int ReadInputFile()
        {
            int nError = 0;
            try
            {

                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int WriteUpdateOutputFile()
        {
            int nError = 0;

            try
            {
                if (m_ProductRTSSProcess.GetEvent("GGUI_RSEQ_DRY_RUN_MODE") == true && m_ProductRTSSProcess.GetSettingBool("EnableVision") == false)
                {
                    return nError;
                }
                //Machine.LogDisplay.AddLogDisplay("Process", "Wait Vision Generate Data");
                HiPerfTimer timerVisionFileGeneration = new HiPerfTimer();
                timerVisionFileGeneration.Start();

                //if (m_ProductShareVariables.UserChooseContinueLot == false)
                //{
                //    if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput)
                //        && (m_ProductShareVariables.bolFirstTimeCreateOutputFile == true || m_ProductShareVariables.bolFirstTimeCreateOutputFileAfterCombineLot == true))
                //    {
                //        for (int i = 1; i < 100; i++)
                //        {
                //            if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString()) == false)
                //            {
                //                RenameFolder(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                //                    m_ProductShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                //                break;
                //            }
                //        }
                //    }
                //    if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput)
                //        && (m_ProductShareVariables.bolFirstTimeCreateOutputFile == true || m_ProductShareVariables.bolFirstTimeCreateOutputFileAfterCombineLot == true))
                //    {
                //        for (int i = 1; i < 100; i++)
                //        {
                //            if (Directory.Exists(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString()) == false)
                //            {
                //                RenameFolder(m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "",
                //                    m_ProductShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_Backup_" + i.ToString());
                //                break;
                //            }
                //        }
                //    }
                //}
                m_ProductShareVariables.bolFirstTimeCreateOutputFileAfterCombineLot = false;
                m_ProductShareVariables.bolFirstTimeCreateOutputFile = false;
                CustomizeOutputFile();

                return nError;
            }
            catch (Exception ex)
            {
                Machine.SequenceControl.SetAlarm(10014);
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }
        virtual public int CheckInputFile()
        {
            int nError = 0;
            try
            {
                //Naming issue need to solve
                if ((m_ProductShareVariables.mappingInfo.Row_Max == m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput
                    && m_ProductShareVariables.mappingInfo.Col_Max == m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColInput
                    ) == false)
                {
                    return 10;
                }

                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        virtual public int CustomizeOutputFile()
        {
            int nError = 0;
            try
            {
                
            }
            catch (Exception ex)
            {
                nError = -1;
                Machine.DebugLogger.WriteLog(string.Format("{0} {1} {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), "Exception in CustomizeOutputFile"));
            }
            return nError;
        }
        public int CopyAndDeleteFiles(string sourceDirectory, string destinationDirectory, List<string> listOfFiles, bool overwrite, bool deleteAfterCopy)
        {
            int nError = 0;
            bool bRetry = true;
            try
            {
                bRetry = false;
                if (Directory.Exists(sourceDirectory) == false)
                    return 1;
                if (Directory.Exists(destinationDirectory) == false)
                    Directory.CreateDirectory(destinationDirectory);
                foreach (string _file in listOfFiles)
                {
                    if (File.Exists(sourceDirectory + "\\" + _file))
                        File.Copy(sourceDirectory + "\\" + _file, destinationDirectory + "\\" + _file, overwrite);
                }
                if(deleteAfterCopy)
                {
                    foreach (string _file in listOfFiles)
                    {
                        if (File.Exists(sourceDirectory + "\\" + _file))
                            File.Delete(sourceDirectory + "\\" + _file);
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.SequenceControl.SetAlarm(60412);
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                if (MessageBox.Show("Please close all output folder and files", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Retry)
                    bRetry = true;
                else
                    bRetry = false;
                return -1;
            }
            finally
            {
                if (bRetry)
                {
                    CopyAndDeleteFiles(sourceDirectory, destinationDirectory, listOfFiles, overwrite, deleteAfterCopy);
                }
            }
        }
        public int RenameFolder(string targetFolder, string targetName)
        {
            int nError = 0;
            bool bRetry = true;
            try
            {
                Directory.Move(targetFolder, targetName);
                bRetry = false;
                return nError;
            }
            catch (Exception ex)
            {
                int code = ex.GetHashCode();
                if (ex.GetHashCode() == 1)
                {

                }
                //Machine.SequenceControl.SetAlarm(10014);
                Machine.SequenceControl.SetAlarm(60411);
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                if (MessageBox.Show("Please close all output folder and files", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Retry)
                    bRetry = true;
                else
                    bRetry = false;
                return -1;
            }
            finally
            {
                if (bRetry)
                {
                    RenameFolder(targetFolder, targetName);
                }
            }
        }

        virtual public int UpdateOutputDefectCode(int noOfUnit)
        {
            int nError = 0;
            try
            {

            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            return nError;
        }

        virtual public int UpdateInputDefectCode(int noOfUnit)
        {
            int nError = 0;
            try
            {

            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            return nError;
        }

        public int SaveContinueLotInfo()
        {
            int nError = 0;
            try
            {
                Tools.Serialize(m_ProductShareVariables.strSaveLotInfoPath + "LotData.xml",m_ProductShareVariables.productContinueLotInfo);
            }
            catch(Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            return nError;
        }
        public int SendOutputDataMES(MoveonMESAPI.OutputData outputData)
        {
            int nError = 0;
            try
            {
                for (int nRetry = 1; nRetry <= 1; nRetry++)
                {
                    nError = MoveonMESAPI.MESAPI.Post(outputData);
                    if (nError != 0)
                    {
                        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Send Output Data To MES Fail, StatusMessage: " + MoveonMESAPI.MESAPI.StatusMessage + ", ErrorMessage: "+MoveonMESAPI.MESAPI.ErrorMessage+"\n"));
                    }
                    else
                    {
                        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Send Output Data To MES Done, StatusMessage: " + MoveonMESAPI.MESAPI.StatusMessage+"\n"));
                        break;
                    }
                }
                if(nError != 0)
                {
                    Machine.SequenceControl.SetAlarm(80001);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            return nError;
        }
        public int SendEndJobDataMES(MoveonMESAPI.EndJobData endjobData)
        {
            int nError = 0;
            try
            {
                for (int nRetry = 1; nRetry <= 1; nRetry++)
                {
                    nError = MoveonMESAPI.MESAPI.Post(endjobData);
                    if (nError != 0)
                    {
                        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Send EndJob Data To MES Fail, StatusMessage: " + MoveonMESAPI.MESAPI.StatusMessage + ", ErrorMessage: " + MoveonMESAPI.MESAPI.ErrorMessage + "\n"));
                    }
                    else
                    {
                        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Send EndJob Data To MES Done, StatusMessage: " + MoveonMESAPI.MESAPI.StatusMessage + "\n"));
                        break;
                    }
                }
                if (nError != 0)
                {
                    Machine.SequenceControl.SetAlarm(80001);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            return nError;
        }
        public int ConfigureMESURL(string InputURL, string OutputURL, string EndJobURL)
        {
            int nError = 0;
            try
            {
                nError = MoveonMESAPI.MESAPI.Configure(InputURL, OutputURL, EndJobURL);
                if(nError !=0)
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Configure MES URL fail" + MoveonMESAPI.MESAPI.ErrorMessage));
                    Machine.SequenceControl.SetAlarm(80002);
                }
                else
                {
                    Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Configure MES URL Done"));
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            return nError;
        }
        public int SettingURL()
        {
            int nError = 0;
            try
            {
                m_ProductShareVariables.MESInputURLUsed = m_ProductShareVariables.productOptionSettings.MESInputURL + m_ProductShareVariables.strucInputProductInfo.Shift;
                m_ProductShareVariables.MESOutputURLUsed = m_ProductShareVariables.productOptionSettings.MESOutputURL;
                m_ProductShareVariables.MESEndJobURLUsed = m_ProductShareVariables.productOptionSettings.MESEndJobURL;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            return nError;
        }
        virtual public bool VerifySorting(string Barcode, string verifyStart, string verifyEnd, string style)
        {
            int length = Barcode.Length;

            string[] SplitBarcodePoint = new string[length];
            char[] charSplitString = new char[length];
            int[] intSplitString = new int[length];

            string[] SplitVerifyStartPoint = new string[verifyStart.Length];
            char[] charSplitVerifyStartString = new char[verifyStart.Length];
            int[] intSplitVerifyStartString = new int[verifyStart.Length];

            string[] SplitVerifyEndPoint = new string[verifyEnd.Length];
            char[] charSplitVerifyEndString = new char[verifyEnd.Length];
            int[] intSplitVerifyEndString = new int[verifyEnd.Length];

            int totalStringAsciiValue = 0;
            string strtotalStringAsciiValue = "";
            int totalStringStartAsciiValue = 0;
            string strtotalStringStartAsciiValue = "";
            int totalStringEndAsciiValue = 0;
            string strtotalStringEndAsciiValue = "";

            if (style.Contains("A"))
            {
                for (int i = 0; i < Barcode.Length; i++)
                {
                    SplitBarcodePoint[i] = Barcode.Substring(i, 1);
                    charSplitString[i] = char.Parse(SplitBarcodePoint[i]);
                    intSplitString[i] = charSplitString[i];

                    SplitVerifyStartPoint[i] = verifyStart.Substring(i, 1);
                    charSplitVerifyStartString[i] = char.Parse(SplitVerifyStartPoint[i]);
                    intSplitVerifyStartString[i] = charSplitVerifyStartString[i];

                    SplitVerifyEndPoint[i] = verifyEnd.Substring(i, 1);
                    charSplitVerifyEndString[i] = char.Parse(SplitVerifyEndPoint[i]);
                    intSplitVerifyEndString[i] = charSplitVerifyEndString[i];

                    if (intSplitString[i] < 65 || intSplitString[i] > 90)
                    {
                        return false;
                    }
                }
                for (int k = 0; k < intSplitString.Length; k++)
                {
                    strtotalStringAsciiValue += intSplitString[k].ToString();
                    strtotalStringStartAsciiValue += intSplitVerifyStartString[k].ToString();
                    strtotalStringEndAsciiValue += intSplitVerifyEndString[k].ToString();
                }
                totalStringAsciiValue = Int32.Parse(strtotalStringAsciiValue);
                totalStringStartAsciiValue = Int32.Parse(strtotalStringStartAsciiValue);
                totalStringEndAsciiValue = Int32.Parse(strtotalStringEndAsciiValue);
                if (totalStringAsciiValue >= totalStringStartAsciiValue && totalStringAsciiValue <= totalStringEndAsciiValue)
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                for (int i = 0; i < Barcode.Length; i++)
                {
                    SplitBarcodePoint[i] = Barcode.Substring(i, 1);
                    charSplitString[i] = char.Parse(SplitBarcodePoint[i]);
                    intSplitString[i] = charSplitString[i];

                    SplitVerifyStartPoint[i] = verifyStart.Substring(i, 1);
                    charSplitVerifyStartString[i] = char.Parse(SplitVerifyStartPoint[i]);
                    intSplitVerifyStartString[i] = charSplitVerifyStartString[i];

                    SplitVerifyEndPoint[i] = verifyEnd.Substring(i, 1);
                    charSplitVerifyEndString[i] = char.Parse(SplitVerifyEndPoint[i]);
                    intSplitVerifyEndString[i] = charSplitVerifyEndString[i];

                    if (intSplitString[i] < 48 || intSplitString[i] > 57)
                    {
                        return false;
                    }
                }
                for (int k = 0; k < intSplitString.Length; k++)
                {
                    strtotalStringAsciiValue += intSplitString[k].ToString();
                    strtotalStringStartAsciiValue += intSplitVerifyStartString[k].ToString();
                    strtotalStringEndAsciiValue += intSplitVerifyEndString[k].ToString();
                }
                totalStringAsciiValue = Int32.Parse(strtotalStringAsciiValue);
                totalStringStartAsciiValue = Int32.Parse(strtotalStringStartAsciiValue);
                totalStringEndAsciiValue = Int32.Parse(strtotalStringEndAsciiValue);
                if (totalStringAsciiValue >= totalStringStartAsciiValue && totalStringAsciiValue <= totalStringEndAsciiValue)
                {
                    return true;
                }
                else
                    return false;
            }
            return true;
        }

        #region Turret Application
        //#if TurretApplication
        //        public int ReadTapeIndexPosition(byte station)
        //        {
        //            int nPosition = 0;
        //            ModBus m_Modbus = new ModBus();
        //            m_Modbus.ConnectModbus();
        //            nPosition = m_Modbus.ReadTapeIndexEncoderValue(station);
        //            m_Modbus.CloseModbus();
        //            return nPosition;
        //        }
        //#endif
        #endregion Turret Application

        public void PnP1Data(ref List<LookUpTableOffsetData> PnP1Return)
        {
            var Table = new List<LookUpTableOffsetData>
            {
                new LookUpTableOffsetData{ Angle = 0, XOffset = 0.00, YOffset = 0.00},
                new LookUpTableOffsetData{ Angle = 1, XOffset = -0.14, YOffset = 4.21},
                new LookUpTableOffsetData{ Angle = 2, XOffset = 0.11, YOffset = 3.78},
                new LookUpTableOffsetData{ Angle = 3, XOffset = 0.31, YOffset = 7.00},
                new LookUpTableOffsetData{ Angle = 4, XOffset = 1.66, YOffset = 9.52},
                new LookUpTableOffsetData{ Angle = 5, XOffset = 1.03, YOffset = 10.49},
                new LookUpTableOffsetData{ Angle = 6, XOffset = 1.95, YOffset = 12.93},
                new LookUpTableOffsetData{ Angle = 7, XOffset = 2.24, YOffset = 15.25},
                new LookUpTableOffsetData{ Angle = 8, XOffset = 3.17, YOffset = 17.00},
                new LookUpTableOffsetData{ Angle = 9, XOffset = 3.46, YOffset = 18.77},
                new LookUpTableOffsetData{ Angle = 10, XOffset = 3.08, YOffset = 19.91},

                new LookUpTableOffsetData{ Angle = 11, XOffset = 3.87, YOffset = 22.44},
                new LookUpTableOffsetData{ Angle = 12, XOffset = 3.6, YOffset = 24.04},
                new LookUpTableOffsetData{ Angle = 13, XOffset = 5.19, YOffset = 27.09},
                new LookUpTableOffsetData{ Angle = 14, XOffset = 4.45, YOffset = 27.43},
                new LookUpTableOffsetData{ Angle = 15, XOffset = 5.64, YOffset = 29.97},
                new LookUpTableOffsetData{ Angle = 16, XOffset = 7.50, YOffset = 31.33},
                new LookUpTableOffsetData{ Angle = 17, XOffset = 6.22, YOffset = 33.68},
                new LookUpTableOffsetData{ Angle = 18, XOffset = 8.17, YOffset = 35.58},
                new LookUpTableOffsetData{ Angle = 19, XOffset = 8.99, YOffset = 36.94},
                new LookUpTableOffsetData{ Angle = 20, XOffset = 9.3, YOffset = 39.55},

                new LookUpTableOffsetData{ Angle = 21, XOffset = 10.85, YOffset = 40.94},
                new LookUpTableOffsetData{ Angle = 22, XOffset = 12.29, YOffset = 43.17},
                new LookUpTableOffsetData{ Angle = 23, XOffset = 13.32, YOffset = 44.76},
                new LookUpTableOffsetData{ Angle = 24, XOffset = 14.27, YOffset = 46.28},
                new LookUpTableOffsetData{ Angle = 25, XOffset = 14.20, YOffset = 48.11},
                new LookUpTableOffsetData{ Angle = 26, XOffset = 15, YOffset = 49.26},
                new LookUpTableOffsetData{ Angle = 27, XOffset = 17, YOffset = 52.1},
                new LookUpTableOffsetData{ Angle = 28, XOffset = 18.59, YOffset = 54.2},
                new LookUpTableOffsetData{ Angle = 29, XOffset = 19.13, YOffset = 55.42},
                new LookUpTableOffsetData{ Angle = 30, XOffset = 21.07, YOffset = 55.98},

                new LookUpTableOffsetData{ Angle = 31, XOffset = 21.65, YOffset = 59.01},
                new LookUpTableOffsetData{ Angle = 32, XOffset = 24.84, YOffset = 60.97},
                new LookUpTableOffsetData{ Angle = 33, XOffset = 25.46, YOffset = 61.84},
                new LookUpTableOffsetData{ Angle = 34, XOffset = 27.05, YOffset = 63.5},
                new LookUpTableOffsetData{ Angle = 35, XOffset = 28.49, YOffset = 65},
                new LookUpTableOffsetData{ Angle = 36, XOffset = 28.46, YOffset = 65.7},
                new LookUpTableOffsetData{ Angle = 37, XOffset = 30.1, YOffset = 66.97},
                new LookUpTableOffsetData{ Angle = 38, XOffset = 31.87, YOffset = 68.84},
                new LookUpTableOffsetData{ Angle = 39, XOffset = 32.46, YOffset = 70},
                new LookUpTableOffsetData{ Angle = 40, XOffset = 33.83, YOffset = 70.75},

                new LookUpTableOffsetData{ Angle = 41, XOffset = 36.3, YOffset = 73.18},
                new LookUpTableOffsetData{ Angle = 42, XOffset = 37.36, YOffset = 74.48},
                new LookUpTableOffsetData{ Angle = 43, XOffset = 38.79, YOffset = 75.05},
                new LookUpTableOffsetData{ Angle = 44, XOffset = 39.12, YOffset = 76.02},
                new LookUpTableOffsetData{ Angle = 45, XOffset = 43.54, YOffset = 76.72},
                new LookUpTableOffsetData{ Angle = 46, XOffset = 43.84, YOffset = 78.5},
                new LookUpTableOffsetData{ Angle = 47, XOffset = 44.08, YOffset = 78.23},
                new LookUpTableOffsetData{ Angle = 48, XOffset = 46.36, YOffset = 78.75},
                new LookUpTableOffsetData{ Angle = 49, XOffset = 47.65, YOffset = 80.27},
                new LookUpTableOffsetData{ Angle = 50, XOffset = 48.62, YOffset = 79.7},

                new LookUpTableOffsetData{ Angle = 51, XOffset = 49.8, YOffset = 80.07},
                new LookUpTableOffsetData{ Angle = 52, XOffset = 50.58, YOffset = 79.91},
                new LookUpTableOffsetData{ Angle = 53, XOffset = 53.21, YOffset = 81.81},
                new LookUpTableOffsetData{ Angle = 54, XOffset = 56.17, YOffset = 82.79},
                new LookUpTableOffsetData{ Angle = 55, XOffset = 58.13, YOffset = 81.64},
                new LookUpTableOffsetData{ Angle = 56, XOffset = 59.35, YOffset = 82.43},
                new LookUpTableOffsetData{ Angle = 57, XOffset = 59.25, YOffset = 83.77},
                new LookUpTableOffsetData{ Angle = 58, XOffset = 60.99, YOffset = 83.8},
                new LookUpTableOffsetData{ Angle = 59, XOffset = 62.65, YOffset = 84.22},
                new LookUpTableOffsetData{ Angle = 60, XOffset = 62.29, YOffset = 84.41},

                new LookUpTableOffsetData{ Angle = 61, XOffset = 66.02, YOffset = 88.73},
                new LookUpTableOffsetData{ Angle = 62, XOffset = 67.23, YOffset = 85.48},
                new LookUpTableOffsetData{ Angle = 63, XOffset = 69.77, YOffset = 84.96},
                new LookUpTableOffsetData{ Angle = 64, XOffset = 70.34, YOffset = 86.1},
                new LookUpTableOffsetData{ Angle = 65, XOffset = 70.4, YOffset = 86.45},
                new LookUpTableOffsetData{ Angle = 66, XOffset = 73.99, YOffset = 86.23},
                new LookUpTableOffsetData{ Angle = 67, XOffset = 74.7, YOffset = 86.83},
                new LookUpTableOffsetData{ Angle = 68, XOffset = 74.81, YOffset = 87.25},
                new LookUpTableOffsetData{ Angle = 69, XOffset = 78.36, YOffset = 86.14},
                new LookUpTableOffsetData{ Angle = 70, XOffset = 78.64, YOffset = 86.61},

                new LookUpTableOffsetData{ Angle = 71, XOffset = 82.57, YOffset = 86.43},
                new LookUpTableOffsetData{ Angle = 72, XOffset = 84.55, YOffset = 86.47},
                new LookUpTableOffsetData{ Angle = 73, XOffset = 84.3, YOffset = 86.77},
                new LookUpTableOffsetData{ Angle = 74, XOffset = 85.68, YOffset = 86.43},
                new LookUpTableOffsetData{ Angle = 75, XOffset = 87.74, YOffset = 86.9},
                new LookUpTableOffsetData{ Angle = 76, XOffset = 89.6, YOffset = 86.61},
                new LookUpTableOffsetData{ Angle = 77, XOffset = 94.91, YOffset = 87.28},
                new LookUpTableOffsetData{ Angle = 78, XOffset = 94.11, YOffset = 86.84},
                new LookUpTableOffsetData{ Angle = 79, XOffset = 96.07, YOffset = 87.05},
                new LookUpTableOffsetData{ Angle = 80, XOffset = 98.59, YOffset = 86.66},

                new LookUpTableOffsetData{ Angle = 81, XOffset = 99.73, YOffset = 86.55},
                new LookUpTableOffsetData{ Angle = 82, XOffset = 102.1, YOffset = 87.13},
                new LookUpTableOffsetData{ Angle = 83, XOffset = 102.74, YOffset = 86.76},
                new LookUpTableOffsetData{ Angle = 84, XOffset = 105.98, YOffset = 87.82},
                new LookUpTableOffsetData{ Angle = 85, XOffset = 107.2, YOffset = 86.26},
                new LookUpTableOffsetData{ Angle = 86, XOffset = 109.66, YOffset = 86.26},
                new LookUpTableOffsetData{ Angle = 87, XOffset = 112.36, YOffset = 85.63},
                new LookUpTableOffsetData{ Angle = 88, XOffset = 113.49, YOffset = 85.69},
                new LookUpTableOffsetData{ Angle = 89, XOffset = 117.51, YOffset = 85.71},
                new LookUpTableOffsetData{ Angle = 90, XOffset = 118.67, YOffset = 84.85},

                new LookUpTableOffsetData{ Angle = 91, XOffset = 119.5, YOffset = 86.1},
                new LookUpTableOffsetData{ Angle = 92, XOffset = 124.08, YOffset = 83.46},
                new LookUpTableOffsetData{ Angle = 93, XOffset = 124.39, YOffset = 85.15},
                new LookUpTableOffsetData{ Angle = 94, XOffset = 126.22, YOffset = 84.21},
                new LookUpTableOffsetData{ Angle = 95, XOffset = 127.19, YOffset = 83.9},
                new LookUpTableOffsetData{ Angle = 96, XOffset = 129.9, YOffset = 83.03},
                new LookUpTableOffsetData{ Angle = 97, XOffset = 133.45, YOffset = 81.94},
                new LookUpTableOffsetData{ Angle = 98, XOffset = 133.34, YOffset = 82.22},
                new LookUpTableOffsetData{ Angle = 99, XOffset = 135.36, YOffset = 80.51},
                new LookUpTableOffsetData{ Angle = 100, XOffset = 137.21, YOffset = 80.04},

                new LookUpTableOffsetData{ Angle = 101, XOffset = 139.27, YOffset = 78.9},
                new LookUpTableOffsetData{ Angle = 102, XOffset = 141.29, YOffset = 79.16},
                new LookUpTableOffsetData{ Angle = 103, XOffset = 144.34, YOffset = 76.930},
                new LookUpTableOffsetData{ Angle = 104, XOffset = 148, YOffset = 74.82},
                new LookUpTableOffsetData{ Angle = 105, XOffset = 147.8, YOffset = 76.1},
                new LookUpTableOffsetData{ Angle = 106, XOffset = 149.03, YOffset = 75.39},
                new LookUpTableOffsetData{ Angle = 107, XOffset = 151.81, YOffset = 74.72},
                new LookUpTableOffsetData{ Angle = 108, XOffset = 152.96, YOffset = 73.48},
                new LookUpTableOffsetData{ Angle = 109, XOffset = 156.32, YOffset = 71.14},
                new LookUpTableOffsetData{ Angle = 110, XOffset = 156.04, YOffset = 71.98},

                new LookUpTableOffsetData{ Angle = 111, XOffset = 160, YOffset = 69.7},
                new LookUpTableOffsetData{ Angle = 112, XOffset = 161.69, YOffset = 67.3},
                new LookUpTableOffsetData{ Angle = 113, XOffset = 163.75, YOffset = 65.43},
                new LookUpTableOffsetData{ Angle = 114, XOffset = 162.89, YOffset = 68.01},
                new LookUpTableOffsetData{ Angle = 115, XOffset = 165.53, YOffset = 66.35},
                new LookUpTableOffsetData{ Angle = 116, XOffset = 166.32, YOffset = 65.2},
                new LookUpTableOffsetData{ Angle = 117, XOffset = 169.54, YOffset = 63.83},
                new LookUpTableOffsetData{ Angle = 118, XOffset = 172.33, YOffset = 59.33},
                new LookUpTableOffsetData{ Angle = 119, XOffset = 172.66, YOffset = 61.6},
                new LookUpTableOffsetData{ Angle = 120, XOffset = 173.83, YOffset = 61.21},

                new LookUpTableOffsetData{ Angle = 121, XOffset = 174.76, YOffset = 60.21},
                new LookUpTableOffsetData{ Angle = 122, XOffset = 179.13, YOffset = 54.57},
                new LookUpTableOffsetData{ Angle = 123, XOffset = 178.27, YOffset = 57.38},
                new LookUpTableOffsetData{ Angle = 124, XOffset = 179.57, YOffset = 56.65},
                new LookUpTableOffsetData{ Angle = 125, XOffset = 181.26, YOffset = 54.74},
                new LookUpTableOffsetData{ Angle = 126, XOffset = 183.03, YOffset = 53.59},
                new LookUpTableOffsetData{ Angle = 127, XOffset = 183.9, YOffset = 50.57},
                new LookUpTableOffsetData{ Angle = 128, XOffset = 185.6, YOffset = 50.1},
                new LookUpTableOffsetData{ Angle = 129, XOffset = 187.49, YOffset = 47.31},
                new LookUpTableOffsetData{ Angle = 130, XOffset = 188.53, YOffset = 47.09},

                new LookUpTableOffsetData{ Angle = 131, XOffset = 189.9, YOffset = 44.98},
                new LookUpTableOffsetData{ Angle = 132, XOffset = 190.6, YOffset = 43.41},
                new LookUpTableOffsetData{ Angle = 133, XOffset = 192.65, YOffset = 41.73},
                new LookUpTableOffsetData{ Angle = 134, XOffset = 193.55, YOffset = 40.09},
                new LookUpTableOffsetData{ Angle = 135, XOffset = 194.46, YOffset = 39.32},
                new LookUpTableOffsetData{ Angle = 136, XOffset = 196.50, YOffset = 37.26},
                new LookUpTableOffsetData{ Angle = 137, XOffset = 198.84, YOffset = 35.01},
                new LookUpTableOffsetData{ Angle = 138, XOffset = 199.46, YOffset = 33.3},
                new LookUpTableOffsetData{ Angle = 139, XOffset =  200.37, YOffset = 29.41},
                new LookUpTableOffsetData{ Angle = 140, XOffset = 207.96, YOffset = 27.47},

                new LookUpTableOffsetData{ Angle = 141, XOffset = 202.06, YOffset = 29.82},
                new LookUpTableOffsetData{ Angle = 142, XOffset = 203.11, YOffset = 27.63},
                new LookUpTableOffsetData{ Angle = 143, XOffset = 204.33, YOffset = 25.98},
                new LookUpTableOffsetData{ Angle = 144, XOffset = 205.53, YOffset = 24.44},
                new LookUpTableOffsetData{ Angle = 145, XOffset = 20.61, YOffset = 18.82},
                new LookUpTableOffsetData{ Angle = 146, XOffset = 207.09, YOffset = 20.28},
                new LookUpTableOffsetData{ Angle = 147, XOffset = 208.92, YOffset = 19.18},
                new LookUpTableOffsetData{ Angle = 148, XOffset = 209.39, YOffset = 16.45},
                new LookUpTableOffsetData{ Angle = 149, XOffset = 209.98, YOffset = 15.02},
                new LookUpTableOffsetData{ Angle = 150, XOffset = 212.91, YOffset = 13.92},

                new LookUpTableOffsetData{ Angle = 151, XOffset = 212.48, YOffset = 9.84},
                new LookUpTableOffsetData{ Angle = 152, XOffset = 211.96, YOffset = 8.6},
                new LookUpTableOffsetData{ Angle = 153, XOffset = 213.55, YOffset = 7.12},
                new LookUpTableOffsetData{ Angle = 154, XOffset = 214.42, YOffset = 0.38},
                new LookUpTableOffsetData{ Angle = 155, XOffset = 214.53, YOffset = 0.3},
                new LookUpTableOffsetData{ Angle = 156, XOffset = 215.59, YOffset = -0.32},
                new LookUpTableOffsetData{ Angle = 157, XOffset = 216.39, YOffset = -3.35},
                new LookUpTableOffsetData{ Angle = 158, XOffset = 216.47, YOffset = -7.09},
                new LookUpTableOffsetData{ Angle = 159, XOffset = 217.52, YOffset = -4.7},
                new LookUpTableOffsetData{ Angle = 160, XOffset = 217.43, YOffset = -6.92},

                new LookUpTableOffsetData{ Angle = 161, XOffset = 218.47, YOffset = -9.01},
                new LookUpTableOffsetData{ Angle = 162, XOffset = 218.19, YOffset = -13.92},
                new LookUpTableOffsetData{ Angle = 163, XOffset = 219, YOffset = -16.92},
                new LookUpTableOffsetData{ Angle = 164, XOffset = 219.09, YOffset = -15.13},
                new LookUpTableOffsetData{ Angle = 165, XOffset = 219.23, YOffset = -17.23},
                new LookUpTableOffsetData{ Angle = 166, XOffset = 220.43, YOffset = -20.31},
                new LookUpTableOffsetData{ Angle = 167, XOffset = 218.58, YOffset = -26.66},
                new LookUpTableOffsetData{ Angle = 168, XOffset = 219.08, YOffset = -26.88},
                new LookUpTableOffsetData{ Angle = 169, XOffset = 220.37, YOffset = -26.94},
                new LookUpTableOffsetData{ Angle = 170, XOffset = 218.67, YOffset = -31.84},

                new LookUpTableOffsetData{ Angle = 171, XOffset = 220.61, YOffset = -30.49},
                new LookUpTableOffsetData{ Angle = 172, XOffset = 219.5, YOffset = -34.24},
                new LookUpTableOffsetData{ Angle = 173, XOffset = 220.29, YOffset = -35.5},
                new LookUpTableOffsetData{ Angle = 174, XOffset = 220.3, YOffset = -36.31},
                new LookUpTableOffsetData{ Angle = 175, XOffset = 220.78, YOffset = -38.31},
                new LookUpTableOffsetData{ Angle = 176, XOffset = 218.48, YOffset = -43.33},
                new LookUpTableOffsetData{ Angle = 177, XOffset = 220.18, YOffset = -43.07},
                new LookUpTableOffsetData{ Angle = 178, XOffset = 218.19, YOffset = -45.64},
                new LookUpTableOffsetData{ Angle = 179, XOffset = 218.41, YOffset = -47.99},
                new LookUpTableOffsetData{ Angle = 180, XOffset = 217.9, YOffset = -49.34},

                new LookUpTableOffsetData{ Angle = 181, XOffset = 217.15, YOffset = -51.74},
                new LookUpTableOffsetData{ Angle = 182, XOffset = 215.27, YOffset = -58.71},
                new LookUpTableOffsetData{ Angle = 183, XOffset = 214.77, YOffset = -59.39},
                new LookUpTableOffsetData{ Angle = 184, XOffset = 213.31, YOffset = -62.73},
                new LookUpTableOffsetData{ Angle = 185, XOffset = 215.96, YOffset = -61.49},
                new LookUpTableOffsetData{ Angle = 186, XOffset = 215.97, YOffset = -61.66},
                new LookUpTableOffsetData{ Angle = 187, XOffset = 215.03, YOffset = -63.84},
                new LookUpTableOffsetData{ Angle = 188, XOffset = 215.12, YOffset = -65.45},
                new LookUpTableOffsetData{ Angle = 189, XOffset = 210.92, YOffset = -71.31},
                new LookUpTableOffsetData{ Angle = 190, XOffset = 213.42, YOffset = -69.95},

                new LookUpTableOffsetData{ Angle = 191, XOffset = 212.54, YOffset = -71.9},
                new LookUpTableOffsetData{ Angle = 192, XOffset = 210.31, YOffset = -73.42},
                new LookUpTableOffsetData{ Angle = 193, XOffset = 209.81, YOffset = -77.33},
                new LookUpTableOffsetData{ Angle = 194, XOffset = 208.84, YOffset = -78.84},
                new LookUpTableOffsetData{ Angle = 195, XOffset = 208.39, YOffset = -80.39},
                new LookUpTableOffsetData{ Angle = 196, XOffset = 208.59, YOffset = -81.43},
                new LookUpTableOffsetData{ Angle = 197, XOffset = 208.18, YOffset = -81.41},
                new LookUpTableOffsetData{ Angle = 198, XOffset = 206.8, YOffset = -84.23},
                new LookUpTableOffsetData{ Angle = 199, XOffset = 205.16, YOffset = -85.91},
                new LookUpTableOffsetData{ Angle = 200, XOffset = 202.09, YOffset = -88.38},

                new LookUpTableOffsetData{ Angle = 201, XOffset = 203.8, YOffset = -89.46},
                new LookUpTableOffsetData{ Angle = 202, XOffset = 203.38, YOffset = -90.59},
                new LookUpTableOffsetData{ Angle = 203, XOffset = 201.93, YOffset = -92.08},
                new LookUpTableOffsetData{ Angle = 204, XOffset = 200.34, YOffset = -93.63},
                new LookUpTableOffsetData{ Angle = 205, XOffset = 200.53, YOffset = -94.83},
                new LookUpTableOffsetData{ Angle = 206, XOffset = 199.59, YOffset = -95.75},
                new LookUpTableOffsetData{ Angle = 207, XOffset = 198.49, YOffset = -96.81},
                new LookUpTableOffsetData{ Angle = 208, XOffset = 194.93, YOffset = -99.54},
                new LookUpTableOffsetData{ Angle = 209, XOffset = 194.32, YOffset = -100.85},
                new LookUpTableOffsetData{ Angle = 210, XOffset = 194.03, YOffset = -101.74},

                new LookUpTableOffsetData{ Angle = 211, XOffset = 192.69, YOffset = -102.79},
                new LookUpTableOffsetData{ Angle = 212, XOffset = 191.87, YOffset = -104.76},
                new LookUpTableOffsetData{ Angle = 213, XOffset = 192.12, YOffset = -105.2},
                new LookUpTableOffsetData{ Angle = 214, XOffset = 190, YOffset = -106.97},
                new LookUpTableOffsetData{ Angle = 215, XOffset = 188.37, YOffset = -107.34},
                new LookUpTableOffsetData{ Angle = 216, XOffset = 186.78, YOffset = -109.01},
                new LookUpTableOffsetData{ Angle = 217, XOffset = 184.77, YOffset = -110.36},
                new LookUpTableOffsetData{ Angle = 218, XOffset = 184.26, YOffset = -110.73},
                new LookUpTableOffsetData{ Angle = 219, XOffset = 182.89, YOffset = -112.08},
                new LookUpTableOffsetData{ Angle = 220, XOffset = 182.52, YOffset = -113.21},

                new LookUpTableOffsetData{ Angle = 221, XOffset = 178.59, YOffset = -114.61},
                new LookUpTableOffsetData{ Angle = 222, XOffset = 179.4, YOffset = -115.08},
                new LookUpTableOffsetData{ Angle = 223, XOffset = 177.58, YOffset = -115.63},
                new LookUpTableOffsetData{ Angle = 224, XOffset = 176.26, YOffset = -116.28},
                new LookUpTableOffsetData{ Angle = 225, XOffset = 176.1, YOffset = -118.44},
                new LookUpTableOffsetData{ Angle = 226, XOffset = 172.41, YOffset = -119.26},
                new LookUpTableOffsetData{ Angle = 227, XOffset = 171.43, YOffset = -120.67},
                new LookUpTableOffsetData{ Angle = 228, XOffset = 171.43, YOffset = -120.62},
                new LookUpTableOffsetData{ Angle = 229, XOffset = 168.04, YOffset = -121.87},
                new LookUpTableOffsetData{ Angle = 230, XOffset = 168.38, YOffset = -122.47},

                new LookUpTableOffsetData{ Angle = 231, XOffset = 166.93, YOffset = -122.59},
                new LookUpTableOffsetData{ Angle = 232, XOffset = 161.25, YOffset = -123.76},
                new LookUpTableOffsetData{ Angle = 233, XOffset = 163.86, YOffset = -124.55},
                new LookUpTableOffsetData{ Angle = 234, XOffset = 159.94, YOffset = -125},
                new LookUpTableOffsetData{ Angle = 235, XOffset = 159.89, YOffset = -125.76},
                new LookUpTableOffsetData{ Angle = 236, XOffset = 157.98, YOffset = -126.39},
                new LookUpTableOffsetData{ Angle = 237, XOffset = 156.47, YOffset = -126.38},
                new LookUpTableOffsetData{ Angle = 238, XOffset = 155.17, YOffset = -127.43},
                new LookUpTableOffsetData{ Angle = 239, XOffset = 153.83, YOffset = -127.38},
                new LookUpTableOffsetData{ Angle = 240, XOffset = 151.04, YOffset = -128.37},

                new LookUpTableOffsetData{ Angle = 241, XOffset = 145.95, YOffset = -128.19},
                new LookUpTableOffsetData{ Angle = 242, XOffset = 145.41, YOffset = -129.43},
                new LookUpTableOffsetData{ Angle = 243, XOffset = 146.33, YOffset = -129.46},
                new LookUpTableOffsetData{ Angle = 244, XOffset = 143.5, YOffset = -129.41},
                new LookUpTableOffsetData{ Angle = 245, XOffset = 143.78, YOffset = -129.8},
                new LookUpTableOffsetData{ Angle = 246, XOffset = 142.69, YOffset = -130.19},
                new LookUpTableOffsetData{ Angle = 247, XOffset = 139.91, YOffset = -130.68},
                new LookUpTableOffsetData{ Angle = 248, XOffset = 139.99, YOffset = -130.47},
                new LookUpTableOffsetData{ Angle = 249, XOffset = 136.74, YOffset = -131.29},
                new LookUpTableOffsetData{ Angle = 250, XOffset = 136.05, YOffset = -131.6},

                new LookUpTableOffsetData{ Angle = 251, XOffset = 132.96, YOffset = -130.86},
                new LookUpTableOffsetData{ Angle = 252, XOffset = 129.58, YOffset = -130.87},
                new LookUpTableOffsetData{ Angle = 253, XOffset = 128.29, YOffset = -132.07},
                new LookUpTableOffsetData{ Angle = 254, XOffset = 127.59, YOffset = -133.16},
                new LookUpTableOffsetData{ Angle = 255, XOffset = 124.34, YOffset = -132.39},
                new LookUpTableOffsetData{ Angle = 256, XOffset = 123.17, YOffset = -132.79},
                new LookUpTableOffsetData{ Angle = 257, XOffset = 120.28, YOffset = -132.61},
                new LookUpTableOffsetData{ Angle = 258, XOffset = 120.56, YOffset = -133.09},
                new LookUpTableOffsetData{ Angle = 259, XOffset = 117.63, YOffset = -132.88},
                new LookUpTableOffsetData{ Angle = 260, XOffset = 114.68, YOffset = -133.56},

                new LookUpTableOffsetData{ Angle = 261, XOffset = 113.7, YOffset = -133.08},
                new LookUpTableOffsetData{ Angle = 262, XOffset = 109.25, YOffset = -132.71},
                new LookUpTableOffsetData{ Angle = 263, XOffset = 109.64, YOffset = -133.44},
                new LookUpTableOffsetData{ Angle = 264, XOffset = 105.46, YOffset = -132.29},
                new LookUpTableOffsetData{ Angle = 265, XOffset = 105.63, YOffset = -132.28},
                new LookUpTableOffsetData{ Angle = 266, XOffset = 104, YOffset = -132.96},
                new LookUpTableOffsetData{ Angle = 267, XOffset = 100.98, YOffset = -131.68},
                new LookUpTableOffsetData{ Angle = 268, XOffset = 99.57, YOffset = -131.44},
                new LookUpTableOffsetData{ Angle = 269, XOffset = 96.78, YOffset = -131.33},
                new LookUpTableOffsetData{ Angle = 270, XOffset = 92.94, YOffset = -130.12},

                new LookUpTableOffsetData{ Angle = 271, XOffset = 94.54, YOffset = -130.49},
                new LookUpTableOffsetData{ Angle = 272, XOffset = 86.7, YOffset = -129.22},
                new LookUpTableOffsetData{ Angle = 273, XOffset = 87.66, YOffset = -129.73},
                new LookUpTableOffsetData{ Angle = 274, XOffset = 88.3, YOffset = -130.18},
                new LookUpTableOffsetData{ Angle = 275, XOffset = 83.65, YOffset = -128.4},
                new LookUpTableOffsetData{ Angle = 276, XOffset = 84.27, YOffset = -129.19},
                new LookUpTableOffsetData{ Angle = 277, XOffset = 80.87, YOffset = -129.09},
                new LookUpTableOffsetData{ Angle = 278, XOffset = 80.09, YOffset = -129.15},
                new LookUpTableOffsetData{ Angle = 279, XOffset = 78.37, YOffset = -127.29},
                new LookUpTableOffsetData{ Angle = 280, XOffset = 74.94, YOffset = -127.24},

                new LookUpTableOffsetData{ Angle = 281, XOffset = 73.62, YOffset = -125.98},
                new LookUpTableOffsetData{ Angle = 282, XOffset = 71.56, YOffset = -125.88},
                new LookUpTableOffsetData{ Angle = 283, XOffset = 69.88, YOffset = -125.62},
                new LookUpTableOffsetData{ Angle = 284, XOffset = 68.88, YOffset = -124.29},
                new LookUpTableOffsetData{ Angle = 285, XOffset = 67.73, YOffset = -123.96},
                new LookUpTableOffsetData{ Angle = 286, XOffset = 64.36, YOffset = -122.68},
                new LookUpTableOffsetData{ Angle = 287, XOffset = 61.12, YOffset = -121.72},
                new LookUpTableOffsetData{ Angle = 288, XOffset = 62, YOffset = -121.15},
                new LookUpTableOffsetData{ Angle = 289, XOffset = 58.63, YOffset = -120.1},
                new LookUpTableOffsetData{ Angle = 290, XOffset = 56.73, YOffset = -117.6},

                new LookUpTableOffsetData{ Angle = 291, XOffset = 56.54, YOffset = -118.83},
                new LookUpTableOffsetData{ Angle = 292, XOffset = 54.87, YOffset = -117.58},
                new LookUpTableOffsetData{ Angle = 293, XOffset = 52.46, YOffset = -115.94},
                new LookUpTableOffsetData{ Angle = 294, XOffset = 48.77, YOffset = -111.84},
                new LookUpTableOffsetData{ Angle = 295, XOffset = 48.15, YOffset = -111.59},
                new LookUpTableOffsetData{ Angle = 296, XOffset = 45.91, YOffset = -110.47},
                new LookUpTableOffsetData{ Angle = 297, XOffset = 46.33, YOffset = -111.25},
                new LookUpTableOffsetData{ Angle = 298, XOffset = 44.72, YOffset = -109.98},
                new LookUpTableOffsetData{ Angle = 299, XOffset = 42.55, YOffset = -109.02},
                new LookUpTableOffsetData{ Angle = 300, XOffset = 41.77, YOffset = -107.33},

                new LookUpTableOffsetData{ Angle = 301, XOffset = 39.18, YOffset = -106.52},
                new LookUpTableOffsetData{ Angle = 302, XOffset = 37.97, YOffset = -105.45},
                new LookUpTableOffsetData{ Angle = 303, XOffset = 36.87, YOffset = -103.08},
                new LookUpTableOffsetData{ Angle = 304, XOffset = 35.72, YOffset = -102.43},
                new LookUpTableOffsetData{ Angle = 305, XOffset = 34.22, YOffset = -99.66},
                new LookUpTableOffsetData{ Angle = 306, XOffset = 31.9, YOffset = -97.77},
                new LookUpTableOffsetData{ Angle = 307, XOffset = 30.6, YOffset = -96.75},
                new LookUpTableOffsetData{ Angle = 308, XOffset = 29.75, YOffset = -95.81},
                new LookUpTableOffsetData{ Angle = 309, XOffset = 28.35, YOffset = -94.19},
                new LookUpTableOffsetData{ Angle = 310, XOffset = 28, YOffset = -92.55},

                new LookUpTableOffsetData{ Angle = 311, XOffset = 25.44, YOffset = -90.93},
                new LookUpTableOffsetData{ Angle = 312, XOffset = 24.59, YOffset = -88.99},
                new LookUpTableOffsetData{ Angle = 313, XOffset = 23.85, YOffset = -87.25},
                new LookUpTableOffsetData{ Angle = 314, XOffset = 22.12, YOffset = -85.71},
                new LookUpTableOffsetData{ Angle = 315, XOffset = 20.76, YOffset = -84.26},
                new LookUpTableOffsetData{ Angle = 316, XOffset = 20.04, YOffset = -83.42},
                new LookUpTableOffsetData{ Angle = 317, XOffset = 18.71, YOffset = -81.3},
                new LookUpTableOffsetData{ Angle = 318, XOffset = 18.18, YOffset = -79.25},
                new LookUpTableOffsetData{ Angle = 319, XOffset = 17.03, YOffset = -78.07},
                new LookUpTableOffsetData{ Angle = 320, XOffset = 15.46, YOffset = -74.7},

                new LookUpTableOffsetData{ Angle = 321, XOffset = 15.81, YOffset = -73.88},
                new LookUpTableOffsetData{ Angle = 322, XOffset = 14.66, YOffset = -71.16},
                new LookUpTableOffsetData{ Angle = 323, XOffset = 13.48, YOffset = -69.66},
                new LookUpTableOffsetData{ Angle = 324, XOffset = 12.64, YOffset = -67.23},
                new LookUpTableOffsetData{ Angle = 325, XOffset = 11.23, YOffset = -66.24},
                new LookUpTableOffsetData{ Angle = 326, XOffset = 11.17, YOffset = -64.52},
                new LookUpTableOffsetData{ Angle = 327, XOffset = 9.97, YOffset = -62.55},
                new LookUpTableOffsetData{ Angle = 328, XOffset = 9.54, YOffset = -61.48},
                new LookUpTableOffsetData{ Angle = 329, XOffset = 9.2, YOffset = -58.69},
                new LookUpTableOffsetData{ Angle = 330, XOffset = 7.83, YOffset = -55.75},

                new LookUpTableOffsetData{ Angle = 331, XOffset = 6.98, YOffset = -53.88},
                new LookUpTableOffsetData{ Angle = 332, XOffset = 6.77, YOffset = -51.42},
                new LookUpTableOffsetData{ Angle = 333, XOffset = 5.35, YOffset = -51.91},
                new LookUpTableOffsetData{ Angle = 334, XOffset = 5.53, YOffset = -48.89},
                new LookUpTableOffsetData{ Angle = 335, XOffset = 4.16, YOffset = -46.99},
                new LookUpTableOffsetData{ Angle = 336, XOffset = 3.6, YOffset = -45.65},
                new LookUpTableOffsetData{ Angle = 337, XOffset = 4.12, YOffset = -42.05},
                new LookUpTableOffsetData{ Angle = 338, XOffset = 3.03, YOffset = -41},
                new LookUpTableOffsetData{ Angle = 339, XOffset = 2.34, YOffset = -38.99},
                new LookUpTableOffsetData{ Angle = 340, XOffset = 1.82, YOffset = -37.11},

                new LookUpTableOffsetData{ Angle = 341, XOffset = 2.16, YOffset = -34.43},
                new LookUpTableOffsetData{ Angle = 342, XOffset = 0.81, YOffset = -32.88},
                new LookUpTableOffsetData{ Angle = 343, XOffset = 1.04, YOffset = -29.91},
                new LookUpTableOffsetData{ Angle = 344, XOffset = 0.98, YOffset = -28.8},
                new LookUpTableOffsetData{ Angle = 345, XOffset = -0.14, YOffset = -27.09},
                new LookUpTableOffsetData{ Angle = 346, XOffset = -0.60, YOffset = -25.48},
                new LookUpTableOffsetData{ Angle = 347, XOffset = 0, YOffset = -22.74},
                new LookUpTableOffsetData{ Angle = 348, XOffset = 0.21, YOffset = -17.3},
                new LookUpTableOffsetData{ Angle = 349, XOffset = -0.26, YOffset = -18.74},
                new LookUpTableOffsetData{ Angle = 350, XOffset = -0.9, YOffset = -16.75},

                new LookUpTableOffsetData{ Angle = 351, XOffset = -1.78, YOffset = -15.23},
                new LookUpTableOffsetData{ Angle = 352, XOffset = -0.5, YOffset = -12.39},
                new LookUpTableOffsetData{ Angle = 353, XOffset = -1.39, YOffset = -10.22},
                new LookUpTableOffsetData{ Angle = 354, XOffset = -2.03, YOffset = -7.25},
                new LookUpTableOffsetData{ Angle = 355, XOffset = -1.77, YOffset = -6.63},
                new LookUpTableOffsetData{ Angle = 356, XOffset = -1.49, YOffset = -5.12},
                new LookUpTableOffsetData{ Angle = 357, XOffset = -1.49, YOffset = -2.42},
                new LookUpTableOffsetData{ Angle = 358, XOffset = -1.37, YOffset = -1.13},
                new LookUpTableOffsetData{ Angle = 359, XOffset = -1.38, YOffset = 2.77},
            };

            PnP1Return = Table;
        }

        public void PnP2Data(ref List<LookUpTableOffsetData> PnP2Return)
        {
            var Table = new List<LookUpTableOffsetData>
            {
                new LookUpTableOffsetData{ Angle = 0, XOffset = 0.00, YOffset = 0.00 },
                new LookUpTableOffsetData{ Angle = 1, XOffset = 0.00, YOffset = -1.38 },
                new LookUpTableOffsetData{ Angle = 2, XOffset = 1.38, YOffset = -2.76 },
                new LookUpTableOffsetData{ Angle = 3, XOffset = 3.45, YOffset = -4.83 },
                new LookUpTableOffsetData{ Angle = 4, XOffset = 5.52, YOffset = -6.90 },
                new LookUpTableOffsetData{ Angle = 5, XOffset = 5.52, YOffset = -8.28 },
                new LookUpTableOffsetData{ Angle = 6, XOffset = 8.28, YOffset = -11.04 },
                new LookUpTableOffsetData{ Angle = 7, XOffset = 9.66, YOffset = -12.42 },
                new LookUpTableOffsetData{ Angle = 8, XOffset = 11.04, YOffset = -13.81 },
                new LookUpTableOffsetData{ Angle = 9, XOffset = 12.42, YOffset = -15.19 },
                new LookUpTableOffsetData{ Angle = 10, XOffset = 15.19, YOffset = -16.57 },
                new LookUpTableOffsetData{ Angle = 11, XOffset = 15.19, YOffset = -17.95 },
                new LookUpTableOffsetData{ Angle = 12, XOffset = 16.57, YOffset = -20.71 },
                new LookUpTableOffsetData{ Angle = 13, XOffset = 17.95, YOffset = -20.71 },
                new LookUpTableOffsetData{ Angle = 14, XOffset = 17.95, YOffset = -23.47 },
                new LookUpTableOffsetData{ Angle = 15, XOffset = 17.95, YOffset = -26.23 },
                new LookUpTableOffsetData{ Angle = 16, XOffset = 20.75, YOffset = -29.00 },
                new LookUpTableOffsetData{ Angle = 17, XOffset = 23.44, YOffset = -31.76 },
                new LookUpTableOffsetData{ Angle = 18, XOffset = 24.85, YOffset = -33.14 },
                new LookUpTableOffsetData{ Angle = 19, XOffset = 23.47, YOffset = -34.52 },
                new LookUpTableOffsetData{ Angle = 20, XOffset = 26.23, YOffset = -34.52 },
                new LookUpTableOffsetData{ Angle = 21, XOffset = 27.60, YOffset = -37.28 },
                new LookUpTableOffsetData{ Angle = 22, XOffset = 29.00, YOffset = -40.05 },
                new LookUpTableOffsetData{ Angle = 23, XOffset = 31.05, YOffset = -41.79 },
                new LookUpTableOffsetData{ Angle = 24, XOffset = 30.38, YOffset = -44.19 },
                new LookUpTableOffsetData{ Angle = 25, XOffset = 31.76, YOffset = -45.57 },
                new LookUpTableOffsetData{ Angle = 26, XOffset = 33.14, YOffset = -51.09 },
                new LookUpTableOffsetData{ Angle = 27, XOffset = 37.28, YOffset = -51.09 },
                new LookUpTableOffsetData{ Angle = 28, XOffset = 34.52, YOffset = -51.09 },
                new LookUpTableOffsetData{ Angle = 29, XOffset = 38.66, YOffset = -55.24 },
                new LookUpTableOffsetData{ Angle = 30, XOffset = 37.28, YOffset = -56.62 },
                new LookUpTableOffsetData{ Angle = 31, XOffset = 37.28, YOffset = -59.38 },
                new LookUpTableOffsetData{ Angle = 32, XOffset = 40.04, YOffset = -62.14 },
                new LookUpTableOffsetData{ Angle = 33, XOffset = 40.04, YOffset = -62.14 },
                new LookUpTableOffsetData{ Angle = 34, XOffset = 41.43, YOffset = -64.90 },
                new LookUpTableOffsetData{ Angle = 35, XOffset = 42.81, YOffset = -67.66 },
                new LookUpTableOffsetData{ Angle = 36, XOffset = 40.04, YOffset = -70.43 },
                new LookUpTableOffsetData{ Angle = 37, XOffset = 42.81, YOffset = -71.81 },
                new LookUpTableOffsetData{ Angle = 38, XOffset = 42.81, YOffset = -73.19 },
                new LookUpTableOffsetData{ Angle = 39, XOffset = 42.81, YOffset = -78.71 },
                new LookUpTableOffsetData{ Angle = 40, XOffset = 42.81, YOffset = -78.71 },
                new LookUpTableOffsetData{ Angle = 41, XOffset = 45.57, YOffset = -81.47 },
                new LookUpTableOffsetData{ Angle = 42, XOffset = 45.57, YOffset = -84.24 },
                new LookUpTableOffsetData{ Angle = 43, XOffset = 46.95, YOffset = -88.83 },
                new LookUpTableOffsetData{ Angle = 44, XOffset = 44.19, YOffset = -88.83 },
                new LookUpTableOffsetData{ Angle = 45, XOffset = 45.57, YOffset = -91.14 },
                new LookUpTableOffsetData{ Angle = 46, XOffset = 46.95, YOffset = -91.14 },
                new LookUpTableOffsetData{ Angle = 47, XOffset = 45.57, YOffset = -95.28 },
                new LookUpTableOffsetData{ Angle = 48, XOffset = 45.57, YOffset = -98.05 },
                new LookUpTableOffsetData{ Angle = 49, XOffset = 46.54, YOffset = -100.81 },
                new LookUpTableOffsetData{ Angle = 50, XOffset = 45.57, YOffset = -103.57 },
                new LookUpTableOffsetData{ Angle = 51, XOffset = 45.57, YOffset = -104.95 },
                new LookUpTableOffsetData{ Angle = 52, XOffset = 46.88, YOffset = -107.79 },
                new LookUpTableOffsetData{ Angle = 53, XOffset = 45.57, YOffset = -110.48 },
                new LookUpTableOffsetData{ Angle = 54, XOffset = 45.57, YOffset = -111.56 },
                new LookUpTableOffsetData{ Angle = 55, XOffset = 45.57, YOffset = -114.62 },
                new LookUpTableOffsetData{ Angle = 56, XOffset = 46.95, YOffset = -116.00 },
                new LookUpTableOffsetData{ Angle = 57, XOffset = 45.57, YOffset = -118.76 },
                new LookUpTableOffsetData{ Angle = 58, XOffset = 44.19, YOffset = -120.14 },
                new LookUpTableOffsetData{ Angle = 59, XOffset = 45.57, YOffset = -125.67 },
                new LookUpTableOffsetData{ Angle = 60, XOffset = 44.19, YOffset = -125.67 },
                new LookUpTableOffsetData{ Angle = 61, XOffset = 42.81, YOffset = -128.43 },
                new LookUpTableOffsetData{ Angle = 62, XOffset = 40.04, YOffset = -131.19 },
                new LookUpTableOffsetData{ Angle = 63, XOffset = 41.43, YOffset = -132.57 },
                new LookUpTableOffsetData{ Angle = 64, XOffset = 41.43, YOffset = -135.33 },
                new LookUpTableOffsetData{ Angle = 65, XOffset = 41.43, YOffset = -138.10 },
                new LookUpTableOffsetData{ Angle = 66, XOffset = 40.04, YOffset = -139.48 },
                new LookUpTableOffsetData{ Angle = 67, XOffset = 40.74, YOffset = -141.55 },
                new LookUpTableOffsetData{ Angle = 68, XOffset = 37.28, YOffset = -143.62 },
                new LookUpTableOffsetData{ Angle = 69, XOffset = 37.28, YOffset = -145.00 },
                new LookUpTableOffsetData{ Angle = 70, XOffset = 35.90, YOffset = -147.76 },
                new LookUpTableOffsetData{ Angle = 71, XOffset = 34.52, YOffset = -150.52 },
                new LookUpTableOffsetData{ Angle = 72, XOffset = 33.14, YOffset = -151.95 },
                new LookUpTableOffsetData{ Angle = 73, XOffset = 31.76, YOffset = -156.05 },
                new LookUpTableOffsetData{ Angle = 74, XOffset = 31.76, YOffset = -156.05 },
                new LookUpTableOffsetData{ Angle = 75, XOffset = 30.38, YOffset = -157.43 },
                new LookUpTableOffsetData{ Angle = 76, XOffset = 30.38, YOffset = -160.19 },
                new LookUpTableOffsetData{ Angle = 77, XOffset = 27.60, YOffset = -162.95 },
                new LookUpTableOffsetData{ Angle = 78, XOffset = 26.23, YOffset = -164.33 },
                new LookUpTableOffsetData{ Angle = 79, XOffset = 24.80, YOffset = -165.70 },
                new LookUpTableOffsetData{ Angle = 80, XOffset = 24.80, YOffset = -168.48 },
                new LookUpTableOffsetData{ Angle = 81, XOffset = 22.10, YOffset = -171.20 },
                new LookUpTableOffsetData{ Angle = 82, XOffset = 20.75, YOffset = -172.62 },
                new LookUpTableOffsetData{ Angle = 83, XOffset = 19.33, YOffset = -174.00 },
                new LookUpTableOffsetData{ Angle = 84, XOffset = 17.95, YOffset = -175.38 },
                new LookUpTableOffsetData{ Angle = 85, XOffset = 17.12, YOffset = -177.04 },
                new LookUpTableOffsetData{ Angle = 86, XOffset = 15.19, YOffset = -180.91 },
                new LookUpTableOffsetData{ Angle = 87, XOffset = 11.04, YOffset = -180.91 },
                new LookUpTableOffsetData{ Angle = 88, XOffset = 9.66, YOffset = -183.67 },
                new LookUpTableOffsetData{ Angle = 89, XOffset = 9.66, YOffset = -185.05 },
                new LookUpTableOffsetData{ Angle = 90, XOffset = 6.90, YOffset = -185.05 },
                new LookUpTableOffsetData{ Angle = 91, XOffset = 6.90, YOffset = -187.81 },
                new LookUpTableOffsetData{ Angle = 92, XOffset = 4.14, YOffset = -190.57 },
                new LookUpTableOffsetData{ Angle = 93, XOffset = 1.38, YOffset = -190.57 },
                new LookUpTableOffsetData{ Angle = 94, XOffset = 1.38, YOffset = -191.95 },
                new LookUpTableOffsetData{ Angle = 95, XOffset = -2.07, YOffset = -192.65 },
                new LookUpTableOffsetData{ Angle = 96, XOffset = -2.76, YOffset = -196.10 },
                new LookUpTableOffsetData{ Angle = 97, XOffset = -5.52, YOffset = -197.48 },
                new LookUpTableOffsetData{ Angle = 98, XOffset = -6.90, YOffset = -197.48 },
                new LookUpTableOffsetData{ Angle = 99, XOffset = -8.28, YOffset = -200.24 },
                new LookUpTableOffsetData{ Angle = 100, XOffset = -11.04, YOffset = -201.62 },
                new LookUpTableOffsetData{ Angle = 101, XOffset = -13.81, YOffset = -201.62 },
                new LookUpTableOffsetData{ Angle = 102, XOffset = -13.81, YOffset = -203.00 },
                new LookUpTableOffsetData{ Angle = 103, XOffset = -17.95, YOffset = -204.38 },
                new LookUpTableOffsetData{ Angle = 104, XOffset = -17.95, YOffset = -205.76 },
                new LookUpTableOffsetData{ Angle = 105, XOffset = -19.33, YOffset = -207.15 },
                new LookUpTableOffsetData{ Angle = 106, XOffset = -20.71, YOffset = -208.53 },
                new LookUpTableOffsetData{ Angle = 107, XOffset = -23.47, YOffset = -211.29 },
                new LookUpTableOffsetData{ Angle = 108, XOffset = -24.85, YOffset = -211.29 },
                new LookUpTableOffsetData{ Angle = 109, XOffset = -27.62, YOffset = -212.67 },
                new LookUpTableOffsetData{ Angle = 110, XOffset = -29.00, YOffset = -211.29 },
                new LookUpTableOffsetData{ Angle = 111, XOffset = -30.38, YOffset = -212.67 },
                new LookUpTableOffsetData{ Angle = 112, XOffset = -34.52, YOffset = -214.05 },
                new LookUpTableOffsetData{ Angle = 113, XOffset = -34.52, YOffset = -216.81 },
                new LookUpTableOffsetData{ Angle = 114, XOffset = -35.90, YOffset = -216.81 },
                new LookUpTableOffsetData{ Angle = 115, XOffset = -38.66, YOffset = -218.19 },
                new LookUpTableOffsetData{ Angle = 116, XOffset = -41.43, YOffset = -218.19 },
                new LookUpTableOffsetData{ Angle = 117, XOffset = -42.81, YOffset = -219.57 },
                new LookUpTableOffsetData{ Angle = 118, XOffset = -44.19, YOffset = -219.57 },
                new LookUpTableOffsetData{ Angle = 119, XOffset = -45.57, YOffset = -219.07 },
                new LookUpTableOffsetData{ Angle = 120, XOffset = -48.33, YOffset = -223.70 },
                new LookUpTableOffsetData{ Angle = 121, XOffset = -51.10, YOffset = -222.34 },
                new LookUpTableOffsetData{ Angle = 122, XOffset = -52.47, YOffset = -222.34 },
                new LookUpTableOffsetData{ Angle = 123, XOffset = -55.24, YOffset = -223.72 },
                new LookUpTableOffsetData{ Angle = 124, XOffset = -56.62, YOffset = -225.10 },
                new LookUpTableOffsetData{ Angle = 125, XOffset = -59.38, YOffset = -225.10 },
                new LookUpTableOffsetData{ Angle = 126, XOffset = -60.76, YOffset = -225.10 },
                new LookUpTableOffsetData{ Angle = 127, XOffset = -63.52, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 128, XOffset = -64.90, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 129, XOffset = -69.05, YOffset = -226.48 },
                new LookUpTableOffsetData{ Angle = 130, XOffset = -70.43, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 131, XOffset = -73.19, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 132, XOffset = -75.95, YOffset = -229.24 },
                new LookUpTableOffsetData{ Angle = 133, XOffset = -75.95, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 134, XOffset = -80.10, YOffset = -230.62 },
                new LookUpTableOffsetData{ Angle = 135, XOffset = -80.10, YOffset = -229.24 },
                new LookUpTableOffsetData{ Angle = 136, XOffset = -84.24, YOffset = -229.24 },
                new LookUpTableOffsetData{ Angle = 137, XOffset = -84.24, YOffset = -229.24 },
                new LookUpTableOffsetData{ Angle = 138, XOffset = -80.76, YOffset = -230.62 },
                new LookUpTableOffsetData{ Angle = 139, XOffset = -89.76, YOffset = -229.24 },
                new LookUpTableOffsetData{ Angle = 140, XOffset = -93.90, YOffset = -230.62 },
                new LookUpTableOffsetData{ Angle = 141, XOffset = -93.90, YOffset = -229.24 },
                new LookUpTableOffsetData{ Angle = 142, XOffset = -96.60, YOffset = -229.24 },
                new LookUpTableOffsetData{ Angle = 143, XOffset = -98.05, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 144, XOffset = -103.57, YOffset = -230.62 },
                new LookUpTableOffsetData{ Angle = 145, XOffset = -102.19, YOffset = -229.24 },
                new LookUpTableOffsetData{ Angle = 146, XOffset = -103.57, YOffset = -229.24 },
                new LookUpTableOffsetData{ Angle = 147, XOffset = -106.33, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 148, XOffset = -109.80, YOffset = -228.55 },
                new LookUpTableOffsetData{ Angle = 149, XOffset = -111.86, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 150, XOffset = -113.24, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 151, XOffset = -114.62, YOffset = -227.86 },
                new LookUpTableOffsetData{ Angle = 152, XOffset = -116.00, YOffset = -226.48 },
                new LookUpTableOffsetData{ Angle = 153, XOffset = -120.14, YOffset = -226.48 },
                new LookUpTableOffsetData{ Angle = 154, XOffset = -122.90, YOffset = -225.10 },
                new LookUpTableOffsetData{ Angle = 155, XOffset = -125.67, YOffset = -225.10 },
                new LookUpTableOffsetData{ Angle = 156, XOffset = -125.67, YOffset = -225.10 },
                new LookUpTableOffsetData{ Angle = 157, XOffset = -131.19, YOffset = -222.34 },
                new LookUpTableOffsetData{ Angle = 158, XOffset = -131.20, YOffset = -223.72 },
                new LookUpTableOffsetData{ Angle = 159, XOffset = -133.90, YOffset = -222.34 },
                new LookUpTableOffsetData{ Angle = 160, XOffset = -136.71, YOffset = -222.34 },
                new LookUpTableOffsetData{ Angle = 161, XOffset = -138.10, YOffset = -220.96 },
                new LookUpTableOffsetData{ Angle = 162, XOffset = -140.86, YOffset = -219.57 },
                new LookUpTableOffsetData{ Angle = 163, XOffset = -143.62, YOffset = -218.19 },
                new LookUpTableOffsetData{ Angle = 164, XOffset = -145.00, YOffset = -219.57 },
                new LookUpTableOffsetData{ Angle = 165, XOffset = -146.38, YOffset = -215.43 },
                new LookUpTableOffsetData{ Angle = 166, XOffset = -149.14, YOffset = -216.81 },
                new LookUpTableOffsetData{ Angle = 167, XOffset = -101.90, YOffset = -212.67 },
                new LookUpTableOffsetData{ Angle = 168, XOffset = -153.30, YOffset = -214.00 },
                new LookUpTableOffsetData{ Angle = 169, XOffset = -156.00, YOffset = -214.00 },
                new LookUpTableOffsetData{ Angle = 170, XOffset = -157.40, YOffset = -209.90 },
                new LookUpTableOffsetData{ Angle = 171, XOffset = -158.80, YOffset = -209.90 },
                new LookUpTableOffsetData{ Angle = 172, XOffset = -161.57, YOffset = -208.53 },
                new LookUpTableOffsetData{ Angle = 173, XOffset = -164.33, YOffset = -208.53 },
                new LookUpTableOffsetData{ Angle = 174, XOffset = -166.41, YOffset = -206.40 },
                new LookUpTableOffsetData{ Angle = 175, XOffset = -167.10, YOffset = -205.76 },
                new LookUpTableOffsetData{ Angle = 176, XOffset = -167.10, YOffset = -205.76 },
                new LookUpTableOffsetData{ Angle = 177, XOffset = -171.24, YOffset = -203.00 },
                new LookUpTableOffsetData{ Angle = 178, XOffset = -171.24, YOffset = -200.24 },
                new LookUpTableOffsetData{ Angle = 179, XOffset = -173.88, YOffset = -198.42 },
                new LookUpTableOffsetData{ Angle = 180, XOffset = -172.60, YOffset = -200.24 },
                new LookUpTableOffsetData{ Angle = 181, XOffset = -175.38, YOffset = -197.48 },
                new LookUpTableOffsetData{ Angle = 182, XOffset = -180.90, YOffset = -194.72 },
                new LookUpTableOffsetData{ Angle = 183, XOffset = -180.22, YOffset = -194.03 },
                new LookUpTableOffsetData{ Angle = 184, XOffset = -182.30, YOffset = -190.57 },
                new LookUpTableOffsetData{ Angle = 185, XOffset = -185.05, YOffset = -190.60 },
                new LookUpTableOffsetData{ Angle = 186, XOffset = -186.43, YOffset = -189.19 },
                new LookUpTableOffsetData{ Angle = 187, XOffset = -186.43, YOffset = -186.43 },
                new LookUpTableOffsetData{ Angle = 188, XOffset = -189.20, YOffset = -185.05 },
                new LookUpTableOffsetData{ Angle = 189, XOffset = -191.95, YOffset = -183.67 },
                new LookUpTableOffsetData{ Angle = 190, XOffset = -191.95, YOffset = -181.00 },
                new LookUpTableOffsetData{ Angle = 191, XOffset = -196.10, YOffset = -179.50 },
                new LookUpTableOffsetData{ Angle = 192, XOffset = -193.34, YOffset = -176.78 },
                new LookUpTableOffsetData{ Angle = 193, XOffset = -194.72, YOffset = -174.00 },
                new LookUpTableOffsetData{ Angle = 194, XOffset = -194.70, YOffset = -175.30 },
                new LookUpTableOffsetData{ Angle = 195, XOffset = -197.48, YOffset = -172.60 },
                new LookUpTableOffsetData{ Angle = 196, XOffset = -198.80, YOffset = -171.20 },
                new LookUpTableOffsetData{ Angle = 197, XOffset = -200.25, YOffset = -169.80 },
                new LookUpTableOffsetData{ Angle = 198, XOffset = -203.00, YOffset = -167.10 },
                new LookUpTableOffsetData{ Angle = 199, XOffset = -203.00, YOffset = -164.40 },
                new LookUpTableOffsetData{ Angle = 200, XOffset = -204.20, YOffset = -162.60 },
                new LookUpTableOffsetData{ Angle = 201, XOffset = -204.45, YOffset = -160.44 },
                new LookUpTableOffsetData{ Angle = 202, XOffset = -207.15, YOffset = -160.20 },
                new LookUpTableOffsetData{ Angle = 203, XOffset = -207.15, YOffset = -157.43 },
                new LookUpTableOffsetData{ Angle = 204, XOffset = -207.00, YOffset = -154.50 },
                new LookUpTableOffsetData{ Angle = 205, XOffset = -208.50, YOffset = -152.00 },
                new LookUpTableOffsetData{ Angle = 206, XOffset = -210.00, YOffset = -150.50 },
                new LookUpTableOffsetData{ Angle = 207, XOffset = -210.00, YOffset = -147.70 },
                new LookUpTableOffsetData{ Angle = 208, XOffset = -211.30, YOffset = -147.70 },
                new LookUpTableOffsetData{ Angle = 209, XOffset = -212.60, YOffset = -143.60 },
                new LookUpTableOffsetData{ Angle = 210, XOffset = -212.60, YOffset = -140.90 },
                new LookUpTableOffsetData{ Angle = 211, XOffset = -214.00, YOffset = -139.50 },
                new LookUpTableOffsetData{ Angle = 212, XOffset = -214.00, YOffset = -136.70 },
                new LookUpTableOffsetData{ Angle = 213, XOffset = -215.40, YOffset = -133.95 },
                new LookUpTableOffsetData{ Angle = 214, XOffset = -216.80, YOffset = -131.20 },
                new LookUpTableOffsetData{ Angle = 215, XOffset = -216.80, YOffset = -128.40 },
                new LookUpTableOffsetData{ Angle = 216, XOffset = -219.50, YOffset = -125.70 },
                new LookUpTableOffsetData{ Angle = 217, XOffset = -219.50, YOffset = -125.60 },
                new LookUpTableOffsetData{ Angle = 218, XOffset = -218.20, YOffset = -121.50 },
                new LookUpTableOffsetData{ Angle = 219, XOffset = -219.50, YOffset = -120.10 },
                new LookUpTableOffsetData{ Angle = 220, XOffset = -219.50, YOffset = -117.40 },
                new LookUpTableOffsetData{ Angle = 221, XOffset = -219.50, YOffset = -114.60 },
                new LookUpTableOffsetData{ Angle = 222, XOffset = -220.90, YOffset = -113.20 },
                new LookUpTableOffsetData{ Angle = 223, XOffset = -219.50, YOffset = -110.48 },
                new LookUpTableOffsetData{ Angle = 224, XOffset = -219.50, YOffset = -109.10 },
                new LookUpTableOffsetData{ Angle = 225, XOffset = -219.50, YOffset = -106.37 },
                new LookUpTableOffsetData{ Angle = 226, XOffset = -222.34, YOffset = -103.50 },
                new LookUpTableOffsetData{ Angle = 227, XOffset = -222.34, YOffset = -100.80 },
                new LookUpTableOffsetData{ Angle = 228, XOffset = -220.90, YOffset = -99.40 },
                new LookUpTableOffsetData{ Angle = 229, XOffset = -220.90, YOffset = -96.67 },
                new LookUpTableOffsetData{ Angle = 230, XOffset = -220.90, YOffset = -93.90 },
                new LookUpTableOffsetData{ Angle = 231, XOffset = -220.96, YOffset = -91.14 },
                new LookUpTableOffsetData{ Angle = 232, XOffset = -219.50, YOffset = -89.76 },
                new LookUpTableOffsetData{ Angle = 233, XOffset = -219.50, YOffset = -87.00 },
                new LookUpTableOffsetData{ Angle = 234, XOffset = -219.50, YOffset = -84.24 },
                new LookUpTableOffsetData{ Angle = 235, XOffset = -219.50, YOffset = -81.47 },
                new LookUpTableOffsetData{ Angle = 236, XOffset = -218.20, YOffset = -80.10 },
                new LookUpTableOffsetData{ Angle = 237, XOffset = -219.50, YOffset = -74.33 },
                new LookUpTableOffsetData{ Angle = 238, XOffset = -218.20, YOffset = -74.50 },
                new LookUpTableOffsetData{ Angle = 239, XOffset = -218.20, YOffset = -73.20 },
                new LookUpTableOffsetData{ Angle = 240, XOffset = -218.20, YOffset = -70.40 },
                new LookUpTableOffsetData{ Angle = 241, XOffset = -218.20, YOffset = -67.70 },
                new LookUpTableOffsetData{ Angle = 242, XOffset = -215.40, YOffset = -65.00 },
                new LookUpTableOffsetData{ Angle = 243, XOffset = -216.80, YOffset = -62.10 },
                new LookUpTableOffsetData{ Angle = 244, XOffset = -216.80, YOffset = -62.10 },
                new LookUpTableOffsetData{ Angle = 245, XOffset = -214.00, YOffset = -59.40 },
                new LookUpTableOffsetData{ Angle = 246, XOffset = -214.00, YOffset = -56.60 },
                new LookUpTableOffsetData{ Angle = 247, XOffset = -214.00, YOffset = -55.20 },
                new LookUpTableOffsetData{ Angle = 248, XOffset = -209.90, YOffset = -52.47 },
                new LookUpTableOffsetData{ Angle = 249, XOffset = -211.30, YOffset = -49.70 },
                new LookUpTableOffsetData{ Angle = 250, XOffset = -211.30, YOffset = -48.30 },
                new LookUpTableOffsetData{ Angle = 251, XOffset = -209.90, YOffset = -45.50 },
                new LookUpTableOffsetData{ Angle = 252, XOffset = -207.10, YOffset = -44.20 },
                new LookUpTableOffsetData{ Angle = 253, XOffset = -207.15, YOffset = -41.43 },
                new LookUpTableOffsetData{ Angle = 254, XOffset = -207.10, YOffset = -40.00 },
                new LookUpTableOffsetData{ Angle = 255, XOffset = -205.70, YOffset = -37.20 },
                new LookUpTableOffsetData{ Angle = 256, XOffset = -205.70, YOffset = -37.30 },
                new LookUpTableOffsetData{ Angle = 257, XOffset = -204.30, YOffset = -34.50 },
                new LookUpTableOffsetData{ Angle = 258, XOffset = -203.00, YOffset = -31.70 },
                new LookUpTableOffsetData{ Angle = 259, XOffset = -201.60, YOffset = -29.00 },
                new LookUpTableOffsetData{ Angle = 260, XOffset = -200.20, YOffset = -29.00 },
                new LookUpTableOffsetData{ Angle = 261, XOffset = -200.20, YOffset = -26.23 },
                new LookUpTableOffsetData{ Angle = 262, XOffset = -197.40, YOffset = -23.40 },
                new LookUpTableOffsetData{ Angle = 263, XOffset = -196.10, YOffset = -22.10 },
                new LookUpTableOffsetData{ Angle = 264, XOffset = -194.70, YOffset = -20.70 },
                new LookUpTableOffsetData{ Angle = 265, XOffset = -193.30, YOffset = -19.30 },
                new LookUpTableOffsetData{ Angle = 266, XOffset = -192.00, YOffset = -17.90 },
                new LookUpTableOffsetData{ Angle = 267, XOffset = -190.50, YOffset = -15.20 },
                new LookUpTableOffsetData{ Angle = 268, XOffset = -189.20, YOffset = -12.40 },
                new LookUpTableOffsetData{ Angle = 269, XOffset = -186.40, YOffset = -12.40 },
                new LookUpTableOffsetData{ Angle = 270, XOffset = -186.40, YOffset = -9.60 },
                new LookUpTableOffsetData{ Angle = 271, XOffset = -183.60, YOffset = -9.60 },
                new LookUpTableOffsetData{ Angle = 272, XOffset = -182.30, YOffset = -6.90 },
                new LookUpTableOffsetData{ Angle = 273, XOffset = -178.10, YOffset = -4.10 },
                new LookUpTableOffsetData{ Angle = 274, XOffset = -178.80, YOffset = -2.10 },
                new LookUpTableOffsetData{ Angle = 275, XOffset = -176.70, YOffset = -1.38 },
                new LookUpTableOffsetData{ Angle = 276, XOffset = -174.00, YOffset = 1.38 },
                new LookUpTableOffsetData{ Angle = 277, XOffset = -169.80, YOffset = 1.38 },
                new LookUpTableOffsetData{ Angle = 278, XOffset = -171.20, YOffset = 1.38 },
                new LookUpTableOffsetData{ Angle = 279, XOffset = -167.10, YOffset = 4.10 },
                new LookUpTableOffsetData{ Angle = 280, XOffset = -166.10, YOffset = 5.57 },
                new LookUpTableOffsetData{ Angle = 281, XOffset = -164.30, YOffset = 6.90 },
                new LookUpTableOffsetData{ Angle = 282, XOffset = -161.90, YOffset = 6.90 },
                new LookUpTableOffsetData{ Angle = 283, XOffset = -161.50, YOffset = 9.66 },
                new LookUpTableOffsetData{ Angle = 284, XOffset = -158.80, YOffset = 11.00 },
                new LookUpTableOffsetData{ Angle = 285, XOffset = -157.40, YOffset = 12.40 },
                new LookUpTableOffsetData{ Angle = 286, XOffset = -153.20, YOffset = 12.40 },
                new LookUpTableOffsetData{ Angle = 287, XOffset = -152.60, YOffset = 11.70 },
                new LookUpTableOffsetData{ Angle = 288, XOffset = -150.50, YOffset = 15.20 },
                new LookUpTableOffsetData{ Angle = 289, XOffset = -147.70, YOffset = 16.50 },
                new LookUpTableOffsetData{ Angle = 290, XOffset = -145.70, YOffset = 17.26 },
                new LookUpTableOffsetData{ Angle = 291, XOffset = -143.60, YOffset = 18.00 },
                new LookUpTableOffsetData{ Angle = 292, XOffset = -142.20, YOffset = 18.00 },
                new LookUpTableOffsetData{ Angle = 293, XOffset = -139.40, YOffset = 20.70 },
                new LookUpTableOffsetData{ Angle = 294, XOffset = -138.00, YOffset = 20.70 },
                new LookUpTableOffsetData{ Angle = 295, XOffset = -136.70, YOffset = 20.70 },
                new LookUpTableOffsetData{ Angle = 296, XOffset = -133.90, YOffset = 22.10 },
                new LookUpTableOffsetData{ Angle = 297, XOffset = -132.50, YOffset = 22.10 },
                new LookUpTableOffsetData{ Angle = 298, XOffset = -130.50, YOffset = 22.78 },
                new LookUpTableOffsetData{ Angle = 299, XOffset = -129.80, YOffset = 22.10 },
                new LookUpTableOffsetData{ Angle = 300, XOffset = -125.60, YOffset = 24.90 },
                new LookUpTableOffsetData{ Angle = 301, XOffset = -122.90, YOffset = 26.20 },
                new LookUpTableOffsetData{ Angle = 302, XOffset = -122.90, YOffset = 26.20 },
                new LookUpTableOffsetData{ Angle = 303, XOffset = -121.50, YOffset = 26.20 },
                new LookUpTableOffsetData{ Angle = 304, XOffset = -118.80, YOffset = 27.50 },
                new LookUpTableOffsetData{ Angle = 305, XOffset = -114.60, YOffset = 26.20 },
                new LookUpTableOffsetData{ Angle = 306, XOffset = -114.60, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 307, XOffset = -113.20, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 308, XOffset = -109.10, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 309, XOffset = -109.10, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 310, XOffset = -106.30, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 311, XOffset = -106.30, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 312, XOffset = -103.57, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 313, XOffset = -102.20, YOffset = 30.00 },
                new LookUpTableOffsetData{ Angle = 314, XOffset = -96.60, YOffset = 30.38 },
                new LookUpTableOffsetData{ Angle = 315, XOffset = -95.20, YOffset = 31.70 },
                new LookUpTableOffsetData{ Angle = 316, XOffset = -92.50, YOffset = 31.70 },
                new LookUpTableOffsetData{ Angle = 317, XOffset = -92.50, YOffset = 31.30 },
                new LookUpTableOffsetData{ Angle = 318, XOffset = -88.30, YOffset = 31.70 },
                new LookUpTableOffsetData{ Angle = 319, XOffset = -85.60, YOffset = 33.10 },
                new LookUpTableOffsetData{ Angle = 320, XOffset = -81.40, YOffset = 31.70 },
                new LookUpTableOffsetData{ Angle = 321, XOffset = -81.40, YOffset = 31.70 },
                new LookUpTableOffsetData{ Angle = 322, XOffset = -77.30, YOffset = 30.38 },
                new LookUpTableOffsetData{ Angle = 323, XOffset = -75.90, YOffset = 30.38 },
                new LookUpTableOffsetData{ Angle = 324, XOffset = -73.19, YOffset = 31.70 },
                new LookUpTableOffsetData{ Angle = 325, XOffset = -73.19, YOffset = 30.38 },
                new LookUpTableOffsetData{ Angle = 326, XOffset = -73.19, YOffset = 30.38 },
                new LookUpTableOffsetData{ Angle = 327, XOffset = -69.00, YOffset = 30.38 },
                new LookUpTableOffsetData{ Angle = 328, XOffset = -64.90, YOffset = 31.70 },
                new LookUpTableOffsetData{ Angle = 329, XOffset = -66.20, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 330, XOffset = -59.30, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 331, XOffset = -56.20, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 332, XOffset = -56.60, YOffset = 29.00 },
                new LookUpTableOffsetData{ Angle = 333, XOffset = -53.80, YOffset = 27.60 },
                new LookUpTableOffsetData{ Angle = 334, XOffset = -51.10, YOffset = 26.20 },
                new LookUpTableOffsetData{ Angle = 335, XOffset = -49.70, YOffset = 27.60 },
                new LookUpTableOffsetData{ Angle = 336, XOffset = -46.90, YOffset = 26.20 },
                new LookUpTableOffsetData{ Angle = 337, XOffset = -45.50, YOffset = 24.80 },
                new LookUpTableOffsetData{ Angle = 338, XOffset = -42.80, YOffset = 23.40 },
                new LookUpTableOffsetData{ Angle = 339, XOffset = -41.43, YOffset = 23.47 },
                new LookUpTableOffsetData{ Angle = 340, XOffset = -38.66, YOffset = 23.47 },
                new LookUpTableOffsetData{ Angle = 341, XOffset = -37.20, YOffset = 20.70 },
                new LookUpTableOffsetData{ Angle = 342, XOffset = -31.70, YOffset = 20.70 },
                new LookUpTableOffsetData{ Angle = 343, XOffset = -33.10, YOffset = 20.70 },
                new LookUpTableOffsetData{ Angle = 344, XOffset = -29.00, YOffset = 17.90 },
                new LookUpTableOffsetData{ Angle = 345, XOffset = -27.60, YOffset = 16.60 },
                new LookUpTableOffsetData{ Angle = 346, XOffset = -24.80, YOffset = 16.50 },
                new LookUpTableOffsetData{ Angle = 347, XOffset = -22.40, YOffset = 13.80 },
                new LookUpTableOffsetData{ Angle = 348, XOffset = -20.70, YOffset = 13.80 },
                new LookUpTableOffsetData{ Angle = 349, XOffset = -22.10, YOffset = 12.40 },
                new LookUpTableOffsetData{ Angle = 350, XOffset = -16.50, YOffset = 11.00 },
                new LookUpTableOffsetData{ Angle = 351, XOffset = -16.50, YOffset = 13.80 },
                new LookUpTableOffsetData{ Angle = 352, XOffset = -13.80, YOffset = 9.60 },
                new LookUpTableOffsetData{ Angle = 353, XOffset = -11.04, YOffset = 8.28 },
                new LookUpTableOffsetData{ Angle = 354, XOffset = -11.00, YOffset = 5.50 },
                new LookUpTableOffsetData{ Angle = 355, XOffset = -8.28, YOffset = 4.14 },
                new LookUpTableOffsetData{ Angle = 356, XOffset = -5.52, YOffset = 4.14 },
                new LookUpTableOffsetData{ Angle = 357, XOffset = -4.14, YOffset = 2.76 },
                new LookUpTableOffsetData{ Angle = 358, XOffset = -2.76, YOffset = 1.38 },
                new LookUpTableOffsetData{ Angle = 359, XOffset = -2.07, YOffset = 0.69 },
            };

            PnP2Return = Table;
        }


    }
}
