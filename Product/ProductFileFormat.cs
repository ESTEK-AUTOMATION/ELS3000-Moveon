using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Forms;

namespace Product
{
    public class ProductFrameOrTileDefect
    {
        public string InputLotID;
        public int InputTrayNo;
        public int OutputTrayNo;

        public List<DefectQuantity> listDefectQuantity;
        public int InputQuantity;
        public int OutputQuantity;
        public int UnitRemainOnFrame;
        public int InputOtherDefectsQuantity;
        public int UnitUntest;
        public int UnitLoss;
        public int RejectTray1Quantity;
        public int RejectTray2Quantity;
        public int RejectTray3Quantity;
        public int RejectTray4Quantity;
        public int RejectTray5Quantity;
        public int PassQuantity;
        public int FullTestQuantity;
        public int InputEmptyQuantity;

        public int InputRejectQuantity;
        public int S2RejectQuantity;
        public int SetupRejectQuantity;
        public int S1RejectQuantity;
        public int SidewallLeftRejectQuantity;
        public int SidewallRightRejectQuantity;
        public int S3RejectQuantity;
        public int SidewallFrontRejectQuantity;
        public int SidewallRearRejectQuantity;
        public int OutputRejectQuantity;

        public int OutputMissingQuantity;
        public int RejectMissingQuantity;
    }

    public class ProductSummary
    {
        public string LotID;
        public string StartDateTime;
        public string Recipe;
        public string OperatorID;
        public string Shift;
        public string MachineID;
        public string Version;
        public string WorkOrder;
        public string EndDateTime;
        public string Theoretical_UPH;

    }

    public class ProductInputOutputFileFormat
    {
        public BinInfo mapdata = new BinInfo();

        private ProductShareVariables m_ProductShareVariables;

        public ProductShareVariables productShareVariables
        {
            set
            {
                m_ProductShareVariables = value;
            }
        }

        public string m_strmode = "Input Output File";

