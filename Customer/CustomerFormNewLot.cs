using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Common;
using Product;
using Machine;
using MoveonMESAPI;
using Newtonsoft.Json;

namespace Customer
{
    public partial class CustomerFormNewLot : Form
    {
        public string m_strmode = "New Lot Form";
        public InputData InputLotMES = new InputData();
        BackgroundWorker m_bgwUserInterface;

        private Keys keysPrevious;

        private CustomerShareVariables m_CustomerShareVariables;// = new ProductShareVariables();
        private CustomerProcessEvent m_CustomerProcessEvent;// = new ProductProcessEvent();

        private CustomerRTSSProcess m_CustomerRTSSProcess;
        private CustomerReportProcess m_CustomerReportProcess;
        private CustomerReportEvent m_CustomerReportEvent;
        private CustomerPreviousLotInfo m_CustomerPreviousLotInfo;

        public CustomerShareVariables customerShareVariables
        {
            set { m_CustomerShareVariables = value; }
        }

        public CustomerProcessEvent customerProcessEvent
        {
            set { m_CustomerProcessEvent = value; }
        }

        public CustomerRTSSProcess customerRTSSProcess
        {
            set
            {
                m_CustomerRTSSProcess = value;
            }
        }

        public CustomerReportEvent customerReportEvent
        {
            set
            {
                m_CustomerReportEvent = value;
            }
        }

        public CustomerReportProcess customerReportProcess
        {
            set
            {
                m_CustomerReportProcess = value;
            }
        }

        public CustomerPreviousLotInfo customerPreviousLotInfo
        {
            set { m_CustomerPreviousLotInfo = value; }
        }
        #region Form Events

