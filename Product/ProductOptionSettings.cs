using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product
{
    public class ProductOptionSettings : Machine.OptionSettings
    {
        public bool EnableVision;
        public bool EnableOnlineMode;
        public bool EnableLaunchVisionSoftware;
        public string VisionRecipeFolderPath;

        public bool EnablePH1 = false;
        public bool EnablePH2 = false;
        
        #region Secsgem
        public bool EnableSpoolingConfig;
        public bool EnableSpoolingOverwrite;
        public int SecsgemMode;
        public string SecsgemIPAddress;
        public int SecsgemPortNumber;
        #endregion Secsgem

        public string ServerVisPath;
        public string LocalOutputPath;
        public string VisionOutputPath;

        public bool EnableInputVision;
        public bool EnableS2Vision;
        public bool EnableSetupVision;
        public bool EnableBottomVision;
        public bool EnableSWLRVision;
        public bool EnableSWFRVision;
        public bool EnableS3Vision;
        public bool EnableOutputVision;

        public uint InputVisionFOVWidth = 10240;
        public uint InputVisionFOVHeight = 10240;
        public double UnitOfMeasurement = 2.242896746005401;
        public bool EnableGenerateOutputFilesInVisionPC = false;

        public bool EnablePickupHeadRetryPickingNo;
        public uint PickupHeadRetryPickingNo;
        public uint PulseWidth = 0;
        public int[] PickUpHeadCompensationXOffset = new int[2] { 0, 0};
        public int[] PickUpHeadCompensationYOffset = new int[2] { 0, 0};

        public int[] PickUpHeadHeadCompensationXOffset = new int[2] { 0, 0};
        public int[] PickUpHeadHeadCompensationYOffset = new int[2] { 0, 0};

        public int[] PickUpHeadRotationXOffset = new int[2] { 0, 0};
        public int[] PickUpHeadRotationYOffset = new int[2] { 0, 0};

        public int[] PickUpHeadOutputCompensationXOffset = new int[2] { 0, 0};

        public int[] PickUpHeadOutputCompensationYOffset = new int[2] { 0, 0 };
        public int[] PickUpHeadOutputCompensationThetaOffset = new int[2] { 0, 0 };

        public bool bEnableCheckingBarcodeID = false;

        public bool bEnableBarcodePrinter = false;

        public int TrayPresentSensorOffTimeBeforeAlarm_ms = 0;

        //public int NoOfCycleToPerformPickupHeadOffsetChecking = 0;
        //public int NoOfCycleToPerformTrayTableOffsetChecking = 0;
        //public int NoOfCycleToPerformAutoHoming = 0;

        public bool EnableVisionWaitResult = false;

        public bool EnablePickupHeadRetryPlacingNo;
        public uint PickupHeadRetryPlacingNo;

        public uint BarcodePrinterRibbonSensor = 0;

        //public bool EnableGoodSamplingSequence = false;

        public bool EnableInputTableVacuum = false;

        public bool EnableCountDownByInputQuantity = false;
        public bool EnableCountDownByInputTrayNo = false;
        #region MES
        public bool EnableMES = false;
        public string MESInputURL= "http://192.168.1.200:8923/mesapi/ProdGetInput/HD4_H401_";
        public string MESOutputURL= "http://192.168.1.200:8923/mesapi/ProdOutput";
        public string MESEndJobURL= "http://192.168.1.200:8923/mesapi/EndJob";
        public string MachineNo;
        #endregion MES
    }
}
