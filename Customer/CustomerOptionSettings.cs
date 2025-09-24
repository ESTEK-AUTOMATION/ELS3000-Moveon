using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer
{
    public class CustomerOptionSettings : Product.ProductOptionSettings
    {
        //public Product.ProductOptionSettings productOptionSettings = new Product.ProductOptionSettings();

        public string InvisFileFolderName;
        public string InbluFileFolderName;
        //public string ServerVisPath;
        public string OutbluFileFolderName;
        public string ServerPPLotPath;
        //public string LocalOutputPath;
        //public string VisionOutputPath;
        public string strInputMachineProcessCode;
        public string strCurrentMachineProcessCode;

        //public bool EnableGenerateOutputFilesInVisionPC = false;

        //public bool ClearMemory = false;
        //public int PercentageMemoryToStartClearMemory = 50;

    }
}