        public CustomerFormNewLot()
        {
            InitializeComponent();
            //Initialize();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            int nError = 0;
            try
            {
                #region Verify Parameters

                #region Input XY Table
//#if InputXYTable 
                if (textBoxLotID1.Text == "")
                {
                    richTextBoxMessage.Text += string.Format("{0} : Please enter Lot ID", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;                    
                    //MessageBox.Show("Please enter Lot ID");
                    return;
                }
                if(textBoxLotID2.Text == "" && groupBoxLotID2.Visible ==true)
                {
                    richTextBoxMessage.Text += string.Format("{0} : Please enter Lot ID 2", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Lot ID");
                    return;
                }
                if (textBoxLotID3.Text == "" && groupBoxLotID3.Visible == true)
                {
                    richTextBoxMessage.Text += string.Format("{0} : Please enter Lot ID 3", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Lot ID");
                    return;
                }
                //if (textBoxLotIDOutput.Text == "" && m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //{
                //    //richTextBoxMessage.Text += string.Format("{0} : Please enter Lot ID2", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    richTextBoxMessage.Text += string.Format("{0} : Please enter Lot ID Output", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    return;
                //}

                //if (textBoxLotID2.Text == "" && m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Please enter Lot ID 2", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    return;
                //}
                //if (textBoxLotIDOutput2.Text == "" && m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0 
                //    && m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Please enter Lot ID Output 2", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    return;
                //}
                if (textBoxOperatorID.Text == "")
                {
                    richTextBoxMessage.Text += string.Format("{0} : Please enter Operator ID", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Work Order");
                    return;
                }
                if (textBoxPartName.Text == "")
                {
                    richTextBoxMessage.Text += string.Format("{0} : Please enter Part Name", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Work Order");
                    return;
                }
                if (textBoxPartNumber.Text == "")
                {
                    richTextBoxMessage.Text += string.Format("{0} : Please enter Part Number", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Work Order");
                    return;
                }
                if (textBoxBuild.Text == "")
                {
                    richTextBoxMessage.Text += string.Format("{0} : Please enter Build Name", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Work Order");
                    return;
                }
                //if (textBoxERPPart.Text == "")
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Please enter Part Number", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine; 
                //    //MessageBox.Show("Please enter Part Number");
                //    return;
                //}
                //if (textBoxWaferBin.Text == "")
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Please enter Wafer Bin", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine; 
                //    //MessageBox.Show("Please enter Wafer Bin");
                //    return;
                //}
                //if (textBoxPPLot.Text == "")
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Please enter PP Lot", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine; 
                //    return;
                //}
                //if (textBoxPPLot.Text == "")
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Please enter Operator ID", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine; 
                //    //MessageBox.Show("Please enter Operator ID");
                //    return;
                //}
                //if (textBoxShift.Text == "")
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Please select Operator Shift", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please select Operator Shift");
                //    return;
                //}
                if (IscContainInvalidCharacter(textBoxLotID1.Text))
                {
                    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 1. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Lot ID");
                    return;
                }
                if (IscContainInvalidCharacter(textBoxLotID2.Text))
                {
                    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 2. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Lot ID");
                    return;
                }
                if (IscContainInvalidCharacter(textBoxLotID3.Text))
                {
                    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 3. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Lot ID");
                    return;
                }
                //if (IscContainInvalidCharacter(textBoxLotID4.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 4. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Lot ID");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxLotID5.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 5. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Lot ID");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxLotID6.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 6. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Lot ID");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxLotID7.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID  7. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Lot ID");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxLotID8.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 8. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Lot ID");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxLotID9.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 9. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Lot ID");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxLotID10.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 10. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Lot ID");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxLotIDOutput.Text) && m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID2. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    return;
                //}

                //if (IscContainInvalidCharacter(textBoxLotID2.Text) && m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID 2", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxLotIDOutput2.Text) && m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0
                //    && m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Lot ID Output 2", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxWorkOrder.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Work Order. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Work Order");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxERPPart.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Part Number. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Part Number");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxWaferBin.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Wafer Bin. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter Wafer Bin");
                //    return;
                //}
                //if (IscContainInvalidCharacter(textBoxPPLot.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in PP Lot. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please enter PP Lot");
                //    return;
                //}
                if (IscContainInvalidCharacter(textBoxOperatorID.Text))
                {
                    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Operator ID. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Operator ID");
                    return;
                }
                //if (IscContainInvalidCharacter(textBoxShift.Text))
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Invalid character in Operator Shift. Please check again", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Please select Operator Shift");
                //    return;
                //}
                //if (textBoxERPPart.Text != m_CustomerShareVariables.currentMainRecipeName)
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Part number key in is different. Please check again.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    //MessageBox.Show("Part number key in is different. Please check again.");
                //    return;
                //}
                if (m_CustomerShareVariables.productOptionSettings.EnableVision == true)
                {
                    if (m_CustomerShareVariables.strVisionConnection != "Connect")
                    {
                        richTextBoxMessage.Text += string.Format("{0} : Vision is not connected.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                        return;
                    }
                }
                //#endif
                //Output Quantity (WC)
                //if (numericUpDownTotalOutputQuantity.Value == 0)
                //{
                //    richTextBoxMessage.Text += string.Format("{0} : Please enter the total output unit quantity required.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                //    return;
                //}
                #endregion Input XY Table
                m_CustomerShareVariables.listLotID.Clear();
                //m_CustomerPreviousLotInfo.PreviouslistLotID.Clear();
                if (textBoxLotID1.Text != "")
                {
                    m_CustomerShareVariables.listLotID.Add(textBoxLotID1.Text.ToUpper());
                    //m_CustomerPreviousLotInfo.PreviouslistLotID.Add(textBoxLotID1.Text);
                }
                if (textBoxLotID2.Text != "")
                {
                    m_CustomerShareVariables.listLotID.Add(textBoxLotID2.Text.ToUpper());
                    //m_CustomerPreviousLotInfo.PreviouslistLotID.Add(textBoxLotID2.Text);
                }
                if (textBoxLotID3.Text != "")
                {
                    m_CustomerShareVariables.listLotID.Add(textBoxLotID3.Text.ToUpper());
                    //m_CustomerPreviousLotInfo.PreviouslistLotID.Add(textBoxLotID3.Text);
                }
                //m_CustomerPreviousLotInfo.PreviousInputLotID = m_CustomerShareVariables.LotID;
                if(m_CustomerProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0)==false)
                {
                    m_CustomerShareVariables.dtProductionStartTime = DateTime.Now;
                    if(m_CustomerRTSSProcess.GetEvent("RMAIN_RTHD_THR_IS_REMAINTRAY") ==true)
                    {
                        if(m_CustomerShareVariables.LotIDForTrayRemainedOnInputTrayTable != m_CustomerShareVariables.listLotID[0])
                        {
                            richTextBoxMessage.Text += string.Format("{0} : Lot ID entered is not matched with lot ID for current tray on input tray table.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                            return;
                        }
                    }
                }
                if (m_CustomerProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0) == false)
                {
                    DateTime Shift1Start;
                    DateTime Shift1End;
                    DateTime Shift2Start;
                    DateTime Shift2End;
                    DateTime Shift3Start;
                    DateTime Shift3End;
                    if (m_CustomerShareVariables.m_reportSetting.NoOfShift == 0)
                    {
                        Shift1Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_CustomerShareVariables.m_reportSetting.Shift1StartHour, m_CustomerShareVariables.m_reportSetting.Shift1StartMinutes, 0);
                        Shift1End = Shift1Start.Add(new System.TimeSpan(m_CustomerShareVariables.m_reportSetting.Shift1OperationHour, 0, 0));
                        if (m_CustomerShareVariables.dtProductionStartTime >= Shift1Start && m_CustomerShareVariables.dtProductionStartTime < Shift1End)
                        {
                            m_CustomerShareVariables.strucInputProductInfo.Shift = "A";
                        }
                        else
                        {
                            richTextBoxMessage.Text += string.Format("{0} : Please complete the shift setting on option page.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                            return;
                        }
                    }
                    else if (m_CustomerShareVariables.m_reportSetting.NoOfShift == 1)
                    {
                        Shift1Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_CustomerShareVariables.m_reportSetting.Shift1StartHour, m_CustomerShareVariables.m_reportSetting.Shift1StartMinutes, 0);
                        Shift1End = Shift1Start.Add(new System.TimeSpan(m_CustomerShareVariables.m_reportSetting.Shift1OperationHour, 0, 0));
                        Shift2Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_CustomerShareVariables.m_reportSetting.Shift2StartHour, m_CustomerShareVariables.m_reportSetting.Shift2StartMinutes, 0);
                        Shift2End = Shift2Start.Add(new System.TimeSpan(m_CustomerShareVariables.m_reportSetting.Shift2OperationHour, 0, 0));
                        if (m_CustomerShareVariables.dtProductionStartTime >= Shift1Start && m_CustomerShareVariables.dtProductionStartTime < Shift1End)
                        {
                            m_CustomerShareVariables.strucInputProductInfo.Shift = "A";
                        }
                        else if (m_CustomerShareVariables.dtProductionStartTime >= Shift2Start && m_CustomerShareVariables.dtProductionStartTime < Shift2End)
                        {
                            m_CustomerShareVariables.strucInputProductInfo.Shift = "B";
                        }
                        else
                        {
                            richTextBoxMessage.Text += string.Format("{0} : Please complete the shift setting on option page.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                            return;
                        }
                    }
                    else if (m_CustomerShareVariables.m_reportSetting.NoOfShift == 2)
                    {
                        Shift1Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_CustomerShareVariables.m_reportSetting.Shift1StartHour, m_CustomerShareVariables.m_reportSetting.Shift1StartMinutes, 0);
                        Shift1End = Shift1Start.Add(new System.TimeSpan(m_CustomerShareVariables.m_reportSetting.Shift1OperationHour, 0, 0));
                        Shift2Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_CustomerShareVariables.m_reportSetting.Shift2StartHour, m_CustomerShareVariables.m_reportSetting.Shift2StartMinutes, 0);
                        Shift2End = Shift2Start.Add(new System.TimeSpan(m_CustomerShareVariables.m_reportSetting.Shift2OperationHour, 0, 0));
                        Shift3Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_CustomerShareVariables.m_reportSetting.Shift3StartHour, m_CustomerShareVariables.m_reportSetting.Shift3StartMinutes, 0);
                        Shift3End = Shift3Start.Add(new System.TimeSpan(m_CustomerShareVariables.m_reportSetting.Shift3OperationHour, 0, 0));
                        if (m_CustomerShareVariables.dtProductionStartTime >= Shift1Start && m_CustomerShareVariables.dtProductionStartTime < Shift1End)
                        {
                            m_CustomerShareVariables.strucInputProductInfo.Shift = "A";
                        }
                        else if (m_CustomerShareVariables.dtProductionStartTime >= Shift2Start && m_CustomerShareVariables.dtProductionStartTime < Shift2End)
                        {
                            m_CustomerShareVariables.strucInputProductInfo.Shift = "B";
                        }
                        else if (m_CustomerShareVariables.dtProductionStartTime >= Shift3Start && m_CustomerShareVariables.dtProductionStartTime < Shift3End)
                        {
                            m_CustomerShareVariables.strucInputProductInfo.Shift = "C";
                        }
                        else
                        {
                            richTextBoxMessage.Text += string.Format("{0} : Please complete the shift setting on option page.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                            return;
                        }
                    }
                }
                if (m_CustomerShareVariables.productOptionSettings.EnableMES == true)
                {                 
                    nError = SettingURL();
                    if (nError != 0)
                    {
                        return;
                    }
                    nError = ConfigureMESURL(m_CustomerShareVariables.MESInputURLUsed, m_CustomerShareVariables.MESOutputURLUsed, m_CustomerShareVariables.MESEndJobURLUsed);
                    if (nError != 0)
                    {
                        return;
                    }
                    nError = ReadMESInputData();
                    if (nError != 0)
                    {
                        return;
                    }
                    nError = CompareMESInputWithLotIDEntered();
                    if (nError != 0)
                    {
                        return;
                    }

                }
                else
                {
                    //if (numericUpDownInputQuantity1.Value == 0)
                    //{
                    //    richTextBoxMessage.Text += string.Format("{0} : Please enter the input quantity required for Lot 1.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //    return;
                    //}
                    //else
                    //{
                    //    m_CustomerShareVariables.InputLotQuantity[0] = (int)numericUpDownInputQuantity1.Value;
                    //}
                    if (numericUpDownInputQuantity2.Value == 0 && groupBoxLotID2.Visible == true && numericUpDownInputQuantity2.Visible==true)
                    {
                        richTextBoxMessage.Text += string.Format("{0} : Please enter the input quantity required for Lot 2.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                        return;
                    }
                    else if (groupBoxLotID2.Visible==true && numericUpDownInputQuantity2.Value != 0 && numericUpDownInputQuantity2.Visible == true)
                    {
                        m_CustomerShareVariables.InputLotQuantity[1] = (int)numericUpDownInputQuantity2.Value;
                    }
                    if (numericUpDownInputQuantity3.Value == 0 && groupBoxLotID3.Visible ==true && numericUpDownInputQuantity3.Visible == true)
                    {
                        richTextBoxMessage.Text += string.Format("{0} : Please enter the input quantity required for Lot 3.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                        return;
                    }
                    else if (numericUpDownInputQuantity3.Value !=0 && groupBoxLotID3.Visible==true && numericUpDownInputQuantity3.Visible == true)
                    {
                        m_CustomerShareVariables.InputLotQuantity[2] = (int)numericUpDownInputQuantity3.Value;

                    }
                }
                if(m_CustomerShareVariables.productOptionSettings.EnableCountDownByInputTrayNo==true)
                {
                    if (numericUpDownInputTrayQuantity1.Value == 0)
                    {
                        richTextBoxMessage.Text += string.Format("{0} : Please enter the input tray quantity required for Lot 1.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                        return;
                    }
                    else
                    {
                        m_CustomerShareVariables.InputLotTrayQuantity[0] = (int)numericUpDownInputTrayQuantity1.Value;
                    }
                    if (numericUpDownInputTrayQuantity2.Value == 0 && groupBoxLotID2.Visible == true)
                    {
                        richTextBoxMessage.Text += string.Format("{0} : Please enter the input tray quantity required for Lot 2.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                        return;
                    }
                    else if (groupBoxLotID2.Visible == true && numericUpDownInputTrayQuantity2.Value != 0)
                    {
                        m_CustomerShareVariables.InputLotTrayQuantity[1] = (int)numericUpDownInputTrayQuantity2.Value;
                    }
                    if (numericUpDownInputTrayQuantity3.Value == 0 && groupBoxLotID3.Visible == true)
                    {
                        richTextBoxMessage.Text += string.Format("{0} : Please enter the input tray quantity required for Lot 3.", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                        return;
                    }
                    else if (numericUpDownInputTrayQuantity3.Value != 0 && groupBoxLotID3.Visible == true)
                    {
                        m_CustomerShareVariables.InputLotTrayQuantity[2] = (int)numericUpDownInputTrayQuantity3.Value;

                    }
                }
                if (m_CustomerShareVariables.listLotID.Count > 1)
                {
                    nError = GetOutputIDName(m_CustomerShareVariables.listLotID, out m_CustomerShareVariables.strucInputProductInfo.LotIDOutput);
                    if (nError == 1)
                    {
                        richTextBoxMessage.Text += string.Format("{0} : Cav Number Is Different", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                        return;
                    }
                    if (nError == 2)
                    {
                        richTextBoxMessage.Text += string.Format("{0} : Date Of LotID More Than 3 days", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                        return;
                    }
                }
                else
                {
                    //m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = textBoxLotID1.Text.ToUpper();
                    nError = GetOutputIDNameFromSingleLot(m_CustomerShareVariables.listLotID, out m_CustomerShareVariables.strucInputProductInfo.LotIDOutput);
                    if (Directory.Exists(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput))
                    {
                        for (int i = 1; i < 100; i++)
                        {
                            if (Directory.Exists(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "_" + i.ToString()) == false)
                            {
                                m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "_" + i.ToString();
                                break;
                            }
                        }
                    }
                    if (Directory.Exists(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput))
                    {
                        for (int i = 1; i < 100; i++)
                        {
                            if (Directory.Exists(m_CustomerShareVariables.productRecipeOutputFileSettings.strOutputLocalFilePath + "\\Output\\" + m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "_" + i.ToString()) == false)
                            {
                                m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = m_CustomerShareVariables.strucInputProductInfo.LotIDOutput + "_" + i.ToString();
                                break;
                            }
                        }
                    }
                }
                #endregion
                m_CustomerRTSSProcess.SetProductionInt("nInputLotQuantity",m_CustomerShareVariables.InputLotQuantity[m_CustomerShareVariables.nLotIDNumber]);
                m_CustomerRTSSProcess.SetProductionInt("nInputLotTrayNo", m_CustomerShareVariables.InputLotTrayQuantity[m_CustomerShareVariables.nLotIDNumber]);
                //m_CustomerShareVariables.strucInputProductInfo = new Product.Input_Product_Info();
                
                //m_CustomerShareVariables.strucInputProductInfo.LotID = textBoxLotID1.Text.ToUpper();
                m_CustomerShareVariables.strucInputProductInfo.LotID = m_CustomerShareVariables.listLotID[m_CustomerShareVariables.nLotIDNumber].ToUpper();
                //temporary
                //m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = "temporaryID";
                //
                m_CustomerShareVariables.strucInputProductInfo.Recipe = m_CustomerShareVariables.currentMainRecipeName;
                m_CustomerShareVariables.strucInputProductInfo.dtStartDate = DateTime.Now;
                m_CustomerShareVariables.LotID = m_CustomerShareVariables.strucInputProductInfo.LotID;
                m_CustomerShareVariables.PartName = textBoxPartName.Text;
                m_CustomerShareVariables.PartNumber = textBoxPartNumber.Text;
                m_CustomerShareVariables.BuildName = textBoxBuild.Text;
                //Output Quantity (WC)
                //CustomerRTSSProcess.SetShareMemorySettingUInt("TotalOutputUnitQuantity", (uint)numericUpDownTotalOutputQuantity.Value);
                //m_CustomerShareVariables.strucInputProductInfo.LotID2 = textBoxLotID2.Text.ToUpper();

                //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //{
                //    m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = textBoxLotIDOutput.Text.ToUpper();

                //    if(m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true
                //        && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //        m_CustomerShareVariables.strucInputProductInfo.LotIDOutput2 = textBoxLotIDOutput2.Text.ToUpper();

                //    m_CustomerShareVariables.strucInputProductInfo.LotIDOutput2 = "";
                //}
                //else
                //{
                //    m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = m_CustomerShareVariables.strucInputProductInfo.LotID;
                //    m_CustomerShareVariables.strucInputProductInfo.LotIDOutput2 = m_CustomerShareVariables.strucInputProductInfo.LotID2;
                //}
                //m_CustomerShareVariables.strucInputProductInfo.WorkOrder = textBoxWorkOrder.Text.ToUpper();
                //m_CustomerShareVariables.strucInputProductInfo.Recipe = textBoxERPPart.Text;
                //m_CustomerShareVariables.strucInputProductInfo.WaferBin = textBoxWaferBin.Text.ToUpper();
                //m_CustomerShareVariables.strucInputProductInfo.PPLot = textBoxPPLot.Text.ToUpper();
                m_CustomerShareVariables.strucInputProductInfo.OperatorID = textBoxOperatorID.Text.ToUpper();
                //m_CustomerShareVariables.strucInputProductInfo.Shift = textBoxShift.Text.ToUpper();
                //m_CustomerShareVariables.strCurrentBarcodeID = "";

                //m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = textBoxLotID1.Text;

                if (m_CustomerProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0) == false)
                {
                    bool IsLotExists = false;
                    int nPreviousMatchLotNo = 0;
                    //m_CustomerShareVariables.dtProductionCurrentUnitStartTime = DateTime.Now;
                    LoadPreviousLotSettings();
                    foreach (LotDetail _Lot in m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo)
                    {
                        if (_Lot.LotID == m_CustomerShareVariables.strucInputProductInfo.LotID)
                        {
                            IsLotExists = true;
                            break;
                        }
                        nPreviousMatchLotNo++;
                    }
                    m_CustomerProcessEvent.PCS_PCS_Send_Vision_Setting.Set();
                    m_CustomerRTSSProcess.SetProductionInt("nCurrentInputTrayNo", 0);
                    //m_CustomerRTSSProcess.SetProductionInt("nCurrentBottomStationTrayNo", 0);
                    //m_CustomerRTSSProcess.SetProductionInt("nCurrentS3StationTrayNo", 0);
                    m_CustomerRTSSProcess.SetProductionInt("nCurrentOutputTrayNo", 0);
                    m_CustomerRTSSProcess.SetProductionInt("nCurrentRejectTrayNo", 0);
                    
                    m_CustomerShareVariables.PreviousReportTrayNo = 0;
                    m_CustomerShareVariables.nInputTrayNo = 0;
                    if (IsLotExists == false)
                    {
                        m_CustomerRTSSProcess.SetProductionInt("nEdgeCoordinateX", 1);
                        m_CustomerRTSSProcess.SetProductionInt("nEdgeCoordinateY", 1);
                        m_CustomerRTSSProcess.SetEvent("RMAIN_RTHD_IS_PREVIOUS_SAVE_LOT_MATCH", false);
                    }
                    else
                    {
                        m_CustomerRTSSProcess.SetProductionInt("nEdgeCoordinateX", m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].Row);
                        m_CustomerRTSSProcess.SetProductionInt("nEdgeCoordinateY", m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].Column);
                        m_CustomerRTSSProcess.SetProductionInt("nCurrentInputTrayNo", m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].nCurrentInputTrayNo);
                        m_CustomerShareVariables.PreviousReportTrayNo = m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].nCurrentInputTrayNo;
                        m_CustomerShareVariables.nInputTrayNo = m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].nCurrentInputTrayNo;
                        m_CustomerRTSSProcess.SetEvent("RMAIN_RTHD_IS_PREVIOUS_SAVE_LOT_MATCH", true);
                        m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo.RemoveAt(nPreviousMatchLotNo);
                        SaveContinueLotInfoAfterRemove();
                    }
                    m_CustomerShareVariables.TotalLotMES.Clear();
                    m_CustomerShareVariables.CurrentDownTimeCounterMES.Clear();
                    m_CustomerShareVariables.CurrentDownTimeNo = -1;
                    m_CustomerShareVariables.AlarmStart = 0;
                    m_CustomerShareVariables.PrintOutputTrayID.Clear();
                }
                else
                {
                    m_CustomerRTSSProcess.SetEvent("RINT_RSEQ_SWITCH_NEXT_INPUT_LOT", true);
                    m_CustomerRTSSProcess.SetProductionInt("nCurrentInputTrayNo", 0);
                    m_CustomerShareVariables.PreviousReportTrayNo = 0;
                    m_CustomerShareVariables.nInputTrayNo = 0;
                    m_CustomerRTSSProcess.SetEvent("RMAIN_RTHD_NEW_OR_END_LOT_CONDITION",false);
                    m_CustomerRTSSProcess.SetEvent("RMAIN_RTHD_ENDLOT_CONDITION", true);
                }
                m_CustomerRTSSProcess.SetProductionInt("nCurrentInputLotQuantityRun", 0);
                m_CustomerRTSSProcess.SetProductionInt("nCurrentInputLotTrayNoRun", 0);
                m_CustomerShareVariables.CurrentLotMES = new Lot();
                m_CustomerShareVariables.CurrentLotMES.lot_no = m_CustomerShareVariables.strucInputProductInfo.LotID;
                m_CustomerShareVariables.CurrentLotMES.good_qty = 0;
                m_CustomerShareVariables.CurrentLotMES.ng_qty = 0;
                m_CustomerShareVariables.CurrentDefectCounterMES.Clear();
                m_CustomerRTSSProcess.SetProductionInt("nLotIDNumber", m_CustomerShareVariables.nLotIDNumber);
                //m_CustomerProcessEvent.PCS_PCS_Send_Vision_NewLot.Set();
                //m_CustomerShareVariables.nNoOfAssistSinceCurrentLotStart = 0;
                //m_CustomerShareVariables.nNoOfFailureSinceCurrentLotStart = 0;
                m_CustomerProcessEvent.GUI_PCS_NewLotDone.Set();
                m_CustomerProcessEvent.GUI_PCS_NewLotDone2.Set();
                m_CustomerProcessEvent.PCS_GUI_CreateFrameSelection.Set();
                m_CustomerRTSSProcess.SetEvent("RMAIN_RTHD_START_RUNNING", true);
                m_CustomerProcessEvent.PCS_PCS_StartReadInputFile.Set();
                m_CustomerShareVariables.PreviousReportLotID = "";
                m_CustomerProcessEvent.GUI_GUI_GET_DATA.Reset();
                //m_CustomerShareVariables.IsNewLotDone = true;
                //m_CustomerReportProcess.AddRecord(new ReportProcess.Record { dateTime = DateTime.Now, eventID = m_ProductShareVariables.productReportEvent.EventUnloadMaterialTime.EventID, eventName = m_ProductShareVariables.productReportEvent.EventUnloadMaterialTime.EventName, lotID = m_ProductShareVariables.LotID, alarmID = 0, alarmType = 0 }); ocess.AddRecord(new ReportProcess.Record { dateTime = DateTime.Now, eventID = m_ProductShareVariables.productReportEvent.EventUnloadMaterialTime.EventID, eventName = m_ProductShareVariables.productReportEvent.EventUnloadMaterialTime.EventName, lotID = m_ProductShareVariables.LotID, alarmID = 0, alarmType = 0 });
                m_CustomerReportProcess.AddRecord(new ReportProcess.Record { dateTime = DateTime.Now, eventID = m_CustomerShareVariables.customerReportEvent.EventLoadMaterialTime.EventID, eventName = m_CustomerShareVariables.customerReportEvent.EventLoadMaterialTime.EventName, lotID = m_CustomerShareVariables.strucInputProductInfo.LotID, alarmID = 0, alarmType = 0 });
                if (m_bgwUserInterface.WorkerSupportsCancellation == true)
                {
                    m_bgwUserInterface.CancelAsync();
                }
               
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if(m_CustomerProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0)==true)
                {
                    m_CustomerShareVariables.listLotID = m_CustomerShareVariables.RecoverlistLotID;
                    m_CustomerShareVariables.InputLotQuantity = m_CustomerShareVariables.RecoverInputLotQuantity;
                    m_CustomerShareVariables.InputLotTrayQuantity = m_CustomerShareVariables.RecoverInputLotTrayQuantity;
                    m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = m_CustomerShareVariables.RecoverLotIDOutput;
                }
                m_CustomerProcessEvent.GUI_PCS_NewLotCancel.Set();
                if (m_bgwUserInterface.WorkerSupportsCancellation == true)
                {
                    m_bgwUserInterface.CancelAsync();
                }                
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
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
                        OutputName = LotIDAfterSplit[0] + "_" + Tool + Cav + Year + Month + nMinDay.ToString("D2") + nMaxDay.ToString("D2");
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
                m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = OutputName;
            }
            catch (Exception ex)
            {
                return -1;
            }
            return nError;
        }

        public int GetOutputIDNameFromSingleLot(List<string> LotIDName, out string OutputName)
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
                        if (int.TryParse(LotIDAfterSplit[1].Substring(4, 2), out nCurrentDay) == false)
                        {
                            OutputName = LotIDName[0];
                            return 0;
                        }
                        OutputName = LotIDAfterSplit[0] + "_" + Tool + Cav + Year + Month + nCurrentDay.ToString("D2") + nCurrentDay.ToString("D2");
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
                m_CustomerShareVariables.strucInputProductInfo.LotIDOutput = OutputName;
            }
            catch (Exception ex)
            {
                return -1;
            }
            return nError;
        }
        private void richTextBoxMessage_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Machine.GeneralTools.RichTextBoxScrollToCaret(richTextBoxMessage);
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

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
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        public void bgwUserInterface_UpdateInterface(Object sender, ProgressChangedEventArgs e)
        {
            try
            {
                //if (textBoxLotID1.Text != "" && textBoxLotID2.Visible == false)
                //{
                //    textBoxLotID2.Visible = true;
                //}
                //if (textBoxLotID2.Text != "" && textBoxLotID3.Visible == false)
                //{
                //    textBoxLotID3.Visible = true;
                //}
                //if (textBoxLotID3.Text != "" && textBoxLotID4.Visible == false)
                //{
                //    textBoxLotID4.Visible = true;
                //}
                //if (textBoxLotID4.Text != "" && textBoxLotID5.Visible == false)
                //{
                //    textBoxLotID5.Visible = true;
                //}
                //if (textBoxLotID5.Text != "" && textBoxLotID6.Visible == false)
                //{
                //    textBoxLotID6.Visible = true;
                //}
                //if (textBoxLotID6.Text != "" && textBoxLotID7.Visible == false)
                //{
                //    textBoxLotID7.Visible = true;
                //}
                //if (textBoxLotID7.Text != "" && textBoxLotID8.Visible == false)
                //{
                //    textBoxLotID8.Visible = true;
                //}
                //if (textBoxLotID8.Text != "" && textBoxLotID9.Visible == false)
                //{
                //    textBoxLotID9.Visible = true;
                //}
                //if (textBoxLotID9.Text != "" && textBoxLotID10.Visible == false)
                //{
                //    textBoxLotID10.Visible = true;
                //}
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
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
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        #endregion

        #region Input XY Table
        private void textBoxOperatorID_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    //EventLogger.WriteLog(string.Format("{0} Click Scan New Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
                //    textBoxLotID1.Focus();
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textBoxLotID1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    textBoxLotID2.Visible = true;
                //    textBoxLotID2.Focus();
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }

        private void textBoxLotID2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    textBoxLotID3.Visible = true;
                //    textBoxLotID3.Focus();
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textBoxLotID3_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    textBoxLotID4.Visible = true;
                //    textBoxLotID4.Focus();
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textBoxLotID4_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    textBoxLotID5.Visible = true;
                //    textBoxLotID5.Focus();
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textBoxLotID5_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    textBoxLotID6.Visible = true;
                //    textBoxLotID6.Focus();
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textBoxLotID6_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    textBoxLotID7.Visible = true;
                //    textBoxLotID7.Focus();
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textBoxLotID7_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    textBoxLotID8.Visible = true;
                //    textBoxLotID8.Focus();
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textBoxLotID8_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    textBoxLotID9.Visible = true;
                //    textBoxLotID9.Focus();
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textBoxLotID9_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    textBoxLotID10.Visible = true;
                //    textBoxLotID10.Focus();
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        private void textBoxLotID10_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
                //{
                //    richTextBoxMessage.Text += string.Format("{0}  LotID reach Limit at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode);
                //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
                //    //{
                //    //    textBoxLotID2.Focus();
                //    //}
                //    //else
                //    //{
                //    //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
                //    //    {
                //    //        textBoxLotIDOutput.Focus();
                //    //    }
                //    //    else
                //    //    {
                //    //        textBoxWorkOrder.Focus();
                //    //    }
                //    //}
                //}
                //keysPrevious = e.KeyData;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        //private void textBoxLotIDOutput_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
        //        {
        //            //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
        //            //{
        //            //    textBoxLotIDOutput2.Focus();
        //            //}
        //            //else
        //            //{
        //                textBoxWorkOrder.Focus();
        //            //}
        //        }
        //        keysPrevious = e.KeyData;
        //    }
        //    catch (Exception ex)
        //    {
        //        richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void textBoxLotID2_KeyUp(object sender, KeyEventArgs e)
        //{

        //}

        //private void textBoxLotIDOutput2_KeyUp(object sender, KeyEventArgs e)
        //{

        //}
        //private void textBoxWorkOrder_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
        //        {
        //            //EventLogger.WriteLog(string.Format("{0} Click Scan New Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
        //            textBoxERPPart.Focus();
        //        }
        //        keysPrevious = e.KeyData;
        //    }
        //    catch (Exception ex)
        //    {
        //        richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void textBoxERPPart_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
        //        {
        //            //EventLogger.WriteLog(string.Format("{0} Click Scan New Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
        //            textBoxWaferBin.Focus();
        //        }
        //        keysPrevious = e.KeyData;
        //    }
        //    catch (Exception ex)
        //    {
        //        richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void textBoxWaferBin_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
        //        {
        //            //EventLogger.WriteLog(string.Format("{0} Click Scan New Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
        //            textBoxPPLot.Focus();
        //        }
        //        keysPrevious = e.KeyData;
        //    }
        //    catch (Exception ex)
        //    {
        //        richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        //private void textBoxPPLot_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
        //        {
        //            //EventLogger.WriteLog(string.Format("{0} Click Scan New Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
        //            textBoxOperatorID.Focus();
        //        }
        //        keysPrevious = e.KeyData;
        //    }
        //    catch (Exception ex)
        //    {
        //        richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}

        

        //private void textBoxShift_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab || (e.KeyData == Keys.D0 && keysPrevious == Keys.Menu) || (e.KeyData == Keys.D0 && keysPrevious == Keys.LWin))
        //        {
        //            //EventLogger.WriteLog(string.Format("{0} Click Scan New Lot at {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), m_strmode));
        //            buttonOK.Focus();
        //        }
        //        keysPrevious = e.KeyData;
        //    }
        //    catch (Exception ex)
        //    {
        //        richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
        //        Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
        //    }
        //}
        #endregion Input XY Table

        #endregion Form Events

        #region private

        private void AutoCreateFolderFile()
        {
            
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
                m_bgwUserInterface.RunWorkerAsync();
                textBoxOperatorID.Focus();
                if(m_CustomerShareVariables.productOptionSettings.EnableMES==true)
                {
                    //numericUpDownInputQuantity1.Visible = false;
                    //numericUpDownInputQuantity2.Visible = false;
                    //numericUpDownInputQuantity3.Visible = false;
                    //labelUnitQty1.Visible = false;
                    labelUnitQty2.Visible = false;
                    labelUnitQty3.Visible = false;
                    //buttonVerifyMatchLot.Visible = false;
                }
                else
                {
                    //numericUpDownInputQuantity1.Visible = true;
                    //numericUpDownInputQuantity2.Visible = true;
                    //numericUpDownInputQuantity3.Visible = true;
                    //labelUnitQty1.Visible = true;
                    labelUnitQty2.Visible = true;
                    labelUnitQty3.Visible = true;
                    //buttonVerifyMatchLot.Visible = true;
                }
                if(m_CustomerShareVariables.productOptionSettings.EnableCountDownByInputTrayNo == true)
                {
                    numericUpDownInputTrayQuantity1.Visible = true;
                    numericUpDownInputTrayQuantity2.Visible = true;
                    numericUpDownInputTrayQuantity3.Visible = true;
                    labelTrayQty1.Visible = true;
                    labelTrayQty2.Visible = true;
                    labelTrayQty3.Visible = true;
                }
                else
                {
                    numericUpDownInputTrayQuantity1.Visible = false;
                    numericUpDownInputTrayQuantity2.Visible = false;
                    numericUpDownInputTrayQuantity3.Visible = false;
                    labelTrayQty1.Visible = false;
                    labelTrayQty2.Visible = false;
                    labelTrayQty3.Visible = false;
                }
                groupBoxLotID1.Visible = true;
                groupBoxLotID2.Visible = false;
                groupBoxLotID3.Visible = false;
                m_CustomerShareVariables.nLotIDNumber = 0;
                m_CustomerShareVariables.LastLotIDOutput = "";
                if (m_CustomerProcessEvent.PCS_PCS_Current_Lot_Is_Running.WaitOne(0) == true)
                {
                    if (m_CustomerShareVariables.listLotID != null && m_CustomerShareVariables.listLotID.Count > 0)
                    {
                        int No = 0;
                        foreach (var m_ListID in m_CustomerShareVariables.listLotID)
                        {
                            No++;
                            if (No == 1)
                            {
                                m_CustomerShareVariables.nLotIDNumber = 1;
                                textBoxLotID1.Text = m_ListID;
                                textBoxLotID1.Enabled = false;
                                //numericUpDownInputQuantity1.Value = m_CustomerShareVariables.InputLotQuantity[0];
                                //numericUpDownInputQuantity1.Enabled = false;
                                numericUpDownInputTrayQuantity1.Value = m_CustomerShareVariables.InputLotTrayQuantity[0];
                                numericUpDownInputTrayQuantity1.Enabled = false;
                                groupBoxLotID2.Visible = true;
                            }
                            else if (No == 2)
                            {
                                m_CustomerShareVariables.nLotIDNumber = 2;
                                textBoxLotID2.Enabled = false;
                                textBoxLotID2.Text = m_ListID;
                                numericUpDownInputQuantity2.Value = m_CustomerShareVariables.InputLotQuantity[1];
                                numericUpDownInputQuantity2.Enabled = false;
                                numericUpDownInputTrayQuantity2.Value = m_CustomerShareVariables.InputLotTrayQuantity[1];
                                numericUpDownInputTrayQuantity2.Enabled = false;
                                groupBoxLotID3.Visible = true;
                            }
                            else if (No == 3)
                            {
                                textBoxLotID3.Text = m_ListID;
                                textBoxLotID3.Enabled = false;
                                numericUpDownInputQuantity3.Value = m_CustomerShareVariables.InputLotQuantity[2];
                                numericUpDownInputQuantity3.Enabled = false;
                                numericUpDownInputTrayQuantity3.Value = m_CustomerShareVariables.InputLotTrayQuantity[2];
                                numericUpDownInputTrayQuantity3.Enabled = false;
                            }
                            //else if (No == 4)
                            //{
                            //    textBoxLotID4.Text = m_ListID;
                            //}
                            //else if (No == 5)
                            //{
                            //    textBoxLotID5.Text = m_ListID;
                            //}
                            //else if (No == 6)
                            //{
                            //    textBoxLotID6.Text = m_ListID;
                            //}
                            //else if (No == 7)
                            //{
                            //    textBoxLotID7.Text = m_ListID;
                            //}
                            //else if (No == 8)
                            //{
                            //    textBoxLotID8.Text = m_ListID;
                            //}
                            //else if (No == 9)
                            //{
                            //    textBoxLotID9.Text = m_ListID;
                            //}
                            //else if (No == 10)
                            //{
                            //    textBoxLotID10.Text = m_ListID;
                            //}
                        }
                        buttonVerifyMatchLot.Visible = false;
                        textBoxOperatorID.Text = m_CustomerShareVariables.strucInputProductInfo.OperatorID;
                        textBoxOperatorID.Enabled = false;
                        textBoxPartName.Text = m_CustomerShareVariables.PartName;
                        textBoxPartName.Enabled = false;
                        textBoxPartNumber.Text = m_CustomerShareVariables.PartNumber;
                        textBoxPartNumber.Enabled = false;
                        textBoxBuild.Text = m_CustomerShareVariables.BuildName;
                        textBoxBuild.Enabled = false;
                        //numericUpDownTotalOutputQuantity.Value = (decimal)CustomerRTSSProcess.GetShareMemorySettingUInt("TotalOutputUnitQuantity");
                        //numericUpDownTotalOutputQuantity.Enabled = false;
                        m_CustomerShareVariables.RecoverlistLotID = m_CustomerShareVariables.listLotID;
                        m_CustomerShareVariables.RecoverInputLotQuantity = m_CustomerShareVariables.InputLotQuantity;
                        m_CustomerShareVariables.RecoverInputLotTrayQuantity = m_CustomerShareVariables.InputLotTrayQuantity;
                        m_CustomerShareVariables.RecoverLotIDOutput = m_CustomerShareVariables.strucInputProductInfo.LotIDOutput;
                        m_CustomerShareVariables.LastLotIDOutput = m_CustomerShareVariables.strucInputProductInfo.LotIDOutput;
                        m_CustomerShareVariables.bolFirstTimeCreateOutputFileAfterCombineLot = true;
                    }
                }
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
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
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return false;
            }
        }

