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
    public class ProductPreviousLotInfo
    {
        public List<string> PreviouslistLotID = new List<string>();

        public int PreviousTotalQuantityDone;
        public int PreviousTotalQuantityDoneByLot;
        public int PreviousTotalInputQuantityDone;
        public int PreviousOutputQuantity;
        public int PreviousInputTrayNo;
        public int PreviousOutputTrayNo;
        public int PreviousReject1TrayNo;
        public int PreviousReject2TrayNo;
        public int PreviousReject3TrayNo;
        public int PreviousReject4TrayNo;
        public int PreviousReject5TrayNo;
        public int TotalOutputUnitQuantity;
        public int nInputLotQuantity;
        public int nLotIDNumber;
    }
}