        public int InitializeMapArrayData(ref BinInfo mappingInfo, int Row, int Column, int ncolumnDirection, int nrowDirection)
        {
            int nError = 0;

            try
            {
                int nRow = 1;
                int nCol = 1;
                string strBinCode = "UTT";
                int nArrayNumber = 0;
                int nCellNumber = 1;
                //if ((nrowDirection == 1 || nrowDirection == 3))
                //{
                mappingInfo.Row_Max = Row;
                mappingInfo.Col_Max = Column;
                mappingInfo.dicUnitIndex = new Dictionary<string, int>();
                mappingInfo.dicUnitIndex.Clear();
                for (int i = 0; i < mappingInfo.Col_Max * mappingInfo.Row_Max; i++)
                {
                    mappingInfo.dicUnitIndex.Add($"{nRow},{nCol}", nArrayNumber);
                    mappingInfo.arrayUnitInfo[nArrayNumber].InputRow = nRow;
                    mappingInfo.arrayUnitInfo[nArrayNumber].InputColumn = nCol;
                    mappingInfo.arrayUnitInfo[nArrayNumber].BinCode = strBinCode;
                    mappingInfo.arrayUnitInfo[nArrayNumber].CellNumber = nCellNumber;
                    nCol++;
                    nCellNumber++;
                    if (nCol > mappingInfo.Col_Max)
                    {
                        nRow++;      //Next Row
                        nCol = 1;                //Next Row, Column Start From 1
                    }
                    nArrayNumber++;
                }

                return nError;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
        }

        public int UpdateInputMapArrayData(ref BinInfo mappingInfo, int Row, int Column, string BinCode, bool overWriteDefectCode, string destination)
        {
            int nError = 0;
            try
            {
                lock (mappingInfo.arrayUnitInfo)
                {
                    int Array_Index = 0;
                    for (int i = 0; i < mappingInfo.arrayUnitInfo.Length; i++)
                    {
                        if (mappingInfo.arrayUnitInfo[i].InputRow == Row && mappingInfo.arrayUnitInfo[i].InputColumn == Column)
                        {
                            Array_Index = i;
                            break;
                        }
                    }
                    if (overWriteDefectCode == true)
                    {
                        mappingInfo.arrayUnitInfo[Array_Index].BinCode = BinCode;
                        mappingInfo.arrayUnitInfo[Array_Index].Destination = destination;
                    }
                    else
                    {
                        if (mappingInfo.arrayUnitInfo[Array_Index].BinCode == "UTT" || mappingInfo.arrayUnitInfo[Array_Index].BinCode == "P")
                        {
                            mappingInfo.arrayUnitInfo[Array_Index].BinCode = BinCode;
                            mappingInfo.arrayUnitInfo[Array_Index].Destination = destination;
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

        public int UpdateInputMapArrayData(ref BinInfo mappingInfo, int Row, int Column, string BinCode, bool overWriteDefectCode)
        {
            int nError = 0;
            try
            {
                lock (mappingInfo.arrayUnitInfo)
                {
                    int Array_Index = 0;
                    for (int i = 0; i < mappingInfo.arrayUnitInfo.Length; i++)
                    {
                        if (mappingInfo.arrayUnitInfo[i].InputRow == Row && mappingInfo.arrayUnitInfo[i].InputColumn == Column)
                        {
                            Array_Index = i;
                            break;
                        }
                    }
                    if (overWriteDefectCode == true)
                    {
                        mappingInfo.arrayUnitInfo[Array_Index].BinCode = BinCode;
                    }
                    else
                    {
                        if (mappingInfo.arrayUnitInfo[Array_Index].BinCode == "UTT" || mappingInfo.arrayUnitInfo[Array_Index].BinCode == "P" || mappingInfo.arrayUnitInfo[Array_Index].BinCode == "---" 
                            || mappingInfo.arrayUnitInfo[Array_Index].BinCode == "PROG")
                        {
                            mappingInfo.arrayUnitInfo[Array_Index].BinCode = BinCode;
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

        public int UpdateOutputMapArrayData(ref BinInfo mappingInfo, int Row, int Column, string BinCode, bool overWriteDefectCode)
        {
            int nError = 0;
            try
            {
                lock (mappingInfo.arrayUnitInfo)
                {
                    int Array_Index = 0;
                    for (int i = 0; i < mappingInfo.arrayUnitInfo.Length; i++)
                    {
                        if (mappingInfo.arrayUnitInfo[i].OutputRow == Row && mappingInfo.arrayUnitInfo[i].OutputColumn == Column)
                        {
                            Array_Index = i;
                            break;
                        }
                    }
                    if (overWriteDefectCode == true)
                    {
                        mappingInfo.arrayUnitInfo[Array_Index].BinCode = BinCode;
                    }
                    else
                    {
                        if (mappingInfo.arrayUnitInfo[Array_Index].BinCode == "UTT")
                            mappingInfo.arrayUnitInfo[Array_Index].BinCode = BinCode;
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

        public int GenerateInputMapFile(List<DefectProperty> listDefect, BinInfo mappingInfo, string folder, string filename, int First_Pos, string EmptyCode)
        {
            int nError = 0;
            int nCol = mappingInfo.Col_Max;
            int nRow = mappingInfo.Row_Max;
            try
            {
                if (Directory.Exists(folder) == false)
                    Directory.CreateDirectory(folder);

                //Top left File Generate
                int nMaxLengthOfDefectCode = 0;
                foreach (DefectProperty _defectProperty in listDefect)
                {
                    if (_defectProperty.Code.Length > nMaxLengthOfDefectCode)
                    {
                        nMaxLengthOfDefectCode = _defectProperty.Code.Length;
                    }
                }
                if (First_Pos == 0)
                {
                    string s = "";
                    //nCol = outputCol;
                    //nRow = outputRow;
                    int nColQty = nCol;
                    //1st Row Data generate
                    for (int i = 1; i <= nCol; i++)
                    {
                        s = s + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                    }
                    s = s + Environment.NewLine;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";

                    string strFileGenerateBincode;
                    for (int i = 1; i <= nRow; i++)
                    {
                        for (int j = 1; j <= nCol; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].InputRow == i &&
                                    mappingInfo.arrayUnitInfo[K].InputColumn == j)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;
                        using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                        {
                            Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBincodeData);
                            fs.Write(TopRightFile, 0, TopRightFile.Length);
                            strFileGenerateBincodeData = "";
                        }
                    }
                }
                //Top Rigth File Generate
                if (First_Pos == 1)
                {
                    string s = "";
                    //int nCol = mappingInfo.Col_Max;
                    //int nRow = mappingInfo.Row_Max;
                    int nColQty = nCol;
                    //1st Row Data generate
                    for (int i = 1; i <= nCol; i++)
                    {
                        s = s + nColQty.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                        nColQty = nColQty - 1;
                    }
                    s = s + Environment.NewLine;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    //int nFileGenerateArrayNum = 0;
                    string strFileGenerateBincode;
                    for (int i = 1; i <= nRow; i++)
                    {
                        for (int j = 1; j <= nCol; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].InputRow == i &&
                                    mappingInfo.arrayUnitInfo[K].InputColumn == j)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                        {
                            Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBincodeData);
                            fs.Write(TopRightFile, 0, TopRightFile.Length);
                            strFileGenerateBincodeData = "";
                        }
                    }
                }
                //Bottom left file Generate
                if (First_Pos == 2)
                {
                    string s = "";
                    //int nCol = mappingInfo.Col_Max;
                    //int nRow = mappingInfo.Row_Max;
                    int nColQty = nCol;
                    int nFileGenerateBottomLeftArrayNumber = 0;
                    string[] FileGenerateBottomLeft = new string[nRow];
                    //1st Row Data generate
                    for (int i = 1; i <= nCol; i++)
                    {
                        s = s + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                    }
                    s = s + Environment.NewLine;
                    FileGenerateBottomLeft[nFileGenerateBottomLeftArrayNumber] = s;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    string strFileGenerateBincode;
                    nFileGenerateBottomLeftArrayNumber = nRow - 1;
                    for (int i = 1; i <= nRow; i++)
                    {
                        for (int j = 1; j <= nCol; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].InputRow == i &&
                                    mappingInfo.arrayUnitInfo[K].InputColumn == j)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        //Addd the Data to Last of tht string array
                        FileGenerateBottomLeft[nFileGenerateBottomLeftArrayNumber] = strFileGenerateBincodeData;
                        nFileGenerateBottomLeftArrayNumber = nFileGenerateBottomLeftArrayNumber - 1;
                        strFileGenerateBincodeData = "";
                    }
                    string strFileGenerateBottomLeftData = ConvertStringArrayToString(FileGenerateBottomLeft);
                    using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                    {
                        Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBottomLeftData);
                        fs.Write(TopRightFile, 0, TopRightFile.Length);
                    }
                }
                if (First_Pos == 3)
                {
                    string s = "";
                    //int nCol = mappingInfo.Col_Max;
                    //int nRow = mappingInfo.Row_Max;
                    int nColQty = nCol;
                    int nFileGenerateBottomRightArrayNumber = 0;
                    string[] strArrFileGenerateBottomRight = new string[nRow];
                    //1st Row Data generate
                    for (int i = 1; i <= nCol; i++)
                    {
                        s = s + nColQty.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                        nColQty = nColQty - 1;
                    }
                    s = s + Environment.NewLine;
                    strArrFileGenerateBottomRight[nFileGenerateBottomRightArrayNumber] = s;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    //int nFileGenerateArrayNum = 0;
                    string strFileGenerateBincode;
                    nFileGenerateBottomRightArrayNumber = nRow - 1;
                    for (int i = 1; i <= nRow; i++)
                    {
                        for (int j = 1; j <= nCol; j++)
                        {
                            bool isThisEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].InputRow == i &&
                                    mappingInfo.arrayUnitInfo[K].InputColumn == j)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                        isThisEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isThisEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        //Addd the Data to Last of tht string array
                        strArrFileGenerateBottomRight[nFileGenerateBottomRightArrayNumber] = strFileGenerateBincodeData;
                        nFileGenerateBottomRightArrayNumber = nFileGenerateBottomRightArrayNumber - 1;
                        strFileGenerateBincodeData = "";
                    }
                    string strFileGenerateBottomLeftData = ConvertStringArrayToString(strArrFileGenerateBottomRight);
                    using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                    {
                        Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBottomLeftData);
                        fs.Write(TopRightFile, 0, TopRightFile.Length);
                    }
                }
                if (First_Pos == 4)
                {
                    string s = "";
                    int nRowQty = nRow;
                    //1st Row Data generate
                    for (int i = 1; i <= nRow; i++)
                    {
                        s = s + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                    }
                    s = s + "R" + Environment.NewLine;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    string strFileGenerateBincode;
                    for (int i = 1; i <= nCol; i++)
                    {
                        for (int j = 1; j <= nRow; j++)
                        {

                            bool isEmpty = true;
                            for (int K = 0; K < nCol * nRow; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].InputRow == j &&
                                    mappingInfo.arrayUnitInfo[K].InputColumn == i)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;
                        using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                        {
                            Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBincodeData);
                            fs.Write(TopRightFile, 0, TopRightFile.Length);
                            strFileGenerateBincodeData = "";
                        }
                    }
                }
                if (First_Pos == 5)
                {
                    string s = "";
                    int nRowQty = nRow;
                    //1st Row Data generate
                    for (int i = 1; i <= nRow; i++)
                    {
                        s = s + nRowQty.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                        nRowQty = nRowQty - 1;
                    }
                    s = s + "R" + Environment.NewLine;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    string strFileGenerateBincode;
                    for (int i = 1; i <= nCol; i++)
                    {
                        for (int j = 1; j <= nRow; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < nCol * nRow; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].InputRow == j &&
                                    mappingInfo.arrayUnitInfo[K].InputColumn == i)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;
                        using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                        {
                            Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBincodeData);
                            fs.Write(TopRightFile, 0, TopRightFile.Length);
                            strFileGenerateBincodeData = "";
                        }
                    }
                }
                if (First_Pos == 6)
                {
                    string s = "";
                    int nRowQty = nRow;
                    int nFileGenerateBottomLeftArrayNumber = 0;
                    string[] FileGenerateBottomLeft = new string[nRow];
                    //1st Row Data generate
                    for (int i = 1; i <= nRow; i++)
                    {
                        s = s + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                    }
                    s = s + "R" + Environment.NewLine;
                    FileGenerateBottomLeft[nFileGenerateBottomLeftArrayNumber] = s;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    string strFileGenerateBincode;
                    nFileGenerateBottomLeftArrayNumber = nRow - 1;
                    for (int i = 1; i <= nCol; i++)
                    {
                        for (int j = 1; j <= nRow; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < nCol * nRow; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].InputRow == j &&
                                    mappingInfo.arrayUnitInfo[K].InputColumn == i)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        //Addd the Data to Last of tht string array
                        FileGenerateBottomLeft[nFileGenerateBottomLeftArrayNumber] = strFileGenerateBincodeData;
                        nFileGenerateBottomLeftArrayNumber = nFileGenerateBottomLeftArrayNumber - 1;
                        strFileGenerateBincodeData = "";
                    }
                    string strFileGenerateBottomLeftData = ConvertStringArrayToString(FileGenerateBottomLeft);
                    using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                    {
                        Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBottomLeftData);
                        fs.Write(TopRightFile, 0, TopRightFile.Length);
                    }
                }
                if (First_Pos == 7)
                {
                    string s = "";
                    int nRowQty = nRow;
                    int nFileGenerateBottomRightArrayNumber = 0;
                    string[] strArrFileGenerateBottomRight = new string[nRow];
                    //1st Row Data generate
                    for (int i = 1; i <= nRow; i++)
                    {
                        s = s + nRowQty.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                        nRowQty = nRowQty - 1;
                    }
                    s = s + "C" + Environment.NewLine;
                    strArrFileGenerateBottomRight[nFileGenerateBottomRightArrayNumber] = s;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    string strFileGenerateBincode;
                    nFileGenerateBottomRightArrayNumber = nRow - 1;

                    for (int i = 1; i <= nCol; i++)
                    {
                        for (int j = 1; j <= nRow; j++)
                        {
                            bool isThisEmpty = true;
                            for (int K = 0; K < nCol * nRow; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].InputRow == j &&
                                    mappingInfo.arrayUnitInfo[K].InputColumn == i)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                        else if ((mappingInfo.arrayUnitInfo[K].BinCode).Length > 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                        else if ((mappingInfo.arrayUnitInfo[K].BinCode) == "")
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                        isThisEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isThisEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        //Addd the Data to Last of tht string array
                        strArrFileGenerateBottomRight[nFileGenerateBottomRightArrayNumber] = strFileGenerateBincodeData;
                        nFileGenerateBottomRightArrayNumber = nFileGenerateBottomRightArrayNumber - 1;
                        strFileGenerateBincodeData = "";
                    }
                    string strFileGenerateBottomLeftData = ConvertStringArrayToString(strArrFileGenerateBottomRight);
                    using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                    {
                        Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBottomLeftData);
                        fs.Write(TopRightFile, 0, TopRightFile.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }
            return nError;
        }

        public int GenerateOutputMapFile(List<DefectProperty> listDefect, BinInfo mappingInfo, int outputRow, int outputCol, string folder, string filename, int First_Pos, string EmptyCode)
        {
            int nError = 0;
            int nCol = outputCol;
            int nRow = outputRow;
            try
            {
                if (Directory.Exists(folder) == false)
                    Directory.CreateDirectory(folder);

                int nMaxLengthOfDefectCode = 0;
                foreach (DefectProperty _defectProperty in listDefect)
                {
                    if (_defectProperty.Code.Length > nMaxLengthOfDefectCode)
                    {
                        nMaxLengthOfDefectCode = _defectProperty.Code.Length;
                    }
                }
                //Top left File Generate
                if (First_Pos == 0)
                {
                    string s = "";
                    //nCol = outputCol;
                    //nRow = outputRow;
                    int nColQty = nCol;
                    //1st Row Data generate
                    for (int i = 1; i <= nCol; i++)
                    {
                        s = s + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                    }
                    s = s + Environment.NewLine;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";

                    string strFileGenerateBincode;
                    for (int i = 1; i <= nRow; i++)
                    {
                        for (int j = 1; j <= nCol; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].OutputRow == i &&
                                    mappingInfo.arrayUnitInfo[K].OutputColumn == j)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                        {
                            Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBincodeData);
                            fs.Write(TopRightFile, 0, TopRightFile.Length);
                            strFileGenerateBincodeData = "";
                        }
                    }
                }
                //Top Rigth File Generate
                if (First_Pos == 1)
                {
                    string s = "";
                    //int nCol = mappingInfo.Col_Max;
                    //int nRow = mappingInfo.Row_Max;
                    int nColQty = nCol;
                    //1st Row Data generate
                    for (int i = 1; i <= nCol; i++)
                    {
                        s = s + nColQty.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                        nColQty = nColQty - 1;
                    }
                    s = s + Environment.NewLine;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    string strFileGenerateBincode;
                    for (int i = 1; i <= nRow; i++)
                    {
                        for (int j = 1; j <= nCol; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].OutputRow == i &&
                                    mappingInfo.arrayUnitInfo[K].OutputColumn == j)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                        isEmpty = false;
                                        break;
                                    }
                                }

                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                        {
                            Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBincodeData);
                            fs.Write(TopRightFile, 0, TopRightFile.Length);
                            strFileGenerateBincodeData = "";
                        }
                    }
                }
                //Bottom left file Generate
                if (First_Pos == 2)
                {
                    string s = "";
                    int nColQty = nCol;
                    int nFileGenerateBottomLeftArrayNumber = 0;
                    string[] FileGenerateBottomLeft = new string[nRow];
                    //1st Row Data generate
                    for (int i = 1; i <= nCol; i++)
                    {
                        s = s + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                    }
                    s = s + Environment.NewLine;
                    FileGenerateBottomLeft[nFileGenerateBottomLeftArrayNumber] = s;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";

                    string strFileGenerateBincode;
                    nFileGenerateBottomLeftArrayNumber = nRow - 1;
                    for (int i = 1; i <= nRow; i++)
                    {
                        for (int j = 1; j <= nCol; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].OutputRow == i &&
                                    mappingInfo.arrayUnitInfo[K].OutputColumn == j)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        //Addd the Data to Last of tht string array
                        FileGenerateBottomLeft[nFileGenerateBottomLeftArrayNumber] = strFileGenerateBincodeData;
                        nFileGenerateBottomLeftArrayNumber = nFileGenerateBottomLeftArrayNumber - 1;
                        strFileGenerateBincodeData = "";
                    }
                    string strFileGenerateBottomLeftData = ConvertStringArrayToString(FileGenerateBottomLeft);
                    using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                    {
                        Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBottomLeftData);
                        fs.Write(TopRightFile, 0, TopRightFile.Length);
                    }
                }
                if (First_Pos == 3)
                {
                    string s = "";
                    //int nCol = mappingInfo.Col_Max;
                    //int nRow = mappingInfo.Row_Max;
                    int nColQty = nCol;
                    int nFileGenerateBottomRightArrayNumber = 0;
                    string[] strArrFileGenerateBottomRight = new string[nRow];
                    //1st Row Data generate
                    for (int i = 1; i <= nCol; i++)
                    {
                        s = s + nColQty.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                        nColQty = nColQty - 1;
                    }
                    s = s + Environment.NewLine;
                    strArrFileGenerateBottomRight[nFileGenerateBottomRightArrayNumber] = s;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    //int nFileGenerateArrayNum = 0;
                    string strFileGenerateBincode;
                    nFileGenerateBottomRightArrayNumber = nRow - 1;
                    for (int i = 1; i <= nRow; i++)
                    {
                        for (int j = 1; j <= nCol; j++)
                        {
                            bool isThisEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].OutputRow == i &&
                                    mappingInfo.arrayUnitInfo[K].OutputColumn == j)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                        isThisEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isThisEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        //Addd the Data to Last of tht string array
                        strArrFileGenerateBottomRight[nFileGenerateBottomRightArrayNumber] = strFileGenerateBincodeData;
                        nFileGenerateBottomRightArrayNumber = nFileGenerateBottomRightArrayNumber - 1;
                        strFileGenerateBincodeData = "";
                    }
                    string strFileGenerateBottomLeftData = ConvertStringArrayToString(strArrFileGenerateBottomRight);
                    using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                    {
                        Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBottomLeftData);
                        fs.Write(TopRightFile, 0, TopRightFile.Length);
                    }
                }
                if (First_Pos == 4)
                {
                    string s = "";
                    int nRowQty = nRow;
                    //1st Row Data generate
                    for (int i = 1; i <= nRow; i++)
                    {
                        s = s + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                    }
                    s = s + "R" + Environment.NewLine;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    string strFileGenerateBincode;
                    for (int i = 1; i <= nCol; i++)
                    {
                        for (int j = 1; j <= nRow; j++)
                        {

                            bool isEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].OutputRow == j &&
                                    mappingInfo.arrayUnitInfo[K].OutputColumn == i)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;
                        using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                        {
                            Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBincodeData);
                            fs.Write(TopRightFile, 0, TopRightFile.Length);
                            strFileGenerateBincodeData = "";
                        }
                    }
                }
                if (First_Pos == 5)
                {
                    string s = "";
                    int nRowQty = nRow;
                    //1st Row Data generate
                    for (int i = 1; i <= nRow; i++)
                    {
                        s = s + nRowQty.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                        nRowQty = nRowQty - 1;
                    }
                    s = s + "R" + Environment.NewLine;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    //int nFileGenerateArrayNum = 0;
                    string strFileGenerateBincode;
                    for (int i = 1; i <= nCol; i++)
                    {
                        for (int j = 1; j <= nRow; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].OutputRow == j &&
                                    mappingInfo.arrayUnitInfo[K].OutputColumn == i)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;
                        using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                        {
                            Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBincodeData);
                            fs.Write(TopRightFile, 0, TopRightFile.Length);
                            strFileGenerateBincodeData = "";
                        }
                    }
                }
                if (First_Pos == 6)
                {
                    string s = "";
                    int nRowQty = nRow;
                    int nFileGenerateBottomLeftArrayNumber = 0;
                    //string[] FileGenerateBottomLeft = new string[nRow];
                    string[] FileGenerateBottomLeft = new string[nCol];
                    //1st Row Data generate
                    for (int i = 1; i <= nRow; i++)
                    {
                        s = s + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                    }
                    s = s + "R" + Environment.NewLine;
                    FileGenerateBottomLeft[nFileGenerateBottomLeftArrayNumber] = s;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    string strFileGenerateBincode;
                    nFileGenerateBottomLeftArrayNumber = nCol - 1;

                    for (int i = 1; i <= nCol; i++)
                    {
                        for (int j = 1; j <= nRow; j++)
                        {
                            bool isEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].OutputRow == j &&
                                    mappingInfo.arrayUnitInfo[K].OutputColumn == i)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                        else
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode);
                                            isEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                                        isEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData + string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode);
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        //Add the Data to Last of tht string array
                        FileGenerateBottomLeft[nFileGenerateBottomLeftArrayNumber] = strFileGenerateBincodeData;
                        nFileGenerateBottomLeftArrayNumber = nFileGenerateBottomLeftArrayNumber - 1;
                        strFileGenerateBincodeData = "";
                    }
                    string strFileGenerateBottomLeftData = ConvertStringArrayToString(FileGenerateBottomLeft);
                    using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                    {
                        Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBottomLeftData);
                        fs.Write(TopRightFile, 0, TopRightFile.Length);
                    }
                }
                if (First_Pos == 7)
                {
                    string s = "";
                    int nRowQty = nRow;
                    int nFileGenerateBottomRightArrayNumber = 0;
                    string[] strArrFileGenerateBottomRight = new string[nCol];
                    //1st Row Data generate
                    for (int i = 1; i <= nRow; i++)
                    {
                        s = s + nRowQty.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + " ";
                        nRowQty = nRowQty - 1;
                    }
                    s = s + "C" + Environment.NewLine;
                    strArrFileGenerateBottomRight[nFileGenerateBottomRightArrayNumber] = s;
                    using (FileStream fs = File.Create(folder + "\\" + filename))
                    {

                        Byte[] info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    //Data from Array.
                    //Initialize the array
                    string strFileGenerateBincodeData = "";
                    string strFileGenerateBincode;
                    nFileGenerateBottomRightArrayNumber = nCol - 1;
                    for (int i = 1; i <= nCol; i++)
                    {
                        for (int j = 1; j <= nRow; j++)
                        {
                            bool isThisEmpty = true;
                            for (int K = 0; K < mappingInfo.Col_Max * mappingInfo.Row_Max; K++)
                            {
                                strFileGenerateBincode = mappingInfo.arrayUnitInfo[K].BinCode;

                                if (mappingInfo.arrayUnitInfo[K].OutputRow == j &&
                                    mappingInfo.arrayUnitInfo[K].OutputColumn == i)
                                {
                                    if (mappingInfo.arrayUnitInfo[K].BinCode != null)
                                    {
                                        if ((mappingInfo.arrayUnitInfo[K].BinCode).Length == 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                        else if ((mappingInfo.arrayUnitInfo[K].BinCode).Length > 1)
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", mappingInfo.arrayUnitInfo[K].BinCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                        else if ((mappingInfo.arrayUnitInfo[K].BinCode) == "")
                                        {
                                            strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                            isThisEmpty = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                        isThisEmpty = false;
                                        break;
                                    }
                                }
                            }
                            if (isThisEmpty == true)
                            {
                                strFileGenerateBincodeData = strFileGenerateBincodeData.Insert(0, string.Format($"{{0,{-nMaxLengthOfDefectCode}}} ", EmptyCode));
                                break;
                            }
                        }
                        //After 1 Col add Col Number at back of data
                        strFileGenerateBincodeData = strFileGenerateBincodeData + i.ToString().PadLeft(nMaxLengthOfDefectCode, '0') + Environment.NewLine;

                        //Add the Data to Last of tht string array
                        strArrFileGenerateBottomRight[nFileGenerateBottomRightArrayNumber] = strFileGenerateBincodeData;
                        nFileGenerateBottomRightArrayNumber = nFileGenerateBottomRightArrayNumber - 1;
                        strFileGenerateBincodeData = "";
                    }
                    string strFileGenerateBottomLeftData = ConvertStringArrayToString(strArrFileGenerateBottomRight);
                    using (var fs = new FileStream(folder + "\\" + filename, FileMode.Append))
                    {
                        Byte[] TopRightFile = new UTF8Encoding(true).GetBytes(strFileGenerateBottomLeftData);
                        fs.Write(TopRightFile, 0, TopRightFile.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
                return -1;
            }

            return nError;
        }


        public int GenerateMappingInfoForOutput(int trayType, ref BinInfo mappingInfo, ref Dictionary<string, Product.UnitInfo> dicMappingInfo, int MapColDirection, int MapRowDirection, int row, int col)
        {
            int nError = 0;
            int nRow = 1;
            int nCol = 1;
            int nArrayNumber = 0;
            try
            {
                if (trayType == 2)
                {
                    mappingInfo.arrayUnitInfo = new Product.UnitInfo[row * col];

                    mappingInfo.Row_Max = (int)row;
                    mappingInfo.Col_Max = (int)col;

                    double digit = Math.Floor(Math.Log10(mappingInfo.Row_Max) + 1) >= Math.Floor(Math.Log10(mappingInfo.Col_Max) + 1) ? Math.Floor(Math.Log10(mappingInfo.Row_Max) + 1) : Math.Floor(Math.Log10(mappingInfo.Col_Max) + 1);
                    dicMappingInfo = new Dictionary<string, UnitInfo>();
                    for (int i = 0; i < mappingInfo.Col_Max * mappingInfo.Row_Max; i++)
                    {
                        mappingInfo.arrayUnitInfo[nArrayNumber].OutputRow = nRow;
                        mappingInfo.arrayUnitInfo[nArrayNumber].OutputColumn = nCol;
                        mappingInfo.arrayUnitInfo[nArrayNumber].CellNumber = i;
                        dicMappingInfo.Add(string.Format("{0:D" + digit + "}X{1:D" + digit + "}", mappingInfo.arrayUnitInfo[i].OutputRow, mappingInfo.arrayUnitInfo[i].OutputColumn), mappingInfo.arrayUnitInfo[i]);
                        //mappingInfo.arrayUnitInfo[nArrayNumber].First_Pos = FirstPosition;
                        nRow++;
                        if (nRow > mappingInfo.Row_Max)
                        {
                            nCol++;      //Next Row
                            nRow = 1;                //Next Row, Column Start From 1
                        }
                        nArrayNumber++;

                    }
                }
                else
                {
                    mappingInfo.arrayUnitInfo = new Product.UnitInfo[row * col];

                    mappingInfo.Row_Max = (int)row;
                    mappingInfo.Col_Max = (int)col;

                    double digit = Math.Floor(Math.Log10(mappingInfo.Row_Max) + 1) >= Math.Floor(Math.Log10(mappingInfo.Col_Max) + 1) ? Math.Floor(Math.Log10(mappingInfo.Row_Max) + 1) : Math.Floor(Math.Log10(mappingInfo.Col_Max) + 1);
                    dicMappingInfo = new Dictionary<string, UnitInfo>();
                    for (int i = 0; i < mappingInfo.Col_Max * mappingInfo.Row_Max; i++)
                    {
                        mappingInfo.arrayUnitInfo[nArrayNumber].OutputRow = nRow;
                        mappingInfo.arrayUnitInfo[nArrayNumber].OutputColumn = nCol;
                        mappingInfo.arrayUnitInfo[nArrayNumber].CellNumber = i;
                        dicMappingInfo.Add(string.Format("{0:D" + digit + "}X{1:D" + digit + "}", mappingInfo.arrayUnitInfo[i].OutputRow, mappingInfo.arrayUnitInfo[i].OutputColumn), mappingInfo.arrayUnitInfo[i]);
                        //mappingInfo.arrayUnitInfo[nArrayNumber].First_Pos = FirstPosition;
                        nCol++;
                        if (nCol > mappingInfo.Col_Max)
                        {
                            nRow++;      //Next Row
                            nCol = 1;                //Next Row, Column Start From 1
                        }
                        nArrayNumber++;

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

        string ConvertStringArrayToString(string[] array)
        {
            //
            // Concatenate all the elements into a StringBuilder.
            //
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                //builder.Append('.');
            }
            return builder.ToString();
        }


        public int GeneratePNPCalibrationReport(List<LookUpTableOffsetData>LUT, string folder, string filename)
        {
            int nError = 0;
            string strSeperator = ",";
            StringBuilder sbOutput = new StringBuilder();
            try
            {
                if (Directory.Exists(folder) == false)
                    Directory.CreateDirectory(folder);
                if (File.Exists(folder + "\\" + filename))
                {
                }
                string[] Header1 = new string[] { "Angle", "XOffset", "YOffset" };
                sbOutput.AppendLine(string.Join(strSeperator, Header1));

                foreach (LookUpTableOffsetData info in LUT)
                {
                    string[] InfoLine = new string[] { $"{info.Angle.ToString()}", $"{info.XOffset.ToString()}", $"{info.YOffset.ToString()}" };
                    sbOutput.AppendLine(string.Join(strSeperator, InfoLine));
                }

                File.AppendAllText((folder + "\\" + filename), sbOutput.ToString());
                return 0;
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
                return -1;
            }
            finally
            {
                sbOutput = null;
            }
        }
    }
}
