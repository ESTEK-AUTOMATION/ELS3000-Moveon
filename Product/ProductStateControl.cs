using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Common;
using Machine;
using System.IO;
using MoveonMESAPI;

namespace Product
{
    public class ProductStateControl : Machine.StateControl
    {
        private ProductShareVariables m_ProductShareVariables;

        private ProductProcessEvent m_ProductProcessEvent;

        private ProductRTSSProcess m_ProductRTSSProcess;
        
        private ProductFormConsumableParts m_ProductFormConsumableParts = new ProductFormConsumableParts();
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
        override public int OnStateChangedAfterInitializeDone()
        {
            int nError = 0;
            try
            {
                base.OnStateChangedAfterInitializeDone();
                Task SetProcessTask = Task.Run(() => SetProcess(m_ProductShareVariables.StateMain.ToString()));
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int OnHomeStart()
        {
            int nError = 0;
            bool StartHoming = true;
            try
            {
                base.OnHomeStart();
                //m_ProductProcessEvent.PCS_PCS_Send_Vision_EndTile.Set();
                m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot.Reset();
                m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_INP.Reset();
                m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_OUT.Reset();
                m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_BTM.Reset();
                m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_REJECT.Reset();
                m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW1.Reset();
                m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW2.Reset();
                m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW3.Reset();
                m_ProductProcessEvent.PCS_PCS_Send_Vision_EndLot.Set();

                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_CONTINUE_LOT", false);
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_START_RUNNING", false);
                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_THR_IS_REMAINTRAY", false);

                m_ProductProcessEvent.GUI_PCS_NewLotDone2.Reset();
                if (m_ProductShareVariables.AlarmStart == 1)
                {
                    //m_ProductShareVariables.AlarmStart = 0;
                    //m_ProductShareVariables.CurrentDownTimeCounterMES[m_ProductShareVariables.CurrentDownTimeNo].etime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0)==true)
                {
                    m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_LOT_UNLOAD_COMPLETE", true);
                    m_ProductRTSSProcess.SetProductionInt("WriteReportTrayNo",m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"));
                    m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_ENDLOT", true);
                    if (m_ProductShareVariables.productOptionSettings.bEnableBarcodePrinter == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_TRIGGER_BARCODE_PRINTER", true);
                    }
                    m_ProductRTSSProcess.SetProductionInt("nInputRunningState", 0);
                    m_ProductRTSSProcess.SetProductionInt("nPNPRunningState", 0);
                    m_ProductRTSSProcess.SetProductionInt("nOutputRunningState", 0);
                    if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                    {
                        if (m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotQuantityRun") < m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") /*&&( m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX")<=10)*/)
                        {
                            if (m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX") > 10)
                            {
                                m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateX", 1);
                                m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateY", 1);
                                //m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo")+1);
                            }
                            m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_SAVE_UNFINISHED_LOT", true);
                        }
                    }
                    else if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputTrayNo == true)
                    {
                        if (m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun") < m_ProductRTSSProcess.GetProductionInt("nInputLotTrayNo") /*&&( m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX")<=10)*/)
                        {
                            if (m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX") > 10)
                            {
                                m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateX", 1);
                                m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateY", 1);
                                //m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo")+1);
                            }
                            m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_SAVE_UNFINISHED_LOT", true);
                        }
                    }

                }
                m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.Reset();
                m_ProductProcessEvent.PCS_GUI_EndLot.Set();
                m_ProductShareVariables.nLotIDNumber = 0;
                m_ProductShareVariables.bFormProductionProductPartNumberEnable = false;
                if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_UNIT_PLACED_ON_OUTPUT_FRAME") == true)
                {
                    HiPerfTimer PurgeFileTimer = new HiPerfTimer();
                    PurgeFileTimer.Start();
                    m_ProductProcessEvent.GUI_PCS_Force_Generate_Last_Data.Set();
                    while (StartHoming == true)
                    {
                        PurgeFileTimer.Elapse();
                        if (m_ProductProcessEvent.PCS_GUI_Last_Data_Generated.WaitOne(0))
                        {
                            if (m_ProductProcessEvent.PCS_GUI_Copy_Task_Complete.WaitOne(0))
                            {
                                Machine.LogDisplay.AddLogDisplay("Process", "Purge Half Way Data Completed in" + (PurgeFileTimer.ElapseMiliSecond() / 1000).ToString() + "s");
                                StartHoming = false;
                            }
                            else if (m_ProductProcessEvent.PCS_GUI_Error_Copy_Task.WaitOne(0))
                            {
                                Machine.LogDisplay.AddLogDisplay("Caution", "Handler Copy Previous Lot Image Not Complete");
                                StartHoming = false;
                            }
                        }
                        else if (PurgeFileTimer.ElapseMiliSecond() >= 60000)
                        {
                            Machine.LogDisplay.AddLogDisplay("Caution", $"Purge Half Way Data Timeout in {(PurgeFileTimer.ElapseMiliSecond() / 1000).ToString()}");
                            StartHoming = false;
                        }
                        Thread.Sleep(1);
                    }
                }
                m_ProductProcessEvent.PCS_GUI_Close_Remaining_Active_Form.Set();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int OnPreProductionStart()
        {
            bool StartPreProduction = true;
            bool IsBondHeadMaintDueReach = false;
            bool IsFlipperHeadMaintDueReach = false;
            bool IsEjectorNeedleMaintDueReach = false;
            bool IsAlignerTipMaitDueReach = false;
            bool IsBondHeadMaintCleanReach = false;
            bool IsFlipperHeadMaintCleanReach = false;
            bool IsAlignerTipMaintCleanReach = false;
            int nError = 0;
            try
            {
                m_ProductShareVariables.UserChooseContinueLot = false;
                if (m_ProductProcessEvent.GUI_PCS_NewLotDone2.WaitOne(0) == false)
                {
                    if (m_ProductShareVariables.bEnableContinueLot == true && m_ProductShareVariables.productOptionSettings.EnableMES==false)
                    {
                        DialogResult result = MessageBox.Show("Please enter a new lot, or do you want to continue the previous lot " + m_ProductShareVariables.strucInputProductInfo.LotID, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.None);
                        if (result == DialogResult.No)
                        {
                            Machine.SequenceControl.SetAlarm(6009);
                            m_ProductRTSSProcess.SetEvent("JobStop", true);
                            m_ProductShareVariables.bFormProductionProductRecipeEnable = true;
                            m_ProductShareVariables.bFormProductionProductButtonLotEnable = true;
                            return nError;
                        }
                        else
                        {
                            m_ProductShareVariables.UserChooseContinueLot = true;
                            OutputData outputdata = new OutputData();
                            EndJobData endjobdata = new EndJobData();
                            m_ProductShareVariables.LotID = m_ProductShareVariables.strucInputProductInfo.LotID;
                            m_ProductShareVariables.currentMainRecipeName = m_ProductShareVariables.strucInputProductInfo.Recipe;
                            m_ProductShareVariables.PartName = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("PartName", "Unknown").ToString();
                            m_ProductShareVariables.PartNumber = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("PartNumber", "Unknown").ToString();
                            m_ProductShareVariables.BuildName = m_ProductShareVariables.RegKey.GetJobDataKeyParameter("BuildName", "Unknown").ToString();
                            ProductRTSSProcess.SetShareMemorySettingUInt("TotalOutputUnitQuantity", (uint)(int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("TotalOutputUnitQuantity", 0).ToString())));
                            m_ProductRTSSProcess.SetProductionInt("nInputLotQuantity", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nInputLotQuantity", 0).ToString()));
                            m_ProductRTSSProcess.SetProductionInt("nCurrentInputLotQuantityRun", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputLotQuantityRun", 0).ToString()));
                            m_ProductRTSSProcess.SetProductionInt("nTotalInputUnitDone", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nTotalInputUnitDone", 0).ToString()));
                            m_ProductRTSSProcess.SetProductionInt("nCurrentLotNotGoodQuantity", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentLotNotGoodQuantity", 0).ToString()));
                            m_ProductRTSSProcess.SetProductionInt("nCurrentLotGoodQuantity", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentLotGoodQuantity", 0).ToString()));
                            m_ProductShareVariables.nLotIDNumber = int.Parse(m_ProductShareVariables.RegKey.GetJobDataKeyParameter("nLotIDNumber", "Unknown").ToString());
                            m_ProductShareVariables.listLotID.Clear();
                            if(m_ProductShareVariables.nLotIDNumber == 0)
                            {
                                m_ProductShareVariables.listLotID.Add(m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotID1", "Unknown").ToString());
                            }
                            else if (m_ProductShareVariables.nLotIDNumber == 1)
                            {
                                m_ProductShareVariables.listLotID.Add(m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotID1", "Unknown").ToString());
                                m_ProductShareVariables.listLotID.Add(m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotID2", "Unknown").ToString());
                            }
                            else
                            {
                                m_ProductShareVariables.listLotID.Add(m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotID1", "Unknown").ToString());
                                m_ProductShareVariables.listLotID.Add(m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotID2", "Unknown").ToString());
                                m_ProductShareVariables.listLotID.Add(m_ProductShareVariables.RegKey.GetJobDataKeyParameter("LotID3", "Unknown").ToString());
                            }
                            m_ProductRTSSProcess.SetProductionInt("nCurrentTotalUnitDone", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentTotalUnitDone", 0).ToString()));
                            m_ProductRTSSProcess.SetProductionInt("nCurrrentTotalUnitDoneByLot", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrrentTotalUnitDoneByLot", 0).ToString()));
                            m_ProductRTSSProcess.SetProductionInt("nCurrentTotalInputUnitDone", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentTotalInputUnitDone", 0).ToString()));

                            if (int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()) == 0)
                            {
                                m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()));
                                m_ProductRTSSProcess.SetProductionInt("nCurrentBottomStationTrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()));
                                m_ProductRTSSProcess.SetProductionInt("nCurrentS3StationTrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()));
                                m_ProductShareVariables.nInputTrayNo = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString());
                            }
                            else
                            {
                                m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()) - 1);
                                m_ProductRTSSProcess.SetProductionInt("nCurrentBottomStationTrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()) - 1);
                                m_ProductRTSSProcess.SetProductionInt("nCurrentS3StationTrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()) - 1);
                                m_ProductShareVariables.nInputTrayNo = int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()) - 1;
                            }

                            m_ProductRTSSProcess.SetProductionInt("nCurrentOutputTrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentOutputTrayNo", 0).ToString()));
                            LoadPreviousLotSettings();
                            bool IsLotExists = false;
                            int nPreviousMatchLotNo = 0;
                            foreach (LotDetail _Lot in m_ProductShareVariables.productContinueLotInfo.PreviousLotInfo)
                            {
                                if (_Lot.LotID == m_ProductShareVariables.strucInputProductInfo.LotID)
                                {
                                    IsLotExists = true;
                                    break;
                                }
                                nPreviousMatchLotNo++;
                            }
                            if (IsLotExists == false)
                            {
                                m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateX", 1);
                                m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateY", 1);
                                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_IS_PREVIOUS_SAVE_LOT_MATCH", false);
                            }
                            else
                            {
                                m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateX", m_ProductShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].Row);
                                m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateY", m_ProductShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].Column);
                                m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_IS_PREVIOUS_SAVE_LOT_MATCH", true);
                                m_ProductShareVariables.productContinueLotInfo.PreviousLotInfo.RemoveAt(nPreviousMatchLotNo);
                            }
                            //Output Quantity (WC)
                            //m_ProductRTSSProcess.SetProductionInt("nCurrentReject1TrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentReject1TrayNo", 0).ToString()));
                            //m_ProductRTSSProcess.SetProductionInt("nCurrentReject2TrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentReject2TrayNo", 0).ToString()));
                            //m_ProductRTSSProcess.SetProductionInt("nCurrentReject3TrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentReject3TrayNo", 0).ToString()));
                            //m_ProductRTSSProcess.SetProductionInt("nCurrentReject4TrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentReject4TrayNo", 0).ToString()));
                            //m_ProductRTSSProcess.SetProductionInt("nCurrentReject5TrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentReject5TrayNo", 0).ToString()));
                            m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_CONTINUE_LOT", true);
                            m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                            m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_START_RUNNING", true);
                           
                            m_ProductShareVariables.AlarmStart = 0;
                            //if (Directory.Exists("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput))
                            //{
                            //    outputdata = Newtonsoft.Json.JsonConvert.DeserializeObject<OutputData>(File.ReadAllText("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput+ "\\outputData.json"));
                            //    endjobdata = Newtonsoft.Json.JsonConvert.DeserializeObject<EndJobData>(File.ReadAllText("F:\\MES\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput+ "\\endjobData.json"));
                            //    m_ProductShareVariables.TotalLotMES = outputdata.details.ToList();
                            //    m_ProductShareVariables.CurrentDownTimeCounterMES = outputdata.downtime.ToList();
                            //    m_ProductShareVariables.CurrentDownTimeNo = m_ProductShareVariables.CurrentDownTimeCounterMES.Count();
                            //    int currentLot = 0;
                            //    bool IsExists = false;
                            //    foreach(Lot _Lot in m_ProductShareVariables.TotalLotMES)
                            //    {
                            //        if(_Lot.lot_no == m_ProductShareVariables.strucInputProductInfo.LotID)
                            //        {
                            //            m_ProductShareVariables.CurrentLotMES = _Lot;
                            //            m_ProductShareVariables.CurrentDefectCounterMES = _Lot.defective.ToList();
                            //            IsExists = true;
                            //            break;
                            //        }
                            //        currentLot++;
                            //    }
                            //    if(IsExists==true)
                            //    {
                            //        m_ProductShareVariables.TotalLotMES.RemoveAt(currentLot);
                            //    }

                            //}

                        }
                    }
                    else if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_AvailableDiskSpaceLow.WaitOne(0) == true)
                    {
                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_AvailableDiskSpaceLow.Reset();
                        Machine.SequenceControl.SetAlarm(60002);
                    }
                    else
                    {
                        //Machine.SequenceControl.SetAlarm(6009);
                        Machine.LogDisplay.AddLogDisplay("Error",$"Please Enter New Lot.\n");
                        Machine.EventLogger.WriteLog(string.Format("{0}: No New Lot.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        MessageBox.Show("Please Enter New Lot.\n", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                        m_ProductShareVariables.bFormProductionProductRecipeEnable = true;
                        m_ProductShareVariables.bFormProductionProductButtonLotEnable = true;
                        return nError;
                    }
                }
                else
                {
                    //m_ProductShareVariables.nInputTrayNo = 0;
                }
                nError = base.OnPreProductionStart();
                if (nError != 0)
                    return nError;

                //m_ProductShareVariables.nInputTrayNo = 0;
                m_ProductShareVariables.dtProductionCurrentUnitStartTime = DateTime.Now;
                m_ProductProcessEvent.PCS_PCS_Send_Write_XML.Reset();
                //m_ProductProcessEvent.PCS_PCS_StartReadInputFile.Set();
                m_ProductProcessEvent.PCS_PCS_Start_Write_OutputFile_Only.Reset();
                m_ProductProcessEvent.PCS_PCS_Start_Write_SortingFile_Only.Reset();
                m_ProductProcessEvent.PCS_PCS_Start_Write_RejectFile_Only.Reset();
                m_ProductProcessEvent.PCS_PCS_Start_Write_AllFile.Reset();
                //m_ProductProcessEvent.GUI_GUI_GET_DATA.Reset();
                //m_ProductShareVariables.nLotIDNumber = 0;
                //m_ProductShareVariables.PreviousReportLotID = "";
                //m_ProductShareVariables.PreviousReportTrayNo = m_ProductShareVariables.nInputTrayNo;
                #region Secsgem
                Task TriggerEventStartTask = Task.Run(() => TriggerEventStart(m_ProductShareVariables.strucInputProductInfo));
                #endregion Secsgem

                //m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentInputTrayNo", 0).ToString()));
                //m_ProductRTSSProcess.SetProductionInt("nCurrentOutputTrayNo", int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("nCurrentOutputTrayNo", 0).ToString()));
                //Module PreProductionStartState;
                //SetShareMemoryEvent("JobStop", false);
                //if (m_ProductProcessEvent.PCS_PCS_Vision_AllStationDefectCode_Error.WaitOne(0))
                //{
                //    SetShareMemoryEvent("JobStop", true);
                //    Machine.LogDisplay.AddLogDisplay("Error", $"Statemain= {m_ProductShareVariables.StateMain}");
                //    Machine.LogDisplay.AddLogDisplay("Error", "Vision Defect Code not Tally.");
                //    Machine.SequenceControl.SetAlarm(6008);
                //}
                m_ProductShareVariables.nNewValueForXML = 0;
                m_ProductShareVariables.nNoOfXMLForOutput = 0;
                m_ProductShareVariables.nNoOfXMLForSorting = 0;
                m_ProductShareVariables.nNoOfXMLForReject = 0;

                m_ProductShareVariables.nNoOfBarcode = 0;
                m_ProductShareVariables.nCurrentUnitNo = 0;
                m_ProductShareVariables.nCurrentPHUnitNo = 0;
                m_ProductShareVariables.nCurrentOutputUnitNo = 0;
                m_ProductShareVariables.nCurrentPickUpHeadNo = 0;
                m_ProductShareVariables.bolFirstTimeCreateOutputFile = true;
                Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} m_ProductShareVariables.bolFirstTimeCreateOutputFile = true");
                //for (int i = 0; i < 1; i++)
                //{
                //    if (m_ProductRTSSProcess.GetProductionArray("PickUpHeadStationResult", i, "UnitPresent") == 1)
                //    {
                //        m_ProductShareVariables.nBarcodeIDNo++;
                //    }
                //}
                m_ProductShareVariables.ArrayCurrentBarcodeID = new string[6];
                m_ProductShareVariables.lstBarcode = new List<TempBarcodeAdd>();
                m_ProductShareVariables.TempInfo = new BinInfo();
                HiPerfTimer VisionReceive = new HiPerfTimer();
                VisionReceive.Start();
                while (StartPreProduction == true)
                {
                    VisionReceive.Elapse();
                    if ((m_ProductProcessEvent.PCS_PCS_Vision_Receive_Setting.WaitOne(0)
                     && m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_INP.WaitOne(0)
                     && m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_OUT.WaitOne(0)
                     && m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_REJECT.WaitOne(0)
                     && m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW1.WaitOne(0)
                     && m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW2.WaitOne(0)
                     && m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW3.WaitOne(0)
                        ) || m_ProductShareVariables.productOptionSettings.EnableVision == false)
                    {
                        m_ProductRTSSProcess.SetEvent("JobStop", false);
                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_ENDLOT", false);
                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_ENDLOT_UPDATE_SUMMARY_DONE", false);
                        StartPreProduction = false;
                        m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.Set();
                    }
                    else if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_Info_Fail.WaitOne(0))
                    {
                        //Machine.SequenceControl.SetAlarm(6002);
                        Machine.LogDisplay.AddLogDisplay("Error",$"Fail To Load Lot Info To Vision Software, Please Restart Vision Software Or Redo New Lot");
                        Machine.EventLogger.WriteLog(string.Format("{0}: Fail To Load Lot Info To Vision Software.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        MessageBox.Show("Fail To Load Lot Info To Vision Software, Please Restart Vision Software Or Redo New Lot.\n", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                        StartPreProduction = false;
                        m_ProductShareVariables.bFormProductionProductRecipeEnable = true;
                        m_ProductShareVariables.bFormProductionProductButtonLotEnable = true;
                    }
                    else if (VisionReceive.ElapseMiliSecond() >= 40000)
                    {
                        if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_Info_Fail.WaitOne(0) == false)
                        {
                            Machine.LogDisplay.AddLogDisplay("Error", $"Vision New Lot Fail.");
                        }
                        if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_Setting.WaitOne(0) == false)
                        {
                            Machine.LogDisplay.AddLogDisplay("Error", $"Vision Fail To Acknowledge Recipe.");
                        }
                        if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_INP.WaitOne(0) == false)
                        {
                            Machine.LogDisplay.AddLogDisplay("Error", $"Vision Fail To Acknowledge Input newlot.");
                        }
                        if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_OUT.WaitOne(0) == false)
                        {
                            Machine.LogDisplay.AddLogDisplay("Error", $"Vision Fail To Acknowledge output newlot.");
                        }
                        if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_REJECT.WaitOne(0) == false)
                        {
                            Machine.LogDisplay.AddLogDisplay("Error", $"Vision Fail To Acknowledge reject newlot.");
                        }
                        if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW1.WaitOne(0) == false)
                        {
                            Machine.LogDisplay.AddLogDisplay("Error", $"Vision Fail To Acknowledge SW1 newlot.");
                        }
                        if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW2.WaitOne(0) == false)
                        {
                            Machine.LogDisplay.AddLogDisplay("Error", $"Vision Fail To Acknowledge SW2 newlot.");
                        }
                        if (m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW3.WaitOne(0) == false)
                        {
                            Machine.LogDisplay.AddLogDisplay("Error", $"Vision Fail To Acknowledge SW3 newlot.");
                        }
                        Machine.LogDisplay.AddLogDisplay("Error", $"Fail To Load Lot Info To Vision Software, Please Restart Vision Software Or Redo New Lot");
                        Machine.EventLogger.WriteLog(string.Format("{0}: Fail To Load Lot Info To Vision Software.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        MessageBox.Show("Fail To Load Lot Info To Vision Software, Please Restart Vision Software Or Redo New Lot.\n", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                        StartPreProduction = false;
                        m_ProductShareVariables.bFormProductionProductRecipeEnable = true;
                        m_ProductShareVariables.bFormProductionProductButtonLotEnable = true;
                    }
                    Thread.Sleep(1);
                }

                
                m_ProductShareVariables.CurrentInputDefectCode.Clear();
                m_ProductShareVariables.CurrentSetupDefectCode.Clear();
                m_ProductShareVariables.CurrentS2DefectCode.Clear();
                m_ProductShareVariables.CurrentS1DefectCode.Clear();
                m_ProductShareVariables.CurrentSWLDefectCode.Clear();
                m_ProductShareVariables.CurrentSWRDefectCode.Clear();
                m_ProductShareVariables.CurrentSWFDefectCode.Clear();
                m_ProductShareVariables.CurrentSWREARDefectCode.Clear();
                m_ProductShareVariables.CurrentS3DefectCode.Clear();
                m_ProductShareVariables.CurrentOutputDefectCode.Clear();

                m_ProductShareVariables.DefectCounterInput.Clear();
                m_ProductShareVariables.DefectCounterSetup.Clear();
                m_ProductShareVariables.DefectCounterS2.Clear();
                m_ProductShareVariables.DefectCounterS1.Clear();
                m_ProductShareVariables.DefectCounterSWL.Clear();
                m_ProductShareVariables.DefectCounterSWR.Clear();
                m_ProductShareVariables.DefectCounterSWF.Clear();
                m_ProductShareVariables.DefectCounterSWREAR.Clear();
                m_ProductShareVariables.DefectCounterS3.Clear();
                m_ProductShareVariables.DefectCounterOutput.Clear();


                #region Due Count
                for (int i = 0; i < 2; i++)
                {
                    if (m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "") >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadDueCount", 3000).ToString()))
                    {
                        Machine.SequenceControl.SetAlarm(60201);
                        Machine.LogDisplay.AddLogDisplay("Error", $"Pick Up Head {i + 1}'s Due Count is reached. Please clean the count.");
                        IsBondHeadMaintDueReach = true;
                        //SetShareMemoryEvent("JobStop", true);
                        //return -1;
                    }
                }
                if (IsFlipperHeadMaintDueReach == true || IsBondHeadMaintDueReach == true || IsAlignerTipMaitDueReach == true || IsEjectorNeedleMaintDueReach == true)
                {
                    m_ProductRTSSProcess.SetEvent("JobStop", true);
                    return -1;
                }
                #endregion Due Count

                #region Clean Count
                for (int i = 0; i < 2; i++)
                {
                    if (m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "") >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"Pick Up Head {i + 1}CleanCount", 1000).ToString()))
                    {
                        if (bool.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"Pick Up Head {i + 1}CleanAlarm", "false").ToString()) == false)
                        {
                            IsBondHeadMaintCleanReach = true;
                            Machine.SequenceControl.SetAlarm(60203);
                            Machine.LogDisplay.AddLogDisplay("Error", $"Pick Up Head {i + 1}'s Clean Count is Reached.");
                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"Pick Up Head {i + 1}CleanAlarm", "true");
                        }
                    }
                }

                ////if (m_ProductRTSSProcess.GetProductionArray("EjectorNeedle", 0, "") >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("Ejector NeedleCleanCount", 1000).ToString()))
                ////{
                ////    if (bool.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"Ejector NeedleCleanAlarm", "false").ToString()) == false)
                ////    {

                ////        Machine.SequenceControl.SetAlarm(60209);
                ////        Machine.LogDisplay.AddLogDisplay("Error", "Ejector Needle's Clean Count is Reached.");
                ////        m_ProductShareVariables.RegKey.SetEstekKeyParameter($"Ejector NeedleCleanAlarm", "true");
                ////    }
                ////}
                if (IsBondHeadMaintCleanReach == true || IsFlipperHeadMaintCleanReach == true || IsAlignerTipMaintCleanReach == true)
                {
                    return nError;
                }
                #endregion Clean Count

                #region Warning Count
                for (int i = 0; i < 2; i++)
                {
                    if (m_ProductRTSSProcess.GetProductionArray("PickUpHeadCount", i, "") >= int.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter("PickUpHeadWarningCount", 2000).ToString()))
                    {
                        if (bool.Parse(m_ProductShareVariables.RegKey.GetEstekKeyParameter($"Pick Up Head {i + 1}WarningAlarm", "false").ToString()) == false)
                        {
                            Machine.SequenceControl.SetAlarm(60202);
                            Machine.LogDisplay.AddLogDisplay("Error", $"Pick Up Head {i + 1}'s Warning Count is Reached.");
                            m_ProductShareVariables.RegKey.SetEstekKeyParameter($"Pick Up Head {i + 1}WarningAlarm", "true");
                            //break;
                        }
                    }
                }

                #endregion Warning Count

                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int OnAbortStart()
        {
            int nError = 0;
            try
            {
                base.OnAbortStart();
                m_ProductProcessEvent.PCS_GUI_Close_Remaining_Active_Form.Set();
                if (m_ProductShareVariables.AlarmStart == 1)
                {
                    m_ProductShareVariables.AlarmStart = 0;
                    m_ProductShareVariables.CurrentDownTimeCounterMES[m_ProductShareVariables.CurrentDownTimeNo].etime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        public override int OnPostProductionStart()
        {
            int nError = 0;
            try
            {
                base.OnPostProductionStart();
              
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int GUIUpdateBeforePreProductionStartState()
        {
            int nError = 0;
            try
            {
                base.GUIUpdateBeforePreProductionStartState();
                m_ProductShareVariables.bFormProductionProductRecipeEnable = false;
                m_ProductShareVariables.bFormProductionProductEndLotEnable = false;
                m_ProductShareVariables.bFormProductionProductButtonLotEnable = false;
                //m_ProductProcessEvent.PCS_GUI_SelectSlotToRun.Set();
                // m_ProductProcessEvent.PCS_GUI_ClearCheckBoxText.Set();

                //while (m_bRunThread)
                //{
                //    if (m_ProductShareVariables.FS_CompletedSelection)
                //    {
                //        m_ProductShareVariables.FS_CompletedSelection = false;
                //        break;
                //    }
                //    Thread.Sleep(1);
                //}

                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int OnAlarmStart()
        {
            int nError = 0;
            try
            {
                base.OnAlarmStart();
                #region Secsgem
                Task SetAlarmTask = Task.Run(() => SetAlarm(7007));
                #endregion Secsgem
                //m_ProductShareVariables.bAlarmStart = true;
                //if(m_ProductShareVariables.bAlarmRepeat == false)
                //{
                //    m_ProductShareVariables.bAlarmRepeat = true;
                //    m_ProductShareVariables.dtAlarmStartTime = DateTime.Now;
                //}
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int OnShutDownStart()
        {
            int nError = 0;
            try
            {
                base.OnShutDownStart();
                if (m_ProductRTSSProcess.GetEvent("PowerLost") == false)
                {
                    //if (m_ProductShareVariables.AlarmStart == 1)
                    //{
                    //    m_ProductShareVariables.AlarmStart = 0;
                    //    m_ProductShareVariables.CurrentDownTimeCounterMES[m_ProductShareVariables.CurrentDownTimeNo].etime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //}
                    if (m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0) == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_LOT_UNLOAD_COMPLETE", true);
                        m_ProductRTSSProcess.SetProductionInt("WriteReportTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"));
                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_ENDLOT", true);
                        if (m_ProductShareVariables.productOptionSettings.bEnableBarcodePrinter == true)
                        {
                            m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_TRIGGER_BARCODE_PRINTER", true);
                        }
                        m_ProductRTSSProcess.SetProductionInt("nInputRunningState", 0);
                        m_ProductRTSSProcess.SetProductionInt("nPNPRunningState", 0);
                        m_ProductRTSSProcess.SetProductionInt("nOutputRunningState", 0);
                        if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                        {
                            if (m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotQuantityRun") < m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") /*&&( m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX")<=10)*/)
                            {
                                if (m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX") > 10)
                                {
                                    m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateX", 1);
                                    m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateY", 1);
                                    //m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo")+1);
                                }
                                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_SAVE_UNFINISHED_LOT", true);
                            }
                        }
                        else if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputTrayNo == true)
                        {
                            if (m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun") < m_ProductRTSSProcess.GetProductionInt("nInputLotTrayNo") /*&&( m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX")<=10)*/)
                            {
                                if (m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX") > 10)
                                {
                                    m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateX", 1);
                                    m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateY", 1);
                                    //m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo")+1);
                                }
                                m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_SAVE_UNFINISHED_LOT", true);
                            }
                        }

                    }
                }
                #region Secsgem
                Task SetSecsgemModeTask = Task.Run(() => SetSecsgemMode(0));//SetControlState
                Task SetControlStateTask = Task.Run(() => SetControlState(0));
                #endregion Secsgem
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        private void TriggerEventStart(Input_Product_Info InputProductInfo)
        {
            int nError = 0;
            try
            {
                if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
                {
                    if (Machine.Platform.SecsgemControl.SetEventLotInfo(InputProductInfo.LotID, InputProductInfo.WorkOrder,
                        InputProductInfo.Recipe, InputProductInfo.WaferBin, InputProductInfo.PPLot,
                        InputProductInfo.OperatorID, InputProductInfo.Shift) != 0)
                    {
                        m_ProductRTSSProcess.SetGeneralInt("AlarmID", 7003);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return;
            }
        }

        void SetAlarm(int AlarmId)
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                if (Machine.Platform.SecsgemControl.SetAlarm(m_ProductRTSSProcess.GetGeneralInt("AlarmID").ToString()) != 0)
                {
                    m_ProductRTSSProcess.SetGeneralInt("AlarmID", AlarmId);
                }
            }
        }

        void SetProcess(string ProcessState)
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                Machine.Platform.SecsgemControl.SetProcess(ProcessState);
            }
        }

        void SetSecsgemMode(int SecsgemMode)
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                Machine.Platform.SecsgemControl.SetSecsgemMode(SecsgemMode);
            }
        }
        void SetControlState(int State)
        {
            if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
            {
                Machine.Platform.SecsgemControl.ControlState = State;
            }
        }

        override public int GUIUpdateInIdleDoneState()
        {
            int nError = 0;
            try
            {
                base.GUIUpdateInIdleDoneState();

                #region FrameSelection
                m_ProductProcessEvent.PCS_GUI_EnableFrameSelection.Set();
                #endregion

                if (m_ProductRTSSProcess.GetEvent("GPCS_RSEQ_ABORT") == true)
                {
                    m_ProductShareVariables.bFormProductionMaintenanceEnable = false;
                }
                else
                {
                    m_ProductShareVariables.bFormProductionMaintenanceEnable = true;
                }
                m_ProductShareVariables.bFormProductionProductRecipeEnable = true;
                m_ProductShareVariables.bFormProductionProductPartNumberEnable = true;
                m_ProductShareVariables.bFormProductionProductButtonLotEnable = true;
                m_ProductShareVariables.bFormProductionCycleEnable = true;
                m_ProductShareVariables.bFormProductionReviewEnable = true;

                m_ProductShareVariables.bFormPusherControlEnable = false;

                m_ProductShareVariables.bFormLightingCalibrationEnable = true;
                m_ProductShareVariables.bFormCalibrationEnable = true;
                m_ProductShareVariables.bFormOutputMotionMoveEnable = true;
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int GUIUpdateInRunStartState()
        {
            int nError = 0;
            try
            {
                base.GUIUpdateInRunStartState();
                m_ProductShareVariables.bFormProductionProductRecipeEnable = false;
                m_ProductShareVariables.bFormProductionProductButtonLotEnable = false;
                m_ProductShareVariables.bFormProductionProductEndLotEnable = false;
                m_ProductShareVariables.bFormProductionMaintenanceEnable = false;

                m_ProductShareVariables.bFormProductionCycleEnable = false;
                m_ProductShareVariables.bFormProductionReviewEnable = false;
              
                m_ProductShareVariables.bFormPusherControlEnable = false;

                m_ProductShareVariables.bFormLightingCalibrationEnable = false;
                m_ProductShareVariables.bFormOutputMotionMoveEnable = false;
                m_ProductShareVariables.bFormCalibrationEnable = false;

                if (m_ProductProcessEvent.PCS_PCS_Vision_AllStationDefectCode_Error.WaitOne(0)
                    && m_ProductShareVariables.StateMain != Machine.StateControl.PreHomeStartState
                    && m_ProductShareVariables.StateMain != Machine.StateControl.ResumeStartState)
                {
                    //SetShareMemoryEvent("JobStop", true);
                    Machine.LogDisplay.AddLogDisplay("Error", $"Statemain= {m_ProductShareVariables.StateMain}");
                    Machine.LogDisplay.AddLogDisplay("Error", "Vision Defect Code not Tally.");
                    Machine.SequenceControl.SetAlarm(6008);
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }
        override public int GUIUpdateInInitializeStartState()
        {
            int nError = 0;
            try
            {
                m_ProductShareVariables.bFormPusherControlEnable = false;
                m_ProductShareVariables.bFormProductionCycleEnable = false;
                m_ProductShareVariables.bFormLightingCalibrationEnable = false;
                m_ProductShareVariables.bFormProductionReviewEnable = false;
                m_ProductShareVariables.bFormCalibrationEnable = false;
                m_ProductShareVariables.bFormOutputMotionMoveEnable = false;
                nError = base.GUIUpdateInInitializeStartState();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        override public int GUIUpdateInInitializeDoneState()
        {
            int nError = 0;
            try
            {
                m_ProductShareVariables.bFormPusherControlEnable = false;
                m_ProductShareVariables.bFormLightingCalibrationEnable = true;
                m_ProductShareVariables.bFormCalibrationEnable = true;
                m_ProductShareVariables.bFormProductionCycleEnable = false;
                m_ProductShareVariables.bFormProductionReviewEnable = false;
                m_ProductShareVariables.bFormOutputMotionMoveEnable = false;
                nError = base.GUIUpdateInInitializeDoneState();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }
        
        override public int GUIUpdateInPauseDoneState()
        {
            int nError = 0;
            try
            {
               m_ProductShareVariables.bFormLightingCalibrationEnable = true;
                m_ProductShareVariables.bFormProductionCycleEnable = true;
                m_ProductShareVariables.bFormProductionReviewEnable = true;
                m_ProductShareVariables.bFormCalibrationEnable = true;
                m_ProductShareVariables.bFormOutputMotionMoveEnable = true;
                nError = base.GUIUpdateInPauseDoneState();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }
        override public int GUIUpdateInPauseDoneStateAndPreviousStateIsRunningProductionState()
        {
            int nError = 0;
            try
            {
                if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_ENDLOT_CONDITION") == true)
                {
                    m_ProductShareVariables.bFormProductionProductEndLotEnable = true;
                    m_ProductShareVariables.bFormProductionProductButtonLotEnable = false;
                    m_ProductShareVariables.bFormProductionProductRecipeEnable = false;
                }
                else if(m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_NEW_OR_END_LOT_CONDITION")==true)
                {
                    m_ProductShareVariables.bFormProductionProductEndLotEnable = true;
                    m_ProductShareVariables.bFormProductionProductButtonLotEnable = true;
                    m_ProductShareVariables.bFormProductionProductRecipeEnable = false;
                }
                else
                {
                    m_ProductShareVariables.bFormProductionProductEndLotEnable = false;
                    m_ProductShareVariables.bFormProductionProductButtonLotEnable = false;
                    m_ProductShareVariables.bFormProductionProductRecipeEnable = false;
                }
                m_ProductShareVariables.bFormPusherControlEnable = true;
                nError = base.GUIUpdateInPauseDoneStateAndPreviousStateIsRunningProductionState();
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        public override int OnProductionStart()
        {
            int nError = 0;
            try
            {
                base.OnProductionStart();
                if (m_ProductShareVariables.AlarmStart == 1)
                {
                    m_ProductShareVariables.AlarmStart = 0;
                    m_ProductShareVariables.CurrentDownTimeCounterMES[m_ProductShareVariables.CurrentDownTimeNo].etime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }

        //void CalculateDownTime()
        //{
        //    bool StartCalculate = true;
        //    HiPerfTimer CalculateDownTimer = new HiPerfTimer();
        //    CalculateDownTimer.Start();
        //    while(StartCalculate==true)
        //    {
        //        CalculateDownTimer.Elapse();
        //        if(m_ProductShareVariables.bAlarmStart == true)
        //        {
        //            StartCalculate = false;
        //        }
        //        else if (CalculateDownTimer.ElapseMiliSecond()>=10000)
        //        {
        //            StartCalculate = false;
        //            m_ProductShareVariables.bAlarmRepeat = false;
        //            Task TriggerSecsgemDownTimeTask = Task.Run(() => TriggerSecsgemDownTime());
        //        }
        //    }
        //    return;
        //}
        //void TriggerSecsgemDownTime()
        //{
        //    if (m_ProductShareVariables.productConfigurationSettings.EnableSecsgem)
        //    {
        //        //Send DownTime
        //    }
        //    return;
        //}
        public int CalculateDowntimePerformance(ReportProcess.Record[] record, List<int> alarmFilter, ReportEvent reportEvent, out MachinePerformance machinePerformance)
        {
            int m_nError = 0;

            machinePerformance = new MachinePerformance
            {
                UpTime = new TimeSpan(),
                DownTime = new TimeSpan(),
                FailureTime = new TimeSpan(),
                WaitTime = new TimeSpan(),
                AssistTime = new TimeSpan(),
                MaintenanceTime = new TimeSpan(),
                TotalTime = new TimeSpan(),
                MTBA = new TimeSpan(),
                MTBF = new TimeSpan(),
                MTTR = new TimeSpan(),
                PercentDownTime = 0,
                PercentUpTime = 0,
                PercentWaitTime = 0,
                PercentMaintanainceTime = 0,
                MCBA = 0,
                MCBF = 0,
                AssistCount = 0,
                FailureCount = 0,
                ShotCount = 0
            };

            ReportProcess.Record recordPrevious = new ReportProcess.Record();
            try
            {
                if (record == null)
                {
                    return 1;
                }
                if (record.Length == 0)
                {
                    return 2;
                }
                foreach (var _reportInfo in record)
                {
                    if (recordPrevious.Equals(new ReportProcess.Record()))
                    {
                        recordPrevious = _reportInfo;
                        continue;
                    }
                    if (alarmFilter.Contains(recordPrevious.alarmID))
                    {
                    }
                    else
                    {
                        if (recordPrevious.eventID == 8)  // maintainence 
                        {
                            machinePerformance.MaintenanceTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                            //nAssistCount++;
                        }
                        if (recordPrevious.eventID == 11)  // Production 
                        {
                            machinePerformance.ShotCount++;
                        }
                        if (recordPrevious.eventID == 10) // wait time
                        {
                            machinePerformance.WaitTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                        if (recordPrevious.eventID == 12) // Assist time
                        {
                            machinePerformance.AssistCount++;
                            machinePerformance.AssistTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                        if (recordPrevious.eventID == 13)  // alarm
                        {
                            machinePerformance.FailureCount++;
                            machinePerformance.FailureTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                        if (GetUpDownSetting(recordPrevious.eventID, reportEvent) == 1)  // 
                        {
                            machinePerformance.UpTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                            machinePerformance.TotalTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                        else if (GetUpDownSetting(recordPrevious.eventID, reportEvent) == 0) //
                        {
                            machinePerformance.DownTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                            machinePerformance.TotalTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                    }
                    recordPrevious = _reportInfo;
                }

                if (machinePerformance.TotalTime.Ticks != 0)
                {
                    machinePerformance.PercentDownTime = ((double)machinePerformance.DownTime.Ticks / (double)machinePerformance.TotalTime.Ticks) * 100;
                    machinePerformance.PercentUpTime = ((double)machinePerformance.UpTime.Ticks / (double)machinePerformance.TotalTime.Ticks) * 100;
                    machinePerformance.PercentMaintanainceTime = ((double)machinePerformance.MaintenanceTime.Ticks / (double)machinePerformance.TotalTime.Ticks) * 100;
                    machinePerformance.PercentWaitTime = ((double)machinePerformance.WaitTime.Ticks / (double)machinePerformance.TotalTime.Ticks) * 100;
                }
                else
                {
                    machinePerformance.PercentDownTime = 0;
                    machinePerformance.PercentUpTime = 0;
                    machinePerformance.PercentMaintanainceTime = 0;
                    machinePerformance.PercentWaitTime = 0;
                }
                machinePerformance.PercentDownTime = Math.Round(machinePerformance.PercentDownTime, 2);
                machinePerformance.PercentUpTime = Math.Round(machinePerformance.PercentUpTime, 2);
                machinePerformance.PercentMaintanainceTime = Math.Round(machinePerformance.PercentMaintanainceTime, 2);
                machinePerformance.PercentWaitTime = Math.Round(machinePerformance.PercentWaitTime, 2);

                if (machinePerformance.AssistCount == 0)
                {
                    //nAssistCount = 1;
                    machinePerformance.MTBA = new TimeSpan(machinePerformance.UpTime.Ticks);
                    machinePerformance.MCBA = (double)machinePerformance.ShotCount;
                }
                else
                {
                    machinePerformance.MTBA = new TimeSpan(machinePerformance.UpTime.Ticks / machinePerformance.AssistCount);
                    machinePerformance.MCBA = (double)machinePerformance.ShotCount / machinePerformance.AssistCount;
                }
                if (machinePerformance.FailureCount == 0)
                {
                    //nFailureCount = 1;
                    machinePerformance.MTBF = new TimeSpan(machinePerformance.UpTime.Ticks);
                    machinePerformance.MCBF = (double)machinePerformance.ShotCount;
                    machinePerformance.MTTR = new TimeSpan(machinePerformance.DownTime.Ticks);
                }
                else
                {
                    machinePerformance.MTBF = new TimeSpan(machinePerformance.UpTime.Ticks / machinePerformance.FailureCount);
                    machinePerformance.MCBF = (double)machinePerformance.ShotCount / machinePerformance.FailureCount;
                    machinePerformance.MTTR = new TimeSpan(machinePerformance.DownTime.Ticks / machinePerformance.FailureCount);
                }
                if ((machinePerformance.AssistCount + machinePerformance.FailureCount) == 0)
                    machinePerformance.MTTA = new TimeSpan(machinePerformance.WaitTime.Ticks);
                else
                    machinePerformance.MTTA = new TimeSpan(machinePerformance.WaitTime.Ticks / (machinePerformance.AssistCount + machinePerformance.FailureCount));
            }
            catch (Exception ex)
            {
                m_nError = -1;
                DebugLogger.WriteLog(string.Format("{0}  {1}.",DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return m_nError;
        }

        public int GetUpDownSetting(int eventID, ReportEvent reportEvent)
        {
            int nSetting = 2;

            if (eventID == reportEvent.EventSoftwareInitialize.EventID)
            {
                nSetting = reportEvent.EventSoftwareInitialize.EventResult;
            }

            if (eventID == reportEvent.EventSoftwareDisable.EventID)
            {
                nSetting = reportEvent.EventSoftwareDisable.EventResult;
            }

            if (eventID == reportEvent.EventSoftwareShutDown.EventID)
            {
                nSetting = reportEvent.EventSoftwareShutDown.EventResult;
            }

            if (eventID == reportEvent.EventMachineIdle.EventID)
            {
                nSetting = reportEvent.EventMachineIdle.EventResult;
            }

            if (eventID == reportEvent.EventMachineHome.EventID)
            {
                nSetting = reportEvent.EventMachineHome.EventResult;
            }

            if (eventID == reportEvent.EventMachineEngineering.EventID)
            {
                nSetting = reportEvent.EventMachineEngineering.EventResult;
            }

            if (eventID == reportEvent.EventMachineConversion.EventID)
            {
                nSetting = reportEvent.EventMachineConversion.EventResult;
            }

            if (eventID == reportEvent.EventMachineMaintenance.EventID)
            {
                nSetting = reportEvent.EventMachineMaintenance.EventResult;
            }

            if (eventID == reportEvent.EventMachineMaterial.EventID)
            {
                nSetting = reportEvent.EventMachineMaterial.EventResult;
            }

            if (eventID == reportEvent.EventMachineResponse.EventID)
            {
                nSetting = reportEvent.EventMachineResponse.EventResult;
            }

            if (eventID == reportEvent.EventMachineProduction.EventID)
            {
                nSetting = reportEvent.EventMachineProduction.EventResult;
            }

            if (eventID == reportEvent.EventMachineFailure.EventID)
            {
                nSetting = reportEvent.EventMachineFailure.EventResult;
            }
            if (eventID == reportEvent.EventMachineMessage.EventID)
            {
                nSetting = reportEvent.EventMachineMessage.EventResult;
            }
            else
            {

            }
            return nSetting;
        }

        public int LoadPreviousLotSettings()
        {
            int nError = 0;
            try
            {
                if (Tools.IsFileExist(m_ProductShareVariables.strSaveLotInfoPath, "LotData", ".xml"))
                {
                    m_ProductShareVariables.productContinueLotInfo = Tools.Deserialize<ProductContinueLotInfo>(m_ProductShareVariables.strSaveLotInfoPath + "LotData.xml");
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            return nError;
        }
    }
}
