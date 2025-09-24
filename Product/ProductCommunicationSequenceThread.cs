using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using Common;

namespace Product
{
    public class ProductCommunicationSequenceThread
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
        public static string m_strSend = "";
        public static string m_strResponse = "";
        public TCPIP.AsychronousServer m_Server = null;

        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        private ProductRTSSProcess m_ProductRTSSProcess;// = new ProductRTSSProcess();
        private ProductProcessEvent m_ProductProcessEvent;// = new ProductProcessEvent()
        public tabpageVisionRecipe m_tabpageVisionRecipe = new tabpageVisionRecipe();
        public ProductRTSSProcess productRTSSProcess
        {
            set { m_ProductRTSSProcess = value; }
        }
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
        public void CommunicationThread()
        {
            try
            {
                int nInp_Receive_Data = 0;
                int nS1_Receive_Data = 0;
                int nOut_Pre_Receive_Data = 0;
                int nOut_Post_Receive_Data = 0;
                int nSort_Pre_Receive_Data = 0;
                int nSort_Post_Receive_Data = 0;
                int nReject_Pre_Receive_Data = 0;
                int nReject_Post_Receive_Data = 0;
                bool SendSingleBottomRC = false;
                bool GetSingleBottomRC = false;
                bool m_bPowerLost = false;
                int nNoOfInputUnit = 0;
                int nNoOfPickupHeadUnit = 0;
                int nNoOfOutputUnit = 0;
                int nNoOfSortingUnit = 0;
                int nNoOfOutputUnit_Post = 0;
                int nNoOfSortingUnit_Post = 0;
                m_Server = null;// = new TCPIP.AsychronousServer();
                int nNoOfRetrySendVisionNewLot = 0;
                int nNoOfRetrySendVisionRecipe = 0;
                m_ProductProcessEvent.GUI_PCS_ConnectCommunication.Set();
                ProductInputOutputFileFormat readInfo = new ProductInputOutputFileFormat();
                while (m_bRunThread)
                {
                    if (m_Server != null)
                    {
                        if (m_Server.IsConnected() == false || (m_Server.IsConnected() == true && m_Server.IsClientConnected() == false))
                        {
                            m_ProductShareVariables.strVisionConnection = "Disconnect";
                            //MessageBox.Show("Need reconnect");
                            m_ProductProcessEvent.GUI_PCS_ConnectCommunication.Set();
                        }
                    }

                    if (m_ProductProcessEvent.GUI_PCS_ConnectCommunication.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.Kill();
                            m_Server.Disconnect();
                            m_Server = null;
                            m_ProductShareVariables.strVisionConnection = "Disconnect";
                        }
                        m_Server = new TCPIP.AsychronousServer();
                        m_ProductShareVariables.barcodeDevice.BarcodeConnection = m_Server;
                        m_Server.ConfigureTimeOut(500, 500, 500);
                        if (m_Server.Connect("192.168.2.1", 5100) == true)
                        {
                            m_Server.ReceiveData();
                            m_ProductShareVariables.strVisionConnection = "Connect";
                            m_Server.SendData("client connected" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Vision Connected.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        else
                        {

                            Machine.EventLogger.WriteLog(string.Format("{0} : Fail to start listening.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            Machine.SequenceControl.SetAlarm(6007);
                            Machine.LogDisplay.AddLogDisplay("Error", "Fail to start listening");
                            //MessageBox.Show("Fail to start listening");
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_DisconnectCommunication.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            Machine.EventLogger.WriteLog(string.Format("{0} : Vision Disconnect.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.Disconnect();
                            m_Server.Kill();
                            m_Server = null;
                            m_ProductShareVariables.strVisionConnection = "Disconnect";
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_CommunicationSend.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            //(m_Server.SendData(m_strSend + "\r\n") ? Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision fail=>" + m_strSend + " with error " + m_Server.GetErrorMessage(), DateTime.Now.ToString("yyyyMMdd HHmmss"))): Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + m_strSend, DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData(m_strSend + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + m_strSend, DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_EndLot.WaitOne(0))
                    {
                        m_ProductProcessEvent.PCS_PCS_Vision_All_Inspection_Complete.Reset();
                        m_ProductProcessEvent.PCS_PCS_Vision_All_Inspection_Not_Complete.Reset();
                        if (m_Server != null)
                        {
                            //m_Server.SendData("EndLot=" + "\r\n");
                            //Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "EndLot=" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("ENDLOT=" + "INP" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "INP" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("ENDLOT=" + "BTM" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "BTM" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("ENDLOT=" + "SETUP" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "SETUP" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("ENDLOT=" + "SW2_PARTING" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "SW2_PARTING" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("ENDLOT=" + "SW3_PARTING" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "SW3_PARTING" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("ENDLOT=" + "OUT_PRE" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "OUT_PRE" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));


                            m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_CONTINUE_LOT", false);
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_NewLot.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            nNoOfRetrySendVisionNewLot = 0;
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_INP.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_OUT.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_BTM.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_REJECT.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW1.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW2.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW3.Reset();
                            //if (m_ProductShareVariables.strucInputProductInfo.InputFileNo == 1)
                            //{

                            //m_Server.SendData("NewLot=" + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            //Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));

                            m_Server.SendData("NewLot=" + "INP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "INP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "SW2_PARTING," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SW2," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "SETUP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SETUP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "SW1_1," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SW1," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "BTM," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "BTM," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "SW3_PARTING," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SW3," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "OUT_PRE," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "OUT," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "REJECT_PRE," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "REJECT," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));

                            if (m_ProductShareVariables.strCurrentBarcodeID != "")//&& m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_TRUE_TO_SEND_TILEID") == true)
                            {
                                if (m_ProductProcessEvent.PCS_PCS_Is_Sending_Vision_Setting.WaitOne(0))
                                {
                                    m_ProductProcessEvent.PCS_PCS_Is_Sending_Vision_Setting.Reset();
                                }
                                else
                                {
                                    //m_ProductProcessEvent.PCS_PCS_Send_Vision_TrayNo.Set();
                                }
                            }
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Resend_Vision_NewLot.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            nNoOfRetrySendVisionNewLot++;
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_INP.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_OUT.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_BTM.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_REJECT.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW1.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW2.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW3.Reset();

                            m_Server.SendData("NewLot=" + "INP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "INP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "SW2_PARTING," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SW2," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "SETUP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SETUP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "SW1_1," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SW1," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "BTM," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "BTM," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "SW3_PARTING," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SW3," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "OUT_PRE," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "OUT," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("NewLot=" + "REJECT_PRE," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "REJECT," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Resend_Vision_Setting.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            nNoOfRetrySendVisionRecipe++;
                            m_ProductProcessEvent.PCS_PCS_Resend_Vision_Setting.Reset();
                            if (m_ProductShareVariables.productRecipeMainSettings.InspectionRecipeName == "")
                            {
                                //m_Server.SendData("ProductName=" + ShareVariables.strucInputProductInfo.Recipe + "\r\n");
                                //EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ProductName=" + ShareVariables.strucInputProductInfo.Recipe + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                                m_Server.SendData("ProductName=" + m_ProductShareVariables.currentMainRecipeName + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Resend message to vision=>" + "ProductName=" + m_ProductShareVariables.currentMainRecipeName + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            else
                            {
                                m_Server.SendData("ProductName=" + m_ProductShareVariables.productRecipeMainSettings.InspectionRecipeName + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Resend message to vision=>" + "ProductName=" + m_ProductShareVariables.productRecipeMainSettings.InspectionRecipeName + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_LotID_BarcodeID.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            //nNoOfRetrySendVisionNewLot = 0;
                            m_ProductProcessEvent.PCS_PCS_Send_Vision_LotID_BarcodeID.Reset();
                            Thread.Sleep(1000);
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            nNoOfRetrySendVisionRecipe = 0;
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_Info_Fail.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_Setting.Reset();


                            if (m_ProductShareVariables.productRecipeMainSettings.InspectionRecipeName == "")
                            {
                                m_Server.SendData("ProductName=" + m_ProductShareVariables.currentMainRecipeName + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ProductName=" + m_ProductShareVariables.currentMainRecipeName + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            else
                            {
                                m_Server.SendData("ProductName=" + m_ProductShareVariables.productRecipeMainSettings.InspectionRecipeName + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ProductName=" + m_ProductShareVariables.productRecipeMainSettings.InspectionRecipeName + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_Calibration_Setting.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            nNoOfRetrySendVisionRecipe = 0;
                            //m_Server.SendData("ProductName=" + "crosshaircalib" + "\r\n");
                            m_Server.SendData("ProductName=" + m_ProductShareVariables.currentMainRecipeName + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ProductName=" + m_ProductShareVariables.currentMainRecipeName + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));

                        }
                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Calibration_Setting.Reset();
                    }

                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_OtherSettings.WaitOne(0))
                    {
                        if (m_Server != null)
                        {

                        }
                    }

                    if (m_bPowerLost == false && m_ProductRTSSProcess.GetEvent("PowerLost") == true)
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("POWER_LOST" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "POWER_LOST" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_bPowerLost = true;
                        }
                        if (m_ProductShareVariables.AlarmStart == 1)
                        {
                            m_ProductShareVariables.AlarmStart = 0;
                            m_ProductShareVariables.CurrentDownTimeCounterMES[m_ProductShareVariables.CurrentDownTimeNo].etime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (m_ProductProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0) == true)
                        {
                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_LOT_UNLOAD_COMPLETE", true);
                            m_ProductRTSSProcess.SetProductionInt("WriteReportTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo"));
                            m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_ENDLOT", true);
                            //if (m_ProductShareVariables.productOptionSettings.bEnableBarcodePrinter == true)
                            //{
                            //    m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_TRIGGER_BARCODE_PRINTER", true);
                            //}
                            m_ProductRTSSProcess.SetProductionInt("nInputRunningState", 0);
                            m_ProductRTSSProcess.SetProductionInt("nPNPRunningState", 0);
                            m_ProductRTSSProcess.SetProductionInt("nOutputRunningState", 0);
                            //if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputQuantity == true)
                            //{
                            //    if (m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotQuantityRun") < m_ProductRTSSProcess.GetProductionInt("nInputLotQuantity") /*&&( m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX")<=10)*/)
                            //    {
                            //        if (m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX") > 10)
                            //        {
                            //            m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateX", 1);
                            //            m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateY", 1);
                            //            //m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo")+1);
                            //        }
                            //        m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_SAVE_UNFINISHED_LOT", true);
                            //    }
                            //}
                            //else if (m_ProductShareVariables.productOptionSettings.EnableCountDownByInputTrayNo == true)
                            //{
                            //    if (m_ProductRTSSProcess.GetProductionInt("nCurrentInputLotTrayNoRun") < m_ProductRTSSProcess.GetProductionInt("nInputLotTrayNo") /*&&( m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX")<=10)*/)
                            //    {
                            //        if (m_ProductRTSSProcess.GetProductionInt("nEdgeCoordinateX") > 10)
                            //        {
                            //            m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateX", 1);
                            //            m_ProductRTSSProcess.SetProductionInt("nEdgeCoordinateY", 1);
                            //            //m_ProductRTSSProcess.SetProductionInt("nCurrentInputTrayNo", m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo")+1);
                            //        }
                            //        m_ProductRTSSProcess.SetEvent("RMAIN_GMAIN_SAVE_UNFINISHED_LOT", true);
                            //    }
                            //}

                        }
                    }

                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_INPUT_SEND_NEWLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_INPUT_SEND_NEWLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("NewLot=" + "INP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "INP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S2_SEND_NEWLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S2_SEND_NEWLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("NewLot=" + "SW2_PARTING," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SW2," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S1_SEND_NEWLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S1_SEND_NEWLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("NewLot=" + "SW1_1," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SW1_1," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //NEWLOT=S1,ACK\r\n
                            //NEWLOT=S1,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S3_SEND_NEWLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S3_SEND_NEWLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("NewLot=" + "SW3_PARTING," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "SW3," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //NEWLOT=S3,ACK\r\n
                            //NEWLOT=S3,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_OUTPUT_SEND_NEWLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_OUTPUT_SEND_NEWLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("NewLot=" + "OUT_PRE," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "OUT," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //NEWLOT=OUT,ACK\r\n
                            //NEWLOT=OUT,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_REJECT_SEND_NEWLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_REJECT_SEND_NEWLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("NewLot=" + "REJECT_PRE," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "REJECT," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //NEWLOT=REJECT1,ACK\r\n
                            //NEWLOT=REJECT1,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_BOTTOM_SEND_NEWLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_BOTTOM_SEND_NEWLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("NewLot=" + "BTM," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=" + "BTM," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //NEWLOT=REJECT5,ACK\r\n
                            //NEWLOT=REJECT5,NAK,00\r\n
                        }
                    }

                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_INPUT_SEND_TRAYNO") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_INPUT_SEND_TRAYNO", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("TrayNo=" + "INP," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "TrayNo=" + "INP," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //TrayNo=INP,ACK\r\n
                            //TrayNo=INP,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S2_SEND_TRAYNO") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S2_SEND_TRAYNO", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("TrayNo=" + "SW2_PARTING," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "TrayNo=" + "S2," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //TrayNo=S2,ACK\r\n
                            //TrayNo=S2,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S1_SEND_TRAYNO") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S1_SEND_TRAYNO", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("TrayNo=" + "SW1_1," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "TrayNo=" + "SW1_1," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //TrayNo=S1,ACK\r\n
                            //TrayNo=S1,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S3_SEND_TRAYNO") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S3_SEND_TRAYNO", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("TrayNo=" + "SW3_PARTING," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "TrayNo=" + "S3," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //TrayNo=S3,ACK\r\n
                            //TrayNo=S3,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_BTM_SEND_TRAYNO") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_BTM_SEND_TRAYNO", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("TrayNo=" + "BTM," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "TrayNo=" + "BTM," + m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //TrayNo=S3,ACK\r\n
                            //TrayNo=S3,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_OUTPUT_SEND_TRAYNO") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_OUTPUT_SEND_TRAYNO", false);
                        if (m_Server != null)
                        {

                            m_Server.SendData("TrayNo=" + "OUT_PRE," + m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo") + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "TrayNo=" + "OUT," + m_ProductShareVariables.OutputTrayIDFromBarcode + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));

                            //TrayNo=OUT,ACK\r\n
                            //TrayNo=OUT,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_REJECT_SEND_TRAYNO") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_REJECT_SEND_TRAYNO", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("TrayNo=" + "REJECT_PRE," + m_ProductRTSSProcess.GetProductionInt("nCurrentRejectTrayNo").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "TrayNo=" + "REJECT," + m_ProductRTSSProcess.GetProductionInt("nCurrentRejectTrayNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //TrayNo=REJECT1,ACK\r\n
                            //TrayNo=REJECT1,NAK,00\r\n
                        }
                    }

                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_INPUT_SEND_ENDLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_INPUT_SEND_ENDLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("ENDLOT=" + "INP" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "INP" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ENDLOT=INP,ACK\r\n
                            //ENDLOT=INP,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_BTM_SEND_ENDLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_BTM_SEND_ENDLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("ENDLOT=" + "BTM" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "BTM" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ENDLOT=INP,ACK\r\n
                            //ENDLOT=INP,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S2_SEND_ENDLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S2_SEND_ENDLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("ENDLOT=" + "SW2_PARTING" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "S2" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ENDLOT=S2,ACK\r\n
                            //ENDLOT=S2,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S1_SEND_ENDLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S1_SEND_ENDLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("ENDLOT=" + "SW1_1" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "SW1_1" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ENDLOT=S1,ACK\r\n
                            //ENDLOT=S1,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S3_SEND_ENDLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S3_SEND_ENDLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("ENDLOT=" + "SW3_PARTING" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "S3" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ENDLOT=S3,ACK\r\n
                            //ENDLOT=S3,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_OUTPUT_SEND_ENDLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_OUTPUT_SEND_ENDLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("ENDLOT=" + "OUT_PRE" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "OUT" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ENDLOT=OUT,ACK\r\n
                            //ENDLOT=OUT,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_REJECT_SEND_ENDLOT") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_REJECT_SEND_ENDLOT", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("ENDLOT=" + "REJECT_PRE" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ENDLOT=" + "REJECT1" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ENDLOT=REJECT1,ACK\r\n
                            //ENDLOT=REJECT1,NAK,00\r\n
                        }
                    }

                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_INPUT_SEND_ENDTRAY") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_INPUT_SEND_ENDTRAY", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("EndTray=" + "INP" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "EndTray=" + "INP" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //EndTray=INP,ACK\r\n
                            //EndTray=INP,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_BTM_SEND_ENDTRAY") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_BTM_SEND_ENDTRAY", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("EndTray=" + "BTM" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "EndTray=" + "BTM" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //EndTray=INP,ACK\r\n
                            //EndTray=INP,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S2_SEND_ENDTRAY") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S2_SEND_ENDTRAY", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("EndTray=" + "SW2_PARTING" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "EndTray=" + "SW2_PARTING" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //EndTray=S2,ACK\r\n
                            //EndTray=S2,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S1_SEND_ENDTRAY") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S1_SEND_ENDTRAY", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("EndTray=" + "SW1_1" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "EndTray=" + "SW1_1" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //EndTray=S1,ACK\r\n
                            //EndTray=S1,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_S3_SEND_ENDTRAY") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S3_SEND_ENDTRAY", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("EndTray=" + "SW3_PARTING" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "EndTray=" + "SW3_PARTING" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //EndTray=S3,ACK\r\n
                            //EndTray=S3,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_OUTPUT_SEND_ENDTRAY") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_OUTPUT_SEND_ENDTRAY", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("EndTray=" + "OUT_PRE" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "EndTray=" + "OUT" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //EndTray=OUT,ACK\r\n
                            //EndTray=OUT,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_REJECT_SEND_ENDTRAY") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_REJECT_SEND_ENDTRAY", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("EndTray=" + "REJECT_PRE" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "EndTray=" + "REJECT1" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //EndTray=REJECT1,ACK\r\n
                            //EndTray=REJECT1,NAK,00\r\n
                        }
                    }

                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_INP_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_INP_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("INP=" + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "INP=" + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_INP_VISION_ADD_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_INP_VISION_ADD_RC_START", false);
                        if (m_Server != null)
                        {
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                                m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn"))];

                            m_Server.SendData($"INP_{m_ProductRTSSProcess.GetProductionInt("CurrentInputVisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"INP_{m_ProductRTSSProcess.GetProductionInt("CurrentInputVisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID
                                + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_SETUP_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_SETUP_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                               m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow"),
                               m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn"))];

                            m_Server.SendData("SETUP=" + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "SETUP=" + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S2_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S2_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("S2=" + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "S2=" + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S2_VISION_ADD_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S2_VISION_ADD_RC_START", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData($"S2_{m_ProductRTSSProcess.GetProductionInt("nS2VisionAdditionalSnapNo")}="
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"S2_{m_ProductRTSSProcess.GetProductionInt("nS2VisionAdditionalSnapNo")}="
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S2_PARTING_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                               m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow"),
                               m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn"))];

                            m_Server.SendData($"SW2_PARTING="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"S2_PARTING="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S2_FACET_VISION_RC_START", false);
                        if (m_Server != null)
                        {//CurrentS2VisionLoopNo
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                               m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow"),
                               m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn"))];

                            m_Server.SendData($"SW2_FACET_{m_ProductRTSSProcess.GetProductionInt("CurrentS2VisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"S2_FACET_{m_ProductRTSSProcess.GetProductionInt("CurrentS2VisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S1_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S1_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow"),
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn"))];

                            m_Server.SendData($"SW1_{m_ProductRTSSProcess.GetProductionInt("CurrentS1VisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","

                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n");

                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"SW1_{m_ProductRTSSProcess.GetProductionInt("CurrentS1VisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                              + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S1_VISION_ADD_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S1_VISION_ADD_RC_START", false);
                        if (m_Server != null)
                        {
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow"),
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn"))];

                            m_Server.SendData($"SW1_{m_ProductRTSSProcess.GetProductionInt("CurrentS1VisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n");

                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"SW1_{m_ProductRTSSProcess.GetProductionInt("CurrentS1VisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_BOTTOM_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_BOTTOM_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            if(m_ProductRTSSProcess.GetEvent("GMNL_RMNL_MANUAL_MODE") == true)
                            {
                                m_Server.SendData("BTM=" + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                    + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                    + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                    + "-" + ","
                                    + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "BTM=" + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                    + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                    + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                    + "-" + ","
                                    + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            else
                            {
                                int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow"),
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn"))];

                                m_Server.SendData("BTM=" + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                    + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                    + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                    + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                    + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "BTM=" + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                    + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                    + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                    + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                    + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_BOTTOM_VISION_ADD_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_BOTTOM_VISION_ADD_RC_START", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData($"BTM_{m_ProductRTSSProcess.GetProductionInt("nS1VisionAdditionalSnapNo")}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"BTM_{m_ProductRTSSProcess.GetProductionInt("nS1VisionAdditionalSnapNo")}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S3_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S3_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("S3=" + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "S3=" + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S3_VISION_ADD_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S3_VISION_ADD_RC_START", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData($"S3_{m_ProductRTSSProcess.GetProductionInt("nS3VisionAdditionalSnapNo")}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"S3_{m_ProductRTSSProcess.GetProductionInt("nS3VisionAdditionalSnapNo")}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S3_PARTING_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow"),
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn"))];

                            m_Server.SendData($"SW3_PARTING="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"S3_PARTING="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                               + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S3_FACET_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow"),
                                m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn"))];

                            m_Server.SendData($"SW3_FACET_{m_ProductRTSSProcess.GetProductionInt("CurrentS2VisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"S3_FACET_{m_ProductRTSSProcess.GetProductionInt("CurrentS2VisionLoopNo") + 1}="
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_OUT_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_OUT_VISION_RC_START", false);
                        if (m_Server != null)
                        {

                            m_Server.SendData("OUT_PRE=" + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputTrayNo").ToString()
                                + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "OUT_PRE=" + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputTrayNo").ToString()
                                + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_OUT_POST_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_OUT_POST_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                              m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"),
                              m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))];

                            m_Server.SendData("OUT_POST=" + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo").ToString()
                                + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "OUT_POST=" + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo").ToString()
                                + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_OUT_POST_VISION_ADD_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_OUT_POST_VISION_ADD_RC_START", false);
                        if (m_Server != null)
                        {

                            m_Server.SendData($"OUT_POST_{m_ProductRTSSProcess.GetProductionInt("nOutputVisionAdditionalSnapNo")}= "
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductShareVariables.OutputTrayIDFromBarcode + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput").ToString()
                                + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo").ToString()
                                + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + $"OUT_POST_{m_ProductRTSSProcess.GetProductionInt("nOutputVisionAdditionalSnapNo")}= "
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductShareVariables.OutputTrayIDFromBarcode + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput").ToString()
                                + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo").ToString()
                                + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));

                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_REJECT_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_REJECT_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            if (m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "CurrentOutputTableNo") == 0)
                            {
                                m_Server.SendData("REJECT_PRE=" + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString() + ","
                                    + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString() + ","
                                    + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectTrayNo").ToString()
                                    + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "REJECT_PRE=" + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString() + ","
                                    + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString() + ","
                                    + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectTrayNo").ToString()
                                    + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_REJECT_POST_VISION_RC_START"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_REJECT_POST_VISION_RC_START", false);
                        if (m_Server != null)
                        {
                            int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}",
                              m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"),
                              m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))];

                            if (m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "CurrentOutputTableNo") == 0)
                            {
                                m_Server.SendData("REJECT_POST=" + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo").ToString()
                                + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "REJECT_POST=" + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectTrayNo").ToString() + ","
                                + m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID + ","
                                + m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtOutput").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow").ToString() + ","
                                + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn").ToString() + ","
                                + m_ProductShareVariables.strucInputProductInfo.LotID + "," + m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo").ToString()
                                + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_EndTile.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_ProductProcessEvent.PCS_PCS_Vision_All_Inspection_Complete.Reset();
                            m_ProductProcessEvent.PCS_PCS_Vision_All_Inspection_Not_Complete.Reset();
                            m_Server.SendData("EndTile=" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "EndTile=" + m_ProductShareVariables.strCurrentBarcodeID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //TileID,ACK\r\n
                            //TileID,NACK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Set_Vision_Live_On.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("ALGCONFIG_LIVE_ON" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ALGCONFIG_LIVE_ON" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALGCONFIG_LIVE_ON,ACK\r\n
                            //ALGCONFIG_LIVE_ON,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Set_Vision_Live_Off.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("ALGCONFIG_LIVE_OFF" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ALGCONFIG_LIVE_OFF" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALGCONFIG_LIVE_ON,ACK\r\n
                            //ALGCONFIG_LIVE_ON,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Set_Vision_Launch_Teach_Window.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            //m_Server.SendData("CONFIG_ALIGNMENT_ON" + "\r\n");
                            //Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "CONFIG_ALIGNMENT_ON" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //CONFIG_ALIGNMENT_ON,ACK\r\n
                            //CONFIG_ALIGNMENT_ON,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Set_Vision_Close_Teach_Window.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            //m_Server.SendData("CONFIG_ALIGNMENT_OFF" + "\r\n");
                            //Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "CONFIG_ALIGNMENT_OFF" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //CONFIG_ALIGNMENT_OFF,ACK\r\n
                            //CONFIG_ALIGNMENT_OFF,NAK,00\r\n
                        }
                    }
                    //if (m_ProductProcessEvent.PCS_PCS_Set_Vision_Launch_Teach_Window.WaitOne(0))
                    //{
                    //    if (m_Server != null)
                    //    {
                    //        m_Server.SendData("CONFIG_ALIGNMENT" + "\r\n");
                    //        Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "CONFIG_ALIGNMENT" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                    //        //CONFIG_ALIGNMENT,ACK\r\n
                    //        //CONFIG_ALIGNMENT,NAK,00\r\n
                    //    }
                    //}
                    if (m_ProductProcessEvent.PCS_PCS_Set_Vision_Teach.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            if (m_ProductShareVariables.TeachItem >= 1 && m_ProductShareVariables.TeachItem <= 10)
                            {
                                m_Server.SendData("ALGCONFIG_SETMODE_FID=" + m_ProductShareVariables.TeachItem.ToString() + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ALGCONFIG_SETMODE_FID " + m_ProductShareVariables.TeachItem.ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                                //ALGCONFIG_SETMODE_FID N,ACK\r\n
                                //ALGCONFIG_SETMODE_FID,NAK,00\r\n
                            }
                            else if (m_ProductShareVariables.TeachItem == 11)
                            {
                                m_Server.SendData("ALGCONFIG_SETMODE_XYTHETA=" + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ALGCONFIG_SETMODE_XYTHETA" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                                //ALGCONFIG_SETMODE_XYTHETA,ACK\r\n
                                //ALGCONFIG_SETMODE_XYTHETA,NAK,00\r\n
                            }
                            else if (m_ProductShareVariables.TeachItem == 12)
                            {
                                m_Server.SendData("ALGCONFIG_SETMODE_XYTHETA=" + "\r\n");
                                Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ALGCONFIG_SETMODE_XYTHETA" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                                //ALGCONFIG_SETMODE_XYTHETA,ACK\r\n
                                //ALGCONFIG_SETMODE_XYTHETA,NAK,00\r\n
                            }
                        }
                    }

                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_Remove_RC.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            //m_Server.SendData(m_ProductShareVariables.RemoveRC + "\r\n");
                            //Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "m_ProductShareVariables.RemoveRC" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //RemoveRC,ACK\r\n
                            //RemoveRC,NAK,00\r\n
                        }
                    }

                    //if (m_ProductRTSSProcess.GetEvent("RMNL_GMAIN_SET_MANUAL_MODE_START_VISION") == true)
                    //{
                    //    m_ProductRTSSProcess.SetEvent("RMNL_GMAIN_SET_MANUAL_MODE_START_VISION", false);
                    //    if (m_Server != null)
                    //    {
                    //        m_Server.SendData(string.Format("StationRequireResult={0};{1};{2};{3};{4};{5};{6};{7};{8}\r\n"
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 0, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 1, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 2, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 3, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 4, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 5, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 6, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 7, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 8, "")));
                    //        Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + string.Format("StationRequireResult={0};{1};{2};{3};{4};{5};{6};{7};{8}\r\n"
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 0, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 1, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 2, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 3, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 4, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 5, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 6, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 7, "")
                    //            , m_ProductRTSSProcess.GetProductionArray("ManualVisionTrigger", 8, "")), DateTime.Now.ToString("yyyyMMdd HHmmss")));
                    //        //SetShareMemoryEvent("RS1V_RSEQ_MANUAL_MODE_VISION_DONE", true);
                    //    }
                    //}
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_SETUP_VISION_LATENCY_ON_START") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_SETUP_VISION_LATENCY_ON_START", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("NewLot=SETUP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "NewLot=SETUP," + m_ProductShareVariables.strucInputProductInfo.LotID + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            m_Server.SendData("SETUPLC=ON" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "SETUPLC=ON" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //S1LC=ON,ACK\r\n
                            //S1LC_ON,NAK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_SETUP_VISION_LATENCY_OFF_START") == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_SETUP_VISION_LATENCY_OFF_START", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("SETUPLC=OFF" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "SETUPLC=OFF" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //S1LC_OFF,ACK\r\n
                            //S1LC_OFF,NAK,00\r\n
                        }
                    }

                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_Manual_Mode_On.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            //m_Server.SendData("ManualMode=On\r\n");
                            //Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ManualMode=On\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ManualModeOn,ACK\r\n
                            //ManualModeOn,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_Manual_Mode_Off.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("ManualMode=Off\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ManualMode=Off\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ManualModeOff,ACK\r\n
                            //ManualModeOff,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_DryRun_Mode_On.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("DryRunMode=On\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "DryRunMode=On\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //DryRunMode,ACK\r\n
                            //DryRunMode,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_DryRun_Mode_Off.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("DryRunMode=Off\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "DryRunMode=Off\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //DryRunMode,ACK\r\n
                            //DryRunMode,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.PCS_PCS_Send_Vision_Report.WaitOne(0))
                    {
                        if (m_ProductShareVariables.strucInputProductInfo.InputFileNo == 1)
                        {
                            m_Server.SendData("Report=" + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput
                                + "\\" + m_ProductShareVariables.strCurrentBarcodeID + "," + "Report-" + m_ProductShareVariables.strCurrentBarcodeID + ".txt" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "Report=" + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput
                        + "\\" + m_ProductShareVariables.strCurrentBarcodeID + "," + "Report-" + m_ProductShareVariables.strCurrentBarcodeID + ".txt" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        else if (m_ProductShareVariables.strucInputProductInfo.InputFileNo == 2)
                        {
                            m_Server.SendData("Report=" + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput2
                                + "\\" + m_ProductShareVariables.strCurrentBarcodeID + "," + "Report-" + m_ProductShareVariables.strCurrentBarcodeID + ".txt" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "Report=" + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput2
                        + "\\" + m_ProductShareVariables.strCurrentBarcodeID + "," + "Report-" + m_ProductShareVariables.strCurrentBarcodeID + ".txt" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        else
                        {
                            m_Server.SendData("Report=" + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput
                                + "\\" + m_ProductShareVariables.strCurrentBarcodeID + "," + "Report-" + m_ProductShareVariables.strCurrentBarcodeID + ".txt" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "Report=" + "\\Output\\" + m_ProductShareVariables.strucInputProductInfo.LotIDOutput
                        + "\\" + m_ProductShareVariables.strCurrentBarcodeID + "," + "Report-" + m_ProductShareVariables.strCurrentBarcodeID + ".txt" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Initialize.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("ALNCALMODE=On\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ALNCALMODE=On\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALNCALMODE=On,ACK\r\n
                            //ALNCALMODE=On,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_End.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("ALNCALMODE=Off\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ALNCALMODE=Off\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALNCALMODE=Off,ACK\r\n
                            //ALNCALMODE=Off,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Snap.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("ALNCAL=1\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ALNCAL=1\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALNCAL=1,ACK\r\n
                            //ALNCAL=1,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_Snap2.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("ALNCAL=2\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "ALNCAL=2\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALNCAL=2,ACK\r\n
                            //ALNCAL=2,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_Aligner_Center_Rotation_Teach_GetResult.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("GETALNCAL\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "GETALNCAL\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //GETALNCAL= X, Y,ACK\r\n
                            //GETALNCAL,NAK,00\r\n
                        }
                    }

                    if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Initialize.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("INPCALMODE=On\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "INPCALMODE=On\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALNCALMODE=On,ACK\r\n
                            //ALNCALMODE=On,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_End.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("INPCALMODE=Off\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "INPCALMODE=Off\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALNCALMODE=Off,ACK\r\n
                            //ALNCALMODE=Off,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Snap.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("INPCAL=1\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "INPCAL=1\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALNCAL=1,ACK\r\n
                            //ALNCAL=1,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_Snap2.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("INPCAL=2\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "INPCAL=2\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //ALNCAL=2,ACK\r\n
                            //ALNCAL=2,NAK,00\r\n
                        }
                    }
                    if (m_ProductProcessEvent.GUI_PCS_IPT_Center_Rotation_Teach_GetResult.WaitOne(0))
                    {
                        if (m_Server != null)
                        {
                            m_Server.SendData("GETINPCAL\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "GETINPCAL\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //GETALNCAL= X, Y,ACK\r\n
                            //GETALNCAL,NAK,00\r\n
                        }
                    }
                    if (m_bRunThread == false)
                    {
                        break;
                    }


                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_INP_VISION_VERIFY_DOTGRID"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_INP_VISION_VERIFY_DOTGRID", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyDotgrids=INP" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyDotgrids=INP" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyDotgrids,ACK\r\n
                            //VerifyDotgrids,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S2_VISION_VERIFY_DOTGRID"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S2_VISION_VERIFY_DOTGRID", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyDotgrids=S2" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyDotgrids=S2" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyDotgrids,ACK\r\n
                            //VerifyDotgrids,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S1_VISION_VERIFY_DOTGRID"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S1_VISION_VERIFY_DOTGRID", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyDotgrids=S1" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyDotgrids=S1" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyDotgrids,ACK\r\n
                            //VerifyDotgrids,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S3_VISION_VERIFY_DOTGRID"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S3_VISION_VERIFY_DOTGRID", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyDotgrids=S3" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyDotgrids=S3" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyDotgrids,ACK\r\n
                            //VerifyDotgrids,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_OUT_VISION_VERIFY_DOTGRID"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_OUT_VISION_VERIFY_DOTGRID", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyDotgrids=OUT" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyDotgrids=OUT" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyDotgrids,ACK\r\n
                            //VerifyDotgrids,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_BTM_VISION_VERIFY_DOTGRID"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_BTM_VISION_VERIFY_DOTGRID", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyDotgrids=BTM" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyDotgrids=BTM" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyDotgrids,ACK\r\n
                            //VerifyDotgrids,NACK,00\r\n
                        }
                    }

                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_INP_VISION_VERIFY_GRAYSCALE"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_INP_VISION_VERIFY_GRAYSCALE", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyGrayscale=INP" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyGrayscale=INP" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyGrayscale,ACK\r\n
                            //VerifyGrayscale,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S2_VISION_VERIFY_GRAYSCALE"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S2_VISION_VERIFY_GRAYSCALE", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyGrayscale=S2" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyGrayscale=S2" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyGrayscale,ACK\r\n
                            //VerifyGrayscale,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S1_VISION_VERIFY_GRAYSCALE"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S1_VISION_VERIFY_GRAYSCALE", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyGrayscale=S1" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyGrayscale=S1" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyGrayscale,ACK\r\n
                            //VerifyGrayscale,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_S3_VISION_VERIFY_GRAYSCALE"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_S3_VISION_VERIFY_GRAYSCALE", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyGrayscale=S3" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyGrayscale=S3" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyGrayscale,ACK\r\n
                            //VerifyGrayscale,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_OUT_VISION_VERIFY_GRAYSCALE"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_OUT_VISION_VERIFY_GRAYSCALE", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyGrayscale=OUT" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyGrayscale=OUT" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyGrayscale,ACK\r\n
                            //VerifyGrayscale,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SEND_BTM_VISION_VERIFY_GRAYSCALE"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_BTM_VISION_VERIFY_GRAYSCALE", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("VerifyGrayscale=BTM" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "VerifyGrayscale=BTM" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            //VerifyGrayscale,ACK\r\n
                            //VerifyGrayscale,NACK,00\r\n
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RINPV_GMAIN_INP_VISION_RESET_EOV"))
                    {
                        m_ProductRTSSProcess.SetEvent("RINPV_GMAIN_INP_VISION_RESET_EOV", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("INP_REOV" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "INP_REOV" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RS2V_GMAIN_S2_VISION_RESET_EOV"))
                    {
                        m_ProductRTSSProcess.SetEvent("RS2V_GMAIN_S2_VISION_RESET_EOV", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("SW2_REOV" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "SW2REOV" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RSTPV_GMAIN_SETUP_VISION_RESET_EOV"))
                    {
                        m_ProductRTSSProcess.SetEvent("RSTPV_GMAIN_SETUP_VISION_RESET_EOV", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("SETUP_REOV" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "SETUPREOV" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RBTMV_GMAIN_BOTTOM_VISION_RESET_EOV"))
                    {
                        m_ProductRTSSProcess.SetEvent("RBTMV_GMAIN_BOTTOM_VISION_RESET_EOV", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("SW1_1_REOV" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "S1REOV" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }

                    if (m_ProductRTSSProcess.GetEvent("RS3V_GMAIN_S3_VISION_RESET_EOV"))
                    {
                        m_ProductRTSSProcess.SetEvent("RS3V_GMAIN_S3_VISION_RESET_EOV", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("SW3_REOV" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "S3REOV" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("ROUTV_GMAIN_OUT_VISION_RESET_EOV"))
                    {
                        m_ProductRTSSProcess.SetEvent("ROUTV_GMAIN_OUT_VISION_RESET_EOV", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("OUT_PRE_REOV" + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "OUTREOV" + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SW1_SEND_SNAP_POS"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SW1_SEND_SNAP_POS", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("SETSNAPPOS=SW1," + (m_ProductRTSSProcess.GetProductionInt("CurrentS1VisionLoopNo") + 1).ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "SetSnapPos=SW1," + m_ProductRTSSProcess.GetProductionInt("CurrentS1VisionLoopNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SW2_FACET_SEND_SNAP_POS"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SW2_FACET_SEND_SNAP_POS", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("SETSNAPPOS=SW2_FACET," + (m_ProductRTSSProcess.GetProductionInt("CurrentS2VisionLoopNo") + 1).ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "SetSnapPos=SW2_FACET," + m_ProductRTSSProcess.GetProductionInt("CurrentS2VisionLoopNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_ProductRTSSProcess.GetEvent("RTHD_GMAIN_SW3_FACET_SEND_SNAP_POS"))
                    {
                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SW3_FACET_SEND_SNAP_POS", false);
                        if (m_Server != null)
                        {
                            m_Server.SendData("SETSNAPPOS=SW3_FACET," + (m_ProductRTSSProcess.GetProductionInt("CurrentS2VisionLoopNo") + 1).ToString() + "\r\n");
                            Machine.EventLogger.WriteLog(string.Format("{0} : Send message to vision=>" + "SetSnapPos=SW3_FACET," + m_ProductRTSSProcess.GetProductionInt("CurrentS2VisionLoopNo").ToString() + "\r\n", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                    }
                    if (m_Server != null)
                    {
                        m_strResponse = m_Server.GetListData();
                        m_Server.ClearData(m_strResponse);
                        if (m_strResponse.Length >= 2)
                        {
                            if (m_strResponse.Substring(m_strResponse.Length - 2, 2) != "\r\n")
                            {
                                HiPerfTimer timerClientReply = new HiPerfTimer();
                                timerClientReply.Start();

                                string m_tempResponse;
                                while (m_bRunThread)
                                {
                                    m_tempResponse = m_Server.GetListData();
                                    m_strResponse += m_tempResponse;
                                    m_Server.ClearData(m_tempResponse);
                                    Machine.EventLogger.WriteLog(string.Format("{0} : Combine message from vision 1=>" + m_tempResponse, DateTime.Now.ToString("yyyyMMdd HHmmss")));
                                    if (m_tempResponse.Length >= 2)
                                    {
                                        if (m_strResponse.Substring(m_tempResponse.Length - 2, 2) == "\r\n")
                                        {
                                            break;
                                        }
                                    }
                                    timerClientReply.Elapse();
                                    if (timerClientReply.ElapseMiliSecond() >= 3000)
                                    {
                                        Machine.EventLogger.WriteLog(string.Format("{0} : Possible missing data from vision 1=>", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                                        break;
                                    }
                                    Thread.Sleep(1);
                                }
                            }
                        }
                        else if (m_strResponse != "")
                        {
                            HiPerfTimer timerClientReply = new HiPerfTimer();
                            timerClientReply.Start();

                            string m_tempResponse;
                            while (m_bRunThread)
                            {
                                m_tempResponse = m_Server.GetListData();
                                m_strResponse += m_tempResponse;
                                m_Server.ClearData(m_tempResponse);
                                Machine.EventLogger.WriteLog(string.Format("{0} : Combine message from vision=>" + m_tempResponse, DateTime.Now.ToString("yyyyMMdd HHmmss")));
                                if (m_tempResponse.Length >= 2)
                                {
                                    if (m_strResponse.Substring(m_tempResponse.Length - 2, 2) == "\r\n")
                                    {
                                        break;
                                    }
                                }
                                timerClientReply.Elapse();
                                if (timerClientReply.ElapseMiliSecond() >= 3000)
                                {
                                    Machine.EventLogger.WriteLog(string.Format("{0} : Possible missing data from vision=>", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                                    break;
                                }
                                Thread.Sleep(1);
                            }
                        }
                        if (m_strResponse != "" && m_strResponse != null)
                        {
                            Machine.EventLogger.WriteLog(string.Format("{0} : Receive message from vision=>" + m_strResponse, DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            string[] strReceive = Regex.Split(m_strResponse, "\r\n");
                            foreach (string _strReceive in strReceive)
                            {
                                if (_strReceive == "")
                                {
                                    //Ignore
                                    continue;
                                }
                                if (_strReceive.Contains("Hello") || _strReceive.Contains("Vision Connected"))
                                {
                                    m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                }
                                int nIndex = _strReceive.IndexOf('=');
                                if (nIndex == 0 || nIndex == -1)
                                {
                                    if (_strReceive == "RC,ACK" || _strReceive == "INP,ACK")
                                    {
                                        nInp_Receive_Data = 0;
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INP_VISION_GET_RC_DONE", true);
                                        //m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INP_VISION_GET_ADD_RC_DONE", true);
                                    }
                                    else if (_strReceive == "INP,NAK,04" || _strReceive == "INP,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INP_VISION_GET_RC_NAK", true);

                                    }
                                    else if (_strReceive == "INP_1,ACK" || _strReceive == "INP_2,ACK" || _strReceive == "INP_3,ACK"
                                       || _strReceive == "INP_4,ACK" || _strReceive == "INP_5,ACK" || _strReceive == "INP_6,ACK"
                                       || _strReceive == "INP_7,ACK" || _strReceive == "INP_8,ACK" || _strReceive == "INP_9,ACK"
                                       || _strReceive == "INP_10,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INP_VISION_GET_ADD_RC_DONE", true);
                                    }
                                    else if (_strReceive == "INP_1,NAK,04" || _strReceive == "INP_2,NAK,04" || _strReceive == "INP_3,NAK,04"
                                       || _strReceive == "INP_4,NAK,04" || _strReceive == "INP_5,NAK,04" || _strReceive == "INP_6,NAK,04"
                                       || _strReceive == "INP_7,NAK,04" || _strReceive == "INP_8,NAK,04" || _strReceive == "INP_9,NAK,04"
                                       || _strReceive == "INP_10,NAK,04")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INP_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "INP_1,NAK,05" || _strReceive == "INP_2,NAK,05" || _strReceive == "INP_3,NAK,05"
                                      || _strReceive == "INP_4,NAK,05" || _strReceive == "INP_5,NAK,05" || _strReceive == "INP_6,NAK,05"
                                      || _strReceive == "INP_7,NAK,05" || _strReceive == "INP_8,NAK,05" || _strReceive == "INP_9,NAK,05"
                                      || _strReceive == "INP_10,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INP_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "BTM,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BOTTOM_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "BTM,NAK,04" || _strReceive == "BTM,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BOTTOM_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "S2,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "S2_1,ACK" || _strReceive == "S2_2,ACK" || _strReceive == "S2_3,ACK"
                                       || _strReceive == "S2_4,ACK" || _strReceive == "S2_5,ACK" || _strReceive == "S2_6,ACK"
                                       || _strReceive == "S2_7,ACK" || _strReceive == "S2_8,ACK" || _strReceive == "S2_9,ACK"
                                       || _strReceive == "S2_10,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_VISION_GET_ADD_RC_DONE", true);
                                    }
                                    else if (_strReceive == "SW2_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_PARTING_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "SW2_PARTING,NAK,04" || _strReceive == "SW2_PARTING,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_PARTING_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "SW2_FACET_1,ACK" || _strReceive == "SW2_FACET_2,ACK" || _strReceive == "SW2_FACET_3,ACK"
                                        || _strReceive == "SW2_FACET_4,ACK" || _strReceive == "SW2_FACET_5,ACK" || _strReceive == "SW2_FACET_6,ACK"
                                        || _strReceive == "SW2_FACET_7,ACK" || _strReceive == "SW2_FACET_8,ACK" || _strReceive == "SW2_FACET_9,ACK"
                                        || _strReceive == "SW2_FACET_10,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_FACET_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "SW2_FACET_1,NAK,04" || _strReceive == "SW2_FACET_2,NAK,04" || _strReceive == "SW2_FACET_3,NAK,04"
                                        || _strReceive == "SW2_FACET_4,NAK,04" || _strReceive == "SW2_FACET_5,NAK,04" || _strReceive == "SW2_FACET_6,NAK,04"
                                        || _strReceive == "SW2_FACET_7,NAK,04" || _strReceive == "SW2_FACET_8,NAK,04" || _strReceive == "SW2_FACET_9,NAK,04"
                                        || _strReceive == "SW2_FACET_10,NAK,04")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "SW2_FACET_1,NAK,05" || _strReceive == "SW2_FACET_2,NAK,05" || _strReceive == "SW2_FACET_3,NAK,05"
                                        || _strReceive == "SW2_FACET_4,NAK,05" || _strReceive == "SW2_FACET_5,NAK,05" || _strReceive == "SW2_FACET_6,NAK,05"
                                        || _strReceive == "SW2_FACET_7,NAK,05" || _strReceive == "SW2_FACET_8,NAK,05" || _strReceive == "SW2_FACET_9,NAK,05"
                                        || _strReceive == "SW2_FACET_10,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_FACET_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "SETUP,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_SETUP_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "SETUP,NAK,04" || _strReceive == "SETUP,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_SETUP_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "SW1_1,ACK" || _strReceive == "SW1_2,ACK" || _strReceive == "SW1_3,ACK"
                                        || _strReceive == "SW1_4,ACK" || _strReceive == "SW1_5,ACK" || _strReceive == "SW1_6,ACK"
                                        || _strReceive == "SW1_7,ACK" || _strReceive == "SW1_8,ACK" || _strReceive == "SW1_9,ACK"
                                        || _strReceive == "SW1_10,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S1_VISION_GET_RC_DONE", true);
                                        //m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BOTTOM_VISION_GET_ADD_RC_DONE", true);
                                    }
                                    else if (_strReceive == "SW1_1,NAK,04" || _strReceive == "SW1_2,NAK,04" || _strReceive == "SW1_3,NAK,04"
                                        || _strReceive == "SW1_4,NAK,04" || _strReceive == "SW1_5,NAK,04" || _strReceive == "SW1_6,NAK,04"
                                        || _strReceive == "SW1_7,NAK,04" || _strReceive == "SW1_8,NAK,04" || _strReceive == "SW1_9,NAK,04"
                                        || _strReceive == "SW1_10,NAK,04")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S1_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "SW1_1,NAK,05" || _strReceive == "SW1_2,NAK,05" || _strReceive == "SW1_3,NAK,05"
                                        || _strReceive == "SW1_4,NAK,05" || _strReceive == "SW1_5,NAK,05" || _strReceive == "SW1_6,NAK,05"
                                        || _strReceive == "SW1_7,NAK,05" || _strReceive == "SW1_8,NAK,05" || _strReceive == "SW1_9,NAK,05"
                                        || _strReceive == "SW1_10,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S1_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "S1_1,ACK" || _strReceive == "S1_2,ACK" || _strReceive == "S1_3,ACK"
                                        || _strReceive == "S1_4,ACK" || _strReceive == "S1_5,ACK" || _strReceive == "S1_6,ACK"
                                        || _strReceive == "S1_7,ACK" || _strReceive == "S1_8,ACK" || _strReceive == "S1_9,ACK"
                                        || _strReceive == "S1_10,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S1_VISION_GET_ADD_RC_DONE", true);
                                    }
                                    else if (_strReceive == "S1,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BOTTOM_VISION_GET_RC_DONE", true);
                                        //m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BOTTOM_VISION_GET_ADD_RC_DONE", true);
                                    }
                                    else if (_strReceive == "S1_1,ACK" || _strReceive == "S1_2,ACK" || _strReceive == "S1_3,ACK"
                                        || _strReceive == "S1_4,ACK" || _strReceive == "S1_5,ACK" || _strReceive == "S1_6,ACK"
                                        || _strReceive == "S1_7,ACK" || _strReceive == "S1_8,ACK" || _strReceive == "S1_9,ACK"
                                        || _strReceive == "S1_10,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BOTTOM_VISION_GET_ADD_RC_DONE", true);
                                    }
                                    else if (_strReceive == "S3,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_VISION_GET_RC_DONE", true);
                                        //m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_VISION_GET_ADD_RC_DONE", true);
                                    }
                                    else if (_strReceive == "S3_1,ACK" || _strReceive == "S3_2,ACK" || _strReceive == "S3_3,ACK"
                                        || _strReceive == "S3_4,ACK" || _strReceive == "S3_5,ACK" || _strReceive == "S3_6,ACK"
                                        || _strReceive == "S3_7,ACK" || _strReceive == "S3_8,ACK" || _strReceive == "S3_9,ACK"
                                        || _strReceive == "S3_10,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_VISION_GET_ADD_RC_DONE", true);
                                    }
                                    else if (_strReceive == "SW3_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_PARTING_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "SW3_PARTING,NAK,04" || _strReceive == "SW3_PARTING,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_PARTING_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "SW3_FACET_1,ACK" || _strReceive == "SW3_FACET_2,ACK" || _strReceive == "SW3_FACET_3,ACK"
                                        || _strReceive == "SW3_FACET_4,ACK" || _strReceive == "SW3_FACET_5,ACK" || _strReceive == "SW3_FACET_6,ACK"
                                        || _strReceive == "SW3_FACET_7,ACK" || _strReceive == "SW3_FACET_8,ACK" || _strReceive == "SW3_FACET_9,ACK"
                                        || _strReceive == "SW3_FACET_10,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_FACET_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "SW3_FACET_1,NAK,04" || _strReceive == "SW3_FACET_2,NAK,04" || _strReceive == "SW3_FACET_3,NAK,04"
                                       || _strReceive == "SW3_FACET_4,NAK,04" || _strReceive == "SW3_FACET_5,NAK,04" || _strReceive == "SW3_FACET_6,NAK,04"
                                       || _strReceive == "SW3_FACET_7,NAK,04" || _strReceive == "SW3_FACET_8,NAK,04" || _strReceive == "SW3_FACET_9,NAK,04"
                                       || _strReceive == "SW3_FACET_10,NAK,04")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "SW3_FACET_1,NAK,05" || _strReceive == "SW3_FACET_2,NAK,05" || _strReceive == "SW3_FACET_3,NAK,05"
                                       || _strReceive == "SW3_FACET_4,NAK,05" || _strReceive == "SW3_FACET_5,NAK,05" || _strReceive == "SW3_FACET_6,NAK,05"
                                       || _strReceive == "SW3_FACET_7,NAK,05" || _strReceive == "SW3_FACET_8,NAK,05" || _strReceive == "SW3_FACET_9,NAK,05"
                                       || _strReceive == "SW3_FACET_10,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_FACET_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "OUT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUT_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "OUT_PRE,NAK,04" || _strReceive == "OUT_PRE,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUT_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "OUT_POST,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUT_POST_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "OUT_POST,NAK,04" || _strReceive == "OUT_POST,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUT_POST_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "REJECT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_REJECT_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "REJECT_PRE,NAK,04" || _strReceive == "REJECT_PRE,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_REJECT_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "REJECT_POST,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_REJECT_POST_VISION_GET_RC_DONE", true);
                                    }
                                    else if (_strReceive == "REJECT_POST,NAK,04" || _strReceive == "REJECT_POST,NAK,05")
                                    {
                                        //To Fill Up
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_REJECT_POST_VISION_GET_RC_NAK", true);
                                    }
                                    else if (_strReceive == "NAK, 011")
                                    {
                                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                                        Machine.SequenceControl.SetAlarm(6003);
                                    }
                                    else if (_strReceive == "VerifyCrossHair,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_VISION_CROSS_HAIR_DONE", true);
                                        Machine.LogDisplay.AddLogDisplay("Process", "Vision Cross Hair Done");
                                    }
                                    else if (_strReceive.Contains("VERIFYCROSSHAIR,NAK"))
                                    {
                                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_VISION_CROSS_HAIR_FAIL", true);
                                        Machine.LogDisplay.AddLogDisplay("Process", "Vision Cross Hair Fail");
                                    }
                                    else if (_strReceive == "VerifyDotgrids,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_VISION_VERIFY_DOTGRID_DONE", true);
                                        //m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_VISION_DOT_GRIDS_DONE", true);
                                        Machine.LogDisplay.AddLogDisplay("Process", "Dot Grids Verification/Calibration Done");
                                    }
                                    else if (_strReceive.Contains("VerifyDotgrids,NAK"))
                                    {
                                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_VISION_VERIFY_DOTGRID_FAIL", true);
                                        //m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_VISION_DOT_GRIDS_FAIL", true);
                                        Machine.LogDisplay.AddLogDisplay("Process", "Dot Grids Verification/Calibration Fail");
                                    }
                                    else if (_strReceive == "VerifyGrayscale,ACK")
                                    {
                                        //m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_VISION_GRAY_SCALE_DONE", true);
                                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_VISION_VERIFY_GRAYSCALE_DONE", true);
                                        Machine.LogDisplay.AddLogDisplay("Process", "Gray Scale Verification/Calibration Done");
                                    }
                                    else if (_strReceive.Contains("VerifyGrayscale,NAK"))
                                    {
                                        m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_SEND_VISION_VERIFY_GRAYSCALE_FAIL", true);
                                        //m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_VISION_GRAY_SCALE_FAIL", true);
                                        Machine.LogDisplay.AddLogDisplay("Process", "Gray Scale Verification/Calibration Fail");
                                    }

                                    else if (_strReceive == "TileID,NAK,00" || _strReceive.ToUpper() == "TILEID,NAK,00")
                                    {
                                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                                        Machine.SequenceControl.SetAlarm(6003);
                                    }
                                    else if (_strReceive == "TileID,NAK,01" || _strReceive.ToUpper() == "TILEID,NAK,01")
                                    {
                                        //RTSSProcess.SetShareMemoryEvent("JobStop", true);
                                        //SequenceControl.SetAlarm(6507);
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_LotID_BarcodeID.Set();
                                    }
                                    else if (_strReceive == "NewLot,ACK")
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot.Set();
                                    }
                                    else if (_strReceive == "NewLot,NAK,01")
                                    {
                                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot.Reset();
                                        //ProcessEvent.PCS_PCS_Send_Vision_LotID_BarcodeID.Set();
                                    }
                                    else if (_strReceive.Contains("NewLot,NAK"))
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot.Reset();
                                        if (nNoOfRetrySendVisionNewLot >= 3)
                                        {
                                            m_ProductRTSSProcess.SetEvent("JobStop", true);
                                            Machine.LogDisplay.AddLogDisplay("Caution", "Vision Fail To Receive New Lot");
                                        }
                                        else
                                        {
                                            m_ProductProcessEvent.PCS_PCS_Resend_Vision_NewLot.Set();
                                            Machine.LogDisplay.AddLogDisplay("Caution", "Resend Vision New Lot");
                                        }
                                    }
                                    else if (_strReceive.Contains("NewLot,INP,NAK,01")
                                            || _strReceive.Contains("NewLot,SW2_PARTING,NAK,01")
                                            || _strReceive.Contains("NewLot,SW1_1,NAK,01")
                                            || _strReceive.Contains("NewLot,SW3_PARTING,NAK,01")
                                            || _strReceive.Contains("NewLot,BTM,NAK,01")
                                            || _strReceive.Contains("NewLot,OUT_PRE,NAK,01")
                                            || _strReceive.Contains("NewLot,REJECT_PRE,NAK,01")
                                            )
                                    {
                                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_Info_Fail.Set();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_INP.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_OUT.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_BTM.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW1.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW2.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW3.Reset();
                                    }
                                    else if (_strReceive.Contains("NewLot,INP,NAK")
                                        || _strReceive.Contains("NewLot,SW2_PARTING,NAK")
                                        || _strReceive.Contains("NewLot,SW1_1,NAK")
                                        || _strReceive.Contains("NewLot,SW3_PARTING,NAK")
                                        || _strReceive.Contains("NewLot,OUT_PRE,NAK")
                                        || _strReceive.Contains("NewLot,BTM,NAK")
                                        || _strReceive.Contains("NewLot,REJECT_PRE,NAK")
                                        )
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_INP.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_OUT.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW1.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW2.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW3.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_BTM.Reset();
                                        if (nNoOfRetrySendVisionNewLot >= 3)
                                        {
                                            m_ProductRTSSProcess.SetEvent("JobStop", true);
                                            Machine.LogDisplay.AddLogDisplay("Caution", "Vision Fail To Receive New Lot");
                                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_Info_Fail.Set();
                                        }
                                        else
                                        {
                                            m_ProductProcessEvent.PCS_PCS_Resend_Vision_NewLot.Set();
                                            Machine.LogDisplay.AddLogDisplay("Caution", "Resend Vision New Lot");
                                        }
                                    }
                                    else if (_strReceive == "Recipe,ACK" || _strReceive == "ProductName,ACK")
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_Setting.Set();
                                        //if (m_ProductShareVariables.IsSendVisionInfo == true)
                                        //{
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_NewLot.Set();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_Info_Fail.Reset();
                                        //}
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_OtherSettings.Set();
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_LOADING_CALIBRATION_RECIPE_DONE", true);
                                        // m_ProductProcessEvent.PCS_PCS_Send_Vision_CanisterDefectCode.Set();
                                        // m_ProductProcessEvent.PCS_PCS_Send_Vision_PurgeDefectCode.Set();
                                        // m_ProductProcessEvent.PCS_PCS_Send_Vision_InputDefectCode.Set();

                                        if (m_ProductRTSSProcess.GetEvent("GMNL_RMNL_MANUAL_MODE"))
                                        {
                                            m_ProductProcessEvent.PCS_PCS_Send_Vision_Manual_Mode_On.Set();
                                        }
                                        else
                                        {
                                            m_ProductProcessEvent.PCS_PCS_Send_Vision_Manual_Mode_Off.Set();
                                        }
                                        //m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_RESORT_MODE_OFF_DONE", false);
                                        m_ProductProcessEvent.PCS_PCS_Send_Vision_Resort_Mode_Off.Set();
                                    }
                                    else if (_strReceive == "ProductName;crosshaircalib,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RMAIN_LOADING_CALIBRATION_RECIPE_DONE", true);
                                    }
                                    else if (_strReceive == "Recipe,NAK,01" || _strReceive == "ProductName,NAK,01")
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_Setting.Reset();
                                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                                        Machine.LogDisplay.AddLogDisplay("Caution", "Inspection recipe not found");
                                    }
                                    else if (_strReceive == "Recipe,NAK,02" || _strReceive == "ProductName,NAK,02")
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_Setting.Reset();
                                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                                        Machine.LogDisplay.AddLogDisplay("Caution", "Vision software fail to connect to vision devices during load recipe");
                                    }
                                    else if (_strReceive == "Recipe,NAK,03" || _strReceive == "ProductName,NAK,03")
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_Setting.Reset();
                                        m_ProductRTSSProcess.SetEvent("JobStop", true);
                                        Machine.LogDisplay.AddLogDisplay("Caution", "Vision item configuration window is not close.");
                                    }
                                    else if (_strReceive == "INPTRAY,ACK")
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_INPTRAY.Set();
                                    }
                                    else if (_strReceive == "OUTTRAY,ACK")
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_OUTTRAY.Set();
                                    }
                                    else if (_strReceive == "SORTTRAY,ACK")
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_SORTTRAY.Set();
                                    }
                                    else if (_strReceive == "REJECTTRAY,ACK")
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_REJECTTRAY.Set();
                                    }
                                    else if (_strReceive.Contains("Recipe,NAK")
                                        || _strReceive.Contains("ProductName,NAK")
                                        // || _strReceive.Contains("MOVEDIR,NAK")
                                        //|| _strReceive.Contains("FOVUNITINFO,NAK")
                                        || _strReceive.Contains("CanisterDefectCode,NAK")
                                        || _strReceive.Contains("PurgeDefectCode,NAK")
                                        //|| _strReceive.Contains("RCDIRECTION,NAK")
                                        )
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_Setting.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_InputDefectCode.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_CanisterDefectCode.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_PurgeDefectCode.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_AllStationDefectCode.Reset();
                                        m_ProductProcessEvent.PCS_PCS_Vision_AllStationDefectCode_Error.Reset();


                                        if (nNoOfRetrySendVisionRecipe >= 3)
                                        {
                                            m_ProductRTSSProcess.SetEvent("JobStop", true);
                                            Machine.LogDisplay.AddLogDisplay("Caution", "Vision Fail To Load Setting");
                                            m_ProductProcessEvent.PCS_PCS_Vision_Receive_Info_Fail.Set();
                                        }
                                        else
                                        {
                                            m_ProductProcessEvent.PCS_PCS_Resend_Vision_Setting.Set();
                                            Machine.LogDisplay.AddLogDisplay("Caution", "Resend Vision Setting");
                                        }
                                    }
                                    else if (_strReceive.Contains("DiskStorageExceedsLimit"))
                                    {
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_AvailableDiskSpaceLow.Set();
                                        Machine.EventLogger.WriteLog("Vision Disk Storage Exceeds Limit");
                                        //ProductLogger.WriteLog(m_ProductShareVariables.LotID, "Vision Disk Storage Exceeds Limit");
                                    }
                                    else if (_strReceive == "GET_THETA,ACK" || _strReceive == "ALGCONFIG_GET_THETA,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_GET_VISION_THETA_READY", true);
                                    }
                                    else if (_strReceive == "GET_XY,ACK" || _strReceive == "ALGCONFIG_GET_XY,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_GET_VISION_XY_READY", true);
                                    }
                                    else if (_strReceive == "AllocateUnit,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_GET_VISION_ALLOCATE_UNIT_READY", true);
                                    }
                                    else if (_strReceive == "ALGCONFIG_ALLOCATEUNIT,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_GET_VISION_ALGCONFIG_ALLOCATE_UNIT_READY", true);
                                    }
                                    else if (_strReceive == "StationRequireResult,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RMNL_SET_MANUAL_MODE_VISION_DONE", true);
                                    }
                                    else if (_strReceive == "CONFIG_ALIGNMENT,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_VISION_ALIGNMENT_ON_DONE", true);
                                    }
                                    else if (_strReceive == "CONFIG_ALIGNMENT_OFF,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_VISION_ALIGNMENT_OFF_DONE", true);
                                    }
                                    else if (_strReceive == "RemoveRC,ACK")
                                    {
                                        SetShareMemoryEvent("GMAIN_RSEQ_UPDATE_VISION_DONE", true);
                                    }
                                    else if (_strReceive == "OutputCheckEmpty,ACK")
                                    {
                                        SetShareMemoryEvent("RMAIN_RTHD_OUT_VISION_CHECK_EMPTY_READY", true);
                                    }
                                    else if (_strReceive == "OutputCheckEmpty,NAK")
                                    {
                                        //SetShareMemoryEvent("RMAIN_RTHD_OUT_VISION_CHECK_EMPTY_READY", true);
                                    }
                                    else if (_strReceive == "GetUnitXY,ACK")
                                    {
                                        SetShareMemoryEvent("RMAIN_RTHD_OUT_VISION_GET_NEARBY_UNIT_READY", true);
                                    }
                                    else if (_strReceive == "GetUnitXY,NAK")
                                    {
                                        //SetShareMemoryEvent("RMAIN_RTHD_OUT_VISION_GET_NEARBY_UNIT_READY", true);
                                    }
                                    else if (_strReceive == "RenameImage,NAK")
                                    {
                                        Machine.SequenceControl.SetAlarm(6005);
                                    }
                                    else if (_strReceive == "RESORTUNIT_OUTOLD,ACK")
                                    {
                                        SetShareMemoryEvent("GMAIN_RTHD_SEND_RESORT_MODE_OLD_LIST_DONE", true);
                                    }
                                    else if (_strReceive == "RESORTUNIT_OUTNEW,ACK")
                                    {
                                        SetShareMemoryEvent("GMAIN_RTHD_SEND_RESORT_MODE_NEW_LIST_DONE", true);
                                    }
                                    else if (_strReceive == "RESORTUNIT_S1,ACK")
                                    {
                                        SetShareMemoryEvent("GMAIN_RTHD_SEND_RESORT_MODE_S1_LIST_DONE", true);
                                    }
                                    continue;
                                }
                                string strReceiveCommand = _strReceive.Substring(0, nIndex);
                                string strValue = _strReceive.Substring(nIndex + 1);
                                if (strValue == "")
                                {
                                    //Ignore because no value
                                    continue;
                                }
                                //switch(strReceiveCommand)
                                //{
                                //    case "XYResult":
                                //        break;
                                //}
                                if (strReceiveCommand == "NEWLOT")
                                {
                                    if (_strReceive == "NEWLOT=INP,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INPUT_SEND_NEWLOT_DONE", true);
                                        //if(m_ProductShareVariables.IsSendVisionInfo == true)
                                        //{
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_INP.Set();
                                        if (m_ProductRTSSProcess.GetProductionInt("nInputRunningState") == 1)
                                        {
                                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_INPUT_SEND_TRAYNO", true);
                                        }
                                        //}
                                    }
                                    else if (_strReceive == "NEWLOT=SW2_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_SEND_NEWLOT_DONE", true);
                                        //if (m_ProductShareVariables.IsSendVisionInfo == true)
                                        //{
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW2.Set();
                                        if (m_ProductRTSSProcess.GetProductionInt("nInputRunningState") == 1)
                                        {
                                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S2_SEND_TRAYNO", true);
                                        }
                                        //}
                                    }
                                    else if (_strReceive == "NEWLOT=SW1_1,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S1_SEND_NEWLOT_DONE", true);
                                        //if (m_ProductShareVariables.IsSendVisionInfo == true)
                                        //{
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW1.Set();
                                        if (m_ProductRTSSProcess.GetProductionInt("nPNPRunningState") == 1)
                                        {
                                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S1_SEND_TRAYNO", true);
                                        }
                                        //}
                                    }
                                    else if (_strReceive == "NEWLOT=SW3_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_SEND_NEWLOT_DONE", true);
                                        //if (m_ProductShareVariables.IsSendVisionInfo == true)
                                        //{
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW3.Set();
                                        if (m_ProductRTSSProcess.GetProductionInt("nPNPRunningState") == 1)
                                        {
                                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_S3_SEND_TRAYNO", true);
                                        }
                                        //}
                                    }
                                    else if (_strReceive == "NEWLOT=BTM,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BOTTOM_SEND_NEWLOT_DONE", true);
                                        //if (m_ProductShareVariables.IsSendVisionInfo == true)
                                        //{
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_SW3.Set();
                                        if (m_ProductRTSSProcess.GetProductionInt("nPNPRunningState") == 1)
                                        {
                                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_BTM_SEND_TRAYNO", true);
                                        }
                                        //}
                                    }
                                    else if (_strReceive == "NEWLOT=OUT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUTPUT_SEND_NEWLOT_DONE", true);
                                        //if (m_ProductShareVariables.IsSendVisionInfo == true)
                                        //{
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_OUT.Set();
                                        if (m_ProductRTSSProcess.GetProductionInt("nOutputRunningState") == 1)
                                        {
                                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_OUTPUT_SEND_TRAYNO", true);
                                        }
                                        //}
                                    }
                                    else if (_strReceive == "NEWLOT=REJECT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_REJECT_SEND_NEWLOT_DONE", true);
                                        m_ProductProcessEvent.PCS_PCS_Vision_Receive_NewLot_REJECT.Set();
                                        //if (m_ProductShareVariables.IsSendVisionInfo == true)
                                        //{
                                        if (m_ProductRTSSProcess.GetProductionInt("nOutputRunningState") == 1)
                                        {
                                            m_ProductRTSSProcess.SetEvent("RTHD_GMAIN_REJECT_SEND_TRAYNO", true);
                                        }
                                        //}
                                    }
                                }
                                if (strReceiveCommand == "SETSNAPPOS")
                                {
                                    if (_strReceive == "SETSNAPPOS=SW1,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_SW1_SEND_SNAP_POS_DONE", true);
                                    }
                                    else if(_strReceive == "SETSNAPPOS=SW2_FACET,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_SW2_FACET_SEND_SNAP_POS_DONE", true);
                                    }
                                    else if (_strReceive == "SETSNAPPOS=SW3_FACET,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_SW3_FACET_SEND_SNAP_POS_DONE", true);
                                    }
                                }
                                if (_strReceive.Contains("ENDLOT"))
                                {
                                    if (_strReceive == "ENDLOT=INP,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INPUT_SEND_ENDLOT_DONE", true);
                                    }
                                    else if (_strReceive == "ENDLOT=BTM,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BTM_SEND_ENDLOT_DONE", true);
                                    }
                                    else if (_strReceive == "ENDLOT=SW2_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_SEND_ENDLOT_DONE", true);
                                    }
                                    else if (_strReceive == "ENDLOT=SW1_1,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S1_SEND_ENDLOT_DONE", true);
                                    }
                                    else if (_strReceive == "ENDLOT=SW3_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_SEND_ENDLOT_DONE", true);
                                    }
                                    else if (_strReceive == "ENDLOT=OUT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUTPUT_SEND_ENDLOT_DONE", true);
                                    }
                                    else if (_strReceive == "ENDLOT=REJECT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_REJECT_SEND_ENDLOT_DONE", true);
                                    }
                                }
                                if (_strReceive.Contains("TrayNo"))
                                {
                                    if (_strReceive == "TrayNo=INP,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INPUT_SEND_TRAYNO_DONE", true);
                                    }
                                    else if (_strReceive == "TrayNo=BTM,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BTM_SEND_TRAYNO_DONE", true);
                                    }
                                    else if (_strReceive == "TrayNo=SW2_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_SEND_TRAYNO_DONE", true);
                                    }
                                    else if (_strReceive == "TrayNo=SW1_1,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S1_SEND_TRAYNO_DONE", true);
                                    }
                                    else if (_strReceive == "TrayNo=SW3_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_SEND_TRAYNO_DONE", true);
                                    }
                                    else if (_strReceive == "TrayNo=OUT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUTPUT_SEND_TRAYNO_DONE", true);
                                    }
                                    else if (_strReceive == "TrayNo=REJECT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_REJECT_SEND_TRAYNO_DONE", true);
                                    }
                                }
                                if (_strReceive.Contains("EndTray"))
                                {
                                    if (_strReceive == "EndTray=INP,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_INPUT_SEND_ENDTRAY_DONE", true);
                                    }
                                    else if (_strReceive == "EndTray=BTM,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_BTM_SEND_ENDTRAY_DONE", true);
                                    }
                                    else if (_strReceive == "EndTray=SW2_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S2_SEND_ENDTRAY_DONE", true);
                                    }
                                    else if (_strReceive == "EndTray=SW1_1,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S1_SEND_ENDTRAY_DONE", true);
                                    }
                                    else if (_strReceive == "EndTray=SW3_PARTING,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_S3_SEND_ENDTRAY_DONE", true);
                                    }
                                    else if (_strReceive == "EndTray=OUT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUTPUT_SEND_ENDTRAY_DONE", true);
                                    }
                                    else if (_strReceive == "EndTray=OUT1,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_OUTPUT_POST_ALIGNMENT_SEND_ENDTRAY_DONE", true);
                                    }
                                    else if (_strReceive == "EndTray=REJECT_PRE,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_REJECT_SEND_ENDTRAY_DONE", true);
                                    }
                                    else if (_strReceive == "EndTray=SAMPLING1,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_SAMPLING1_SEND_ENDTRAY_DONE", true);
                                    }
                                }
                                if (_strReceive.Contains("MOVEDIR"))
                                {

                                }
                                if (_strReceive.Contains("RCDIR"))
                                {

                                }
                                if (strReceiveCommand == "SETUPLC")
                                {
                                    if (_strReceive == "SETUPLC=On,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_SETUP_VISION_GET_LATENCY_ON_DONE", true);
                                    }
                                    else if (_strReceive == "SETUPLC=Off,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_SETUP_VISION_GET_LATENCY_OFF_DONE", true);
                                    }
                                }
                                if (strReceiveCommand == "RESORTMODE")
                                {
                                    if (_strReceive == "RESORTMODE=On,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_RESORT_MODE_ON_DONE", true);
                                    }
                                    else if (_strReceive == "RESORTMODE=Off,ACK")
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_RESORT_MODE_OFF_DONE", true);
                                    }
                                }
                                if (strReceiveCommand == "INP"
                                    || strReceiveCommand == "INP_1" || strReceiveCommand == "INP_2" || strReceiveCommand == "INP_3"
                                    || strReceiveCommand == "INP_4" || strReceiveCommand == "INP_5" || strReceiveCommand == "INP_6"
                                    || strReceiveCommand == "INP_7" || strReceiveCommand == "INP_8" || strReceiveCommand == "INP_9"
                                    || strReceiveCommand == "INP_10")//R,C,Result,X,Y,T
                                {
                                    bool IsAdditional = false;
                                    bool IsLastSnap = false;
                                    bool bAlarmStatus = false;
                                    int AdditionalNo = 0;
                                    if (strReceiveCommand == "INP")
                                    {
                                        IsAdditional = false;
                                        IsLastSnap = false;
                                    }
                                    else
                                    {
                                        IsAdditional = true;
                                        string[] temp = strReceiveCommand.Split('_');
                                        if (int.TryParse(temp[1], out AdditionalNo) == false)
                                        {
                                            Machine.SequenceControl.SetAlarm(6112);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Input Vision Data Not In Correct Format");
                                        }
                                        else
                                        {
                                            if (AdditionalNo == m_ProductShareVariables.LastInputSnapNo)
                                            {
                                                IsLastSnap = true;
                                            }
                                            else
                                            {
                                                IsLastSnap = false;
                                            }
                                        }

                                    }
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "" && i != (strArrayResult.Length - 1))
                                        {
                                            Machine.SequenceControl.SetAlarm(6112);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Input Vision Data Not In Correct Format");
                                            //MessageBox.Show("Vision result not correct");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    bool dataNotComplete = false;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "")
                                        {
                                            continue;
                                        }
                                        string[] strArrayData = strResult.Split(',');
                                        //if (strArrayData.Length != 6)
                                        ////if (strArrayData.Length != 9)
                                        //{
                                        //    dataNotComplete = true;
                                        //    Machine.SequenceControl.SetAlarm(6112);
                                        //    Machine.LogDisplay.AddLogDisplay("Error", "Input Vision Data Not Complete");
                                        //}
                                        //i++;
                                    }
                                    i = 0;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            //int nBinCode = 0;
                                            double dX_um = 0;
                                            double dY_um = 0;
                                            double dTheta_mDegree = 0;
                                            string Barcode2DID = "";
                                            double dXSleeve_um = 0;
                                            double dYSleeve_um = 0;
                                            double dTheta_mDegreeSleeve_um = 0;

                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true
                                                && Double.TryParse(strArrayData[3], out dX_um) && Double.TryParse(strArrayData[4], out dY_um) && Double.TryParse(strArrayData[5], out dTheta_mDegree)
                                                && Double.TryParse(strArrayData[7], out dXSleeve_um) && Double.TryParse(strArrayData[8], out dYSleeve_um) && Double.TryParse(strArrayData[9], out dTheta_mDegreeSleeve_um)
                                                )
                                            {
                                                if (Double.IsNaN(dX_um) || Double.IsNaN(dY_um) || Double.IsNaN(dTheta_mDegree)
                                                    || Double.IsNaN(dXSleeve_um) || Double.IsNaN(dYSleeve_um) || Double.IsNaN(dTheta_mDegreeSleeve_um)
                                                   )
                                                {
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                    goto Exit;
                                                }

                                                bool bIsAvailableUnitOnInputFrame = false;
                                                bool bIsInputDefectCode = false;
                                                Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Come in INP1");

                                                {

                                                }

                                                if (strArrayData[2] == "P")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString()
                                                        )
                                                    {
                                                        if (IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "InputResult", 1);
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "UnitPresent", 1);
                                                            Barcode2DID = strArrayData[6];
                                                            m_ProductShareVariables.Barcode2DID[0] = Barcode2DID;
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("InputVision", AdditionalNo - 1, "Result", 1);
                                                        }
                                                    }
                                                }
                                                else if (strArrayData[2] == "EP")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString()
                                                        )
                                                    {
                                                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false);
                                                        m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                                                        //int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                        //m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputTrayType = "INPUT";
                                                        if (IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "InputResult", 3);
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "UnitPresent", 0);
                                                            Barcode2DID = strArrayData[6];
                                                            m_ProductShareVariables.Barcode2DID[0] = Barcode2DID;
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("InputVision", AdditionalNo - 1, "Result", 3);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                if (_defectProperty.Pick == "Yes")
                                                                {
                                                                    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false);
                                                                    if (IsAdditional == false)
                                                                    {
                                                                        m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "InputResult", 1);
                                                                        m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "UnitPresent", 1);
                                                                        Barcode2DID = strArrayData[6];
                                                                        m_ProductShareVariables.Barcode2DID[0] = Barcode2DID;
                                                                    }
                                                                    else
                                                                    {
                                                                        m_ProductRTSSProcess.SetSettingArray("InputVision", AdditionalNo - 1, "Result", 1);
                                                                        m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "UnitPresent", 1);
                                                                    }
                                                                }
                                                                else if (_defectProperty.Pick == "No")
                                                                {
                                                                    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false);
                                                                    m_ProductProcessEvent.PCS_GUI_Update_Map.Set();
                                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputTrayType = "INPUT";
                                                                    if (IsAdditional == false)
                                                                    {
                                                                        m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "InputResult", 5);
                                                                        m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "UnitPresent", 0);
                                                                        Barcode2DID = strArrayData[6];
                                                                        m_ProductShareVariables.Barcode2DID[0] = Barcode2DID;
                                                                    }
                                                                    else
                                                                    {
                                                                        m_ProductRTSSProcess.SetSettingArray("InputVision", AdditionalNo - 1, "Result", 3);
                                                                        m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "UnitPresent", 0);
                                                                    }
                                                                }
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            if (IsAdditional == false)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "InputResult", 3);
                                                                m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "UnitPresent", 0);
                                                                Barcode2DID = strArrayData[6];
                                                                m_ProductShareVariables.Barcode2DID[0] = Barcode2DID;
                                                            }
                                                            else
                                                            {
                                                                m_ProductRTSSProcess.SetSettingArray("InputVision", AdditionalNo - 1, "Result", 3);
                                                                m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "UnitPresent", 0);
                                                            }
                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6140);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "Input Vision defect code error");
                                                            //goto InputVisionExit;
                                                        }
                                                    }

                                                }

                                                if (m_ProductRTSSProcess.GetEvent("GGUI_RSEQ_DRY_RUN_MODE") == true)
                                                {
                                                    m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "InputResult", 1);
                                                    m_ProductRTSSProcess.SetProductionArray("InputTableResult", 0, "UnitPresent", 1);
                                                }

                                                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputColumn"))))
                                                {

                                                    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1],
                                                           nRow, nCol, strArrayData[2], false);

                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].InputTrayNo = m_ProductRTSSProcess.GetProductionInt("nCurrentInputTrayNo");

                                                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].InputResult == null
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].InputResult == "P"
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].InputResult == "UTT")
                                                    {
                                                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].InputResult = strArrayData[2];
                                                    }

                                                    if (IsAdditional == false)
                                                    {
                                                        if (Barcode2DID != "" && Barcode2DID != "NA" && Barcode2DID != "N/A")
                                                        {
                                                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID = Barcode2DID;
                                                        }
                                                        else
                                                        {
                                                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].UnitID = "NA";
                                                        }
                                                    }                                                    
                                                }
                                                else
                                                {
                                                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                }
                                                {
                                                    Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} Length = {m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo.Length}");
                                                    Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} InputTrayNo = {m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo")}");
                                                    Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} nRow = {nRow}");
                                                    Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} nCol = {nCol}");
                                                }

                                                bIsInputDefectCode = false;
                                                int nX_um = Convert.ToInt32(dX_um);
                                                int nY_um = Convert.ToInt32(dY_um);
                                                int nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                                int nXSleeve_um = Convert.ToInt32(dXSleeve_um);
                                                int nYSleeve_um = Convert.ToInt32(dYSleeve_um);
                                                int nTheta_mDegreeSleeve_um = Convert.ToInt32(dTheta_mDegreeSleeve_um);
                                                for (int j = 0; j < 1; j++)
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("InputTableResult", j, "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("InputTableResult", j, "InputColumn").ToString()
                                                        )
                                                    {
                                                        if (IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", j, "InputXOffset_um", nX_um);
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", j, "InputYOffset_um", nY_um);
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", j, "InputThetaOffset_mDegree", nTheta_MDegree);
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", j, "InputSleeveXOffset_um", nXSleeve_um);
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", j, "InputSleeveYOffset_um", nYSleeve_um);
                                                            m_ProductRTSSProcess.SetProductionArray("InputTableResult", j, "InputSleeveThetaOffset_mDegree", nTheta_mDegreeSleeve_um);
                                                        }
                                                    }
                                                }
                                                nInp_Receive_Data++;
                                                //if (nInp_Receive_Data == GetShareMemoryProductionInt("nNoOfInputVisionInspectionUnit"))
                                                {
                                                    SetShareMemoryEvent("GMAIN_RTHD_GET_INP_VISION_XYTR_DONE", true);
                                                }
                                                if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                {
                                                    if (IsLastSnap == true)
                                                    {
                                                        foreach (string _defectcode in m_ProductShareVariables.CurrentInputDefectCode)
                                                        {
                                                            bool IsDefectCodeExists = false;
                                                            foreach (var _DefectCounter in m_ProductShareVariables.DefectCounterInput)
                                                            {
                                                                if (_defectcode == _DefectCounter.DefectCode)
                                                                {
                                                                    _DefectCounter.Counter++;
                                                                    IsDefectCodeExists = true;
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    IsDefectCodeExists = false;
                                                                }
                                                            }
                                                            if (IsDefectCodeExists == false)
                                                            {
                                                                m_ProductShareVariables.DefectCounterInput.Add(new ProductShareVariables.DefectCounter { DefectCode = _defectcode, Counter = 1 });
                                                            }
                                                        }
                                                        m_ProductShareVariables.CurrentInputDefectCode.Clear();
                                                        bAlarmStatus = false;
                                                        foreach (var _DefectCounter in m_ProductShareVariables.DefectCounterInput)
                                                        {
                                                            if (_DefectCounter.Counter >= m_ProductShareVariables.productRecipeOutputSettings.CheckContinuousDefectCode)
                                                            {
                                                                bAlarmStatus = true;
                                                                _DefectCounter.Counter = 0;
                                                                Machine.LogDisplay.AddLogDisplay("Error", _DefectCounter.DefectCode + " Occur Constinuously.");
                                                            }
                                                        }
                                                        if (bAlarmStatus == true)
                                                        {
                                                            Machine.SequenceControl.SetAlarm(6137);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6112);
                                                Machine.LogDisplay.AddLogDisplay("Error", "Input Vision defect code error");
                                                //MessageBox.Show("Vision error result");
                                            }
                                        InputVisionExit:
                                            { }
                                        }
                                    }
                                    //XYResult=65,1,MD;xoffset,yoffset,thetaoffset;\r\n
                                    //SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XYTR_DONE", true);
                                }
                                else if (strReceiveCommand == "SW2_PARTING")//R,C,Result,X,Y,T
                                {
                                    bool IsAdditional = false;
                                    bool IsLastSnap = false;
                                    bool bAlarmStatus = false;
                                    int AdditionalNo = 0;
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    i = 0;
                                    bool dataNotComplete = false;
                                    if (strReceiveCommand == "SW2_PARTING")
                                    {
                                        IsAdditional = false;
                                        IsLastSnap = false;
                                    }
                                    else
                                    {
                                        IsAdditional = true;
                                        string[] temp = strReceiveCommand.Split('_');
                                        if (int.TryParse(temp[2], out AdditionalNo) == false)
                                        {
                                            Machine.SequenceControl.SetAlarm(6112);
                                            Machine.LogDisplay.AddLogDisplay("Error", "SW2 Vision Data Not In Correct Format");
                                        }
                                        else
                                        {
                                            if (AdditionalNo == m_ProductShareVariables.LastInputSnapNo)
                                            {
                                                IsLastSnap = true;
                                            }
                                            else
                                            {
                                                IsLastSnap = false;
                                            }
                                        }
                                    }
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true)
                                            {
                                                if (strArrayData[2] == "P")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        if (IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S2PartingResult", 1);
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("S2Vision", AdditionalNo - 1, "Result", 1);
                                                        }
                                                    }
                                                }
                                                else if (strArrayData[2] == "MU" || strArrayData[2] == "E")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        if (IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S2PartingResult", 3);
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("S2Vision", AdditionalNo - 1, "Result", 3);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                if (IsAdditional == false)
                                                                {
                                                                    m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S2PartingResult", 3);
                                                                }
                                                                else
                                                                {
                                                                    m_ProductRTSSProcess.SetSettingArray("S2Vision", AdditionalNo - 1, "Result", 3);
                                                                }
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            if (IsAdditional == false)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S2PartingResult", 3);
                                                            }
                                                            else
                                                            {
                                                                m_ProductRTSSProcess.SetSettingArray("S2Vision", AdditionalNo - 1, "Result", 3);
                                                            }
                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6238);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "S2 Vision defect code error");
                                                            //goto SW2_PARTINGVisionExit;
                                                        }
                                                    }
                                                }
                                                int TrayIndex = m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1;
                                                if (m_ProductShareVariables.MultipleMappingInfo[TrayIndex].dicUnitIndex.ContainsKey(string.Format("{0},{1}", nRow, nCol)))
                                                {
                                                    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[TrayIndex], nRow, nCol, strArrayData[2], false);
                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[TrayIndex].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                    if (m_ProductShareVariables.MultipleMappingInfo[TrayIndex].arrayUnitInfo[curindex].S2PartingResult == null
                                                        || m_ProductShareVariables.MultipleMappingInfo[TrayIndex].arrayUnitInfo[curindex].S2PartingResult == "P"
                                                        || m_ProductShareVariables.MultipleMappingInfo[TrayIndex].arrayUnitInfo[curindex].S2PartingResult == ""
                                                        || m_ProductShareVariables.MultipleMappingInfo[TrayIndex].arrayUnitInfo[curindex].S2PartingResult == "UTT")
                                                    {
                                                        m_ProductShareVariables.MultipleMappingInfo[TrayIndex].arrayUnitInfo[curindex].S2PartingResult = strArrayData[2];
                                                    }
                                                    else
                                                    {
                                                        Machine.DebugLogger.WriteLog(string.Format("{0} : [caobi] Row = {1}, Col = {2}. S2PartingResult = {3}", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol, m_ProductShareVariables.MultipleMappingInfo[TrayIndex].arrayUnitInfo[curindex].S2PartingResult));
                                                    }
                                                }
                                                else
                                                {
                                                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                }
                                                SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_RESULT_DONE", true);
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6309);
                                                Machine.LogDisplay.AddLogDisplay("Error", "Bottom Vision defect code error");
                                            }
                                            SW2_PARTINGVisionExit:
                                            { }
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "SW2_FACET"
                                    || strReceiveCommand == "SW2_FACET_1" || strReceiveCommand == "SW2_FACET_2" || strReceiveCommand == "SW2_FACET_3"
                                    || strReceiveCommand == "SW2_FACET_4" || strReceiveCommand == "SW2_FACET_5" || strReceiveCommand == "SW2_FACET_6"
                                    || strReceiveCommand == "SW2_FACET_7" || strReceiveCommand == "SW2_FACET_8" || strReceiveCommand == "SW2_FACET_9"
                                    || strReceiveCommand == "SW2_FACET_10")//R,C,Result,X,Y,T
                                {
                                    bool IsAdditional = false;
                                    bool IsLastSnap = false;
                                    bool bAlarmStatus = false;
                                    int AdditionalNo = 0;
                                    if (strReceiveCommand == "SW2_FACET")
                                    {
                                        IsAdditional = false;
                                        IsLastSnap = false;
                                    }
                                    else
                                    {
                                        IsAdditional = true;
                                        string[] temp = strReceiveCommand.Split('_');
                                        if (int.TryParse(temp[2], out AdditionalNo) == false)
                                        {
                                            Machine.SequenceControl.SetAlarm(6209);
                                            Machine.LogDisplay.AddLogDisplay("Error", "S2 Vision Data Not In Correct Format");
                                        }
                                        else
                                        {
                                            m_ProductRTSSProcess.SetProductionInt("CurrentS2FacetSnapTimes", m_ProductRTSSProcess.GetProductionInt("CurrentS2FacetSnapTimes") + 1);
                                        }
                                    }
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    i = 0;
                                    bool dataNotComplete = false;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true)
                                            {
                                                if (strArrayData[2] == "P")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        if(IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S2Result", 1);
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("S2FacetVision", AdditionalNo - 1, "Result", 1);
                                                        }
                                                    }
                                                }
                                                else if (strArrayData[2] == "MU" || strArrayData[2] == "E")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        if (IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S2Result", 3);
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("S2FacetVision", AdditionalNo - 1, "Result", 3);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                if (IsAdditional == false)
                                                                {
                                                                    m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S2Result", 3);
                                                                }
                                                                else
                                                                {
                                                                    m_ProductRTSSProcess.SetSettingArray("S2FacetVision", AdditionalNo - 1, "Result", 3);
                                                                }
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            if (IsAdditional == false)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S2Result", 3);
                                                            }
                                                            else
                                                            {
                                                                m_ProductRTSSProcess.SetSettingArray("S2FacetVision", AdditionalNo - 1, "Result", 3);
                                                            }
                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6238);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "S2 Vision defect code error");
                                                            //goto SW2VisionExit;
                                                        }
                                                    }
                                                }
                                                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", nRow, nCol)))
                                                {
                                                    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false);
                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                    
                                                    //if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S2Result == null
                                                    //    || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S2Result == "P"
                                                    //    || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S2Result == ""
                                                    //    || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S2Result == "UTT")
                                                    //{
                                                    //    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S2Result = strArrayData[2];
                                                    //}

                                                    m_ProductShareVariables.S2FacetVisionResult[AdditionalNo - 1] = strArrayData[2];

                                                    for (int z = 0; z < 10; z++)
                                                    {
                                                        if (m_ProductRTSSProcess.GetSettingArray("S2FacetVision", z, "Enable") == 1)
                                                        {
                                                            if (m_ProductShareVariables.S2FacetVisionResult[z] != "P")
                                                            {
                                                                Machine.EventLogger.WriteLog(string.Format("{0} : S2_{1} Result=>{2}", DateTime.Now.ToString("yyyyMMdd HHmmss"), z.ToString(), m_ProductShareVariables.S2FacetVisionResult[z]));
                                                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S2Result = m_ProductShareVariables.S2FacetVisionResult[z];
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                Machine.EventLogger.WriteLog(string.Format("{0} : S2_{1} Result=>{2}", DateTime.Now.ToString("yyyyMMdd HHmmss"), z.ToString(), "P"));
                                                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S2Result = "P";
                                                            }
                                                        }
                                                        
                                                    }
                                                }
                                                else
                                                {
                                                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                }
                                                SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_RESULT_DONE", true);

                                                if (AdditionalNo == 1)
                                                {

                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_1_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 2)
                                                {
                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_2_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 3)
                                                {
                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_3_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 4)
                                                {
                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_4_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 5)
                                                {
                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_5_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 6)
                                                {
                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_6_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 7)
                                                {
                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_7_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 8)
                                                {
                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_8_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 9)
                                                {
                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_9_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 10)
                                                {
                                                    SetShareMemoryEvent("RMAIN_RS2V_GET_VISION_FACET_10_RESULT_DONE", true);
                                                }
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6309);
                                                Machine.LogDisplay.AddLogDisplay("Error", "Bottom Vision defect code error");
                                            }
                                        SW2VisionExit:
                                            { }
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "SETUP")//R,C,Result,X,Y,T
                                {
                                    bool IsLastSnap = true;
                                    bool bAlarmStatus = false;
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "" && i != (strArrayResult.Length - 1))
                                        {
                                            Machine.SequenceControl.SetAlarm(6112);
                                            Machine.LogDisplay.AddLogDisplay("Error", "SETUP Vision Data Not In Correct Format");
                                            //MessageBox.Show("Vision result not correct");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    bool dataNotComplete = false;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "")
                                        {
                                            continue;
                                        }
                                        string[] strArrayData = strResult.Split(',');
                                        //if (strArrayData.Length != 7)
                                        ////if (strArrayData.Length != 9)
                                        //{
                                        //    dataNotComplete = true;
                                        //    Machine.SequenceControl.SetAlarm(6112);
                                        //    Machine.LogDisplay.AddLogDisplay("Error", "SETUP Vision Data Not Complete");
                                        //}
                                        //i++;
                                    }
                                    i = 0;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            //int nBinCode = 0;
                                            double dTheta_mDegree = 0;
                                            double dZ_um = 0;

                                           //if(Char.IsNumber(strArrayData[5],0) == false )
                                           // {
                                           //     strArrayData[5] = "0";
                                           // }

                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true
                                                && Double.TryParse(strArrayData[5], out dTheta_mDegree) && Double.TryParse(strArrayData[4], out dZ_um)
                                                )
                                            {
                                                if (Double.IsNaN(dTheta_mDegree) || Double.IsNaN(dZ_um)
                                                   )
                                                {
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                    goto Exit;
                                                }

                                                bool bIsAvailableUnitOnInputFrame = false;
                                                bool bIsInputDefectCode = false;

                                                if (strArrayData[2] == "P")
                                                {
                                                    m_ProductRTSSProcess.SetProductionInt("nCurrentSetupFailValue", 0);
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SetupResult", 1);
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "UnitPresent", 1);
                                                    }
                                                }
                                                else if (strArrayData[2] == "MU" || strArrayData[2] == "E")//Empty
                                                {
                                                    int TempSetupFail = m_ProductRTSSProcess.GetProductionInt("nCurrentSetupFailValue");
                                                    TempSetupFail++;
                                                    m_ProductRTSSProcess.SetProductionInt("nCurrentSetupFailValue", TempSetupFail);
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SetupResult", 3);
                                                        //m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "UnitPresent", 0);
                                                    }
                                                }
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    int TempSetupFail = m_ProductRTSSProcess.GetProductionInt("nCurrentSetupFailValue");
                                                    TempSetupFail++;
                                                    m_ProductRTSSProcess.SetProductionInt("nCurrentSetupFailValue", TempSetupFail);
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                if (m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SetupResult") == 1)
                                                                {
                                                                    m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SetupResult", 3);
                                                                    //m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "UnitPresent", 1);
                                                                }
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            if (m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SetupResult") == 1)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SetupResult", 3);
                                                                //m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "UnitPresent", 1);
                                                            }
                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6238);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "S2 Vision defect code error");
                                                            //goto SetupVisionExit;
                                                        }
                                                    }
                                                }
                                                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", nRow, nCol)))
                                                {
                                                    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false);
                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].SetupResult == null
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].SetupResult == "P"
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].SetupResult == "UTT")
                                                    {
                                                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].SetupResult = strArrayData[2];
                                                    }
                                                }
                                                else
                                                {
                                                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                }
                                                bIsInputDefectCode = false;

                                                int nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                                int nZ_um = Convert.ToInt32(dZ_um);
                                                for (int j = 0; j < 1; j++)
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SetupThetaOffset_mDegree", nTheta_MDegree);
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SetupZOffset_um", nZ_um);
                                                    }
                                                }
                                                nInp_Receive_Data++;
                                                {
                                                    SetShareMemoryEvent("RMAIN_RSTPV_GET_VISION_RESULT_DONE", true);
                                                }
                                                if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                {
                                                    if (IsLastSnap == true)
                                                    {
                                                        foreach (string _defectcode in m_ProductShareVariables.CurrentSetupDefectCode)
                                                        {
                                                            bool IsDefectCodeExists = false;
                                                            foreach (var _DefectCounter in m_ProductShareVariables.DefectCounterSetup)
                                                            {
                                                                if (_defectcode == _DefectCounter.DefectCode)
                                                                {
                                                                    _DefectCounter.Counter++;
                                                                    IsDefectCodeExists = true;
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    IsDefectCodeExists = false;
                                                                }
                                                            }
                                                            if (IsDefectCodeExists == false)
                                                            {
                                                                m_ProductShareVariables.DefectCounterSetup.Add(new ProductShareVariables.DefectCounter { DefectCode = _defectcode, Counter = 1 });
                                                            }
                                                        }
                                                        m_ProductShareVariables.CurrentSetupDefectCode.Clear();
                                                        bAlarmStatus = false;
                                                        foreach (var _DefectCounter in m_ProductShareVariables.DefectCounterSetup)
                                                        {
                                                            if (_DefectCounter.Counter >= m_ProductShareVariables.productRecipeOutputSettings.CheckContinuousDefectCode)
                                                            {
                                                                bAlarmStatus = true;
                                                                _DefectCounter.Counter = 0;
                                                                Machine.LogDisplay.AddLogDisplay("Error", _DefectCounter.DefectCode + " Occur Constinuously.");

                                                            }
                                                        }
                                                        if (bAlarmStatus == true)
                                                        {
                                                            Machine.SequenceControl.SetAlarm(7019);
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6112);
                                                Machine.LogDisplay.AddLogDisplay("Error", "Setup Vision defect code error");
                                                //MessageBox.Show("Vision error result");
                                            }
                                        SetupVisionExit:
                                            { }
                                        }
                                    }
                                    //XYResult=65,1,MD;xoffset,yoffset,thetaoffset;\r\n
                                    //SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XYTR_DONE", true);
                                }
                                else if (strReceiveCommand == "BTM")//R,C,Result,X,Y,T
                                {
                                    bool IsLastSnap = true;
                                    bool bAlarmStatus = false;
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "" && i != (strArrayResult.Length - 1))
                                        {
                                            Machine.SequenceControl.SetAlarm(6112);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Bottom Vision Data Not In Correct Format");
                                            //MessageBox.Show("Vision result not correct");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    bool dataNotComplete = false;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "")
                                        {
                                            continue;
                                        }
                                        string[] strArrayData = strResult.Split(',');
                                        //if (strArrayData.Length != 7)
                                        ////if (strArrayData.Length != 9)
                                        //{
                                        //    dataNotComplete = true;
                                        //    Machine.SequenceControl.SetAlarm(6112);
                                        //    Machine.LogDisplay.AddLogDisplay("Error", "BTM Vision Data Not Complete");
                                        //}
                                        i++;
                                    }
                                    i = 0;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            //int nBinCode = 0;
                                            double dX_um = 0;
                                            double dY_um = 0;
                                            double dTheta_mDegree = 0;
                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true
                                                && Double.TryParse(strArrayData[3], out dX_um) && Double.TryParse(strArrayData[4], out dY_um) && Double.TryParse(strArrayData[5], out dTheta_mDegree)
                                                )
                                            {
                                                if (Double.IsNaN(dX_um) || Double.IsNaN(dY_um) || Double.IsNaN(dTheta_mDegree)
                                                   )
                                                {
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                    goto Exit;
                                                }

                                                bool bIsAvailableUnitOnInputFrame = false;
                                                bool bIsInputDefectCode = false;

                                                if (strArrayData[2] == "P")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "BottomResult", 1);
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "UnitPresent", 1);
                                                    }
                                                }
                                                else if (strArrayData[2] == "MU" || strArrayData[2] == "E")//Empty
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "BottomResult", 3);
                                                        //m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "UnitPresent", 0);
                                                    }
                                                }
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "BottomResult", 3);
                                                                //m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "UnitPresent", 1);
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "BottomResult", 3);

                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6959);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "Bottom Vision defect code error");
                                                            //goto BottomVisionExit;
                                                        }
                                                    }
                                                }
                                                if (m_ProductRTSSProcess.GetEvent("GMNL_RMNL_MANUAL_MODE") == false)
                                                {
                                                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", nRow, nCol)))
                                                    {
                                                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false);
                                                        int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                        if(m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].BottomResult == null
                                                            || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].BottomResult == "P"
                                                            || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].BottomResult == "UTT")
                                                        {
                                                            m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].BottomResult = strArrayData[2];
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                    }
                                                }

                                                bIsInputDefectCode = false;

                                                int nX_um = Convert.ToInt32(dX_um);
                                                int nY_um = Convert.ToInt32(dY_um);
                                                int nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                                for (int j = 0; j < 1; j++)
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "BottomXOffset_um", nX_um);
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "BottomYOffset_um", nY_um);
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "BottomThetaOffset_mDegree", nTheta_MDegree);
                                                    }
                                                }
                                                nInp_Receive_Data++;
                                                if (m_ProductRTSSProcess.GetEvent("GMNL_RMNL_MANUAL_MODE") == true)
                                                {
                                                    if (m_ProductShareVariables.PnPCalibrationInfo != null)
                                                    {
                                                        LookUpTableOffsetData TempData = new LookUpTableOffsetData { Angle = nCol, XOffset = nX_um, YOffset = nY_um };
                                                        m_ProductShareVariables.PnPCalibrationInfo.Add(TempData);
                                                    }
                                                }
                                                //if (nInp_Receive_Data == GetShareMemoryProductionInt("nNoOfInputVisionInspectionUnit"))
                                                {
                                                    SetShareMemoryEvent("GMAIN_RBTMV_GET_VISION_RESULT_DONE", true);
                                                }
                                                if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                {
                                                    if (IsLastSnap == true)
                                                    {
                                                        foreach (string _defectcode in m_ProductShareVariables.CurrentSetupDefectCode)
                                                        {
                                                            bool IsDefectCodeExists = false;
                                                            foreach (var _DefectCounter in m_ProductShareVariables.DefectCounterSetup)
                                                            {
                                                                if (_defectcode == _DefectCounter.DefectCode)
                                                                {
                                                                    _DefectCounter.Counter++;
                                                                    IsDefectCodeExists = true;
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    IsDefectCodeExists = false;
                                                                }
                                                            }
                                                            if (IsDefectCodeExists == false)
                                                            {
                                                                m_ProductShareVariables.DefectCounterSetup.Add(new ProductShareVariables.DefectCounter { DefectCode = _defectcode, Counter = 1 });
                                                            }
                                                        }
                                                        m_ProductShareVariables.CurrentSetupDefectCode.Clear();
                                                        bAlarmStatus = false;
                                                        foreach (var _DefectCounter in m_ProductShareVariables.DefectCounterSetup)
                                                        {
                                                            if (_DefectCounter.Counter >= m_ProductShareVariables.productRecipeOutputSettings.CheckContinuousDefectCode)
                                                            {
                                                                bAlarmStatus = true;
                                                                _DefectCounter.Counter = 0;
                                                                Machine.LogDisplay.AddLogDisplay("Error", _DefectCounter.DefectCode + " Occur Constinuously.");

                                                            }
                                                        }
                                                        if (bAlarmStatus == true)
                                                        {
                                                            Machine.SequenceControl.SetAlarm(7019);
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6909);
                                                Machine.LogDisplay.AddLogDisplay("Error", "BTM Vision defect code error");
                                                //MessageBox.Show("Vision error result");
                                            }
                                            BottomVisionExit:
                                            { }
                                        }
                                    }
                                    //XYResult=65,1,MD;xoffset,yoffset,thetaoffset;\r\n
                                    //SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XYTR_DONE", true);
                                }
                                else if (strReceiveCommand == "SW1"
                                    || strReceiveCommand == "SW1_1" || strReceiveCommand == "SW1_2" || strReceiveCommand == "SW1_3"
                                    || strReceiveCommand == "SW1_4" || strReceiveCommand == "SW1_5" || strReceiveCommand == "SW1_6"
                                    || strReceiveCommand == "SW1_7" || strReceiveCommand == "SW1_8" || strReceiveCommand == "SW1_9"
                                    || strReceiveCommand == "SW1_10")
                                {
                                    bool IsAdditional = false;
                                    bool IsLastSnap = false;
                                    bool bAlarmStatus = false;
                                    int AdditionalNo = 0;
                                    if (strReceiveCommand == "SW1")
                                    {
                                        IsAdditional = false;
                                        IsLastSnap = false;
                                    }
                                    else
                                    {
                                        IsAdditional = true;
                                        string[] temp = strReceiveCommand.Split('_');
                                        if (int.TryParse(temp[1], out AdditionalNo) == false)
                                        {
                                            Machine.SequenceControl.SetAlarm(6309);
                                            Machine.LogDisplay.AddLogDisplay("Error", "S1 Vision Data Not In Correct Format");
                                        }
                                        else
                                        {
                                        }
                                    }
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    i = 0;
                                    bool dataNotComplete = false;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "")
                                        {
                                            continue;
                                        }
                                        string[] strArrayData = strResult.Split(',');
                                        //                              if (!(strArrayData.Length == 6 || strArrayData.Length == 8))
                                        //                              {
                                        //                                  dataNotComplete = true;
                                        //                                  Machine.SequenceControl.SetAlarm(6309);
                                        //                                  Machine.LogDisplay.AddLogDisplay("Error", "Bottom Vision Data Not Complete");
                                        //                              }
                                        i++;
                                    }
                                    i = 0;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            //int nBinCode = 0;
                                            double dX_um = 0;
                                            double dY_um = 0;
                                            double dTheta_mDegree = 0;
                                            double dSizeX_um = 0;
                                            double dSizeY_um = 0;
                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true
                                                && Double.TryParse(strArrayData[3], out dX_um) && Double.TryParse(strArrayData[4], out dY_um) && Double.TryParse(strArrayData[5], out dTheta_mDegree)
                                                )
                                            {
                                                if (Double.IsNaN(dX_um) || Double.IsNaN(dY_um) || Double.IsNaN(dTheta_mDegree))
                                                {
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                    goto Exit;
                                                }
                                                if (strArrayData[2] == "P")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        if(IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "S1Result", 1);
                                                            //m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "UnitPresent", 1);
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("S1Vision", AdditionalNo - 1, "Result", 1);
                                                        }
                                                    }
                                                }
                                                else if (strArrayData[2] == "MU" || strArrayData[2] == "E")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        if (IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "S1Result", 3);
                                                            //m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "UnitPresent", 0);
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("S1Vision", AdditionalNo - 1, "Result", 3);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                if (IsAdditional == false)
                                                                {
                                                                    m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "S1Result", 3);
                                                                }
                                                                else
                                                                {
                                                                    m_ProductRTSSProcess.SetSettingArray("S1Vision", AdditionalNo - 1, "Result", 3);
                                                                }
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            if (IsAdditional == false)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "S1Result", 3);
                                                            }
                                                            else
                                                            {
                                                                m_ProductRTSSProcess.SetSettingArray("S1Vision", AdditionalNo - 1, "Result", 3);
                                                            }
                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6238);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "S2 Vision defect code error");
                                                            //goto SW1_VisionExit;
                                                        }
                                                    }
                                                }
                                                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", nRow, nCol)))
                                                {
                                                    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false);
                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S1Result == null
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S1Result == "P"
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S1Result == ""
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S1Result == "UTT")
                                                    {
                                                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S1Result = strArrayData[2];
                                                    }
                                                }
                                                else
                                                {
                                                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                }
                                                int nX_um = Convert.ToInt32(dX_um);
                                                int nY_um = Convert.ToInt32(dY_um);
                                                int nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                                for (int j = 0; j < 1; j++)
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        if (strReceiveCommand == "SW1_1")
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SelveXOffset_um", nX_um);
                                                        }
                                                        if (strReceiveCommand == "SW1_2")
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS1"), "SelveYOffset_um", nX_um);
                                                        }
                                                    }
                                                }
                                                nS1_Receive_Data++;
                                                //if (nS1_Receive_Data == 6)
                                                //{
                                                SetShareMemoryEvent("GMAIN_RS1V_GET_VISION_RESULT_DONE", true);
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6309);
                                                Machine.LogDisplay.AddLogDisplay("Error", "S1 Vision incorrect format");
                                            }
                                        SW1_VisionExit:
                                            { }
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "SW3_PARTING")//R,C,Result,X,Y,T
                                {
                                    bool IsAdditional = false;
                                    bool IsLastSnap = false;
                                    bool bAlarmStatus = false;
                                    int AdditionalNo = 0;
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    i = 0;
                                    bool dataNotComplete = false;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true)
                                            {
                                                if (strArrayData[2] == "P")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S3PartingResult", 1);
                                                    }
                                                }
                                                else if (strArrayData[2] == "MU" || strArrayData[2] == "E")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S3PartingResult", 3);
                                                    }
                                                }
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S3PartingResult", 3);
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S3PartingResult", 3);

                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6738);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "S3 Vision defect code error");
                                                            //goto SW3VisionExit;
                                                        }
                                                    }
                                                }
                                                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", nRow, nCol)))
                                                {
                                                    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false);
                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3PartingResult == null
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3PartingResult == "P"
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3PartingResult == ""
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3PartingResult == "UTT")
                                                    {
                                                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3PartingResult = strArrayData[2];
                                                    }
                                                }
                                                else
                                                {
                                                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                }
                                                SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_RESULT_DONE", true);
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6738);
                                                Machine.LogDisplay.AddLogDisplay("Error", "S3 Vision defect code error");
                                            }
                                            SW3VisionExit:
                                            { }
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "SW3_FACET"
                                    || strReceiveCommand == "SW3_FACET_1" || strReceiveCommand == "SW3_FACET_2" || strReceiveCommand == "SW3_FACET_3"
                                    || strReceiveCommand == "SW3_FACET_4" || strReceiveCommand == "SW3_FACET_5" || strReceiveCommand == "SW3_FACET_6"
                                    || strReceiveCommand == "SW3_FACET_7" || strReceiveCommand == "SW3_FACET_8" || strReceiveCommand == "SW3_FACET_9"
                                    || strReceiveCommand == "SW3_FACET_10")//R,C,Result,X,Y,T
                                {
                                    bool IsAdditional = false;
                                    bool IsLastSnap = false;
                                    bool bAlarmStatus = false;
                                    int AdditionalNo = 0;
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    i = 0;
                                    bool dataNotComplete = false;
                                    if (strReceiveCommand == "SW3_FACET")
                                    {
                                        IsAdditional = false;
                                        IsLastSnap = false;
                                    }
                                    else
                                    {
                                        IsAdditional = true;
                                        string[] temp = strReceiveCommand.Split('_');
                                        if (int.TryParse(temp[2], out AdditionalNo) == false)
                                        {
                                            Machine.SequenceControl.SetAlarm(6112);
                                            Machine.LogDisplay.AddLogDisplay("Error", "SW3 Vision Data Not In Correct Format");
                                        }
                                        else
                                        {                                           
                                            m_ProductRTSSProcess.SetProductionInt("CurrentS3FacetSnapTimes", m_ProductRTSSProcess.GetProductionInt("CurrentS3FacetSnapTimes") + 1);
                                            if (AdditionalNo == m_ProductShareVariables.LastInputSnapNo)
                                            {
                                                IsLastSnap = true;
                                            }
                                            else
                                            {
                                                IsLastSnap = false;
                                            }
                                        }
                                    }
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true)
                                            {
                                                if (strArrayData[2] == "P")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        if(IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S3Result", 1);
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("S3FacetVision", AdditionalNo - 1, "Result", 1);
                                                        }

                                                    }
                                                }
                                                else if (strArrayData[2] == "MU" || strArrayData[2] == "E")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        if (IsAdditional == false)
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S3Result", 3);
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetSettingArray("S3FacetVision", AdditionalNo - 1, "Result", 3);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputColumn").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                if (IsAdditional == false)
                                                                {
                                                                    m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S3Result", 3);
                                                                }
                                                                else
                                                                {
                                                                    m_ProductRTSSProcess.SetSettingArray("S3FacetVision", AdditionalNo - 1, "Result", 3);
                                                                }
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            if (IsAdditional == false)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "S3Result", 3);
                                                            }
                                                            else
                                                            {
                                                                m_ProductRTSSProcess.SetSettingArray("S3FacetVision", AdditionalNo - 1, "Result", 3);
                                                            }
                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6738);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "S3 Vision defect code error");
                                                            //goto SW3VisionExit;
                                                        }
                                                    }
                                                }
                                                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", nRow, nCol)))
                                                {
                                                    readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false);
                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", nRow, nCol)];
                                                    //if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3Result == null
                                                    //    || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3Result == "P"
                                                    //    || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3Result == ""
                                                    //    || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3Result == "UTT")
                                                    //{
                                                    //    m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3Result = strArrayData[2];
                                                    //}

                                                    m_ProductShareVariables.S3FacetVisionResult[AdditionalNo - 1] = strArrayData[2];

                                                    for (int z = 0; z < 10; z++)
                                                    {
                                                        if (m_ProductRTSSProcess.GetSettingArray("S2FacetVision", z, "Enable") == 1)
                                                        {
                                                            if (m_ProductShareVariables.S3FacetVisionResult[z] != "P")
                                                            {
                                                                Machine.EventLogger.WriteLog(string.Format("{0} : S3_{1} Result=>{2}", DateTime.Now.ToString("yyyyMMdd HHmmss"), z.ToString(), m_ProductShareVariables.S3FacetVisionResult[z]));
                                                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3Result = m_ProductShareVariables.S3FacetVisionResult[z];
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                Machine.EventLogger.WriteLog(string.Format("{0} : S3_{1} Result=>{2}", DateTime.Now.ToString("yyyyMMdd HHmmss"), z.ToString(), "P"));
                                                                m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("PickAndPlacePickUpHeadStationResult", m_ProductRTSSProcess.GetProductionInt("nCurrentPickupHeadAtS3"), "InputTrayNo") - 1].arrayUnitInfo[curindex].S3Result = "P";
                                                            }
                                                        }
                                                        
                                                    }
                                                }
                                                else
                                                {
                                                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                }
                                                SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_RESULT_DONE", true);

                                                if (AdditionalNo == 1)                                                {
                                                    
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_1_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 2)
                                                {
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_2_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 3)
                                                {
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_3_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 4)
                                                {
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_4_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 5)
                                                {
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_5_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 6)
                                                {
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_6_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 7)
                                                {
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_7_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 8)
                                                {
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_8_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 9)
                                                {
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_9_RESULT_DONE", true);
                                                }
                                                else if (AdditionalNo == 10)
                                                {
                                                    SetShareMemoryEvent("GMAIN_RS3V_GET_VISION_FACET_10_RESULT_DONE", true);
                                                }
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6309);
                                                Machine.LogDisplay.AddLogDisplay("Error", "Bottom Vision defect code error");
                                            }
                                            SW3VisionExit:
                                            { }
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "OUT_PRE")
                                {
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "" && i != (strArrayResult.Length - 1))
                                        {
                                            Machine.SequenceControl.SetAlarm(6709);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not In Correct Format");
                                            //MessageBox.Show("Vision result not correct");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    bool dataNotComplete = false;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "")
                                        {
                                            continue;
                                        }
                                        string[] strArrayData = strResult.Split(',');
                                        if (strArrayData.Length != 6)
                                        {
                                            dataNotComplete = true;
                                            Machine.SequenceControl.SetAlarm(6709);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not Complete");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            //int nBinCode = 0;
                                            double dX_um = 0;
                                            double dY_um = 0;
                                            double dTheta_mDegree = 0;

                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true
                                                && Double.TryParse(strArrayData[3], out dX_um) && Double.TryParse(strArrayData[4], out dY_um) && Double.TryParse(strArrayData[5], out dTheta_mDegree)
                                                )
                                            {
                                                if (Double.IsNaN(dX_um) || Double.IsNaN(dY_um) || Double.IsNaN(dTheta_mDegree))
                                                {
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                    goto Exit;
                                                }

                                                if (strArrayData[2] == "P")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputResult", 1);
                                                        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputUnitPresent", 0);
                                                    }
                                                }
                                                else if (strArrayData[2] == "SKP")
                                                {
                                                    for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputResult", 7);
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputUnitPresent", 0);
                                                        }
                                                    }
                                                }
                                                else if (strArrayData[2] == "UP")
                                                {
                                                    for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputResult", 5);
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputUnitPresent", 1);
                                                        }
                                                    }
                                                    //readInfo.UpdateMapArrayData(ref m_ProductShareVariables.mappingInfo, nRow, nCol, "---", false);
                                                }
                                               
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputResult", 5);
                                                                m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputUnitPresent", 1);
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputResult", 5);
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputUnitPresent", 1);
                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6859);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "Output Vision defect code error");
                                                            //goto OutputVisionExit;
                                                        }
                                                    }

                                                }
                                            }

                                            int nX_um = Convert.ToInt32(dX_um);
                                            int nY_um = Convert.ToInt32(dY_um);
                                            int nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                            for (int j = 0; j < 1; j++)
                                            {
                                                if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputRow").ToString()
                                                    && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputColumn").ToString()
                                                    )
                                                {
                                                    m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputXOffset_um", nX_um);
                                                    m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputYOffset_um", nY_um);
                                                    m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputThetaOffset_mDegree", nTheta_MDegree);
                                                }
                                            }
                                            nOut_Pre_Receive_Data++;
                                            //if (m_ProductShareVariables.productRecipeOutputSettings.OutputTrayType == "Special Carrier")
                                            //{
                                            //if (nOut_Pre_Receive_Data == GetShareMemoryProductionInt("nNoOfOutputVisionInspectionUnit"))
                                            {
                                                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_GET_OUT_VISION_XYTR_DONE", true);
                                            }
                                        //}
                                        //else
                                        //{
                                        //    if (nOut_Pre_Receive_Data == 6)
                                        //    {
                                        //        m_ProductRTSSProcess.SetEvent("GMAIN_ROUTV_GET_VISION_RESULT_DONE", true);
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    Machine.SequenceControl.SetAlarm(6709);
                                        //    Machine.LogDisplay.AddLogDisplay("Error", "Output Vision defect code error");
                                        //}
                                        OutputVisionExit:
                                            { }
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "OUT_POST"/*|| strReceiveCommand == "OUT_POST_2"*/)
                                {
                                    bool IsAdditional = false;
                                    bool IsLastSnap = false;
                                    bool bAlarmStatus = false;
                                    int AdditionalNo = 0;
                                    if (strReceiveCommand == "OUT_POST" || strReceiveCommand == "SAMPLING1_POST")
                                    {
                                        IsAdditional = false;
                                        IsLastSnap = true;
                                    }
                                    else
                                    {
                                        IsAdditional = true;
                                        //string[] temp = strReceiveCommand.Split('_');
                                        //if(int.TryParse(temp[1],out AdditionalNo) == false)
                                        //{
                                        //    Machine.SequenceControl.SetAlarm(6709);
                                        //    Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not In Correct Format");
                                        //}
                                    }
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "" && i != (strArrayResult.Length - 1))
                                        {
                                            Machine.SequenceControl.SetAlarm(6709);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not In Correct Format");
                                            //MessageBox.Show("Vision result not correct");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    bool dataNotComplete = false;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "")
                                        {
                                            continue;
                                        }
                                        string[] strArrayData = strResult.Split(',');
                                        if (strArrayData.Length != 6)
                                        {
                                            dataNotComplete = true;
                                            Machine.SequenceControl.SetAlarm(6709);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not Complete");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            //int nBinCode = 0;
                                            double dX_um = 0;
                                            double dY_um = 0;
                                            double dTheta_mDegree = 0;

                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true
                                                && Double.TryParse(strArrayData[3], out dX_um) && Double.TryParse(strArrayData[4], out dY_um) && Double.TryParse(strArrayData[5], out dTheta_mDegree)
                                                )
                                            {
                                                if (Double.IsNaN(dX_um) || Double.IsNaN(dY_um) || Double.IsNaN(dTheta_mDegree))
                                                {
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                    goto Exit;
                                                }

                                                if (strArrayData[2] == "P")
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputResult_Post", 1);
                                                    }
                                                }
                                                else if (strArrayData[2] == "OUT")
                                                {
                                                    for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputResult", 8);
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputResult_Post", 8);
                                                        }
                                                    }
                                                    //readInfo.UpdateMapArrayData(ref m_ProductShareVariables.mappingInfo, nRow, nCol, "---", false);
                                                }
                                                else if (strArrayData[2] == "OMU")
                                                {
                                                    for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputResult", 5);
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputResult_Post", 5);
                                                        }
                                                    }
                                                    //readInfo.UpdateMapArrayData(ref m_ProductShareVariables.mappingInfo, nRow, nCol, "---", false);
                                                }
                                                else
                                                {
                                                    bool bFoundDefectCode = false;
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputRow").ToString()
                                                        )
                                                    {
                                                        foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                        {
                                                            if (strArrayData[2] == _defectProperty.Code)
                                                            {
                                                                m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputResult_Post", 3);
                                                                bFoundDefectCode = true;
                                                                break;
                                                            }
                                                        }
                                                        if (bFoundDefectCode)
                                                        {
                                                        }
                                                        else
                                                        {
                                                            if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                            {
                                                                if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                                {
                                                                    m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                                }
                                                            }
                                                            //Machine.SequenceControl.SetAlarm(6738);
                                                            //Machine.LogDisplay.AddLogDisplay("Error", "Output Vision defect code error");
                                                            //goto OutputVisionExit;
                                                        }
                                                    }
                                                }
                                                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))))
                                                {
                                                    if (strArrayData[2] == "OMU")
                                                    {
                                                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1], m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"), strArrayData[2], true);
                                                    }
                                                    else
                                                    {
                                                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1], m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"), strArrayData[2], false);
                                                    }
                                                    
                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))];
                                                    //m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputTrayType = "OUTPUT";
                                                    //m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputTrayNo = m_ProductRTSSProcess.GetProductionInt("nCurrentOutputTrayNo");
                                                    //m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputRow = nRow;
                                                    //m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputColumn = nCol;
                                                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult",0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputResult == null
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputResult == "P"
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputResult == "UTT"
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputResult == "")
                                                    {
                                                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].OutputResult = strArrayData[2];
                                                    }
                                                }
                                                else
                                                {
                                                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                }
                                                if (strArrayData[2] == "P" && strReceiveCommand == "OUT_POST")
                                                {

                                                    if (!m_ProductShareVariables.PrintOutputTrayID.Contains(m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputTrayNo").ToString()))
                                                    {
                                                        m_ProductShareVariables.PrintOutputTrayID.Add(m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "OutputTrayNo").ToString());
                                                    }
                                                }

                                                int nX_um = Convert.ToInt32(dX_um);
                                                int nY_um = Convert.ToInt32(dY_um);
                                                int nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                                for (int j = 0; j < 1; j++)
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "OutputColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputXOffset_um_Post", nX_um);
                                                        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputYOffset_um_Post", nY_um);
                                                        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "OutputThetaOffset_mDegree_Post", nTheta_MDegree);
                                                    }
                                                }

                                                nOut_Post_Receive_Data++;
                                                //if (m_ProductShareVariables.productRecipeOutputSettings.OutputTrayType == "Special Carrier")
                                                //{
                                                //if (nOut_Post_Receive_Data == GetShareMemoryProductionInt("nNoOfOutputVisionInspectionUnit"))
                                                {
                                                    m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_GET_OUT_POST_VISION_XYTR_DONE", true);
                                                }
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6809);
                                                Machine.LogDisplay.AddLogDisplay("Error", "Output Vision defect code error");
                                            }
                                        OutputVisionExit:
                                            { }
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "REJECT_PRE")
                                {
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "" && i != (strArrayResult.Length - 1))
                                        {
                                            Machine.SequenceControl.SetAlarm(6709);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not In Correct Format");
                                            //MessageBox.Show("Vision result not correct");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    bool dataNotComplete = false;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "")
                                        {
                                            continue;
                                        }
                                        string[] strArrayData = strResult.Split(',');
                                        if (strArrayData.Length != 6)
                                        {
                                            dataNotComplete = true;
                                            Machine.SequenceControl.SetAlarm(6709);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not Complete");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            //int nBinCode = 0;
                                            double dX_um = 0;
                                            double dY_um = 0;
                                            double dTheta_mDegree = 0;

                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true
                                                && Double.TryParse(strArrayData[3], out dX_um) && Double.TryParse(strArrayData[4], out dY_um) && Double.TryParse(strArrayData[5], out dTheta_mDegree)
                                                )
                                            {
                                                if (Double.IsNaN(dX_um) || Double.IsNaN(dY_um) || Double.IsNaN(dTheta_mDegree))
                                                {
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                    goto Exit;
                                                }

                                                if (strArrayData[2] == "P")
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult", 1);
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectUnitPresent", 0);
                                                        }
                                                    }

                                                    //readInfo.UpdateMapArrayData(ref m_ProductShareVariables.mappingInfo, nRow, nCol, "---", false);
                                                }
                                                else if (strArrayData[2] == "UP")
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult", 5);
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectUnitPresent", 1);
                                                        }
                                                    }
                                                    //readInfo.UpdateMapArrayData(ref m_ProductShareVariables.mappingInfo, nRow, nCol, "---", false);
                                                }
                                                else if (strArrayData[2] == "SKP")
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult", 7);
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputUnitPresent", 0);
                                                        }
                                                    }
                                                }
                                                
                                                else
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult", 6);
                                                        }
                                                    }
                                                    //bool bFoundDefectCode = false;
                                                    //foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                    //{
                                                    //    if (strArrayData[2] == _defectProperty.Code)
                                                    //    {
                                                    //        bFoundDefectCode = true;
                                                    //        break;
                                                    //    }
                                                    //}
                                                    //if (bFoundDefectCode)
                                                    //{
                                                    //    //readInfo.UpdateMapArrayData(ref m_ProductShareVariables.mappingInfo, nRow, nCol, strArrayData[2] + "", false);
                                                    //}
                                                    //else
                                                    //{
                                                    //    Machine.SequenceControl.SetAlarm(63709);
                                                    //    Machine.LogDisplay.AddLogDisplay("Error", "Output Vision defect code error");
                                                    //    goto Reject1PreVisionExit;
                                                    //}
                                                }
                                            }

                                            int nX_um = Convert.ToInt32(dX_um);
                                            int nY_um = Convert.ToInt32(dY_um);
                                            int nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                            //for (int j = 0; j < 1; j++)
                                            {
                                                if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                    && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                    )
                                                {
                                                    m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectXOffset_um", nX_um);
                                                    m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectYOffset_um", nY_um);
                                                    m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectThetaOffset_mDegree", nTheta_MDegree);
                                                }
                                            }
                                            nReject_Pre_Receive_Data++;
                                            //if (m_ProductShareVariables.productRecipeOutputSettings.RejectTrayType == "Special Carrier")
                                            //{
                                            //if (nReject_Pre_Receive_Data == GetShareMemoryProductionInt("nNoOfRejectVisionInspectionUnit"))
                                            {
                                                m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_GET_REJECT_VISION_XYTR_DONE", true);
                                            }
                                        //}
                                        //else
                                        //{
                                        //    if (nReject_Pre_Receive_Data == 6)
                                        //    {
                                        //        m_ProductRTSSProcess.SetEvent("RMAIN_RREJECTV_GET_VISION_RESULT_DONE", true);
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    Machine.SequenceControl.SetAlarm(6709);
                                        //    Machine.LogDisplay.AddLogDisplay("Error", "Output Vision defect code error");
                                        //}
                                        Reject1PreVisionExit:
                                            { }
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "REJECT_POST")
                                {
                                    string[] strArrayResult = strValue.Split(';');
                                    int i = 0;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "" && i != (strArrayResult.Length - 1))
                                        {
                                            Machine.SequenceControl.SetAlarm(6709);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not In Correct Format");
                                            //MessageBox.Show("Vision result not correct");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    bool dataNotComplete = false;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "")
                                        {
                                            continue;
                                        }
                                        string[] strArrayData = strResult.Split(',');
                                        if (strArrayData.Length != 6)
                                        {
                                            dataNotComplete = true;
                                            Machine.SequenceControl.SetAlarm(6709);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not Complete");
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    if (dataNotComplete == false)
                                    {
                                        foreach (string strResult in strArrayResult)
                                        {
                                            if (strResult == "")
                                            {
                                                continue;
                                            }
                                            string[] strArrayData = strResult.Split(',');
                                            int nRow = 0;
                                            int nCol = 0;
                                            //int nBinCode = 0;
                                            double dX_um = 0;
                                            double dY_um = 0;
                                            double dTheta_mDegree = 0;

                                            if (Int32.TryParse(strArrayData[0], out nRow) == true && Int32.TryParse(strArrayData[1], out nCol) == true
                                                && Double.TryParse(strArrayData[3], out dX_um) && Double.TryParse(strArrayData[4], out dY_um) && Double.TryParse(strArrayData[5], out dTheta_mDegree)
                                                )
                                            {
                                                //if (Double.IsNaN(dX_um) || Double.IsNaN(dY_um) || Double.IsNaN(dTheta_mDegree))
                                                //{
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                //    goto Exit;
                                                //}

                                                if (strArrayData[2] == "P")
                                                {
                                                    //for (int j = 0; j < 6; j++)
                                                    //{
                                                    //    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "RejectRow").ToString()
                                                    //        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", j, "RejectColumn").ToString()
                                                    //        )
                                                    //    {
                                                    //        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", j, "RejectResult_Post", 1);
                                                    //    }
                                                    //}
                                                    m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult_Post", 1);

                                                }
                                                else if (strArrayData[2] == "RUT")
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult_Post", 8);
                                                            //m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputUnitPresent", 0);
                                                        }
                                                    }
                                                }
                                                else if (strArrayData[2] == "RMU")
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult_Post", 5);
                                                            //m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "OutputUnitPresent", 0);
                                                        }
                                                    }
                                                }
                                                else if (strArrayData[2] == "OE" || strArrayData[2] == "MU" || strArrayData[2] == "EP")
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult_Post", 3);
                                                        }
                                                    }
                                                }
                                                else if (strArrayData[2] == "FO")
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult_Post", 2);
                                                        }
                                                    }
                                                }
                                                else if (strArrayData[2] == "FP")
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult_Post", 4);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //for (int j = 0; j < 1; j++)
                                                    {
                                                        if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                            && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                            )
                                                        {
                                                            m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectResult_Post", 6);
                                                        }
                                                    }
                                                    bool bFoundDefectCode = false;
                                                    foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                                    {
                                                        if (strArrayData[2].Contains(_defectProperty.Code) == true)
                                                        {
                                                            bFoundDefectCode = true;
                                                            break;
                                                        }
                                                    }
                                                    if (bFoundDefectCode)
                                                    {
                                                        //readInfo.UpdateMapArrayData(ref m_ProductShareVariables.mappingInfo, nRow, nCol, strArrayData[2] + "", false);
                                                    }
                                                    else
                                                    {
                                                        if (m_ProductShareVariables.productRecipeOutputSettings.EnableCheckContinuousDefectCode == true)
                                                        {
                                                            if (!m_ProductShareVariables.CurrentInputDefectCode.Contains(strArrayData[2]))
                                                            {
                                                                m_ProductShareVariables.CurrentInputDefectCode.Add(strArrayData[2]);
                                                            }
                                                        }
                                                        //Machine.SequenceControl.SetAlarm(63709);
                                                        //Machine.LogDisplay.AddLogDisplay("Error", "Output Vision defect code error");
                                                        //goto Reject1PostVisionExit;
                                                    }
                                                }

                                                if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex.ContainsKey(string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))))
                                                {
                                                    if (strArrayData[2] == "RMU")
                                                    {
                                                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1], m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"), strArrayData[2], true);
                                                    }
                                                    else
                                                    {
                                                        readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1], m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"), strArrayData[2], false);
                                                    }
                                                    
                                                    int curindex = m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].dicUnitIndex[string.Format("{0},{1}", m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow"), m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))];
                                                    
                                                    if (m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].RejectResult == null
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].RejectResult == "P"
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].RejectResult == "UTT"
                                                        || m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].RejectResult == "")
                                                    {
                                                        m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") - 1].arrayUnitInfo[curindex].RejectResult = strArrayData[2];
                                                    }
                                                }
                                                else
                                                {
                                                    Machine.DebugLogger.WriteLog(string.Format("{0} : Unit Index Not Found Row = {1}, Col = {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), nRow, nCol));
                                                }
                                                //if ((m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo") % 2) == 1)
                                                //{
                                                //    for (int j = 0; j < m_ProductShareVariables.mappingInfo.arrayUnitInfo.Length; j++)
                                                //    {
                                                //        if (m_ProductShareVariables.mappingInfo.arrayUnitInfo[j].InputRow == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow")
                                                //            && m_ProductShareVariables.mappingInfo.arrayUnitInfo[j].InputColumn == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))
                                                //        {
                                                //            m_ProductShareVariables.mappingInfo.arrayUnitInfo[j].OutputResult = strArrayData[2];
                                                //            m_ProductShareVariables.mappingInfo.arrayUnitInfo[j].OutputTrayNo = 1;
                                                //            m_ProductShareVariables.mappingInfo.arrayUnitInfo[j].OutputRow = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow");
                                                //            m_ProductShareVariables.mappingInfo.arrayUnitInfo[j].OutputColumn = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn");
                                                //            m_ProductShareVariables.mappingInfo.arrayUnitInfo[j].OutputTrayType = "Reject";

                                                //            readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.mappingInfo, m_ProductShareVariables.mappingInfo.arrayUnitInfo[j].InputRow, m_ProductShareVariables.mappingInfo.arrayUnitInfo[j].InputColumn, strArrayData[2], false, "");
                                                //            break;
                                                //        }
                                                //    }
                                                //}
                                                //else
                                                //{
                                                //    for (int j = 0; j < m_ProductShareVariables.mappingInfo2.arrayUnitInfo.Length; j++)
                                                //    {
                                                //        if (m_ProductShareVariables.mappingInfo2.arrayUnitInfo[j].InputRow == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputRow")
                                                //            && m_ProductShareVariables.mappingInfo2.arrayUnitInfo[j].InputColumn == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputColumn"))
                                                //        {
                                                //            m_ProductShareVariables.mappingInfo2.arrayUnitInfo[j].OutputResult = strArrayData[2];
                                                //            m_ProductShareVariables.mappingInfo2.arrayUnitInfo[j].OutputTrayNo = 1;
                                                //            m_ProductShareVariables.mappingInfo2.arrayUnitInfo[j].OutputRow = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow");
                                                //            m_ProductShareVariables.mappingInfo2.arrayUnitInfo[j].OutputColumn = m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn");
                                                //            m_ProductShareVariables.mappingInfo2.arrayUnitInfo[j].OutputTrayType = "Reject";
                                                //            readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.mappingInfo2, m_ProductShareVariables.mappingInfo2.arrayUnitInfo[j].InputRow, m_ProductShareVariables.mappingInfo2.arrayUnitInfo[j].InputColumn, strArrayData[2], false, "");
                                                //            readInfo.UpdateInputMapArrayData(ref m_ProductShareVariables.MultipleMappingInfo[m_ProductRTSSProcess.GetProductionArray("InputTableResult", 0, "InputTrayNo") - 1], nRow, nCol, strArrayData[2], false, "");
                                                //            break;
                                                //        }
                                                //    }
                                                //}
                                                //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} 3: InputTrayNo = {m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "InputTrayNo")}");

                                                //m_ProductProcessEvent.PCS_PCS_Set_Mapping_Result.Set();

                                                int nX_um = Convert.ToInt32(dX_um);
                                                int nY_um = Convert.ToInt32(dY_um);
                                                int nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                                //for (int j = 0; j < 1; j++)
                                                {
                                                    if (strArrayData[0] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectRow").ToString()
                                                        && strArrayData[1] == m_ProductRTSSProcess.GetProductionArray("OutputTableResult", 0, "RejectColumn").ToString()
                                                        )
                                                    {
                                                        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectXOffset_um_Post", nX_um);
                                                        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectYOffset_um_Post", nY_um);
                                                        m_ProductRTSSProcess.SetProductionArray("OutputTableResult", 0, "RejectThetaOffset_mDegree_Post", nTheta_MDegree);
                                                    }
                                                }
                                                nReject_Post_Receive_Data++;
                                                //if (m_ProductShareVariables.productRecipeOutputSettings.RejectTrayType == "Special Carrier")
                                                //{
                                                //if (nReject_Post_Receive_Data == GetShareMemoryProductionInt("nNoOfRejectVisionInspectionUnit"))
                                                {
                                                    m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_GET_REJECT_POST_VISION_XYTR_DONE", true);
                                                }
                                               
                                                //}
                                                //else
                                                //{
                                                //    if (nReject_Post_Receive_Data == 6)
                                                //    {
                                                //        m_ProductRTSSProcess.SetEvent("RMAIN_RREJECTV_POST_GET_VISION_RESULT_DONE", true);
                                                //    }
                                                //}
                                            }
                                            else
                                            {
                                                Machine.SequenceControl.SetAlarm(6709);
                                                Machine.LogDisplay.AddLogDisplay("Error", "Output Vision defect code error");
                                            }
                                        Reject1PostVisionExit:
                                            { }
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "XYR")
                                {
                                    string[] strArrayXYR = strValue.Split(',');
                                    double[] dArrayXYR = new double[strArrayXYR.Length];

                                    int i = 0;
                                    int nIndexOfDefectCode = 0;
                                    int nIndexOfX = 0;
                                    int nIndexOfY = 1;
                                    int nIndexOfTheta = 2;
                                    if (strArrayXYR.Length == 4)
                                    {
                                        //i = 1;
                                        nIndexOfDefectCode = 0;
                                        nIndexOfX = 1;
                                        nIndexOfY = 2;
                                        nIndexOfTheta = 3;
                                    }
                                    foreach (string _strXYR in strArrayXYR)
                                    {
                                        if (i < nIndexOfX)
                                            goto Exit2;
                                        if (Double.TryParse(_strXYR, out dArrayXYR[i]) == false)
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                        if (Double.IsNaN(dArrayXYR[i]))
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                    Exit2:
                                        i++;
                                    }
                                    //double dX_um = dArrayXYR[0];
                                    //double dY_um = dArrayXYR[1];
                                    //double dTheta_mDegree = dArrayXYR[2];
                                    double dX_um = dArrayXYR[nIndexOfX];
                                    double dY_um = dArrayXYR[nIndexOfY];
                                    double dTheta_mDegree = dArrayXYR[nIndexOfTheta];
                                    int nX_um = Convert.ToInt32(dX_um);
                                    int nY_um = Convert.ToInt32(dY_um);
                                    int nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);

                                    SetShareMemoryProductionLong("nVisionXOffset", nX_um);
                                    SetShareMemoryProductionLong("nVisionYOffset", nY_um);
                                    SetShareMemoryProductionLong("nVisionThetaOffset", nTheta_MDegree);
                                    //SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XYTR_DONE", true);
                                    if (strArrayXYR.Length == 4)
                                    {
                                        if (strArrayXYR[nIndexOfDefectCode] == "P")
                                        {
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XYTR_DONE", true);
                                        }
                                        else
                                        {
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XYTR_FAIL", true);
                                        }
                                        //if (m_tabpageVisionRecipe.checkBoxEnablePattern.Checked == true)
                                        //{
                                        //    if (strArrayXYR[nIndexOfDefectCode] == "E")
                                        //    {
                                        //        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XYTR_ForVerifyFirstPosition_FAIL", true);
                                        //    }
                                        //    else 
                                        //    {
                                        //        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XYTR_ForVerifyFirstPosition_DONE", true);
                                        //    }
                                        //}
                                    }
                                    else
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XYTR_DONE", true);
                                }
                                else if (strReceiveCommand == "Theta")
                                {
                                    string[] strArrayTheta = strValue.Split(',');
                                    double[] dArrayTheta = new double[strArrayTheta.Length];

                                    int i = 0;
                                    int nIndexOfDefectCode = 0;
                                    int nIndexOfTheta = 0;
                                    if (strArrayTheta.Length == 2)
                                    {
                                        //i = 1;
                                        nIndexOfDefectCode = 0;
                                        nIndexOfTheta = 1;
                                    }
                                    foreach (string _strTheta in strArrayTheta)
                                    {
                                        if (i < nIndexOfTheta)
                                            goto Exit2;
                                        if (Double.TryParse(_strTheta, out dArrayTheta[i]) == false)
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                        if (Double.IsNaN(dArrayTheta[i]))
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                    Exit2:
                                        i++;
                                    }
                                    double dThetaSum = 0;
                                    for (int j = 0; j < dArrayTheta.Length; j++)
                                    {
                                        dThetaSum += dArrayTheta[j];
                                    }
                                    double dThetaMean_degree = dThetaSum / dArrayTheta.Length;
                                    //int dThetaMean_mdegree = Convert.ToInt32(dThetaMean_degree);
                                    int dThetaMean_mdegree = Convert.ToInt32(dArrayTheta[nIndexOfTheta]);
                                    SetShareMemoryProductionLong("nVisionThetaOffset", dThetaMean_mdegree);
                                    SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_THETA_DONE", true);
                                    //SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_THETA_DONE", true);
                                    if (strArrayTheta.Length == 2)
                                    {
                                        if (strArrayTheta[nIndexOfDefectCode] == "P")
                                        {
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_THETA_DONE", true);
                                        }
                                        else
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_THETA_FAIL", true);
                                    }
                                    else
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_THETA_DONE", true);
                                }
                                else if (strReceiveCommand == "XY")
                                {
                                    //No Command
                                    string[] strArrayXY = strValue.Split(',');
                                    double[] dArrayXY = new double[strArrayXY.Length];

                                    int i = 0;
                                    int nIndexOfDefectCode = 0;
                                    int nIndexOfX = 0;
                                    int nIndexOfY = 1;
                                    if (strArrayXY.Length == 3)
                                    {
                                        //i = 1;
                                        nIndexOfDefectCode = 0;
                                        nIndexOfX = 1;
                                        nIndexOfY = 2;
                                    }
                                    foreach (string _strXY in strArrayXY)
                                    {
                                        if (i < nIndexOfX)
                                            goto Exit2;

                                        if (Double.TryParse(_strXY, out dArrayXY[i]) == false)
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                        if (Double.IsNaN(dArrayXY[i]))
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                    Exit2:
                                        i++;
                                    }
                                    //double dX_um = dArrayXY[0];
                                    //double dY_um = dArrayXY[1];
                                    double dX_um = dArrayXY[nIndexOfX];
                                    double dY_um = dArrayXY[nIndexOfY];
                                    int nX_um = Convert.ToInt32(dX_um);
                                    int nY_um = Convert.ToInt32(dY_um);
                                    SetShareMemoryProductionLong("nVisionXOffset", nX_um);
                                    SetShareMemoryProductionLong("nVisionYOffset", nY_um);
                                    //SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XY_DONE", true);
                                    if (strArrayXY.Length == 3)
                                    {
                                        if (strArrayXY[nIndexOfDefectCode] == "P")
                                        {
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XY_DONE", true);
                                        }
                                        else
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XY_FAIL", true);
                                    }
                                    else
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XY_DONE", true);
                                }
                                else if (strReceiveCommand == "UNITPITCH")
                                {
                                    //No Command
                                    string[] strArrayXY = strValue.Split(',');
                                    double[] dArrayXY = new double[strArrayXY.Length];

                                    int i = 0;
                                    int nIndexOfDefectCode = 0;
                                    int nIndexOfX = 0;
                                    int nIndexOfY = 1;
                                    if (strArrayXY.Length == 3)
                                    {
                                        //i = 1;
                                        nIndexOfDefectCode = 0;
                                        nIndexOfX = 1;
                                        nIndexOfY = 2;
                                    }
                                    foreach (string _strXY in strArrayXY)
                                    {
                                        if (i < nIndexOfX)
                                            goto Exit2;

                                        if (Double.TryParse(_strXY, out dArrayXY[i]) == false)
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                        if (Double.IsNaN(dArrayXY[i]))
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                    Exit2:
                                        i++;
                                    }
                                    //double dX_um = dArrayXY[0];
                                    //double dY_um = dArrayXY[1];
                                    double dX_um = dArrayXY[nIndexOfX];
                                    double dY_um = dArrayXY[nIndexOfY];
                                    int nX_um = Convert.ToInt32(dX_um);
                                    int nY_um = Convert.ToInt32(dY_um);
                                    SetShareMemoryProductionLong("nVisionXPitch_um", nX_um);
                                    SetShareMemoryProductionLong("nVisionYPitch_um", nY_um);

                                    //SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_XY_DONE", true);
                                    if (strArrayXY.Length == 3)
                                    {
                                        if (strArrayXY[nIndexOfDefectCode] == "P")
                                        {
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_PITCH_DONE", true);
                                        }
                                        else
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_PITCH_FAIL", true);
                                    }
                                    else
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_PITCH_DONE", true);

                                }
                                else if (strReceiveCommand == "FD")
                                {
                                    //No Command
                                    string[] strArrayNRXYR = strValue.Split(',');
                                    double[] dArrayXYR = new double[3];

                                    int i = 0;
                                    foreach (string _strNRXYR in strArrayNRXYR)
                                    {
                                        if (i == 0)
                                        {

                                        }
                                        else if (i == 1)
                                        {
                                            //P or F

                                        }
                                        else if (i == 2)
                                        {
                                            if (Double.TryParse(_strNRXYR, out dArrayXYR[0]) == false)
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                            if (Double.IsNaN(dArrayXYR[0]))
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                        }
                                        else if (i == 3)
                                        {
                                            if (Double.TryParse(_strNRXYR, out dArrayXYR[1]) == false)
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                            if (Double.IsNaN(dArrayXYR[1]))
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                        }
                                        else if (i == 4)
                                        {
                                            if (Double.TryParse(_strNRXYR, out dArrayXYR[2]) == false)
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                            if (Double.IsNaN(dArrayXYR[2]))
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                        }

                                        i++;
                                    }
                                    if (strArrayNRXYR[1] == "P")
                                    {
                                        int nX_um = Convert.ToInt32(dArrayXYR[0]);
                                        int nY_um = Convert.ToInt32(dArrayXYR[1]);
                                        int dThetaMean_mdegree = Convert.ToInt32(dArrayXYR[2]);

                                        SetShareMemoryProductionLong("nVisionXOffset", nX_um);
                                        SetShareMemoryProductionLong("nVisionYOffset", nY_um);
                                        SetShareMemoryProductionLong("nVisionThetaOffset", dThetaMean_mdegree);
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_FIDUCIAL_DONE", true);
                                    }
                                    else
                                    {
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_FIDUCIAL_FAIL", true);
                                    }
                                }
                                else if (strReceiveCommand == "AllocateUnit")
                                {
                                    string[] strArrayNRXYR = strValue.Split(',');
                                    foreach (string _strNRXYR in strArrayNRXYR)
                                    {
                                        if (_strNRXYR.Contains("ACK"))
                                        {
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_ALLOCATE_UNIT_RC_READY", true);
                                            goto Exit;
                                        }
                                        else if (_strNRXYR.Contains("NAK"))
                                        {
                                            SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_ALLOCATE_UNIT_RC_FAIL", true);
                                            goto Exit;
                                        }
                                    }
                                    double[] dArrayXYR = new double[3];

                                    int i = 0;
                                    foreach (string _strNRXYR in strArrayNRXYR)
                                    {
                                        if (i == 0)
                                        {
                                            //P or F

                                        }
                                        else if (i == 1)
                                        {
                                            if (Double.TryParse(_strNRXYR, out dArrayXYR[0]) == false)
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                            if (Double.IsNaN(dArrayXYR[0]))
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                        }
                                        else if (i == 2)
                                        {
                                            if (Double.TryParse(_strNRXYR, out dArrayXYR[1]) == false)
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                            if (Double.IsNaN(dArrayXYR[1]))
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                        }
                                        else if (i == 3)
                                        {
                                            if (Double.TryParse(_strNRXYR, out dArrayXYR[2]) == false)
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                            if (Double.IsNaN(dArrayXYR[2]))
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                                goto Exit;
                                            }
                                        }

                                        i++;
                                    }
                                    if (strArrayNRXYR[0] == "P")
                                    {
                                        int nX_um = Convert.ToInt32(dArrayXYR[0]);
                                        int nY_um = Convert.ToInt32(dArrayXYR[1]);
                                        int dThetaMean_mdegree = Convert.ToInt32(dArrayXYR[2]);

                                        SetShareMemoryProductionLong("nVisionXOffset", nX_um);
                                        SetShareMemoryProductionLong("nVisionYOffset", nY_um);
                                        SetShareMemoryProductionLong("nVisionThetaOffset", dThetaMean_mdegree);

                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_ALLOCATE_UNIT_DONE", true);
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_ALGCONFIG_ALLOCATE_UNIT_DONE", true);
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_ALLOCATE_UNIT_RC_DONE", true);
                                    }
                                    else
                                    {
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_ALLOCATE_UNIT_FAIL", true);
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_ALGCONFIG_ALLOCATE_UNIT_FAIL", true);
                                        SetShareMemoryEvent("RMAIN_RTHD_GET_VISION_ALLOCATE_UNIT_RC_DONE", true);
                                    }
                                }

                                else if (strReceiveCommand == "OUTE")
                                {
                                    if (strValue == "P")
                                    {
                                        m_ProductRTSSProcess.SetProductionInt("OutputVisionResult", 1);
                                        SetShareMemoryEvent("RMAIN_RTHD_OUT_VISION_CHECK_EMPTY_DONE", true);
                                    }
                                    else if (strValue == "F1")
                                    {
                                        m_ProductRTSSProcess.SetProductionInt("OutputVisionResult", 2);
                                        SetShareMemoryEvent("RMAIN_RTHD_OUT_VISION_CHECK_EMPTY_DONE", true);
                                    }
                                    else
                                    {
                                        Machine.SequenceControl.SetAlarm(6709);
                                        Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Data Not In Correct Format");
                                    }
                                }
                                else if (strReceiveCommand == "GetUnitXY")
                                {
                                    string[] strArrayXYR = strValue.Split(',');
                                    double[] dArrayXYR = new double[strArrayXYR.Length];

                                    int i = 0;
                                    foreach (string _strXYR in strArrayXYR)
                                    {
                                        if (Double.TryParse(_strXYR, out dArrayXYR[i]) == false)
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                        if (Double.IsNaN(dArrayXYR[i]))
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                            goto Exit;
                                        }
                                        i++;
                                    }
                                    if (i != 12)
                                    {
                                        Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision");
                                        goto Exit;
                                    }
                                    double dX_um = 0;
                                    double dY_um = 0;
                                    double dTheta_mDegree = 0;

                                    int nX_um = 0;
                                    int nY_um = 0;
                                    int nTheta_mDegree = 0;
                                    for (int j = 0; j <= 3; j++)
                                    {
                                        dX_um = dArrayXYR[0 + 3 * j];
                                        dY_um = dArrayXYR[1 + 3 * j];
                                        dTheta_mDegree = dArrayXYR[2 + 3 * j];
                                        nX_um = Convert.ToInt32(dX_um);
                                        nY_um = Convert.ToInt32(dY_um);
                                        nTheta_mDegree = Convert.ToInt32(dTheta_mDegree);
                                        m_ProductRTSSProcess.SetProductionLong("nVisionXOffsetOutput" + (j + 1).ToString(), nX_um);
                                        m_ProductRTSSProcess.SetProductionLong("nVisionYOffsetOutput" + (j + 1).ToString(), nY_um);
                                        m_ProductRTSSProcess.SetProductionLong("nVisionThetaOffsetOutput" + (j + 1).ToString(), nTheta_mDegree);
                                    }

                                    //m_ProductRTSSProcess.SetProductionLong("nVisionXOffsetOutput1", nX_um);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionYOffsetOutput1", nY_um);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionThetaOffsetOutput1", nTheta_MDegree);

                                    //dX_um = dArrayXYR[3];
                                    //dY_um = dArrayXYR[4];
                                    //dTheta_mDegree = dArrayXYR[5];
                                    //nX_um = Convert.ToInt32(dX_um);
                                    //nY_um = Convert.ToInt32(dY_um);
                                    //nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionXOffsetOutput2", nX_um);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionYOffsetOutput2", nY_um);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionThetaOffsetOutput2", nTheta_MDegree);

                                    //dX_um = dArrayXYR[6];
                                    //dY_um = dArrayXYR[7];
                                    //dTheta_mDegree = dArrayXYR[8];
                                    //nX_um = Convert.ToInt32(dX_um);
                                    //nY_um = Convert.ToInt32(dY_um);
                                    //nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionXOffsetOutput3", nX_um);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionYOffsetOutput3", nY_um);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionThetaOffsetOutput3", nTheta_MDegree);

                                    //dX_um = dArrayXYR[9];
                                    //dY_um = dArrayXYR[10];
                                    //dTheta_mDegree = dArrayXYR[11];
                                    //nX_um = Convert.ToInt32(dX_um);
                                    //nY_um = Convert.ToInt32(dY_um);
                                    //nTheta_MDegree = Convert.ToInt32(dTheta_mDegree);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionXOffsetOutput4", nX_um);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionYOffsetOutput4", nY_um);
                                    //m_ProductRTSSProcess.SetProductionLong("nVisionThetaOffsetOutput4", nTheta_MDegree);

                                    SetShareMemoryEvent("RMAIN_RTHD_OUT_VISION_GET_NEARBY_UNIT_DONE", true);
                                }
                                else if (strReceiveCommand == "RC")
                                {
                                    if (strValue.Contains("ACK"))
                                    {
                                        m_ProductRTSSProcess.SetEvent("GMAIN_RTHD_SEND_VISION_INP_RC_DONE", true);
                                    }
                                }
                                else if (strReceiveCommand == "NumImgInTile")
                                {
                                    int nNoOfImages = 0;
                                    string strImage = strValue.Substring(0, strValue.Length - ",ACK".Length);
                                    if (Int32.TryParse(strImage, out nNoOfImages) == true)
                                    {
                                        if (nNoOfImages > 0)
                                        {
                                            m_ProductShareVariables.nTotalImages = nNoOfImages;
                                            m_Server.SendData("NumImgInTile=" + strImage + ",ACK\r\n");
                                        }
                                        else
                                        {
                                            m_ProductShareVariables.nTotalImages = 0;
                                            m_Server.SendData("NumImgInTile=" + strImage + ",NAK,02\r\n");
                                        }
                                    }
                                    else
                                    {
                                        m_ProductShareVariables.nTotalImages = 0;
                                        m_Server.SendData("NumImgInTile=" + strImage + ",NAK,01\r\n");
                                    }
                                }
                                else if (strReceiveCommand == "BarcodeID" || strReceiveCommand == "TileID")
                                {
                                    m_ProductShareVariables.barcodeDevice.BarcodeID = strValue;
                                    m_ProductShareVariables.barcodeDevice.ReadBarcodeDone = true;
                                    m_ProductShareVariables.strCurrentBarcodeID = strValue;
                                }
                                else if (strReceiveCommand == "ALNCALMODE")
                                {
                                    if (strValue.Contains("ACK"))
                                    {
                                        string strStatus = strValue.Substring(0, strValue.Length - ",ACK".Length);
                                        if (strStatus.Contains("On"))
                                        {
                                            m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Initialize_ACK.Set();
                                        }
                                        else if (strStatus.Contains("Off"))
                                        {
                                            //m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_End_ACK.Set();
                                        }
                                    }
                                    else if (strValue.Contains("NAK"))
                                    {
                                        string strStatus = strValue.Substring(0, strValue.Length - ",NAK".Length);
                                        if (strStatus.Contains("On"))
                                        {
                                            m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Initialize_NAK.Set();
                                            //Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision SemiAuto Teach fail to Initialize.");
                                        }
                                        else if (strStatus.Contains("Off"))
                                        {
                                            //Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision SemiAuto Teach fail to Exit.");
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "ALNCAL")
                                {
                                    if (strValue.Contains("ACK"))
                                    {
                                        int nNoOfCapture = 0;
                                        string strCapture = strValue.Substring(0, strValue.Length - ",ACK".Length);
                                        if (Int32.TryParse(strCapture, out nNoOfCapture) == true)
                                        {
                                            if (nNoOfCapture == 1)
                                            {
                                                m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Snap_ACK.Set();
                                            }
                                            else if (nNoOfCapture == 2)
                                            {
                                                m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Snap2_ACK.Set();
                                            }
                                            else
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision fail to Snap in SemiAuto Teach.");
                                            }
                                        }
                                        else
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision fail to Snap in SemiAuto Teach.");
                                        }
                                    }
                                    else if (strValue.Contains("NAK"))
                                    {
                                        int nNoOfCapture = 0;
                                        string strCapture = strValue.Substring(0, strValue.Length - ",NAK".Length);
                                        if (Int32.TryParse(strCapture, out nNoOfCapture) == true)
                                        {
                                            if (nNoOfCapture == 1)
                                            {
                                                m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Snap_NAK.Set();
                                                Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision fail to Snap in SemiAuto Teach.");
                                            }
                                            else if (nNoOfCapture == 2)
                                            {
                                                m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_Snap2_NAK.Set();
                                                Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision fail to Snap in SemiAuto Teach.");
                                            }
                                            else
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision fail to Snap in SemiAuto Teach.");
                                            }
                                        }
                                        else
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision fail to Snap in SemiAuto Teach.");
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "GETALNCAL")
                                {
                                    //No Command
                                    string[] strArrayXYOffset = strValue.Split(',');
                                    double[] dArrayXY = new double[strArrayXYOffset.Length];

                                    int i = 0;
                                    int nIndexOfResult = 0;
                                    int nIndexOfXOffset = 0;
                                    int nIndexOfYOffset = 0;
                                    if (strArrayXYOffset.Length == 3)
                                    {
                                        nIndexOfXOffset = 0;
                                        nIndexOfYOffset = 1;
                                        nIndexOfResult = 2;
                                    }
                                    foreach (string _strXYOffset in strArrayXYOffset)
                                    {
                                        if (i > nIndexOfYOffset)
                                            goto Exit2;
                                        if (Double.TryParse(_strXYOffset, out dArrayXY[i]) == false)
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision in SemiAuto Teach");
                                            goto Exit;
                                        }
                                        if (Double.IsNaN(dArrayXY[i]))
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision in SemiAuto Teach");
                                            goto Exit;
                                        }
                                    Exit2:
                                        i++;
                                    }
                                    double dXOffset_um = dArrayXY[nIndexOfXOffset];
                                    double dYOffset_um = dArrayXY[nIndexOfYOffset];
                                    int nXOffset_um = Convert.ToInt32(dXOffset_um);
                                    int nYOffset_um = Convert.ToInt32(dYOffset_um);
                                    Machine.RTSSProcess.SetShareMemoryTeachPointLong("AlignerCenterPositionXOffset", nXOffset_um);
                                    Machine.RTSSProcess.SetShareMemoryTeachPointLong("AlignerCenterPositionYOffset", nYOffset_um);
                                    if (strArrayXYOffset.Length == 3)
                                    {
                                        if (strArrayXYOffset[nIndexOfResult] == "ACK")
                                        {
                                            Machine.EventLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), $"AlignerCenterPositionXOffset = {Machine.RTSSProcess.GetShareMemoryTeachPointLong("AlignerCenterPositionXOffset")}"));
                                            Machine.EventLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), $"AlignerCenterPositionYOffset = {Machine.RTSSProcess.GetShareMemoryTeachPointLong("AlignerCenterPositionYOffset")}"));
                                            m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_GetResult_Done.Set();
                                        }
                                        else
                                        {
                                            m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_GetResult_Fail.Set();
                                            Machine.LogDisplay.AddLogDisplay("Error", "Fail to get result from vision in SemiAuto Teach");
                                        }
                                    }
                                }

                                else if (strReceiveCommand == "INPCALMODE")
                                {
                                    if (strValue.Contains("ACK"))
                                    {
                                        string strStatus = strValue.Substring(0, strValue.Length - ",ACK".Length);
                                        if (strStatus.Contains("On"))
                                        {
                                            m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Initialize_ACK.Set();
                                        }
                                        else if (strStatus.Contains("Off"))
                                        {
                                            //m_ProductProcessEvent.PCS_GUI_Aligner_Center_Rotation_Teach_End_ACK.Set();
                                        }
                                    }
                                    else if (strValue.Contains("NAK"))
                                    {
                                        string strStatus = strValue.Substring(0, strValue.Length - ",NAK".Length);
                                        if (strStatus.Contains("On"))
                                        {
                                            m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Initialize_NAK.Set();
                                            //Machine.LogDisplay.AddLogDisplay("Error", "Input Vision SemiAuto Teach fail to Initialize.");
                                        }
                                        else if (strStatus.Contains("Off"))
                                        {
                                            //Machine.LogDisplay.AddLogDisplay("Error", "Input Vision SemiAuto Teach fail to Exit.");
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "INPCAL")
                                {
                                    if (strValue.Contains("ACK"))
                                    {
                                        int nNoOfCapture = 0;
                                        string strCapture = strValue.Substring(0, strValue.Length - ",ACK".Length);
                                        if (Int32.TryParse(strCapture, out nNoOfCapture) == true)
                                        {
                                            if (nNoOfCapture == 1)
                                            {
                                                m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Snap_ACK.Set();
                                            }
                                            else if (nNoOfCapture == 2)
                                            {
                                                m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Snap2_ACK.Set();
                                            }
                                            else
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Input Vision fail to Snap in SemiAuto Teach.");
                                            }
                                        }
                                        else
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Input Vision fail to Snap in SemiAuto Teach.");
                                        }
                                    }
                                    else if (strValue.Contains("NAK"))
                                    {
                                        int nNoOfCapture = 0;
                                        string strCapture = strValue.Substring(0, strValue.Length - ",NAK".Length);
                                        if (Int32.TryParse(strCapture, out nNoOfCapture) == true)
                                        {
                                            if (nNoOfCapture == 1)
                                            {
                                                m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Snap_NAK.Set();
                                                Machine.LogDisplay.AddLogDisplay("Error", "Input Vision fail to Snap in SemiAuto Teach.");
                                            }
                                            else if (nNoOfCapture == 2)
                                            {
                                                m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_Snap2_NAK.Set();
                                                Machine.LogDisplay.AddLogDisplay("Error", "Input Vision fail to Snap in SemiAuto Teach.");
                                            }
                                            else
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", "Input Vision fail to Snap in SemiAuto Teach.");
                                            }
                                        }
                                        else
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Input Vision fail to Snap in SemiAuto Teach.");
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "GETINPCAL")
                                {
                                    string[] strArrayXYOffset = strValue.Split(',');
                                    double[] dArrayXY = new double[strArrayXYOffset.Length];

                                    int i = 0;
                                    int nIndexOfResult = 0;
                                    int nIndexOfXOffset = 0;
                                    int nIndexOfYOffset = 0;
                                    if (strArrayXYOffset.Length == 3)
                                    {
                                        nIndexOfXOffset = 0;
                                        nIndexOfYOffset = 1;
                                        nIndexOfResult = 2;
                                    }
                                    foreach (string _strXYOffset in strArrayXYOffset)
                                    {
                                        if (i > nIndexOfYOffset)
                                            goto Exit2;
                                        if (Double.TryParse(_strXYOffset, out dArrayXY[i]) == false)
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision in SemiAuto Teach");
                                            goto Exit;
                                        }
                                        if (Double.IsNaN(dArrayXY[i]))
                                        {
                                            Machine.LogDisplay.AddLogDisplay("Error", "Receive invalid result from vision in SemiAuto Teach");
                                            goto Exit;
                                        }
                                    Exit2:
                                        i++;
                                    }
                                    double dXOffset_um = dArrayXY[nIndexOfXOffset];
                                    double dYOffset_um = dArrayXY[nIndexOfYOffset];
                                    int nXOffset_um = Convert.ToInt32(dXOffset_um);
                                    int nYOffset_um = Convert.ToInt32(dYOffset_um);
                                    Machine.RTSSProcess.SetShareMemoryTeachPointLong("InputTableCenterPositionXOffset", nXOffset_um);
                                    Machine.RTSSProcess.SetShareMemoryTeachPointLong("InputTableCenterPositionYOffset", nYOffset_um);
                                    if (strArrayXYOffset.Length == 3)
                                    {
                                        if (strArrayXYOffset[nIndexOfResult] == "ACK")
                                        {
                                            Machine.EventLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), $"InputTableCenterPositionXOffset = {Machine.RTSSProcess.GetShareMemoryTeachPointLong("InputTableCenterPositionXOffset")}"));
                                            Machine.EventLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), $"InputTableCenterPositionYOffset = {Machine.RTSSProcess.GetShareMemoryTeachPointLong("InputTableCenterPositionYOffset")}"));
                                            m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_GetResult_Done.Set();
                                        }
                                        else
                                        {
                                            m_ProductProcessEvent.PCS_GUI_IPT_Center_Rotation_Teach_GetResult_Fail.Set();
                                            Machine.LogDisplay.AddLogDisplay("Error", "Fail to get result from vision in SemiAuto Teach");
                                        }
                                    }
                                }
                                else if (strReceiveCommand == "PRODUCTINFO")
                                {
                                    bool RowAndColumnNotTally = false;
                                    m_ProductProcessEvent.PCS_PCS_Vision_Receive_INPSetting.Reset();
                                    //m_ProductProcessEvent.PCS_PCS_Vision_Receive_S1Setting.Reset();
                                    m_ProductProcessEvent.PCS_PCS_Vision_Receive_PREOUTSetting.Reset();
                                    m_ProductProcessEvent.PCS_PCS_Vision_Receive_POSTOUTSetting.Reset();
                                    m_ProductProcessEvent.PCS_PCS_Vision_Receive_PRESORTSetting.Reset();
                                    m_ProductProcessEvent.PCS_PCS_Vision_Receive_POSTSORTSetting.Reset();

                                    string[] ListOfStation = strValue.Split(';');
                                    //string[] ListData = strValue.Split(',');
                                    //int NumUnitPerFrame_Col = Int32.Parse(ListData[0]);
                                    //int NumUnitPerFrame_Row = Int32.Parse(ListData[1]);
                                    //int NumUnitPerFOV_Col = Int32.Parse(ListData[2]);
                                    //int NumUnitPerFOV_RoW = Int32.Parse(ListData[3]);
                                    //int NumIslandPerFrame_Col = Int32.Parse(ListData[4]);
                                    //int NumIslandPerFrame_Row = Int32.Parse(ListData[5]);
                                    //int NumUnitPerIslandPerFrame_Col = Int32.Parse(ListData[6]);
                                    //int NumUnitPerIslandPerFrame_Row = Int32.Parse(ListData[7]);
                                    string[] ListData_INP_SOFT = ListOfStation[1].Split(',');
                                    int NumUnitPerFrame_Col_INP_SOFT = 0;
                                    int NumUnitPerFrame_Row_INP_SOFT = 0;
                                    int NumUnitPerFOV_Col_INP_SOFT = 0;
                                    int NumUnitPerFOV_Row_INP_SOFT = 0;

                                    string[] ListData_INP_JEDEC = ListOfStation[0].Split(',');
                                    int NumUnitPerFrame_Col_INP_JEDEC = 0;
                                    int NumUnitPerFrame_Row_INP_JEDEC = 0;
                                    int NumUnitPerFOV_Col_INP_JEDEC = 0;
                                    int NumUnitPerFOV_Row_INP_JEDEC = 0;

                                    string[] ListData_S1 = ListOfStation[2].Split(',');
                                    int NumUnitPerFrame_Col_S1 = 0;
                                    int NumUnitPerFrame_Row_S1 = 0;
                                    int NumUnitPerFOV_Col_S1 = 0;
                                    int NumUnitPerFOV_Row_S1 = 0;

                                    string[] ListData_PREOUT_JEDEC = ListOfStation[3].Split(',');
                                    int NumUnitPerFrame_Col_PREOUT_JEDEC = 0;
                                    int NumUnitPerFrame_Row_PREOUT_JEDEC = 0;
                                    int NumUnitPerFOV_Col_PREOUT_JEDEC = 0;
                                    int NumUnitPerFOV_Row_PREOUT_JEDEC = 0;

                                    string[] ListData_POSTOUT_JEDEC = ListOfStation[4].Split(',');
                                    int NumUnitPerFrame_Col_POSTOUT_JEDEC = 0;
                                    int NumUnitPerFrame_Row_POSTOUT_JEDEC = 0;
                                    int NumUnitPerFOV_Col_POSTOUT_JEDEC = 0;
                                    int NumUnitPerFOV_Row_POSTOUT_JEDEC = 0;

                                    string[] ListData_PREOUT_SOFT = ListOfStation[5].Split(',');
                                    int NumUnitPerFrame_Col_PREOUT_SOFT = 0;
                                    int NumUnitPerFrame_Row_PREOUT_SOFT = 0;
                                    int NumUnitPerFOV_Col_PREOUT_SOFT = 0;
                                    int NumUnitPerFOV_Row_PREOUT_SOFT = 0;

                                    string[] ListData_POSTOUT_SOFT = ListOfStation[6].Split(',');
                                    int NumUnitPerFrame_Col_POSTOUT_SOFT = 0;
                                    int NumUnitPerFrame_Row_POSTOUT_SOFT = 0;
                                    int NumUnitPerFOV_Col_POSTOUT_SOFT = 0;
                                    int NumUnitPerFOV_Row_POSTOUT_SOFT = 0;

                                    string[] ListData_PREOUT_SPC = ListOfStation[7].Split(',');
                                    int NumUnitPerFrame_Col_PREOUT_SPC = 0;
                                    int NumUnitPerFrame_Row_PREOUT_SPC = 0;
                                    int NumUnitPerFOV_Col_PREOUT_SPC = 0;
                                    int NumUnitPerFOV_Row_PREOUT_SPC = 0;

                                    string[] ListData_POSTOUT_SPC = ListOfStation[8].Split(',');
                                    int NumUnitPerFrame_Col_POSTOUT_SPC = 0;
                                    int NumUnitPerFrame_Row_POSTOUT_SPC = 0;
                                    int NumUnitPerFOV_Col_POSTOUT_SPC = 0;
                                    int NumUnitPerFOV_Row_POSTOUT_SPC = 0;
                                    //if (m_ProductShareVariables.productRecipeInputSettings.InputTrayType == "Soft Tray")
                                    //{
                                    //    if (ListData_INP_SOFT.Length != 4)
                                    //    {
                                    //        Machine.LogDisplay.AddLogDisplay("Error", "ListData_INP_SOFT integer parse failed.");
                                    //        goto Exit;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (Int32.TryParse(ListData_INP_SOFT[0], out NumUnitPerFrame_Col_INP_SOFT)
                                    //            && Int32.TryParse(ListData_INP_SOFT[1], out NumUnitPerFrame_Row_INP_SOFT)
                                    //            && Int32.TryParse(ListData_INP_SOFT[2], out NumUnitPerFOV_Col_INP_SOFT)
                                    //            && Int32.TryParse(ListData_INP_SOFT[3], out NumUnitPerFOV_Row_INP_SOFT))
                                    //        {
                                    //            //Parse Successful
                                    //        }
                                    //        else
                                    //        {
                                    //            Machine.LogDisplay.AddLogDisplay("Error", "ListData_INP_SOFT integer parse failed.");
                                    //            goto Exit;
                                    //        }
                                    //    }

                                    //    if (NumUnitPerFrame_Col_INP_SOFT != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColSoft || NumUnitPerFrame_Row_INP_SOFT != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowSoft
                                    //        || NumUnitPerFOV_Col_INP_SOFT != m_ProductShareVariables.productRecipeVisionSettings.InputSoftVisionInspectionCountInCol || NumUnitPerFOV_Row_INP_SOFT != m_ProductShareVariables.productRecipeVisionSettings.InputSoftVisionInspectionCountInRow)
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_INPSetting.Reset();
                                    //    }
                                    //    else
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_INPSetting.Set();
                                    //    }
                                    //}
                                    //else if (m_ProductShareVariables.productRecipeInputSettings.InputTrayType == "Jedec Tray")
                                    //{
                                    //    if (ListData_INP_JEDEC.Length != 4)
                                    //    {
                                    //        Machine.LogDisplay.AddLogDisplay("Error", "ListData_INP_JEDEC integer parse failed.");
                                    //        goto Exit;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (Int32.TryParse(ListData_INP_JEDEC[0], out NumUnitPerFrame_Col_INP_JEDEC)
                                    //            && Int32.TryParse(ListData_INP_JEDEC[1], out NumUnitPerFrame_Row_INP_JEDEC)
                                    //            && Int32.TryParse(ListData_INP_JEDEC[2], out NumUnitPerFOV_Col_INP_JEDEC)
                                    //            && Int32.TryParse(ListData_INP_JEDEC[3], out NumUnitPerFOV_Row_INP_JEDEC))
                                    //        {
                                    //            //Parse Successful
                                    //        }
                                    //        else
                                    //        {
                                    //            Machine.LogDisplay.AddLogDisplay("Error", "ListData_INP_JEDEC integer parse failed.");
                                    //            goto Exit;
                                    //        }
                                    //    }

                                    //    if (NumUnitPerFrame_Col_INP_JEDEC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColJedec || NumUnitPerFrame_Row_INP_JEDEC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowJedec
                                    //        || NumUnitPerFOV_Col_INP_JEDEC != m_ProductShareVariables.productRecipeVisionSettings.InputJedecVisionInspectionCountInCol || NumUnitPerFOV_Row_INP_JEDEC != m_ProductShareVariables.productRecipeVisionSettings.InputJedecVisionInspectionCountInRow)
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_INPSetting.Reset();
                                    //    }
                                    //    else
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_INPSetting.Set();
                                    //    }
                                    //}


                                    //if (ListData_S1.Length != 4)
                                    //{
                                    //    Machine.LogDisplay.AddLogDisplay("Error", "ListData_S1 integer parse failed.");
                                    //    goto Exit;
                                    //}
                                    //else
                                    //{
                                    //    if (Int32.TryParse(ListData_S1[0], out NumUnitPerFrame_Col_S1)
                                    //        && Int32.TryParse(ListData_S1[1], out NumUnitPerFrame_Row_S1)
                                    //        && Int32.TryParse(ListData_S1[2], out NumUnitPerFOV_Col_S1)
                                    //        && Int32.TryParse(ListData_S1[3], out NumUnitPerFOV_Row_S1))
                                    //    {
                                    //        //Parse Successful
                                    //    }
                                    //    else
                                    //    {
                                    //        Machine.LogDisplay.AddLogDisplay("Error", "ListData_S1 integer parse failed.");
                                    //        goto Exit;
                                    //    }
                                    //}

                                    //if (NumUnitPerFOV_Col_S1 != m_ProductShareVariables.productRecipeVisionSettings.S1VisionInspectionCountInCol || NumUnitPerFOV_Row_S1 != m_ProductShareVariables.productRecipeVisionSettings.S1VisionInspectionCountInRow)
                                    //{
                                    //    m_ProductProcessEvent.PCS_PCS_Vision_Receive_S1Setting.Reset();
                                    //}
                                    //else
                                    //{
                                    //    m_ProductProcessEvent.PCS_PCS_Vision_Receive_S1Setting.Set();
                                    //}

                                    //if (m_ProductShareVariables.productRecipeInputSettings.OutputTrayType == "Jedec Tray")
                                    //{
                                    //    if (ListData_PREOUT_JEDEC.Length != 4)
                                    //    {
                                    //        Machine.LogDisplay.AddLogDisplay("Error", "ListData_PREOUT_JEDEC integer parse failed.");
                                    //        goto Exit;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (Int32.TryParse(ListData_PREOUT_JEDEC[0], out NumUnitPerFrame_Col_PREOUT_JEDEC)
                                    //            && Int32.TryParse(ListData_PREOUT_JEDEC[1], out NumUnitPerFrame_Row_PREOUT_JEDEC)
                                    //            && Int32.TryParse(ListData_PREOUT_JEDEC[2], out NumUnitPerFOV_Col_PREOUT_JEDEC)
                                    //            && Int32.TryParse(ListData_PREOUT_JEDEC[3], out NumUnitPerFOV_Row_PREOUT_JEDEC))
                                    //        {
                                    //            //Parse Successful
                                    //        }
                                    //        else
                                    //        {
                                    //            Machine.LogDisplay.AddLogDisplay("Error", "ListData_PREOUT_JEDEC integer parse failed.");
                                    //            goto Exit;
                                    //        }
                                    //    }

                                    //    if (NumUnitPerFrame_Col_PREOUT_JEDEC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColJedec || NumUnitPerFrame_Row_PREOUT_JEDEC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowJedec
                                    //        || NumUnitPerFOV_Col_PREOUT_JEDEC != m_ProductShareVariables.productRecipeVisionSettings.OUTJedecVisionInspectionCountInCol || NumUnitPerFOV_Row_PREOUT_JEDEC != m_ProductShareVariables.productRecipeVisionSettings.OUTJedecVisionInspectionCountInRow)
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_PREOUTSetting.Reset();
                                    //    }
                                    //    else
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_PREOUTSetting.Set();
                                    //    }

                                    //    if (ListData_POSTOUT_JEDEC.Length != 4)
                                    //    {
                                    //        Machine.LogDisplay.AddLogDisplay("Error", "ListData_POSTOUT_JEDEC integer parse failed.");
                                    //        goto Exit;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (Int32.TryParse(ListData_POSTOUT_JEDEC[0], out NumUnitPerFrame_Col_POSTOUT_JEDEC)
                                    //            && Int32.TryParse(ListData_POSTOUT_JEDEC[1], out NumUnitPerFrame_Row_POSTOUT_JEDEC)
                                    //            && Int32.TryParse(ListData_POSTOUT_JEDEC[2], out NumUnitPerFOV_Col_POSTOUT_JEDEC)
                                    //            && Int32.TryParse(ListData_POSTOUT_JEDEC[3], out NumUnitPerFOV_Row_POSTOUT_JEDEC))
                                    //        {
                                    //            //Parse Successful
                                    //        }
                                    //        else
                                    //        {
                                    //            Machine.LogDisplay.AddLogDisplay("Error", "ListData_POSTOUT_JEDEC integer parse failed.");
                                    //            goto Exit;
                                    //        }
                                    //    }

                                    //    if (NumUnitPerFrame_Col_POSTOUT_JEDEC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColJedec || NumUnitPerFrame_Row_POSTOUT_JEDEC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowJedec
                                    //        || NumUnitPerFOV_Col_POSTOUT_JEDEC != m_ProductShareVariables.productRecipeVisionSettings.OUTJedecVisionInspectionCountInCol || NumUnitPerFOV_Row_POSTOUT_JEDEC != m_ProductShareVariables.productRecipeVisionSettings.OUTJedecVisionInspectionCountInRow)
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_POSTOUTSetting.Reset();
                                    //    }
                                    //    else
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_POSTOUTSetting.Set();
                                    //    }
                                    //}

                                    //else if (m_ProductShareVariables.productRecipeInputSettings.OutputTrayType == "Soft Tray")
                                    //{
                                    //    if (ListData_PREOUT_SOFT.Length != 4)
                                    //    {
                                    //        Machine.LogDisplay.AddLogDisplay("Error", "ListData_PREOUT_JEDEC integer parse failed.");
                                    //        goto Exit;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (Int32.TryParse(ListData_PREOUT_SOFT[0], out NumUnitPerFrame_Col_PREOUT_SOFT)
                                    //            && Int32.TryParse(ListData_PREOUT_SOFT[1], out NumUnitPerFrame_Row_PREOUT_SOFT)
                                    //            && Int32.TryParse(ListData_PREOUT_SOFT[2], out NumUnitPerFOV_Col_PREOUT_SOFT)
                                    //            && Int32.TryParse(ListData_PREOUT_SOFT[3], out NumUnitPerFOV_Row_PREOUT_SOFT))
                                    //        {
                                    //            //Parse Successful
                                    //        }
                                    //        else
                                    //        {
                                    //            Machine.LogDisplay.AddLogDisplay("Error", "ListData_PREOUT_SOFT integer parse failed.");
                                    //            goto Exit;
                                    //        }
                                    //    }

                                    //    if (NumUnitPerFrame_Col_PREOUT_SOFT != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColSoft || NumUnitPerFrame_Row_PREOUT_SOFT != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowSoft
                                    //        || NumUnitPerFOV_Col_PREOUT_SOFT != m_ProductShareVariables.productRecipeVisionSettings.OUTSoftVisionInspectionCountInCol || NumUnitPerFOV_Row_PREOUT_SOFT != m_ProductShareVariables.productRecipeVisionSettings.OUTSoftVisionInspectionCountInRow)
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_PREOUTSetting.Reset();
                                    //    }
                                    //    else
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_PREOUTSetting.Set();
                                    //    }

                                    //    if (ListData_POSTOUT_SOFT.Length != 4)
                                    //    {
                                    //        Machine.LogDisplay.AddLogDisplay("Error", "ListData_POSTOUT_SOFT integer parse failed.");
                                    //        goto Exit;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (Int32.TryParse(ListData_POSTOUT_SOFT[0], out NumUnitPerFrame_Col_POSTOUT_SOFT)
                                    //            && Int32.TryParse(ListData_POSTOUT_SOFT[1], out NumUnitPerFrame_Row_POSTOUT_SOFT)
                                    //            && Int32.TryParse(ListData_POSTOUT_SOFT[2], out NumUnitPerFOV_Col_POSTOUT_SOFT)
                                    //            && Int32.TryParse(ListData_POSTOUT_SOFT[3], out NumUnitPerFOV_Row_POSTOUT_SOFT))
                                    //        {
                                    //            //Parse Successful
                                    //        }
                                    //        else
                                    //        {
                                    //            Machine.LogDisplay.AddLogDisplay("Error", "ListData_POSTOUT_SOFT integer parse failed.");
                                    //            goto Exit;
                                    //        }
                                    //    }

                                    //    if (NumUnitPerFrame_Col_POSTOUT_SOFT != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColSoft || NumUnitPerFrame_Row_POSTOUT_SOFT != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowSoft
                                    //        || NumUnitPerFOV_Col_POSTOUT_SOFT != m_ProductShareVariables.productRecipeVisionSettings.OUTSoftVisionInspectionCountInCol || NumUnitPerFOV_Row_POSTOUT_SOFT != m_ProductShareVariables.productRecipeVisionSettings.OUTSoftVisionInspectionCountInRow)
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_POSTOUTSetting.Reset();
                                    //    }
                                    //    else
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_POSTOUTSetting.Set();
                                    //    }
                                    //}
                                    //else if (m_ProductShareVariables.productRecipeInputSettings.OutputTrayType == "Special Carrier")
                                    //{
                                    //    if (ListData_PREOUT_SPC.Length != 4)
                                    //    {
                                    //        Machine.LogDisplay.AddLogDisplay("Error", "ListData_PREOUT_SPC integer parse failed.");
                                    //        goto Exit;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (Int32.TryParse(ListData_PREOUT_SPC[0], out NumUnitPerFrame_Col_PREOUT_SPC)
                                    //            && Int32.TryParse(ListData_PREOUT_SPC[1], out NumUnitPerFrame_Row_PREOUT_SPC)
                                    //            && Int32.TryParse(ListData_PREOUT_SPC[2], out NumUnitPerFOV_Col_PREOUT_SPC)
                                    //            && Int32.TryParse(ListData_PREOUT_SPC[3], out NumUnitPerFOV_Row_PREOUT_SPC))
                                    //        {
                                    //            //Parse Successful
                                    //        }
                                    //        else
                                    //        {
                                    //            Machine.LogDisplay.AddLogDisplay("Error", "ListData_PREOUT_SPC integer parse failed.");
                                    //            goto Exit;
                                    //        }
                                    //    }

                                    //    if (NumUnitPerFrame_Col_PREOUT_SPC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColSpecial || NumUnitPerFrame_Row_PREOUT_SPC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowSpecial
                                    //        || NumUnitPerFOV_Col_PREOUT_SPC != m_ProductShareVariables.productRecipeVisionSettings.OUTSPCVisionInspectionCountInCol || NumUnitPerFOV_Row_PREOUT_SPC != m_ProductShareVariables.productRecipeVisionSettings.OUTSPCVisionInspectionCountInRow)
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_PREOUTSetting.Reset();
                                    //    }
                                    //    else
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_PREOUTSetting.Set();
                                    //    }

                                    //    if (ListData_POSTOUT_SPC.Length != 4)
                                    //    {
                                    //        Machine.LogDisplay.AddLogDisplay("Error", "ListData_POSTOUT_SPC integer parse failed.");
                                    //        goto Exit;
                                    //    }
                                    //    else
                                    //    {
                                    //        if (Int32.TryParse(ListData_POSTOUT_SPC[0], out NumUnitPerFrame_Col_POSTOUT_SPC)
                                    //            && Int32.TryParse(ListData_POSTOUT_SPC[1], out NumUnitPerFrame_Row_POSTOUT_SPC)
                                    //            && Int32.TryParse(ListData_POSTOUT_SPC[2], out NumUnitPerFOV_Col_POSTOUT_SPC)
                                    //            && Int32.TryParse(ListData_POSTOUT_SPC[3], out NumUnitPerFOV_Row_POSTOUT_SPC))
                                    //        {
                                    //            //Parse Successful
                                    //        }
                                    //        else
                                    //        {
                                    //            Machine.LogDisplay.AddLogDisplay("Error", "ListData_POSTOUT_SPC integer parse failed.");
                                    //            goto Exit;
                                    //        }
                                    //    }

                                    //    if (NumUnitPerFrame_Col_POSTOUT_SPC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInColSpecial || NumUnitPerFrame_Row_POSTOUT_SPC != m_ProductShareVariables.productRecipeInputSettings.NoOfDeviceInRowSpecial
                                    //        || NumUnitPerFOV_Col_POSTOUT_SPC != m_ProductShareVariables.productRecipeVisionSettings.OUTSPCVisionInspectionCountInCol || NumUnitPerFOV_Row_POSTOUT_SPC != m_ProductShareVariables.productRecipeVisionSettings.OUTSPCVisionInspectionCountInRow)
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_POSTOUTSetting.Reset();
                                    //    }
                                    //    else
                                    //    {
                                    //        m_ProductProcessEvent.PCS_PCS_Vision_Receive_POSTOUTSetting.Set();
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    m_ProductProcessEvent.PCS_PCS_Vision_Receive_PRESORTSetting.Set();
                                    //    m_ProductProcessEvent.PCS_PCS_Vision_Receive_POSTSORTSetting.Set();
                                    //}
                                }
                                else if (strReceiveCommand == "DefCode")
                                {
                                    //-----Temporary Bypass Until Vision Side Set Recipe
                                    //1.0.0.0be Thor
                                    m_ProductProcessEvent.PCS_PCS_Vision_Receive_AllStationDefectCode.Reset();
                                    m_ProductProcessEvent.PCS_PCS_Vision_AllStationDefectCode_Error.Reset();
                                    //--

                                    //if (m_ProductShareVariables.currentMainRecipeName == "Default")
                                    {
                                        goto skipProcess;
                                    }
                                    
                                    string[] strArrayResult = strValue.Split(';');
                                    int a = strArrayResult.Length;
                                    if (strArrayResult.Length != 6)
                                    {
                                        Machine.SequenceControl.SetAlarm(6005);
                                        Machine.LogDisplay.AddLogDisplay("Error", "Vision Defect Code Not In Correct Format " + strArrayResult.Length.ToString());
                                        m_ProductProcessEvent.PCS_PCS_Vision_AllStationDefectCode_Error.Set();
                                        goto Exit;
                                    }
                                    int i = 0;
                                    foreach (string strResult in strArrayResult)
                                    {
                                        if (strResult == "" && i != (strArrayResult.Length - 1))
                                        {
                                            Machine.SequenceControl.SetAlarm(6005);
                                            Machine.LogDisplay.AddLogDisplay("Error", "Vision Defect Code Not In Correct Format " + i.ToString() + "!=" + (strArrayResult.Length - 1).ToString());
                                        }
                                        i++;
                                    }
                                    i = 0;
                                    //bool dataNotComplete = false;
                                    //foreach (string strResult in strArrayResult)
                                    //{
                                    //    if (strResult == "")
                                    //    {
                                    //        continue;
                                    //    }
                                    //    string[] strArrayData = strResult.Split(',');
                                    //    //if (strArrayData.Length != 9)
                                    //    //{
                                    //    //    dataNotComplete = true;
                                    //    //    Machine.SequenceControl.SetAlarm(6609);
                                    //    //    Machine.LogDisplay.AddLogDisplay("Error", "Side Wall Top Vision Data Not Complete");
                                    //    //}
                                    //    i++;
                                    //}
                                    //i = 0;
                                    //if (dataNotComplete == false)
                                    //{
                                    foreach (string strResult in strArrayResult)
                                    {
                                        i++;
                                        if (strResult == "")
                                        {
                                            continue;
                                        }
                                        string[] strArrayData = strResult.Split(',');

                                        for (int j = 0; j < strArrayData.Length; j++)
                                        {
                                            bool bFoundDefectCode = false;
                                            bool bFoundWrongDefect = false;
                                            foreach (DefectProperty _defectProperty in m_ProductShareVariables.outputFileOption.listDefect)
                                            {
                                                //if (i == 1)
                                                //{
                                                //    if ((strArrayData[j] == _defectProperty.Code) || strArrayData[j] == "E" || strArrayData[j] == "T" || strArrayData[j] == "F")
                                                //    {
                                                //        bFoundDefectCode = true;
                                                //        break;
                                                //    }
                                                //}
                                                //else if (i == strArrayResult.Length - 1)
                                                //{
                                                //    if ((strArrayData[j] == _defectProperty.Code) || strArrayData[j] == "F1" || strArrayData[j] == "F2" || strArrayData[j] == "F3"
                                                //        || strArrayData[j] == "E" || strArrayData[j] == "T" || strArrayData[j] == "F"
                                                //        )
                                                //    {
                                                //        bFoundDefectCode = true;
                                                //        break;
                                                //    }
                                                //    else if (strArrayData[j] == _defectProperty.Code)
                                                //    {
                                                //        bFoundWrongDefect = true;
                                                //        break;
                                                //    }
                                                //}
                                                //else
                                                {
                                                    if ((strArrayData[j] == _defectProperty.Code) || strArrayData[j] == "E" || strArrayData[j] == "T" || strArrayData[j] == "F")
                                                    {
                                                        bFoundDefectCode = true;
                                                        break;
                                                    }
                                                    else if (strArrayData[j] == _defectProperty.Code)
                                                    {
                                                        bFoundWrongDefect = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (bFoundDefectCode)
                                            {
                                                //check is it input defect in other station
                                            }
                                            else if (bFoundWrongDefect)
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", $"Defect code {strArrayData[j]} exist in Input Defect Code.");
                                                if (i == 2)
                                                {
                                                    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                    {
                                                        Machine.SequenceControl.SetAlarm(6234);
                                                    }
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Top Vision Contain Input Defect Code.");
                                                }
                                                else if (i == 4)
                                                {
                                                    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                    {
                                                        Machine.SequenceControl.SetAlarm(6334);
                                                    }
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Bottom Vision Contain Input Defect Code.");
                                                }
                                                else if (i == 3)
                                                {
                                                    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                    {
                                                        Machine.SequenceControl.SetAlarm(6434);
                                                    }
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision Contain Input Defect Code.");
                                                }
                                                else if (i == 5)
                                                {
                                                    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                    {
                                                        Machine.SequenceControl.SetAlarm(6531);
                                                    }
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Side Wall Left Vision Contain Input Defect Code.");
                                                }
                                                else if (i == 6)
                                                {
                                                    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                    {
                                                        Machine.SequenceControl.SetAlarm(6533);
                                                    }
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Side Wall Right Vision Contain Input Defect Code.");
                                                }
                                                else if (i == 7)
                                                {
                                                    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                    {
                                                        Machine.SequenceControl.SetAlarm(6631);
                                                    }
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Side Wall Top Vision Contain Input Defect Code.");
                                                }
                                                else if (i == 8)
                                                {
                                                    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                    {
                                                        Machine.SequenceControl.SetAlarm(6633);
                                                    }
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Side Wall Bottom Vision Contain Input Defect Code.");
                                                }
                                                else if (i == 9)
                                                {
                                                    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                    {
                                                        Machine.SequenceControl.SetAlarm(6734);
                                                    }
                                                    Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Contain Input Defect Code.");
                                                }
                                                m_ProductProcessEvent.PCS_PCS_Vision_AllStationDefectCode_Error.Set();
                                                goto Exit;
                                            }
                                            else
                                            {
                                                Machine.LogDisplay.AddLogDisplay("Error", $"Defect code {strArrayData[j]} not exist in handler.");
                                                Machine.SequenceControl.SetAlarm(6133);
                                                Machine.LogDisplay.AddLogDisplay("Error", "Input Vision Defect Code not Tally.");
                                                //if (i == 1)
                                                //{
                                                //    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                //    {
                                                //        Machine.SequenceControl.SetAlarm(6133);
                                                //    }
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Input Vision Defect Code not Tally.");
                                                //}
                                                //else if (i == 2)
                                                //{
                                                //    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                //    {
                                                //        Machine.SequenceControl.SetAlarm(6233);
                                                //    }
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Top Vision Defect Code not Tally.");
                                                //}
                                                //else if (i == 3)
                                                //{
                                                //    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                //    {
                                                //        Machine.SequenceControl.SetAlarm(6433);
                                                //    }
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Aligner Vision Defect Code not Tally.");
                                                //}
                                                //else if (i == 4)
                                                //{
                                                //    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                //    {
                                                //        Machine.SequenceControl.SetAlarm(6333);
                                                //    }
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Bottom Vision Defect Code not Tally.");
                                                //}
                                                //else if (i == 5)
                                                //{
                                                //    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                //    {
                                                //        Machine.SequenceControl.SetAlarm(6530);
                                                //    }
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Side Wall Left Vision Defect Code not Tally.");
                                                //}
                                                //else if (i == 6)
                                                //{
                                                //    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                //    {
                                                //        Machine.SequenceControl.SetAlarm(6532);
                                                //    }
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Side Wall Right Vision Defect Code not Tally.");
                                                //}
                                                //else if (i == 7)
                                                //{
                                                //    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                //    {
                                                //        Machine.SequenceControl.SetAlarm(6630);
                                                //    }
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Side Wall Top Vision Defect Code not Tally.");
                                                //}
                                                //else if (i == 8)
                                                //{
                                                //    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                //    {
                                                //        Machine.SequenceControl.SetAlarm(6632);
                                                //    }
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Side Wall Bottom Vision Defect Code not Tally.");
                                                //}
                                                //else if (i == 9)
                                                //{
                                                //    if (m_ProductShareVariables.StateMain != Machine.StateControl.IdleDoneState)
                                                //    {
                                                //        Machine.SequenceControl.SetAlarm(6733);
                                                //    }
                                                //    Machine.LogDisplay.AddLogDisplay("Error", "Output Vision Defect Code not Tally.");
                                                //}
                                                m_ProductProcessEvent.PCS_PCS_Vision_AllStationDefectCode_Error.Set();
                                                goto Exit;
                                            }
                                        }
                                    }
                                skipProcess:
                                    m_ProductProcessEvent.PCS_PCS_Vision_Receive_AllStationDefectCode.Set();
                                    //}
                                    //----------
                                }
                                else if (strReceiveCommand == "")
                                {
                                    //No Command
                                }
                            Exit:
                                continue;
                            }
                        }
                    }
                    //Thread.Sleep(1);
                    Thread.Sleep(1);
                }
                if (m_Server != null)
                {
                    m_Server.Kill();
                    m_Server.Disconnect();
                    //m_Server = null;
                }
                m_bAbortThread = true;
            }
            catch (Exception ex)
            {
                m_bAbortThread = true;
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }

        }


    }
}
