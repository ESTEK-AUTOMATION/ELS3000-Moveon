using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductRecipeSortingSetting
    {
        public bool EnableSortingMode;
        public int SortingMode;
        public string SortingInputFilePath;
        public List<Sorting> lstSortings = new List<Sorting>();
       
    }

    public class InfoSorting
    {
        public List<listInfo> lstInfoSortings = new List<listInfo>();
    }
    public class listInfo
    {
        public int StartNo;
        public int EndNo;
        public string words;
        public int SelectType;
        public string RangeStart;
        public string RangeEnd;
    }

    public class Sorting
    {
        public string strSortingBarcode;
        public int intBarcodeLength;
        public List<SortData> SortDataCorrect;
    }

    public class SortData
    {
        public string strName;
        public int intSelectType;
        public int intWordStartNo;
        public int intWordEndNo;
        public string strRangeStart;
        public string strRangeEnd;
    }
}