        virtual public void UpdateGUI()
        {
           
            //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableLotID2 == true && m_CustomerShareVariables.productRecipeOutputFileSettings.nFileFormat == 0)
            //{
            //    labelMESLot2.Visible = true;
            //    textBoxLotID2.Visible = true;
            //    if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
            //    {
            //        labelMESLotOutput.Visible = true;
            //        textBoxLotIDOutput.Visible = true;

            //        labelMESLotOutput2.Visible = true;
            //        textBoxLotIDOutput2.Visible = true;
            //    }
            //    else
            //    {
            //        labelMESLotOutput.Visible = false;
            //        textBoxLotIDOutput.Visible = false;

            //        labelMESLotOutput2.Visible = false;
            //        textBoxLotIDOutput2.Visible = false;
            //    }
            //}
            //else
            //{
            //    labelMESLot2.Visible = false;
            //    textBoxLotID2.Visible = false;
            //    labelMESLotOutput2.Visible = false;
            //    textBoxLotIDOutput2.Visible = false;
            //    //if (m_CustomerShareVariables.productRecipeOutputFileSettings.bEnableDifferentInputOutputLotID == true)
            //    //{
            //    //    labelMESLotOutput.Visible = true;
            //    //    textBoxLotIDOutput.Visible = true;

            //    //}
            //    //else
            //    //{
            //    //    labelMESLotOutput.Visible = false;
            //    //    textBoxLotIDOutput.Visible = false;
            //    //}
            //}
        }

