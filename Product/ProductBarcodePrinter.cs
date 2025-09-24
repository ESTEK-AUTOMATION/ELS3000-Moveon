using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;//This is about DllImport.
using System.Threading;
using Common;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

namespace Product
{
    public class ProductBarcodePrinter
    {
        const uint IMAGE_BITMAP = 0;
        const uint LR_LOADFROMFILE = 16;
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType,
           int cxDesired, int cyDesired, uint fuLoad);
        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int DeleteObject(IntPtr ho);
        const string szSavePath = "C:\\Argox";
        const string szSaveFile = "C:\\Argox\\PPLB_Example.Prn";
        const string sznop1 = "nop_front\r\n";
        const string sznop2 = "nop_middle\r\n";
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_Maxi(int x, int y, int cl, int cc, int pc, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_PDF417(int x, int y, int w, int v, int s, int c, int px,
            int py, int r, int l, int t, int o, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_PDF417_N(int x, int y, int w, int h, string pParameter, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_DataMatrix(int x, int y, int r, int l, int h, int v, string data);
        [DllImport("Winpplb.dll")]
        private static extern void B_ClosePrn();
        [DllImport("Winpplb.dll")]
        private static extern int B_CreatePrn(int selection, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Del_Form(string formname);
        [DllImport("Winpplb.dll")]
        private static extern int B_Del_Pcx(string pcxname);
        [DllImport("Winpplb.dll")]
        private static extern int B_Draw_Box(int x, int y, int thickness, int hor_dots,
            int ver_dots);
        [DllImport("Winpplb.dll")]
        private static extern int B_Draw_Line(char mode, int x, int y, int hor_dots, int ver_dots);
        [DllImport("Winpplb.dll")]
        private static extern int B_Error_Reporting(char option);
        [DllImport("Winpplb.dll")]
        private static extern IntPtr B_Get_DLL_Version(int nShowMessage);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_DLL_VersionA(int nShowMessage);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_Graphic_ColorBMP(int x, int y, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_Graphic_ColorBMPEx(int x, int y, int nWidth, int nHeight,
            int rotate, string id_name, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_Graphic_ColorBMP_HBitmap(int x, int y, int nWidth, int nHeight,
           int rotate, string id_name, IntPtr hbm);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_Pcx(int x, int y, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Initial_Setting(int Type, string Source);
        [DllImport("Winpplb.dll")]
        private static extern int B_WriteData(int IsImmediate, byte[] pbuf, int length);
        [DllImport("Winpplb.dll")]
        private static extern int B_ReadData(byte[] pbuf, int length, int dwTimeoutms);
        [DllImport("Winpplb.dll")]
        private static extern int B_Load_Pcx(int x, int y, string pcxname);
        [DllImport("Winpplb.dll")]
        private static extern int B_Open_ChineseFont(string path);
        [DllImport("Winpplb.dll")]
        private static extern int B_Print_Form(int labset, int copies, string form_out, string var);
        [DllImport("Winpplb.dll")]
        private static extern int B_Print_MCopy(int labset, int copies);
        [DllImport("Winpplb.dll")]
        private static extern int B_Print_Out(int labset);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Barcode(int x, int y, int ori, string type, int narrow,
            int width, int height, char human, string data);
        [DllImport("Winpplb.dll")]
        private static extern void B_Prn_Configuration();
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text(int x, int y, int ori, int font, int hor_factor,
            int ver_factor, char mode, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_Chinese(int x, int y, int fonttype, string id_name,
            string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_TrueType(int x, int y, int FSize, string FType,
            int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut, string id_name,
            string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_TrueType_W(int x, int y, int FHeight, int FWidth,
            string FType, int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut,
            string id_name, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Select_Option(int option);
        [DllImport("Winpplb.dll")]
        private static extern int B_Select_Option2(int option, int p);
        [DllImport("Winpplb.dll")]
        private static extern int B_Select_Symbol(int num_bit, int symbol, int country);
        [DllImport("Winpplb.dll")]
        private static extern int B_Select_Symbol2(int num_bit, string csymbol, int country);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Backfeed(char option);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Backfeed_Offset(int offset);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_CutPeel_Offset(int offset);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_BMPSave(int nSave, string strBMPFName);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Darkness(int darkness);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_DebugDialog(int nEnable);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Direction(char direction);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Form(string formfile);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Labgap(int lablength, int gaplength);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Labwidth(int labwidth);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Originpoint(int hor, int ver);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Prncomport(int baud, char parity, int data, int stop);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Prncomport_PC(int nBaudRate, int nByteSize, int nParity,
            int nStopBits, int nDsr, int nCts, int nXonXoff);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Speed(int speed);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_ProcessDlg(int nShow);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_ErrorDlg(int nShow);
        [DllImport("Winpplb.dll")]
        private static extern int B_GetUSBBufferLen();
        [DllImport("Winpplb.dll")]
        private static extern int B_EnumUSB(byte[] buf);
        [DllImport("Winpplb.dll")]
        private static extern int B_CreateUSBPort(int nPort);
        [DllImport("Winpplb.dll")]
        private static extern int B_ResetPrinter();
        [DllImport("Winpplb.dll")]
        private static extern int B_GetPrinterResponse(byte[] buf, int nMax);
        [DllImport("Winpplb.dll")]
        private static extern int B_TFeedMode(int nMode);
        [DllImport("Winpplb.dll")]
        private static extern int B_TFeedTest();
        [DllImport("Winpplb.dll")]
        private static extern int B_CreatePort(int nPortType, int nPort, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Execute_Form(string form_out, string var);
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_QR(int x, int y, int model, int scl, char error,
            char dinput, int c, int d, int p, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_GetNetPrinterBufferLen();
        [DllImport("Winpplb.dll")]
        private static extern int B_EnumNetPrinter(byte[] buf);
        [DllImport("Winpplb.dll")]
        private static extern int B_CreateNetPort(int nPort);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_TrueType_Uni(int x, int y, int FSize, string FType,
            int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut, string id_name,
            byte[] data, int format);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_TrueType_UniB(int x, int y, int FSize, string FType,
            int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut, string id_name,
            byte[] data, int format);
        [DllImport("Winpplb.dll")]
        private static extern int B_GetUSBDeviceInfo(int nPort, byte[] pDeviceName,
            out int pDeviceNameLen, byte[] pDevicePath, out int pDevicePathLen);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_EncryptionKey(string encryptionKey);
        [DllImport("Winpplb.dll")]
        private static extern int B_Check_EncryptionKey(string decodeKey, string encryptionKey,
            int dwTimeoutms);
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
        public static string m_strBarcodePrinterResponse = "";
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
        public void BarcodePrinterThread()
        {
            try
            {
                while (m_bRunThread)
                {
                    int nError;
                    int labLength =0;
                    int gapLength=0;
                    int labWidth=0;
                    int DataMatrixInitialXPosition = 0;
                    int DataMatrixInitialYPosition = 0;
                    int font = 0;
                    System.Text.Encoding encAscII = System.Text.Encoding.ASCII;

                    if (m_ProductRTSSProcess.GetEvent("RMAIN_RTHD_TRIGGER_BARCODE_PRINTER") == true || m_ProductShareVariables.PrintTemporary == true)
                    {
                        m_ProductRTSSProcess.SetEvent("RMAIN_RTHD_TRIGGER_BARCODE_PRINTER", false);


                        nError = B_CreatePrn(13, "192.168.2.103:9100");
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to open LAN port.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        // sample setting.
                        nError = B_Set_DebugDialog(0);
                        if(nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Debug Dialog.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        nError = B_Set_Originpoint(0, 0);
                        if(nError!=0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Origin Point.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        nError = B_Select_Option((int)(m_ProductShareVariables.productOptionSettings.BarcodePrinterRibbonSensor + 1));
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Settings of Printer.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        nError = B_Set_Darkness(15);
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Darkness.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        nError = B_Del_Pcx("*");// delete all picture.
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Delete Previous Image.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        //if(m_ProductShareVariables.productRecipeOutputSettings.BarcodeLabelSize == 0) //70mmx30mm
                        //{
                        //    labLength = (int)(30*0.0393701*203); //240
                        //    gapLength = (int)(1.3 * 0.0393701 * 203); //10
                        //    labWidth = (int)(70*0.0393701*203); //559
                        //    DataMatrixInitialXPosition = 430;
                        //    DataMatrixInitialYPosition = 80;
                        //    font = 2;
                        //}
                        //else if(m_ProductShareVariables.productRecipeOutputSettings.BarcodeLabelSize == 1) // 30mmX40mm, 1mm = 0.0393701 inches
                        {
                            labLength = (int)(40 * 0.0393701 * 203); //320
                            gapLength = (int)(1.3 * 0.0393701 * 203); //10
                            //labWidth = (int)(40 * 0.0393701 * 203); //320
                            labWidth = (int)(40 * 0.0393701 * 203); //320
                            DataMatrixInitialXPosition = 110;
                            //DataMatrixInitialXPosition = 191;
                            DataMatrixInitialYPosition = 200;
                            //DataMatrixInitialYPosition = 80;
                            font = 1;
                        }
                        nError = B_Set_Labgap(labLength, gapLength); //label length & gap
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Label Length And Gap.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        nError = B_Set_Labwidth(labWidth); //label width
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Label Width.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        nError = B_WriteData(0, encAscII.GetBytes(sznop2), sznop2.Length);
                        nError += B_WriteData(1, encAscII.GetBytes(sznop1), sznop1.Length);
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Write Data.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        //draw box.
                        //nError = B_Draw_Box(10, 10, 4, 539, 220);
                        //if (nError != 0)
                        //{
                        //    Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Draw Box.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        //}
                        //nError = B_Draw_Line('O', 400, 10, 4, 210);
                        //if (nError != 0)
                        //{
                        //    Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Draw Line.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        //}
                        //print text
                        //if (m_ProductShareVariables.productRecipeOutputSettings.BarcodeLabelSize == 1) //30x40
                        {
                            nError = B_Prn_Text(2, 20, 0, font, 1, 1, 'N', "Supplier: ");
                            nError = B_Prn_Text(114, 20, 0, font, 1, 1, 'N', ": Crystal Moveon");
                            //nError = B_Prn_Text(114, 40, 0, font, 1, 1, 'N', "  Technologies");
                            nError = B_Prn_Text(2, 40, 0, font, 1, 1, 'N', "Part Name");
                            nError = B_Prn_Text(114, 40, 0, font, 1, 1, 'N', ": " + m_ProductShareVariables.PartName);
                            nError += B_Prn_Text(2, 60, 0, font, 1, 1, 'N', "Part Number");
                            nError += B_Prn_Text(114, 60, 0, font, 1, 1, 'N', ": " + m_ProductShareVariables.PartNumber);
                            //nError += B_Prn_Text(5, 40, 0, font, 1, 1, 'N', "Part #: " + m_ProductShareVariables.PartNumber);
                            nError += B_Prn_Text(2, 80, 0, font, 1, 1, 'N', "Build");
                            nError += B_Prn_Text(114, 80, 0, font, 1, 1, 'N', ": " + m_ProductShareVariables.BuildName);
                            //nError += B_Prn_Text(5, 60, 0, font, 1, 1, 'N', "Build: "+ m_ProductShareVariables.BuildName);
                            nError += B_Prn_Text(2, 100, 0, font, 1, 1, 'N', "Quantity");
                            if(m_ProductShareVariables.PrintTemporary==true)
                            {
                                //m_ProductShareVariables.PrintTemporary = false;
                                nError += B_Prn_Text(114, 100, 0, font, 1, 1, 'N', ": " + m_ProductShareVariables.TemporaryNumber+ " pcs");
                            }
                            else
                            {
                                nError += B_Prn_Text(114, 100, 0, font, 1, 1, 'N', ": " + Convert.ToString(GetShareMemoryProductionInt("nCurrentTotalUnitDone")) + " pcs");
                            }
                            //nError += B_Prn_Text(5, 80, 0, font, 1, 1, 'N', "Quantity: " + Convert.ToString(GetShareMemoryProductionInt("nCurrentTotalUnitDone"))+" pcs");
                            nError += B_Prn_Text(2, 120, 0, font, 1, 1, 'N', "AOI Date");

                            nError += B_Prn_Text(114, 120, 0, font, 1, 1, 'N', ": " + DateTime.Now.ToString("dd-MMM-yy"));
                            //temporary date
                            //nError += B_Prn_Text(114, 220, 0, font, 1, 1, 'N', ": NA");
                            //nError += B_Prn_Text(5, 100, 0, font, 1, 1, 'N', "AOI Date: " + DateTime.Now.ToString("dd/MM/yyyy"));
                            nError += B_Prn_Text(2, 140, 0, font, 1, 1, 'N', "Lot #");
                            nError += B_Prn_Text(114, 140, 0, font, 1, 1, 'N', ": " + m_ProductShareVariables.strucInputProductInfo.LotIDOutput);
                            //nError += B_Prn_Text(5, 120, 0, font, 1, 1, 'N', "Lot #: " + m_ProductShareVariables.strucInputProductInfo.LotIDOutput);
                            nError += B_Prn_Text(2, 160, 0, font, 1, 1, 'N', "AOI #");
                            nError += B_Prn_Text(114, 160, 0, font, 1, 1, 'N', ": "+m_ProductShareVariables.productOptionSettings.MachineNo);
                            //nError += B_Prn_Text(5, 140, 0, font, 1, 1, 'N', "AOI #: 1");
                        }
                        //else if (m_ProductShareVariables.productRecipeOutputSettings.BarcodeLabelSize == 0) //70x30
                        //{
                        //    m_ProductShareVariables.PrintTemporary = false;
                        //    nError = B_Prn_Text(5, 20, 0, font, 1, 1, 'N', "Part Name: " + m_ProductShareVariables.PartName);
                        //    nError += B_Prn_Text(5, 40, 0, font, 1, 1, 'N', "Part #: " + m_ProductShareVariables.PartNumber);
                        //    nError += B_Prn_Text(5, 60, 0, font, 1, 1, 'N', "Build: " + m_ProductShareVariables.BuildName);
                        //    nError += B_Prn_Text(5, 80, 0, font, 1, 1, 'N', "Quantity: " + Convert.ToString(GetShareMemoryProductionInt("nCurrentTotalUnitDone")) + " pcs");
                        //    nError += B_Prn_Text(5, 100, 0, font, 1, 1, 'N', "AOI Date: " + DateTime.Now.ToString("dd/MM/yyyy"));
                        //    nError += B_Prn_Text(5, 120, 0, font, 1, 1, 'N', "Lot #: " + m_ProductShareVariables.strucInputProductInfo.LotIDOutput);
                        //    nError += B_Prn_Text(5, 140, 0, font, 1, 1, 'N', "AOI #: 1");
                        //}
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Print Text.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        //barcode
                        //nError = B_Bar2d_DataMatrix(DataMatrixInitialXPosition, DataMatrixInitialYPosition, 40, 40, 2, 0, m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_"+ Convert.ToString(GetShareMemoryProductionInt("nCurrentTotalUnitDone")));
                        if (m_ProductShareVariables.PrintTemporary == true)
                        {
                            m_ProductShareVariables.PrintTemporary = false;
                            nError = B_Bar2d_DataMatrix(DataMatrixInitialXPosition, DataMatrixInitialYPosition, 26, 26, 3, 0, m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_" + m_ProductShareVariables.TemporaryNumber);
                        }
                        else
                        {
                            nError = B_Bar2d_DataMatrix(DataMatrixInitialXPosition, DataMatrixInitialYPosition, 26, 26, 3, 0, m_ProductShareVariables.strucInputProductInfo.LotIDOutput + "_" + Convert.ToString(GetShareMemoryProductionInt("nCurrentTotalUnitDone")));
                        }
                        //nError = B_Bar2d_DataMatrix(430, 80, 40, 40, 2, 0, m_ProductShareVariables.strucInputProductInfo.LotIDOutput);
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Print 2D Matrix.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        // output.
                        nError = B_Print_Out(1);// copy 1
                        if (nError != 0)
                        {
                            Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Print 1 Copy.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                        }
                        // close port.
                        B_ClosePrn();
                        Thread.Sleep(1);
                        if (m_ProductShareVariables.PrintOutputTrayID != null || m_ProductShareVariables.PrintOutputTrayID.Any() == true)
                        {
                            nError = B_CreatePrn(13, "192.168.2.103:9100");
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to open LAN port.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            // sample setting.
                            nError = B_Set_DebugDialog(0);
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Debug Dialog.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            nError = B_Set_Originpoint(0, 0);
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Origin Point.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            nError = B_Select_Option((int)(m_ProductShareVariables.productOptionSettings.BarcodePrinterRibbonSensor + 1));
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Settings of Printer.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            nError = B_Set_Darkness(15);
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Darkness.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            nError = B_Del_Pcx("*");// delete all picture.
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Delete Previous Image.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            //if(m_ProductShareVariables.productRecipeOutputSettings.BarcodeLabelSize == 0) //70mmx30mm
                            //{
                            //    labLength = (int)(30*0.0393701*203); //240
                            //    gapLength = (int)(1.3 * 0.0393701 * 203); //10
                            //    labWidth = (int)(70*0.0393701*203); //559
                            //    DataMatrixInitialXPosition = 430;
                            //    DataMatrixInitialYPosition = 80;
                            //    font = 2;
                            //}
                            //else if(m_ProductShareVariables.productRecipeOutputSettings.BarcodeLabelSize == 1) // 30mmX40mm, 1mm = 0.0393701 inches}
                            nError = B_Set_Labgap(labLength, gapLength); //label length & gap
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Label Length And Gap.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            nError = B_Set_Labwidth(labWidth); //label width
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Set Label Width.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            nError = B_WriteData(0, encAscII.GetBytes(sznop2), sznop2.Length);
                            nError += B_WriteData(1, encAscII.GetBytes(sznop1), sznop1.Length);
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Write Data.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            nError = B_Prn_Text(2, 20, 0, font, 1, 1, 'N', "Tray No :");
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Print Text.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            if (m_ProductShareVariables.PrintOutputTrayID != null || m_ProductShareVariables.PrintOutputTrayID.Any()==true)
                            {
                                int y = 20;
                                foreach (string strTrayID in m_ProductShareVariables.PrintOutputTrayID)
                                {
                                    nError = B_Prn_Text(113, y, 0, font, 1, 1, 'N', strTrayID);
                                    if (nError != 0)
                                    {
                                        Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Print Text.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                                    }
                                    y += 20;
                                }
                            }
                            // output.
                            nError = B_Print_Out(1);// copy 1
                            if (nError != 0)
                            {
                                Machine.DebugLogger.WriteLog(string.Format("{0} Barcode Printer Fail to Print 1 Copy.", DateTime.Now.ToString("yyyyMMdd HHmmss")));
                            }
                            // close port.
                            B_ClosePrn();
                        }
                    }
                    Thread.Sleep(1);
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
