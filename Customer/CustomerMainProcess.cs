using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Common;
using Machine;

namespace Customer
{
    public class CustomerMainProcess : Product.ProductMainProcess
    {
        private CustomerShareVariables m_CustomerShareVariables;
        private CustomerProcessEvent m_CustomerProcessEvent;

        private CustomerStateControl m_CustomerStateControl;
        private CustomerRTSSProcess m_CustomerRTSSProcess;
        private CustomerReportProcess m_CustomerReportProcess;

        CustomerInputOutputFileFormat readInfo = new CustomerInputOutputFileFormat();
        
        Task m_TaskCopyFile;



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
                base.productStateControl = m_CustomerStateControl;
            }
        }

        public CustomerRTSSProcess customerRTSSProcess
        {
            set
            {
                m_CustomerRTSSProcess = value;
                base.productRTSSProcess = m_CustomerRTSSProcess;
            }
        }

        public CustomerReportProcess customerReportProcess
        {
            set
            {
                m_CustomerReportProcess = value;
                //base.productReportProcess = m_CustomerReportProcess;
            }
        }

        override public int InitializeThread()
        {
            int nError = 0;
            try
            {
                readInfo.customerShareVariables = m_CustomerShareVariables;

                m_CustomerStateControl.customerShareVariables = m_CustomerShareVariables;
                m_CustomerStateControl.customerRTSSProcess = m_CustomerRTSSProcess;
                m_thdStateControl = new Thread(m_CustomerStateControl.StateThread);
                m_thdStateControl.Start(m_CustomerShareVariables.StateMain);
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
            base.OnRunningMainThread();

            if (m_CustomerRTSSProcess.GetEvent("RSEQ_GMAIN_UNLOAD_MATERIAL_START") == true)
            {
                m_CustomerRTSSProcess.SetEvent("RSEQ_GMAIN_UNLOAD_MATERIAL_START", false);
                m_CustomerReportProcess.AddRecord(new ReportProcess.Record { dateTime = DateTime.Now, eventID = m_CustomerShareVariables.customerReportEvent.EventUnloadMaterialTime.EventID, eventName = m_CustomerShareVariables.customerReportEvent.EventUnloadMaterialTime.EventName, lotID = m_CustomerShareVariables.strucInputProductInfo.LotID, alarmID = 0, alarmType = 0 });
            }
            if (m_CustomerRTSSProcess.GetEvent("RSEQ_GMAIN_REJECT_TRAY_EXCHANGE_START") == true)
            {
                m_CustomerRTSSProcess.SetEvent("RSEQ_GMAIN_REJECT_TRAY_EXCHANGE_START", false);
                m_CustomerReportProcess.AddRecord(new ReportProcess.Record { dateTime = DateTime.Now, eventID = m_CustomerShareVariables.customerReportEvent.EventRejectTrayExchangeTime.EventID, eventName = m_CustomerShareVariables.customerReportEvent.EventRejectTrayExchangeTime.EventName, lotID = m_CustomerShareVariables.strucInputProductInfo.LotID, alarmID = 0, alarmType = 0 });
            }
        }

        override public void OnRunningSecondThread()
        {
            base.OnRunningSecondThread();
            
        }

        override public int LoadConfiguration()
        {
            try
            {
                if (Tools.IsFileExist(m_CustomerShareVariables.strSystemPath, m_CustomerShareVariables.strConfigurationFile.Remove(m_CustomerShareVariables.strConfigurationFile.Length - m_CustomerShareVariables.strXmlExtension.Length), m_CustomerShareVariables.strXmlExtension))
                {
                    m_CustomerShareVariables.customerConfigurationSettings = Tools.Deserialize<CustomerConfigurationSettings>(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strConfigurationFile);
                    m_CustomerShareVariables.productConfigurationSettings = m_CustomerShareVariables.customerConfigurationSettings;
                    m_CustomerShareVariables.configurationSettings = m_CustomerShareVariables.customerConfigurationSettings;
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
                if (Tools.IsFileExist(m_CustomerShareVariables.strSystemPath, m_CustomerShareVariables.strOptionFile.Remove(m_CustomerShareVariables.strOptionFile.Length - m_CustomerShareVariables.strXmlExtension.Length), m_CustomerShareVariables.strXmlExtension))
                {
                    m_CustomerShareVariables.customerOptionSettings = Tools.Deserialize<CustomerOptionSettings>(m_CustomerShareVariables.strSystemPath + m_CustomerShareVariables.strOptionFile);
                    m_CustomerShareVariables.productOptionSettings = m_CustomerShareVariables.customerOptionSettings;
                    m_CustomerShareVariables.optionSettings = m_CustomerShareVariables.customerOptionSettings;
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
            if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeInputPath, m_CustomerShareVariables.recipeMainSettings.InputRecipeName, m_CustomerShareVariables.strXmlExtension))
            {
                m_CustomerShareVariables.customerRecipeInputSettings = Tools.Deserialize<CustomerRecipeInputSettings>(m_CustomerShareVariables.strRecipeInputPath + m_CustomerShareVariables.recipeMainSettings.InputRecipeName + m_CustomerShareVariables.strXmlExtension);
                m_CustomerShareVariables.productRecipeInputSettings = m_CustomerShareVariables.customerRecipeInputSettings;
                m_CustomerShareVariables.recipeInputSettings = m_CustomerShareVariables.productRecipeInputSettings;
            }
            else
            {
                nError = 1;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeOutputPath, m_CustomerShareVariables.recipeMainSettings.OutputRecipeName, m_CustomerShareVariables.strXmlExtension))
            {
                m_CustomerShareVariables.customerRecipeOutputSettings = Tools.Deserialize<CustomerRecipeOutputSettings>(m_CustomerShareVariables.strRecipeOutputPath + m_CustomerShareVariables.recipeMainSettings.OutputRecipeName + m_CustomerShareVariables.strXmlExtension);
                m_CustomerShareVariables.productRecipeOutputSettings = m_CustomerShareVariables.customerRecipeOutputSettings;
                m_CustomerShareVariables.recipeOutputSettings = m_CustomerShareVariables.productRecipeOutputSettings;
            }
            else
            {
                nError = 2;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeDelayPath, m_CustomerShareVariables.recipeMainSettings.DelayRecipeName, m_CustomerShareVariables.strXmlExtension))
            {
                m_CustomerShareVariables.customerRecipeDelaySettings = Tools.Deserialize<CustomerRecipeDelaySettings>(m_CustomerShareVariables.strRecipeDelayPath + m_CustomerShareVariables.recipeMainSettings.DelayRecipeName + m_CustomerShareVariables.strXmlExtension);
                m_CustomerShareVariables.productRecipeDelaySettings = m_CustomerShareVariables.customerRecipeDelaySettings;
                m_CustomerShareVariables.recipeDelaySettings = m_CustomerShareVariables.productRecipeDelaySettings;
            }
            else
            {
                nError = 3;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeMotorPositionPath, m_CustomerShareVariables.recipeMainSettings.MotorPositionRecipeName, m_CustomerShareVariables.strXmlExtension))
            {
                m_CustomerShareVariables.customerRecipeMotorPositionSettings = Tools.Deserialize<CustomerRecipeMotorPositionSettings>(m_CustomerShareVariables.strRecipeMotorPositionPath + m_CustomerShareVariables.recipeMainSettings.MotorPositionRecipeName + m_CustomerShareVariables.strXmlExtension);
                m_CustomerShareVariables.productRecipeMotorPositionSettings = m_CustomerShareVariables.customerRecipeMotorPositionSettings;
                m_CustomerShareVariables.recipeMotorPositionSettings = m_CustomerShareVariables.productRecipeMotorPositionSettings;
            }
            else
            {
                nError = 4;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeOutputFilePath, m_CustomerShareVariables.recipeMainSettings.OutputFileRecipeName, m_CustomerShareVariables.strXmlExtension))
            {
                m_CustomerShareVariables.customerRecipeOutputFileSettings = Tools.Deserialize<CustomerRecipeOutputFileSettings>(m_CustomerShareVariables.strRecipeOutputFilePath + m_CustomerShareVariables.recipeMainSettings.OutputFileRecipeName + m_CustomerShareVariables.strXmlExtension);
                m_CustomerShareVariables.productRecipeOutputFileSettings = m_CustomerShareVariables.customerRecipeOutputFileSettings;
                m_CustomerShareVariables.recipeOutputFileSettings = m_CustomerShareVariables.productRecipeOutputFileSettings;
            }
            else
            {
                nError = 5;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            //if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeInputCassettePath, m_CustomerShareVariables.productRecipeMainSettings.InputCassetteRecipeName, m_CustomerShareVariables.strXmlExtension))
            //{
            //    m_CustomerShareVariables.customerRecipeInputCassetteSettings = Tools.Deserialize<CustomerRecipeCassetteSettings>(m_CustomerShareVariables.strRecipeInputCassettePath + m_CustomerShareVariables.productRecipeMainSettings.InputCassetteRecipeName + m_CustomerShareVariables.strXmlExtension);
            //    m_CustomerShareVariables.productRecipeInputCassetteSettings = m_CustomerShareVariables.customerRecipeInputCassetteSettings;
            //}
            //else
            //{
            //    nError = 6;
            //    return nError;
            //    //stripStatusLabel.Text += "Main recipe not exist.";
            //}
            //if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeOutputCassettePath, m_CustomerShareVariables.productRecipeMainSettings.OutputCassetteRecipeName, m_CustomerShareVariables.strXmlExtension))
            //{
            //    m_CustomerShareVariables.customerRecipeOutputCassetteSettings = Tools.Deserialize<CustomerRecipeCassetteSettings>(m_CustomerShareVariables.strRecipeOutputCassettePath + m_CustomerShareVariables.productRecipeMainSettings.OutputCassetteRecipeName + m_CustomerShareVariables.strXmlExtension);
            //    m_CustomerShareVariables.productRecipeOutputCassetteSettings = m_CustomerShareVariables.customerRecipeOutputCassetteSettings;
            //}
            //else
            //{
            //    nError = 7;
            //    return nError;
            //    //stripStatusLabel.Text += "Main recipe not exist.";
            //}
            if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeVisionPath, m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName, m_CustomerShareVariables.strXmlExtension))
            {
                m_CustomerShareVariables.customerRecipeVisionSettings = Tools.Deserialize<CustomerRecipeVisionSettings>(m_CustomerShareVariables.strRecipeVisionPath + m_CustomerShareVariables.productRecipeMainSettings.VisionRecipeName + m_CustomerShareVariables.strXmlExtension);
                m_CustomerShareVariables.productRecipeVisionSettings = m_CustomerShareVariables.customerRecipeVisionSettings;
            }
            else
            {
                nError = 8;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }
            //if (Tools.IsFileExist(m_CustomerShareVariables.strRecipeSortingPath, m_CustomerShareVariables.productRecipeMainSettings.SortingRecipeName, m_CustomerShareVariables.strXmlExtension))
            //{
            //    m_CustomerShareVariables.customerRecipeSortingSettings = Tools.Deserialize<CustomerRecipeSortingSettings>(m_CustomerShareVariables.strRecipeSortingPath + m_CustomerShareVariables.productRecipeMainSettings.SortingRecipeName + m_CustomerShareVariables.strXmlExtension);
            //    m_CustomerShareVariables.productRecipeSortingSettings = m_CustomerShareVariables.customerRecipeSortingSettings;
            //}
            //else
            //{
            //    nError = 9;
            //    return nError;
            //    //stripStatusLabel.Text += "Main recipe not exist.";
            //}
            if (Tools.IsFileExist(m_CustomerShareVariables.strRecipePickUpHeadPath, m_CustomerShareVariables.productRecipeMainSettings.PickUpHeadRecipeName, m_CustomerShareVariables.strXmlExtension))
            {
                m_CustomerShareVariables.customerRecipePickUpHeadSettings = Tools.Deserialize<CustomerRecipePickUpHeadSetting>(m_CustomerShareVariables.strRecipePickUpHeadPath + m_CustomerShareVariables.productRecipeMainSettings.PickUpHeadRecipeName + m_CustomerShareVariables.strXmlExtension);
                m_CustomerShareVariables.productRecipePickUpHeadSettings = m_CustomerShareVariables.customerRecipePickUpHeadSettings;
            }
            else
            {
                nError = 10;
                return nError;
                //stripStatusLabel.Text += "Main recipe not exist.";
            }

            return nError;
        }

        override public int ReadInputFile()
        {
            int nError = 0;
            try
            {
                //map col dir:0/2
                //0:left to right
                //1:top to bottom
                //2:right to left
                //3:bottom to top

                //map row dir:1/3
                //0:left to right
                //1:top to bottom
                //2:right to left
                //3:bottom to top

                //Top Left Position
                //First_Pos = 0
                //Top Right Position
                //First_Pos = 1
                //Bottom Left Position
                //First_Pos = 2
                //Bottom Right Position
                //First_Pos = 3

                int nFirstPos = 0;
                //if (m_CustomerShareVariables.customerRecipeInputSettings.MapColDirection == 0 && m_CustomerShareVariables.customerRecipeInputSettings.MapRowDirection == 1)
                //{
                //    nFirstPos = 0;
                //}
                //else if (m_CustomerShareVariables.customerRecipeInputSettings.MapColDirection == 2 && m_CustomerShareVariables.customerRecipeInputSettings.MapRowDirection == 1)
                //{
                //    nFirstPos = 1;
                //}
                //else if (m_CustomerShareVariables.customerRecipeInputSettings.MapColDirection == 0 && m_CustomerShareVariables.customerRecipeInputSettings.MapRowDirection == 3)
                //{
                //    nFirstPos = 2;
                //}
                //else if (m_CustomerShareVariables.customerRecipeInputSettings.MapColDirection == 2 && m_CustomerShareVariables.customerRecipeInputSettings.MapRowDirection == 3)
                //{
                //    nFirstPos = 3;
                //}
                //else if (m_CustomerShareVariables.customerRecipeInputSettings.MapColDirection == 1 && m_CustomerShareVariables.customerRecipeInputSettings.MapRowDirection == 0)
                //{
                //    nFirstPos = 4;
                //}
                //else if (m_CustomerShareVariables.customerRecipeInputSettings.MapColDirection == 1 && m_CustomerShareVariables.customerRecipeInputSettings.MapRowDirection == 2)
                //{
                //    nFirstPos = 5;
                //}
                //else if (m_CustomerShareVariables.customerRecipeInputSettings.MapColDirection == 3 && m_CustomerShareVariables.customerRecipeInputSettings.MapRowDirection == 0)
                //{
                //    nFirstPos = 6;
                //}
                //else if (m_CustomerShareVariables.customerRecipeInputSettings.MapColDirection == 3 && m_CustomerShareVariables.customerRecipeInputSettings.MapRowDirection == 2)
                //{
                //    nFirstPos = 7;
                //}
                //else
                //{
                //    return 11;
                //}

                #region EnableEmptyInputFile
                //CustomerInputOutputFileFormat readInfo = new CustomerInputOutputFileFormat();
                //m_CustomerShareVariables.mappingInfo = new Product.BinInfo();
                //m_CustomerShareVariables.mappingData = new Product.MapData();
                //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableInputFile == true)
                //(
                    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 1)
                    //{
                    //nError = readInfo.ReadInputXMLFile(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID, "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID + "-Input" + ".XML" /*+ m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension*/
                    //    , ref m_CustomerShareVariables.mappingData, ref m_CustomerShareVariables.mappingInfo, ref m_CustomerShareVariables.dicMappingInfo, m_CustomerShareVariables.customerRecipeInputSettings.NoOfDeviceInRow, m_CustomerShareVariables.customerRecipeInputSettings.NoOfDeviceInCol);

                    //m_CustomerShareVariables.nUnitNo = 0;
                    //for (int i = 0; i < m_CustomerShareVariables.mappingData.Overlay[0].Bindefinition.Length; i++)
                    //{
                    //    m_CustomerShareVariables.nUnitNo += m_CustomerShareVariables.mappingData.Overlay[0].Bindefinition[i].BinCount;
                    //}
                    //if (nError == 0)
                    //{
                    //    for (int i = 0; i < m_CustomerShareVariables.mappingData.Overlay[1].BinDef.Length;i++)
                    //    {
                    //        m_CustomerShareVariables.mappingUnitIdList.Add(m_CustomerShareVariables.mappingData.Overlay[1].BinDef[i].Unitid);
                    //    }
                    //    m_CustomerShareVariables.mappingDataOut = m_CustomerShareVariables.mappingData;
                    //    m_CustomerShareVariables.mappingDataOut.EQPTINFO = m_CustomerShareVariables.mappingData.EQPTINFO;
                    //    m_CustomerShareVariables.mappingDataOut.LOTINFO = m_CustomerShareVariables.mappingData.LOTINFO;
                    //    m_CustomerShareVariables.mappingDataOut.Overlay = new Product.Overlay[2];
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0] = new Product.Overlay();
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition = new Product.MapDataOverlayBindefinition[3];
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[0] = new Product.MapDataOverlayBindefinition();
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[0].BinDescription = "Output Unit";
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[0].BinCode = "0001";
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[0].Pick = true;
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[0].BinCount = 0;
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[1] = new Product.MapDataOverlayBindefinition();
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[1].BinDescription = "Sorting Unit";
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[1].BinCode = "0002";
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[1].Pick = true;
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[1].BinCount = 0;
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[2] = new Product.MapDataOverlayBindefinition();
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[2].BinDescription = "Reject Unit";
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[2].BinCode = "0003";
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[2].Pick = true;
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[0].Bindefinition[2].BinCount = 0;

                    //    m_CustomerShareVariables.mappingDataOut.Overlay[1] = new Product.Overlay();
                    //    m_CustomerShareVariables.mappingDataOut.Overlay[1].BinDef = new Product.MapDataOverlayBinDef[m_CustomerShareVariables.nUnitNo];
                    //    for (int i = 0; i < m_CustomerShareVariables.mappingDataOut.Overlay[1].BinDef.Length; i++)
                    //    {
                    //        m_CustomerShareVariables.mappingDataOut.Overlay[1].BinDef[i] = new Product.MapDataOverlayBinDef();
                    //        m_CustomerShareVariables.mappingDataOut.Overlay[1].BinDef[i].BinCode = "";
                    //        m_CustomerShareVariables.mappingDataOut.Overlay[1].BinDef[i].Unitid = "";
                    //        m_CustomerShareVariables.mappingDataOut.Overlay[1].BinDef[i].BinDescription = "";
                    //    }
                        
                    //    //Machine.LogDisplay.AddLogDisplay("Process", "Fail to read input file " + m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID
                    //    //    //+ "\\" + ShareVariables.recipeOutputFileSettings.strPickAndPlaceFileNamePrefix + ShareVariables.strCurrentTileID + ".txt");
                    //    //    + "\\" + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceFileNamePrefix + m_CustomerShareVariables.strRawBarcodeID + ".XML" );
                    //    return nError;
                    //}
                    //}

                    //else
                    //{
                    //    Machine.LogDisplay.AddLogDisplay("Process", "Fail to read input file " + m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID
                    //        //+ "\\" + ShareVariables.recipeOutputFileSettings.strPickAndPlaceFileNamePrefix + ShareVariables.strCurrentTileID + ".txt");
                    //        + "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID + "-Input" + ".XML");
                    //    return nError;
                    //}
                //}
                
                #endregion 

                //nError = CheckInputFile();
                if (nError != 0)
                {
                    return nError;
                }
                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
        }
        int GetAddNewColumnCode()
        {
            int nCode = 1;
            //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLAAddColumn == true)
            //{
            //    nCode = 2;
            //}
            //else
            //{
                nCode = 1;
            //}
            return nCode ;
        }

        override public int ProcessInputFile()
        {
            int nError = 0;
            int nMaxInputRow = 0;
            int nMaxInputCol = 0;
            int nMinInputRow = 1400;
            int nMinInputCol = 1400;
            int nCenterUnitRow = 0;
            int nCenterUnitCol = 0;
            try
            {
                //if ((m_CustomerShareVariables.nInputTrayNo % 2) == 1)
                //{
                if (m_CustomerShareVariables.nInputTrayNo <= 150)
                {
                    m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.nInputTrayNo] = new Product.BinInfo();
                    //m_CustomerShareVariables.mappingInfo = new Product.BinInfo();

                    m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.nInputTrayNo].arrayUnitInfo = new Product.UnitInfo[(int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput
                        * (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInColInput];

                    m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.nInputTrayNo].Row_Max = (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput;
                    m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.nInputTrayNo].Col_Max = (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInColInput;

                    readInfo.InitializeMapArrayData(ref m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.nInputTrayNo], (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput, (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInColInput, 0, 1);

                    //nError = readInfo.InitializeMapArrayData(ref m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.nInputTrayNo], (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput, (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInColInput, 0, 1);
                    //}
                    //else
                    //{
                    //    m_CustomerShareVariables.mappingInfo2 = new Product.BinInfo();

                    //    m_CustomerShareVariables.mappingInfo2.arrayUnitInfo = new Product.UnitInfo[(int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput
                    //        * (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInColInput];

                    //    m_CustomerShareVariables.mappingInfo2.Row_Max = (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput;
                    //    m_CustomerShareVariables.mappingInfo2.Col_Max = (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInColInput;

                    //    nError = readInfo.InitializeMapArrayData(ref m_CustomerShareVariables.mappingInfo2, (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInRowInput, (int)m_CustomerShareVariables.productRecipeInputSettings.NoOfDeviceInColInput, 0, 1);
                    //}
                    //m_CustomerProcessEvent.PCS_GUI_Initial_Map.Set();
                }
                else
                {
                    Machine.SequenceControl.SetAlarm(10015);
                }
                //if (Directory.Exists(ShareVariables.optionSettings.InvisFileFolderName + "\\" + ShareVariables.strucInputProductInfo.Lot_ID + "\\" + ShareVariables.strCurrentTileID))
                //if (Directory.Exists(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath)
                //    || m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableInputFile == false)
                //{
                //    CustomerInputOutputFileFormat readInfo = new CustomerInputOutputFileFormat();
                //    int NoOfTileFound = 0;
                //    if (Directory.Exists(m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp"))
                //        Directory.Delete(m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp", true);
                //    nError = ReadInputFile();
                //    if (nError == 0)
                //    {
                //        #region EnableEmptyInputFile
                //        if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableInputFile == true)
                //        {
                //            //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true)
                //            //{
                //            //    if (m_CustomerShareVariables.strucInputProductInfo.InputFileNo == 1)
                //            //    {
                //            //        if (Directory.Exists(m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID) == false)
                //            //        {
                //            //            Directory.CreateDirectory(m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID);
                //            //        }
                //            //    }
                //            //    else if (m_CustomerShareVariables.strucInputProductInfo.InputFileNo == 2)
                //            //    {
                //            //        if (Directory.Exists(m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput2 + "\\" + m_CustomerShareVariables.strCurrentBarcodeID) == false)
                //            //        {
                //            //            Directory.CreateDirectory(m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput2 + "\\" + m_CustomerShareVariables.strCurrentBarcodeID);
                //            //        }
                //            //    }
                //            //}
                //            //else
                //            //{
                //            //    if (Directory.Exists(m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID) == false)
                //            //    {
                //            //        Directory.CreateDirectory(m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID);
                //            //    }
                //            //}
                //            //if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //            //{
                //            //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true)
                //            //    {
                //            //        if (m_CustomerShareVariables.strucInputProductInfo.InputFileNo == 1)
                //            //        {
                //            //            File.Copy(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID
                //            //            //File.Copy(ShareVariables.optionSettings.InvisFileFolderName + "\\" + ShareVariables.strucInputProductInfo.Lot_ID + "\\" + ShareVariables.strCurrentTileID
                //            //            + "\\WFR" + m_CustomerShareVariables.strCurrentBarcodeID + "-" + m_CustomerShareVariables.strucInputProductInfo.WaferBin + "-"
                //            //            + m_CustomerShareVariables.productRecipeOutputFileSettings.strInputMachineProcessCode + ".vis",
                //            //            m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID
                //            //            + "\\WFR" + m_CustomerShareVariables.strCurrentBarcodeID + "-" + m_CustomerShareVariables.strucInputProductInfo.WaferBin + "-"
                //            //            + m_CustomerShareVariables.productRecipeOutputFileSettings.strInputMachineProcessCode + "-Input" + ".vis", true);
                //            //        }
                //            //        else if (m_CustomerShareVariables.strucInputProductInfo.InputFileNo == 2)
                //            //        {
                //            //            File.Copy(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID2
                //            //            //File.Copy(ShareVariables.optionSettings.InvisFileFolderName + "\\" + ShareVariables.strucInputProductInfo.Lot_ID + "\\" + ShareVariables.strCurrentTileID
                //            //            + "\\WFR" + m_CustomerShareVariables.strCurrentBarcodeID + "-" + m_CustomerShareVariables.strucInputProductInfo.WaferBin + "-"
                //            //            + m_CustomerShareVariables.productRecipeOutputFileSettings.strInputMachineProcessCode + ".vis",
                //            //            m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput2 + "\\" + m_CustomerShareVariables.strCurrentBarcodeID
                //            //            + "\\WFR" + m_CustomerShareVariables.strCurrentBarcodeID + "-" + m_CustomerShareVariables.strucInputProductInfo.WaferBin + "-"
                //            //            + m_CustomerShareVariables.productRecipeOutputFileSettings.strInputMachineProcessCode + "-Input" + ".vis", true);
                //            //        }
                //            //    }
                //            //    else
                //            //    {
                //            //        File.Copy(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID
                //            //            //File.Copy(ShareVariables.optionSettings.InvisFileFolderName + "\\" + ShareVariables.strucInputProductInfo.Lot_ID + "\\" + ShareVariables.strCurrentTileID
                //            //            + "\\WFR" + m_CustomerShareVariables.strCurrentBarcodeID + "-" + m_CustomerShareVariables.strucInputProductInfo.WaferBin + "-"
                //            //            + m_CustomerShareVariables.productRecipeOutputFileSettings.strInputMachineProcessCode + ".vis",
                //            //            m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID
                //            //            + "\\WFR" + m_CustomerShareVariables.strCurrentBarcodeID + "-" + m_CustomerShareVariables.strucInputProductInfo.WaferBin + "-"
                //            //            + m_CustomerShareVariables.productRecipeOutputFileSettings.strInputMachineProcessCode + "-Input" + ".vis", true);
                //            //    }
                //            //}
                //            //else if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 1)
                //            //{
                //            //    File.Copy(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID
                //            //        //File.Copy(ShareVariables.optionSettings.InvisFileFolderName + "\\" + ShareVariables.strucInputProductInfo.Lot_ID + "\\" + ShareVariables.strCurrentTileID
                //            //        //+ "\\" + ShareVariables.recipeOutputFileSettings.strPickAndPlaceFileNamePrefix + ShareVariables.strCurrentTileID + ".txt",
                //            //        + "\\" + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceFileNamePrefix + m_CustomerShareVariables.strRawBarcodeID + "." + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension,
                //            //        m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID
                //            //        //+ "\\" + ShareVariables.recipeOutputFileSettings.strPickAndPlaceFileNamePrefix + ShareVariables.strCurrentTileID + "-Input" + ".txt", true);
                //            //        + "\\" + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceFileNamePrefix + m_CustomerShareVariables.strRawBarcodeID + "-Input" + "." + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension, true);
                //            //}
                //            //else if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 3)
                //            //{
                //            //    bool bFoundInputFile = false;
                //            //    string strInputPath = m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" + m_CustomerShareVariables.strucInputProductInfo.WorkOrder;
                //            //    if (Directory.Exists(strInputPath) == true)
                //            //    {
                //            //        if (File.Exists(strInputPath + "\\" + m_CustomerShareVariables.strCurrentBarcodeID + "." + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension))
                //            //        {
                //            //            bFoundInputFile = true;
                //            //        }
                //            //    }
                //            //    if (bFoundInputFile == false && Directory.Exists(strInputPath + "A") == true)
                //            //    {
                //            //        if (File.Exists(strInputPath + "A\\" + m_CustomerShareVariables.strCurrentBarcodeID + "." + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension))
                //            //        {
                //            //            bFoundInputFile = true;
                //            //            strInputPath = strInputPath + "A";
                //            //        }
                //            //    }
                //            //    if (bFoundInputFile == false && Directory.Exists(strInputPath + "B") == true)
                //            //    {
                //            //        if (File.Exists(strInputPath + "B\\" + m_CustomerShareVariables.strCurrentBarcodeID + "." + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension))
                //            //        {
                //            //            bFoundInputFile = true;
                //            //            strInputPath = strInputPath + "B";
                //            //        }
                //            //    }
                //            //    if (bFoundInputFile == false && Directory.Exists(strInputPath + "C") == true)
                //            //    {
                //            //        if (File.Exists(strInputPath + "C\\" + m_CustomerShareVariables.strCurrentBarcodeID + "." + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension))
                //            //        {
                //            //            bFoundInputFile = true;
                //            //            strInputPath = strInputPath + "C";
                //            //        }
                //            //    }
                //            //    if (bFoundInputFile == false && Directory.Exists(strInputPath + "D") == true)
                //            //    {
                //            //        if (File.Exists(strInputPath + "D\\" + m_CustomerShareVariables.strCurrentBarcodeID + "." + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension))
                //            //        {
                //            //            bFoundInputFile = true;
                //            //            strInputPath = strInputPath + "D";
                //            //        }
                //            //    }
                //            //    File.Copy(strInputPath
                //            //        //File.Copy(ShareVariables.optionSettings.InvisFileFolderName + "\\" + ShareVariables.strucInputProductInfo.Lot_ID + "\\" + ShareVariables.strCurrentTileID
                //            //        //+ "\\" + ShareVariables.recipeOutputFileSettings.strPickAndPlaceFileNamePrefix + ShareVariables.strCurrentTileID + ".txt",
                //            //        + "\\" + m_CustomerShareVariables.strCurrentBarcodeID + "." + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension,
                //            //        m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.WorkOrder + "\\" + m_CustomerShareVariables.strCurrentBarcodeID
                //            //        //+ "\\" + ShareVariables.recipeOutputFileSettings.strPickAndPlaceFileNamePrefix + ShareVariables.strCurrentTileID + "-Input" + ".txt", true);
                //            //        + "\\" + m_CustomerShareVariables.strCurrentBarcodeID + "-Input" + "." + m_CustomerShareVariables.productRecipeOutputFileSettings.strPickAndPlaceInputFileExtension, true);
                //            //}
                //            //else if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 4)
                //            //{
                //            //    string strFilename = "";
                //            //    //nError = readInfo.FindFileWithPreFixAndSuffix(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath,// + "\\" + ShareVariables.strucInputProductInfo.LotID,
                //            //    //    "LA_" + m_CustomerShareVariables.strucInputProductInfo.WorkOrder, m_CustomerShareVariables.strCurrentBarcodeID + ".txt", out strFilename);
                //            //    ////File.Copy(ShareVariables.optionSettings.LocalOutputPath + "\\Output\\Temp\\" + ShareVariables.strucInputProductInfo.LotID + "\\" + ShareVariables.strCurrentTileID
                //            //    ////+ strFilename,
                //            //    ////ShareVariables.optionSettings.LocalOutputPath + "\\Output\\" + ShareVariables.strucInputProductInfo.LotID + "\\" + ShareVariables.strCurrentTileID
                //            //    ////+ "\\" + strFilename, true);

                //            //    //File.Copy(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath// + "\\" + ShareVariables.strucInputProductInfo.LotID
                //            //    //    + "\\" + strFilename,
                //            //    //    m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID + "\\"
                //            //    //    + strFilename.Substring(0, strFilename.Length - 4) + ".txt", true);
                //            //}
                //            //else if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 5)
                //            //{
                //            //    File.Copy(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" + m_CustomerShareVariables.strucInputProductInfo.LotID
                //            //        + "\\" + m_CustomerShareVariables.productRecipeOutputFileSettings.strDPFileNamePrefix + m_CustomerShareVariables.strCurrentBarcodeID + ".txt",
                //            //        m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID
                //            //        + "\\" + m_CustomerShareVariables.productRecipeOutputFileSettings.strDPFileNamePrefix + m_CustomerShareVariables.strCurrentBarcodeID + "-Input.txt", true);
                //            //}
                //            //else if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 6)
                //            //{
                //            //    string strFilename = m_CustomerShareVariables.strCurrentBarcodeID + ".txt";
                //            //    if (Directory.Exists(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath))
                //            //    {
                //            //        if (File.Exists(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + "\\" +
                //            //        m_CustomerShareVariables.strCurrentBarcodeID + "_ORISG.txt"))
                //            //        {
                //            //            strFilename = m_CustomerShareVariables.strCurrentBarcodeID + "_ORISG.txt";
                //            //        }
                //            //    }
                //            //    File.Copy(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath
                //            //        + "\\" + strFilename,
                //            //        m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID
                //            //        + "\\" + m_CustomerShareVariables.strCurrentBarcodeID + "-Input.txt", true);
                //            //}
                //            //else if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 7)
                //            //{
                //            //    File.Copy(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath
                //            //        + "\\" + m_CustomerShareVariables.strCurrentBarcodeID + ".txt",
                //            //        m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID
                //            //        + "\\" + m_CustomerShareVariables.strCurrentBarcodeID + "-Input.txt", true);
                //            //}
                //            //else if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 8)  //
                //            //{
                //            //    File.Copy(m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath
                //            //        + "\\" + m_CustomerShareVariables.strCurrentBarcodeID + ".txt",
                //            //        m_CustomerShareVariables.productOptionSettings.LocalOutputPath + "\\Output\\Temp\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "\\" + m_CustomerShareVariables.strCurrentBarcodeID
                //            //        + "\\" + m_CustomerShareVariables.strCurrentBarcodeID + "-Input.txt", true);
                //            //}
                //        }
                //        #endregion
                        
                //        //SetShareMemoryEvent("RTHD_GMAIN_BARCODE_RESULT_VALID", true);

                //        //m_CustomerShareVariables.nTotalImages = 0;
                //        //m_CustomerShareVariables.tileIDInfo = new Product.TileIdInfo();
                //        //m_CustomerShareVariables.tileIDInfo.TileId = m_CustomerShareVariables.strCurrentBarcodeID;
                //        //m_CustomerShareVariables.RegKey.SetJobDataKeyParameter("strCurrentTileID", m_CustomerShareVariables.strCurrentBarcodeID);
                //        //m_CustomerShareVariables.RegKey.SetJobDataKeyParameter("strCurrentTileID2", m_CustomerShareVariables.strCurrentBarcodeID2);
                //        //if (m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0 && m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true)
                //        //{
                //        //    m_CustomerProcessEvent.PCS_PCS_Send_Vision_LotID_BarcodeID.Set();
                //        //}
                //        //else
                //        //    m_CustomerProcessEvent.PCS_PCS_Send_Vision_BarcodeID.Set();
                
                //        SetShareMemoryEvent("RTHD_GMAIN_TRUE_TO_SEND_TILEID", true);
                //        //m_CustomerProcessEvent.PCS_GUI_Initial_Map.Set();
                //        //#region FrameSelection
                //        //m_CustomerProcessEvent.PCS_GUI_UpdateInputBarcodeID.Set();
                //        //#endregion
                //        OnReadBarcodeID();
                //    }
                //    else if (nError == 1)
                //    {
                //        Machine.SequenceControl.SetAlarm(10010);
                //        Machine.LogDisplay.AddLogDisplay("Error", "Input Directory not exist");
                //        //m_CustomerProcessEvent.PCS_GUI_ShowBarcodeRetry.Set();
                //    }
                //    else if (nError == 2)
                //    {
                //        Machine.SequenceControl.SetAlarm(10007);
                //        Machine.LogDisplay.AddLogDisplay("Error", "Input File not exist");
                //        //m_CustomerProcessEvent.PCS_GUI_ShowBarcodeRetry.Set();
                //    }
                //    else if (nError == 10)
                //    {
                //        Machine.SequenceControl.SetAlarm(10015);
                //    }
                //    else if (nError == 11)
                //    {
                //        Machine.SequenceControl.SetAlarm(3001);
                //        Machine.LogDisplay.AddLogDisplay("Error", "Incorrect recipe setting for map row and column direction");
                //        //m_CustomerProcessEvent.PCS_GUI_ShowBarcodeRetry.Set();
                //    }
                //    else
                //    {
                //        Machine.SequenceControl.SetAlarm(10007);
                //        Machine.LogDisplay.AddLogDisplay("Error", "Fail to read input file");
                //        //LogDisplay.AddLogDisplay("Error", "Fail to read file in " + ShareVariables.recipeOutputFileSettings.strInputFilePath + "\\" + ShareVariables.strucInputProductInfo.LotID
                //        //MessageBox.Show("Read vis file fail in " + ShareVariables.optionSettings.InvisFileFolderName + "\\" + ShareVariables.strucInputProductInfo.Lot_ID + "\\" + ShareVariables.strCurrentTileID
                //        //+ "\\WFR" + ShareVariables.strCurrentTileID + "-" + ShareVariables.strucInputProductInfo.WaferBin + "-" + ShareVariables.recipeOutputFileSettings.strInputMachineProcessCode + ".vis");
                //        //MessageBox.Show("Read vis file fail in " + ShareVariables.optionSettings.InvisFileFolderName + "\\" + ShareVariables.strucInputProductInfo.Lot_ID
                //        //    //MessageBox.Show("Read vis file fail in " + ShareVariables.optionSettings.InvisFileFolderName + "\\" + ShareVariables.strucInputProductInfo.Lot_ID + "\\" + ShareVariables.strCurrentTileID
                //        //+ "\\WFR" + ShareVariables.strCurrentTileID + "-" + ShareVariables.strucInputProductInfo.Wafer_Bin + "-" + ShareVariables.optionSettings.strInputMachineProcessCode + ".vis");
                //        //m_CustomerProcessEvent.PCS_GUI_ShowBarcodeRetry.Set();
                //    }
                //    base.ProcessInputFile();
                //}
                //else
                //{
                //    Machine.SequenceControl.SetAlarm(10010);
                //    Machine.LogDisplay.AddLogDisplay("Error", "Directory " + m_CustomerShareVariables.productRecipeOutputFileSettings.strInputFilePath + " not exist");
                //    //m_CustomerProcessEvent.PCS_GUI_ShowBarcodeRetry.Set();
                //}
            }
            catch (Exception ex)
            {
                nError = -1;
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
            return nError;
        }
        override public int WriteAllOutputFile()
        {
            int nError = 0;
            try
            {
                //if (m_CustomerShareVariables.customerRecipeOutputFileSettings.EnableXMLFile == true)
                //{
                //    if (readInfo.WriteOutputXML(m_CustomerShareVariables.customerRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotID
                //        , m_CustomerShareVariables.strucInputProductInfo.LotID + "-Output" + ".xml",
                //         m_CustomerShareVariables.mappingDataOut) != 0)
                //    {
                //        Machine.SequenceControl.SetAlarm(10014);
                //        Machine.LogDisplay.AddLogDisplay("Error", "Error during generate Output XML File");
                //    }
                //    Machine.EventLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} : Write all output file Done.");
                //}
            }
            catch (Exception ex)
            {
                nError = -1;
                Machine.DebugLogger.WriteLog(string.Format("{0} {1} {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), "Exception in CustomizeOutputFile"));
            }
            return nError;
        }
        override public int CustomizeOutputFile()
        {
            int nError = 0;
            m_CustomerProcessEvent.PCS_GUI_Last_Data_Generated.Reset();
            m_CustomerProcessEvent.PCS_GUI_Copy_Task_Complete.Reset();
            m_CustomerProcessEvent.PCS_GUI_Writing_File_Start.Set();
            m_CustomerProcessEvent.PCS_GUI_Error_Copy_Task.Reset();
            try
            {
            RewriteReport:

                CustomerInputOutputFileFormat readInfo = new CustomerInputOutputFileFormat();
                readInfo.customerShareVariables = m_CustomerShareVariables;


                if (m_CustomerShareVariables.PreviousReportTrayNo + 1 < m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo"))
                {
                    m_CustomerShareVariables.PreviousReportTrayNo = m_CustomerShareVariables.PreviousReportTrayNo + 1;

                    Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} WriteReportTrayNo = {m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo")}");
                    Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} PreviousReportTrayNo = {m_CustomerShareVariables.PreviousReportTrayNo}");

                    if (m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo") != m_CustomerShareVariables.PreviousReportTrayNo)
                    {
                        m_CustomerShareVariables.PreviousReportLotID = m_CustomerShareVariables.strucInputProductInfo.LotID;
                        //m_CustomerShareVariables.PreviousReportTrayNo = m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo");

                        if (m_CustomerShareVariables.LastLotIDOutput != "")
                        {
                            readInfo.RenameFileAndFolder(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputFilePath,
                                m_CustomerShareVariables.LastLotIDOutput, m_CustomerShareVariables.strucInputProductInfo.LotID);

                            readInfo.RenameFileAndFolder(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath,
                                m_CustomerShareVariables.LastLotIDOutput, m_CustomerShareVariables.strucInputProductInfo.LotID);
                        }
                        m_CustomerShareVariables.dtProductionEndTime = DateTime.Now;
                        if (readInfo.GenerateSummary(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotID, "Summary-" + m_CustomerShareVariables.strucInputProductInfo.LotID + ".txt",
                                   m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.PreviousReportTrayNo - 1], ref m_CustomerShareVariables.strucInputProductInfo,
                                   m_CustomerShareVariables.dtProductionStartTime, m_CustomerShareVariables.dtProductionEndTime, m_CustomerShareVariables.outputFileOption,
                                   m_CustomerShareVariables.optionSettings.MachineID, m_CustomerShareVariables.listLotID,
                                    m_CustomerShareVariables.strucInputProductInfo.LotID, m_CustomerRTSSProcess.GetProductionInt("nCurrrentTotalUnitDoneByLot"),
                                     m_CustomerShareVariables.PreviousReportTrayNo, m_CustomerShareVariables.strucInputProductInfo.LotID) != 0)
                        {
                            Machine.SequenceControl.SetAlarm(70014);
                            Machine.LogDisplay.AddLogDisplay("Error", "Error during generate full summary");
                        }
                        if (readInfo.GenerateSummary(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotID, "Summary-" + m_CustomerShareVariables.strucInputProductInfo.LotID + ".txt",
                                   m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.PreviousReportTrayNo - 1], ref m_CustomerShareVariables.strucInputProductInfo,
                                   m_CustomerShareVariables.dtProductionStartTime, m_CustomerShareVariables.dtProductionEndTime, m_CustomerShareVariables.outputFileOption,
                                   m_CustomerShareVariables.optionSettings.MachineID, m_CustomerShareVariables.listLotID,
                                    m_CustomerShareVariables.strucInputProductInfo.LotID, m_CustomerRTSSProcess.GetProductionInt("nCurrrentTotalUnitDoneByLot"),
                                     m_CustomerShareVariables.PreviousReportTrayNo, m_CustomerShareVariables.strucInputProductInfo.LotID) != 0)
                        {
                            Machine.SequenceControl.SetAlarm(70014);
                            Machine.LogDisplay.AddLogDisplay("Error", "Error during generate Local full summary");
                        }


                        if (readInfo.WriteOutputMapFile(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotID,
                             "Map-" + m_CustomerShareVariables.strucInputProductInfo.dtStartDate.ToString("yyyyMMdd") + "-" + m_CustomerShareVariables.strucInputProductInfo.LotID + ".csv",
                           m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.PreviousReportTrayNo - 1], m_CustomerShareVariables.strucInputProductInfo, m_CustomerShareVariables.optionSettings.MachineID, DateTime.Now) != 0)
                        {
                            Machine.SequenceControl.SetAlarm(70017);
                            Machine.LogDisplay.AddLogDisplay("Error", "Error during generate Output Map File");
                        }

                        if (readInfo.WriteOutputMapFile(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotID,
                             "Map-" + m_CustomerShareVariables.strucInputProductInfo.dtStartDate.ToString("yyyyMMdd") + "-" + m_CustomerShareVariables.strucInputProductInfo.LotID + ".csv",
                           m_CustomerShareVariables.MultipleMappingInfo[m_CustomerShareVariables.PreviousReportTrayNo - 1], m_CustomerShareVariables.strucInputProductInfo, m_CustomerShareVariables.optionSettings.MachineID, DateTime.Now) != 0)
                        {
                            Machine.SequenceControl.SetAlarm(70017);
                            Machine.LogDisplay.AddLogDisplay("Error", "Error during generate Output Local Map File");
                        }
                        m_CustomerProcessEvent.PCS_GUI_Update_Summary_Report.Set();
                        m_CustomerProcessEvent.PCS_GUI_Update_Yeild_Pareto.Set();
                    }
                    else
                    {
                        if (m_CustomerShareVariables.IsPassYieldMES == true)
                        {
                            m_CustomerShareVariables.IsPassYieldMES = false;
                            m_CustomerShareVariables.IsPassYieldMESDone = true;
                        }
                    }
                    goto RewriteReport;
                }
                else
                {
                    Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} WriteReportTrayNo = {m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo")}");
                    Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} PreviousReportTrayNo = {m_CustomerShareVariables.PreviousReportTrayNo}");
                    //Machine.DebugLogger.WriteLog($"{DateTime.Now.ToString("yyyyMMdd HHmmss")} {m_CustomerShareVariables.strucInputProductInfo.LotID}");
                    if (/*m_CustomerShareVariables.strucInputProductInfo.LotID != m_CustomerShareVariables.PreviousReportLotID
                    &&*/ m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo") != m_CustomerShareVariables.PreviousReportTrayNo)
                    {
                        m_CustomerShareVariables.PreviousReportLotID = m_CustomerShareVariables.strucInputProductInfo.LotID;
                        m_CustomerShareVariables.PreviousReportTrayNo = m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo");

                        if (m_CustomerShareVariables.LastLotIDOutput != "")
                        {
                            readInfo.RenameFileAndFolder(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputFilePath,
                                m_CustomerShareVariables.LastLotIDOutput, m_CustomerShareVariables.strucInputProductInfo.LotID);

                            readInfo.RenameFileAndFolder(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath,
                                m_CustomerShareVariables.LastLotIDOutput, m_CustomerShareVariables.strucInputProductInfo.LotID);
                        }
                        m_CustomerShareVariables.dtProductionEndTime = DateTime.Now;
                        if (readInfo.GenerateSummary(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotID, "Summary-" + m_CustomerShareVariables.strucInputProductInfo.LotID + ".txt",
                                   m_CustomerShareVariables.MultipleMappingInfo[m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo") - 1], ref m_CustomerShareVariables.strucInputProductInfo,
                                   m_CustomerShareVariables.dtProductionStartTime, m_CustomerShareVariables.dtProductionEndTime, m_CustomerShareVariables.outputFileOption,
                                   m_CustomerShareVariables.optionSettings.MachineID, m_CustomerShareVariables.listLotID,
                                    m_CustomerShareVariables.strucInputProductInfo.LotID, m_CustomerRTSSProcess.GetProductionInt("nCurrrentTotalUnitDoneByLot"),
                                     m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo"), m_CustomerShareVariables.strucInputProductInfo.LotID) != 0)
                        {
                            Machine.SequenceControl.SetAlarm(70014);
                            Machine.LogDisplay.AddLogDisplay("Error", "Error during generate full summary");
                        }

                        if (readInfo.GenerateSummary(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotID, "Summary-" + m_CustomerShareVariables.strucInputProductInfo.LotID + ".txt",
                                   m_CustomerShareVariables.MultipleMappingInfo[m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo") - 1], ref m_CustomerShareVariables.strucInputProductInfo,
                                   m_CustomerShareVariables.dtProductionStartTime, m_CustomerShareVariables.dtProductionEndTime, m_CustomerShareVariables.outputFileOption,
                                   m_CustomerShareVariables.optionSettings.MachineID, m_CustomerShareVariables.listLotID,
                                    m_CustomerShareVariables.strucInputProductInfo.LotID, m_CustomerRTSSProcess.GetProductionInt("nCurrrentTotalUnitDoneByLot"),
                                     m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo"), m_CustomerShareVariables.strucInputProductInfo.LotID) != 0)
                        {
                            Machine.SequenceControl.SetAlarm(70014);
                            Machine.LogDisplay.AddLogDisplay("Error", "Error during generate full summary");
                        }


                        if (readInfo.WriteOutputMapFile(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotID,
                            "Map-" + m_CustomerShareVariables.strucInputProductInfo.dtStartDate.ToString("yyyyMMdd") + "-" + m_CustomerShareVariables.strucInputProductInfo.LotID + ".csv",
                           m_CustomerShareVariables.MultipleMappingInfo[m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo") - 1], m_CustomerShareVariables.strucInputProductInfo, m_CustomerShareVariables.optionSettings.MachineID, DateTime.Now) != 0)
                        {
                            Machine.SequenceControl.SetAlarm(70017);
                            Machine.LogDisplay.AddLogDisplay("Error", "Error during generate Output Map File");
                        }

                        if (readInfo.WriteOutputMapFile(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotID,
                            "Map-" + m_CustomerShareVariables.strucInputProductInfo.dtStartDate.ToString("yyyyMMdd") + "-" + m_CustomerShareVariables.strucInputProductInfo.LotID + ".csv",
                           m_CustomerShareVariables.MultipleMappingInfo[m_CustomerRTSSProcess.GetProductionInt("WriteReportTrayNo") - 1], m_CustomerShareVariables.strucInputProductInfo, m_CustomerShareVariables.optionSettings.MachineID, DateTime.Now) != 0)
                        {
                            Machine.SequenceControl.SetAlarm(70017);
                            Machine.LogDisplay.AddLogDisplay("Error", "Error during generate Output Map File");
                        }
                        m_CustomerProcessEvent.PCS_GUI_Update_Summary_Report.Set();
                        m_CustomerProcessEvent.PCS_GUI_Update_Yeild_Pareto.Set();
                    }
                    else
                    {
                        if (m_CustomerShareVariables.IsPassYieldMES == true)
                        {
                            m_CustomerShareVariables.IsPassYieldMES = false;
                            m_CustomerShareVariables.IsPassYieldMESDone = true;
                        }
                    }
                }

                //if ((m_CustomerShareVariables.nInputTrayNo % 2) == 1)
                //{               

                if (m_CustomerRTSSProcess.GetEvent("GMAIN_RTHD_ENDLOT_UPDATE_SUMMARY_DONE") == true)
                {
                    m_CustomerRTSSProcess.SetEvent("GMAIN_RTHD_ENDLOT_UPDATE_SUMMARY_DONE", false);
                    readInfo.RenameFileAndFolder(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputFilePath,
                               m_CustomerShareVariables.strucInputProductInfo.LotID, m_CustomerShareVariables.strucInputProductInfo.LotID + "_DONE", false, m_CustomerShareVariables.strucInputProductInfo.dtStartDate);

                    m_CustomerProcessEvent.PCS_GUI_Update_Summary_Report_EndLot.Set();
                    m_CustomerProcessEvent.PCS_GUI_Update_Yeild_Pareto_EndLot.Set();
                    m_CustomerProcessEvent.PCS_GUI_Update_Alarm_Pareto_EndLot.Set();
                }
                m_CustomerProcessEvent.PCS_GUI_Last_Data_Generated.Set();

                //SetShareMemoryEvent("RTHD_GMAIN_UNIT_PLACED_ON_OUTPUT_FRAME", false);
                //m_CustomerProcessEvent.PCS_GUI_Writing_File_Start.Reset();
            }
            catch (Exception ex)
            {
                m_CustomerProcessEvent.PCS_GUI_Last_Data_Generated.Set();
                //SetShareMemoryEvent("RTHD_GMAIN_UNIT_PLACED_ON_OUTPUT_FRAME", false);
                //m_CustomerProcessEvent.PCS_GUI_Writing_File_Start.Reset();
                m_CustomerProcessEvent.PCS_GUI_Error_Copy_Task.Set();
                nError = -1;
                Machine.DebugLogger.WriteLog(string.Format("{0} {1} {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), "Exception in CustomizeOutputFile"));
            }
            return nError;
        }
        
    }
}
