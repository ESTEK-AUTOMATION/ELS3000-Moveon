using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductContinueLotInfo
    {
        public List<LotDetail> PreviousLotInfo = new List<LotDetail>();
    }
    public class LotDetail
    {
        public string LotID { get; set; }
        public int InputBalance { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int InputTrayNo { get; set; }
        public int nCurrentInputTrayNo { get; set; }
    }
}