        virtual public bool IscContainInvalidCharacter(string input)
        {
            try
            {
                if (input.Contains("#")
                || input.Contains("$")
                || input.Contains("%")
                || input.Contains("*")
                || input.Contains("+")
                //|| input.Contains("-")
                || input.Contains(".")
                || input.Contains(",")
                || input.Contains("/")
                || input.Contains("!")
                || input.Contains("&")
                || input.Contains("'")
                || input.Contains("(")
                || input.Contains(")")
                || input.Contains(":")
                || input.Contains(";")
                || input.Contains("<")
                || input.Contains("=")
                || input.Contains(">")
                || input.Contains("?")
                || input.Contains("@")
                || input.Contains("\\")
                || input.Contains("^")
                //|| input.Contains("_")
                || input.Contains("{")
                || input.Contains("|")
                || input.Contains("}")
                || input.Contains("~")
                || input.Contains("`")
                || input.Contains("\"")
                || input.Contains("{")
                || input.Contains("}")
                || input.Contains(" ")
                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return true;
            }
        }

        public int LoadPreviousLotSettings()
        {
            int nError = 0;
            try
            {
                if (Tools.IsFileExist(m_CustomerShareVariables.strSaveLotInfoPath,"LotData", ".xml"))
                {
                    m_CustomerShareVariables.productContinueLotInfo = Tools.Deserialize<ProductContinueLotInfo>(m_CustomerShareVariables.strSaveLotInfoPath+"LotData.xml");
                }
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
            return nError;
        }

        public int ReadMESInputData()
        {
            int nError = 0;
            try
            {
                for (int nRetry = 1; nRetry <= 1; nRetry++)
                {
                    nError = MoveonMESAPI.MESAPI.Get();
                    if(nError !=0)
                    {
                        //Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Get Input from MES Fail, StatusMessage: " + MoveonMESAPI.MESAPI.StatusMessage + ", ErrorMessage: " + MoveonMESAPI.MESAPI.ErrorMessage + "\n"));
                    }
                    else
                    {
                        Machine.DebugLogger.WriteLog(string.Format("{0} {1}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Get Input from MES Done, StatusMessage: " + MoveonMESAPI.MESAPI.StatusMessage + "\n"));
                        InputLotMES = MoveonMESAPI.MESAPI.InputResult.ToObject<InputData>();
                        break;
                    }
                }
                if(nError!=0)
                {
                    richTextBoxMessage.Text += string.Format("{0} {1}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Get Input from MES Fail, StatusMessage: " + MoveonMESAPI.MESAPI.StatusMessage + ", ErrorMessage: " + MoveonMESAPI.MESAPI.ErrorMessage + "\n");
                }
            }
            catch(Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                //Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
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
                if (nError != 0)
                {
                    //Machine.DebugLogger.WriteLog(string.Format("{0} {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Configure MES URL fail" + MoveonMESAPI.MESAPI.ErrorMessage,m_strmode));
                    richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Configure MES URL fail" + MoveonMESAPI.MESAPI.ErrorMessage, m_strmode);
                }
                else
                {
                    //Machine.DebugLogger.WriteLog(string.Format("{0} {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Configure MES URL Done",m_strmode));
                }
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0} {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(),m_strmode));
                return -1;
            }
            return nError;
        }
        public int SettingURL()
        {
            int nError = 0;
            try
            {
                m_CustomerShareVariables.MESInputURLUsed = m_CustomerShareVariables.productOptionSettings.MESInputURL + m_CustomerShareVariables.strucInputProductInfo.Shift;
                m_CustomerShareVariables.MESOutputURLUsed = m_CustomerShareVariables.productOptionSettings.MESOutputURL;
                m_CustomerShareVariables.MESEndJobURLUsed = m_CustomerShareVariables.productOptionSettings.MESEndJobURL;
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
            return nError;
        }

        public int CompareMESInputWithLotIDEntered ()
        {
            int nError = 0;
            try
            {
                //if(m_CustomerShareVariables.listLotID.Count != InputLotMES.items.Length)
                //{
                //    richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "The numbers of Lot Scanned at AOI Machine Not Tally with MES", m_strmode);
                //    return -1;
                //}
                if(InputLotMES.items.Length<=0)
                {
                    richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Input MES is empty", m_strmode);
                    return -1;
                }
                for (int i =0; i<m_CustomerShareVariables.listLotID.Count;i++)
                {
                    if (InputLotMES.items[i].lot_no != m_CustomerShareVariables.listLotID[i])
                    {
                        richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Lots Entered at AOI Machine Do Not Match with MES", m_strmode);
                        //Machine.DebugLogger.WriteLog(string.Format("{0} {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Lots Entered at AOI Machine Do Not Match with MES", m_strmode));
                        return -1;
                    }
                }
                for (int i =0;i< m_CustomerShareVariables.listLotID.Count; i++)
                {
                   m_CustomerShareVariables.InputLotQuantity[i] = InputLotMES.items[i].input_qty;
				   //Machine.DebugLogger.WriteLog(string.Format("{0} {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Input Quantity For "+m_CustomerShareVariables.listLotID[i] + m_CustomerShareVariables.InputLotQuantity[i].ToString(), m_strmode));
                }

            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0} {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
            return nError;
        }
        public int SaveContinueLotInfoAfterRemove()
        {
            int nError = 0;
            try
            {
                Tools.Serialize(m_CustomerShareVariables.strSaveLotInfoPath + "LotData.xml", m_CustomerShareVariables.productContinueLotInfo);
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            return nError;
        }
        #endregion public function

        public void buttonVerifyMatchLot_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBoxLotID1.Text == "")
                {
                    richTextBoxMessage.Text += string.Format("{0} : Please enter Lot ID", DateTime.Now.ToString("yyyyMMdd HHmmss")) + Environment.NewLine;
                    //MessageBox.Show("Please enter Lot ID");
                    return;
                }
                bool IsLotExists = false;
                int nPreviousMatchLotNo = 0;
                //m_CustomerShareVariables.dtProductionCurrentUnitStartTime = DateTime.Now;
                LoadPreviousLotSettings();
                foreach (LotDetail _Lot in m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo)
                {
                    if (_Lot.LotID == textBoxLotID1.Text)
                    {
                        IsLotExists = true;
                        break;
                    }
                    nPreviousMatchLotNo++;
                }
                if (IsLotExists == false)
                {
                    richTextBoxMessage.Text += string.Format("{0}  {1}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), "No Lot Match");
                }
                else
                {
                    if (m_CustomerShareVariables.productOptionSettings.EnableMES == false)
                    {
                        richTextBoxMessage.Text += string.Format("{0}  {1}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Lot Match With Previous Lot, previous input balance = " + m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].InputBalance.ToString());
                    }
                    if (m_CustomerShareVariables.productOptionSettings.EnableCountDownByInputTrayNo == true && m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].InputTrayNo>0)
                    {
                        richTextBoxMessage.Text += string.Format("{0}  {1}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Previous input tray left quantity = " + m_CustomerShareVariables.productContinueLotInfo.PreviousLotInfo[nPreviousMatchLotNo].InputTrayNo.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                richTextBoxMessage.Text += string.Format("{0}  {1} at {2}.\n", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode);
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return;
            }
        }
    }
}
