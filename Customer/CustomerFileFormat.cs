using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Common;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Reflection;

namespace Customer
{
    public class CustomerFrameOrTileDefect : Product.ProductFrameOrTileDefect
    {
        //public int InputQuantity;
    }

    public class CustomerSummary : Product.ProductSummary
    {
        public List<CustomerFrameOrTileDefect> listFrameOrTileDefect;
        public string PPLot;
        public string BinNo;
    }

    public struct DataInfo
    {
        public int InputRow;
        public int InputColumn;
        public int BinLA;
    }

    public class CustomerInputOutputFileFormat : Product.ProductInputOutputFileFormat
    {
        private CustomerShareVariables m_CustomerShareVariables;

        public CustomerShareVariables customerShareVariables
        {
            set
            {
                m_CustomerShareVariables = value;
            }
        }

        public int GenerateSummary(string FolderName, string FileName, Product.BinInfo binInfo, ref Product.Input_Product_Info InputProductInfo, DateTime startDateTime, DateTime endDateTime, Product.OutputFileOption outputFileoption, string MachineID, List<string>ListOfLotID,string CurrentLotID, int OutputPlaceQuantity, int CurrentTrayNo, string OutputLotID)
        {
            int nError = 0;
            try
            {
                CustomerSummary summary = new CustomerSummary();

                int nTotalDefectQuantity = 0;
                int nDefectQuantityTemp = 0;
                int nInputEmptyQuantity = 0;
                int nPassQuantity = 0;
                int nInputTiltedQuantity = 0;
                int nTotalEmptyQuantity = 0;
                int nTotalTiltedQuantity = 0;
                int nOutputQuantity = 0;
                int nRejectQuantity = 0;
                int nMissingOutputQuantity = 0;
                int nMissingRejectQuantity = 0;
                int nInputQuantity = 0;
                int nLossUnitQuantity = 0;
                int nInputOtherDefectsQuantity = 0;

                string OutputID = "";

                List<Product.DefectProperty> listDefectProperty = new List<Product.DefectProperty>();
                List<Product.DefectQuantity> listCurrentDefectQuantity = new List<Product.DefectQuantity>();
                CustomerFrameOrTileDefect frameOrTileDefectCurrent = new CustomerFrameOrTileDefect();

                if (SortDefectList(outputFileoption.listDefect, out listDefectProperty) != 0)
                    return 1;

                m_CustomerShareVariables.TotalInputQuantityByTray = 0;
                m_CustomerShareVariables.TotalOutputQuantityByTray = 0;
                m_CustomerShareVariables.TotalDefectQuantityByTray = 0;
                nTotalDefectQuantity = 0;
                foreach (Product.DefectProperty _defectProperty in listDefectProperty)
                {
                    GetDefectDestinationQuantity(binInfo, outputFileoption, _defectProperty.Code, out nDefectQuantityTemp);
                    listCurrentDefectQuantity.Add(new Product.DefectQuantity { Defect = _defectProperty, DefectQty = nDefectQuantityTemp });
                    nTotalDefectQuantity += nDefectQuantityTemp;
                }

                GetOutputIDName(ListOfLotID, out OutputID);
                frameOrTileDefectCurrent.InputLotID = CurrentLotID;
                frameOrTileDefectCurrent.InputTrayNo = CurrentTrayNo;
                //InputProductInfo.LotIDOutput = OutputID;
                //frameOrTileDefectCurrent.InputQuantity = binInfo.Row_Max * binInfo.Col_Max - binInfo.InputRejectQty;
                frameOrTileDefectCurrent.listDefectQuantity = listCurrentDefectQuantity;

                //GetInputDefectQuantity(binInfo, outputFileoption, "MU", out nDefectQuantityTemp);
                //nInputEmptyQuantity += nDefectQuantityTemp;
                //GetInputDefectQuantity(binInfo, outputFileoption, "OT", out nDefectQuantityTemp);
                //nInputTiltedQuantity += nDefectQuantityTemp;
                GetInputDefectQuantity(binInfo, outputFileoption, "EP", out nDefectQuantityTemp);
                nInputEmptyQuantity += nDefectQuantityTemp;
                //GetInputDefectQuantity(binInfo, outputFileoption, "UP", out nDefectQuantityTemp);
                //nInputTiltedQuantity += nDefectQuantityTemp;
                GetInputDefectQuantity(binInfo, outputFileoption, "2DID", out nDefectQuantityTemp);
                nInputTiltedQuantity += nDefectQuantityTemp;

                GetInputOthersDefectQuantity(binInfo, out nDefectQuantityTemp);
                nInputOtherDefectsQuantity += nDefectQuantityTemp;

                GetDefectQuantity(binInfo, outputFileoption, "MU", out nDefectQuantityTemp);
                nTotalEmptyQuantity += nDefectQuantityTemp;
                GetDefectQuantity(binInfo, outputFileoption, "EP", out nDefectQuantityTemp);
                nTotalEmptyQuantity += nDefectQuantityTemp;
                GetDefectQuantity(binInfo, outputFileoption, "P", out nPassQuantity);
                GetDefectQuantity(binInfo, outputFileoption, "OT", out nDefectQuantityTemp);
                nTotalTiltedQuantity += nDefectQuantityTemp;
                GetDefectQuantity(binInfo, outputFileoption, "UP", out nDefectQuantityTemp);
                nTotalTiltedQuantity += nDefectQuantityTemp;

                frameOrTileDefectCurrent.UnitRemainOnFrame = 0;
                frameOrTileDefectCurrent.UnitUntest = 0;
                foreach (Product.DefectProperty _defectProperty in listDefectProperty)
                {
                    nDefectQuantityTemp = 0;
                    if (_defectProperty.Destination == "Input")
                    {
                        GetDefectDestinationQuantity(binInfo, outputFileoption, _defectProperty.Code, out nDefectQuantityTemp);
                        //frameOrTileDefectCurrent.UnitRemainOnFrame += nDefectQuantityTemp;
                    }                   
                }
                int nFullTestQty = 0;
                
                GetFullTestQuantity(binInfo, out frameOrTileDefectCurrent.FullTestQuantity);
                frameOrTileDefectCurrent.InputEmptyQuantity = nInputEmptyQuantity;
                frameOrTileDefectCurrent.UnitRemainOnFrame += nInputTiltedQuantity;
                frameOrTileDefectCurrent.InputOtherDefectsQuantity += nInputOtherDefectsQuantity;
                GetDefectQuantity(binInfo, outputFileoption, "UTT", out nDefectQuantityTemp);
                //frameOrTileDefectCurrent.UnitRemainOnFrame += nDefectQuantityTemp;
                frameOrTileDefectCurrent.UnitUntest += nDefectQuantityTemp;
                GetDefectQuantity(binInfo, outputFileoption, null, out nDefectQuantityTemp);
                //frameOrTileDefectCurrent.UnitRemainOnFrame += nDefectQuantityTemp;
                frameOrTileDefectCurrent.UnitUntest += nDefectQuantityTemp;
                //if (frameOrTileDefectCurrent.FullTestQuantity != nFullTestQty)
                //    Machine.LogDisplay.AddLogDisplay("Caution", "Full Test Quantity for new method and old method are different");

                //GetOutputQuantity(binInfo, out nOutputQuantity);
                GetOutputNRejectQuantity(binInfo, out nOutputQuantity, out nRejectQuantity, out nMissingOutputQuantity, out nMissingRejectQuantity);
                GetUnitLossQuantity(binInfo, out nLossUnitQuantity);
                frameOrTileDefectCurrent.OutputQuantity = nOutputQuantity;
                frameOrTileDefectCurrent.RejectTray1Quantity = nRejectQuantity;
                frameOrTileDefectCurrent.OutputMissingQuantity = nMissingOutputQuantity;
                frameOrTileDefectCurrent.RejectMissingQuantity = nMissingRejectQuantity;


                //frameOrTileDefectCurrent.UnitLoss = nTotalEmptyQuantity + nTotalTiltedQuantity - (nInputEmptyQuantity + nInputTiltedQuantity);
                frameOrTileDefectCurrent.UnitLoss = nLossUnitQuantity;
                //1.0.0.0c Thor 20220414
                nInputQuantity = (binInfo.Col_Max * binInfo.Row_Max) - frameOrTileDefectCurrent.UnitUntest
                    - frameOrTileDefectCurrent.InputEmptyQuantity;
                //- (frameOrTileDefectCurrent.InputEmptyQuantity + frameOrTileDefectCurrent.UnitRemainOnFrame);
                //--
                //nFullTestQty = nInputQuantity - (nTotalEmptyQuantity + nTotalTiltedQuantity);
                Machine.DebugLogger.WriteLog($"{ DateTime.Now.ToString("yyyyMMdd HHmmss")} Thor: nInputQuantity= {nInputQuantity}");
                frameOrTileDefectCurrent.InputQuantity = nInputQuantity;
                frameOrTileDefectCurrent.PassQuantity = nPassQuantity;
                //frameOrTileDefectCurrent.CanisterQuantity = nCanisterQuantity;
                //frameOrTileDefectCurrent.PurgeQuantity = nPurgeQuantity;


                m_CustomerShareVariables.TotalInputQuantityByTray = frameOrTileDefectCurrent.InputQuantity;
                m_CustomerShareVariables.TotalDefectQuantityByTray = nTotalDefectQuantity;
                m_CustomerShareVariables.TotalOutputQuantityByTray = frameOrTileDefectCurrent.OutputQuantity;


                if (Directory.Exists(FolderName) == false)
                    Directory.CreateDirectory(FolderName);

                if (File.Exists(FolderName + "\\" + FileName) == false)
                {
                    summary.LotID = OutputLotID;
                    summary.WorkOrder = InputProductInfo.WorkOrder;
                    summary.StartDateTime = startDateTime.ToString("yyyy/MM/dd,HH:mm:ss");                    
                    summary.Recipe = InputProductInfo.Recipe;
                    //summary.PPLot = InputProductInfo.PPLot;
                    summary.OperatorID = InputProductInfo.OperatorID;
                    summary.Shift = InputProductInfo.Shift;
                    summary.MachineID = MachineID;
                    summary.Version = m_CustomerShareVariables.MachineVersion;
                    //summary.BinNo = InputProductInfo.WaferBin;
                    summary.listFrameOrTileDefect = new List<CustomerFrameOrTileDefect>();
                }
                else
                {
                    Machine.DebugLogger.WriteLog($"{ DateTime.Now.ToString("yyyyMMdd HHmmss")} Thor: Get summary");
                    int nErrorGetSummary = GetSummary(FolderName, FileName, outputFileoption.listDefect, out summary);
                    summary.LotID = OutputLotID;
                    if (nErrorGetSummary != 0)
                    {
                        if (nErrorGetSummary == 3)
                        {
                            nError = 2;
                            Machine.EventLogger.WriteLog(string.Format("{0} : Summary file Error, Defect code not tally. ", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            return nError;
                        }
                        else
                        {
                            nError = 2;
                            Machine.EventLogger.WriteLog(string.Format("{0} : Fail to get summary file. Probably the summary file is corrupted", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            return nError;
                        }
                    }
                    
                }
                summary.EndDateTime = endDateTime.ToString("yyyy/MM/dd,HH:mm:ss");
                summary.Theoretical_UPH = m_CustomerShareVariables.Throughput;
                bool bCurrentFrameOrTileExist = false;

                //for (int i = 0; i < summary.listFrameOrTileDefect.Count; i++)
                //{
                //    if (summary.listFrameOrTileDefect[i].InputTrayNo == frameOrTileDefectCurrent.InputTrayNo
                //        && summary.listFrameOrTileDefect[i].InputLotID == frameOrTileDefectCurrent.InputLotID)
                //    {
                //        //summary.listFrameOrTileDefect[i] = frameOrTileDefectCurrent;
                //        bCurrentFrameOrTileExist = true;
                //        break;
                //    }
                //}

                if (bCurrentFrameOrTileExist == false)
                {
                    summary.listFrameOrTileDefect.Add(frameOrTileDefectCurrent);
                }

                if (WriteSummary(FolderName, FileName, summary, false) != 0)
                {
                    nError = 3;
                    Machine.EventLogger.WriteLog(string.Format("{0} : Fail to write summary file.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                    return nError;
                }

                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        public int RenameFileAndFolder(string OuptutPath, string OldOutputID, string NewOutputID)
        {
            int nError = 0;
            bool bRetry = true;
            try
            {
                if (OldOutputID == NewOutputID)
                {
                    bRetry = false;
                    return nError;
                }
                if (File.Exists(OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Summary-" + OldOutputID + ".txt"))
                {
                    File.Move(OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Summary-" + OldOutputID + ".txt", OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Summary-" + NewOutputID + ".txt");
                }
                if (File.Exists(OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Map" + DateTime.Now.ToString("yyyyMMdd") + "-" + OldOutputID + ".csv"))
                {
                    File.Move(OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Map" + DateTime.Now.ToString("yyyyMMdd") + "-" + OldOutputID + ".csv", OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Map" + DateTime.Now.ToString("yyyyMMdd") + "-" + NewOutputID + ".csv");
                }
                if (Directory.Exists(OuptutPath + "\\Output\\" + OldOutputID))
                {
                    Directory.Move(OuptutPath + "\\Output\\" + OldOutputID, OuptutPath + "\\Output\\" + NewOutputID);
                }
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
                    RenameFileAndFolder(OuptutPath, OldOutputID, NewOutputID);
                }
            }
        }

        public int RenameFileAndFolder(string OuptutPath, string OldOutputID, string NewOutputID, bool RenameFolder, DateTime LotDateTime)
        {
            int nError = 0;
            bool bRetry = true;
            try
            {
                if (OldOutputID == NewOutputID)
                {
                    bRetry = false;
                    return nError;
                }
                if (File.Exists(OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Summary-" + OldOutputID + ".txt"))
                {
                    File.Move(OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Summary-" + OldOutputID + ".txt", OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Summary-" + NewOutputID + ".txt");
                }
                if (File.Exists(OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Map-" + LotDateTime.ToString("yyyyMMdd") + "-" + OldOutputID  + ".csv"))
                {
                    File.Move(OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Map-" + LotDateTime.ToString("yyyyMMdd") + "-" + OldOutputID + ".csv", OuptutPath + "\\Output\\" + OldOutputID + "\\" + "Map-" + DateTime.Now.ToString("yyyyMMdd") + "-" + NewOutputID + ".csv");
                }
                if (Directory.Exists(OuptutPath + "\\Output\\" + OldOutputID) && RenameFolder)
                {
                    Directory.Move(OuptutPath + "\\Output\\" + OldOutputID, OuptutPath + "\\Output\\" + NewOutputID);
                }
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
                    RenameFileAndFolder(OuptutPath, OldOutputID, NewOutputID);
                }
            }
        }
        private int SortDefectList(List<Product.DefectProperty> listDefectProperty, out List<Product.DefectProperty> listSortedDefectProperty)
        {
            int nError = 0;

            listSortedDefectProperty = new List<Product.DefectProperty>();
            try
            {
                int nNoMax = 0;

                //Find the highest defect no
                foreach (Product.DefectProperty _defect in listDefectProperty)
                {
                    if (_defect.No > nNoMax)
                    {
                        nNoMax = _defect.No;
                    }
                }

                //Re-arrange accoding to defect no
                for (int i = 1; i <= nNoMax; i++)
                {
                    foreach (Product.DefectProperty _defectProperty in listDefectProperty)
                    {
                        if (_defectProperty.No == i)
                        {
                            //Skip repeat defect code
                            bool bRepeatDefectCode = false;
                            foreach (Product.DefectProperty _defect1 in listDefectProperty)
                            {
                                if (_defectProperty.Code == _defect1.Code && _defectProperty.No > _defect1.No)
                                {
                                    bRepeatDefectCode = true;
                                    break;
                                }
                            }
                            if (bRepeatDefectCode == false)
                            {
                                listSortedDefectProperty.Add(_defectProperty);
                            }
                            break;
                        }
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        public int GetOutputIDName(List<string> LotIDName, out string OutputName)
        {
            string[] LotIDAfterSplit;
            int nError = 0;
            int nCurrentDay = 0;
            int nMinDay = 1000;
            int nMaxDay = 0;
            string Tool = "";
            string Cav = "";
            int Year = 0;
            int Month = 0;
            OutputName = "";
            try
            {

                if (LotIDName[0].Contains("_") == true)
                {
                    LotIDAfterSplit = LotIDName[0].Split('_');
                    if (LotIDAfterSplit[1].Length > 6)
                    {
                        Tool = LotIDAfterSplit[1].Substring(0, 1);
                        Cav = LotIDAfterSplit[1].Substring(1, 1);
                        if (int.TryParse(LotIDAfterSplit[1].Substring(2, 1), out Year) == false)
                        {
                            OutputName = LotIDName[0];
                            return 0;
                        }
                        if (int.TryParse(LotIDAfterSplit[1].Substring(3, 1), out Month) == false)
                        {
                            OutputName = LotIDName[0];
                            return 0;
                        }
                        foreach (var lotIDName in LotIDName)
                        {
                            LotIDAfterSplit = lotIDName.Split('_');
                            if (LotIDAfterSplit[1].Substring(1, 1) != Cav)
                            {
                                return 1;
                            }
                            nCurrentDay = int.Parse(LotIDAfterSplit[1].Substring(4, 2));
                            if (nCurrentDay < nMinDay)
                            {
                                nMinDay = nCurrentDay;
                            }
                            if (nCurrentDay > nMaxDay)
                            {
                                nMaxDay = nCurrentDay;
                            }
                        }
                        if ((nMaxDay - nMinDay) > 2)
                        {
                            return 2;
                        }
                        OutputName = LotIDAfterSplit[0] + "_" + Tool + Cav + Year + Month + nMinDay + nMaxDay;
                    }
                    else
                    {
                        OutputName = LotIDName[0];
                    }
                }
                else
                {
                    OutputName = LotIDName[0];
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return nError;
        }
        private int GetDefectQuantity(Product.BinInfo mappingInfo, Product.OutputFileOption outputFileOption, string DefectCode, out int defectQuantity)
        {
            int nError = 0;
            defectQuantity = 0;
            try
            {
                for (int i = 0; i < mappingInfo.Row_Max * mappingInfo.Col_Max; i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].BinCode != null)
                    {
                        if (mappingInfo.arrayUnitInfo[i].BinCode == DefectCode)
                        {
                            defectQuantity++;
                        }
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }
        private int GetDefectDestinationQuantity(Product.BinInfo mappingInfo, Product.OutputFileOption outputFileOption, string DefectCode, out int defectQuantity)
        {
            int nError = 0;
            defectQuantity = 0;
            try
            {
                for (int i = 0; i < mappingInfo.Row_Max * mappingInfo.Col_Max; i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].BinCode != null)
                    {
                        if (mappingInfo.arrayUnitInfo[i].BinCode.Length >= 2)
                        {
                            //if (mappingInfo.arrayUnitInfo[i].BinCode.Substring(0, 2).Contains(DefectCode) == true)
                            if (mappingInfo.arrayUnitInfo[i].BinCode == DefectCode)
                            {
                                defectQuantity++;
                            }
                        }
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }
        private int GetInputDefectQuantity(Product.BinInfo mappingInfo, Product.OutputFileOption outputFileOption, string DefectCode, out int defectQuantity)
        {
            int nError = 0;
            defectQuantity = 0;
            try
            {
                for (int i = 0; i < mappingInfo.Row_Max * mappingInfo.Col_Max; i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].InputResult != null)
                    {
                        if (mappingInfo.arrayUnitInfo[i].InputResult == DefectCode)//&& mappingInfo.arrayUnitInfo[i].BinCode == DefectCode)
                        {
                            defectQuantity++;
                        }
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        private int GetInputOthersDefectQuantity(Product.BinInfo mappingInfo, out int defectQuantity)
        {
            int nError = 0;
            defectQuantity = 0;
            try
            {
                for (int i = 0; i < mappingInfo.Row_Max * mappingInfo.Col_Max; i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].OutputTrayType == "INPUT" && mappingInfo.arrayUnitInfo[i].InputResult != null)
                    {
                        if (mappingInfo.arrayUnitInfo[i].InputResult != "2DID" &&
                            mappingInfo.arrayUnitInfo[i].InputResult != "EP")//&& mappingInfo.arrayUnitInfo[i].BinCode == DefectCode)
                        {
                            defectQuantity++;
                        }
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        private int GetOutputQuantity(Product.BinInfo mappingInfo, out int OutputQuantity)
        {
            int nError = 0;
            OutputQuantity = 0;
            try
            {
                for (int i = 0; i < mappingInfo.Row_Max * mappingInfo.Col_Max; i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].OutputTrayType != null)
                    {
                        if (mappingInfo.arrayUnitInfo[i].OutputTrayType == "Output")
                        {
                            OutputQuantity++;
                        }
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        private int GetOutputNRejectQuantity(Product.BinInfo mappingInfo, out int OutputQuantity, out int RejectQuantity, out int MissingOutputQuantity, out int MissingRejectQuantity)
        {
            int nError = 0;
            OutputQuantity = 0;
            RejectQuantity = 0;
            MissingOutputQuantity = 0;
            MissingRejectQuantity = 0;
            try
            {
                for (int i = 0; i < mappingInfo.Row_Max * mappingInfo.Col_Max; i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].OutputTrayType != null)
                    {
                        if (mappingInfo.arrayUnitInfo[i].OutputTrayType == "OUTPUT")
                        {
                            OutputQuantity++;
                            if (mappingInfo.arrayUnitInfo[i].OutputResult != "P")
                            {
                                MissingOutputQuantity++;
                            }    
                        }
                        else if (mappingInfo.arrayUnitInfo[i].OutputTrayType == "REJECT" )
                        {
                            RejectQuantity++;
                            if ( mappingInfo.arrayUnitInfo[i].RejectResult != "P")
                            {
                                MissingRejectQuantity++;
                            }                                                    
                        }
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        private int GetUnitLossQuantity(Product.BinInfo mappingInfo, out int OutputQuantity)
        {
            int nError = 0;
            OutputQuantity = 0;
            try
            {
                for (int i = 0; i < mappingInfo.Row_Max * mappingInfo.Col_Max; i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].SetupResult == "MU" ||
                        mappingInfo.arrayUnitInfo[i].S1Result == "MU" ||
                        mappingInfo.arrayUnitInfo[i].S2Result == "MU" ||
                        mappingInfo.arrayUnitInfo[i].S3Result == "MU"
                        )
                    {
                        OutputQuantity++;                        
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }

        }

        private int GetInputQuantity(Product.BinInfo mappingInfo, out int InputQuantity)
        {
            int nError = 0;
            InputQuantity = 0;
            try
            {
                for (int i = 0; i < mappingInfo.Row_Max * mappingInfo.Col_Max; i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].InputSequenceNumber > 0 && mappingInfo.arrayUnitInfo[i].InputResult != "E")
                    {
                        InputQuantity++;
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        private int GetFullTestQuantity(Product.BinInfo mappingInfo, out int fullTestQuantity)
        {
            int nError = 0;
            fullTestQuantity = 0;
            try
            {
                for (int i = 0; i < mappingInfo.Row_Max * mappingInfo.Col_Max; i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].InputResult != null && mappingInfo.arrayUnitInfo[i].InputResult != "" && mappingInfo.arrayUnitInfo[i].InputResult != "UTT" && mappingInfo.arrayUnitInfo[i].InputResult != "MU" && mappingInfo.arrayUnitInfo[i].InputResult != "OT" && mappingInfo.arrayUnitInfo[i].InputResult != "EP" && mappingInfo.arrayUnitInfo[i].InputResult != "UP"
                        && mappingInfo.arrayUnitInfo[i].S2Result != null && mappingInfo.arrayUnitInfo[i].S2Result != "" && mappingInfo.arrayUnitInfo[i].S2Result != "UTT" && mappingInfo.arrayUnitInfo[i].S2Result != "MU" && mappingInfo.arrayUnitInfo[i].S2Result != "EP"
                         //&& mappingInfo.arrayUnitInfo[i].S1Result != null && mappingInfo.arrayUnitInfo[i].S1Result != "" && mappingInfo.arrayUnitInfo[i].S1Result != "UTT" && mappingInfo.arrayUnitInfo[i].S1Result != "MU"
                         && mappingInfo.arrayUnitInfo[i].SetupResult != null && mappingInfo.arrayUnitInfo[i].SetupResult != "" && mappingInfo.arrayUnitInfo[i].SetupResult != "UTT" && mappingInfo.arrayUnitInfo[i].SetupResult != "MU"
                         //&& mappingInfo.arrayUnitInfo[i].SWLeftResult != null && mappingInfo.arrayUnitInfo[i].SWLeftResult != "" && mappingInfo.arrayUnitInfo[i].SWLeftResult != "UTT" && mappingInfo.arrayUnitInfo[i].SWLeftResult != "MU"
                         //&& mappingInfo.arrayUnitInfo[i].SWRightResult != null && mappingInfo.arrayUnitInfo[i].SWRightResult != "" && mappingInfo.arrayUnitInfo[i].SWRightResult != "UTT" && mappingInfo.arrayUnitInfo[i].SWRightResult != "MU" 
                         //&& mappingInfo.arrayUnitInfo[i].S3Result != null && mappingInfo.arrayUnitInfo[i].S3Result != "" && mappingInfo.arrayUnitInfo[i].S3Result != "UTT" && mappingInfo.arrayUnitInfo[i].S3Result != "MU"
                         //&& mappingInfo.arrayUnitInfo[i].SWFrontResult != null && mappingInfo.arrayUnitInfo[i].SWFrontResult != "" && mappingInfo.arrayUnitInfo[i].SWFrontResult != "UTT" && mappingInfo.arrayUnitInfo[i].SWFrontResult != "MU"
                         //&& mappingInfo.arrayUnitInfo[i].SWRearResult != null && mappingInfo.arrayUnitInfo[i].SWRearResult != "" && mappingInfo.arrayUnitInfo[i].SWRearResult != "UTT" && mappingInfo.arrayUnitInfo[i].SWRearResult != "MU"
                         //&& mappingInfo.arrayUnitInfo[i].OutputResult != null && mappingInfo.arrayUnitInfo[i].OutputResult != "" && mappingInfo.arrayUnitInfo[i].OutputResult != "UTT" && mappingInfo.arrayUnitInfo[i].OutputResult != "MU"
                         )
                    {
                        fullTestQuantity++;
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        private int GetSummary(string FolderName, string FileName, List<Product.DefectProperty> listDefectProperty, out CustomerSummary summary)
        {
            int nError = 0;
            summary = new CustomerSummary();

            try
            {
                List<Product.DefectProperty> listSortedDefectProperty = new List<Product.DefectProperty>();
                List<Product.DefectQuantity> listCurrentDefectQuantity = new List<Product.DefectQuantity>();
                CustomerFrameOrTileDefect frameOrTileDefectCurrent = new CustomerFrameOrTileDefect();

                SortDefectList(listDefectProperty, out listSortedDefectProperty);

                string strSummaryAllLine = "";
                string[] strArraySummaryLine;
                string[] strArraySummaryData;
                int nSummaryLineNo = 0;
                List<Product.DefectQuantity> defectQuantity = new List<Product.DefectQuantity>();
                summary.listFrameOrTileDefect = new List<CustomerFrameOrTileDefect>();
                using (StreamReader srSummary = new StreamReader(FolderName + "\\" + FileName))
                {
                    strSummaryAllLine = srSummary.ReadToEnd();
                    strArraySummaryLine = Regex.Split(strSummaryAllLine, "\r\n");
                    foreach (string _strSummaryLine in strArraySummaryLine)
                    {
                        nSummaryLineNo++;
                        if (nSummaryLineNo == 3)
                        {
                            if (_strSummaryLine.Contains(" "))
                            {
                                strArraySummaryData = _strSummaryLine.Split(' ');
                                if (strArraySummaryData.Length >= 1)
                                {
                                    summary.LotID = strArraySummaryData[2];
                                }
                            }
                            else
                            {
                                string strHeader = "LOT ID: ";
                                if (strHeader.Length > 0)
                                    summary.LotID = _strSummaryLine.Substring(strHeader.Length);
                                //summary.LotID = _strSummaryLine.Substring(0, _strSummaryLine.IndexOf("LOT ID: "));
                            }
                        }
                        else if (nSummaryLineNo == 5)
                        {
                            strArraySummaryData = _strSummaryLine.Split(' ');
                            if (strArraySummaryData.Length >= 1)
                            {
                                summary.StartDateTime = strArraySummaryData[2];
                            }
                        }
                        else if (nSummaryLineNo == 7)
                        {
                            strArraySummaryData = _strSummaryLine.Split(' ');
                            if (strArraySummaryData.Length >= 1)
                            {
                                summary.Recipe = strArraySummaryData[1];
                            }
                        }
                        else if (nSummaryLineNo == 8)
                        {
                            strArraySummaryData = _strSummaryLine.Split(' ');
                            if (strArraySummaryData.Length >= 1)
                            {
                                summary.OperatorID = strArraySummaryData[2];
                            }
                        }
                        else if (nSummaryLineNo == 9)
                        {
                            strArraySummaryData = _strSummaryLine.Split(' ');
                            if (strArraySummaryData.Length >= 1)
                            {
                                summary.Shift = strArraySummaryData[1];
                            }
                        }
                        else if (nSummaryLineNo == 10)
                        {
                            strArraySummaryData = _strSummaryLine.Split(' ');
                            if (strArraySummaryData.Length >= 1)
                            {
                                summary.MachineID = strArraySummaryData[2];
                            }
                        }
                        else if (nSummaryLineNo == 11)
                        {
                            strArraySummaryData = _strSummaryLine.Split(' ');
                            if (strArraySummaryData.Length >= 1)
                            {
                                summary.Version = strArraySummaryData[1];
                            }
                        }
                        //else if (nSummaryLineNo == 11)
                        //{
                        //    int nDataNo = 3;

                        //    List<Product.DefectQuantity> listDefectQuantityTemp = new List<Product.DefectQuantity>();
                        //    if (_strSummaryLine.Contains(" "))
                        //    {
                        //        strArraySummaryData = _strSummaryLine.Split(' ');
                        //    }
                        //    else
                        //    {
                        //        string[] strArraySeparator = { " " };
                        //        strArraySummaryData = _strSummaryLine.Split(strArraySeparator, StringSplitOptions.RemoveEmptyEntries);
                        //    }
                        //    //double check previous data content
                        //    if (strArraySummaryData.Length >= 4)
                        //    {
                        //        //if (strArraySummaryData.Length != listSortedDefectProperty.Count + 5)
                        //        //    return 3;

                        //        foreach (Product.DefectProperty _defectProperty in listSortedDefectProperty)
                        //        {
                        //            if (_defectProperty.Destination != strArraySummaryData[nDataNo])
                        //                return 3;
                        //            nDataNo++;
                        //        }
                        //    }
                        //}
                        else if (nSummaryLineNo > 13)
                        {
                            Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} _strSummaryLine = {_strSummaryLine}");
                            if (_strSummaryLine.Contains(" "))
                            {
                                string[] strArraySeparator = { " " };
                                strArraySummaryData = _strSummaryLine.Split(strArraySeparator, StringSplitOptions.RemoveEmptyEntries);
                               
                            }
                            else
                            {
                                strArraySummaryData = _strSummaryLine.Split(' ');
                            }
                            int nDataNo = 4;

                            int nTemp = 0;
                            CustomerFrameOrTileDefect frameOrTileDefectTemp = new CustomerFrameOrTileDefect();

                            string[] checkSummaryLine = _strSummaryLine.Split(' ');
                            if (checkSummaryLine[0] != string.Empty)
                            {
                                //if (strArraySummaryData.Length >= 7)
                                if (strArraySummaryData.Length >= 10)
                                {
                                    if (strArraySummaryData[0] == "")
                                        continue;
                                    //frameOrTileDefectTemp.FrameOrTileID = strArraySummaryData[0];
                                    //if (Int32.TryParse(strArraySummaryData[1], out nTemp) == true)
                                    //{
                                    //    frameOrTileDefectTemp.InputQuantity = nTemp;
                                    //}
                                    //else
                                    //{
                                    //    return 3;
                                    //}
                                    //for (int i = 0; i < strArraySummaryData.Length; i++)
                                    //{
                                    //    Machine.DebugLogger.WriteLog($"{ DateTime.Now.ToString("yyyyMMdd HHmmss")} Thor: strArraySummaryData[{i}]= {strArraySummaryData[i]}");
                                    //}
                                    frameOrTileDefectTemp.InputLotID = strArraySummaryData[0];
                                    if (Int32.TryParse(strArraySummaryData[1], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.InputTrayNo = nTemp;
                                    }
                                    else
                                    {
                                        return 3;
                                    }
                                    
                                    if (Int32.TryParse(strArraySummaryData[2], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.InputQuantity = nTemp;
                                    }
                                    else
                                    {
                                        return 3;
                                    }
                                    if (Int32.TryParse(strArraySummaryData[3], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.InputEmptyQuantity = nTemp;
                                    }
                                    else
                                    {
                                        return 3;
                                    }
                                    //if (Int32.TryParse(strArraySummaryData[4], out nTemp) == true)
                                    //{
                                    //    frameOrTileDefectTemp.UnitRemainOnFrame = nTemp;
                                    //}
                                    //else
                                    //{
                                    //    return 3;
                                    //}
                                    frameOrTileDefectTemp.listDefectQuantity = new List<Product.DefectQuantity>();
                                    foreach (Product.DefectProperty _defectProperty in listSortedDefectProperty)
                                    {
                                        if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                        {
                                            frameOrTileDefectTemp.listDefectQuantity.Add(new Product.DefectQuantity { Defect = _defectProperty, DefectQty = nTemp });
                                            if (_defectProperty.Code == "2DID")
                                            {
                                                frameOrTileDefectTemp.UnitRemainOnFrame = nTemp;
                                            }
                                        }
                                        nDataNo++;
                                    }
                                    //nDataNo++;
                                    if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.RejectTray1Quantity = nTemp;
                                        nDataNo++;
                                    }
                                    if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.RejectMissingQuantity = nTemp;
                                        nDataNo++;
                                    }
                                    if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.PassQuantity = nTemp;
                                        nDataNo++;
                                    }
                                    //if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                    //{
                                    //    frameOrTileDefectTemp.FullTestQuantity = nTemp;
                                    //    nDataNo++;
                                    //}
                                    nDataNo++;
                                    if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.InputOtherDefectsQuantity = nTemp;
                                        nDataNo++;
                                    }
                                    if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.UnitLoss = nTemp;
                                        nDataNo++;
                                    }
                                    //nDataNo++;
                                    if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.OutputQuantity = nTemp;
                                        nDataNo++;
                                    }
                                    //if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                    //{
                                    //    frameOrTileDefectTemp.InputEmptyQuantity = nTemp;
                                    //    nDataNo++;
                                    //}
                                    if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                                    {
                                        frameOrTileDefectTemp.OutputMissingQuantity = nTemp;
                                        nDataNo++;
                                    }
                                    summary.listFrameOrTileDefect.Add(frameOrTileDefectTemp);
                                }
                            }

                            //if (strArraySummaryData.Length >= 5)
                            //{
                            //    if (strArraySummaryData[0] == "")
                            //        continue;
                            //    frameOrTileDefectTemp.FrameOrTileID = strArraySummaryData[0];
                            //    if (Int32.TryParse(strArraySummaryData[1], out nTemp) == true)
                            //    {
                            //        frameOrTileDefectTemp.InputQuantity = nTemp;
                            //    }
                            //    else
                            //    {
                            //        return 3;
                            //    }
                            //    frameOrTileDefectTemp.listDefectQuantity = new List<DefectQuantity>();
                            //    foreach (DefectProperty _defectProperty in listSortedDefectProperty)
                            //    {
                            //        if (Int32.TryParse(strArraySummaryData[nDataNo], out nTemp) == true)
                            //        {
                            //            frameOrTileDefectTemp.listDefectQuantity.Add(new DefectQuantity { Defect = _defectProperty, DefectQty = nTemp });
                            //        }
                            //        nDataNo++;
                            //    }

                            //    summary.listFrameOrTileDefect.Add(frameOrTileDefectTemp);
                            //}
                        }
                    }
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        private int WriteSummary(string folder, string filename, CustomerSummary summary, bool selectedDefectOnly)
        {
            int nError = 0;

            try
            {
                int nTotalReject = 0;
                int nTotalInputQty = 0;
                int nTotalInputOtherDefectsQty = 0;
                int nTotalOutputQty = 0;
                int nTotalAbnormalUnitAtInput = 0;
                int nTotalLossUnitDuringTransfer = 0;
                int nTotalRejectInFrameOrTile = 0;
                int nTotalCanisterQty = 0;
                int nTotalPurgeQty = 0;
                int nTotalPassQty = 0;
                int nTotalEmpty = 0;
                int nTotalUnpick = 0;
                string strDefectHeader = "";
                string strDefectData = "";
                string strDefectPercentage = "";
                string strTotalDefectQuantity = "";
                string strTotalEmptyPercentage = "";
                int nPadLeftWidth = 8;
                int nTotalOutputMissingUnits = 0;
                int nTotalRejectMissingUnits = 0;

                List<Product.DefectQuantity> listDefectQuantityTotal = new List<Product.DefectQuantity>();

                int nTotalPassWithOptionSummary = 0;
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyName assemblyName = assembly.GetName();

                using (StreamWriter writeSummary = new StreamWriter(folder + "\\" + filename))
                {
                    //Summary Header
                    writeSummary.WriteLine("ELS1200 SUMMARY REPORT");
                    writeSummary.WriteLine("------------------------------------------------------------------------------------------------------------------");
                    writeSummary.WriteLine($"OUTPUT LOT ID: {summary.LotID}");
                    writeSummary.WriteLine("------------------------------------------------------------------------------------------------------------------");
                    writeSummary.WriteLine($"START DATE/TIME: {summary.StartDateTime}");
                    writeSummary.WriteLine($"END DATE/TIME: {summary.EndDateTime}");
                    writeSummary.WriteLine($"RECIPE: {summary.Recipe}", summary.StartDateTime, summary.Recipe);
                    writeSummary.WriteLine($"OPERATOR ID: {summary.OperatorID}");
                    writeSummary.WriteLine($"SHIFT: {summary.Shift}");
                    writeSummary.WriteLine($"MACHINE ID: {summary.MachineID}");
                    writeSummary.WriteLine($"Version: {summary.Version}");
                    writeSummary.WriteLine("");

                    if (summary.listFrameOrTileDefect.Count == 0)
                    {
                        nError = 1;
                        return nError;
                    }

                    //Check tile id max length
                    int nMaxBarcodeIDLength = 7;
                    //int nMaxBarcodeIDLength = 0;
                    int nMaxOutputBarcodeIDLength = 7;
                    foreach (CustomerFrameOrTileDefect _listFrameOrTileDefect in summary.listFrameOrTileDefect)
                    {
                        if (_listFrameOrTileDefect.InputLotID.Length > nMaxBarcodeIDLength)
                            nMaxBarcodeIDLength = _listFrameOrTileDefect.InputLotID.Length;
                    }
                    nMaxBarcodeIDLength += 3;
                    //foreach (CustomerFrameOrTileDefect _listFrameOrTileDefect in summary.listFrameOrTileDefect)
                    //{
                    //    if (_listFrameOrTileDefect.OutputFrameOrTileID.Length > nMaxOutputBarcodeIDLength)
                    //        nMaxOutputBarcodeIDLength = _listFrameOrTileDefect.OutputFrameOrTileID.Length;
                    //}
                    nMaxOutputBarcodeIDLength += 3;
                    foreach (CustomerFrameOrTileDefect _listFrameOrTileDefect in summary.listFrameOrTileDefect)
                    {
                        foreach (Product.DefectQuantity _defectQuantity in _listFrameOrTileDefect.listDefectQuantity)
                        {
                            //if ((_defectQuantity.Defect.EnableInReport == true && selectedDefectOnly == true) || selectedDefectOnly == false)
                            {
                                //if ((_defectQuantity.Defect.Code).Length > nPadLeftWidth)
                                //    nPadLeftWidth = (_defectQuantity.Defect.Code).Length;
                                //strDefectHeader += ((_defectQuantity.Defect.Code)/*.PadLeft(nPadLeftWidth)*/ + "\t");
                                strDefectHeader += string.Format("{0,-15}", _defectQuantity.Defect.Code);
                            }
                        }
                        //writeSummary.WriteLine(string.Format("{0,-11}{1,-12}", _listFrameOrTileDefect.FrameOrTileID, "InputQty") + strDefectHeader + string.Format("{0,-14}{1,-12}{2:F2,-6}", nTotalRejectInFrameOrTile, _listFrameOrTileDefect.InputQuantity - nTotalRejectInFrameOrTile, (double)(_listFrameOrTileDefect.InputQuantity - nTotalRejectInFrameOrTile) / (double)_listFrameOrTileDefect.InputQuantity));
                        break;
                    }
                    writeSummary.WriteLine(string.Format("{0,-" + nMaxBarcodeIDLength + "}{1,-11}{2,-12}{3,-10}", "InputID", "TrayNo", "InputQty","EP") 
                        + strDefectHeader + string.Format("{0,-14}{1,-23}{2,-10}{3,-22}{4,-23}{5,-18}{6,-22}{7,-23}", "TotalReject", "RejectMissingUnitQty", "PassQty", "PerformanceYield(%)", "OtherInputDefectQty","OtherLossQty", "OutputPlaceUnitQty", "OutputMissingUnitQty"));
                    
                    foreach (CustomerFrameOrTileDefect _listFrameOrTileDefect in summary.listFrameOrTileDefect)
                    {
                        nTotalPassWithOptionSummary = 0;
                        nTotalRejectInFrameOrTile = 0;
                        strDefectData = "";
                        
                        foreach (Product.DefectQuantity _defectQuantity in _listFrameOrTileDefect.listDefectQuantity)
                        {
                            //if ((_defectQuantity.Defect.EnableInReport == true && selectedDefectOnly == true) || selectedDefectOnly == false)
                            {
                                //if ((_defectQuantity.Defect.Code).Length > nPadLeftWidth)
                                //    nPadLeftWidth = (_defectQuantity.Defect.Code).Length;
                                //strDefectData += (_defectQuantity.DefectQty.ToString()/*.PadLeft(nPadLeftWidth)*/ + "\t");
                                strDefectData += string.Format("{0,-15}", _defectQuantity.DefectQty.ToString());
                                nTotalRejectInFrameOrTile += _defectQuantity.DefectQty;
                            }
                            //else if (_defectQuantity.Defect.EnableInReport == false && selectedDefectOnly == true)
                            {
                            //    nTotalPassWithOptionSummary += _defectQuantity.DefectQty;
                            }
                            if (_defectQuantity.Defect.Destination == "OUTPUT")
                            {
                                if (_defectQuantity.Defect.Code == "MU")
                                {
                                    nTotalOutputMissingUnits = _defectQuantity.DefectQty;
                                }
                            }
                        }
                        //nTotalReject += nTotalRejectInFrameOrTile - nTotalOutputMissingUnits;
                        nTotalInputQty += _listFrameOrTileDefect.InputQuantity;
                        nTotalAbnormalUnitAtInput += _listFrameOrTileDefect.UnitRemainOnFrame;
                        nTotalLossUnitDuringTransfer += _listFrameOrTileDefect.UnitLoss;
                        nTotalOutputQty += _listFrameOrTileDefect.OutputQuantity;
                        nTotalReject += _listFrameOrTileDefect.RejectTray1Quantity;
                        nTotalOutputMissingUnits += _listFrameOrTileDefect.OutputMissingQuantity;
                        nTotalRejectMissingUnits += _listFrameOrTileDefect.RejectMissingQuantity;
                        nTotalPassQty += _listFrameOrTileDefect.PassQuantity + nTotalPassWithOptionSummary;
                        nTotalEmpty += _listFrameOrTileDefect.InputEmptyQuantity;
                        nTotalUnpick += _listFrameOrTileDefect.UnitRemainOnFrame;
                        nTotalInputOtherDefectsQty += _listFrameOrTileDefect.InputOtherDefectsQuantity;

                        writeSummary.WriteLine(string.Format("{0,-" + nMaxBarcodeIDLength + "}{1,-11}{2,-12}{3,-10}",
                           _listFrameOrTileDefect.InputLotID, _listFrameOrTileDefect.InputTrayNo, _listFrameOrTileDefect.InputQuantity
                           , _listFrameOrTileDefect.InputEmptyQuantity)
                        
                            + strDefectData
                            + string.Format("{0,-14}{1,-23}{2,-10}{3,-22:F2}{4,-23}{5,-18}{6,-22}{7,-23}"
                            //, nTotalRejectInFrameOrTile
                            , _listFrameOrTileDefect.RejectTray1Quantity
                            , _listFrameOrTileDefect.RejectMissingQuantity
                            , _listFrameOrTileDefect.PassQuantity
                            //, _listFrameOrTileDefect.FullTestQuantity
                            //, _listFrameOrTileDefect.InputQuantity - _listFrameOrTileDefect.FullTestQuantity > 0 ? ((double)(_listFrameOrTileDefect.PassQuantity) / (double)(_listFrameOrTileDefect.FullTestQuantity) * 100.0) : 0.0
                            , _listFrameOrTileDefect.InputQuantity > 0 ? ((double)(_listFrameOrTileDefect.PassQuantity) / (double)(_listFrameOrTileDefect.InputQuantity) * 100.0) : 0.0
                            , _listFrameOrTileDefect.InputOtherDefectsQuantity
                            , _listFrameOrTileDefect.UnitLoss
                            //, _listFrameOrTileDefect.InputQuantity > 0 ? (double)(_listFrameOrTileDefect.PassQuantity) / (double)_listFrameOrTileDefect.InputQuantity * 100.0 : 0.0
                           , _listFrameOrTileDefect.OutputQuantity
                           , _listFrameOrTileDefect.OutputMissingQuantity));
                    }

                    //Percentage
                    //Copy one set of defect

                    foreach (Product.DefectQuantity _defectQuantity in summary.listFrameOrTileDefect[0].listDefectQuantity)
                    {
                        listDefectQuantityTotal.Add(_defectQuantity.Clone());
                    }
                    //Reset count to 0
                    for (int i = 0; i < listDefectQuantityTotal.Count; i++)
                    {
                        listDefectQuantityTotal[i].DefectQty = 0;
                    }
                    foreach (CustomerFrameOrTileDefect _listFrameOrTileDefect in summary.listFrameOrTileDefect)
                    {
                        foreach (Product.DefectQuantity _defectQuantity in _listFrameOrTileDefect.listDefectQuantity)
                        {
                            for (int i = 0; i < listDefectQuantityTotal.Count; i++)
                            {
                                if (listDefectQuantityTotal[i].Defect.Code == _defectQuantity.Defect.Code)
                                {
                                    if ((_defectQuantity.Defect.EnableInReport == true && selectedDefectOnly == true) || selectedDefectOnly == false)
                                    {
                                        listDefectQuantityTotal[i].DefectQty += _defectQuantity.DefectQty;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    foreach (Product.DefectQuantity _defectQuantity in listDefectQuantityTotal)
                    {
                        if ((_defectQuantity.Defect.EnableInReport == true && selectedDefectOnly == true) || selectedDefectOnly == false)
                        {
                            if ((_defectQuantity.Defect.Code).Length > nPadLeftWidth)
                                nPadLeftWidth = (_defectQuantity.Defect.Code).Length;
                            //strDefectPercentage += (string.Format("{0:F2}%", (double)_defectQuantity.DefectQty / (double)(nTotalInputQty) * 100.0) + "\t");
                            if (nTotalInputQty == 0)
                            {
                                strDefectPercentage += (string.Format("{0,-15}", string.Format("{0:F2}%", (double)0)));
                                strTotalDefectQuantity += (string.Format("{0,-15}", 0.ToString()));
                            }
                            else
                            {
                                //strDefectPercentage += (string.Format("{0,-10}", string.Format("{0:F2}%", (double)_defectQuantity.DefectQty / (double)(nTotalInputQty + nTotalEmpty + nTotalUnpick) * 100.0)));
                                strDefectPercentage += (string.Format("{0,-15}", string.Format("{0:F2}%", (double)_defectQuantity.DefectQty / (double)(nTotalInputQty) * 100.0)));
                                strTotalDefectQuantity += (string.Format("{0,-15}", _defectQuantity.DefectQty.ToString()));
                            }
                        }
                    }

                    if (nTotalEmpty == 0)
                    {
                        strTotalEmptyPercentage = (string.Format("{0,-10}", string.Format("{0:F2}%", (double)0)));
                    }
                    else
                    {
                        strTotalEmptyPercentage = (string.Format("{0,-10}", string.Format("{0:F2}%", (double)(nTotalEmpty) / (double)(nTotalInputQty + nTotalEmpty + nTotalUnpick) * 100.0)));
                    }
                    //writeSummary.WriteLine(string.Format("{0,-" + (nMaxBarcodeIDLength) + "}{1,-" + (nMaxOutputBarcodeIDLength + 12) + "}{2}", "", "", strDefectPercentage));
                    writeSummary.WriteLine(string.Format("{0,-" + nMaxBarcodeIDLength + "}{1,-11}{2,-12}{3,-10}{4}",
                           "", "", "", "", strTotalDefectQuantity));
                    writeSummary.WriteLine(string.Format("{0,-" + nMaxBarcodeIDLength + "}{1,-11}{2,-12}{3,-10}{4}",
                           "", "", "", "", strDefectPercentage));

                    //Summary Defect Footer
                    writeSummary.WriteLine(Environment.NewLine);
                    writeSummary.WriteLine(Environment.NewLine);
                    writeSummary.WriteLine("TOTAL TRAY(S)                                 {0}", summary.listFrameOrTileDefect.Count);
                    writeSummary.WriteLine("TOTAL INPUT                                   {0}", nTotalInputQty);
                    writeSummary.WriteLine("TOTAL INPUT EMPTY                             {0}", nTotalEmpty);
                    writeSummary.WriteLine("TOTAL INPUT 2DID                              {0}", nTotalUnpick);
                    writeSummary.WriteLine("TOTAL INPUT REJECT (REMAIN ON INPUT TRAY)     {0}", nTotalInputOtherDefectsQty);
                    writeSummary.WriteLine("TOTAL PASS (OUTPUT TRAY)                      {0}", nTotalOutputQty);
                    //writeSummary.WriteLine("TOTAL OUTPUT                        {0}", nTotalOutputQty);
                    writeSummary.WriteLine("TOTAL OUTPUT REJECT (REJECT TRAY)             {0}", nTotalReject);                  
                    //writeSummary.WriteLine("PASS + REJECT                       {0}", nTotalPassQty + nTotalReject);
                    writeSummary.WriteLine("TOTAL HANDLING LOSS                           {0}", nTotalLossUnitDuringTransfer);
                    writeSummary.WriteLine("TOTAL REJECT                                  {0}", nTotalUnpick + nTotalInputOtherDefectsQty + nTotalReject + nTotalLossUnitDuringTransfer);
                    //writeSummary.WriteLine("TOTAL OUTPUT MISSING UNITS          {0}", nTotalOutputMissingUnits);
                    //writeSummary.WriteLine("TOTAL REJECT MISSING UNITS          {0}", nTotalRejectMissingUnits);

                    writeSummary.WriteLine(Environment.NewLine);
                    writeSummary.WriteLine("PASS YIELD                                    {0:F2}%", (double)(nTotalOutputQty) / (double)nTotalInputQty * 100.0);
                    writeSummary.WriteLine("TOSSING YIELD                                 {0:F2}%", (double)(nTotalLossUnitDuringTransfer) / (double)nTotalInputQty * 100.0); 


                    writeSummary.WriteLine(Environment.NewLine);

                    DateTime _start, _end;
                    _start = DateTime.ParseExact(summary.StartDateTime, "yyyy/MM/dd,HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    _end = DateTime.ParseExact(summary.EndDateTime, "yyyy/MM/dd,HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    TimeSpan duration = _end - _start;
                    writeSummary.WriteLine("TOTAL TIME USED                     {0}", $"{duration.Hours} hours, {duration.Minutes} minutes, {duration.Seconds} seconds");
                    writeSummary.WriteLine("UPH                                 {0:F2}", (double)nTotalInputQty/((double)duration.TotalSeconds/3600.0));
                    writeSummary.WriteLine("THROUGHPUT                          {0:F2}", (double)nTotalOutputQty/ ((double)duration.TotalSeconds/ 3600.0));




                    double dblUPH;
                    Double.TryParse(summary.Theoretical_UPH, out dblUPH);
                    //writeSummary.WriteLine("THEORETICAL UPH                     {0}", summary.Theoretical_UPH);

                    //1. Efficiency considering total available time: (Actual Output / (Theoretical UPH * Total Time)) * 100, which factors in both speed and uptime.
                    //writeSummary.WriteLine("EFFICIENCY                          {0:F2}%", (double)(nTotalOutputQty/(dblUPH * duration.TotalHours) * 100.0));

                    double dblStandardTime;
                    dblStandardTime = (nTotalOutputQty + nTotalReject) / dblUPH;


                    //2. Time-based efficiency: (Standard Time / Actual Time) * 100, where Standard Time = Units Produced / Theoretical UPH.
                    //writeSummary.WriteLine("TIME-BASED EFFICIENCY               {0:F2}%", (double)(dblStandardTime/duration.TotalHours * 100.0));

                    m_CustomerShareVariables.LotYield = (double)(nTotalOutputQty) / (double)nTotalInputQty * 100.0;
                    if(m_CustomerShareVariables.IsPassYieldMES == true)
                    {
                        m_CustomerShareVariables.IsPassYieldMES = false;
                        m_CustomerShareVariables.IsPassYieldMESDone = true;
                    }
                    writeSummary.Flush();
                    writeSummary.Close();
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        public int WriteOutputMapFile(string folder, string filename, Product.BinInfo mappingInfo, Product.Input_Product_Info inputProductInfo, string machineName, DateTime startDateTime)
        {
            int nError = 0;
            string strSeperator = ",";
            int LotIDIndex = 0;
            StringBuilder sbOutput = new StringBuilder();
            try
            {
                if (Directory.Exists(folder) == false)
                    Directory.CreateDirectory(folder);
                if (File.Exists(folder + "\\" + filename))
                {
                    LotIDIndex = 0;
                    string[] arrLine = File.ReadAllLines(folder + "\\" + filename);
                    for (int i = 0; i < arrLine.Length; i++)
                    {
                        if (arrLine[i].Contains("OUTPUT LOTID"))
                        {
                            LotIDIndex = i;
                            break;
                        }
                    }
                    string[] word = arrLine[LotIDIndex].Split(':');
                    if (inputProductInfo.LotID != word[1])
                    {
                        arrLine[LotIDIndex] = $"OUTPUT LOTID:{inputProductInfo.LotID}";
                        File.WriteAllLines(folder + "\\" + filename, arrLine);
                    }
                    goto AppendData;
                }
                string[] Header = new string[] { "OUTPUT LOTID:", $"{inputProductInfo.LotID}" };
                sbOutput.AppendLine(string.Join(strSeperator, Header));
                string[] Header2 = new string[] { "RECIPE:", $"{inputProductInfo.Recipe}" };
                sbOutput.AppendLine(string.Join(strSeperator, Header2));
                string[] Header3 = new string[] { "MACHINE ID:", $"{machineName}" };
                sbOutput.AppendLine(string.Join(strSeperator, Header3));
                string[] Header4 = new string[] { "DATE/TIME:", $"{startDateTime}" };
                sbOutput.AppendLine(string.Join(strSeperator, Header4));
                string[] Header5 = new string[] { "OPERATOR ID:", $"{inputProductInfo.OperatorID}" };
                sbOutput.AppendLine(string.Join(strSeperator, Header5));
                string[] Header6 = new string[] { "", "" };
                sbOutput.AppendLine(string.Join(strSeperator, Header6));
                string[] Header7 = new string[] {"InputTrayNo","InputTrayRow",
                "InputTrayColumn","UnitID","OutputTrayType","OutputTrayNo","OutputTrayRow","OutputTrayColumn","BinCode","Pass/Faill",
                    "InputResult","BottomResult","SetupResult","S1Result","S2PartingResult","S3PartingResult","S2Result","S3Result" };
                sbOutput.AppendLine(string.Join(strSeperator, Header7));

                AppendData:;
                string[][] info = new string[mappingInfo.arrayUnitInfo.Count()][];
                for (int i = 0; i < mappingInfo.arrayUnitInfo.Count(); i++)
                {
                    if (mappingInfo.arrayUnitInfo[i].InputTrayNo != 0)
                    {
                        string[] Header8 = new string[] {   $"{mappingInfo.arrayUnitInfo[i].InputTrayNo}",
                                                            $"{mappingInfo.arrayUnitInfo[i].InputRow}",
                                                            $"{mappingInfo.arrayUnitInfo[i].InputColumn}",
                                                            $"{mappingInfo.arrayUnitInfo[i].UnitID}",
                                                            $"{mappingInfo.arrayUnitInfo[i].OutputTrayType}",
                                                            $"{mappingInfo.arrayUnitInfo[i].OutputTrayNo}",
                                                            $"{mappingInfo.arrayUnitInfo[i].OutputRow}",
                                                            $"{mappingInfo.arrayUnitInfo[i].OutputColumn}",
                                                            $"{mappingInfo.arrayUnitInfo[i].BinCode}",
                                                            $"{(mappingInfo.arrayUnitInfo[i].BinCode!="P"?"Fail":"Pass")}",
                                                            $"{mappingInfo.arrayUnitInfo[i].InputResult}",
                                                            $"{mappingInfo.arrayUnitInfo[i].BottomResult}",
                                                            $"{mappingInfo.arrayUnitInfo[i].SetupResult}",
                                                            $"{mappingInfo.arrayUnitInfo[i].S1Result}",
                                                            $"{mappingInfo.arrayUnitInfo[i].S2PartingResult}",
                                                            $"{mappingInfo.arrayUnitInfo[i].S3PartingResult}",
                                                            $"{mappingInfo.arrayUnitInfo[i].S2Result}",
                                                            $"{mappingInfo.arrayUnitInfo[i].S3Result}",
                        };
                        sbOutput.AppendLine(string.Join(strSeperator, Header8));
                    }
                }
                File.AppendAllText((folder + "\\" + filename), sbOutput.ToString());
            }
            catch (Exception ex)
            {
                nError = -1;
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return nError;
        }

        public struct UnitNo
        {
            public int No;
            public int Row;
            public int Column;
        }
       
        List<string> listStrNewData = new List<string>();

        BinInfo vision = new BinInfo();
        BinInfo handler = new BinInfo();
        public struct HandlerReportInfo
        {
            public string LotID;
            public string IRingID;
            public string Vision;
            public string REFDES;
            public int FHNO;
            public int BHNO;
            public int ISEQ;
            public string INPUT;
            public string TOP;
            public string ALN;
            public string BTM;
            public string SWL;
            public string SWR;
            public string SWT;
            public string SWB;
            public string CNT;
            public string PBT;
            public string ORingID;
            public int OSEQ;
            public int OROW;
            public int OCOL;
            public string OUTPUTCODE;
        }

        public struct BinInfo
        {
            public HandlerReportInfo[] arrayUnitInfo;
        }
    }
}
